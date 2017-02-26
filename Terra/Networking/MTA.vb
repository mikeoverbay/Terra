Imports System.Threading
Imports System.IO
Imports System.String
Imports System.Net.Sockets
Imports System.Text

Public Enum _ERR As Integer
    NONE = 0
    CANT_WRITE = 1
    CANT_READ = 2
    UNAUTH = 3
    NORESPONSE = 4
    PORTERR = 5
End Enum
Module MTA

    Public Class users
        Public id As Integer
        Public ip As String
        Public tcp As New TcpClient
        Public data_ready As Boolean = False
        Public thrd As New thd
        Public Sub start()
            thrd.Id = Me.id
            thrd.run()
        End Sub
    End Class
    Public Class class_st
        Public wait_hdl_W = New ManualResetEvent(False)
        Public wait_hdl_R = New ManualResetEvent(False)
        Public bytes(20000) As Byte
        Public network As NetworkStream
        Public str As String = ""
        Public t_str As String = ""
        Public err As Integer = 0
    End Class
    Public Class thd
        Public in_str As String = ""
        Public send_string As String = ""
        Public trd As New Thread(AddressOf main_thd)
        Public Id As Integer
        Public state As New class_st
        Public active As Boolean = False
        Public chat_room As Integer = 0
        Public abortme As Boolean = False
        Public Sub run()
            Try
                state.network = tcpClient.GetStream()

            Catch ex As Exception
                Return
            End Try
            trd.Start()
            trd.IsBackground = True
            trd.Name = "TCP Thread"
            trd.Priority = ThreadPriority.Normal
        End Sub
        <MTAThread()> _
        Public Sub main_thd()
            active = True
            Dim kill As Boolean = False
            tcpClient.NoDelay = True
            Dim strIn As String = ""
            While 1
                If Me.send_string.Length > 0 Then
                    SyncLock locktite
                        If Me.send_string.Length > 0 Then
                            If InStr(Me.send_string, "6`") > 0 Then
                                kill = True
                            End If
                        End If
                        write_tcp(Me.send_string, thrd.state)
                    End SyncLock

                End If

                Thread.Sleep(15)
                Dim trs As String = ""
                thrd.in_str = read_tcp(thrd.state)
                If thrd.in_str.Length > 0 Then
                    RET_STRING = thrd.in_str
                End If
                If kill Then
                    Thread.Sleep(2000)
                    connected_user(Id).id = -1
                    tcpClient.Close()
                    Me.abortme = True
                End If
no_comm:
                Thread.Sleep(15)
                If abortme Then
                    Me.trd.Abort()
                End If
                SENDING = False ' kill wait on send flag
            End While
        End Sub
    End Class
    <MTAThread()> _
   Public Sub write_tcp(ByVal command As String, ByVal state As Object)
        Dim Id = thrd.Id
        Try
            tcpClient.ReceiveBufferSize = 20000
        Catch ex As Exception
            Return
        End Try
        Dim strIn, ts0 As String

        ts0 = ""
        strIn = ""


        If state.network.CanWrite And state.network.CanRead And tcpClient.Connected Then
            Dim sendBytes As [Byte]() = Encoding.ASCII.GetBytes(command & "e-o-l") 'convert command"
            Dim er0 As Boolean = False


            state.wait_hdl_w.reset()

            Try
                state.network.beginWrite(sendBytes, 0, sendBytes.Length, _
                            New AsyncCallback(AddressOf WriteCallback), state) ' send stream
            Catch ex As Exception
                state.err = _ERR.CANT_WRITE
                Try
                    're_conx(Id, data.form)
                Catch ex2 As Exception
                    MsgBox(ex2.ToString)
                End Try
                'state.wait_hdl_W.waitone()
                If state.wait_hdl_W.WaitOne(New TimeSpan(0, 0, 0, 5, 0), True) Then
                    Debug.WriteLine("Work method signaled.")
                Else
                    SERVER_DOWN = True
                    state.str = ""
                End If
                er0 = True

            End Try
        Else
            If Not state.network.CanRead Then
                'connected_user(Id).tcp.Close()
                ' Update_thread_state_window(Id, "Port Not Working", data.form) ' post to message q
                state.err = _ERR.PORTERR
                GoTo continue_thrd
            Else
                If Not state.network.CanWrite Then
                    SERVER_DOWN = True
                    state.err = _ERR.PORTERR
                    GoTo continue_thrd
                End If
            End If
        End If
continue_thrd:
        thrd.send_string = ""
        WAIT_SEND = False
    End Sub
    <MTAThread()> _
    Public Function read_tcp(ByVal state As Object) As String
        Dim Id = thrd.Id
        Try
            tcpClient.ReceiveBufferSize = 8192

        Catch ex As Exception
            Return ""
        End Try
        Dim strIn, ts0 As String
        ts0 = ""
        strIn = ""

        If state.network.CanWrite And state.network.CanRead And tcpClient.Connected Then

            state.str = ""
            state.t_str = ""
            Dim er1 As Boolean = False
get_more:
            If tcpClient.Available = 0 Then
                state.str = ""
                Return ""
            End If
            While (1)
                ' repete until all of data is read

                state.wait_hdl_R.reset()
                Try
                    state.network.BeginRead(state.bytes, _
                          0, state.bytes.Length, New System.AsyncCallback(AddressOf async_read_ack), _
                          state)
                Catch ex As Exception
                    state.err = _ERR.CANT_READ
                    er1 = True
                End Try
                If er1 Then
                    er1 = False
                    GoTo continue_thrd
                End If

                'state.wait_hdl_R.waitone()
                If state.wait_hdl_R.WaitOne(New TimeSpan(0, 0, 0, 5, 0), True) Then
                    'Debug.WriteLine("Work method signaled.")
                Else
                    SERVER_DOWN = True
                    state.str = ""
                End If

                ts0 = state.str
                If ts0 = "" Then Exit While
                If InStr(ts0, "e-o-l") > 0 Then

                    Exit While
                End If
                '               ts0 = Replace(ts0, Chr(0), "")
                strIn += ts0
            End While
            ts0 = Microsoft.VisualBasic.Replace(state.str, "e-o-l", "")
            state.str = ts0
            If state.str = Nothing Then
                state.str = ""
            End If
            If tcpClient.Available > 0 Then
                GoTo get_more

            End If
        Else
            If Not state.network.CanRead Then
                SERVER_DOWN = True
                'connected_user(Id).tcp.Close()
                state.err = _ERR.PORTERR
                GoTo continue_thrd
            Else
                If Not state.network.CanWrite Then
                    state.err = _ERR.PORTERR
                    GoTo continue_thrd
                End If
            End If
        End If
continue_thrd:
        Return state.str
    End Function
    <MTAThread()> _
     Private Sub async_read_ack(ByVal ar As IAsyncResult)
        Dim _state As class_st = CType(ar.AsyncState, class_st)
        Dim BytesRead As Integer
        Dim ts As String = ""

        Try
            BytesRead = _state.network.EndRead(ar)

            ts = Encoding.ASCII.GetString(_state.bytes, 0, BytesRead)
            _state.t_str += ts
        Catch ex As Exception

        End Try
        _state.str = _state.t_str
        _state.wait_hdl_R.set()
    End Sub
    <MTAThread()> _
    Private Sub WriteCallback(ByVal ar As IAsyncResult)
        ' result is always a BlueToolsAsyncResult object
        Dim _state As class_st = CType(ar.AsyncState, class_st)
        ' We passed on buffer as custom object, you can pass on any object here. We passed the stream object
        ' Dim stream As ServiceStream = blueAsyncResult.AsyncState

        Try
            ' EndWrite() must always be called!
            ' If stream has been closed due to an error, we'll have an excpetion here
            _state.network.EndWrite(ar)

        Catch ex As ObjectDisposedException
            ' Thrown if stream has been closed.
        End Try
        _state.wait_hdl_W.set()
    End Sub
End Module


