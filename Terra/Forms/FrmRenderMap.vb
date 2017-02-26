Imports System.Windows.Forms
Imports System.Windows
Public Class FrmRenderMap

	Private Sub img_512_rb_CheckedChanged(sender As Object, e As EventArgs) Handles img_512_rb.CheckedChanged
		render_size = 512
	End Sub

	Private Sub img_1024_rb_CheckedChanged(sender As Object, e As EventArgs) Handles img_1024_rb.CheckedChanged
		render_size = 1024
	End Sub

	Private Sub img_2048_rb_CheckedChanged(sender As Object, e As EventArgs) Handles img_2048_rb.CheckedChanged
		render_size = 2048
	End Sub

	Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
		render_size = 4096
	End Sub

	Private Sub Render_btn_Click(sender As Object, e As EventArgs) Handles Render_btn.Click
		frmMain.Render_minimap(render_size)
		Dim da_map = frmMain.Render_minimap(render_size)
		frmMain.draw_scene()
		Dim sfd As New SaveFileDialog
		sfd.Title = "Save BMP"
		sfd.InitialDirectory = GAME_PATH
		sfd.Filter = "Bitmap Files (*.bmp)|*.bmp"
		If sfd.ShowDialog = Forms.DialogResult.OK Then
			Dim p = sfd.FileName
			If p.Contains(".bmp") Then
				p = p.Replace(".bmp", "")
			End If
			p += ".bmp"
			da_map.Save(p)
			da_map.Dispose()
			frmMain.draw_scene()
		End If
		sfd.Dispose()
		Me.Hide()
	End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		Me.Hide()
	End Sub
End Class