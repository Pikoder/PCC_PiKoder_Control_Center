Imports System.Net
Imports System.Text.Encoding
Imports System.Net.Sockets
Imports System.Threading


Public Class WLANLink

    ' This is a program for evaluating the PiKoder platform - please refer to http://pikoder.com for more details.
    ' 
    ' Copyright 2019 Gregor Schlechtriem
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


    '''''''''''''''''''''''Set up variables''''''''''''''''''''
    Private Const txPort As Integer = 12001                      'Port number to send data on
    Private Const rxPort As Integer = 12000                      'Port number to recieve data on
    Private Const broadcastAddress As String = "255.255.255.255" 'Sends data to all LOCAL listening clients, to send data over WAN you'll need to enter a public (external) IP address of the other client
    Private receivingClient As UdpClient                         'Client for handling incxoming data
    Private sendingClient As UdpClient                           'Client for sending data
    Private receivingThread As Thread                            'Create a separate thread to listen for incoming data, helps to prevent the form from freezing up
    Private closing As Boolean = False                           'Used to close clients if form is closing
    Private Connected As Boolean = False                         'connection status
    Private MessageBuffer As String = ""
    Private MessageFullyReceived As Boolean = False
    Private endPoint As IPEndPoint

    ''''''''''''''''''''Initialize listening & sending subs'''''''''''''''''
    '
    Public Function EstablishWLANLink() As Boolean
        InitializeSender()          'Initializes startup of sender client
        InitializeReceiver()        'Starts listening for incoming data                                             
        Connected = True
        MessageFullyReceived = False
        MessageBuffer = ""
        Return Connected
    End Function
    ''''''''''''''''''''Setup sender client'''''''''''''''''
    '
    Private Sub InitializeSender()
        sendingClient = New UdpClient(broadcastAddress, txPort)
        sendingClient.EnableBroadcast = True
    End Sub
    '''''''''''''''''''''Setup receiving client'''''''''''''
    '
    Private Sub InitializeReceiver()
        receivingClient = New UdpClient(rxPort)
    End Sub
    Public Function Receiver() As String
        Dim iTimeOutCounter As Integer = 0
        While Not MessageFullyReceived And iTimeOutCounter < 5
            Thread.Sleep(100)
            iTimeOutCounter = iTimeOutCounter + 1
        End While
        If iTimeOutCounter = 5 Then
            Try
                receivingThread.Abort()
            Catch ex As Exception
            End Try
            MessageFullyReceived = False 'free up buffer
            Return "TimeOut"
        End If
        MessageFullyReceived = False 'free up buffer
        Return (MessageBuffer)
    End Function
    Private Sub ReceiverThread()
        Dim endPoint As IPEndPoint = New IPEndPoint(IPAddress.Any, rxPort) 'Listen for incoming data from any IP address on the specified port
        Dim myMessage As String = ""
        Dim Receiving As Boolean = True
        Dim messageStarted As Boolean = False
        Dim eomDetect As Integer = 2
        While (Receiving)                                                     'Setup an infinite loop
            Try
                Dim rcvbytes() As Byte = receivingClient.Receive(endPoint)   'Receive incoming bytes
                If ((rcvbytes(0) <> &HD) And (rcvbytes(0) <> &HA)) Then
                    myMessage = myMessage + System.Text.Encoding.ASCII.GetString(rcvbytes) 'Convert bytes back to string
                    messageStarted = True
                ElseIf (messageStarted) Then
                    eomDetect = eomDetect - 1
                    If (Not eomDetect) Then
                        Receiving = False
                    End If
                End If
            Catch ex As Exception
            End Try
        End While
        MessageBuffer = String.Copy(myMessage)
        MessageFullyReceived = True
    End Sub
    Public Function WLANLinkConnected() As Boolean
        Return Connected
    End Function
    Public Sub SendDataToWLAN(ByVal strWriteBuffer As String)
        Dim sendbytes() As Byte = ASCII.GetBytes(strWriteBuffer)
        Dim start As ThreadStart = New ThreadStart(AddressOf ReceiverThread)
        receivingThread = New Thread(start)
        receivingThread.IsBackground = True
        receivingThread.Start()
        sendingClient.Send(sendbytes, sendbytes.Length)
    End Sub
    Public Sub MyForm_Dispose()
        Try
            receivingThread.Abort()
        Catch ex As Exception
        End Try
        MessageFullyReceived = False
        receivingClient.Close()
        sendingClient.Close()
    End Sub
End Class





