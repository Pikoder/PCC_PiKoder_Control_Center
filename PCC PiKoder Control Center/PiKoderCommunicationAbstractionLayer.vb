Imports Microsoft.VisualBasic

Public Class PiKoderCommunicationAbstractionLayer
    ' This class is designed as an abstraction layer to serve the different hardware communication links to the PiKoder 
    ' such as COM or WLAN. All PiKoder parameters or commands are executed transparently using respective methods. 
    '
    ' The class maintains information regarding the protocol when involved by EstablishLink(). From this point onwards an 
    ' existing connection would be monitored and an event would be generated once the connection is lost (LinkLost).
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
    '

    Public Enum iPhysicalLink As Integer
        iSerialLink
        iWLANlink
    End Enum
    Private mySerialLink As New SerialLink
    Private myWLANLink As New WLANLink
    Private Connected As Boolean = False ' connection status
    Private iConnectedTo As Integer
    Public Function LinkConnected() As Boolean
        Return Connected
    End Function
    Public Function EstablishLink(ByVal SelectedPort As String, ByVal ConnectionType As Integer) As Boolean
        Connected = False
        If (ConnectionType = iPhysicalLink.iSerialLink) Then
            Connected = mySerialLink.EstablishSerialLink(SelectedPort)
            If (Connected) Then
                iConnectedTo = iPhysicalLink.iSerialLink
            End If
        ElseIf (ConnectionType = iPhysicalLink.iWLANlink) Then
            Connected = myWLANLink.EstablishWLANLink()
            If (Connected) Then
                iConnectedTo = iPhysicalLink.iWLANlink
            End If
        End If
        Return Connected
    End Function
    Public Function DisconnectLink(ByVal ConnectionType As Integer) As Boolean
        If Connected And (ConnectionType = iPhysicalLink.iSerialLink) Then
            Call mySerialLink.MyForm_Dispose()
        ElseIf Connected And (ConnectionType = iPhysicalLink.iWLANlink) Then
            Call myWLANLink.MyForm_Dispose()
        End If
        MyForm_Dispose()
    End Function
    Public Sub GetHPPulseLength(ByRef SerialInputString As String, ByVal iChannelNo As Integer)
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial(iChannelNo.ToString() + "?")
            SerialInputString = mySerialLink.SerialReceiver()
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            myWLANLink.SendDataToWLAN(iChannelNo.ToString() + "?")
            SerialInputString = myWLANLink.Receiver()
        End If
    End Sub
    Public Sub GetPulseLength(ByRef SerialInputString As String, ByVal iChannelNo As Integer)
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial(iChannelNo.ToString() + "?")
            SerialInputString = mySerialLink.SerialReceiver()
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            Do
                myWLANLink.SendDataToWLAN(iChannelNo.ToString() + "?")
                SerialInputString = myWLANLink.Receiver()
            Loop Until ValidatePulseValue(SerialInputString)
        End If
    End Sub
    Public Sub GetNeutralPosition(ByRef SerialInputString As String, ByVal iChannelNo As Integer)
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial("N" + iChannelNo.ToString() + "?")
            SerialInputString = mySerialLink.SerialReceiver()
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            Do
                myWLANLink.SendDataToWLAN("N" + iChannelNo.ToString() + "?")
                SerialInputString = myWLANLink.Receiver()
            Loop Until ValidatePulseValue(SerialInputString)
        End If
    End Sub
    Public Sub GetLowerLimit(ByRef SerialInputString As String, ByVal iChannelNo As Integer)
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial("L" + iChannelNo.ToString() + "?")
            SerialInputString = mySerialLink.SerialReceiver()
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            Do
                myWLANLink.SendDataToWLAN("L" + iChannelNo.ToString() + "?")
                SerialInputString = myWLANLink.Receiver()
            Loop Until ValidatePulseValue(SerialInputString)
        End If
    End Sub
    Public Sub GetUpperLimit(ByRef SerialInputString As String, ByVal iChannelNo As Integer)
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial("U" + iChannelNo.ToString() + "?")
            SerialInputString = mySerialLink.SerialReceiver()
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            Do
                myWLANLink.SendDataToWLAN("U" + iChannelNo.ToString() + "?")
                SerialInputString = myWLANLink.Receiver()
            Loop Until ValidatePulseValue(SerialInputString)
        End If
    End Sub
    Public Sub GetIOType(ByRef SerialInputString As String, ByVal iChannelNo As Integer)
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial("O" + iChannelNo.ToString() + "?")
            SerialInputString = mySerialLink.SerialReceiver()
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            myWLANLink.SendDataToWLAN("O" + iChannelNo.ToString() + "?")
            SerialInputString = myWLANLink.Receiver()
        End If
    End Sub
    Public Sub GetFirmwareVersion(ByRef SerialInputString As String)
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial("0")
            SerialInputString = mySerialLink.SerialReceiver()
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            myWLANLink.SendDataToWLAN("0")
            SerialInputString = myWLANLink.Receiver()
        End If
    End Sub
    Public Sub GetStatusRecord(ByRef SerialInputString As String)
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial("?")
            If mySerialLink.SerialLinkConnected() Then  'Catch Error Message
                SerialInputString = mySerialLink.SerialReceiver()
            Else
                SerialInputString = "TimeOut"
            End If
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            myWLANLink.SendDataToWLAN("?")
            If myWLANLink.WLANLinkConnected() Then  'Catch Error Message
                SerialInputString = myWLANLink.Receiver()
            Else
                SerialInputString = "TimeOut"
            End If
        End If
    End Sub
    Public Sub GetTimeOut(ByRef SerialInputString As String)
        Dim iTimeOut As Integer = 0
        SerialInputString = ""
        Do Until (ValidateTimeOut(SerialInputString) Or (iTimeOut = 5))
            If (iConnectedTo = iPhysicalLink.iSerialLink) Then
                mySerialLink.SendDataToSerial("T?")
                SerialInputString = mySerialLink.SerialReceiver()
            ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
                myWLANLink.SendDataToWLAN("T?")
                SerialInputString = myWLANLink.Receiver()
            End If
            iTimeOut = iTimeOut + 1
        Loop
        If (iTimeOut = 5) Then SerialInputString = "TimeOut"
    End Sub
    Public Sub GetMiniSSCOffset(ByRef SerialInputString As String)
        Dim iTimeOut As Integer = 0
        SerialInputString = ""
        Do Until (ValidateZeroOffset(SerialInputString) Or (iTimeOut = 5))
            If (iConnectedTo = iPhysicalLink.iSerialLink) Then
                mySerialLink.SendDataToSerial("M?")
                SerialInputString = mySerialLink.SerialReceiver()
            ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
                myWLANLink.SendDataToWLAN("M?")
                SerialInputString = myWLANLink.Receiver()
            End If
            iTimeOut = iTimeOut + 1
        Loop
        If (iTimeOut = 5) Then SerialInputString = "TimeOut"
    End Sub
    Public Function SetChannelNeutral(ByVal strNeutralVal As String, ByVal iChannelNo As Integer) As Boolean
        Dim ReturnCode As String = ""
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial("N" + iChannelNo.ToString() + "=" + strNeutralVal)
            ReturnCode = mySerialLink.SerialReceiver()
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            myWLANLink.SendDataToWLAN("N" + iChannelNo.ToString() + "=" + strNeutralVal)
            ReturnCode = myWLANLink.Receiver()
        End If
        Return InterpretReturnCode(ReturnCode)
    End Function
    Public Function SetChannelLowerLimit(ByVal strNeutralVal As String, ByVal iChannelNo As Integer) As Boolean
        Dim ReturnCode As String = ""
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial("L" + iChannelNo.ToString() + "=" + strNeutralVal)
            ReturnCode = mySerialLink.SerialReceiver()
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            myWLANLink.SendDataToWLAN("L" + iChannelNo.ToString() + "=" + strNeutralVal)
            ReturnCode = myWLANLink.Receiver()
        End If
        Return InterpretReturnCode(ReturnCode)
    End Function
    Public Function SetChannelUpperLimit(ByVal strNeutralVal As String, ByVal iChannelNo As Integer) As Boolean
        Dim ReturnCode As String = ""
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            ReturnCode = mySerialLink.SendDataToSerialwithAck("U" + iChannelNo.ToString() + "=" + strNeutralVal)
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            myWLANLink.SendDataToWLAN("U" + iChannelNo.ToString() + "=" + strNeutralVal)
            ReturnCode = myWLANLink.Receiver()
        End If
        Return InterpretReturnCode(ReturnCode)
    End Function
    Public Function SetChannelOutputType(ByVal iChannelNo As Integer, ByVal strOutputType As String) As Boolean
        Dim ReturnCode As String = ""
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial("O" + iChannelNo.ToString() + "=" + strOutputType)
            ReturnCode = mySerialLink.SerialReceiver()
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            myWLANLink.SendDataToWLAN("O" + iChannelNo.ToString() + "=" + strOutputType)
            ReturnCode = myWLANLink.Receiver()
        End If
        Return InterpretReturnCode(ReturnCode)
    End Function
    Public Function SetChannelPulseLength(ByVal iChannelNo As Integer, ByVal strPulseLength As String) As Boolean
        Dim strSendString As String
        Dim ReturnCode As String = ""
        strSendString = iChannelNo.ToString() + "="
        If Len(strPulseLength) = 3 Then strSendString = strSendString + "0"
        strSendString = strSendString + strPulseLength
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial(strSendString)
            ReturnCode = mySerialLink.SerialReceiver()
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            myWLANLink.SendDataToWLAN(strSendString)
            ReturnCode = myWLANLink.Receiver()
        End If
        Return InterpretReturnCode(ReturnCode)
    End Function
    Public Function SetHPChannelPulseLength(ByVal iChannelNo As Integer, ByVal strPulseLength As String) As Boolean
        Dim strSendString As String
        Dim ReturnCode As String = ""
        strSendString = iChannelNo.ToString() + "="
        If Len(strPulseLength) = 4 Then strSendString = strSendString + "0"
        strSendString = strSendString + strPulseLength
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial(strSendString)
            ReturnCode = mySerialLink.SerialReceiver()
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            myWLANLink.SendDataToWLAN(strSendString)
            ReturnCode = myWLANLink.Receiver()
        End If
        Return InterpretReturnCode(ReturnCode)
    End Function
    Public Function SetPiKoderTimeOut(ByVal strTimeOut As String) As Boolean
        Dim myString As String = ""
        If Len(strTimeOut) = 1 Then myString = "00" + myString
        If Len(strTimeOut) = 2 Then myString = "0" + myString
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            Return InterpretReturnCode(mySerialLink.SendDataToSerialwithAck("T=" + myString + strTimeOut))
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            myWLANLink.SendDataToWLAN("T=")
            Return InterpretReturnCode(myWLANLink.Receiver())
        End If
    End Function
    Public Function SetPiKoderMiniSSCOffset(ByVal strMiniSSCOffset As String) As Boolean
        Dim strSendString As String = ""
        If Len(strMiniSSCOffset) = 1 Then strSendString = "00" + strSendString
        If Len(strMiniSSCOffset) = 2 Then strSendString = "0" + strSendString
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            Return InterpretReturnCode(mySerialLink.SendDataToSerialwithAck("M=" + strSendString + strMiniSSCOffset))
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            myWLANLink.SendDataToWLAN("M=")
            Return InterpretReturnCode(myWLANLink.Receiver())
        End If
    End Function
    Public Function SetPiKoderPreferences() As Boolean
        Dim ReturnCode As String = ""
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            mySerialLink.SendDataToSerial("S")
            Return InterpretReturnCode(mySerialLink.SerialReceiver())
        ElseIf (iConnectedTo = iPhysicalLink.iWLANlink) Then
            myWLANLink.SendDataToWLAN("S")
            Return InterpretReturnCode(myWLANLink.Receiver())
        End If
    End Function
    Public Function SetPiKoderPPMChannels(ByRef iNumberChannels As Integer) As Boolean
        Dim myByteArray() As Byte = {83, 21, 0, 0}
        myByteArray(3) = iNumberChannels
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            If Connected Then
                Call mySerialLink.SendBinaryDataToSerial(myByteArray, 4)
            End If
            Return True
        Else
        End If
    End Function
    Public Function SetPiKoderPPMMode(ByRef iPPMMode As Integer) As Boolean
        Dim myByteArray() As Byte = {83, 22, 0, 0}
        myByteArray(3) = iPPMMode
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            If Connected Then
                Call mySerialLink.SendBinaryDataToSerial(myByteArray, 4)
            End If
        End If
        Return True
    End Function
    Public Function PiKoderConnected() As Boolean
        If (iConnectedTo = iPhysicalLink.iSerialLink) Then
            Return mySerialLink.PiKoderConnected()
        Else
        End If
    End Function
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
    Private Function ValidateHPPulseValue(ByRef strVal As String) As Boolean
        Dim intChannelPulseLength As Double
        intChannelPulseLength = Val(strVal) 'no check on chars this time
        If (intChannelPulseLength < 3750) Or (intChannelPulseLength > 11250) Then
            Return False
        End If
        'format string
        If (intChannelPulseLength < 10000) And (Len(strVal) = 5) Then strVal = Mid(strVal, 2, 4)
        Return True
    End Function
    Private Function ValidateZeroOffset(ByRef strVal As String) As Boolean
        Dim intZeroOffset As Integer
        intZeroOffset = Val(strVal) 'no check on chars this time
        If (intZeroOffset < 0) Or (intZeroOffset > 248) Then
            Return False
        End If
        Return True
    End Function
    Private Function ValidateTimeOut(ByRef strVal As String) As Boolean
        Dim intZeroOffset As Integer
        intZeroOffset = Val(strVal) 'no check on chars this time
        If (intZeroOffset < 0) Or (intZeroOffset > 999) Then
            Return False
        End If
        Return True
    End Function
    Private Function InterpretReturnCode(ByVal RC) As Boolean
        If (Not String.Compare(RC, "!")) Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Sub MyForm_Dispose()
        Connected = False 'make sure to force new connect
    End Sub
End Class