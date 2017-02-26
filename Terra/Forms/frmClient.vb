Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.IO
Imports System.Threading
Imports Lidgren.Network

Public Class frmClient
	Dim server_port As Integer = 1956
	Delegate Sub update_tb(ByVal s As String)
	public server_dead As Boolean = False
	Dim ping_time As Single
	Dim diag_msg_buff As New List(Of String)
	Dim msg_lock As New Object
	Dim chat_buffer_line_cnt As Integer = 30
	'create thread
	Dim get_messages As New Thread(AddressOf get_messages_thread)
	Dim pump_messages As New Thread(AddressOf pump_messages_thread)
	Dim old_tank_string As String = ""
	Dim autosynch As New AutoResetEvent(False)
	Dim pcount As UInteger = 0
	Dim client_reset As Boolean = False

	Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		Thread.Sleep(100)
		Panel1.Dock = DockStyle.Bottom
		Panel1.Height = Me.ClientSize.Height - 55
		'this will just sit here until both threads had shut down
		server_dead = True
		CanStart = True
		While pump_messages.IsAlive
			While get_messages.IsAlive
				Application.DoEvents()
				Thread.Sleep(50)
			End While
		End While
		frmMain.m_join_session.Enabled = True
		frmMain.m_host_session.Enabled = True
		frmMain.m_join_server_as_host.Enabled = True
		If ImHost Then
			If frmServer.Visible Then
				frmServer.WindowState = FormWindowState.Normal
				frmServer.Focus()
			End If
		End If
		ImHost = False
		ImDriver = False
		NetData = False
		frmMain.m_show_chat.Visible = False
		
	End Sub
	Public Sub shut_down_gracefully()
		Try
			client.Disconnect("Thanks, It was fun") ' NEVER CHANGE THIS TEXT!!!! Its used serve logic!!
		Catch ex As Exception
		End Try
		Try
			While client.ConnectionsCount > 0
				Application.DoEvents()
			End While
			client.Shutdown("See ya")
		Catch ex As Exception
		End Try
		server_dead = True
		Thread.Sleep(100)
		ImDriver = False
		If ImHost Then
			If frmServer.Visible Then
				frmServer.WindowState = FormWindowState.Normal
				frmServer.Focus()
			End If
		End If
		Me.Close()
	End Sub

	Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		diag_tb.BringToFront()
		diag_tb.Text = DateTime.Now.ToShortTimeString + vbCrLf
		diag_tb.Visible = False
		stop_button.Enabled = True
		ImDriver = False
		echo_window_tb.Text = ""
		Me.Text = "Terra! Chat: " + client_name_tb.Text
	End Sub

	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
		client_reset = False	' clear this  every startup
		Packet_in.I_Id = ""
		echo_window_tb.Text = ""
		chat_message_buffer.Clear()
		Application.DoEvents()
		clients = New List(Of client_)
		Dim cl As New client_
		cl.Name = client_name_tb.Text
		clients.Add(cl)
		Application.DoEvents()
		Application.DoEvents()
		Button1.Enabled = False
		stop_button.Enabled = True
		'this will connect and fire up the threads
		If Not ImHost Then
			NetData = True
		End If
		ImDriver = False
		Panel1.Dock = DockStyle.Fill
		Start_server()

		Application.DoEvents()
		Application.DoEvents()
		chat_entry_tb.Focus()
		server_dead = False
		Packet_in.tankId = -1
		Packet_out.tankId = -1
	End Sub


	Private Sub Start_server()
		ImDriver = False
		diag_tb.Clear()
		server_dead = False
		CanStart = False
		Dim strHostName As String
		Dim strIPAddress As String = ""
		strHostName = System.Net.Dns.GetHostName()
		For Each ipa In System.Net.Dns.GetHostEntry(strHostName).AddressList
			If Not ipa.IsIPv6LinkLocal And Not ipa.IsIPv6Multicast And Not ipa.IsIPv6SiteLocal And Not ipa.IsIPv6Teredo Then
				strIPAddress = ipa.ToString()
			End If
		Next
		write_diagnostics(DateTime.Now.ToLongTimeString + " using IP: " + strIPAddress + vbCrLf)

		Dim Config As New NetPeerConfiguration(password_tb.Text)
		Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval)
		Config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage)
		'Config.Port = 1956
		'Config.LocalAddress = System.Net.IPAddress.Parse(strIPAddress)

		'Config.EnableMessageType(NetIncomingMessageType.Error)
		'Config.EnableMessageType(NetIncomingMessageType.WarningMessage)
		'Config.EnableMessageType(NetIncomingMessageType.DebugMessage)
		'Config.Port = server_port
		Config.ReceiveBufferSize = 1000
		Config.SendBufferSize = 1000
		Config.ConnectionTimeout = 10.0!
		client = New NetClient(Config)
		Dim hostip As String = dest_ip_tb.Text
		client.Start()
		Dim outMsg As NetOutgoingMessage = client.CreateMessage
		outMsg.Write(CByte(packetType.login))
		If imHost_cb.Checked Then
			outMsg.Write(CByte(clientType.host))
		Else
			outMsg.Write(CByte(clientType.guest))
		End If
		outMsg.Write(client_name_tb.Text)
		client.Connect(hostip, 1956, outMsg)
		Thread.Sleep(1000)
		If Not WaitForServerConnection() Then
			chat_message_buffer.Clear()
			AppendTextBox(vbCrLf + "Server never responded" + vbCrLf)
			Button1.Enabled = True
			Return ' no point in doing aything else
		End If
		'set focus on chat input box
		chat_entry_tb.Focus()
		'disabe the controls so they cant be changed
		dest_ip_tb.Enabled = False
		client_name_tb.Enabled = False
		password_tb.Enabled = False
		'We are connected so fire up the threads
		Dim get_messages As New Thread(AddressOf get_messages_thread)
		Dim pump_messages As New Thread(AddressOf pump_messages_thread)
		Dim draw_it As New Thread(AddressOf display)
		draw_it.Priority = ThreadPriority.Normal
		draw_it.IsBackground = True
		draw_it.Name = "drawScene"
		get_messages.Priority = ThreadPriority.Highest
		get_messages.IsBackground = True
		get_messages.Name = "get_messages"
		pump_messages.Priority = ThreadPriority.Normal
		pump_messages.IsBackground = True
		pump_messages.Name = "pump_messages"
		pump_messages.Start()
		get_messages.Start()
		draw_it.Start()
		'this will just sit here until both threads had shut down
		While pump_messages.IsAlive
			While get_messages.IsAlive
				Application.DoEvents()
				Thread.Sleep(50)
			End While
		End While
		While draw_it.IsAlive
			autosynch.Set()
			Application.DoEvents()
		End While

		Ping_text.Text = ""
		dest_ip_tb.Enabled = False
		client_name_tb.Enabled = False
		password_tb.Enabled = False
		Button1.Enabled = True
		AppendTextBox("")
		Button1.Enabled = True
	End Sub

	Private Function WaitForServerConnection() As Boolean
		AppendTextBox("Please wait " + client_name_tb.Text + " while I connect you..." + vbCrLf)
		Dim inc As NetIncomingMessage
		clients = New List(Of client_)
		Dim cnt As Integer = 0
		diag_tb.AppendText(" Starting connection with server..." + vbCrLf)
		Dim time_n As New Stopwatch
		time_n.Start()
		While Not CanStart And Not server_dead
			Application.DoEvents()
			Application.DoEvents()
			'going to need to add a way to escape this endless loop
			'in case the connection fails.
			inc = client.ReadMessage
			If inc IsNot Nothing Then
				Select Case inc.MessageType
					Case NetIncomingMessageType.StatusChanged

						Dim s = client.ReadMessage()
						write_diagnostics(s.ReadString + vbCrLf)
						If inc.SenderConnection.Status = NetConnectionStatus.Connected Then
							'CanStart = True
							write_diagnostics("Connect Respond time: " + _
								CStr(time_n.ElapsedMilliseconds / 1000) + "ms" + vbCrLf)
							Thread.Sleep(500)
						End If
						Try
							write_diagnostics(s.ReadString + vbCrLf)

						Catch ex As Exception

						End Try
					Case NetIncomingMessageType.Data
						If inc.ReadByte = packetType.login_ack Then
							write_diagnostics("Received connection response" + vbCrLf)
							'get client count
							Dim c = inc.ReadByte
							For i = 0 To c - 1
								Dim cl = New client_
								cl.client_type = inc.ReadByte
								cl.Name = inc.ReadString
								If cl.client_type = clientType.host Then
									If cl.Name = client_name_tb.Text Then
										cl.host = True
										ImHost = True
									End If
								Else
									cl.host = False
									ImHost = False
								End If
								'test if we get back our name..
								'means the server is aware of us
								If cl.Name = client_name_tb.Text Then
									chat_message_buffer.Clear()
									echo_window_tb.AppendText("Welcome to Terra! " + cl.Name + vbCrLf)
								End If
								clients.Add(cl)
							Next
							'read chat_buffer length
							c = inc.ReadByte
							Dim ts As String = ""
							Dim tss As String = ""
							For i = 0 To c - 1
								tss = inc.ReadString
								'remove any login mssages.
								If Not tss.Contains("Connected") Then
									ts += tss
									chat_message_buffer.Add(tss)
								End If
							Next
							echo_window_tb.AppendText(ts)
							update_client_panel()
							'need to update display
							CanStart = True
							Return True
							Exit Select
						End If
					Case NetIncomingMessageType.DebugMessage
						Dim s = inc.ReadString
						Debug.Write(s + vbCrLf)
					Case NetIncomingMessageType.WarningMessage
					Case NetIncomingMessageType.Error
					Case NetIncomingMessageType.ErrorMessage
					Case NetIncomingMessageType.DebugMessage
					Case NetIncomingMessageType.VerboseDebugMessage
						Dim s = inc.ReadString
						If Not s.Contains("Found ") Then
							write_diagnostics(DateTime.Now.ToLongTimeString + " : " + s + vbCrLf)
						End If

					Case Else
						Dim s = inc.ReadString
						Debug.Write(s + vbCrLf)

				End Select
			End If
			Application.DoEvents()
			Thread.Sleep(50)
			cnt += 1
			If cnt > 40 Then
				If client.ConnectionsCount > 0 Then
					write_diagnostics(vbCrLf + "Connected but never got back connection ACK.." + vbCrLf _
											+ "Sending request for Client list" + vbCrLf + vbCrLf)
					Dim msg = client.CreateMessage
					msg.Write(CByte(packetType.req_client_list))
					chat_message_buffer.Clear()
					AppendTextBox("Welcome to Terra! " + client_name_tb.Text + vbCrLf)
					Thread.Sleep(50)
					client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0)
					Return True
				End If
				write_diagnostics(DateTime.Now.ToLongTimeString + "Server never connected" + vbCrLf)

				Return False ' server not responding
			End If
		End While
	End Function

	'========================================================

	Private Sub get_messages_thread()
		Dim p_type, c As Byte
		Dim inc As NetIncomingMessage
		While Not server_dead
			inc = client.ReadMessage

			If inc IsNot Nothing Then
				Select Case inc.MessageType
					Case NetIncomingMessageType.StatusChanged
						If inc.SenderConnection.Status = NetConnectionStatus.Disconnected Or _
					inc.SenderConnection.Status = NetConnectionStatus.Disconnecting Then
							Dim s = inc.ReadString
							write_diagnostics("StatusChanged : " + s + vbCrLf)
							Try
								If chat_message_buffer.Count > 3 Then
									chat_message_buffer.Clear()
								End If
								AppendTextBox(vbCrLf + "Server Disconnected!" + vbCrLf)
								'clean out list of clients and update it
								clients.Clear()
								update_client_panel()
								server_dead = True ' this will kill the threads
								Thread.Sleep(200)	' give the threads time to shut down
								Return
							Catch ex As Exception
							End Try
						End If

					Case NetIncomingMessageType.Data
						p_type = inc.ReadByte
						Select Case p_type

							'--------------------------------------------------------------------------------------------
							Case packetType.login_ack	'new client logged in

								'This will refresh the connected user list
								'every time some one connects.
								'get client count
								clients.Clear()

								c = inc.ReadByte
								For i = 0 To c - 1
									Dim cl = New client_
									cl.client_type = inc.ReadByte
									cl.Name = inc.ReadString
									clients.Add(cl)
								Next
								'read chat_buffer length
								c = inc.ReadByte
								Dim ts As String = ""
								Dim tss As String = ""
								For i = 0 To c - 1
									tss = inc.ReadString	' just dispose of the data.. dont need it
									'ts += tss
									'chat_message_buffer.Add(tss)
								Next
								'echo_window_tb.AppendText(ts)
								update_client_panel()
								Exit Select
								'--------------------------------------------------------------------------------------------
							Case packetType.ack_req_client_list		'new client logged in

								'This will refresh the connected user list
								'every time some one connects.
								'get client count
								clients.Clear()

								c = inc.ReadByte
								For i = 0 To c - 1
									Dim cl = New client_
									cl.client_type = inc.ReadByte
									cl.Name = inc.ReadString
									clients.Add(cl)
								Next
								'read chat_buffer length
								c = inc.ReadByte
								Dim ts As String = ""
								Dim tss As String = ""
								For i = 0 To c - 1
									tss = inc.ReadString	' just dispose of the data.. dont need it
									ts += tss
								Next
								AppendTextBox(ts)
								update_client_panel()
								Exit Select
								'--------------------------------------------------------------------------------------------
							Case packetType.logout
								Dim cl As New client_
								cl.client_type = inc.ReadByte
								cl.Name = inc.ReadString
								If cl.client_type = clientType.host Then
									AppendTextBox("The one Hosting this session has left the house!" + vbCrLf)
								End If
								If cl.client_type = clientType.driverAndhost Then
									AppendTextBox("The one Hosting this session has left the house!" + vbCrLf)
									AppendTextBox("No one is the driver... You can wait for the host to come " + _
													  "back or close this down." + vbCrLf)
								End If
								If cl.client_type = clientType.driver Then
									AppendTextBox("The previous Driver disconnected." + vbCrLf)
									If ImHost Then
										ImDriver = True
									End If
									For Each cl In clients
										If cl.client_type = clientType.host Then
											cl.client_type = clientType.driverAndhost
										End If
									Next
								End If
								'this client signed out.. remove it from the list
								For Each u In clients
									If u.Name = cl.Name Then
										clients.Remove(u)
										update_client_panel()
										Exit For
									End If
								Next
								'--------------------------------------------------------------------------------------------
							Case packetType.chatMsg
								Dim cl As New client_
								'update chat text window
								AppendTextBox("(" + inc.ReadString + ") " + inc.ReadString + vbCrLf)
								My.Computer.Audio.PlaySystemSound(SystemSounds.Beep)
								'--------------------------------------------------------------------------------------------
							Case packetType.timedOut
								Dim cl As New client_
								cl.client_type = inc.ReadByte
								cl.Name = inc.ReadString
								If cl.client_type = clientType.host Then
									AppendTextBox("The one Hosting this session has timed out!" + vbCrLf)
								End If
								If cl.client_type = clientType.driverAndhost Then
									AppendTextBox("The one Hosting this session has timed out!" + vbCrLf)
									AppendTextBox("No one is the driver... You can wait for the host to come " + _
													  "back or close this down." + vbCrLf)
								End If
								If cl.client_type = clientType.driver Then
									AppendTextBox("The previous Driver has timmed out." + vbCrLf)
									If ImHost Then
										ImDriver = True
										NetData = False
									End If
									For Each cl In clients
										If cl.client_type = clientType.host Then
											cl.client_type = clientType.driverAndhost
										End If
									Next
								End If
								'this client timmed out.. remove it from the list
								For Each u In clients
									If u.Name = cl.Name Then
										clients.Remove(u)
										update_client_panel()
										Exit For
									End If
								Next
								'--------------------------------------------------------------------------------------------
							Case packetType.driverChange
								Dim cl As New client_
								cl.Name = inc.ReadString
								If cl.Name = client_name_tb.Text Then
									ImDriver = True
									NetData = False
								Else
									ImDriver = False
									NetData = True
									'need to redirect any editting events
								End If
								For Each u In clients
									If u.Name = cl.Name Then
										If u.client_type = clientType.host _
											Or u.client_type = clientType.driverAndhost Then
											u.client_type = clientType.driverAndhost
										Else
											'make this one driver even it the 
											'command was sent 2 times or more by
											'clicking its name over and over.
											u.client_type = clientType.driver

										End If
									Else
										If u.client_type = clientType.driverAndhost Then
											u.client_type = clientType.host
										End If
										If u.client_type <> clientType.host Then ' never change the host type
											u.client_type = clientType.guest
										End If
									End If
								Next
								Dim ts As String = cl.Name + " " + " is now the driver." + vbCrLf
								AppendTextBox(ts)
								'will need to update the display
								update_client_panel()
								'----------------------------------------------------
								'----------------------------------------------------
								'update terra's state
							Case packetType.terra_state
								get_terra_state(inc)
								'pcount += 1
								'updatePing(pcount.ToString)
								'----------------------------------------------------
								'----------------------------------------------------
							Case packetType.map_change
								Dim file_n = inc.ReadString
								If file_n <> load_map_name Then
									net_loadmap(file_n)
								End If
								'----------------------------------------------------
								'----------------------------------------------------
							Case packetType.client_reset
								net_reset()
							Case packetType.req_client_list
								'dont need to ack this.
								'write_diagnostics(DateTime.Now.ToLongTimeString + "in msg: Client list requested" + vbCrLf)
							Case packetType.req_client_list
								'dont need to ack this.
								'write_diagnostics(DateTime.Now.ToLongTimeString + "in msg: Client list requested" + vbCrLf)
							Case Else
								'and thing here goes in the diagnostics screen too.
								'Dim s = inc.ReadString
								'write_diagnostics("non-Captured data: " + s + vbCrLf)
								write_diagnostics(DateTime.Now.ToLongTimeString + " Unknown data : " + p_type.ToString + vbCrLf)
						End Select
					Case NetIncomingMessageType.WarningMessage
					Case NetIncomingMessageType.Error
					Case NetIncomingMessageType.ErrorMessage
					Case NetIncomingMessageType.DebugMessage
					Case NetIncomingMessageType.VerboseDebugMessage
						Dim s = inc.ReadString
						If Not s.Contains("Found ") Then
							write_diagnostics(DateTime.Now.ToLongTimeString + " : " + s.ToString + vbCrLf)
						End If
						Try
							updatePing("Ping: " + CStr(Math.Round(client.Connections(0).AverageRoundtripTime * 1000.0!, 2).ToString) + "ms")
						Catch ex As Exception
						End Try
				End Select
				client.Recycle(inc) ' saves time by reusing this message
			End If
			Thread.Sleep(20)
		End While

	End Sub

	'========================================================

	Private Sub pump_messages_thread()
		'pumps state messages if this client is the driver
		While Not server_dead
			If ImDriver And Not client_reset Then
				Prepare_and_send_state()
			Else
				If client_reset Then
					send_reset()
					send_reset()
					send_reset()
				End If
			End If
			process_terra_state_packet()

			Thread.Sleep(60) ' 16.66 UDPS
		End While
		ImDriver = False
	End Sub

	Private Sub display()
		While Not server_dead
			autosynch.WaitOne()
			draw()
			Thread.Sleep(10)
		End While

	End Sub


	Private Sub send_reset()
		If ImHost And client.Connections.Count > 0 Then
			Dim Dmsg As NetOutgoingMessage = client.CreateMessage
			Dmsg.Write(packetType.client_reset)
			client.SendMessage(Dmsg, NetDeliveryMethod.ReliableOrdered, 0)
		End If
		client_reset = False
	End Sub
	'========================================================
	Private Sub Prepare_and_send_state()
		If Not frmTanks.Visible Then
			'current_tank_index = -1
		End If
		If Not maploaded Then
			Return ' cant send data that doesn't exist
		End If
		Dim Dmsg As NetOutgoingMessage = client.CreateMessage
		Dim data() As Byte
		Dim ms As New MemoryStream()
		Dim bw As New BinaryWriter(ms)
		Dim size As Byte = 0

		bw.Write(Packet_out.Ex)	'x
		bw.Write(Packet_out.Ez)	'z
		bw.Write(Packet_out.Rx)	'rot_x
		bw.Write(Packet_out.Ry)	'rot_y
		bw.Write(Packet_out.Lr)
		size = 20 '(4*5)
		SyncLock packet_lock
			bw.Write(Packet_out.tankId)
			size += 2
			If Packet_out.tankId <> -1 Then
				bw.Write(Packet_out.Tx)
				bw.Write(Packet_out.Tz)
				bw.Write(Packet_out.Tr)
				size += 12 '(3*4)
				bw.Write(Packet_out.ID)
				size += Packet_out.ID.Length + 1
				bw.Write(Packet_out.comment)
				size += Packet_out.comment.Length + 1
			End If

			Dim tp = current_tank_index
			bw.Write(CShort(tp))
			size += 2
			If tp = -1 Then
				'jump to no_more_tank_data.
				'there is no point in sending this data id
				'the host is not editing tanks.
				GoTo No_tank_data
			End If
			current_tank_index += 1
			If current_tank_index > 29 Then
				Dim msg As NetOutgoingMessage = client.CreateMessage
				msg.Write(CByte(packetType.map_change))
				msg.Write(load_map_name)
				If ImHost And client.ConnectionsCount > 0 Then
					SyncLock msg_lock
						client.SendMessage(msg, NetDeliveryMethod.UnreliableSequenced)
					End SyncLock
				End If
				current_tank_index = 0
			End If

			If tp < 15 Then
				bw.Write(locations.team_1(tp).loc_x)
				bw.Write(locations.team_1(tp).loc_z)
				bw.Write(locations.team_1(tp).rot_y)
				bw.Write(locations.team_1(tp).id)
				size += (4 * 3) + locations.team_1(tp).id.Length + 1 ' it will have the srt len at the in the data
				bw.Write(locations.team_1(tp).comment)
				size += locations.team_1(tp).comment.Length + 1
			Else
				bw.Write(locations.team_2(tp - 15).loc_x)
				bw.Write(locations.team_2(tp - 15).loc_z)
				bw.Write(locations.team_2(tp - 15).rot_y)
				bw.Write(locations.team_2(tp - 15).id)
				size += (4 * 3) + locations.team_2(tp - 15).id.Length + 1 ' it will have the srt len at the in the data
				bw.Write(locations.team_2(tp - 15).comment)
				size += locations.team_2(tp - 15).comment.Length + 1
			End If

No_tank_data:
		End SyncLock
		data = ms.ToArray
		ms.Dispose()
		bw.Close()
		'build the state data packet
		Dmsg.Write(CByte(packetType.terra_state))
		Dmsg.Write(size)
		For i = 0 To data.Length - 1
			Dmsg.Write(data(i))
		Next
		If ImDriver And client.ConnectionsCount > 0 Then
			SyncLock msg_lock
				client.SendMessage(Dmsg, NetDeliveryMethod.UnreliableSequenced)
			End SyncLock
		End If
	End Sub
	'========================================================
	Private Sub get_terra_state(ByRef inc As NetIncomingMessage)
		'get driver x,z,rot,x and rot_y
		Dim indexed_tank As Integer = 0
		Dim l = inc.ReadByte
		Packet_in.Ex = inc.ReadSingle
		Packet_in.Ez = inc.ReadSingle
		Packet_in.Rx = inc.ReadSingle
		Packet_in.Ry = inc.ReadSingle
		Packet_in.Lr = inc.ReadSingle
		'-------
		Packet_in.tankId = inc.ReadInt16
		If Packet_in.tankId <> -1 Then
			Packet_in.Tx = inc.ReadSingle
			Packet_in.Tz = inc.ReadSingle
			Packet_in.Tr = inc.ReadSingle
			Packet_in.ID = inc.ReadString
			Packet_in.comment = inc.ReadString
		End If
		'-------
no_tank_data1:
		Packet_in.index_tank = inc.ReadInt16
		If Packet_in.index_tank = -1 Then GoTo no_tank_data
		Packet_in.Ix = inc.ReadSingle
		Packet_in.Iz = inc.ReadSingle
		Packet_in.Ir = inc.ReadSingle
		Packet_in.I_Id = ""
		Packet_in.I_Id = inc.ReadString
		Packet_in.Icomment = inc.ReadString
no_tank_data:
		autosynch.Set()

	End Sub


	'========================================================
	Private Sub process_terra_state_packet()
		'-------------------------------
		If ImDriver Then
			Return
		End If
		Dim team As Integer = 0
		Dim b_index As Integer = 0
		tankID = Packet_in.tankId
		If tankID <> -1 Then

			Dim ar = Packet_in.ID.Split("_")
			b_index = CInt(ar(3))
			Dim t_id = CInt(ar(2))
			Dim nation = ar(1)
			team = CInt(ar(0))
			Dim img As New Object
			Dim txt As String = ""

			If team = 1 Then
				locations.team_1(b_index).loc_x = Packet_in.Tx
				locations.team_1(b_index).loc_z = Packet_in.Tz
				locations.team_1(b_index).rot_y = Packet_in.Tr
				locations.team_1(b_index).comment = Packet_in.comment
				frmMain.m_comment.Text = Packet_in.comment
			Else
				locations.team_2(b_index).loc_x = Packet_in.Tx
				locations.team_2(b_index).loc_z = Packet_in.Tz
				locations.team_2(b_index).rot_y = Packet_in.Tr
				locations.team_2(b_index).comment = Packet_in.comment
				frmMain.m_comment.Text = Packet_in.comment
			End If
		End If
		If Packet_in.I_Id.Length > 0 Then
			'			uTank.id = Packet_in.I_Id
			uTank.loc_x = Packet_in.Ix
			uTank.loc_z = Packet_in.Iz
			uTank.rot_y = Packet_in.Ir
			uTank.id = Packet_in.I_Id
			uTank.comment = Packet_in.Icomment
			update_tank_form()
		End If
		If NetOldtankID <> tankID Then
			For i = 0 To 14
				select_button(1, i, Color.DarkRed)
			Next
			For i = 0 To 14
				select_button(2, i, Color.Green)
			Next
			NetOldtankID = tankID
			If Packet_in.tankId <> -1 Then
				select_button(team, b_index, Color.Blue)
			End If
		End If


	End Sub
	'========================================================
	Private Delegate Sub select_button_Delgegate(ByVal t As Integer, i As Integer, c As Color)
	Private Sub select_button(ByVal t As Integer, i As Integer, c As Color)
		'highlights the currently selected tank on a clients tank setup list
		If Me.InvokeRequired Then
			Try
				Me.Invoke(New select_button_Delgegate(AddressOf select_button), New Object() {t, i, c})
			Catch ex As Exception
			End Try
		Else
			Try
				If t = 1 Then
					frmTanks.SplitContainer1.Panel1.Controls(i).BackColor = c
				Else
					frmTanks.SplitContainer1.Panel2.Controls(i).BackColor = c
				End If
			Catch ex As Exception
			End Try
		End If
	End Sub

	'========================================================
	Private Delegate Sub draw_Delgegate()
	Private Sub draw()
		If Me.InvokeRequired Then
			'Try
			Me.Invoke(New draw_Delgegate(AddressOf draw))
			'
		Else
			'Try
			Try
				frmMain.update_screen()

			Catch ex As Exception

			End Try
		End If
	End Sub
	'========================================================

	Private Delegate Sub update_tank_form_Delgegate()
	Private Sub update_tank_form()
		If Me.InvokeRequired Then
			Try
				Me.Invoke(New update_tank_form_Delgegate(AddressOf update_tank_form))
			Catch ex As Exception
			End Try
		Else
			Try
				'global_frmTanks.Show()
				If uTank.id.Length = 0 Then
					Return
				End If
				Dim Id, team, b_index As Integer
				Dim ar = uTank.id.Split("_")
				If ar.Length > 2 Then
					team = CInt(ar(0))
					Id = CInt(ar(2))
					b_index = CInt(ar(3))
				Else
					Return ' there is no data for this button
				End If
				If team = 1 Then
					If locations.team_1(b_index).id = uTank.id Then
						Return ' we dont want to update tanks that are already updated
					End If
					frmTanks.SplitContainer1.Panel1.Controls(b_index).Font = _
									New Font(frmMain.pfc.Families(0), 6, System.Drawing.FontStyle.Regular)
					frmTanks.SplitContainer1.Panel1.Controls(b_index).BackColor = Color.DarkRed
					locations.team_1(b_index).loc_x = uTank.loc_x
					locations.team_1(b_index).loc_z = uTank.loc_z
					locations.team_1(b_index).rot_y = uTank.rot_y
					locations.team_1(b_index).comment = uTank.comment
					Select Case ar(1)
						Case "A"
							frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
									= a_tanks(Id).image
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = a_tanks(Id).gui_string
							locations.team_1(b_index).name = a_tanks(Id).gui_string
							frmMain.get_tank(a_tanks(Id).file_name, locations.team_1(b_index))
							locations.team_1(b_index).id = team.ToString + "_A_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = _
								team.ToString + "_A_" + Id.ToString & "_" & b_index.ToString
							locations.team_1(b_index).type = a_tanks(Id).sortorder
						Case "R"
							frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
							= r_tanks(Id).image
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = r_tanks(Id).gui_string
							locations.team_1(b_index).name = r_tanks(Id).gui_string
							frmMain.get_tank(r_tanks(Id).file_name, locations.team_1(b_index))
							locations.team_1(b_index).id = team.ToString + "_R_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_R_" + Id.ToString & "_" & b_index.ToString
							locations.team_1(b_index).type = r_tanks(Id).sortorder
						Case "G"
							frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
							= g_tanks(Id).image
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = g_tanks(Id).gui_string
							locations.team_1(b_index).name = g_tanks(Id).gui_string
							frmMain.get_tank(g_tanks(Id).file_name, locations.team_1(b_index))
							locations.team_1(b_index).id = team.ToString + "_G_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_G_" + Id.ToString & "_" & b_index.ToString
							locations.team_1(b_index).type = g_tanks(Id).sortorder
						Case "B"
							frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
							= b_tanks(Id).image
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = b_tanks(Id).gui_string
							locations.team_1(b_index).name = b_tanks(Id).gui_string
							frmMain.get_tank(b_tanks(Id).file_name, locations.team_1(b_index))
							locations.team_1(b_index).id = team.ToString + "_B_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_B_" + Id.ToString & "_" & b_index.ToString
							locations.team_1(b_index).type = b_tanks(Id).sortorder
						Case "F"
							frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
							= f_tanks(Id).image
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = f_tanks(Id).gui_string
							locations.team_1(b_index).name = f_tanks(Id).gui_string
							frmMain.get_tank(f_tanks(Id).file_name, locations.team_1(b_index))
							locations.team_1(b_index).id = team.ToString + "_F_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_F_" + Id.ToString & "_" & b_index.ToString
							locations.team_1(b_index).type = f_tanks(Id).sortorder
						Case "C"
							frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
							= c_tanks(Id).image
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = c_tanks(Id).gui_string
							locations.team_1(b_index).name = c_tanks(Id).gui_string
							frmMain.get_tank(c_tanks(Id).file_name, locations.team_1(b_index))
							locations.team_1(b_index).id = team.ToString + "_C_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_C_" + Id.ToString & "_" & b_index.ToString
							locations.team_1(b_index).type = c_tanks(Id).sortorder
						Case "J"
							frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
							= j_tanks(Id).image
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = j_tanks(Id).gui_string
							locations.team_1(b_index).name = j_tanks(Id).gui_string
							frmMain.get_tank(j_tanks(Id).file_name, locations.team_1(b_index))
							locations.team_1(b_index).id = team.ToString + "_J_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_J_" + Id.ToString & "_" & b_index.ToString
							locations.team_1(b_index).type = j_tanks(Id).sortorder
					End Select
				Else
					If locations.team_2(b_index).id = uTank.id Then
						Return ' we dont want to update tanks that are already updated
					End If
					frmTanks.SplitContainer1.Panel2.Controls(b_index).Font = _
							New Font(frmMain.pfc.Families(0), 6, System.Drawing.FontStyle.Regular)
					frmTanks.SplitContainer1.Panel2.Controls(b_index).BackColor = Color.Green
					locations.team_2(b_index).loc_x = uTank.loc_x
					locations.team_2(b_index).loc_z = uTank.loc_z
					locations.team_2(b_index).rot_y = uTank.rot_y
					locations.team_2(b_index).comment = uTank.comment
					Select Case ar(1)
						Case "A"
							frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
							= a_tanks(Id).image
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = a_tanks(Id).gui_string
							locations.team_2(b_index).name = a_tanks(Id).gui_string
							frmMain.get_tank(a_tanks(Id).file_name, locations.team_2(b_index))
							locations.team_2(b_index).id = team.ToString + "_A_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_A_" + Id.ToString & "_" & b_index.ToString
							locations.team_2(b_index).type = a_tanks(Id).sortorder
						Case "R"
							frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
							= r_tanks(Id).image
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = r_tanks(Id).gui_string
							locations.team_2(b_index).name = r_tanks(Id).gui_string
							frmMain.get_tank(r_tanks(Id).file_name, locations.team_2(b_index))
							locations.team_2(b_index).id = team.ToString + "_R_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_R_" + Id.ToString & "_" & b_index.ToString
							locations.team_2(b_index).type = r_tanks(Id).sortorder
						Case "G"
							frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
							= g_tanks(Id).image
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = g_tanks(Id).gui_string
							locations.team_2(b_index).name = g_tanks(Id).gui_string
							frmMain.get_tank(g_tanks(Id).file_name, locations.team_2(b_index))
							locations.team_2(b_index).id = team.ToString + "_G_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_G_" + Id.ToString & "_" & b_index.ToString
							locations.team_2(b_index).type = g_tanks(Id).sortorder
						Case "B"
							frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
							= b_tanks(Id).image
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = b_tanks(Id).gui_string
							locations.team_2(b_index).name = b_tanks(Id).gui_string
							frmMain.get_tank(b_tanks(Id).file_name, locations.team_2(b_index))
							locations.team_2(b_index).id = team.ToString + "_B_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_B_" + Id.ToString & "_" & b_index.ToString
							locations.team_2(b_index).type = b_tanks(Id).sortorder
						Case "F"
							frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
							= f_tanks(Id).image
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = f_tanks(Id).gui_string
							locations.team_2(b_index).name = f_tanks(Id).gui_string
							frmMain.get_tank(f_tanks(Id).file_name, locations.team_2(b_index))
							locations.team_2(b_index).id = team.ToString + "_F_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_F_" + Id.ToString & "_" & b_index.ToString
							locations.team_2(b_index).type = f_tanks(Id).sortorder
						Case "C"
							frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
							= c_tanks(Id).image
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = c_tanks(Id).gui_string
							locations.team_2(b_index).name = c_tanks(Id).gui_string
							frmMain.get_tank(c_tanks(Id).file_name, locations.team_2(b_index))
							locations.team_2(b_index).id = team.ToString + "_C_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_C_" + Id.ToString & "_" & b_index.ToString
							locations.team_2(b_index).type = c_tanks(Id).sortorder
						Case "J"
							frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
							= j_tanks(Id).image
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = j_tanks(Id).gui_string
							locations.team_2(b_index).name = j_tanks(Id).gui_string
							frmMain.get_tank(j_tanks(Id).file_name, locations.team_2(b_index))
							locations.team_2(b_index).id = team.ToString + "_J_" + Id.ToString & "_" & b_index.ToString
							frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_J_" + Id.ToString & "_" & b_index.ToString
							locations.team_2(b_index).type = j_tanks(Id).sortorder
					End Select

				End If


			Catch ex As Exception
			End Try
		End If
	End Sub



	Private Delegate Sub w_diag_Delgegate(ByVal s As String)
	Private Sub write_diagnostics(ByVal s As String)
		'writes the diagnostics to the screen.
		'Uses FiFo of 50 lines to stop from building up too much
		'data in the textbox control.
		If Me.InvokeRequired Then
			Try
				Me.Invoke(New w_diag_Delgegate(AddressOf write_diagnostics), New Object() {s})
			Catch ex As Exception
			End Try
		Else
			Try
				If diag_msg_buff.Count > 50 Then
					diag_msg_buff.RemoveAt(0)
				End If
				diag_msg_buff.Add(s)
				Dim m As String = ""
				For Each n In diag_msg_buff
					m += n
				Next
				diag_tb.Clear()
				diag_tb.AppendText(m)
			Catch ex As Exception
			End Try
		End If
	End Sub

	Private Sub send_chat_msg()
		Try
			Dim outMsg As NetOutgoingMessage = client.CreateMessage
			outMsg.Write(CByte(packetType.chatMsg))
			outMsg.Write(client_name_tb.Text)
			chat_entry_tb.Text = ""
			outMsg.Write(chat_string)
			SyncLock msg_lock
				client.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered)
			End SyncLock

		Catch ex As Exception

		End Try

	End Sub


	Private Delegate Sub resetdelegate()
	Private Sub net_reset()
		If Me.InvokeRequired Then
			Me.Invoke(New resetdelegate(AddressOf net_reset))
		Else
			Try
				frmMain.make_locations()
				frmTanks.make_btns()
				SyncLock packet_lock
					Packet_out.tankId = -1
					Packet_in.tankId = -1
					tankID = -1
					client_reset = False
				End SyncLock

			Catch ex As Exception
			End Try
		End If
	End Sub
	Private Delegate Sub loadmapdelegate(ByVal s As String)
	Private Sub net_loadmap(ByVal s As String)
		If Me.InvokeRequired Then
			Me.Invoke(New loadmapdelegate(AddressOf net_loadmap), New Object() {s})
		Else
			Try
				frmMain.flush_data()
				load_map_name = s
				open_pkg(load_map_name)
			Catch ex As Exception
			End Try
		End If
	End Sub

	'========================================================
	Private Delegate Sub Appendping(ByVal s As String)
	Private Sub updatePing(ByVal s As String)
		If Me.InvokeRequired Then
			Me.Invoke(New Appendping(AddressOf updatePing), New Object() {s})
		Else
			Try
				Ping_text.Text = s
			Catch ex As Exception
			End Try
		End If
	End Sub

	'========================================================
	Private Delegate Sub AppendTextBoxDelegate(ByVal s As String)
	Private Sub AppendTextBox(ByVal s As String)
		If Me.InvokeRequired Then
			Me.Invoke(New AppendTextBoxDelegate(AddressOf AppendTextBox), New Object() {s})
		Else
			Try
				echo_window_tb.Clear()
				If s.Length > 0 Then
					chat_message_buffer.Add(s)
				End If
				If chat_message_buffer.Count > 50 Then
					chat_message_buffer.RemoveAt(0)
				End If
				Dim m As String = ""
				For Each n In chat_message_buffer
					m += n
				Next
				echo_window_tb.AppendText(m)
			Catch ex As Exception
			End Try
		End If
	End Sub
	Private Delegate Sub clearTextBoxDelegate(ByVal TB As TextBox, ByVal txt As String)
	Private Sub clearTextBox(ByVal TB As TextBox, ByVal txt As String)
		If TB.InvokeRequired Then
			TB.Invoke(New clearTextBoxDelegate(AddressOf clearTextBox), New Object() {TB, ""})
		Else
			Try
				echo_window_tb.Text = txt
			Catch ex As Exception
			End Try

		End If
	End Sub
	'========================================================
	Private Sub chat_entry_tb_KeyDown(sender As Object, e As KeyEventArgs) Handles chat_entry_tb.KeyDown

		If e.KeyCode = Keys.Enter And e.Shift Then
			e.Handled = True
		Else
			If e.KeyCode = Keys.Enter Then
				If chat_entry_tb.Text = "" Then Return
				chat_string = chat_entry_tb.Text
				chat_entry_tb.Text = ""
				send_chat_msg()
				e.SuppressKeyPress = True
				e.Handled = True
			End If

		End If

	End Sub
	Private Sub chat_entry_tb_TextChanged(sender As Object, e As EventArgs) Handles chat_entry_tb.TextChanged
		'Dim t = chat_entry_tb.Text
		'sender.text = t.Replace(vbCrLf, "")
		'If t.Length > 100 Then Return
	End Sub
	'========================================================
	Public Class my_label
		Inherits Label
		Public cli_type As Byte
		Protected Overrides Sub OnMouseEnter(e As EventArgs)
			MyBase.OnMouseEnter(e)
			Me.ForeColor = Color.White
		End Sub
		Protected Overrides Sub OnPaint(e As PaintEventArgs)
			MyBase.OnPaint(e)
		End Sub
		Protected Overrides Sub OnCreateControl()
			MyBase.OnCreateControl()
			Me.ForeColor = Color.Black
			Me.BackColor = Color.DimGray
			Select Case Me.cli_type
				Case clientType.host
					Me.ForeColor = Color.DarkRed
				Case clientType.driver, clientType.driverAndhost
					Me.ForeColor = Color.Yellow
			End Select
		End Sub
		Protected Overrides Sub OnMouseHover(e As EventArgs)
			MyBase.OnMouseHover(e)
			Me.ForeColor = Color.White
		End Sub
		Protected Overrides Sub OnMouseLeave(e As EventArgs)
			MyBase.OnMouseLeave(e)
			Me.ForeColor = Color.Black
			Me.BackColor = Color.DimGray
			Select Case Me.cli_type
				Case clientType.host
					Me.ForeColor = Color.DarkRed
				Case clientType.driver, clientType.driverAndhost
					Me.ForeColor = Color.Yellow
			End Select
		End Sub
		Protected Overrides Sub OnMouseClick(e As MouseEventArgs)
			If e.Button = Windows.Forms.MouseButtons.Right Then
				If frmMain.m_comment.Visible Then
					frmMain.m_comment.Text = Me.Text
				End If
			Else
				frmClient.handle_client_selection(Me.Text)
			End If
			MyBase.OnMouseClick(e)
		End Sub
	End Class
	'=====================================================
	Private Delegate Sub pick_client(ByVal name As String)
	Public Sub handle_client_selection(ByVal name As String)
		If Me.InvokeRequired Then
			Me.BeginInvoke(New pick_client(AddressOf handle_client_selection), New Object() {name})
			Return
		End If
		If Not server_dead Then
			If ImDriver Or ImHost Then
				Dim outmsg As NetOutgoingMessage = client.CreateMessage
				outmsg.Write(CByte(packetType.driverChange))
				outmsg.Write(name)
				client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered)
				'frmMain.chat_entry_tb.Text = name
			End If
		End If
		chat_entry_tb.Focus()

	End Sub
	'=====================================================
	Private Delegate Sub update_clientPanelDelgate()
	Private Sub update_client_panel()
		'Updates all the client lables 
		If Me.InvokeRequired Then
			Me.BeginInvoke(New update_clientPanelDelgate(AddressOf update_client_panel))
			Return
		End If
		Dim w = client_panel.ClientSize.Width
		client_panel.HorizontalScroll.Visible = False
		Try
			client_panel.Controls.Clear()

		Catch ex As Exception

		End Try
		Dim vw = System.Windows.Forms.SystemInformation.VerticalScrollBarWidth
		Dim i As Integer = 0
		For Each u In clients
			Dim l As New my_label
			l.cli_type = u.client_type
			l.Text = u.Name
			l.Font = client_name_tb.Font.Clone
			l.AutoSize = False
			l.Width = w - vw
			l.Height = 16
			client_panel.Controls.Add(l)
			l.Location = New Point(0, i * 16)
			i += 1
		Next
		Application.DoEvents()
	End Sub

	Private Sub stop_button_Click(sender As Object, e As EventArgs) Handles stop_button.Click
		shut_down_gracefully()
	End Sub


	Private Sub diagnostics_bt_Click(sender As Object, e As EventArgs) Handles diagnostics_bt.Click
		If diag_tb.Visible Then
			diag_tb.Visible = False
		Else
			diag_tb.Visible = True
		End If
		chat_entry_tb.Focus()

	End Sub

	Private Sub client_name_tb_TextChanged(sender As Object, e As EventArgs) Handles client_name_tb.TextChanged
		Me.Text = "Terra! Chat: " + client_name_tb.Text
	End Sub

	Private Sub frmClient_MouseEnter(sender As Object, e As EventArgs) Handles Me.MouseEnter

	End Sub

	Private Sub echo_window_tb_MouseEnter(sender As Object, e As EventArgs) Handles echo_window_tb.MouseEnter
		chat_entry_tb.Focus()

	End Sub

	Private Sub frmClient_Resize(sender As Object, e As EventArgs) Handles Me.Resize
		If Panel1.Dock <> DockStyle.Fill Then
			Panel1.Height = Me.ClientSize.Height - 56
		End If
		AppendTextBox("")

	End Sub

	Private Sub frmClient_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
		If Panel1.Dock <> DockStyle.Fill Then
			Panel1.Height = Me.ClientSize.Height - 56
		End If
		AppendTextBox("")
	End Sub
	Public Sub resetClients()
		If ImHost Then
			client_reset = True
		End If
	End Sub
End Class
