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
                .WriteTimeout = 100
                .ReadTimeout = 100
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
    Public Function SerialReceiver() As String
        Dim myMessage As String = ""
        Dim Receiving As Boolean = True
        Dim messageStarted As Boolean = False
        Dim eomDetect As Integer = 2
        Dim myByte As Byte
        Dim j As Integer = 0

        While (Receiving)                                                     'Setup an infinite loop
            If (Connected And (mySerialPort.BytesToRead > 0)) Then ' make sure to receive complete message
                myByte = mySerialPort.ReadByte
                If ((myByte <> &HD) And (myByte <> &HA)) Then
                    myMessage = myMessage + Chr(myByte) 'Convert bytes back to string
                    messageStarted = True
                ElseIf (messageStarted) Then
                    eomDetect = eomDetect - 1
                    If (Not eomDetect) Then
                        Receiving = False
                    End If
                End If
            Else
                j = j + 1
                System.Threading.Thread.Sleep(10)
                If j = 20 Then
                    Receiving = False
                    Return "TimeOut"
                End If
            End If

        End While
        Return myMessage
    End Function
    Public Function SendDataToSerialwithAck(ByVal strWriteBuffer As String) As String
        Try
            mySerialPort.Write(strWriteBuffer, 0, Len(strWriteBuffer))
            Return SerialReceiver()
        Catch ex As SystemException
            Connected = False
            Return "?"
        End Try
    End Function
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
    Public Sub MyForm_Dispose()
        Try
            mySerialPort.Close()
            Connected = False 'make sure to force new connect
        Catch ex As Exception
            MessageBox.Show(ex.Message)
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
            If SendDataToSerialwithAck("*") = "?" Then
                Return True
            End If
        End If
        Return False
    End Function

End Class
