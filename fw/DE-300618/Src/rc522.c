/**
 ******************************************************************************
 * File Name          : mfrc522.c
 * Date               : 28/02/2016 23:16:19
 * Description        : mifare RC522 software modul
 ******************************************************************************
 *
 *
 ******************************************************************************
 */

/* Includes ------------------------------------------------------------------*/
#include "rc522.h"


/* Private defines    --------------------------------------------------------*/
#define RC522_DUMMY                         0x00    //Dummy byte
#define RC522_MAX_LEN                       16      // FIFO & buffer byte lenght

/* RC522 Commands */
#define PCD_IDLE                            0x00   //NO action; Cancel the current command
#define PCD_AUTHENT                         0x0E   //Authentication Key
#define PCD_RECEIVE                         0x08   //Receive Data
#define PCD_TRANSMIT                        0x04   //Transmit data
#define PCD_TRANSCEIVE                      0x0C   //Transmit and receive data,
#define PCD_RESETPHASE                      0x0F   //Reset
#define PCD_CALCCRC                         0x03   //CRC Calculate

/* Mifare_One card command word */
#define PICC_REQIDL                         0x26   // find the antenna area does not enter hibernation
#define PICC_REQALL                         0x52   // find all the cards antenna area
#define PICC_ANTICOLL                       0x93   // anti-collision
#define PICC_SELECTTAG                      0x93   // election card
#define PICC_AUTHENT1A                      0x60   // authentication key A
#define PICC_AUTHENT1B                      0x61   // authentication key B
#define PICC_READ                           0x30   // Read Block
#define PICC_WRITE                          0xA0   // write block
#define PICC_DECREMENT                      0xC0   // debit
#define PICC_INCREMENT                      0xC1   // recharge
#define PICC_RESTORE                        0xC2   // transfer block data to the buffer
#define PICC_TRANSFER                       0xB0   // save the data in the buffer
#define PICC_HALT                           0x50   // Sleep

/* RC522 Registers */
//Page 0: Command and Status
#define RC522_REG_RESERVED00			0x00
#define RC522_REG_COMMAND               0x01
#define RC522_REG_COMM_IE_N             0x02
#define RC522_REG_DIV1_EN               0x03
#define RC522_REG_COMM_IRQ              0x04
#define RC522_REG_DIV_IRQ               0x05
#define RC522_REG_ERROR                 0x06
#define RC522_REG_STATUS1               0x07
#define RC522_REG_STATUS2               0x08
#define RC522_REG_FIFO_DATA             0x09
#define RC522_REG_FIFO_LEVEL			0x0A
#define RC522_REG_WATER_LEVEL			0x0B
#define RC522_REG_CONTROL               0x0C
#define RC522_REG_BIT_FRAMING			0x0D
#define RC522_REG_COLL                  0x0E
#define RC522_REG_RESERVED01			0x0F
//Page 1: Command
#define RC522_REG_RESERVED10			0x10
#define RC522_REG_MODE                  0x11
#define RC522_REG_TX_MODE               0x12
#define RC522_REG_RX_MODE               0x13
#define RC522_REG_TX_CONTROL			0x14
#define RC522_REG_TX_AUTO               0x15
#define RC522_REG_TX_SELL               0x16
#define RC522_REG_RX_SELL               0x17
#define RC522_REG_RX_THRESHOLD			0x18
#define RC522_REG_DEMOD                 0x19
#define RC522_REG_RESERVED11			0x1A
#define RC522_REG_RESERVED12			0x1B
#define RC522_REG_MIFARE                0x1C
#define RC522_REG_RESERVED13			0x1D
#define RC522_REG_RESERVED14			0x1E
#define RC522_REG_SERIALSPEED			0x1F
//Page 2: CFG
#define RC522_REG_RESERVED20			0x20
#define RC522_REG_CRC_RESULT_M			0x21
#define RC522_REG_CRC_RESULT_L			0x22
#define RC522_REG_RESERVED21			0x23
#define RC522_REG_MOD_WIDTH             0x24
#define RC522_REG_RESERVED22			0x25
#define RC522_REG_RF_CFG                0x26
#define RC522_REG_GS_N                  0x27
#define RC522_REG_CWGS_PREG             0x28
#define RC522_REG__MODGS_PREG			0x29
#define RC522_REG_T_MODE                0x2A
#define RC522_REG_T_PRESCALER			0x2B
#define RC522_REG_T_RELOAD_H			0x2C
#define RC522_REG_T_RELOAD_L			0x2D
#define RC522_REG_T_COUNTER_VALUE_H		0x2E
#define RC522_REG_T_COUNTER_VALUE_L		0x2F
//Page 3:TestRegister
#define RC522_REG_RESERVED30			0x30
#define RC522_REG_TEST_SEL1             0x31
#define RC522_REG_TEST_SEL2             0x32
#define RC522_REG_TEST_PIN_EN			0x33
#define RC522_REG_TEST_PIN_VALUE		0x34
#define RC522_REG_TEST_BUS              0x35
#define RC522_REG_AUTO_TEST             0x36
#define RC522_REG_VERSION               0x37
#define RC522_REG_ANALOG_TEST			0x38
#define RC522_REG_TEST_ADC1             0x39
#define RC522_REG_TEST_ADC2             0x3A
#define RC522_REG_TEST_ADC0             0x3B
#define RC522_REG_RESERVED31			0x3C
#define RC522_REG_RESERVED32			0x3D
#define RC522_REG_RESERVED33			0x3E
#define RC522_REG_RESERVED34			0x3F



/* Private types  -----------------------------------------------------------*/
/* Private variables  --------------------------------------------------------*/
uint8_t rc522_rx_buf[RC522_MAX_LEN];
uint8_t rc522_tx_buf[RC522_MAX_LEN];
uint8_t aMifareKeyA[6] = {0xff,0xff,0xff,0xff,0xff,0xff}; 
uint8_t str[RC522_MAX_LEN];
uint8_t aCardID[5]; //Recognized card ID
uint8_t aCardSerial[5];
uint8_t rc522_sector;
uint8_t rc522_command;

RC522_SectorTypeDef Sector;


extern SPI_HandleTypeDef hspi1;

/* Private macros   ----------------------------------------------------------*/
#define mRC522_ChipSelect()     (HAL_GPIO_WritePin(GPIOA, GPIO_PIN_3, GPIO_PIN_RESET))
#define mRC522_ChipRelease()    (HAL_GPIO_WritePin(GPIOA, GPIO_PIN_3, GPIO_PIN_SET))
#define mRC522_ResetAssert()    (HAL_GPIO_WritePin(GPIOA, GPIO_PIN_4, GPIO_PIN_RESET))
#define mRC522_ResetRelease()   (HAL_GPIO_WritePin(GPIOA, GPIO_PIN_4, GPIO_PIN_SET))


/* Private prototypes    -----------------------------------------------------*/
RC522_StatusTypeDef RC522_Check(uint8_t* id);
RC522_StatusTypeDef RC522_Compare(uint8_t* aCardID, uint8_t* CompareID);
void RC522_WriteRegister(uint8_t addr, uint8_t val);
uint8_t RC522_ReadRegister(uint8_t addr);
void RC522_SetBitMask(uint8_t reg, uint8_t mask);
void RC522_ClearBitMask(uint8_t reg, uint8_t mask);
void RC522_AntennaOn(void);
void RC522_AntennaOff(void);
RC522_StatusTypeDef RC522_Reset(void);
RC522_StatusTypeDef RC522_Request(uint8_t reqMode, uint8_t* TagType);
RC522_StatusTypeDef RC522_ToCard(uint8_t command, uint8_t* sendData, uint8_t sendLen, uint8_t* backData, uint32_t* backLen);
RC522_StatusTypeDef RC522_Anticoll(uint8_t* serNum);
void RC522_CalculateCRC(uint8_t* pIndata, uint8_t len, uint8_t* pOutData);
RC522_StatusTypeDef RC522_SelectTag(uint8_t* serNum);
RC522_StatusTypeDef RC522_Auth(uint8_t authMode, uint8_t BlockAddr, uint8_t* Sectorkey, uint8_t* serNum);
RC522_StatusTypeDef RC522_Read(uint8_t blockAddr, uint8_t* recvData);
RC522_StatusTypeDef RC522_Write(uint8_t blockAddr, uint8_t* writeData);
void RC522_Halt(void);
RC522_StatusTypeDef RC522_ReadCard(void);
RC522_StatusTypeDef RC522_WriteCard(void);
void RC522_ClearData(void);


/* Program code   ------------------------------------------------------------*/
void RC522_Init(void)
{
    rc522_command = RC522_IDLE;
    mRC522_ChipRelease();
    mRC522_ResetAssert();
    HAL_Delay(10);
    mRC522_ResetRelease();
    HAL_Delay(100);
    RC522_Reset();
    RC522_WriteRegister(RC522_REG_T_MODE, 0x8D);
    RC522_WriteRegister(RC522_REG_T_PRESCALER, 0x3E);
    RC522_WriteRegister(RC522_REG_T_RELOAD_L, 30);
    RC522_WriteRegister(RC522_REG_T_RELOAD_H, 0);
    RC522_WriteRegister(RC522_REG_RF_CFG, 0x70);
    RC522_WriteRegister(RC522_REG_TX_AUTO, 0x40);
    RC522_WriteRegister(RC522_REG_MODE, 0x3D);
    RC522_AntennaOn(); // Open the antenna
}


RC522_StatusTypeDef RC522_Service(void)
{	
    RC522_StatusTypeDef status = MI_OK;
    
	status += RC522_Check(aCardSerial);
    
    if(status == MI_OK)
    {
        switch(rc522_command) 
        {
            case RC522_READ:
                status += RC522_ReadCard();
                break;
            
            case RC522_WRITE:
                status += RC522_WriteCard();
                break;
                
            default:
                RC522_ClearData();
                status = MI_ERR;
                break;
        }   
    }
    
	RC522_Reset();
	RC522_WriteRegister(RC522_REG_T_MODE, 0x8D);
	RC522_WriteRegister(RC522_REG_T_PRESCALER, 0x3E);
	RC522_WriteRegister(RC522_REG_T_RELOAD_L, 30);           
	RC522_WriteRegister(RC522_REG_T_RELOAD_H, 0);
	RC522_WriteRegister(RC522_REG_TX_AUTO, 0x40);
	RC522_WriteRegister(RC522_REG_MODE, 0x3D);
	RC522_AntennaOn();// Open the antenna
    return(status);
}

RC522_StatusTypeDef RC522_Check(uint8_t* id) 
{
	RC522_StatusTypeDef status = MI_OK;
    
	status += RC522_Request(PICC_REQIDL, id);	
    
	if (status == MI_OK) 
    {                                  
		status += RC522_Anticoll(id);
	}
    
	RC522_Halt();
	return (status);
}

RC522_StatusTypeDef RC522_Compare(uint8_t* aCardID, uint8_t* CompareID) 
{
    
	uint8_t i;
    
	for (i = 0; i < 5; i++) {
        
		if (aCardID[i] != CompareID[i]) {
            
			return (MI_ERR);
            
		}
	}
    
	return (MI_OK);
}


void RC522_WriteRegister(uint8_t addr, uint8_t val) 
{
    mRC522_ChipSelect();
    rc522_tx_buf[0] = (addr << 1) & 0x7E;
    rc522_tx_buf[1] = val;
    HAL_SPI_Transmit(&hspi1, rc522_tx_buf, 2, 10);
    mRC522_ChipRelease();
}


uint8_t RC522_ReadRegister(uint8_t addr) 
{
    mRC522_ChipSelect();
    rc522_tx_buf[0]  = ((addr << 1) & 0x7E) | 0x80;
    rc522_tx_buf[1]  = 0x00;
    HAL_SPI_TransmitReceive(&hspi1, rc522_tx_buf, rc522_rx_buf, 2, 10);
    mRC522_ChipRelease();
    return (rc522_rx_buf[1]);
}

void RC522_SetBitMask(uint8_t reg, uint8_t mask) 
{   
	RC522_WriteRegister(reg, RC522_ReadRegister(reg) | mask);
    
}

void RC522_ClearBitMask(uint8_t reg, uint8_t mask)
{
	RC522_WriteRegister(reg, RC522_ReadRegister(reg) & (~mask));
}

void RC522_AntennaOn(void) 
{
    
	uint8_t temp;

	temp = RC522_ReadRegister(RC522_REG_TX_CONTROL);
    
	if (!(temp & 0x03)) {
        
		RC522_SetBitMask(RC522_REG_TX_CONTROL, 0x03);
        
	}// End of if...
    
}// End of antena on function

void RC522_AntennaOff(void) 
{   
	RC522_ClearBitMask(RC522_REG_TX_CONTROL, 0x03);
    
}// End of antena off function

RC522_StatusTypeDef RC522_Reset(void) 
{
    uint8_t delay = 0;
    /**
    *   Issue the SoftReset command.
    */
	RC522_WriteRegister(RC522_REG_COMMAND, PCD_RESETPHASE);
    /**
    *   The datasheet does not mention how long the SoftRest command takes to complete.
    *   But the RC522 might have been in soft power-down mode (triggered by bit 4 of CommandReg)
    *   Section 8.8.2 in the datasheet says the oscillator start-up time is the start up time of the crystal + 37,74us. Let us be generous: 50ms.
    */
    /**
    *   Wait for the PowerDown bit in CommandReg to be cleared
    */
    while (RC522_ReadRegister(RC522_REG_COMMAND) & (1<<4))
    {
        /**
        *   RC522 still restarting - unlikely after waiting 50ms and more
        *   mifare modul is unresponsive so return error status
        */
        HAL_Delay(1);
        
        if(++delay == 100) 
        {
            return (MI_ERR);
        }
    }
    /**
    *   reset finished - return OK flag
    */
    return (MI_OK);
    
}// End of software reset function

RC522_StatusTypeDef RC522_Request(uint8_t reqMode, uint8_t* TagType) 
{
    
	RC522_StatusTypeDef status;  
	uint32_t backBits;			                            //The received data bits

	RC522_WriteRegister(RC522_REG_BIT_FRAMING, 0x07);	// TxLastBits = BitFramingReg[2..0]	???

	TagType[0] = reqMode;
	status = RC522_ToCard(PCD_TRANSCEIVE, TagType, 1, TagType, &backBits);

	if ((status != MI_OK) || (backBits != 0x10)) 
    {
		status = MI_ERR;
	}

	return (status);
}

RC522_StatusTypeDef RC522_ToCard(uint8_t command, uint8_t* sendData, uint8_t sendLen, uint8_t* backData, uint32_t* backLen) 
{
	
    RC522_StatusTypeDef status = MI_OK;
	uint8_t irqEn = 0x00;
	uint8_t waitIRq = 0x00;
	uint8_t lastBits;
	uint8_t n;
	uint32_t i;
  

	switch (command) 
    {
        
		case PCD_AUTHENT:
        {
			irqEn = 0x12;
			waitIRq = 0x10;
			break;
		}
        
		case PCD_TRANSCEIVE:             
        {
			irqEn = 0x77;
			waitIRq = 0x30;
			break;
		}
        
		default:            
        {
            break;
        
        }
	}

	RC522_WriteRegister(RC522_REG_COMM_IE_N, irqEn|0x80);
	RC522_ClearBitMask(RC522_REG_COMM_IRQ, 0x80);
	RC522_SetBitMask(RC522_REG_FIFO_LEVEL, 0x80);
	RC522_WriteRegister(RC522_REG_COMMAND, PCD_IDLE);

	//Writing data to the FIFO
	for (i = 0; i < sendLen; i++) {   
        
		RC522_WriteRegister(RC522_REG_FIFO_DATA, sendData[i]);   
        
	}// End of for loop

	//Execute the command
	RC522_WriteRegister(RC522_REG_COMMAND, command);
    
	if (command == PCD_TRANSCEIVE) {    
        
		RC522_SetBitMask(RC522_REG_BIT_FRAMING, 0x80);  //StartSend=1,transmission of data starts  
	
    }// End of if... 
    /**
    *   Waiting to receive data to complete
    */
	i = 2000;	//i according to the clock frequency adjustment, the operator M1 card maximum waiting time 25ms???
	do {
        /**
        *   CommIrqReg[7..0]
        *   Set1 TxIRq RxIRq IdleIRq HiAlerIRq LoAlertIRq ErrIRq TimerIRq
        */
		n = RC522_ReadRegister(RC522_REG_COMM_IRQ);
		i--;
        
	} while ((i != 0) && !(n & 0x01) && !(n&waitIRq));          // End of do...while loop              
    /**
    *   StartSend=0
    */
	RC522_ClearBitMask(RC522_REG_BIT_FRAMING, 0x80);

	if (i != 0)  {
        
		if (!(RC522_ReadRegister(RC522_REG_ERROR) & 0x1B)) {
            
			status = MI_OK;
            
			if (n & irqEn & 0x01) {
                
				status = MI_NOTAGERR;
                
			}// End of if...

			if (command == PCD_TRANSCEIVE) {
                
				n = RC522_ReadRegister(RC522_REG_FIFO_LEVEL);
				lastBits = RC522_ReadRegister(RC522_REG_CONTROL) & 0x07;
                
				if (lastBits) *backLen = (n - 1)*8 + lastBits;  
                else *backLen = n * 8;  

				if (n == 0) n = 1;
                
				if (n > RC522_MAX_LEN) n = RC522_MAX_LEN;   
				/**
                *   Reading the received data in FIFO
                */
				for (i = 0; i < n; i++) {
                    
					backData[i] = RC522_ReadRegister(RC522_REG_FIFO_DATA);
                    
				}// End of for loop
			}// End of if (command == PCD_TRANSCEIVE)
            
		} else {
            
			status = MI_ERR;
            
		}// End of else
	}// End of if (i != 0)

	return (status);
    
}// End of to card function

RC522_StatusTypeDef RC522_Anticoll(uint8_t* serNum) 
{
    
	RC522_StatusTypeDef status;
	uint8_t i;
	uint8_t serNumCheck = 0;
	uint32_t unLen;

	RC522_WriteRegister(RC522_REG_BIT_FRAMING, 0x00);   // TxLastBists = BitFramingReg[2..0]

	serNum[0] = PICC_ANTICOLL;
	serNum[1] = 0x20;
	status = RC522_ToCard(PCD_TRANSCEIVE, serNum, 2, serNum, &unLen);

	if (status == MI_OK) {
		/**
        *   Check card serial number
        */
		for (i = 0; i < 4; i++) {
            
			serNumCheck ^= serNum[i];
            
		}// End of for loop....
        
		if (serNumCheck != serNum[i]) {
            
			status = MI_ERR;
            
		}// End of if...
	}// End of if (status == MI_OK)
    
	return (status);
    
}// End of anticollision function

void RC522_CalculateCRC(uint8_t*  pIndata, uint8_t len, uint8_t* pOutData)
{
    
	uint8_t i, n;

	RC522_ClearBitMask(RC522_REG_DIV_IRQ, 0x04);			//CRCIrq = 0
	RC522_SetBitMask(RC522_REG_FIFO_LEVEL, 0x80);			//Clear the FIFO pointer
	/**
    *   Write_RC522(CommandReg, PCD_IDLE);
    *   Writing data to the FIFO
    */
	for (i = 0; i < len; i++) {
        
		RC522_WriteRegister(RC522_REG_FIFO_DATA, *(pIndata+i)); 
        
	}// End of for loop...
    
	RC522_WriteRegister(RC522_REG_COMMAND, PCD_CALCCRC);

	//Wait CRC calculation is complete
	i = 0xFF;
	do {
		n = RC522_ReadRegister(RC522_REG_DIV_IRQ);
		i--;
	} while ((i!=0) && !(n&0x04));			//CRCIrq = 1

	//Read CRC calculation result
	pOutData[0] = RC522_ReadRegister(RC522_REG_CRC_RESULT_L);
	pOutData[1] = RC522_ReadRegister(RC522_REG_CRC_RESULT_M);
    
}// End of calculate CRC function

RC522_StatusTypeDef RC522_SelectTag(uint8_t* serNum) 
{
    RC522_StatusTypeDef status = MI_OK;
	uint8_t i;
	uint32_t recvBits;
	uint8_t buffer[9]; 

	buffer[0] = PICC_SELECTTAG;
	buffer[1] = 0x70;
    
	for (i = 0; i < 5; i++) 
    {
		buffer[i+2] = *(serNum+i);
	}
    
	RC522_CalculateCRC(buffer, 7, &buffer[7]);		//??
	status += RC522_ToCard(PCD_TRANSCEIVE, buffer, 9, buffer, &recvBits);
	return (status);
}

RC522_StatusTypeDef RC522_Auth(uint8_t authMode, uint8_t BlockAddr, uint8_t* Sectorkey, uint8_t* serNum) 
{
	
    RC522_StatusTypeDef status = MI_OK;
	uint32_t recvBits;
	uint8_t i;
	uint8_t buff[12]; 

	//Verify the command block address + sector + password + card serial number
	buff[0] = authMode;
	buff[1] = BlockAddr;
    
	for (i = 0; i < 6; i++) 
    { 
		buff[i + 2] = *(Sectorkey + i); 
	}
    
	for (i = 0; i < 4; i++) 
    {
		buff[i + 8] = *(serNum + i);
	}
    
	status = RC522_ToCard(PCD_AUTHENT, buff, 12, buff, &recvBits);

	if ((status != MI_OK) || (!(RC522_ReadRegister(RC522_REG_STATUS2) & 0x08))) 
    {
		status = MI_ERR;
	}

	return (status);
    
}// End of auth function

RC522_StatusTypeDef RC522_Read(uint8_t blockAddr, uint8_t* recvData) 
{
	RC522_StatusTypeDef status = MI_OK;
	uint32_t unLen;

	recvData[0] = PICC_READ;
	recvData[1] = blockAddr;
	RC522_CalculateCRC(recvData, 2, &recvData[2]);
	status += RC522_ToCard(PCD_TRANSCEIVE, recvData, 4, recvData, &unLen);

	if ((status != MI_OK) || (unLen != 0x90)) {
        
		status = MI_ERR;
        
	}// End of if...

	return (status);
    
}// End of read function

RC522_StatusTypeDef RC522_Write(uint8_t blockAddr, uint8_t* writeData) 
{

    RC522_StatusTypeDef status;
    uint32_t recvBits;
    uint8_t i;
    uint8_t buff[18];

	buff[0] = PICC_WRITE;
	buff[1] = blockAddr;
	RC522_CalculateCRC(buff, 2, &buff[2]);
	status = RC522_ToCard(PCD_TRANSCEIVE, buff, 4, buff, &recvBits);

	if ((status != MI_OK) || (recvBits != 4) || ((buff[0] & 0x0f) != 0x0a)) {   
        
		status = MI_ERR;   
        
	}// End of if...

	if (status == MI_OK) {
		/**
        *   Data to the FIFO write 16Byte
        */
		for (i = 0; i < 16; i++) {  
            
			buff[i] = *(writeData+i);
            
		}// End of for loop..
        
		RC522_CalculateCRC(buff, 16, &buff[16]);
		status = RC522_ToCard(PCD_TRANSCEIVE, buff, 18, buff, &recvBits);

		if ((status != MI_OK) || (recvBits != 4) || ((buff[0] & 0x0f) != 0x0a)) { 
            
			status = MI_ERR;   
            
		}// End of if...
	}// End of if (status == MI_OK)

	return (status);
    
}// End of write function

void RC522_Halt(void) 
{
    
	uint32_t unLen;
	uint8_t buff[4]; 

	buff[0] = PICC_HALT;
	buff[1] = 0;
	RC522_CalculateCRC(buff, 2, &buff[2]);
	RC522_ToCard(PCD_TRANSCEIVE, buff, 4, buff, &unLen);
    
}


RC522_StatusTypeDef RC522_ReadCard(void)
{
    RC522_StatusTypeDef status = MI_OK;
    
    status += RC522_Request(PICC_REQIDL, str);	
    status += RC522_Anticoll(str);

    for(uint8_t i = 0; i < 5; i++)
    {
        aCardID[i] = str[i];
    }

    status += RC522_SelectTag(aCardID);	
    status += RC522_Auth(PICC_AUTHENT1A, rc522_sector, aMifareKeyA, aCardID);

    if(status == MI_OK)
    {
        status += RC522_Read(rc522_sector, &Sector.Block_0[0]);
        status += RC522_Read(rc522_sector + 1, &Sector.Block_1[0]);
        status += RC522_Read(rc522_sector + 2, &Sector.Block_2[0]);
    }

    RC522_Halt();
    return(status);
}


RC522_StatusTypeDef RC522_WriteCard(void)
{
    RC522_StatusTypeDef status = MI_OK;
    
    status += RC522_Request(PICC_REQIDL, str);	
    status += RC522_Anticoll(str);
	
    for(uint8_t i = 0; i < 5; i++)
    {
        aCardID[i]=str[i];
    }

    status += RC522_SelectTag(aCardID);           
    status += RC522_Auth(PICC_AUTHENT1A, rc522_sector, aMifareKeyA, aCardID);

    if (status == MI_OK)
    {
        if (rc522_sector != SECTOR_0) status += RC522_Write(rc522_sector, Sector.Block_0);
        status += RC522_Write(rc522_sector + 1, Sector.Block_1);
        status += RC522_Write(rc522_sector + 2, Sector.Block_2);
    }

    RC522_Halt();
    return(status);
            
}

void RC522_ClearData(void)
{
    uint8_t i;

    aCardSerial[0] = 0;
    aCardSerial[1] = 0;
    aCardSerial[2] = 0;
    aCardSerial[3] = 0;
    aCardSerial[4] = 0;
    
    
    for (i = 0; i < 16; i++)
    {
        Sector.Block_0[i] = 0;
        Sector.Block_1[i] = 0;
        Sector.Block_2[i] = 0;
    }
}
