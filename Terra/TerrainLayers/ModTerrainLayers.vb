#Region "imports"
Imports System.IO
Imports System.Math
Imports System.Runtime.InteropServices
Imports System
Imports System.Text
Imports System.Windows.Media.Media3D
Imports System.Windows.Media.Media3D.Vector3D

Imports Tao.OpenGl
Imports Tao.FreeGlut
Imports Tao.DevIl
Imports Tao.Platform

Imports Hjg.Pngcs
Imports Hjg.Pngcs.Chunks

Imports Ionic.Zip
Imports Ionic.Zlib.GZipStream
Imports Ionic.BZip2
'
Imports Ionic
Imports System.IO.Compression
#End Region

Module ModTerrainLayers

    Private Class DDS_HEADER_ ' for reference
        Public magic As UInt32 = &H20564444
        Public dwSize As UInt32 = &H7C
        Public dwFlags As UInt32 = &HA1007
        Public dwHeight As UInt32 = &H80
        Public dwWidth As UInt32 = &H80
        Public dwPitchOrLinearSize As UInt32 = &H4000
        Public dwDepth As UInt32 = &H1
        Public dwMipMapCount As UInt32 = &H1
        Public dwReserved1(11) As UInt32
        '--------- pixel format
        Public pf_dwSize As UInt32 = &H20 'Structure size = 32
        Public pf_dwFlags As UInt32 = 4 'Flags - DDPF_FOURCC - compressed RGB data
        Public pf_dwFourCC As UInt32 = &H35545844 'DXT5' backwards
        Public pf_dwRGBBitCount As UInt32 = 0
        Public pf_dwRBitMask As UInt32 = 0
        Public pf_dwGBitMask As UInt32 = 0
        Public pf_dwBBitMask As UInt32 = 0
        Public pf_dwABitMask As UInt32 = 0
        '--------------------------------
        Public dwCaps As UInt32 = &H1000
        Public dwCaps2 As UInt32 = &H0
        Public dwCaps3 As UInt32 = &H0
        Public dwCaps4 As UInt32 = &H0
        Public dwReserved2 As UInt32 = &H0
    End Class
    Dim DDS_HEADER_ARRAY() As Byte = _
           {&H44, &H44, &H53, &H20, &H7C, &H0, &H0, &H0, &H7, &H10, &HA, &H0, &H80, &H0, &H0, &H0, &H80, &H0, &H0, &H0, &H0, &H40, &H0, &H0, &H1, &H0, &H0, &H0, &H1, &H0, &H0, &H0, _
             &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, _
             &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H20, &H0, &H0, &H0, &H4, &H0, &H0, &H0, &H44, &H58, &H54, &H35, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, _
             &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H10, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0 _
           }
    Dim DDS_HEADER_SIZE As UInt32 = 128

    Public layer_info As layer_render_info_
    Public Structure layer_render_info_
        Public used_on() As UInt32
        Public render_info() As layer_render_info_entry_
    End Structure
    Public Structure layer_render_info_entry_
        Public texture_name As String
        Public width As Integer
        Public height As Integer
        Public count As Integer
        Public u As vect4
        Public v As vect4
        Public flags As UInt32
        Dim v1 As vect3 ' unknown?
        Public r1 As vect4
        Public r2 As vect4
        Public scale As vect4
    End Structure
    Dim cur_layer_info_pnt As Integer = 0
#Region "Layer building functions"


    Public Sub get_dominate_texture(ByVal map As Integer, ByRef ms As MemoryStream)

        Dim enc As New System.Text.ASCIIEncoding

        ms.Position = 0
        Dim br As New BinaryReader(ms)

        Dim magic1 = br.ReadInt32
        Dim version = br.ReadInt32
        'unzip the data
        ms.Position = 0
        magic1 = br.ReadUInt32
        version = br.ReadUInt32
        Dim number_of_textures As Integer = br.ReadUInt32
        Dim texture_string_length As Integer = br.ReadUInt32
        Dim d_width As Integer = br.ReadUInt32
        Dim d_height As Integer = br.ReadUInt32
        br.ReadUInt64()
        Dim texture_names(number_of_textures) As String
        Dim s_buff(texture_string_length) As Byte
        For i = 0 To number_of_textures - 1
            s_buff = br.ReadBytes(texture_string_length)
            texture_names(i) = enc.GetString(s_buff)
            'Threading.Thread.Sleep(10)
        Next



        Dim mg1 = br.ReadInt32
        Dim mg2 = br.ReadInt32
        Dim uncompressedsize = br.ReadInt32
        Dim buff(65536) As Byte
        Dim ps As New MemoryStream(buff)
        Dim count As UInteger = 0
        Dim total_read As Integer = 0
        Dim p_w As New StreamWriter(ps)

        Using Decompress As Zlib.ZlibStream = New Zlib.ZlibStream(ms, Zlib.CompressionMode.Decompress, False)
            Decompress.BufferSize = 65536
            Dim buffer(65536) As Byte
            Dim numRead As Integer
            numRead = Decompress.Read(buff, 0, buff.Length)
            total_read = numRead 'debug

        End Using

        Dim p_rd As New BinaryReader(ps)
        ReDim Preserve buff(total_read)
        Dim c_buff((total_read) * 4) As Byte


        For i = 0 To total_read - 1
            c_buff((i * 4) + 0) = (buff(i) + 1 And 7) << 4
            c_buff((i * 4) + 1) = (buff(i) + 1 And 7) << 4
            c_buff((i * 4) + 2) = (buff(i) + 1 And 7) << 4
            c_buff((i * 4) + 3) = 255
        Next
        'done with these so dispose of them.
        p_rd.Close()
        ps.Dispose()
        br.Close()
        br.Dispose()
        ms.Dispose()

        Dim h, w As Integer
        w = d_width
        h = d_height
        stride = (w / 2)
        count = 0
        'convert to 4 color data.

        '------------------------------------------------------------------
        'w = stride * 8 : h = h * 2
        'need point in to dbuff color buffer array
        Dim bufPtr As IntPtr = Marshal.AllocHGlobal(c_buff.Length - 1)
        Marshal.Copy(c_buff, 0, bufPtr, c_buff.Length - 1) ' copy dbuff to pufPtr's memory
        Dim texID = Ilu.iluGenImage() ' Generation of one image name
        Il.ilBindImage(texID) ' Binding of image name 
        Dim success = Il.ilGetError

        Il.ilTexImage(w, h, 1, 4, Il.IL_RGBA, Il.IL_UNSIGNED_BYTE, bufPtr) ' Create new image from pufPtr's data
        success = Il.ilGetError
        Marshal.FreeHGlobal(bufPtr) ' free this up
        If success = Il.IL_NO_ERROR Then
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)
            Dim f = Il.IL_FALSE
            Dim t = Il.IL_TRUE
            Ilu.iluMirror()

            Gl.glGenTextures(1, maplist(map).dominateId) ' make texture id
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(map).dominateId) ' bind the texture
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST) ' no filtering
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
            Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, Il.ilGetData()) '  Texture specification 
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Il.ilBindImage(0)
            Ilu.iluDeleteImage(texID)
            frmTestView.update_screen()
            'Stop
        Else
            MsgBox("Error Dom Texture! Il Error" + success.ToString, MsgBoxStyle.Exclamation, "Well Shit...")
        End If
    End Sub

    Public Sub open_hole_info(ByVal map As Integer, ByRef ms As MemoryStream)
        'Unpacks and creates a red channel only hole map texture for the hole data in terrain2
        ms.Position = 0
        Dim br As New BinaryReader(ms)

        Dim magic1 = br.ReadInt32
        Dim magic2 = br.ReadInt32
        Dim uncompressedsize = br.ReadInt32
        Dim buff(uncompressedsize) As Byte
        Dim ps As New MemoryStream(buff)
        Dim count As UInteger = 0
        Dim total_read As Integer = 0
        'unzip the data
        Using Decompress As Zlib.ZlibStream = New Zlib.ZlibStream(ms, Zlib.CompressionMode.Decompress, False)
            Decompress.BufferSize = 65536
            Dim buffer(65536) As Byte
            Dim numRead As Integer
            numRead = Decompress.Read(buffer, 0, buffer.Length)
            total_read += numRead 'debug
            Do While numRead <> 0
                ps.Write(buffer, 0, numRead)
                numRead = Decompress.Read(buffer, 0, buffer.Length)
                total_read += numRead 'debug
            Loop
        End Using
        If False Then
            File.WriteAllBytes("c:\!_bin_data\hole_data_" + map.ToString + ".bin", buff)
        End If
        Dim p_rd As New BinaryReader(ps)
        ps.Position = 0
        magic1 = p_rd.ReadUInt32
        Dim w As UInt32 = p_rd.ReadUInt32 / 4
        Dim h As UInt32 = p_rd.ReadUInt32 / 2
        Dim version As UInt32 = p_rd.ReadUInt32
        Dim data(w * h) As Byte

        p_rd.Read(data, 0, w * h)
        p_rd.Close()
        ps.Dispose()

        stride = (w / 2)
        Dim dbuff((stride * 8) * (h * 2) * 4) As Byte ' make room
        count = 0
        'convert to 4 color data.
        ReDim maplist(map).holes((stride * 8) - 1, (h * 2) - 1)


        For z1 = 0 To (h * 2) - 1
            For x1 = 0 To (stride) - 1
                Dim val = data((z1 * stride) + x1)
                For q = 0 To 7
                    Dim b = (1 And (val >> q))
                    If b > 0 Then b = 1
                    maplist(map).holes(63 - ((x1 * 8) + q), z1) = b
                Next
            Next
        Next

        br.Close()
        br.Dispose()
        ms.Dispose()

    End Sub

    Public Sub get_horizonShadowMap(map As Integer, e As Ionic.Zip.ZipEntry)
        'unused function
        Dim data(e.UncompressedSize) As Byte
        Dim s As New MemoryStream
        e.Extract(s)
        s.Position = 0
        Dim br1 As New BinaryReader(s)
        Dim magic1 = br1.ReadInt32
        Dim magic2 = br1.ReadInt32
        Dim uncompressedsize = br1.ReadInt32
        'Dim b(s.Length - 12) As Byte
        's.Read(b, 0, s.Length - 12)
        Dim buff(uncompressedsize) As Byte
        Dim ps As New MemoryStream(buff)
        'Dim ds = New Ionic.Zlib.GZipStream(s, Ionic.Zlib.CompressionMode.Decompress, False)

        'ds.Read(buff, 0, uncompressedsize)
        s.Position = 12

        Using Decompress As Zlib.ZlibStream = New Zlib.ZlibStream(s, Zlib.CompressionMode.Decompress, False)
            ' Copy the compressed file into the decompression stream. 
            Dim buffer As Byte() = New Byte(4096) {}
            Dim numRead As Integer
            numRead = Decompress.Read(buffer, 0, buffer.Length)
            Do While numRead <> 0
                ps.Write(buffer, 0, numRead)
                numRead = Decompress.Read(buffer, 0, buffer.Length)
            Loop

        End Using
        'ds.Dispose()
        Dim smap As New MemoryStream
        Dim texID As UInt32

        texID = Ilu.iluGenImage() ' Generation of one image name
        Il.ilBindImage(texID) ' Binding of image name 
        Dim br As New BinaryReader(New MemoryStream(buff))
        br.ReadInt32()
        Dim w = br.ReadUInt32
        Dim h = br.ReadUInt32
        Dim bpp = br.ReadUInt32
        Dim version = br.ReadUInt32
        Dim padding = br.ReadInt32
        padding = br.ReadInt32
        padding = br.ReadInt32
        padding = br.ReadInt32
        Dim buff2(w * h * 4) As Byte
        buff2 = br.ReadBytes(w * h * 4)
        Dim bufPtr As IntPtr = Marshal.AllocHGlobal(buff2.Length)
        Marshal.Copy(buff2, 0, bufPtr, buff2.Length)

        Il.ilTexImage(w, h, 1, 4, Il.IL_RGBA, Il.IL_UNSIGNED_BYTE, bufPtr) ' should have a new image here
        Dim success = Il.ilGetError
        Marshal.FreeHGlobal(bufPtr) ' free this up
        If success = Il.IL_NO_ERROR Then
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)

            success = Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE)    ' Convert every colour component into unsigned bytes
            'Ilu.iluFlipImage()
            'Ilu.iluMirror()

            'If your image contains alpha channel you can replace IL_RGB with IL_RGBA 
            '  Gl.glGenTextures(1, map_layers(map).shadowMapID)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            '  Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(map).shadowMapID)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
            Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
            Il.ilGetData()) '  Texture specification 
            'Gl.glGenerateMipmapEXT(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Il.ilBindImage(0)
            Ilu.iluDeleteImage(texID)
        End If
        br.Close()
        s.Dispose()

    End Sub

    Public Function split_blend_texture_data(ByRef ck As Ionic.Zip.ZipFile, ByVal map As Integer) As Boolean

        '---------------------------------------------------------------------
        'lets get the layer render info first
        get_layer_variables_from_file(ck, map)
        '---------------------------------------------------------------------
        Dim blend As Ionic.Zip.ZipEntry = ck("terrain2/blend_textures")
        Dim ms As New MemoryStream
        blend.Extract(ms)
        ms.Position = 0
        If map = 7 Then
            'Stop
        End If
        Dim br As New BinaryReader(ms)

        Dim magic = br.ReadUInt32()
        Dim section_cnt = br.ReadUInt32
        section_cnt = 4
        Dim sec_sizes(section_cnt - 1) As UInt32
        For i = 0 To section_cnt - 1
            sec_sizes(i) = br.ReadUInt32
        Next

        For i = 0 To section_cnt - 1
            Dim len = sec_sizes(i)
            If len > 0 Then
                map_layers(map).layers(i + 1).data = br.ReadBytes(len)
                If False Then
                    File.WriteAllBytes("C:\!_bin_data\Map " + map.ToString("000") + " layer_" + _
                                       i.ToString + ".bin", map_layers(map).layers(i + 1).data)
                End If
            End If
        Next
        Return True
    End Function

    Public Sub get_layer_variables_from_file(ByRef ck As Ionic.Zip.ZipFile, ByVal map As Integer)
        Dim blend As Ionic.Zip.ZipEntry = ck("terrain2/layers")
        Dim ms As New MemoryStream
        blend.Extract(ms)
        ms.Position = 0
        Dim br As New BinaryReader(ms)

        Dim magic = br.ReadUInt32
        Dim map_count = br.ReadUInt32
        ReDim layer_info.used_on(7)
        For i = 0 To 7
            layer_info.used_on(i) = br.ReadUInt32
        Next
        ReDim layer_info.render_info(0)
        layer_info.render_info(0) = New layer_render_info_entry_
        ReDim Preserve layer_info.render_info(7)
        For i = 0 To map_count - 1
            layer_info.render_info(i) = New layer_render_info_entry_
            br.ReadUInt32() 'magic
            layer_info.render_info(i).width = br.ReadUInt32
            layer_info.render_info(i).height = br.ReadUInt32
            layer_info.render_info(i).count = br.ReadUInt32

            layer_info.render_info(i).u.x = br.ReadSingle
            layer_info.render_info(i).u.y = br.ReadSingle
            layer_info.render_info(i).u.z = br.ReadSingle
            layer_info.render_info(i).u.w = br.ReadSingle

            layer_info.render_info(i).v.x = br.ReadSingle
            layer_info.render_info(i).v.y = br.ReadSingle
            layer_info.render_info(i).v.z = br.ReadSingle
            layer_info.render_info(i).v.w = br.ReadSingle

            layer_info.render_info(i).flags = br.ReadUInt32

            'not sure about these 3
            layer_info.render_info(i).v1.x = br.ReadSingle
            layer_info.render_info(i).v1.y = br.ReadSingle
            layer_info.render_info(i).v1.z = br.ReadSingle

            layer_info.render_info(i).r1.x = br.ReadSingle
            layer_info.render_info(i).r1.y = br.ReadSingle
            layer_info.render_info(i).r1.z = br.ReadSingle
            layer_info.render_info(i).r1.w = br.ReadSingle

            layer_info.render_info(i).r2.x = br.ReadSingle
            layer_info.render_info(i).r2.y = br.ReadSingle
            layer_info.render_info(i).r2.z = br.ReadSingle
            layer_info.render_info(i).r2.w = br.ReadSingle

            'not sure about these
            layer_info.render_info(i).scale.x = br.ReadSingle
            layer_info.render_info(i).scale.y = br.ReadSingle
            layer_info.render_info(i).scale.z = br.ReadSingle
            layer_info.render_info(i).scale.w = br.ReadSingle

            Dim bs = br.ReadUInt32
            Dim d = br.ReadBytes(bs)
            layer_info.render_info(i).texture_name = Encoding.UTF8.GetString(d, 0, d.Length)
            br.ReadByte()

        Next
        ms.Dispose()
        GC.Collect()

    End Sub

    Public Function get_layers(ByVal ck As Ionic.Zip.ZipFile, map As Integer) As Integer
        Dim layer_count As Integer = 0
        cur_layer_info_pnt = 0
        ReDim map_layers(map).layers(4)
        map_layers(map).used_layers = 0
        map_layers(map).layers(1).text_id = dummy_texture 'preset these to the dummys so they are not empty.
        map_layers(map).layers(2).text_id = dummy_texture
        map_layers(map).layers(3).text_id = dummy_texture
        map_layers(map).layers(4).text_id = dummy_texture
        map_layers(map).layers(1).text_id2 = dummy_texture
        map_layers(map).layers(2).text_id2 = dummy_texture
        map_layers(map).layers(3).text_id2 = dummy_texture
        map_layers(map).layers(4).text_id2 = dummy_texture

        map_layers(map).layers(1).norm_id = dummy_texture 'preset these to the dummys so they are not empty.
        map_layers(map).layers(2).norm_id = dummy_texture
        map_layers(map).layers(3).norm_id = dummy_texture
        map_layers(map).layers(4).norm_id = dummy_texture
        map_layers(map).layers(1).norm_id2 = dummy_texture
        map_layers(map).layers(2).norm_id2 = dummy_texture
        map_layers(map).layers(3).norm_id2 = dummy_texture
        map_layers(map).layers(4).norm_id2 = dummy_texture

        map_layers(map).layers(1).mix_texture_Id = dummy_texture 'preset these to the dummys so they are not empty.
        map_layers(map).layers(2).mix_texture_Id = dummy_texture
        map_layers(map).layers(3).mix_texture_Id = dummy_texture
        map_layers(map).layers(4).mix_texture_Id = dummy_texture


        layer_uv_list += "..........................................." + vbCrLf
        Dim sm = "map:" + map.ToString + vbCrLf
        layer_uv_list += sm
        'Look for each layer file. If it exist, go get its data.
        '================================================================
        'get blend_textures and split in to an array of raw data
        split_blend_texture_data(ck, map)

        If map_layers(map).layers(1).data IsNot Nothing Then
            get_layer_data(1, map)
            'map_layers(map).layers(1).ms.Dispose()
            layer_count += 1
        End If

        If map_layers(map).layers(2).data IsNot Nothing Then
            get_layer_data(2, map)
            'map_layers(map).layers(2).ms.Dispose()
            layer_count += 1
        End If

        If map_layers(map).layers(3).data IsNot Nothing Then
            get_layer_data(3, map)
            'map_layers(map).layers(3).ms.Dispose()
            layer_count += 1
        End If

        If map_layers(map).layers(4).data IsNot Nothing Then
            get_layer_data(4, map)
            'map_layers(map).layers(4).ms.Dispose()
            layer_count += 1
        End If


        map_layers(map).layer_count = layer_count

        'If layer_count = 2 Then
        '	set_layer_2(map)
        'End If
        If layer_count = 0 Then
            'MsgBox("This chunk has no layers! Map:" + map.ToString("000"), MsgBoxStyle.Exclamation, "Well Shit....")
            Return layer_count
        End If
        'make the mix map for this layer
        'make_mix_texture_id(map, map_layers(map).mix_image)
        map_layers(map).mix_image = Nothing
        GC.Collect()
        'frmMain.PictureBox1.Image = map_layers(map).mix_image.Clone
        'Application.DoEvents()
        'Application.DoEvents()
    End Function

    Public Function get_raw_dds(ByRef data() As Byte) As Integer

        Dim texID = Il.ilGenImage
        Dim text_id As Integer

        Il.ilBindImage(texID)
        Dim success = Il.ilGetError

        Il.ilLoadL(Il.IL_DDS, data, data.Length)

        success = Il.ilGetError
        If success = Il.IL_NO_ERROR Then
            Ilu.iluMirror()
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)
            Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE)
            Gl.glGenTextures(1, text_id)

            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, text_id)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
                    Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
                    Il.ilGetData()) '  Texture specification 
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Il.ilBindImage(0)
            Ilu.iluDeleteImage(texID)
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Return text_id
        Else
            MsgBox("Failed to load terrian mix map!", MsgBoxStyle.Exclamation, "Well Shit!")
        End If
        Il.ilBindImage(0)
        Ilu.iluDeleteImage(texID)

    End Function

    Public Sub get_layer_data(layer As Integer, map As Integer)
        Dim ms As New MemoryStream(map_layers(map).layers(layer).data)
        ms.Position = 0
        Dim b As New BinaryReader(ms)

        Dim magic = b.ReadInt32() 'Magic

        Dim version = b.ReadInt32() 'version?

        'read size
        map_layers(map).layers(layer).sizex = b.ReadInt16 ' get size in x
        map_layers(map).layers(layer).sizez = b.ReadInt16 ' get size in z
        Dim uk = b.ReadInt16()
        If uk <> 19 Then
            Stop
        End If
        '------------------------------------------
        'get textures
        Dim tex_cnt = b.ReadUInt16
        b.ReadUInt32() 'fill
        b.ReadUInt32() 'fill
        map_layers(map).layers(layer).l_name = ""
        map_layers(map).layers(layer).l_name2 = ""

        Dim bs = b.ReadUInt32
        Dim d = b.ReadBytes(bs)
        map_layers(map).layers(layer).l_name = Encoding.UTF8.GetString(d, 0, d.Length)
        'Debug.WriteLine(map.ToString + " 1 " + map_layers(map).layers(layer).l_name)

        If tex_cnt > 1 Then
            bs = b.ReadUInt32
            d = b.ReadBytes(bs)
            map_layers(map).layers(layer).l_name2 = Encoding.UTF8.GetString(d, 0, d.Length)
            'Debug.WriteLine(map.ToString + " 2 " + map_layers(map).layers(layer).l_name2)

        End If
        Dim buf = b.ReadBytes(16384) 'read the DDS data with out a header. It has none!
        '------------------------------------------
        'Create a dds.. add header followed by the data from blend_textures

        ReDim map_layers(map).layers(layer).Mix_data(DDS_HEADER_SIZE + 16384)
        DDS_HEADER_ARRAY.CopyTo(map_layers(map).layers(layer).Mix_data, 0) 'copy the DDS header
        buf.CopyTo(map_layers(map).layers(layer).Mix_data, DDS_HEADER_SIZE) 'copy DDS data
        'get the dds from the mix_data.
        map_layers(map).layers(layer).mix_texture_Id = get_raw_dds(map_layers(map).layers(layer).Mix_data)
        'File.WriteAllBytes("C:\!_DDS\DDS_M" + map.ToString + "_L" + layer.ToString + ".dds", map_layers(map).layers(layer).Mix_data)
        '--------------------------------------------------------------------------------------------------------------
        'there should always be a layer 1 and at this point we need to create the mix value bitmap
        If layer = 1 Then
            map_layers(map).mix_image = New Bitmap(256, 256, Imaging.PixelFormat.Format32bppArgb)
        End If

        '--------------------------------------------------------------------------------------------------------------
        '--------------------------------------------------------------------------------------------------------------


        'read uv mapping. The next 8 bytes are actually 2 vect4 numbers. We want the X and the Z from these
        'read unused
        Dim u = layer_info.render_info(cur_layer_info_pnt).u
        Dim v = layer_info.render_info(cur_layer_info_pnt).v
        map_layers(map).layers(layer).uP1 = u
        map_layers(map).layers(layer).vP1 = v

        cur_layer_info_pnt += 1

        Dim u2 = layer_info.render_info(cur_layer_info_pnt).u
        Dim v2 = layer_info.render_info(cur_layer_info_pnt).v
        map_layers(map).layers(layer).uP2 = u2
        map_layers(map).layers(layer).vP2 = v2

        cur_layer_info_pnt += 1

        'Debug.Write("map" + map.ToString + " layer:" + layer.ToString + " U:" + _
        '				  map_layers(map).layers(layer).u.ToString + " V:" + map_layers(map).layers(layer).v.ToString + vbCrLf)
        'Dim sx = Sign(u.x) * Sqrt((u.x ^ 2) + (u.z ^ 2))
        'Dim sy = Sign(v.y) * Sqrt((v.x ^ 2) + (v.z ^ 2))
        'Dim rt = Atan2(v.x, v.z)

        Dim su = "  u.x:" + u.x.ToString("000.00000") + "  u.y:" + u.y.ToString("000.00000") + "  u.z:" + u.z.ToString("000.00000") + "  u.w:" + u.w.ToString("000.00000")
        Dim sv = "  v.x:" + v.x.ToString("000.00000") + "  v.y:" + v.y.ToString("000.00000") + "  v.z:" + v.z.ToString("000.00000") + "  v.w:" + v.w.ToString("000.00000")
        Dim su2 = "  u.x:" + u2.x.ToString("000.00000") + "  u.y:" + u2.y.ToString("000.00000") + "  u.z:" + u2.z.ToString("000.00000") + "  u.w:" + u2.w.ToString("000.00000")
        Dim sv2 = "  v.x:" + v2.x.ToString("000.00000") + "  v.y:" + v2.y.ToString("000.00000") + "  v.z:" + v2.z.ToString("000.00000") + "  v.w:" + v2.w.ToString("000.00000")
        'Dim sc = "scale x:" + sx.ToString + "   scale y:" + sy.ToString
        'Dim rs = "rotation:" + rt.ToString
        layer_uv_list += "U1 V1 :" + su + vbCrLf + sv + vbCrLf + vbCrLf
        layer_uv_list += "U2 V2 :" + su2 + vbCrLf + sv2 + vbCrLf + vbCrLf



        If tex_cnt > 0 Then

            '=======================================
set_mask:
            Select Case layer
                Case 1
                    map_layers(map).used_layers = map_layers(map).used_layers Or 1
                Case 2
                    map_layers(map).used_layers = map_layers(map).used_layers Or 2
                Case 3
                    map_layers(map).used_layers = map_layers(map).used_layers Or 4
                Case 4
                    map_layers(map).used_layers = map_layers(map).used_layers Or 8
            End Select
        Else
            'If Not map_name.ToLower.ToLower.Contains("global_am") Then
            '    frmMapInfo.I__Map_Textures_tb.Text += "No Normal Map in layer data.  Map:" + map.ToString + "   Layer:" + layer.ToString + vbCrLf 'save info
            Stop
        End If

        'End If
        'get_layer_mix(map, layer)
        ms.Dispose()

    End Sub


    Public Sub get_layer_mix(ByVal map As Int32, ByRef layer As Integer)
        'Dim data((map_layers(map).layers(layer).sizex * map_layers(map).layers(layer).sizex)) As Byte
        Dim cnt As UInt32 = 0
        Dim i As UInt32 = 0
        Dim cols As Integer = 0
        Dim x, y As Integer

        Dim rect As New Rectangle(0, 0, 256, 256)
        Dim bmpData As System.Drawing.Imaging.BitmapData = map_layers(map).mix_image.LockBits(rect, _
             Drawing.Imaging.ImageLockMode.ReadWrite, map_layers(map).mix_image.PixelFormat)

        ' Get the address of the first line. 
        Dim ptr As IntPtr = bmpData.Scan0

        Dim bbytes As Integer = Math.Abs(bmpData.Stride) * map_layers(map).mix_image.Height
        Dim rgbValues(bbytes - 1) As Byte

        ' Copy the RGB values into the array.
        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bbytes)
        Dim lc As Integer = map_layers(map).layer_count
        Dim check As ULong = 0
        ' Set the color byte based on its layer. A, R, G and B
        Dim repeat As Boolean = True
loop_it:
        For lp = 1 To 1



            Dim x_cnt = 0
            Dim y_cnt = 0
            cnt = 0
            For i = 0 To 126

                For j = 0 To 127
                    For k = 0 To 1

                        Select Case layer
                            Case 1
                                check += map_layers(map).layers(layer).Mix_data(j + (i * 128))
                                Dim pos = y_cnt
                                If repeat Then
                                    rgbValues(x_cnt + pos + 0) = map_layers(map).layers(layer).Mix_data(j + (i * 128)) ' And &HF
                                    rgbValues(x_cnt + pos + 1) = map_layers(map).layers(layer).Mix_data(j + (i * 128)) ' And &HF
                                    cnt += 1
                                    x_cnt += 4
                                    rgbValues(x_cnt + pos + 0) += map_layers(map).layers(layer).Mix_data(j + (i * 128)) ' And &HF
                                    rgbValues(x_cnt + pos + 1) += map_layers(map).layers(layer).Mix_data(j + (i * 128)) ' And &HF
                                    cnt += 1
                                    x_cnt += 4
                                Else
                                    rgbValues(x_cnt + pos + 0) += map_layers(map).layers(layer).Mix_data(j + (i * 128)) ' And &HF
                                    x_cnt += 4
                                    rgbValues(x_cnt + pos + 1) += map_layers(map).layers(layer).Mix_data(j + (i * 128)) ' And &HF
                                    x_cnt += 4

                                End If

                            Case 2
                                check += map_layers(map).layers(layer).data(i)
                                'rgbValues((i * 4) + 1) = map_layers(map).layers(layer).data(i)
                            Case 3
                                check += map_layers(map).layers(layer).data(i)
                                'rgbValues((i * 4) + 2) = map_layers(map).layers(layer).data(i)
                            Case 4
                                check += map_layers(map).layers(layer).data(i)
                                'rgbValues((i * 4) + 3) = map_layers(map).layers(layer).data(i)
                        End Select
                        If x_cnt > bmpData.Stride - 1 Then
                            x_cnt = 0
                            y_cnt += bmpData.Stride
                        End If
                    Next 'k
                Next 'j
            Next 'i
            repeat = False
        Next 'lp

        If check = 0 Then
            '	MsgBox("This layer has no blend values - Map: " + map.ToString + " layer: " + layer.ToString, MsgBoxStyle.Exclamation, "Debug:")
        End If
        ' Copy the RGB values back to the bitmap
        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bbytes)

        ' Unlock the bits.
        map_layers(map).mix_image.UnlockBits(bmpData)

        Return
    End Sub

    Public Function ResizeImage(ByVal image As Image, _
    ByVal size As Size, Optional ByVal preserveAspectRatio As Boolean = True) As Image
        Dim newWidth As Integer
        Dim newHeight As Integer
        If preserveAspectRatio Then
            Dim originalWidth As Integer = image.Width
            Dim originalHeight As Integer = image.Height
            Dim percentWidth As Single = CSng(size.Width) / CSng(originalWidth)
            Dim percentHeight As Single = CSng(size.Height) / CSng(originalHeight)
            Dim percent As Single = If(percentHeight < percentWidth,
                 percentHeight, percentWidth)
            newWidth = CInt(originalWidth * percent)
            newHeight = CInt(originalHeight * percent)
        Else
            newWidth = size.Width
            newHeight = size.Height
        End If
        Dim newImage As Bitmap = New Bitmap(newWidth, newHeight, image.PixelFormat)
        Using graphicsHandle As Graphics = Graphics.FromImage(newImage)
            graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic
            graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight)
        End Using
        image.Dispose()
        Return newImage

    End Function

    Public Function get_layer_color_image(map As Integer, layer As Integer) As Boolean
        'this will be rewrote 
        Dim cnt = layer_texture_cache.Length
        Dim map_name As String = map_layers(map).layers(layer).l_name
        Dim map_name2 As String = map_layers(map).layers(layer).l_name2

        'Debug.WriteLine(map.ToString + " " + map_name)
        '------------------ texture 1
        If map_name = "" Then
            map_layers(map).layers(layer).text_id = dummy_texture
        End If
        For i = 0 To cnt - 1
            If layer_texture_cache(i).name = map_name Then
                map_layers(map).layers(layer).text_id = layer_texture_cache(i).id
                saved_texture_loads += 1
                GoTo get_map_2
            End If
        Next

        Dim dds As New MemoryStream
        'map_layers(map).layers(layer).image = 
        '================= get color map
        'if get_main_texture is false, it wont read the huge main texture again!
        If map_name.ToLower.Contains("global_am") Then
            cnt = layer_texture_cache.Length
            ReDim Preserve layer_texture_cache(cnt)
            layer_texture_cache(cnt - 1) = New texture_
            layer_texture_cache(cnt - 1).name = map_name
        Else
            Dim map_entry As Ionic.Zip.ZipEntry = active_pkg(map_name)
            If map_entry Is Nothing Then
                map_entry = get_shared(map_name)
                If map_entry Is Nothing Then
                    frmMapInfo.I__Map_Textures_tb.Text += "MISSING TEXTURE: " + map_name + vbCrLf 'save info
                    Return True
                End If
            End If
            map_entry.Extract(dds)
            map_layers(map).layers(layer).text_id = build_layer_color_texture(map, dds, layer)        'We dont need the Bitmap. This saves some time

        End If

        frmMapInfo.I__Map_Textures_tb.Text += "Color: " + map_name + vbCrLf 'save info

        'update the cache
        cnt = layer_texture_cache.Length
        ReDim Preserve layer_texture_cache(cnt)
        layer_texture_cache(cnt - 1) = New texture_
        layer_texture_cache(cnt - 1).name = map_name
        layer_texture_cache(cnt - 1).id = map_layers(map).layers(layer).text_id

        '------------------ texture 2
        '------------------ texture 2
        '------------------ texture 2
        '------------------ texture 2
        '------------------ texture 2
        '------------------ texture 2
get_map_2:
        If map_name2 = "" Then
            map_layers(map).layers(layer).text_id2 = dummy_texture
            Return False
        End If
        cnt = layer_texture_cache.Length
        For i = 0 To cnt - 1
            If layer_texture_cache(i).name = map_name2 Then
                map_layers(map).layers(layer).text_id2 = layer_texture_cache(i).id
                saved_texture_loads += 1
                Return False
            End If
        Next
        '================= get color map 2
        'if get_main_texture is false, it wont read the huge main texture again!
        If map_name2.ToLower.Contains("global_am") Then
            cnt = layer_texture_cache.Length
            ReDim Preserve layer_texture_cache(cnt)
            layer_texture_cache(cnt - 1) = New texture_
            layer_texture_cache(cnt - 1).name = map_name2
        Else
            Dim map_entry As Ionic.Zip.ZipEntry = active_pkg(map_name2)
            If map_entry Is Nothing Then
                map_entry = get_shared(map_name2)
                If map_entry Is Nothing Then
                    frmMapInfo.I__Map_Textures_tb.Text += "MISSING TEXTURE: " + map_name2 + vbCrLf 'save info
                    Return True
                End If
            End If
            dds = New MemoryStream
            map_entry.Extract(dds)
            map_layers(map).layers(layer).text_id2 = build_layer_color_texture(map, dds, layer)        'We dont need the Bitmap. This saves some time
            dds.Dispose()
        End If

        frmMapInfo.I__Map_Textures_tb.Text += "Color: " + map_name2 + vbCrLf 'save info

        'update the cache
        cnt = layer_texture_cache.Length
        ReDim Preserve layer_texture_cache(cnt)
        layer_texture_cache(cnt - 1) = New texture_
        layer_texture_cache(cnt - 1).name = map_name2
        layer_texture_cache(cnt - 1).id = map_layers(map).layers(layer).text_id2

        Return False
    End Function

    Public Function get_layer_normal_image(map As Integer, layer As Integer) As Boolean
        Dim cnt = layer_texture_cache.Length
        Dim normal_update As Boolean = True
        map_layers(map).layers(layer).n_name = map_layers(map).layers(layer).l_name.Replace("_AM", "_NM")
        map_layers(map).layers(layer).n_name2 = map_layers(map).layers(layer).l_name2.Replace("_AM", "_NM")
        Dim n_map_name As String = map_layers(map).layers(layer).n_name
        Dim n_map_name2 As String = map_layers(map).layers(layer).n_name2
        'Debug.WriteLine(map.ToString + " " + n_map_name)

        ' get normal_map 1 ----------------------------------------------------------------
        'check if this texture already exists
        If n_map_name = "" Then
            map_layers(map).layers(layer).norm_id = dummy_texture
        End If
        'check if this texture already exists
        cnt = layer_texture_cache.Length
        n_map_name = map_layers(map).layers(layer).n_name
        For i = 0 To cnt - 1
            If layer_texture_cache(i).name = n_map_name Then
                map_layers(map).layers(layer).norm_id = layer_texture_cache(i).id
                saved_texture_loads += 1
                GoTo get_map_2
            End If
        Next
        Dim retry As Boolean = False
try_again1:
        '================= get norml map
        Dim nmap_entry As Ionic.Zip.ZipEntry = active_pkg(n_map_name)
        If nmap_entry Is Nothing Then
            nmap_entry = get_shared(n_map_name)
            If nmap_entry Is Nothing Then

                If retry Then
                    normal_update = False
                    If Not n_map_name.ToLower.Contains("global_am") Then ' stop spamming
                        frmMapInfo.I__Map_Textures_tb.Text += "MISSING TEXTURE: " + n_map_name + vbCrLf 'save info
                    End If
                    Return True
                End If
                'see if we can find it by adding "_NM" to the diffuse map name.
                retry = True ' to stop endless looping
                n_map_name = map_layers(map).layers(layer).l_name.Replace(".", "") + "_NM.dds"
                For i = 0 To cnt - 1
                    If layer_texture_cache(i).name = n_map_name Then
                        map_layers(map).layers(layer).norm_id = layer_texture_cache(i).id
                        saved_texture_loads += 1
                        Return False
                    End If
                Next
                GoTo try_again1
            End If
        End If
        Dim mdds As New MemoryStream
        nmap_entry.Extract(mdds)
        map_layers(map).layers(layer).norm_id = build_layer_normal_texture(map, mdds, layer)

        frmMapInfo.I__Map_Textures_tb.Text += "Normal: " + n_map_name + vbCrLf 'save info

        'update the cache
        cnt = layer_texture_cache.Length
        ReDim Preserve layer_texture_cache(cnt)
        layer_texture_cache(cnt - 1) = New texture_
        layer_texture_cache(cnt - 1).name = n_map_name
        layer_texture_cache(cnt - 1).id = map_layers(map).layers(layer).norm_id

        ' get normal_map 2 ----------------------------------------------------------------
        ' get normal_map 2 ----------------------------------------------------------------
        ' get normal_map 2 ----------------------------------------------------------------
        ' get normal_map 2 ----------------------------------------------------------------
        ' get normal_map 2 ----------------------------------------------------------------
get_map_2:
        If n_map_name2 = "" Then
            map_layers(map).layers(layer).norm_id2 = dummy_texture
            Return False
        End If
        'check if this texture already exists
        cnt = layer_texture_cache.Length
        n_map_name2 = map_layers(map).layers(layer).n_name2
        For i = 0 To cnt - 1
            If layer_texture_cache(i).name = n_map_name2 Then
                map_layers(map).layers(layer).norm_id2 = layer_texture_cache(i).id
                saved_texture_loads += 1
                Return False
            End If
        Next
        Dim retry2 As Boolean = False
try_again2:
        '================= get norml map
        Dim nmap_entry2 As Ionic.Zip.ZipEntry = active_pkg(n_map_name)
        If nmap_entry2 Is Nothing Then
            nmap_entry2 = get_shared(n_map_name2)
            If nmap_entry2 Is Nothing Then

                If retry2 Then
                    If Not n_map_name2.ToLower.Contains("global_am") Then ' stop spamming
                        frmMapInfo.I__Map_Textures_tb.Text += "MISSING TEXTURE: " + n_map_name + vbCrLf 'save info
                    End If
                    Return True
                End If
                'see if we can find it by adding "_NM" to the diffuse map name.
                retry2 = True ' to stop endless looping
                n_map_name2 = map_layers(map).layers(layer).l_name.Replace(".", "") + "_NM.dds"
                For i = 0 To cnt - 1
                    If layer_texture_cache(i).name = n_map_name2 Then
                        map_layers(map).layers(layer).norm_id2 = layer_texture_cache(i).id
                        saved_texture_loads += 1
                        Return False
                    End If
                Next
                GoTo try_again2
            End If
        End If
        Dim mdds2 As New MemoryStream
        nmap_entry2.Extract(mdds2)
        map_layers(map).layers(layer).norm_id2 = build_layer_normal_texture(map, mdds2, layer)
        mdds2.Dispose()

        frmMapInfo.I__Map_Textures_tb.Text += "Normal: " + n_map_name + vbCrLf 'save info

        'update the cache
        cnt = layer_texture_cache.Length
        ReDim Preserve layer_texture_cache(cnt)
        layer_texture_cache(cnt - 1) = New texture_
        layer_texture_cache(cnt - 1).name = n_map_name2
        layer_texture_cache(cnt - 1).id = map_layers(map).layers(layer).norm_id2

        Return False
    End Function

    Public Function get_layer_image(map As Integer, layer As Integer) As Boolean
        're-writing this
        If Not frmMain.m_high_rez_Terrain.Checked Then Return False 'no hires terrain textures
        '---------------------------------------------------
        Dim color_status = get_layer_color_image(map, layer)
        Dim norm_status = get_layer_normal_image(map, layer)
        '---------------------------------------------------
        If color_status Or norm_status Then
            Return False
        Else
            Return True
        End If

    End Function

    Public Sub create_mixMaps(ByVal layer As Integer)
        'Setup to adding the 8 bit pixel border around the mix map images
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Gl.glFinish()
        Gl.glFrontFace(Gl.GL_CW)
        Gl.glCullFace(Gl.GL_BACK)
        Gl.glLineWidth(1)
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        '-------------------------------------------------------------------------------
        Dim w = CInt(Sqrt(maplist.Length - 1))
        Dim psize = 144 ' 128 + 16
        '---------------------
        'setup size of renderwindow
        frmMain.pb2.Location = New Point(0, frmMain.mainMenu.Height + frmMain.ProgressBar1.Height)
        frmMain.pb2.Width = psize
        frmMain.pb2.Height = psize

        'frmMain.pb2.BringToFront()
        'frmMain.pb2.Visible = True
        ResizeGL_2(psize, psize)
        ViewPerspective_2()
        ViewOrtho_2()
        Gl.glDisable(Gl.GL_LIGHTING)
        'Gl.glColor3f(0.8!, 0.0!, 0.0!)
        'Gl.glColor3f(1.0!, 1.0!, 1.0!)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        '----------------------
        'We draw bigger and bigger squares to put a boarder around the actual image.
        'This is to reduce the impact of linear filtering.
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        draw_mixmap_quads(w, psize, layer)



        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glFinish()
        Gl.glFlush()

        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If


    End Sub
    Public Sub blur_color_map(ByVal texture_id As Integer) ' unused sub
        'Not used currrently
        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Dim wx, wy As Integer
        Dim uc As vect2
        Dim lc As vect2
        Gl.glFinish()
        Gl.glFrontFace(Gl.GL_CW)
        Gl.glCullFace(Gl.GL_BACK)
        Gl.glLineWidth(1)
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        '-------------------------------------------------------------------------------
        Dim w = CInt(Sqrt(maplist.Length - 1))
        Dim psize = 272
        '---------------------
        'setup size of renderwindow
        frmMain.pb2.Location = New Point(0, frmMain.mainMenu.Height + frmMain.ProgressBar1.Height)
        frmMain.pb2.Width = psize
        frmMain.pb2.Height = psize

        'frmMain.pb2.BringToFront()
        'frmMain.pb2.Visible = True
        ResizeGL_2(psize, psize)
        ViewPerspective_2()
        ViewOrtho_2()
        Gl.glDisable(Gl.GL_LIGHTING)
        'Gl.glColor3f(0.8!, 0.0!, 0.0!)
        'Gl.glColor3f(1.0!, 1.0!, 1.0!)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        '----------------------
        'We draw bigger and bigger squares to put a boarder around the actual image.
        'This is to reduce the impact of linear filtering.
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture_id)
        Gl.glBegin(Gl.GL_QUADS)
        Dim border As Integer = (psize - 256) / 2
        For off = 0 To border
            Dim off_X2 = psize - (off * 2)

            wx = 0
            wy = 0

            '---
            uc.x = off : lc.y = off
            lc.x = uc.x + off_X2 : uc.y = lc.y + off_X2
            Gl.glTexCoord2f(0.0!, 0.0!)
            Gl.glVertex3f(uc.x, -lc.y, -1.0!)

            Gl.glTexCoord2f(0.0!, -1.0!)
            Gl.glVertex3f(uc.x, -uc.y, -1.0!)

            Gl.glTexCoord2f(1.0!, -1.0!)
            Gl.glVertex3f(lc.x, -uc.y, -1.0!)

            Gl.glTexCoord2f(1.0!, 0.0!)
            Gl.glVertex3f(lc.x, -lc.y, -1.0!)

        Next ' off loop
        Gl.glEnd()
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        'frmShowImage.draw_texture(map_layers(cnt).mix_texture_Id)
        replace_mix_layer(texture_id, psize)



        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glFinish()
        Gl.glFlush()

        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If


    End Sub

    Private Sub draw_mixmap_quads(ByVal w As Integer, ByVal size As Integer, ByVal layer As Integer)
        'This adds a 8 pixel wide border around the mix image to stop visible artifacts at the edges.
        Dim uc As vect2
        Dim lc As vect2
        Dim x, y As Integer
        Dim cnt As Integer

        'We draw smaller and smaller quads to put a boarder around the actual image.
        'This is to deal with how the bluring screw the boarders up.
        cnt = 0
        For x = 0 To w - 1
            For y = 0 To w - 1
                Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(cnt).layers(layer).mix_texture_Id)
                Gl.glBegin(Gl.GL_QUADS)
                Dim border As Integer = (size - 128) / 2
                For off = 0 To border
                    Dim off_X2 = size - (off * 2)
                    '---
                    uc.x = off : lc.y = off
                    lc.x = uc.x + off_X2 : uc.y = lc.y + off_X2
                    Gl.glTexCoord2f(0.0!, 0.0!)
                    Gl.glVertex3f(uc.x, -lc.y, -1.0!)

                    Gl.glTexCoord2f(0.0!, -1.0!)
                    Gl.glVertex3f(uc.x, -uc.y, -1.0!)

                    Gl.glTexCoord2f(1.0!, -1.0!)
                    Gl.glVertex3f(lc.x, -uc.y, -1.0!)

                    Gl.glTexCoord2f(1.0!, 0.0!)
                    Gl.glVertex3f(lc.x, -lc.y, -1.0!)

                Next ' off loop
                Gl.glEnd()
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
                replace_mix_layer(map_layers(cnt).layers(layer).mix_texture_Id, size)
                '---------
                'cnt has to be on outside of "off" loop
                frmMain.ProgressBar1.Value = cnt
                cnt += 1
            Next
        Next

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

    End Sub

    Private Sub replace_mix_layer(ByVal mix_texture_Id As Integer, ByVal size As Integer)
        'regenerates the mix map image and blurs it so it doesn't look blocky
        Dim texID As Int32
        texID = Ilu.iluGenImage() ' Generation of one image name
        Il.ilBindImage(texID) ' Binding of image name 
        Dim success = Il.ilTexImage(size, size, 0, 4, Il.IL_BGRA, Gl.GL_UNSIGNED_BYTE, Nothing) '  Texture specification 
        Dim ptr_2 As IntPtr = Il.ilGetData()
        Gl.glReadBuffer(Gl.GL_BACK)
        Gl.glReadPixels(0, 0, size, size, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, ptr_2)



        success = Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes


        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, mix_texture_Id)
        Dim e = Gl.glGetError
        If largestAnsio > 0 Then
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, largestAnsio)
        End If

        Dim width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
        Dim heigth = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)

        Dim type = Gl.GL_NEAREST
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, type)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, type)

        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)

        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
           Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
           Il.ilGetData()) '  Texture specification 

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Il.ilBindImage(0)
        Il.ilDeleteImages(1, texID)
        ' has to run on vertical and horz
        'frmShowImage.draw_texture(mix_texture_Id)
        For i = 1 To 2
            mix_texture_Id = blur_image(mix_texture_Id, "vert", True)
            mix_texture_Id = blur_image(mix_texture_Id, "horz", True)
        Next


    End Sub

    Public Function blur_image(ByRef image As Integer, orientatin As String, filter As Boolean) As Integer

        Dim uc As vect2
        Dim lc As vect2
        frmMain.pb2.Location = New Point(0, 0)
        'frmMain.pb2.BringToFront()
        'frmMain.pb2.Visible = True

        lc.x = frmMain.pb2.Width
        lc.y = -frmMain.pb2.Height  ' top to bottom is negitive
        uc.x = 0.0
        uc.y = 0.0
        ResizeGL_2(lc.x, Abs(lc.y))
        'ViewPerspective_2()
        ViewOrtho_2()
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        Gl.glDisable(Gl.GL_LIGHTING)

        Dim texMap_adress, scale_adress As Integer
        scale_adress = Gl.glGetUniformLocation(shader_list.gaussian_shader, "blurScale")
        texMap_adress = Gl.glGetUniformLocation(shader_list.gaussian_shader, "s_texture")

        Gl.glUseProgram(shader_list.gaussian_shader)
        Gl.glUniform1i(texMap_adress, 0) 'texture map address

        If orientatin = "vert" Then
            Gl.glUniform3f(scale_adress, 1.0 \ frmMain.pb2.Width, 0.0, 0.0)
        Else
            Gl.glUniform3f(scale_adress, 0.0, 1.0 \ frmMain.pb2.Height, 0.0)
        End If

        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, image)
        Dim e = Gl.glGetError

        Gl.glBegin(Gl.GL_QUADS)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex2f(uc.x, lc.y)

        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex2f(uc.x, uc.y)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex2f(lc.x, uc.y)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex2f(lc.x, lc.y)

        Gl.glEnd()
        Gl.glUseProgram(0)

        '--------------------
        If filter Then
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
        Else
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)

        End If
        Gl.glCopyTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, 0, 0, frmMain.pb2.Width, frmMain.pb2.Height, 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        Return image
    End Function

#End Region

End Module
