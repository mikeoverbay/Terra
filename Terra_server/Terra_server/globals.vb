
Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Module globals
  Public server_packId As UInteger = 1
  Public client_state(150) As Byte
  Public send_lock As New Object
  Public inc_lock As New Object
  Public form_closing As Boolean = False
    Private Structure clients_
        Public timeout As Integer
        Public msg_in As String
        Public ip As IPEndPoint

    End Structure
    Public Class state_
        Public wait_hdl_W = New ManualResetEvent(False)
        Public wait_hdl_R = New ManualResetEvent(False)
        Public bytes(20000) As Byte
        Public SendingSocket As UdpClient
        Public ReceivingEndpoint As EndPoint
        Public str As String = ""
        Public t_str As String = ""
        Public err As Integer = 0
  End Class
  Public ack_buff(150) As buff_
  Public send_buff(150) As buff_
  Public Structure buff_
    Public data() As Byte
    Public id As UInteger
    Public retry As Integer
  End Structure
End Module
