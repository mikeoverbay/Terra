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

Imports Ionic.Zip
Imports Ionic.BZip2
Imports Ionic
'
#End Region

Module modRoadDecals
    Public road_decal_count As Integer
    Public road_decals() As roads_
    Public Structure roads_
        Public textures() As road_texture_list_
        Public road_decal_list() As road_decal_list_
    End Structure
    Public road_texture_list() As road_texture_list_
    Public Structure road_texture_list_
        Public texture_name As String
        Public textureID As Integer
    End Structure

    Public road_decal_list() As decal_matrix_list_
    Public Structure road_decal_list_
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
        Public decal_extra As String
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
        Public is_parallax As Boolean
        Public is_wet As Boolean
    End Structure

#Region "ROAD"
    'Decal entries
    Public cROAD As cROAD_
    Public Structure cROAD_
        Public decalEntries() As DecalEntries_
        Public Structure DecalEntries_
            Public v1, v2 As UInt32
            Public accuracyType As Byte
            Public matrix() As Single
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

            'these 3 only exist in type 3 decals
            Public tiles_fade As Single
            Public parallax_offset As Single
            Public parallax_amplitude As Single
            '---------------------------
            Public s1, s2, s3 As String
        End Structure
        Public Structure textures_
            Public diffuseMap As String
            Public normalMap As String
            Public gmmMap As String

        End Structure
    End Structure
#End Region


    Public Road_decal_sections() As rm_data_
    Public Structure rm_data_
        Public data() As Byte
    End Structure

    Public Sub split_road_map(z As Ionic.Zip.ZipEntry)
        road_decal_count = 0
        Dim ms As New MemoryStream
        z.Extract(ms)
        ms.Position = 0&
        Dim br As New BinaryReader(ms)
        Dim zip_cnt = br.ReadUInt32
        ReDim Road_decal_sections(zip_cnt - 1)
        For k = 0 To zip_cnt - 1
            br = New BinaryReader(ms)
            Dim b_size = br.ReadUInt32

            Road_decal_sections(k).data = br.ReadBytes(b_size)

            Dim buff(1065536) As Byte

            Dim total_read As Integer = 0
            Dim csm As New MemoryStream(Road_decal_sections(k).data)
            Dim csbr As New BinaryReader(csm)
            Dim mg1 = csbr.ReadInt32
            Dim mg2 = csbr.ReadInt32
            Dim uncompressedsize = csbr.ReadInt32

            Using Decompress As Zlib.ZlibStream = New Zlib.ZlibStream(csm, Zlib.CompressionMode.Decompress, False)
                Decompress.BufferSize = 1065536
                total_read = Decompress.Read(buff, 0, buff.Length)

            End Using
            ReDim Preserve buff(uncompressedsize)
            ReDim Road_decal_sections(k).data(total_read)
            buff.CopyTo(Road_decal_sections(k).data, 0)
        Next
        If False Then
            For k = 0 To zip_cnt - 1
                File.WriteAllBytes("c:\!_bin_data\road_map_" + k.ToString + ".bin", Road_decal_sections(k).data)
            Next
        End If
        get_road_map_decals()
    End Sub

    Public Sub get_road_map_decals()
        If Not m_decals_ Then ' our disable loading boolean
            Return
        End If
        FrmInfoWindow.tb1.Text = "Getting Road Decals..."
        Application.DoEvents()
        Dim ms As MemoryStream = Nothing
        ReDim road_decals(Road_decal_sections.Length - 1)

        For k = 0 To Road_decal_sections.Length - 1
            FrmInfoWindow.tb1.Text = "Getting Road Decals(" + k.ToString("000") + ")"
            Application.DoEvents()
            road_decals(k) = New roads_
            ReDim road_decals(k).textures(3)
            road_decals(k).textures(0) = New road_texture_list_
            road_decals(k).textures(1) = New road_texture_list_
            road_decals(k).textures(2) = New road_texture_list_
            ms = New MemoryStream(Road_decal_sections(k).data)
            Dim br As New BinaryReader(ms)
            ReDim road_texture_list(3)
            Dim d = br.ReadBytes(256)
            Dim dd(256) As Byte
            For i = 0 To 256
                If d(i) = 0 Then
                    ReDim Preserve dd(i - 1)
                    Exit For
                Else
                    dd(i) = d(i)
                End If
            Next
            road_decals(k).textures(0).texture_name = Encoding.UTF8.GetString(dd, 0, dd.Length)
            d = br.ReadBytes(256)
            ReDim dd(256)
            For i = 0 To 256
                If d(i) = 0 Then
                    ReDim Preserve dd(i - 1)
                    Exit For
                Else
                    dd(i) = d(i)
                End If
            Next

            road_decals(k).textures(1).texture_name = Encoding.UTF8.GetString(dd, 0, dd.Length)
            d = br.ReadBytes(256)
            ReDim dd(256)
            For i = 0 To 256
                If d(i) = 0 Then
                    ReDim Preserve dd(i - 1)
                    Exit For
                Else
                    dd(i) = d(i)
                End If
            Next
            road_decals(k).textures(2).texture_name = Encoding.UTF8.GetString(dd, 0, dd.Length)

            '4 unknown uint32
            Dim u1 = br.ReadUInt32
            Dim u2 = br.ReadUInt32
            Dim u3 = br.ReadUInt32
            Dim u4 = br.ReadUInt32

            Dim ffffffff = br.ReadUInt32
            Dim something1 = br.ReadSingle
            Dim someByte = br.ReadByte
            Dim something2 = br.ReadSingle
            Dim something3 = br.ReadSingle
            Dim someInt16 = br.ReadUInt16

            Dim decal_count = br.ReadUInt32
            ReDim road_decals(k).road_decal_list(decal_count - 1)
            For z = 0 To decal_count - 1
                road_decal_count += 1
                road_decals(k).road_decal_list(z) = New road_decal_list_
                ReDim road_decals(k).road_decal_list(z).matrix(15)
                For i = 0 To 15
                    road_decals(k).road_decal_list(z).matrix(i) = br.ReadSingle
                Next

                Dim something4 = br.ReadSingle
                Dim something5 = br.ReadSingle
                Dim something6 = br.ReadSingle
                Dim something7 = br.ReadSingle
                Dim something8 = br.ReadSingle
                Dim something9 = br.ReadSingle
                Dim something10 = br.ReadSingle
                Dim something11 = br.ReadSingle
                Dim something12 = br.ReadSingle
                Dim something13 = br.ReadSingle
                Dim something14 = br.ReadSingle
                Dim something15 = br.ReadSingle
                Dim something16 = br.ReadSingle


            Next
        Next
        ms.Dispose()
        GC.Collect()
        make_road_new_decals()
        For kk = 0 To road_decals.Length - 1
            FrmInfoWindow.tb1.Text = "Getting Road Decals(" + kk.ToString("000") + ")"
            Application.DoEvents()
            get_road_decal_textures(kk)
        Next

    End Sub
End Module
