
Public Class frmBiasing
    Public SB_SIZE As Single = 128
    Public SHOW_SELECT_MESH As Boolean = True


    Private Sub terrain_clip_MouseEnter(sender As Object, e As EventArgs) Handles terrain_clip.MouseEnter
        terrain_clip.Focus()
    End Sub
    Private Sub terrain_clip_Scroll(sender As Object, e As EventArgs) Handles terrain_clip.Scroll
        If Not maploaded Then Return
        TERRAIN_BIAS = CSng(terrain_clip.Value / 1000.0) * 15.0
        decal_matrix_list(frmMain.d_counter).t_bias = TERRAIN_BIAS
        update_includer_string()
        If decal_matrix_list(frmMain.d_counter).good Then
            remake_Decal(frmMain.d_counter)
        End If
        frmMain.draw_scene()
    End Sub

    Private Sub decal_clip_MouseEnter(sender As Object, e As EventArgs) Handles decal_clip.MouseEnter
        decal_clip.Focus()
    End Sub

    Private Sub decal_clip_Scroll(sender As Object, e As EventArgs) Handles decal_clip.Scroll
        If Not maploaded Then Return
        DECAL_BIAS = CSng(decal_clip.Value / 1000.0) * 15.0
        decal_matrix_list(frmMain.d_counter).d_bias = DECAL_BIAS
        update_includer_string()
        If decal_matrix_list(frmMain.d_counter).good Then
            remake_Decal(frmMain.d_counter)
        End If
        frmMain.draw_scene()
    End Sub

    Private Sub zoom_up_Click(sender As Object, e As EventArgs) Handles zoom_up.Click
        SB_SIZE *= 2
        If SB_SIZE > 1024 Then
            SB_SIZE = 1024
        End If
        frmMain.draw_scene()
    End Sub

    Private Sub zoom_down_Click(sender As Object, e As EventArgs) Handles zoom_down.Click
        SB_SIZE /= 2
        If SB_SIZE < 16 Then
            SB_SIZE = 16
        End If
        frmMain.draw_scene()
    End Sub

    Private Sub view_mode_btn_Click(sender As Object, e As EventArgs) Handles view_mode_btn.Click
        If frmMain.view_mode = True Then
            frmMain.view_mode = False
        Else
            frmMain.view_mode = True
        End If
        frmMain.draw_scene()
    End Sub

    Private Sub exclude_chk_box_CheckedChanged(sender As Object, e As EventArgs) Handles exclude_chk_box.CheckedChanged
        If Not maploaded Then Return
        If exclude_chk_box.Checked Then
            decal_matrix_list(frmMain.d_counter).exclude = True
            EXCLUDED = True
        Else
            decal_matrix_list(frmMain.d_counter).exclude = False
            EXCLUDED = False
        End If
        update_includer_string()
        If decal_matrix_list(frmMain.d_counter).good Then
            remake_Decal(frmMain.d_counter)
        End If
        frmMain.draw_scene()
    End Sub

    Public Sub update_includer_string()
        If Not maploaded Then Return
        Dim index = frmMain.d_counter
        With decal_matrix_list(index)
            frmMain.tb1.Text = "Index: " + index.ToString("0000") + _
                 vbCrLf + "Terrain Bias:" + .t_bias.ToString("0.00") + _
                 " Decal Bias: " + .d_bias.ToString("0.00") + " Excluded" + .exclude.ToString
            Dim s = "-" + index.ToString("0000")
            If decal_includers_string.Contains(s) Then
                Dim newlist As List(Of String) = results_tb.Lines.ToList
                Dim cnt = 0
                For Each line In newlist
                    If line.Contains(s) Then
                        newlist.RemoveAt(cnt)
                        Exit For
                    End If
                    cnt += 1
                Next
                DECAL_BIAS = .d_bias
                TERRAIN_BIAS = .t_bias
                EXCLUDED = .exclude
                s = "-" + index.ToString("0000") + ":" + .t_bias.ToString("0.00") + ":" + .d_bias.ToString("0.00") + ":" + .exclude.ToString
                newlist.Insert(cnt, s)
                results_tb.Lines = newlist.ToArray
                decal_includers_string = results_tb.Text
            Else
            End If
        End With
    End Sub


    Private Sub frmBiasing_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If maploaded Then
            Me.BringToFront()
            If MsgBox("You are in EDIT INCLUDERS mode..." + vbCrLf + _
                           "Do you wish to save your changes?", MsgBoxStyle.YesNo, "Warning!!!") = MsgBoxResult.Yes Then
                IO.File.WriteAllText(Application.StartupPath + "\decal_includer_files\" + JUST_MAP_NAME + "_decal_includers.txt", decal_includers_string)
            End If
        End If
        EDIT_INCULDERS = False
        frmMain.draw_scene()
    End Sub

    Private Sub frmBiasing_Load(sender As Object, e As EventArgs) Handles Me.Load
        results_tb.Text = IO.File.ReadAllText(Application.StartupPath + "\decal_includer_files\" + JUST_MAP_NAME + "_decal_includers.txt")
        decal_includers_string = results_tb.Text
        EDIT_INCULDERS = True
        check_decal_include_strings()
        get_decal_bias_settings()
        frmMain.draw_scene()
    End Sub

    Private Sub results_tb_Click(sender As Object, e As EventArgs) Handles results_tb.Click
        results_tb.SelectionLength = 0
        Dim a = results_tb.GetLineFromCharIndex(results_tb.GetFirstCharIndexOfCurrentLine())
        results_tb.Select(results_tb.GetFirstCharIndexOfCurrentLine(), results_tb.Lines(a).Length)
        If results_tb.SelectedText.Length < 4 Then
            Return
        End If
        Try
            Dim ar = results_tb.SelectedText.ToArray
            Dim sel = results_tb.SelectedText.Split(":")
            Dim lx, ly, lz As Single
            If ar(0) = "-" Then 'decal
                Dim id = CInt(sel(0)) * -1
                lx = decal_matrix_list(id).matrix(12)
                ly = decal_matrix_list(id).matrix(13)
                lz = decal_matrix_list(id).matrix(14)
                frmMain.d_counter = id
                With decal_matrix_list(id)

                    TERRAIN_BIAS = .t_bias
                    DECAL_BIAS = .d_bias
                    EXCLUDED = .exclude
                    Application.DoEvents()

                    decal_clip.Value = CInt((DECAL_BIAS / 15.0) * 1000.0)
                    terrain_clip.PerformLayout()
                    terrain_clip.Invalidate()
                    terrain_clip.Value = CInt((TERRAIN_BIAS / 15.0) * 1000.0)
                    terrain_clip.PerformLayout()
                    terrain_clip.Invalidate()
                    exclude_chk_box.Checked = EXCLUDED
                    Application.DoEvents()
                End With
            Else
                Dim id = CInt(sel(0)) ' model
                lx = Model_Matrix_list(id).matrix(12)
                ly = Model_Matrix_list(id).matrix(13)
                lz = Model_Matrix_list(id).matrix(14)
            End If
            look_point_X = lx
            u_look_point_X = lx
            look_point_Z = lz
            u_look_point_Z = lz
            look_point_Y = ly
            u_look_point_Y = ly
            frmMain.position_camera()
            frmMain.draw_scene()
        Catch ex As Exception
        End Try
        Return
    End Sub

    Private Sub show_select_mesh_btn_Click(sender As Object, e As EventArgs) Handles show_select_mesh_btn.Click
        If SHOW_SELECT_MESH Then
            SHOW_SELECT_MESH = False
        Else
            SHOW_SELECT_MESH = True
        End If
        frmMain.draw_scene()
    End Sub

    Private Sub decals_Click(sender As Object, e As EventArgs) Handles decals.Click
        frmMain.m_wire_decals.PerformClick()
    End Sub

    Private Sub models_Click(sender As Object, e As EventArgs) Handles models.Click
        frmMain.m_wire_models.PerformClick()
    End Sub

    Private Sub trees_Click(sender As Object, e As EventArgs) Handles trees.Click
        frmMain.m_wire_trees.PerformClick()
    End Sub

    Private Sub terrain_Click(sender As Object, e As EventArgs) Handles terrain.Click
        frmMain.m_wire_terrain.PerformClick()
    End Sub
End Class