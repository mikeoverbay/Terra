Imports System.Threading
Imports System.IO
Imports System.String
Imports System.Net.Sockets
Imports System.Text
Imports System.Net
Imports Microsoft.VisualBasic.ControlChars
Imports System.Windows.Forms
Imports System.Data
Module client_main
    Public new_driver_string As String = ""
    Public Sub connect_to_host()
        frmMain.Net_Timer.Enabled = False
        frmMain.Net_Timer.Interval = 36
        clear_chat_window()
        Dim id As Integer = 0
        Dim app_local As String = Application.StartupPath.ToString
        msgCC = app_local + "/notify.wav"
        ringCC = app_local + "/ringin.wav"
        Chat_text = ""
        SERVER_DOWN = False
        clear_chat_window()
        frmGetIP.Show(frmMain)
        frmGetIP.Location = New System.Drawing.Point((frmMain.Width / 2) - (frmGetIP.Width / 2) + frmMain.Location.X _
                                   , (frmMain.Height / 2) - (frmGetIP.Height / 2) + frmMain.Location.Y)

    frmMain.m_disconnect.Enabled = False
    frmMain.m_host_session.Enabled = False
    frmMain.m_join_session.Enabled = False

        While frmGetIP.Visible
            Application.DoEvents()
        End While
        If frmGetIP.OK = True Then
            users_name = frmGetIP.user_name_tb.Text
            tcpClient = New System.Net.Sockets.TcpClient()

            Dim myHost As String = System.Net.Dns.GetHostName
            Dim myIPs As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(myHost)
            Dim myip As System.Net.IPAddress = myIPs.AddressList(0)
            my_IP = frmGetIP.my_ip_tb.Text
            server = System.Net.IPAddress.Parse(frmGetIP.ip_tb.Text)
            Dim _state As New Object
            _state = myip.ToString
            Dim endp As IPEndPoint = New IPEndPoint(IPAddress.Parse(frmGetIP.ip_tb.Text), 1956)
            'tcpClient.Connect(endp)
            Try
                tcpClient.BeginConnect(server, 1956, New AsyncCallback(AddressOf conx_callback), _state)

            Catch ex As Exception

            End Try
            thrd = New thd
      frmMain.m_disconnect.Enabled = True
            frmChat.chat_input_tb.Enabled = True
            frmChat.chat_input_tb.Focus()
      frmMain.Net_Timer.Enabled = True
            frmChat.clients_dgv.Enabled = True


        End If
    End Sub
    Public Sub conx_callback(ByVal ar As IAsyncResult)

        Dim myip As String = CStr(ar.AsyncState)
        ' Dim networkStream As NetworkStream = tcpClient.GetStream()

        thrd.Id = This_Id
        thrd.run()
        If imHost Then
            thrd.send_string = "5`" & users_name & "`" & my_IP & "~3` " & "~2`host"
        Else
            thrd.send_string = "5`" & users_name & "`" & my_IP & "~3` "

        End If
    End Sub


    Public Sub _send_text()
        WAIT_SEND = True
    If InStr(frmChat.chat_input_tb.Text, "<clr>") > 0 Then
      frmMain.Net_Timer.Enabled = False ' timer has to stop to avoid bogus errors !
      clear_chat_window()
      frmChat.chat_input_tb.Clear()
      WAIT_SEND = False
      frmMain.Net_Timer.Enabled = True
      Return
    End If
    If InStr(frmChat.chat_input_tb.Text, "<exit>") > 0 Then
      Dim e As New System.EventArgs
      Dim sender As New Object
      disconect_client()
      frmChat.chat_input_tb.Clear()
      WAIT_SEND = False
      Return
    End If

    SEND_STRING = " ~" + "1" + "`" & frmGetIP.user_name_tb.Text & "~" _
    & "2" & "`" & load_map_name & "~" & "3" & "`" & "  " & frmChat.chat_input_tb.Text & new_driver_string

        'RET_STRING = SEND_STRING
    frmChat.chat_input_tb.Text = ""
        SyncLock locktite
            thrd.send_string = SEND_STRING
        End SyncLock
        'SEND_STRING = Microsoft.VisualBasic.Replace(SEND_STRING, vbCrLf, "\par")
    End Sub

    Public Sub clear_chat_window()
    frmChat.chat_input_tb.Clear()
    frmChat.chat_box_tb.Clear()
    End Sub


    Public Sub display_update()
        'item codes:
        ' 0 - Id
        ' 1 - name
        ' 2 - current map
        ' 3 - text content
        ' 4 - triger change host event
        ' 5 - join
        ' 6 - left
        Dim ring As Boolean = False
        Dim work_str As String = ""
        Dim tm_str As String = RET_STRING
        RET_STRING = ""
        'Dim os_chop As Array = tm_str.Split(vbCrLf)
        'For inside = 0 To os_chop.Length - 1
        Dim chop As Array = tm_str.Split("~")
        Dim no_scroll As Boolean = False
        For lsegment = 0 To chop.Length - 1

            Dim chop_sec As Array = chop(lsegment).split("`")
            Try
                Select Case CInt(chop_sec(0))
                    Case 0
                        thrd.Id = CInt(chop_sec(1))
                        Dim tmp_id = chop_sec(1)
                        Dim temp_IP As String = chop_sec(2)
                        If InStr(my_IP, temp_IP) > 0 Then
                            '   Id_textbox.Text = chop_sec(1)
                        End If
                    Case 1 ' name
                        work_str = pre_text
                        ' work_str = Microsoft.VisualBasic.Replace(work_str, "&&", CStr(_COLOR + 1))
                        'work_str = Microsoft.VisualBasic.Replace(work_str, "^^", CStr(_FONT_SIZE))
                        work_str += chop_sec(1) + "- "
                        Chat_text += work_str
                        no_scroll = False
                    Case 2
                        If load_map_name.Length > 1 Then
                            Dim mn As String = chop_sec(1)
                            load_map_name = " "
                            open_pkg(mn)
                        End If
                    Case 3 ' text
                        work_str = ""
                        work_str = pre_text
                        ' work_str = Microsoft.VisualBasic.Replace(work_str, "&&", CStr(_COLOR + 1))
                        'work_str = Microsoft.VisualBasic.Replace(work_str, "^^", CStr(_FONT_SIZE))
                        work_str += chop_sec(1) + vbCrLf
                        Chat_text += work_str
                        no_scroll = False

                    Case 5 ' joined chat
                        work_str = pre_text
                        '  work_str = Microsoft.VisualBasic.Replace(work_str, "&&", CStr(_COLOR + 1))
                        ' work_str = Microsoft.VisualBasic.Replace(work_str, "^^", CStr(_FONT_SIZE))
                        work_str += chop_sec(1) + " entered."
                        Chat_text += work_str
                        no_scroll = False
                    Case 6 ' left chat
                        work_str = pre_text
                        'work_str = Microsoft.VisualBasic.Replace(work_str, "&&", CStr(_COLOR + 1))
                        'work_str = Microsoft.VisualBasic.Replace(work_str, "^^", CStr(_FONT_SIZE))
                        work_str += chop_sec(1) + " left."
                        Chat_text += work_str
                        no_scroll = False
                    Case 12 ' triger no scroll event
                        no_scroll = True
                    Case 14 'online user list
                        Dim cltbl As New DataTable
                        cltbl.Columns.Add("name", GetType(String))
                        cltbl.Columns.Add("Id", GetType(Integer))

                        cltbl.Columns.Add("Icon", GetType(Image)).SetOrdinal(0)  'Set our new column to the first order of the table

                        Dim clrow As DataRow = cltbl.NewRow

                        cltbl.Clear()
                        For i = 0 To 30
                            Dim client_data As Array = chop_sec(i + 1).split("|")
                            If client_data(0).ToString.Length < 2 Then GoTo next_name
                            client_name(i) = client_data(0)
                            Select Case client_data(1)
                                Case 1
                                    clrow(0) = My.Resources.user_silhouette
                                    If users_name = client_data(0) Then
                                        imHost = False
                                        imtemphost = False 'used to signal who is driver
                                    End If
                                Case 2
                                    If users_name = client_data(0) Then
                                        imHost = True
                                    End If
                                    clrow(0) = My.Resources.user_business
                                Case 3
                                    If users_name = client_data(0) Then
                                        imtemphost = True 'used to signal who is driver
                                        imHost = True
                                    End If
                                    clrow(0) = My.Resources.user_business_yellow
                                Case 4
                                    If users_name = client_data(0) Then
                                        imtemphost = True 'used to signal who is driver
                                    End If
                                    clrow(0) = My.Resources.user_business_yellow

                            End Select
                            clrow(1) = client_data(0)
                            clrow(2) = client_data(1)
                            cltbl.Rows.Add(clrow)
next_name:
                        Next
            frmChat.clients_dgv.SuspendLayout()
                        frmMain.ClientsBindingSource.DataSource = cltbl
            frmChat.clients_dgv.DataSource = frmMain.ClientsBindingSource
            frmChat.clients_dgv.Columns(0).Width = 30
            frmChat.clients_dgv.Columns(2).Visible = False
            frmChat.clients_dgv.ColumnHeadersVisible = False
            frmChat.clients_dgv.ClearSelection()
            frmChat.clients_dgv.ResumeLayout()
                        no_scroll = True
                        cltbl.Dispose()

                    Case 16 ' buzz sent
                        ring = True
                        no_scroll = True
                End Select
            Catch ex As Exception
            End Try
        Next

        If Not Chat_text.Length = 0 Then
      Dim bs As String = frmChat.chat_box_tb.Text
            'Dim bs As String = LSet(ts, ts.Length - 5)
            bs += Chat_text
      frmChat.chat_box_tb.Text = bs
            scrolling_flag = True
      frmChat.chat_box_tb.SuspendLayout()
            Dim cf As Object = frmMain.ActiveControl
      frmChat.chat_box_tb.Focus()
      frmChat.chat_box_tb.SelectionStart = frmChat.chat_box_tb.Text.Length
      frmChat.chat_box_tb.ScrollToCaret()
            cf.focus()
      frmChat.chat_box_tb.ResumeLayout()
        End If
        scrolling_flag = False
        Chat_text = ""

        If _refresh Then
            _refresh = False
            'Button5.Enabled = True
            'entry_TextBox1.Text = ""
        End If
        'If audio.Checked Then
        If ring Then
            play_ringer()
        Else
            If InStr(tm_str, "12`ping") = 0 Then
                play_msg()
            End If
        End If
        'End If
        RET_STRING = ""
    End Sub



    Public Sub disconect_client()
    frmChat.chat_input_tb.Text = "Until next time...."
        _send_text()
        SENDING = True
        While SENDING

        End While

        frmMain.ClientsBindingSource.DataSource = Nothing
    frmChat.clients_dgv.DataSource = frmMain.ClientsBindingSource
    frmChat.clients_dgv.ClearSelection()
    frmChat.chat_box_tb.Text = ""

    frmMain.m_disconnect.Enabled = False
    frmMain.m_join_session.Enabled = True
    frmMain.m_host_session.Enabled = True
    frmMain.m_session.ForeColor = Color.Black
    frmChat.chat_input_tb.Enabled = False
        thrd.abortme = True
        thrd.state.network.Close()
        tcpClient.Close()
        frmMain.Net_Timer.Enabled = False

    End Sub


    Private Sub play_msg()
        'My.Computer.Audio.Play(msgCC)
    End Sub
    Private Sub play_ringer()
        My.Computer.Audio.Play(ringCC)
    End Sub


#Region "unused"


    'Private Sub clients_dgv_DataBindingComplete(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewBindingCompleteEventArgs) Handles clients_dgv.DataBindingComplete
    '    If clients_dgv.RowCount > 0 Then
    '        If dgv_position >= 0 Then
    '            clients_dgv.FirstDisplayedScrollingRowIndex = dgv_position
    '        End If
    '    End If
    '    clients_dgv.ClearSelection()

    'End Sub


    'Private Sub clients_dgv_Scroll(ByVal sender As Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles clients_dgv.Scroll
    '    dgv_position = clients_dgv.FirstDisplayedScrollingRowIndex

    'End Sub

    'Private Sub pvt_chat_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pvt_chat.Click

    '    _SEND = False
    '    If Form2.Visible Then
    '        Return
    '    End If

    '    Form2.Label1.Text = "Send a Private Message to [ " & clients_dgv(0, PM_VAL).Value & " ]"
    '    Form2.Show(Me)
    '    Form2.Location = New point(Me.Location.X + clients_dgv.Location.X - 390, _
    '                               Me.Location.Y + clients_dgv.Location.Y + (PM_VAL * 20) + 11 + 40)

    'End Sub

    'Private Sub page_client_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles page_client.Click
    '    thrd.send_string = "<RING>|" & CStr(PM_VAL) & "|" & CStr(thrd.Id)
    'End Sub

    'Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
    '    If scroll_lock Then
    '        scroll_lock = False
    '        Button4.Image = My.Resources.lock_unlock
    '    Else
    '        scroll_lock = True
    '        Button4.Image = My.Resources.lock
    '    End If
    'End Sub
    'Protected Sub Link_Clicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkClickedEventArgs)
    '    System.Diagnostics.Process.Start(e.LinkText)
    'End Sub

    'Private Sub NumericUpDown1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown1.ValueChanged
    '    _FONT_SIZE = NumericUpDown1.Value
    'End Sub


    'Private Sub rooms_dgv_DataBindingComplete(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewBindingCompleteEventArgs) Handles rooms_dgv.DataBindingComplete
    '    Try
    '        For i = 0 To 4
    '            If i = thrd.chat_room Then
    '                rooms_dgv(2, i).Value = My.Resources.arrow_180
    '            Else
    '                rooms_dgv(2, i).Value = My.Resources.__empty
    '            End If
    '        Next

    '    Catch ex As Exception

    '    End Try

    'End Sub

    'Private Sub conx_test_ResizeEnd(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ResizeEnd
    '    If Not scroll_lock Then
    '        Try
    '            RTB1.SuspendLayout()
    '            Dim cf As Object = Me.ActiveControl
    '            RTB1.Focus()
    '            RTB1.SelectionStart = RTB1.Text.Length
    '            cf.focus()
    '            RTB1.ResumeLayout()
    '        Catch ex As Exception
    '        End Try
    '        RTB1.SuspendLayout()
    '    End If
    'End Sub

    'Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
    '    Button5.Enabled = False
    '    While timer_flag
    '        timer1.Enabled = False
    '        'wait till timer event is done working
    '    End While
    '    clear_chat_window()
    '    _refresh = True
    '    entry_TextBox1.Text = "I'm Refreshing the Chat Window." & vbCrLf & _
    '                           "Useful if you hid the Avatars."
    '    SyncLock locktite
    '        thrd.send_string = "<GET_BUFFER>" + vbCrLf

    '    End SyncLock

    '    timer1.Enabled = True
    'End Sub


    'Private Sub jump_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles jump.Click
    '    If thrd.chat_room = CInt(ctm_client.Items(3).Text) Then 'in this room already?
    '        timer1.Enabled = True
    '        Return
    '    End If
    '    While timer_flag
    '        timer1.Enabled = False
    '        'wait till timer event is done working
    '    End While
    '    SyncLock locktite
    '        thrd.send_string = "<roomchange>|" & ctm_client.Items(3).Text
    '    End SyncLock
    '    thrd.chat_room = CInt(ctm_client.Items(3).Text)
    '    timer1.Enabled = True
    'End Sub

    'Private Sub save_chat_btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles save_chat_btn.Click
    '    If Not FileIO.FileSystem.DirectoryExists("C:\!_CC") Then
    '        FileIO.FileSystem.CreateDirectory("C:\!_CC")
    '    End If
    '    Dim st As String = RTB1.Rtf
    '    RTB1.SaveFile("c:\!_CC\Chat_Dump.rtf")
    '    'My.Computer.FileSystem.WriteAllText("c:\!_CC\Chat_Dump.rtf", st, False)
    '    System.Diagnostics.Process.Start("c:\!_CC\Chat_Dump.rtf")

    'End Sub

#End Region

End Module
