'/********************************************************************
' FileName:		Form1.vb
' Dependencies:	When compiled, needs .NET framework 2.0 redistributable 
'               to be installed in order to run.
' Hardware:		Need a free USB port to connect USB peripheral device
'				programmed with appropriate Generic HID firmware.  VID and
'				PID in firmware must match the VID and PID in this
'				program.
' Compiler:  	Microsoft Visual Basic 2008 Express Edition (or higher)
' Company:		Microchip Technology, Inc.

' Software License Agreement:

' The software supplied herewith by Microchip Technology Incorporated
' (the “Company”) for its PIC® Microcontroller is intended and
' supplied to you, the Company’s customer, for use solely and
' exclusively with Microchip PIC Microcontroller products. The
' software is owned by the Company and/or its supplier, and is
' protected under applicable copyright laws. All rights are reserved.
' Any use in violation of the foregoing restrictions may subject the
' user to criminal sanctions under applicable laws, as well as to
' civil liability for the breach of the terms and conditions of this
' license.

' THIS SOFTWARE IS PROVIDED IN AN “AS IS” CONDITION. NO WARRANTIES,
' WHETHER EXPRESS, IMPLIED OR STATUTORY, INCLUDING, BUT NOT LIMITED
' TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
' PARTICULAR PURPOSE APPLY TO THIS SOFTWARE. THE COMPANY SHALL NOT,
' IN ANY CIRCUMSTANCES, BE LIABLE FOR SPECIAL, INCIDENTAL OR
' CONSEQUENTIAL DAMAGES, FOR ANY REASON WHATSOEVER.

'********************************************************************
' File Description:

' Change History:
'  Rev   Date         Description
'  2.8	 06/30/2010	  Initial Release.  Ported project from the existing
'                     HID PnP Demo projects (written in Microsoft Visual 
'                     C++, as well as Microsoft Visual C#) from the
'                     MCHPFSUSB Framework v2.7
'********************************************************************
'NOTE:	All user made code contained in this project is in the Form1.vb file.
'		All other code and files were generated automatically by either the
'		new project wizard, or by the development environment (ex: code is
'		automatically generated if you create a new button on the form, and
'		then double click on it, which creates a click event handler
'		function). 
'********************************************************************/

'IMPORTANT NOTE: In order for this PC application to "find" the USB device,
'this application needs to know the USB Vendor ID (VID) and Product 
'ID (PID) of the USB device we want to communicate with.  This is
'specified in the DeviceIDToFind String shown below.  The VID and PID
'in the DeviceIDToFind string must match the VID/PID from the USB
'device descriptor specified in the microcontroller firmware.
'When changing the VID/PID used by the application, make sure to update
'both the firmware and the DeviceIDToFind string below.

'//NOTE 2: This VB program makes use of several functions in setupapi.dll and
'//other Win32 DLLs.  However, one cannot call the functions directly in a 
'//32-bit DLL if the project is built in "Any CPU" mode, when run on a 64-bit OS.
'//When configured to build an "Any CPU" executable, the executable will "become"
'//a 64-bit executable when run on a 64-bit OS.  On a 32-bit OS, it will run as 
'//a 32-bit executable, and the pointer sizes and other aspects of this 
'//application will be compatible with calling 32-bit DLLs.

'//Therefore, on a 64-bit OS, this application will not work unless it is built in
'//"x86" mode.  When built in this mode, the exectuable always runs in 32-bit mode
'//even on a 64-bit OS.  This allows this application to make 32-bit DLL function 
'//calls, when run on either a 32-bit or 64-bit OS.

'//By default, on a new project, VB normally wants to build in "Any CPU" mode.  
'//To switch to "x86" (32-bit only) mode:
'//1.  Click: Tools --> Options...
'//2.  Under "Project and Solutions", make sure the "Show advanced build configurations" checkbox is checked.
'//3.  Now click: Build --> Configuration Manager...
'//4.  Under "Active Solution Platform", select "x86".  If x86 doesn't exist in the list, select "<New...>"
'//    Then under the "Type or select the new platform" dropdown box, select "x86" and click OK


Imports Microsoft.Win32.SafeHandles
Imports System.Runtime.InteropServices      'Need this for Marshal class and for DLLImport operations
Imports System.Threading
Imports System.Text

Public Class Form1
    '//----------------------------------------------------------------------------------------------
    '//Use the formatting: "Vid_xxxx&Pid_xxxx" where xxxx is a 16-bit hexadecimal number.
    '//Make sure the value appearing in the parathesis matches the USB device descriptor
    '//of the device that this aplication is intending to find.
    Dim DeviceIDToFind As String = "Vid_04d8&Pid_002f" ' stari "Vid_04d8&Pid_003f"
    '//----------------------------------------------------------------------------------------------


    '//Constant definitions from setupapi.h
    Const DIGCF_PRESENT As UInteger = &H2
    Const DIGCF_DEVICEINTERFACE As UInteger = &H10
    '//Constants for CreateFile() and other file I/O functions
    Const FILE_ATTRIBUTE_NORMAL As Short = &H80
    Const INVALID_HANDLE_VALUE As Short = -1
    Const GENERIC_READ As UInteger = &H80000000&                    'Hmm... It seems VB doesn't really support unsigned types...
    'Dim GENERIC_READ As UInteger = Convert.ToUInt32(&H80000000)     'Need to use this instead.
    Const GENERIC_WRITE As UInteger = &H40000000
    Const CREATE_NEW As UInteger = &H1
    Const CREATE_ALWAYS As UInteger = &H2
    Const OPEN_EXISTING As UInteger = &H3
    Const FILE_SHARE_READ As UInteger = &H1
    Const FILE_SHARE_WRITE As UInteger = &H2
    '//Constant definitions for certain WM_DEVICECHANGE messages
    Const WM_DEVICECHANGE As UInteger = &H219
    Const DBT_DEVICEARRIVAL As UInteger = &H8000
    Const DBT_DEVICEREMOVEPENDING As UInteger = &H8003
    Const DBT_DEVICEREMOVECOMPLETE As UInteger = &H8004
    Const DBT_CONFIGCHANGED As UInteger = &H18

    '//Other constant definitions
    Const DBT_DEVTYP_DEVICEINTERFACE As UInteger = &H5
    Const DEVICE_NOTIFY_WINDOW_HANDLE As UInteger = &H0
    Const ERROR_SUCCESS As UInteger = &H0
    Const ERROR_NO_MORE_ITEMS As UInteger = &H103
    Const SPDRP_HARDWAREID As UInteger = &H1


    '//Various structure definitions for structures that this code will be using
    Structure SP_DEVICE_INTERFACE_DATA
        Dim cbSize As UInteger
        Dim InterfaceClassGuid As Guid
        Dim Flags As UInteger
        Dim Reserved As IntPtr
    End Structure

    Structure SP_DEVICE_INTERFACE_DETAIL_DATA
        Dim cbSize As UInteger
        Dim DevicePath() As Char
    End Structure

    Structure SP_DEVINFO_DATA
        Dim cbSize As UInteger
        Dim ClassGuid As Guid
        Dim DevInst As UInteger
        Dim Reserved As IntPtr
    End Structure

    Structure DEV_BROADCAST_DEVICEINTERFACE
        Dim dbcc_size As UInteger
        Dim dbcc_devicetype As UInteger
        Dim dbcc_reserved As UInteger
        Dim dbcc_classguid As Guid
        Dim dbcc_name() As Char
    End Structure

    '//DLL Imports.  Need these to access various C style unmanaged functions contained in their respective DLL files.
    '//--------------------------------------------------------------------------------------------------------------

    '//Returns a HDEVINFO type for a device information set.  We will need the 
    '//HDEVINFO as in input parameter for calling many of the other SetupDixxx() functions.
    <DllImport("setupapi.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Public Shared Function SetupDiGetClassDevs(ByRef ClassGuid As Guid, ByVal Enumerator As IntPtr, ByVal hwndParent As IntPtr, ByVal Flags As UInteger) As IntPtr
    End Function

    '//Gives us "PSP_DEVICE_INTERFACE_DATA" which contains the Interface specific GUID (different
    '//from class GUID).  We need the interface GUID to get the device path.
    <DllImport("setupapi.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Public Shared Function SetupDiEnumDeviceInterfaces(ByVal DeviceInfoSet As IntPtr, ByVal DeviceInfoData As IntPtr, ByRef InterfaceClassGuid As Guid, ByVal MemberIndex As UInteger, ByRef DeviceInterfaceData As SP_DEVICE_INTERFACE_DATA) As Boolean
    End Function

    ''//SetupDiDestroyDeviceInfoList() frees up memory by destroying a DeviceInfoList
    <DllImport("setupapi.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Public Shared Function SetupDiDestroyDeviceInfoList(ByVal DeviceInfoSet As IntPtr) As Boolean
    End Function

    ''//SetupDiEnumDeviceInfo() fills in an "SP_DEVINFO_DATA" structure, which we need for SetupDiGetDeviceRegistryProperty()
    <DllImport("setupapi.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Public Shared Function SetupDiEnumDeviceInfo(ByVal DeviceInfoSet As IntPtr, ByVal MemberIndex As UInteger, ByRef DeviceInterfaceData As SP_DEVINFO_DATA) As Boolean
    End Function

    ''//SetupDiGetDeviceRegistryProperty() gives us the hardware ID, which we use to check to see if it has matching VID/PID
    <DllImport("setupapi.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Public Shared Function SetupDiGetDeviceRegistryProperty( _
        ByVal DeviceInfoSet As IntPtr, _
        ByRef DeviceInfoData As SP_DEVINFO_DATA, _
        ByVal dwProperty As UInteger, _
        ByRef PropertyRegDataType As UInteger, _
        ByVal PropertyBuffer As IntPtr, _
        ByVal PropertyBufferSize As UInteger, _
        ByRef RequiredSize As UInteger) As Boolean
    End Function

    ''//SetupDiGetDeviceInterfaceDetail() gives us a device path, which is needed before CreateFile() can be used.
    <DllImport("setupapi.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Public Shared Function SetupDiGetDeviceInterfaceDetail( _
        ByVal DeviceInfoSet As IntPtr, _
        ByRef DeviceInterfaceData As SP_DEVICE_INTERFACE_DATA, _
        ByVal DeviceInterfaceDetailData As IntPtr, _
        ByVal DeviceInterfaceDetailDataSize As UInteger, _
        ByRef RequiredSize As UInteger, _
        ByVal DeviceInfoData As IntPtr) As Boolean
    End Function

    ''//Overload for SetupDiGetDeviceInterfaceDetail().  Need this one since we can't pass NULL pointers directly.
    <DllImport("setupapi.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Public Shared Function SetupDiGetDeviceInterfaceDetail( _
        ByVal DeviceInfoSet As IntPtr, _
        ByRef DeviceInterfaceData As SP_DEVICE_INTERFACE_DATA, _
        ByVal DeviceInterfaceDetailData As IntPtr, _
        ByVal DeviceInterfaceDetailDataSize As UInteger, _
        ByVal RequiredSize As IntPtr, _
        ByVal DeviceInfoData As IntPtr) As Boolean
    End Function

    '//Need this function for receiving all of the WM_DEVICECHANGE messages.  See MSDN documentation for
    '//description of what this function does/how to use it.
    <DllImport("user32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Public Shared Function RegisterDeviceNotification( _
        ByVal hRecipient As IntPtr, _
        ByVal NotificationFilter As IntPtr, _
        ByVal Flags As UInteger) As IntPtr
    End Function

    '//Takes in a device path and opens a handle to the device.  We need to create read and write
    '//handles, before we can read/write to the USB device endpoints
    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Public Shared Function CreateFile( _
        ByVal FileName As String, _
        ByVal dwDesiredAccess As UInteger, _
        ByVal dwShareMode As UInteger, _
        ByVal lpSecurityAttributes As IntPtr, _
        ByVal dwCreationDisposition As UInteger, _
        ByVal dwFlagsAndAttributes As UInteger, _
        ByVal hTemplateFile As IntPtr) As SafeFileHandle
    End Function

    '//Uses a handle (created with CreateFile()), and lets us write USB data to the HID device interrupt OUT endpoint
    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Public Shared Function WriteFile( _
        ByVal hFile As SafeFileHandle, _
        ByVal Buffer As Byte(), _
        ByVal nNumberOfBytesToWrite As UInteger, _
        ByRef lpNumberOfBytesWritten As UInteger, _
        ByVal lpOverlapped As IntPtr) As Boolean
    End Function


    '//Uses a handle (created with CreateFile()), and lets us read USB data from the HID device interrupt IN endpoint.
    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Public Shared Function ReadFile( _
        ByVal hFile As SafeFileHandle, _
        ByVal lpBuffer As IntPtr, _
        ByVal nNumberOfBytesToWrite As UInteger, _
        ByRef lpNumberOfBytesRead As UInteger, _
        ByVal lpOverlapped As IntPtr) As Boolean
    End Function



    '/*** This section is all of the global variables used in this project ***/
    '//--------------------------------------------------------------------------------------------------------

    'Dim DeviceIDToFind As String = "Vid_xxxx&Pid_yyyy" 'NOTE: The global variable DeviceIDToFind is located near the
    'top of this file, to make it easier to find/modify.

    '//Globally Unique Identifier (GUID) for HID class devices.  Windows uses GUIDs to identify things.
    Dim InterfaceClassGuid As New Guid("4D1E55B2-F16F-11CF-88CB-001111000030")

    '//USB related variables that need to have wide scope.
    Dim AttachedState As Boolean = False     '//Need to keep track of the USB device attachment status for proper plug and play operation.
    Dim AttachedButBroken As Boolean = False

    Dim DetailedInterfaceDataStructure As New SP_DEVICE_INTERFACE_DETAIL_DATA
    Dim ReadHandleToUSBDevice As SafeFileHandle
    Dim WriteHandleToUSBDevice As SafeFileHandle
    Dim DevicePath As String    '//Need the find the proper device path before you can open file handles.

    '//Variables used by the application/form updates.
    Dim ADCValue As Integer = 0              '//Updated by ReadWriteThread, read by FormUpdateTimer tick handler (needs to be atomic)
    Dim PushbuttonPressed = False            '//Updated by ReadWriteThread, read by FormUpdateTimer tick handler (needs to be atomic)
    Dim ToggleLEDsPending As Boolean = False '//Updated by ToggleLED(s) button click event handler, used by ReadWriteThread (needs to be atomic)



    '//--------------------------------------------------------------------------------------------------------------------------
    '//FUNCTION:	Form1_Load()
    '//PURPOSE:	    This callback function gets called when the application is initially launched and the form loads.  This is a good
    '//             place to do initialization for the rest of the application.  
    '//INPUT:	    None
    '//OUTPUT:	    May write to the WriteHandleToUSBDevice, ReadHandleToUSBDevice, AttachedState, and AttachedButBroken global variables
    '//--------------------------------------------------------------------------------------------------------------------------
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '//Initialize tool tips, to provide pop up help when the mouse cursor is moved over objects on the form.


        '//Register for WM_DEVICECHANGE notifications.  This code uses these messages to detect plug and play connection/disconnection events for USB devices
        Dim DeviceBroadcastHeader As New DEV_BROADCAST_DEVICEINTERFACE
        DeviceBroadcastHeader.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE
        DeviceBroadcastHeader.dbcc_size = Marshal.SizeOf(DeviceBroadcastHeader)
        DeviceBroadcastHeader.dbcc_reserved = 0   '//Reserved says not to use...
        DeviceBroadcastHeader.dbcc_classguid = InterfaceClassGuid

        '//Need to get the address of the DeviceBroadcastHeader to call RegisterDeviceNotification(), but
        '//can't use "&DeviceBroadcastHeader".  Instead, using a roundabout means to get the address by 
        '//making a duplicate copy using Marshal.StructureToPtr().
        Dim pDeviceBroadcastHeader As IntPtr = IntPtr.Zero  '//Make a pointer.
        'Note: Below line could throw an exception if the Mashal.AllocHGlobal() fails to allocate the memory, ex: because the system doesn't have enough free RAM available
        pDeviceBroadcastHeader = Marshal.AllocHGlobal(Marshal.SizeOf(DeviceBroadcastHeader)) '//allocate memory for a new DEV_BROADCAST_DEVICEINTERFACE structure, and return the address 
        Marshal.StructureToPtr(DeviceBroadcastHeader, pDeviceBroadcastHeader, False)    '//Copies the DeviceBroadcastHeader structure into the unmanaged memory already allocated at pDeviceBroadcastHeader
        RegisterDeviceNotification(Me.Handle, pDeviceBroadcastHeader, DEVICE_NOTIFY_WINDOW_HANDLE)
        'Now that we have registered to receive Windows Messages like WM_DEVICECHANGE, free the unmanaged RAM we previously allocated
        Marshal.FreeHGlobal(pDeviceBroadcastHeader)

        '//Now make an initial attempt to find the USB device, if it was already connected to the PC and enumerated prior to launching the application.
        '//If it is connected and present, we should open read and write handles to the device so we can communicate with it later.
        '//If it was not connected, we will have to wait until the user plugs the device in, and the WM_DEVICECHANGE callback function can process
        '//the message and again search for the device.
        If CheckIfPresentAndGetUSBDevicePath() Then '//Check and make sure at least one device with matching VID/PID is attached
            Dim ErrorStatusWrite As UInteger
            Dim ErrorStatusRead As UInteger

            '//We now have the proper device path, and we can finally open read and write handles to the device.
            WriteHandleToUSBDevice = CreateFile(DevicePath, GENERIC_WRITE, FILE_SHARE_READ Or FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero)
            ErrorStatusWrite = Marshal.GetLastWin32Error()
            ReadHandleToUSBDevice = CreateFile(DevicePath, GENERIC_READ, FILE_SHARE_READ Or FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero)
            ErrorStatusRead = Marshal.GetLastWin32Error()


            If (ErrorStatusWrite = ERROR_SUCCESS) And (ErrorStatusRead = ERROR_SUCCESS) Then
                AttachedState = True        '//Let the rest of the PC application know the USB device is connected, and it is safe to read/write to it
                AttachedButBroken = False
                StatusBox_txtbx.Text = "Device Connected"
            Else '//for some reason the device was physically plugged in, but one or both of the read/write handles didn't open successfully...
                AttachedState = False       '//Let the rest of this application known not to read/write to the device.
                AttachedButBroken = True    '//Flag so that next time a WM_DEVICECHANGE message occurs, can retry to re-open read/write pipes
                If ErrorStatusWrite = ERROR_SUCCESS Then
                    WriteHandleToUSBDevice.Close()
                End If

                If ErrorStatusRead = ERROR_SUCCESS Then
                    ReadHandleToUSBDevice.Close()
                End If
            End If
        Else
            AttachedState = False
            AttachedButBroken = False
        End If


        If AttachedState = True Then
            StatusBox_txtbx.Text = "Device Connected"
        Else
            StatusBox_txtbx.Text = "Device Not Connected"
        End If


        '//Recommend performing USB read/write operations in a separate thread.  Otherwise,
        '//the Read/Write operations are effectively blocking functions and can lock up the
        '//user interface if the I/O operations take a long time to complete.
        ReadWriteThread.RunWorkerAsync()    '//Starts the thread
    End Sub



    '//--------------------------------------------------------------------------------------------------------------------------
    '//FUNCTION:	WndProc()
    '//PURPOSE:	    This callback function gets called when Windows sends windows messages to the form (ex: WM_DEVICECHANGE for instance)
    '//INPUT:	    The windows message.
    '//OUTPUT:	    Updates the AttachedState and AttachedButBroken boolean variables, based on the USB connection status following WM_DEVICECHANGE events.
    '//--------------------------------------------------------------------------------------------------------------------------
    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        'Check for WM_DEVICECHANGE messages, which Windows will send you when certain events
        '(ex: Plug and Play hardware changes) take place in the system.
        If m.Msg = WM_DEVICECHANGE Then
            If ((m.WParam = DBT_DEVICEARRIVAL) Or (m.WParam = DBT_DEVICEREMOVEPENDING) Or (m.WParam = DBT_DEVICEREMOVECOMPLETE) Or (m.WParam = DBT_CONFIGCHANGED)) Then
                '//WM_DEVICECHANGE messages by themselves are quite generic, and can be caused by a number of different
                '//sources, not just your USB hardware device.  Therefore, must check to find out if any changes relavant
                '//to your device (with known VID/PID) took place before doing any kind of opening or closing of handles/endpoints.
                '//(the message could have been totally unrelated to your application/USB device)

                If CheckIfPresentAndGetUSBDevicePath() = True Then      '//Check and make sure at least one device with matching VID/PID is attached
                    If ((AttachedState = False) Or (AttachedButBroken = True)) Then '//Check the previous attachment state
                        Dim ErrorStatusRead As UInteger
                        Dim ErrorStatusWrite As UInteger

                        '//We obtained the proper device path (from CheckIfPresentAndGetUSBDevicePath() function call), and it
                        '//is now possible to open read and write handles to the device.
                        WriteHandleToUSBDevice = CreateFile(DevicePath, GENERIC_WRITE, FILE_SHARE_READ Or FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero)
                        ErrorStatusWrite = Marshal.GetLastWin32Error()
                        ReadHandleToUSBDevice = CreateFile(DevicePath, GENERIC_READ, FILE_SHARE_READ Or FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero)
                        ErrorStatusRead = Marshal.GetLastWin32Error()


                        If ((ErrorStatusWrite = ERROR_SUCCESS) And (ErrorStatusRead = ERROR_SUCCESS)) Then
                            AttachedState = True        '//Let the rest of the PC application know the USB device is connected, and it is safe to read/write to it
                            AttachedButBroken = False
                            StatusBox_txtbx.Text = "Device Found, AttachedState = TRUE"
                        Else '//for some reason the device was physically plugged in, but one or both of the read/write handles didn't open successfully...
                            AttachedState = False       '//Let the rest of this application known not to read/write to the device.
                            AttachedButBroken = True    '//Flag so that next time a WM_DEVICECHANGE message occurs, can retry to re-open read/write pipes
                            If ErrorStatusWrite = ERROR_SUCCESS Then
                                WriteHandleToUSBDevice.Close()
                            End If
                            If ErrorStatusRead = ERROR_SUCCESS Then
                                ReadHandleToUSBDevice.Close()
                            End If
                        End If
                    End If
                    '//else we did find the device, but AttachedState was already true.  In this case, don't do anything to the read/write handles,
                    '//since the WM_DEVICECHANGE message presumably wasn't caused by our USB device.  
                Else 'CheckIfPresentAndGetUSBDevicePath() must have returned False

                    If AttachedState = True Then    '//If it is currently set to true, that means the device was just now disconnected
                        AttachedState = False
                        WriteHandleToUSBDevice.Close()
                        ReadHandleToUSBDevice.Close()
                    End If
                    AttachedState = False
                    AttachedButBroken = False
                End If 'If CheckIfPresentAndGetUSBDevicePath() = True
            End If 'If ((m.WParam = DBT_DEVICEARRIVAL) Or (m.WParam = DBT_DEVICEREMOVEPENDING) Or (m.WParam = DBT_DEVICEREMOVECOMPLETE) Or (m.WParam = DBT_CONFIGCHANGED)) Then
        End If 'If m.Msg = WM_DEVICECHANGE Then

        'Call the regular event handler, to it can handle normal windows messages
        MyBase.WndProc(m)
    End Sub




    '//FUNCTION: CheckIfPresentAndGetUSBDevicePath()
    '//PURPOSE:	 Check if a USB device is currently plugged in with a matching VID and PID
    '//INPUT:	 Uses globally declared String DevicePath, globally declared GUID, and the MY_DEVICE_ID constant.
    '//OUTPUT:	 Returns BOOL.  TRUE when device with matching VID/PID found.  FALSE if device with VID/PID could not be found.
    '//			 When returns TRUE, the globally accessable "DetailedInterfaceDataStructure" will contain the device path
    '//			 to the USB device with the matching VID/PID.
    Private Function CheckIfPresentAndGetUSBDevicePath() As Boolean
        'Before we can "connect" our application to our USB embedded device, we must first find the device.
        'A USB bus can have many devices simultaneously connected, so somehow we have to find our device only.
        'This is done with the Vendor ID (VID) and Product ID (PID).  Each USB product line should have
        'a unique combination of VID and PID.  

        'Microsoft has created a number of functions which are useful for finding plug and play devices.  Documentation
        'for each function used can be found in the MSDN library.  We will be using the following functions (unmanaged C functions):

        'SetupDiGetClassDevs()					//provided by setupapi.dll, which comes with Windows
        'SetupDiEnumDeviceInterfaces()			//provided by setupapi.dll, which comes with Windows
        'GetLastError()							//provided by kernel32.dll, which comes with Windows
        'SetupDiDestroyDeviceInfoList()			//provided by setupapi.dll, which comes with Windows
        'SetupDiGetDeviceInterfaceDetail()		//provided by setupapi.dll, which comes with Windows
        'SetupDiGetDeviceRegistryProperty()		//provided by setupapi.dll, which comes with Windows
        'CreateFile()							//provided by kernel32.dll, which comes with Windows

        'In order to call these unmanaged functions, the Marshal class is very useful.

        'We will also be using the following unusual data types and structures.  Documentation can also be found in
        'the MSDN library:
        'PSP_DEVICE_INTERFACE_DATA()
        'PSP_DEVICE_INTERFACE_DETAIL_DATA()
        'SP_DEVINFO_DATA()
        'HDEVINFO()
        'Handle()
        'Guid()

        'The ultimate objective of the following code is to get the device path, which will be used elsewhere for getting
        'read and write handles to the USB device.  Once the read/write handles are opened, only then can this
        'PC application begin reading/writing to the USB device using the WriteFile() and ReadFile() functions.

        'Getting the device path is a multi-step round about process, which requires calling several of the
        'SetupDixxx() functions provided by setupapi.dll.
        Try
            Dim DeviceInfoTable As IntPtr = IntPtr.Zero
            Dim InterfaceDataStructure As New SP_DEVICE_INTERFACE_DATA()
            Dim DetailedInterfaceDataStructure As New SP_DEVICE_INTERFACE_DETAIL_DATA()
            Dim DevInfoData As New SP_DEVINFO_DATA()
            Dim InterfaceIndex As UInteger = 0
            Dim dwRegType As UInteger = 0
            Dim dwRegSize As UInteger = 0
            Dim dwRegSize2 As UInteger = 0
            Dim StructureSize As UInteger = 0
            Dim PropertyValueBuffer As IntPtr = IntPtr.Zero
            Dim MatchFound As Boolean = False
            Dim ErrorStatus As UInteger = 0
            Dim LoopCounter As UInteger = 0

            '//Use the formatting: "Vid_xxxx&Pid_xxxx" where xxxx is a 16-bit hexadecimal number.
            '//Make sure the value appearing in the parathesis matches the USB device descriptor
            '//of the device that this aplication is intending to find.
            Dim DeviceIDToFind As String = "Vid_04d8&Pid_002f"

            '//First populate a list of plugged in devices (by specifying "DIGCF_PRESENT"), which are of the specified class GUID. 
            DeviceInfoTable = SetupDiGetClassDevs(InterfaceClassGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT Or DIGCF_DEVICEINTERFACE)
            'Make sure SetupDiGetClassDevs returned a valid/non-NULL pointer
            If DeviceInfoTable <> IntPtr.Zero Then
                While True
                    InterfaceDataStructure.cbSize = Marshal.SizeOf(InterfaceDataStructure)

                    If SetupDiEnumDeviceInterfaces(DeviceInfoTable, IntPtr.Zero, InterfaceClassGuid, InterfaceIndex, InterfaceDataStructure) Then
                        ErrorStatus = Marshal.GetLastWin32Error()

                        If ErrorStatus = ERROR_NO_MORE_ITEMS Then   '//Did we reach the end of the list of matching devices in the DeviceInfoTable?
                            '//Cound not find the device.  Must not have been attached.
                            SetupDiDestroyDeviceInfoList(DeviceInfoTable)   '//Clean up the old structure we no longer need.
                            Return False
                        End If

                        '//Now retrieve the hardware ID from the registry.  The hardware ID contains the VID and PID, which we will then 
                        '//check to see if it is the correct device or not.

                        '//Initialize an appropriate SP_DEVINFO_DATA structure.  We need this structure for SetupDiGetDeviceRegistryProperty().
                        DevInfoData.cbSize = Marshal.SizeOf(DevInfoData)
                        SetupDiEnumDeviceInfo(DeviceInfoTable, InterfaceIndex, DevInfoData)

                        '//First query for the size of the hardware ID, so we can know how big a buffer to allocate for the data.
                        SetupDiGetDeviceRegistryProperty(DeviceInfoTable, DevInfoData, SPDRP_HARDWAREID, dwRegType, IntPtr.Zero, 0, dwRegSize)

                        '//Allocate a buffer for the hardware ID.
                        '//Should normally work, but could throw exception "OutOfMemoryException" if not enough resources available.
                        PropertyValueBuffer = Marshal.AllocHGlobal(CType(dwRegSize, IntPtr))

                        '//Retrieve the hardware IDs for the current device we are looking at.  PropertyValueBuffer gets filled with a 
                        '//REG_MULTI_SZ (array of null terminated strings).  To find a device, we only care about the very first string in the
                        '//buffer, which will be the "device ID".  The device ID is a string which contains the VID and PID, in the example 
                        '//format "Vid_04d8&Pid_003f".
                        SetupDiGetDeviceRegistryProperty(DeviceInfoTable, DevInfoData, SPDRP_HARDWAREID, dwRegType, PropertyValueBuffer, dwRegSize, dwRegSize2)
                        ErrorStatus = Marshal.GetLastWin32Error()

                        '//Now check if the first string in the hardware ID matches the device ID of the USB device we are trying to find.
                        Dim DeviceIDFromRegistry As String
                        DeviceIDFromRegistry = Marshal.PtrToStringUni(PropertyValueBuffer) '//Fill the string with the contents from the PropertyValueBuffer
                        Marshal.FreeHGlobal(PropertyValueBuffer)                            '//No longer need the PropertyValueBuffer, free the memory to prevent potential memory leaks

                        '//Convert both strings to lower case.  This makes the code more robust/portable accross OS Versions
                        DeviceIDFromRegistry = DeviceIDFromRegistry.ToLowerInvariant()
                        DeviceIDToFind = DeviceIDToFind.ToLowerInvariant()
                        '//Now check if the hardware ID we are looking at contains the correct VID/PID
                        MatchFound = DeviceIDFromRegistry.Contains(DeviceIDToFind)

                        If MatchFound Then
                            '//Device must have been found.  In order to open I/O file handle(s), we will need the actual device path first.
                            '//We can get the path by calling SetupDiGetDeviceInterfaceDetail(), however, we have to call this function twice:  The first
                            '//time to get the size of the required structure/buffer to hold the detailed interface data, then a second time to actually 
                            '//get the structure (after we have allocated enough memory for the structure.)
                            DetailedInterfaceDataStructure.cbSize = Marshal.SizeOf(DetailedInterfaceDataStructure)
                            '//First call populates "StructureSize" with the correct value
                            SetupDiGetDeviceInterfaceDetail(DeviceInfoTable, InterfaceDataStructure, IntPtr.Zero, 0, StructureSize, IntPtr.Zero)
                            '//Need to call SetupDiGetDeviceInterfaceDetail() again, this time specifying a pointer to a SP_DEVICE_INTERFACE_DETAIL_DATA buffer with the correct size of RAM allocated.
                            '//First need to allocate the unmanaged buffer and get a pointer to it.
                            Dim pUnmanagedDetailedInterfaceDataStructure As IntPtr = IntPtr.Zero    '//Declare a pointer.
                            pUnmanagedDetailedInterfaceDataStructure = Marshal.AllocHGlobal(CType(StructureSize, IntPtr))    '//Reserve some unmanaged memory for the structure.
                            DetailedInterfaceDataStructure.cbSize = 6   '//Initialize the cbSize parameter (4 bytes for DWORD + 2 bytes for unicode null terminator)
                            Marshal.StructureToPtr(DetailedInterfaceDataStructure, pUnmanagedDetailedInterfaceDataStructure, False) '//Copy managed structure contents into the unmanaged memory buffer.

                            '//Now call SetupDiGetDeviceInterfaceDetail() a second time to receive the device path in the structure at pUnmanagedDetailedInterfaceDataStructure.
                            If SetupDiGetDeviceInterfaceDetail(DeviceInfoTable, InterfaceDataStructure, pUnmanagedDetailedInterfaceDataStructure, StructureSize, IntPtr.Zero, IntPtr.Zero) Then
                                '//Need to extract the path information from the unmanaged "structure".  The path starts at (pUnmanagedDetailedInterfaceDataStructure + sizeof(DWORD)).
                                Dim pToDevicePath As IntPtr
                                pToDevicePath = pUnmanagedDetailedInterfaceDataStructure.ToInt32() + 4  '//Add 4 to the pointer (to get the pointer to point to the path, instead of the DWORD cbSize parameter)
                                DevicePath = Marshal.PtrToStringUni(pToDevicePath)  '//Now copy the path information into the globally defined DevicePath String.

                                '//We now have the proper device path, and we can finally use the path to open I/O handle(s) to the device.
                                SetupDiDestroyDeviceInfoList(DeviceInfoTable)   '//Clean up the old structure we no longer need.
                                Marshal.FreeHGlobal(pUnmanagedDetailedInterfaceDataStructure)   '//No longer need this unmanaged SP_DEVICE_INTERFACE_DETAIL_DATA buffer.  We already extracted the path information.
                                Return True '//Returning the device path in the global DevicePath String
                            Else
                                '//Some unknown failure occurred
                                Dim ErrorCode As UInteger = Marshal.GetLastWin32Error()
                                SetupDiDestroyDeviceInfoList(DeviceInfoTable)       '//Clean up the old structure.
                                Marshal.FreeHGlobal(pUnmanagedDetailedInterfaceDataStructure)   '//No longer need this unmanaged SP_DEVICE_INTERFACE_DETAIL_DATA buffer.  We already extracted the path information.
                                Return False
                            End If
                        End If

                        InterfaceIndex = InterfaceIndex + 1
                        '//Keep looping until we either find a device with matching VID and PID, or until we run out of devices to check.
                        '//However, just in case some unexpected error occurs, keep track of the number of loops executed.
                        '//If the number of loops exceeds a very large number, exit anyway, to prevent inadvertent infinite looping.
                        LoopCounter = LoopCounter + 1
                        If LoopCounter = 10000000 Then  '//Surely there aren't more than 10 million devices attached to any forseeable PC...
                            Return False
                        End If
                    Else
                        '//Else some other kind of unknown error ocurred...
                        ErrorStatus = Marshal.GetLastWin32Error()
                        SetupDiDestroyDeviceInfoList(DeviceInfoTable)   '//Clean up the old structure we no longer need.
                        Return False
                    End If 'If SetupDiEnumDeviceInterfaces(DeviceInfoTable, IntPtr.Zero, InterfaceClassGuid, InterfaceIndex, InterfaceDataStructure) Then
                End While 'While True
            End If  'If DeviceInfoTable <> IntPtr.Zero Then
            Return False
        Catch ex As Exception
            '//Something went wrong if PC gets here.  Maybe a Marshal.AllocHGlobal() failed due to insufficient resources or something.
            Return False
        End Try
    End Function


    '//--------------------------------------------------------------------------------------------------------------------------
    '//FUNCTION:	ToggleLEDs_btn_Click()
    '//PURPOSE:	    Queues up a USB I/O request for the ReadWriteThread_DoWork() thread to handle.
    '//INPUT:	    None.
    '//OUTPUT:	    Uses a global variable as a flag, which the thread watches.
    '//--------------------------------------------------------------------------------------------------------------------------
    Private Sub ToggleLEDs_btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ToggleLEDsPending = True    '//Will get used asynchronously by the ReadWriteThread
    End Sub


    '//--------------------------------------------------------------------------------------------------------------------------
    '//FUNCTION:	FormUpdateTimer_Tick()
    '//PURPOSE:	    Periodically updates items on the main form, based on connection status changes and I/O results from the ReadWriteThread_DoWork() thread.
    '//INPUT:	    Uses global variables.
    '//OUTPUT:	    Updates the form printed on the screen.
    '//--------------------------------------------------------------------------------------------------------------------------
    Private Sub FormUpdateTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FormUpdateTimer.Tick

        '-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '-------------------------------------------------------BEGIN CUT AND PASTE BLOCK-----------------------------------------------------------------------------------
        'This timer tick event handler function is used to update the user interface on the form, based on data
        'obtained asynchronously by the ReadWriteThread and the WM_DEVICECHANGE event handler functions.

        'Check if user interface on the form should be enabled or not, based on the attachment state of the USB device.

        If AttachedState = True Then
            '//Device is connected and ready to communicate, enable user interface on the form 
            StatusBox_txtbx.Text = "Device Connected"

        End If



        If (AttachedState = False) Or (AttachedButBroken = True) Then
            '//Device not available to communicate. Disable user interface on the form.
            StatusBox_txtbx.Text = "Device Not Connected"
            ADCValue = 0
        End If
        'Update the various status indicators on the form with the latest info obtained from the ReadWriteThread()
        If AttachedState = True Then
            'Update the pushbutton state label.
            'if (PushbuttonPressed == false)
            'PushbuttonState_lbl.Text = "Pushbutton State: Not Pressed";		//Update the pushbutton state text label on the form, so the user can see the result 
            'else
            'PushbuttonState_lbl.Text = "Pushbutton State: Pressed";			//Update the pushbutton state text label on the form, so the user can see the result 

            'Update the ANxx/POT Voltage indicator value (progressbar)
            'progressBar1.Value = (int)ADCValue;
            If TextUpdated = True Then
                Select Case txt_cmd
                    Case &H1
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 0: "
                        For i As Byte = 0 To 63
                            txtBoxResponse.Text += response(i).ToString()
                        Next
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &H2
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 1: "
                        For i As Byte = 0 To 63
                            txtBoxResponse.Text += response(i).ToString()
                        Next
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &H3
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 2: "
                        For i As Byte = 0 To 63
                            txtBoxResponse.Text += response(i).ToString()
                        Next
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &H4
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 3: "
                        For i As Byte = 0 To 63
                            txtBoxResponse.Text += response(i).ToString()
                        Next
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &H5
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 4: "
                        For i As Byte = 0 To 63
                            txtBoxResponse.Text += response(i).ToString()
                        Next
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &H6
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 5: "
                        For i As Byte = 0 To 63
                            txtBoxResponse.Text += response(i).ToString()
                        Next
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &H7
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 6: "
                        For i As Byte = 0 To 63
                            txtBoxResponse.Text += response(i).ToString()
                        Next
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &H8
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 7: "
                        For i As Byte = 0 To 63
                            txtBoxResponse.Text += response(i).ToString()
                        Next
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &H9
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 8: "
                        For i As Byte = 0 To 63
                            txtBoxResponse.Text += response(i).ToString()
                        Next
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &HA
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 0 written"
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &HB
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 1 written"
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &HC
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 2 written"
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &HD
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 3 written"
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &HE
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 4 written"
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &HF
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 5 written"
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &H10
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 6 written"
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &H11
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 7 written"
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &H12
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "sector 8 written"
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select

                    Case &H17
                        txtBoxResponse.Clear()
                        txtBoxResponse.Text = "card ID: "
                        For i As Byte = 3 To 7
                            txtBoxResponse.Text += response(i).ToString()
                        Next
                        txt_cmd = 0
                        TextUpdated = False
                        Exit Select
                End Select


            End If
        End If
        '-------------------------------------------------------END CUT AND PASTE BLOCK-------------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------------------------------------------------------------------------------
    End Sub

    '=======================================================
    'Service provided by Telerik (www.telerik.com)
    'Conversion powered by NRefactory.
    'Twitter: @telerik
    'Facebook: facebook.com/telerik
    '=======================================================


    '//--------------------------------------------------------------------------------------------------------------------------
    '//FUNCTION:	ReadWriteThread_DoWork()
    '//PURPOSE:	    This thread executes USB read/write requests and updates globabl variables.
    '//             It is generally preferrable to perform I/O operations in a separate thread from
    '//             the Windows form, so that the I/O operations (which can take a long time to complete/may
    '//             be blocking functions) do not block graphical and other updates to the form (leading
    '//             to the appearance of a "locked up" user interface.
    '//INPUT:	    Requires the global WriteHandleToUSBDevice and ReadHandleToUSBDevice to be initialized
    '//             and valid, in order for this thread to do anything useful.  This thread also takes in other global
    '//             variables like ToggleLEDsPending to determine what to do.
    '//OUTPUT:	    This thread updates global variables like ToggleLEDsPending (after processing the I/O request), and
    '//             ADCValue.
    '//--------------------------------------------------------------------------------------------------------------------------
    Public CommandButtonPressed As Boolean = False
    Public txt_cmd As Byte
    Public TextUpdated As Boolean = False
    Public command As Byte
    Public response As Byte() = New Byte(64) {}
    Public WriteTextUpdated As Boolean = False

    Private Sub ReadWriteThread_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles ReadWriteThread.DoWork
        '-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '-------------------------------------------------------BEGIN CUT AND PASTE BLOCK-----------------------------------------------------------------------------------

        'This thread does the actual USB read/write operations (but only when AttachedState == true) to the USB device.
        '            It is generally preferrable to write applications so that read and write operations are handled in a separate
        '            thread from the main form.  This makes it so that the main form can remain responsive, even if the I/O operations
        '            take a very long time to complete.
        '
        '            Since this is a separate thread, this code below executes independently from the rest of the
        '            code in this application.  All this thread does is read and write to the USB device.  It does not update
        '            the form directly with the new information it obtains (such as the ANxx/POT Voltage or pushbutton state).
        '            The information that this thread obtains is stored in atomic global variables.
        '            Form updates are handled by the FormUpdateTimer Tick event handler function.
        '
        '            This application sends packets to the endpoint buffer on the USB device by using the "WriteFile()" function.
        '            This application receives packets from the endpoint buffer on the USB device by using the "ReadFile()" function.
        '            Both of these functions are documented in the MSDN library.  Calling ReadFile() is a not perfectly straight
        '            foward in C# environment, since one of the input parameters is a pointer to a buffer that gets filled by ReadFile().
        '            The ReadFile() function is therefore called through a wrapper function ReadFileManagedBuffer().
        '
        '            All ReadFile() and WriteFile() operations in this example project are synchronous.  They are blocking function
        '            calls and only return when they are complete, or if they fail because of some event, such as the user unplugging
        '            the device.  It is possible to call these functions with "overlapped" structures, and use them as non-blocking
        '            asynchronous I/O function calls.  
        '
        '            Note:  This code may perform differently on some machines when the USB device is plugged into the host through a
        '            USB 2.0 hub, as opposed to a direct connection to a root port on the PC.  In some cases the data rate may be slower
        '            when the device is connected through a USB 2.0 hub.  This performance difference is believed to be caused by
        '            the issue described in Microsoft knowledge base article 940021:
        '            http://support.microsoft.com/kb/940021/en-us 
        '
        '            Higher effective bandwidth (up to the maximum offered by interrupt endpoints), both when connected
        '            directly and through a USB 2.0 hub, can generally be achieved by queuing up multiple pending read and/or
        '            write requests simultaneously.  This can be done when using	asynchronous I/O operations (calling ReadFile() and
        '            WriteFile()	with overlapped structures).  The Microchip	HID USB Bootloader application uses asynchronous I/O
        '            for some USB operations and the source code can be used	as an example.



        Dim OUTBuffer As [Byte]() = New Byte(64) {}
        'Allocate a memory buffer equal to the OUT endpoint size + 1
        Dim INBuffer As [Byte]() = New Byte(64) {}
        'Allocate a memory buffer equal to the IN endpoint size + 1
        Dim BytesWritten As UInteger = 0
        Dim BytesRead As UInteger = 0

        While True
            Try
                If AttachedState = True Then
                    'Do not try to use the read/write handles unless the USB device is attached and ready

                    If CommandButtonPressed = True Then
                        CommandButtonPressed = False

                        Select Case command
                            Case &H1

                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H52
                                'READ 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H30
                                '0
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0

                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If

                                Exit Select

                            Case &H2

                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H52
                                'READ 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H31
                                '0
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)

                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &H3

                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H52
                                'READ 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H32
                                '0
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &H4
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H52
                                'READ 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H33
                                '0
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &H5
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H52
                                'READ 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H34
                                '0
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &H6
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H52
                                'READ 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H35
                                '0
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &H7
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H52
                                'READ 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H36
                                '0
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &H8
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H52
                                'READ 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H37
                                '0
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &H9
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H52
                                'READ 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H38
                                '0
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &HA
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H57
                                'WRITE 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H30
                                '0
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteTextUpdated = True Then
                                    Dim buffer As Byte() = New Byte(63) {}
                                    buffer = Encoding.ASCII.GetBytes(WriteData)
                                    Dim i As Byte = 0
                                    For Each element As Byte In buffer
                                        OUTBuffer(i + 4) = element
                                        i += 1
                                    Next
                                End If


                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &HB
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H57
                                'WRITE 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H31
                                '1
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteTextUpdated = True Then
                                    Dim buffer As Byte() = New Byte(63) {}
                                    buffer = Encoding.ASCII.GetBytes(WriteData)
                                    Dim i As Byte = 0
                                    For Each element As Byte In buffer
                                        OUTBuffer(i + 4) = element
                                        i += 1
                                    Next
                                End If


                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &HC
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H57
                                'WRITE 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H32
                                '2
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteTextUpdated = True Then
                                    Dim buffer As Byte() = New Byte(63) {}
                                    buffer = Encoding.ASCII.GetBytes(WriteData)
                                    Dim i As Byte = 0
                                    For Each element As Byte In buffer
                                        OUTBuffer(i + 4) = element
                                        i += 1
                                    Next
                                End If


                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &HD
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H57
                                'WRITE 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H33
                                '3
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteTextUpdated = True Then
                                    Dim buffer As Byte() = New Byte(63) {}
                                    buffer = Encoding.ASCII.GetBytes(WriteData)
                                    Dim i As Byte = 0
                                    For Each element As Byte In buffer
                                        OUTBuffer(i + 4) = element
                                        i += 1
                                    Next
                                End If


                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &HE
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H57
                                'WRITE 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H34
                                '4
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteTextUpdated = True Then
                                    Dim buffer As Byte() = New Byte(63) {}
                                    buffer = Encoding.ASCII.GetBytes(WriteData)
                                    Dim i As Byte = 0
                                    For Each element As Byte In buffer
                                        OUTBuffer(i + 4) = element
                                        i += 1
                                    Next
                                End If


                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &HF
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H57
                                'WRITE 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H35
                                '5
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteTextUpdated = True Then
                                    Dim buffer As Byte() = New Byte(63) {}
                                    buffer = Encoding.ASCII.GetBytes(WriteData)
                                    Dim i As Byte = 0
                                    For Each element As Byte In buffer
                                        OUTBuffer(i + 4) = element
                                        i += 1
                                    Next
                                End If


                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &H10
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H57
                                'WRITE 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H36
                                '6
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteTextUpdated = True Then
                                    Dim buffer As Byte() = New Byte(63) {}
                                    buffer = Encoding.ASCII.GetBytes(WriteData)
                                    Dim i As Byte = 0
                                    For Each element As Byte In buffer
                                        OUTBuffer(i + 4) = element
                                        i += 1
                                    Next
                                End If


                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &H11
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H57
                                'WRITE 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H37
                                '7
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteTextUpdated = True Then
                                    Dim buffer As Byte() = New Byte(63) {}
                                    buffer = Encoding.ASCII.GetBytes(WriteData)
                                    Dim i As Byte = 0
                                    For Each element As Byte In buffer
                                        OUTBuffer(i + 4) = element
                                        i += 1
                                    Next
                                End If


                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &H12
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H57
                                'WRITE 
                                OUTBuffer(2) = &H53
                                'SECTOR
                                OUTBuffer(3) = &H38
                                '8
                                For i As UInteger = 4 To 64
                                    OUTBuffer(i) = &HFF
                                Next

                                If WriteTextUpdated = True Then
                                    Dim buffer As Byte() = New Byte(63) {}
                                    buffer = Encoding.ASCII.GetBytes(WriteData)
                                    Dim i As Byte = 0
                                    For Each element As Byte In buffer
                                        OUTBuffer(i + 4) = element
                                        i += 1
                                    Next
                                End If


                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H53 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                            Case &H13
                                Exit Select

                            Case &H14
                                Exit Select

                            Case &H15
                                Exit Select

                            Case &H16
                                Exit Select

                            Case &H17
                                OUTBuffer(0) = &H0
                                'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                                OUTBuffer(1) = &H52
                                'WRITE
                                OUTBuffer(2) = &H49
                                'SECTOR
                                If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                                    'Blocking function, unless an "overlapped" structure is used
                                    INBuffer(0) = 0

                                    If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                                        'Blocking function, unless an "overlapped" structure is used	
                                        If INBuffer(1) = &H52 Then
                                            txt_cmd = command
                                            TextUpdated = True
                                            command = 0
                                            CommandButtonPressed = False
                                            For i As UInteger = 4 To 64
                                                response(i) = INBuffer(i)
                                            Next
                                        End If
                                    End If
                                End If
                                Exit Select

                        End Select
                    End If

                    'Check if this thread should send a Toggle LED(s) command to the firmware.  ToggleLEDsPending will get set
                    'by the ToggleLEDs_btn click event handler function if the user presses the button on the form.
                    If ToggleLEDsPending = True Then
                        OUTBuffer(0) = 0
                        'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                        OUTBuffer(1) = &H80
                        '0x80 is the "Toggle LED(s)" command in the firmware
                        For i As UInteger = 2 To 64
                            'This loop is not strictly necessary.  Simply initializes unused bytes to
                            OUTBuffer(i) = &HFF
                        Next
                        '0xFF for lower EMI and power consumption when driving the USB cable.
                        'Now send the packet to the USB firmware on the microcontroller
                        WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero)
                        'Blocking function, unless an "overlapped" structure is used
                        ToggleLEDsPending = False
                    End If
                Else
                    'end of: if(AttachedState == true)
                    'Add a small delay.  Otherwise, this while(true) loop can execute very fast and cause 
                    'high CPU utilization, with no particular benefit to the application.
                    Thread.Sleep(5)
                End If
                'Exceptions can occur during the read or write operations.  For example,
                'exceptions may occur if for instance the USB device is physically unplugged
                'from the host while the above read/write functions are executing.

                'Don't need to do anything special in this case.  The application will automatically
                're-establish communications based on the global AttachedState boolean variable used
                'in conjunction with the WM_DEVICECHANGE messages to dyanmically respond to Plug and Play
                'USB connection events.
            Catch

            End Try
        End While
        'end of while(true) loop
        '-------------------------------------------------------END CUT AND PASTE BLOCK-------------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------------------------------------------------------------------------------
    End Sub


    '=======================================================
    'Service provided by Telerik (www.telerik.com)
    'Conversion powered by NRefactory.
    'Twitter: @telerik
    'Facebook: facebook.com/telerik
    '=======================================================

    '//--------------------------------------------------------------------------------------------------------------------------
    '//FUNCTION: ReadFileManagedBuffer()
    '//PURPOSE:	 Wrapper function to call ReadFile()
    '//
    '//INPUT:	 Uses managed versions of the same input parameters as ReadFile() uses.
    '//
    '//OUTPUT:	 Returns boolean indicating if the function call was successful or not.
    '//          Also returns data in the byte[] INBuffer, and the number of bytes read. 
    '//
    '//Notes:    Wrapper function used to call the ReadFile() function.  ReadFile() takes a pointer to an unmanaged buffer and deposits
    '//          the bytes read into the buffer.  However, can't pass a pointer to a managed buffer directly to ReadFile().
    '//          This ReadFileManagedBuffer() is a wrapper function to make it so application code can call ReadFile() easier
    '//          by specifying a managed buffer.
    '//--------------------------------------------------------------------------------------------------------------------------
    Private Function ReadFileManagedBuffer(ByVal hFile As SafeFileHandle, ByRef INBuffer() As Byte, ByVal nNumberOfBytesToRead As UInteger, ByRef lpNumberOfBytesRead As UInteger, ByVal lpOverlapped As IntPtr) As Boolean
        Dim pINBuffer As IntPtr = IntPtr.Zero

        Try
            pINBuffer = Marshal.AllocHGlobal(CType(nNumberOfBytesToRead, Integer))      '//Allocate some unmanged RAM for the receive data buffer.

            If ReadFile(hFile, pINBuffer, nNumberOfBytesToRead, lpNumberOfBytesRead, lpOverlapped) Then
                Marshal.Copy(pINBuffer, INBuffer, 0, lpNumberOfBytesRead)   '//Copy over the data from unmanged memory into the managed byte[] INBuffer
                Marshal.FreeHGlobal(pINBuffer)
                Return True
            Else
                Marshal.FreeHGlobal(pINBuffer)
                Return False
            End If

        Catch ex As Exception
            If pINBuffer <> IntPtr.Zero Then
                Marshal.FreeHGlobal(pINBuffer)
                Return False
            End If
        End Try
    End Function



    Private Sub cbxCommandSelect_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub txtBoxResponse_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBoxResponse.TextChanged

    End Sub
    Public WriteData As String = Nothing
    Private Sub txtCardWriteData_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        WriteTextUpdated = True
    End Sub

    Private Sub btnCommand_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        CommandButtonPressed = True
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If TextBox1.Text.Trim <> "" Then
            Cursor = Cursors.WaitCursor
            write(TextBox1.Text)
            Cursor = Cursors.Default
        End If
    End Sub
    Private Sub write(ByVal str As String)
        Try
            TextBox2.Text = ""
            Dim OUTBuffer As [Byte]() = New Byte(64) {}
            'Allocate a memory buffer equal to the OUT endpoint size + 1
            Dim INBuffer As [Byte]() = New Byte(64) {}
            'Allocate a memory buffer equal to the IN endpoint size + 1
            Dim BytesWritten As UInteger = 0
            Dim BytesRead As UInteger = 0

            OUTBuffer(0) = &H0
            'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
            OUTBuffer(1) = &H57
            'WRITE 
            OUTBuffer(2) = &H53
            'SECTOR
            OUTBuffer(3) = &H32
            '1 &H31
            For i As UInteger = 4 To 64
                OUTBuffer(i) = &HFF
            Next

            Dim buffer As Byte() = New Byte(63) {}
            buffer = Encoding.ASCII.GetBytes(str)
            Dim ii As Byte = 0
            For Each element As Byte In buffer
                OUTBuffer(ii + 4) = element
                ii += 1
            Next
            'Try
            '    ii = 0
            '    For Each element As Byte In response
            '        OUTBuffer(ii + 4) = element
            '        ii += 1
            '    Next
            'Catch
            'End Try

            If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                'Blocking function, unless an "overlapped" structure is used
                INBuffer(0) = 0

                If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                    'Blocking function, unless an "overlapped" structure is used	

                    If INBuffer(1) = &H53 Then
                        txt_cmd = command
                        TextUpdated = True
                        command = 0
                        CommandButtonPressed = False
                        For i As UInteger = 4 To 64
                            response(i) = INBuffer(i)
                            TextBox2.AppendText(INBuffer(i))
                        Next
                    End If
                End If
            End If
        Catch ef As Exception

        End Try
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        If RadioButton1.Checked Then pisisoba("G")
        If RadioButton2.Checked Then pisisoba("H")
        If RadioButton3.Checked Then pisisoba("0")
        If RadioButton4.Checked Then pisisoba("S")
        If RadioButton5.Checked Then pisisoba("M")
    End Sub



    Private Function pisisoba(ByVal tip As String) As String
        Try
            TextBox2.Text = ""
            Dim OUTBuffer As [Byte]() = New Byte(64) {}
            'Allocate a memory buffer equal to the OUT endpoint size + 1
            Dim INBuffer As [Byte]() = New Byte(64) {}
            'Allocate a memory buffer equal to the IN endpoint size + 1
            Dim BytesWritten As UInteger = 0
            Dim BytesRead As UInteger = 0
            For i As UInteger = 0 To 64
                OUTBuffer(i) = &HFF
            Next
            OUTBuffer(0) = &H0
            'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.

            OUTBuffer(1) = Encoding.ASCII.GetBytes("W")(0)  'pisem R read87:
            'WRITE 
            OUTBuffer(2) = Encoding.ASCII.GetBytes(tip)(0) 'G gost H sobarica M manager S servis P preset

            'vrijeme
            Dim dan As String = DateTimePicker1.Value.Day.ToString
            Dim mje As String = DateTimePicker1.Value.Month.ToString
            Dim god As String = DateTimePicker1.Value.Year.ToString.Substring(2, 2)
            Dim sat As String = DateTimePicker2.Value.Hour.ToString
            Dim min As String = DateTimePicker2.Value.Minute.ToString
            If dan.Length = 1 Then dan = "0" & dan
            If mje.Length = 1 Then mje = "0" & mje
            If god.Length = 1 Then god = "0" & god
            If sat.Length = 1 Then sat = "0" & sat
            If min.Length = 1 Then min = "0" & min

            OUTBuffer(3) = Encoding.ASCII.GetBytes(dan.Substring(0, 1))(0)
            OUTBuffer(4) = Encoding.ASCII.GetBytes(dan.Substring(1, 1))(0)
            OUTBuffer(5) = Encoding.ASCII.GetBytes(mje.Substring(0, 1))(0)
            OUTBuffer(6) = Encoding.ASCII.GetBytes(mje.Substring(1, 1))(0)
            OUTBuffer(7) = Encoding.ASCII.GetBytes(god.Substring(0, 1))(0)
            OUTBuffer(8) = Encoding.ASCII.GetBytes(god.Substring(1, 1))(0)
            OUTBuffer(9) = Encoding.ASCII.GetBytes(sat.Substring(0, 1))(0)
            OUTBuffer(10) = Encoding.ASCII.GetBytes(sat.Substring(1, 1))(0)
            OUTBuffer(11) = Encoding.ASCII.GetBytes(min.Substring(0, 1))(0)
            OUTBuffer(12) = Encoding.ASCII.GetBytes(min.Substring(1, 1))(0)

            Dim adr As String = "00000" & NumericUpDown1.Value
            If tip = "0" Then
                OUTBuffer(13) = &H30
                OUTBuffer(14) = &H30
                OUTBuffer(15) = &H30
                OUTBuffer(16) = &H30
                OUTBuffer(17) = &H30
            Else
                OUTBuffer(13) = Encoding.ASCII.GetBytes(adr.Substring(adr.Length - 5, 1))(0)
                OUTBuffer(14) = Encoding.ASCII.GetBytes(adr.Substring(adr.Length - 4, 1))(0)
                OUTBuffer(15) = Encoding.ASCII.GetBytes(adr.Substring(adr.Length - 3, 1))(0)
                OUTBuffer(16) = Encoding.ASCII.GetBytes(adr.Substring(adr.Length - 2, 1))(0)
                OUTBuffer(17) = Encoding.ASCII.GetBytes(adr.Substring(adr.Length - 1, 1))(0)
            End If
            OUTBuffer(18) = Encoding.ASCII.GetBytes(ComboBox2.Text.Trim)(0)  'N normal , I invalid
            OUTBuffer(19) = Encoding.ASCII.GetBytes(ComboBox4.Text.Trim)(0) 'Eng, bos, ger, arap, tur, franc, ita
            OUTBuffer(20) = Encoding.ASCII.GetBytes(ComboBox1.Text)(0)  'space bez loga, broj pozicije logo firme (broj ili slovo)
            OUTBuffer(21) = Encoding.ASCII.GetBytes(ComboBox3.Text.Trim)(0) 'M musko Z zensko gsp, gdja, mis itd

            'id objekta 22-25
            Dim adrid As String = "00000" & idObjekta.Value 'system ID = 12345 for example'
            OUTBuffer(22) = Encoding.ASCII.GetBytes(adrid.Substring(adrid.Length - 5, 1))(0) '= 1 from sys ID'
            OUTBuffer(23) = Encoding.ASCII.GetBytes(adrid.Substring(adrid.Length - 4, 1))(0) '= 2 from sys ID'
            OUTBuffer(24) = Encoding.ASCII.GetBytes(adrid.Substring(adrid.Length - 3, 1))(0) '= 3 from sys ID'
            OUTBuffer(25) = Encoding.ASCII.GetBytes(adrid.Substring(adrid.Length - 2, 1))(0) '= 4 from sys ID'
            OUTBuffer(26) = Encoding.ASCII.GetBytes(adrid.Substring(adrid.Length - 1, 1))(0) '= 5 from sys ID'
            'od 32- 45 prezime
            Dim prezime As String = TextBox4.Text.Replace("Č", "C").Replace("Ć", "C").Replace("č", "c").Replace("ć", "c").Replace("Ž", "Z").Replace("ž", "z").Replace("Š", "S").Replace("š", "s").Replace("Đ", "D").Replace("đ", "d")
            For i = 0 To prezime.Length - 1
                If i + 32 > 48 Then Exit For
                OUTBuffer(32 + i) = Encoding.ASCII.GetBytes(prezime(i))(0)
            Next

            'od 46- 64 ime
            Dim ime As String = TextBox3.Text.Replace("Č", "C").Replace("Ć", "C").Replace("č", "c").Replace("ć", "c").Replace("Ž", "Z").Replace("ž", "z").Replace("Š", "S").Replace("š", "s").Replace("Đ", "D").Replace("đ", "d")
            For i = 0 To ime.Length - 1
                If i + 48 > 64 Then Exit For
                OUTBuffer(48 + i) = Encoding.ASCII.GetBytes(ime(i))(0)

            Next
            txtBoxResponse.Text = ""
            For i As UInteger = 0 To 64
                txtBoxResponse.AppendText(OUTBuffer(i) & ",")
            Next
            Dim buffer As Byte() = New Byte(63) {}
            If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                'Blocking function, unless an "overlapped" structure is used
                INBuffer(0) = 0

                If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                    'Blocking function, unless an "overlapped" structure is used	
                    For i As UInteger = 0 To 64
                        response(i) = INBuffer(i)
                        TextBox2.AppendText(INBuffer(i) & ",")
                    Next
                    txt_cmd = command
                    TextUpdated = True
                    command = 0
                    CommandButtonPressed = False
                    Dim kar() As Byte = New Byte() {response(2), response(3), response(4), response(5), response(6)}
                    'Array.Reverse(kard, 0, 4)
                    Dim len As UInt32 = BitConverter.ToUInt32(kar, 0)
                    TextBox1.Text = len
                    If INBuffer(1).ToString.Trim = "70" Then
                        TextBox1.Text = "GRESKA"
                    End If
                    'pisisobar("G")4107191324


                End If
            End If
        Catch ef As Exception

        End Try
    End Function
    Private Function pisiostalo(ByVal tip As String) As String
        Try
            TextBox2.Text = ""
            Dim OUTBuffer As [Byte]() = New Byte(64) {}
            'Allocate a memory buffer equal to the OUT endpoint size + 1
            Dim INBuffer As [Byte]() = New Byte(64) {}
            'Allocate a memory buffer equal to the IN endpoint size + 1
            Dim BytesWritten As UInteger = 0
            Dim BytesRead As UInteger = 0
            For i As UInteger = 0 To 64
                OUTBuffer(i) = &H7F
            Next
            OUTBuffer(0) = &H0
            'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.

            OUTBuffer(1) = Encoding.ASCII.GetBytes("P")(0)  'pisem R read87:
            'WRITE 
            '  OUTBuffer(2) = Encoding.ASCII.GetBytes(tip)(0) 'G gost H sobarica M manager S servis P preset

            'vrijeme
            Dim dan As String = DateTimePicker1.Value.Day.ToString
            Dim mje As String = DateTimePicker1.Value.Month.ToString
            Dim god As String = DateTimePicker1.Value.Year.ToString.Substring(2, 2)
            Dim sat As String = DateTimePicker2.Value.Hour.ToString
            Dim min As String = DateTimePicker2.Value.Minute.ToString
            If dan.Length = 1 Then dan = "0" & dan
            If mje.Length = 1 Then mje = "0" & mje
            If god.Length = 1 Then god = "0" & god
            If sat.Length = 1 Then sat = "0" & sat
            If min.Length = 1 Then min = "0" & min

            OUTBuffer(2) = Encoding.ASCII.GetBytes(dan.Substring(0, 1))(0)
            OUTBuffer(3) = Encoding.ASCII.GetBytes(dan.Substring(1, 1))(0)
            OUTBuffer(4) = Encoding.ASCII.GetBytes(mje.Substring(0, 1))(0)
            OUTBuffer(5) = Encoding.ASCII.GetBytes(mje.Substring(1, 1))(0)
            OUTBuffer(6) = Encoding.ASCII.GetBytes(god.Substring(0, 1))(0)
            OUTBuffer(7) = Encoding.ASCII.GetBytes(god.Substring(1, 1))(0)
            OUTBuffer(8) = Encoding.ASCII.GetBytes(sat.Substring(0, 1))(0)
            OUTBuffer(9) = Encoding.ASCII.GetBytes(sat.Substring(1, 1))(0)
            OUTBuffer(10) = Encoding.ASCII.GetBytes(min.Substring(0, 1))(0)
            OUTBuffer(11) = Encoding.ASCII.GetBytes(min.Substring(1, 1))(0)

            Dim adr As String = "0" & NumericUpDown2.Value
            ' broj korisnika
            OUTBuffer(12) = Encoding.ASCII.GetBytes(adr.Substring(adr.Length - 2, 1))(0)
            OUTBuffer(13) = Encoding.ASCII.GetBytes(adr.Substring(adr.Length - 1, 1))(0)
            ' C kartica, W narukvica, Q privjesak
            If ComboBox5.SelectedIndex = 0 Then
                OUTBuffer(14) = Encoding.ASCII.GetBytes("C")(0)
            ElseIf ComboBox5.SelectedIndex = 1 Then
                OUTBuffer(14) = Encoding.ASCII.GetBytes("W")(0)
            ElseIf ComboBox5.SelectedIndex = 2 Then
                OUTBuffer(14) = Encoding.ASCII.GetBytes("Q")(0)
            End If

            ' 0 jednokratno, 1 vremenski
            If ComboBox6.SelectedIndex = 0 Then
                OUTBuffer(15) = Encoding.ASCII.GetBytes("O")(0)
            Else
                OUTBuffer(15) = Encoding.ASCII.GetBytes("E")(0)
            End If


            'OUTBuffer(17) = Encoding.ASCII.GetBytes(adr.Substring(adr.Length - 1, 1))(0)
            'OUTBuffer(18) = Encoding.ASCII.GetBytes(ComboBox2.Text.Trim)(0)  'N normal , I invalid
            'OUTBuffer(19) = Encoding.ASCII.GetBytes(ComboBox4.Text.Trim)(0) 'Eng, bos, ger, arap, tur, franc, ita
            'OUTBuffer(20) = Encoding.ASCII.GetBytes(ComboBox1.Text)(0)  'space bez loga, broj pozicije logo firme (broj ili slovo)
            'OUTBuffer(21) = Encoding.ASCII.GetBytes(ComboBox3.Text.Trim)(0) 'M musko Z zensko gsp, gdja, mis itd

            'id objekta 16-20
            Dim adrid As String = "00000" & idObjekta.Value 'system ID = 12345 for example'
            OUTBuffer(16) = Encoding.ASCII.GetBytes(adrid.Substring(adrid.Length - 5, 1))(0) '= 1 from sys ID'
            OUTBuffer(17) = Encoding.ASCII.GetBytes(adrid.Substring(adrid.Length - 4, 1))(0) '= 2 from sys ID'
            OUTBuffer(18) = Encoding.ASCII.GetBytes(adrid.Substring(adrid.Length - 3, 1))(0) '= 3 from sys ID'
            OUTBuffer(19) = Encoding.ASCII.GetBytes(adrid.Substring(adrid.Length - 2, 1))(0) '= 4 from sys ID'
            OUTBuffer(20) = Encoding.ASCII.GetBytes(adrid.Substring(adrid.Length - 1, 1))(0) '= 5 from sys ID'
            'od 32- 45 prezime

            If CheckBox1.Checked Then OUTBuffer(21) = Encoding.ASCII.GetBytes("B")(0)
            If CheckBox2.Checked Then OUTBuffer(22) = Encoding.ASCII.GetBytes("B")(0)
            If CheckBox3.Checked Then OUTBuffer(23) = Encoding.ASCII.GetBytes("B")(0)
            If CheckBox4.Checked Then OUTBuffer(24) = Encoding.ASCII.GetBytes("B")(0)
            If CheckBox12.Checked Then OUTBuffer(25) = Encoding.ASCII.GetBytes("B")(0)
            If CheckBox13.Checked Then OUTBuffer(26) = Encoding.ASCII.GetBytes("B")(0)
            'dani
            If CheckBox5.Checked Then OUTBuffer(27) = Encoding.ASCII.GetBytes("B")(0)
            If CheckBox6.Checked Then OUTBuffer(28) = Encoding.ASCII.GetBytes("B")(0)
            If CheckBox11.Checked Then OUTBuffer(29) = Encoding.ASCII.GetBytes("B")(0)
            If CheckBox7.Checked Then OUTBuffer(30) = Encoding.ASCII.GetBytes("B")(0)
            If CheckBox8.Checked Then OUTBuffer(31) = Encoding.ASCII.GetBytes("B")(0)
            If CheckBox9.Checked Then OUTBuffer(32) = Encoding.ASCII.GetBytes("B")(0)
            If CheckBox10.Checked Then OUTBuffer(33) = Encoding.ASCII.GetBytes("B")(0)

            'Dim tt As String = 2159
            'Dim ids As Byte()
            'ids = Encoding.ASCII.GetBytes(tt)
            'idslike 
            Dim idslike As String = "00000" & TextBox5.Text 'system ID = 12345 for example'
            OUTBuffer(34) = Encoding.ASCII.GetBytes(adrid.Substring(idslike.Length - 4, 1))(0) '= 1 from sys ID'
            OUTBuffer(35) = Encoding.ASCII.GetBytes(adrid.Substring(idslike.Length - 3, 1))(0) '= 2 from sys ID'
            OUTBuffer(36) = Encoding.ASCII.GetBytes(adrid.Substring(idslike.Length - 2, 1))(0) '= 3 from sys ID'
            OUTBuffer(37) = Encoding.ASCII.GetBytes(adrid.Substring(idslike.Length - 1, 1))(0) '= 4 from sys ID'
           
            'duzina boravka
            Dim dbor As String = "00000" & NumericUpDown3.Value
            OUTBuffer(38) = Encoding.ASCII.GetBytes(adrid.Substring(idslike.Length - 2, 1))(0) '= 3 from sys ID'
            OUTBuffer(39) = Encoding.ASCII.GetBytes(adrid.Substring(idslike.Length - 1, 1))(0)
            'od do 
            sat = DateTimePicker3.Value.Hour.ToString
            min = DateTimePicker3.Value.Minute.ToString 
            If sat.Length = 1 Then sat = "0" & sat
            If min.Length = 1 Then min = "0" & min
            OUTBuffer(40) = Encoding.ASCII.GetBytes(sat.Substring(0, 1))(0)
            OUTBuffer(41) = Encoding.ASCII.GetBytes(sat.Substring(1, 1))(0)
            OUTBuffer(42) = Encoding.ASCII.GetBytes(min.Substring(0, 1))(0)
            OUTBuffer(43) = Encoding.ASCII.GetBytes(min.Substring(1, 1))(0)

            sat = DateTimePicker4.Value.Hour.ToString
            min = DateTimePicker4.Value.Minute.ToString
            If sat.Length = 1 Then sat = "0" & sat
            If min.Length = 1 Then min = "0" & min
            OUTBuffer(44) = Encoding.ASCII.GetBytes(sat.Substring(0, 1))(0)
            OUTBuffer(45) = Encoding.ASCII.GetBytes(sat.Substring(1, 1))(0)
            OUTBuffer(46) = Encoding.ASCII.GetBytes(min.Substring(0, 1))(0)
            OUTBuffer(47) = Encoding.ASCII.GetBytes(min.Substring(1, 1))(0)
         
            'grupa kor
            Dim gk As String = "00000" & NumericUpDown4.Value
            OUTBuffer(48) = Encoding.ASCII.GetBytes(adrid.Substring(idslike.Length - 2, 1))(0) '= 3 from sys ID'
            OUTBuffer(49) = Encoding.ASCII.GetBytes(adrid.Substring(idslike.Length - 1, 1))(0)

            txtBoxResponse.Text = ""
            For i As UInteger = 0 To 64
                txtBoxResponse.AppendText(OUTBuffer(i) & ",")
            Next
            Dim buffer As Byte() = New Byte(63) {}
            If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
                'Blocking function, unless an "overlapped" structure is used
                INBuffer(0) = 0

                If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                    'Blocking function, unless an "overlapped" structure is used	
                    For i As UInteger = 0 To 64
                        response(i) = INBuffer(i)
                        TextBox2.AppendText(INBuffer(i) & ",")
                    Next
                    txt_cmd = command
                    TextUpdated = True
                    command = 0
                    CommandButtonPressed = False
                    Dim kar() As Byte = New Byte() {response(2), response(3), response(4), response(5), response(6)}
                    'Array.Reverse(kard, 0, 4)
                    Dim len As UInt32 = BitConverter.ToUInt32(kar, 0)
                    TextBox1.Text = len
                    If INBuffer(1).ToString.Trim = "70" Then
                        TextBox1.Text = "GRESKA"
                    End If
                    'pisisobar("G")4107191324


                End If
            End If
        Catch ef As Exception

        End Try
    End Function
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        PROCITAJ("R")
    End Sub
    Private Sub PROCITAJ(ByVal T As String)
        TextBox2.Text = ""
        TextBox1.Text = ""
        Dim OUTBuffer As [Byte]() = New Byte(64) {}
        'Allocate a memory buffer equal to the OUT endpoint size + 1
        Dim INBuffer As [Byte]() = New Byte(64) {}
        'Allocate a memory buffer equal to the IN endpoint size + 1
        Dim BytesWritten As UInteger = 0
        Dim BytesRead As UInteger = 0


        OUTBuffer(0) = &H0
        'The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
        OUTBuffer(1) = Encoding.ASCII.GetBytes(T)(0)

        For i As UInteger = 2 To 64
            OUTBuffer(i) = &HFF
        Next

        If WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, BytesWritten, IntPtr.Zero) Then
            'Blocking function, unless an "overlapped" structure is used
            INBuffer(0) = 0

            If ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, BytesRead, IntPtr.Zero) Then
                For i As UInteger = 0 To 64
                    response(i) = INBuffer(i)
                    TextBox2.AppendText(INBuffer(i) & ",")
                Next
                'Blocking function, unless an "overlapped" structure is used	
                Dim kar() As Byte = New Byte() {response(2), response(3), response(4), response(5), response(6)}
                'Array.Reverse(kard, 0, 4)
                Dim len As UInt32 = BitConverter.ToUInt32(kar, 0)
                Dim gr() As Byte = New Byte() {response(8)}

                Dim go() As Byte = New Byte() {response(14), response(15)}
                Dim mj() As Byte = New Byte() {response(12), response(13)}
                Dim da() As Byte = New Byte() {response(10), response(11)}
                Dim sa() As Byte = New Byte() {response(16), response(17)}
                Dim mi() As Byte = New Byte() {response(18), response(19)}

                ' A slovo adresa response(22),
                Dim adresa() As Byte = New Byte() {response(23), response(24), response(25), response(26), response(27)}

                Dim g1 As String = System.Text.Encoding.UTF8.GetString(New Byte() {response(28)})
                Dim g2 As String = System.Text.Encoding.UTF8.GetString(New Byte() {response(29)})
                Dim g3 As String = System.Text.Encoding.UTF8.GetString(New Byte() {response(30)})
                Dim g4 As String = System.Text.Encoding.UTF8.GetString(New Byte() {response(31)})

                Dim g5 As String = System.Text.Encoding.UTF8.GetString(New Byte() {response(33)})
                Dim g6 As String = System.Text.Encoding.UTF8.GetString(New Byte() {response(34)})
                Dim g7 As String = System.Text.Encoding.UTF8.GetString(New Byte() {response(35)})
                Dim g8 As String = System.Text.Encoding.UTF8.GetString(New Byte() {response(36)})
                Dim g9 As String = System.Text.Encoding.UTF8.GetString(New Byte() {response(37)})

                Dim ime() As Byte = New Byte() {response(32), response(33), response(34), response(35), response(36), response(37), response(38), response(39), response(40), response(41), response(42), response(43), response(44), response(45), response(46), response(47)}
                Dim prez() As Byte = New Byte() {response(48), response(49), response(50), response(51), response(52), response(53), response(54), response(55), response(56), response(57), response(58), response(59), response(60), response(61), response(62), response(63)}
                Dim im As String = System.Text.Encoding.UTF8.GetString(prez).Replace("�", "")
                'Dim pre As String = System.Text.Encoding.UTF8.GetString(ime).Replace("�", "")

                TextBox1.Text = len & ", " & System.Text.Encoding.UTF8.GetString(gr) & ", " & g1 & ", " & g2 & ", " & g3 & ", " & g4 & ", " & g5 & g6 & g7 & g8 & g9 & ", " & System.Text.Encoding.UTF8.GetString(adresa) & ", " & System.Text.Encoding.UTF8.GetString(da) & "." & System.Text.Encoding.UTF8.GetString(mj) & ".20" & System.Text.Encoding.UTF8.GetString(go) & " " & System.Text.Encoding.UTF8.GetString(sa) & ":" & System.Text.Encoding.UTF8.GetString(mi) & ", " & ", " & im


            End If
        End If



    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
         If RadioButton1.Checked Then pisiostalo("G")
        If RadioButton2.Checked Then pisiostalo("H")
        If RadioButton3.Checked Then pisiostalo("0")
        If RadioButton4.Checked Then pisiostalo("S")
        If RadioButton5.Checked Then pisiostalo("M")
    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        PROCITAJ("C")
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click

        If RadioButton1.Checked Then pisisoba("G")
        If RadioButton2.Checked Then pisisoba("H")
        If RadioButton3.Checked Then pisisoba("0")
        If RadioButton4.Checked Then pisisoba("S")
        If RadioButton5.Checked Then pisisoba("M")
        If RadioButton1.Checked Then pisiostalo("G")
        If RadioButton2.Checked Then pisiostalo("H")
        If RadioButton3.Checked Then pisiostalo("0")
        If RadioButton4.Checked Then pisiostalo("S")
        If RadioButton5.Checked Then pisiostalo("M")
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        PROCITAJ("K")
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        PROCITAJ("S")
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        PROCITAJ("R")
        PROCITAJ("C")
    End Sub

    Private Sub CheckBox14_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox14.CheckedChanged
        If CheckBox14.Checked Then
            PROCITAJ("U")
        Else
            PROCITAJ("S")
        End If

    End Sub
End Class
