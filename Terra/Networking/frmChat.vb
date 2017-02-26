Imports System.Windows.Forms
Imports System.Drawing

Public Class frmChat

  Private Sub chat_input_tb_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles chat_input_tb.KeyDown
    If e.KeyCode = Keys.Enter Then
      If chat_input_tb.Text.Length = 0 Then
        chat_input_tb.Text = ""
        Return ' dont send empty text
      End If
      _send_text()
    End If

  End Sub

  Private Sub chat_input_tb_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chat_input_tb.TextChanged
    If chat_input_tb.Text.Length > 64 Then
      chat_input_tb.Text = Microsoft.VisualBasic.Left(chat_input_tb.Text, chat_input_tb.Text.Length - 1)
    End If
  End Sub

  Private Sub chat_box_tb_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chat_box_tb.TextChanged

  End Sub

  Private Sub hide_chat_bt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles hide_chat_bt.Click
    Me.Hide()
  End Sub
End Class