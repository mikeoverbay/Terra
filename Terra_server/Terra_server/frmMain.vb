Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.IO
Imports System.Collections.Generic

Public Class frmMain
  'These are the byte control codes
  'to tell the server what the data is.
  'some will be simply passed on to the clients.
  ' 
  '--------------------------------------------------------------------------------
  'User type code byte
  ' 1 = guest
  ' 2 = driver
  ' 3 = driver and host
  ' 4 = host
  '--------------------------------------------------------------------------------
  ' packets will be fixed at 150 bytes
  ' 100 packets will be spooled on the server for each user incase a packet doesnt make it
  ' a request for the packet can be sent to the server
  ' [control][data]
  '
  ' magic number 4 bytes ( will eval to 26001 dec) used to allow connections to the server.
  ' (may want to add a password later if it becomes a security issue)
  ' crtl = sbyte
  ' len = Sbyte
  ' user type = Sbyte
  ' tankId = Sbyte
  '
  ' client name Max of 16 characters (................)
  ' chat text Max of 100 characters (way more than needed but we have packet size so why not??)
  '
  ' 1 client connect request      [packid][clientPckId][ctrl][magic][len][clientname][user type][packetId]
  ' 2 client leaving               [packid][clientPckId][ctrl][len][clientname][user type][packetId]
  ' 3 client text msg              [packid][clientPckId][ctrl][len][clientname][len][text][user][packetId]
  '                                (max name length is 16 bytes. max chat text len is 100)
  ' 4 map change                   [packid][clientPckId][ctrl][len][text][packetId]
  ' 5 driver change                [packid][clientPckId][ctrl][len][clientname][packetId]
  ' reserverd
  ' 8                               [uint32][clientPckId][crtl][len][username]
  ' reserverd
  ' reserverd
  ' 10 host data... as:
  '   [crtl][eye-x][eye-y][rot-x][rot-y][radius]
  '
  '   [selected tank] -1 means no tank selected (green 0-14)(red 100-114)
  ' if selected tank = -1 than next byte will be the multiplexed tankId.
  '   [len][id_string] ( team _ nation _ tank id _ button index )
  '   [x_pos][y_pos][rotation]
  ' To keep bandwidth usage down, rather than sending all the tank setup at once,
  '   the next tankId will be a multiplex of 'one to defined tank count', less the selected tank'
  '   If there is no selected tank, it will be -1
  '
  '   [selected tank] -1 means no tank data coming for this and this packet is done.
  '   [len][id_string] ( team _ nation _ tank id _ button index )
  '   [x_pos][y_pos][rotation]
  '   and finally, [packetId]

  '--------------------------------------------------------------------------------

  Dim cnt As Long = 0
  Dim state As state_
  Dim magicPacket() As Byte = {1, 9, 5, 6} '= 26001 decimal
  Dim myip As String
  Dim in_port As Integer = 1956
  Dim out_port As Integer = 1957
  Dim l_udp As New UdpClient(1956)
  Dim s_udp As New UdpClient(1957)


  Dim send_ep As IPEndPoint
  'Dim sendSocket As Socket = New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
  Dim mydelegate As New Object
  Public Delegate Sub ShowMessage(ByVal message As String)
  Public rec_trd As New Thread(AddressOf accept_user)
  Dim send_trd As New Thread(AddressOf send_thread)
  Public running As Integer = 0
  Public msg_array(150) As Byte
  Public waiting_ack_list As New List(Of Byte())

  Dim delta As Integer
  Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
    form_closing = True
    If rec_trd.IsAlive Then
      rec_trd.Abort()
    End If
    If send_trd.IsAlive Then
      send_trd.Abort()
    End If
  End Sub

  Private Sub frmmain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    ' set up 31 client threads
    For i = 0 To 30
      Clients(i) = New Client_
      Clients(i).start_up(i)
    Next
    'use for postig nack messages.
    mydelegate = New ShowMessage(AddressOf showMessageMethod)
    GetIPAddress()
  End Sub


  Function GetIPAddress() As String
    GetIPAddress = ""
    Dim str As String = ""
    Dim CompName As String = System.Net.Dns.GetHostName

    Try
      Dim hostInfo As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(CompName)
      GetIPAddress = hostInfo.AddressList(0).ToString()
      Dim z As Integer = 0
      For Each I In hostInfo.AddressList
        Dim ipstr As String = hostInfo.AddressList(z).ToString()
        ComboBox1.Items.Add(ipstr.ToString)
        z += 1
      Next
    Catch Excep As Exception
      MsgBox(Excep.Message, MsgBoxStyle.OkOnly, "Lan not working.")

    Finally
    End Try
  End Function


  Private Sub start_btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles start_btn.Click

    'If ComboBox1.Text.Length = 0 Then
    '  MsgBox("Need a ip selected first", MsgBoxStyle.Exclamation)
    '  Return
    'End If
    start_btn.Enabled = False
    'use this just to start the udpclient

    Dim send_ep As New IPEndPoint(IPAddress.Parse(TextBox2.Text), out_port)
    Dim ip = IPAddress .Parse (TextBox2.Text)
    l_udp.Client.SendBufferSize = 150
    l_udp.Client.ReceiveBufferSize = 150
    s_udp.Client.SendBufferSize = 150
    s_udp.Client.ReceiveBufferSize = 150

    s_udp.Connect(ip, out_port)

    myip = TextBox2.Text
    ComboBox1.Enabled = False
    '---------------------
    rec_trd.IsBackground = True
    rec_trd.Name = "rec_thread"
    rec_trd.Priority = ThreadPriority.Normal
    rec_trd.Start()
    '----------------------
    send_trd.IsBackground = True
    send_trd.Name = "send_thread"
    send_trd.Priority = ThreadPriority.Normal
    send_trd.Start()
  End Sub
  Public Sub accept_user()
    state = New state_
    Dim remoteEndPoint = New IPEndPoint(IPAddress.Any, in_port)
    l_udp.DontFragment = False
    state.SendingSocket = l_udp
    state.ReceivingEndpoint = remoteEndPoint
    While True
      state.wait_hdl_R.reset()
      'Thread.Sleep(1)
      Dim status = l_udp.BeginReceive(New AsyncCallback(AddressOf async_read_callback), state).AsyncWaitHandle
      If state.wait_hdl_R.WaitOne(200, True) Then
        'Debug.WriteLine("Work method signaled." + " " + cnt.ToString + vbCrLf)
      Else
        'Debug.Write("socket is fucking screwed up" + vbCrLf)
      End If
      Thread.Sleep(5)
    End While
  End Sub
  Private Sub showMessageMethod(ByVal s As String)
    user_list_tb.Text = s
  End Sub

  '---------------------------------
  Public Sub async_read_callback(ByVal ar As IAsyncResult)
    Dim a = ar
    state = CType(ar.AsyncState, state_)
    Dim data(150) As Byte
    If ar.IsCompleted Then

      Dim IPEndPoint As EndPoint = TryCast(state.ReceivingEndpoint, IPEndPoint)
      'Dim waitsize As Integer = state.SendingSocket.Available
      'Debug.Write("wait size: " + waitsize.ToString + vbCrLf)
      'If waitsize > 0 Then
      If form_closing Then
        Return
      End If
      data = state.SendingSocket.EndReceive(a, state.ReceivingEndpoint)
      If data.Length > 0 Then
        state.wait_hdl_R.set() ' resetting so more data can be read as fast as we can

        Dim ms As New MemoryStream(data)
        Dim br As New BinaryReader(ms)
        Dim bw As New BinaryWriter(ms)
        Dim code As Byte
        '-----------------------------------------------------
        Dim Spck_id = br.ReadUInt32
        Dim Cpck_id = br.ReadUInt32
        code = br.ReadByte()
        '-----------------------------------------------------
        ' codes that must be acknowledged
        ' 1 client connect request      [packid][ctrl][magic][len][clientname][user type][packetId]
        ' 2 client leaving              [packid][ctrl][len][clientname][user type][packetId]
        ' 3 client text msg             [packid][ctrl][len][clientname][len][text][user][packetId]
        '                               (max name length is 16 bytes. max chat text len is 100)
        ' 8 packet ack                  [packId][ctrl][len][clientname]
        ' 4 map change                  [packid][ctrl][len][text][packetId]
        ' 5 driver change               [packid][ctrl][len][clientname][packetId]
        ' 10 state info                 see description at top of form
        '----------------------------------------------------------------------------------------
        Select Case code
          '--------------------------------------------------------------------------------------
          Case 1 'user connect quest
            'str len and the string
            Dim sl = br.ReadByte ' string length
            Dim username As String = Encoding.ASCII.GetString(data, ms.Position, sl)
            Me.Invoke(mydelegate, New Object() {username + "connected" + vbCrLf})
            Dim slot As Boolean = False
            'first, see if this client exists
            For i = 0 To 30
              Clients(i).client_name = username
              'already has a thread but wating for ack from server
              Exit Select
            Next
            For i = 0 To 30
              If Clients(i).client_name = "" Then
                Clients(i).setup(username)
                ' need to let all clients know some one connected
                ' we will add this to the send msg queue
                ' we will replace pack_id with the servers so ack's are not duplicated
                ms.Position = 0 ' need to write at begining
                bw.Write(server_packId)
                'bw.Write(1) 'client connect code
                For j = 0 To 30
                  If Clients(j).client_name <> "" Then
                    Clients(j).data_in(data, False)
                    If Clients(j).client_name = username Then
                      Clients(j).data_in(data, True) 'originator
                    Else
                      Clients(j).data_in(data, False)

                    End If
                    'thread will update its buffer.
                    'this will go in the connecting clients buffer as well
                    'anything in the ack buffes will be resent until they are ack'ed
                  End If
                Next
                'This client just sent connect request. We need to send back a response.
                'The server sends the join requests to all clients... including the new one.
                'Each client thread will resend this until it gets a reponse from the user to
                'ack that they are aware of a new client.
                '
                'The message is in that clients ack buffer and will stay there until the
                'client sees its name come back and acks it by sending the server_packId,
                'code and user name back to the server.
                '
                slot = True
                Exit For
              End If
            Next
            If Not slot Then
              'no more room for users.. ack?
              'this means we sent no messages and dont need to inc the server_packId
            Else
              'we added a user and sent to everyone. server_packId has to increment
              inc_server_packId()

            End If
            '--------------------------------------------------------------------------------------
          Case 2 ' chat message
            Dim sl = br.ReadByte 'string length
            Dim username As String = Encoding.ASCII.GetString(data, ms.Position, sl)
            Dim cl = br.ReadByte
            Dim chatmsg As String = Encoding.ASCII.GetString(data, ms.Position, cl)
            Me.Invoke(mydelegate, New Object() {username + ": " + chatmsg + vbCrLf})

            '--------------------------------------------------------------------------------------
          Case 8 ' user ack'ed a packet
            Dim sl = br.ReadByte 'string length
            Dim username As String = Encoding.ASCII.GetString(data, ms.Position, sl)
            For i = 0 To 30
              If Clients(i).client_name = username Then
                Clients(i).ack_in(pck_id, True) ' this will remove the packet from the in_buff
              End If
            Next
            '--------------------------------------------------------------------------------------
          Case 10 ' this is 'state' info and can be sent with out waiting for a response
            ms.Position = 0
            bw.Write(server_packId)
            SyncLock send_lock
              'need to insert the pack_id
              data.CopyTo(client_state, 0)
              'this is a send from the server so the server_packId must increment
            End SyncLock
            inc_server_packId()
        End Select

      Else
        'state.SendingSocket.EndReceive(a, state.ReceivingEndpoint)

      End If
      'Else

      'Debug.Write("IsComplete but nothing Available" + vbCrLf)
      'state.wait_hdl_R.set()
      'End If
    Else
      Debug.Write("begin but not complete" + vbCrLf)
      Dim k = 1
    End If

  End Sub
  '---------------------------------

  Private Sub inc_server_packId()
    SyncLock inc_lock
      server_packId += 1
      'the server_packId can NEVER be equal to zero.
      If server_packId = 0 Then
        server_packId = 1
      End If
    End SyncLock

  End Sub
  Private Structure UdpState
    Public SendingSocket As UdpClient
    Public ReceivingEndpoint As EndPoint

  End Structure
  Public pck_id As UInteger = 1
  Public Sub send_thread()
    Dim b(150) As Byte
    Dim ms As New MemoryStream(b)
    Dim bw As New BinaryWriter(ms)
    Dim br As New BinaryReader(ms)
    Dim by As Byte = 1
    Dim blank As UInteger = 0
    Try
    Catch e As Exception
      Console.WriteLine(e.ToString())
    End Try
    While 1
      If form_closing Then
        Return
      End If
      For i = 0 To 30
        If Clients(i).client_name.Length > 0 Then
          For j = 0 To 150
            If Clients(i).inbuff(j).SpId > 0 Then
              SyncLock Clients(i).lock_in
                'resend any un ack'ed messages
                s_udp.Send(Clients(i).inbuff(j).d, b.Length - 1)
                Clients(i).inbuff(j).retry += 1
              End SyncLock
            End If
          Next
        End If
      Next
      '---------------------------------------------
      'check for any liguring data waiting on ack's
      For i = 0 To 30
        If Clients(i).inbuff(i).retry > 100 Then
          'If this has been sent n- number of times and no
          'ack, the client is probably lost
          'We need to reset this client and tell everyone about it.
          '1st, create new data to send
          Dim name = Clients(i).client_name
          Clients(i).reset()  ' this resets the client
          Dim os(150) As Byte
          ms = New MemoryStream(os)
          bw = New BinaryWriter(ms)
          ' server is sending so we need to inc the packId.
          SyncLock inc_lock
            bw.Write(server_packId)
            inc_server_packId()
          End SyncLock
          Dim disco As Byte = 2
          bw.Write(disco)
          Dim ln As Byte = name.Length
          bw.Write(ln)
          bw.Write(Encoding.UTF8.GetBytes(name))
          For z = 0 To 30
            If Clients(z).client_name <> "" Then
              SyncLock Clients(i).lock_in
                Clients(z).data_in(os, False)
              End SyncLock
            End If
          Next
        End If
      Next


      ' this checks for any state data that needs to be echoed
      Dim tempbuff(150) As Byte
      SyncLock send_lock
        client_state.CopyTo(tempbuff, 0)
      End SyncLock
      tempbuff(0) = 128
      ms = New MemoryStream(tempbuff)
      br = New BinaryReader(ms)
      Dim avail = br.ReadInt32
      If avail > 0 Then
        s_udp.Send(tempbuff, tempbuff.Length - 1)
        'it was sent so lets clear the packid so it wont get sent again.
        'we dont care if this packet gets lost (? need to test this more)
        ms.Position = 0 'rewind to start
        bw = New BinaryWriter(ms)
        bw.Write(blank)
        SyncLock send_lock
          tempbuff.CopyTo(client_state, 0) ' update the data
        End SyncLock
      End If
      Thread.Sleep(10)
      ms.Position = 0
    End While
  End Sub
  Private Sub stop_btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles stop_btn.Click
    Me.Close()
  End Sub
End Class
