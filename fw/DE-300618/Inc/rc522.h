/**
 ******************************************************************************
 * File Name          : rc522.h
 * Date               : 08/05/2016 23:15:16
 * Description        : mifare RC522 modul header
 ******************************************************************************
 *
 *
 ******************************************************************************
 */
#ifndef RC522_H
#define RC522_H                             200	// version 1.00

/* Includes ------------------------------------------------------------------*/
#include "stm32f1xx.h"

#define CARD_PENDING                        (0x00)
#define CARD_VALID                          (0x06)
#define CARD_INVALID                        (0x15)
#define CARD_DATA_FORMATED                  (0x7f)
/**
*--------------      card user groups predefine     ---------------------
*/
#define CARD_USER_GROUP_GUEST				('G')
#define CARD_USER_GROUP_HANDMAID			('H')
#define CARD_USER_GROUP_MANAGER				('M')
#define CARD_USER_GROUP_SERVICE				('S')
#define CARD_USER_GROUP_PRESET				('P')
#define CARD_USER_GROUP_KINDERGARDEN		('K')
#define CARD_USER_GROUP_POOL				('B')
#define CARD_USER_GROUP_PARKING				('R')
#define CARD_USER_GROUP_SAUNA				('H')
/**
*--------------      card type predefine     ---------------------
*/
#define CARD_TYPE_ONE_TIME					('O')
#define CARD_TYPE_MULTI						('E')
#define CARD_TYPE_PRESET					('F')
/**
*--------------      rfid tag type predefine     ---------------------
*/
#define CARD_TAG_TYPE_KEY_RING				('Q')
#define CARD_TAG_TYPE_CLASIC				('C')
#define CARD_TAG_TYPE_WRIST					('W')
/**
*--------------      card data define    ---------------------
*/
#define CARD_ID_INVALID                     ('!')
#define CARD_ID_INVALID_DATA                ('"')
#define CARD_USER_GROUP_INVALID             ('?')	
#define CARD_USER_GROUP_DATA_INVALID        ('$')
#define CARD_EXPIRY_TIME_INVALID            ('%')
#define CARD_EXPIRY_TIME_DATA_INVALID       ('&')
#define CARD_USAGE_TYPE_INVALID             ('/')
#define CARD_USAGE_TYPE_DATA_INVALID        ('(')
#define CARD_NUMBER_OF_USERS_INVALID        (')')
#define CARD_NUMBER_OF_USERS_DATA_INVALID   ('=')

/* mifare card memory sectors offset */
#define SECTOR_0                            0x00
#define SECTOR_1                            0x04
#define SECTOR_2                            0x08
#define SECTOR_3                            0x0c
#define SECTOR_4                            0x10
#define SECTOR_5                            0x14
#define SECTOR_6                            0x18
#define SECTOR_7                            0x1c
#define SECTOR_8                            0x20
#define SECTOR_9                            0x24
#define SECTOR_10                           0x28
#define SECTOR_11                           0x2c
#define SECTOR_12                           0x30
#define SECTOR_13                           0x34
#define SECTOR_14                           0x38
#define SECTOR_15                           0x3c
#define SECTOR_16                           0x40
#define SECTOR_17                           0x44
#define SECTOR_18                           0x48
#define SECTOR_19                           0x4c
#define SECTOR_20                           0x50
#define SECTOR_21                           0x54
#define SECTOR_22                           0x58
#define SECTOR_23                           0x5c
#define SECTOR_24                           0x60
#define SECTOR_25                           0x64
#define SECTOR_26                           0x6c
#define SECTOR_27                           0x70
#define SECTOR_28                           0x74
#define SECTOR_29                           0x78
#define SECTOR_30                           0x7c
#define SECTOR_31                           0x80

#define RC522_IDLE                          0x00
#define RC522_READ                          0x01
#define RC522_WRITE                         0x02
#define RC522_WRITE_KEY                     0x03

/* Exported types    ---------------------------------------------------------*/
typedef enum
{
	MI_OK		= 0x00,
	MI_ERR 		= 0x01,
	MI_NOTAGERR	= 0x02

} RC522_StatusTypeDef;




typedef struct
{
    unsigned char   Block_0[16];
    unsigned char   Block_1[16];
    unsigned char   Block_2[16];
    
} RC522_SectorTypeDef;



/* Exported variables  -------------------------------------------------------*/
extern uint8_t rc522_sector;
extern uint8_t rc522_command;
extern uint8_t aCardSerial[5];
extern RC522_SectorTypeDef Sector;


/* Exported macros     -------------------------------------------------------*/
/* Exported functions  -------------------------------------------------------*/
void RC522_Init(void);
RC522_StatusTypeDef RC522_Service(void);
void RC522_ClearData(void);

#endif
