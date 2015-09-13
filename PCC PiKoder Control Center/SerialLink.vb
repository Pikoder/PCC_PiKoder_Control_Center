Option Strict Off
Option Explicit On
Imports System.IO.Ports
Public Class SerialLink
    
    ' This class is designed to interface to a PiKoder through an available virtual serial
    ' port (COMn). 
    '
    ' The class would be involved by EstablishSerialLink(). From this point onwards an 
    ' existing connection would be monitored and an event would be generated once the
    ' connection is lost (SerialLinkLost).
    ' 
    ' The class provides for specialized methods to read and write PiKoder/COM information
    ' such as GetPulseLength() and SentPulseLength(). Please refer to the definitons for more
    ' details.
    '
    ' Copyright 2015 Gregor Schlechtriem
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
    '
    Private mySerialPort As New SerialPort
    Private Connected As Boolean = False ' connection status

    Public Function SerialLinkConnected() As Boolean
        Return Connected
    End Function
    Public Function EstablishSerialLink(ByVal SelectedPort As String) As Boolean
        If (mySerialPort.PortName = SelectedPort) And Connected Then Return True ' port is already open
        If Connected Then mySerialPort.Close() ' another port has been selected
        Try
            mySerialPort.PortName = SelectedPort
            With mySerialPort
                .BaudRate = 9600
                .DataBits = 8
                .Parity = Parity.None
                .StopBits = StopBits.One
                .Handshake = Handshake.None
            End With
            mySerialPort.Open()
            Connected = True
            Return True
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Connected = False
            Return False
        End Try
    End Function
    Public Sub GetPulseLength(ByRef SerialInputString As String)
        Dim i As Integer
        Dim j As Integer = 0
        Dim myByte As Char
        SerialInputString = ""
        Do
            If Connected And (mySerialPort.BytesToRead >= 8) Then ' make sure to receive complete message
                For i = 1 To mySerialPort.BytesToRead
                    myByte = Chr(mySerialPort.ReadByte)
                    If IsNumeric(myByte) Then SerialInputString = SerialInputString + myByte
                Next
                If ValidatePulseValue(SerialInputString) Then Return
            End If
            'check for TimeOut
            j = j + 1
            If j > 10 Then
                SerialInputString = "TimeOut" : Exit Do ' timeout exit
            End If
            System.Threading.Thread.Sleep(20) 'wait for next frame and allow for task switch
        Loop
    End Sub
    Public Sub GetTimeOut(ByRef SerialInputString As String, ByVal intCharsToRead As Integer)
        Dim i As Integer
        Dim j As Integer = 0
        Dim myByte As Char
        SerialInputString = ""
        '
        Call SendDataToSerial("T?")
        Do
            If Connected And (mySerialPort.BytesToRead >= intCharsToRead) Then ' make sure to receive complete message
                For i = 1 To mySerialPort.BytesToRead
                    myByte = Chr(mySerialPort.ReadByte)
                    If IsNumeric(myByte) Then SerialInputString = SerialInputString + myByte
                Next
                Return
            End If
            'check for TimeOut
            j = j + 1
            If j > 10 Then
                SerialInputString = "TimeOut" : Exit Do ' timeout exit
            End If
            System.Threading.Thread.Sleep(20) 'wait for next frame and allow for task switch
        Loop
    End Sub
    Public Sub GetMiniSSCOffset(ByRef SerialInputString As String, ByVal intCharsToRead As Integer)
        Dim i As Integer
        Dim j As Integer = 0
        Dim myByte As Char
        SerialInputString = ""
        Do
            If Connected And (mySerialPort.BytesToRead >= intCharsToRead) Then ' make sure to receive complete message
                For i = 1 To mySerialPort.BytesToRead
                    myByte = Chr(mySerialPort.ReadByte)
                    If IsNumeric(myByte) Then SerialInputString = SerialInputString + myByte
                Next
                If ValidateZeroOffset(SerialInputString) Then Return
            End If
            'check for TimeOut
            j = j + 1
            If j > 10 Then
                SerialInputString = "TimeOut" : Exit Do ' timeout exit
            End If
            System.Threading.Thread.Sleep(20) 'wait for next frame and allow for task switch
        Loop
    End Sub
    Public Sub GetNeutralPosition(ByRef SerialInputString As String, ByVal intCharsToRead As Integer)
        Dim i As Integer
        Dim j As Integer = 0
        Dim myByte As Char
        SerialInputString = ""
        Do
            If mySerialPort.BytesToRead >= intCharsToRead Then ' make sure to receive complete message
                For i = 1 To mySerialPort.BytesToRead
                    myByte = Chr(mySerialPort.ReadByte)
                    If IsNumeric(myByte) Then SerialInputString = SerialInputString + myByte
                Next
                Return
            End If
            'check for TimeOut
            j = j + 1
            If j > 10 Then
                SerialInputString = "TimeOut" : Exit Do ' timeout exit
            End If
            System.Threading.Thread.Sleep(20) 'wait for next frame and allow for task switch
        Loop
    End Sub
    Public Sub GetFirmwareVersion(ByRef SerialInputString As String)
        Dim j As Integer = 0
        Dim i As Integer
        Dim myByte As Char
        SerialInputString = ""
        Do
            If mySerialPort.BytesToRead >= 8 Then ' make sure to receive complete message
                For i = 1 To mySerialPort.BytesToRead
                    myByte = Chr(mySerialPort.ReadByte)
                    If myByte > Chr(31) Then SerialInputString = SerialInputString + myByte
                Next
                Return
            End If
            'check for TimeOut
            j = j + 1
            If j > 10 Then
                SerialInputString = "TimeOut" : Exit Do ' timeout exit
            End If
            System.Threading.Thread.Sleep(20) 'wait for next frame and allow for task switch
        Loop
    End Sub
    ''' <summary>
    ''' This one still has to move to a separate class PPMChannel t.b.d.
    ''' </summary>
    ''' <param name="strVal"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidatePulseValue(ByRef strVal As String) As Boolean
        Dim intChannelPulseLength As Double
        intChannelPulseLength = Val(strVal) 'no check on chars this time
        If (intChannelPulseLength < 750) Or (intChannelPulseLength > 2250) Then
            Return False
        End If
        'format string
        If (intChannelPulseLength < 1000) And (Len(strVal) = 4) Then strVal = Mid(strVal, 2, 3)
        Return True
    End Function
    Private Function ValidateZeroOffset(ByRef strVal As String) As Boolean
        Dim intZeroOffset As Integer
        intZeroOffset = Val(strVal) 'no check on chars this time
        If (intZeroOffset < 0) Or (intZeroOffset > 248) Then
            Return False
        End If
        'format string
        Return True
    End Function
    Public Sub SendPulseLengthToPiKoder(ByVal iChannel As Integer, ByVal strPulseLength As String)
        Dim strSendString As String
        Dim iError As Integer
        Do
            strSendString = Chr(iChannel + Asc("0")) + "="
            If Len(strPulseLength) = 3 Then strSendString = strSendString + "0"
            strSendString = strSendString + strPulseLength
            Try
                mySerialPort.Write(strSendString, 0, Len(strSendString))
                iError = GetErrorCode()
                If iError = 0 Then Exit Do ' job completed
                If iError = 2 Then iError = GetErrorCode() ' retry after timeout
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Connected = False
            End Try
        Loop While (iError <> 0) And Connected
    End Sub
    Public Sub SendTimeOutToPiKoder(ByVal strPulseLength As String)
        Dim strSendString As String
        strSendString = "T=" + strPulseLength
        Try
            mySerialPort.Write(strSendString, 0, Len(strSendString))
            GetErrorCode()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Connected = False
        End Try
    End Sub
    Public Sub SendMiniSSCOffsetToPiKoder(ByVal strPulseLength As String)
        Dim strSendString As String
        strSendString = "M=" + strPulseLength
        Try
            mySerialPort.Write(strSendString, 0, Len(strSendString))
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Connected = False
        End Try
    End Sub

    Public Sub SendDataToSerial(ByVal strWriteBuffer As String)
        Try
            mySerialPort.Write(strWriteBuffer, 0, Len(strWriteBuffer))
        Catch ex As SystemException
            Connected = False
        End Try
    End Sub
    Public Sub SendBinaryDataToSerial(ByVal myByteArray() As Byte, ByVal numBytes As Integer)
        Try
            mySerialPort.Write(myByteArray, 0, numBytes)
        Catch ex As SystemException
            Connected = False
        End Try
    End Sub
    Public Function GetErrorCode() As Integer
        Dim j As Integer
        Dim iRetCode As Integer
        Dim StartTime As DateTime
        Dim myChar As Char
        StartTime = Now
        Do
            If mySerialPort.BytesToRead >= 5 Then ' make sure to receive complete message
                For i = 1 To mySerialPort.BytesToRead
                    myChar = Chr(mySerialPort.ReadByte)
                    If myChar = "!" Then iRetCode = 0 
                    If myChar = "?" Then iRetCode = 1
                Next
                Return iRetCode
            End If
            'check for TimeOut
            j = j + 1
            If j > 10 Then
                Return 2 : Exit Do ' timeout exit
            End If
            System.Threading.Thread.Sleep(20) 'wait for next frame and allow for task switch
        Loop
    End Function

    Public Sub GetStatusRecord(ByRef SerialInputString As String)
        Dim j As Integer = 0
        Dim myByte As Char
        Dim CpMode As Integer = 0
        SerialInputString = ""
        Do
            While (Connected And (mySerialPort.BytesToRead > 0)) 'read complete message one by one
                myByte = Chr(mySerialPort.ReadByte)
                If ((myByte = Chr(10)) And (SerialInputString.Length = 0)) Then
                    CpMode = CpMode + 1
                ElseIf ((myByte = Chr(13)) And (SerialInputString.Length = 0)) Then
                    CpMode = CpMode + 1
                ElseIf ((myByte >= Chr(48)) And (CpMode = 2)) Then
                    SerialInputString = SerialInputString + myByte
                ElseIf ((myByte = Chr(10)) And (SerialInputString.Length > 0)) Then
                    If (CpMode = 1) Then
                        Return
                    Else : CpMode = CpMode - 1
                    End If
                ElseIf ((myByte = Chr(13)) And (SerialInputString.Length > 0)) Then
                    If (CpMode = 1) Then
                        Return
                    Else : CpMode = CpMode - 1
                    End If
                End If
            End While
            'check for TimeOut
            j = j + 1
            If j > 10 Then
                SerialInputString = "TimeOut" : Exit Do ' timeout exit
            End If
            System.Threading.Thread.Sleep(20) 'wait for next frame and allow for task switch
        Loop
    End Sub

    Public Sub MyForm_Dispose()
        Try
            mySerialPort.Close()
        Catch ex As Exception
        End Try
    End Sub
    ''' <summary>
    ''' Sent a single char and retreive error message
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function PiKoderConnected() As Boolean
        Dim strChannelBuffer As String = ""
        If Connected Then
            Call SendDataToSerial("*")
            If (GetErrorCode() = 1) Then Return True
        End If
        Return False
    End Function

End Class
