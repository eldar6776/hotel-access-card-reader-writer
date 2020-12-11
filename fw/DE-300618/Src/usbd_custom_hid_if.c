/**
  ******************************************************************************
  * @file           : usbd_custom_hid_if.c
  * @version        : v2.0_Cube
  * @brief          : USB Device Custom HID interface file.
  ******************************************************************************
  * This notice applies to any and all portions of this file
  * that are not between comment pairs USER CODE BEGIN and
  * USER CODE END. Other portions of this file, whether 
  * inserted by the user or by software development tools
  * are owned by their respective copyright owners.
  *
  * Copyright (c) 2018 STMicroelectronics International N.V. 
  * All rights reserved.
  *
  * Redistribution and use in source and binary forms, with or without 
  * modification, are permitted, provided that the following conditions are met:
  *
  * 1. Redistribution of source code must retain the above copyright notice, 
  *    this list of conditions and the following disclaimer.
  * 2. Redistributions in binary form must reproduce the above copyright notice,
  *    this list of conditions and the following disclaimer in the documentation
  *    and/or other materials provided with the distribution.
  * 3. Neither the name of STMicroelectronics nor the names of other 
  *    contributors to this software may be used to endorse or promote products 
  *    derived from this software without specific written permission.
  * 4. This software, including modifications and/or derivative works of this 
  *    software, must execute solely and exclusively on microcontroller or
  *    microprocessor devices manufactured by or for STMicroelectronics.
  * 5. Redistribution and use of this software other than as permitted under 
  *    this license is void and will automatically terminate your rights under 
  *    this license. 
  *
  * THIS SOFTWARE IS PROVIDED BY STMICROELECTRONICS AND CONTRIBUTORS "AS IS" 
  * AND ANY EXPRESS, IMPLIED OR STATUTORY WARRANTIES, INCLUDING, BUT NOT 
  * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
  * PARTICULAR PURPOSE AND NON-INFRINGEMENT OF THIRD PARTY INTELLECTUAL PROPERTY
  * RIGHTS ARE DISCLAIMED TO THE FULLEST EXTENT PERMITTED BY LAW. IN NO EVENT 
  * SHALL STMICROELECTRONICS OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
  * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
  * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
  * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
  * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
  * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
  * EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
  *
  ******************************************************************************
  */

/* Includes ------------------------------------------------------------------*/
#include "usbd_custom_hid_if.h"
#include "rc522.h"
#include "common.h"

extern uint8_t rx_buf[64];
extern uint8_t tx_buf[64];

uint8_t cmd_cnt = 0;
uint8_t led_gn_cnt = 0;
uint8_t led_rd_cnt = 0;
uint8_t buzz_cnt = 0;
uint32_t address = 0;
                     
          
/** Usb HID report descriptor. */
__ALIGN_BEGIN static uint8_t CUSTOM_HID_ReportDesc_FS[USBD_CUSTOM_HID_REPORT_DESC_SIZE] __ALIGN_END =
{
 0x06, 0x00, 0xFF,       // Usage Page = 0xFF00 (Vendor Defined Page 1)
    0x09, 0x01,             // Usage (Vendor Usage 1)
    0xA1, 0x01,             // Collection (Application)
    0x19, 0x01,             //      Usage Minimum 
    0x29, 0x40,             //      Usage Maximum 	//64 input usages total (0x01 to 0x40)
    0x15, 0x01,             //      Logical Minimum (data bytes in the report may have minimum value = 0x00)
    0x25, 0x40,      	  	//      Logical Maximum (data bytes in the report may have maximum value = 0x00FF = unsigned 255)
    0x75, 0x08,             //      Report Size: 8-bit field size
    0x95, 0x40,             //      Report Count: Make sixty-four 8-bit fields (the next time the parser hits an "Input", "Output", or "Feature" item)
    0x81, 0x00,             //      Input (Data, Array, Abs): Instantiates input packet fields based on the above report size, count, logical min/max, and usage.
    0x19, 0x01,             //      Usage Minimum 
    0x29, 0x40,             //      Usage Maximum 	//64 output usages total (0x01 to 0x40)
    0x91, 0x00,             //      Output (Data, Array, Abs): Instantiates output packet fields.  Uses same report size and count as "Input" fields, since nothing new/different was specified to the parser since the "Input" item.
    0xC0
};


extern USBD_HandleTypeDef hUsbDeviceFS;

static int8_t CUSTOM_HID_Init_FS(void);
static int8_t CUSTOM_HID_DeInit_FS(void);
static int8_t CUSTOM_HID_OutEvent_FS(uint8_t event_idx);
void CardDataFail(void);
void CopyCardData(void);
void CardDataSucceed(void);
uint8_t ReadHotelCard(void);


USBD_CUSTOM_HID_ItfTypeDef USBD_CustomHID_fops_FS =
{
    CUSTOM_HID_ReportDesc_FS,
    CUSTOM_HID_Init_FS,
    CUSTOM_HID_DeInit_FS,
    CUSTOM_HID_OutEvent_FS
};


/* Private functions ---------------------------------------------------------*/

/**
  * @brief  Initializes the CUSTOM HID media low layer
  * @retval USBD_OK if all operations are OK else USBD_FAIL
  */
static int8_t CUSTOM_HID_Init_FS(void)
{

    return (USBD_OK);

}

/**
  * @brief  DeInitializes the CUSTOM HID media low layer
  * @retval USBD_OK if all operations are OK else USBD_FAIL
  */
static int8_t CUSTOM_HID_DeInit_FS(void)
{
  return (USBD_OK);
}


static int8_t USBD_CUSTOM_HID_SendReport_FS(uint8_t *report, uint16_t len)
{
    return USBD_CUSTOM_HID_SendReport(&hUsbDeviceFS, report, len);
}

/**
  * @brief  Manage the CUSTOM HID class events
  * @param  event_idx: Event index
  * @param  state: Event state
  * @retval USBD_OK if all operations are OK else USBD_FAIL
  */
static int8_t CUSTOM_HID_OutEvent_FS(uint8_t event_idx)
{
    cmd_cnt = 0;
    led_rd_cnt = 0;
    GreenLed_Off();
    RC522_ClearData();
    ClearBuffer(tx_buf, sizeof(tx_buf));
    
    switch (event_idx) //Look at the data the host sent, to see what kind of application specific command it sent.
    {
        case ('R'): // Read hotel card      
        {
            rc522_sector = SECTOR_0;
            rc522_command = RC522_READ;

            while (RC522_Service() != MI_OK) 
            {
                if (++led_rd_cnt == 10) 
                {
                    RedLed_Toggle();
                    led_rd_cnt = 0;

                    if (++cmd_cnt == 10) 
                    {
                        CardDataFail();
                        return (USBD_OK);
                    }
                }
            }
            
            if (ReadHotelCard() != MI_OK) return (USBD_OK);
            USBD_CUSTOM_HID_SendReport_FS(tx_buf, 64);
            CardDataSucceed();
            break;
        }
        
            
        case ('C'): // Read pool card   
        {
            rc522_sector = SECTOR_3;
            rc522_command = RC522_READ;

            while (RC522_Service() != MI_OK) 
            {
                if (++led_rd_cnt == 10) 
                {
                    RedLed_Toggle();
                    led_rd_cnt = 0;

                    if (++cmd_cnt == 10) 
                    {
                        CardDataFail();
                        return (USBD_OK);
                    }
                }
            }
            
            CopyCardData();
            USBD_CUSTOM_HID_SendReport_FS(tx_buf, 64);
            CardDataSucceed();
            break;
        }
       
        
        case ('W'): // Write hotel card
        {
            rc522_sector = SECTOR_0;
            rc522_command = RC522_READ;
            
            while (RC522_Service() != MI_OK) 
            {
                if (++led_rd_cnt == 10) 
                {
                    RedLed_Toggle();
                    led_rd_cnt = 0;

                    if (++cmd_cnt == 10) 
                    {
                        CardDataFail();
                        return (USBD_OK);
                    }
                }
            }

            tx_buf[0] = 'I';
            tx_buf[1] = aCardSerial[0];
            tx_buf[2] = aCardSerial[1];
            tx_buf[3] = aCardSerial[2];
            tx_buf[4] = aCardSerial[3];
            tx_buf[5] = aCardSerial[4];
            /*
             *  WRITE SECTOR 0
             */
            RC522_ClearData();
            
            for (cmd_cnt = 0; cmd_cnt < 16; cmd_cnt++)
            {
                Sector.Block_1[cmd_cnt] = rx_buf[cmd_cnt + 31];
                Sector.Block_2[cmd_cnt] = rx_buf[cmd_cnt + 47];
            }

            rc522_sector = SECTOR_0;
            rc522_command = RC522_WRITE;
            
            if(RC522_Service() != MI_OK) 
            {
                CardDataFail();
                return (USBD_OK);
            }
            /*
             *  WRITE SECTOR 1
             */
            RC522_ClearData();
            
            if ((rx_buf[1] == CARD_USER_GROUP_GUEST) || \
            (rx_buf[1] == CARD_USER_GROUP_HANDMAID) || \
            (rx_buf[1] == CARD_USER_GROUP_MANAGER) || \
            (rx_buf[1] == CARD_USER_GROUP_SERVICE) || \
            (rx_buf[1] == CARD_USER_GROUP_PRESET))
            {
                Sector.Block_0[1] = rx_buf[1];
            }
            else
            {
                CardDataFail();
                return (USBD_OK);
            }

            for (cmd_cnt = 0; cmd_cnt < 5; cmd_cnt++)
            {
                Sector.Block_1[cmd_cnt] = rx_buf[cmd_cnt + 21];
            }

            rc522_sector = SECTOR_1;
            rc522_command = RC522_WRITE;
            
            if(RC522_Service() != MI_OK) 
            {
                CardDataFail();
                return (USBD_OK);
            }
            /*
             *  WRITE SECTOR 2
             */
            RC522_ClearData();
            Sector.Block_0[0] = rx_buf[2] << 4;
            Sector.Block_0[0] += rx_buf[3] & 0x0f;
            Sector.Block_0[1] = rx_buf[4] << 4;
            Sector.Block_0[1] += rx_buf[5] & 0x0f;
            Sector.Block_0[2] = rx_buf[6] << 4;
            Sector.Block_0[2] += rx_buf[7] & 0x0f;
            Sector.Block_0[3] = rx_buf[8] << 4;
            Sector.Block_0[3] += rx_buf[9] & 0x0f;
            Sector.Block_0[4] = rx_buf[10] << 4;
            Sector.Block_0[4] += rx_buf[11] & 0x0f;
            Sector.Block_0[5] = 0x00;
            
            address = Str2IntSized(&rx_buf[12], 5);

            Sector.Block_0[6] = (address >> 8);
            Sector.Block_0[7] = (address & 0xff);

            for (cmd_cnt = 8; cmd_cnt < 12; cmd_cnt++)
            {
                Sector.Block_0[cmd_cnt] = rx_buf[cmd_cnt + 9];
            }

            rc522_sector = SECTOR_2;
            rc522_command = RC522_WRITE;
            
            if(RC522_Service() != MI_OK) 
            {
                CardDataFail();
                return (USBD_OK);
            }
            /***********************************************************
             * 
             *          C H E C K    W R I T T E N     D A T A
             * 
             **********************************************************/
            RC522_ClearData();
            rc522_sector = SECTOR_0;
            rc522_command = RC522_READ;
            
            while (RC522_Service() != MI_OK) 
            {
                if (++led_rd_cnt == 10) 
                {
                    RedLed_Toggle();
                    led_rd_cnt = 0;

                    if (++cmd_cnt == 10) 
                    {
                        CardDataFail();
                        return (USBD_OK);
                    }
                }
            }
            
            if ((tx_buf[1] != aCardSerial[0]) ||  \
                (tx_buf[2] != aCardSerial[1]) ||  \
                (tx_buf[3] != aCardSerial[2]) ||  \
                (tx_buf[4] != aCardSerial[3]) ||  \
                (tx_buf[5] != aCardSerial[4]))
            {
                CardDataFail();
                return (USBD_OK);
            }
            
            RC522_ClearData();
            rc522_sector = SECTOR_1;
            rc522_command = RC522_READ;

            while (RC522_Service() != MI_OK) 
            {
                if (++led_rd_cnt == 10) 
                {
                    RedLed_Toggle();
                    led_rd_cnt = 0;

                    if (++cmd_cnt == 10) 
                    {
                        CardDataFail();
                        return (USBD_OK);
                    }
                }
            }
            
            if(Sector.Block_0[1] != rx_buf[1])
            {
                CardDataFail();
                return (USBD_OK);
            }
        
            RC522_ClearData();
            rc522_sector = SECTOR_2;
            rc522_command = RC522_READ;
            
            while (RC522_Service() != MI_OK) 
            {
                if (++led_rd_cnt == 10) 
                {
                    RedLed_Toggle();
                    led_rd_cnt = 0;

                    if (++cmd_cnt == 10) 
                    {
                        CardDataFail();
                        return (USBD_OK);
                    }
                }
            }

            if (((Sector.Block_0[0] & 0xf0) != ((rx_buf[2] << 4) & 0xf0))||  \
                ((Sector.Block_0[0] & 0x0f) != (rx_buf[3] & 0x0f)) ||  \
                ((Sector.Block_0[1] & 0xf0) != ((rx_buf[4] << 4) & 0xf0)) ||  \
                ((Sector.Block_0[1] & 0x0f) != (rx_buf[5] & 0x0f)) ||  \
                ((Sector.Block_0[2] & 0xf0) != ((rx_buf[6] << 4) & 0xf0)) ||  \
                ((Sector.Block_0[2] & 0x0f) != (rx_buf[7] & 0x0f)) ||  \
                ((Sector.Block_0[3] & 0xf0) != ((rx_buf[8] << 4)& 0xf0)) ||  \
                ((Sector.Block_0[3] & 0x0f) != (rx_buf[9] & 0x0f)) ||  \
                ((Sector.Block_0[4] & 0xf0) != ((rx_buf[10] << 4)& 0xf0)) ||  \
                ((Sector.Block_0[4] & 0x0f) != (rx_buf[11] & 0x0f)) ||  \
                (Sector.Block_0[5] != 0x00))
            {
                CardDataFail();
                return (USBD_OK);
            }
            else if((Sector.Block_0[6] != (address >> 8)) ||  \
                    (Sector.Block_0[7] != (address & 0xff)) ||  \
                    (Sector.Block_0[8] != rx_buf[17]) ||  \
                    (Sector.Block_0[9] != rx_buf[18]) ||  \
                    (Sector.Block_0[10] != rx_buf[19]) ||  \
                    (Sector.Block_0[11] != rx_buf[20]))
            {
                CardDataFail();
                return (USBD_OK);
            }

            USBD_CUSTOM_HID_SendReport_FS(tx_buf, 64);
            CardDataSucceed();
            break;
        }

          
        case ('P'):
        {
            if (rx_buf[20] != CARD_USER_GROUP_POOL) 
            {
                CardDataFail();
                return (USBD_OK);
            }
            
            rc522_sector = SECTOR_3;
            rc522_command = RC522_READ;
            
            while (RC522_Service() != MI_OK) 
            {
                if (++led_rd_cnt == 10) 
                {
                    RedLed_Toggle();
                    led_rd_cnt = 0;

                    if (++cmd_cnt == 10) 
                    {
                        CardDataFail();
                        return (USBD_OK);
                    }
                }
            }

            tx_buf[0] = 'I';
            tx_buf[1] = aCardSerial[0];
            tx_buf[2] = aCardSerial[1];
            tx_buf[3] = aCardSerial[2];
            tx_buf[4] = aCardSerial[3];
            tx_buf[5] = aCardSerial[4];
            /***********************************************************
             * 
             *              SECTOR 3 BLOCK 0
             * 
             ***********************************************************
             * 
             *  expiry date & time
             */
            RC522_ClearData();
            
            Sector.Block_0[0] = rx_buf[1] << 4;
            Sector.Block_0[0] += rx_buf[2] & 0x0f;
            Sector.Block_0[1] = rx_buf[3] << 4;
            Sector.Block_0[1] += rx_buf[4] & 0x0f;
            Sector.Block_0[2] = rx_buf[5] << 4;
            Sector.Block_0[2] += rx_buf[6] & 0x0f;
            Sector.Block_0[3] = rx_buf[7] << 4;
            Sector.Block_0[3] += rx_buf[8] & 0x0f;
            Sector.Block_0[4] = rx_buf[9] << 4;
            Sector.Block_0[4] += rx_buf[10] & 0x0f;
            Sector.Block_0[5] = 0x00;
            /**
             *  number of users
             */
            Sector.Block_0[6] = (rx_buf[11] << 4);
            Sector.Block_0[6] += rx_buf[12] & 0x0f;
            /**
             *  tag type
             */
            Sector.Block_0[10] = rx_buf[13];
            /**
             *  card type
             * 
             */
            Sector.Block_0[7] = rx_buf[14];
            /**
             *  system id
             */
            address = Str2IntSized(&rx_buf[15], 5);

            Sector.Block_0[8] = (address >> 8);
            Sector.Block_0[9] = (address & 0xff);
            /**
             *  user group
             */
            Sector.Block_0[11] = rx_buf[20];

            rc522_sector = SECTOR_3;
            rc522_command = RC522_WRITE;
            
            if(RC522_Service() != MI_OK) 
            {
                CardDataFail();
                return (USBD_OK);
            }
            /***********************************************************
             * 
             *          C H E C K    W R I T T E N     D A T A
             * 
             **********************************************************/
            RC522_ClearData();
            rc522_sector = SECTOR_3;
            rc522_command = RC522_READ;
            
            while (RC522_Service() != MI_OK) 
            {
                if (++led_rd_cnt == 10) 
                {
                    RedLed_Toggle();
                    led_rd_cnt = 0;

                    if (++cmd_cnt == 10) 
                    {
                        CardDataFail();
                        return (USBD_OK);
                    }
                }
            }

            if ((tx_buf[1] != aCardSerial[0]) ||  \
                (tx_buf[2] != aCardSerial[1]) ||  \
                (tx_buf[3] != aCardSerial[2]) ||  \
                (tx_buf[4] != aCardSerial[3]) ||  \
                (tx_buf[5] != aCardSerial[4])) 
            {
                CardDataFail();
                return (USBD_OK);
            }
            else if (((Sector.Block_0[0] & 0xf0) != ((rx_buf[1] << 4) & 0xf0))||  \
                ((Sector.Block_0[0] & 0x0f) != (rx_buf[2] & 0x0f)) ||  \
                ((Sector.Block_0[1] & 0xf0) != ((rx_buf[3] << 4) & 0xf0)) ||  \
                ((Sector.Block_0[1] & 0x0f) != (rx_buf[4] & 0x0f)) ||  \
                ((Sector.Block_0[2] & 0xf0) != ((rx_buf[5] << 4) & 0xf0)) ||  \
                ((Sector.Block_0[2] & 0x0f) != (rx_buf[6] & 0x0f)) ||  \
                ((Sector.Block_0[3] & 0xf0) != ((rx_buf[7] << 4)& 0xf0)) ||  \
                ((Sector.Block_0[3] & 0x0f) != (rx_buf[8] & 0x0f)) ||  \
                ((Sector.Block_0[4] & 0xf0) != ((rx_buf[9] << 4)& 0xf0)) ||  \
                ((Sector.Block_0[4] & 0x0f) != (rx_buf[10] & 0x0f)) ||  \
                (Sector.Block_0[5] != 0x00))
            {
                CardDataFail();
                return (USBD_OK);
            }
            else if((Sector.Block_0[6] != (((rx_buf[11] << 4) & 0xf0) + (rx_buf[12] & 0x0f))) ||  \
                (Sector.Block_0[7] != rx_buf[14]) ||  \
                (Sector.Block_0[8] != (address >> 8)) ||  \
                (Sector.Block_0[9] != (address & 0xff)) ||  \
                (Sector.Block_0[10] != rx_buf[13]) ||  \
                (Sector.Block_0[11] != rx_buf[20])) 
            {
                CardDataFail();
                return (USBD_OK);
            }
            
            USBD_CUSTOM_HID_SendReport_FS(tx_buf, 64);
            CardDataSucceed();
            break; 
        }
    }
    
    return (USBD_OK);
}


void CardDataFail(void)
{
    Buzzer_On();
    GreenLed_On();
    HAL_Delay(500);
    Buzzer_Off();
    GreenLed_Off();
    ClearBuffer(tx_buf, sizeof(tx_buf));
    ClearBuffer(rx_buf, sizeof(rx_buf));
    rc522_command = RC522_IDLE;
    tx_buf[0] = 'F';
    tx_buf[1] = 'A';
    tx_buf[2] = 'I';
    tx_buf[3] = 'L';
    USBD_CUSTOM_HID_SendReport_FS(tx_buf, 64);
}

void CardDataSucceed(void) 
{
    cmd_cnt = 0;

    while (cmd_cnt < 3)        
    {
        Buzzer_On();
        GreenLed_On();
        HAL_Delay(30);
        Buzzer_Off();
        GreenLed_Off();
        HAL_Delay(100);
        cmd_cnt++;
    }
}

uint8_t ReadHotelCard(void)
{
    ClearBuffer(tx_buf, sizeof(tx_buf));
    
    tx_buf[0] = 'I';
    tx_buf[1] = aCardSerial[0];
    tx_buf[2] = aCardSerial[1];
    tx_buf[3] = aCardSerial[2];
    tx_buf[4] = aCardSerial[3];
    tx_buf[5] = aCardSerial[4];

    for (cmd_cnt = 0; cmd_cnt < 16; cmd_cnt++) 
    {
        //tx_buf[cmd_cnt + 32] = Sector.BlockData.Block_1[cmd_cnt];
        tx_buf[cmd_cnt + 48] = Sector.Block_2[cmd_cnt];
    }
    /*
     *  --------------------------------  R E A D    S E C T O R   1
     */
    cmd_cnt = 0;
    led_rd_cnt = 0;
    rc522_sector = SECTOR_1;
    rc522_command = RC522_READ;
    
    while (RC522_Service() != MI_OK) 
    {
        if (++led_rd_cnt == 10) 
        {
            RedLed_Toggle();
            led_rd_cnt = 0;

            if (++cmd_cnt == 10) 
            {
                CardDataFail();
                return (MI_ERR);
            }
        }
    }

    tx_buf[6] = 'G';

    for (cmd_cnt = 0; cmd_cnt < 16; cmd_cnt++) 
    {
        if ((Sector.Block_0[cmd_cnt] == CARD_USER_GROUP_GUEST) || \
            (Sector.Block_0[cmd_cnt] == CARD_USER_GROUP_HANDMAID) || \
            (Sector.Block_0[cmd_cnt] == CARD_USER_GROUP_MANAGER) || \
            (Sector.Block_0[cmd_cnt] == CARD_USER_GROUP_SERVICE) || \
            (Sector.Block_0[cmd_cnt] == CARD_USER_GROUP_PRESET))
        {
            tx_buf[7] = Sector.Block_0[cmd_cnt];
        }
    }

    tx_buf[32] = Sector.Block_1[0];
    tx_buf[33] = Sector.Block_1[1];
    tx_buf[34] = Sector.Block_1[2];
    tx_buf[35] = Sector.Block_1[3];
    tx_buf[36] = Sector.Block_1[4];
    /*
     *  --------------------------------  R E A D    S E C T O R   2
     */
    led_rd_cnt = 0;
    rc522_sector = SECTOR_2;
    rc522_command = RC522_READ;

    while (RC522_Service() != MI_OK) 
    {
        if (++led_rd_cnt == 10) 
        {
            RedLed_Toggle();
            led_rd_cnt = 0;

            if (++cmd_cnt == 10) 
            {
                CardDataFail();
                return (MI_ERR);
            }
        }
    }

    tx_buf[8] = 'T';
    tx_buf[9] = (Sector.Block_0[0] >> 4) + 0x30;
    tx_buf[10] = (Sector.Block_0[0] & 0x0f) + 0x30;
    tx_buf[11] = (Sector.Block_0[1] >> 4) + 0x30;
    tx_buf[12] = (Sector.Block_0[1] & 0x0f) + 0x30;
    tx_buf[13] = (Sector.Block_0[2] >> 4) + 0x30;
    tx_buf[14] = (Sector.Block_0[2] & 0x0f) + 0x30;
    tx_buf[15] = (Sector.Block_0[3] >> 4) + 0x30;
    tx_buf[16] = (Sector.Block_0[3] & 0x0f) + 0x30;
    tx_buf[17] = (Sector.Block_0[4] >> 4) + 0x30;
    tx_buf[18] = (Sector.Block_0[4] & 0x0f) + 0x30;
    tx_buf[19] = (Sector.Block_0[5] >> 4) + 0x30;
    tx_buf[20] = (Sector.Block_0[5] & 0x0f) + 0x30;
    tx_buf[21] = 'A';
    
    address = Sector.Block_0[6];
    address <<= 8;
    address += Sector.Block_0[7];
    Int2StrSized(&tx_buf[22], address, 5);

    tx_buf[27] = Sector.Block_0[8];
    tx_buf[28] = Sector.Block_0[9];
    tx_buf[29] = Sector.Block_0[10];
    tx_buf[30] = Sector.Block_0[11];
    return (MI_OK);
}

void CopyCardData(void)    
{
    tx_buf[0] = 'I';                                    //  1
    tx_buf[1] = aCardSerial[0];                         // 	2
    tx_buf[2] = aCardSerial[1];                         //	3
    tx_buf[3] = aCardSerial[2];                         //	4
    tx_buf[4] = aCardSerial[3];                         //	5
    tx_buf[5] = aCardSerial[4];                         //	6
    /*
     * usage type
     */
    tx_buf[6] = 'J';
    tx_buf[7] = Sector.Block_0[7];                      //  8   jednodnevna ili vremenska
    /*
     * expiry time
     */
    tx_buf[8] = 'T';                                    //  9
    tx_buf[9] = (Sector.Block_0[0] >> 4) + 0x30;        //	10
    tx_buf[10] = (Sector.Block_0[0] & 0x0f) + 0x30;     //	11
    tx_buf[11] = (Sector.Block_0[1] >> 4) + 0x30;       //	12
    tx_buf[12] = (Sector.Block_0[1] & 0x0f) + 0x30;     //	13
    tx_buf[13] = (Sector.Block_0[2] >> 4) + 0x30;       //	14
    tx_buf[14] = (Sector.Block_0[2] & 0x0f) + 0x30;     //	15
    tx_buf[15] = (Sector.Block_0[3] >> 4) + 0x30;       //	16
    tx_buf[16] = (Sector.Block_0[3] & 0x0f) + 0x30;     //	17
    tx_buf[17] = (Sector.Block_0[4] >> 4) + 0x30;       //	18
    tx_buf[18] = (Sector.Block_0[4] & 0x0f) + 0x30;     //	19
    tx_buf[19] = (Sector.Block_0[5] >> 4) + 0x30;       //	20
    tx_buf[20] = (Sector.Block_0[5] & 0x0f) + 0x30;     //	21
    /*
     * system id
     */
    tx_buf[21] = 'A';                                   //	22
    tx_buf[22] = 0x30;                                  //	23
    tx_buf[23] = 0x30;                                  //	24
    tx_buf[24] = 0x30;                                  //	25
    tx_buf[25] = 0x30;                                  //	26
    tx_buf[26] = 0x30;                                  //	27

    address = Sector.Block_0[8];
    address <<= 8;
    address += Sector.Block_0[9];
    Int2StrSized(&tx_buf[22], address, 5);
    /*
     * number of users
     */
    tx_buf[27] = 'U';                                 // 28
    tx_buf[28] = (Sector.Block_0[6] >> 4) + 0x30;     // 29 broj korisnika
    tx_buf[29] = (Sector.Block_0[6] & 0x0f) + 0x30;	// 30 broj korisnika
    /*
     * tag type
     */
    tx_buf[30] = 'C';                                 // 31
    tx_buf[31] = Sector.Block_0[10];                  // 32 tip kartice
    
    tx_buf[32] = 'M';                                 // 33
    tx_buf[33] = 0x30;                                // 34
    tx_buf[34] = 0x30;                                // 35
    tx_buf[35] = 0x30;                                // 36
    tx_buf[36] = 0x30;                                // 37
    tx_buf[37] = 0x30;                                // 38
    tx_buf[38] = 0x30;                                // 39
    tx_buf[39] = 0x30;                                // 40
    tx_buf[40] = 0x30;                                // 41
    tx_buf[41] = 0x30;                                // 42
    tx_buf[42] = 0x30;                                // 43

    address = Sector.Block_1[0];
    address <<= 8;
    address += Sector.Block_1[1];
    address <<= 8;
    address += Sector.Block_1[2];
    address <<= 8;
    address += Sector.Block_1[3];
    Int2StrSized(&tx_buf[33], address, 10);
   
    tx_buf[43] = 'D';                               // 44
    tx_buf[44] = Sector.Block_1[4];	                // 45 vece od nule koristena kartica
    tx_buf[45] = 'L';                               // 46
    tx_buf[46] = 0x30;
    tx_buf[47] = 0x30;
    tx_buf[48] = 0x30;
    tx_buf[49] = 0x30;
    tx_buf[50] = 0x30;
    
    address = Sector.Block_1[5];
    address <<= 8;
    address += Sector.Block_1[6];
    Int2StrSized(&tx_buf[46], address, 5);
}


/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
