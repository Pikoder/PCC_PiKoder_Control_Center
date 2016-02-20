Option Strict Off
Option Explicit On
Imports System
Imports System.IO.Ports
Public Class PCC_PiKoder_Control_Center

    ' This is a program for evaluating the PiKoder platform - please refer to http://pikoder.com for more details.
    ' 
    ' Copyright 2015-2016 Gregor Schlechtriem
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
    Private mySerialLink As New SerialLink

    ' declaration of variables
    Dim boolErrorFlag As Boolean ' global flag for errors in communication
    Dim boolSentChangeValueNotRequired(24) As Boolean 'avoid unnecessary sending of param. changes to PiKoder
    Dim sDefaultMinValue As String = "750" ' default values for USB2PMM
    Dim sDefaultMaxValue As String = "2250"

    ' declaration of subroutines
    ''' <summary>
    ''' This method is used to initialize the application. It will collect all
    ''' available COM ports in a list box. If no ports were found then the program
    ''' will stop.
    ''' </summary> 
    ''' <remarks></remarks>

    Public Sub New()

        InitializeComponent()

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
        If mySerialLink.SerialLinkConnected() Then
            'check if we still have a connection on USB system level
            If Not (mySerialLink.PiKoderConnected()) Then
                    LostConnection()
            End If
        Else
            'Update the COM ports list so that we can detect
            '  new COM ports that have been added.
            UpdateCOMPortList()
        End If
    End Sub
    Private Sub IndicateConnectionOk()
        TextBox1.Text = "Parameters loaded ok."
        Led2.Color = LED.LEDColorSelection.LED_Green
        Led2.Blink = False ' indicate that the connection established
    End Sub
    Private Sub AvailableCOMPorts_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AvailableCOMPorts.SelectedIndexChanged

        Dim strChannelBuffer As String = ""
        Dim strPiKoderType As String = ""

        If (AvailableCOMPorts.SelectedItem = Nothing) Then Exit Sub

        Led2.Blink = True 'indicate connection testing

        boolErrorFlag = Not (mySerialLink.EstablishSerialLink(AvailableCOMPorts.SelectedItem))

        RetrievePiKoderType(strPiKoderType)

        If Not boolErrorFlag Then
            If (InStr(strPiKoderType, "UART2PPM") > 0) Then
                TextBox1.Text = "Found UART2PPM @ " + AvailableCOMPorts.SelectedItem.ToString
                TypeId.Text = "UART2PPM"
                RetrieveUART2PPMParameters()
                IndicateConnectionOk()
            ElseIf (InStr(strPiKoderType, "USB2PPM") > 0) Then
                TextBox1.Text = "Found USB2PPM @ " + AvailableCOMPorts.SelectedItem.ToString
                TypeId.Text = "USB2PPM"
                RetrieveUSB2PPMParameters()
                IndicateConnectionOk()
            ElseIf (InStr(strPiKoderType, "SSC") > 0) Then
                TextBox1.Text = "Found SSC @ " + AvailableCOMPorts.SelectedItem.ToString
                TypeId.Text = "SSC"
                RetrieveSSCParameters()
                IndicateConnectionOk()
            Else ' error message
                Led2.Blink = False
                TextBox1.Text = "Device on " + AvailableCOMPorts.SelectedItem.ToString + " not supported"
            End If
        End If

ErrorExit:
        AvailableCOMPorts.SelectedItem = Nothing
        Timer1.Enabled = True
    End Sub

    Private Sub RetrievePiKoderType(ByRef SerialInputString As String)

        Dim strChannelBuffer As String = ""

        If mySerialLink.SerialLinkConnected() Then
            If Not boolErrorFlag Then
                'check for identifier
                Call mySerialLink.SendDataToSerial("?")
                Call mySerialLink.GetStatusRecord(strChannelBuffer)
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
    ''' This function is used to validate a pulse length value provided in a string. 
    ''' </summary>
    ''' <param name="strVal"></param>
    ''' <remarks></remarks>
    ''' 
    Private Function ValidatePulseValue(ByRef strVal As String) As Boolean
        Dim intChannelPulseLength As Integer
        intChannelPulseLength = Val(strVal) 'no check on chars this time
        If (intChannelPulseLength < 750) Or (intChannelPulseLength > 2250) Then
            Return False
        End If
        'format string
        If (intChannelPulseLength < 1000) And (Len(strVal) = 4) Then strVal = Mid(strVal, 2, 3)
        Return True
    End Function
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
        ' initiate save by sending command
        Call mySerialLink.SendDataToSerial("S")
        iRetCode = mySerialLink.GetErrorCode()  'wait for status 
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
        If mySerialLink.PiKoderConnected() Then Exit Sub
        LostConnection()
    End Sub
    ''' <summary>
    ''' Set UI to indicate that we have lost connection - either on USB system level or due to a time out condition on application level
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LostConnection()
        ' indicate that connection is lost
        Led2.Color = LED.LEDColorSelection.LED_Red
        TextBox1.Text = "Lost connection to PiKoder."
        stopHeartBeat()
        Timer1.Enabled = True
        TypeId.Text = ""

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

        ' close port
        mySerialLink.MyForm_Dispose()
    End Sub
    Private Sub RetrieveUART2PPMParameters()

        Dim strChannelBuffer As String = ""
        Dim b As Integer

        For b = 0 To 24
            boolSentChangeValueNotRequired(b) = False
        Next

        'Make sure that all required fields are enabled
        GroupBox8.Enabled = True
        GroupBox8.Visible = True
        GroupBox4.Enabled = True
        GroupBox4.Visible = True
        GroupBox7.Enabled = True 'Save Parameters
        GroupBox7.Visible = True

        If mySerialLink.SerialLinkConnected() Then
            If Not boolErrorFlag Then
                'request status information from SSC    
                Call mySerialLink.SendDataToSerial("0")
                Call mySerialLink.GetFirmwareVersion(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strSSC_Firmware.Text = strChannelBuffer
                    If Val(strChannelBuffer) > 2.04 Then
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
                Call mySerialLink.SendDataToSerial("N1?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(16) = True
                    strCH_1_Neutral.Maximum = 2500
                    strCH_1_Neutral.Value = Val(strChannelBuffer)
                    strCH_1_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N2?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(17) = True
                    strCH_2_Neutral.Maximum = 2500
                    strCH_2_Neutral.Value = Val(strChannelBuffer)
                    strCH_2_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N3?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(18) = True
                    strCH_3_Neutral.Maximum = 2500
                    strCH_3_Neutral.Value = Val(strChannelBuffer)
                    strCH_3_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N4?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(19) = True
                    strCH_4_Neutral.Maximum = 2500
                    strCH_4_Neutral.Value = Val(strChannelBuffer)
                    strCH_4_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N5?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(20) = True
                    strCH_5_Neutral.Maximum = 2500
                    strCH_5_Neutral.Value = Val(strChannelBuffer)
                    strCH_5_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N6?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(21) = True
                    strCH_6_Neutral.Maximum = 2500
                    strCH_6_Neutral.Value = Val(strChannelBuffer)
                    strCH_6_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N7?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(22) = True
                    strCH_7_Neutral.Maximum = 2500
                    strCH_7_Neutral.Value = Val(strChannelBuffer)
                    strCH_7_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N8?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(23) = True
                    strCH_8_Neutral.Maximum = 2500
                    strCH_8_Neutral.Value = Val(strChannelBuffer)
                    strCH_8_Neutral.ForeColor = Color.Black
                End If
            End If
            'retrieve min & max information for all channels
            'channel 1
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("L1?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(0) = True
                    strCH_1_Min.Value = Val(strChannelBuffer)
                    ch1_HScrollBar.Minimum = strCH_1_Min.Value
                    strCH_1_Min.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("U1?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(1) = True
                    strCH_1_Max.Value = Val(strChannelBuffer)
                    ch1_HScrollBar.Maximum = strCH_1_Max.Value
                    strCH_1_Max.ForeColor = Color.Black
                End If
            End If
        End If

        'channel 2
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L2?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(2) = True
                strCH_2_Min.Value = Val(strChannelBuffer)
                ch2_HScrollBar.Minimum = strCH_2_Min.Value
                strCH_2_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U2?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(3) = True
                strCH_2_Max.Value = Val(strChannelBuffer)
                ch2_HScrollBar.Maximum = strCH_2_Max.Value
                strCH_2_Max.ForeColor = Color.Black
            End If
        End If

        'channel 3
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L3?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(4) = True
                strCH_3_Min.Value = Val(strChannelBuffer)
                ch3_HScrollBar.Minimum = strCH_3_Min.Value
                strCH_3_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U3?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(5) = True
                strCH_3_Max.Value = Val(strChannelBuffer)
                ch3_HScrollBar.Maximum = strCH_3_Max.Value
                strCH_3_Max.ForeColor = Color.Black
            End If
        End If

        'channel 4
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L4?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(6) = True
                strCH_4_Min.Value = Val(strChannelBuffer)
                ch4_HScrollBar.Minimum = strCH_4_Min.Value
                strCH_4_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U4?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(7) = True
                strCH_4_Max.Value = Val(strChannelBuffer)
                ch4_HScrollBar.Maximum = strCH_4_Max.Value
                strCH_4_Max.ForeColor = Color.Black
            End If
        End If

        'channel 5
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L5?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(8) = True
                strCH_5_Min.Value = Val(strChannelBuffer)
                ch5_HScrollBar.Minimum = strCH_5_Min.Value
                strCH_5_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U5?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(9) = True
                strCH_5_Max.Value = Val(strChannelBuffer)
                ch5_HScrollBar.Maximum = strCH_5_Max.Value
                strCH_5_Max.ForeColor = Color.Black
            End If
        End If

        'channel 6
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L6?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(10) = True
                strCH_6_Min.Value = Val(strChannelBuffer)
                ch6_HScrollBar.Minimum = strCH_6_Min.Value
                strCH_6_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U6?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(11) = True
                strCH_6_Max.Value = Val(strChannelBuffer)
                ch6_HScrollBar.Maximum = strCH_6_Max.Value
                strCH_6_Max.ForeColor = Color.Black
            End If
        End If

        'channel 7
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L7?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(12) = True
                strCH_7_Min.Value = Val(strChannelBuffer)
                ch7_HScrollBar.Minimum = strCH_7_Min.Value
                strCH_7_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U7?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(13) = True
                strCH_7_Max.Value = Val(strChannelBuffer)
                ch7_HScrollBar.Maximum = strCH_7_Max.Value
                strCH_7_Max.ForeColor = Color.Black
            End If
        End If

        'channel 8
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L8?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(14) = True
                strCH_8_Min.Value = Val(strChannelBuffer)
                ch8_HScrollBar.Minimum = strCH_8_Min.Value
                strCH_8_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U8?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(15) = True
                strCH_8_Max.Value = Val(strChannelBuffer)
                ch8_HScrollBar.Maximum = strCH_8_Max.Value
                strCH_8_Max.ForeColor = Color.Black
            End If

        End If

        'retrieve TimeOut
        If Not boolErrorFlag Then
            Call mySerialLink.GetTimeOut(strChannelBuffer, 7)
            If strChannelBuffer <> "TimeOut" Then
                TimeOut.Value = Val(strChannelBuffer)
                TimeOut.ForeColor = Color.Black
            End If
        End If

        'retrieve miniSSC offset
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("M?")
            Call mySerialLink.GetMiniSSCOffset(strChannelBuffer, 6)
            If strChannelBuffer <> "TimeOut" Then
                miniSSCOffset.Value = Val(strChannelBuffer)
                miniSSCOffset.ForeColor = Color.Black
            End If
        End If

        GroupBox13.Enabled = False
        GroupBox13.Visible = False
        GroupBox17.Enabled = False
        GroupBox17.Visible = False

    End Sub
    Private Sub RetrieveSSCParameters()

        Dim strChannelBuffer As String = ""
        Dim b As Integer

        For b = 0 To 24
            boolSentChangeValueNotRequired(b) = False
        Next

        'Make sure that all required fields are enabled
        GroupBox8.Enabled = True
        GroupBox8.Visible = True
        GroupBox4.Enabled = True
        GroupBox4.Visible = True
        GroupBox7.Enabled = True 'Save Parameters
        GroupBox7.Visible = True

        If mySerialLink.SerialLinkConnected() Then
            If Not boolErrorFlag Then
                'request status information from SSC    
                Call mySerialLink.SendDataToSerial("0")
                Call mySerialLink.GetFirmwareVersion(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strSSC_Firmware.Text = strChannelBuffer
                    If Val(strChannelBuffer) > 2.03 Then
                        MsgBox("The PiKoder firmware version found is not supported! Please goto www.pikoder.com and upgrade PCC Control Center to the latest version.", MsgBoxStyle.OkOnly, "Error Message")
                        End
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
                Call mySerialLink.SendDataToSerial("N1?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(16) = True
                    strCH_1_Neutral.Maximum = 2500
                    strCH_1_Neutral.Value = Val(strChannelBuffer)
                    strCH_1_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N2?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(17) = True
                    strCH_2_Neutral.Maximum = 2500
                    strCH_2_Neutral.Value = Val(strChannelBuffer)
                    strCH_2_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N3?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(18) = True
                    strCH_3_Neutral.Maximum = 2500
                    strCH_3_Neutral.Value = Val(strChannelBuffer)
                    strCH_3_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N4?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(19) = True
                    strCH_4_Neutral.Maximum = 2500
                    strCH_4_Neutral.Value = Val(strChannelBuffer)
                    strCH_4_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N5?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(20) = True
                    strCH_5_Neutral.Maximum = 2500
                    strCH_5_Neutral.Value = Val(strChannelBuffer)
                    strCH_5_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N6?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(21) = True
                    strCH_6_Neutral.Maximum = 2500
                    strCH_6_Neutral.Value = Val(strChannelBuffer)
                    strCH_6_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N7?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(22) = True
                    strCH_7_Neutral.Maximum = 2500
                    strCH_7_Neutral.Value = Val(strChannelBuffer)
                    strCH_7_Neutral.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("N8?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(23) = True
                    strCH_8_Neutral.Maximum = 2500
                    strCH_8_Neutral.Value = Val(strChannelBuffer)
                    strCH_8_Neutral.ForeColor = Color.Black
                End If
            End If
            'retrieve min & max information for all channels
            'channel 1
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("L1?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(0) = True
                    strCH_1_Min.Value = Val(strChannelBuffer)
                    ch1_HScrollBar.Minimum = strCH_1_Min.Value
                    strCH_1_Min.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("U1?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(1) = True
                    strCH_1_Max.Value = Val(strChannelBuffer)
                    ch1_HScrollBar.Maximum = strCH_1_Max.Value
                    strCH_1_Max.ForeColor = Color.Black
                End If
            End If
        End If

        'channel 2
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L2?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(2) = True
                strCH_2_Min.Value = Val(strChannelBuffer)
                ch2_HScrollBar.Minimum = strCH_2_Min.Value
                strCH_2_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U2?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(3) = True
                strCH_2_Max.Value = Val(strChannelBuffer)
                ch2_HScrollBar.Maximum = strCH_2_Max.Value
                strCH_2_Max.ForeColor = Color.Black
            End If
        End If

        'channel 3
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L3?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(4) = True
                strCH_3_Min.Value = Val(strChannelBuffer)
                ch3_HScrollBar.Minimum = strCH_3_Min.Value
                strCH_3_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U3?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(5) = True
                strCH_3_Max.Value = Val(strChannelBuffer)
                ch3_HScrollBar.Maximum = strCH_3_Max.Value
                strCH_3_Max.ForeColor = Color.Black
            End If
        End If

        'channel 4
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L4?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(6) = True
                strCH_4_Min.Value = Val(strChannelBuffer)
                ch4_HScrollBar.Minimum = strCH_4_Min.Value
                strCH_4_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U4?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(7) = True
                strCH_4_Max.Value = Val(strChannelBuffer)
                ch4_HScrollBar.Maximum = strCH_4_Max.Value
                strCH_4_Max.ForeColor = Color.Black
            End If
        End If

        'channel 5
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L5?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(8) = True
                strCH_5_Min.Value = Val(strChannelBuffer)
                ch5_HScrollBar.Minimum = strCH_5_Min.Value
                strCH_5_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U5?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(9) = True
                strCH_5_Max.Value = Val(strChannelBuffer)
                ch5_HScrollBar.Maximum = strCH_5_Max.Value
                strCH_5_Max.ForeColor = Color.Black
            End If
        End If

        'channel 6
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L6?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(10) = True
                strCH_6_Min.Value = Val(strChannelBuffer)
                ch6_HScrollBar.Minimum = strCH_6_Min.Value
                strCH_6_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U6?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(11) = True
                strCH_6_Max.Value = Val(strChannelBuffer)
                ch6_HScrollBar.Maximum = strCH_6_Max.Value
                strCH_6_Max.ForeColor = Color.Black
            End If
        End If

        'channel 7
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L7?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(12) = True
                strCH_7_Min.Value = Val(strChannelBuffer)
                ch7_HScrollBar.Minimum = strCH_7_Min.Value
                strCH_7_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U7?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(13) = True
                strCH_7_Max.Value = Val(strChannelBuffer)
                ch7_HScrollBar.Maximum = strCH_7_Max.Value
                strCH_7_Max.ForeColor = Color.Black
            End If
        End If

        'channel 8
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("L8?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(14) = True
                strCH_8_Min.Value = Val(strChannelBuffer)
                ch8_HScrollBar.Minimum = strCH_8_Min.Value
                strCH_8_Min.ForeColor = Color.Black
            End If
        End If
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("U8?")
            Call mySerialLink.GetNeutralPosition(strChannelBuffer, 8)
            If strChannelBuffer <> "TimeOut" Then
                boolSentChangeValueNotRequired(15) = True
                strCH_8_Max.Value = Val(strChannelBuffer)
                ch8_HScrollBar.Maximum = strCH_8_Max.Value
                strCH_8_Max.ForeColor = Color.Black
            End If

        End If

        'retrieve TimeOut
        If Not boolErrorFlag Then
            Call mySerialLink.GetTimeOut(strChannelBuffer, 7)
            If strChannelBuffer <> "TimeOut" Then
                TimeOut.Value = Val(strChannelBuffer)
                TimeOut.ForeColor = Color.Black
            End If
        End If

        'retrieve miniSSC offset
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("M?")
            Call mySerialLink.GetMiniSSCOffset(strChannelBuffer, 6)
            If strChannelBuffer <> "TimeOut" Then
                miniSSCOffset.Value = Val(strChannelBuffer)
                miniSSCOffset.ForeColor = Color.Black
            End If
        End If

        GroupBox13.Enabled = False '# PPM Channels
        GroupBox13.Visible = False
        GroupBox17.Enabled = False 'PPM mode
        GroupBox17.Visible = False

    End Sub

    Private Sub RetrieveUSB2PPMParameters()

        Dim strChannelBuffer As String = ""
        Dim b As Integer


        For b = 0 To 24
            boolSentChangeValueNotRequired(b) = True
        Next

        GroupBox13.Enabled = True
        GroupBox13.Visible = True
        GroupBox17.Enabled = True
        GroupBox17.Visible = True

        If mySerialLink.SerialLinkConnected() Then
            If Not boolErrorFlag Then
                'request firm ware version from PiKoder    
                Call mySerialLink.SendDataToSerial("0")
                Call mySerialLink.GetFirmwareVersion(strChannelBuffer)
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


    Private Sub strCH_1_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_1_Neutral.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(16) Then
                Dim strChannelBuffer As String = "N1="
                If strCH_1_Neutral.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_1_Neutral.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
            End If
            boolSentChangeValueNotRequired(16) = False
        End If
    End Sub


    Private Sub strCH_2_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_2_Neutral.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(17) Then
                Dim strChannelBuffer As String = "N2="
                If strCH_2_Neutral.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_2_Neutral.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
            End If
            boolSentChangeValueNotRequired(17) = False
        End If
    End Sub
    Private Sub strCH_3_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_3_Neutral.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(18) Then
                Dim strChannelBuffer As String = "N3="
                If strCH_2_Neutral.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_3_Neutral.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
            End If
            boolSentChangeValueNotRequired(18) = False
        End If
    End Sub
    Private Sub strCH_4_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_4_Neutral.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(19) Then
                Dim strChannelBuffer As String = "N4="
                If strCH_4_Neutral.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_4_Neutral.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
            End If
            boolSentChangeValueNotRequired(19) = False
        End If
    End Sub
    Private Sub strCH_5_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_5_Neutral.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(20) Then
                Dim strChannelBuffer As String = "N5="
                If strCH_5_Neutral.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_5_Neutral.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
            End If
            boolSentChangeValueNotRequired(20) = False
        End If
    End Sub
    Private Sub strCH_6_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_6_Neutral.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(21) Then
                Dim strChannelBuffer As String = "N6="
                If strCH_6_Neutral.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_6_Neutral.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
            End If
            boolSentChangeValueNotRequired(21) = False
        End If
    End Sub
    Private Sub strCH_7_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_7_Neutral.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(22) Then
                Dim strChannelBuffer As String = "N7="
                If strCH_7_Neutral.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_7_Neutral.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
            End If
            boolSentChangeValueNotRequired(22) = False
        End If
    End Sub
    Private Sub strCH_8_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_8_Neutral.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(23) Then
                Dim strChannelBuffer As String = "N8="
                If strCH_8_Neutral.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_8_Neutral.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
            End If
            boolSentChangeValueNotRequired(23) = False
        End If
    End Sub
    Private Sub strCH_1_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_1_Min.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(0) Then
                Dim strChannelBuffer As String = "L1="
                If strCH_1_Min.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_1_Min.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch1_HScrollBar.Value < strCH_1_Min.Value Then
                    ch1_HScrollBar.Value = strCH_1_Min.Value
                    strCH_1_Current.Text = Convert.ToString(ch1_HScrollBar.Value)
                End If
                ch1_HScrollBar.Minimum = strCH_1_Min.Value
            End If
            boolSentChangeValueNotRequired(0) = False
        End If
    End Sub
    Private Sub strCH_2_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_2_Min.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(2) Then
                Dim strChannelBuffer As String = "L2="
                If strCH_2_Min.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_2_Min.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch2_HScrollBar.Value < strCH_2_Min.Value Then
                    ch1_HScrollBar.Value = strCH_2_Min.Value
                    strCH_2_Current.Text = Convert.ToString(ch2_HScrollBar.Value)
                End If
                ch2_HScrollBar.Minimum = strCH_2_Min.Value
            End If
            boolSentChangeValueNotRequired(2) = False
        End If
    End Sub
    Private Sub strCH_3_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_3_Min.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(4) Then
                Dim strChannelBuffer As String = "L3="
                If strCH_3_Min.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_3_Min.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch3_HScrollBar.Value < strCH_3_Min.Value Then
                    ch3_HScrollBar.Value = strCH_3_Min.Value
                    strCH_3_Current.Text = Convert.ToString(ch3_HScrollBar.Value)
                End If
                ch3_HScrollBar.Minimum = strCH_3_Min.Value
            End If
            boolSentChangeValueNotRequired(4) = False
        End If
    End Sub
    Private Sub strCH_4_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_4_Min.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(6) Then
                Dim strChannelBuffer As String = "L4="
                If strCH_4_Min.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_4_Min.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch4_HScrollBar.Value < strCH_4_Min.Value Then
                    ch4_HScrollBar.Value = strCH_4_Min.Value
                    strCH_4_Current.Text = Convert.ToString(ch4_HScrollBar.Value)
                End If
                ch1_HScrollBar.Minimum = strCH_4_Min.Value
            End If
            boolSentChangeValueNotRequired(6) = False
        End If
    End Sub
    Private Sub strCH_5_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_5_Min.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(8) Then
                Dim strChannelBuffer As String = "L5="
                If strCH_5_Min.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_5_Min.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch5_HScrollBar.Value < strCH_5_Min.Value Then
                    ch5_HScrollBar.Value = strCH_5_Min.Value
                    strCH_5_Current.Text = Convert.ToString(ch5_HScrollBar.Value)
                End If
                ch1_HScrollBar.Minimum = strCH_5_Min.Value
            End If
            boolSentChangeValueNotRequired(8) = False
        End If
    End Sub
    Private Sub strCH_6_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_6_Min.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(10) Then
                Dim strChannelBuffer As String = "L6="
                If strCH_6_Min.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_6_Min.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch6_HScrollBar.Value < strCH_6_Min.Value Then
                    ch6_HScrollBar.Value = strCH_6_Min.Value
                    strCH_6_Current.Text = Convert.ToString(ch6_HScrollBar.Value)
                End If
                ch6_HScrollBar.Minimum = strCH_6_Min.Value
            End If
            boolSentChangeValueNotRequired(10) = False
        End If
    End Sub
    Private Sub strCH_7_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_7_Min.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(12) Then
                Dim strChannelBuffer As String = "L7="
                If strCH_7_Min.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_7_Min.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch7_HScrollBar.Value < strCH_7_Min.Value Then
                    ch7_HScrollBar.Value = strCH_7_Min.Value
                    strCH_7_Current.Text = Convert.ToString(ch7_HScrollBar.Value)
                End If
                ch1_HScrollBar.Minimum = strCH_7_Min.Value
            End If
            boolSentChangeValueNotRequired(12) = False
        End If
    End Sub
    Private Sub strCH_8_Min_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_8_Min.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(14) Then
                Dim strChannelBuffer As String = "L8="
                If strCH_8_Min.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_8_Min.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch8_HScrollBar.Value < strCH_8_Min.Value Then
                    ch8_HScrollBar.Value = strCH_8_Min.Value
                    strCH_8_Current.Text = Convert.ToString(ch8_HScrollBar.Value)
                End If
                ch1_HScrollBar.Minimum = strCH_8_Min.Value
            End If
            boolSentChangeValueNotRequired(14) = False
        End If
    End Sub
    Private Sub strCH_1_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_1_Max.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(1) Then
                Dim strChannelBuffer As String = "U1="
                If strCH_1_Max.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_1_Max.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch1_HScrollBar.Value > strCH_1_Max.Value Then
                    ch1_HScrollBar.Value = strCH_1_Max.Value
                    strCH_1_Current.Text = Convert.ToString(ch1_HScrollBar.Value)
                End If
                ch1_HScrollBar.Maximum = strCH_1_Max.Value
            End If
            boolSentChangeValueNotRequired(1) = False
        End If
    End Sub
    Private Sub strCH_2_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_2_Max.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(3) Then
                Dim strChannelBuffer As String = "U2="
                If strCH_2_Max.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_2_Max.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch2_HScrollBar.Value > strCH_2_Max.Value Then
                    ch2_HScrollBar.Value = strCH_2_Max.Value
                    strCH_2_Current.Text = Convert.ToString(ch2_HScrollBar.Value)
                End If
                ch2_HScrollBar.Maximum = strCH_2_Max.Value
            End If
            boolSentChangeValueNotRequired(3) = False
        End If
    End Sub
    Private Sub strCH_3_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_3_Max.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(5) Then
                Dim strChannelBuffer As String = "U3="
                If strCH_3_Max.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_3_Max.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch3_HScrollBar.Value > strCH_3_Max.Value Then
                    ch3_HScrollBar.Value = strCH_3_Max.Value
                    strCH_3_Current.Text = Convert.ToString(ch3_HScrollBar.Value)
                End If
                ch3_HScrollBar.Maximum = strCH_3_Max.Value
            End If
            boolSentChangeValueNotRequired(5) = False
        End If
    End Sub
    Private Sub strCH_4_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_4_Max.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(7) Then
                Dim strChannelBuffer As String = "U4="
                If strCH_4_Max.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_4_Max.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch4_HScrollBar.Value > strCH_4_Max.Value Then
                    ch4_HScrollBar.Value = strCH_4_Max.Value
                    strCH_4_Current.Text = Convert.ToString(ch4_HScrollBar.Value)
                End If
                ch4_HScrollBar.Maximum = strCH_4_Max.Value
            End If
            boolSentChangeValueNotRequired(7) = False
        End If
    End Sub
    Private Sub strCH_5_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_5_Max.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(9) Then
                Dim strChannelBuffer As String = "U5="
                If strCH_5_Max.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_5_Max.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch5_HScrollBar.Value > strCH_5_Max.Value Then
                    ch5_HScrollBar.Value = strCH_5_Max.Value
                    strCH_5_Current.Text = Convert.ToString(ch5_HScrollBar.Value)
                End If
                ch5_HScrollBar.Maximum = strCH_5_Max.Value
            End If
            boolSentChangeValueNotRequired(9) = False
        End If
    End Sub
    Private Sub strCH_6_MAx_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_6_Max.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(11) Then
                Dim strChannelBuffer As String = "U6="
                If strCH_6_Max.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_6_Max.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch6_HScrollBar.Value > strCH_6_Max.Value Then
                    ch6_HScrollBar.Value = strCH_6_Max.Value
                    strCH_6_Current.Text = Convert.ToString(ch6_HScrollBar.Value)
                End If
                ch6_HScrollBar.Maximum = strCH_6_Max.Value
            End If
            boolSentChangeValueNotRequired(11) = False
        End If
    End Sub
    Private Sub strCH_7_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_7_Max.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(13) Then
                Dim strChannelBuffer As String = "U7="
                If strCH_7_Max.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_7_Max.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch7_HScrollBar.Value > strCH_7_Max.Value Then
                    ch7_HScrollBar.Value = strCH_7_Max.Value
                    strCH_7_Current.Text = Convert.ToString(ch7_HScrollBar.Value)
                End If
                ch7_HScrollBar.Maximum = strCH_7_Max.Value
            End If
            boolSentChangeValueNotRequired(13) = False
        End If
    End Sub
    Private Sub strCH_8_Max_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_8_Max.ValueChanged
        If mySerialLink.SerialLinkConnected Then
            If Not boolSentChangeValueNotRequired(15) Then
                Dim strChannelBuffer As String = "U8="
                If strCH_8_Max.Value < 1000 Then
                    strChannelBuffer = strChannelBuffer + "0"
                End If
                strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_8_Max.Value)
                Call mySerialLink.SendDataToSerialwithAck(strChannelBuffer)
                If ch8_HScrollBar.Value > strCH_8_Max.Value Then
                    ch8_HScrollBar.Value = strCH_8_Max.Value
                    strCH_8_Current.Text = Convert.ToString(ch8_HScrollBar.Value)
                End If
                ch8_HScrollBar.Maximum = strCH_8_Max.Value
            End If
            boolSentChangeValueNotRequired(15) = False
        End If
    End Sub

    Private Sub ch1_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch1_HScrollBar.Scroll
        strCH_1_Current.Text = Convert.ToString(ch1_HScrollBar.Value)
        Call mySerialLink.SendPulseLengthToPiKoder(1, strCH_1_Current.Text)
    End Sub

    Private Sub ch2_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch2_HScrollBar.Scroll
        strCH_2_Current.Text = Convert.ToString(ch2_HScrollBar.Value)
        Call mySerialLink.SendPulseLengthToPiKoder(2, strCH_2_Current.Text)
    End Sub

    Private Sub ch3_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch3_HScrollBar.Scroll
        strCH_3_Current.Text = Convert.ToString(ch3_HScrollBar.Value)
        Call mySerialLink.SendPulseLengthToPiKoder(3, strCH_3_Current.Text)
    End Sub

    Private Sub ch4_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch4_HScrollBar.Scroll
        strCH_4_Current.Text = Convert.ToString(ch4_HScrollBar.Value)
        Call mySerialLink.SendPulseLengthToPiKoder(4, strCH_4_Current.Text)
    End Sub

    Private Sub ch5_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch5_HScrollBar.Scroll
        strCH_5_Current.Text = Convert.ToString(ch5_HScrollBar.Value)
        Call mySerialLink.SendPulseLengthToPiKoder(5, strCH_5_Current.Text)
    End Sub

    Private Sub ch6_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch6_HScrollBar.Scroll
        strCH_6_Current.Text = Convert.ToString(ch6_HScrollBar.Value)
        Call mySerialLink.SendPulseLengthToPiKoder(6, strCH_6_Current.Text)
    End Sub

    Private Sub ch7_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch7_HScrollBar.Scroll
        strCH_7_Current.Text = Convert.ToString(ch7_HScrollBar.Value)
        Call mySerialLink.SendPulseLengthToPiKoder(7, strCH_7_Current.Text)
    End Sub

    Private Sub ch8_HScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles ch8_HScrollBar.Scroll
        strCH_8_Current.Text = Convert.ToString(ch8_HScrollBar.Value)
        Call mySerialLink.SendPulseLengthToPiKoder(8, strCH_8_Current.Text)
    End Sub

    Private Sub TimeOut_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimeOut.ValueChanged
        Dim myStringBuffer As String = "" ' used for temporary storage of value
        If mySerialLink.SerialLinkConnected Then
            If TimeOut.Value < 10 Then myStringBuffer = "0"
            If TimeOut.Value < 100 Then myStringBuffer = myStringBuffer + "0"
            Call mySerialLink.SendTimeOutToPiKoder(myStringBuffer + Convert.ToString(TimeOut.Value))
            If (TimeOut.Value <> 0) Then
                startHeartBeat(TimeOut.Value * 100 / 2) ' make sure to account for admin
            Else : stopHeartBeat()
            End If
        End If
    End Sub
    Private Sub miniSSCOffset_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles miniSSCOffset.ValueChanged
        Dim myStringBuffer As String = "" ' used for temporary storage of value
        If mySerialLink.SerialLinkConnected Then
            If miniSSCOffset.Value < 100 Then myStringBuffer = "0"
            If miniSSCOffset.Value < 10 Then myStringBuffer = myStringBuffer + "0"
            Call mySerialLink.SendMiniSSCOffsetToPiKoder(myStringBuffer + Convert.ToString(miniSSCOffset.Value))
        End If
    End Sub
    Private Sub PPM_Channels_ValueChanged(sender As Object, e As EventArgs) Handles PPM_Channels.ValueChanged
        Dim myByteArray() As Byte = {83, 21, 0, 0}
        If mySerialLink.SerialLinkConnected Then
            myByteArray(3) = PPM_Channels.Value
            Call mySerialLink.SendBinaryDataToSerial(myByteArray, 4)
        End If
    End Sub

    Private Sub PPM_Mode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles PPM_Mode.SelectedIndexChanged
        Dim myByteArray() As Byte = {83, 22, 0, 0}
        If (mySerialLink.SerialLinkConnected And (PPM_Mode.SelectedIndex >= 0)) Then
            myByteArray(3) = PPM_Mode.SelectedIndex
            Call mySerialLink.SendBinaryDataToSerial(myByteArray, 4)
            PPM_Mode.ClearSelected()
        End If
    End Sub
    Private Sub RetrieveChannel1Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("1?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_1_Current.Text = strChannelBuffer
                ch1_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel2Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("2?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_2_Current.Text = strChannelBuffer
                ch2_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel3Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("3?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_3_Current.Text = strChannelBuffer
                ch3_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel4Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("4?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_4_Current.Text = strChannelBuffer
                ch4_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel5Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("5?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_5_Current.Text = strChannelBuffer
                ch5_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel6Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("6?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_6_Current.Text = strChannelBuffer
                ch6_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel7Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("7?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_7_Current.Text = strChannelBuffer
                ch7_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
    Private Sub RetrieveChannel8Information()
        Dim strChannelBuffer As String = ""
        If Not boolErrorFlag Then
            Call mySerialLink.SendDataToSerial("8?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_8_Current.Text = strChannelBuffer
                ch8_HScrollBar.Value = Val(strChannelBuffer)
            End If
        End If
    End Sub
End Class

