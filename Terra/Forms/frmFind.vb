Public Class frmFind

	Private Sub cancel_btn_Click(sender As Object, e As EventArgs) Handles cancel_btn.Click
		Me.Close()
	End Sub

	Private Sub find_btn_Click(sender As Object, e As EventArgs) Handles find_btn.Click
		If search_tb.Text.Length = 0 Then
			Return
		End If
     
        If Not maploaded Then
            MsgBox("Load a map before trying to find something, OK?", MsgBoxStyle.Exclamation, "No MAP Loaded!")
            Return
        End If
		results_tb.Text = ""
		Dim found As Boolean = False
        For i = 0 To Model_Matrix_list.Count - 2
            Dim cnt As Integer = 0
            If Model_Matrix_list(i).primitive_name.ToLower.Contains(search_tb.Text.ToLower) Then
                results_tb.Text += i.ToString + ": " + Model_Matrix_list(i).primitive_name + vbCrLf
                found = True
            End If
        Next
		If Not found Then
			results_tb.Text = "*** Nothing was Found! ***"
		End If
	End Sub

	Private Sub grid_list_btn_Click(sender As Object, e As EventArgs) Handles grid_list_btn.Click
		If Not maploaded Then
			MsgBox("Load a map before trying to find something, OK?", MsgBoxStyle.Exclamation, "No MAP Loaded!")
			Return
		End If
		FrmGrid_Listing.Show()
		Me.Close()
    End Sub

    Private Sub frmFind_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

    End Sub

    Private Sub frmFind_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub show_img_btn_Click(sender As Object, e As EventArgs) Handles show_img_btn.Click
        Me.visible = False
        frmShowImage.Show()
        Me.visible = True
        Me.Close()
    End Sub

    Private Sub showlayeruvs_bt_Click(sender As Object, e As EventArgs) Handles showlayeruvs_bt.Click
        frmLayerUV.Show()
        frmLayerUV.TextBox1.Text = layer_uv_list
        Me.Close()
    End Sub

    Private Sub search_tb_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs) Handles search_tb.PreviewKeyDown
        If e.KeyCode = Keys.Enter Then
            find_btn.PerformClick()
        End If
    End Sub


    Private Sub results_tb_MouseClick(sender As Object, e As MouseEventArgs) Handles results_tb.MouseClick
        results_tb.SelectionLength = 0
        Dim a = results_tb.GetLineFromCharIndex(results_tb.GetFirstCharIndexOfCurrentLine())
        results_tb.Select(results_tb.GetFirstCharIndexOfCurrentLine(), results_tb.Lines(a).Length)
        If results_tb.SelectedText.Length < 4 Then
            Return
        End If

        Try

            Dim mapa = results_tb.SelectedText.Split(":")
            Dim L = CInt(mapa(0))

            Dim x, y, z As Single
            x = Model_Matrix_list(L).matrix(12)
            y = Model_Matrix_list(L).matrix(13)
            z = Model_Matrix_list(L).matrix(14)

            look_point_X = x
            u_look_point_X = x
            look_point_Z = z
            u_look_point_Z = z
            look_point_Y = y
            u_look_point_Y = y
            frmMain.position_camera()
            frmMain.need_screen_update()
        Catch ex As Exception

        End Try

    End Sub

End Class