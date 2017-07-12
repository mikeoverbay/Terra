#Region "imports"
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
Imports System.Windows.Media.Media3D
'Imports System.Windows.Media.Media3D
Imports Ionic.Zip
Imports Ionic.BZip2
'
Imports Ionic
#End Region



Module modWater
    Public Sub make_water_mesh()
        'todo: crete water mesh for FFT water testing
        Dim w = 0.0 ' water.position.y
        Dim cell_count As Double = 10.0
        Dim p0, p1, p2, p3 As vect3
        Dim v_space As Double = 1.0 / (64.0 * cell_count)
        'we can use the vertex in the shader directly for UV coordinates
        'before mutiplying it by the waters transform matrix.
        If water.displayID_plane > 0 Then
            Gl.glDeleteLists(water.displayID_plane, 1)
        End If
        water.displayID_plane = Gl.glGenLists(1)
        Gl.glNewList(water.displayID_plane, Gl.GL_COMPILE)
        Gl.glBegin(Gl.GL_TRIANGLES)
        For z = 0 To 1 - v_space Step v_space
            For x = 0 To 1 - v_space Step v_space
                p0.x = x
                p0.z = z
                p0.y = w
                '-----------
                p1.x = x
                p1.z = z + v_space
                p1.y = w
                '-----------
                p2.x = x + v_space
                p2.z = z + v_space
                p2.y = w
                '-----------
                p3.x = x + v_space
                p3.z = z
                p3.y = w
                '-----------
                '1
                Gl.glVertex3f(p0.x - 0.5, p0.y, p0.z - 0.5)
                Gl.glVertex3f(p1.x - 0.5, p1.y, p1.z - 0.5)
                Gl.glVertex3f(p2.x - 0.5, p2.y, p2.z - 0.5)
                '2
                Gl.glVertex3f(p0.x - 0.5, p0.y, p0.z - 0.5)
                Gl.glVertex3f(p2.x - 0.5, p2.y, p2.z - 0.5)
                Gl.glVertex3f(p3.x - 0.5, p3.y, p3.z - 0.5)

            Next x
        Next z
        Gl.glEnd()
        Gl.glEndList()
    End Sub
End Module
