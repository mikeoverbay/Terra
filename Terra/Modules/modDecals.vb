
Imports System.IO
Imports System.Math
Imports System
Module modDecals
    Public top_end As Integer = 0
    Public model_too As Boolean
    Public decal_cache() As decal_
    Public TERRAIN_BIAS As Single
    Public DECAL_BIAS As Single
    Public EXCLUDED
    Public default_terrain_bias As Single = 2.0
    Public default_decal_bias As Single = 1.0
    Public Structure decal_
        Public name As String
        Public normal_name As String
        Public texture_id As Integer
        Public normal_id As Integer
    End Structure

    Public Sub lightTransform(decal As Integer, depth_flag As Boolean)
        'special projection for decals.
        'this projection is aligined to the decals face.
        Dim c_ As vect3 = decal_matrix_list(decal).cam_pos
        Dim l_ As vect3 = decal_matrix_list(decal).look_at
        Dim tl = decal_matrix_list(decal).top_left
        Dim tr = decal_matrix_list(decal).top_right
        Dim bl = decal_matrix_list(decal).bot_left
        Dim br = decal_matrix_list(decal).bot_right
        Dim c_rot = decal_matrix_list(decal).cam_rotation
        Dim c_pos = decal_matrix_list(decal).cam_location
        Dim near = decal_matrix_list(decal).near_clip
        Dim far = decal_matrix_list(decal).far_clip
        Gl.glViewport(0, 0, decal_grid_size, decal_grid_size)     'Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glDisable(Gl.GL_LIGHTING)
        Dim scale As Single = 1.0 'pb1.Height / pb1.Width
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
        Gl.glLoadIdentity() 'Reset The Matrix

        Glu.gluLookAt(c_.x, c_.y, c_.z, l_.x, l_.y, l_.z, c_rot.x, c_rot.y, c_rot.z)
        Dim m(16) As Single
        Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, m)
        tl = translate_to(tl, m)
        tr = translate_to(tr, m)
        bl = translate_to(bl, m)
        br = translate_to(br, m)
        c_pos = translate_to(c_pos, m)
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Mat4rix
        With decal_matrix_list(decal)
            If depth_flag Then 'true is for terrain
                Gl.glOrtho(br.x, tl.x, bl.y, tr.y, -Abs(get_length_vect3(near) * .t_bias), Abs(get_length_vect3(far) * .t_bias)) 'Select Ortho Mode
            Else
                Gl.glOrtho(br.x, tl.x, bl.y, tr.y, -Abs(get_length_vect3(near) * .d_bias), Abs(get_length_vect3(far) * .d_bias)) 'Select Ortho Mode

            End If
            Glu.gluLookAt(c_.x, c_.y, c_.z, l_.x, l_.y, l_.z, c_rot.x, c_rot.y, c_rot.z)
            Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
            Gl.glLoadIdentity() 'Reset The Matrix
        End With

    End Sub
    Public Sub mask_makers(ByVal decal As Integer)
        Gl.glColor3f(0.0, 0.0, 0.0)
        model_too = False
        If frmMain.m_show_models.Checked And Not decal_matrix_list(decal).exclude Then 'If excluded, we dont want models affecting the decals geo.
            Gl.glColor3f(0, 0.0, 0.0)
            For model As UInt32 = 0 To Models.matrix.Length - 1
                For k = 0 To Models.models(model)._count - 1
                    If Model_Matrix_list(model).mask Then
                        model_too = True
                        Gl.glPushMatrix()
                        Gl.glMultMatrixf(Models.matrix(model).matrix)
                        Gl.glCallList(Models.models(model).componets(k).callList_ID)
                        Gl.glPopMatrix()
                    End If
                Next
            Next
        End If
        With decal_matrix_list(decal)
            If decal_chunks(decal).chunk_count > 0 Then
                For k = 0 To decal_chunks(decal).chunk_count - 1
                    Gl.glCallList(decal_chunks(decal).chunk_Ids(k))
                    Gl.glCallList(decal_chunks(decal).seam_Ids(k))
                Next
            End If
        End With
    End Sub
    Public Sub draw_terrain(ByVal decal As Integer)
        With decal_matrix_list(decal)
            If decal_chunks(decal).chunk_count > 0 Then
                For k = 0 To decal_chunks(decal).chunk_count - 1
                    Gl.glCallList(decal_chunks(decal).chunk_Ids(k))
                    Gl.glCallList(decal_chunks(decal).seam_Ids(k))
                Next
            End If
        End With
    End Sub
    Public Sub draw_models(ByVal decal As Integer)
        If frmMain.m_show_models.Checked Then
            If Not decal_matrix_list(decal).exclude Then 'if excluded, we dont want models affecting the decals geo.
                For model As UInt32 = 0 To Models.matrix.Length - 1
                    For k = 0 To Models.models(model)._count - 1
                        If Model_Matrix_list(model).mask Then
                            Gl.glPushMatrix()
                            Gl.glMultMatrixf(Models.matrix(model).matrix)
                            Gl.glCallList(Models.models(model).componets(k).callList_ID)
                            Gl.glPopMatrix()
                        End If
                    Next
                Next
            End If
        End If
    End Sub
    Public Sub remake_Decal(decal As Integer)
        'used to update a decal in realtime using the bias tool.
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, fboID)
        Gl.glPushMatrix()
        make_decal_depthMap(decal)
        Gl.glPopMatrix()
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        Gl.glDrawBuffer(Gl.GL_BACK)

    End Sub
    Public Sub attach_texture_to_FBO(ByVal textureID As Integer)
        'attaches the texture we render to.
        Gl.glActiveTexture(Gl.GL_TEXTURE0) 'ensure we are using texture 0
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureID)
        Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, textureID, 0)

    End Sub

    Public Sub make_decal_depthMap(decal)
        'This makes projects the decal over the models and terrain and fits the conture to them.
        'If the decal is excluded, Models are not rendered and wont effect the decals geo.
        attach_texture_to_FBO(coMapID)
        'check status
        'Dim er = Gl.glGetError
        'ResizeGL()
        Gl.glPushMatrix()
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

        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glColor4f(0.0, 0.0, 0.0, 0.0)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_BLEND)

        Gl.glUseProgram(shader_list.depth_shader)

        '===============================================================================
        lightTransform(decal, True)
        'first render both in solid black for masking
        'Pass one.. render models in red
        'mask_makers(decal)
        Gl.glColor3f(1.0, 0.0, 0.0) 'very important to shader!
        draw_terrain(decal)
        Dim ar1((decal_grid_size ^ 2) * 3) As Single
        Gl.glReadPixels(0, 0, decal_grid_size, decal_grid_size, Gl.GL_RGB, Gl.GL_FLOAT, ar1)


        '===============================================================================
        ''first render both in solid black for making
        ''than render the model in green
        'frmMain.attache_texture(coMapID2)
        attach_texture_to_FBO(coMapID2)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        lightTransform(decal, False)
        mask_makers(decal)
        Gl.glColor3f(1.0, 1.0, 0.0) 'very important to shader!
        draw_models(decal)
        Dim ar2((decal_grid_size ^ 2) * 3) As Single
        Gl.glReadPixels(0, 0, decal_grid_size, decal_grid_size, Gl.GL_RGB, Gl.GL_FLOAT, ar2)
        '===============================================================================
        Gl.glUseProgram(0)

        For k = 0 To ar1.Length - 1
            If ar1(k) <> 0.0 And ar2(k) <> 0.0 Then
                'Dim vv = 1.0
                ar1(k) = ar2(k)
            Else
                ar1(k) += ar2(k)
            End If
        Next

        attach_texture_to_FBO(0)

        Dim c As Integer = (decal_grid_size / 2) - 1.0

        make_decal_mesh(decal)

        Dim w As Integer = decal_grid_size
        For i = 0 To decal_matrix_list(decal).decal_count - 1
            Dim x As Integer = decal_matrix_list(decal).decal_data(i).x
            Dim y As Integer = (decal_matrix_list(decal).decal_data(i).y) * w
            Dim p = (x + y) * 3
            Dim v As vect3
            v.x = ar1(p + 0)
            v.y = ar1(p + 1)
            v.z = ar1(p + 2)

            decal_matrix_list(decal).decal_data(i).x = v.x
            decal_matrix_list(decal).decal_data(i).y = v.y
            decal_matrix_list(decal).decal_data(i).z = v.z
        Next
        'need to find a way to reduce the decals poly count. Its a waste drawing 2048 polys on a flat surface

        If decal_matrix_list(decal).decal_data.Length > 1 Then
            Try
                make_decal_normals(decal)
                agv_mesh_norms(decal_matrix_list(decal))
                reduce_polys(decal, ar1)
                If decal_matrix_list(decal).display_id > 0 Then
                    Gl.glDeleteLists(decal_matrix_list(decal).display_id, 1)
                    Gl.glFinish()
                End If
                decal_matrix_list(decal).display_id = Gl.glGenLists(1)
                Gl.glNewList(decal_matrix_list(decal).display_id, Gl.GL_COMPILE)

                Gl.glBegin(Gl.GL_TRIANGLES)
                Dim cc As Integer = 0
                With decal_matrix_list(decal)
                    For i = 0 To decal_matrix_list(decal).decal_count - 1 Step 3
                        Dim ln As Boolean = True
                        If get_length_vertex(.decal_data(i)) = 0.0 Then ln = False
                        If get_length_vertex(.decal_data(i + 1)) = 0.0 Then ln = False
                        If get_length_vertex(.decal_data(i + 2)) = 0.0 Then ln = False
                        If ln Then
                            For j = i To i + 2
                                With .decal_data(j)
                                    Gl.glNormal3f(.nx, .ny, .nz)
                                    Gl.glTexCoord2f(.u, .v)
                                    Gl.glVertex3f(.x, .y, .z)
                                    c = j
                                End With
                            Next j
                        Else
                            Dim kkk = 1.0
                        End If
                    Next i
                End With
                With decal_matrix_list(decal).decal_data(c + 1)
                    Gl.glNormal3f(.nx, .ny, .nz)
                    Gl.glTexCoord2f(.u, .v)
                    Gl.glVertex3f(.x, .y, .z)

                End With
                ReDim decal_matrix_list(decal).decal_data(0)
                GC.Collect()
                Gl.glEnd()
                Gl.glEndList()

            Catch ex As Exception

            End Try
        End If
        'er = Gl.glGetError
        Gl.glUseProgram(0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
    End Sub
    Private Sub reduce_polys(ByVal decal As Integer, ar1() As Single)
        'I'm thinking the best way to do this is find the variance of the decals Y across X and Z.
        'I found it works better to sample less of the decals points.
        'I average the Y normal and use that to determine the variance.
        '

        Dim Variance As Single
        Dim running As Double
        Dim count As Integer = (decal_grid_size) * 3
        Dim check As Double = 0

        'loop thru and average the  normal;
        For i = 0 To (decal_matrix_list(decal).decal_count / 6) - (decal_grid_size - 1) Step ((decal_grid_size - 1)) * 6.0
            For j = 0 To ((decal_grid_size - 1) * 6) - 1 Step 23.125
                running += 1.0 - Abs(decal_matrix_list(decal).decal_data(CInt(i * 6) + CInt(j)).ny)
                check += 1
                Dim p = CInt(i * 6) + CInt(j)
                'Debug.WriteLine("I:" + i.ToString("0000") + " J:" + j.ToString("0000") + " P:" + p.ToString("0000"))
            Next
        Next
        Variance = running / check ' get average variance
        Dim reduction As Integer = 4
        If frmBiasing.Visible Then
            frmMain.tb1.Text += " Variance:" + Variance.ToString("00.000000")
        End If
        If Variance > 0.02 Then Return
        If Variance > 0.005 Then
            reduction = 8.0
        End If
        If Variance <= 0.005 Then
            reduction = 4.0
        End If
        '==================================================================================================
        'rebuild the decal to the new size
        Dim decal_count As Integer = 0
        ReDim decal_matrix_list(decal).decal_data((decal_grid_size ^ 2) * 6)
        decal_matrix_list(decal).decal_count = 0

        Dim uvScale = 1.0# / CSng(decal_grid_size - 1.0)
        Dim scale = 1.0 / CSng(decal_grid_size)
        Dim w = (decal_grid_size - 1) / reduction
        With decal_matrix_list(decal)
            Dim fixshift As Boolean = False

            For j As Single = 0 To decal_grid_size - w Step w
                For i As Single = 0 To decal_grid_size - w Step w

                    topleft.x = Int(i)
                    topleft.y = Int(j)
                    topleft.z = -0.5
                    topleft.u = Int(i) * uvScale
                    topleft.v = Int(j) * uvScale


                    topRight.x = Int(i + w)
                    topRight.y = Int(j)
                    topRight.z = -0.5
                    topRight.u = Int((i) + w) * uvScale
                    topRight.v = Int(j) * uvScale

                    bottomRight.x = Int(i + w)
                    bottomRight.y = Int(j + w)
                    bottomRight.z = -0.5
                    bottomRight.u = Int((i) + w) * uvScale
                    bottomRight.v = Int((j) + w) * uvScale

                    bottomleft.x = Int(i)
                    bottomleft.y = Int(j + w)
                    bottomleft.z = -0.5
                    bottomleft.u = Int((i)) * uvScale
                    bottomleft.v = Int((j) + w) * uvScale

                    save_vertex(topRight, decal, ar1, decal_count, reduction)
                    decal_count += 1
                    save_vertex(bottomRight, decal, ar1, decal_count, reduction)
                    decal_count += 1
                    save_vertex(topleft, decal, ar1, decal_count, reduction)
                    decal_count += 1

                    save_vertex(topleft, decal, ar1, decal_count, reduction)
                    decal_count += 1
                    save_vertex(bottomRight, decal, ar1, decal_count, reduction)
                    decal_count += 1
                    save_vertex(bottomleft, decal, ar1, decal_count, reduction)
                    decal_count += 1

                Next
            Next
        End With
        decal_matrix_list(decal).decal_count = decal_count
        ReDim Preserve decal_matrix_list(decal).decal_data(decal_matrix_list(decal).decal_count)
        'sucks but the normals have to be recalculated.
        make_decal_normals(decal)
        agv_mesh_norms(decal_matrix_list(decal))

    End Sub
    Private Sub save_vertex(ByVal vt As vertex_data, ByVal decal As Integer, ar1() As Single, ByVal i As Integer, ByVal reduction As Single)
        Dim x As Integer = vt.x
        Dim y As Integer = (vt.y) * (decal_grid_size)
        Dim p = (x + y) * 3
        Dim v As vect3
        v.x = ar1(p + 0)
        v.y = ar1(p + 1)
        v.z = ar1(p + 2)
        decal_matrix_list(decal).decal_data(i).x = v.x
        decal_matrix_list(decal).decal_data(i).y = v.y
        decal_matrix_list(decal).decal_data(i).z = v.z
        decal_matrix_list(decal).decal_data(i).u = vt.u
        decal_matrix_list(decal).decal_data(i).v = vt.v

    End Sub
    'Private Sub transpose_matrix(ByRef Ms() As Single, ByRef Md() As Single)
    '    ReDim Md(16)
    '    Md(0) = Ms(0)
    '    Md(1) = Ms(4)
    '    Md(2) = Ms(8)
    '    Md(3) = Ms(12)

    '    Md(4) = Ms(1)
    '    Md(5) = Ms(5)
    '    Md(6) = Ms(9)
    '    Md(7) = Ms(10)

    '    Md(8) = Ms(2)
    '    Md(9) = Ms(6)
    '    Md(10) = Ms(10)
    '    Md(11) = Ms(14)

    '    Md(12) = Ms(3)
    '    Md(13) = Ms(7)
    '    Md(14) = Ms(11)
    '    Md(15) = Ms(15)

    'End Sub
    Public Sub make_decals()
        ReDim decal_cache(0)
        decal_cache(0) = New decal_
        maploaded = False


        '----------------------------------------------------------
        'set up chunk list information for texture projection
        ReDim decal_chunks(decal_matrix_list.Length - 1)



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


            'decal_matrix_list(k).matrix(0) *= -1.0
            decal_matrix_list(k).matrix(1) *= -1.0
            decal_matrix_list(k).matrix(2) *= -1.0
            'decal_matrix_list(k).matrix(3) *= -1.0

            decal_matrix_list(k).matrix(4) *= -1.0
            'decal_matrix_list(k).matrix(5) *= -1.0
            'decal_matrix_list(k).matrix(6) *= -1.0
            'decal_matrix_list(k).matrix(7) *= -1.0

            decal_matrix_list(k).matrix(8) *= -1.0
            'decal_matrix_list(k).matrix(9) *= -1.0
            'decal_matrix_list(k).matrix(10) *= -1.0
            'decal_matrix_list(k).matrix(11) *= -1.0
            decal_matrix_list(k).matrix(12) *= -1.0

            'decal_matrix_list(k).matrix(13) *= -1.0
            'decal_matrix_list(k).matrix(14) *= -1.0
            'decal_matrix_list(k).matrix(15) *= -1.0
            Dim m = decal_matrix_list(k).matrix

            ReDim decal_matrix_list(k).texture_matrix(16)
            'Dim id As Integer
            'id = Gl.glGenLists(1)
            'Gl.glNewList(id, Gl.GL_COMPILE)
            make_decal_mesh(k)
            'now that we have the transforms,, we can figure out which chunks this decal touches.
            With decal_matrix_list(k)
                decal_chunks(k) = New decal_chunks_
                ReDim decal_chunks(k).chunk_Ids(4) ' max it will ever touch
                ReDim decal_chunks(k).seam_Ids(4)
                Dim d_tl As vect3 = .top_left
                Dim d_tr As vect3 = .top_right
                Dim d_bl As vect3 = .bot_left
                Dim d_br As vect3 = .bot_right
                Dim c As vect3
                Dim cnt As Integer = 0
                Dim hit As Integer = 0

                For i = 0 To test_count
                    hit = 0
                    c = maplist(i).location
                    'check each decal point for inside chunk
                    If d_tl.x > c.x - 60.0 And d_tl.x < c.x + 60.0 Then
                        If d_tl.z > c.y - 60.0 And d_tl.z < c.y + 60.0 Then
                            hit += 1
                        End If
                    End If
                    If d_tr.x > c.x - 60.0 And d_tr.x < c.x + 60.0 Then
                        If d_tr.z > c.y - 60.0 And d_tr.z < c.y + 60.0 Then
                            hit += 1
                        End If
                    End If
                    If d_br.x > c.x - 60.0 And d_br.x < c.x + 60.0 Then
                        If d_br.z > c.y - 60.0 And d_br.z < c.y + 60.0 Then
                            hit += 1
                        End If
                    End If
                    If d_bl.x > c.x - 60.0 And d_bl.x < c.x + 60.0 Then
                        If d_bl.z > c.y - 60.0 And d_bl.z < c.y + 60.0 Then
                            hit += 1
                        End If
                    End If
                    'check each chunk point for inside decal rectangle
                    If hit = 0 Then
                        Dim p1, p2, p3, p4 As vect3
                        p1.x = c.x - 50 : p1.z = c.y + 50
                        p2.x = c.x + 50 : p2.z = c.y + 50
                        p3.x = c.x + 50 : p3.z = c.y - 50
                        p4.x = c.x - 50 : p4.z = c.y - 50
                        If isInsideSquare(d_tl, d_tr, d_br, d_bl, p1) Then hit += 1
                        If isInsideSquare(d_tl, d_tr, d_br, d_bl, p2) Then hit += 1
                        If isInsideSquare(d_tl, d_tr, d_br, d_bl, p3) Then hit += 1
                        If isInsideSquare(d_tl, d_tr, d_br, d_bl, p4) Then hit += 1
                    End If

                    If hit > 0 Then
                        cnt += 1
                        decal_chunks(k).chunk_Ids(cnt - 1) = maplist(i).calllist_Id
                        decal_chunks(k).seam_Ids(cnt - 1) = maplist(i).seamCallId
                        decal_chunks(k).chunk_count = cnt
                    End If
                Next
                ReDim Preserve decal_chunks(k).chunk_Ids(cnt)
                ReDim Preserve decal_chunks(k).seam_Ids(cnt)

            End With
            Application.DoEvents()
skipthis:
        Next

    End Sub
    Public Function triangleArea(A As vect3, B As vect3, C As vect3) As Single
        Return (C.x * B.z - B.x * C.z) - (C.x * A.z - A.x * C.z) + (B.x * A.z - A.x * B.z)
    End Function
    Public Function isInsideSquare(A As vect3, B As vect3, C As vect3, D As vect3, P As vect3) As Boolean
        If triangleArea(A, B, P) > 0 And triangleArea(B, C, P) > 0 And _
           triangleArea(C, D, P) > 0 And triangleArea(D, A, P) > 0 Then
            Return True
        End If
        If triangleArea(A, B, P) < 0 And triangleArea(B, C, P) < 0 And _
           triangleArea(C, D, P) < 0 And triangleArea(D, A, P) < 0 Then
            Return True
        End If
        Return False
    End Function

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

    Public Sub make_decal_mesh(ByVal map As Int32)

        With decal_matrix_list(map)

            .top_left.x = -0.5
            .top_left.y = 0.5
            .top_left.z = -0.0

            .top_right.x = 0.5
            .top_right.y = 0.5
            .top_right.z = -0.0

            .bot_left.x = -0.5
            .bot_left.y = -0.5
            .bot_left.z = -0.0

            .bot_right.x = 0.5
            .bot_right.y = -0.5
            .bot_right.z = -0.0

            .cam_pos.x = 0.0
            .cam_pos.y = 0.0
            .cam_pos.z = -0.1
            .look_at.x = 0.0
            .look_at.y = 0.0
            .look_at.z = 0.0
            .far_clip.x = 0.0
            .far_clip.y = 0.0
            .far_clip.z = 1.0
            .near_clip.x = 0.0
            .near_clip.y = 0.0
            .near_clip.z = -1.0
            .far_clip = rotate_only(.far_clip, .matrix)
            .near_clip = rotate_only(.near_clip, .matrix)


            .look_at = translate_to(.look_at, .matrix)
            .cam_pos = translate_to(.cam_pos, .matrix)
            .top_left = translate_to(.top_left, .matrix)
            .top_right = translate_to(.top_right, .matrix)
            .bot_left = translate_to(.bot_left, .matrix)
            .bot_right = translate_to(.bot_right, .matrix)
            .cam_rotation = rotate_decal_view(.matrix)
            .cam_location.x = .matrix(12)
            .cam_location.y = .matrix(13)
            .cam_location.z = .matrix(14)

        End With

        ReDim decal_matrix_list(map).decal_data((decal_grid_size ^ 2) * 6)
        decal_matrix_list(map).decal_count = 0

        Dim uvScale = 1.0# / CSng(decal_grid_size - 1)
        Dim w_ = decal_grid_size / 2.0#
        Dim h_ = decal_grid_size / 2.0#
        Dim scale = 1.0 / CSng(decal_grid_size)
        For j As Single = 0 To decal_grid_size - 2
            For i As Single = 0 To decal_grid_size - 2


                topleft.x = (i)
                topleft.y = (j)
                topleft.z = -0.5
                topleft.u = (i) * uvScale
                topleft.v = (j) * uvScale


                topRight.x = (i + 1)
                topRight.y = (j)
                topRight.z = -0.5
                topRight.u = (i + 1) * uvScale
                topRight.v = (j) * uvScale

                bottomRight.x = (i + 1)
                bottomRight.y = (j + 1)
                bottomRight.z = -0.5
                bottomRight.u = (i + 1) * uvScale
                bottomRight.v = (j + 1) * uvScale

                bottomleft.x = (i)
                bottomleft.y = (j + 1)
                bottomleft.z = -0.5
                bottomleft.u = (i) * uvScale
                bottomleft.v = (j + 1) * uvScale

                store_decal_triangle(topRight, bottomRight, topleft, scale, map)
                store_decal_triangle(topleft, bottomRight, bottomleft, scale, map)
                '	If i = 62 Then
                '		Stop
                '	End If
            Next
        Next
        ReDim Preserve decal_matrix_list(map).decal_data(decal_matrix_list(map).decal_count)

    End Sub
    Public Sub store_decal_triangle(ByVal vt1 As vertex_data, ByVal vt2 As vertex_data, ByVal vt3 As vertex_data, ByRef scale As Single, ByVal decal As Int32)
        tri_count += 1
        'add offsets
        'vt1 = transform(decal_matrix_list(decal).matrix, vt1, scale, decal)
        'vt2 = transform(decal_matrix_list(decal).matrix, vt2, scale, decal)
        'vt3 = transform(decal_matrix_list(decal).matrix, vt3, scale, decal)
        'vt1.y = get_Z_at_XY(vt1.x, vt1.z) + 0.02
        'vt2.y = get_Z_at_XY(vt2.x, vt2.z) + 0.02
        'vt3.y = get_Z_at_XY(vt3.x, vt3.z) + 0.02
        'Dim shift = (decal_grid_size / 2) - 1
        'vt1.x -= shift
        'vt2.x -= shift
        'vt3.x -= shift
        'vt1.y -= shift
        'vt2.y -= shift
        'vt3.y -= shift

        decal_matrix_list(decal).decal_data(decal_matrix_list(decal).decal_count) = vt1
        decal_matrix_list(decal).decal_data(decal_matrix_list(decal).decal_count + 1) = vt2
        decal_matrix_list(decal).decal_data(decal_matrix_list(decal).decal_count + 2) = vt3
        decal_matrix_list(decal).decal_count += 3


    End Sub
    Public Sub make_decal_normals(ByVal decal As Integer)
        Dim vt1, vt2, vt3 As vertex_data
        Dim a, b, n As vect3
        Dim ln As Single
        For i = 0 To decal_matrix_list(decal).decal_count - 1 Step 3
            With decal_matrix_list(decal)

                vt1 = .decal_data(i + 0)
                vt2 = .decal_data(i + 1)
                vt3 = .decal_data(i + 2)

                a.x = vt1.x - vt2.x
                a.y = vt1.y - vt2.y
                a.z = vt1.z - vt2.z
                b.x = vt2.x - vt3.x
                b.y = vt2.y - vt3.y
                b.z = vt2.z - vt3.z
                n.x = (a.y * b.z) - (a.z * b.y)
                n.y = (a.z * b.x) - (a.x * b.z)
                n.z = (a.x * b.y) - (a.y * b.x)
                ln = Sqrt((n.x * n.x) + (n.y * n.y) + (n.z * n.z))
                If ln = 0 Then ln = 1.0 ' no divide by zero
                'normalize length
                n.x /= ln
                n.y /= ln
                n.z /= ln

                vt1.v *= -1.0
                vt2.v *= -1.0
                vt3.v *= -1.0
                vt1.u *= -1.0
                vt2.u *= -1.0
                vt3.u *= -1.0

                vt1.nx = n.x
                vt1.ny = n.y
                vt1.nz = n.z
                vt2.nx = n.x
                vt2.ny = n.y
                vt2.nz = n.z
                vt3.nx = n.x
                vt3.ny = n.y
                vt3.nz = n.z

                .decal_data(i + 0) = vt1
                .decal_data(i + 1) = vt2
                .decal_data(i + 2) = vt3
            End With
        Next

    End Sub
    Public Sub build_decals()
        frmMain.ProgressBar1.Value = 0
        frmMain.ProgressBar1.Maximum = decal_matrix_list.Length
        Application.DoEvents()
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, fboID)
        For k = 0 To decal_matrix_list.Length - 1
            If decal_matrix_list(k).good Then
                make_decal_depthMap(k) 'create the projected texture
                get_decal_textures(k) ' get the textures
                frmMain.ProgressBar1.Value = k
                frmMain.tb1.Text = "Populating the sections with decals ( " + k.ToString + " )" 'talk to user
                Application.DoEvents()
            End If
        Next
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        Gl.glDrawBuffer(Gl.GL_BACK)
    End Sub
    Public Sub agv_mesh_norms(ByRef d As decal_matrix_list_)
        Dim count = d.decal_count
        'do the inside of the mesh
        If d.decal_texture.ToLower.Contains("shadow_true") Then
            Return
        End If
        Dim n As Integer = Sqrt(count / 6) * 6
        For i As Integer = 0 To (count - 1) - (n) Step n
            For k As Integer = i To (i + n) - 12 Step 6
                'loop thru and grab the 6 vertices that share the same exact space.
                Dim a1 = d.decal_data(k + 1)
                Dim a2 = d.decal_data(k + 4)
                Dim a3 = d.decal_data(k + 6 + 5)
                Dim a4 = d.decal_data((k + n) + 0)
                Dim a5 = d.decal_data((k + n) + 6 + 2)
                Dim a6 = d.decal_data((k + n) + 6 + 3)
                'Average out the normals
                a1.nx = (a1.nx + a2.nx + a3.nx + a4.nx + a5.nx + a6.nx) / 6.0
                a1.ny = (a1.ny + a2.ny + a3.ny + a4.ny + a5.ny + a6.ny) / 6.0
                a1.nz = (a1.nz + a2.nz + a3.nz + a4.nz + a5.nz + a6.nz) / 6.0

                Dim len As Single = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
                If len = 0 Then len = 1.0 ' no divide by zero
                a1.nx /= len
                a1.ny /= len
                a1.nz /= len


                'Store the averaged normal back in to the 6 vertices
                d.decal_data(k + 1).nx = a1.nx
                d.decal_data(k + 1).ny = a1.ny
                d.decal_data(k + 1).nz = a1.nz

                d.decal_data(k + 4).nx = a1.nx
                d.decal_data(k + 4).ny = a1.ny
                d.decal_data(k + 4).nz = a1.nz

                d.decal_data(k + 6 + 5).nx = a1.nx
                d.decal_data(k + 6 + 5).ny = a1.ny
                d.decal_data(k + 6 + 5).nz = a1.nz

                d.decal_data((k + n) + 0).nx = a1.nx
                d.decal_data((k + n) + 0).ny = a1.ny
                d.decal_data((k + n) + 0).nz = a1.nz

                d.decal_data((k + n) + 6 + 2).nx = a1.nx
                d.decal_data((k + n) + 6 + 2).ny = a1.ny
                d.decal_data((k + n) + 6 + 2).nz = a1.nz

                d.decal_data((k + n) + 6 + 3).nx = a1.nx
                d.decal_data((k + n) + 6 + 3).ny = a1.ny
                d.decal_data((k + n) + 6 + 3).nz = a1.nz

            Next
        Next
        'do the left side
        For i As Integer = 0 To (count - 1) - (n) Step n
            Dim a1 = d.decal_data(i + 5)
            Dim a2 = d.decal_data((i + n) + 2)
            Dim a3 = d.decal_data((i + n) + 3)

            a1.nx = (a1.nx + a2.nx + a3.nx) / 3.0
            a1.ny = (a1.ny + a2.ny + a3.ny) / 3.0
            a1.nz = (a1.nz + a2.nz + a3.nz) / 3.0

            Dim len As Single = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
            If len = 0 Then len = 1.0 ' no divide by zero
            a1.nx /= len
            a1.ny /= len
            a1.nz /= len

            d.decal_data(i + 5).nx = a1.nx
            d.decal_data(i + 5).ny = a1.ny
            d.decal_data(i + 5).nz = a1.nz

            d.decal_data((i + n) + 2).nx = a1.nx
            d.decal_data((i + n) + 2).ny = a1.ny
            d.decal_data((i + n) + 2).nz = a1.nz

            d.decal_data((i + n) + 3).nx = a1.nx
            d.decal_data((i + n) + 3).ny = a1.ny
            d.decal_data((i + n) + 3).nz = a1.nz

        Next
        'bottom left corner
        For i = 0 To 0 ' so the varibles are not definded ( I, a1, a2)

            Dim a1 = d.decal_data(i + 2)
            Dim a2 = d.decal_data(i + 3)

            a1.nx = (a1.nx + a2.nx) / 2.0
            a1.ny = (a1.ny + a2.ny) / 2.0
            a1.nz = (a1.nz + a2.nz) / 2.0

            Dim len As Single = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
            If len = 0 Then len = 1.0 ' no divide by zero
            a1.nx /= len
            a1.ny /= len
            a1.nz /= len

            d.decal_data(i + 2).nx = a1.nx
            d.decal_data(i + 2).ny = a1.ny
            d.decal_data(i + 2).nz = a1.nz

            d.decal_data(i + 3).nx = a1.nx
            d.decal_data(i + 3).ny = a1.ny
            d.decal_data(i + 3).nz = a1.nz
        Next
        'top right corner
        For i = (count - 6) To (count - 6) ' so the varibles are not definded ( I, a1, a2)

            Dim a1 = d.decal_data(i + 1)
            Dim a2 = d.decal_data(i + 4)

            a1.nx = (a1.nx + a2.nx) / 2.0
            a1.ny = (a1.ny + a2.ny) / 2.0
            a1.nz = (a1.nz + a2.nz) / 2.0

            Dim len As Single = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
            If len = 0 Then len = 1.0 ' no divide by zero
            a1.nx /= len
            a1.ny /= len
            a1.nz /= len

            d.decal_data(i + 1).nx = a1.nx
            d.decal_data(i + 1).ny = a1.ny
            d.decal_data(i + 1).nz = a1.nz

            d.decal_data(i + 4).nx = a1.nx
            d.decal_data(i + 4).ny = a1.ny
            d.decal_data(i + 4).nz = a1.nz
        Next
        'do the right side
        For i As Integer = n - 6 To (count) - (n) Step n
            Dim a1 = d.decal_data(i + 1)
            Dim a2 = d.decal_data(i + 4)
            Dim a3 = d.decal_data((i + n) + 0)

            a1.nx = (a1.nx + a2.nx + a3.nx) / 3.0
            a1.ny = (a1.ny + a2.ny + a3.ny) / 3.0
            a1.nz = (a1.nz + a2.nz + a3.nz) / 3.0

            Dim len As Single = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
            If len = 0 Then len = 1.0 ' no divide by zero
            a1.nx /= len
            a1.ny /= len
            a1.nz /= len

            d.decal_data(i + 1).nx = a1.nx
            d.decal_data(i + 1).ny = a1.ny
            d.decal_data(i + 1).nz = a1.nz

            d.decal_data(i + 4).nx = a1.nx
            d.decal_data(i + 4).ny = a1.ny
            d.decal_data(i + 4).nz = a1.nz

            d.decal_data((i + n) + 0).nx = a1.nx
            d.decal_data((i + n) + 0).ny = a1.ny
            d.decal_data((i + n) + 0).nz = a1.nz

        Next
        'do top row
        For i As Integer = (count) - n To (count) - 12 Step 6
            Dim a1 = d.decal_data(i + 1)
            Dim a2 = d.decal_data(i + 4)
            Dim a3 = d.decal_data((i + 6) + 5)

            a1.nx = (a1.nx + a2.nx + a3.nx) / 2.0
            a1.ny = (a1.ny + a2.ny + a3.ny) / 2.0
            a1.nz = (a1.nz + a2.nz + a3.nz) / 2.0

            Dim len As Single = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
            If len = 0 Then len = 1.0 ' no divide by zero
            a1.nx /= len
            a1.ny /= len
            a1.nz /= len

            d.decal_data(i + 1).nx = a1.nx
            d.decal_data(i + 1).ny = a1.ny
            d.decal_data(i + 1).nz = a1.nz

            d.decal_data(i + 4).nx = a1.nx
            d.decal_data(i + 4).ny = a1.ny
            d.decal_data(i + 4).nz = a1.nz

            d.decal_data((i + 6) + 5).nx = a1.nx
            d.decal_data((i + 6) + 5).ny = a1.ny
            d.decal_data((i + 6) + 5).nz = a1.nz

        Next
        'do the bottom row (first row)
        For i As Integer = 0 To n - 12 Step 6
            Dim a1 = d.decal_data(i + 0)
            Dim a2 = d.decal_data((i + 6) + 2)
            Dim a3 = d.decal_data((i + 6) + 3)

            a1.nx = (a1.nx + a2.nx + a3.nx) / 3.0
            a1.ny = (a1.ny + a2.ny + a3.ny) / 3.0
            a1.nz = (a1.nz + a2.nz + a3.nz) / 3.0

            Dim len As Single = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
            If len = 0 Then len = 1.0 ' no divide by zero
            a1.nx /= len
            a1.ny /= len
            a1.nz /= len

            d.decal_data(i + 0).nx = a1.nx
            d.decal_data(i + 0).ny = a1.ny
            d.decal_data(i + 0).nz = a1.nz

            d.decal_data((i + 6) + 2).nx = a1.nx
            d.decal_data((i + 6) + 2).ny = a1.ny
            d.decal_data((i + 6) + 2).nz = a1.nz

            d.decal_data((i + 6) + 3).nx = a1.nx
            d.decal_data((i + 6) + 3).ny = a1.ny
            d.decal_data((i + 6) + 3).nz = a1.nz

        Next

    End Sub
    Public Sub create_decal_FBO()
        Dim max_size As Single
        Gl.glGetFloatv(Gl.GL_MAX_TEXTURE_SIZE, max_size)

        'Dim drawbuffs() = {Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_NONE}
        coMapID = Make_Depth_texture(decal_grid_size)
        coMapID2 = Make_Depth_texture(decal_grid_size)
        ' utility_texture = Make_Depth_texture(decal_grid_size)
        Dim er = Gl.glGetError
        ' create a framebuffer object
        '-------------------------------------------------------------------------------
        ' make first FBO
        '-------------------------------------------------------------------------------
        Gl.glGenFramebuffersEXT(1, fboID)
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, fboID)
        'er = Gl.glGetError
        '-------------------------------------------------------------------------------
        ' create a depth buffer
        Gl.glGenRenderbuffersEXT(1, depthID)
        'er = Gl.glGetError
        Gl.glBindRenderbufferEXT(Gl.GL_RENDERBUFFER_EXT, depthID)
        Gl.glRenderbufferStorageEXT(Gl.GL_RENDERBUFFER_EXT, Gl.GL_DEPTH_COMPONENT, decal_grid_size, decal_grid_size)
        'er = Gl.glGetError
        Gl.glFramebufferRenderbufferEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_ATTACHMENT_EXT, Gl.GL_RENDERBUFFER_EXT, depthID)
        er = Gl.glGetError


        Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, coMapID, 0)
        'er = Gl.glGetError
        Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_TEXTURE_2D, coMapID2, 0)
        'er = Gl.glGetError
        'Gl.glFramebufferRenderbufferEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_ATTACHMENT_EXT, Gl.GL_RENDERBUFFER_EXT, depthID)
        'er = Gl.glGetError

        If er <> 0 Then
            Dim s = Glu.gluErrorString(er)
            MsgBox("Error switching render texture! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If
        '-------------------------------------------------------------------------------

        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
    End Sub

End Module
