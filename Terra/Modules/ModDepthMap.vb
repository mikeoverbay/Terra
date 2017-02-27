Imports System.IO
Imports System.Math
Imports System
Imports Tao.OpenGl

Module ModDepthMap
    Public post_map_width As Integer
    Public post_map_height As Integer
    Public post_color_image_id As Integer = -1
    Public post_depth_image_id As Integer = -1
    Public post_3D_image_id As Integer = -1
    Public PostdepthID As Integer = 0
    Public PostFBO_ID As Integer = -1
    Public MVP_MAT(16) As Single
    Public MV_MAT(16) As Single
    Public Sub copy_to_post_map()
        If maploaded Then

            Gl.glReadBuffer(Gl.GL_BACK) 'Ensure we are reading from the back buffer.
            Gl.glActiveTexture(0)

            '===========================================================================
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, post_color_image_id)
            Gl.glCopyTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, 0, 0, post_map_width, post_map_height, 0)
            '===========================================================================
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, post_depth_image_id)
            Gl.glCopyTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_DEPTH_COMPONENT, 0, 0, post_map_width, post_map_height, 0)
            '===========================================================================
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, PostFBO_ID)
            attach_texture_to_FBO(post_3D_image_id)

            ResizeGL()
            ViewPerspective()
            render_depths()
            attach_texture_to_FBO(0)
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)

        End If
    End Sub
    Public Sub draw_post_test()
        If Not maploaded Then
            Return
        End If
        Dim width = post_map_width
        Dim height = post_map_height
        '=================
        'draw window frame

        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glColor3f(0.3, 0.1, 0.1)
        Gl.glBegin(Gl.GL_QUADS)
        '---

        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex3f(0, 0, 0.0)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex3f(width, 0, 0.0)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex3f(width, -height, 0.0)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(0, -height, 0.0)
        Gl.glEnd()

        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glColor3f(0.5, 0.5, 0.5)

        Gl.glUseProgram(shader_list.fog_shader)
        Dim loc_color = Gl.glGetUniformLocation(shader_list.fog_shader, "map_color")
        Gl.glUniform1i(loc_color, 0)
        Dim loc_depth = Gl.glGetUniformLocation(shader_list.fog_shader, "map_depth")
        Gl.glUniform1i(loc_depth, 1)
        'Dim loc_3D = Gl.glGetUniformLocation(shader_list.fog_shader, "map_3D")
        'Gl.glUniform1i(loc_3D, 2)
        'Dim loc_light = Gl.glGetUniformLocation(shader_list.fog_shader, "light_pos")
        'Gl.glUniform3f(loc_light, position(0), position(1), position(2))
        Dim fog_loc = Gl.glGetUniformLocation(shader_list.fog_shader, "fog_level")
        Gl.glUniform1f(fog_loc, 1.0 - frmLighting.s_fog_level.Value / 100.0)

        'Dim width_loc = Gl.glGetUniformLocation(shader_list.fog_shader, "width")
        'Gl.glUniform1f(width_loc, frmMain.pb1.Width)
        'Dim heigth_loc = Gl.glGetUniformLocation(shader_list.fog_shader, "height")
        'Gl.glUniform1f(heigth_loc, frmMain.pb1.Height)

        'Dim MVPM_loc = Gl.glGetUniformLocation(shader_list.fog_shader, "MVP_MAT")
        'Gl.glUniformMatrix4fv(MVPM_loc, 1, 0, MVP_MAT)
        'Dim MVM_loc = Gl.glGetUniformLocation(shader_list.fog_shader, "MV_MAT")
        'Gl.glUniformMatrix4fv(MVM_loc, 1, 0, MV_MAT)



        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, post_color_image_id)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, post_depth_image_id)
        'Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        'Gl.glBindTexture(Gl.GL_TEXTURE_2D, post_3D_image_id)

        Gl.glBegin(Gl.GL_QUADS)
        '---

        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex3f(0, 0, 0.0)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex3f(width, 0, 0.0)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex3f(width, -height, 0.0)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(0, -height, 0.0)
        Gl.glEnd()
        Gl.glUseProgram(0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)



    End Sub
    Public Sub render_depths()
        Dim loc_map = Gl.glGetUniformLocation(shader_list.write3D_shader, "map")
        Dim flag = Gl.glGetUniformLocation(shader_list.write3D_shader, "flag")

        Gl.glUseProgram(shader_list.write3D_shader)
        Gl.glUniform1i(loc_map, 0)


        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glEnable(Gl.GL_DEPTH_TEST)

        Gl.glColorMask(1, 1, 1, 0)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glUniform1i(flag, 1)

        'draw terrain
        For i = 0 To test_count
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(i).colorMapId)
            Gl.glCallList(maplist(i).calllist_Id)
            Gl.glCallList(maplist(i).seamCallId)

        Next
        'draw models
        For model As UInt32 = 0 To Models.matrix.Length - 1
            For k = 0 To Models.models(model)._count - 1
                Gl.glPushMatrix()
                Gl.glMultMatrixf(Models.matrix(model).matrix)
                Gl.glCallList(Models.models(model).componets(k).callList_ID)
                Gl.glPopMatrix()
            Next
        Next
        'draw trees 
        GoTo f_this
        If maploaded And frmMain.m_show_trees.Checked _
           And Not frmMain.m_hell_mode.Checked Then
            If Trees.flora IsNot Nothing Then

                ' Dim map_loc As Integer = Gl.glGetUniformLocation(alpha_shader, "map")
                'Gl.glUseProgram(alpha_shader)
                ' Gl.glUniform1i(map_loc, 0)

                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
                Gl.glDisable(Gl.GL_CULL_FACE)
                Gl.glActiveTexture(Gl.GL_TEXTURE0)


                Dim rad As Single
                For mode = 0 To 1

                    If mode = 1 Then
                        Gl.glDisable(Gl.GL_CULL_FACE)
                    End If
                    'Dim draw As Boolean = True
                    Dim t_cut_off As Single = 300000
                    For i As UInt32 = 0 To speedtree_matrix_list.Length - 2
                        Gl.glPushMatrix()
                        Gl.glMultMatrixf(Trees.matrix(i).matrix)
                        Dim ll As vect3
                        ll.x = Trees.matrix(i).matrix(12)
                        ll.y = Trees.matrix(i).matrix(13)
                        ll.z = Trees.matrix(i).matrix(14)
                        ll.x -= eyeX
                        ll.y -= eyeY
                        ll.z -= eyeZ
                        rad = (ll.x ^ 2) + (ll.y ^ 2) + (ll.z ^ 2)


                        'rad = 10
                        If frmMain.m_low_quality_trees.Checked Then
                            rad = 200001
                        End If
                        'If rad > 300000 Then
                        'draw = True
                        If rad > t_cut_off And mode = 0 Then
                            'draw = False
                            Gl.glEnable(Gl.GL_CULL_FACE)
                            Gl.glActiveTexture(Gl.GL_TEXTURE0)
                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, speedtree_imageID)
                            Gl.glCallList(Trees.flora(i).billboard_displayID)
                        Else
                            Gl.glDisable(Gl.GL_CULL_FACE)
                            If mode = 0 Then
                                If Trees.flora(i).branch_displayID > 0 Then
                                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, Trees.flora(i).branch_textureID)
                                    Gl.glCallList(Trees.flora(i).branch_displayID)
                                Else
                                End If
                                If Trees.flora(i).frond_displayID > 0 Then
                                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, speedtree_imageID)
                                    Gl.glCallList(Trees.flora(i).frond_displayID)
                                End If
                            Else
                                If rad <= t_cut_off Then
                                    If Trees.flora(i).leaf_displayID > 0 Then
                                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, speedtree_imageID)
                                        Gl.glCallList(Trees.flora(i).leaf_displayID)
                                    End If
                                End If
                            End If
                        End If
                        Gl.glPopMatrix()
                    Next
                Next

            End If
        End If
f_this:
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glColorMask(0, 0, 0, 1)
        Gl.glUniform1i(flag, 0)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, noise_map_id)
        Dim l, r, t, b As Single
        l = map_x_min * 2
        r = map_x_max * 2
        t = map_y_max * 2
        b = map_y_min * 2
        Gl.glBegin(Gl.GL_QUADS)
        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex3f(l, 0.0, t)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex3f(r, 0.0, t)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex3f(r, 0.0, b)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(l, 0.0, b)
        Gl.glEnd()
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        Gl.glDisable(Gl.GL_BLEND)
        Gl.glColorMask(1, 1, 1, 1)
        Gl.glUseProgram(0)

    End Sub


    Public Function make_post_FBO_and_Textures() As Boolean
        Dim e = Gl.glGetError
        'constant errors I cant find are being generated.
        'I think Tao opengl has many issues with this.
        If e <> 0 Then
            'Dim s = Glu.gluErrorString(e).ToString
            'Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            'MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If
        post_map_width = frmMain.pb1.Width
        post_map_height = frmMain.pb1.Height
        'ensure we are powers of 2
        post_map_width += post_map_width And 1
        post_map_height += post_map_height And 1

        Gl.glDeleteTextures(1, post_color_image_id)
        Gl.glFinish() ' make sure its actually deleted
        post_color_image_id = Make_post_texture(post_map_width, post_map_height)
        Gl.glFinish()

        Gl.glDeleteTextures(1, post_depth_image_id)
        Gl.glFinish()
        post_depth_image_id = Make_post_texture(post_map_width, post_map_height)
        Gl.glFinish()

        Gl.glDeleteTextures(1, post_3D_image_id)
        Gl.glFinish()
        post_3D_image_id = Make_post_texture(post_map_width, post_map_height)
        Gl.glFinish()

        create_FBO_post()
        Gl.glFinish()
    End Function
    Private Function Make_post_texture(ByVal width As Integer, ByVal height As Integer) As Integer
        'use for shadow mapping opjects
        'creates an empty texture ready for writing to
        Dim Id As Integer
        '1st texture target
        Dim er = Gl.glGetError

        Gl.glGenTextures(1, Id)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, Id)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)

        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA32F_ARB, width, height, 0, Gl.GL_RGBA, Gl.GL_FLOAT, Nothing)
        'Gl.glGenerateMipmapEXT(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Dim e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e).ToString
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If
        Return Id

    End Function
    Public Sub create_FBO_post()
        Dim max_size As Single
        Gl.glGetFloatv(Gl.GL_MAX_TEXTURE_SIZE, max_size)
        If PostFBO_ID > 0 Then
            Gl.glDeleteFramebuffersEXT(1, PostFBO_ID)
            Gl.glFinish()
        End If
        'Dim drawbuffs() = {Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_NONE}
        ' utility_texture = Make_Depth_texture(decal_depth_size)
        Dim er = Gl.glGetError
        ' create a framebuffer object
        '-------------------------------------------------------------------------------
        ' make first FBO
        '-------------------------------------------------------------------------------
        Gl.glGenFramebuffersEXT(1, PostFBO_ID)
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, PostFBO_ID)
        'er = Gl.glGetError
        '-------------------------------------------------------------------------------
        ' create a depth buffer
        Gl.glGenRenderbuffersEXT(1, PostdepthID)
        'er = Gl.glGetError
        Gl.glBindRenderbufferEXT(Gl.GL_RENDERBUFFER_EXT, PostdepthID)
        Gl.glRenderbufferStorageEXT(Gl.GL_RENDERBUFFER_EXT, Gl.GL_DEPTH_COMPONENT, post_map_width, post_map_height)
        'er = Gl.glGetError
        Gl.glFramebufferRenderbufferEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_ATTACHMENT_EXT, Gl.GL_RENDERBUFFER_EXT, PostdepthID)
        er = Gl.glGetError


        Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, post_color_image_id, 0)
        'er = Gl.glGetError
        'Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_TEXTURE_2D, coMapID2, 0)
        'er = Gl.glGetError
        'Gl.glFramebufferRenderbufferEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_ATTACHMENT_EXT, Gl.GL_RENDERBUFFER_EXT, depthID)
        er = Gl.glGetError

        If er <> 0 Then
            Dim s = Glu.gluErrorString(E).ToString
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If
        '-------------------------------------------------------------------------------
        Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, 0, 0)

        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
    End Sub

End Module
