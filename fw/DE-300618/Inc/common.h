/**
 ******************************************************************************
 * File Name          : common.h
 * Date               : 10.3.2018
 * Description        : usefull function set and program shared constants
 ******************************************************************************
 */


/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __COMMON_H
#define __COMMON_H


/* Include  ------------------------------------------------------------------*/
#include "stm32f1xx.h"


/* Exported Type  ------------------------------------------------------------*/
/* Exported Define  ----------------------------------------------------------*/
/** ==========================================================================*/
/**		G P I O			P O R T		&		P I N			A L I A S		  */
/** ==========================================================================*/

/** ==========================================================================*/
/**		S T M 3 2 F 1 0 3 C 8		F L A S H		A D D R E S S E			  */
/** ==========================================================================*/
#define BOOTLOADER_ADDRESS			((uint32_t)0x08000000)
#define BOOTLOADER_END_ADDRESS		((uint32_t)0x08003000)
#define APPLICATION_ADDRESS 		((uint32_t)0x08003000)
#define USER_FLASH_END_ADDRESS		((uint32_t)0x08010000)
#define BOOTLOADER_FLASH_SIZE		((uint32_t)0x00003000)
#define USER_FLASH_SIZE      		((uint32_t)0x0000D000)
/* Define bitmap representing user flash area that could be write protected (check restricted to pages 16-63 = app_code). */
#define FLASH_PAGE_TO_BE_PROTECTED (OB_WRP_PAGES16TO19 	| OB_WRP_PAGES20TO23 | OB_WRP_PAGES24TO27 | OB_WRP_PAGES28TO31 | \
                                    OB_WRP_PAGES32TO35 	| OB_WRP_PAGES36TO39 | OB_WRP_PAGES40TO43 | OB_WRP_PAGES44TO47 | \
                                    OB_WRP_PAGES48TO51 	| OB_WRP_PAGES52TO55 | OB_WRP_PAGES56TO59 | OB_WRP_PAGES60TO63)  
/** ==========================================================================*/
/**		B O O T L O A D E R		P R E D E F I N E D		C O M M A N D		  */
/** ==========================================================================*/
#define	BOOTLOADER_CMD_RUN					'W'
#define	BOOTLOADER_STATUS_UPDATE_FAIL		'O'
#define	BOOTLOADER_STATUS_UPDATE_SUCCESS	'N'


/* Exported Variable   -------------------------------------------------------*/
/* Exported Macro ------------------------------------------------------------*/
#define IS_CAP_LETTER(c)    	(((c) >= 'A') && ((c) <= 'F'))
#define IS_LC_LETTER(c)     	(((c) >= 'a') && ((c) <= 'f'))
#define IS_09(c)            	(((c) >= '0') && ((c) <= '9'))
#define ISVALIDHEX(c)       	(IS_CAP_LETTER(c) || IS_LC_LETTER(c) || IS_09(c))
#define ISVALIDDEC(c)     		IS_09(c)
#define CONVERTDEC(c)       	(c - '0')
#define CONVERTHEX_ALPHA(c) 	(IS_CAP_LETTER(c) ? ((c) - 'A'+10) : ((c) - 'a'+10))
#define CONVERTHEX(c)       	(IS_09(c) ? ((c) - '0') : CONVERTHEX_ALPHA(c))


/* Exported function  ------------------------------------------------------- */
void Delay(__IO uint32_t nCount);
int GetLenght(char * p);
uint8_t Bcd2Dec(uint8_t bcd);
uint8_t Dec2Bcd(uint8_t dec);
uint8_t CalcCRC(uint8_t *buff, uint8_t size);
void Int2Str(uint8_t *p_str, uint32_t intnum);
void Int2StrSized(uint8_t *p_str, uint32_t intnum, uint8_t size);
void ClearBuffer(uint8_t *buffer, uint16_t size);
void ClearBuffer16(uint16_t * buffer, uint16_t size);
uint32_t Str2Int(uint8_t *p_str);
uint32_t Str2IntSized(uint8_t *p_str, uint8_t size);
uint16_t CalcChecksum(const uint8_t *p_data, uint32_t size);
void Str2Hex(uint8_t *p_str, uint16_t lenght, uint8_t *p_hex);
void Hex2Str(uint8_t *p_hex, uint16_t lenght, uint8_t *p_str);

void CharToBin(unsigned char c, char *out);

#endif  /* __COMMON_H */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
