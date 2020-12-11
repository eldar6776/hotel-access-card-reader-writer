/********************************************************************
 FileName:     	HardwareProfile - PICDEM FSUSB K50.h
 Dependencies:  See INCLUDES section
 Processor:     PIC18 USB Microcontrollers
 Hardware:      PICDEM FSUSB populated with PIC18F45K50
 Compiler:      Microchip C18
 Company:       Microchip Technology, Inc.

 Software License Agreement:

 The software supplied herewith by Microchip Technology Incorporated
 (the "Company") for its PIC� Microcontroller is intended and
 supplied to you, the Company's customer, for use solely and
 exclusively on Microchip PIC Microcontroller products. The
 software is owned by the Company and/or its supplier, and is
 protected under applicable copyright laws. All rights are reserved.
 Any use in violation of the foregoing restrictions may subject the
 user to criminal sanctions under applicable laws, as well as to
 civil liability for the breach of the terms and conditions of this
 license.

 THIS SOFTWARE IS PROVIDED IN AN "AS IS" CONDITION. NO WARRANTIES,
 WHETHER EXPRESS, IMPLIED OR STATUTORY, INCLUDING, BUT NOT LIMITED
 TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 PARTICULAR PURPOSE APPLY TO THIS SOFTWARE. THE COMPANY SHALL NOT,
 IN ANY CIRCUMSTANCES, BE LIABLE FOR SPECIAL, INCIDENTAL OR
 CONSEQUENTIAL DAMAGES, FOR ANY REASON WHATSOEVER.

 ********************************************************************
 File Description:

 Change History:
  Rev   Date         Description
  2.9f  06/25/2012   Initial release
 ********************************************************************/

#if defined(__18F2550)
#include <p18f2550.h>
#elif defined(__18F25K50)
#include <p18f25k50.h>
#endif   

#ifndef HARDWARE_PROFILE_H
#define HARDWARE_PROFILE_H

/*******************************************************************/
/******** USB stack hardware selection options *********************/
/*******************************************************************/
//This section is the set of definitions required by the MCHPFSUSB
//  framework.  These definitions tell the firmware what mode it is
//  running in, and where it can find the results to some information
//  that the stack needs.
//These definitions are required by every application developed with
//  this revision of the MCHPFSUSB framework.  Please review each
//  option carefully and determine which options are desired/required
//  for your application.

//The PICDEM FS USB Demo Board platform supports the USE_SELF_POWER_SENSE_IO
//and USE_USB_BUS_SENSE_IO features.  Uncomment the below line(s) if
//it is desireable to use one or both of the features.
//#define USE_SELF_POWER_SENSE_IO
//#define tris_self_power     TRISAbits.TRISA2    // Input
#if defined(USE_SELF_POWER_SENSE_IO)
#define self_power          PORTAbits.RA2
#else
#define self_power          1
#endif

//#define USE_USB_BUS_SENSE_IO
//#define tris_usb_bus_sense  TRISAbits.TRISA1    // Input
#if defined(USE_USB_BUS_SENSE_IO)
#define USB_BUS_SENSE       PORTAbits.RA1
#else
#define USB_BUS_SENSE       1
#endif


//Uncomment the following line to make the output HEX of this  
//  project work with the MCHPUSB Bootloader    
//#define PROGRAMMABLE_WITH_USB_MCHPUSB_BOOTLOADER

//Uncomment the following line to make the output HEX of this 
//  project work with the HID Bootloader
#define PROGRAMMABLE_WITH_USB_HID_BOOTLOADER		


#if defined(__18F2550)

#define DEMO_BOARD PICDEM_FS_USB
#define PICDEM_FS_USB
#define CLOCK_FREQ 48000000 

#define LED_BLUE_DOUT               (LATAbits.LATA6)
#define LED_RED_DOUT                (LATAbits.LATA5)
#define LED_GREEN_DOUT              (LATAbits.LATA4)
#define BUZZER_DOUT                 (LATCbits.LATC1)
#define POWER_CONTROL_DOUT          (LATCbits.LATC6)

//#define LED_RED_DOUT                (LATAbits.LATA0)
//#define LED_BLUE_DOUT               (LATAbits.LATA1)
//#define BUZZER_DOUT                 (LATAbits.LATA2)


#define mRC522_Reset(STATE)         (LATBbits.LATB2 = STATE)
#define mRC522_ChipSelect(STATE)    (LATBbits.LATB4 = STATE)
//#define mRC522_ChipSelect(STATE)  (LATCbits.LATC7 = STATE)
//#define mRC522_ChipSelect(STATE)  (LATBbits.LATB3 = STATE)

#elif defined(__18F25K50)

#define DEMO_BOARD PICDEM_FS_USB_K50
#define PICDEM_FS_USB_K50
#define PIC18F45K50_FAMILY
#define CLOCK_FREQ 48000000

#define LED_BLUE_DOUT               (LATAbits.LATA4)
#define LED_RED_DOUT                (LATAbits.LATA7)
#define BUZZER_DOUT                 (LATCbits.LATC1)


#define mRC522_Reset(STATE)         (LATBbits.LATB2 = STATE)
#define mRC522_ChipSelect(STATE)    (LATBbits.LATB4 = STATE)
//#define mRC522_ChipSelect(STATE)  (LATCbits.LATC7 = STATE)
//#define mRC522_ChipSelect(STATE)  (LATBbits.LATB3 = STATE)

#endif

#define RedLed_On()                 (LED_RED_DOUT = 0)
#define RedLed_Off()                (LED_RED_DOUT = 1)
#define RedLed_Toggle()             (LED_RED_DOUT = !LED_RED_DOUT)
#define GetRedLedState()            (LED_RED_DOUT)

#define GreenLed_On()               (LED_GREEN_DOUT = 0)
#define GreenLed_Off()              (LED_GREEN_DOUT = 1)
#define GreenLed_Toggle()           (LED_GREEN_DOUT = !LED_GREEN_DOUT)
#define GetGreenLedState()          (LED_GREEN_DOUT)

#define BlueLed_On()                (LED_BLUE_DOUT = 1)
#define BlueLed_Off()               (LED_BLUE_DOUT = 0)
#define BlueLed_Toggle()            (LED_BLUE_DOUT = !LED_BLUE_DOUT)
#define GetBlueLedState()           (LED_BLUE_DOUT)

#define Buzzer_On()                 (BUZZER_DOUT = 1)
#define Buzzer_Off()                (BUZZER_DOUT = 0)
#define Buzzer_Toggle()             (BUZZER_DOUT = !BUZZER_DOUT)
#define GetBuzzerState()            (BUZZER_DOUT)

#define PowerControl_On()           (POWER_CONTROL_DOUT = 0)
#define PowerControl_Off()          (POWER_CONTROL_DOUT = 1)
#define PowerControl_Toggle()       (POWER_CONTROL_DOUT = !POWER_CONTROL_DOUT)
#define GetPowerControlState()      (POWER_CONTROL_DOUT)

#endif  //HARDWARE_PROFILE_PICDEM_FSUSB_K50_H
