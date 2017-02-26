Public Class frmGetIP
    Public OK As Boolean = False
    Private Sub frmGetIP_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        OK = False

    End Sub

    Private Sub frmGetIP_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ip_tb.Text = My.Settings.server_address
        Me.Show()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If user_name_tb.Text.Length = 0 Or _
        game_port_tb.Text.Length = 0 Then
            MsgBox("You need ALL connection items filled.", MsgBoxStyle.Critical, "Empty Fields")
            Return
        End If
        'server_address = ip_tb.Text
        'server_port_number = port_tb.Text
        'game_port = game_port_tb.Text
        OK = True
        Me.Hide()
    End Sub

    'Private Sub abort_btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    abort_connect_starter = True
    'End Sub

    'Private Sub user_name_tb_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles user_name_tb.KeyDown
    '    If e.KeyCode = Keys.Enter Then
    '        user_password_tb.Focus()
    '    End If
    'End Sub

    Private Sub user_name_tb_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles user_name_tb.TextChanged
        'cant allow more than 15 characters for the name
        If user_name_tb.Text.Length > 15 Then
            user_name_tb.Text = Microsoft.VisualBasic.Left(user_name_tb.Text, user_name_tb.Text.Length - 1)
        End If
    End Sub



    Private Sub get_my_ip_btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles get_my_ip_btn.Click
        System.Diagnostics.Process.Start("IExplore.exe", "http://www.whatismyip.com/")
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        System.Diagnostics.Process.Start("IExplore.exe", "http://portforward.com/english/routers/port_forwarding/")

    End Sub

    Private Sub game_port_tb_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles game_port_tb.TextChanged

    End Sub

    Private Sub ip_tb_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ip_tb.TextChanged

    End Sub
End Class