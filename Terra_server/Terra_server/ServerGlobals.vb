
Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports Lidgren.Network
Module Serverglobals
	Public ServerAlive As Boolean = False
	Public Serverclients As New List(Of client_)
	Enum packetType
		login
		login_ack
		logout
		chatMsg
		terra_state
		map_change
		client_reset
		driverChange
		timedOut
		req_client_list
		ack_req_client_list
	End Enum
	Enum clientType
		guest
		host
		driver
		driverAndhost
	End Enum
	Public Class client_
		Public Name As String
		Public client_type As Byte
		Public connection As NetConnection
	End Class
End Module
