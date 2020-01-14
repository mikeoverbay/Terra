Imports System.Text


Module Mod_space_bin_variables

    Public bw_strings As New StringBuilder

    Public table_Rows() As space_tables_
    Public Structure space_tables_
        Public header As String
        Public type As Int32
        Public section_Start As Long
        Public section_length As Long
        Public data() As Byte
    End Structure

    Public visual_sections() As visual_sections_

    Public Structure visual_sections_
        Public shader As String
        Public model As String
        Public entries() As visual_entries_
    End Structure
    Public Structure visual_entries_
        Public Property_name As String
        Public Property_value As Object
    End Structure
    '============================================================================================
    Public BSMA_PropertyType() = {" ", "Bool", "Float", "Int", "?", "Vector4", "String", "?"}


    '============================================================================================
    Public SpTr As SpTr_
    Public Structure SpTr_
        Public d_length As UInt32
        Public entry_count As UInt32
        Public Stree() As Stree_
    End Structure
    Public Structure Stree_
        Public key As UInt32
        Public e1 As UInt32
        Public e2 As UInt32
        Public e3 As UInt32 ' no idea what the e1-e3 are for yet
        Public Tree_name As String
        Public Matrix() As Single
    End Structure
    '============================================================================================

    Public BWWa As BWWa_
    Public Structure BWWa_
        Public t1_start As UInt32
        Public t1_dl As UInt32
        Public t1_dc As UInt32

        Public t2_start As UInt32
        Public t2_dl As UInt32
        Public t2_dc As UInt32

        Public bwwa_t1() As bwwa_t1_
        Public bwwa_t2() As bwwa_t2_

    End Structure
    Public Structure bwwa_t1_
        Public position As vect3
        Public width As Single
        Public height As Single
        Public plane As Single
        Public orientation As Single

    End Structure
    Public Structure bwwa_t2_

    End Structure
    '============================================================================================

    Public WSMI As WSMI_
    Public Structure WSMI_
        Public t1_start As UInt32
        Public t1_dl As UInt32
        Public t1_dc As UInt32

        Public t2_start As UInt32
        Public t2_dl As UInt32
        Public t2_dc As UInt32

        Public wsmi_t1() As wsmi_t1_
        Public wsmi_t2() As wsmi_t2_
    End Structure
    Public Structure wsmi_t1_
        Public flag1 As UInt32
        Public flag2 As UInt32
        Public flag3 As UInt32
    End Structure
    Public Structure wsmi_t2_
        Public flag1 As UInt32

    End Structure
    '============================================================================================

    Public BSMI As BSMI_
    Public Structure BSMI_
        Public t1_start As UInt32
        Public t1_dl As UInt32
        Public t1_dc As UInt32

        Public t2_start As UInt32
        Public t2_dl As UInt32
        Public t2_dc As UInt32

        Public t3_start As UInt32
        Public t3_dl As UInt32
        Public t3_dc As UInt32

        Public t4_start As UInt32
        Public t4_dl As UInt32
        Public t4_dc As UInt32

        Public t5_start As UInt32
        Public t5_dl As UInt32
        Public t5_dc As UInt32

        Public t6_start As UInt32
        Public t6_dl As UInt32
        Public t6_dc As UInt32

        Public t7_start As UInt32
        Public t7_dl As UInt32
        Public t7_dc As UInt32

        Public t8_start As UInt32
        Public t8_dl As UInt32
        Public t8_dc As UInt32

        Public t9_start As UInt32
        Public t9_dl As UInt32
        Public t9_dc As UInt32
        Public anim_list_start As Long
        Public anima_cnt As UInt32
        Public anima_dlen As UInt32
        Public anima_list() As UInt32
        Public bsmi_t1() As bsmi_t1_
        Public bsmi_t2() As bsmi_t2_
        Public bsmi_t3() As bsmi_t3_
        Public bsmi_t4() As bsmi_t4_
        Public bsmi_t5() As bsmi_t5_
        Public bsmi_t6() As bsmi_t6_
        Public bsmi_t7() As bsmi_t7_
        Public bsmi_t8() As bsmi_t8_
        Public bsmi_t9() As bsmi_t9_
    End Structure

    Public Structure bsmi_t1_
        Public matrix() As Single
    End Structure
    Public Structure bsmi_t2_
        Public u1_Index As UInt32
        Public u2_Index As UInt32
    End Structure
    Public Structure bsmi_t3_
        Public BSMO_Index As UInt32
        Public BSMO_Index2 As UInt32
    End Structure
    Public Structure bsmi_t4_
        Public u1_index As UInt32
        Public u2_index As UInt32
        Public str_key As UInt32
        Public flags As UInt32
        Public mask As UInt32
        Public factor As Single
        Public s1 As Single
        Public s2 As Single
    End Structure
    Public Structure vMask_
        Public vMask As UInt32
    End Structure
    Public Structure bsmi_t5_
        Public u1_index As UInt32
    End Structure
    Public Structure bsmi_t6_
        Public u1_index As UInt32
        Public u2_index As UInt32
        Public u3_index As UInt32
        Public u4_index As UInt32
    End Structure
    Public Structure bsmi_t7_
        Public v_mask As Int32
    End Structure
    Public Structure bsmi_t8_
        Public v As UInt32
    End Structure
    Public Structure bsmi_t9_
        Public s1 As Single
        Public s2 As Single
        Public s3 As Single
        Public s4 As Single
        Public s5 As Single
    End Structure

    '============================================================================================
    Public BSMA As BSMA_
    Public Structure BSMA_
        Public t1_start As UInt32
        Public t1_dl As UInt32
        Public t1_dc As UInt32

        Public t2_start As UInt32
        Public t2_dl As UInt32
        Public t2_dc As UInt32

        Public t3_start As UInt32
        Public t3_dl As UInt32
        Public t3_dc As UInt32

        Public shaders() As String
        Public bsma_t1() As bsma_t1_
        Public bsma_t2() As bsma_t2_
        Public bsma_t3() As bsma_t3_

    End Structure
    Public Structure bsma_t1_
        Public fx_index As UInt32
        Public index_start As UInt64
        Public index_end As UInt64
        Public bwst_key As UInt32
        Public identifier As String
    End Structure
    Public Structure bsma_t2_
        Public bwst_key_or_value As UInt32
        Public property_type As UInt32
        Public bwst_key As UInt32
        Public property_string As String
        Public val_boolean As Boolean
        Public val_float As Single
        Public val_int As Integer
        Public val_vec4 As vect4
        Public texture_string As String
    End Structure
    Public Structure bsma_t3_
        Public float_val As Single
    End Structure
    Public Structure bsma_t4_
        Public matrix() As Single
    End Structure
    Public Structure bsma_t5_
        Public vector4 As vect4
    End Structure

    '============================================================================================

    Public BSMO As BSMO_
    Public Structure BSMO_
        Public t1_start As UInt32
        Public t1_dl As UInt32
        Public t1_dc As UInt32

        Public t2_start As UInt32
        Public t2_dl As UInt32
        Public t2_dc As UInt32

        Public t3_start As UInt32
        Public t3_dl As UInt32
        Public t3_dc As UInt32

        Public t4_start As UInt32
        Public t4_dl As UInt32
        Public t4_dc As UInt32

        Public t5_start As UInt32
        Public t5_dl As UInt32
        Public t5_dc As UInt32

        Public t6_start As UInt32
        Public t6_dl As UInt32
        Public t6_dc As UInt32

        Public t7_start As UInt32
        Public t7_dl As UInt32
        Public t7_dc As UInt32

        Public t8_start As UInt32
        Public t8_dl As UInt32
        Public t8_dc As UInt32

        Public t9_start As UInt32
        Public t9_dl As UInt32
        Public t9_dc As UInt32

        Public t10_start As UInt32
        Public t10_dl As UInt32
        Public t10_dc As UInt32

        Public t11_start As UInt32
        Public t11_dl As UInt32
        Public t11_dc As UInt32

        Public bsmo_t1() As bsmo_t1_
        Public bsmo_t2() As bsmo_t2_
        Public bsmo_t3() As bsmo_t3_
        Public bsmo_t4() As bsmo_t4_
        Public bsmo_t5() As bsmo_t5_
        Public bsmo_t6() As bsmo_t6_
        Public bsmo_t7() As bsmo_t7_
        Public bsmo_t8() As bsmo_t8_
        Public bsmo_t9() As bsmo_t9_
        Public bsmo_t10() As bsmo_t10_
        Public bsmo_t11() As bsmo_t11_
    End Structure
    Public Structure model_nodes_
        Public node_count As UInt32
        Public key As UInt32
        Public nodes() As bsmo_t2_
    End Structure
    Public Structure bsmo_t1_
        'points in to table 6?
        Public mask_pointer As UInt32
        Public exclude_list As UInt32
    End Structure

    Public Structure bsmo_t2_
        Public min_BB As vect3
        Public max_BB As vect3
        Public BWST_String_key As UInt32
        Public index_from As UInt32
        Public index_to As UInt32
        Public model_str As String
    End Structure
    Public Structure bsmo_t3_
        Public index As UInt32
        Public offset As UInt32
    End Structure
    Public Structure bsmo_t4_
        Public min_BB As vect3
        Public max_BB As vect3
    End Structure
    Public Structure bsmo_t5_
        Public mask As UInt32
    End Structure
    Public Structure bsmo_t6_
        Public exclude_start As UInt32
        Public exclude_end As UInt32
    End Structure
    Public Structure bsmo_t7_
        Public u1_uint32 As UInt32
        Public u2_uint32 As UInt32
        Public material_index As UInt32
        Public group_index As UInt32
        Public vert_key As UInt32
        Public indi_key As UInt32
        Public u3_int32 As UInt32
        Public vert_string As String
        Public indi_string As String
    End Structure
    Public Structure bsmo_t8_
        Public v As UInt32
    End Structure
    Public Structure bsmo_t9_
        Public v As UInt32
    End Structure
    Public Structure bsmo_t10_
        Public int1 As UInt32
        Public int2 As UInt32
    End Structure
    Public Structure bsmo_t11_
        Public int1 As UInt32

    End Structure
    '============================================================================================

    Public BWST As BWST_
    Public Structure BWST_
        Public d_length As UInt32
        Public entry_count As UInt32
        Public total_length As UInt32
        Public entries() As str_entry_
    End Structure
    Public Structure str_entry_
        Public offset As UInt32
        Public str_length As UInt32
        Public key As UInt32
        Public str As String
    End Structure
    '============================================================================================

    Public BWT2 As BWT2_
    Public Structure BWT2_
        Public map_size As UInt32
        Public grid_Meter_size As Single
        Public t_1_start As UInt32
        Public t_1_d_Length As UInt32
        Public t_1_entry_count As UInt32
        '
        Public t_2_start As UInt32
        Public t_2_d_Length As UInt32
        Public t_2_entry_count As UInt32
        '
        Public t_3_start As UInt32
        Public t_3_d_Length As UInt32
        Public t_3_entry_count As UInt32
        '
        Public t_4_start As UInt32
        Public t_4_d_Length As UInt32
        Public t_4_entry_count As UInt32

        Public location_table_1() As location_table_
        Public location_table_3() As location_table3_
        Public index_list_2() As index_List_2_
        Public grid_info_4() As grid_info_
        Public sorted_table() As sorted_table_
    End Structure
    Public Structure location_table_
        Public key As UInt32
        Public location As UInt32
        Public loc_str As String
        Public cdata_str As String
    End Structure
    Public Structure location_table3_
        Public LX As Single
        Public min As Single
        Public LY As Single
        Public UX As Single
        Public max As Single
        Public UY As Single
    End Structure
    Public Structure sorted_table_
        Public LX As Single
        Public min As Single
        Public LY As Single
        Public UX As Single
        Public max As Single
        Public UY As Single
    End Structure
    Public Structure index_List_2_
        Public index As UInt32
    End Structure
    Public Structure grid_info_

    End Structure
    '============================================================================================


    Public WGSD As WGSD_
    Public Structure WGSD_
        Public d_length As UInt32
        Public entry_count As UInt32
        Public Table_Entries() As WGSD_entries_
    End Structure
    Public Structure WGSD_entries_
        Public v1, v2 As UInt32
        Public accuracyType As Byte
        Public matrix() As Single
        Public diffuseMapKey As UInt32
        Public normalMapKey As UInt32
        Public gmmMapkey As UInt32
        Public extrakey As UInt32
        '
        Public flags As UInt16
        '
        Public off_x As Single
        Public off_y As Single
        Public off_z As Single
        Public off_w As Single
        '
        Public uv_wrapping_u As Single
        Public uv_wrapping_v As Single
        Public visibilityMask As UInt32

        Public tiles_fade As Single
        Public parallax_offset As Single
        Public parallax_amplitude As Single
        '---------------------------
        Public diffuseMap As String
        Public normalMap As String
        Public gmmMap As String
        Public extraMap As String
        '---------------------------
        Public s1, s2, s3 As String
    End Structure
    '============================================================================================

    Public BWSG As BWSG_
    Public Structure BWSG_
        Public str_entries() As str_entry_
        Public table_0_d_length As UInt32
        Public table_0_entry_count As UInt32

        Public table_1_start As UInt32
        Public table_1_d_length As UInt32
        Public table_1_entry_count As UInt32
        Public Table_1_entries() As tbl_1_entries_

        Public table_2_start As UInt32
        Public table_2_d_length As UInt32
        Public table_2_entry_count As UInt32
        Public table_2_entries() As tbl_2_entries_

        Public table_3_start As UInt32
        Public table_3_d_length As UInt32
        Public table_3_entry_count As UInt32
        Public table_3_entries() As tbl_3_entries_
        Public table_4_start As UInt32
        Public data_chunks() As data_chunk_
    End Structure
    Public Structure tbl_1_entries_
        Public key1 As UInt32
        Public start_index As UInt32
        Public end_index As UInt32
        Public vertex_count As UInt32
        Public key2 As UInt32
        Public vertex_type As String
        Public model As String
    End Structure
    Public Structure tbl_2_entries_
        'If block type = 0 than its a vertices block
        'if block type = &hA than its a UV2 block
        Public block_type As UInt32
        Public vertex_stride As UInt32
        Public data_length As UInt32
        Public section_index As UInt32
        Public offset As UInt32
        Public data() As Byte
    End Structure
    Public Structure tbl_3_entries_
        Public data_length As UInt32
    End Structure
    Public Structure data_chunk_
        Public data() As Byte
    End Structure
    Public item_list() As c_items_
    Public Structure c_items_
        Public start As Int32
        Public length As Int32
        Public data_start As Int32
        Public item_name As String
        Public data() As Int32
        Public data_length As Integer
    End Structure
    Public BSGD As BSGD_
    Public Structure BSGD_
        Public data() As Byte
    End Structure
    '============================================================================================

    Public Structure BGDE_t1_
        Public int1 As UInt32 '?
        Public int2 As UInt32
        Public int3 As UInt32
    End Structure
    Public Structure BGDE_t2_
        Public int1 As UInt32 '?
        Public int2 As UInt32
    End Structure
    Public Structure BGDE_t3_
        Public int1 As UInt32 '?
    End Structure
    Public Structure BGDE_
        Public t1_pos As Long
        Public t1_dc As UInt32
        Public t1_dl As UInt32

        Public t2_pos As Long
        Public t2_dc As UInt32
        Public t2_dl As UInt32

        Public t3_pos As Long
        Public t3_dc As UInt32
        Public t3_dl As UInt32
        Public BGDE_t1() As BGDE_t1_
        Public BGDE_t2() As BGDE_t2_
        Public BGDE_t3() As BGDE_t3_
    End Structure
    Public BGDE As BGDE_
    ' Public Structure vect3
    '    Public X, Y, Z As Single
    'End Structure
    'Public Structure vect4
    '    Public X, Y, Z, W As Single
    'End Structure

End Module
