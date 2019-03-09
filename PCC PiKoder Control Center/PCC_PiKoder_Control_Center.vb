Option Strict Off
Option Explicit On
Imports System
Imports System.IO.Ports
Imports System.Collections.ObjectModel
Imports System.Text
Imports NativeWifi

Public Class PCC_PiKoder_Control_Center

    ' This is a program for evaluating the PiKoder platform - please refer to http://pikoder.com for more details.
    ' 
    ' Copyright 2015-2019 Gregor Schlechtriem
    '
    ' Licensed under the Apache License, Version 2.0 (the "License");
    ' you may not use this file except in compliance with the License.
    ' You may obtain a copy of the License at
    '
    ' http://www.apache.org/licenses/LICENSE-2.0
    '
    ' Unless required by applicable law or agreed to in writing, software
    ' distributed under the License is distributed on an "AS IS" BASIS,
    ' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    ' See the License for the specific language governing permissions and
    ' limitations under the License.

    Inherits System.Windows.Forms.Form
    Private myPCAL As New PiKoderCommunicationAbstractionLayer

    ' declaration of variables
    Dim boolErrorFlag As Boolean ' global flag for errors in communication
    Dim IOSwitching As Boolean = False
    Dim bDataLoaded As Boolean = False ' flag to avoid data updates while uploading data from Pikoder 
    Dim bConnectCom As Boolean = False ' flag status of connection
    Dim bConnectWLAN As Boolean = False ' flag status of connection
    Dim sDefaultMinValue As String = "750" ' default values for USB2PMM
    Dim sDefaultMaxValue As String = "2250"
    Dim strPiKoderType As String = "" ' PiKoder type we are currently connected to
    Dim HPMath As Boolean = False ' indicating that high precision computing is required
    Dim iChannelSetting(8) As Integer ' contains the current output type (would be 1 for P(WM) and 2 for S(witch)


    ' declaration of subroutines
    ''' <summary>
    ''' This method is used to initialize the application. It will collect all
    ''' available COM ports in a list box. If no ports were found then the program
    ''' will stop.
    ''' </summary> 
    ''' <remarks></remarks>

    Public Sub New()

        InitializeComponent()
        ObtainCurrentSSID()
        boolErrorFlag = False

        ' Set active control 
        Me.ActiveControl = Me.AvailableCOMPorts

        ' Initialize LED
        Led2.Color = LED.LEDColorSelection.LED_Red
        Led2.Interval = 500
        Led2.State = True

    End Sub
    '****************************************************************************
    '   Function:
    '       private void UpdateCOMPortList()
    '
    '   Summary:
    '       This function updates the COM ports listbox.
    '
    '   Description:
    '       This function updates the COM ports listbox.  This function is launched 
    '       periodically based on its Interval attribute (set in the form editor under
    '       the properties window).
    '
    '   Precondition:
    '       None
    '
    '   Parameters:
    '       None
    '
    '   Return Values
    '       None
    '
    '   Remarks:
    '       None
    '***************************************************************************
    Private Sub UpdateCOMPortList()
        Dim s As String
        Dim i As Integer
        Dim k As Integer
        Dim foundDifference As Boolean
        Dim foundDifferenceHere As Boolean
        Dim iBufferSelectedIndex As Integer
        Dim myStringBuffer As String ' used for temporary storage of COM port designator
        ' define and initialize array for sorting COM ports 
        Dim myCOMPorts(99) As String ' assume we have a max of 100 COM Ports

        i = 0
        foundDifference = False
        iBufferSelectedIndex = AvailableCOMPorts.SelectedIndex 'buffer selection

        'If the number of COM ports is different than the last time we
        '  checked, then we know that the COM ports have changed and we
        '  don't need to verify each entry.

        If AvailableCOMPorts.Items.Count = SerialPort.GetPortNames().Length Then
            'Search the entire SerialPort object.  Look at COM port name
            '  returned and see if it already exists in the list.
            For Each s In SerialPort.GetPortNames()
                'If any of the names have changed then we need to update 
                '  the list
                k = 0
                foundDifferenceHere = True
                For k = 0 To SerialPort.GetPortNames().Length - 1
                    If AvailableCOMPorts.Items(k).Equals(s) = True Then
                        foundDifferenceHere = False
                    End If
                Next
                i = i + 1
                foundDifference = foundDifference Or foundDifferenceHere
            Next s
        Else
                foundDifference = True
        End If

        'If nothing has changed, exit the function.
        If foundDifference = False Then
            Exit Sub
        End If

        'If something has changed, then clear the list
        AvailableCOMPorts.Items.Clear()

        For Each sp As String In myCOMPorts
            sp = ""
        Next

        ' Connection setup - check for ports and display result
        For Each sp As String In My.Computer.Ports.SerialPortNames
            myStringBuffer = ""
            i = 0
            For intI = 0 To Len(sp) - 1
                If ((sp(intI) >= "A" And (sp(intI) <= "Z")) Or IsNumeric(sp(intI))) Then
                    myStringBuffer = myStringBuffer + sp(intI)
                    If IsNumeric(sp(intI)) Then i = (i * 10) + Val(sp(intI))
                End If
            Next
            myCOMPorts(i) = myStringBuffer
        Next

        For i = 1 To 99
            If myCOMPorts(i) <> "" Then AvailableCOMPorts.Items.Add(myCOMPorts(i))
        Next

        'Set the listbox to point to the first entry in the list
        AvailableCOMPorts.SelectedIndex = iBufferSelectedIndex
    End Sub


    '****************************************************************************
    '   Function:
    '       private void timer1_Tick(object sender, EventArgs e)
    '
    '   Summary:
    '       This function updates the COM ports listbox.
    '
    '   Description:
    '       This function updates the COM ports listbox.  This function is launched 
    '       periodically based on its Interval attribute (set in the form editor under
    '       the properties window).
    '
    '   Precondition:
    '       None
    '
    '   Parameters:
    '       object sender     - Sender of the event (this form)
    '       EventArgs e       - The event arguments
    '
    '   Return Values
    '       None
    '
    '   Remarks:
    '       None
    '***************************************************************************/

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If myPCAL.LinkConnected() Then
            'check if we still have a connection on USB system level
            If Not (myPCAL.PiKoderConnected()) Then
                LostConnection()
            End If
        Else
            'Update the COM ports list so that we can detect
            '  new COM ports that have been added.
            UpdateCOMPortList()
            'and update SSID 
            ObtainCurrentSSID()
        End If
    End Sub
    Private Sub IndicateConnectionOk()
        TextBox1.Text = "Parameters loaded ok."
        Led2.Color = LED.LEDColorSelection.LED_Green
        Led2.Blink = False ' indicate that the connection established
    End Sub
    Private Sub RetrievePiKoderType(ByRef SerialInputString As String)
        Dim strChannelBuffer As String = ""
        If myPCAL.LinkConnected() Then
            If Not boolErrorFlag Then
                'check for identifier
                Call myPCAL.GetStatusRecord(strChannelBuffer)
                If strChannelBuffer = "TimeOut" Then
                    boolErrorFlag = True
                ElseIf strChannelBuffer = "?" Then
                    boolErrorFlag = True
                Else
                    SerialInputString = strChannelBuffer
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' This method does evaluate the input fields and handles the data transfer to the PiKoder/COM 
    ''' upon hitting the 'sent new values to pikoder' - Button.
    ''' </summary>
    ''' <param name="eventSender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    ''' 
    Private Sub sentButton_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)
        RetrieveUART2PPMParameters()
    End Sub
    ''' <summary>
    ''' This method initiates the the saving of the configuration of the PiKoder/SSC 
    ''' upon hitting the 'safe Parameters' - Button.
    ''' </summary>
    ''' <param name="eventSender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    ''' 
    Private Sub saveButton_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles saveButton.Click
        Dim iRetCode As Integer
        iRetCode = myPCAL.SetPiKoderPreferences()
    End Sub
    ''' <summary>
    ''' Subroutine for starting heartbeat timer
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub startHeartBeat(ByVal heartBeatInterval As Integer)
        tHeartBeat.Enabled = True
        tHeartBeat.Interval = heartBeatInterval
    End Sub
    ''' <summary>
    ''' Subroutine for stopping heartbeat timer
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub stopHeartBeat()
        If tHeartBeat.Enabled = True Then
            tHeartBeat.Enabled = False
        End If
    End Sub
    ''' <summary>
    ''' Handle Heartbeat ticks: check connection
    ''' </summary>
    ''' <param name="eventSender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Private Sub tHeartBeat_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles tHeartBeat.Tick
        If myPCAL.PiKoderConnected() Then Exit Sub
        LostConnection()
    End Sub
    ''' <summary>
    ''' Set UI to indicate that we have lost connection - either on USB system level or due to a time out condition on application level
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CleanUpUI()
        ' indicate that connection is lost
        Led2.Color = LED.LEDColorSelection.LED_Red
        TextBox1.Text = ""
        stopHeartBeat()
        Timer1.Enabled = True
        TypeId.Text = ""    ' reset type information
        strPiKoderType = ""
        HPMath = False
        bDataLoaded = False

        ' delete text fields
        strCH_1_Current.Text = ""
        strCH_2_Current.Text = ""
        strCH_3_Current.Text = ""
        strCH_4_Current.Text = ""
        strCH_5_Current.Text = ""
        strCH_6_Current.Text = ""
        strCH_7_Current.Text = ""
        strCH_8_Current.Text = ""

        'set scroll bars to neutral
        ch1_HScrollBar.Value = (ch1_HScrollBar.Maximum + ch1_HScrollBar.Minimum) / 2
        ch2_HScrollBar.Value = (ch2_HScrollBar.Maximum + ch2_HScrollBar.Minimum) / 2
        ch3_HScrollBar.Value = (ch3_HScrollBar.Maximum + ch3_HScrollBar.Minimum) / 2
        ch4_HScrollBar.Value = (ch4_HScrollBar.Maximum + ch4_HScrollBar.Minimum) / 2
        ch5_HScrollBar.Value = (ch5_HScrollBar.Maximum + ch5_HScrollBar.Minimum) / 2
        ch6_HScrollBar.Value = (ch6_HScrollBar.Maximum + ch6_HScrollBar.Minimum) / 2
        ch7_HScrollBar.Value = (ch7_HScrollBar.Maximum + ch7_HScrollBar.Minimum) / 2
        ch8_HScrollBar.Value = (ch8_HScrollBar.Maximum + ch8_HScrollBar.Minimum) / 2

        strCH_1_Neutral.ForeColor = Color.White
        strCH_2_Neutral.ForeColor = Color.White
        strCH_3_Neutral.ForeColor = Color.White
        strCH_4_Neutral.ForeColor = Color.White
        strCH_5_Neutral.ForeColor = Color.White
        strCH_6_Neutral.ForeColor = Color.White
        strCH_7_Neutral.ForeColor = Color.White
        strCH_8_Neutral.ForeColor = Color.White

        strCH_1_Min.ForeColor = Color.White
        strCH_2_Min.ForeColor = Color.White
        strCH_3_Min.ForeColor = Color.White
        strCH_4_Min.ForeColor = Color.White
        strCH_5_Min.ForeColor = Color.White
        strCH_6_Min.ForeColor = Color.White
        strCH_7_Min.ForeColor = Color.White
        strCH_8_Min.ForeColor = Color.White

        strCH_1_Max.ForeColor = Color.White
        strCH_2_Max.ForeColor = Color.White
        strCH_3_Max.ForeColor = Color.White
        strCH_4_Max.ForeColor = Color.White
        strCH_5_Max.ForeColor = Color.White
        strCH_6_Max.ForeColor = Color.White
        strCH_7_Max.ForeColor = Color.White
        strCH_8_Max.ForeColor = Color.White

        TimeOut.ForeColor = Color.White
        strSSC_Firmware.Text = " "

        miniSSCOffset.ForeColor = Color.White

        PPM_Channels.ForeColor = Color.White
        PPM_Mode.ForeColor = Color.White

        ListBox1.ForeColor = Color.White
        ListBox2.ForeColor = Color.White
        ListBox3.ForeColor = Color.White
        ListBox4.ForeColor = Color.White
        ListBox5.ForeColor = Color.White
        ListBox6.ForeColor = Color.White
        ListBox7.ForeColor = Color.White
        ListBox8.ForeColor = Color.White

        ' close port
        myPCAL.MyForm_Dispose()
    End Sub
    Private Sub LostConnection()
        If ConnectCOM.Checked Then
            ConnectCOM.Checked = False 'this will take care of the CleanUp of the UI
            TextBox1.Text = "Lost connection to PiKoder."
        Else
        End If
    End Sub
    Private Sub RetrieveUART2PPMParameters()

        Dim strChannelBuffer As String = ""

        bDataLoaded = False

        'Make sure that all required fields are enabled
        GroupBox8.Enabled = True
        GroupBox8.Visible = True
        GroupBox4.Enabled = True
        GroupBox4.Visible = True
        GroupBox7.Enabled = True 'Save Parameters
        GroupBox7.Visible = True

        If myPCAL.LinkConnected() Then
            If Not boolErrorFlag Then
                'request status information from SSC    
                Call myPCAL.GetFirmwareVersion(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strSSC_Firmware.Text = strChannelBuffer
                    If Val(strChannelBuffer) > 2.06 Then
                        MsgBox("The PiKoder firmware version found is not supported! Please goto www.pikoder.com and upgrade PCC Control Center to the latest version.", MsgBoxStyle.OkOnly, "Error Message")
                        End
                    End If
                Else ' error message
                    boolErrorFlag = True
                End If
            End If

            'retrieve information for channel 1
            Call RetrieveChannel1Information()

            'retrieve information for channel 2
            Call RetrieveChannel2Information()

            'retrieve information for channel 3
            Call RetrieveChannel3Information()

            'retrieve information for channel 4
            Call RetrieveChannel4Information()

            'retrieve information for channel 5
            Call RetrieveChannel5Information()

            'retrieve information for channel 6
            Call RetrieveChannel6Information()

            'retrieve information for channel 7
            Call RetrieveChannel7Information()

            'retrieve information for channel 8
            Call RetrieveChannel8Information()

            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 1)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_1_Neutral.Maximum = 2500
                    strCH_1_Neutral.Value = Val(strChannelBuffer)
                    strCH_1_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 2)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_2_Neutral.Maximum = 2500
                    strCH_2_Neutral.Value = Val(strChannelBuffer)
                    strCH_2_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 3)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_3_Neutral.Maximum = 2500
                    strCH_3_Neutral.Value = Val(strChannelBuffer)
                    strCH_3_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 4)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_4_Neutral.Maximum = 2500
                    strCH_4_Neutral.Value = Val(strChannelBuffer)
                    strCH_4_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 5)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_5_Neutral.Maximum = 2500
                    strCH_5_Neutral.Value = Val(strChannelBuffer)
                    strCH_5_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 6)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_6_Neutral.Maximum = 2500
                    strCH_6_Neutral.Value = Val(strChannelBuffer)
                    strCH_6_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 7)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_7_Neutral.Maximum = 2500
                    strCH_7_Neutral.Value = Val(strChannelBuffer)
                    strCH_7_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_8_Neutral.Maximum = 2500
                    strCH_8_Neutral.Value = Val(strChannelBuffer)
                    strCH_8_Neutral.ForeColor = Color.Black
                End If
            End If
            'retrieve min & max information for all channels
            'channel 1
            If Not boolErrorFlag Then
                Call myPCAL.GetLowerLimit(strChannelBuffer, 1)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_1_Min.Value = Val(strChannelBuffer)
                    ch1_HScrollBar.Minimum = strCH_1_Min.Value
                    strCH_1_Min.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetUpperLimit(strChannelBuffer, 1)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_1_Max.Value = Val(strChannelBuffer)
                    ch1_HScrollBar.Maximum = strCH_1_Max.Value
                    strCH_1_Max.ForeColor = Color.Black
                End If
            End If
        End If

        'channel 2
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 2)
            If strChannelBuffer <> "TimeOut" Then
                strCH_2_Min.Value = Val(strChannelBuffer)
                ch2_HScrollBar.Minimum = strCH_2_Min.Value
                strCH_2_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 2)
            If strChannelBuffer <> "TimeOut" Then
                strCH_2_Max.Value = Val(strChannelBuffer)
                ch2_HScrollBar.Maximum = strCH_2_Max.Value
                strCH_2_Max.ForeColor = Color.Black
            End If
        End If

        'channel 3
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 3)
            If strChannelBuffer <> "TimeOut" Then
                strCH_3_Min.Value = Val(strChannelBuffer)
                ch3_HScrollBar.Minimum = strCH_3_Min.Value
                strCH_3_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 3)
            If strChannelBuffer <> "TimeOut" Then
                strCH_3_Max.Value = Val(strChannelBuffer)
                ch3_HScrollBar.Maximum = strCH_3_Max.Value
                strCH_3_Max.ForeColor = Color.Black
            End If
        End If

        'channel 4
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 4)
            If strChannelBuffer <> "TimeOut" Then
                strCH_4_Min.Value = Val(strChannelBuffer)
                ch4_HScrollBar.Minimum = strCH_4_Min.Value
                strCH_4_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 4)
            If strChannelBuffer <> "TimeOut" Then
                strCH_4_Max.Value = Val(strChannelBuffer)
                ch4_HScrollBar.Maximum = strCH_4_Max.Value
                strCH_4_Max.ForeColor = Color.Black
            End If
        End If

        'channel 5
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 5)
            If strChannelBuffer <> "TimeOut" Then
                strCH_5_Min.Value = Val(strChannelBuffer)
                ch5_HScrollBar.Minimum = strCH_5_Min.Value
                strCH_5_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 5)
            If strChannelBuffer <> "TimeOut" Then
                strCH_5_Max.Value = Val(strChannelBuffer)
                ch5_HScrollBar.Maximum = strCH_5_Max.Value
                strCH_5_Max.ForeColor = Color.Black
            End If
        End If

        'channel 6
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 6)
            If strChannelBuffer <> "TimeOut" Then
                strCH_6_Min.Value = Val(strChannelBuffer)
                ch6_HScrollBar.Minimum = strCH_6_Min.Value
                strCH_6_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 6)
            If strChannelBuffer <> "TimeOut" Then
                strCH_6_Max.Value = Val(strChannelBuffer)
                ch6_HScrollBar.Maximum = strCH_6_Max.Value
                strCH_6_Max.ForeColor = Color.Black
            End If
        End If

        'channel 7
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 7)
            If strChannelBuffer <> "TimeOut" Then
                strCH_7_Min.Value = Val(strChannelBuffer)
                ch7_HScrollBar.Minimum = strCH_7_Min.Value
                strCH_7_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 7)
            If strChannelBuffer <> "TimeOut" Then
                strCH_7_Max.Value = Val(strChannelBuffer)
                ch7_HScrollBar.Maximum = strCH_7_Max.Value
                strCH_7_Max.ForeColor = Color.Black
            End If
        End If

        'channel 8
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                strCH_8_Min.Value = Val(strChannelBuffer)
                ch8_HScrollBar.Minimum = strCH_8_Min.Value
                strCH_8_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                strCH_8_Max.Value = Val(strChannelBuffer)
                ch8_HScrollBar.Maximum = strCH_8_Max.Value
                strCH_8_Max.ForeColor = Color.Black
            End If

        End If

        'retrieve TimeOut
        If Not boolErrorFlag Then
            Call myPCAL.GetTimeOut(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                TimeOut.Value = Val(strChannelBuffer)
                TimeOut.ForeColor = Color.Black
            End If
        End If

        'retrieve miniSSC offset
        If Not boolErrorFlag Then
            Call myPCAL.GetMiniSSCOffset(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                miniSSCOffset.Value = Val(strChannelBuffer)
                miniSSCOffset.ForeColor = Color.Black
            End If
        End If

        GroupBox13.Enabled = False
        GroupBox13.Visible = False
        GroupBox17.Enabled = False
        GroupBox17.Visible = False

        bDataLoaded = True
    End Sub
    Private Sub RetrieveSSC_HPParameters()

        Dim strChannelBuffer As String = ""

        bDataLoaded = False

        'Make sure that all required fields are enabled
        GroupBox8.Enabled = True
        GroupBox8.Visible = True
        GroupBox4.Enabled = True
        GroupBox4.Visible = True
        GroupBox7.Enabled = True 'Save Parameters
        GroupBox7.Visible = True

        IOSwitching = False 'Better safe than sorry
        bDataLoaded = False 'Avoid overridding of channel type due to re-reading data after value change

        If myPCAL.LinkConnected() Then
            If Not boolErrorFlag Then
                'request status information from SSC    
                Call myPCAL.GetFirmwareVersion(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strSSC_Firmware.Text = strChannelBuffer
                    If Val(strChannelBuffer) > 2.04 Then
                        MsgBox("The PiKoder firmware version found is not supported! Please goto www.pikoder.com and upgrade PCC Control Center to the latest version.", MsgBoxStyle.OkOnly, "Error Message")
                        End
                    ElseIf Val(strChannelBuffer) = 2.04 Then
                        IOSwitching = True
                    ElseIf Val(strChannelBuffer) < 2.03 Then
                        MsgBox("The PiKoder firmware version found is not supported! Please goto www.pikoder.com and upgrade the PiKoder firmware to the latest version.", MsgBoxStyle.OkOnly, "Error Message")
                        End
                    End If
                Else ' error message
                    boolErrorFlag = True
                End If
            End If

            'retrieve min & max information for all channels and set params
            'channel 1
            If Not boolErrorFlag Then
                Call myPCAL.GetLowerLimit(strChannelBuffer, 1)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_1_Min.Value = Val(strChannelBuffer) / 5
                    ch1_HScrollBar.Minimum = strCH_1_Min.Value
                    strCH_1_Min.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetUpperLimit(strChannelBuffer, 1)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_1_Max.Value = Val(strChannelBuffer) / 5
                    ch1_HScrollBar.Maximum = strCH_1_Max.Value
                    strCH_1_Max.ForeColor = Color.Black
                End If
            End If
        End If

        'channel 2
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 2)
            If strChannelBuffer <> "TimeOut" Then
                strCH_2_Min.Value = Val(strChannelBuffer) / 5
                ch2_HScrollBar.Minimum = strCH_2_Min.Value
                strCH_2_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 2)
            If strChannelBuffer <> "TimeOut" Then
                strCH_2_Max.Value = Val(strChannelBuffer) / 5
                ch2_HScrollBar.Maximum = strCH_2_Max.Value
                strCH_2_Max.ForeColor = Color.Black
            End If
        End If

        'channel 3
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 3)
            If strChannelBuffer <> "TimeOut" Then
                strCH_3_Min.Value = Val(strChannelBuffer) / 5
                ch3_HScrollBar.Minimum = strCH_3_Min.Value
                strCH_3_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 3)
            If strChannelBuffer <> "TimeOut" Then
                strCH_3_Max.Value = Val(strChannelBuffer) / 5
                ch3_HScrollBar.Maximum = strCH_3_Max.Value
                strCH_3_Max.ForeColor = Color.Black
            End If
        End If

        'channel 4
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 4)
            If strChannelBuffer <> "TimeOut" Then
                strCH_4_Min.Value = Val(strChannelBuffer) / 5
                ch4_HScrollBar.Minimum = strCH_4_Min.Value
                strCH_4_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 4)
            If strChannelBuffer <> "TimeOut" Then
                strCH_4_Max.Value = Val(strChannelBuffer) / 5
                ch4_HScrollBar.Maximum = strCH_4_Max.Value
                strCH_4_Max.ForeColor = Color.Black
            End If
        End If

        'channel 5
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 5)
            If strChannelBuffer <> "TimeOut" Then
                strCH_5_Min.Value = Val(strChannelBuffer) / 5
                ch5_HScrollBar.Minimum = strCH_5_Min.Value
                strCH_5_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 5)
            If strChannelBuffer <> "TimeOut" Then
                strCH_5_Max.Value = Val(strChannelBuffer) / 5
                ch5_HScrollBar.Maximum = strCH_5_Max.Value
                strCH_5_Max.ForeColor = Color.Black
            End If
        End If

        'channel 6
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 6)
            If strChannelBuffer <> "TimeOut" Then
                strCH_6_Min.Value = Val(strChannelBuffer) / 5
                ch6_HScrollBar.Minimum = strCH_6_Min.Value
                strCH_6_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 6)
            If strChannelBuffer <> "TimeOut" Then
                strCH_6_Max.Value = Val(strChannelBuffer) / 5
                ch6_HScrollBar.Maximum = strCH_6_Max.Value
                strCH_6_Max.ForeColor = Color.Black
            End If
        End If

        'channel 7
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 7)
            If strChannelBuffer <> "TimeOut" Then
                strCH_7_Min.Value = Val(strChannelBuffer) / 5
                ch7_HScrollBar.Minimum = strCH_7_Min.Value
                strCH_7_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 7)
            If strChannelBuffer <> "TimeOut" Then
                strCH_7_Max.Value = Val(strChannelBuffer) / 5
                ch7_HScrollBar.Maximum = strCH_7_Max.Value
                strCH_7_Max.ForeColor = Color.Black
            End If
        End If

        'channel 8
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                strCH_8_Min.Value = Val(strChannelBuffer) / 5
                ch8_HScrollBar.Minimum = strCH_8_Min.Value
                strCH_8_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                strCH_8_Max.Value = Val(strChannelBuffer) / 5
                ch8_HScrollBar.Maximum = strCH_8_Max.Value
                strCH_8_Max.ForeColor = Color.Black
            End If

        End If

        If Not boolErrorFlag Then
            Call myPCAL.GetNeutralPosition(strChannelBuffer, 1)
            If strChannelBuffer <> "TimeOut" Then
                strCH_1_Neutral.Maximum = strCH_1_Max.Value
                strCH_1_Neutral.Value = Val(strChannelBuffer) / 5
                strCH_1_Neutral.ForeColor = Color.Black
            End If
        End If

        If Not boolErrorFlag Then
            Call myPCAL.GetNeutralPosition(strChannelBuffer, 2)
            If strChannelBuffer <> "TimeOut" Then
                strCH_2_Neutral.Maximum = strCH_2_Max.Value
                strCH_2_Neutral.Value = Val(strChannelBuffer) / 5
                strCH_2_Neutral.ForeColor = Color.Black
            End If
        End If

        If Not boolErrorFlag Then
            Call myPCAL.GetNeutralPosition(strChannelBuffer, 3)
            If strChannelBuffer <> "TimeOut" Then
                strCH_3_Neutral.Maximum = strCH_3_Max.Value
                strCH_3_Neutral.Value = Val(strChannelBuffer) / 5
                strCH_3_Neutral.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetNeutralPosition(strChannelBuffer, 4)
            If strChannelBuffer <> "TimeOut" Then
                strCH_4_Neutral.Maximum = strCH_4_Max.Value
                strCH_4_Neutral.Value = Val(strChannelBuffer) / 5
                strCH_4_Neutral.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetNeutralPosition(strChannelBuffer, 5)
            If strChannelBuffer <> "TimeOut" Then
                strCH_5_Neutral.Maximum = strCH_5_Max.Value
                strCH_5_Neutral.Value = Val(strChannelBuffer) / 5
                strCH_5_Neutral.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetNeutralPosition(strChannelBuffer, 6)
            If strChannelBuffer <> "TimeOut" Then
                strCH_6_Neutral.Maximum = strCH_6_Max.Value
                strCH_6_Neutral.Value = Val(strChannelBuffer) / 5
                strCH_6_Neutral.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetNeutralPosition(strChannelBuffer, 7)
            If strChannelBuffer <> "TimeOut" Then
                strCH_7_Neutral.Maximum = strCH_7_Max.Value
                strCH_7_Neutral.Value = Val(strChannelBuffer) / 5
                strCH_7_Neutral.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                strCH_8_Neutral.Maximum = strCH_8_Max.Value
                strCH_8_Neutral.Value = Val(strChannelBuffer) / 5
                strCH_8_Neutral.ForeColor = Color.Black
            End If
        End If

        'retrieve information for channel 1
        Call RetrieveChannel1HPInformation()

        'retrieve information for channel 2
        Call RetrieveChannel2HPInformation()

        'retrieve information for channel 3
        Call RetrieveChannel3HPInformation()

        'retrieve information for channel 4
        Call RetrieveChannel4HPInformation()

        'retrieve information for channel 5
        Call RetrieveChannel5HPInformation()

        'retrieve information for channel 6
        Call RetrieveChannel6HPInformation()

        'retrieve information for channel 7
        Call RetrieveChannel7HPInformation()

        'retrieve information for channel 8
        Call RetrieveChannel8HPInformation()

        'retrieve TimeOut
        If Not boolErrorFlag Then
            Call myPCAL.GetTimeOut(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                TimeOut.Value = Val(strChannelBuffer)
                TimeOut.ForeColor = Color.Black
            End If
        End If

        'retrieve miniSSC offset
        If Not boolErrorFlag Then
            Call myPCAL.GetMiniSSCOffset(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                miniSSCOffset.Value = Val(strChannelBuffer)
                miniSSCOffset.ForeColor = Color.Black
            End If
        End If

        If IOSwitching Then
            Call ReadIOSwitching()
        End If

        GroupBox13.Enabled = False '# PPM Channels
        GroupBox13.Visible = False
        GroupBox17.Enabled = False 'PPM mode
        GroupBox17.Visible = False

        bDataLoaded = True
    End Sub
    Private Sub RetrieveSSCParameters()

        Dim strChannelBuffer As String = ""

        bDataLoaded = False

        'Make sure that all required fields are enabled
        GroupBox8.Enabled = True
        GroupBox8.Visible = True
        GroupBox4.Enabled = True
        GroupBox4.Visible = True
        GroupBox7.Enabled = True 'Save Parameters
        GroupBox7.Visible = True

        IOSwitching = False 'Better safe than sorry
        bDataLoaded = False 'Avoid overridding of channel type due to re-reading data after value change

        If myPCAL.LinkConnected() Then
            If Not boolErrorFlag Then
                'request status information from SSC    
                Call myPCAL.GetFirmwareVersion(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strSSC_Firmware.Text = strChannelBuffer
                    If Val(strChannelBuffer) > 2.08 Then
                        MsgBox("The PiKoder firmware version found is not supported! Please goto www.pikoder.com and upgrade PCC Control Center to the latest version.", MsgBoxStyle.OkOnly, "Error Message")
                        End
                    ElseIf Val(strChannelBuffer) >= 2.07 Then
                        IOSwitching = True
                    ElseIf Val(strChannelBuffer) < 2.0 Then
                        MsgBox("The PiKoder firmware version found is not supported! Please goto www.pikoder.com and upgrade the PiKoder firmware to the latest version.", MsgBoxStyle.OkOnly, "Error Message")
                        End
                    End If
                Else ' error message
                    boolErrorFlag = True
                End If
            End If

            'retrieve information for channel 1
            Call RetrieveChannel1Information()

            'retrieve information for channel 2
            Call RetrieveChannel2Information()

            'retrieve information for channel 3
            Call RetrieveChannel3Information()

            'retrieve information for channel 4
            Call RetrieveChannel4Information()

            'retrieve information for channel 5
            Call RetrieveChannel5Information()

            'retrieve information for channel 6
            Call RetrieveChannel6Information()

            'retrieve information for channel 7
            Call RetrieveChannel7Information()

            'retrieve information for channel 8
            Call RetrieveChannel8Information()

            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 1)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_1_Neutral.Maximum = 2500
                    strCH_1_Neutral.Value = Val(strChannelBuffer)
                    strCH_1_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 2)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_2_Neutral.Maximum = 2500
                    strCH_2_Neutral.Value = Val(strChannelBuffer)
                    strCH_2_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 3)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_3_Neutral.Maximum = 2500
                    strCH_3_Neutral.Value = Val(strChannelBuffer)
                    strCH_3_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 4)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_4_Neutral.Maximum = 2500
                    strCH_4_Neutral.Value = Val(strChannelBuffer)
                    strCH_4_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 5)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_5_Neutral.Maximum = 2500
                    strCH_5_Neutral.Value = Val(strChannelBuffer)
                    strCH_5_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 6)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_6_Neutral.Maximum = 2500
                    strCH_6_Neutral.Value = Val(strChannelBuffer)
                    strCH_6_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 7)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_7_Neutral.Maximum = 2500
                    strCH_7_Neutral.Value = Val(strChannelBuffer)
                    strCH_7_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_8_Neutral.Maximum = 2500
                    strCH_8_Neutral.Value = Val(strChannelBuffer)
                    strCH_8_Neutral.ForeColor = Color.Black
                End If
            End If
            'retrieve min & max information for all channels
            'channel 1
            If Not boolErrorFlag Then
                Call myPCAL.GetLowerLimit(strChannelBuffer, 1)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_1_Min.Value = Val(strChannelBuffer)
                    ch1_HScrollBar.Minimum = strCH_1_Min.Value
                    strCH_1_Min.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call myPCAL.GetUpperLimit(strChannelBuffer, 1)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_1_Max.Value = Val(strChannelBuffer)
                    ch1_HScrollBar.Maximum = strCH_1_Max.Value
                    strCH_1_Max.ForeColor = Color.Black
                End If
            End If
        End If

        'channel 2
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 2)
            If strChannelBuffer <> "TimeOut" Then
                strCH_2_Min.Value = Val(strChannelBuffer)
                ch2_HScrollBar.Minimum = strCH_2_Min.Value
                strCH_2_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 2)
            If strChannelBuffer <> "TimeOut" Then
                strCH_2_Max.Value = Val(strChannelBuffer)
                ch2_HScrollBar.Maximum = strCH_2_Max.Value
                strCH_2_Max.ForeColor = Color.Black
            End If
        End If

        'channel 3
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 3)
            If strChannelBuffer <> "TimeOut" Then
                strCH_3_Min.Value = Val(strChannelBuffer)
                ch3_HScrollBar.Minimum = strCH_3_Min.Value
                strCH_3_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 3)
            If strChannelBuffer <> "TimeOut" Then
                strCH_3_Max.Value = Val(strChannelBuffer)
                ch3_HScrollBar.Maximum = strCH_3_Max.Value
                strCH_3_Max.ForeColor = Color.Black
            End If
        End If

        'channel 4
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 4)
            If strChannelBuffer <> "TimeOut" Then
                strCH_4_Min.Value = Val(strChannelBuffer)
                ch4_HScrollBar.Minimum = strCH_4_Min.Value
                strCH_4_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 4)
            If strChannelBuffer <> "TimeOut" Then
                strCH_4_Max.Value = Val(strChannelBuffer)
                ch4_HScrollBar.Maximum = strCH_4_Max.Value
                strCH_4_Max.ForeColor = Color.Black
            End If
        End If

        'channel 5
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 5)
            If strChannelBuffer <> "TimeOut" Then
                strCH_5_Min.Value = Val(strChannelBuffer)
                ch5_HScrollBar.Minimum = strCH_5_Min.Value
                strCH_5_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 5)
            If strChannelBuffer <> "TimeOut" Then
                strCH_5_Max.Value = Val(strChannelBuffer)
                ch5_HScrollBar.Maximum = strCH_5_Max.Value
                strCH_5_Max.ForeColor = Color.Black
            End If
        End If

        'channel 6
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 6)
            If strChannelBuffer <> "TimeOut" Then
                strCH_6_Min.Value = Val(strChannelBuffer)
                ch6_HScrollBar.Minimum = strCH_6_Min.Value
                strCH_6_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 6)
            If strChannelBuffer <> "TimeOut" Then
                strCH_6_Max.Value = Val(strChannelBuffer)
                ch6_HScrollBar.Maximum = strCH_6_Max.Value
                strCH_6_Max.ForeColor = Color.Black
            End If
        End If

        'channel 7
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 7)
            If strChannelBuffer <> "TimeOut" Then
                strCH_7_Min.Value = Val(strChannelBuffer)
                ch7_HScrollBar.Minimum = strCH_7_Min.Value
                strCH_7_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 7)
            If strChannelBuffer <> "TimeOut" Then
                strCH_7_Max.Value = Val(strChannelBuffer)
                ch7_HScrollBar.Maximum = strCH_7_Max.Value
                strCH_7_Max.ForeColor = Color.Black
            End If
        End If

        'channel 8
        If Not boolErrorFlag Then
            Call myPCAL.GetLowerLimit(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                strCH_8_Min.Value = Val(strChannelBuffer)
                ch8_HScrollBar.Minimum = strCH_8_Min.Value
                strCH_8_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call myPCAL.GetUpperLimit(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                strCH_8_Max.Value = Val(strChannelBuffer)
                ch8_HScrollBar.Maximum = strCH_8_Max.Value
                strCH_8_Max.ForeColor = Color.Black
            End If

        End If

        'retrieve TimeOut
        If Not boolErrorFlag Then
            Call myPCAL.GetTimeOut(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                TimeOut.Value = Val(strChannelBuffer)
                TimeOut.ForeColor = Color.Black
            End If
        End If

        'retrieve miniSSC offset
        If Not boolErrorFlag Then
            Call myPCAL.GetMiniSSCOffset(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                miniSSCOffset.Value = Val(strChannelBuffer)
                miniSSCOffset.ForeColor = Color.Black
            End If
        End If

        If IOSwitching Then
            Call ReadIOSwitching()
        End If

        GroupBox13.Enabled = False '# PPM Channels
        GroupBox13.Visible = False
        GroupBox17.Enabled = False 'PPM mode
        GroupBox17.Visible = False

        bDataLoaded = True

    End Sub
    Private Sub RetrieveUSB2PPMParameters()

        Dim strChannelBuffer As String = ""

        bDataLoaded = False

        GroupBox13.Enabled = True
        GroupBox13.Visible = True
        GroupBox17.Enabled = True
        GroupBox17.Visible = True

        If myPCAL.LinkConnected() Then
            If Not boolErrorFlag Then
                'request firm ware version from PiKoder    
                Call myPCAL.GetFirmwareVersion(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strSSC_Firmware.Text = strChannelBuffer
                    If Val(strChannelBuffer) > 2.0 Then
                        MsgBox("The PiKoder firmware version found is not supported! Please goto www.pikoder.com and upgrade PCC PiKoder Control Center to the latest version.", MsgBoxStyle.OkOnly, "Error Message")
                        End
                    ElseIf Val(strChannelBuffer) < 1.02 Then
                        MsgBox("The PiKoder firmware version found is not supported! Please goto www.pikoder.com and upgrade the PiKoder firmware to the latest version.", MsgBoxStyle.OkOnly, "Error Message")
                        End
                    End If
                Else ' error message
                    boolErrorFlag = True
                End If
            End If

            'retrieve information for channel 1
            Call RetrieveChannel1Information()

            'retrieve information for channel 2
            Call RetrieveChannel2Information()

            'retrieve information for channel 3
            Call RetrieveChannel3Information()

            'retrieve information for channel 4
            Call RetrieveChannel4Information()

            'retrieve information for channel 5
            Call RetrieveChannel5Information()

            'retrieve information for channel 6
            Call RetrieveChannel6Information()

            'retrieve information for channel 7
            Call RetrieveChannel7Information()

            'retrieve information for channel 8
            Call RetrieveChannel8Information()

            'set min & max information for all channels
            strCH_1_Min.Value = sDefaultMinValue
            ch1_HScrollBar.Minimum = strCH_1_Min.Value
            strCH_1_Min.ForeColor = Color.Black
            strCH_1_Max.Value = sDefaultMaxValue
            ch1_HScrollBar.Maximum = strCH_1_Max.Value
            strCH_1_Max.ForeColor = Color.Black

            strCH_2_Min.Value = sDefaultMinValue
            ch2_HScrollBar.Minimum = strCH_2_Min.Value
            strCH_2_Min.ForeColor = Color.Black
            strCH_2_Max.Value = sDefaultMaxValue
            ch2_HScrollBar.Maximum = strCH_2_Max.Value
            strCH_2_Max.ForeColor = Color.Black

            strCH_3_Min.Value = sDefaultMinValue
            ch3_HScrollBar.Minimum = strCH_3_Min.Value
            strCH_3_Min.ForeColor = Color.Black
            strCH_3_Max.Value = sDefaultMaxValue
            ch3_HScrollBar.Maximum = strCH_3_Max.Value
            strCH_3_Max.ForeColor = Color.Black

            strCH_4_Min.Value = sDefaultMinValue
            ch4_HScrollBar.Minimum = strCH_4_Min.Value
            strCH_4_Min.ForeColor = Color.Black
            strCH_4_Max.Value = sDefaultMaxValue
            ch4_HScrollBar.Maximum = strCH_4_Max.Value
            strCH_4_Max.ForeColor = Color.Black

            strCH_5_Min.Value = sDefaultMinValue
            ch5_HScrollBar.Minimum = strCH_5_Min.Value
            strCH_5_Min.ForeColor = Color.Black
            strCH_5_Max.Value = sDefaultMaxValue
            ch5_HScrollBar.Maximum = strCH_5_Max.Value
            strCH_5_Max.ForeColor = Color.Black

            strCH_6_Min.Value = sDefaultMinValue
            ch6_HScrollBar.Minimum = strCH_6_Min.Value
            strCH_6_Min.ForeColor = Color.Black
            strCH_6_Max.Value = sDefaultMaxValue
            ch6_HScrollBar.Maximum = strCH_6_Max.Value
            strCH_6_Max.ForeColor = Color.Black

            strCH_7_Min.Value = sDefaultMinValue
            ch7_HScrollBar.Minimum = strCH_7_Min.Value
            strCH_7_Min.ForeColor = Color.Black
            strCH_7_Max.Value = sDefaultMaxValue
            ch7_HScrollBar.Maximum = strCH_7_Max.Value
            strCH_7_Max.ForeColor = Color.Black

            strCH_8_Min.Value = sDefaultMinValue
            ch8_HScrollBar.Minimum = strCH_8_Min.Value
            strCH_8_Min.ForeColor = Color.Black
            strCH_8_Max.Value = sDefaultMaxValue
            ch8_HScrollBar.Maximum = strCH_8_Max.Value
            strCH_8_Max.ForeColor = Color.Black

            GroupBox11.Enabled = False 'neutral positions
            GroupBox11.Visible = False
            GroupBox8.Enabled = False 'miniSSC Offset
            GroupBox8.Visible = False
            GroupBox4.Enabled = False 'zero offset
            GroupBox4.Visible = False
            GroupBox7.Enabled = False 'Save Parameters
            GroupBox7.Visible = False

            PPM_Channels.Value = 8
            PPM_Channels.ForeColor = Color.Black
            PPM_Mode.Items.Add("positive")
            PPM_Mode.Items.Add("negative (Futaba)")
            PPM_Mode.ForeColor = Color.Black
        End If
    End Sub
    Private Function FormatChannelValue(ByVal iChannelInput) As String
        Dim strChannelBuffer = ""
        Dim iChannelValue = iChannelInput
        If (HPMath) Then
            iChannelValue = iChannelValue * 5
            If (iChannelValue < 10000) Then strChannelBuffer = strChannelBuffer + "0"
        End If
        If (iChannelValue < 1000) Then strChannelBuffer = strChannelBuffer + "0"
        Return strChannelBuffer + Convert.ToString(iChannelValue)
    End Function
    Private Sub strCH_1_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_1_Neutral.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                Call myPCAL.SetChannelNeutral(FormatChannelValue(strCH_1_Neutral.Value), 1)
            End If
        End If
    End Sub
    Private Sub strCH_2_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_2_Neutral.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                Call myPCAL.SetChannelNeutral(FormatChannelValue(strCH_2_Neutral.Value), 2)
            End If
        End If
    End Sub
    Private Sub strCH_3_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_3_Neutral.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                Call myPCAL.SetChannelNeutral(FormatChannelValue(strCH_3_Neutral.Value), 3)
            End If
        End If
    End Sub
    Private Sub strCH_4_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_4_Neutral.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                Call myPCAL.SetChannelNeutral(FormatChannelValue(strCH_4_Neutral.Value), 4)
            End If
        End If
    End Sub
    Private Sub strCH_5_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_5_Neutral.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                Call myPCAL.SetChannelNeutral(FormatChannelValue(strCH_5_Neutral.Value), 5)
            End If
        End If
    End Sub
    Private Sub strCH_6_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_6_Neutral.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                Call myPCAL.SetChannelNeutral(FormatChannelValue(strCH_6_Neutral.Value), 6)
            End If
        End If
    End Sub
    Private Sub strCH_7_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_7_Neutral.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                Call myPCAL.SetChannelNeutral(FormatChannelValue(strCH_7_Neutral.Value), 7)
            End If
        End If
    End Sub
    Private Sub strCH_8_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_8_Neutral.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                Call myPCAL.SetChannelNeutral(FormatChannelValue(strCH_7_Neutral.Value), 7)
            End If
        End If
    End Sub
    Private Sub strCH_1_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_1_Min.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch1_HScrollBar.Value < strCH_1_Min.Value Then
                    ch1_HScrollBar.Value = strCH_1_Min.Value
                    strCH_1_Current.Text = Convert.ToString(ch1_HScrollBar.Value)
                End If
                ch1_HScrollBar.Minimum = strCH_1_Min.Value
                Call myPCAL.SetChannelLowerLimit(FormatChannelValue(strCH_1_Min.Value), 1)
            End If
        End If
    End Sub
    Private Sub strCH_2_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_2_Min.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch2_HScrollBar.Value < strCH_2_Min.Value Then
                    ch2_HScrollBar.Value = strCH_2_Min.Value
                    strCH_2_Current.Text = Convert.ToString(ch2_HScrollBar.Value)
                End If
                ch2_HScrollBar.Minimum = strCH_2_Min.Value
                Call myPCAL.SetChannelLowerLimit(FormatChannelValue(strCH_2_Min.Value), 2)
            End If
        End If
    End Sub
    Private Sub strCH_3_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_3_Min.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch3_HScrollBar.Value < strCH_3_Min.Value Then
                    ch3_HScrollBar.Value = strCH_3_Min.Value
                    strCH_3_Current.Text = Convert.ToString(ch3_HScrollBar.Value)
                End If
                ch3_HScrollBar.Minimum = strCH_3_Min.Value
                Call myPCAL.SetChannelLowerLimit(FormatChannelValue(strCH_3_Min.Value), 3)
            End If
        End If
    End Sub
    Private Sub strCH_4_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_4_Min.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch4_HScrollBar.Value < strCH_4_Min.Value Then
                    ch4_HScrollBar.Value = strCH_4_Min.Value
                    strCH_4_Current.Text = Convert.ToString(ch4_HScrollBar.Value)
                End If
                ch4_HScrollBar.Minimum = strCH_4_Min.Value
                Call myPCAL.SetChannelLowerLimit(FormatChannelValue(strCH_4_Min.Value), 4)
            End If
        End If
    End Sub
    Private Sub strCH_5_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_5_Min.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch5_HScrollBar.Value < strCH_5_Min.Value Then
                    ch5_HScrollBar.Value = strCH_5_Min.Value
                    strCH_5_Current.Text = Convert.ToString(ch5_HScrollBar.Value)
                End If
                ch5_HScrollBar.Minimum = strCH_5_Min.Value
                Call myPCAL.SetChannelLowerLimit(FormatChannelValue(strCH_5_Min.Value), 5)
            End If
        End If
    End Sub
    Private Sub strCH_6_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_6_Min.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch6_HScrollBar.Value < strCH_6_Min.Value Then
                    ch6_HScrollBar.Value = strCH_6_Min.Value
                    strCH_6_Current.Text = Convert.ToString(ch6_HScrollBar.Value)
                End If
                ch6_HScrollBar.Minimum = strCH_6_Min.Value
                Call myPCAL.SetChannelLowerLimit(FormatChannelValue(strCH_6_Min.Value), 6)
            End If
        End If
    End Sub
    Private Sub strCH_7_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_7_Min.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch7_HScrollBar.Value < strCH_7_Min.Value Then
                    ch7_HScrollBar.Value = strCH_7_Min.Value
                    strCH_7_Current.Text = Convert.ToString(ch7_HScrollBar.Value)
                End If
                ch7_HScrollBar.Minimum = strCH_7_Min.Value
                Call myPCAL.SetChannelLowerLimit(FormatChannelValue(strCH_7_Min.Value), 7)
            End If
        End If
    End Sub
    Private Sub strCH_8_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_8_Min.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch8_HScrollBar.Value < strCH_8_Min.Value Then
                    ch8_HScrollBar.Value = strCH_8_Min.Value
                    strCH_8_Current.Text = Convert.ToString(ch8_HScrollBar.Value)
                End If
                ch8_HScrollBar.Minimum = strCH_8_Min.Value
                Call myPCAL.SetChannelLowerLimit(FormatChannelValue(strCH_8_Min.Value), 8)
            End If
        End If
    End Sub
    Private Sub strCH_1_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_1_Max.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch1_HScrollBar.Value > strCH_1_Max.Value Then
                    ch1_HScrollBar.Value = strCH_1_Max.Value
                    strCH_1_Current.Text = Convert.ToString(ch1_HScrollBar.Value)
                End If
                ch1_HScrollBar.Maximum = strCH_1_Max.Value
                Call myPCAL.SetChannelUpperLimit(FormatChannelValue(strCH_1_Max.Value), 1)
            End If
        End If
    End Sub
    Private Sub strCH_2_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_2_Max.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch2_HScrollBar.Value > strCH_2_Max.Value Then
                    ch2_HScrollBar.Value = strCH_2_Max.Value
                    strCH_2_Current.Text = Convert.ToString(ch2_HScrollBar.Value)
                End If
                ch2_HScrollBar.Maximum = strCH_2_Max.Value
                Call myPCAL.SetChannelUpperLimit(FormatChannelValue(strCH_2_Max.Value), 2)
            End If
        End If
    End Sub
    Private Sub strCH_3_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_3_Max.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch3_HScrollBar.Value > strCH_3_Max.Value Then
                    ch3_HScrollBar.Value = strCH_3_Max.Value
                    strCH_3_Current.Text = Convert.ToString(ch3_HScrollBar.Value)
                End If
                ch3_HScrollBar.Maximum = strCH_3_Max.Value
                Call myPCAL.SetChannelUpperLimit(FormatChannelValue(strCH_3_Max.Value), 3)
            End If
        End If
    End Sub
    Private Sub strCH_4_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_4_Max.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch4_HScrollBar.Value > strCH_4_Max.Value Then
                    ch4_HScrollBar.Value = strCH_4_Max.Value
                    strCH_4_Current.Text = Convert.ToString(ch4_HScrollBar.Value)
                End If
                ch4_HScrollBar.Maximum = strCH_4_Max.Value
                Call myPCAL.SetChannelUpperLimit(FormatChannelValue(strCH_4_Max.Value), 4)
            End If
        End If
    End Sub
    Private Sub strCH_5_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_5_Max.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch5_HScrollBar.Value > strCH_5_Max.Value Then
                    ch5_HScrollBar.Value = strCH_5_Max.Value
                    strCH_5_Current.Text = Convert.ToString(ch5_HScrollBar.Value)
                End If
                ch5_HScrollBar.Maximum = strCH_5_Max.Value
                Call myPCAL.SetChannelUpperLimit(FormatChannelValue(strCH_5_Max.Value), 5)
            End If
        End If
    End Sub
    Private Sub strCH_6_MAx_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_6_Max.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch6_HScrollBar.Value > strCH_6_Max.Value Then
                    ch6_HScrollBar.Value = strCH_6_Max.Value
                    strCH_6_Current.Text = Convert.ToString(ch6_HScrollBar.Value)
                End If
                ch6_HScrollBar.Maximum = strCH_6_Max.Value
                Call myPCAL.SetChannelUpperLimit(FormatChannelValue(strCH_6_Max.Value), 6)
            End If
        End If
    End Sub
    Private Sub strCH_7_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_7_Max.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch7_HScrollBar.Value > strCH_7_Max.Value Then
                    ch7_HScrollBar.Value = strCH_7_Max.Value
                    strCH_7_Current.Text = Convert.ToString(ch7_HScrollBar.Value)
                End If
                ch7_HScrollBar.Maximum = strCH_7_Max.Value
                Call myPCAL.SetChannelUpperLimit(FormatChannelValue(strCH_7_Max.Value), 7)
            End If
        End If
    End Sub
    Private Sub strCH_8_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_8_Max.ValueChanged
        If myPCAL.LinkConnected Then
            If bDataLoaded Then
                If ch8_HScrollBar.Value > strCH_8_Max.Value Then
                    ch8_HScrollBar.Value = strCH_8_Max.Value
                    strCH_8_Current.Text = Convert.ToString(ch8_HScrollBar.Value)
                End If
                ch8_HScrollBar.Maximum = strCH_8_Max.Value
                Call myPCAL.SetChannelUpperLimit(FormatChannelValue(strCH_8_Max.Value), 8)
            End If
        End If
    End Sub
    Private Sub ch1_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch1_HScrollBar.Scroll
        strCH_1_Current.Text = Convert.ToString(ch1_HScrollBar.Value)
        If (HPMath) Then
            Call myPCAL.SetHPChannelPulseLength(1, Convert.ToString(ch1_HScrollBar.Value * 5))
        Else
            Call myPCAL.SetChannelPulseLength(1, Convert.ToString(ch1_HScrollBar.Value))
        End If
    End Sub
    Private Sub ch2_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch2_HScrollBar.Scroll
        strCH_2_Current.Text = Convert.ToString(ch2_HScrollBar.Value)
        If (HPMath) Then
            Call myPCAL.SetHPChannelPulseLength(2, Convert.ToString(ch2_HScrollBar.Value * 5))
        Else
            Call myPCAL.SetChannelPulseLength(2, Convert.ToString(ch2_HScrollBar.Value))
        End If
    End Sub
    Private Sub ch3_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch3_HScrollBar.Scroll
        strCH_3_Current.Text = Convert.ToString(ch3_HScrollBar.Value)
        If (HPMath) Then
            Call myPCAL.SetHPChannelPulseLength(3, Convert.ToString(ch3_HScrollBar.Value * 5))
        Else
            Call myPCAL.SetChannelPulseLength(3, Convert.ToString(ch3_HScrollBar.Value))
        End If
    End Sub

    Private Sub ch4_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch4_HScrollBar.Scroll
        strCH_4_Current.Text = Convert.ToString(ch4_HScrollBar.Value)
        If (HPMath) Then
            Call myPCAL.SetHPChannelPulseLength(4, Convert.ToString(ch4_HScrollBar.Value * 5))
        Else
            Call myPCAL.SetChannelPulseLength(4, Convert.ToString(ch4_HScrollBar.Value))
        End If
    End Sub
    Private Sub ch5_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch5_HScrollBar.Scroll
        strCH_5_Current.Text = Convert.ToString(ch5_HScrollBar.Value)
        If (HPMath) Then
            Call myPCAL.SetHPChannelPulseLength(5, Convert.ToString(ch5_HScrollBar.Value * 5))
        Else
            Call myPCAL.SetChannelPulseLength(5, Convert.ToString(ch5_HScrollBar.Value))
        End If
    End Sub
    Private Sub ch6_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch6_HScrollBar.Scroll
        strCH_6_Current.Text = Convert.ToString(ch6_HScrollBar.Value)
        If (HPMath) Then
            Call myPCAL.SetHPChannelPulseLength(6, Convert.ToString(ch6_HScrollBar.Value * 5))
        Else
            Call myPCAL.SetChannelPulseLength(6, Convert.ToString(ch6_HScrollBar.Value))
        End If
    End Sub

    Private Sub ch7_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch7_HScrollBar.Scroll
        strCH_7_Current.Text = Convert.ToString(ch7_HScrollBar.Value)
        If (HPMath) Then
            Call myPCAL.SetHPChannelPulseLength(7, Convert.ToString(ch7_HScrollBar.Value * 5))
        Else
            Call myPCAL.SetChannelPulseLength(7, Convert.ToString(ch7_HScrollBar.Value))
        End If
    End Sub
    Private Sub ch8_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch8_HScrollBar.Scroll
        strCH_8_Current.Text = Convert.ToString(ch8_HScrollBar.Value)
        If (HPMath) Then
            Call myPCAL.SetHPChannelPulseLength(8, Convert.ToString(ch8_HScrollBar.Value * 5))
        Else
            Call myPCAL.SetChannelPulseLength(8, Convert.ToString(ch8_HScrollBar.Value))
        End If
    End Sub
    Private Sub TimeOut_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimeOut.ValueChanged
        Dim myStringBuffer As String = "" ' used for temporary storage of value
        If myPCAL.LinkConnected Then
            If TimeOut.Value < 10 Then myStringBuffer = "0"
            If TimeOut.Value < 100 Then myStringBuffer = myStringBuffer + "0"
            Call myPCAL.SetPiKoderTimeOut(myStringBuffer + Convert.ToString(TimeOut.Value))
            If (TimeOut.Value <> 0) Then
                startHeartBeat(TimeOut.Value * 100 / 2) ' make sure to account for admin
            Else : stopHeartBeat()
            End If
        End If
    End Sub
    Private Sub miniSSCOffset_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles miniSSCOffset.ValueChanged
        Dim myStringBuffer As String = "" ' used for temporary storage of value
        If myPCAL.LinkConnected Then
            If miniSSCOffset.Value < 100 Then myStringBuffer = "0"
            If miniSSCOffset.Value < 10 Then myStringBuffer = myStringBuffer + "0"
            Call myPCAL.SetPiKoderMiniSSCOffset(myStringBuffer + Convert.ToString(miniSSCOffset.Value))
        End If
    End Sub
    Private Sub PPM_Channels_ValueChanged(sender As Object, e As EventArgs) Handles PPM_Channels.ValueChanged
        myPCAL.SetPiKoderPPMChannels(PPM_Channels.Value)
    End Sub
    Private Sub PPM_Mode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles PPM_Mode.SelectedIndexChanged
        If (PPM_Mode.SelectedIndex >= 0) Then
            myPCAL.SetPiKoderPPMMode(PPM_Mode.SelectedIndex)
        End If
        PPM_Mode.ClearSelected()
    End Sub
    Private Sub RetrieveChannel1HPInformation()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetHPPulseLength(strChannelBuffer, 1)
            If strChannelBuffer <> "TimeOut" Then
                ch1_HScrollBar.Value = Val(strChannelBuffer) / 5
                strCH_1_Current.Text = Convert.ToString(ch1_HScrollBar.Value)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel2HPInformation()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetHPPulseLength(strChannelBuffer, 2)
            If strChannelBuffer <> "TimeOut" Then
                ch2_HScrollBar.Value = Val(strChannelBuffer) / 5
                strCH_2_Current.Text = Convert.ToString(ch2_HScrollBar.Value)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel3HPInformation()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetHPPulseLength(strChannelBuffer, 3)
            If strChannelBuffer <> "TimeOut" Then
                ch3_HScrollBar.Value = Val(strChannelBuffer) / 5
                strCH_3_Current.Text = Convert.ToString(ch3_HScrollBar.Value)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel4HPInformation()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetHPPulseLength(strChannelBuffer, 4)
            If strChannelBuffer <> "TimeOut" Then
                ch4_HScrollBar.Value = Val(strChannelBuffer) / 5
                strCH_4_Current.Text = Convert.ToString(ch4_HScrollBar.Value)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel5HPInformation()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetHPPulseLength(strChannelBuffer, 5)
            If strChannelBuffer <> "TimeOut" Then
                ch5_HScrollBar.Value = Val(strChannelBuffer) / 5
                strCH_5_Current.Text = Convert.ToString(ch5_HScrollBar.Value)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel6HPInformation()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetHPPulseLength(strChannelBuffer, 6)
            If strChannelBuffer <> "TimeOut" Then
                ch6_HScrollBar.Value = Val(strChannelBuffer) / 5
                strCH_6_Current.Text = Convert.ToString(ch6_HScrollBar.Value)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel7HPInformation()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetHPPulseLength(strChannelBuffer, 7)
            If strChannelBuffer <> "TimeOut" Then
                ch7_HScrollBar.Value = Val(strChannelBuffer) / 5
                strCH_7_Current.Text = Convert.ToString(ch7_HScrollBar.Value)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel8HPInformation()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetHPPulseLength(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                ch8_HScrollBar.Value = Val(strChannelBuffer) / 5
                strCH_8_Current.Text = Convert.ToString(ch8_HScrollBar.Value)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel1Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetPulseLength(strChannelBuffer, 1)
            If strChannelBuffer <> "TimeOut" Then
                strCH_1_Current.Text = strChannelBuffer
                ch1_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel2Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetPulseLength(strChannelBuffer, 2)
            If strChannelBuffer <> "TimeOut" Then
                strCH_2_Current.Text = strChannelBuffer
                ch2_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel3Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetPulseLength(strChannelBuffer, 3)
            If strChannelBuffer <> "TimeOut" Then
                strCH_3_Current.Text = strChannelBuffer
                ch3_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel4Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetPulseLength(strChannelBuffer, 4)
            If strChannelBuffer <> "TimeOut" Then
                strCH_4_Current.Text = strChannelBuffer
                ch4_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel5Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetPulseLength(strChannelBuffer, 5)
            If strChannelBuffer <> "TimeOut" Then
                strCH_5_Current.Text = strChannelBuffer
                ch5_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel6Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetPulseLength(strChannelBuffer, 6)
            If strChannelBuffer <> "TimeOut" Then
                strCH_6_Current.Text = strChannelBuffer
                ch6_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel7Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetPulseLength(strChannelBuffer, 7)
            If strChannelBuffer <> "TimeOut" Then
                strCH_7_Current.Text = strChannelBuffer
                ch7_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel8Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetPulseLength(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                strCH_8_Current.Text = strChannelBuffer
                ch8_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    ' <summary>
    ' The following block of routines handles the I/O type changes for the PiKoder/SSC
    ' </summary>
    ' 
    ' <remarks>
    ' The form loads a specific listbox eventserver which then calls the actual generic sevrice
    ' </remarks>
    '
    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
        If (ListBox1.SelectedItem = Nothing) Then Exit Sub
        OutputTypeChange(1, ListBox1.SelectedIndex)
        ListBox1.SelectedItem = Nothing
    End Sub
    Private Sub ListBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox2.SelectedIndexChanged
        If (ListBox2.SelectedItem = Nothing) Then Exit Sub
        OutputTypeChange(2, ListBox2.SelectedIndex)
        ListBox2.SelectedItem = Nothing
    End Sub
    Private Sub ListBox3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox3.SelectedIndexChanged
        If (ListBox3.SelectedItem = Nothing) Then Exit Sub
        OutputTypeChange(3, ListBox3.SelectedIndex)
        ListBox3.SelectedItem = Nothing
    End Sub
    Private Sub ListBox4_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox4.SelectedIndexChanged
        If (ListBox4.SelectedItem = Nothing) Then Exit Sub
        OutputTypeChange(4, ListBox4.SelectedIndex)
        ListBox4.SelectedItem = Nothing
    End Sub
    Private Sub ListBox5_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox5.SelectedIndexChanged
        If (ListBox5.SelectedItem = Nothing) Then Exit Sub
        OutputTypeChange(5, ListBox5.SelectedIndex)
        ListBox5.SelectedItem = Nothing
    End Sub
    Private Sub ListBox6_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox6.SelectedIndexChanged
        If (ListBox6.SelectedItem = Nothing) Then Exit Sub
        OutputTypeChange(6, ListBox6.SelectedIndex)
        ListBox6.SelectedItem = Nothing
    End Sub
    Private Sub ListBox7_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox7.SelectedIndexChanged
        If (ListBox7.SelectedItem = Nothing) Then Exit Sub
        OutputTypeChange(7, ListBox7.SelectedIndex)
        ListBox7.SelectedItem = Nothing
    End Sub
    Private Sub ListBox8_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox8.SelectedIndexChanged
        If (ListBox8.SelectedItem = Nothing) Then Exit Sub
        OutputTypeChange(8, ListBox8.SelectedIndex)
        ListBox8.SelectedItem = Nothing
    End Sub
    ' <summary>
    ' The following Sub handles the communication to the PiKoder regarding I/O type changes
    ' </summary>
    ' <param name="iChannelNo"> channel number to be served </param>
    ' <param name="iSettingIndex"> selected index representing output type </param>
    ' <remarks></remarks>
    '
    Private Sub OutputTypeChange(iChannelNo As Integer, iSettingIndex As Integer)
        If Not bDataLoaded Then Exit Sub
        If (InStr(strPiKoderType, "SSC") < 0) Then Exit Sub 'connected to the right PiKoder type?
        If Not boolErrorFlag Then
            If (iSettingIndex = 0) Then
                iChannelSetting(iChannelNo) = 0
                boolErrorFlag = Not myPCAL.SetChannelOutputType(iChannelNo, "P")
            Else
                iChannelSetting(iChannelNo) = 1
                boolErrorFlag = Not myPCAL.SetChannelOutputType(iChannelNo, "S")
            End If
        End If
    End Sub
    Private Sub ObtainCurrentSSID()
        Dim wlan = New WlanClient()
        Dim connectedSsids As Collection(Of [String]) = New Collection(Of String)()

        If (wlan.Interfaces(0).InterfaceState = NativeWifi.Wlan.WlanInterfaceState.Disconnected) Then
            InfoSSID.Text = ""
            Exit Sub
        End If

        Try
            For Each wlanInterface As WlanClient.WlanInterface In wlan.Interfaces
                Dim ssid As Wlan.Dot11Ssid = wlanInterface.CurrentConnection.wlanAssociationAttributes.dot11Ssid
                connectedSsids.Add(New [String](Encoding.ASCII.GetChars(ssid.SSID, 0, CInt(ssid.SSIDLength))))

                For Each item As String In connectedSsids
                    InfoSSID.Text = item
                Next
            Next
        Catch ex As Exception
        End Try
    End Sub
    Private Sub Connect2PiKoder(iLinkType As Integer)
        Dim strChannelBuffer As String = ""

        Led2.Blink = True 'indicate connection testing

        boolErrorFlag = Not (myPCAL.EstablishLink(AvailableCOMPorts.Items(AvailableCOMPorts.TopIndex), iLinkType))

        If Not boolErrorFlag Then
            RetrievePiKoderType(strPiKoderType)
            If (InStr(strPiKoderType, "UART2PPM") > 0) Then
                TypeId.Text = "UART2PPM"
                Call DisplayFoundMessage(TypeId.Text)
                RetrieveUART2PPMParameters()
                IndicateConnectionOk()
            ElseIf (InStr(strPiKoderType, "USB2PPM") > 0) Then
                TypeId.Text = "USB2PPM"
                Call DisplayFoundMessage(TypeId.Text)
                RetrieveUSB2PPMParameters()
                IndicateConnectionOk()
            ElseIf (InStr(strPiKoderType, "SSC-HP") > 0) Then
                TypeId.Text = "SSC-HP"
                Call DisplayFoundMessage(TypeId.Text)
                RetrieveSSC_HPParameters()
                IndicateConnectionOk()
                HPMath = True
            ElseIf (InStr(strPiKoderType, "SSC") > 0) Then
                TypeId.Text = "SSC"
                Call DisplayFoundMessage(TypeId.Text)
                RetrieveSSCParameters()
                IndicateConnectionOk()
            Else ' error message
                Led2.Blink = False
                TextBox1.Text = "Device on " + AvailableCOMPorts.Items(AvailableCOMPorts.TopIndex) + " not supported"
                boolErrorFlag = True
            End If
        Else
            Timer1.Enabled = True
            Led2.Blink = False
            If ConnectCOM.Checked Then
                TextBox1.Text = "Could not open " + AvailableCOMPorts.Items(AvailableCOMPorts.TopIndex)
            Else
                TextBox1.Text = "Could not connect to " + InfoSSID.Text
            End If
        End If
    End Sub
    Private Sub DisplayFoundMessage(ByVal sPiKoderTpye As String)
        Dim myMessage As String = "Found "
        myMessage = myMessage + sPiKoderTpye + " @ "
        If ConnectCOM.Checked Then
            myMessage = myMessage + AvailableCOMPorts.Items(AvailableCOMPorts.TopIndex)
        Else
            myMessage = myMessage + InfoSSID.Text
        End If
        TextBox1.Text = myMessage
    End Sub
    Private Sub ReadIOSwitching()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call myPCAL.GetIOType(strChannelBuffer, 1)
            If strChannelBuffer = "TimeOut" Then
                boolErrorFlag = True
            Else
                If (String.Compare(strChannelBuffer, "P") = 0) Then
                    iChannelSetting(1) = 0
                Else
                    iChannelSetting(1) = 1
                End If
                ListBox1.Enabled = True
                ListBox1.SelectedIndex = iChannelSetting(1)
                ListBox1.ClearSelected()
                ListBox1.ForeColor = Color.Black
            End If
        End If

        If Not boolErrorFlag Then
            Call myPCAL.GetIOType(strChannelBuffer, 2)
            If strChannelBuffer = "TimeOut" Then
                boolErrorFlag = True
            Else
                If (String.Compare(strChannelBuffer, "P") = 0) Then
                    iChannelSetting(2) = 0
                Else
                    iChannelSetting(2) = 1
                End If
                ListBox2.Enabled = True
                ListBox2.SelectedIndex = iChannelSetting(2)
                ListBox2.ClearSelected()
                ListBox2.ForeColor = Color.Black
            End If
        End If

        If Not boolErrorFlag Then
            Call myPCAL.GetIOType(strChannelBuffer, 3)
            If strChannelBuffer = "TimeOut" Then
                boolErrorFlag = True
            Else
                If (String.Compare(strChannelBuffer, "P") = 0) Then
                    iChannelSetting(3) = 0
                Else
                    iChannelSetting(3) = 1
                End If
                ListBox3.Enabled = True
                ListBox3.SelectedIndex = iChannelSetting(3)
                ListBox3.ClearSelected()
                ListBox3.ForeColor = Color.Black
            End If
        End If

        If Not boolErrorFlag Then
            Call myPCAL.GetIOType(strChannelBuffer, 4)
            If strChannelBuffer = "TimeOut" Then
                boolErrorFlag = True
            Else
                If (String.Compare(strChannelBuffer, "P") = 0) Then
                    iChannelSetting(4) = 0
                Else
                    iChannelSetting(4) = 1
                End If
                ListBox4.Enabled = True
                ListBox4.SelectedIndex = iChannelSetting(4)
                ListBox4.ClearSelected()
                ListBox4.ForeColor = Color.Black
            End If
        End If


        If Not boolErrorFlag Then
            Call myPCAL.GetIOType(strChannelBuffer, 5)
            If strChannelBuffer = "TimeOut" Then
                boolErrorFlag = True
            Else
                If (String.Compare(strChannelBuffer, "P") = 0) Then
                    iChannelSetting(5) = 0
                Else
                    iChannelSetting(5) = 1
                End If
                ListBox5.Enabled = True
                ListBox5.SelectedIndex = iChannelSetting(5)
                ListBox5.ClearSelected()
                ListBox5.ForeColor = Color.Black
            End If
        End If

        If Not boolErrorFlag Then
            Call myPCAL.GetIOType(strChannelBuffer, 6)
            If strChannelBuffer = "TimeOut" Then
                boolErrorFlag = True
            Else
                If (String.Compare(strChannelBuffer, "P") = 0) Then
                    iChannelSetting(6) = 0
                Else
                    iChannelSetting(6) = 1
                End If
                ListBox6.Enabled = True
                ListBox6.SelectedIndex = iChannelSetting(6)
                ListBox6.ClearSelected()
                ListBox6.ForeColor = Color.Black
            End If
        End If

        If Not boolErrorFlag Then
            Call myPCAL.GetIOType(strChannelBuffer, 7)
            If strChannelBuffer = "TimeOut" Then
                boolErrorFlag = True
            Else
                If (String.Compare(strChannelBuffer, "P") = 0) Then
                    iChannelSetting(7) = 0
                Else
                    iChannelSetting(7) = 1
                End If
                ListBox7.Enabled = True
                ListBox7.SelectedIndex = iChannelSetting(7)
                ListBox7.ClearSelected()
                ListBox7.ForeColor = Color.Black
            End If
        End If

        If Not boolErrorFlag Then
            Call myPCAL.GetIOType(strChannelBuffer, 8)
            If strChannelBuffer = "TimeOut" Then
                boolErrorFlag = True
            Else
                If (String.Compare(strChannelBuffer, "P") = 0) Then
                    iChannelSetting(8) = 0
                Else
                    iChannelSetting(8) = 1
                End If
                ListBox8.Enabled = True
                ListBox8.SelectedIndex = iChannelSetting(8)
                ListBox8.ClearSelected()
                ListBox8.ForeColor = Color.Black
            End If
        End If

    End Sub
    Private Sub ConnectCOM_CheckedChanged(sender As Object, e As EventArgs) Handles ConnectCOM.CheckedChanged
        If ConnectCOM.Checked Then
            If ConnectWLAN.Checked Then
                ConnectWLAN.CheckState = False
            End If
            Connect2PiKoder(PiKoderCommunicationAbstractionLayer.iPhysicalLink.iSerialLink)
            If boolErrorFlag Then
                ConnectCOM.Checked = False
            End If
        Else
            myPCAL.DisconnectLink(PiKoderCommunicationAbstractionLayer.iPhysicalLink.iSerialLink)
            Dim myMessage As String
            If boolErrorFlag Then
                myMessage = "Device on " + AvailableCOMPorts.Items(AvailableCOMPorts.TopIndex) + " not supported"
            Else
                myMessage = TypeId.Text + "@" + AvailableCOMPorts.Items(AvailableCOMPorts.TopIndex) + " disconnected"
            End If
            CleanUpUI()
            TextBox1.Text = myMessage
        End If
    End Sub
    Private Sub ConnectWLAN_CheckedChanged(sender As Object, e As EventArgs) Handles ConnectWLAN.CheckedChanged
        Dim myMessage As String = ""
        If ConnectWLAN.Checked Then
            If InfoSSID.Text = "" Then 'make sure we are connected to an AP
                myMessage = "Not connected to a WLAN"
                ConnectWLAN.Checked = False
            Else
                If ConnectCOM.Checked Then
                    ConnectCOM.CheckState = False
                End If
                Connect2PiKoder(PiKoderCommunicationAbstractionLayer.iPhysicalLink.iWLANlink)
                If boolErrorFlag Then
                    ConnectWLAN.Checked = False
                End If
            End If
        Else
            myPCAL.DisconnectLink(PiKoderCommunicationAbstractionLayer.iPhysicalLink.iWLANlink)
            If boolErrorFlag Then
                myMessage = "Access Point " + InfoSSID.Text + " not supported"
            End If
            CleanUpUI()
            TextBox1.Text = myMessage
        End If
    End Sub
    Private Sub AvailableCOMPorts_SelectedIndexChanged(sender As Object, e As EventArgs) Handles AvailableCOMPorts.SelectedIndexChanged
        AvailableCOMPorts.ClearSelected()
    End Sub
End Class

