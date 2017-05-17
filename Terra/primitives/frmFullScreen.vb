Public Class frmFullScreen

    Private Sub frmFullScreen_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
		If e.Control Then
			If e.KeyCode = Keys.OemMinus Then
				icon_scale -= 0.5!
				If icon_scale < 3 Then
					icon_scale = 3
				End If
				tb1.Text = "Icon size: " + icon_scale.ToString
				My.Settings.icon_scale = icon_scale
				frmMain.need_screen_update()
				Return
			End If
			If e.KeyCode = Keys.Oemplus Then
				icon_scale += 0.5!
				If icon_scale > 100.0! Then
					icon_scale = 100.0!
				End If
				tb1.Text = "Icon size: " + icon_scale.ToString
				My.Settings.icon_scale = icon_scale
				frmMain.need_screen_update()
				Return
			End If
		End If
		If e.KeyCode = Keys.C Then
			If frmMain.m_show_chunks.Checked Then
				frmMain.m_show_chunks.Checked = False
			Else
				frmMain.m_show_chunks.Checked = True
			End If
		End If
		If e.KeyCode = Keys.T Then
			If tankID > -1 Then
				MOVE_TANK = True
				move_mod = True ' SHIFT KET
			End If
		End If
		If e.KeyCode = Keys.R Then
			If Not ROTATE_TANK Then
				If tankID > -1 Then
					ROTATE_TANK = True
				Else
					If rendermode Then
						rendermode = False
						frmMain.need_screen_update()
					Else
						rendermode = True
						frmMain.need_screen_update()
					End If
				End If
			End If
		End If
		If e.KeyCode = 16 Then
			If Not move_mod Then
				move_mod = True ' SHIFT KET
				If Not NetData Then
					frmMain.need_screen_update()
				End If
			End If

		End If
		If e.KeyCode = 17 Then
			If Not z_move Then
				z_move = True ' CTRL KEY
				'draw_scene()
			End If
		End If
		If e.KeyCode = Keys.OemMinus Then
			minimap_size -= 32.0!
			If minimap_size < 128.0! Then
				minimap_size = 128.0!
			End If
			'tb1.Text = "Minimap size: " + minimap_size.ToString
			My.Settings.minimap_size = minimap_size
			frmMain.need_screen_update()
		End If
		If e.KeyCode = Keys.Oemplus Then
			minimap_size += 32.0!
			If minimap_size > 900.0! Then
				minimap_size = 900.0!
			End If
			'tb1.Text = "Minimap size: " + minimap_size.ToString
			My.Settings.minimap_size = minimap_size
			frmMain.need_screen_update()
		End If


	End Sub

	Private Sub frmFullScreen_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
		MOVE_TANK = False
		ROTATE_TANK = False
		If move_mod Then
			move_mod = False
			frmMain.need_screen_update()
		End If
		If z_move Then
			z_move = False
			frmMain.need_screen_update()
		End If

	End Sub

    Private Sub frmFullScreen_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.KeyPreview = True 'so i catch keyboard before despatching it

    End Sub
End Class