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
            ' save data to drive (only for hacking the files)
            'If True Then
            '    File.WriteAllBytes("C:\!_bin_data\" + space_table_rows(t_cnt).header + ".bin", _
            '                        space_table_rows(t_cnt).data)
            'End If
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
            End Select
        Next

        ReDim spaceBindata(0)
        GC.Collect()
        GC.WaitForFullGCComplete()
        'The order these tables are created is very important as some rely on previous tables data!
        For t_cnt = 0 To table_size - 1
            Dim header As String = space_table_rows(t_cnt).header
            Select Case True
                Case header = "BWST"
                    get_BWST_data(t_cnt)
                Case header = "BWAL"
                Case header = "BWCS"
                Case header = "BWSG"
                    get_BWSG_data(t_cnt)
                Case header = "BWT2"
                    'get_BWT2_data(t_cnt)
                Case header = "BWSS"
                Case header = "BSMI"
                    get_BSMI_data(t_cnt)
                Case header = "WSMI"
                    get_WSMI_data(t_cnt)
                Case header = "BSMO"
                    get_BSMO_data(t_cnt)
                Case header = "WSMO"
                Case header = "BSMA"
                    get_BSMA_data(t_cnt)
                Case header = "SpTr"
                    get_SpTr_data(t_cnt)
                Case header = "BWfr"
                Case header = "WGSD"
                    get_WGSD_data(t_cnt)
                Case header = "WTCP"
                Case header = "BWWa"
                    get_BWWa_data(t_cnt)
                Case header = "BWSV"
                Case header = "BWPs"
                Case header = "CENT"
                Case header = "UDOS"
                Case header = "WGDE"
                Case header = "WGDN"
                Case header = "BWLC"
                Case header = "WTau"

            End Select

        Next
        ' save data to drive (only for debuging)
        If False Then
            File.WriteAllText("C:\!_bin_data\" + "BW_Strings.txt", _
                                bw_strings.ToString)
        End If
        bw_strings.Clear()
        ReDim Model_Matrix_list(BSMI.t3_dc)
        For k = 0 To BSMI.t3_dc - 1
            Dim BSMO_Index = BSMI.bsmi_t3(k).BSMO_Index
            Dim m = BSMO.bsmo_t2(BSMO_Index).model_str
            Model_Matrix_list(k) = New model_matrix_list_
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
            'Console.WriteLine(k.ToString("000") + " : " + m)
        Next

        mr.Dispose()
        br.Close()
        'clean up some space
        bw_strings.Clear()

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
    Private Sub get_translated_bb_model(ByRef mm As model_matrix_list_)
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
