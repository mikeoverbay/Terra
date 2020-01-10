
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
        Public BB_Min As vect3
        Public BB_Max As vect3
        Public BB() As vect3
        Public exclude As Boolean
        Public destructible As Boolean
        Public exclude_list() As Integer
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
        Public texture_id As Integer
        Public normal_id As Integer
        Public gmm_id As Integer
        Public display_id As Integer
        Public decal_texture As String
        Public decal_normal As String
        Public decal_gmm As String
        Public matrix() As Single
        Public good As Boolean
        Public offset As vect4
        Public priority As Integer
        Public influence As Integer
        Public texture_matrix() As Single
        Public lbl As vect3
        Public lbr As vect3
        Public ltl As vect3
        Public ltr As vect3
        Public rbl As vect3
        Public rbr As vect3
        Public rtl As vect3
        Public rtr As vect3
        Public BB() As vect3
        Public visible As Boolean
        Public flags As UInteger
        Public cull_method As Integer
    End Structure
#End Region


    Public Sub get_BSMA_data(ByVal t_cnt As Integer)
        'bigworld Static Material Array
        'more model data.
        'ms is pointing at the BSMA secion after this next line
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0
        'material info
        BSMA.t1_start = br.BaseStream.Position
        BSMA.t2_dl = br.ReadUInt32
        BSMA.t1_dc = br.ReadUInt32
        ReDim BSMA.bsma_t1(BSMA.t1_dc)
        For k = 0 To BSMA.t1_dc - 1
            BSMA.bsma_t1(k).fx_index = br.ReadUInt32
            BSMA.bsma_t1(k).index_start = br.ReadUInt32 ' t3 ref
            BSMA.bsma_t1(k).index_end = br.ReadUInt32 ' t3 ref
            BSMA.bsma_t1(k).bwst_key = br.ReadUInt32
            BSMA.bsma_t1(k).identifier = find_str_BWST(BSMA.bsma_t1(k).bwst_key)
        Next
        'ms.Close()
        'Return
        Dim cn = br.ReadUInt32
        Dim num = br.ReadUInt32
        ReDim BSMA.shaders(num)
        For i = 0 To (num) - 1
            Dim hv = br.ReadUInt32
            BSMA.shaders(i) = find_str_BWST(hv)
        Next
        'Property info
        BSMA.t1_start = br.BaseStream.Position
        BSMA.t2_dl = br.ReadUInt32
        BSMA.t2_dc = br.ReadUInt32
        ReDim BSMA.bsma_t2(BSMA.t2_dc)
        For k = 0 To BSMA.t2_dc - 1
            BSMA.bsma_t2(k).bwst_key = br.ReadUInt32
            BSMA.bsma_t2(k).property_type = br.ReadUInt32
            BSMA.bsma_t2(k).bwst_key_or_value = br.ReadUInt32
            'this either points as a string or is a value depending on BSMA.bsma_t2(k).property_type
            'property types (BSMA.bsma_t2(k).property_type)
            '0 = ?
            '1 = boolean, bwst_key_or_value = 1 if true, 0 if false
            '2 = float, read the single other wise read as uint32.. se code above.
            '3 = integer, bwst_key_or_value = value
            '4 =
            '5 = vec4, bwst_key_or_value = start index in to float table. n*4 will point at first entry
            '6 = texture name, bwst_key_or_value = look up key

            If BSMA.bsma_t2(k).property_type = 4 Then
                Debug.WriteLine("id:" + k.ToString("00000") + " : " + BSMA.bsma_t2(k).bwst_key_or_value)
            End If

            BSMA.bsma_t2(k).property_string = find_str_BWST(BSMA.bsma_t2(k).bwst_key)
            If BSMA.bsma_t2(k).property_type = 1 Then
                If BSMA.bsma_t2(k).bwst_key_or_value = 1 Then
                    BSMA.bsma_t2(k).val_boolean = True
                Else
                    BSMA.bsma_t2(k).val_boolean = False
                End If
            End If
            If BSMA.bsma_t2(k).property_type = 2 Then
                'BSMA.bsma_t2(k).bwst_key_or_value is a float stored as a uint32.
                'We must convert it from hex value to float
                Dim hexString = BSMA.bsma_t2(k).bwst_key_or_value.ToString("x8")
                Dim floatVals() = BitConverter.GetBytes(BSMA.bsma_t2(k).bwst_key_or_value)
                BSMA.bsma_t2(k).val_float = BitConverter.ToSingle(floatVals, 0)
            End If

            If BSMA.bsma_t2(k).property_type = 3 Then
                BSMA.bsma_t2(k).val_int = BSMA.bsma_t2(k).bwst_key_or_value
            End If
            '5 handled is below
            If BSMA.bsma_t2(k).property_type = 6 Then
                BSMA.bsma_t2(k).texture_string = find_str_BWST(BSMA.bsma_t2(k).bwst_key_or_value)
            End If
        Next
        'got 2 32bit words .. no idea
        br.ReadUInt32()
        br.ReadUInt32()
        'float number table (Or vector4 numbers used with property_type 5)
        BSMA.t3_dl = br.ReadUInt32
        BSMA.t3_dc = br.ReadUInt32
        Dim zz = 0
        ReDim BSMA.bsma_t3((BSMA.t3_dc * 4) - 1)

        For i = 0 To (BSMA.t3_dc * 4) - 1
            Dim hv = br.ReadSingle
            BSMA.bsma_t3(i).float_val = hv
        Next

        'we need to get the vector4 tables and store them now that we have the vector4s
        For k = 0 To BSMA.t2_dc - 1
            If BSMA.bsma_t2(k).property_type = 5 Then
                Dim sIndx = 4 * BSMA.bsma_t2(k).bwst_key_or_value ' index in to vector4 list
                BSMA.bsma_t2(k).val_vec4.x = BSMA.bsma_t3(sIndx + 0).float_val
                BSMA.bsma_t2(k).val_vec4.y = BSMA.bsma_t3(sIndx + 1).float_val
                BSMA.bsma_t2(k).val_vec4.z = BSMA.bsma_t3(sIndx + 2).float_val
                BSMA.bsma_t2(k).val_vec4.w = BSMA.bsma_t3(sIndx + 3).float_val
            End If

        Next
        Return

        ms.Close()
  
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
            BSMO.bsmo_t1(k).exclude_list = br.ReadUInt32
            BSMO.bsmo_t1(k).mask_pointer = br.ReadUInt32
        Next
        '
        BSMO.t2_start = br.BaseStream.Position
        BSMO.t2_dl = br.ReadUInt32
        BSMO.t2_dc = br.ReadUInt32
        'skip unknow shit
        Dim np = BSMO.t2_dc * BSMO.t2_dl + BSMO.t2_start
        br.BaseStream.Position = np

        'these 2 are junk i guess
        BSMO.t2_dl = br.ReadUInt32
        BSMO.t2_dc = br.ReadUInt32

        'get node entries
        BSMO.t2_start = br.BaseStream.Position
        BSMO.t2_dl = br.ReadUInt32
        BSMO.t2_dc = br.ReadUInt32

        ReDim BSMO.bsmo_t2(BSMO.t2_dc)

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
            BSMO.bsmo_t4(k).min_BB.x = -br.ReadSingle
            BSMO.bsmo_t4(k).min_BB.y = br.ReadSingle
            BSMO.bsmo_t4(k).min_BB.z = br.ReadSingle

            BSMO.bsmo_t4(k).max_BB.x = -br.ReadSingle
            BSMO.bsmo_t4(k).max_BB.y = br.ReadSingle
            BSMO.bsmo_t4(k).max_BB.z = br.ReadSingle
        Next

        BSMO.t10_start = br.BaseStream.Position
        BSMO.t10_dl = br.ReadUInt32
        BSMO.t10_dc = br.ReadUInt32
        ReDim BSMO.bsmo_t10(BSMO.t10_dc)
        For i = 0 To BSMO.t10_dc - 1
            BSMO.bsmo_t10(i).int1 = br.ReadUInt32
            BSMO.bsmo_t10(i).int2 = br.ReadUInt32
        Next

        BSMO.t11_start = br.BaseStream.Position
        BSMO.t11_dl = br.ReadUInt32
        BSMO.t11_dc = br.ReadUInt32
        ReDim BSMO.bsmo_t11(BSMO.t11_dc)
        For i = 0 To BSMO.t11_dc - 1
            BSMO.bsmo_t11(i).int1 = br.ReadUInt32
        Next

        BSMO.t5_start = br.BaseStream.Position
        BSMO.t5_dl = br.ReadUInt32
        BSMO.t5_dc = br.ReadUInt32
        ReDim BSMO.bsmo_t5(BSMO.t5_dc)
        'get table data
        For k = 0 To BSMO.t5_dc - 1
            BSMO.bsmo_t5(k).mask = br.ReadUInt32
        Next

        BSMO.t6_start = br.BaseStream.Position
        BSMO.t6_dl = br.ReadUInt32
        BSMO.t6_dc = br.ReadUInt32
        ReDim BSMO.bsmo_t6(BSMO.t6_dc)
        For k = 0 To BSMO.t6_dc - 1
            BSMO.bsmo_t6(k).exclude_start = br.ReadUInt32
            BSMO.bsmo_t6(k).exclude_end = br.ReadUInt32 'list of hiden primitiveGroup
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
            Dim l, r As vect3
            l.x = br.ReadSingle
            l.y = br.ReadSingle
            l.z = br.ReadSingle

            r.x = br.ReadSingle
            r.y = br.ReadSingle
            r.z = br.ReadSingle

            BWWa.bwwa_t1(0).position.x = (l.x + r.x) / 2.0!
            BWWa.bwwa_t1(0).position.y = l.y
            BWWa.bwwa_t1(0).position.z = (l.z + r.z) / 2.0!
            BWWa.bwwa_t1(0).width = Abs(l.x) + Abs(r.x)
            BWWa.bwwa_t1(0).plane = l.y
            BWWa.bwwa_t1(0).height = Abs(l.z) + Abs(r.z)
            water.IsWater = True
            WATER_LINE_ = BWWa.bwwa_t1(0).position.y
        Catch ex As Exception
            water.IsWater = False
            WATER_LINE_ = -500.0
        End Try

    End Sub


    Public Sub get_BSMI_data(ByVal t_cnt As Integer)
        'model matrix and index data
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0
        '============================================
        'Transform
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
        '============================================
        'Trransform
        'chunk models

        BSMI.t2_start = br.BaseStream.Position
        BSMI.t2_dl = br.ReadUInt32
        BSMI.t2_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t2(BSMI.t2_dc)
        For k = 0 To BSMI.t2_dc - 1
            BSMI.bsmi_t2(k).u1_Index = br.ReadUInt32
            BSMI.bsmi_t2(k).u2_Index = br.ReadUInt32
            If BSMI.bsmi_t2(k).u1_Index = &H109 Then
                Debug.WriteLine(k.ToString("0000") + " : " + BSMI.bsmi_t2(k).u1_Index.ToString("x8") + " : " + _
                                BSMI.bsmi_t2(k).u1_Index.ToString("x8"))
            End If
        Next

        'List of destructible models
        'now there is a huge bunch of more FFFFFFFF data
        'table order is wrong but i dont want to rename all of them because of this new table.
        BSMI.t7_start = br.BaseStream.Position

        BSMI.t7_dl = br.ReadUInt32 'length 4 uints
        BSMI.t7_dc = br.ReadUInt32 'count
        Debug.WriteLine("--- BSMI.t7 ----")
        ReDim BSMI.bsmi_t7(BSMI.t7_dc)
        For k = 0 To BSMI.t7_dc - 1
            BSMI.bsmi_t7(k).v_mask = br.ReadUInt32
            If BSMI.bsmi_t7(k).v_mask <> 4294967295 Then
                Debug.WriteLine(k.ToString("00000000") + ":" + BSMI.bsmi_t7(k).v_mask.ToString)
            End If
        Next
        'br.BaseStream.Position += offset ' move to next block

        BSMI.t3_start = br.BaseStream.Position
        BSMI.t3_dl = br.ReadUInt32
        BSMI.t3_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t3(BSMI.t3_dc)
        'materia Id?
        Dim max As Integer = 0
        For k = 0 To BSMI.t3_dc - 1
            BSMI.bsmi_t3(k).BSMO_Index = br.ReadUInt32 'points as model in BSMO data : BSMO.bsmo_t2(BSMO_Index).model_str
            BSMI.bsmi_t3(k).BSMO_Index2 = br.ReadUInt32 ' visiblity - hides lots of detail models. 0 = extra models 1 = main models
        Next
        'bunch of FFFF values
        BSMI.anim_list_start = ms.Position
        BSMI.anima_dlen = br.ReadUInt32
        BSMI.anima_cnt = br.ReadUInt32
        ReDim BSMI.anima_list(BSMI.anima_cnt - 1)
        For i = 0 To BSMI.anima_cnt - 1
            BSMI.anima_list(i) = br.ReadUInt32
            If BSMI.anima_list(i) <> 4294967295 Then
                'if not FFFFFFFF than its an address of the animation of some kinds
                'this is a animated model
                Debug.WriteLine(BSMI.anima_list(i).ToString("x") + ":" + i.ToString)
            End If
        Next
        'the next table defines if its a animation of some kind.. <> 4294967295 its a animation
        BSMI.t4_start = br.BaseStream.Position
        BSMI.t4_dl = br.ReadUInt32
        BSMI.t4_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t4(BSMI.t4_dc)
        Debug.WriteLine("--- BSMI.t4 ----")
        For k = 0 To BSMI.t4_dc - 1
            BSMI.bsmi_t4(k).u1_index = br.ReadUInt32
            BSMI.bsmi_t4(k).u2_index = br.ReadUInt32
            BSMI.bsmi_t4(k).str_key = br.ReadUInt32
            BSMI.bsmi_t4(k).flags = br.ReadUInt32
            BSMI.bsmi_t4(k).mask = br.ReadUInt32
            BSMI.bsmi_t4(k).factor = br.ReadSingle
            BSMI.bsmi_t4(k).s1 = br.ReadSingle
            BSMI.bsmi_t4(k).s2 = br.ReadSingle

            If BSMI.bsmi_t4(k).mask <> 4294967295 Then
                Debug.WriteLine(k.ToString("00000000") + ":" + BSMI.bsmi_t4(k).u1_index.ToString)
            End If
        Next
        'the next table defines ??
        BSMI.t5_start = br.BaseStream.Position
        BSMI.t5_dl = br.ReadUInt32
        BSMI.t5_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t5(BSMI.t5_dc)
        For k = 0 To BSMI.t5_dc - 1
            BSMI.bsmi_t5(k).u1_index = br.ReadUInt32
        Next

        '
        'broken models?
        BSMI.t6_start = br.BaseStream.Position
        BSMI.t6_dl = br.ReadUInt32
        BSMI.t6_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t6(BSMI.t6_dc)
        For k = 0 To BSMI.t6_dc - 1
            BSMI.bsmi_t6(k).u1_index = br.ReadUInt32 'boat id by model index
            BSMI.bsmi_t6(k).u2_index = br.ReadUInt32 'setting of some kind
            BSMI.bsmi_t6(k).u3_index = br.ReadUInt32 'always zero?
            'BSMI.bsmi_t6(k).u4_index = br.ReadUInt32 'setting of some kind
            If BSMI.bsmi_t6(k).u1_index <> 4294967295 Then
                Debug.WriteLine("1 " + k.ToString("00000000") + ":" + BSMI.bsmi_t6(k).u1_index.ToString("x"))
            End If
            If BSMI.bsmi_t6(k).u2_index <> 4294967295 Then
                Debug.WriteLine("2 " + k.ToString("00000000") + ":" + BSMI.bsmi_t6(k).u2_index.ToString("x"))
            End If
            If BSMI.bsmi_t6(k).u3_index <> 4294967295 Then
                Debug.WriteLine("3 " + k.ToString("00000000") + ":" + BSMI.bsmi_t6(k).u3_index.ToString("x"))
            End If
        Next
        Debug.WriteLine("--- BSMI.t6 ----")
        'this table is a list of ?
        BSMI.t8_start = br.BaseStream.Position
        BSMI.t8_dl = br.ReadUInt32
        BSMI.t8_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t8(BSMI.t8_dc)
        For k = 0 To BSMI.t8_dc - 1
            BSMI.bsmi_t8(k).v = br.ReadUInt32 '?
            If BSMI.bsmi_t8(k).v <> 4294967295 Then
                Debug.WriteLine(k.ToString("00000000") + ":" + BSMI.bsmi_t8(k).v.ToString("x"))
            End If
        Next
        'componet list of sub models
        BSMI.t9_start = br.BaseStream.Position
        BSMI.t9_dl = br.ReadUInt32
        BSMI.t9_dc = br.ReadUInt32
        ReDim BSMI.bsmi_t9(BSMI.t9_dc)
        For k = 0 To BSMI.t9_dc - 1
            BSMI.bsmi_t9(k).s1 = br.ReadSingle '????
            BSMI.bsmi_t9(k).s2 = br.ReadSingle '
            BSMI.bsmi_t9(k).s3 = br.ReadSingle '
            BSMI.bsmi_t9(k).s4 = br.ReadSingle '
            BSMI.bsmi_t9(k).s5 = br.ReadSingle '
        Next
        'end of file -----------

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
        ms.Position = &H24
        BWT2.t_1_start = ms.Position
        BWT2.t_1_d_Length = br.ReadUInt32
        BWT2.t_1_entry_count = br.ReadUInt32
        Dim size As Integer = Sqrt(BWT2.t_1_entry_count)
        ReDim BWT2.location_table_1(BWT2.t_1_entry_count)
        ReDim maplist(BWT2.t_1_entry_count + 1)
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
        For k = 0 To 6
            br.ReadUInt32() '7 : no idea what these are
        Next
        BWT2.t_3_start = ms.Position
        BWT2.t_3_d_Length = br.ReadUInt32
        BWT2.t_3_entry_count = br.ReadUInt32

        ReDim BWT2.location_table_3(BWT2.t_3_entry_count)
        For k = 0 To BWT2.t_3_entry_count - 1
            BWT2.location_table_3(k).LX = br.ReadSingle
            BWT2.location_table_3(k).min = br.ReadSingle
            BWT2.location_table_3(k).LY = br.ReadSingle
            BWT2.location_table_3(k).UX = br.ReadSingle
            BWT2.location_table_3(k).max = br.ReadSingle
            BWT2.location_table_3(k).UY = br.ReadSingle
        Next
        Dim index As UInt32 = 0
        For k = 0 To BWT2.t_2_entry_count - 1
            index = BWT2.index_list_2(k).index
            BWT2.sorted_table(k).LX = BWT2.location_table_3(index).LX
            BWT2.sorted_table(k).min = BWT2.location_table_3(index).min
            BWT2.sorted_table(k).LY = BWT2.location_table_3(index).LY
            BWT2.sorted_table(k).UX = BWT2.location_table_3(index).UX
            BWT2.sorted_table(k).max = BWT2.location_table_3(index).max
            BWT2.sorted_table(k).UY = BWT2.location_table_3(index).UY
            Console.Write("UX: " + BWT2.location_table_3(k).LX.ToString("0000.0") + vbTab)
            Console.Write("UY: " + BWT2.location_table_3(k).LY.ToString("0000.0") + vbTab)
            Console.Write("LX: " + BWT2.location_table_3(k).UX.ToString("0000.0") + vbTab)
            Console.Write("LY: " + BWT2.location_table_3(k).UY.ToString("0000.0") + vbCrLf)
        Next

        br.Close()
        ms.Dispose()
    End Sub

    Public Sub get_WGSD_data(ByVal t_cnt As Integer)
        'Static Decals
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0
        Dim ty = br.ReadUInt32() 'type?
        Dim vr = br.ReadUInt32() 'version?
        If ty = 2 And vr = 3 Then
            GoTo read3_only
        End If
        If ty = 1 And vr = 3 Then
            Stop
        End If
        WGSD.entry_count = br.ReadUInt32
        WGSD.d_length = br.ReadUInt32
        Dim temp(0) As decal_matrix_list_
        ReDim temp(WGSD.entry_count - 1)
        ReDim WGSD.Table_Entries(WGSD.entry_count)
        ReDim decal_matrix_list(WGSD.entry_count - 1)
        Debug.WriteLine("Decals====================")
        For k = 0 To WGSD.entry_count - 1
            'If k = 350 Then
            '    Stop
            'End If
            decal_matrix_list(k) = New decal_matrix_list_
            ReDim Preserve WGSD.Table_Entries(k).matrix(16)
            'we have 2 unknown int32s and a byte.. 
            WGSD.Table_Entries(k).v1 = br.ReadUInt32 'Unknown always 0?
            WGSD.Table_Entries(k).v2 = br.ReadUInt32 'Unknown always 0?
            WGSD.Table_Entries(k).accuracyType = br.ReadByte()
            If WGSD.Table_Entries(k).accuracyType <> 0 Then
                'Stop
            End If
            For i = 0 To 15
                WGSD.Table_Entries(k).matrix(i) = br.ReadSingle
            Next
            decal_matrix_list(k).matrix = WGSD.Table_Entries(k).matrix

            WGSD.Table_Entries(k).diffuseMapKey = br.ReadUInt32
            WGSD.Table_Entries(k).normalMapKey = br.ReadUInt32
            WGSD.Table_Entries(k).gmmMapkey = br.ReadUInt32
            WGSD.Table_Entries(k).extrakey = br.ReadUInt32

            If WGSD.Table_Entries(k).extrakey <> 2216829733 Then
                Debug.Write(k.ToString("00000") + " : " + WGSD.Table_Entries(k).extrakey.ToString)
                Debug.WriteLine(" : " + find_str_BWST(WGSD.Table_Entries(k).extrakey))
            End If
            Dim priority = br.ReadUInt32 'Unknown always 0?
            If priority > 0 Then
                'Stop
            End If
            WGSD.Table_Entries(k).flags = br.ReadUInt16
            decal_matrix_list(k).flags = WGSD.Table_Entries(k).flags
            'Debug.WriteLine(k.ToString("0000") + ":" + decal_matrix_list(k).flags.ToString("X"))
            WGSD.Table_Entries(k).off_x = br.ReadSingle
            WGSD.Table_Entries(k).off_y = br.ReadSingle
            WGSD.Table_Entries(k).off_z = br.ReadSingle
            WGSD.Table_Entries(k).off_w = br.ReadSingle

            decal_matrix_list(k).offset.z = WGSD.Table_Entries(k).off_x
            decal_matrix_list(k).offset.y = WGSD.Table_Entries(k).off_y
            decal_matrix_list(k).offset.z = WGSD.Table_Entries(k).off_z
            decal_matrix_list(k).offset.w = WGSD.Table_Entries(k).off_w

            If decal_matrix_list(k).offset.x <> 0 Then
                'Stop
            End If
            If decal_matrix_list(k).offset.y <> 0 Then
                'Stop
            End If
            If decal_matrix_list(k).offset.z <> 0 Then
                'Stop
            End If
            If decal_matrix_list(k).offset.w <> 0 Then
                'Stop
            End If

            WGSD.Table_Entries(k).uv_wrapping_u = br.ReadSingle
            WGSD.Table_Entries(k).uv_wrapping_v = br.ReadSingle
            decal_matrix_list(k).u_wrap = WGSD.Table_Entries(k).uv_wrapping_u
            decal_matrix_list(k).v_wrap = WGSD.Table_Entries(k).uv_wrapping_v

            WGSD.Table_Entries(k).visibilityMask = br.ReadUInt32 'always 0xFFFFFFFF?
            Dim un = br.ReadUInt32
            If WGSD.Table_Entries(k).visibilityMask <> 4294967295 Then
                'Stop
                'Debug.WriteLine(k.ToString + ":" + WGSD.Table_Entries(k).visibilityMask.ToString("00000000000"))
            End If
            If WGSD.Table_Entries(k).visibilityMask <> 4294967295 Then
                GoTo ignore_this
            End If
            'now we can get the strings from the keys.
            WGSD.Table_Entries(k).diffuseMap = find_str_BWST(WGSD.Table_Entries(k).diffuseMapKey)
            WGSD.Table_Entries(k).normalMap = find_str_BWST(WGSD.Table_Entries(k).normalMapKey)
            WGSD.Table_Entries(k).gmmMap = find_str_BWST(WGSD.Table_Entries(k).gmmMapkey)
            WGSD.Table_Entries(k).extraMap = find_str_BWST(WGSD.Table_Entries(k).extrakey)
            'this is a temp hack
            If WGSD.Table_Entries(k).extraMap <> "" Then
                WGSD.Table_Entries(k).diffuseMap = WGSD.Table_Entries(k).extraMap
                WGSD.Table_Entries(k).normalMap = WGSD.Table_Entries(k).extraMap
            End If
            decal_matrix_list(k).decal_texture = WGSD.Table_Entries(k).diffuseMap
            '' the normal map for Stone_06 does not exist in the pkg files!!
            If decal_matrix_list(k).decal_texture.Contains("Stone06.") Then
                WGSD.Table_Entries(k).normalMap = "Stone06_NM.dds"
            End If
            decal_matrix_list(k).decal_normal = WGSD.Table_Entries(k).normalMap
            decal_matrix_list(k).decal_gmm = WGSD.Table_Entries(k).gmmMap
ignore_this:
            decal_matrix_list(k).influence = WGSD.Table_Entries(k).flags 'CInt((WGSD.Table_Entries(k).flags And &HFF00) / 256)
            If decal_matrix_list(k).influence = 6 Then
                decal_matrix_list(k).influence = 2
            End If

            decal_matrix_list(k).priority = priority '(WGSD.Table_Entries(k).flags And &HFF)
            Dim d_type As Integer = (WGSD.Table_Entries(k).flags And &HF0000) / 65536
            'If d_type <> 1 Then
            '    Stop
            'End If
            'Debug.WriteLine("ID:" + k.ToString)
            'Debug.WriteLine(decal_matrix_list(k).decal_texture)
            'Debug.WriteLine(decal_matrix_list(k).influence.ToString)
            'Debug.WriteLine(CStr(WGSD.Table_Entries(k).flags And &HFF0000) / 65536)
        Next

        Dim cnt2 As UInt32

        ty = br.ReadUInt32
read3_only:
        cnt2 = br.ReadUInt32
        Dim dl = br.ReadUInt32
        ReDim Preserve decal_matrix_list(WGSD.entry_count + cnt2 + -1)
        ReDim Preserve WGSD.Table_Entries(WGSD.entry_count + cnt2 + -1)
        ReDim temp(WGSD.entry_count + cnt2 + -1)
        '2nd group
        For k = WGSD.entry_count To (WGSD.entry_count + cnt2) - 1
            'If k = 350 Then
            '    Stop
            'End If
            decal_matrix_list(k) = New decal_matrix_list_
            ReDim Preserve WGSD.Table_Entries(k).matrix(16)
            'we have 2 unknown int32s and a byte.. 
            WGSD.Table_Entries(k).v1 = br.ReadUInt32 'Unknown always 0?
            WGSD.Table_Entries(k).v2 = br.ReadUInt32 'Unknown always 0?
            WGSD.Table_Entries(k).accuracyType = br.ReadByte()
            If WGSD.Table_Entries(k).accuracyType <> 0 Then
                'Stop
            End If
            For i = 0 To 15
                WGSD.Table_Entries(k).matrix(i) = br.ReadSingle
            Next
            decal_matrix_list(k).matrix = WGSD.Table_Entries(k).matrix

            WGSD.Table_Entries(k).diffuseMapKey = br.ReadUInt32
            WGSD.Table_Entries(k).normalMapKey = br.ReadUInt32
            WGSD.Table_Entries(k).gmmMapkey = br.ReadUInt32
            WGSD.Table_Entries(k).extrakey = br.ReadUInt32

            If WGSD.Table_Entries(k).extrakey <> 2216829733 Then
                Debug.Write(k.ToString("00000") + " : " + WGSD.Table_Entries(k).extrakey.ToString)
                Debug.WriteLine(" : " + find_str_BWST(WGSD.Table_Entries(k).extrakey))
            End If
            Dim priority = br.ReadUInt32 'Unknown always 0?
            If priority > 0 Then
                'Stop
            End If
            WGSD.Table_Entries(k).flags = br.ReadUInt16
            decal_matrix_list(k).flags = WGSD.Table_Entries(k).flags
            'Debug.WriteLine(k.ToString("0000") + ":" + decal_matrix_list(k).flags.ToString("X"))
            WGSD.Table_Entries(k).off_x = br.ReadSingle
            WGSD.Table_Entries(k).off_y = br.ReadSingle
            WGSD.Table_Entries(k).off_z = br.ReadSingle
            WGSD.Table_Entries(k).off_w = br.ReadSingle

            decal_matrix_list(k).offset.z = WGSD.Table_Entries(k).off_x
            decal_matrix_list(k).offset.y = WGSD.Table_Entries(k).off_y
            decal_matrix_list(k).offset.z = WGSD.Table_Entries(k).off_z
            decal_matrix_list(k).offset.w = WGSD.Table_Entries(k).off_w

            If decal_matrix_list(k).offset.x <> 0 Then
                'Stop
            End If
            If decal_matrix_list(k).offset.y <> 0 Then
                'Stop
            End If
            If decal_matrix_list(k).offset.z <> 0 Then
                'Stop
            End If
            If decal_matrix_list(k).offset.w <> 0 Then
                'Stop
            End If

            WGSD.Table_Entries(k).uv_wrapping_u = br.ReadSingle
            WGSD.Table_Entries(k).uv_wrapping_v = br.ReadSingle
            decal_matrix_list(k).u_wrap = WGSD.Table_Entries(k).uv_wrapping_u
            decal_matrix_list(k).v_wrap = WGSD.Table_Entries(k).uv_wrapping_v

            WGSD.Table_Entries(k).visibilityMask = br.ReadUInt32 'always 0xFFFFFFFF?
            'Dim un = br.ReadUInt32
            If WGSD.Table_Entries(k).visibilityMask <> 4294967295 Then
                'Stop
                'Debug.WriteLine(k.ToString + ":" + WGSD.Table_Entries(k).visibilityMask.ToString("00000000000"))
            End If
            If WGSD.Table_Entries(k).visibilityMask <> 4294967295 Then
                GoTo ignore_this2
            End If
            WGSD.Table_Entries(k).tiles_fade = br.ReadSingle
            WGSD.Table_Entries(k).parallax_offset = br.ReadSingle
            WGSD.Table_Entries(k).parallax_amplitude = br.ReadSingle


            'now we can get the strings from the keys.
            WGSD.Table_Entries(k).diffuseMap = find_str_BWST(WGSD.Table_Entries(k).diffuseMapKey)
            WGSD.Table_Entries(k).normalMap = find_str_BWST(WGSD.Table_Entries(k).normalMapKey)
            WGSD.Table_Entries(k).gmmMap = find_str_BWST(WGSD.Table_Entries(k).gmmMapkey)
            WGSD.Table_Entries(k).extraMap = find_str_BWST(WGSD.Table_Entries(k).extrakey)
            'this is a temp hack
            If WGSD.Table_Entries(k).extraMap <> "" Then
                WGSD.Table_Entries(k).diffuseMap = WGSD.Table_Entries(k).extraMap
                WGSD.Table_Entries(k).normalMap = WGSD.Table_Entries(k).extraMap
            End If
            decal_matrix_list(k).decal_texture = WGSD.Table_Entries(k).diffuseMap
            '' the normal map for Stone_06 does not exist in the pkg files!!
            If decal_matrix_list(k).decal_texture.Contains("Stone06.") Then
                WGSD.Table_Entries(k).normalMap = "Stone06_NM.dds"
            End If
            decal_matrix_list(k).decal_normal = WGSD.Table_Entries(k).normalMap
            decal_matrix_list(k).decal_gmm = WGSD.Table_Entries(k).gmmMap
ignore_this2:
            decal_matrix_list(k).influence = WGSD.Table_Entries(k).flags 'CInt((WGSD.Table_Entries(k).flags And &HFF00) / 256)
            If decal_matrix_list(k).influence = 6 Then
                decal_matrix_list(k).influence = 2
            End If

            decal_matrix_list(k).priority = priority '(WGSD.Table_Entries(k).flags And &HFF)
            Dim d_type As Integer = (WGSD.Table_Entries(k).flags And &HF0000) / 65536
            'If d_type <> 1 Then
            '    Stop
            'End If
            'Debug.WriteLine("ID:" + k.ToString)
            'Debug.WriteLine(decal_matrix_list(k).decal_texture)
            'Debug.WriteLine(decal_matrix_list(k).influence.ToString)
            'Debug.WriteLine(CStr(WGSD.Table_Entries(k).flags And &HFF0000) / 65536)
        Next

        'have to sort these by priority.
        'Dim cnt As Integer = 0
        'Dim level As Integer = 0
        'For k = 0 To 5000
        '    For i = 0 To temp.Length - 1
        '        Dim t = decal_matrix_list(i)
        '        If t.priority > 5000 Then Stop ' catch out of bounds value
        '        If t.priority = level Then
        '            temp(cnt) = t
        '            cnt += 1
        '        End If
        '    Next
        '    level += 1
        'Next

        ''Console.WriteLine("------------------------------")

        'For i = 0 To temp.Length - 1
        '    Dim t = temp(i)
        '    decal_matrix_list(i) = t
        '    'Console.WriteLine("b1:" + decal_matrix_list(i).priority.ToString("00") + " id:" + i.ToString("0000"))
        'Next
        If False Then 'save some info for hacking.
            bw_strings.Clear()
            For i = 0 To decal_matrix_list.Length - 1
                bw_strings.Append(i.ToString("0000") + " : priority :" + decal_matrix_list(i).priority.ToString("x") _
                                  + ": V1 :" + WGSD.Table_Entries(i).v1.ToString("x") + ": V2 :" + WGSD.Table_Entries(i).v2.ToString("x") + vbCrLf)

                bw_strings.Append("Flags : " + decal_matrix_list(i).flags.ToString("x") + "ACC : " + WGSD.Table_Entries(i).accuracyType.ToString)
                bw_strings.Append(" : tex AM  : " + WGSD.Table_Entries(i).diffuseMap)
                bw_strings.Append(" : tex AN  : " + WGSD.Table_Entries(i).normalMap + vbCrLf)
                bw_strings.Append(" : tex GMM : " + WGSD.Table_Entries(i).gmmMap)
                bw_strings.Append(" : tex Etr : " + WGSD.Table_Entries(i).extraMap + vbCrLf + vbCrLf)

            Next
            File.WriteAllText("C:\!_bin_data\decals.txt", bw_strings.ToString)
            bw_strings.Clear()
        End If
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
        BWST.d_length = br.ReadUInt32 ' length of data
        BWST.entry_count = br.ReadUInt32 ' number of entries
        '
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
            Dim hexCharArray = BitConverter.GetBytes(BWST.entries(k).key)
            'Array.Reverse(hexCharArray)
            Dim hexStringReversed = BitConverter.ToString(hexCharArray)

            bw_strings.AppendLine(hexStringReversed.Replace("-", "") + " : " + BWST.entries(k).str)
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
                SpTr.Stree(k).Matrix(i) = speedtree_matrix_list(k).matrix(i)

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

    Public Sub get_BGDE_data(ByVal t_cnt As Integer)
        'speed tree data
        Dim ms As New MemoryStream(space_table_rows(t_cnt).data)
        Dim br As New BinaryReader(ms)
        ms.Position = 0
        '============================================
        BGDE.t1_pos = ms.Position
        BGDE.t1_dl = br.ReadUInt32 ' lenght of data
        BGDE.t1_dc = br.ReadUInt32 ' number of entries
        ReDim BGDE.BGDE_t1(BGDE.t1_dc) ' resize
        For k = 0 To BGDE.t1_dc - 1
            BGDE.BGDE_t1(k).int1 = br.ReadUInt32
            BGDE.BGDE_t1(k).int2 = br.ReadUInt32
            BGDE.BGDE_t1(k).int3 = br.ReadUInt32
        Next
        '============================================
        BGDE.t2_pos = ms.Position
        BGDE.t2_dl = br.ReadUInt32 ' lenght of data
        BGDE.t2_dc = br.ReadUInt32 ' number of entries
        ReDim BGDE.BGDE_t2(BGDE.t2_dc) ' resize
        For k = 0 To BGDE.t2_dc - 1
            BGDE.BGDE_t2(k).int1 = br.ReadUInt32
            BGDE.BGDE_t2(k).int2 = br.ReadUInt32
        Next
        '============================================
        BGDE.t3_pos = ms.Position
        BGDE.t3_dl = br.ReadUInt32 ' lenght of data
        BGDE.t3_dc = br.ReadUInt32 ' number of entries
        ReDim BGDE.BGDE_t3(BGDE.t3_dc) ' resize
        For k = 0 To BGDE.t3_dc - 1
            BGDE.BGDE_t3(k).int1 = br.ReadUInt32
        Next
        'Stop
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
