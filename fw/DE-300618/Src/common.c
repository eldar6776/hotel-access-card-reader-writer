/**
 ******************************************************************************
 * File Name          : common.c
 * Date               : 10.3.2018
 * Description        : usefull function set
 ******************************************************************************
 *
 *
 ******************************************************************************
 */
 

/* Include  ------------------------------------------------------------------*/
#include "common.h"


/* Imported Type  ------------------------------------------------------------*/
/* Imported Variable  --------------------------------------------------------*/
uint16_t calc;
uint8_t res;


/* Imported Function  --------------------------------------------------------*/
/* Private Type --------------------------------------------------------------*/
/* Private Define ------------------------------------------------------------*/
/* Private Variable ----------------------------------------------------------*/
/* Private Macro -------------------------------------------------------------*/
/* Private Function Prototype ------------------------------------------------*/
/* Program Code  -------------------------------------------------------------*/
void Delay(__IO uint32_t nCount)
{
	__IO uint32_t index = 0;
	
	for(index = nCount; index != 0; index--)
	{
	}
}


int GetLenght(char * p) 
{
	for (calc = 0; *p++; calc++);
	return calc;
}


uint8_t Bcd2Dec(uint8_t bcd)
{
	return(((bcd >> 4) * 10) + (bcd & 0x0f));
}


uint8_t Dec2Bcd(uint8_t dec)
{
	res = 0;
	while(dec > 9)
	{
		dec -= 10;
		res++;
	}

	res = res << 4;
	res += dec;
	return (res);
}


uint8_t CalcCRC(uint8_t *buff, uint8_t size)
{
	calc = 0;

	while(size)
	{
		calc += *buff++;
		size--;
	}

	res = (((~calc) & 0xff) + 1);
	return(res);
}

void Int2Str(uint8_t *p_str, uint32_t intnum)
{
	uint32_t i, divider = 1000000000, pos = 0, status = 0;

	for (i = 0; i < 10; i++)
	{
		p_str[pos++] = (intnum / divider) + 48;

		intnum = intnum % divider;
		divider /= 10;
		
		if ((p_str[pos-1] == '0') & (status == 0))
		{
			pos = 0;
		}
		else
		{
			status++;
		}
	}
}


void Int2StrSized(uint8_t *p_str, uint32_t intnum, uint8_t size)
{
	uint8_t result[16];
	uint32_t i, divider = 1000000000, pos = 0;

	ClearBuffer(result, 16);
	
	for (i = 0; i < 10; i++)
	{
		result[pos++] = (intnum / divider) + 48;
		intnum = intnum % divider;
		divider /= 10;
	}
	
	pos -= 1;
	
	while(size && pos)
	{
		p_str[size - 1] = result[pos];
		size--;
		pos--;
	}
}


void Str2Hex(uint8_t *p_str, uint16_t lenght, uint8_t *p_hex)
{
	uint32_t hex_cnt = 0, str_cnt = 0;
	
	while(lenght)
	{
		if((p_str[str_cnt] - 48) > 9) p_hex[hex_cnt] = CONVERTHEX_ALPHA(p_str[str_cnt]) << 4;
		else p_hex[hex_cnt] = CONVERTDEC(p_str[str_cnt]) << 4;
		++str_cnt;
		if((p_str[str_cnt] - 48) > 9) p_hex[hex_cnt] += CONVERTHEX_ALPHA(p_str[str_cnt]);
		else p_hex[hex_cnt] += CONVERTDEC(p_str[str_cnt]);
		++str_cnt;
		++hex_cnt;
		--lenght;
	}
}

void Hex2Str(uint8_t *p_hex, uint16_t lenght, uint8_t *p_str)
{
	uint32_t hex_cnt = 0, str_cnt = 0;
	
	while(lenght)
	{
		if((p_hex[hex_cnt] >> 4) > 9) p_str[str_cnt] = (p_hex[hex_cnt] >> 4) + 55; // convert to upper ascii letter
		else p_str[str_cnt] = (p_hex[hex_cnt] >> 4) + 48;
		++str_cnt;
		if((p_hex[hex_cnt] & 0x0f) > 9) p_str[str_cnt] = (p_hex[hex_cnt] & 0x0f) + 55; // convert to upper ascii letter
		else p_str[str_cnt]  = (p_hex[hex_cnt]& 0x0f) + 48;
		++str_cnt;
		++hex_cnt;
		--lenght;
	}
}	
void ClearBuffer(uint8_t * buffer, uint16_t size)
{
	calc = 0;	
	while(calc < size) buffer[calc++] = 0x00;
}


void ClearBuffer16(uint16_t * buffer, uint16_t size)
{
	calc = 0;	
	while(calc < size) buffer[calc++] = 0x0000;
}


uint32_t Str2Int(uint8_t *p_str)
{
    uint32_t val = 0;

    if (ISVALIDDEC(*p_str)) val = CONVERTDEC(*p_str++);
    else return (val);

    while (ISVALIDDEC(*p_str))
    {
        val *= 10;
        val += CONVERTDEC(*p_str++);
    }
    
    return (val);
}


uint32_t Str2IntSized(uint8_t *p_str, uint8_t size)
{
    uint32_t val = 0;
    uint32_t len = 0;

    while((len < size) && (ISVALIDDEC(*p_str)))
    {
        if (len == 0) val = CONVERTDEC(*p_str++);
        else 
        {
            val *= 10;
            val += CONVERTDEC(*p_str++);
        }
        
        ++len;
    }
    
    return (val);
}


void CharToBin(unsigned char c, char *out) 
{
	*(unsigned long long*)out = 3472328296227680304ULL +
    (((c * 9241421688590303745ULL) / 128) & 72340172838076673ULL);
}


/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
