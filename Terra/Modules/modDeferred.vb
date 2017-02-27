Imports System.IO
Imports System.Math
Imports System
Imports Tao.OpenGl
Imports Tao.FreeGlut
'=========================================================================
' So far I cant use this.
' I can't figure out a way to deal with the normal mixing of the terrain
'=========================================================================

Module modDeferred
    Public GBUFFER As GBuffer_
    Public Class GBuffer_
        Enum GBUFFER_TEXTURE_TYPE As Integer
            GBUFFER_TEXTURE_TYPE_POSITION
            GBUFFER_TEXTURE_TYPE_DIFFUSE
            GBUFFER_TEXTURE_TYPE_NORMAL
            GBUFFER_TEXTURE_TYPE_TEXCOORD
            GBUFFER_NUM_TEXTURES
        End Enum

        Public Function init(windowWidth As Integer, windowHeight As Integer) As Boolean

            'Create the FBO
            Gl.glGenFramebuffersEXT(1, deferred_fob)
            Gl.glBindFramebufferEXT(Gl.GL_DRAW_BUFFER, deferred_fob)
            'Create the gBuffer textures
            Gl.glGenTextures(defTextures.Length - 1, defTextures)
            Gl.glGenTextures(1, defDepthtexture)

            'Setup textures and attach to color_frambuffers
            For i = 0 To defTextures.Length - 1
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, defTextures(i))
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA32F_ARB, windowWidth, windowHeight, 0, Gl.GL_RGB, Gl.GL_FLOAT, 0)
                Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT + i, Gl.GL_TEXTURE_2D, defTextures(i), 0)
            Next

            'Attach depthtexture
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, defDepthtexture)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_DEPTH_COMPONENT32, windowWidth, windowHeight, 0, Gl.GL_RGB, Gl.GL_FLOAT, 0)
            Dim DrawBuffers() As Integer = {Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_ATTACHMENT_EXT, Gl.GL_COLOR_ATTACHMENT2_EXT, Gl.GL_COLOR_ATTACHMENT3_EXT}

            'attach draw buffers
            Gl.glDrawBuffers(drawbuffer0.Length - 1, DrawBuffers)
            Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)

            If Status <> Gl.GL_FRAMEBUFFER_COMPLETE_EXT Then
                MsgBox("Failed to create Deferred FBO", MsgBoxStyle.Critical, "Not good!")
                Return False
            End If

            'resture fbo to stock
            Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, 0)
            Return True
        End Function

        Private deferred_fob As UInteger
        Private defTextures(GBUFFER_TEXTURE_TYPE.GBUFFER_NUM_TEXTURES) As UInteger
        Private defDepthtexture As UInteger
    End Class


    Private Sub SetLightView()

    End Sub

    Public Sub make_shadow_map()
        If Not maploaded Then
            Return
        End If
        'Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, fboID)
        attach_texture_to_FBO(coMapID)
        'Gl.glDrawBuffers(2, drawbuffer0(0))
        'check status
        Dim er = Gl.glGetError
        'ResizeGL()
        'lightTransform()
        'lightTransform_preview()
        Gl.glColorMask(Gl.GL_TRUE, Gl.GL_TRUE, Gl.GL_TRUE, Gl.GL_TRUE)
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        'Gl.glColorMask(Gl.GL_FALSE, Gl.GL_FALSE, Gl.GL_FALSE, Gl.GL_FALSE)
        Gl.glDepthFunc(Gl.GL_LEQUAL)
        Gl.glFrontFace(Gl.GL_CW)
        Gl.glCullFace(Gl.GL_FRONT)
        Gl.glLineWidth(1)
        'ViewPerspective()
        'terra
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        'Gl.glEnable(Gl.GL_LIGHTING)
        Gl.glColor4f(1.0, 1.0, 1.0, 0.0)
        Gl.glCullFace(Gl.GL_FRONT)
        Gl.glDisable(Gl.GL_CULL_FACE)

        ' Gl.glUseProgram(depth_shader)
        Gl.glUseProgram(shader_list.depth_shader)
        For i = 0 To test_count
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(i).colorMapId)
            Gl.glCallList(maplist(i).calllist_Id)
            Gl.glCallList(maplist(i).seamCallId)

        Next
        Gl.glCullFace(Gl.GL_FRONT)
        'models
        For model As UInt32 = 0 To Models.matrix.Length - 1
            For k = 0 To Models.models(model)._count - 1
                Gl.glPushMatrix()
                Gl.glMultMatrixf(Models.matrix(model).matrix)
                Gl.glCallList(Models.models(model).componets(k).callList_ID)
                Gl.glPopMatrix()
            Next
        Next

        'trees
        'For map = 0 To test_count
        '    If maplist(map).flora IsNot Nothing Then
        '        For i As UInt32 = 0 To maplist(map).flora_count
        '            Dim mapL As vect3 = maplist(map).location
        '            Gl.glPushMatrix()
        '            Gl.glTranslatef(mapL.x + 50, mapL.z, mapL.y - 50)

        '            'Gl.glDisable(Gl.GL_CULL_FACE)
        '            If maplist(map).flora(i).branch_displayID > 0 Then
        '                Gl.glCallList(maplist(map).flora(i).branch_displayID)
        '            Else
        '            End If
        '            If maplist(map).flora(i).frond_displayID > 0 Then
        '                Gl.glCallList(maplist(map).flora(i).frond_displayID)
        '            End If
        '            If maplist(map).flora(i).leaf_displayID > 0 Then
        '                Gl.glCallList(maplist(map).flora(i).leaf_displayID)
        '            End If

        '            Gl.glPopMatrix()
        '        Next

        '    End If
        'Next
        ' must save the matrix!
        Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, MV)
        Gl.glGetFloatv(Gl.GL_PROJECTION_MATRIX, lightProject)

        Gl.glMatrixMode(Gl.GL_TEXTURE)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)

        Gl.glLoadIdentity()
        Gl.glLoadMatrixf(bias)

        ' concatating all matrices into one.
        Gl.glMultMatrixf(lightProject)
        Gl.glMultMatrixf(MV)
        'Gl.glPopMatrix()
        ' Go back to normal matrix mode
        Gl.glMatrixMode(Gl.GL_MODELVIEW)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        er = Gl.glGetError
        Gl.glUseProgram(0)
        blur_shadow(0)
        'Gl.glBindTexture(Gl.GL_TEXTURE_2D, coMapID)
        'Gl.glGenerateMipmapEXT(Gl.GL_TEXTURE_2D) ' this makes no sense
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        er = Gl.glGetError
        'attache_texture(0)
        ' switch back to window-system-provided framebuffer
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        Gl.glDrawBuffer(Gl.GL_BACK)
        'Gdi.SwapBuffers(pb1_hDC)
        'Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        'Gdi.SwapBuffers(pb1_hDC)
        'Thread.Sleep(5)
        'Gdi.SwapBuffers(pb1_hDC)
        'Gl.glPushMatrix()

    End Sub
    Public Sub blur_shadow(ByVal texture_id As Integer)

        'Dim uc As vect2
        'Dim lc As vect2
        Dim comap, si As Integer
        'frmMain.pb2.Location = New Point(0, 0)
        'frmMain.pb2.BringToFront()
        'frmMain.pb2.Visible = True

        'lc.x = smrs
        'lc.y = -smrs    ' top to bottom is negitive ' may wanna change this!
        'uc.x = 0.0
        'uc.y = 0.0

        '---------------------------------------------------------
        '1st render/blur vert coMapID to coMapID2
        attach_texture_to_FBO(coMapID2)
        Dim e5 = Gl.glGetError

        Gl.glViewport(0, 0, smrs, smrs)
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Matrix
        Dim e3 = Gl.glGetError
        Gl.glOrtho(0, smrs, -smrs, 0.0, 100.0, -100.0)    'Select Ortho Mode
        Dim e2 = Gl.glGetError
        ' TODO: use create a real furstum for this!
        'Gl.glOrtho(Frustum(4).x, Frustum(5).x, Frustum(0).y, Frustum(6).y, -1500, 1500.0) 'Select Ortho Mode
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
        Gl.glLoadIdentity() 'Reset The Matrix


        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)



        comap = Gl.glGetUniformLocation(shader_list.gaussian_shader, "s_texture")
        si = Gl.glGetUniformLocation(shader_list.gaussian_shader, "blurScale")
        Gl.glUseProgram(shader_list.gaussian_shader)
        'set switch
        Gl.glUniform3f(si, 1.0 / smrs, 0.0, 0.0)

        Gl.glUniform1i(comap, 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture_id)
        'Gl.glBindTexture(Gl.GL_TEXTURE_2D, minimap_textureid)
        Dim e = Gl.glGetError

        Dim gridS = smrs
        Gl.glBegin(Gl.GL_QUADS)
        '---
        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex3f(-0, 0, 0.0)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex3f(gridS, 0, 0.0)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex3f(gridS, -gridS, 0.0)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(-0, -gridS, 0.0)
        Gl.glEnd()
        'Gl.glUseProgram(0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        e = Gl.glGetError
        'Gdi.SwapBuffers(pb1_hDC)
        '-------------------------------------------------------------
        ' 2nd. horzonal. render/blur horz coMapID2 in to coMapID

        attach_texture_to_FBO(texture_id)
        'attache_texture(0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)


        Gl.glUseProgram(shader_list.gaussian_shader)
        'set switch
        Gl.glUniform3f(si, 0.0, 1.0 \ smrs, 0.0)


        Gl.glUniform1i(comap, 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, utility_texture)
        e = Gl.glGetError

        Gl.glBegin(Gl.GL_QUADS)
        '---
        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex3f(-0, 0, 0.0)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex3f(gridS, 0, 0.0)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex3f(gridS, -gridS, 0.0)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(-0, -gridS, 0.0)
        Gl.glEnd()

        Gl.glUseProgram(0)

        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        Gl.glDrawBuffer(Gl.GL_BACK)
        '--------------------

        'frmMain.pb2.Visible = False
        'frmMain.pb2.SendToBack()

    End Sub


End Module
