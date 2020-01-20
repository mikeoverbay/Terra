Public Class frmMapInfo

    Private Sub frmMapInfo_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Me.Hide()
        Return
    End Sub

    Private Sub frmMapInfo_Load(sender As Object, e As EventArgs) Handles Me.Load
    End Sub

    Private Sub I__Decal_Textures_tb_MouseClick(sender As Object, e As MouseEventArgs) Handles I__Decal_Textures_tb.MouseClick
        show_image(sender)
    End Sub

    Private Sub I__Map_Textures_tb_MouseClick(sender As Object, e As MouseEventArgs) Handles I__Map_Textures_tb.MouseClick
        show_image(sender)
    End Sub

    Private Sub I__Model_Textures_tb_MouseClick(sender As Object, e As MouseEventArgs) Handles I__Model_Textures_tb.MouseClick
        show_image(sender)
    End Sub

    Private Sub I__Tree_Textures_tb_MouseClick(sender As Object, e As MouseEventArgs) Handles I__Tree_Textures_tb.MouseClick
        show_image(sender)
    End Sub

    Private Sub show_image(ByVal TextBox1 As TextBox)
        TextBox1.SelectionLength = 0
        Dim a = TextBox1.GetLineFromCharIndex(TextBox1.GetFirstCharIndexOfCurrentLine())
        TextBox1.Select(TextBox1.GetFirstCharIndexOfCurrentLine(), TextBox1.Lines(a).Length)
        If TextBox1.SelectedText.Length < 4 Then
            Return
        End If
        Dim mapa = TextBox1.SelectedText.Split(": ")
        Dim shr_name As String
        Try
            shr_name = mapa(1).Replace(" ", "")
            If shr_name.ToLower.Contains("composite") Then
                If shr_name.ToLower.Contains("normal") Then
                    'show_texture(speedtree_NormalMapID)
                Else
                    'show_texture(speedtree_imageID)

                End If
            End If
        Catch ex As Exception
            Return
        End Try
        For i = 0 To decal_cache.Length - 1
            If shr_name = decal_cache(i).name Then
                show_texture(decal_cache(i).texture_id)
                Return
            End If
            If shr_name = decal_cache(i).normal_name Then
                show_texture(decal_cache(i).normal_id)
                Return
            End If
        Next
        For i = 0 To texture_cache.Length - 1
            If shr_name = texture_cache(i).name Then
                show_texture(texture_cache(i).textureID)
                Return
            End If
        Next

        For i = 0 To texture_cache.Length - 1
            If shr_name = texture_cache(i).normalname Then
                show_texture(texture_cache(i).textureNormID)
                Return
            End If
        Next
        For i = 0 To layer_texture_cache.Length - 1
            If shr_name = layer_texture_cache(i).name Then
                show_texture(layer_texture_cache(i).id)
                Return
            End If
        Next

        For i = 0 To tree_textures.Length - 1
            If shr_name = tree_textures(i).name Then
                show_texture(tree_textures(i).textureID)
                Return
            End If
            If shr_name = tree_textures(i).normalname Then
                show_texture(tree_textures(i).textureNormID)
                Return
            End If
        Next



    End Sub
    Private Sub show_texture(ByVal id As Integer)
        If Not frmShowImage.visible Then
            frmShowImage.visible = True
            While Not frmShowImage.ready_to_render
                Application.DoEvents()
            End While
        End If
        frmShowImage.draw_texture(id)
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        frmShowImage.draw_texture(id) 'draw again just to make sure it shows up
    End Sub

End Class