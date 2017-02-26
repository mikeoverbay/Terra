Public Class frmTestView
    Private image_scale As Single = 0.25
    Private image_id As Integer = -1

    Private Sub frmTestView_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Me.Hide()
        e.Cancel = True ' if we close this form, we lose the event handlers added at load time!!
    End Sub
    Private Sub frmTestView_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Show()
        AddHandler full_scale.CheckedChanged, AddressOf CheckedChanged
        AddHandler half_scale.CheckedChanged, AddressOf CheckedChanged
        AddHandler quater_scale.CheckedChanged, AddressOf CheckedChanged
        AddHandler eigth_scale.CheckedChanged, AddressOf CheckedChanged
        AddHandler sixtenth_scale.CheckedChanged, AddressOf CheckedChanged

        AddHandler img_1.CheckedChanged, AddressOf image_changed
        AddHandler img_2.CheckedChanged, AddressOf image_changed

        image_id = CInt(img_1.Tag)
        update_screen()
    End Sub


    Private Sub CheckedChanged(sender As Object, e As EventArgs)
        image_scale = CSng(sender.tag)
        update_screen()
    End Sub
    Private Sub image_changed(sender As Object, e As EventArgs)
        image_id = CInt(sender.tag)
        update_screen()
    End Sub

    Public Sub update_screen()
        If Not maploaded Then Return
        If Not (Wgl.wglMakeCurrent(pb3_hDC, pb3_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Dim width, height As Integer
        Gl.glViewport(0, 0, pb3.Width, pb3.Height)
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glOrtho(0, pb3.Width, -pb3.Height, 0, 30.0, -30.0) 'Select Ortho Mode
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
        Gl.glLoadIdentity() 'Reset The Matrix

        Gl.glClearColor(0.0, 0.3, 0.0, 0.0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT)

        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(0)
        'select image and shader by selected radio button
        Select Case image_id
            Case 1
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, post_depth_image_id)
                Gl.glUseProgram(shader_list.toLinear_shader)
            Case 2
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, post_color_image_id)
                Gl.glUseProgram(shader_list.basicONEtexture_shader)
            Case 3
            Case 4
            Case 5

        End Select

        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_WIDTH, width)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_HEIGHT, height)
        h_label.Text = "Height:" + height.ToString("0000")
        w_label.Text = "Width:" + width.ToString("0000")
        width *= image_scale
        height *= image_scale
        Gl.glBegin(Gl.GL_QUADS)

        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex2f(0.0, 0.0)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex2f(width, 0.0)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex2f(width, -height)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex2f(0.0, -height)
        Gl.glEnd()

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        Gdi.SwapBuffers(pb3_hDC) ' swap back to front

        'switch back to main context
        Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)
    End Sub

    Private Sub frmTestView_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        update_screen()
    End Sub

    Private Sub frmTestView_ResizeBegin(sender As Object, e As EventArgs) Handles Me.ResizeBegin

    End Sub

    Private Sub frmTestView_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        update_screen()

    End Sub
End Class