Public Class frmOutput


    Private Sub TextBox1_MouseClick(sender As Object, e As MouseEventArgs) Handles TextBox1.MouseClick
        TextBox1.SelectionLength = 0
        Dim a = TextBox1.GetLineFromCharIndex(TextBox1.GetFirstCharIndexOfCurrentLine())
        TextBox1.Select(TextBox1.GetFirstCharIndexOfCurrentLine(), TextBox1.Lines(a).Length)
        If TextBox1.SelectedText.Length < 4 Then
            Return
        End If

        Try

            Dim mapa = TextBox1.SelectedText.Split(":")
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
            frmMain.draw_scene()
        Catch ex As Exception

        End Try
    End Sub
End Class