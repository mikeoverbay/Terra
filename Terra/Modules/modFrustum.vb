
Imports System.Math

Module modFrustum
    Public frustum(6, 4) As Single

    Public culled_count As Integer
    Public Sub check_terrain_visible()
        For i = 0 To test_count
            maplist(i).visible = CubeInFrustum(maplist(i).BB)
        Next
    End Sub

    Public Sub check_decals_visible()
        For i = 0 To decal_matrix_list.Length - 1
            If decal_matrix_list(i).good Then
                decal_matrix_list(i).visible = CubeInFrustum(decal_matrix_list(i).BB)
            End If
        Next
    End Sub
    Public Sub check_models_visible()
        For model As UInt32 = 0 To Models.matrix.Length - 2
            Models.models(model).visible = CubeInFrustum(Model_Matrix_list(model).BB)
        Next
    End Sub
    Public Sub check_trees_visible()
        For t = 0 To treeCache.Length - 2
            For tree As UInt32 = 0 To treeCache(t).tree_cnt - 1
                treeCache(t).BB(tree).visible = CubeInFrustum(treeCache(t).BB(tree).BB)
            Next
        Next
    End Sub

    Public Sub ExtractFrustum()
        culled_count = 0
        Dim proj(16) As Single
        Dim modl(16) As Single
        Dim clip(16) As Single
        Dim t As Single

        ' Get the current PROJECTION matrix from OpenGL 
        Gl.glGetFloatv(Gl.GL_PROJECTION_MATRIX, proj)

        ' Get the current MODELVIEW matrix from OpenGL 
        Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, modl)

        ' Combine the two matrices (multiply projection by modelview) 
        clip(0) = modl(0) * proj(0) + modl(1) * proj(4) + modl(2) * proj(8) + modl(3) * proj(12)
        clip(1) = modl(0) * proj(1) + modl(1) * proj(5) + modl(2) * proj(9) + modl(3) * proj(13)
        clip(2) = modl(0) * proj(2) + modl(1) * proj(6) + modl(2) * proj(10) + modl(3) * proj(14)
        clip(3) = modl(0) * proj(3) + modl(1) * proj(7) + modl(2) * proj(11) + modl(3) * proj(15)

        clip(4) = modl(4) * proj(0) + modl(5) * proj(4) + modl(6) * proj(8) + modl(7) * proj(12)
        clip(5) = modl(4) * proj(1) + modl(5) * proj(5) + modl(6) * proj(9) + modl(7) * proj(13)
        clip(6) = modl(4) * proj(2) + modl(5) * proj(6) + modl(6) * proj(10) + modl(7) * proj(14)
        clip(7) = modl(4) * proj(3) + modl(5) * proj(7) + modl(6) * proj(11) + modl(7) * proj(15)

        clip(8) = modl(8) * proj(0) + modl(9) * proj(4) + modl(10) * proj(8) + modl(11) * proj(12)
        clip(9) = modl(8) * proj(1) + modl(9) * proj(5) + modl(10) * proj(9) + modl(11) * proj(13)
        clip(10) = modl(8) * proj(2) + modl(9) * proj(6) + modl(10) * proj(10) + modl(11) * proj(14)
        clip(11) = modl(8) * proj(3) + modl(9) * proj(7) + modl(10) * proj(11) + modl(11) * proj(15)

        clip(12) = modl(12) * proj(0) + modl(13) * proj(4) + modl(14) * proj(8) + modl(15) * proj(12)
        clip(13) = modl(12) * proj(1) + modl(13) * proj(5) + modl(14) * proj(9) + modl(15) * proj(13)
        clip(14) = modl(12) * proj(2) + modl(13) * proj(6) + modl(14) * proj(10) + modl(15) * proj(14)
        clip(15) = modl(12) * proj(3) + modl(13) * proj(7) + modl(14) * proj(11) + modl(15) * proj(15)

        ' Extract the numbers for the RIGHT plane 
        frustum(0, 0) = clip(3) - clip(0)
        frustum(0, 1) = clip(7) - clip(4)
        frustum(0, 2) = clip(11) - clip(8)
        frustum(0, 3) = clip(15) - clip(12)

        ' Normalize the result 
        t = Sqrt(frustum(0, 0) * frustum(0, 0) + frustum(0, 1) * frustum(0, 1) + frustum(0, 2) * frustum(0, 2))
        frustum(0, 0) /= t
        frustum(0, 1) /= t
        frustum(0, 2) /= t
        frustum(0, 3) /= t

        ' Extract the numbers for the LEFT plane 
        frustum(1, 0) = clip(3) + clip(0)
        frustum(1, 1) = clip(7) + clip(4)
        frustum(1, 2) = clip(11) + clip(8)
        frustum(1, 3) = clip(15) + clip(12)

        ' Normalize the result 
        t = Sqrt(frustum(1, 0) * frustum(1, 0) + frustum(1, 1) * frustum(1, 1) + frustum(1, 2) * frustum(1, 2))
        frustum(1, 0) /= t
        frustum(1, 1) /= t
        frustum(1, 2) /= t
        frustum(1, 3) /= t

        ' Extract the BOTTOM plane 
        frustum(2, 0) = clip(3) + clip(1)
        frustum(2, 1) = clip(7) + clip(5)
        frustum(2, 2) = clip(11) + clip(9)
        frustum(2, 3) = clip(15) + clip(13)

        ' Normalize the result 
        t = Sqrt(frustum(2, 0) * frustum(2, 0) + frustum(2, 1) * frustum(2, 1) + frustum(2, 2) * frustum(2, 2))
        frustum(2, 0) /= t
        frustum(2, 1) /= t
        frustum(2, 2) /= t
        frustum(2, 3) /= t

        ' Extract the TOP plane 
        frustum(3, 0) = clip(3) - clip(1)
        frustum(3, 1) = clip(7) - clip(5)
        frustum(3, 2) = clip(11) - clip(9)
        frustum(3, 3) = clip(15) - clip(13)

        ' Normalize the result 
        t = Sqrt(frustum(3, 0) * frustum(3, 0) + frustum(3, 1) * frustum(3, 1) + frustum(3, 2) * frustum(3, 2))
        frustum(3, 0) /= t
        frustum(3, 1) /= t
        frustum(3, 2) /= t
        frustum(3, 3) /= t

        ' Extract the FAR plane 
        frustum(4, 0) = clip(3) - clip(2)
        frustum(4, 1) = clip(7) - clip(6)
        frustum(4, 2) = clip(11) - clip(10)
        frustum(4, 3) = clip(15) - clip(14)

        ' Normalize the result 
        t = Sqrt(frustum(4, 0) * frustum(4, 0) + frustum(4, 1) * frustum(4, 1) + frustum(4, 2) * frustum(4, 2))
        frustum(4, 0) /= t
        frustum(4, 1) /= t
        frustum(4, 2) /= t
        frustum(4, 3) /= t

        ' Extract the NEAR plane 
        frustum(5, 0) = clip(3) + clip(2)
        frustum(5, 1) = clip(7) + clip(6)
        frustum(5, 2) = clip(11) + clip(10)
        frustum(5, 3) = clip(15) + clip(14)

        ' Normalize the result 
        t = Sqrt(frustum(5, 0) * frustum(5, 0) + frustum(5, 1) * frustum(5, 1) + frustum(5, 2) * frustum(5, 2))
        frustum(5, 0) /= t
        frustum(5, 1) /= t
        frustum(5, 2) /= t
        frustum(5, 3) /= t
        'tb1.text = ""
        Return
        For i = 0 To 5
            tb1.text += frustum(i, 0).ToString("000.0000") + "   " + frustum(i, 1).ToString("000.0000") + "   " + frustum(i, 2).ToString("000.0000") + "   " + frustum(i, 3).ToString("000.0000") + vbCrLf
        Next


    End Sub

    Public Function CubeInFrustum(ByRef bb() As vect3) As Boolean
        If bb Is Nothing Then
            Return False
        End If
        For p = 0 To 5
            If (frustum(p, 0) * (bb(0).x) + frustum(p, 1) * (bb(0).y) + frustum(p, 2) * (bb(0).z) + frustum(p, 3) > 0) Then
                Continue For
            End If
            If (frustum(p, 0) * (bb(1).x) + frustum(p, 1) * (bb(1).y) + frustum(p, 2) * (bb(1).z) + frustum(p, 3) > 0) Then
                Continue For
            End If
            If (frustum(p, 0) * (bb(2).x) + frustum(p, 1) * (bb(2).y) + frustum(p, 2) * (bb(2).z) + frustum(p, 3) > 0) Then
                Continue For
            End If
            If (frustum(p, 0) * (bb(3).x) + frustum(p, 1) * (bb(3).y) + frustum(p, 2) * (bb(3).z) + frustum(p, 3) > 0) Then
                Continue For
            End If
            If (frustum(p, 0) * (bb(4).x) + frustum(p, 1) * (bb(4).y) + frustum(p, 2) * (bb(4).z) + frustum(p, 3) > 0) Then
                Continue For
            End If
            If (frustum(p, 0) * (bb(5).x) + frustum(p, 1) * (bb(5).y) + frustum(p, 2) * (bb(5).z) + frustum(p, 3) > 0) Then
                Continue For
            End If
            If (frustum(p, 0) * (bb(6).x) + frustum(p, 1) * (bb(6).y) + frustum(p, 2) * (bb(6).z) + frustum(p, 3) > 0) Then
                Continue For
            End If
            If (frustum(p, 0) * (bb(7).x) + frustum(p, 1) * (bb(7).y) + frustum(p, 2) * (bb(7).z) + frustum(p, 3) > 0) Then
                Continue For
            End If
            culled_count += 1
            Return False
        Next
        Return True
    End Function

End Module
