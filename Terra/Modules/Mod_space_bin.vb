Imports System.IO
Imports System.Windows.Forms
Imports System.Math
Imports System.String
Imports System.Text


Module Mod_space_bin
    Public space_table_rows() As space_tables_
    Public Structure space_tables_
        Public header As String
        Public type As Int32
        Public section_Start As Long
        Public section_length As Long
        Public data() As Byte
    End Structure
    Public Sub get_spaceBin_data(ByVal spaceBindata() As Byte)
        bw_strings.Clear()
        Dim mr As New MemoryStream(spaceBindata)
        Dim br As New BinaryReader(mr)
        mr.Position = &H14
        Dim table_size = br.ReadInt32
        ReDim space_table_rows(table_size - 1)

        mr.Position = &H18 ' point at next row in the table
        'create the table index information.
        Dim old_pos = br.BaseStream.Position
        For t_cnt = 0 To table_size - 1
            br.BaseStream.Position = old_pos
            Dim ds() = br.ReadBytes(4)
            space_table_rows(t_cnt).header = System.Text.Encoding.UTF8.GetString(ds, 0, 4)

            space_table_rows(t_cnt).type = br.ReadInt32
            space_table_rows(t_cnt).section_Start = br.ReadInt64
            space_table_rows(t_cnt).section_length = br.ReadInt64
            old_pos = br.BaseStream.Position
            br.BaseStream.Position = space_table_rows(t_cnt).section_Start
            space_table_rows(t_cnt).data = br.ReadBytes(space_table_rows(t_cnt).section_length)
            Debug.WriteLine(space_table_rows(t_cnt).header)
            ' save data to drive (only for hacking the files)
            If True Then
                File.WriteAllBytes("C:\!_bin_data\" + space_table_rows(t_cnt).header + ".bin", _
                                    space_table_rows(t_cnt).data)
            End If
        Next
        BWST = New BWST_
        BWSG = New BWSG_
        BSGD = New BSGD_
        BSMI = New BSMI_
        BSMO = New BSMO_
        BSMA = New BSMA_
        BWWa = New BWWa_
        SpTr = New SpTr_
        'get the BSGD data first! this must be ready to use below for BWSG
        For t_cnt = 0 To table_size - 1
            Dim header As String = space_table_rows(t_cnt).header
            Select Case True
                Case header = "BSGD"
                    get_BSGD_data(t_cnt)
                    Exit For
            End Select
        Next

        ReDim spaceBindata(0)
        GC.Collect()
        GC.WaitForFullGCComplete()
        'The order these tables are created is very important as some rely on previous tables data!
        'get string table first!
        For t_cnt = 0 To table_size - 1
            Dim header As String = space_table_rows(t_cnt).header
            Select Case True
                Case header = "BWST"
                    get_BWST_data(t_cnt)
            End Select
        Next

        For t_cnt = 0 To table_size - 1
            Dim header As String = space_table_rows(t_cnt).header
            Select Case True
                Case header = "BWCS"
                Case header = "BWSG"
                    get_BWSG_data(t_cnt)
                Case header = "BWT2"
                    'get_BWT2_data(t_cnt)
                Case header = "BWSS"
                Case header = "BSMI" 'Matrix data
                    get_BSMI_data(t_cnt)
                Case header = "WTbl"
                    get_WTbl_data(t_cnt)
                Case header = "BSMO" 'models
                    get_BSMO_data(t_cnt)
                Case header = "WSMO"
                Case header = "BSMA" 'materials
                    get_BSMA_data(t_cnt)
                Case header = "SpTr" ' speed tree
                    get_SpTr_data(t_cnt)
                Case header = "BWfr"
                Case header = "WGSD" 'decals
                    get_WGSD_data(t_cnt)
                Case header = "WTCP"
                Case header = "BWWa" 'water
                    get_BWWa_data(t_cnt)
                Case header = "BWSV"
                Case header = "BWPs"
                Case header = "CENT"
                Case header = "UDOS"
                Case header = "WGDE"
                    get_BGDE_data(t_cnt) '?
                Case header = "WGDN"
                Case header = "BWLC"
                Case header = "WTau"

            End Select

        Next
        '===========================================================================
        ' save data to drive (only for debuging)
        If True Then
            File.WriteAllText("C:\!_bin_data\" + "BW_Strings.txt", _
                                bw_strings.ToString)
        End If
        bw_strings.Clear()
        '===========================================================================
        ReDim Model_Matrix_list(BSMI.t3_dc)
        Dim cnt As Integer = 0

        For k = 0 To BSMI.t3_dc - 1

            Model_Matrix_list(k) = New model_matrix_list_
            'Model_Matrix_list(k).exclude = False

            Dim BSMO_Index = BSMI.bsmi_t3(k).BSMO_Index
            Dim m = BSMO.bsmo_t2(BSMO_Index).model_str
            If m.ToLower.Contains("building_wall1") Then
                'Stop
            End If

            Model_Matrix_list(k).primitive_name = m.Replace("primitives", "model")

            Model_Matrix_list(k).matrix = BSMI.bsmi_t1(k).matrix
            Model_Matrix_list(k).matrix(1) *= -1.0
            Model_Matrix_list(k).matrix(2) *= -1.0
            Model_Matrix_list(k).matrix(4) *= -1.0
            Model_Matrix_list(k).matrix(8) *= -1.0
            Model_Matrix_list(k).matrix(12) *= -1.0

            Model_Matrix_list(k).mask = False
            Model_Matrix_list(k).BB_Min = BSMO.bsmo_t4(BSMO_Index).min_BB
            Model_Matrix_list(k).BB_Max = BSMO.bsmo_t4(BSMO_Index).max_BB
            ReDim Model_Matrix_list(k).BB(8)
            get_translated_bb_model(Model_Matrix_list(k))
            bw_strings.Append(k.ToString("0000") + " : " + m + vbCrLf)
            If BSMI.bsmi_t6(k).u1_index <> 4294967295 Then
                cnt += 1
                'Debug.WriteLine(k.ToString("00000") + " : " + m)
                'Debug.WriteLine("1 " + BSMI.bsmi_t6(k).u1_index.ToString("x"))
            End If
            If BSMI.bsmi_t6(k).u2_index <> 4294967295 Then
                'Debug.WriteLine("2 " + ":" + BSMI.bsmi_t6(k).u2_index.ToString("x"))
            End If
            If BSMI.bsmi_t6(k).u3_index <> 4294967295 Then
                'Debug.WriteLine("3 " + ":" + BSMI.bsmi_t6(k).u3_index.ToString("x"))
            End If
            '===========================================================================
            'get non rendered primitive groups
            Dim mask_pnt = BSMO.bsmo_t1(BSMO_Index).mask_pointer
            Dim exclude_list_pnt As UInt32 = BSMO.bsmo_t1(BSMO_Index).exclude_list
            Dim exclude_start As UInt32 = BSMO.bsmo_t6(exclude_list_pnt).exclude_start
            Dim exclude_end As UInt32 = BSMO.bsmo_t6(exclude_list_pnt).exclude_end
            Dim x_size As Int32 = exclude_end - exclude_start
            If x_size > 0 Then

                ReDim Model_Matrix_list(k).exclude_list(x_size)
                For j As UInt32 = exclude_start To exclude_end
                    Model_Matrix_list(k).exclude_list(j - exclude_start) = BSMO.bsmo_t7(j).group_index
                Next
            End If
            '===========================================================================

        Next
        Debug.WriteLine(cnt.ToString("00000"))
        '===========================================================================
        If True Then
            File.WriteAllText("C:\!_bin_data\" + "model_Strings.txt", _
                                bw_strings.ToString)
        End If
        bw_strings.Clear()
        '===========================================================================

        'lets compress the model_matrix_list to only used models
        Debug.WriteLine("Building Model_Matrix_list")
        Dim mc As Int32 = 0
        Dim HQ As Integer = 1
        Dim tm(BSMI.bsmi_t2.Length) As model_matrix_list_
        For i = 0 To BSMI.bsmi_t7.Length - 2
            If BSMI.bsmi_t3(i).BSMO_Index2 = HQ Then
                If BSMI.bsmi_t7(i).v_mask = &HFFFFFFFF Then 'visibility mask
                    tm(mc) = New model_matrix_list_
                    tm(mc) = Model_Matrix_list(i)
                    mc += 1
                Else
                    Debug.WriteLine(i.ToString("00000") + ":" + BSMI.bsmi_t7(i).v_mask.ToString("x") + ":" + Path.GetFileNameWithoutExtension(Model_Matrix_list(i).primitive_name))
                End If
            End If
        Next
        ReDim Preserve tm(mc)
        ReDim Model_Matrix_list(mc)
        'pack the Model_Matrix_list to used models.
        For i = 0 To mc
            Model_Matrix_list(i) = tm(i)
            If Model_Matrix_list(i).exclude = True Then
                Debug.WriteLine(i.ToString("0000") + " : " + Model_Matrix_list(i).primitive_name)
            End If

        Next
        mr.Dispose()
        br.Close()
        'clean up some space
        bw_strings.Clear()
        tm = Nothing
        BWST = Nothing
        BWSG = Nothing
        BSGD = Nothing
        BSMI = Nothing
        BSMO = Nothing
        BSMA = Nothing
        SpTr = Nothing
        GC.Collect()
        GC.WaitForFullGCComplete()

    End Sub
    Public Sub get_translated_bb_model(ByRef mm As model_matrix_list_)
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
            mm.BB(i) = translate_to(mm.BB(i), mm.matrix)
        Next

    End Sub
End Module
