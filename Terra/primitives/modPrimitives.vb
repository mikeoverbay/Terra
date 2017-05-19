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
Imports System.Windows.Media.Media3D.Vector3D
Imports System.Windows.Media.Media3D
Imports Ionic.Zip
Imports Ionic.BZip2
'
Imports Ionic
#End Region


Module modPrimitives
    Public d_hx, d_hy As Integer
    Dim thefile As String
    Public short_vertex_type As Boolean = False ' used to tell display list maker if this is a short vertex xtznuv type
    Private Function look_for_mdl(ByVal s As String) As Boolean
        'dumb but this is used by the list to find a match lol
        If s = thefile Then
            Return True
        End If
        Return False
    End Function
    Public Function get_primitive(ByVal model_ID As Integer, ByVal pkg As Ionic.Zip.ZipFile) As Boolean
        'If InStr(models.model_list(model_ID), "bld202_vhouse01") > 0 Then
        '    'Stop
        'End If
        'Destructible buildings are stored as components and as a merged version.
        'The mergerd version is faster to load and is a sigle mesh.

        thefile = Models.Model_list(model_ID).ToLower
        thefile = thefile.Replace("model", "visual_processed")
        If InStr(Models.Model_list(model_ID), "bld_Constr") > 0 Then
            Models.Model_list(model_ID) = Replace(Models.Model_list(model_ID), "normal", "merged")
        End If
        Dim m_name = thefile.Replace("normal", "merged")
        Dim mergerd_entry As Ionic.Zip.ZipEntry = pkg(m_name)
        If mergerd_entry IsNot Nothing Then
            thefile = m_name
            Models.Model_list(model_ID) = m_name
        Else
            mergerd_entry = get_shared_model(m_name)
            If mergerd_entry IsNot Nothing Then
                thefile = m_name
                Models.Model_list(model_ID) = m_name
            End If
        End If


        Dim old_file As String = thefile
        Dim trycount As Integer = 2
        If frmMain.m_load_lod.Checked Then ' if this is checked, use best LOD there is.. lod0
            trycount = 0
        End If
        'If map = 4 Then
        '    Stop
        'End If
        'we are looking for the lowest LOD we can find to speed rendering up and
        'reduce mem usage.
fing_loop:
        Select Case trycount
            Case 2
                If InStr(old_file, "lod0") > 0 Then
                    thefile = Replace(old_file, "lod0", "lod2")
                Else
                    thefile = old_file
                    trycount = 0
                End If
            Case 1
                If InStr(old_file, "lod0") > 0 Then
                    thefile = Replace(old_file, "lod0", "lod1")
                End If
            Case 0
                thefile = old_file
        End Select
        '1st.. lets see if we have this loaded already..
        'If so.. we will take the display list and put it in this model_id model
        If loaded_models.names.Contains(thefile) Then
            Dim addy = loaded_models.names.IndexOf(thefile)
            If addy > -1 Then ' addy points at the list of displaylists in the stack
                Dim mc As Integer = loaded_models.stack(addy)._count
                ReDim Preserve Models.models(model_ID).componets(mc)
                Models.models(model_ID)._count = mc
                saved_model_loads += 1
                loaded_models.stack(addy).model_count += 1
                Dim mdl_cnt As Integer = loaded_models.stack(addy).model_count
                ReDim Preserve loaded_models.stack(addy).matrix(mdl_cnt)
                ReDim Preserve loaded_models.stack(addy).model_id(mdl_cnt)
                loaded_models.stack(addy).model_id(mdl_cnt - 1) = model_ID
                loaded_models.stack(addy).matrix(mdl_cnt - 1) = New matrix_
                ReDim loaded_models.stack(addy).matrix(mdl_cnt - 1).matrix(16)
                loaded_models.stack(addy).matrix(mdl_cnt - 1).matrix = Model_Matrix_list(model_ID).matrix
                Models.models(model_ID).isBuilding = loaded_models.stack(addy).isBuilding
                For i = 0 To mc - 1
                    Models.models(model_ID).componets(i).callList_ID = _
                              loaded_models.stack(addy).dispId(i)
                    Models.models(model_ID).componets(i).color_id = _
                              loaded_models.stack(addy).textId(i)
                    Models.models(model_ID).componets(i).color_name = _
                              loaded_models.stack(addy).color_name(i)
                    Models.models(model_ID).componets(i).color2_Id = _
                              loaded_models.stack(addy).text2Id(i)
                    Models.models(model_ID).componets(i).color2_name = _
                              loaded_models.stack(addy).color2_name(i)
                    Models.models(model_ID).componets(i).normal_Id = _
                              loaded_models.stack(addy).normID(i)
                    Models.models(model_ID).componets(i).alphaRef = _
                              loaded_models.stack(addy).alphaRef(i)
                    Models.models(model_ID).componets(i).alphaTestEnable = _
                              loaded_models.stack(addy).alphaTestEnable(i)
                    Models.models(model_ID).componets(i).bumped = _
                              loaded_models.stack(addy).bumped(i)
                    Models.models(model_ID).componets(i).GAmap = _
                              loaded_models.stack(addy).GAmap(i)
                    Models.models(model_ID).componets(i).multi_textured = _
                              loaded_models.stack(addy).multi_textured(i)
                    'Debug.Write("in queue: " & models.model_list(model_ID) & vbCrLf)
                    'Debug.Write("color_Id:" & loaded_models.stack(addy).dispId(i) & vbCrLf)
                Next
                'transform matrix is already loaded from previous functions
                Return True ' dont need to do anything else? Nope.. its all good
            End If
        End If
        Dim a11() = thefile.Split("/")
        Dim a2() = a11(a11.Length - 1).Split(".")

        Dim p_name = thefile.Replace(".visual_processed", ".primitives_processed")
        Dim v_name = thefile
        bw_strings.Append(model_ID.ToString("0000") + "  " + Model_Matrix_list(model_ID).primitive_name)

        Dim xmlentry As Ionic.Zip.ZipEntry = get_shared_model(v_name)
        Dim ms = New MemoryStream
        If xmlentry Is Nothing Then
            xmlentry = get_shared(v_name)
            If xmlentry Is Nothing Then
                xmlentry = shared_content2(v_name)
                If xmlentry Is Nothing Then

                    ms.Close()
                    ms.Dispose()
                    If trycount > 0 Then
                        trycount -= 1
                        GoTo fing_loop
                    End If
                    Debug.Write("Cant find: " & v_name & vbCrLf)
                    ms.Dispose()
                    'Stop
                    Return False
                End If
            Else
                xmlentry.Extract(ms)
            End If
            If xmlentry Is Nothing Then
                ms.Close()
                ms.Dispose()
                Return False
            End If
            xmlentry.Extract(ms)
        Else
            xmlentry.Extract(ms)

        End If

        'If InStr(v_name, "bldAS_01") > 0 Then
        '    Stop
        'End If

        'must get the .visual information
        openXml_stream(ms, a2(0))
        '---------------------------------------------------
        Dim actual_name_1 = TheXML_String.Split(New String() {"</primitivesName>"}, StringSplitOptions.None)
        Dim actual_name_2 = actual_name_1(0).Split(New String() {"<primitivesName>"}, StringSplitOptions.None)
        If actual_name_2.Length > 1 Then
            p_name = actual_name_2(1) + ".primitives"

        End If

        '*******************************************************
        Dim vms = New MemoryStream
        xmlentry = pkg(p_name)
        If xmlentry Is Nothing Then
            xmlentry = get_shared_model(p_name)
            If xmlentry Is Nothing Then
                xmlentry = shared_content2(p_name)
                If xmlentry Is Nothing Then
                    vms.Close()
                    vms.Dispose()
                    Return False
                End If
            Else
                xmlentry.Extract(vms)
            End If
            xmlentry.Extract(vms)
        Else
            xmlentry.Extract(vms)

        End If
        Models.Model_list(model_ID) = thefile
        'If InStr(p_name, "bldAS_011") > 0 Then
        '    Stop
        'End If
        'If thefile.Contains("Willy") Then
        '    'Stop
        'End If
        Dim success = get_primitive_data(vms, model_ID, True)
        'If success Then

        'End If
        'this is used to determine what the decals is drawn on.
        If a11(1).ToLower.Contains("building") Or a11(1).ToLower.Contains("installations") Or _
            a11(2).ToLower.Contains("portwall") Or a11(2).ToLower.Contains("bridge") Then
            Models.models(model_ID).isBuilding = True
        Else
            Models.models(model_ID).isBuilding = False
        End If
        make_prim_list(model_ID)
        ms.Close()
        ms.Dispose()
        Select Case trycount
            Case 2
                lod2_swap += 1
            Case 1
                lod1_swap += 1
            Case 0
                lod0_swap += 1
        End Select
        Return True
    End Function
    Private has_uv2 As Boolean = False
    Private has_color As Boolean = False
    Public Function get_primitive_data(ByVal Pms As MemoryStream, ByVal model_id As Integer, ByVal loadtex As Boolean)
        'Dim sw As New Stopwatch
        'sw.Start()
        'If model_id = 649 Then
        '    Stop
        'End If
        Dim pGroups(1) As primGroup

        Dim br As New BinaryReader(Pms)
        Try
            Pms.Position = Pms.Length - 4

        Catch ex As Exception
            Return False
        End Try
        Dim index_start As Long = br.ReadUInt32
        Dim number_of_groups As UInt32 = 0
        Dim section(100) As section_info
        Dim na As String = ""
        Pms.Position = Pms.Length - 4 - index_start
        Dim runner As UInt32 = 0
        Dim sub_groups As Integer = 0
        Dim last_ind_pos As UInt32 = 0
        Dim last_vert_pos As UInt32 = 0
        Dim last_uv_pos As UInt32 = 0
        Dim color_start As UInt32 = 0
        'Dim color_length As UInt32
        Dim uv2_start, uv2_length As UInt32

        'Dim uv2_length As UInt32
        has_color = False : has_uv2 = False
        Dim sub_group_count As Integer = 0
        For i = 0 To 99
            If Pms.Position < Pms.Length - 4 Then
                section(i) = New section_info
                section(i).size = br.ReadUInt32
                If i > 0 Then
                    section(i).location = runner + 4
                End If
                runner += section(i).size
                Dim m = section(i).size Mod 4
                If m > 0 Then
                    runner += 4 - m

                End If
                'read 16 bytes of unused junk
                Dim dummy = br.ReadUInt32
                dummy = br.ReadUInt32
                dummy = br.ReadUInt32
                dummy = br.ReadUInt32
                'get section names length
                Dim sec_name_len As UInt32 = br.ReadUInt32
                'get this sections name
                For read_at As UInteger = 1 To sec_name_len
                    na = na & br.ReadChar
                Next
                section(i).name = na
                If InStr(na, "vertices") > 0 Then sub_groups += 1

                If InStr(na, "colour") > 0 Then
                    has_color = True
                End If

                If InStr(na, "uv2") > 0 And frmMain.m_show_uv2.Checked Then
                    uv2s_loaded = True
                    has_uv2 = True
                End If
                Dim l = na.Length Mod 4 'read off pad characters
                If l > 0 Then
                    br.ReadChars(4 - l)
                End If
                na = ""
            Else
                ReDim Preserve section(i)
                number_of_groups = i
                Exit For
            End If

        Next
        Dim got_subs As Boolean = False
        Dim gp_pointer As Integer
        Dim cur_sub As Integer
        gp_pointer = sub_groups
        If sub_groups > 1 Then
            got_subs = True
            'Stop
        End If
        Dim ind_start As UInt32 = 0
        Dim ind_length As UInt32 = 0
        Dim vert_start As UInt32 = 0
        Dim vert_length As UInt32 = 0
        While sub_groups > 0
            cur_sub = gp_pointer - sub_groups
            sub_groups -= 1 ' take one off.. if there is one, this results in zero and collects only one model set

            For i = 0 To section.Length - 1
                If InStr(section(i).name, "indices") > 0 Then
                    If last_ind_pos < (section(i).location + 1) Then
                        ind_length = section(i).size
                        ind_start = section(i).location
                        last_ind_pos = ind_start + 3
                        Exit For
                    End If
                End If
            Next
            'need to find the start of the vertices section
            For i = 0 To section.Length - 1
                If InStr(section(i).name, "vertices") > 0 Then
                    If last_vert_pos < (section(i).location + 1) Then
                        vert_length = section(i).size
                        vert_start = section(i).location
                        last_vert_pos = vert_start + 3
                        Exit For
                    End If
                End If
            Next
            'For i = 0 To section.Length - 1
            '    If InStr(section(i).name, "colour") > 0 Then
            '        color_length = section(i).size
            '        color_start = section(i).location
            '        Exit For
            '    End If
            'Next
            'If map = 28 And model_id = 22 Then
            '	Stop
            'End If
            For i = 0 To section.Length - 1
                If InStr(section(i).name, "uv2") > 0 Then
                    If last_uv_pos < (section(i).location + 1) Then
                        uv2_length = section(i).size
                        uv2_start = section(i).location
                        last_uv_pos = uv2_start + 3
                        has_uv2 = True
                        Exit For
                    End If
                End If
            Next
            'need to check if indices are before the vertices.. if so we need to do some off setting
            Dim lucky_72 As UInt32 = 72
            If Not got_subs Then
                If ind_start < vert_start Then
                    lucky_72 = 72
                    ind_start += 4
                    vert_start += 0
                Else
                    lucky_72 = 72
                    vert_start += 4
                End If
            Else
                If vert_start = 0 Then
                    vert_start += 4
                End If
            End If
            Dim cr As Byte
            Dim dr As Boolean = False
            ' get its name string
            Pms.Position = ind_start
            If Pms.Position = 0 Then
                Pms.Position += 4
                ind_start += 4
            End If
            For i = 0 To 63
                cr = br.ReadByte
                If cr = 0 Then dr = True
                If cr > 30 And cr <= 123 Then
                    If Not dr Then
                        na = na & Chr(cr)

                    End If
                End If
            Next
            Dim ih As IndexHeader
            Dim vh As VerticesHeader

            'Dim r_count As UInt32 = 0
            ih.ind_h = na
            If InStr(na, "list32") > 0 Then
                ind_scale = 4
            Else
                ind_scale = 2
            End If
            ih.nIndices_ = br.ReadUInt32
            ih.nInd_groups = br.ReadUInt16

            dr = False
            ReDim pGroups(ih.nInd_groups)

            Dim nOffset As UInteger = (ih.nIndices_ * ind_scale) + ind_start + lucky_72
            Pms.Position = nOffset
            For i = 0 To ih.nInd_groups - 1
                pGroups(i).startIndex_ = br.ReadUInt32
                pGroups(i).nPrimitives_ = br.ReadUInt32
                pGroups(i).startVertex_ = br.ReadUInt32
                pGroups(i).nVertices_ = br.ReadUInt32
            Next
            Dim indices_start = ind_start + lucky_72
            Pms.Position = vert_start

            Dim curpos As Long = br.BaseStream.Position

            na = ""
            For i = 0 To 63
                cr = br.ReadByte
                If cr = 0 Then dr = True
                If cr > 64 And cr <= 123 Then
                    If Not dr Then
                        na = na & Chr(cr)

                    End If
                End If
            Next
            vh.header_text = na
            dr = False
            na = ""
            '-------------------------------
            ' get stride of each vertex element
            Dim BPVT_mode As Boolean = False
            Dim realNormals As Boolean = False
            short_vertex_type = False
            stride = 0
            If vh.header_text = "xyznuv" Then
                short_vertex_type = True
                stride = 32
                realNormals = True
            End If
            If vh.header_text = "BPVTxyznuv" Then
                short_vertex_type = True
                BPVT_mode = True
                stride = 24
            End If
            If vh.header_text = "xyznuviiiwwtb" Then
                stride = 37
            End If
            If InStr(vh.header_text, "BPVTxyznuviiiwwtb") > 0 Then
                BPVT_mode = True
                stride = 40
            End If
            If vh.header_text = "xyznuvtb" Then
                stride = 32
            End If
            If InStr(vh.header_text, "BPVTxyznuvtb") > 0 Then
                BPVT_mode = True
                stride = 32
            End If
            If stride = 0 Then
                Stop
            End If
            'now that we have a count.. lets see if we need to get the uv2 coords
            If has_uv2 Then
                uv2s_loaded = True
                If BPVT_mode Then
                    Pms.Position = uv2_start + 136
                    ReDim Preserve uv2(uv2_length - 135)
                    get_uv2((uv2_length - 136) / 8, Pms)
                Else
                    Pms.Position = uv2_start
                    ReDim Preserve uv2(uv2_length + 1)
                    get_uv2(uv2_length / 8, Pms)
                End If

            End If

            If BPVT_mode Then
                br.BaseStream.Position = curpos + 132
            End If
            vh.nVertice_count = br.ReadUInt32
            'lets just read the entire vertice data.. might be faster
            Dim vertices(vh.nVertice_count * stride) As Byte


            Pms.Read(vertices, 0, vh.nVertice_count * stride)
            Dim vp As New MemoryStream(vertices)
            Dim vr As New BinaryReader(vp)

            'ReDim Preserve Models.models(model_id).componets(ih.nInd_groups)
            Dim cur_pointer = sub_group_count 'SGC is used when there are multipal models in one file
            Dim uv2_offset As UInt32 = 0

            For os_loop As UInt32 = 0 To ih.nInd_groups - 1

                'All primitives with only ONE group has to be drawn!
                If ih.nInd_groups > 1 Or got_subs Then
                    If this_is_uncrushed(model_id, os_loop + cur_sub) Then
                        GoTo dont_save_this
                    End If
                End If
                uv2_offset = pGroups(os_loop).startIndex_ / 3

                ' End If
                Models.models(model_id).componets(cur_pointer) = New Model_Section
                If has_uv2 Then
                    Models.models(model_id).componets(cur_pointer).multi_textured = True
                Else
                    Models.models(model_id).componets(cur_pointer).multi_textured = False
                End If
                If loadtex Then
                    If Not get_textures_and_names(model_id, cur_pointer, os_loop, has_uv2) Then
                        GoTo dont_save_this
                    End If
                End If
                If Models.models(model_id).componets(cur_pointer).color_name Is Nothing Then
                    GoTo dont_save_this
                End If
                'resize to fit
                Try

                    ReDim Preserve Models.models(model_id).componets(cur_pointer). _
                              vertices(pGroups(os_loop).nPrimitives_ * 3)
                    ReDim Preserve Models.models(model_id).componets(cur_pointer). _
                              normals(pGroups(os_loop).nPrimitives_ * 3)
                    ReDim Preserve Models.models(model_id).componets(cur_pointer). _
                              tangents(pGroups(os_loop).nPrimitives_ * 3)
                    ReDim Preserve Models.models(model_id).componets(cur_pointer). _
                              binormals(pGroups(os_loop).nPrimitives_ * 3)
                    ReDim Preserve Models.models(model_id).componets(cur_pointer). _
                              UVs(pGroups(os_loop).nPrimitives_ * 3)
                    ReDim Preserve Models.models(model_id).componets(cur_pointer). _
                          UV2s(pGroups(os_loop).nPrimitives_ * 3)
                    Models.models(model_id).componets(cur_pointer). _
                                  _count = pGroups(os_loop).nPrimitives_

                Catch ex As Exception
                    bw_strings.Append("   <-- Error" + vbCrLf)
                    Return False
                End Try
                'Next
                build_model_textures(model_id, cur_pointer)
                'found_dup_texture:
                '        '********************************************
                Dim ipos As UInt32
                'Dim v_first = ind_start - vert_start
                'If v_first > 0 And BPVT_mode Then
                '    ipos = (pGroups(os_loop).startIndex_ * 2) + lucky_72 + ind_start
                'Else
                'End If
                ipos = (pGroups(os_loop).startIndex_ * ind_scale) + indices_start ' + ind_start

                Dim indice As indi

                '.componets makes up each part in the visual file
                'and in this is the primitve structure for each one.. vert.. indis.. norms.. ect

                Dim packed As UInt32
                Pms.Position = ipos
                Models.models(model_id).componets(cur_pointer).callList_ID = -1
                Models.models(model_id).componets(cur_pointer).alphaRef = 255
                Dim uvPnt As indi
                For i As UInt32 = 0 To (pGroups(os_loop).nPrimitives_ - 1) * 3 Step 3

                    With Models.models(model_id).componets(cur_pointer)

                        Dim nor As New vect3Norm
                        'get indice set
                        indice.a1 = br.ReadUInt16 * stride
                        indice.a2 = br.ReadUInt16 * stride
                        indice.a3 = br.ReadUInt16 * stride
                        uvPnt.a1 = (indice.a1 / stride) '+ uv2_offset
                        uvPnt.a2 = (indice.a2 / stride) '+ uv2_offset
                        uvPnt.a3 = (indice.a3 / stride) '+ uv2_offset

                        'now get the vertices

                        '#1
                        vp.Position = indice.a1
                        .vertices(i) = New vect3
                        .normals(i) = New vect3Norm
                        .tangents(i) = New vect3Norm
                        .binormals(i) = New vect3Norm
                        .UVs(i) = New vect2uv
                        If has_uv2 Then
                            .UV2s(i) = New vect2uv
                        End If


                        .vertices(i).x = vr.ReadSingle
                        .vertices(i).y = vr.ReadSingle
                        .vertices(i).z = vr.ReadSingle
                        ' 
                        If realNormals Then
                            .normals(i).nx = vr.ReadSingle * -1.0
                            .normals(i).ny = vr.ReadSingle
                            .normals(i).nz = vr.ReadSingle
                        Else
                            packed = vr.ReadUInt32
                            If BPVT_mode Then
                                .normals(i) = unpackNormal_8_8_8(packed)
                            Else
                                .normals(i) = unpackNormal(packed)
                            End If
                        End If
                        .UVs(i).u = vr.ReadSingle
                        .UVs(i).v = vr.ReadSingle

                        If stride = 40 Then
                            vr.ReadByte()
                            vr.ReadByte()
                            vr.ReadByte()
                        End If
                        If stride = 37 Or stride = 40 Then
                            vr.ReadByte() 'i
                            vr.ReadByte() 'i
                            vr.ReadByte() 'i
                            vr.ReadByte() 'w
                            vr.ReadByte() 'w

                            packed = vr.ReadUInt32
                            If BPVT_mode Then
                                .tangents(i) = unpackNormal_8_8_8(packed)
                            Else
                                .tangents(i) = unpackNormal(packed)
                            End If
                            '
                            packed = vr.ReadUInt32
                            If BPVT_mode Then
                                '.binormals(i) = unpackNormal_8_8_8(packed)
                            Else
                                '.binormals(i) = unpackNormal(packed)
                            End If
                        Else
                            If Not realNormals And Not stride = 24 Then
                                If BPVT_mode Then
                                    .tangents(i) = unpackNormal_8_8_8(packed)
                                Else
                                    .tangents(i) = unpackNormal(packed)
                                End If
                                '
                                packed = vr.ReadUInt32
                                If BPVT_mode Then
                                    '.binormals(i) = unpackNormal_8_8_8(packed)
                                Else
                                    '.binormals(i) = unpackNormal(packed)
                                End If
                            End If
                        End If


                        If Models.models(model_id).componets(cur_pointer).multi_textured Then
                            .UV2s(i).u = uv2(uvPnt.a1).u
                            .UV2s(i).v = uv2(uvPnt.a1).v
                        End If

                        '#2
                        .vertices(i + 1) = New vect3
                        .normals(i + 1) = New vect3Norm
                        .tangents(i + 1) = New vect3Norm
                        .binormals(i + 1) = New vect3Norm
                        .UVs(i + 1) = New vect2uv
                        If has_uv2 Then
                            .UV2s(i + 1) = New vect2uv
                        End If


                        vp.Position = indice.a2
                        .vertices(i + 1).x = vr.ReadSingle
                        .vertices(i + 1).y = vr.ReadSingle
                        .vertices(i + 1).z = vr.ReadSingle
                        '
                        If realNormals Then
                            .normals(i + 1).nx = vr.ReadSingle * -1.0
                            .normals(i + 1).ny = vr.ReadSingle
                            .normals(i + 1).nz = vr.ReadSingle
                        Else
                            packed = vr.ReadUInt32
                            If BPVT_mode Then
                                .normals(i + 1) = unpackNormal_8_8_8(packed)
                            Else
                                .normals(i + 1) = unpackNormal(packed)
                            End If
                        End If
                        .UVs(i + 1).u = vr.ReadSingle
                        .UVs(i + 1).v = vr.ReadSingle

                        If stride = 40 Then
                            vr.ReadByte()
                            vr.ReadByte()
                            vr.ReadByte()
                        End If
                        If stride = 37 Or stride = 40 Then
                            vr.ReadByte() 'i
                            vr.ReadByte() 'i
                            vr.ReadByte() 'i
                            vr.ReadByte() 'w
                            vr.ReadByte() 'w

                            packed = vr.ReadUInt32
                            If BPVT_mode Then
                                .tangents(i + 1) = unpackNormal_8_8_8(packed)
                            Else
                                .tangents(i + 1) = unpackNormal(packed)
                            End If
                            '
                            packed = vr.ReadUInt32
                            If BPVT_mode Then
                                '.binormals(i + 1) = unpackNormal_8_8_8(packed)
                            Else
                                '.binormals(i + 1) = unpackNormal(packed)
                            End If
                        Else
                            If Not realNormals And Not stride = 24 Then
                                If BPVT_mode Then
                                    .tangents(i + 1) = unpackNormal_8_8_8(packed)
                                Else
                                    .tangents(i + 1) = unpackNormal(packed)
                                End If
                                '
                                packed = vr.ReadUInt32
                                If BPVT_mode Then
                                    '.binormals(i + 1) = unpackNormal_8_8_8(packed)
                                Else
                                    '.binormals(i + 1) = unpackNormal(packed)
                                End If
                            End If
                        End If
                        If Models.models(model_id).componets(cur_pointer).multi_textured Then
                            .UV2s(i + 1).u = uv2(uvPnt.a2).u
                            .UV2s(i + 1).v = uv2(uvPnt.a2).v
                        End If
                        '#3
                        .vertices(i + 2) = New vect3
                        .normals(i + 2) = New vect3Norm
                        .tangents(i + 2) = New vect3Norm
                        .binormals(i + 2) = New vect3Norm
                        .UVs(i + 2) = New vect2uv
                        If has_uv2 Then
                            .UV2s(i + 2) = New vect2uv
                        End If


                        vp.Position = indice.a3
                        .vertices(i + 2).x = vr.ReadSingle
                        .vertices(i + 2).y = vr.ReadSingle
                        .vertices(i + 2).z = vr.ReadSingle
                        '
                        If realNormals Then
                            .normals(i + 2).nx = vr.ReadSingle * -1.0
                            .normals(i + 2).ny = vr.ReadSingle
                            .normals(i + 2).nz = vr.ReadSingle
                        Else
                            packed = vr.ReadUInt32
                            If BPVT_mode Then
                                .normals(i + 2) = unpackNormal_8_8_8(packed)
                            Else
                                .normals(i + 2) = unpackNormal(packed)
                            End If
                        End If
                        .UVs(i + 2).u = vr.ReadSingle
                        .UVs(i + 2).v = vr.ReadSingle

                        If stride = 40 Then
                            vr.ReadByte()
                            vr.ReadByte()
                            vr.ReadByte()
                        End If
                        If stride = 37 Or stride = 40 Then
                            vr.ReadByte() 'i
                            vr.ReadByte() 'i
                            vr.ReadByte() 'i
                            vr.ReadByte() 'w
                            vr.ReadByte() 'w

                            packed = vr.ReadUInt32
                            If BPVT_mode Then
                                .tangents(i + 2) = unpackNormal_8_8_8(packed)
                            Else
                                .tangents(i + 2) = unpackNormal(packed)
                            End If
                            '
                            packed = vr.ReadUInt32
                            If BPVT_mode Then
                                '.binormals(i + 2) = unpackNormal_8_8_8(packed)
                            Else
                                '.binormals(i + 2) = unpackNormal(packed)
                            End If
                        Else
                            If Not realNormals And Not stride = 24 Then
                                If BPVT_mode Then
                                    .tangents(i + 2) = unpackNormal_8_8_8(packed)
                                Else
                                    .tangents(i + 2) = unpackNormal(packed)
                                End If
                                '
                                packed = vr.ReadUInt32
                                If BPVT_mode Then
                                    '.binormals(i + 2) = unpackNormal_8_8_8(packed)
                                Else
                                    '.binormals(i + 2) = unpackNormal(packed)
                                End If
                            End If
                        End If
                        If Models.models(model_id).componets(cur_pointer).multi_textured Then
                            .UV2s(i + 2).u = uv2(uvPnt.a3).u
                            .UV2s(i + 2).v = uv2(uvPnt.a3).v
                        End If

                        'get_bb(.vertices(i + 0), model_id)
                        'get_bb(.vertices(i + 1), model_id)
                        'get_bb(.vertices(i + 2), model_id)


                    End With
                Next
                uv2_offset += pGroups(os_loop).nVertices_
                cur_pointer += 1 ' this points at the current .componet (arrays of primitive info)
                If got_subs Then
                    sub_group_count = cur_pointer
                End If
                Models.models(model_id)._count = cur_pointer
                ReDim Preserve Models.models(model_id).componets(cur_pointer + 1)
dont_save_this:
            Next
            vr.Close()
            vp.Close()
            vp.Dispose()
        End While ' end of outside sub_groups loop
        br.Close()
        Pms.Close()
        Pms.Dispose()

        ReDim Model_Matrix_list(model_id).BB(8)
        get_translated_bb_model(Model_Matrix_list(model_id))
        'sw.Stop()
        'Dim t = sw.ElapsedMilliseconds
        'Debug.Write("make model time: " & t & vbCrLf)
        bw_strings.Append(vbCrLf)
        Return True

    End Function
    Private Sub get_bb(ByVal v As vect3, ByVal model As Integer)
        With Model_Matrix_list(model)
            If .BB_Min.x > v.x Then .BB_Min.x = v.x
            If .BB_Min.y > v.y Then .BB_Min.y = v.y
            If .BB_Min.z > v.z Then .BB_Min.z = v.z

            If .BB_Max.x < v.x Then .BB_Max.x = v.x
            If .BB_Max.y < v.y Then .BB_Max.y = v.y
            If .BB_Max.z < v.z Then .BB_Max.z = v.z
        End With
    End Sub
    Private Sub get_uv2(ByVal cnt As UInt32, ByVal f As MemoryStream)
        Dim uv2_reader = New BinaryReader(f)
        ReDim uv2(1)
        ReDim Preserve uv2(cnt)
        For i = 0 To cnt - 1
            uv2(i) = New vect2uv
            uv2(i).u = uv2_reader.ReadSingle
            uv2(i).v = uv2_reader.ReadSingle
        Next

        'r.Close()
        uv2_reader = Nothing
    End Sub
    Private Function unpackNormal_8_8_8(ByVal packed As UInt32) As vect3Norm
        'Console.WriteLine(packed.ToString("x"))
        Dim pkz, pky, pkx As Int32
        pkx = CLng(packed) And &HFF Xor 127
        pky = CLng(packed >> 8) And &HFF Xor 127
        pkz = CLng(packed >> 16) And &HFF Xor 127

        Dim x As Single = (pkx)
        Dim y As Single = (pky)
        Dim z As Single = (pkz)

        Dim p As New vect3Norm
        If x > 127 Then
            x = -128 + (x - 128)
        End If
        If y > 127 Then
            y = -128 + (y - 128)
        End If
        If z > 127 Then
            z = -128 + (z - 128)
        End If
        p.nx = CSng(x) / 127
        p.ny = CSng(y) / 127
        p.nz = CSng(z) / 127
        Dim len As Single = Sqrt((p.nx ^ 2) + (p.ny ^ 2) + (p.nz ^ 2))

        'avoid division by 0
        If len = 0.0F Then len = 1.0F
        'len = 1.0
        'reduce to unit size
        p.nx = (p.nx / len)
        p.ny = -(p.ny / len)
        p.nz = -(p.nz / len)
        'Console.WriteLine(p.x.ToString("0.000000") + " " + p.y.ToString("0.000000") + " " + p.z.ToString("0.000000"))
        Return p
    End Function
    Public Function unpackNormal(ByVal packed As UInt32)
        Dim pkz, pky, pkx As Int32
        pkz = packed And &HFFC00000
        pky = packed And &H4FF800
        pkx = packed And &H7FF

        Dim z As Int32 = pkz >> 22
        Dim y As Int32 = (pky << 10L) >> 21
        Dim x As Int32 = (pkx << 21L) >> 21
        Dim p As New vect3
      
        p.x = CSng(x) / 1023.0!
        p.y = CSng(y) / 1023.0!
        p.z = CSng(z) / 511.0!
        Dim len As Single = Sqrt((p.x ^ 2) + (p.y ^ 2) + (p.z ^ 2))

        'avoid division by 0
        If len = 0.0F Then len = 1.0F

        'reduce to unit size
        p.x = (p.x / len)
        p.y = (p.y / len)
        p.z = (p.z / len)
        Return p
    End Function

    Private Function unpackNormal_old(ByVal packed As UInt32) As vect3Norm
        Dim pkz, pky, pkx As Int32
        Dim p As New vect3Norm
        pkz = (packed >> 22) And &H3FF
        pky = (packed >> 11) And &H7FF
        pkx = packed And &H7FF

        If pkx > &H3FF Then
            p.nx = -CSng((pkx And &H3FF Xor &H3FF) + 1) / &H3FF
        Else
            p.nx = CSng(pkx / &H3FF)
        End If

        If pky > &H3FF Then
            p.ny = -CSng((pky And &H3FF Xor &H3FF) + 1) / &H3FF
        Else
            p.ny = CSng(pky / &H3FF)
        End If

        If pkz > &H1FF Then
            p.nz = -CSng((pkz And &H1FF Xor &H1FF) + 1) / &H1FF
        Else
            p.nz = CSng(pkz) / &H1FF
        End If
        Dim len As Single = Sqrt((p.nx ^ 2) + (p.ny ^ 2) + (p.nz ^ 2))

        'avoid division by 0
        If len = 0.0F Then len = 1.0F

        'reduce to unit size
        p.nx = (p.nx / len)
        p.ny = (p.ny / len)
        p.nz = (p.nz / len)
        Return p

    End Function
    Public Sub simple_vertex(ByVal mId As UInt32, ByVal cId As UInt32)
        Gl.glBegin(Gl.GL_TRIANGLES)
        With Models.models(mId).componets(cId)
            For i As UInt32 = 0 To (Models.models(mId).componets(cId)._count - 1) * 3 Step 3

                Gl.glMultiTexCoord2f(0, -.UVs(i).u, .UVs(i).v)
                Gl.glNormal3f(.normals(i).nx, .normals(i).ny, .normals(i).nz)
                Gl.glVertex3f(-.vertices(i).x, .vertices(i).y, .vertices(i).z)

                Gl.glMultiTexCoord2f(0, -.UVs(i + 1).u, .UVs(i + 1).v)
                Gl.glNormal3f(.normals(i + 1).nx, .normals(i + 1).ny, .normals(i + 1).nz)
                Gl.glVertex3f(-.vertices(i + 1).x, .vertices(i + 1).y, .vertices(i + 1).z)

                Gl.glMultiTexCoord2f(0, -.UVs(i + 2).u, .UVs(i + 2).v)
                Gl.glNormal3f(.normals(i + 2).nx, .normals(i + 2).ny, .normals(i + 2).nz)
                Gl.glVertex3f(-.vertices(i + 2).x, .vertices(i + 2).y, .vertices(i + 2).z)
            Next
        End With
        Gl.glEnd()

    End Sub
    Private Function convert_normal(ByVal v As vect3Norm) As Vector3D
        Dim v3d As New Vector3D
        v3d.X = v.nx
        v3d.Y = v.ny
        v3d.Z = v.nz
        Return v3d
    End Function
    Private Function convert_vertex(ByVal v As vect3) As Vector3D
        Dim v3d As New Vector3D
        v3d.X = v.x
        v3d.Y = v.y
        v3d.Z = v.z
        Return v3d
    End Function

    Private Sub ComputeTangentBasis_models(ByRef Md As Model_Section, ByVal i As Integer)
        Dim tangent, bitangent As Vector3D
        Dim n1, n2, n3 As Vector3D

        Dim t1, t2, t3 As Vector3D

        Dim b1, b2, b3 As Vector3D

        n1 = convert_normal(Md.normals(i + 0))
        n2 = convert_normal(Md.normals(i + 1))
        n3 = convert_normal(Md.normals(i + 2))
        n1.Normalize()
        n2.Normalize()
        n3.Normalize()
        'convert to vector3d type... they are WAY easier to do complex math with!!
        Dim v0 = convert_vertex(Md.vertices(i + 0))
        Dim v1 = convert_vertex(Md.vertices(i + 1))
        Dim v2 = convert_vertex(Md.vertices(i + 2))
        'compute new normal
        Dim n As Vector3D = CrossProduct(v0 - v2, v1 - v2)
        Dim uv0, uv1, uv2 As Vector3D
        uv0.x = Md.UVs(i + 0).u
        uv0.Y = Md.UVs(i + 0).v
        uv0.Z = 1.0
        uv1.x = Md.UVs(i + 1).u
        uv1.y = Md.UVs(i + 1).v
        uv1.Z = 1.0
        uv2.X = Md.UVs(i + 2).u
        uv2.y = Md.UVs(i + 2).v
        uv2.Z = 1.0
        'compute uv-wraping normal
        Dim flip As Double = 0.0
        Dim uvn As Vector3D = CrossProduct(uv0 - uv2, uv1 - uv2)
        flip = DotProduct(uvn, n)
        Dim deltaPos1 = v1 - v0
        Dim deltaPos2 = v2 - v0
        Dim deltaUV1 As vect2
        Dim deltaUV2 As vect2

        deltaUV1.x = (uv1.x - uv0.x)
        deltaUV1.y = (uv1.y - uv0.y)
        deltaUV2.x = (uv2.x - uv0.x)
        deltaUV2.y = (uv2.y - uv0.y)

        Dim f As Single = 1.0F / (deltaUV1.x * deltaUV2.y - deltaUV1.y * deltaUV2.x)

        tangent = f * (deltaPos1 * deltaUV2.y - deltaPos2 * deltaUV1.y)
        bitangent = f * (deltaPos2 * deltaUV1.x - deltaPos1 * deltaUV2.x)
        tangent.Normalize()
        bitangent.Normalize()
        If flip < 0.0 Then
            bitangent *= -1.0
        End If
        t1 = CrossProduct(bitangent, n1)
        t2 = CrossProduct(bitangent, n1)
        t3 = CrossProduct(bitangent, n1)

        t1 = tangent - (Vector3D.DotProduct(t1, n1) * n1)
        t2 = tangent - (Vector3D.DotProduct(t2, n1) * n2)
        t3 = tangent - (Vector3D.DotProduct(t3, n1) * n3)

        b1 = bitangent - (DotProduct(bitangent, n1) * n1)
        b2 = bitangent - (DotProduct(bitangent, n2) * n2)
        b3 = bitangent - (DotProduct(bitangent, n3) * n3)

        '
        t1.Normalize()
        t2.Normalize()
        t3.Normalize()
        b1.Normalize()
        b2.Normalize()
        b3.Normalize()

        'If DotProduct(CrossProduct(n1, t1), b1) < 0.0 Then t1 = t1 * -1.0
        'If DotProduct(CrossProduct(n2, t2), b2) < 0.0 Then t2 = t2 * -1.0
        'If DotProduct(CrossProduct(n3, t3), b3) < 0.0 Then t3 = t3 * -1.0

        Md.tangents(i + 0).nx = t1.X
        Md.tangents(i + 0).ny = t1.Y
        Md.tangents(i + 0).nz = t1.Z
        Md.binormals(i + 0).nx = b1.X
        Md.binormals(i + 0).ny = b1.Y
        Md.binormals(i + 0).nz = b1.Z

        Md.tangents(i + 1).nx = t2.X
        Md.tangents(i + 1).ny = t2.Y
        Md.tangents(i + 1).nz = t2.Z
        Md.binormals(i + 1).nx = b2.X
        Md.binormals(i + 1).ny = b2.Y
        Md.binormals(i + 1).nz = b2.Z

        Md.tangents(i + 2).nx = t3.X
        Md.tangents(i + 2).ny = t3.Y
        Md.tangents(i + 2).nz = t3.Z
        Md.binormals(i + 2).nx = b3.X
        Md.binormals(i + 2).ny = b3.Y
        Md.binormals(i + 2).nz = b3.Z

    End Sub
    Public Sub build_primitive(ByVal mId As UInt32, ByVal cId As UInt32)
        'this routine makes the triangles from the loaded primitive data.
        If Models.models(mId).componets(cId).multi_textured Then

            Gl.glBegin(Gl.GL_TRIANGLES)
            With Models.models(mId).componets(cId)
                For i As UInt32 = 0 To (Models.models(mId).componets(cId)._count - 1) * 3 Step 3
                    ComputeTangentBasis_models(Models.models(mId).componets(cId), i)
                    '1-------------
                    Gl.glMultiTexCoord2f(0, -.UVs(i).u, .UVs(i).v)
                    Gl.glMultiTexCoord2f(1, -.UV2s(i).u, .UV2s(i).v)
                    Gl.glMultiTexCoord3f(2, .tangents(i).nx, .tangents(i).ny, .tangents(i).nz)
                    'Gl.glMultiTexCoord3f(3, .binormals(i).nx, .binormals(i).ny, .binormals(i).nz)
                    Gl.glNormal3f(.normals(i).nx, .normals(i).ny, .normals(i).nz)
                    Gl.glVertex3f(-.vertices(i).x, .vertices(i).y, .vertices(i).z)
                    '2--------------
                    Gl.glMultiTexCoord2f(0, -.UVs(i + 1).u, .UVs(i + 1).v)
                    Gl.glMultiTexCoord2f(1, -.UV2s(i + 1).u, .UV2s(i + 1).v)
                    Gl.glMultiTexCoord3f(2, .tangents(i + 1).nx, .tangents(i + 1).ny, .tangents(i + 1).nz)
                    'Gl.glMultiTexCoord3f(3, .binormals(i + 1).nx, .binormals(i + 1).ny, .binormals(i + 1).nz)
                    Gl.glNormal3f(.normals(i + 1).nx, .normals(i + 1).ny, .normals(i + 1).nz)
                    Gl.glVertex3f(-.vertices(i + 1).x, .vertices(i + 1).y, .vertices(i + 1).z)
                    '3--------------
                    Gl.glMultiTexCoord2f(0, -.UVs(i + 2).u, .UVs(i + 2).v)
                    Gl.glMultiTexCoord2f(1, -.UV2s(i + 2).u, .UV2s(i + 2).v)
                    Gl.glMultiTexCoord3f(2, .tangents(i + 2).nx, .tangents(i + 2).ny, .tangents(i + 2).nz)
                    'Gl.glMultiTexCoord3f(3, .binormals(i + 2).nx, .binormals(i + 2).ny, .binormals(i + 2).nz)
                    Gl.glNormal3f(.normals(i + 2).nx, .normals(i + 2).ny, .normals(i + 2).nz)
                    Gl.glVertex3f(-.vertices(i + 2).x, .vertices(i + 2).y, .vertices(i + 2).z)
                Next
            End With
            Gl.glEnd()
        Else
            Gl.glBegin(Gl.GL_TRIANGLES)
            With Models.models(mId).componets(cId)
                For i As UInt32 = 0 To (Models.models(mId).componets(cId)._count - 1) * 3 Step 3

                    ComputeTangentBasis_models(Models.models(mId).componets(cId), i)

                    Gl.glMultiTexCoord2f(0, -.UVs(i).u, .UVs(i).v)
                    Gl.glMultiTexCoord3f(2, .tangents(i).nx, .tangents(i).ny, .tangents(i).nz)
                    'Gl.glMultiTexCoord3f(3, .binormals(i).nx, .binormals(i).ny, .binormals(i).nz)
                    Gl.glNormal3f(.normals(i).nx, .normals(i).ny, .normals(i).nz)
                    Gl.glVertex3f(-.vertices(i).x, .vertices(i).y, .vertices(i).z)

                    Gl.glMultiTexCoord2f(0, -.UVs(i + 1).u, .UVs(i + 1).v)
                    Gl.glMultiTexCoord3f(2, .tangents(i + 1).nx, .tangents(i + 1).ny, .tangents(i + 1).nz)
                    'Gl.glMultiTexCoord3f(3, .binormals(i + 1).nx, .binormals(i + 1).ny, .binormals(i + 1).nz)
                    Gl.glNormal3f(.normals(i + 1).nx, .normals(i + 1).ny, .normals(i + 1).nz)
                    Gl.glVertex3f(-.vertices(i + 1).x, .vertices(i + 1).y, .vertices(i + 1).z)

                    Gl.glMultiTexCoord2f(0, -.UVs(i + 2).u, .UVs(i + 2).v)
                    Gl.glMultiTexCoord3f(2, .tangents(i + 2).nx, .tangents(i + 2).ny, .tangents(i + 2).nz)
                    'Gl.glMultiTexCoord3f(3, .binormals(i + 2).nx, .binormals(i + 2).ny, .binormals(i + 2).nz)
                    Gl.glNormal3f(.normals(i + 2).nx, .normals(i + 2).ny, .normals(i + 2).nz)
                    Gl.glVertex3f(-.vertices(i + 2).x, .vertices(i + 2).y, .vertices(i + 2).z)
                Next
            End With
            Gl.glEnd()

        End If
        'clean up and..  save a byte or 2...thousand
        ReDim Models.models(mId).componets(cId).vertices(0)
        ReDim Models.models(mId).componets(cId).UVs(0)
        ReDim Models.models(mId).componets(cId).UV2s(0)
        ReDim Models.models(mId).componets(cId).normals(0)
        ReDim Models.models(mId).componets(cId).tangents(0)
        ReDim Models.models(mId).componets(cId).binormals(0)

    End Sub
    Public Sub make_prim_list(ByVal id As UInt32)
        If Models.models(id).componets IsNot Nothing Then
            Dim Nmods As Integer
            If Models.models(id)._count = 0 Then
                GoTo notToday
            End If
            loaded_models.names.Add(Models.Model_list(id))
            'Add this model to the list of loaded models so we dont load it again.
            For i = 0 To Models.models(id)._count - 1
                Nmods = loaded_models._count
                loaded_models.stack(Nmods).isBuilding = Models.models(id).isBuilding
                ReDim loaded_models.stack(Nmods).matrix(0)
                ReDim loaded_models.stack(Nmods).model_id(0)
                loaded_models.stack(Nmods).model_id(0) = id
                loaded_models.stack(Nmods).matrix(0) = New matrix_
                ReDim loaded_models.stack(Nmods).matrix(0).matrix(16)
                'Try
                If Models.matrix IsNot Nothing Then
                    loaded_models.stack(Nmods).matrix(0).matrix = Models.matrix(id).matrix

                End If
                'Catch ex As Exception
                'End Try
                loaded_models.stack(Nmods).model_count = 1
                If Models.models(id).componets(i)._count > 0 Then

                    Dim cl = Gl.glGenLists(1)
                    Models.models(id).componets(i).callList_ID = cl
                    ReDim Preserve loaded_models.stack(Nmods).dispId(i + 1)
                    ReDim Preserve loaded_models.stack(Nmods).textId(i + 1)
                    ReDim Preserve loaded_models.stack(Nmods).color_name(i + 1)
                    ReDim Preserve loaded_models.stack(Nmods).text2Id(i + 1)
                    ReDim Preserve loaded_models.stack(Nmods).color2_name(i + 1)
                    ReDim Preserve loaded_models.stack(Nmods).normID(i + 1)
                    ReDim Preserve loaded_models.stack(Nmods).bumped(i + 1)
                    ReDim Preserve loaded_models.stack(Nmods).alphaRef(i + 1)
                    ReDim Preserve loaded_models.stack(Nmods).alphaTestEnable(i + 1)
                    ReDim Preserve loaded_models.stack(Nmods).GAmap(i + 1)
                    ReDim Preserve loaded_models.stack(Nmods).multi_textured(i + 1)


                    loaded_models.stack(Nmods).dispId(i) = cl
                    loaded_models.stack(Nmods).textId(i) = Models.models(id).componets(i).color_id
                    loaded_models.stack(Nmods).color_name(i) = Models.models(id).componets(i).color_name
                    loaded_models.stack(Nmods).text2Id(i) = Models.models(id).componets(i).color2_Id
                    loaded_models.stack(Nmods).color2_name(i) = Models.models(id).componets(i).color2_name
                    loaded_models.stack(Nmods).normID(i) = Models.models(id).componets(i).normal_Id
                    loaded_models.stack(Nmods).bumped(i) = Models.models(id).componets(i).bumped
                    loaded_models.stack(Nmods).alphaRef(i) = Models.models(id).componets(i).alphaRef
                    loaded_models.stack(Nmods).alphaTestEnable(i) = Models.models(id).componets(i).alphaTestEnable
                    loaded_models.stack(Nmods).GAmap(i) = Models.models(id).componets(i).GAmap
                    loaded_models.stack(Nmods).multi_textured(i) = Models.models(id).componets(i).multi_textured

                    loaded_models.stack(Nmods)._count = i + 1

                    Gl.glNewList(Models.models(id).componets(i).callList_ID, Gl.GL_COMPILE)
                    UVs_ON = True
                    If short_vertex_type Then
                        simple_vertex(id, i)
                    Else
                        build_primitive(id, i)
                    End If
                    UVs_ON = False
                    Gl.glEndList()
                    Gl.glFinish()
                End If
            Next
            If Models.models(id)._count - 1 >= 0 Then
                ReDim Preserve loaded_models.stack(Nmods + 1)
                loaded_models._count += 1
            Else
                Dim k As Integer = 0
            End If
nottoday:
        End If

    End Sub

    Public Function build_model_textures(ByVal mod_id As Integer, ByVal peice As Integer) As Boolean
        'Dim sw As New Stopwatch
        Dim cnt As Integer = 0
        Dim got_it As Boolean = False
        'sw.Start()
        With Models.models(mod_id).componets(peice)
            Dim texID(4) As Integer
            'Dim app_local As String = Application.StartupPath.ToString
            Dim fs As String = .color_name
            Dim pre_s As String = ""
            If fs Is Nothing Then
                fs = .color2_name
                If fs Is Nothing Then
                    Return False
                End If
            End If
Good_image:
            Dim ms As New MemoryStream
            cnt = texture_cache.Length
            '	'Check if we have this texture generated already.....
            For i = 0 To cnt - 1
                If .color_name = texture_cache(i).name Then
                    .color_id = texture_cache(i).textureID
                    saved_texture_loads += 1
                    got_it = True
                    GoTo jump_color
                End If
            Next

            .color_name = .color_name.Replace("//", "/")
            If .color_name Is Nothing Then GoTo jump_color
            .color_name = .color_name.Replace(".tga", ".dds")
            'If .color_name.Contains("mle055") Then
            '    Stop
            'End If
            Try
                Dim entry1 As Ionic.Zip.ZipEntry
                entry1 = get_shared(.color_name)
                If entry1 Is Nothing Then
                    entry1 = get_shared(.color_name)
                    If entry1 Is Nothing Then
                        Debug.Write("cant find: " & .color_name & vbCrLf)
                    End If
                    entry1.Extract(ms)
                Else
                    entry1.Extract(ms)
                End If
            Catch ex As Exception
                Stop
            End Try
            'If .color_name.Contains("env404_Sign.dds") Then
            '    .color_id = Load_DDS_File(Application.StartupPath + "\Resources\env404_Sign_hd.dds")
            'Else
            If InStr(Models.Model_list(mod_id), "border_") > 0 Then
                .color_id = get_texture_no_alpha(ms, True)
            Else
                .color_id = get_texture(ms, frmMain.m_low_quality_textures.Checked)
            End If
            'End If

            ms.Close()
            ms.Dispose()
            frmMapInfo.I__Model_Textures_tb.Text += "Color: " + .color_name + vbCrLf
            cnt = texture_cache.Length
            ReDim Preserve texture_cache(cnt)
            texture_cache(cnt - 1) = New tree_textures_
            texture_cache(cnt - 1).name = .color_name
            texture_cache(cnt - 1).textureID = .color_id
            'GoTo jump_normal
            '------------------------------------------------------------------------------------- diffuse2/color2 map
            '------------------------- # 2 -----------------------------'
jump_color:
            If .multi_textured Then
                cnt = texture_cache.Length
                '	'Check if we have this texture generated already.....
                For i = 0 To cnt - 1
                    If .color2_name = texture_cache(i).name2 Then
                        .color2_Id = texture_cache(i).texture2ID
                        saved_texture_loads += 1
                        got_it = True
                        GoTo check_normal
                    End If
                Next
                If .color2_name = "none" Then GoTo check_normal
                .color2_name = .color2_name.Replace(".tga", ".dds")
                Dim entry1 = get_shared(.color2_name)
                ms = New MemoryStream
                If entry1 Is Nothing Then
                    entry1 = get_shared(.color2_name)
                    If entry1 Is Nothing Then
                        Debug.Write("cant find: " & .color2_Id & vbCrLf)
                    End If
                    entry1.Extract(ms)
                Else
                    entry1.Extract(ms)
                End If
                .color2_Id = get_texture(ms, frmMain.m_low_quality_textures.Checked)
                ms.Close()
                ms.Dispose()
                frmMapInfo.I__Model_Textures_tb.Text += "Color2: " + .color2_name + vbCrLf
                cnt = texture_cache.Length
                ReDim Preserve texture_cache(cnt)
                texture_cache(cnt - 1) = New tree_textures_
                texture_cache(cnt - 1).name2 = .color2_name
                texture_cache(cnt - 1).texture2ID = .color2_Id
            End If
            '----------------------------------------------------------------------------------- normal map
            '------------------------- # 3 -----------------------------'
check_normal:

            If .bumped Then
                cnt = texture_cache.Length
                '	'Check if we have this texture generated already.....
                For i = 0 To cnt - 1
                    If .normal_name = texture_cache(i).normalname Then
                        .normal_Id = texture_cache(i).textureNormID
                        saved_texture_loads += 1
                        If .normal_name.Contains("_ANM") Then
                            .GAmap = True
                        Else
                            .GAmap = False
                        End If
                        Return True ' at this point we know we have a texture
                    End If
                Next
                If .normal_name Is Nothing Then GoTo jump_normal
                If .normal_name.Contains("_ANM") Then
                    .GAmap = True
                Else
                    .GAmap = False
                End If
                Dim entry1 = get_shared(.normal_name)
                ms = New MemoryStream
                If entry1 Is Nothing Then
                    entry1 = get_shared(.normal_name)
                    If entry1 Is Nothing Then
                        Debug.Write("cant find: " & .normal_name & vbCrLf)
                    End If
                    entry1.Extract(ms)
                Else
                    entry1.Extract(ms)
                End If
                .normal_Id = get_normal_texture(ms, frmMain.m_low_quality_textures.Checked)
                frmMapInfo.I__Model_Textures_tb.Text += "Normal: " + .normal_name + vbCrLf
                cnt = texture_cache.Length
                ReDim Preserve texture_cache(cnt)
                texture_cache(cnt - 1) = New tree_textures_
                texture_cache(cnt - 1).normalname = .normal_name
                texture_cache(cnt - 1).textureNormID = .normal_Id
            End If
jump_normal:

            ms.Close()
            ms.Dispose()

        End With

        Return got_it
    End Function

    Public Sub build_water()
        water.size_.x = BWWa.bwwa_t1(0).width
        water.size_.z = BWWa.bwwa_t1(0).height
        water.orientation = BWWa.bwwa_t1(0).orientation
        water.position.x = BWWa.bwwa_t1(0).position.x
        water.position.y = BWWa.bwwa_t1(0).position.y
        water.position.z = BWWa.bwwa_t1(0).position.z

        ReDim water.matrix(16)
        'build the waters matrix;
        Gl.glPushMatrix()
        Gl.glLoadIdentity()
        Gl.glMatrixMode(Gl.GL_MODELVIEW)
        Gl.glTranslatef(-water.position.x, water.position.y, water.position.z)
        Gl.glRotatef(-water.orientation * 57.2957795, 0.0, 1.0, 0.0)
        Gl.glScalef(water.size_.x, 1.0, water.size_.z)

        Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, water.matrix)
        Gl.glPopMatrix()

        water.aspect = water.size_.x / water.size_.z
        get_water_corners()
        '-----------------------------------1
        water.displayID_cube = Gl.glGenLists(1)
        Gl.glNewList(water.displayID_cube, Gl.GL_COMPILE)
        Gl.glBegin(Gl.GL_QUADS)
        make_water_box()
        Gl.glEnd()
        Gl.glEndList()
        'make plane
        water.displayID_plane = Gl.glGenLists(1)
        Gl.glNewList(water.displayID_plane, Gl.GL_COMPILE)
        Gl.glBegin(Gl.GL_QUADS)

        Gl.glTexCoord2f(0.0, 6.0)
        Gl.glNormal3f(0.0, 1.0, 0.0)
        Gl.glVertex3f(-0.5, 0.0, 0.5)

        Gl.glTexCoord2f(6.0, 6.0)
        Gl.glNormal3f(0.0, 1.0, 0.0)
        Gl.glVertex3f(0.5, 0.0, 0.5)

        Gl.glTexCoord2f(6.0, 0.0)
        Gl.glNormal3f(0.0, 1.0, 0.0)
        Gl.glVertex3f(0.5, 0.0, -0.5)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glNormal3f(0.0, 1.0, 0.0)
        Gl.glVertex3f(-0.5, 0.0, -0.5)
        Gl.glEnd()
        Gl.glEndList()
        load_animated_water_NMs()
        load_foam_texture()
    End Sub
    Private Sub make_water_box()
        With water
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
    Private Sub get_water_corners()
        With water
            ReDim .BB(8)
            ' left side -----------
            .lbl.x = -0.5 'left bottom left
            .lbl.y = -100.0
            .lbl.z = -0.5
            .BB(0) = .lbl
            '
            .lbr.x = 0.5 ' left bottom right
            .lbr.y = -100.0
            .lbr.z = -0.5
            .BB(1) = .lbr
            '
            .ltl.x = -0.5 'left top left
            .ltl.y = 0.001
            .ltl.z = -0.5
            .BB(2) = .ltl
            '
            .ltr.x = 0.5 ' left top right
            .ltr.y = 0.001
            .ltr.z = -0.5
            .BB(3) = .ltr
            ' right side ----------
            .rbl.x = -0.5 ' right bottom left
            .rbl.y = -100.0
            .rbl.z = 0.5
            .BB(4) = .rbl
            '
            .rbr.x = 0.5 ' right bottom right
            .rbr.y = -100.0
            .rbr.z = 0.5
            .BB(5) = .rbr
            '
            .rtl.x = -0.5 ' right top left
            .rtl.y = 0.001
            .rtl.z = 0.5
            .BB(6) = .rtl
            '
            .rtr.x = 0.5 ' right top right
            .rtr.y = 0.001
            .rtr.z = 0.5
            .BB(7) = .rtr

        End With
        For i = 0 To 7
            water.BB(i) = translate_to(water.BB(i), water.matrix)
        Next
    End Sub
    Private Sub load_animated_water_NMs()
        Using w As Ionic.Zip.ZipFile = ZipFile.Read(Application.StartupPath + "\Resources\water.zip")
            Dim cnt As Integer = 0
            For Each f In w
                Dim ms As New MemoryStream
                f.Extract(ms)
                Debug.WriteLine(f.FileName)
                animated_water_ids(cnt) = get_texture(ms, False)
                cnt += 1
            Next

        End Using
    End Sub
    Private Sub load_foam_texture()
        Using Z As Ionic.Zip.ZipFile = ZipFile.Read(GAME_PATH + "\res\packages\misc.pkg")
            Dim entry = Z("\system\maps\waves3_N.dds")
            If Z IsNot Nothing Then
                Dim ms As New MemoryStream
                entry.Extract(ms)
                water.foam_id = get_texture(ms, False)
            End If
        End Using
    End Sub
    Public Sub get_tree_branch_texture(ByVal diffuse As String, ByVal tree As Integer)

        With Trees
            'see if this texture is created
            For i = 0 To tree_textures.Length - 1
                If diffuse = tree_textures(i).name Then
                    .flora(tree).branch_textureID = tree_textures(i).textureID
                    saved_texture_loads += 1
                    Return
                End If
            Next
            Dim ms As New MemoryStream
            Try
                Dim entry1 As Ionic.Zip.ZipEntry = active_pkg(diffuse)
                If entry1 Is Nothing Then
                    entry1 = get_shared(diffuse)
                    If entry1 Is Nothing Then
                        Debug.Write("cant find: " & diffuse & vbCrLf)
                    End If
                    entry1.Extract(ms)
                Else
                    entry1.Extract(ms)
                End If
            Catch ex As Exception
                Stop
            End Try
            frmMapInfo.I__Tree_Textures_tb.Text += "Branch Texture: " + diffuse + vbCrLf
            .flora(tree).branch_textureID = get_tree_texture(ms, False)
            Dim len = tree_textures.Length
            ReDim Preserve tree_textures(len)
            tree_textures(len - 1).name = diffuse
            tree_textures(len - 1).textureID = .flora(tree).branch_textureID
        End With
    End Sub
    Public Sub get_tree_branch_normalmap(ByVal normal As String, ByVal tree As Integer)


        With Trees
            'see if this texture is created
            For i = 0 To tree_textures.Length - 1
                If normal = tree_textures(i).normalname Then
                    .flora(tree).branch_normalID = tree_textures(i).textureNormID
                    saved_texture_loads += 1
                    Return
                End If
            Next
            Dim ms As New MemoryStream
            Try
                Dim entry1 As Ionic.Zip.ZipEntry = active_pkg(normal)
                If entry1 Is Nothing Then
                    entry1 = get_shared(normal)
                    If entry1 Is Nothing Then
                        Debug.Write("cant find: " & normal & vbCrLf)
                    End If
                    entry1.Extract(ms)
                Else
                    entry1.Extract(ms)
                End If
            Catch ex As Exception
                Stop
            End Try
            frmMapInfo.I__Tree_Textures_tb.Text += "Branch norm: " + normal + vbCrLf
            .flora(tree).branch_normalID = get_normal_texture(ms, False)
            Dim len = tree_textures.Length
            ReDim Preserve tree_textures(len)
            tree_textures(len - 1).normalname = normal
            tree_textures(len - 1).textureNormID = .flora(tree).branch_normalID
        End With
    End Sub

    Public Sub get_tree_billboard_texture(ByVal diffuse As String, ByVal tree As Integer)
        'If diffuse = speedtree_name Then
        '    Trees.flora(tree).branch_textureID = speedtree_imageID
        '    Return
        'End If
        With Trees
            'see if this texture is created
            For i = 0 To tree_textures.Length - 1
                If diffuse = tree_textures(i).name Then
                    .flora(tree).billboard_textureID = tree_textures(i).textureID
                    saved_texture_loads += 1
                    Return
                End If
            Next
            Dim ms As New MemoryStream
            Try
                Dim entry1 As Ionic.Zip.ZipEntry = active_pkg(diffuse)
                If entry1 Is Nothing Then
                    entry1 = get_shared(diffuse)
                    If entry1 Is Nothing Then
                        Debug.Write("cant find: " & diffuse & vbCrLf)
                    End If
                    entry1.Extract(ms)
                Else
                    entry1.Extract(ms)
                End If
            Catch ex As Exception
                Stop
            End Try
            frmMapInfo.I__Tree_Textures_tb.Text += "Billboard Texture: " + diffuse + vbCrLf
            .flora(tree).billboard_textureID = get_tree_texture(ms, False)
            Dim len = tree_textures.Length
            ReDim Preserve tree_textures(len)
            tree_textures(len - 1).name = diffuse
            tree_textures(len - 1).textureID = .flora(tree).billboard_textureID
        End With
    End Sub
    Public Sub get_tree_billboard_normalmap(ByVal normal As String, ByVal tree As Integer)

        'If normal = speedtree_normalmap Then
        '    Trees.flora(tree).branch_normalID = speedtree_NormalMapID
        '    Return
        'End If
        With Trees
            'see if this texture is created
            For i = 0 To tree_textures.Length - 1
                If normal = tree_textures(i).normalname Then
                    .flora(tree).billboard_normalID = tree_textures(i).textureNormID
                    saved_texture_loads += 1
                    Return
                End If
            Next
            Dim ms As New MemoryStream
            Try
                Dim entry1 As Ionic.Zip.ZipEntry = active_pkg(normal)
                If entry1 Is Nothing Then
                    entry1 = get_shared(normal)
                    If entry1 Is Nothing Then
                        Debug.Write("cant find: " & normal & vbCrLf)
                    End If
                    entry1.Extract(ms)
                Else
                    entry1.Extract(ms)
                End If
            Catch ex As Exception
                Stop
            End Try
            frmMapInfo.I__Tree_Textures_tb.Text += "Billboard norm: " + normal + vbCrLf
            .flora(tree).billboard_normalID = get_normal_texture(ms, False)
            Dim len = tree_textures.Length
            ReDim Preserve tree_textures(len)
            tree_textures(len - 1).normalname = normal
            tree_textures(len - 1).textureNormID = .flora(tree).billboard_normalID
        End With
    End Sub

    Public Sub build_branch_model(ByVal tree As Integer, ByVal tree_data As tree_)
        Dim vert, t As vect3
        Dim uv As vect2
        Dim norm As vect3

        Dim tbuf(tree_data.b_indices.Length) As Integer
        Dim c As Integer = 0
        For i = tree_data.b_indices.Length - 2 To 0 Step -1
            tbuf(i) = tree_data.b_indices(c)
            c += 1
        Next

        Dim id As Integer = Gl.glGenLists(1)
        Trees.flora(tree).branch_displayID = id
        Gl.glNewList(id, Gl.GL_COMPILE)
        Gl.glBegin(Gl.GL_TRIANGLE_STRIP)

        For i = 0 To tree_data.b_indices.Length - 2
            vert.x = -tree_data.b_vert((tbuf(i)) * 13 + 0)
            vert.y = tree_data.b_vert((tbuf(i)) * 13 + 1)
            vert.z = tree_data.b_vert((tbuf(i)) * 13 + 2)
            norm.x = -tree_data.b_vert((tbuf(i)) * 13 + 3)
            norm.y = tree_data.b_vert((tbuf(i)) * 13 + 4)
            norm.z = tree_data.b_vert((tbuf(i)) * 13 + 5)
            uv.x = tree_data.b_vert((tbuf(i)) * 13 + 6)
            uv.y = tree_data.b_vert((tbuf(i)) * 13 + 7)
            t.x = -tree_data.b_vert((tbuf(i)) * 13 + 10)
            t.y = tree_data.b_vert((tbuf(i)) * 13 + 11)
            t.z = tree_data.b_vert((tbuf(i)) * 13 + 12)

            Gl.glMultiTexCoord2f(0, -uv.x, uv.y)
            Gl.glMultiTexCoord3f(2, t.x, t.y, t.z)
            Gl.glNormal3f(norm.x, norm.y, norm.z)
            Gl.glVertex3f(vert.x, vert.y, vert.z)
        Next
        Gl.glEnd()
        Gl.glEndList()
    End Sub
    Public Sub build_frond_model(ByVal tree As Integer, ByVal tree_data As tree_)
        Dim vert, t As vect3
        Dim uv As vect2
        Dim norm As vect3
        Dim id As Integer = Gl.glGenLists(1)
        Trees.flora(tree).frond_displayID = id
        Gl.glNewList(id, Gl.GL_COMPILE)
        Gl.glBegin(Gl.GL_TRIANGLE_STRIP)
        ' For k = 0 To tree_data.strip_count - 1
        Dim k As Integer = 0
        For i = 0 To tree_data.strip_inds(k).indices.Length - 2
            vert.x = -tree_data.f_vert((tree_data.strip_inds(k).indices(i)) * 13 + 0)
            vert.y = tree_data.f_vert((tree_data.strip_inds(k).indices(i)) * 13 + 1)
            vert.z = tree_data.f_vert((tree_data.strip_inds(k).indices(i)) * 13 + 2)
            norm.x = -tree_data.f_vert((tree_data.strip_inds(k).indices(i)) * 13 + 3)
            norm.y = tree_data.f_vert((tree_data.strip_inds(k).indices(i)) * 13 + 4)
            norm.z = tree_data.f_vert((tree_data.strip_inds(k).indices(i)) * 13 + 5)
            uv.x = tree_data.f_vert((tree_data.strip_inds(k).indices(i)) * 13 + 6)
            uv.y = tree_data.f_vert((tree_data.strip_inds(k).indices(i)) * 13 + 7)
            t.x = -tree_data.f_vert((tree_data.strip_inds(k).indices(i)) * 13 + 10)
            t.y = tree_data.f_vert((tree_data.strip_inds(k).indices(i)) * 13 + 11)
            t.z = tree_data.f_vert((tree_data.strip_inds(k).indices(i)) * 13 + 12)

            Gl.glMultiTexCoord2f(0, -uv.x, uv.y)
            Gl.glMultiTexCoord3f(2, t.x, t.y, t.z)
            Gl.glNormal3f(norm.x, norm.y, norm.z)
            Gl.glVertex3f(vert.x, vert.y, vert.z)
        Next
        ' Next k
        Gl.glEnd()
        Gl.glEndList()
    End Sub
    Public Sub build_leaf_model(ByVal tree As Integer, ByVal tree_data As tree_)
        If tree_data.l_indices.Length = 1 Then
            Return
        End If
        If tree_data.l_vert.Length = 1 Then
            Return
        End If
        Dim type = tree_data.l_vert(15) ' corner index
        If type = 4 Then ' 4 means its a mesh and not a card
            build_leaf_mesh(tree, tree_data)
            Return
        End If
        Dim m = Trees.matrix(tree).matrix
        Dim sx = Sqrt((m(0) ^ 2) + (m(1) ^ 2) + (m(3) ^ 2))
        Dim sy = Sqrt((m(4) ^ 2) + (m(5) ^ 2) + (m(6) ^ 2))
        Dim sz = Sqrt((m(8) ^ 2) + (m(9) ^ 2) + (m(10) ^ 2))
        Dim tree_scale As Single = (sx + sy + sz) * 0.3333

        Dim vx() As Single = {-0.5!, 0.5!, 0.5!, -0.5!}
        Dim vy() As Single = {0.5!, 0.5!, -0.5!, -0.5!}

        Dim uv As vect2
        Dim norm As vect3
        Dim id As Integer = Gl.glGenLists(1)
        If id = 0 Then
            Stop ' cant make list?
        End If
        Trees.flora(tree).leaf_displayID = id
        Dim tangent As vect3
        Dim vn, vn2 As Integer
        Dim sizex, sizey As Single
        Dim pivot As vect2
        Dim p, pp As New vect3
        Dim cnt As Integer = 0
        Dim idx, rot As vect2
        Gl.glNewList(id, Gl.GL_COMPILE)
        'Gl.glBegin(Gl.GL_TRIANGLES)
        Gl.glBegin(Gl.GL_QUADS)
        Dim sb As New StringBuilder
        For i = 0 To tree_data.l_indices.Length - 3
            'struct LeafVertex
            '
            '00	00 Vector3 position_;
            '12	03 Vector3 normal_;
            '24	06 FLOAT   texCoords_[2];
            '32	08 FLOAT   windInfo_[2];
            '40	10 FLOAT   rotInfo_[2];
            '48	12 FLOAT   geomInfo_[2]; 
            '56	14 FLOAT   extraInfo_[3];
            '68	17 FLOAT   pivotInfo_[2];
            '76	19 Vector3 Tangent;
            If cnt = 6 Then
                cnt = 0
                'Stop
            End If
            If cnt = 2 Or cnt = 3 Then
                GoTo skip_this_vert
            End If
            p.x = -tree_data.l_vert((tree_data.l_indices(i) * 22) + 0)
            p.y = tree_data.l_vert((tree_data.l_indices(i) * 22) + 1)
            p.z = tree_data.l_vert((tree_data.l_indices(i) * 22) + 2)

            norm.x = -tree_data.l_vert((tree_data.l_indices(i) * 22) + 3)
            norm.y = tree_data.l_vert((tree_data.l_indices(i) * 22) + 4)
            norm.z = tree_data.l_vert((tree_data.l_indices(i) * 22) + 5)

            uv.x = tree_data.l_vert((tree_data.l_indices(i) * 22) + 6)
            uv.y = tree_data.l_vert((tree_data.l_indices(i) * 22) + 7)

            rot.x = tree_data.l_vert((tree_data.l_indices(i) * 22) + 17)
            rot.y = tree_data.l_vert((tree_data.l_indices(i) * 22) + 18)

            vn = tree_data.l_vert((tree_data.l_indices(i) * 22) + 15) ' corner index
            vn2 = tree_data.l_vert((tree_data.l_indices(i) * 22) + 16)
            If vn2 <> 1.0 Then
                Stop ' has NEVER been hit. It is always 1.0
            End If
            sizey = tree_data.l_vert((tree_data.l_indices(i) * 22) + 12)
            sizex = tree_data.l_vert((tree_data.l_indices(i) * 22) + 13)

            pivot.x = tree_data.l_vert((tree_data.l_indices(i) * 22) + 10)
            pivot.y = tree_data.l_vert((tree_data.l_indices(i) * 22) + 11)

            'not sure 100% sure.. This might also be the BiTangent
            tangent.x = -tree_data.l_vert((tree_data.l_indices(i) * 22) + 19)
            tangent.y = tree_data.l_vert((tree_data.l_indices(i) * 22) + 20)
            tangent.z = tree_data.l_vert((tree_data.l_indices(i) * 22) + 21)

            'debug string
            'sb.Append(vn.ToString("00") + vbCrLf)
            'If vn = 4 Then
            '    sb.Append("mesh" + vbCrLf)
            'End If
            idx.x = vx(vn)
            idx.y = vy(vn)
            Dim corner As vect3
            corner.x = vx(vn) 'get corner x
            corner.y = vy(vn) 'get corner y
            corner.x *= tree_scale
            corner.y *= tree_scale
            corner.x *= sizex
            corner.y *= sizey
            'Debug.WriteLine("X:" & corner.x.ToString("0.000000") & "  Y:" & corner.y.ToString("0.000000"))
            'Debug.WriteLine("U:" & -uv.x.ToString("0.000000") & "  V:" & uv.y.ToString("0.000000") + vbCrLf)
            Gl.glMultiTexCoord2f(0, -uv.x, uv.y)
            Gl.glMultiTexCoord3f(1, corner.x, corner.y, 0.0)
            Gl.glMultiTexCoord3f(2, tangent.x, tangent.y, tangent.z)
            Gl.glNormal3f(norm.x, norm.y, norm.z)
            Gl.glVertex3f(p.x, p.y, p.z)

skip_this_vert:
            cnt += 1
        Next
        Gl.glEnd()
        Gl.glEndList()
    End Sub
    Public Sub build_leaf_mesh(ByVal tree As Integer, ByVal tree_data As tree_)
        'creates a mesh and not leaf cards.
        If tree_data.l_indices.Length = 1 Then
            Return
        End If
        If tree_data.l_vert.Length = 1 Then
            Return
        End If

        Dim uv As vect2
        Dim norm As vect3
        Dim id As Integer = Gl.glGenLists(1)
        Trees.flora(tree).leaf_displayID = id
        Dim tangent As vect3
        Dim p, pp As New vect3
        Dim cnt As Integer = 0
        'Dim rot As vect2
        Gl.glNewList(id, Gl.GL_COMPILE)
        Gl.glBegin(Gl.GL_TRIANGLES)
        For i = 0 To tree_data.l_indices.Length - 3
            'struct LeafVertex
            '
            '00	00 Vector3 position_;
            '12	03 Vector3 normal_;
            '24	06 FLOAT   texCoords_[2];
            '32	08 FLOAT   windInfo_[2];
            '40	10 FLOAT   rotInfo_[2];
            '48	12 FLOAT   geomInfo_[2]; 
            '56	14 FLOAT   extraInfo_[3];
            '68	17 FLOAT   pivotInfo_[2];
            '76	19 Vector3 Tangent;

            p.x = -tree_data.l_vert((tree_data.l_indices(i) * 22) + 0)
            p.y = tree_data.l_vert((tree_data.l_indices(i) * 22) + 1)
            p.z = tree_data.l_vert((tree_data.l_indices(i) * 22) + 2)

            norm.x = -tree_data.l_vert((tree_data.l_indices(i) * 22) + 3)
            norm.y = tree_data.l_vert((tree_data.l_indices(i) * 22) + 4)
            norm.z = tree_data.l_vert((tree_data.l_indices(i) * 22) + 5)

            uv.x = tree_data.l_vert((tree_data.l_indices(i) * 22) + 6)
            uv.y = tree_data.l_vert((tree_data.l_indices(i) * 22) + 7)

            'rot.x = tree_data.l_vert((tree_data.l_indices(i) * 22) + 17)
            'rot.y = tree_data.l_vert((tree_data.l_indices(i) * 22) + 18)

            'vn = tree_data.l_vert((tree_data.l_indices(i) * 22) + 15) ' corner index
            'vn2 = tree_data.l_vert((tree_data.l_indices(i) * 22) + 16)
            'sizey = tree_data.l_vert((tree_data.l_indices(i) * 22) + 12)
            'sizex = tree_data.l_vert((tree_data.l_indices(i) * 22) + 13)

            'pivot.x = tree_data.l_vert((tree_data.l_indices(i) * 22) + 10)
            'pivot.y = tree_data.l_vert((tree_data.l_indices(i) * 22) + 11)

            'not sure 100% sure.. This might also be the BiNormal
            tangent.x = -tree_data.l_vert((tree_data.l_indices(i) * 22) + 19)
            tangent.y = tree_data.l_vert((tree_data.l_indices(i) * 22) + 20)
            tangent.z = tree_data.l_vert((tree_data.l_indices(i) * 22) + 21)

            ' just use these directly.
            Gl.glMultiTexCoord2f(0, -uv.x, uv.y)
            Gl.glMultiTexCoord3f(1, 0.0, 0.0, 0.0)
            Gl.glMultiTexCoord3f(2, tangent.x, tangent.y, tangent.z)
            Gl.glNormal3f(norm.x, norm.y, norm.z)
            Gl.glVertex3f(p.x, p.y, p.z)
        Next
        Gl.glEnd()
        Gl.glEndList()
    End Sub
    '========================================================================================
    Public Sub build_tree(ByVal tree As UInt32, ByVal map_name As String)

        'This took days of looking at hex to figure out.. 
        'As im typing this, it has been 2 weeks of constant
        'work. In the end.. I guessed at what the data was
        'and got it working.
        'There are differet vertex data types in each file
        'and they change depending on the plants geo.
        'I have no idea how to pre-detect the types so..
        'the code branches depending on what it finds.
        'Not the best way but it works ok.
        'There are still a few trees that are not created 100%
        '
        'ctree files are pre-calculated tree structures.
        '
        'Alot of the code is hacked together to get things to work
        'due to the fact that there are so many different variations
        'of the same thing. Why it's like this, I have no idea.
        '
        'It creates (if its there)
        '   1. trunk/branch models
        '   2. Fronds. Some look like small branches.
        '   3. Leafs. Some trees have no leafs such as Palms
        '   4. 360 degree view billboards.
        '
        ReDim Preserve Trees.flora(tree + 1)
        Trees.flora(tree) = New flora_ 'make a new tree holder
        '--------------------------------------------------------------------
        '--------------------------------------------------------------------
        'first off .. lets look for this tree in the cache.
        For i = 0 To treeCache.Length - 1
            If treeCache(i).name = Trees.Tree_list(tree) Then
                Trees.flora(tree).branch_displayID = treeCache(i).branch_displayID
                Trees.flora(tree).branch_textureID = treeCache(i).branch_textureID
                Trees.flora(tree).branch_normalID = treeCache(i).branch_normalID
                Trees.flora(tree).frond_displayID = treeCache(i).frond_displayID
                Trees.flora(tree).leaf_displayID = treeCache(i).leaf_displayID
                Trees.flora(tree).billboard_displayID = treeCache(i).billboard_displayID
                Trees.flora(tree).billboard_textureID = treeCache(i).billboard_textureID
                Trees.flora(tree).billboard_normalID = treeCache(i).billboard_normalID

                ReDim Preserve treeCache(i).matrix_list(treeCache(i).tree_cnt + 1)
                ReDim Preserve treeCache(i).tree_id(treeCache(i).tree_cnt + 1)
                ReDim Preserve treeCache(i).BB(treeCache(i).tree_cnt + 1)
                treeCache(i).matrix_list(treeCache(i).tree_cnt) = New matrix_

                Dim bb1 As New BB_
                bb1.BB_Max = treeCache(i).D_BB.BB_Max
                bb1.BB_Min = treeCache(i).D_BB.BB_Min
                ReDim bb1.BB(16)

                get_translated_bb_tree(bb1, tree)
                treeCache(i).BB(treeCache(i).tree_cnt) = bb1
                treeCache(i).matrix_list(treeCache(i).tree_cnt).matrix = speedtree_matrix_list(tree).matrix
                treeCache(i).tree_id(treeCache(i).tree_cnt) = tree

                treeCache(i).tree_cnt += 1

                saved_model_loads += 1
                Return ' no need to do anything else.. this tree is copied now
            End If
        Next
        '--------------------------------------------------------------------
        '--------------------------------------------------------------------
        Dim ar = Trees.Tree_list(tree).Split("/")
        Dim treename = ar(ar.Length - 1)
        Dim ctree As String = Trees.Tree_list(tree).Replace("spt", "ctree")
        Dim treems As New MemoryStream
        Dim tree_entry As Ionic.Zip.ZipEntry = active_pkg(ctree)
        If tree_entry Is Nothing Then
            tree_entry = get_shared(ctree)
        End If
        tree_entry.Extract(treems)
        Dim br As New BinaryReader(treems)
        Dim enc As New System.Text.ASCIIEncoding
        'ReDim buff(6)

        'Dim matches(30) As Integer
        Dim bp As Integer = 0
        Dim s_string(6) As Byte
        Dim leafs_only As Boolean = False

        map_name = map_name.Replace(".pkg", "")
        treems.Position = 0
        Dim ind_cnt As Integer
        Dim diffuse_map, diffuse_normmap As String
        Dim version = br.ReadInt32  ' read version number
        Dim tree_data As New tree_
        Dim lod_ As Int32

        'clear display lists ID.. this trigers what to draw 
        Trees.flora(tree).branch_displayID = 0
        Trees.flora(tree).frond_displayID = 0
        Trees.flora(tree).leaf_displayID = 0
        Dim bb As BB_
        ReDim bb.BB(16)
        'get the bounding box corners
        bb.BB_Min.x = br.ReadSingle
        bb.BB_Min.y = br.ReadSingle
        bb.BB_Min.z = br.ReadSingle
        bb.BB_Max.x = br.ReadSingle
        bb.BB_Max.y = br.ReadSingle
        bb.BB_Max.z = br.ReadSingle
        'build and translate the bounding box.

        If ctree.Contains("Sedge_1") Then
            'Stop
        End If
        '------------------------
        ' 1. branch group
        '------------------------
        treems.Position = 36    ' vertex count for branchs/trunks
        Dim b_vert_cnt As Integer = br.ReadInt32
        If b_vert_cnt = 0 Then 'if 0, there is no branch vertices
            treems.Position = 52
            bp = treems.Position
            b_vert_cnt = br.ReadInt32
            If b_vert_cnt <> 0 Then
                treems.Position = bp
                GoTo fronds_2 'todo
            Else
                'next is texture deff block followed by leaf data
                treems.Position = 60    ' point to lenght of texture name
                GoTo reentry2 'this will read off the texture name.. we dont need it anyway
            End If
            'the data here will be frond or leaf vertices.
        Else
            ReDim tree_data.b_vert(b_vert_cnt * 13) ' make room for verts
            For i = 0 To (b_vert_cnt * 13) - 1
                tree_data.b_vert(i) = br.ReadSingle
            Next
            lod_ = br.ReadInt32 ' number of lod models?
            For k = 0 To lod_ - 1
                Dim ind_cnt_ = br.ReadInt32 ' get count
                If ind_cnt_ = 0 Then
                    Dim tr As Integer
                    Dim cnt = 0
                    While tr = 0
                        tr = br.ReadInt32
                        cnt += 1
                    End While
                    If cnt = 3 Then
                        treems.Position -= 4
                        GoTo fronds_2
                    End If
                    If cnt = 6 Then
                        treems.Position -= 4
                        ' next block is always a texture block right before the leaves?
                        GoTo reentry2
                    End If
                    'If lod_ = 2 Then
                    '    treems.Position -= 4
                    '    GoTo reentry2 ' strip text before reading leaf data
                    'End If
                    GoTo fronds
                End If
                If k = 0 Then 'only saving first lod for now
                    ReDim tree_data.b_indices(ind_cnt_) ' make room for them
                    For i = 0 To ind_cnt_ - 1
                        tree_data.b_indices(i) = br.ReadInt32
                    Next
                Else
                    For i = 0 To ind_cnt_ - 1
                        br.ReadInt32()  'just reading off remainding data
                    Next

                End If
            Next
        End If
        'should me pointing at a int32 which is the length of the next texture name string.
        diffuse_map = ""
        diffuse_normmap = ""
        Dim diff_len, norm_len As Int32
        diff_len = br.ReadInt32
        ReDim buff(diff_len)
        buff = br.ReadBytes(diff_len)
        diffuse_map = enc.GetString(buff)
        norm_len = br.ReadInt32
        ReDim buff(norm_len)
        buff = br.ReadBytes(norm_len)
        diffuse_normmap = enc.GetString(buff)
        ' there IS branch data so lets get the data needed
        If diffuse_map <> "" Then
            get_tree_branch_texture(diffuse_map, tree)
            build_branch_model(tree, tree_data)
        End If
        If diffuse_normmap <> "" Then
            get_tree_branch_normalmap(diffuse_normmap, tree)
        End If
        'todo... will need to make a gl texture for this (done) and the normal map if
        'we want to do bump mapping... Most likely, not.. it will slow this down.
        '------------------------------------
        'At this point, we are either pointing at"
        '1. the frond vertice cnt
        '2. the len of a texture string or
        '3. a zero value
        'if its zero it means there is no frond data or leaf data?
reentry:
        Dim tl As Integer = br.ReadInt32
        If tl = 0 Then
            tl = br.ReadInt32   ' guessing this tells how maany int32s to read to get to the
            ' the int32 of the texture length following it
            If tl = 0 Then
                'current location after read should be an int32 o the texture length following
                'There is a unused set of texture deffs starting at the next char
                diff_len = br.ReadInt32
                treems.Position += diff_len
                norm_len = br.ReadInt32
                treems.Position += norm_len
                leafs_only = True
                GoTo reentry ' there should be another unused texture deff block after this
                'with the same codes of either 2 or 3
                ' nothing but billboard after next texture names
            End If
            If tl = 2 Then
                treems.Position += 8    ' 2 integers
                'got to get billboard ' todo
            End If
            If tl = 3 Then
                treems.Position += 12 ' 3 integers
                'got to get billboard ' todo
            End If
            GoTo get_billboard_data
        End If
        'if tl is not zero, than we have a texture deff block
        'that needs to be read. May need to save this depending
        'on if its not the composite_diffuse.dds
        'need to check for texture deff block or fronds
fronds:
        If leafs_only Then
            treems.Position -= 4
            GoTo leafs
        End If
        Dim ps = treems.Position
        Dim s As Byte = br.ReadByte
        Dim p As Byte = br.ReadByte
        Dim e As Byte = br.ReadByte
        If s = 115 And p = 112 And e = 101 Then
            'yep.. its a texture deff block
            treems.Position = ps - 4 'restore pointer
            diff_len = br.ReadInt32
            treems.Position += diff_len
            norm_len = br.ReadInt32
            treems.Position += norm_len
            GoTo leafs
        Else
            treems.Position = ps - 4 'restore pointer so it points at cnt
        End If
fronds_2:
        '--------------
        '2 fronds
        '--------------
        'if sent here from there being no branch data, we should
        'be pointing at the vertice count
        vertex_count = br.ReadInt32
        ReDim tree_data.f_vert(vertex_count * 13)   '13 singles per line just like branch verts
        For i = 0 To (vertex_count * 13) - 1
            tree_data.f_vert(i) = br.ReadSingle 'get the verts
        Next
        Dim numstrips As Integer = br.ReadInt32
        ReDim tree_data.strip_inds(numstrips)
        For i = 0 To numstrips - 1  ' strips are not like LODs .. we need all of them?
            tree_data.strip_inds(i) = New f_indices
            ind_cnt = br.ReadInt32
            ReDim tree_data.strip_inds(i).indices(ind_cnt)
            For k = 0 To ind_cnt - 1
                tree_data.strip_inds(i).indices(k) = br.ReadInt32 ' get the indice
            Next
        Next
        tree_data.strip_count = numstrips
        build_frond_model(tree, tree_data)
        'This is the same as before.. 
        'some trees have branch data and no fronds.. some have fronds and no branchs
        'and still some heave no leafs or branchs.. some have fronds and leafs.
        'depending on whats after each vertex chunk determins this.
reentry2:
        Dim frond_entry As Integer = treems.Position
        tl = br.ReadInt32
        If tl = 0 Then
            tl = br.ReadInt32   ' guessing this tells how maany int32s to read to get to the
            ' the int32 of the texture length following it
            If tl = 0 Then
                'current location after read should be an int32 o the texture length following
                'There is a unused set of texture deffs starting at the next char
                diff_len = br.ReadInt32
                treems.Position += diff_len
                norm_len = br.ReadInt32
                treems.Position += norm_len
                GoTo reentry2 ' there should be another unused texture deff block after this
                'with the same codes of either 2 or 3
                ' nothing ut billboard after next texture names
            End If
            If tl = 2 Then
                treems.Position += 8    ' 2 integers
                'got to get billboard ' todo
            End If
            If tl = 3 Then
                treems.Position += 12 ' 3 integers
                'got to get billboard ' todo
            End If
            GoTo get_billboard_data
        End If
        'if tl is not zero, than we have a texture deff block
        'that needs to be read. May need to save this depeding
        'on if its not the composite_diffuse.dds
        treems.Position += tl
        norm_len = br.ReadInt32
        treems.Position += norm_len
        '-----------
        '3 leafs
        '-----------
leafs:
        tl = br.ReadInt32
        If tl = 0 Then
            tl = br.ReadInt32
            If tl = 3 Then
                treems.Position += 12 ' 2 integers
                'got to get billboard ' todo
            End If
            If tl = 2 Then
                treems.Position += 8    ' 2 integers
                'got to get billboard ' todo
            End If
            If tl = 1 Then
                treems.Position += 4    ' 3 integers
                'got to get billboard ' todo
            End If
            GoTo get_billboard_data
        Else
            treems.Position -= 4
        End If
        'At this point, we should be pointing at the vert count for the leafs.
        'leafs have 22 singles per vertex or 88 bytes.. Not sure what it all means yet.
        ' What I know
        '0-2 vect3 vert.. they are used 2 ways.. more later
        '3-5 vect3 normal
        '6-7 vect2 UV coords
        '12-13 single width and heigth of poly
        '15 <--- this is either 4 or 0 to 3 in each vertex data block...
        ' if its 4, these are directly used vertices and nothing
        ' else has to be done. However, it the are 0 - 3 than
        ' each poly has to be rotated and sized to create what
        ' are called "leafcards". The Internet is your friend :)
        '
        ' Branch and Frond verts are 13 singles long. (52 bytes)
        ' the first 7 are all that I'm using as I dont know what
        ' the rest of the singles are.

        vertex_count = br.ReadInt32
        ReDim tree_data.l_vert(vertex_count * 22)
        For i = 0 To (vertex_count * 22) - 1
            tree_data.l_vert(i) = br.ReadSingle 'get the data
        Next
        'lod? Yes... we are only needing LOD 0
        lod_ = br.ReadInt32 'Get number of levels.
        Dim cnt2 = 0 ' running pointer in to array
        ReDim tree_data.l_indices(1)
        For k = 0 To lod_ - 1
            Dim ind_cnt_ = br.ReadInt32 ' get count
            If k = 0 Then

                ReDim Preserve tree_data.l_indices(tree_data.l_indices.Length + ind_cnt_ - 1)   ' make room for them
                For i = 0 To ind_cnt_ - 1
                    tree_data.l_indices(cnt2) = br.ReadInt32
                    cnt2 += 1
                Next
            Else
                For i = 0 To ind_cnt_ - 1
                    'just read off the other LODs.. we dont need them.
                    br.ReadInt32()
                Next

            End If

        Next
lod_skip:
        build_leaf_model(tree, tree_data)
        '-------------
        '4 billboards
        '-------------
get_billboard_data:
        'dont need to load the textures.. 
        'All billboards use the Composite_Diffuse.dds
        diff_len = br.ReadInt32
        treems.Position += diff_len

        norm_len = br.ReadInt32
        treems.Position += norm_len
        '--------------------------------
        Dim vert_pnt = treems.Position
        Dim vert_cnt = br.ReadInt32
        Dim ind_pnt = treems.Position + (vert_cnt * 68)
        treems.Position = ind_pnt
        br.ReadInt32()  ' dont need this.. its always one
        ind_cnt = br.ReadInt32
        'treems.Position = psn
        '------------ end of test crap
        Dim v1 As vect3
        Dim vn As vect3
        Dim vt As vect2
        Dim t_off As Integer = 36
        vert_pnt += 4
        ind_pnt += 8
        Dim pnt As Integer
        Trees.flora(tree).billboard_displayID = -1
        Trees.flora(tree).billboard_displayID = Gl.glGenLists(1)
        Gl.glNewList(Trees.flora(tree).billboard_displayID, Gl.GL_COMPILE)

        'Grrrrrr
        'ind_cnt changes... some trees have junk on the end Im not sure about.

        Gl.glBegin(Gl.GL_TRIANGLES)
        For i = 0 To (ind_cnt) - 7
            '   For k = 0 To 2
            treems.Position = ind_pnt ' set pos to current indice
            pnt = br.ReadInt32 ' get vert number
            treems.Position = vert_pnt + (pnt * 68) ' set pnt to point at vert

            v1.x = br.ReadSingle
            v1.y = br.ReadSingle
            v1.z = br.ReadSingle
            vn.x = br.ReadSingle
            vn.y = br.ReadSingle
            vn.z = br.ReadSingle
            treems.Position = vert_pnt + (pnt * 68) + 36    ' set pnt to point at vert
            vt.x = br.ReadSingle
            vt.y = br.ReadSingle
            ' rotate them to make them blend better
            'Dim l = Sqrt((v1.x ^ 2) + (v1.z ^ 2))
            'Dim theta = Atan2(v1.z, v1.x)
            'If theta < 0.0 Then theta += (PI * 2)
            'Dim beta = theta + (-PI * 0.3)
            'Dim nx, nz As Single
            'nx = (l * Cos(beta)) - (l * Sin(beta))
            'nz = (l * Sin(beta)) + (l * Cos(beta))
            'v1.x = nx * 0.6
            'v1.z = nz * 0.6
            'l = Sqrt((vn.x ^ 2) + (vn.z ^ 2))
            'nx = (l * Cos(beta)) - (l * Sin(beta))
            'nz = (l * Sin(beta)) + (l * Cos(beta))
            'vn.x = nx
            'vn.z = nz
            Gl.glTexCoord2f(vt.x, vt.y)
            Gl.glNormal3f(vn.x, vn.y, vn.z)
            Gl.glVertex3f(v1.x, v1.y, v1.z)
            ind_pnt += 4
            'Next k
        Next i
        Gl.glEnd()
        Gl.glEndList()
        Gl.glFinish()
        If ctree.Contains("Linden") Then
            'Stop
        End If
        Try

            'tried all kinds a shit to find where the texture info starts.. Its wildly different between the different trees
            'So.. Im' going to search for it.
            Dim cp = treems.Position
            While 1
                cp = treems.Position
                s = br.ReadByte
                p = br.ReadByte
                e = br.ReadByte
                If s = 115 And p = 112 And e = 101 Then
                    treems.Position = cp - 4 'point at lengh info

                    Exit While
                End If
                treems.Position = cp + 1
            End While


            diff_len = br.ReadInt32
            ReDim buff(diff_len)
            buff = br.ReadBytes(diff_len)
            diffuse_map = enc.GetString(buff)
            norm_len = br.ReadInt32
            ReDim buff(norm_len)
            buff = br.ReadBytes(norm_len)
            diffuse_normmap = enc.GetString(buff)
            leafs_only = True
            If diffuse_map <> "" Then
                get_tree_billboard_texture(diffuse_map, tree)
            End If
            If diffuse_normmap <> "" Then
                get_tree_billboard_normalmap(diffuse_normmap, tree)
            End If
        Catch ex As Exception
            frmMapInfo.I__General_Info_tb.Text += "Data After BillBoard + : " + ctree + vbCrLf 'for debug

        End Try
        br.Close()
        treems.Close()
        treems.Dispose()
        '-------------------------------------------------
        'We just created a new tree so..
        'we need to add this tree to the treecache
        'so that if its used again, we already have the dislaylist for it.
        '-------------------------------------------------
        cnt2 = treeCache.Length
        If cnt2 = 0 Then
            ReDim Preserve treeCache(1)
        Else
            ReDim Preserve treeCache(cnt2)
        End If
        cnt2 -= 1
        treeCache(cnt2) = New flora_
        treeCache(cnt2).name = Trees.Tree_list(tree)
        treeCache(cnt2).branch_displayID = Trees.flora(tree).branch_displayID
        treeCache(cnt2).branch_textureID = Trees.flora(tree).branch_textureID
        treeCache(cnt2).branch_normalID = Trees.flora(tree).branch_normalID
        treeCache(cnt2).frond_displayID = Trees.flora(tree).frond_displayID
        treeCache(cnt2).leaf_displayID = Trees.flora(tree).leaf_displayID
        treeCache(cnt2).billboard_displayID = Trees.flora(tree).billboard_displayID
        treeCache(cnt2).billboard_textureID = Trees.flora(tree).billboard_textureID
        treeCache(cnt2).billboard_normalID = Trees.flora(tree).billboard_normalID
        treeCache(cnt2).billboard_normalID = Trees.flora(tree).billboard_normalID

        treeCache(cnt2).D_BB = New BB_
        ReDim treeCache(cnt2).D_BB.BB(16)

        treeCache(cnt2).D_BB.BB = bb.BB
        treeCache(cnt2).D_BB.BB_Max = bb.BB_Max
        treeCache(cnt2).D_BB.BB_Min = bb.BB_Min

        ReDim Preserve treeCache(cnt2).matrix_list(treeCache(cnt2).tree_cnt + 1)
        ReDim Preserve treeCache(cnt2).tree_id(treeCache(cnt2).tree_cnt + 1)
        ReDim Preserve treeCache(cnt2).BB(treeCache(cnt2).tree_cnt + 1)
        get_translated_bb_tree(bb, tree)
        treeCache(cnt2).BB(treeCache(cnt2).tree_cnt + 1).BB = bb.BB

        treeCache(cnt2).matrix_list(treeCache(cnt2).tree_cnt) = New matrix_
        treeCache(cnt2).matrix_list(treeCache(cnt2).tree_cnt).matrix = speedtree_matrix_list(tree).matrix
        treeCache(cnt2).tree_id(treeCache(cnt2).tree_cnt) = tree
        treeCache(cnt2).tree_cnt += 1
        Return
        '----------------------------------------

    End Sub
    Private Sub get_translated_bb_tree(ByRef mm As BB_, ByVal tree As Integer)
        Dim v1, v2, v3, v4, v5, v6, v7, v8 As vect3
        v1.z = mm.BB_Max.z : v2.z = mm.BB_Max.z : v3.z = mm.BB_Max.z : v4.z = mm.BB_Max.z
        v5.z = mm.BB_Min.z : v6.z = mm.BB_Min.z : v7.z = mm.BB_Min.z : v8.z = mm.BB_Min.z

        v1.x = mm.BB_Min.x : v6.x = mm.BB_Min.x : v7.x = mm.BB_Min.x : v4.x = mm.BB_Min.x
        v5.x = mm.BB_Max.x : v8.x = mm.BB_Max.x : v3.x = mm.BB_Max.x : v2.x = mm.BB_Max.x

        v4.y = mm.BB_Max.y : v7.y = mm.BB_Max.y : v8.y = mm.BB_Max.y : v3.y = mm.BB_Max.y
        v6.y = mm.BB_Min.y : v5.y = mm.BB_Min.y : v1.y = mm.BB_Min.y : v2.y = mm.BB_Min.y

        mm.BB(0) = v1
        mm.BB(1) = v2
        mm.BB(2) = v3
        mm.BB(3) = v4
        mm.BB(4) = v5
        mm.BB(5) = v6
        mm.BB(6) = v7
        mm.BB(7) = v8


        For i = 0 To 7
            mm.BB(i) = translate_to(mm.BB(i), speedtree_matrix_list(tree).matrix)
        Next

    End Sub


    Public Sub draw_ring(ByVal x As Single, ByVal z As Single)
        Dim section = (PI * 2) / 200
        Dim r1 As Single = 49.0 : Dim r2 As Single = 50.0
        Dim yup As Double = 0.15
        Dim x1, x2, x3, x4, y1, y2, y3, y4, z1, z2, z3, z4 As Single
        Gl.glBegin(Gl.GL_QUADS)
        For i = 0 To PI * 2 Step section

            x1 = Cos(i) * r1 + x : z1 = Sin(i) * r1 + z : y1 = get_Z_at_XY(x1, z1) + yup
            x2 = Cos(i) * r2 + x : z2 = Sin(i) * r2 + z : y2 = get_Z_at_XY(x2, z2) + yup
            x3 = Cos(i + section) * r1 + x : z3 = Sin(i + section) * r1 + z : y3 = get_Z_at_XY(x3, z3) + yup
            x4 = Cos(i + section) * r2 + x : z4 = Sin(i + section) * r2 + z : y4 = get_Z_at_XY(x4, z4) + yup
            Gl.glNormal3f(0.0, -1.0, 0.0)
            'top
            Gl.glVertex3f(x1, y1, z1)
            Gl.glVertex3f(x2, y2, z2)
            Gl.glVertex3f(x4, y4, z4)
            Gl.glVertex3f(x3, y3, z3)
            'outside face
            Gl.glVertex3f(x4, y4, z4)
            Gl.glVertex3f(x2, y2, z2)
            Gl.glVertex3f(x2, y2 - 1, z2)
            Gl.glVertex3f(x4, y4 - 1, z4)
            'inside face
            Gl.glVertex3f(x1, y1, z1)
            Gl.glVertex3f(x3, y3, z3)
            Gl.glVertex3f(x3, y3 - 1, z3)
            Gl.glVertex3f(x1, y1 - 1, z1)
        Next
        Gl.glEnd()

    End Sub
    Public Sub draw_base_ring(ByVal x As Single, ByVal z As Single, ByVal base As Integer)

        Dim radius, thickness As Single
        radius = 50.0
        thickness = 1.0
        Gl.glUseProgram(shader_list.ring_Shader)
        Gl.glUniform3f(ring_location, x, 0.0, z)
        Gl.glUniform1f(ring_radius, radius)
        Gl.glUniform1f(ring_thickness, thickness)
        Gl.glUniform1i(ring_depthmap, 0)
        If base = 1 Then
            Gl.glColor4f(0.7, 0.0, 0.0, 0.7)
        Else
            Gl.glColor4f(0.0, 0.7, 0.0, 0.8)
        End If

        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gDepthTexture)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        'draw_quad(radius + thickness)
        Gl.glPushMatrix()
        Gl.glTranslatef(0.0, get_Z_at_XY(x, z), 0.0)
        Gl.glScalef(1.0, 0.05, 1.0)
        glutSolidCube((radius + thickness) * 2.0)
        Gl.glPopMatrix()
        Gl.glUseProgram(0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glEnable(Gl.GL_DEPTH_TEST)

        Return



    End Sub
    Public Sub draw_cursor(ByVal x As Single, ByVal z As Single)


        Dim radius, thickness As Single
        radius = 6.0
        thickness = 1.0
        Gl.glUseProgram(shader_list.ring_Shader)
        Gl.glUniform3f(ring_location, x, 0.0, z)
        Gl.glUniform1f(ring_radius, radius)
        Gl.glUniform1f(ring_thickness, thickness)
        Gl.glUniform1i(ring_depthmap, 0)
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glColor4f(1.0, 1.0, 1.0, 0.6)

        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gDepthTexture)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        'draw_quad(radius + thickness)
        Gl.glPushMatrix()
        Gl.glTranslatef(0.0, get_Z_at_XY(x, z), 0.0)
        glutSolidCube((radius + thickness) * 2.0)
        Gl.glPopMatrix()
        Gl.glUseProgram(0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDisable(Gl.GL_BLEND)
        Return

    End Sub
    Public Sub draw_quad(ByVal size As Single)
        Gl.glBegin(Gl.GL_QUADS)
        Gl.glVertex3f(-size, 0.0, size)
        Gl.glVertex3f(size, 0.0, size)
        Gl.glVertex3f(size, 0.0, -size)
        Gl.glVertex3f(-size, 0.0, -size)
        Gl.glEnd()
    End Sub
    Public Sub create_grid_marks()
        'Return
        Dim w As Double = MAP_BB_UR.x - MAP_BB_BL.x
        Dim v As Double = MAP_BB_UR.y - MAP_BB_BL.y
        Dim gx As Double = CInt(w / 10.0)
        Dim gxstep As Double = gx / 100.0
        Dim gy As Double = CInt(v / 10.0)
        Dim gystep As Double = gy / 100.0
        'save the size of each play area cell
        frmMapInfo.I__General_Info_tb.Text += "Player Area Grid Size: " + gx.ToString + " x " + gy.ToString + vbCrLf + vbCrLf
        Dim yup As Double = 0.5
        Gl.glBegin(Gl.GL_QUADS)
        Dim ur As Double = -0.5
        For y As Double = MAP_BB_BL.y + gy To MAP_BB_UR.y - gy Step gy
            For x As Double = MAP_BB_BL.x To MAP_BB_UR.x - gx + 0.0001 Step gx
                For xs As Double = x To x + gx - gxstep + 0.0001 Step gxstep
                    Gl.glVertex3f(xs, get_Z_at_XY(xs, y - ur) + yup, y - ur)
                    Gl.glVertex3f(xs + gxstep, get_Z_at_XY(xs + gxstep, y - ur) + yup, y - ur)
                    Gl.glVertex3f(xs + gxstep, get_Z_at_XY(xs + gxstep, y + ur) + yup, y + ur)
                    Gl.glVertex3f(xs, get_Z_at_XY(xs, y + ur) + yup, y + ur)
                Next xs
            Next
        Next
        For x As Double = MAP_BB_BL.x + gx To MAP_BB_UR.x - gx Step gx
            For y As Double = MAP_BB_BL.y To MAP_BB_UR.y - gy + 0.0001 Step gy
                For ys As Double = y To y + gy - gystep + 0.0001 Step gystep
                    Gl.glVertex3f(x - ur, get_Z_at_XY(x - ur, ys) + yup, ys)
                    Gl.glVertex3f(x + ur, get_Z_at_XY(x + ur, ys) + yup, ys)
                    Gl.glVertex3f(x + ur, get_Z_at_XY(x + ur, ys + gystep) + yup, ys + gystep)
                    Gl.glVertex3f(x - ur, get_Z_at_XY(x - ur, ys + gystep) + yup, ys + gystep)
                Next ys
            Next
        Next
        Gl.glEnd()
    End Sub
    Public Sub make_map_boarder()
        'Return
        Dim w As Double = MAP_BB_UR.x - MAP_BB_BL.x
        Dim v As Double = MAP_BB_UR.y - MAP_BB_BL.y
        Dim gx As Double = w / 10.0
        Dim gxstep As Double = gx / 100.0
        Dim gy As Double = v / 10.0
        Dim gystep As Double = gy / 100.0
        Dim yup As Double = 0.6
        Gl.glBegin(Gl.GL_QUADS)
        Dim ur As Double = -0.5
        Dim y As Double = MAP_BB_BL.y
        Dim x As Double = MAP_BB_BL.x
        For xs As Double = MAP_BB_BL.x + ur To MAP_BB_UR.x Step gxstep
            Gl.glVertex3f(xs, get_Z_at_XY(xs, y - ur) + yup, y - ur)
            Gl.glVertex3f(xs + gxstep, get_Z_at_XY(xs + gxstep, y - ur) + yup, y - ur)
            Gl.glVertex3f(xs + gxstep, get_Z_at_XY(xs + gxstep, y + ur) + yup, y + ur)
            Gl.glVertex3f(xs, get_Z_at_XY(xs, y + ur) + yup, y + ur)
        Next xs
        y = MAP_BB_UR.y
        For xs As Double = MAP_BB_BL.x + ur To MAP_BB_UR.x Step gxstep
            Gl.glVertex3f(xs, get_Z_at_XY(xs, y - ur) + yup, y - ur)
            Gl.glVertex3f(xs + gxstep, get_Z_at_XY(xs + gxstep, y - ur) + yup, y - ur)
            Gl.glVertex3f(xs + gxstep, get_Z_at_XY(xs + gxstep, y + ur) + yup, y + ur)
            Gl.glVertex3f(xs, get_Z_at_XY(xs, y + ur) + yup, y + ur)
        Next xs

        x = MAP_BB_BL.x
        For ys As Double = MAP_BB_BL.y + ur To MAP_BB_UR.y Step gystep
            Gl.glVertex3f(x - ur, get_Z_at_XY(x - ur, ys) + yup, ys)
            Gl.glVertex3f(x + ur, get_Z_at_XY(x + ur, ys) + yup, ys)
            Gl.glVertex3f(x + ur, get_Z_at_XY(x + ur, ys + gystep) + yup, ys + gystep)
            Gl.glVertex3f(x - ur, get_Z_at_XY(x - ur, ys + gystep) + yup, ys + gystep)
        Next ys
        x = MAP_BB_UR.x
        For ys As Double = MAP_BB_BL.y + ur To MAP_BB_UR.y Step gystep
            Gl.glVertex3f(x - ur, get_Z_at_XY(x - ur, ys) + yup, ys)
            Gl.glVertex3f(x + ur, get_Z_at_XY(x + ur, ys) + yup, ys)
            Gl.glVertex3f(x + ur, get_Z_at_XY(x + ur, ys + gystep) + yup, ys + gystep)
            Gl.glVertex3f(x - ur, get_Z_at_XY(x - ur, ys + gystep) + yup, ys + gystep)
        Next ys
        Gl.glEnd()

    End Sub
End Module
