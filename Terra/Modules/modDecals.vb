
Imports System.IO
Imports System.Math
Imports System
Module modDecals
    Public decal_cache() As decal_
    Public Structure decal_
        Public name As String
        Public normal_name As String
        Public texture_id As Integer
        Public normal_id As Integer
    End Structure


    Public Sub make_new_decals()
        ReDim decal_cache(0)
        decal_cache(0) = New decal_
        For k = 0 To decal_matrix_list.Length - 1
            decal_matrix_list(k).good = True
            If decal_matrix_list(k).decal_texture Is Nothing Then
                decal_matrix_list(k).good = False
                GoTo skipthis
            End If
            If decal_matrix_list(k).decal_texture.Contains("Shadow_true") Then
                decal_matrix_list(k).good = False
                GoTo skipthis
            End If
            'Gl.glPushMatrix()


            decal_matrix_list(k).matrix(1) *= -1.0
            decal_matrix_list(k).matrix(2) *= -1.0
            decal_matrix_list(k).matrix(4) *= -1.0
            decal_matrix_list(k).matrix(8) *= -1.0
            decal_matrix_list(k).matrix(12) *= -1.0
            'Dim id As Integer
            'id = Gl.glGenLists(1)
            'Gl.glNewList(id, Gl.GL_COMPILE)
            get_box_corners(k)
            'now that we have the transforms,, we can figure out which chunks this decal touches.
            decal_matrix_list(k).display_id = Gl.glGenLists(1)
            Gl.glNewList(decal_matrix_list(k).display_id, Gl.GL_COMPILE)
            Gl.glBegin(Gl.GL_QUADS)
            make_decal_box(k)
            Gl.glEnd()
            Gl.glEndList()
            get_decal_textures(k)

skipthis:
        Next


    End Sub
    Private Sub make_decal_box(ByVal decal As Integer)
        With decal_matrix_list(decal)
            '1
            Gl.glVertex3f(.lbr.x, .lbr.y, .lbr.z)
            Gl.glVertex3f(.ltr.x, .ltr.y, .ltr.z)
            Gl.glVertex3f(.rtr.x, .rtr.y, .rtr.z)
            Gl.glVertex3f(.rbr.x, .rbr.y, .rbr.z)
            '2
            Gl.glVertex3f(.lbl.x, .lbl.y, .lbl.z)
            Gl.glVertex3f(.ltl.x, .ltl.y, .ltl.z)
            Gl.glVertex3f(.ltr.x, .ltr.y, .ltr.z)
            Gl.glVertex3f(.lbr.x, .lbr.y, .lbr.z)
            '3
            Gl.glVertex3f(.rbl.x, .rbl.y, .rbl.z)
            Gl.glVertex3f(.rtl.x, .rtl.y, .rtl.z)
            Gl.glVertex3f(.ltl.x, .ltl.y, .ltl.z)
            Gl.glVertex3f(.lbl.x, .lbl.y, .lbl.z)
            '4
            Gl.glVertex3f(.rbr.x, .rbr.y, .rbr.z)
            Gl.glVertex3f(.rtr.x, .rtr.y, .rtr.z)
            Gl.glVertex3f(.rtl.x, .rtl.y, .rtl.z)
            Gl.glVertex3f(.rbl.x, .rbl.y, .rbl.z)
            '5
            Gl.glVertex3f(.rtr.x, .rtr.y, .rtr.z)
            Gl.glVertex3f(.ltr.x, .ltr.y, .ltr.z)
            Gl.glVertex3f(.ltl.x, .ltl.y, .ltl.z)
            Gl.glVertex3f(.rtl.x, .rtl.y, .rtl.z)
            '6
            Gl.glVertex3f(.rbl.x, .rbl.y, .rbl.z)
            Gl.glVertex3f(.lbl.x, .lbl.y, .lbl.z)
            Gl.glVertex3f(.lbr.x, .lbr.y, .lbr.z)
            Gl.glVertex3f(.rbr.x, .rbr.y, .rbr.z)


        End With

    End Sub
    Private Sub get_box_corners(ByVal decal As Integer)
        With decal_matrix_list(decal)
            ReDim .BB(8)
            ' left side -----------
            .lbl.x = -0.5 'left bottom left
            .lbl.y = -0.5
            .lbl.z = -0.5
            .BB(0) = .lbl
            '
            .lbr.x = 0.5 ' left bottom right
            .lbr.y = -0.5
            .lbr.z = -0.5
            .BB(1) = .lbr
            '
            .ltl.x = -0.5 'left top left
            .ltl.y = 0.5
            .ltl.z = -0.5
            .BB(2) = .ltl
            '
            .ltr.x = 0.5 ' left top right
            .ltr.y = 0.5
            .ltr.z = -0.5
            .BB(3) = .ltr
            ' right side ----------
            .rbl.x = -0.5 ' right bottom left
            .rbl.y = -0.5
            .rbl.z = 0.5
            .BB(4) = .rbl
            '
            .rbr.x = 0.5 ' right bottom right
            .rbr.y = -0.5
            .rbr.z = 0.5
            .BB(5) = .rbr
            '
            .rtl.x = -0.5 ' right top left
            .rtl.y = 0.5
            .rtl.z = 0.5
            .BB(6) = .rtl
            '
            .rtr.x = 0.5 ' right top right
            .rtr.y = 0.5
            .rtr.z = 0.5
            .BB(7) = .rtr

        End With
        For i = 0 To 7
            decal_matrix_list(decal).BB(i) = translate_to(decal_matrix_list(decal).BB(i), decal_matrix_list(decal).matrix)
        Next
    End Sub

    Private Sub get_decal_textures(ByVal k As Integer)
        If Not decal_matrix_list(k).good Then Return
        If get_decal_diffuse(k) Then
            Return ' found this in cache or not at all
        Else
            'added a new diffuse so we need the normal map
            get_decal_normal_texture(k)
        End If
        '--------------------------------------------------------------
        GC.Collect()
        Return
    End Sub
    Private Function get_decal_diffuse(ByVal k As Integer)
        Dim name = decal_matrix_list(k).decal_texture
        For i = 0 To decal_cache.Length - 1
            If name = decal_cache(i).name Then
                decal_matrix_list(k).texture_id = decal_cache(i).texture_id
                decal_matrix_list(k).normal_id = decal_cache(i).normal_id
                Return True
            End If
        Next
        Dim ms As New MemoryStream
        Dim name_e As Ionic.Zip.ZipEntry = active_pkg(name)
        If name_e Is Nothing Then
            name_e = get_shared(name)
            If name_e Is Nothing Then
                Return True
            End If
        End If
        name_e.Extract(ms)
        decal_matrix_list(k).texture_id = get_texture(ms, frmMain.m_low_quality_textures.Checked)
        ms.Dispose()

        frmMapInfo.I__Decal_Textures_tb.Text += "Color: " + name + vbCrLf
        'add to cache
        Dim d = decal_cache.Length - 1
        ReDim Preserve decal_cache(d + 1)
        decal_cache(d).name = name
        decal_cache(d).texture_id = decal_matrix_list(k).texture_id
        Return False
    End Function
    Private Sub get_decal_normal_texture(ByVal k As Integer)
        Dim norm = decal_matrix_list(k).decal_normal
        Dim name = decal_matrix_list(k).decal_texture
        If norm.Contains("Stone06_") Then
            decal_matrix_list(k).normal_id = Load_DDS_File(Application.StartupPath + "\Resources\Stone06_NM.dds")
            GoTo saveit
        End If
        Dim norm_e As Ionic.Zip.ZipEntry = active_pkg(norm)
        Dim ms2 As New MemoryStream
        If norm_e Is Nothing Then
            norm_e = get_shared(norm)
            If norm_e Is Nothing Then
                Return
            End If
        End If
        norm_e.Extract(ms2)
        decal_matrix_list(k).normal_id = get_texture(ms2, frmMain.m_low_quality_textures.Checked)
        ms2.Dispose()
saveit:
        frmMapInfo.I__Decal_Textures_tb.Text += "Normal: " + norm + vbCrLf
        'find matching texture and save the gl texture normal_id
        For i = 0 To decal_cache.Length - 1
            If name = decal_cache(i).name Then
                decal_cache(i).normal_id = decal_matrix_list(k).normal_id
                decal_cache(i).normal_name = norm
                Return
            End If
        Next

    End Sub

    Public Function rotate_only(ByVal v As vect3, ByVal m() As Single) As vect3
        Dim vo As vect3
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)

        Return vo

    End Function
    Public Function translate_to(ByVal v As vect3, ByVal m() As Single) As vect3
        Dim vo As vect3
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)

        vo.x += m(12)
        vo.y += m(13)
        vo.z += m(14)
        Return vo

    End Function
    Private Function transform(ByRef m() As Single, ByVal v As vertex_data, ByRef scale As Single, ByRef k As Integer) As vertex_data
        Dim vo As vertex_data
        v.x *= scale
        v.y *= scale
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)

        vo.u = v.u
        vo.v = v.v * -1.0

        vo.x += m(12)
        vo.y += m(13)
        vo.z += m(14)

        Return vo
    End Function
    Private Function rotate_decal_view(ByVal m() As Single) As vect3
        Dim vo As vect3
        Dim v As vect3
        v.x = 0.0
        v.y = 1.0
        v.z = 0.0
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)
        Dim l = Sqrt((vo.x ^ 2) + (vo.y ^ 2) + (vo.z ^ 2))
        If l = 0.0 Then l = 1.0
        vo.x /= l
        vo.y /= l
        vo.z /= l

        Return vo
    End Function

  
End Module
