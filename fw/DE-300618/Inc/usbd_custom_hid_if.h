/**
  ******************************************************************************
  * @file           : usbd_custom_hid_if.h
  * @version        : v2.0_Cube
  * @brief          : Header for usbd_custom_hid_if.c file.
  ******************************************************************************
  */

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __USBD_CUSTOM_HID_IF_H__
#define __USBD_CUSTOM_HID_IF_H__

#ifdef __cplusplus
 extern "C" {
#endif

/* Includes ------------------------------------------------------------------*/
#include "usbd_customhid.h"

#define Buzzer_On()         (HAL_GPIO_WritePin(GPIOA, GPIO_PIN_0, GPIO_PIN_SET))
#define Buzzer_Off()        (HAL_GPIO_WritePin(GPIOA, GPIO_PIN_0, GPIO_PIN_RESET))
#define IsBuzzer_On()       (HAL_GPIO_ReadPin(GPIOA, GPIO_PIN_0) == GPIO_PIN_SET)
#define RedLed_On()         (HAL_GPIO_WritePin(GPIOA, GPIO_PIN_1, GPIO_PIN_RESET))
#define RedLed_Off()        (HAL_GPIO_WritePin(GPIOA, GPIO_PIN_1, GPIO_PIN_SET))
#define IsRedLed_On()       (HAL_GPIO_ReadPin(GPIOA, GPIO_PIN_1) == GPIO_PIN_RESET)
#define RedLed_Toggle()     (HAL_GPIO_TogglePin(GPIOA, GPIO_PIN_1))
#define GreenLed_On()       (HAL_GPIO_WritePin(GPIOA, GPIO_PIN_2, GPIO_PIN_RESET))
#define GreenLed_Off()      (HAL_GPIO_WritePin(GPIOA, GPIO_PIN_2, GPIO_PIN_SET))
#define IsGreenLed_On()     (HAL_GPIO_ReadPin(GPIOA, GPIO_PIN_2) == GPIO_PIN_RESET)
#define GreenLed_Toggle()   (HAL_GPIO_TogglePin(GPIOA, GPIO_PIN_2))

/** CUSTOMHID Interface callback. */
extern USBD_CUSTOM_HID_ItfTypeDef USBD_CustomHID_fops_FS;


#ifdef __cplusplus
}
#endif

#endif /* __USBD_CUSTOM_HID_IF_H__ */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
