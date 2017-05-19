Imports System.IO
Imports System.Math
Imports System
Imports Tao.OpenGl
Imports Tao.FreeGlut

Module modDeferred
    Public G_Buffer As New GBuffer_
    Public ssaoFBO, gBufferFBO, gBufferNormalFBO, gBufferColorFBO, gBufferPositionFBO, ssaoColorBuffer As Integer
    Public gPosition, gNormal, gColor As Integer
    Public gDepth As Integer
    Public gDepthTexture As Integer
    Public NoiseTexture As Integer
    Public gFlag As Integer
    Public noise(16 * 4) As Single
    Public randomFloats(64 * 3) As Single
    Public Class GBuffer_
        'Private color_normal_buffers_only() As Integer = {Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_COLOR_ATTACHMENT2_EXT}
        Private color_buffer_only() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT}
        Private attacments() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_COLOR_ATTACHMENT2_EXT, Gl.GL_COLOR_ATTACHMENT3_EXT}
        Private attacments_CandP() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_COLOR_ATTACHMENT2_EXT}
        Private attacments_CandPandN() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_COLOR_ATTACHMENT2_EXT}
        Private attacments_CandNandF() As Integer = {Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_COLOR_ATTACHMENT2_EXT, Gl.GL_COLOR_ATTACHMENT3_EXT}
        Public Sub shut_down()
            delete_textures_and_fob_objects()
        End Sub
        Public Sub make_kernel()
            Dim ran As New Random

            For i = 0 To (64 * 3) - 1 Step 3

                randomFloats(i + 0) = CSng(ran.NextDouble * 2.0 - 1.0)
                randomFloats(i + 1) = CSng(ran.NextDouble * 2.0 - 1.0)
                randomFloats(i + 2) = CSng(ran.NextDouble)

                Dim scale As Single = CSng(i) / 64.0!
                scale = lerp(0.1, 1.0, scale * scale)
                randomFloats(i + 0) *= scale
                randomFloats(i + 1) *= scale
                randomFloats(i + 2) *= scale
                'normalize to unit vector length
                Dim ln = Sqrt((randomFloats(i + 0) ^ 2) + (randomFloats(i + 1) ^ 2) + (randomFloats(i + 2) ^ 2))
                If ln = 0.0 Then ln = 1.0
                randomFloats(i + 0) /= ln
                randomFloats(i + 1) /= ln
                randomFloats(i + 2) /= ln

            Next
        End Sub
        Private Function lerp(ByVal a As Single, ByVal b As Single, ByVal f As Single)
            Return a + f * (b - a)
        End Function
        Private Sub delete_textures_and_fob_objects()
            Dim e As Integer
            If gFlag > 0 Then
                Gl.glDeleteTextures(1, gFlag)
            End If
            If gDepthTexture > 0 Then
                Gl.glDeleteTextures(1, gDepthTexture)
                e = Gl.glGetError
            End If
            If gPosition > 0 Then
                Gl.glDeleteTextures(1, gPosition)
                e = Gl.glGetError
            End If
            If gNormal > 0 Then
                Gl.glDeleteTextures(1, gNormal)
                e = Gl.glGetError
            End If
            If gColor > 0 Then
                Gl.glDeleteTextures(1, gColor)
                e = Gl.glGetError
            End If
            If ssaoColorBuffer > 0 Then
                Gl.glDeleteTextures(1, ssaoColorBuffer)
                e = Gl.glGetError
            End If
            If gDepth > 0 Then
                Gl.glDeleteRenderbuffersEXT(1, gDepth)
                e = Gl.glGetError
            End If
            If gBufferFBO > 0 Then
                Gl.glDeleteFramebuffersEXT(1, gBufferFBO)
                e = Gl.glGetError
            End If
            If ssaoFBO > 0 Then
                Gl.glDeleteFramebuffersEXT(1, ssaoFBO)
                e = Gl.glGetError
            End If
        End Sub
        Public Sub getsize(ByRef w As Integer, ByRef h As Integer)
            frmMain.pb1.Width = frmMain.ClientSize.Width
            frmMain.pb1.Height = frmMain.ClientSize.Height - frmMain.mainMenu.Height
            frmMain.pb1.Location = New System.Drawing.Point(0, frmMain.mainMenu.Height + 1)
            Dim w1, h1 As Integer
            w1 = frmMain.pb1.Width
            h1 = frmMain.pb1.Height
            w = w1 + (w1 Mod 1)
            h = h1 + (h1 Mod 1)
        End Sub
        Private Sub create_textures()
            Dim SCR_WIDTH, SCR_HEIGHT As Integer
            getsize(SCR_WIDTH, SCR_HEIGHT)
            'depth buffer

            If NoiseTexture = 0 Then
                make_kernel()
                Dim rnd As New Random
                For i = 0 To (16 * 3) - 1 Step 3
                    noise(i + 0) = CSng(rnd.NextDouble * 2.0 - 1.0)
                    noise(i + 1) = CSng(rnd.NextDouble * 2.0 - 1.0)
                    noise(i + 2) = 0.0
                Next
                Gl.glGenTextures(1, NoiseTexture)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, NoiseTexture)
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB16F_ARB, 4, 4, 0, Gl.GL_RGB, Gl.GL_FLOAT, noise)
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)

            End If
            Dim e1 = Gl.glGetError

            Gl.glGenTextures(1, gFlag)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gFlag)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_LUMINANCE8, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_LUMINANCE, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)

            Dim e2 = Gl.glGetError
            Gl.glGenTextures(1, gDepthTexture)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gDepthTexture)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_LUMINANCE16F_ARB, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_LUMINANCE, Gl.GL_FLOAT, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_COMPARE_MODE, Gl.GL_NONE)

            ' - Position color buffer
            Gl.glGenTextures(1, gPosition)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gPosition)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB16F_ARB, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGB, Gl.GL_FLOAT, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)

            ' - Normal + Specular color buffer
            Gl.glGenTextures(1, gNormal)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gNormal)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGBA, Gl.GL_FLOAT, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)

            ' - Color color buffer
            Gl.glGenTextures(1, gColor)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            Dim e3 = Gl.glGetError

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        End Sub
        Public Sub attachFOBtextures()
            ' Gl.glBindTexture(Gl.GL_TEXTURE_2D, gPosition)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glDrawBuffers(4, attacments)
            'Dim er = Gl.glGetError
        End Sub
        Public Sub attach_color_and_postion_only()
            Gl.glDrawBuffers(2, attacments_CandP)
        End Sub
        Public Sub attach_flag_only()
            Dim d_buffers() As Integer = {Gl.GL_COLOR_ATTACHMENT2_EXT, Gl.GL_COLOR_ATTACHMENT3_EXT}
            Gl.glDrawBuffers(2, d_buffers)
        End Sub
        Public Sub attach_color_and_postion_and_normal_only()
            Gl.glDrawBuffers(3, attacments_CandPandN)
        End Sub
        Public Sub attachShadowTexture()
            Gl.glDrawBuffers(1, color_buffer_only)
        End Sub
        Public Sub detachShadowTexture()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, 0, 0)
        End Sub

        Public Sub detachFBOtextures()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, 0, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_TEXTURE_2D, 0, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT2_EXT, Gl.GL_TEXTURE_2D, 0, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT3_EXT, Gl.GL_TEXTURE_2D, 0, 0)
        End Sub
        Public Sub attach_color_only()
            Gl.glDrawBuffers(1, color_buffer_only)
        End Sub
        Public Function init() As Boolean
            stopGL = True
            Threading.Thread.Sleep(50)
            Dim SCR_WIDTH, SCR_HEIGHT As Integer
            getsize(SCR_WIDTH, SCR_HEIGHT)

            Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, 0)
            Dim e1 = Gl.glGetError

            delete_textures_and_fob_objects()
            'Create the gBuffer textures
            create_textures()
            Dim e2 = Gl.glGetError

            'Create the FBO
            Gl.glGenFramebuffersEXT(1, gBufferFBO)
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, gBufferFBO)
            Dim e3 = Gl.glGetError

            Gl.glGenRenderbuffersEXT(1, gDepth)
            Gl.glBindRenderbufferEXT(Gl.GL_RENDERBUFFER_EXT, gDepth)
            Gl.glRenderbufferStorageEXT(Gl.GL_RENDERBUFFER_EXT, Gl.GL_DEPTH_COMPONENT24, SCR_WIDTH, SCR_HEIGHT)
            Gl.glFramebufferRenderbufferEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_ATTACHMENT_EXT, Gl.GL_RENDERBUFFER_EXT, gDepth)
            Dim e4 = Gl.glGetError


            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_TEXTURE_2D, gNormal, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT2_EXT, Gl.GL_TEXTURE_2D, gPosition, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT3_EXT, Gl.GL_TEXTURE_2D, gFlag, 0)
            Dim e5 = Gl.glGetError

            'attach draw buffers
            Gl.glDrawBuffers(4, attacments)

            'attach draw buffers
            Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)

            If Status <> Gl.GL_FRAMEBUFFER_COMPLETE_EXT Then
                MsgBox("Failed to create Deferred FBO", MsgBoxStyle.Critical, "Not good!")
                Return False
            End If


            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
            stopGL = False
            Return True
        End Function
 
        Public Sub get_depth_buffer(ByVal w As Integer, ByVal h As Integer)
            Dim e1 = Gl.glGetError
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gDepthTexture)
            Gl.glCopyTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_DEPTH_COMPONENT24, 0, 0, w, h, 0)
            Dim e2 = Gl.glGetError
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        End Sub

    End Class


    Private Sub SetLightView()

    End Sub

 

End Module
