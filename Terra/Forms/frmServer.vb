Imports System
Imports System.Linq
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.IO
Imports System.Collections.Generic
Imports Lidgren.Network
Public Class frmServer
	Dim chat_buffer As New List(Of String)
	Dim diag_buffer As New List(Of String)
	'--------------------------------------------------------------------------------
	Dim server_ As New Thread(AddressOf main_loop)
	Dim cnt As Long = 0
	Dim magicPacket() As Byte = {1, 9, 5, 6} '= 26001 decimal
	Dim myip As String
	Dim in_port As Integer = 1956
	Public serverAlive As Boolean = False
	Public Server As NetServer
	Public Config As NetPeerConfiguration
	Public winnormState As Boolean = False
	Dim pcount As UInteger
	Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		'While Not ServerAlive
		'	Application.DoEvents()
		'	'wait for the main loop to finish
		'End While
		Try
			For Each cl In Server.Connections
				cl.Disconnect("Server Shutting down...")
			Next

			Server.Shutdown("Server Shut Down!")
		Catch ex As Exception
		End Try
		Thread.Sleep(500)
		ImHost = False
		ImDriver = False
		NetData = False
	End Sub

	Private Sub frmmain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Diag_tb.Visible = False
		Diag_tb.Text = ""
		stop_btn.Enabled = False
		Packet_out.Ex = look_point_X
		Packet_out.Ez = look_point_Z
		Packet_out.Rx = Cam_X_angle
		Packet_out.Ry = Cam_Y_angle
		Packet_out.Lr = View_Radius

	End Sub




	Private Sub start_btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles start_btn.Click
		Diag_tb.Text = ""
		chat_text_tb.Text = ""
		stop_btn.Focus()
		chat_buffer = New List(Of String)
		Config = New NetPeerConfiguration(password_tb.Text)  ' think this can be used to stop unwanted connections
		Config.Port = in_port
		Config.MaximumConnections = 30
		Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval)
		Config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage)
		Config.EnableMessageType(NetIncomingMessageType.Error)
		Config.EnableMessageType(NetIncomingMessageType.WarningMessage)
		Config.EnableMessageType(NetIncomingMessageType.DebugMessage)
		Config.ConnectionTimeout = 10.0!
		Config.ReceiveBufferSize = 1000
		Config.SendBufferSize = 1000
		Config.PingFrequency = 5.0
		'now that the config is setup.. pass it to the server
		Server = New NetServer(Config)
		start_btn.Enabled = False
		Thread.Sleep(20)
		ServerAlive = True
		Server.Start()	' start lindgren server

		Diag_tb.Text = "Server Started" + vbCrLf
		If ImHost Then
			frmClient.Visible = True
			frmClient.echo_window_tb.Text = ""
			frmClient.diag_tb.Text = ""
			frmClient.imHost_cb.Checked = True
			frmMain.m_show_chat.Visible = True
			frmMain.m_show_chat.Checked = True
			frmMain.m_show_chat.Text = "Hide Chat"
			Me.WindowState = FormWindowState.Minimized
		Else
			'better never be here connecting as a client ;)
		End If
		Thread.Sleep(100)
		Application.DoEvents()
		stop_btn.Enabled = True
		ImHost = True
		start_thread()

		ServerAlive = True
	End Sub
	Private Sub start_thread()
		server_.IsBackground = True
		server_.Priority = ThreadPriority.Normal
		server_.Name = "Server"
		server_.Start()

		While server_.IsAlive
			If ImHost Then
				If Not frmClient.Visible Then
					Me.WindowState = FormWindowState.Normal
					Me.TopMost = True
				End If
			End If
			Thread.Sleep(30)
			Application.DoEvents()
		End While
	End Sub
	Private Sub main_loop()
		Dim inc As NetIncomingMessage
		write_diagnostics("Entering Listening loop" + vbCrLf)
		Dim msgStack As New List(Of NetOutgoingMessage)
		Dim state_msgStack As New List(Of NetOutgoingMessage)
		While ServerAlive
			inc = Server.ReadMessage
			If inc IsNot Nothing Then
				Select Case inc.MessageType
					Case NetIncomingMessageType.DebugMessage
						Dim s = inc.ReadString
						Debug.Write(s + vbCrLf)
					Case NetIncomingMessageType.WarningMessage
					Case NetIncomingMessageType.Error
						Dim s = inc.ReadString
						write_diagnostics(DateTime.Now.ToLongTimeString + " : " + s + vbCrLf)
					Case NetIncomingMessageType.ErrorMessage
					Case NetIncomingMessageType.DebugMessage
					Case NetIncomingMessageType.VerboseDebugMessage
						Dim s = inc.ReadString
						If Not s.Contains("Found ") Then
							write_diagnostics(DateTime.Now.ToLongTimeString + " : " + s + vbCrLf)
						End If

					Case NetIncomingMessageType.StatusChanged
						If inc.SenderConnection.Status = NetConnectionStatus.Disconnected Or _
							inc.SenderConnection.Status = NetConnectionStatus.Disconnecting Then
							Try

								For Each u In Serverclients
									Dim status = inc.SenderConnection.RemoteUniqueIdentifier
									If (u.connection.RemoteUniqueIdentifier = status) Then
										Dim outMsg As NetOutgoingMessage = Server.CreateMessage
										Dim dummy = inc.ReadByte
										Dim ls As String = inc.ReadString
										If ls.Contains("Thanks") Then
											outMsg.Write(CByte(packetType.logout))
										Else
											outMsg.Write(CByte(packetType.timedOut))
										End If
										outMsg.Write(u.client_type)
										outMsg.Write(u.Name)
										Server.SendMessage(outMsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0)
										Serverclients.Remove(u)
									End If
								Next
							Catch ex As Exception

							End Try
						End If
					Case NetIncomingMessageType.ConnectionApproval
						If inc.ReadByte() = CByte(packetType.login) Then ' code for login
							Thread.Sleep(20)
							inc.SenderConnection.Approve()

							Dim client As New client_
							client.client_type = inc.ReadByte ' host, driver. guest.. ect.
							client.connection = inc.SenderConnection

							client.Name = inc.ReadString ' name.. duh
							Write_chat(" " + client.Name + " Connected" + vbCrLf)
							Dim client_exist As Boolean = False
							If Serverclients.Count = 0 Then
								client_exist = True
								Serverclients.Add(client)
							Else
								For i = 30 To Serverclients.Count
									Try
										If Serverclients(i).Name = client.Name Then
											Serverclients(i).connection.Disconnect("You are already here" + vbCrLf + "Making a new home for you.")
											While Serverclients(i).connection.Status = NetConnectionStatus.Connected
												write_diagnostics(client.Name + " in list already!.. Removing it." + vbCrLf)
											End While
											client_exist = True
											Serverclients.RemoveAt(i)
											Serverclients.Add(client)
											Exit For
										End If
									Catch ex As Exception
									End Try
								Next
								If Not client_exist Then
									Serverclients.Add(client)
								End If
								Thread.Sleep(20)

							End If
							'Need to add this to the list of clients
							'and send this to everyone.
							Dim outMsg As NetOutgoingMessage = Server.CreateMessage
							outMsg.Write(CByte(packetType.login_ack))
							outMsg.Write(CByte(Serverclients.Count)) ' tell it how many names are coming
							For Each c In Serverclients
								'this server just connected and 
								'has no idea who is already logged in so
								'lets send that info back. this will update
								'all clients that are connected.
								outMsg.Write(c.client_type)
								outMsg.Write(c.Name)
							Next
							'send some chat history too
							outMsg.Write(CByte(chat_buffer.Count))
							For Each m In chat_buffer
								outMsg.Write(m)
							Next
							Server.SendMessage(outMsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0)

						End If
					Case NetIncomingMessageType.Data
						Select Case inc.ReadByte
							'-----------------------------------------------------------------------------
							'-----------------------------------------------------------------------------
							Case packetType.logout
								Dim cl As New client_
								Dim b = inc.ReadByte
								cl.Name = inc.ReadString
								Dim outMsg As NetOutgoingMessage = Server.CreateMessage
								outMsg.Write(CByte(packetType.logout))
								outMsg.Write(b)
								outMsg.Write(cl.Name)
								Dim c As Integer = 0
								For Each u In Serverclients
									If u.Name = cl.Name Then
										Serverclients.RemoveAt(cnt)
									End If
									cnt += 1
								Next
								Server.SendMessage(outMsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0)
								'-----------------------------------------------------------------------------
								'-----------------------------------------------------------------------------
							Case packetType.req_client_list
								Dim requesting_clinet = inc.SenderConnection
								Dim outMsg As NetOutgoingMessage = Server.CreateMessage
								outMsg.Write(CByte(packetType.ack_req_client_list))
								outMsg.Write(CByte(Serverclients.Count)) ' tell it how many names are coming
								For Each c In Serverclients
									'this server just connected and 
									'has no idea who is already logged in so
									'lets send that info back. this will update
									'all clients that are connected.
									outMsg.Write(c.client_type)
									outMsg.Write(c.Name)
								Next
								'send some chat history too
								outMsg.Write(CByte(chat_buffer.Count))
								For Each m In chat_buffer
									outMsg.Write(m)
								Next
								Server.SendMessage(outMsg, requesting_clinet, NetDeliveryMethod.ReliableOrdered, 0)
								'-----------------------------------------------------------------------------
								'-----------------------------------------------------------------------------

							Case packetType.chatMsg
								Dim outMsg As NetOutgoingMessage = Server.CreateMessage
								outMsg.Write(CByte(packetType.chatMsg))
								Dim n = inc.ReadString
								Dim m = inc.ReadString
								outMsg.Write(n) 'user name
								outMsg.Write(m) ' chat msg
								Write_chat("(" + n + ") " + m + vbCrLf)
								Server.SendMessage(outMsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0)
								'msgStack.Add(outMsg)
								'-----------------------------------------------------------------------------
								'-----------------------------------------------------------------------------
							Case packetType.driverChange
								Dim outMsg As NetOutgoingMessage = Server.CreateMessage
								outMsg.Write(CByte(packetType.driverChange))
								Dim n = inc.ReadString
								For Each u In Serverclients
									If u.Name = n Then
										If u.client_type = clientType.host Or _
											u.client_type = clientType.driverAndhost Then
											u.client_type = clientType.driverAndhost
										Else
											If u.client_type = clientType.guest Then
												u.client_type = clientType.driver
											End If
										End If
									Else
										If u.client_type <> clientType.host Then ' never change the host type
											u.client_type = clientType.guest

										End If
									End If
								Next
								outMsg.Write(n) 'user name
								Server.SendMessage(outMsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0)
								'-----------------------------------------------------------------------------
								'-----------------------------------------------------------------------------
							Case packetType.terra_state
								Dim outMsg As NetOutgoingMessage = Server.CreateMessage
								outMsg.Write(CByte(packetType.terra_state)) ' packetType
								Dim l = inc.ReadByte	' total size of this packet
								outMsg.Write(l)	' data size
								Dim d() = inc.ReadBytes(l)	' copy the data
								For Each b In d
									outMsg.Write(b) ' write the data
								Next
								'pcount += 1
								'diag_buffer.Clear()
								'write_diagnostics(pcount.ToString)
								Try
									Server.SendMessage(outMsg, Server.Connections, NetDeliveryMethod.UnreliableSequenced, 0)
									'state_msgStack.Add(outMsg)

								Catch ex As Exception
									write_diagnostics(ex.Message)
								End Try
								'-----------------------------------------------------------------------------
							Case packetType.map_change
								Dim outMsg As NetOutgoingMessage = Server.CreateMessage
								outMsg.Write(CByte(packetType.map_change))
								outMsg.Write(inc.ReadString)
								Try
									Server.SendMessage(outMsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0)
								Catch ex As Exception
									write_diagnostics(ex.Message)
								End Try
								'-----------------------------------------------------------------------------
							Case packetType.client_reset
								Dim outMsg As NetOutgoingMessage = Server.CreateMessage
								outMsg.Write(CByte(packetType.client_reset))
								Try
									Server.SendMessage(outMsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0)
								Catch ex As Exception
									write_diagnostics(ex.Message)
								End Try
						End Select
					Case Else
						'Dim s = inc.ReadString
						'write_diagnostics(s + vbCrLf)

				End Select
				Server.Recycle(inc)
			End If
			Thread.Sleep(30)
time_:
			Application.DoEvents()
		End While
	End Sub

	Private Delegate Sub AppendChat(ByVal st As String)
	Private Sub Write_chat(ByVal st As String)
		If Me.InvokeRequired Then
			Try
				Me.Invoke(New AppendChat(AddressOf Write_chat), New Object() {st})
			Catch ex As Exception
			End Try

		Else
			Try
				If chat_buffer.Count > 10 Then
					chat_buffer.RemoveAt(0)
				End If
				chat_buffer.Add(st)
				chat_text_tb.Clear()
				Dim s As String = ""
				For Each m In chat_buffer
					s += m
				Next
				chat_text_tb.AppendText(s)
			Catch ex As Exception
			End Try
		End If
	End Sub


	Private Delegate Sub w_diag_Delgegate(ByVal s As String)
	Private Sub write_diagnostics(ByVal s As String)
		If Me.InvokeRequired Then
			Try
				Me.Invoke(New w_diag_Delgegate(AddressOf write_diagnostics), New Object() {s})

			Catch ex As Exception

			End Try
		Else
			Try
				If diag_buffer.Count > 30 Then
					diag_buffer.RemoveAt(0)
				End If
				diag_buffer.Add(s)
				Dim m As String = ""
				For Each n In diag_buffer
					m += n
				Next
				Diag_tb.Clear()
				Diag_tb.AppendText(m)
			Catch ex As Exception
			End Try

		End If
	End Sub


	Private Sub stop_btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles stop_btn.Click
		ServerAlive = False
		Thread.Sleep(200)
		start_btn.Enabled = True
		stop_btn.Enabled = False
		Me.Close()
	End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		Dim proc As New System.Diagnostics.Process()

		proc = Process.Start("http://www.whatsmyip.org/", "")
	End Sub


	Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
		Dim proc As New System.Diagnostics.Process()

		proc = Process.Start("http://portforward.com/", "")

	End Sub

	Private Sub diagnostics_bt_Click(sender As Object, e As EventArgs) Handles diagnostics_bt.Click
		If Diag_tb.Visible Then
			Diag_tb.Visible = False
		Else
			Diag_tb.Visible = True
		End If
	End Sub

	Private Sub cancel_bt_Click(sender As Object, e As EventArgs) Handles cancel_bt.Click
		serverAlive = False
		Thread.Sleep(200)
		start_btn.Enabled = True
		stop_btn.Enabled = False
		Me.Close()
        frmMain.m_session.Enabled = True
        frmMain.m_host_session.Enabled = True
        frmMain.m_join_server_as_host.Enabled = True
        frmMain.m_join_session.Enabled = True
	End Sub
End Class
