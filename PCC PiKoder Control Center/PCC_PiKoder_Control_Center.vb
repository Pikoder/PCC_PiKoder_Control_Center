Option Strict Off
Option Explicit On
Public Class PCC_PiKoder_Control_Center

    ' This is a program for evaluating the PiKoder platform
    ' 
    ' Gregor Schlechtriem
    ' Copyright (C) 2013-2015
    ' www.pikoder.com
    '
    ' You may incorporate all or parts of this program into your own project.
    ' Keep in mind that you got this code free and as such the author has no 
    ' responsibility to you, your customers, or anyone else regarding the use
    ' of this program code. No support or warrenty of any kind can be provided. All
    ' liability falls upon the user of this code and thus you must judge the suitability
    ' of this code to your application and usage.

    Inherits System.Windows.Forms.Form
    Private mySerialLink As New SerialLink

    ' declaration of variables
    Dim boolErrorFlag As Boolean ' global flag for errors in communication
    Dim boolNeutralAvailable As Boolean = True ' global flag indicating firmware release 0.34 or higher
    Dim boolLimitsAvailable As Boolean = True' feature available for releases > 0.34
    Dim boolZeroOffsetAvailable As Boolean = True ' feature available for releases > 0.35
    Dim boolMiniSSCOffsetAvailable As Boolean = True ' feature available for releases > 1.0
    Dim boolMiniSSCOffsetThreeDigits As Boolean = True 'feature available for release > 1.3
    Dim boolConsistentASCII As Boolean = True 'feature available for release > 1.4
    Dim intCharsToRead As Integer
    Dim boolSentChangeValueNotRequired(24) As Boolean 'avoid unnecessary sending of param. changes to PiKoder
    Dim boolAdvancedNeutral As Boolean = True 'feature available for release > 1.5

    ' declaration of subroutines
    ''' <summary>
    ''' This method is used to initialize the application. It will collect all
    ''' available COM ports in a list box. If no ports were found then the program
    ''' will stop.
    ''' </summary> 
    ''' <remarks></remarks>

    Public Sub New()

        InitializeComponent()

        Dim myStringBuffer As String ' used for temporary storage of COM port designator
        ' define and initialize array for sorting COM ports 
        Dim myCOMPorts(99) As String ' assume we have a max of 100 COM Ports
        Dim i As Integer = 0 'local buffer for serial array

        boolErrorFlag = False

        For Each s As String In myCOMPorts
            s = ""
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

        For Each s As String In myCOMPorts
            If s <> "" Then AvailableCOMPorts.Items.Add(s)
        Next

        ' Set active control 
        Me.ActiveControl = Me.AvailableCOMPorts

        ' Initialize LED
        Led2.Color = LED.LEDColorSelection.LED_Red
        Led2.Interval = 500
        Led2.State = True

        If AvailableCOMPorts.Items.Count = 0 Then
            MsgBox("No COM Port found. Program will stop.", MsgBoxStyle.OkOnly, "Error Message")
            End
        End If


    End Sub

    Private Sub AvailableCOMPorts_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AvailableCOMPorts.SelectedIndexChanged

        Dim strChannelBuffer As String = ""
        Dim strPiKoderType As String = ""

        If (AvailableCOMPorts.SelectedItem = Nothing) Then Exit Sub

        Led2.Blink = True

        boolErrorFlag = Not (mySerialLink.EstablishSerialLink(AvailableCOMPorts.SelectedItem))

        RetrievePiKoderType(strPiKoderType)

        If Not boolErrorFlag Then
            Led2.Blink = False ' indicate that the connection established
            Led2.Color = LED.LEDColorSelection.LED_Green
            If (InStr(strPiKoderType, "UART2PPM") > 0) Then
                TextBox1.Text = "Found UART2PPM @ " + AvailableCOMPorts.SelectedItem.ToString
                TypeId.Text = "UART2PPM"
                RetrieveSSCParameters()
                toggleHeartBeat()
                TextBox1.Text = "Parameters loaded ok."
            End If
        Else ' error message
            Led2.Blink = False
            TextBox1.Text = "Connect Error " + AvailableCOMPorts.SelectedItem.ToString
        End If

ErrorExit:
        AvailableCOMPorts.SelectedItem = Nothing
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
        RetrieveSSCParameters()
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
    ''' Subroutine for toggling heartbeat timer
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub toggleHeartBeat()
        If tHeartBeat.Enabled = True Then
            tHeartBeat.Enabled = False
        Else ' enable time an set interval (3s)
            tHeartBeat.Enabled = True
            tHeartBeat.Interval = 3000
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
        ' indicate that connection is lost
        Led2.Color = LED.LEDColorSelection.LED_Red
        toggleHeartBeat()

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

        ZeroOffset.ForeColor = Color.White
        strSSC_Firmware.Text = " "

        miniSSCOffset.ForeColor = Color.White

        ' close port
        mySerialLink.MyForm_Dispose()
    End Sub
    Private Sub RetrieveSSCParameters()

        Dim strChannelBuffer As String = ""

        For Each b As Boolean In boolSentChangeValueNotRequired
            b = False
        Next

        If mySerialLink.SerialLinkConnected() Then
            If Not boolErrorFlag Then
                'request status information from SSC    
                Call mySerialLink.SendDataToSerial("0")
                Call mySerialLink.GetFirmwareVersion(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strSSC_Firmware.Text = strChannelBuffer
                    If Val(strChannelBuffer) > 2.0 Then
                        MsgBox("The PiKoder firmware version found is not supported! Please goto www.pikoder.com and upgrade SSC Control Center to the latest version.", MsgBoxStyle.OkOnly, "Error Message")
                        End
                    End If
                Else ' error message
                    boolErrorFlag = True
                End If
            End If
            'retrieve information for channel 1
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("1?")
                Call mySerialLink.GetPulseLength(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_1_Current.Text = strChannelBuffer
                    ch1_HScrollBar.Value = Val(strChannelBuffer)
                End If
            End If

            'retrieve information for channel 2
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("2?")
                Call mySerialLink.GetPulseLength(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_2_Current.Text = strChannelBuffer
                    ch2_HScrollBar.Value = Val(strChannelBuffer)
                End If
            End If

            'retrieve information for channel 3
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("3?")
                Call mySerialLink.GetPulseLength(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_3_Current.Text = strChannelBuffer
                    ch3_HScrollBar.Value = Val(strChannelBuffer)
                End If
            End If

            'retrieve information for channel 4
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("4?")
                Call mySerialLink.GetPulseLength(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_4_Current.Text = strChannelBuffer
                    ch4_HScrollBar.Value = Val(strChannelBuffer)
                End If
            End If

            'retrieve information for channel 5
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("5?")
                Call mySerialLink.GetPulseLength(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_5_Current.Text = strChannelBuffer
                    ch5_HScrollBar.Value = Val(strChannelBuffer)
                End If
            End If

            'retrieve information for channel 6
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("6?")
                Call mySerialLink.GetPulseLength(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_6_Current.Text = strChannelBuffer
                    ch6_HScrollBar.Value = Val(strChannelBuffer)
                End If
            End If

            'retrieve information for channel 7
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("7?")
                Call mySerialLink.GetPulseLength(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_7_Current.Text = strChannelBuffer
                    ch7_HScrollBar.Value = Val(strChannelBuffer)
                End If
            End If

            'retrieve information for channel 8
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("8?")
                Call mySerialLink.GetPulseLength(strChannelBuffer)
                If strChannelBuffer <> "TimeOut" Then
                    strCH_8_Current.Text = strChannelBuffer
                    ch8_HScrollBar.Value = Val(strChannelBuffer)
                End If
            End If

            If Not boolAdvancedNeutral Then
                If boolNeutralAvailable Then
                    'retrieve neutral information for channel 1
                    If Not boolErrorFlag Then
                        Dim ch1_ByteArray() As Byte = {&H20, &HFF}
                        Call mySerialLink.SendBinaryDataToSerial(ch1_ByteArray)
                        Call mySerialLink.GetNeutralPosition(strChannelBuffer, 4)
                        If strChannelBuffer <> "TimeOut" Then
                            strCH_1_Neutral.Maximum = 254
                            strCH_1_Neutral.Value = Val(strChannelBuffer)
                            strCH_1_Neutral.ForeColor = Color.Black
                        End If
                    End If

                    'retrieve neutral information for channel 2
                    If Not boolErrorFlag Then
                        Dim ch2_ByteArray() As Byte = {&H21, &HFF}
                        Call mySerialLink.SendBinaryDataToSerial(ch2_ByteArray)
                        Call mySerialLink.GetNeutralPosition(strChannelBuffer, 4)
                        If strChannelBuffer <> "TimeOut" Then
                            strCH_2_Neutral.Value = Val(strChannelBuffer)
                            strCH_2_Neutral.ForeColor = Color.Black
                        End If
                    End If


                    'retrieve neutral information for channel 3
                    If Not boolErrorFlag Then
                        Dim ch3_ByteArray() As Byte = {&H22, &HFF}
                        Call mySerialLink.SendBinaryDataToSerial(ch3_ByteArray)
                        Call mySerialLink.GetNeutralPosition(strChannelBuffer, 4)
                        If strChannelBuffer <> "TimeOut" Then
                            strCH_3_Neutral.Value = Val(strChannelBuffer)
                            strCH_3_Neutral.ForeColor = Color.Black
                        End If
                    End If

                    'retrieve neutral information for channel 4
                    If Not boolErrorFlag Then
                        Dim ch4_ByteArray() As Byte = {&H23, &HFF}
                        Call mySerialLink.SendBinaryDataToSerial(ch4_ByteArray)
                        Call mySerialLink.GetNeutralPosition(strChannelBuffer, 4)
                        If strChannelBuffer <> "TimeOut" Then
                            strCH_4_Neutral.Value = Val(strChannelBuffer)
                            strCH_4_Neutral.ForeColor = Color.Black
                        End If
                    End If

                    'retrieve neutral information for channel 5
                    If Not boolErrorFlag Then
                        Dim ch5_ByteArray() As Byte = {&H24, &HFF}
                        Call mySerialLink.SendBinaryDataToSerial(ch5_ByteArray)
                        Call mySerialLink.GetNeutralPosition(strChannelBuffer, 4)
                        If strChannelBuffer <> "TimeOut" Then
                            strCH_5_Neutral.Value = Val(strChannelBuffer)
                            strCH_5_Neutral.ForeColor = Color.Black
                        End If
                    End If

                    'retrieve neutral information for channel 6
                    If Not boolErrorFlag Then
                        Dim ch6_ByteArray() As Byte = {&H25, &HFF}
                        Call mySerialLink.SendBinaryDataToSerial(ch6_ByteArray)
                        Call mySerialLink.GetNeutralPosition(strChannelBuffer, 4)
                        If strChannelBuffer <> "TimeOut" Then
                            strCH_6_Neutral.Value = Val(strChannelBuffer)
                            strCH_6_Neutral.ForeColor = Color.Black
                        End If
                    End If

                    'retrieve neutral information for channel 7
                    If Not boolErrorFlag Then
                        Dim ch7_ByteArray() As Byte = {&H26, &HFF}
                        Call mySerialLink.SendBinaryDataToSerial(ch7_ByteArray)
                        Call mySerialLink.GetNeutralPosition(strChannelBuffer, 4)
                        If strChannelBuffer <> "TimeOut" Then
                            strCH_7_Neutral.Value = Val(strChannelBuffer)
                            strCH_7_Neutral.ForeColor = Color.Black
                        End If
                    End If

                    'retrieve neutral information for channel 8
                    If Not boolErrorFlag Then
                        Dim ch8_ByteArray() As Byte = {&H27, &HFF}
                        Call mySerialLink.SendBinaryDataToSerial(ch8_ByteArray)
                        Call mySerialLink.GetNeutralPosition(strChannelBuffer, 4)
                        If strChannelBuffer <> "TimeOut" Then
                            strCH_8_Neutral.Value = Val(strChannelBuffer)
                            strCH_8_Neutral.ForeColor = Color.Black
                        End If
                    End If
                End If
            Else
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
            End If
            'retrieve min & max information for all channels
            If boolLimitsAvailable Then
                If boolConsistentASCII Then
                    intCharsToRead = 8
                Else : intCharsToRead = 4
                End If
                'channel 1
                If Not boolErrorFlag Then
                    Call mySerialLink.SendDataToSerial("L1?")
                    Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
                    If strChannelBuffer <> "TimeOut" Then
                        boolSentChangeValueNotRequired(0) = True
                        strCH_1_Min.Value = Val(strChannelBuffer)
                        ch1_HScrollBar.Minimum = strCH_1_Min.Value
                        strCH_1_Min.ForeColor = Color.Black
                    End If
                End If
                If Not boolErrorFlag Then
                    Call mySerialLink.SendDataToSerial("U1?")
                    Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
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
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(2) = True
                    strCH_2_Min.Value = Val(strChannelBuffer)
                    ch2_HScrollBar.Minimum = strCH_2_Min.Value
                    strCH_2_Min.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("U2?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
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
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(4) = True
                    strCH_3_Min.Value = Val(strChannelBuffer)
                    ch3_HScrollBar.Minimum = strCH_3_Min.Value
                    strCH_3_Min.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("U3?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
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
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(6) = True
                    strCH_4_Min.Value = Val(strChannelBuffer)
                    ch4_HScrollBar.Minimum = strCH_4_Min.Value
                    strCH_4_Min.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("U4?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
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
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(8) = True
                    strCH_5_Min.Value = Val(strChannelBuffer)
                    ch5_HScrollBar.Minimum = strCH_5_Min.Value
                    strCH_5_Min.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("U5?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
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
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(10) = True
                    strCH_6_Min.Value = Val(strChannelBuffer)
                    ch6_HScrollBar.Minimum = strCH_6_Min.Value
                    strCH_6_Min.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("U6?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
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
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(12) = True
                    strCH_7_Min.Value = Val(strChannelBuffer)
                    ch7_HScrollBar.Minimum = strCH_7_Min.Value
                    strCH_7_Min.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("U7?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
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
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(14) = True
                    strCH_8_Min.Value = Val(strChannelBuffer)
                    ch8_HScrollBar.Minimum = strCH_8_Min.Value
                    strCH_8_Min.ForeColor = Color.Black
                End If
            End If
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("U8?")
                Call mySerialLink.GetNeutralPosition(strChannelBuffer, intCharsToRead)
                If strChannelBuffer <> "TimeOut" Then
                    boolSentChangeValueNotRequired(15) = True
                    strCH_8_Max.Value = Val(strChannelBuffer)
                    ch8_HScrollBar.Maximum = strCH_8_Max.Value
                    strCH_8_Max.ForeColor = Color.Black
                End If

            End If
        End If
        'retrieve min & max information for all channels
        If boolConsistentASCII Then
            intCharsToRead = 6
        Else : intCharsToRead = 2
        End If
        If boolZeroOffsetAvailable Then
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("Z?")
                Call mySerialLink.GetZeroOffset(strChannelBuffer, intCharsToRead)
                If strChannelBuffer <> "TimeOut" Then
                    ZeroOffset.Value = Val(strChannelBuffer)
                    ZeroOffset.ForeColor = Color.Black
                End If
            End If
        End If
        'retrieve miniSSC offset
        If boolMiniSSCOffsetAvailable Then
            If Not boolErrorFlag Then
                Call mySerialLink.SendDataToSerial("M?")
                Call mySerialLink.GetMiniSSCOffset(strChannelBuffer, intCharsToRead)
                If strChannelBuffer <> "TimeOut" Then
                    miniSSCOffset.Value = Val(strChannelBuffer)
                    miniSSCOffset.ForeColor = Color.Black
                End If
            End If
        End If

    End Sub


    Private Sub strCH_1_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_1_Neutral.ValueChanged
        If Not boolAdvancedNeutral Then
            Dim ch1_ByteArray() As Byte = {&H20, Convert.ToByte(strCH_1_Neutral.Value)}
            Dim strChannelBuffer As String = ""
            Call mySerialLink.SendBinaryDataToSerial(ch1_ByteArray)
            ' update position 
            Call mySerialLink.SendDataToSerial("1?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_1_Current.Text = strChannelBuffer
            Else ' error message
                boolErrorFlag = True
            End If
        Else
            If mySerialLink.SerialLinkConnected Then
                If Not boolSentChangeValueNotRequired(16) Then
                    Dim strChannelBuffer As String = "N1="
                    If strCH_1_Neutral.Value < 1000 Then
                        strChannelBuffer = strChannelBuffer + "0"
                    End If
                    strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_1_Neutral.Value)
                    Call mySerialLink.SendDataToSerial(strChannelBuffer)
                End If
                boolSentChangeValueNotRequired(16) = False
            End If
        End If
    End Sub

    Private Sub strCH_2_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_2_Neutral.ValueChanged
        If Not boolAdvancedNeutral Then
            Dim ch2_ByteArray() As Byte = {&H21, Convert.ToByte(strCH_2_Neutral.Value)}
            Dim strChannelBuffer As String = ""
            Call mySerialLink.SendBinaryDataToSerial(ch2_ByteArray)
            ' update position 
            Call mySerialLink.SendDataToSerial("2?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_2_Current.Text = strChannelBuffer
            Else ' error message
                boolErrorFlag = True
            End If
        Else
            If mySerialLink.SerialLinkConnected Then
                If Not boolSentChangeValueNotRequired(17) Then
                    Dim strChannelBuffer As String = "N2="
                    If strCH_2_Neutral.Value < 1000 Then
                        strChannelBuffer = strChannelBuffer + "0"
                    End If
                    strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_2_Neutral.Value)
                    Call mySerialLink.SendDataToSerial(strChannelBuffer)
                End If
                boolSentChangeValueNotRequired(17) = False
            End If
        End If
    End Sub
    Private Sub strCH_3_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_3_Neutral.ValueChanged
        If Not boolAdvancedNeutral Then
            Dim ch3_ByteArray() As Byte = {&H22, Convert.ToByte(strCH_3_Neutral.Value)}
            Dim strChannelBuffer As String = ""
            Call mySerialLink.SendBinaryDataToSerial(ch3_ByteArray)
            ' update position 
            Call mySerialLink.SendDataToSerial("3?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_3_Current.Text = strChannelBuffer
            Else ' error message
                boolErrorFlag = True
            End If
        Else
            If mySerialLink.SerialLinkConnected Then
                If Not boolSentChangeValueNotRequired(18) Then
                    Dim strChannelBuffer As String = "N3="
                    If strCH_2_Neutral.Value < 1000 Then
                        strChannelBuffer = strChannelBuffer + "0"
                    End If
                    strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_3_Neutral.Value)
                    Call mySerialLink.SendDataToSerial(strChannelBuffer)
                End If
                boolSentChangeValueNotRequired(18) = False
            End If
        End If
    End Sub
    Private Sub strCH_4_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_4_Neutral.ValueChanged
        If Not boolAdvancedNeutral Then
            Dim ch4_ByteArray() As Byte = {&H23, Convert.ToByte(strCH_4_Neutral.Value)}
            Dim strChannelBuffer As String = ""
            Call mySerialLink.SendBinaryDataToSerial(ch4_ByteArray)
            ' update position 
            Call mySerialLink.SendDataToSerial("4?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_4_Current.Text = strChannelBuffer
            Else ' error message
                boolErrorFlag = True
            End If
        Else
            If mySerialLink.SerialLinkConnected Then
                If Not boolSentChangeValueNotRequired(19) Then
                    Dim strChannelBuffer As String = "N4="
                    If strCH_4_Neutral.Value < 1000 Then
                        strChannelBuffer = strChannelBuffer + "0"
                    End If
                    strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_4_Neutral.Value)
                    Call mySerialLink.SendDataToSerial(strChannelBuffer)
                End If
                boolSentChangeValueNotRequired(19) = False
            End If
        End If

    End Sub
    Private Sub strCH_5_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_5_Neutral.ValueChanged
        If Not boolAdvancedNeutral Then
            Dim ch5_ByteArray() As Byte = {&H24, Convert.ToByte(strCH_5_Neutral.Value)}
            Dim strChannelBuffer As String = ""
            Call mySerialLink.SendBinaryDataToSerial(ch5_ByteArray)
            ' update position 
            Call mySerialLink.SendDataToSerial("5?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_5_Current.Text = strChannelBuffer
            Else ' error message
                boolErrorFlag = True
            End If
        Else
            If mySerialLink.SerialLinkConnected Then
                If Not boolSentChangeValueNotRequired(20) Then
                    Dim strChannelBuffer As String = "N5="
                    If strCH_5_Neutral.Value < 1000 Then
                        strChannelBuffer = strChannelBuffer + "0"
                    End If
                    strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_5_Neutral.Value)
                    Call mySerialLink.SendDataToSerial(strChannelBuffer)
                End If
                boolSentChangeValueNotRequired(20) = False
            End If
        End If
    End Sub
    Private Sub strCH_6_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_6_Neutral.ValueChanged
        If Not boolAdvancedNeutral Then
            Dim ch6_ByteArray() As Byte = {&H25, Convert.ToByte(strCH_6_Neutral.Value)}
            Dim strChannelBuffer As String = ""
            Call mySerialLink.SendBinaryDataToSerial(ch6_ByteArray)
            ' update position 
            Call mySerialLink.SendDataToSerial("6?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_6_Current.Text = strChannelBuffer
            Else ' error message
                boolErrorFlag = True
            End If
        Else
            If mySerialLink.SerialLinkConnected Then
                If Not boolSentChangeValueNotRequired(21) Then
                    Dim strChannelBuffer As String = "N6="
                    If strCH_6_Neutral.Value < 1000 Then
                        strChannelBuffer = strChannelBuffer + "0"
                    End If
                    strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_6_Neutral.Value)
                    Call mySerialLink.SendDataToSerial(strChannelBuffer)
                End If
                boolSentChangeValueNotRequired(21) = False
            End If
        End If
    End Sub
    Private Sub strCH_7_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_7_Neutral.ValueChanged
        If Not boolAdvancedNeutral Then
            Dim ch7_ByteArray() As Byte = {&H26, Convert.ToByte(strCH_7_Neutral.Value)}
            Dim strChannelBuffer As String = ""
            Call mySerialLink.SendBinaryDataToSerial(ch7_ByteArray)
            ' update position 
            Call mySerialLink.SendDataToSerial("7?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_7_Current.Text = strChannelBuffer
            Else ' error message
                boolErrorFlag = True
            End If
        Else
            If mySerialLink.SerialLinkConnected Then
                If Not boolSentChangeValueNotRequired(22) Then
                    Dim strChannelBuffer As String = "N7="
                    If strCH_7_Neutral.Value < 1000 Then
                        strChannelBuffer = strChannelBuffer + "0"
                    End If
                    strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_7_Neutral.Value)
                    Call mySerialLink.SendDataToSerial(strChannelBuffer)
                End If
                boolSentChangeValueNotRequired(22) = False
            End If
        End If
    End Sub
    Private Sub strCH_8_Neutral_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles strCH_8_Neutral.ValueChanged
        If Not boolAdvancedNeutral Then
            Dim ch8_ByteArray() As Byte = {&H27, Convert.ToByte(strCH_8_Neutral.Value)}
            Dim strChannelBuffer As String = ""
            Call mySerialLink.SendBinaryDataToSerial(ch8_ByteArray)
            ' update position 
            Call mySerialLink.SendDataToSerial("8?")
            Call mySerialLink.GetPulseLength(strChannelBuffer)
            If strChannelBuffer <> "TimeOut" Then
                strCH_8_Current.Text = strChannelBuffer
            Else ' error message
                boolErrorFlag = True
            End If
        Else
            If mySerialLink.SerialLinkConnected Then
                If Not boolSentChangeValueNotRequired(23) Then
                    Dim strChannelBuffer As String = "N8="
                    If strCH_8_Neutral.Value < 1000 Then
                        strChannelBuffer = strChannelBuffer + "0"
                    End If
                    strChannelBuffer = strChannelBuffer + Convert.ToString(strCH_8_Neutral.Value)
                    Call mySerialLink.SendDataToSerial(strChannelBuffer)
                End If
                boolSentChangeValueNotRequired(23) = False
            End If
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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
                Call mySerialLink.SendDataToSerial(strChannelBuffer)
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

    Private Sub ZeroOffset_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ZeroOffset.ValueChanged
        Dim myStringBuffer As String = "" ' used for temporary storage of value
        If mySerialLink.SerialLinkConnected Then
            If ZeroOffset.Value < 10 Then myStringBuffer = "0"
            Call mySerialLink.SendZeroOffsetToPiKoder(myStringBuffer + Convert.ToString(ZeroOffset.Value))
        End If
    End Sub
    Private Sub miniSSCOffset_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles miniSSCOffset.ValueChanged
        Dim myStringBuffer As String = "" ' used for temporary storage of value
        If mySerialLink.SerialLinkConnected Then
            If boolMiniSSCOffsetThreeDigits Then
                If miniSSCOffset.Value < 100 Then myStringBuffer = "0"
            End If
            If miniSSCOffset.Value < 10 Then myStringBuffer = myStringBuffer + "0"
            Call mySerialLink.SendMiniSSCOffsetToPiKoder(myStringBuffer + Convert.ToString(miniSSCOffset.Value))
        End If
    End Sub
End Class

