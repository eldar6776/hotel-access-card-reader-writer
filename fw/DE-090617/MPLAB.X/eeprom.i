#line 1 "../Src/eeprom.c"
#line 1 "../Src/eeprom.c"

#line 24 "../Src/eeprom.c"
 






 


 


 


 


 


 


 














unsigned char EEPROM_ReadByte (unsigned char address) {
    EECON1 = 0;
    EEADR = address;
    EECON1bits.RD = 1;
    return (EEDATA);
}



unsigned int EEPROM_ReadInt (unsigned char address) {
    volatile unsigned int data;
    data = EEPROM_ReadByte (address);
    data = (data << 8);
    data += EEPROM_ReadByte (address + 0x01);
    return data;
}



void EEPROM_WriteByte (unsigned char address, unsigned char data) {
    
    EECON1 = 0; 
    EECON1bits.WREN = 1; 
    EEADR = address; 
    EEDATA = data; 
    EECON2 = 0x55; 
    EECON2 = 0xaa; 
    EECON1bits.WR = 1; 
    while (!PIR2bits.EEIF); 
    EECON1bits.WREN = 0; 
    while (EECON1bits.WR) continue;
}



void EEPROM_WriteInt (unsigned char address, unsigned int data) {
    EEPROM_WriteByte (address, (data >> 8));
    EEPROM_WriteByte ((address + 0x01), data);
}
