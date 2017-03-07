Imports System.Windows
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Web
Imports Tao.OpenGl
Imports Tao.Platform.Windows
Imports Tao.FreeGlut
Imports Tao.FreeGlut.Glut
Imports Microsoft.VisualBasic.Strings
Imports System.Math
Imports System.Object
Imports System.Threading
Imports System.Data
Imports Tao.DevIl
Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.ComponentModel
Imports Hjg.Pngcs

Imports Ionic.Zip
Imports Ionic.BZip2
'
Imports Ionic

Module modOpenGL


	Public Sub EnableOpenGL()
        frmMain.pb2.Visible = False
		frmMain.pb2.SendToBack()
		pb1_hDC = User.GetDC(frmMain.pb1.Handle)
        pb2_hDC = User.GetDC(frmMain.pb2.Handle)
        pb3_hDC = User.GetDC(frmTestView.pb3.Handle)
        Dim pfd As Gdi.PIXELFORMATDESCRIPTOR
		Dim PixelFormat As Integer

		'ZeroMemory(pfd, Len(pfd))
		pfd.nSize = Len(pfd)
		pfd.nVersion = 1
        pfd.dwFlags = Gdi.PFD_DRAW_TO_WINDOW Or Gdi.PFD_SUPPORT_OPENGL Or Gdi.PFD_DOUBLEBUFFER Or Gdi.PFD_GENERIC_ACCELERATED
		pfd.iPixelType = Gdi.PFD_TYPE_RGBA
		pfd.cColorBits = 32
		pfd.cDepthBits = 32
        pfd.cStencilBits = 16
		pfd.cAlphaBits = 8
		pfd.iLayerType = Gdi.PFD_MAIN_PLANE

		PixelFormat = Gdi.ChoosePixelFormat(pb1_hDC, pfd)
        PixelFormat = Gdi.ChoosePixelFormat(pb2_hDC, pfd)
        PixelFormat = Gdi.ChoosePixelFormat(pb3_hDC, pfd)
        If PixelFormat = 0 Then
            MessageBox.Show("Unable to retrieve pixel format")
            End
        End If
		If Not (Gdi.SetPixelFormat(pb1_hDC, PixelFormat, pfd)) Then
			MessageBox.Show("Unable to set pixel format")
			End
		End If
        If Not (Gdi.SetPixelFormat(pb2_hDC, PixelFormat, pfd)) Then
            MessageBox.Show("Unable to set pixel format")
            End
        End If
        If Not (Gdi.SetPixelFormat(pb3_hDC, PixelFormat, pfd)) Then
            MessageBox.Show("Unable to set pixel format")
            End
        End If
        pb1_hRC = Wgl.wglCreateContext(pb1_hDC)
        pb2_hRC = Wgl.wglCreateContext(pb2_hDC)
        pb3_hRC = Wgl.wglCreateContext(pb3_hDC)
        If pb1_hRC.ToInt32 = 0 Then
            MessageBox.Show("Unable to get rendering context pb1")
            End
        End If
        If pb2_hRC.ToInt32 = 0 Then
            MessageBox.Show("Unable to get rendering context pb2")
            End
        End If
        If pb3_hRC.ToInt32 = 0 Then
            MessageBox.Show("Unable to get rendering context pb3")
            End
        End If
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If

        Glut.glutInit()
		Glut.glutInitDisplayMode(GLUT_RGBA Or GLUT_DOUBLE Or GLUT_MULTISAMPLE)

		Gl.glViewport(0, 0, frmMain.pb1.Width, frmMain.pb1.Height)
        Dim e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e).ToString
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If

		Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
		Gl.glEnable(Gl.GL_COLOR_MATERIAL)
		Gl.glEnable(Gl.GL_LIGHT0)
		Gl.glEnable(Gl.GL_LIGHTING)
		gl_set_lights()
        Wgl.wglShareLists(pb1_hRC, pb2_hRC)
        Wgl.wglShareLists(pb1_hRC, pb3_hRC)
        ' build_shaders()
        e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e).ToString
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If
        'make_post_FBO_and_Textures()
        create_decal_FBO()
        'noise_map_id = Load_DDS_File(Application.StartupPath + "\Resources\noise.dds")
        Gl.glGetFloatv(Gl.GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT, largestAnsio)
        '-----------------------------------
		'next line creates the FBOs and Textures needed to shadow map. Still under development.
		'frmMain.create_shadow_render_texture()
		frmMain.pb2.Visible = False
		Gl.glGetIntegerv(Gl.GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS, max_texture_units)
    End Sub



	Public Sub gl_set_lights()
		'lighting

		'Debug.WriteLine("GL Error A:" + Gl.glGetError().ToString)
		''Gl.glEnable(Gl.GL_SMOOTH)
		''Gl.glShadeModel(Gl.GL_SMOOTH)
		'Debug.WriteLine("GL Error B:" + Gl.glGetError().ToString)

		Dim specReflection() As Single = {0.4F, 0.4F, 0.4F, 1.0F}
		Dim specular() As Single = {0.4F, 0.4F, 0.4F, 1.0F}
		Dim emission() As Single = {0.0F, 0.0F, 0.0F, 1.0F}
		Dim ambient() As Single = {0.8F, 0.8F, 0.8F, 1.0F}
		Dim global_ambient() As Single = {0.9F, 0.9F, 0.9F, 1.0F}
		Dim diffuseLight() As Single = {0.99, 0.99, 0.99, 1.0F}

		Dim mcolor() As Single = {0.9F, 0.9F, 0.9F, 1.0F}
		'Gl.glEnable(Gl.GL_SMOOTH)
		Gl.glShadeModel(Gl.GL_SMOOTH)

		Gl.glEnable(Gl.GL_LIGHT0)
		'Gl.glEnable(Gl.GL_LIGHT1)
		'Gl.glEnable(Gl.GL_LIGHT2)
		'Gl.glEnable(Gl.GL_LIGHT3)
		'Gl.glEnable(Gl.GL_LIGHT4)
		Gl.glEnable(Gl.GL_LIGHTING)

		'light 0
		Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, global_ambient)
		Dim position0() As Single = {0.0F, 300.0F, 0.0F, 1.0F}
		Dim position1() As Single = {400.0F, 100.0F, 400.0F, 1.0F}
		Dim position2() As Single = {400.0F, 100.0F, -400.0F, 1.0F}
		Dim position3() As Single = {-400.0F, 100.0F, -400.0F, 1.0F}
		Dim position4() As Single = {-400.0F, 100.0F, 400.0F, 1.0F}

		' light 1

		'Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position0)
		'Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, specular)
		'Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_EMISSION, emission)
		'Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, diffuseLight)
		'Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, ambient)

		'Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, position1)
		'Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_SPECULAR, specular)
		'Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_EMISSION, emission)
		'Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, diffuseLight)
		'Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT, ambient)


		'Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, position2)
		'Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_SPECULAR, specular)
		'Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_EMISSION, emission)
		'Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_DIFFUSE, diffuseLight)
		'Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_AMBIENT, ambient)


		'Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_POSITION, position3)
		'Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_SPECULAR, specular)
		'Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_EMISSION, emission)
		'Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_DIFFUSE, diffuseLight)
		'Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_AMBIENT, ambient)


		'Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_POSITION, position4)
		'Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_SPECULAR, specular)
		'Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_EMISSION, emission)
		'Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_DIFFUSE, diffuseLight)
		'Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_AMBIENT, ambient)




		Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, global_ambient)


		Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)

		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE, mcolor)
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, specReflection)
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, diffuseLight)
		Gl.glColorMaterial(Gl.GL_FRONT, Gl.GL_SPECULAR Or Gl.GL_AMBIENT_AND_DIFFUSE)


		Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, 60)
		Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST)
		Gl.glEnable(Gl.GL_COLOR_MATERIAL)
		Gl.glLightModeli(Gl.GL_LIGHT_MODEL_TWO_SIDE, Gl.GL_FALSE)

		'Gl.glFrontFace(Gl.GL_CCW)
		Gl.glClearDepth(1.0F)
		Gl.glEnable(Gl.GL_DEPTH_TEST)
		Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_LOCAL_VIEWER, 0.0F)
		Gl.glEnable(Gl.GL_NORMALIZE)


	End Sub
	Public Sub DisableOpenGL()
        Gl.glDeleteRenderbuffersEXT(1, fboID)
        Gl.glDeleteRenderbuffersEXT(1, PostFBO_ID)
        Wgl.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero)
		Wgl.wglDeleteContext(pb1_hRC)
		Wgl.wglDeleteContext(pb2_hRC)

	End Sub

	Public Sub ResizeGL()
		Gl.glViewport(0, 0, frmMain.pb1.Width, frmMain.pb1.Height)
        'Gl.glMatrixMode(Gl.GL_PROJECTION) ' Select The Projection Matrix
        'Gl.glLoadIdentity() ' Reset The Projection Matrix

        '' Calculate The Aspect Ratio Of The Window
        'Glu.gluPerspective(70.0F, CSng((frmMain.pb1.Width) / (frmMain.pb1.Height)), 0.02F, Far_Clip)

        'Gl.glMatrixMode(Gl.GL_MODELVIEW)	' Select The Modelview Matrix
        'Gl.glLoadIdentity() ' Reset The Modelview Matrix

	End Sub
	Public Sub ResizeGL_2()
        Gl.glViewport(0, 0, frmMain.pb2.Width, frmMain.pb2.Height)
    End Sub
	Public Sub ResizeGL_mini()
		frmMain.pb2.Parent = frmMain.pb1
		Dim w = frmMain.pb1.Width / frmMain.pb1.Height
		w = 1.0
		frmMain.pb2.Width = 480
		frmMain.pb2.Height = 240
		frmMain.pb2.BringToFront()
		frmMain.pb2.Visible = True
		frmMain.pb2.BackColor = Color.Blue
		frmMain.pb2.Location = New Point(frmMain.pb1.Width - frmMain.pb2.Width, frmMain.pb1.Height - frmMain.pb2.Height)

		Gl.glViewport(0, 0, frmMain.pb2.Width, frmMain.pb2.Height)
		'Gl.glMatrixMode(Gl.GL_PROJECTION) ' Select The Projection Matrix
		'Gl.glLoadIdentity() ' Reset The Projection Matrix

		'' Calculate The Aspect Ratio Of The Window
		'Glu.gluPerspective(70.0F, CSng((frmMain.pb2.Width) / (frmMain.pb2.Height)), 0.02F, Far_Clip)

		'Gl.glMatrixMode(Gl.GL_MODELVIEW)	' Select The Modelview Matrix
		'Gl.glLoadIdentity() ' Reset The Modelview Matrix

	End Sub
	Public Sub glutPrint(ByVal x As Single, ByVal y As Single, _
ByVal text As String, ByVal r As Single, ByVal g As Single, ByVal b As Single, ByVal a As Single)

		Try
			If text.Length = 0 Then Exit Sub
		Catch ex As Exception
			Return
		End Try
		Dim blending As Boolean = False
        If Gl.glIsEnabled(Gl.GL_BLEND) Then blending = True
		Gl.glEnable(Gl.GL_BLEND)
		Gl.glColor3f(r, g, b)
		Gl.glRasterPos2f(x, y)
		For Each I In text

			Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_8_BY_13, Asc(I))

		Next
		If Not blending Then Gl.glDisable(Gl.GL_BLEND)
	End Sub
	Public Sub glutPrintBox(ByVal x As Single, ByVal y As Single, _
ByVal text As String, ByVal r As Single, ByVal g As Single, ByVal b As Single, ByVal a As Single)

		Try
			If text.Length = 0 Then Exit Sub
		Catch ex As Exception
			Return
		End Try
		Dim blending As Boolean = False
		If Gl.glIsEnabled(Gl.GL_BLEND) Then blending = True
		Gl.glEnable(Gl.GL_BLEND)
		Gl.glColor4f(0, 0, 0, 0.7)
		Gl.glBegin(Gl.GL_QUADS)
		Dim L1 = text.Length * 8
		Dim l2 = 7
		Gl.glVertex2f(x - 2, y - l2 + 2)
		Gl.glVertex2f(x + L1 + 2, y - l2 + 2)
		Gl.glVertex2f(x + L1 + 2, y + l2 + 5)
		Gl.glVertex2f(x - 2, y + l2 + 5)
		Gl.glEnd()
		Gl.glColor3f(r, g, b)
		Gl.glRasterPos2f(x, y)
		For Each I In text

			Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_8_BY_13, Asc(I))

		Next
		If Not blending Then Gl.glDisable(Gl.GL_BLEND)
	End Sub

	Public Sub ViewOrtho_2()
		Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
		Gl.glLoadIdentity() 'Reset The Matrix
		Gl.glOrtho(0, frmMain.pb2.Width, -frmMain.pb2.Height, 0, -200.0, 100.0)	'Select Ortho Mode
		Gl.glMatrixMode(Gl.GL_MODELVIEW)	'Select Modelview Matrix
		Gl.glLoadIdentity() 'Reset The Matrix
	End Sub



    Public Sub ViewOrtho()
        ResizeGL()
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glOrtho(0, frmMain.pb1.Width, -frmMain.pb1.Height, 0, 3000.0, -3000.0) 'Select Ortho Mode
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
        Gl.glLoadIdentity() 'Reset The Matrix
    End Sub
    Public Sub ViewOrtho_map()
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glOrtho(0, frmMain.pb1.Width, frmMain.pb1.Height, 0, -200.0, 100.0) 'Select Ortho Mode
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
        Gl.glLoadIdentity() 'Reset The Matrix
    End Sub
    Public Sub ViewPerspective_2()
        ' Set Up A Perspective View

        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity()

        Glu.gluPerspective(60.0F, CSng((frmMain.pb2.Width) / (frmMain.pb2.Height)), 1.0F, 2500)
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDepthMask(Gl.GL_TRUE)
        Gl.glDepthRange(0.0, 1.0)
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview
        Gl.glLoadIdentity() 'Reset The Matrix
    End Sub
    Public Sub ViewPerspective_d()
        ' Set Up A Perspective View
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity()

        If frmMain.m_Orbit_Light.Checked Then
            Glu.gluPerspective(60.0F, CSng((frmMain.pb1.Width) / (frmMain.pb1.Height)), 1.01F, 5000)
        Else
            Glu.gluPerspective(60.0F, CSng((frmMain.pb1.Width) / (frmMain.pb1.Height)), 1.01F, 5000)
        End If
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        'Gl.glDepthMask(Gl.GL_TRUE)
        Gl.glDepthRange(0.0, 1.0)
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position)
        frmMain.set_eyes()
    End Sub
    Public Sub ViewPerspective()
        ' Set Up A Perspective View
        Gl.glLoadIdentity()

        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity()
        If testing Then
            Glu.gluPerspective(60.0F, CSng((frmMain.pb1.Width) / (frmMain.pb1.Height)), 1.0F, 4000.0)
        Else

            If frmMain.m_Orbit_Light.Checked Then
                Glu.gluPerspective(60.0F, CSng((frmMain.pb1.Width) / (frmMain.pb1.Height)), 1.0F, 6500)
            Else
                Glu.gluPerspective(60.0F, CSng((frmMain.pb1.Width) / (frmMain.pb1.Height)), 1.0F, Far_Clip)
            End If

        End If
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        'Gl.glDepthMask(Gl.GL_TRUE)
        Gl.glDepthRange(0.0, 1.0)
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position) 'must be set before projection matrix is set
        frmMain.set_eyes()
    End Sub

End Module
