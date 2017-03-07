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
            Model_Matrix_list(k).mask = False
            'Console.WriteLine(k.ToString("000") + " : " + m)
        Next
        If File.Exists(Application.StartupPath + "\decal_includer_files\" + JUST_MAP_NAME + "_decal_includers.txt") Then
            decal_includers_string = File.ReadAllText(Application.StartupPath + "\decal_includer_files\" + JUST_MAP_NAME + "_decal_includers.txt")
        Else
            File.Create(Application.StartupPath + "\decal_includer_files\" + JUST_MAP_NAME + "_decal_includers.txt")
            decal_includers_string = ""
        End If

        check_decal_include_strings()
        get_decal_bias_settings()

        mr.Dispose()
        br.Close()
        'clean up some space
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
    Public Sub check_decal_include_strings()
        decal_includers_string = decal_includers_string.Replace(vbCrLf + vbCrLf, vbCrLf)
        For k = 0 To Model_Matrix_list.Length - 2
            If decal_includers_string.Contains("+" + k.ToString) Then
                Model_Matrix_list(k).mask = True
            Else
                Model_Matrix_list(k).mask = False
            End If
        Next
    End Sub
    Public Sub get_decal_bias_settings()
        decal_includers_string = decal_includers_string.Replace(vbCrLf + vbCrLf, vbCrLf)
        Dim ar = decal_includers_string.Split(vbCrLf)
        Dim cnt As Integer = 0
        For Each item In ar
            If item.Contains(":") Then
                Dim a = item.Split(":")
                cnt = -CInt(a(0))
                Dim val As Single = CSng(a(1))
                decal_matrix_list(cnt).t_bias = CSng(a(1))
                decal_matrix_list(cnt).d_bias = CSng(a(2))
                decal_matrix_list(cnt).exclude = CBool(a(3))
            End If

        Next
    End Sub
End Module
