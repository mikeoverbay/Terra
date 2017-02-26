Imports System.Windows.Forms
Imports System.String
Imports System.Math
Public Class frmTanks

	Private Sub frmTanks_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		e.Cancel = True
		frmMain.m_layout_mode.Checked = False
		Me.Visible = False
		tankID = -1
		For i = 0 To SplitContainer1.Panel1.Controls.Count - 1
			If SplitContainer1.Panel1.Controls(i).Text.Length > 0 Then
				SplitContainer1.Panel1.Controls(i).BackColor = Color.DarkRed
			End If
		Next
		For i = 0 To SplitContainer1.Panel2.Controls.Count - 1
			If SplitContainer1.Panel2.Controls(i).Text.Length > 0 Then
				SplitContainer1.Panel2.Controls(i).BackColor = Color.Green
			End If
		Next
		frmMain.draw_scene()
	End Sub

	Private Sub frmTanks_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
		Return
		If e.Control Then
			If e.KeyCode = Keys.OemMinus Then
				icon_scale -= 0.5!
				If icon_scale < 3 Then
					icon_scale = 3
				End If
				frmMain.tb1.Text = "Icon size: " + icon_scale.ToString
				My.Settings.icon_scale = icon_scale
				frmMain.draw_scene()
				Return
			End If
			If e.KeyCode = Keys.Oemplus Then
				icon_scale += 0.5!
				If icon_scale > 100.0! Then
					icon_scale = 100.0!
				End If
				frmMain.tb1.Text = "Icon size: " + icon_scale.ToString
				My.Settings.icon_scale = icon_scale
				frmMain.draw_scene()
				Return
			End If
		End If
		If e.KeyCode = Keys.OemMinus Then
			minimap_size -= 32.0!
			If minimap_size < 128.0! Then
				minimap_size = 128.0!
			End If
			frmMain.tb1.Text = "Minimap size: " + minimap_size.ToString
			My.Settings.minimap_size = minimap_size
			frmMain.draw_scene()
		End If
		If e.KeyCode = Keys.Oemplus Then
			minimap_size += 32.0!
			If minimap_size > 900.0! Then
				minimap_size = 900.0!
			End If
			frmMain.tb1.Text = "Minimap size: " + minimap_size.ToString
			My.Settings.minimap_size = minimap_size
			frmMain.draw_scene()
		End If
		If e.KeyCode = Keys.T Then
			If tankID > -1 Then
				MOVE_TANK = True
				move_mod = True ' SHIFT KET
			End If
		End If
		If e.KeyCode = Keys.R Then
			If tankID > -1 Then
				ROTATE_TANK = True
			End If
		End If
		If e.KeyCode = 16 Then
			move_mod = True ' SHIFT KET
			frmMain.draw_scene()
		End If
		If e.KeyCode = 17 Then
			z_move = True ' CTRL KEY
			frmMain.draw_scene()
		End If
	End Sub

	Private Sub frmTanks_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
		If move_mod Then
			move_mod = False
			frmMain.draw_scene()
		End If
		If z_move Then
			z_move = False
			frmMain.draw_scene()
		End If
	End Sub

	Private Sub frmTanks_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Me.KeyPreview = True	'so i catch keyboard before despatching it
		SplitContainer1.Width = Me.ClientSize.Width - Panel1.Width
		SplitContainer1.SplitterDistance = Me.SplitContainer1.Width / 2
		make_btns()
	End Sub
	Public Sub make_btns()
		SplitContainer1.Panel1.Controls.Clear()
		SplitContainer1.Panel2.Controls.Clear()

		Dim ww = SplitContainer1.Panel1.Width - 6
		Dim t = a_tanks(0).image.Width
		Dim l As New Label
		Dim fnt As New Font(frmMain.pfc.Families(0), 6, FontStyle.Regular)

		For i = 0 To 14
			Dim butt As New Button
			butt.FlatStyle = FlatStyle.Flat
			butt.FlatAppearance.BorderSize = 2
			butt.FlatAppearance.BorderColor = Color.DarkRed
			butt.Font = fnt
			Dim m = ww / t
			butt.Width = t * m
			butt.Height = a_tanks(0).image.Height * m
			butt.Tag = "1_" & i.ToString
			butt.Text = ""
			butt.BackgroundImage = My.Resources.open_slot
			butt.BackgroundImageLayout = ImageLayout.Stretch
			butt.TextAlign = ContentAlignment.TopRight
			butt.ForeColor = Color.White
			butt.Font = frmMain.m_layout_mode.Font
			Dim lbl As New Label
			lbl.Width = butt.Width
			lbl.Height = butt.Height - 3
			lbl.BackColor = Color.Transparent
			lbl.Font = butt.Font
			lbl.ForeColor = Color.Red 'dont matter.. it will show as gray cuz its disabled.
			lbl.TextAlign = ContentAlignment.BottomLeft
			lbl.Text = CStr(i + 1)
			lbl.Enabled = False
			butt.Controls.Add(lbl)
			AddHandler butt.MouseClick, AddressOf Me.team_button_click
			AddHandler butt.GotFocus, AddressOf butt_got_focus
			AddHandler butt.LostFocus, AddressOf butt_lost_focusR
			SplitContainer1.Panel1.Controls.Add(butt)
			butt.Location = (New System.Drawing.Point(3, i * butt.Height))
		Next
		For i = 0 To 14
			Dim butt As New Button
			butt.FlatStyle = FlatStyle.Flat
			butt.FlatAppearance.BorderSize = 2
			butt.FlatAppearance.BorderColor = Color.Green
			butt.Font = fnt
			Dim m = ww / t
			butt.Width = t * m
			butt.Height = a_tanks(0).image.Height * m
			butt.Tag = "2_" & i.ToString
			butt.Text = ""
			butt.BackgroundImage = My.Resources.open_slot
			butt.BackgroundImageLayout = ImageLayout.Stretch
			butt.TextAlign = ContentAlignment.TopRight
			butt.ForeColor = Color.White
			butt.Font = frmMain.m_layout_mode.Font
			Dim lbl As New Label
			lbl.Width = butt.Width
			lbl.Height = butt.Height - 3
			lbl.BackColor = Color.Transparent
			lbl.Font = butt.Font
			lbl.ForeColor = Color.Red 'dont matter.. it will show as gray cuz its disabled.
			lbl.TextAlign = ContentAlignment.BottomLeft
			lbl.Text = CStr(i + 1)
			lbl.Enabled = False
			butt.Controls.Add(lbl)
			AddHandler butt.MouseClick, AddressOf Me.team_button_click
			AddHandler butt.GotFocus, AddressOf butt_got_focus
			AddHandler butt.LostFocus, AddressOf butt_lost_focusG
			SplitContainer1.Panel2.Controls.Add(butt)
			butt.Location = (New System.Drawing.Point(2, i * butt.Height))
		Next
		Dim wo = Me.Height - Me.ClientSize.Height
		Dim h = a_tanks(0).image.Height * (ww / t)
		Me.Height = (15 * h) + wo

	End Sub
	Private Sub butt_lost_focusR(ByVal sender As Object, ByVal e As EventArgs)
		'sender.BackColor = Color.Red
	End Sub
	Private Sub butt_got_focus(ByVal sender As Object, ByVal e As EventArgs)
		'sender.backcolor = Color.Blue
	End Sub
	Private Sub butt_lost_focusG(ByVal sender As Object, ByVal e As EventArgs)
		'sender.BackColor = Color.Red
	End Sub
	Public Sub team_button_click(ByVal sender As Object, ByVal e As EventArgs)
		Dim s As String = sender.tag
		Dim ar() = s.Split("_")
		SyncLock packet_lock
			If ar.Length = 2 Then
				team_setup_selected_tank = s
				tankID = -1
				SyncLock packet_lock
					Packet_out.tankId = -1
				End SyncLock

				frmMain.draw_scene()
				If old_tankID > -1 Then
					If old_tankID >= 100 Then
						SplitContainer1.Panel2.Controls(old_tankID - 100).BackColor = Color.Green
					Else
						SplitContainer1.Panel1.Controls(old_tankID).BackColor = Color.DarkRed
					End If
				End If
				old_tankID = tankID
				Return ' no tank assigned yet
			End If
			team_setup_selected_tank = s
			Dim team As Integer = ar(0).ToString
			Dim nation As String = ar(1)
			Dim index As Integer = ar(2).ToString
			Dim b_index As Integer = ar(3).ToString
			If old_tankID > -1 Then
				If old_tankID >= 100 Then
					SplitContainer1.Panel2.Controls(old_tankID - 100).BackColor = Color.Green
				Else
					SplitContainer1.Panel1.Controls(old_tankID).BackColor = Color.DarkRed
				End If
			End If
			If team > 1 Then
				tankID = 100 + b_index
			Else
				tankID = b_index
			End If
			old_tankID = tankID
			SyncLock packet_lock
				If team = 1 Then
					SplitContainer1.Panel1.Controls(b_index).BackColor = Color.Blue
					look_point_X = locations.team_1(b_index).loc_x
					look_point_Y = get_Z_at_XY(locations.team_1(b_index).loc_x, locations.team_1(b_index).loc_z)
					look_point_Z = locations.team_1(b_index).loc_z
					Cam_X_angle = locations.team_1(b_index).rot_y - PI
					frmMain.m_comment.Text = locations.team_1(b_index).comment
					Packet_out.Rx = Cam_X_angle
					Packet_out.ID = s
					Packet_out.tankId = tankID
					Packet_out.Ex = locations.team_1(b_index).loc_x
					Packet_out.Ez = locations.team_1(b_index).loc_z
					Packet_out.Ey = get_Z_at_XY(locations.team_1(b_index).loc_x, locations.team_1(b_index).loc_z)

					Packet_out.Tx = locations.team_1(b_index).loc_x
					Packet_out.Tz = locations.team_1(b_index).loc_z
					Packet_out.Tr = locations.team_1(b_index).rot_y
					Packet_out.ID = locations.team_1(b_index).id

				Else
					SplitContainer1.Panel2.Controls(b_index).BackColor = Color.Blue
					look_point_X = locations.team_2(b_index).loc_x
					look_point_Y = get_Z_at_XY(locations.team_2(b_index).loc_x, locations.team_2(b_index).loc_z)
					look_point_Z = locations.team_2(b_index).loc_z
					Cam_X_angle = locations.team_2(b_index).rot_y - PI
					frmMain.m_comment.Text = locations.team_2(b_index).comment
					Packet_out.Rx = Cam_X_angle
					Packet_out.ID = s
					Packet_out.tankId = tankID
					Packet_out.Ex = locations.team_2(b_index).loc_x
					Packet_out.Ez = locations.team_2(b_index).loc_z
					Packet_out.Ey = get_Z_at_XY(locations.team_2(b_index).loc_x, locations.team_2(b_index).loc_z)

					Packet_out.Tx = locations.team_2(b_index).loc_x
					Packet_out.Tz = locations.team_2(b_index).loc_z
					Packet_out.Tr = locations.team_2(b_index).rot_y
					Packet_out.ID = locations.team_2(b_index).id

				End If
			End SyncLock

		End SyncLock
		frmMain.draw_scene()
		If frmMain.m_show_minimap.Checked Then
			frmMain.draw_minimap()
		End If

	End Sub

	Public Sub prev_nation()
		Me.Panel1.Controls.Item(current_nation).Visible = False
		If current_nation = 0 Then
			current_nation = 6
		Else
			current_nation -= 1
		End If
		Me.Panel1.Controls.Item(current_nation).Visible = True
		Application.DoEvents()
		Me.Panel1.Update()
		frmMain.update_npb()
		npb.Update()
		Threading.Thread.Sleep(50)
	End Sub
	Public Sub next_nation()
		Me.Panel1.Controls.Item(current_nation).Visible = False
		If current_nation = 6 Then
			current_nation = 0
		Else
			current_nation += 1
		End If
		Me.Panel1.Controls.Item(current_nation).Visible = True
		Application.DoEvents()
		Me.Panel1.Update()
		frmMain.update_npb()
		npb.Update()
		Threading.Thread.Sleep(50)
	End Sub


End Class