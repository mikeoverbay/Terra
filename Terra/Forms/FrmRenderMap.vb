Imports System.Windows.Forms
Imports System.Windows
Imports System.IO
Public Class FrmRenderMap

	Private Sub img_512_rb_CheckedChanged(sender As Object, e As EventArgs) Handles img_512_rb.CheckedChanged
        render_sizex = 512
	End Sub

	Private Sub img_1024_rb_CheckedChanged(sender As Object, e As EventArgs) Handles img_1024_rb.CheckedChanged
        render_sizex = 1024
	End Sub

	Private Sub img_2048_rb_CheckedChanged(sender As Object, e As EventArgs) Handles img_2048_rb.CheckedChanged
        render_sizex = 2048
	End Sub


    Dim save_to_address As String = ""
    Private Sub Render_btn_Click(sender As Object, e As EventArgs) Handles Render_btn.Click
        If m_r_layout.Checked Then
            If Not maploaded Then Return
            render_sizey = render_sizex
            Gl.glPushMatrix()
            frmMain.draw_top_down(render_sizex)
            frmMain.draw_top_down(render_sizex)
            Gl.glPopMatrix()
            Gl.glFlush()
        Else
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
            Gl.glReadBuffer(Gl.GL_BACK)
            render_sizex = frmMain.pb1.Width '+ frmMain.pb1.Width Mod 1 'get even size
            render_sizey = frmMain.pb1.Height '+ frmMain.pb1.Height Mod 1
        End If
        Dim sfd As New SaveFileDialog
        Dim p As String
        If My.Settings.image_save_address.Length > 0 Then
            save_to_address = My.Settings.image_save_address
        Else
            If save_to_address = "" Then
                save_to_address = GAME_PATH
            End If
        End If
        Dim ext As String = ""
        sfd.InitialDirectory = save_to_address
        If m_as_bmp.Checked Then
            sfd.Title = "Save BMP"
            sfd.Filter = "Bitmap Files (*.bmp)|*.bmp"
            p = sfd.FileName
            ext = ".bmp"
        End If
        If m_as_png.Checked Then
            sfd.Title = "Save PNG"
            sfd.Filter = "PNG Files (*.png)|*.png"
            p = sfd.FileName
            ext = ".png"
        End If
        If m_as_dds_5.Checked Then
            Il.ilSetInteger(Il.IL_DXTC_FORMAT, Il.IL_DXT5)
            sfd.Title = "Save DDS"
            sfd.Filter = "DDS Files (*.dds)|*.dds"
            p = sfd.FileName
            ext = ".dds"
        End If
        If m_as_jpg.Checked Then
            sfd.Title = "Save JPEG"
            sfd.Filter = "JPG Files (*.jpg)|*.jpg"
            p = sfd.FileName
            ext = ".jpg"
        End If

        Dim er As Integer = 0
        stopGL = True
        While gl_busy
            Application.DoEvents()
        End While

        Il.ilEnable(Il.IL_FILE_OVERWRITE)
        Dim Id As Integer = Il.ilGenImage
        Il.ilBindImage(Id)
        Gl.glPixelStorei(Gl.GL_PACK_ALIGNMENT, 1)
        er = Il.ilGetError
        Il.ilTexImage(render_sizex, render_sizey, 0, 3, Il.IL_RGB, Il.IL_UNSIGNED_BYTE, Nothing)
        er = Gl.glGetError
        Gl.glReadPixels(0, 0, CInt(render_sizex), CInt(render_sizey), Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, Il.ilGetData())
        er = Gl.glGetError
        Gl.glPixelStorei(Gl.GL_PACK_ALIGNMENT, 4)
        Gl.glReadBuffer(Gl.GL_FRONT)

        er = Gl.glGetError
        Gl.glFinish()
        frmMain.pb1.Dock = DockStyle.Fill

        frmMain.need_screen_update()
        Dim status As Integer = 0
        If sfd.ShowDialog = Forms.DialogResult.OK Then
            Il.ilConvertImage(Il.IL_RGB, Il.IL_UNSIGNED_BYTE)
            p = sfd.FileName
            'save the path to where we are saving stuff
            save_to_address = Path.GetFullPath(p)
            My.Settings.image_save_address = save_to_address.Replace(Path.GetFileName(p), "")
            p = Path.ChangeExtension(p, Nothing)
            p += ext
            er = Il.ilGetError
            If m_as_dds_5.Checked Then
                'If frmSaveOptions.mipmaps_cb.Checked Then
                '    Ilu.iluBuildMipmaps()
                'End If
                status = Il.ilSave(Il.IL_DDS, p)
            End If
            If m_as_jpg.Checked Then
                status = Il.ilSave(Il.IL_JPG, p)
            End If
            If m_as_bmp.Checked Then
                status = Il.ilSave(Il.IL_BMP, p)
            End If
            If m_as_png.Checked Then
                status = Il.ilSave(Il.IL_PNG, p)
            End If
            If m_as_dds_5.Checked Then
                status = Il.ilSave(Il.IL_DDS, p)
            End If
            If Not status Then
                MsgBox("Failed to save File!", MsgBoxStyle.Exclamation, "File Save Error")
            End If
            Il.ilDeleteImage(Id)
            Il.ilBindImage(0)

            GC.Collect()
            GC.WaitForFullGCComplete()
            If open_after_save_cb.Checked Then
                Try
                    Process.Start(p)

                Catch ex As Exception
                    MsgBox("Failed to save """"" + p + """", MsgBoxStyle.Critical, "File not Saved!")
                End Try
            End If
        End If
        stopGL = False
        frmMain.need_screen_update()
        GC.Collect()
        GC.WaitForFullGCComplete()
        sfd.Dispose()
        GC.Collect()
        GC.WaitForFullGCComplete()
        Me.Hide()
    End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		Me.Hide()
	End Sub
End Class