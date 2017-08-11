#Region "Imports"

Imports System
Imports System.Windows
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Text
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
Imports System.Net.Sockets
Imports System.Data
Imports Tao.DevIl
Imports Tao.DevIl.Ilu
Imports Tao.DevIl.Ilut
Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Globalization
Imports System.Windows.Media.Media3D
'Imports ICSharpCode.SharpZipLib.Zip.Compression.Streams
'Imports ICSharpCode.SharpZipLib.Zip.Compression
Imports Hjg.Pngcs
Imports System.Drawing.FontFamily
Imports Ionic.Zip
Imports Ionic.BZip2
'
Imports Ionic
'Imports MySql.Data
Imports System.Windows.Forms.Design
#End Region


Module Mod_X_file
    Private Structure _indice
        Public a, b, c As Integer
    End Structure
    Public Function read_directX_model(file_ As String) As Integer
        'reads single object directX ASCII file.
        ' IN: path and name of file to load
        ' OUT: Display List ID.
        'At some point this will load multi model files.
        Dim start_locations(1) As UInteger
        Dim obj_count As Integer = get_start_locations(start_locations, file_)
        Dim s As New StreamReader(file_)
        Dim size = s.BaseStream.Length
        Dim position As ULong = 0
        Dim txt As String = ""
        Dim list_ID = Gl.glGenLists(1)
        Gl.glNewList(list_ID, Gl.GL_COMPILE)
        Gl.glBegin(Gl.GL_TRIANGLES)
loop_me:
        s.BaseStream.Position = position
        While Not txt.ToLower.Contains("mesh")
            txt = s.ReadLine
        End While
        txt = s.ReadLine ' this should be the number of vertices
        Dim brk = txt.Split(";")
        Dim vertice_count = CInt(brk(0))
        Dim vertices() As vect3
        Dim indices() As _indice
        Dim normals() As vect3
        '	Dim uvs() As vec2
        ReDim vertices(vertice_count)
        Dim scale As Single = 5.0
        For i = 0 To vertice_count - 1
            vertices(i) = New vect3
            txt = s.ReadLine
            brk = txt.Split(";")
            vertices(i).x = CSng(brk(0)) * scale
            vertices(i).y = CSng(brk(1)) * scale
            vertices(i).z = CSng(brk(2)) * scale
        Next
        txt = s.ReadLine ' this should be a blank line
        txt = s.ReadLine ' this should be the indice count for the vertices
        brk = txt.Split(";")
        Dim indice_count As Int32 = 0
        indice_count = CInt(brk(0))
        ReDim indices(indice_count)
        For i = 0 To indice_count - 1
            indices(i) = New _indice
            txt = s.ReadLine
            brk = txt.Split(";")
            brk = brk(1).Split(",")
            indices(i).a = CInt(brk(0))
            indices(i).b = CInt(brk(1))
            indices(i).c = CInt(brk(2))
        Next
        ' get normals
        s.Close()
        s = New StreamReader(file_)
        s.BaseStream.Position = position
        While Not txt.ToLower.Contains("meshnormals")
            txt = s.ReadLine
        End While
        txt = s.ReadLine ' this should be the normal count
        brk = txt.Split(";")
        Dim normal_count As Int32
        normal_count = CInt(brk(0))
        ReDim normals(normal_count)
        For i = 0 To normal_count - 1
            normals(i) = New vect3
            txt = s.ReadLine
            brk = txt.Split(";")
            normals(i).x = CSng(brk(0))
            normals(i).y = CSng(brk(1))
            normals(i).z = CSng(brk(2))
        Next
        s.BaseStream.Position = 0
        txt = s.ReadLine ' this should be the texture coordinate count
        While Not txt.ToLower.Contains("meshtexturecoords")
            txt = s.ReadLine
        End While
        txt = s.ReadLine ' this should be the texture coordinate count
        brk = txt.Split(";")
        Dim txt_coord_cnt As Int32
        txt_coord_cnt = CInt(brk(0))
        Dim uvs(1) As vect2
        ReDim uvs(txt_coord_cnt)
        For i = 0 To txt_coord_cnt - 1
            uvs(i) = New vect2
            txt = s.ReadLine
            brk = txt.Split(";")
            uvs(i).x = CSng(brk(0))
            uvs(i).y = CSng(brk(1))
        Next
        position = s.BaseStream.Position
        s.Close()
        ' at this point, we have all the data to make the mesh
        'Gen Display List ID.
        Dim a, b, c As Integer
        'create all the triangles.
        For i = 0 To indice_count
            a = indices(i).a
            b = indices(i).b
            c = indices(i).c
            Gl.glNormal3f(normals(a).x, normals(a).y, normals(a).z)
            Gl.glTexCoord2f(uvs(a).x, uvs(a).y)
            Gl.glVertex3f(vertices(a).x, vertices(a).y, vertices(a).z)

            Gl.glNormal3f(normals(b).x, normals(b).y, normals(b).z)
            Gl.glTexCoord2f(uvs(b).x, uvs(b).y)
            Gl.glVertex3f(vertices(b).x, vertices(b).y, vertices(b).z)

            Gl.glNormal3f(normals(c).x, normals(c).y, normals(c).z)
            Gl.glTexCoord2f(uvs(c).x, uvs(c).y)
            Gl.glVertex3f(vertices(c).x, vertices(c).y, vertices(c).z)
        Next
        obj_count -= 1
        If obj_count > 0 Then
            s = New StreamReader(file_)
            s.BaseStream.Position = position
            GoTo loop_me
        End If
        Gl.glEnd()
        Gl.glEndList()
        Return list_ID
    End Function
    Public Function get_start_locations(ByRef loc() As UInteger, ByRef file_ As String)
        Dim m_count As Integer = 0
        Dim c_pos As UInteger = 0
        Dim txt As String = ""
        Dim s As New StreamReader(file_)
        While Not s.EndOfStream
            c_pos = s.BaseStream.Position
            txt = s.ReadLine
            If txt.ToLower.Contains("mesh ") Then
                ReDim Preserve loc(m_count + 1)
                loc(m_count) = c_pos
                m_count += 1
            End If
        End While
        Return m_count
    End Function

End Module
