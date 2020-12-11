/************************************************************************
* Copyright (c) 2005-2010,  Microchip Technology Inc.
*
* Microchip licenses this software to you solely for use with Microchip
* products.  The software is owned by Microchip and its licensors, and
* is protected under applicable copyright laws.  All rights reserved.
*
* SOFTWARE IS PROVIDED "AS IS."  MICROCHIP EXPRESSLY DISCLAIMS ANY
* WARRANTY OF ANY KIND, WHETHER EXPRESS OR IMPLIED, INCLUDING BUT
* NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS
* FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT.  IN NO EVENT SHALL
* MICROCHIP BE LIABLE FOR ANY INCIDENTAL, SPECIAL, INDIRECT OR
* CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA, HARM TO YOUR
* EQUIPMENT, COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY
* OR SERVICES, ANY CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT LIMITED
* TO ANY DEFENSE THEREOF), ANY CLAIMS FOR INDEMNITY OR CONTRIBUTION,
* OR OTHER SIMILAR COSTS.
*
* To the fullest extent allowed by law, Microchip and its licensors
* liability shall not exceed the amount of fees, if any, that you
* have paid directly to Microchip to use this software.
*
* MICROCHIP PROVIDES THIS SOFTWARE CONDITIONALLY UPON YOUR ACCEPTANCE
* OF THESE TERMS.
*
* Author        Date        Comment
*************************************************************************
* E. Schlunder  2009/04/29  Code ported from PicKit2 pk2cmd source code.
* F. Schlunder  2011/06/13  Some changes for USB HID Bootloader use.
*************************************************************************/

#include <QFile>
#include "ImportExportHex.h"
#include "Device.h"


HexImporter::HexImporter(void)
{
}

HexImporter::~HexImporter(void)
{
}


/*
    PIC16Fxx parts use only one address for each FLASH program word. Address 0 has 14 bits of data, Address 1 has
    14 bits of data, etc. However, the PIC16Fxx HEX file addresses each byte of data with a unique address number.
    As a result, you basically have to take the HEX file address and divide by 2 to figure out the actual
    PIC16Fxx FLASH memory address that the byte belongs to.

    Example: PIC16F886 has 8K program words, word addressed as 0 to 0x2000.
        A full HEX file for this part would have 16Kbytes of FLASH data. The HEX file bytes would
        be addressed from 0 to 0x4000.

    This presents a predicament for EEPROM data. Instead of starting from HEX file address 0x2100 as
    the EDC device database might indicate, the HEX file has to start EEPROM data at 0x2000 + 0x2100 = 0x4100,
    to avoid overlapping with the HEX file's FLASH addresses.
*/
HexImporter::ErrorCode HexImporter::ImportHexFile(QString fileName, DeviceData* pData, Device* device)
{
    QFile hexfile(fileName);

    hasEndOfFileRecord = false;
    fileExceedsFlash = false;

    if (!hexfile.open(QIODevice::ReadOnly | QIODevice::Text))
    {
        return CouldNotOpenFile;
    }


    bool ok;
    bool betweenProgrammableRegions = true;
    bool includedInProgrammableRange;
    bool addressWasEndofRange;
    unsigned int segmentAddress = 0;
    unsigned int byteCount;
    unsigned int lineAddress;
    unsigned int deviceAddress;
    unsigned int i;
    unsigned int ramArrayIndex = 0;
    unsigned int lastParsedHexAddress = 0;
    unsigned int numBytesInProgrammableRange;
    unsigned int endDeviceAddressofRegion;
    unsigned int bytesPerAddressAndType;

    unsigned char* pHexFileBinaryDataBufferStart = NULL;
    unsigned char* pHexFileBinaryDataBuffer = NULL;
    unsigned int sizeOfDataBuffer;
    unsigned char* pNewBiggerDataBuffer;
    bool firstDataRecordFound = false;


    unsigned char type;


    HEX32_RECORD recordType;

    QString hexByte;
    unsigned int wordByte;

    DeviceData::MemoryRange range;
    range.start = 0;
    range.end = 0;
    pData->ranges.clear();
    rawimport.clear();
    hasConfigBits = false;


    //Allocate ourselves some RAM to hold the parsed binary hex file data that we extract from the user supplied .hex file.
    pHexFileBinaryDataBufferStart = (unsigned char*)malloc(hexfile.size());
    //Make sure the memory allocation worked
    if(pHexFileBinaryDataBufferStart == NULL)
    {
        //The user must not have enough RAM on their PC.  They should close other apps and try again.
        if(hexfile.isOpen())
            hexfile.close();
        return InsufficientMemory;
    }
    else
    {
        sizeOfDataBuffer = hexfile.size();
        //Initialize the entire RAM buffer to 0xFF, which is the default/erased flash value.
        pHexFileBinaryDataBuffer = pHexFileBinaryDataBufferStart;
        for(i = 0; i < sizeOfDataBuffer; i++)
        {
            *pHexFileBinaryDataBuffer = 0xFF;
            pHexFileBinaryDataBuffer++;
        }
        pHexFileBinaryDataBuffer = pHexFileBinaryDataBufferStart;
    }


    //Now parse the entire hex file, line by line.
    while (!hexfile.atEnd())
    {
        //Fetch a line of ASCII text from the .hex file.
        QByteArray line = hexfile.readLine();

        //Do some error checking on the .hex file contents, to make sure the file is
        //formatted like a legitimate Intel 32-bit formatted .hex file.
        if ((line[0] != ':') || (line.size() < 11))
        {
            //Something is wrong if hex line entry, is not minimum length or does not have leading colon (ex: ":BBAAAATTCC")
            //If an error is detected in the hex file formatting, the safest approach is to
            //abort the operation and force the user to supply a properly formatted hex file.
            if(hexfile.isOpen())
                hexfile.close();
            //Done with the RAM buffer.  Release RAM to avoid memory leakage.
            if(pHexFileBinaryDataBufferStart != NULL)
            {
                free(pHexFileBinaryDataBufferStart);
            }
            return ErrorInHexFile;
        }

        //Extract the info prefix fields from the hex file line data.
        //Example Intel 32-bit hex file line format is as follows (Note: spaces added to separate fields, actual line does not have spaces in it):
        //: 10 0100 00 214601360121470136007EFE09D21901 40
        //Leading ":" is always present on each line from the .hex file.
        //Next two chars (10) are the byte count of the data payload on this hex file line. (ex: 10 = 0x10 = 16 bytes)
        //Next four chars (0100) are the 16 bit address (needs to be combined with the extended linear address to generate a 32-bit address).
        //Next two chars (00) are the "record type".  "00" means it is a "data" record, which means it contains programmable payload data bytes.
        //Next 2n characters are the data payload bytes (where n is the number of bytes specified in the first two numbers (10 in this example))
        //Last two characters on the line are the two complement of the byte checksum computed on the other bytes in the line.
        //For more details on Intel 32-bit hex file formatting see: http://en.wikipedia.org/wiki/Intel_HEX
        byteCount = line.mid(1, 2).toInt(&ok, 16);                      //Convert the two ASCII chars corresponding to the byte count into a binary encoded byte
        lineAddress = segmentAddress + line.mid(3, 4).toInt(&ok, 16);   //Convert the four ASCII chars that correspond to the line address into a 16-bit binary encoded word
        recordType = (HEX32_RECORD)line.mid(7, 2).toInt(&ok, 16);       //Convert the two ASCII chars corresponding to the record type into a binary encoded byte

        //Error check: Verify checksum byte at the end of the .hex file line is valid.  Note,
        //this is not the same checksum as MPLAB(R) IDE uses/computes for the entire hex file.
        //This is only the mini-checksum at the end of each line in the .hex file.
        unsigned int hexLineChecksum = 0;
        for(i = 0; i < (byteCount+4); i++)  //+4 correction is for byte count, 16-bit address, and record type bytes
        {
            hexByte = line.mid(1 + (2 * i), 2); //Fetch two adjacent ASCII bytes from the .hex file
            wordByte = hexByte.toInt(&ok, 16);  //Re-format the above two ASCII bytes into a single binary encoded byte (0x00-0xFF)
            //Add the newly fetched byte to the running checksum
            hexLineChecksum += (unsigned char)wordByte;
        }
        //Now get the two's complement of the hexLineChecksum.
        hexLineChecksum = 0 - hexLineChecksum;
        hexLineChecksum &= 0xFF;    //Truncate to a single byte.  We now have our computed checksum.  This should match the .hex file.
        //Fetch checksum byte from the .hex file
        hexByte = line.mid(1 + (2 * i), 2);     //Fetch the two ASCII bytes that correspond to the checksum byte
        wordByte = hexByte.toInt(&ok, 16);      //Re-format the above two ASCII bytes into a single binary encoded byte (0x00-0xFF)
        wordByte &= 0xFF;
        //Now check if the checksum we computed matches the one at the end of the line in the hex file.
        if(hexLineChecksum != wordByte)
        {
            //Checksum in the hex file doesn't match the line contents.  This implies a corrupted hex file.
            //If an error is detected in the hex file formatting, the safest approach is to
            //abort the operation and force the user to supply a properly formatted hex file.
            if(hexfile.isOpen())
                hexfile.close();
            //Done with the RAM buffer.  Release RAM to avoid memory leakage.
            if(pHexFileBinaryDataBufferStart != NULL)
            {
                free(pHexFileBinaryDataBufferStart);
            }
            return ErrorInHexFile;
        }



        //Check the record type of the hex line, to determine how to continue parsing the data.
        if (recordType == END_OF_FILE)                        // end of file record
        {
            hasEndOfFileRecord = true;
            break;
        }
        else if ((recordType == EXTENDED_SEGMENT_ADDR) || (recordType == EXTENDED_LINEAR_ADDR)) // Segment address
        {
            //Error check: Make sure the line contains the correct number of bytes for the specified record type
            if((unsigned int)line.size() >= (11 + (2 * byteCount)))
            {
                //Fetch the payload, which is the upper 4 or 16-bits of the 20-bit or 32-bit hex file address
                segmentAddress = line.mid(9, 4).toInt(&ok, 16);

                //Load the upper bits of the address
                if (recordType == EXTENDED_SEGMENT_ADDR)
                {
                    segmentAddress <<= 4;
                }
                else
                {
                    segmentAddress <<= 16;
                }

                //Update the line address, now that we know the upper bits are something new.
                lineAddress = segmentAddress + line.mid(3, 4).toInt(&ok, 16);
            }
            else
            {
                //Length appears to be wrong in hex line entry.
                //If an error is detected in the hex file formatting, the safest approach is to
                //abort the operation and force the user to supply a properly formatted hex file.
                if(hexfile.isOpen())
                    hexfile.close();
                //Done with the RAM buffer.  Release RAM to avoid memory leakage.
                if(pHexFileBinaryDataBufferStart != NULL)
                {
                    free(pHexFileBinaryDataBufferStart);
                }
                return ErrorInHexFile;
            }

        } // end if ((recordType == EXTENDED_SEGMENT_ADDR) || (recordType == EXTENDED_LINEAR_ADDR)) // Segment address
        else if (recordType == DATA)                        // Data Record
        {
            //Error check to make sure line is long enough to be consistent with the specified record type
            if ((unsigned int)line.size() < (11 + (2 * byteCount)))
            {
                //If an error is detected in the hex file formatting, the safest approach is to
                //abort the operation and force the user to supply a proper hex file.
                if(hexfile.isOpen())
                    hexfile.close();
                //Done with the RAM buffer.  Release RAM to avoid memory leakage.
                if(pHexFileBinaryDataBufferStart != NULL)
                {
                    free(pHexFileBinaryDataBufferStart);
                }
                return ErrorInHexFile;
            }
            //We found a DATA record type in the hex file.
            //If the DATA recrod was the very first one, initialize some stuff.
            if(firstDataRecordFound != true)
            {
                firstDataRecordFound = true;
                lastParsedHexAddress = lineAddress - 1; //We didn't actually parse anything yet, but this is to make the below code work right on the first iteration.
            }


            //Check if there was a hex file address discontinuity.  This can happen if the hex file skips
            //an address range.  In this case we need to "fill the void" with the default unprogrammed/blank value.
            //We also need to check to see if the end of a programmable region was "contained" inside the void
            //region.  If so, we need to close the region.
            while(lastParsedHexAddress != (lineAddress - 1))
            {
                lastParsedHexAddress++;

                //Check if we have an already started region open.  If so, we need to fill the discontinuity void with 0xFF.
                //If we find the end of the open region, we also need to close the region.
                if(betweenProgrammableRegions == false)
                {
                    ramArrayIndex++; //"puts" 0xFF in the location, since the array was already initialized to all 0xFF

                    //Need to make sure we don't overflow our malloc'ed RAM region.
                    //This could potentially happen if the hex file contained a large
                    //block of "blank" addresses, which we must fill with 0xFF (unprogrammed value).  This could cause
                    //the required RAM to be larger than the total size of the .hex file.  We don't know this ahead of
                    //time however, and can only detect this condition now (as we are busy parsing the .hex file).
                    if(ramArrayIndex >= sizeOfDataBuffer)
                    {
                        pNewBiggerDataBuffer = NULL;
                        pNewBiggerDataBuffer = (unsigned char*)malloc(ramArrayIndex + 50000);   //Give ourselves an extra ~50kB of RAM each time we find we need more.
                        if(pNewBiggerDataBuffer == NULL)
                        {
                            //The user must not have enough RAM on their PC.  They should close other apps and try again.
                            free(pHexFileBinaryDataBufferStart);
                            if(hexfile.isOpen())
                                hexfile.close();
                            return InsufficientMemory;
                        }
                        else
                        {
                            //Initialize the new RAM buffer to 0xFF, the default/blank/unprogrammed value
                            memset(pNewBiggerDataBuffer, 0xFF, (ramArrayIndex + 50000));
                            //Now move our previously bufferred data from pHexFileBinaryDataBufferStart into our new, bigger buffer.
                            memcpy(pNewBiggerDataBuffer, pHexFileBinaryDataBufferStart, sizeOfDataBuffer);
                            sizeOfDataBuffer = ramArrayIndex + 50000;   //Update variable to reflect the new size of our new buffer.
                            free(pHexFileBinaryDataBufferStart);        //Free up/release the previously allocated RAM buffer.  We no longer need it since we have a new bigger one now.
                            pHexFileBinaryDataBufferStart = pNewBiggerDataBuffer;   //Update the pHexFileBinaryDataBufferStart pointer to point to the new buffer.
                        }
                    }


                    //Do a check to see if we are at the end of the opened region.
                    device->GetDeviceAddressFromHexAddress(lastParsedHexAddress, type, includedInProgrammableRange, addressWasEndofRange, bytesPerAddressAndType, endDeviceAddressofRegion);
                    if(addressWasEndofRange == true)
                    {
                        //We just found the true end (we just parsed and saved the very last byte) of a contiguous
                        //progammable memory region.
                        //Save the temporary buffer to the global buffer for use later during programming and verify.
                        numBytesInProgrammableRange = (range.end - range.start)* bytesPerAddressAndType;
                        range.data = new unsigned char[numBytesInProgrammableRange];
                        //Copy the data from the parsed .hex file results, into the RAM buffer that will get programmed into the device.
                        memcpy(&range.data[0], pHexFileBinaryDataBufferStart, numBytesInProgrammableRange);
                        pData->ranges.append(range);    //Save to global buffer used for programming the device.
                        //Iniitalize all elements to 0xFF (default unprogrammed value, so we don't have to do this later if we detect hex file address discontinuities).
                        memset(pHexFileBinaryDataBufferStart, 0xFF, sizeOfDataBuffer);
                        ramArrayIndex = 0;
                        betweenProgrammableRegions = true;  //We are no longer in a contiguous programmable region.  We should begin looking again for the start of a new programmable section.
                    }
                }
            }



            //For each data payload byte we find in the hex file line, check if it is contained within
            //a progammable region inside the microcontroller.  If so save it.  If not, discard it.
            for(i = 0; i < byteCount; i++)
            {
                //Use the hex file linear byte address, to compute the address this corresponds
                //to in the microcontroller.  For PIC18, there is a 1:1 correspondence of program memory addresses.
                //For PIC24, the flash memory is organized as a 16-bit word array, so the hex file
                //byte address isn't directly equivalent to the flash memory word address.
                deviceAddress = device->GetDeviceAddressFromHexAddress(lineAddress + i, type, includedInProgrammableRange, addressWasEndofRange, bytesPerAddressAndType, endDeviceAddressofRegion);
                //We can now decide what to do with the new hex file line data bytes.
                if(betweenProgrammableRegions == true)
                {
                    if(includedInProgrammableRange == true)
                    {
                        //We found the first byte of a new programmable region.
                        betweenProgrammableRegions = false;
                        range.start = deviceAddress;
                        range.end = endDeviceAddressofRegion;
                        range.type = type;
                        ramArrayIndex = 0;
                        //Fetch ASCII encoded payload byte from .hex file and save the byte to our temporary RAM buffer.
                        hexByte = line.mid(9 + (2 * i), 2);  //Fetch two ASCII data payload bytes from the .hex file
                        wordByte = hexByte.toInt(&ok, 16);   //Re-format the above two ASCII bytes into a single binary encoded byte (0x00-0xFF)

                        //Save the parsed byte from the hex file into our RAM buffer.
                        //However need to make sure we don't overflow our malloc'ed RAM region.  If we need more RAM,
                        //make sure to allocate more first.  This could potentially happen if the hex file contained a large
                        //block of "blank" addresses, which we must fill with 0xFF (unprogrammed value).  This could cause
                        //the required RAM to be larger than the total size of the .hex file.  We don't know this ahead of
                        //time however, and can only detect this condition now (as we are busy parsing the .hex file).
                        if(ramArrayIndex >= sizeOfDataBuffer)
                        {
                            pNewBiggerDataBuffer = NULL;
                            pNewBiggerDataBuffer = (unsigned char*)malloc(ramArrayIndex + 50000);   //Give ourselves an extra ~50kB of RAM each time we find we need more.
                            if(pNewBiggerDataBuffer == NULL)
                            {
                                //The user must not have enough RAM on their PC.  They should close other apps and try again.
                                free(pHexFileBinaryDataBufferStart);
                                if(hexfile.isOpen())
                                    hexfile.close();
                                return InsufficientMemory;
                            }
                            else
                            {
                                //Initialize the new RAM buffer to 0xFF, the default/blank/unprogrammed value
                                memset(pNewBiggerDataBuffer, 0xFF, (ramArrayIndex + 50000));
                                //Now move our previously bufferred data from pHexFileBinaryDataBufferStart into our new, bigger buffer.
                                memcpy(pNewBiggerDataBuffer, pHexFileBinaryDataBufferStart, sizeOfDataBuffer);
                                sizeOfDataBuffer = ramArrayIndex + 50000;   //Update variable to reflect the new size of our new buffer.
                                free(pHexFileBinaryDataBufferStart);        //Free up/release the previously allocated RAM buffer.  We no longer need it since we have a new bigger one now.
                                pHexFileBinaryDataBufferStart = pNewBiggerDataBuffer;   //Update the pHexFileBinaryDataBufferStart pointer to point to the new buffer.
                            }
                        }
                        //Now we can finally save the byte, now that we are certain we have enough RAM available in the pHexFileBinaryDataBufferStart buffer.
                        *(pHexFileBinaryDataBufferStart + ramArrayIndex) = (unsigned char)wordByte; //Save the byte
                        ramArrayIndex++;

                        //Check if this was a config bit memory region.  If so, set flag later so we know the hex file had config bit info in it.
                        //This way, we can prevent the user from trying to re-program config bits, when their supplied hex file doesn't contain
                        //any config bit data in it.
                        if (range.type == CONFIG_MEMORY)
                        {
                            hasConfigBits = true;
                        }

                        //Although this was the first bytes of a programmable region, we still also need to check to see if
                        //this was also the end of a programmable memory region (ex: a 1 byte region, which is potentially
                        //legal for some types of memory).
                        //First, check if the device address corresponded the to the last address (not necessarily last
                        //byte) of a programmable memory region.
                        if(addressWasEndofRange == true)
                        {
                            //We just found the true end (we just parsed and saved the very last byte) of a contiguous
                            //progammable memory region.
                            //Save the temporary buffer to the global buffer for use later during programming and verify.
                            numBytesInProgrammableRange = (range.end - range.start)* bytesPerAddressAndType;
                            range.data = new unsigned char[numBytesInProgrammableRange];
                            //Copy the data from the parsed .hex file results, into the RAM buffer that will get programmed into the device.
                            memcpy(&range.data[0], pHexFileBinaryDataBufferStart, numBytesInProgrammableRange);
                            pData->ranges.append(range);    //Save to global buffer used for programming the device.
                            //Iniitalize all elements to 0xFF (default unprogrammed value, so we don't have to do this later if we detect hex file address discontinuities).
                            memset(pHexFileBinaryDataBufferStart, 0xFF, sizeOfDataBuffer);
                            betweenProgrammableRegions = true;  //We are no longer in a contiguous programmable region.  We should begin looking again for the start of a new programmable section.
                        }

                    }//if(includedInProgrammableRange == true)
                    else
                    {
                        //In this case the .hex file data byte was not included in any of the device's reported
                        //programmable memory regions (ex: because it was part of the occupied bootloader firmware
                        //program memory space for instance).  Therefore, we simply discard the byte, since we won't
                        //be programming it into the device at all.
                    }
                }//if(betweenProgrammableRegions == true)
                else
                {
                    //We have already started a programmable region.

                    //Check if the byte just parsed from the hex file was contained in the
                    //programmable memory region that we already started
                    if(includedInProgrammableRange == true)
                    {
                        //Save the byte to our temporary RAM buffer.
                        hexByte = line.mid(9 + (2 * i), 2);  //Fetch two ASCII data payload bytes from the .hex file
                        wordByte = hexByte.toInt(&ok, 16);   //Re-format the above two ASCII bytes into a single binary encoded byte (0x00-0xFF)

                        //Save the parsed byte from the hex file into our RAM buffer.
                        //However need to make sure we don't overflow our malloc'ed RAM region.
                        //This could potentially happen if the hex file contained a large
                        //block of "blank" addresses, which we must fill with 0xFF (unprogrammed value).  This could cause
                        //the required RAM to be larger than the total size of the .hex file.  We don't know this ahead of
                        //time however, and can only detect this condition now (as we are busy parsing the .hex file).
                        if(ramArrayIndex >= sizeOfDataBuffer)
                        {
                            pNewBiggerDataBuffer = NULL;
                            pNewBiggerDataBuffer = (unsigned char*)malloc(ramArrayIndex + 50000);   //Give ourselves an extra ~50kB of RAM each time we find we need more.
                            if(pNewBiggerDataBuffer == NULL)
                            {
                                //The user must not have enough RAM on their PC.  They should close other apps and try again.
                                free(pHexFileBinaryDataBufferStart);
                                if(hexfile.isOpen())
                                    hexfile.close();
                                return InsufficientMemory;
                            }
                            else
                            {
                                //Initialize the new RAM buffer to 0xFF, the default/blank/unprogrammed value
                                memset(pNewBiggerDataBuffer, 0xFF, (ramArrayIndex + 50000));
                                //Now move our previously bufferred data from pHexFileBinaryDataBufferStart into our new, bigger buffer.
                                memcpy(pNewBiggerDataBuffer, pHexFileBinaryDataBufferStart, sizeOfDataBuffer);
                                sizeOfDataBuffer = ramArrayIndex + 50000;   //Update variable to reflect the new size of our new buffer.
                                free(pHexFileBinaryDataBufferStart);        //Free up/release the previously allocated RAM buffer.  We no longer need it since we have a new bigger one now.
                                pHexFileBinaryDataBufferStart = pNewBiggerDataBuffer;   //Update the pHexFileBinaryDataBufferStart pointer to point to the new buffer.
                            }
                        }
                        //Now we can finally save the byte, now that we are certain we have enough RAM available in the pHexFileBinaryDataBufferStart buffer.
                        *(pHexFileBinaryDataBufferStart + ramArrayIndex) = (unsigned char)wordByte; //Save the byte
                        ramArrayIndex++;

                        //Check to see if this is the end of a programmable memory region.  First, check if the device address
                        //corresponded the to the last address (not necessarily last byte) of a programmable memory region.
                        if(addressWasEndofRange == true)
                        {
                            //Check if this was the very last byte of the programmable memory region.  (which can be determined
                            //if the ramArrayIndex is an exact integer multiple of the bytesPerAddressAndType).
                            if((ramArrayIndex % bytesPerAddressAndType) == 0)
                            {
                                //We just found the true end (we just parsed and saved the very last byte) of a contiguous
                                //progammable memory region.
                                //Save the temporary buffer to the global buffer for use later during programming and verify.
                                numBytesInProgrammableRange = (range.end - range.start)* bytesPerAddressAndType;
                                range.data = new unsigned char[numBytesInProgrammableRange];
                                //Copy the data from the parsed .hex file results, into the RAM buffer that will get programmed into the device.
                                memcpy(&range.data[0], pHexFileBinaryDataBufferStart, numBytesInProgrammableRange);
                                pData->ranges.append(range);    //Save to global buffer used for programming the device.

                                //Iniitalize all elements to 0xFF (default unprogrammed value, so we don't have to do this later if we detect hex file address discontinuities).
                                memset(pHexFileBinaryDataBufferStart, 0xFF, sizeOfDataBuffer);
                                betweenProgrammableRegions = true;  //We are no longer in a contiguous programmable region.  We should begin looking again for the start of a new programmable section.
                            }
                        }
                    }
                    else
                    {
                        //If we get to here, that means that we already started a programmable memory region, but
                        //have not closed it yet, and yet somehow the most recently parsed byte wasn't included in the
                        //region.
                        //We should never get to this else, unless there is some error in the hex file addresses (ex: they go
                        //out of order, not purely increasing address, but instead something else, ex: increase, then decreasing, then increasing again).

                        //If an error is detected in the hex file formatting, the safest approach is to
                        //abort the operation and force the user to supply a proper hex file.
                        if(hexfile.isOpen())
                            hexfile.close();
                        //Done with the RAM buffer.  Release RAM to avoid memory leakage.
                        if(pHexFileBinaryDataBufferStart != NULL)
                        {
                            free(pHexFileBinaryDataBufferStart);
                        }
                        return ErrorInHexFile;
                    }
                }//end of else (of if(betweenProgrammableRegions == true))

                lastParsedHexAddress = lineAddress + i;    //Save this.  We need to check this later so we can detect hex file address discontinuities.
            }//for(i = 0; i < byteCount; i++)
        } // end else if (recordType == DATA)
    }//while (!hexfile.atEnd())

    if(hexfile.isOpen())
    {
        //Make one last check.  The end of file may have been reached while we were
        //still saving data from a contiguous programmable region.  If so, need to save this
        //data.
        if(betweenProgrammableRegions == false)
        {
            //We are at the end of the contiguous progammable memory region.
            //Save the temporary buffer to the global buffer for use later.
            numBytesInProgrammableRange = (range.end - range.start)* bytesPerAddressAndType;
            range.data = new unsigned char[numBytesInProgrammableRange];
            //Copy the data from the parsed .hex file results, into the RAM buffer that will get programmed into the device.
            memcpy(&range.data[0], pHexFileBinaryDataBufferStart, numBytesInProgrammableRange);
            pData->ranges.append(range);    //Save to global buffer used for programming the device.
        }

        hexfile.close();
    }

    //Done with the RAM buffer.  Release RAM to avoid memory leakage.
    if(pHexFileBinaryDataBufferStart != NULL)
    {
        free(pHexFileBinaryDataBufferStart);
    }


    if(pData->ranges.count())
    {
        //Print some debug info, showing the memory ranges that we successfully imported
        //from the hex file (that was also in common with the device's reported programmable region(s).
        qDebug("Hex Import Address Ranges:");
        for(int i = 0; i < pData->ranges.count(); i++)
        {
            qDebug(QString("  [" + QString::number(pData->ranges[i].start, 16).toUpper() + " - " +
                           QString::number(pData->ranges[i].end, 16).toUpper() +")").toLatin1());
        }

        return Success;
    }
    else
    {
        return NoneInRange;
    }
}





