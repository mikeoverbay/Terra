
Imports System.IO
Imports System.Windows.Forms
Imports System.Math
Imports System.String
Imports System.Text


Module Mod_Space_Bin_Functions
#Region "Structures"

    Public Model_Matrix_list() As model_matrix_list_
    Public Structure model_matrix_list_
        Public primitive_name As String
        Public matrix() As Single
        Public mask As Boolean
    End Structure

    Public speedtree_matrix_list() As speedtree_matrix_list_
    Public Structure speedtree_matrix_list_
        Public tree_name As String
        Public matrix() As Single
    End Structure
    Public decal_matrix_list() As decal_matrix_list_
    Public Structure decal_matrix_list_
        Public u_wrap As Single
        Public v_wrap As Single
        Public decal_data() As vertex_data
        Public decal_count As Integer
        Public texture_id As Integer
        Public normal_id As Integer
        Public depth_map_id As Integer
        Public display_id As Integer
        Public decal_texture As String
        Public decal_normal As String
        Public matrix() As Single
        Public good As Boolean
        Public offset As vect4
        Public priority As Integer
        Public influence As Single
        Public cam_pos As vect3
        Public look_at As vect3
        Public far_clip As vect3
        Public near_clip As vect3
        Public texture_matrix() As Single
        Public top_left As vect3
        Public top_right As vect3
        Public bot_left As vect3
        Public bot_right As vect3
        Public cam_rotation As vect3
        Public cam_location As vect3
        Public flags As UInteger
        Public t_bias As Single
        Public d_bias As Single
        Public exclude As Boolean
        Public old_bias As Single
    End Structure
#End Region


    Public Sub get_BSMA_data(ByVal t_cnt As Integer)
        'bigworld Static Model Array
        'more model data.
        'ms is pointing at the BSMA secion after this next line
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0
        BSMA.t1_start = br.BaseStream.Position
        BSMA.t2_dl = br.ReadUInt32
        BSMA.t1_dc = br.ReadUInt32
        ReDim BSMA.bsma_t1(BSMA.t1_dc)
        For k = 0 To BSMA.t1_dc - 1
            BSMA.bsma_t1(k).fx_index = br.ReadUInt32
            BSMA.bsma_t1(k).index_start = br.ReadUInt32 ' t3 ref
            BSMA.bsma_t1(k).index_end = br.ReadUInt32 ' t3 ref
        Next
        BSMA.t1_start = br.BaseStream.Position
        BSMA.t2_dl = br.ReadUInt32
        BSMA.t2_dc = br.ReadUInt32
        ReDim BSMA.bsma_t2(BSMA.t2_dc)
        For k = 0 To BSMA.t2_dc - 1
            BSMA.bsma_t2(k).bwst_key = br.ReadUInt32
            BSMA.bsma_t2(k).shader_string = find_str_BWST(BSMA.bsma_t2(k).bwst_key)
        Next

        BSMA.t3_start = br.BaseStream.Position
        BSMA.t3_dl = br.ReadUInt32
        BSMA.t3_dc = br.ReadUInt32
        Dim pos = BSMA.t3_start + 8 + (BSMA.t3_dl * BSMA.t3_dc)
        br.BaseStream.Position = pos
        BSMA.t4_start = br.BaseStream.Position
        BSMA.t4_dl = br.ReadUInt32
        BSMA.t4_dc = br.ReadUInt32
        ReDim BSMA.bsma_t4(BSMA.t4_dc)
        For k = 0 To BSMA.t4_dc - 1
            ReDim BSMA.bsma_t4(k).matrix(16)
            For i = 0 To 15
                BSMA.bsma_t4(k).matrix(i) = br.ReadSingle
            Next
        Next
        BSMA.t5_start = br.BaseStream.Position
        BSMA.t5_dl = br.ReadUInt32
        BSMA.t5_dc = br.ReadUInt32
        ReDim BSMA.bsma_t5(BSMA.t5_dc)
        For k = 0 To BSMA.t5_dc - 1
            BSMA.bsma_t5(k).vector4.x = br.ReadSingle
            BSMA.bsma_t5(k).vector4.y = br.ReadSingle
            BSMA.bsma_t5(k).vector4.z = br.ReadSingle
            BSMA.bsma_t5(k).vector4.w = br.ReadSingle
        Next

        br.BaseStream.Position = BSMA.t3_start + 8
        ReDim BSMA.bsma_t3(BSMA.t3_dc)
        For k = 0 To BSMA.t3_dc - 1
            BSMA.bsma_t3(k).Property_key = br.ReadUInt32
            BSMA.bsma_t3(k).value_type = br.ReadUInt32
            BSMA.bsma_t3(k).value_type_string = BSMA_PropertyType(BSMA.bsma_t3(k).value_type)
            BSMA.bsma_t3(k).value = br.ReadUInt32
            BSMA.bsma_t3(k).value_string = BSMA.bsma_t3(k).value.ToString
            BSMA.bsma_t3(k).Property_string = find_str_BWST(BSMA.bsma_t3(k).Property_key)

            Select Case BSMA.bsma_t3(k).value_type
                Case Is = 0
                Case Is = 1
                    If BSMA.bsma_t3(k).value = 1 Then
                        BSMA.bsma_t3(k).value_string = "True"
                    Else
                        BSMA.bsma_t3(k).value_string = "False"
                    End If
                Case Is = 2
                    BSMA.bsma_t3(k).value_string = CSng(BSMA.bsma_t3(k).value).ToString("0.000000")
                Case Is = 3
                    BSMA.bsma_t3(k).value_string = BSMA.bsma_t3(k).value
                Case Is = 4
                    'this never gets hit on with ENSK map.
                    Console.WriteLine(BSMA.bsma_t3(k).Property_key.ToString("x"))
                Case Is = 5
                    Dim v As vect4 = BSMA.bsma_t5(BSMA.bsma_t3(k).value).vector4
                    BSMA.bsma_t3(k).value_string = _
                        v.x.ToString("00.000000") + " " + _
                        v.y.ToString("00.000000") + " " + _
                        v.z.ToString("00.000000") + " " + _
                        v.w.ToString("00.000000")
                Case Is = 6
                    BSMA.bsma_t3(k).value_string = find_str_BWST(BSMA.bsma_t3(k).value)
                Case Is = 7
                    ' this only gets hit when BSMA.bsma_t3(k).Property_string  = "g_useNormalPackDXT1_safe"
                    BSMA.bsma_t3(k).value_string = "True"
            End Select


            'If BSMA.bsma_t3(k).value_type_string = "?" Then
            '    Console.WriteLine(BSMA.bsma_t3(k).Property_string)
            'End If
        Next

        ReDim visual_sections(BSMA.t1_dc)
        Dim index As Integer = 0
        For n = 0 To BSMA.t1_dc - 1
            If BSMA.bsma_t1(n).index_end <= BSMA.t3_dc Then 'ignore &hFFFFFFFF entries
                visual_sections(index) = New visual_sections_
                ReDim visual_sections(index).entries((BSMA.bsma_t1(n).index_end - BSMA.bsma_t1(n).index_start))
                If BSMA.bsma_t1(n).fx_index < 100 Then
                    visual_sections(index).shader = BSMA.bsma_t2(BSMA.bsma_t1(n).fx_index).shader_string
                Else
                    visual_sections(index).shader = "none"
                End If
                For k = BSMA.bsma_t1(n).index_start To BSMA.bsma_t1(n).index_end
                    Dim ind = k - BSMA.bsma_t1(n).index_start
                    visual_sections(index).entries(ind) = New visual_entries_
                    visual_sections(index).entries(ind).Property_name = BSMA.bsma_t3(k).Property_string
                    visual_sections(index).entries(ind).Property_value = BSMA.bsma_t3(k).value_string
                Next
            End If
            index += 1
        Next
        ReDim Preserve visual_sections(index)
        Dim sb As New StringBuilder
        'sb.Length = 0

        'For k = 0 To visual_sections.Length - 2
        '    sb.AppendLine("shader: " + visual_sections(k).shader)
        '    For i = 0 To visual_sections(k).entries.Length - 1
        '        sb.AppendLine(visual_sections(k).entries(i).Property_name + " : " + visual_sections(k).entries(i).Proerty_value)

        '    Next
        '    sb.AppendLine()
        'Next
        ''File.WriteAllText("C:\!!_ENSK\Visuals.txt", sb.ToString)

    End Sub

    Public Sub get_BSMO_data(ByVal t_cnt As Integer)
        'more model data
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0

        BSMO.t1_start = br.BaseStream.Position
        BSMO.t1_dl = br.ReadUInt32 ' data_length in bytes
        BSMO.t1_dc = br.ReadUInt32 ' entry count
        ReDim BSMO.bsmo_t1(BSMO.t1_dc)
        'get node start-end table
        For k = 0 To BSMO.t1_dc - 1
            BSMO.bsmo_t1(k).node_start = br.ReadUInt32
            BSMO.bsmo_t1(k).node_end = br.ReadUInt32
        Next
        '
        BSMO.t2_start = br.BaseStream.Position
        BSMO.t2_dl = br.ReadUInt32
        BSMO.t2_dc = br.ReadUInt32
        ReDim BSMO.bsmo_t2(BSMO.t2_dc)
        'get node entries
        For k = 0 To BSMO.t2_dc - 1
            BSMO.bsmo_t2(k).min_BB.x = br.ReadSingle
            BSMO.bsmo_t2(k).min_BB.y = br.ReadSingle
            BSMO.bsmo_t2(k).min_BB.z = br.ReadSingle

            BSMO.bsmo_t2(k).max_BB.x = br.ReadSingle
            BSMO.bsmo_t2(k).max_BB.y = br.ReadSingle
            BSMO.bsmo_t2(k).max_BB.z = br.ReadSingle

            BSMO.bsmo_t2(k).BWST_String_key = br.ReadUInt32

            BSMO.bsmo_t2(k).index_from = br.ReadUInt32
            BSMO.bsmo_t2(k).index_to = br.ReadUInt32
            If BSMO.bsmo_t2(k).BWST_String_key > 0 Then
                BSMO.bsmo_t2(k).model_str = find_str_BWST(BSMO.bsmo_t2(k).BWST_String_key)
            Else
                BSMO.bsmo_t2(k).model_str = "Nothing"
            End If

        Next

        BSMO.t3_start = br.BaseStream.Position
        BSMO.t3_dl = br.ReadUInt32
        BSMO.t3_dc = br.ReadUInt32
        ReDim BSMO.bsmo_t3(BSMO.t3_dc)
        'get table data
        For k = 0 To BSMO.t3_dc - 1
            BSMO.bsmo_t3(k).index = br.ReadUInt32
            BSMO.bsmo_t3(k).offset = br.ReadUInt32
        Next

        BSMO.t4_start = br.BaseStream.Position
        BSMO.t4_dl = br.ReadUInt32
        BSMO.t4_dc = br.ReadUInt32
        ReDim BSMO.bsmo_t4(BSMO.t4_dc)
        'get table data
        For k = 0 To BSMO.t4_dc - 1
            BSMO.bsmo_t4(k).min_BB.x = br.ReadSingle
            BSMO.bsmo_t4(k).min_BB.y = br.ReadSingle
            BSMO.bsmo_t4(k).min_BB.z = br.ReadSingle

            BSMO.bsmo_t4(k).max_BB.x = br.ReadSingle
            BSMO.bsmo_t4(k).max_BB.y = br.ReadSingle
            BSMO.bsmo_t4(k).max_BB.z = br.ReadSingle
        Next

        Dim dmy_dl = br.ReadUInt32
        Dim dmy_cnt = br.ReadUInt32
        For i = 0 To dmy_cnt - 1
            br.ReadUInt64()
        Next
        dmy_dl = br.ReadUInt32
        dmy_cnt = br.ReadUInt32
        For i = 0 To dmy_cnt - 1
            br.ReadUInt32()
        Next

        BSMO.t5_start = br.BaseStream.Position
        BSMO.t5_dl = br.ReadUInt32
        BSMO.t5_dc = br.ReadUInt32
        ReDim BSMO.bsmo_t5(BSMO.t5_dc)
        'get table data
        For k = 0 To BSMO.t5_dc - 1
            BSMO.bsmo_t5(k).Mask = br.ReadUInt32
        Next

        BSMO.t6_start = br.BaseStream.Position
        BSMO.t6_dl = br.ReadUInt32
        BSMO.t6_dc = br.ReadUInt32
        ReDim BSMO.bsmo_t6(BSMO.t6_dc)
        For k = 0 To BSMO.t6_dc - 1
            BSMO.bsmo_t6(k).start_index = br.ReadUInt32
            BSMO.bsmo_t6(k).end_index = br.ReadUInt32
        Next

        BSMO.t7_start = br.BaseStream.Position
        BSMO.t7_dl = br.ReadUInt32
        BSMO.t7_dc = br.ReadUInt32
        ReDim BSMO.bsmo_t7(BSMO.t7_dc)
        For k = 0 To BSMO.t7_dc - 1
            BSMO.bsmo_t7(k).u1_uint32 = br.ReadUInt32
            BSMO.bsmo_t7(k).u2_uint32 = br.ReadUInt32
            BSMO.bsmo_t7(k).material_index = br.ReadUInt32
            BSMO.bsmo_t7(k).group_index = br.ReadUInt32
            BSMO.bsmo_t7(k).vert_key = br.ReadUInt32
            BSMO.bsmo_t7(k).indi_key = br.ReadUInt32
            BSMO.bsmo_t7(k).u3_int32 = br.ReadUInt32
            BSMO.bsmo_t7(k).vert_string = find_str_BWST(BSMO.bsmo_t7(k).vert_key)
            BSMO.bsmo_t7(k).indi_string = find_str_BWST(BSMO.bsmo_t7(k).indi_key)
        Next

        BSMO.t8_start = br.BaseStream.Position
        BSMO.t8_dl = br.ReadUInt32
        BSMO.t8_dc = br.ReadUInt32
        ReDim BSMO.bsmo_t8(BSMO.t8_dc)
        For k = 0 To BSMO.t8_dc - 1
            BSMO.bsmo_t8(k).v = br.ReadUInt32
        Next

        BSMO.t9_start = br.BaseStream.Position
        BSMO.t9_dl = br.ReadUInt32
        BSMO.t9_dc = br.ReadUInt32
        ReDim BSMO.bsmo_t9(BSMO.t9_dc)
        For k = 0 To BSMO.t9_dc - 1
            BSMO.bsmo_t9(k).v = br.ReadUInt32

        Next

        'BSMO.t10_start = br.BaseStream.Position
        'BSMO.t10_dl = br.ReadUInt32
        'BSMO.t10_dc = br.ReadUInt32
        'ReDim BSMO.bsmo_t10(BSMO.t10_dc)
        'For k = 0 To BSMO.t10_dc - 1
        '    BSMO.bsmo_t10(k).v = br.ReadUInt32

        'Next

        'BSMO.t11_start = br.BaseStream.Position
        'BSMO.t11_dl = br.ReadUInt32
        'BSMO.t11_dc = br.ReadUInt32
        'ReDim BSMO.bsmo_t11(BSMO.t11_dc)
        'For k = 0 To BSMO.t11_dc - 1
        '    BSMO.bsmo_t11(k).index = br.ReadUInt32
        '    ReDim BSMO.bsmo_t11(k).matrix(16)
        '    For i = 0 To 15
        '        BSMO.bsmo_t11(k).matrix(i) = br.ReadSingle
        '    Next
        'Next

    End Sub

    Public Sub get_WSMI_data(ByVal t_cnt As Integer)

        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0

        WSMI.t1_start = br.BaseStream.Position
        WSMI.t1_dl = br.ReadUInt32
        WSMI.t1_dc = br.ReadUInt32

        ReDim WSMI.wsmi_t1(WSMI.t1_dc)
        For k = 0 To WSMI.t1_dc - 1
            WSMI.wsmi_t1(k).flag1 = br.ReadUInt32
            WSMI.wsmi_t1(k).flag2 = br.ReadUInt32
            WSMI.wsmi_t1(k).flag3 = br.ReadUInt32
        Next

        WSMI.t2_start = br.BaseStream.Position
        WSMI.t2_dl = br.ReadUInt32
        WSMI.t2_dc = br.ReadUInt32

        ReDim WSMI.wsmi_t2(WSMI.t2_dc)
        For k = 0 To WSMI.t2_dc - 2
            WSMI.wsmi_t2(k).flag1 = br.ReadUInt32
        Next

        br.Close()
        ms.Dispose()

    End Sub

    Public Sub get_BWWa_data(ByVal t_cnt As Integer)
        'water data
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0

        BWWa.t1_start = br.BaseStream.Position
        BWWa.t1_dl = br.ReadUInt32
        BWWa.t1_dc = br.ReadUInt32
        ReDim BWWa.bwwa_t1(1)
        If BWWa.t1_dc = 0 Then
            'no water
            water.IsWater = False
            Return
        End If
        Try
            BWWa.bwwa_t1(0).position.x = br.ReadSingle
            BWWa.bwwa_t1(0).position.y = br.ReadSingle
            BWWa.bwwa_t1(0).position.z = br.ReadSingle
            BWWa.bwwa_t1(0).width = br.ReadSingle
            BWWa.bwwa_t1(0).height = br.ReadSingle
            water.IsWater = True
        Catch ex As Exception
            water.IsWater = False
        End Try

    End Sub


    Public Sub get_BSMI_data(ByVal t_cnt As Integer)
        'model matrix and index data
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0

        BSMI.t1_start = br.BaseStream.Position
        BSMI.t1_dl = br.ReadUInt32
        BSMI.t1_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t1(BSMI.t1_dc)
        For k = 0 To BSMI.t1_dc - 1
            ReDim BSMI.bsmi_t1(k).matrix(16)
            For i = 0 To 15
                BSMI.bsmi_t1(k).matrix(i) = br.ReadSingle
            Next
        Next


        BSMI.t2_start = br.BaseStream.Position
        BSMI.t2_dl = br.ReadUInt32
        BSMI.t2_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t2(BSMI.t2_dc)
        For k = 0 To BSMI.t2_dc - 1
            BSMI.bsmi_t2(k).u1_Index = br.ReadUInt32
            BSMI.bsmi_t2(k).u2_Index = br.ReadUInt32
            'Console.WriteLine(k.ToString("0000") + " : " + BSMI.bsmi_t2(k).u1_Index.ToString("x8") + " : " + _
            '                   BSMI.bsmi_t2(k).u1_Index.ToString("x8"))

        Next

        'not sure what this is
        Dim n1 As UInteger
        'read off 12 more
        'now there is a huge bunch of more FFFFFFFF data
        n1 = br.ReadUInt32 'lenfth 4 uints
        Dim c1 As UInt32 = br.ReadUInt32 'count
        Dim offset As UInt32 = n1 * c1
        br.BaseStream.Position += offset ' move to next block

        BSMI.t3_start = br.BaseStream.Position
        BSMI.t3_dl = br.ReadUInt32
        BSMI.t3_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t3(BSMI.t3_dc)
        Dim max As Integer = 0
        For k = 0 To BSMI.t3_dc - 1
            BSMI.bsmi_t3(k).BSMO_Index = br.ReadUInt32
            If max < BSMI.bsmi_t3(k).BSMO_Index Then
                max = BSMI.bsmi_t3(k).BSMO_Index
            End If
        Next
        'As far as I can tell, the rest of this data is
        'all FFFF FFFFF (-1 hex) or BF80 00000 (-1.0 float)
        'not sure what its for but I dont need it
        Return

        BSMI.t4_start = br.BaseStream.Position
        BSMI.t4_dl = br.ReadUInt32
        BSMI.t4_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t4(BSMI.t4_dc)
        For k = 0 To BSMI.t4_dc - 1
            BSMI.bsmi_t4(k).u1_index = br.ReadUInt32
        Next
        BSMI.t5_start = br.BaseStream.Position
        BSMI.t5_dl = br.ReadUInt32
        BSMI.t5_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t5(BSMI.t5_dc)
        For k = 0 To BSMI.t5_dc - 1
            BSMI.bsmi_t5(k).u1_index = br.ReadUInt32
        Next

        BSMI.t6_start = br.BaseStream.Position
        BSMI.t6_dl = br.ReadUInt32
        BSMI.t6_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t6(BSMI.t6_dc)
        For k = 0 To BSMI.t6_dc - 1
            BSMI.bsmi_t6(k).u1_index = br.ReadUInt32
            BSMI.bsmi_t6(k).u2_index = br.ReadUInt32
            BSMI.bsmi_t6(k).u3_index = br.ReadUInt32
            BSMI.bsmi_t6(k).u4_index = br.ReadUInt32
        Next

        BSMI.t6_start = br.BaseStream.Position
        BSMI.t6_dl = br.ReadUInt32
        BSMI.t6_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t6(BSMI.t6_dc)
        For k = 0 To BSMI.t6_dc - 1
            BSMI.bsmi_t6(k).u1_index = br.ReadUInt32
        Next

        br.Close()
        ms.Dispose()

    End Sub

    Public Sub get_BWT2_data(ByVal t_cnt As Integer)
        'terrain 2 data
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0
        'There is some data at the start that makes no sense.
        Dim d_size As UInt32 = br.ReadUInt32
        BWT2.grid_Meter_size = br.ReadSingle
        For k = 0 To 3
            br.ReadUInt32() ' no idea what these 4 uint32s are for
        Next
        BWT2.t_1_start = ms.Position
        BWT2.t_1_d_Length = br.ReadUInt32
        BWT2.t_1_entry_count = br.ReadUInt32
        Dim size As Integer = Sqrt(BWT2.t_1_entry_count)
        ReDim BWT2.location_table_1(BWT2.t_1_entry_count)
        For k = 0 To BWT2.t_1_entry_count - 1
            BWT2.location_table_1(k).key = br.ReadUInt32
            BWT2.location_table_1(k).location = br.ReadUInt32
            BWT2.location_table_1(k).loc_str = BWT2.location_table_1(k).location.ToString("x8")
            maplist(k).name = BWT2.location_table_1(k).loc_str
            BWT2.location_table_1(k).cdata_str = BWT2.location_table_1(k).loc_str + "o.cdata"
        Next

        BWT2.t_2_start = ms.Position
        BWT2.t_2_d_Length = br.ReadUInt32
        BWT2.t_2_entry_count = br.ReadUInt32

        ReDim BWT2.index_list_2(BWT2.t_2_entry_count)
        ReDim BWT2.sorted_table(BWT2.t_2_entry_count)
        For k = 0 To BWT2.t_2_entry_count - 1
            BWT2.index_list_2(k).index = br.ReadUInt32
        Next
        Dim sb As New StringBuilder
        sb.Length = 0
        For c = 0 To size - 1
            'sb.Append(":")
            For r = 0 To size - 1
                'sb.Append(maplist((c * size) + r).name)
                If maplist((c * size) + r).location.x > 0 Then
                    sb.Append("( " + maplist((c * size) + r).location.x.ToString("000.0"))
                Else
                    sb.Append("(" + maplist((c * size) + r).location.x.ToString("000.0"))

                End If
                sb.Append(":")
                If maplist((c * size) + r).location.y > 0 Then
                    sb.Append(" " + maplist((c * size) + r).location.y.ToString("000.0") + ")" + " ")
                Else
                    sb.Append(maplist((c * size) + r).location.y.ToString("000.0") + ")" + " ")
                End If
            Next
            sb.Append(vbCrLf)
        Next
        'Console.WriteLine(sb.ToString)
        'For k = 0 To 6
        '    br.ReadUInt32() '7 : no idea what these are
        'Next
        'BWT2.t_3_start = ms.Position
        'BWT2.t_3_d_Length = br.ReadUInt32
        'BWT2.t_3_entry_count = br.ReadUInt32

        'ReDim BWT2.location_table_3(BWT2.t_3_entry_count)
        'For k = 0 To BWT2.t_3_entry_count - 1
        '    BWT2.location_table_3(k).LX = br.ReadSingle
        '    BWT2.location_table_3(k).min = br.ReadSingle
        '    BWT2.location_table_3(k).LY = br.ReadSingle
        '    BWT2.location_table_3(k).UX = br.ReadSingle
        '    BWT2.location_table_3(k).max = br.ReadSingle
        '    BWT2.location_table_3(k).UY = br.ReadSingle
        'Next
        'Dim index As UInt32 = 0
        'For k = 0 To BWT2.t_2_entry_count - 1
        '    index = BWT2.index_list_2(k).index
        '    BWT2.sorted_table(k).LX = BWT2.location_table_3(index).LX
        '    BWT2.sorted_table(k).min = BWT2.location_table_3(index).min
        '    BWT2.sorted_table(k).LY = BWT2.location_table_3(index).LY
        '    BWT2.sorted_table(k).UX = BWT2.location_table_3(index).UX
        '    BWT2.sorted_table(k).max = BWT2.location_table_3(index).max
        '    BWT2.sorted_table(k).UY = BWT2.location_table_3(index).UY
        '    Console.Write("UX: " + BWT2.location_table_3(k).LX.ToString("0000.0") + vbTab)
        '    Console.Write("UY: " + BWT2.location_table_3(k).LY.ToString("0000.0") + vbTab)
        '    Console.Write("LX: " + BWT2.location_table_3(k).UX.ToString("0000.0") + vbTab)
        '    Console.Write("LY: " + BWT2.location_table_3(k).UY.ToString("0000.0") + vbCrLf)
        'Next

        br.Close()
        ms.Dispose()
    End Sub

    Public Sub get_WGSD_data(ByVal t_cnt As Integer)
        'Static Decals
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0
        WGSD.d_length = br.ReadUInt32
        WGSD.entry_count = br.ReadUInt32
        Dim temp(0) As decal_matrix_list_
        ReDim temp(WGSD.entry_count - 1)
        ReDim WGSD.Table_Entries(WGSD.entry_count)
        ReDim decal_matrix_list(WGSD.entry_count - 1)

        For k = 0 To WGSD.entry_count - 1
            'If k = 350 Then
            '    Stop
            'End If
            decal_matrix_list(k) = New decal_matrix_list_
            decal_matrix_list(k).t_bias = default_terrain_bias
            decal_matrix_list(k).d_bias = default_decal_bias
            decal_matrix_list(k).exclude = False
            ReDim Preserve WGSD.Table_Entries(k).matrix(16)
            WGSD.Table_Entries(k).unknown_1 = br.ReadUInt32 'Unknown always 0?
            For i = 0 To 15
                WGSD.Table_Entries(k).matrix(i) = br.ReadSingle
            Next
            decal_matrix_list(k).matrix = WGSD.Table_Entries(k).matrix

            WGSD.Table_Entries(k).diffuseMapKey = br.ReadUInt32
            WGSD.Table_Entries(k).normalMapKey = br.ReadUInt32
            WGSD.Table_Entries(k).u_key = br.ReadUInt32

            WGSD.Table_Entries(k).unknown_2 = br.ReadUInt32 'Unknown always 0?

            WGSD.Table_Entries(k).flags = br.ReadUInt32
            decal_matrix_list(k).flags = WGSD.Table_Entries(k).flags

            WGSD.Table_Entries(k).off_x = br.ReadSingle
            WGSD.Table_Entries(k).off_y = br.ReadSingle
            WGSD.Table_Entries(k).off_z = br.ReadSingle
            WGSD.Table_Entries(k).off_w = br.ReadSingle

            decal_matrix_list(k).offset.z = WGSD.Table_Entries(k).off_x
            decal_matrix_list(k).offset.y = WGSD.Table_Entries(k).off_y
            decal_matrix_list(k).offset.z = WGSD.Table_Entries(k).off_z
            decal_matrix_list(k).offset.w = WGSD.Table_Entries(k).off_w

            'If decal_matrix_list(k).offset.x <> 0 Then
            '    Stop
            'End If
            'If decal_matrix_list(k).offset.y <> 0 Then
            '    Stop
            'End If
            'If decal_matrix_list(k).offset.z <> 0 Then
            '    Stop
            'End If
            'If decal_matrix_list(k).offset.w <> 0 Then
            '    Stop
            'End If

            WGSD.Table_Entries(k).uv_wrapping_u = br.ReadSingle
            WGSD.Table_Entries(k).uv_wrapping_v = br.ReadSingle
            decal_matrix_list(k).u_wrap = WGSD.Table_Entries(k).uv_wrapping_u
            decal_matrix_list(k).v_wrap = WGSD.Table_Entries(k).uv_wrapping_v
            'If WGSD.Table_Entries(k).uv_wrapping_u > 1.0 Then
            '    Stop
            'End If
            'If WGSD.Table_Entries(k).uv_wrapping_v > 1.0 Then
            '    Stop
            'End If

            WGSD.Table_Entries(k).visibilityMask = br.ReadUInt32 'always 0xFFFFFFFF?
            'now we can get the strings from the keys.
            WGSD.Table_Entries(k).diffuseMap = find_str_BWST(WGSD.Table_Entries(k).diffuseMapKey)
            WGSD.Table_Entries(k).normalMap = find_str_BWST(WGSD.Table_Entries(k).normalMapKey)
            WGSD.Table_Entries(k).extraMap = find_str_BWST(WGSD.Table_Entries(k).u_key)
            decal_matrix_list(k).decal_texture = WGSD.Table_Entries(k).diffuseMap
            '' the normal map for Stone_06 does not exist in the pkg files!!
            If decal_matrix_list(k).decal_texture.Contains("Stone06.") Then
                WGSD.Table_Entries(k).normalMap = "Stone06_NM.dds"
            End If
            decal_matrix_list(k).decal_normal = WGSD.Table_Entries(k).normalMap
            decal_matrix_list(k).influence = CSng(WGSD.Table_Entries(k).flags And &HFF00) / 256.0
            decal_matrix_list(k).priority = (WGSD.Table_Entries(k).flags And &HFF)
            Debug.WriteLine("ID:" + k.ToString)
            Debug.WriteLine(decal_matrix_list(k).decal_texture)
            Debug.WriteLine(decal_matrix_list(k).influence.ToString)
            Debug.WriteLine(CStr(WGSD.Table_Entries(k).flags And &HFF0000) / 65536)
        Next

        'have to sort these by priority.
        Dim cnt As Integer = 0
        Dim level As Integer = 0
        For k = 0 To 300
            For i = 0 To temp.Length - 1
                Dim t = decal_matrix_list(i)
                If t.priority = level Then
                    temp(cnt) = t
                    cnt += 1
                End If
            Next
            level += 1
        Next

        'Console.WriteLine("------------------------------")

        For i = 0 To temp.Length - 1
            Dim t = temp(i)
            decal_matrix_list(i) = t
            'Console.WriteLine("b1:" + decal_matrix_list(i).priority.ToString("00") + " id:" + i.ToString("0000"))
        Next

        br.Close()
        ms.Dispose()
    End Sub

    Public Sub get_BSGD_data(ByVal t_cnt As Integer)
        'game data
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ReDim BSGD.data(ms.Length)
        ms.Position = 0
        For i = 0 To ms.Length - 1
            BSGD.data(i) = br.ReadByte
        Next
        br.Dispose()
        ms.Close()
        ms.Dispose()
    End Sub

    Public Sub get_BWSG_data(ByVal t_cnt As Integer)
        'model data.. need to use this at some point.
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0

        'string table 0
        BWSG.table_0_d_length = br.ReadUInt32
        BWSG.table_0_entry_count = br.ReadUInt32
        ReDim BWSG.str_entries(BWSG.table_0_entry_count)
        Dim model_count As Integer = 0
        Dim old_pos = ms.Position
        '------------------------------------------------------------
        bw_strings.AppendLine("-------- BWSG STRINGS ----------")
        For k = 0 To BWSG.table_0_entry_count - 1
            ms.Position = old_pos
            BWSG.str_entries(k).key = br.ReadUInt32
            BWSG.str_entries(k).offset = br.ReadUInt32
            BWSG.str_entries(k).str_length = br.ReadUInt32
            old_pos = ms.Position
            '
            ms.Position = (BWSG.table_0_d_length * BWSG.table_0_entry_count) + 12 + BWSG.str_entries(k).offset
            Dim ds = br.ReadBytes(BWSG.str_entries(k).str_length)
            BWSG.str_entries(k).str = System.Text.Encoding.UTF8.GetString(ds, 0, BWSG.str_entries(k).str_length)
            '------------------------------------------------------------
            bw_strings.AppendLine(BWSG.str_entries(k).str)
            If BWSG.str_entries(k).str.Contains("vertices") Then
                model_count += 1
            End If
        Next
        ms.Position = old_pos
        Dim str_total_length = br.ReadUInt32
        Dim tbl0_end = str_total_length + (BWSG.table_0_d_length * BWSG.table_0_entry_count) + 12
        'table 1
        BWSG.table_1_start = tbl0_end
        ms.Position = tbl0_end
        BWSG.table_1_d_length = br.ReadUInt32
        BWSG.table_1_entry_count = br.ReadUInt32
        ReDim BWSG.Table_1_entries(BWSG.table_1_entry_count)
        Dim tbl1_end = ms.Position + (BWSG.table_1_d_length * BWSG.table_1_entry_count)
        For k = 0 To BWSG.table_1_entry_count - 1
            BWSG.Table_1_entries(k).key1 = br.ReadUInt32
            BWSG.Table_1_entries(k).start_index = br.ReadUInt32
            BWSG.Table_1_entries(k).end_index = br.ReadUInt32
            BWSG.Table_1_entries(k).vertex_count = br.ReadUInt32
            BWSG.Table_1_entries(k).key2 = br.ReadUInt32
            BWSG.Table_1_entries(k).vertex_type = find_str_BWSG(BWSG.Table_1_entries(k).key2)
            BWSG.Table_1_entries(k).model = find_str_BWSG(BWSG.Table_1_entries(k).key1)
        Next

        'table 2
        BWSG.table_2_start = tbl1_end
        ms.Position = tbl1_end

        BWSG.table_2_d_length = br.ReadUInt32
        BWSG.table_2_entry_count = br.ReadUInt32
        ReDim BWSG.table_2_entries(BWSG.table_2_entry_count)
        For k = 0 To BWSG.table_2_entry_count - 1
            BWSG.table_2_entries(k).block_type = br.ReadUInt32
            BWSG.table_2_entries(k).vertex_stride = br.ReadUInt32
            BWSG.table_2_entries(k).data_length = br.ReadUInt32
            BWSG.table_2_entries(k).section_index = br.ReadUInt32
            BWSG.table_2_entries(k).offset = br.ReadUInt32
            If BWSG.table_2_entries(k).block_type <> 0 Then
                If BWSG.table_2_entries(k).block_type <> &HA Then
                    If BWSG.table_2_entries(k).block_type <> &HB Then
                        Stop
                    End If

                End If
            End If
        Next
        Dim tbl2_end = ms.Position

        BWSG.table_3_start = tbl2_end
        BWSG.table_3_d_length = br.ReadUInt32
        BWSG.table_3_entry_count = br.ReadUInt32
        Dim tbl3_end = ms.Position + (BWSG.table_2_d_length * BWSG.table_3_d_length) + 8
        ReDim BWSG.table_3_entries(BWSG.table_3_entry_count)
        ReDim BWSG.data_chunks(BWSG.table_3_entry_count)
        For k = 0 To BWSG.table_3_entry_count - 1
            BWSG.table_3_entries(k).data_length = br.ReadUInt32
        Next
        BWSG.table_4_start = ms.Position
        Dim BSGD_ms As New MemoryStream(BSGD.data)
        Dim BSGD_br As New BinaryReader(BSGD_ms)
        For k = 0 To BWSG.table_3_entry_count - 1
            ReDim BWSG.data_chunks(k).data(BWSG.table_3_entries(k).data_length)
            BWSG.data_chunks(k).data = BSGD_br.ReadBytes(BWSG.table_3_entries(k).data_length)
        Next
        BSGD_br.Dispose()
        BSGD_ms.Close()
        BSGD_ms.Dispose()
        Dim byt As Byte
        Dim running As Long = 0
        For i = 0 To BWSG.table_2_entry_count - 1
            Dim l = BWSG.table_2_entries(i).data_length
            Dim o = BWSG.table_2_entries(i).offset
            Dim ind = BWSG.table_2_entries(i).section_index
            ReDim BWSG.table_2_entries(i).data(l - 1)
            running += l
            For k = 0 To l - 1
                byt = BWSG.data_chunks(ind).data(o + k)
                BWSG.table_2_entries(i).data(k) = byt
            Next
            'Dim path_ = "C:\!!_ENSK\BWSG_DUMP\" + "Chunk_" + ind.ToString("00") + "_" + i.ToString("0000") + ".bin"
            'File.WriteAllBytes(path_, BWSG.table_2_entries(i).data)

        Next

        br.Close()
        ms.Dispose()



    End Sub

    Public Sub get_BWST_data(ByVal t_cnt As Integer)
        'string table
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0
        BWST.d_length = br.ReadUInt32 ' lenght of data
        BWST.entry_count = br.ReadUInt32 ' number of entries
        'Dim unknown = br.ReadUInt32
        '------------------------------------------------------------
        bw_strings.AppendLine("-------- BWST STRINGS ----------")
        ReDim BWST.entries(BWST.entry_count)
        Dim old_pos = ms.Position
        Dim start_offset As Long = (BWST.d_length * BWST.entry_count) + 12
        For k = 0 To BWST.entry_count - 1
            ms.Position = old_pos
            BWST.entries(k).key = br.ReadUInt32
            BWST.entries(k).offset = br.ReadUInt32
            BWST.entries(k).str_length = br.ReadUInt32
            old_pos = ms.Position
            ms.Position = BWST.entries(k).offset + start_offset
            Dim ds() = br.ReadBytes(BWST.entries(k).str_length)

            BWST.entries(k).str = System.Text.Encoding.UTF8.GetString(ds, 0, BWST.entries(k).str_length)
            '------------------------------------------------------------
            bw_strings.AppendLine(BWST.entries(k).str)
        Next

        br.Close()
        ms.Dispose()
    End Sub

    Public Sub get_SpTr_data(ByVal t_cnt As Integer)
        'speed tree data
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0
        SpTr.d_length = br.ReadUInt32 ' lenght of data
        SpTr.entry_count = br.ReadUInt32 ' number of entries

        ReDim SpTr.Stree(SpTr.entry_count)
        ReDim speedtree_matrix_list(SpTr.entry_count)

        For k = 0 To SpTr.entry_count - 1
            speedtree_matrix_list(k) = New speedtree_matrix_list_
            ReDim speedtree_matrix_list(k).matrix(16)
            ReDim SpTr.Stree(k).Matrix(16)
            For i = 0 To 15
                speedtree_matrix_list(k).matrix(i) = br.ReadSingle
            Next
            'For i = 0 To 15
            '    SpTr.Stree(k).Matrix(i) = br.ReadSingle
            'Next
            SpTr.Stree(k).key = br.ReadUInt32
            SpTr.Stree(k).e1 = br.ReadUInt32
            SpTr.Stree(k).e2 = br.ReadUInt32
            SpTr.Stree(k).e3 = br.ReadUInt32

            speedtree_matrix_list(k).tree_name = find_str_BWST(SpTr.Stree(k).key).Replace("spt", "ctree")
        Next


        br.Close()
        ms.Dispose()
    End Sub

    Private Function find_str_BWST(ByVal key As UInt32) As String
        For z = 0 To BWST.entry_count - 1
            If key = BWST.entries(z).key Then
                'Console.WriteLine("key: " + key.ToString("x8").ToUpper + " " + BWST.entries(z).str + vbCrLf)
                Return BWST.entries(z).str
            End If
        Next
        Return "ERROR!"
    End Function
    Private Function find_str_BWSG(ByVal key As UInt32) As String
        For z = 0 To BWSG.table_0_entry_count - 1
            If key = BWSG.str_entries(z).key Then
                'Console.WriteLine("key: " + key.ToString("x8").ToUpper + vbCrLf)
                Return BWSG.str_entries(z).str
            End If
        Next
        Return "ERROR!"
    End Function
    Private Function find_str_SpTr(ByVal key As UInt32) As String
        For z = 0 To SpTr.entry_count - 1
            If key = SpTr.Stree(z).key Then
                'Console.WriteLine("key: " + key.ToString("x8").ToUpper + vbCrLf)
                Return SpTr.Stree(z).Tree_name
            End If
        Next
        Return "ERROR!"
    End Function
End Module
