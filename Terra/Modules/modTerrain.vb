﻿Imports System.IO
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
Module modTerrain
    Public get_main_texture As Boolean = False
    Public mesh(0) As vertex_data
    Public triangle_holder As New mappedFile_
    Public Cursor_point As Vector3D
    Public Structure t_holder_
        Dim v As vertex_data
        Dim mesh_location As Integer
    End Structure
#Region "Layer buidlng functions"


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

    Public Function get_layers(ByVal ck As Ionic.Zip.ZipFile, map As Integer) As Integer
        Dim layer_count As Integer = 0
        ReDim map_layers(map).layers(4)
        map_layers(map).used_layers = 0
        map_layers(map).layers(1).text_id = dummy_texture 'preset these to the dummys so they are not empty.
        map_layers(map).layers(2).text_id = dummy_texture
        map_layers(map).layers(3).text_id = dummy_texture
        map_layers(map).layers(4).text_id = dummy_texture

        map_layers(map).layers(1).norm_id = dummy_texture
        map_layers(map).layers(2).norm_id = dummy_texture
        map_layers(map).layers(3).norm_id = dummy_texture
        map_layers(map).layers(4).norm_id = dummy_texture

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
        make_mix_texture_id(map, map_layers(map).mix_image)
        map_layers(map).mix_image = Nothing
        GC.Collect()
        'frmMain.PictureBox1.Image = map_layers(map).mix_image.Clone
        'Application.DoEvents()
        'Application.DoEvents()
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
        '------------------------------------------
        map_layers(map).layers(layer).Mix_data = b.ReadBytes(16384)

        'there should always be a layer 1 and at this point we need to create the mix value bitmap
        If layer = 1 Then
            map_layers(map).mix_image = New Bitmap(256, 256, Imaging.PixelFormat.Format32bppArgb)
        End If
        GoTo set_mask

        '--------------------------------------------------------------------------------------------------------------
        '--------------------------------------------------------------------------------------------------------------
        '--------------------------------------------------------------------------------------------------------------


        b.ReadInt32() 'read off
        'read uv mapping. The next 8 bytes are actually 2 vect4 numbers. We want the X and the Z from these
        'read unused
        If map = 42 Then
            'Stop
        End If

        For i = 0 To 11 ' read off unknown stuff
            b.ReadUInt32()
        Next
        Dim u, v As vect4
        u.x = b.ReadSingle
        u.y = b.ReadSingle
        u.z = b.ReadSingle
        u.w = b.ReadSingle
        v.x = b.ReadSingle
        v.y = b.ReadSingle
        v.z = b.ReadSingle
        v.w = b.ReadSingle
        ' trial shit
        map_layers(map).layers(layer).uP = u
        map_layers(map).layers(layer).vP = v
        'Debug.Write("map" + map.ToString + " layer:" + layer.ToString + " U:" + _
        '				  map_layers(map).layers(layer).u.ToString + " V:" + map_layers(map).layers(layer).v.ToString + vbCrLf)
        'Dim sx = Sign(u.x) * Sqrt((u.x ^ 2) + (u.z ^ 2))
        'Dim sy = Sign(v.y) * Sqrt((v.x ^ 2) + (v.z ^ 2))
        'Dim rt = Atan2(v.x, v.z)

        Dim su = "  u.x:" + u.x.ToString + "  u.y:" + u.y.ToString + "  u.z:" + u.z.ToString + "  u.w:" + u.w.ToString
        Dim sv = "  v.x:" + v.x.ToString + "  v.y:" + v.y.ToString + "  v.z:" + v.z.ToString + "  v.w:" + v.w.ToString
        'Dim sc = "scale x:" + sx.ToString + "   scale y:" + sy.ToString
        'Dim rs = "rotation:" + rt.ToString
        layer_uv_list += su + vbCrLf + sv + vbCrLf + vbCrLf


        'If map = 65 Then
        '	Stop
        'End If



        b.ReadInt32() ' read version number.. unused but = 2
        Dim textcount As Integer = b.ReadInt32  'get num texture names
        b.ReadInt64() 'read off a total of 8 padding bytes

        Dim map_name As String = b.ReadString.Replace(Chr(0), "")
        map_layers(map).layers(layer).l_name = map_name 'get the filename
        b.ReadByte()
        b.ReadByte()
        b.ReadByte()
        'stupid hack to fix missing normal_map in layer data.
        If map_name.Contains("Farmland_A_4") And textcount = 0 Then
            map_layers(map).layers(layer).n_name = "maps/landscape/00_AllTiles/Farmland_A_4_NM."
            GoTo set_Mask
        End If
        If textcount > 0 Then
            Dim normal_name As String = b.ReadString.Replace(Chr(0), "") ' this is the normal map name
            map_layers(map).layers(layer).n_name = normal_name
            b.ReadByte()
            b.ReadByte()
            b.ReadByte()


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
            If Not map_name.ToLower.ToLower.Contains("global_am") Then
                frmMapInfo.I__Map_Textures_tb.Text += "No Normal Map in layer data.  Map:" + map.ToString + "   Layer:" + layer.ToString + vbCrLf 'save info

            End If

        End If
        get_layer_mix(map, layer)
        ms.Dispose()

    End Sub

    Public Sub set_layer_2(ByVal map As Int32)
        Dim cnt As UInt32 = 0
        Dim i As UInt32 = 0
        Dim rect As New Rectangle(0, 0, map_layers(map).mix_image.Width, map_layers(map).mix_image.Height)
        Dim bmpData As System.Drawing.Imaging.BitmapData = map_layers(map).mix_image.LockBits(rect, _
             Drawing.Imaging.ImageLockMode.ReadWrite, map_layers(map).mix_image.PixelFormat)

        ' Get the address of the first line. 
        Dim ptr As IntPtr = bmpData.Scan0

        Dim bbytes As Integer = Math.Abs(bmpData.Stride) * map_layers(map).mix_image.Height
        Dim rgbValues(bbytes - 1) As Byte

        ' Copy the RGB values into the array.
        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bbytes)

        ' Set the color byte based on its layer. A, R, G and B
        For i = 0 To (bbytes / 4) - 1
            rgbValues((i * 4) + 2) = 188
        Next
        ' Copy the RGB values back to the bitmap
        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bbytes)

        ' Unlock the bits.
        map_layers(map).mix_image.UnlockBits(bmpData)

        Return
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
        Dim cnt = map_layer_cache.Length
        Dim map_name As String = map_layers(map).layers(layer).l_name

        'Debug.WriteLine(map.ToString + " " + map_name)

        For i = 0 To cnt - 1
            If map_layer_cache(i).name = map_name Then
                map_layers(map).layers(layer).text_id = map_layer_cache(i).textureID
                saved_texture_loads += 1
                Return False
            End If
        Next
        Dim dds As New MemoryStream
        'map_layers(map).layers(layer).image = 
        '================= get color map
        'if get_main_texture is false, it wont read the huge main texture again!
        If map_name.ToLower.Contains("global_am") Then
            cnt = map_layer_cache.Length
            ReDim Preserve map_layer_cache(cnt)
            map_layer_cache(cnt - 1) = New tree_textures_
            map_layer_cache(cnt - 1).name = map_name
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
            build_layer_color_texture(map, dds, layer)        'We dont need the Bitmap. This saves some time
            dds.Dispose()
        End If

        frmMapInfo.I__Map_Textures_tb.Text += "Color: " + map_name + vbCrLf 'save info

        'update the cache
        cnt = map_layer_cache.Length
        ReDim Preserve map_layer_cache(cnt)
        map_layer_cache(cnt - 1) = New tree_textures_
        map_layer_cache(cnt - 1).name = map_name
        map_layer_cache(cnt - 1).textureID = map_layers(map).layers(layer).text_id
        Return False
    End Function

    Public Function get_layer_normal_image(map As Integer, layer As Integer) As Boolean
        Dim cnt = map_layer_cache.Length
        Dim normal_update As Boolean = True
        map_layers(map).layers(layer).n_name = map_layers(map).layers(layer).l_name.Replace("_AM", "_NM")
        Dim n_map_name As String = map_layers(map).layers(layer).n_name
        'Debug.WriteLine(map.ToString + " " + n_map_name)

        'check if this texture already exists
        If frmMain.m_high_rez_Terrain.Checked Then 'only if we are using bumps
            'check if this texture already exists
            cnt = normalMap_layer_cache.Length
            n_map_name = map_layers(map).layers(layer).n_name
            For i = 0 To cnt - 1
                If normalMap_layer_cache(i).normalname = n_map_name Then
                    map_layers(map).layers(layer).norm_id = normalMap_layer_cache(i).textureNormID
                    saved_texture_loads += 1
                    Return False
                End If
            Next
        End If
        Dim retry As Boolean = False
try_again:
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
                    If normalMap_layer_cache(i).normalname = n_map_name Then
                        map_layers(map).layers(layer).norm_id = normalMap_layer_cache(i).textureNormID
                        saved_texture_loads += 1
                        Return False
                    End If
                Next
                GoTo try_again
            End If
        End If
        Dim mdds As New MemoryStream
        nmap_entry.Extract(mdds)
        build_layer_normal_texture(map, mdds, layer)
        mdds.Dispose()

        frmMapInfo.I__Map_Textures_tb.Text += "Normal: " + n_map_name + vbCrLf 'save info

        'update the cache
        cnt = normalMap_layer_cache.Length
        ReDim Preserve normalMap_layer_cache(cnt)
        normalMap_layer_cache(cnt - 1) = New tree_textures_
        normalMap_layer_cache(cnt - 1).normalname = n_map_name
        normalMap_layer_cache(cnt - 1).textureNormID = map_layers(map).layers(layer).norm_id

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

    Public Sub create_mixMaps()
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
        draw_mixmap_quads(w, psize)



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

    Private Sub draw_mixmap_quads(ByVal w As Integer, ByVal size As Integer)
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
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(cnt).mix_texture_Id)
                Gl.glBegin(Gl.GL_QUADS)
                Dim border As Integer = (size - 256) / 2
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
                replace_mix_layer(map_layers(cnt).mix_texture_Id, size)
                '---------
                'cnt has to be on outside of "off" loop
                frmMain.ProgressBar1.Value = cnt
                cnt += 1
            Next
        Next

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

    End Sub
    Private Sub replace_mix_layer(ByVal mix_texture_Id As Integer, ByVal size As Integer)
        Dim texID As Int32
        texID = Ilu.iluGenImage() ' Generation of one image name
        Il.ilBindImage(texID) ' Binding of image name 
        Dim success = Il.ilTexImage(size, size, 0, 4, Il.IL_BGRA, Gl.GL_UNSIGNED_BYTE, Nothing) '  Texture specification 
        Dim ptr_2 As IntPtr = Il.ilGetData()
        Gl.glReadBuffer(Gl.GL_BACK)
        Gl.glReadPixels(0, 0, size, size, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, ptr_2)



        success = Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes


        'Ilu.iluFlipImage()
        'Ilu.iluMirror()
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, mix_texture_Id)
        Dim e = Gl.glGetError
        If largestAnsio > 0 Then
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, largestAnsio)
        End If
        Dim width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
        Dim heigth = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)

        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)

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
        mix_texture_Id = blur_image(mix_texture_Id, "vert", True)
        mix_texture_Id = blur_image(mix_texture_Id, "horz", True)


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
    Public Sub get_map_extremes()
        Dim x, y As Integer
        map_x_max = -10000
        map_x_min = 10000
        map_y_max = -10000
        map_y_min = 10000
        For i = 0 To maplist.Length - 2
            Dim a = maplist(i).name.ToCharArray
            If a(0) = "f" Then
                If AscW(a(3)) < 97 Then a(3) = ChrW(AscW(a(3)) + 39)
                x = ((AscW("f") - AscW(a(3))) * 100.0) + 50.0
            Else
                If a(0) = "0" Then
                    x = ((AscW(a(3)) - AscW("0")) * -100.0) - 50.0
                End If
            End If
            If a(4) = "f" Then
                If AscW(a(7)) < 97 Then a(7) = ChrW(AscW(a(7)) + 39)
                y = (((AscW("f") - AscW(a(7))) * -100.0) - 50)
            Else
                If a(4) = "0" Then
                    y = (((AscW(a(7)) - AscW("0")) * 100.0) + 50)
                End If
            End If
            If x > map_x_max Then
                map_x_max = x
            End If
            If x < map_x_min Then
                map_x_min = x
            End If
            If y > map_y_max Then
                map_y_max = y
            End If
            If y < map_y_min Then
                map_y_min = y
            End If
        Next
        map_center_offset_x = -(CInt(MAP_BB_BL.x + 0.78) - CInt(map_x_min))
        map_center_offset_y = (CInt(MAP_BB_BL.y + 0.78) - CInt(map_y_min))
        'If map_center_offset_x > 0 Then
        '    map_center_offset_x -= 49
        'Else
        '    map_center_offset_x += 49
        'End If
        'If map_center_offset_y > 0 Then
        '    map_center_offset_y -= 49
        'Else
        '    map_center_offset_y += 49
        'End If
    End Sub
#End Region

    Public Sub get_location(ByVal map As Integer)
        'Creates the mapBoard array and figures out where each chunk is
        'located based on its name. 
        Dim x, y As Integer
        Dim mod_ = (Sqrt(maplist.Length - 1)) And 1
        Dim offset As Integer = Sqrt(maplist.Length - 1) / 2
        If JUST_MAP_NAME.Contains("101_") Then
            mod_ = 0
        End If
        'This routine gets the maps location in the world grid from its name
        Dim a = maplist(map).name.ToCharArray
        If a(0) = "f" Then
            If AscW(a(3)) < 97 Then a(3) = ChrW(AscW(a(3)) + 39)
            x = AscW("f") - AscW(a(3))  '+ 1
            maplist(map).location.x = ((AscW("f") - AscW(a(3))) * 100.0) + 50.0
        Else
            If a(0) = "0" Then
                x = AscW(a(3)) - AscW("0") + 1
                maplist(map).location.x = ((AscW(a(3)) - AscW("0")) * -100.0) - 50.0
                x *= -1
            End If
        End If
        If a(4) = "f" Then
            If AscW(a(7)) < 97 Then a(7) = ChrW(AscW(a(7)) + 39)
            y = AscW("f") - AscW(a(7))  '+ 1
            maplist(map).location.y = ((AscW("f") - AscW(a(7))) * -100.0) - 50
            y *= -1
        Else
            If a(4) = "0" Then
                y = AscW(a(7)) - AscW("0") + 1
                maplist(map).location.y = ((AscW(a(7)) - AscW("0")) * 100.0) + 50
            End If
        End If
        Try
            'mapBoard(x + offset + mod_, y + offset + mod_) = map
            mapBoard(x + offset + mod_, y + offset) = map
        Catch ex As Exception

        End Try


    End Sub

#Region "Terra Tarrain creation functions"
    Dim vert_cnt As Integer = 0

    Public Sub make_chunk_meshes()
        tri_count = 0
        Dim bm_w As Integer = Sqrt(maplist.Length - 1)
        Dim mesh_stride As Integer = bm_w * 64
        Dim half_w As Integer = bm_w / 2
        Dim p1 As vertex_data
        Dim loc As Integer
        Dim v_step As Integer
        Dim map As Integer
        Dim uv As vect2
        Dim cnt As Integer = 0
        For j = 0 To bm_w - 2
            Application.DoEvents()
            v_step = j * mesh_stride * 64
            For k As Integer = 0 To mesh_stride - 64 Step 64
                If k > 64 * stride Then
                    'Exit For
                End If
                For v2 = 0 To 63
                    Dim y = (v2 * mesh_stride) + v_step
                    For x1 = k To k + 63
                        If v2 = 0 And x1 = k Then
                            loc = (x1 + 4) + y + (mesh_stride * 4)
                            p1 = mesh(loc)
                            map = p1.map
                            Gl.glDeleteLists(maplist(map).calllist_Id, 1)
                            Dim id = Gl.glGenLists(1)
                            maplist(map).calllist_Id = id
                            Gl.glNewList(id, Gl.GL_COMPILE)
                            Gl.glBegin(Gl.GL_TRIANGLES)
                            If y > (((bm_w - 1) * 64) * mesh_stride) + 1 Then
                                Exit For
                            End If
                        End If
                        If x1 = mesh_stride - 1 Then
                            Exit For
                        End If
                        uv.x = -(x1 - k)
                        uv.y = -v2
                        process_verts(x1, y, uv, mesh_stride, map)
                    Next x1
                Next v2
                Gl.glEnd()
                Gl.glEndList()
            Next k
        Next j
        'sucks but i need to deal with the last row
        v_step = (bm_w - 1) * mesh_stride * 64
        For k As Integer = 0 To mesh_stride - 63 Step 64
            For v2 = 0 To 62
                Dim y = (v2 * mesh_stride) + v_step
                For x1 = k To k + 63
                    If v2 = 0 And x1 = k Then
                        'this is where we will create the new display list if I EVER get this working!!!
                        loc = (x1 + 4) + y + (mesh_stride * 4)
                        p1 = mesh(loc)
                        map = p1.map
                        Gl.glDeleteLists(maplist(map).calllist_Id, 1)
                        Dim id = Gl.glGenLists(1)
                        maplist(map).calllist_Id = id
                        Gl.glNewList(id, Gl.GL_COMPILE)
                        Gl.glBegin(Gl.GL_TRIANGLES)
                        If y > (((bm_w - 1) * 64) * mesh_stride) + 1 Then
                            Exit For
                        End If
                    End If
                    If x1 = mesh_stride - 1 Then
                        Exit For
                    End If
                    uv.x = -(x1 - k)
                    uv.y = -v2
                    process_verts(x1, y, uv, mesh_stride, map)
                Next x1
            Next v2
            Gl.glEnd()
            Gl.glEndList()
        Next
    End Sub
    Private Sub process_verts(ByVal x As Integer, ByVal y As Integer, ByVal uv As vect2, ByVal mesh_stride As Integer, ByVal map As Integer)
        Dim p1, p2, p3, p4 As vertex_data
        Dim l1, l2, l3, l4 As Integer
        l1 = x + 0 + y
        l2 = x + 1 + y
        l3 = x + 0 + y + mesh_stride
        l4 = x + 1 + y + mesh_stride
        Dim u, v As Integer
        u = Abs(uv.x)
        v = Abs(uv.y)
        'h1 = Convert.ToSingle(maplist(map).holes(u, v))
        'h2 = Convert.ToSingle(maplist(map).holes(u + 1, v))
        'h3 = Convert.ToSingle(maplist(map).holes(u, v + 1))
        'h4 = Convert.ToSingle(maplist(map).holes(u + 1, v + 1))
        Dim p1_u, p1_v As Single
        Dim p2_u, p2_v As Single
        Dim p3_u, p3_v As Single
        Dim p4_u, p4_v As Single
        p1_u = (uv.x / 64.0) * 10.0
        p1_v = (uv.y / 64.0) * 10.0
        Dim uv_off As Single = -(1.0 / 64.0) * 10.0
        p2_u = p1_u + uv_off
        p2_v = p1_v
        p3_u = p1_u
        p3_v = p1_v + uv_off
        p4_u = p2_u
        p4_v = p3_v

        p1 = mesh(l1)
        p2 = mesh(l2)
        p3 = mesh(l3)
        p4 = mesh(l4)
        If p1.u = -9.84375 Then
            p2.u = -10.0
            p4.u = -10.0
        End If
        If p1.v = -9.84375 Then
            p3.v = -10.0
            p4.v = -10.0
        End If
        'If uv.y = -63 Then
        '    Stop
        'End If
        Gl.glNormal3f(p1.nx, p1.ny, p1.nz)
        Gl.glMultiTexCoord3f(0, p1_u, p1_v, p1.hole) ' hole flag in z of glMultiTexCoord3f
        Gl.glMultiTexCoord3f(2, p1.t.x, p1.t.y, p1.t.z)
        Gl.glMultiTexCoord3f(3, p1.bt.x, p1.bt.y, p1.bt.z)
        'Gl.glMultiTexCoord2f(4, p1.fog_uv.x, p1.fog_uv.y)
        Gl.glVertex3f(p1.x, p1.y, p1.z)

        Gl.glNormal3f(p3.nx, p3.ny, p3.nz)
        Gl.glMultiTexCoord3f(0, p3_u, p3_v, p3.hole)
        Gl.glMultiTexCoord3f(2, p3.t.x, p3.t.y, p3.t.z)
        Gl.glMultiTexCoord3f(3, p3.bt.x, p3.bt.y, p3.bt.z)
        'Gl.glMultiTexCoord2f(4, p3.fog_uv.x, p3.fog_uv.y)
        Gl.glVertex3f(p3.x, p3.y, p3.z)

        Gl.glNormal3f(p2.nx, p2.ny, p2.nz)
        Gl.glMultiTexCoord3f(0, p2_u, p2_v, p2.hole)
        Gl.glMultiTexCoord3f(2, p2.t.x, p2.t.y, p2.t.z)
        Gl.glMultiTexCoord3f(3, p2.bt.x, p2.bt.y, p2.bt.z)
        'Gl.glMultiTexCoord2f(4, p2.fog_uv.x, p2.fog_uv.y)
        Gl.glVertex3f(p2.x, p2.y, p2.z)
        '===============================
        Gl.glNormal3f(p3.nx, p3.ny, p3.nz)
        Gl.glMultiTexCoord3f(0, p3_u, p3_v, p3.hole)
        Gl.glMultiTexCoord3f(2, p3.t.x, p3.t.y, p3.t.z)
        Gl.glMultiTexCoord3f(3, p3.bt.x, p3.bt.y, p3.bt.z)
        'Gl.glMultiTexCoord2f(4, p3.fog_uv.x, p3.fog_uv.y)
        Gl.glVertex3f(p3.x, p3.y, p3.z)

        Gl.glNormal3f(p4.nx, p4.ny, p4.nz)
        Gl.glMultiTexCoord3f(0, p4_u, p4_v, p4.hole)
        Gl.glMultiTexCoord3f(2, p4.t.x, p4.t.y, p4.t.z)
        Gl.glMultiTexCoord3f(3, p4.bt.x, p4.bt.y, p4.bt.z)
        'Gl.glMultiTexCoord2f(4, p4.fog_uv.x, p4.fog_uv.y)
        Gl.glVertex3f(p4.x, p4.y, p4.z)

        Gl.glNormal3f(p2.nx, p2.ny, p2.nz)
        Gl.glMultiTexCoord3f(0, p2_u, p2_v, p2.hole)
        Gl.glMultiTexCoord3f(2, p2.t.x, p2.t.y, p2.t.z)
        Gl.glMultiTexCoord3f(3, p2.bt.x, p2.bt.y, p2.bt.z)
        'Gl.glMultiTexCoord2f(4, p2.fog_uv.x, p2.fog_uv.y)
        Gl.glVertex3f(p2.x, p2.y, p2.z)
    End Sub


    Private Sub store_in_mesh(ByVal v As vertex_data, ByVal map As Integer)
        If map = 41 Then
            'Stop
        End If
        Dim v_copy As vertex_data = v
        v_copy.z = v.y
        v_copy.y = v.z
        Dim a, b As Single
        Dim loc = maplist(map).location
        Dim total_width = Sqrt(maplist.Length - 1) * 100.0
        Dim map_offset = ((Sqrt(maplist.Length - 1) / 2)) * 100.0
        If map_odd Then
            v.x += 50.0
            v.y -= 50.0
        End If
        Dim w = Sqrt(maplist.Length - 1)
        If w / 2 = 7.5 Then
            v.y += 100.0
        End If
        'Just shift it and rescale and use it as the location to write in the mesh() array.
        'It works well with this data.
        Dim xu = v.x + map_offset
        Dim yu = v.y + map_offset
        Dim x = (v.x + map_offset) * 0.64
        Dim y = (v.y + map_offset) * 0.64
        Dim vy = y * w * 64 ' always 64 locations on x,y in a chunk
        Dim abs_loc = x + vy
        a = v.z
        b = v.y
        v.y = a
        v.z = b
        'If map_odd Then
        '    If w / 2 = 7.5 Then
        '        v.z -= 100.0
        '    End If
        '    v.x -= 50.0
        '    v.z += 50.0
        'End If
        mesh(abs_loc) = v_copy
    End Sub

    Private Sub get_translated_bb_terrain(ByRef BB() As vect3, ByVal map As Integer)
        Dim v1, v2, v3, v4, v5, v6, v7, v8 As vect3
        With maplist(map)
            v1.z = .BB_Max.z : v2.z = .BB_Max.z : v3.z = .BB_Max.z : v4.z = .BB_Max.z
            v5.z = .BB_Min.z : v6.z = .BB_Min.z : v7.z = .BB_Min.z : v8.z = .BB_Min.z

            v1.x = .BB_Min.x : v6.x = .BB_Min.x : v7.x = .BB_Min.x : v4.x = .BB_Min.x
            v5.x = .BB_Max.x : v8.x = .BB_Max.x : v3.x = .BB_Max.x : v2.x = .BB_Max.x

            v4.y = .BB_Max.y : v7.y = .BB_Max.y : v8.y = .BB_Max.y : v3.y = .BB_Max.y
            v6.y = .BB_Min.y : v5.y = .BB_Min.y : v1.y = .BB_Min.y : v2.y = .BB_Min.y
        End With

        BB(0) = v1
        BB(1) = v2
        BB(2) = v3
        BB(3) = v4
        BB(4) = v5
        BB(5) = v6
        BB(6) = v7
        BB(7) = v8


    End Sub
    Public Sub build_terra(ByVal map As Int32)

        'good as place as any to set bounding box
        maplist(map).BB_Max.x = maplist(map).location.x + 50
        maplist(map).BB_Min.x = maplist(map).location.x - 50
        maplist(map).BB_Max.z = maplist(map).location.y + 50
        maplist(map).BB_Min.z = maplist(map).location.y - 50
        get_translated_bb_terrain(maplist(map).BB, map)

        Dim w As UInt32 = heightMapSize 'bmp_w
        Dim h As UInt32 = heightMapSize 'bmp_h
        Dim uvScale = (1.0# / 64.0#)
        Dim w_ = w / 2.0#
        Dim h_ = h / 2.0#
        Dim scale = 100.0 / (64.0#)
        Dim cnt As UInt32 = 0
        For j = 0 To w - 2
            For i = 0 To h - 2
                cnt += 1

                midx += (i - w_)
                midy += (j - h_)
                midz += (bmp_data((i), (j)))

                topleft.x = (i) - w_
                topleft.y = (j) - h_
                topleft.z = bmp_data((i), (j))
                topleft.u = (i) * uvScale
                topleft.v = (j) * uvScale
                topleft.hole = maplist(map).holes(i, j)

                topRight.x = (i + 1) - w_
                topRight.y = (j) - h_
                topRight.z = bmp_data((i + 1), (j))
                topRight.u = (i + 1) * uvScale
                topRight.v = (j) * uvScale
                topRight.hole = maplist(map).holes(i, j)

                bottomRight.x = (i + 1) - w_
                bottomRight.y = (j + 1) - h_
                bottomRight.z = bmp_data((i + 1), (j + 1))
                bottomRight.u = (i + 1) * uvScale
                bottomRight.v = (j + 1) * uvScale
                bottomRight.hole = maplist(map).holes(i, j)

                bottomleft.x = (i) - w_
                bottomleft.y = (j + 1) - h_
                bottomleft.z = bmp_data((i), (j + 1))
                bottomleft.u = (i) * uvScale
                bottomleft.v = (j + 1) * uvScale
                bottomleft.hole = maplist(map).holes(i, j)

                make_world_triangle(topRight, bottomRight, topleft, scale, map)
                make_world_triangle(topleft, bottomRight, bottomleft, scale, map)
            Next
        Next

    End Sub
    Public Sub make_world_triangle(ByVal vt1 As vertex_data, ByVal vt2 As vertex_data, ByVal vt3 As vertex_data, ByRef scale As Single, ByVal map As Int32)
        tri_count += 1
        'add offsets
        vt1.map = map
        vt2.map = map
        vt3.map = map

        vt1.x = (vt1.x * scale) + maplist(map).location.x
        vt1.y = (vt1.y * scale) + maplist(map).location.y
        vt2.x = (vt2.x * scale) + maplist(map).location.x
        vt2.y = (vt2.y * scale) + maplist(map).location.y
        vt3.x = (vt3.x * scale) + maplist(map).location.x
        vt3.y = (vt3.y * scale) + maplist(map).location.y

        vt1.u *= -10.0
        vt1.v *= -10.0
        vt2.u *= -10.0
        vt2.v *= -10.0
        vt3.u *= -10.0
        vt3.v *= -10.0
        'store for reworking
        store_in_mesh(vt1, map)
        store_in_mesh(vt2, map)
        store_in_mesh(vt3, map)


    End Sub

    Private Sub make_strip_triangle(ByVal vt1 As vertex_data, ByVal vt2 As vertex_data, ByVal vt3 As vertex_data, ByVal map As Integer)
        tri_count += 1
        vt1.map = map
        vt2.map = map
        vt3.map = map
        'add offsets
        Dim a, b, n As vect3
        'Dim tangent, biTangent As vect3
        ' ComputeTangentBasis(vt1, vt2, vt3, tangent, biTangent)
        Dim h1, h2, h3 As Single
        h1 = vt1.hole
        h2 = vt1.hole
        h3 = vt3.hole
        a.x = vt1.x - vt2.x
        a.y = vt1.y - vt2.y
        a.z = vt1.z - vt2.z
        b.x = vt2.x - vt3.x
        b.y = vt2.y - vt3.y
        b.z = vt2.z - vt3.z
        n.x = (a.y * b.z) - (a.z * b.y)
        n.y = (a.z * b.x) - (a.x * b.z)
        n.z = (a.x * b.y) - (a.y * b.x)
        Dim len As Single = Sqrt((n.x * n.x) + (n.y * n.y) + (n.z * n.z))
        If len = 0 Then len = 1.0 ' no divide by zero
        n.x /= len
        n.y /= len
        n.z /= len

        vt1.u *= -10.0
        vt1.v *= -10.0
        vt2.u *= -10.0
        vt2.v *= -10.0
        vt3.u *= -10.0
        vt3.v *= -10.0

        'store for reworking
        'store_in_mesh(vt1, map)
        'store_in_mesh(vt2, map)
        'store_in_mesh(vt3, map)

        Gl.glNormal3f(n.x, n.z, n.y)
        Gl.glTexCoord3f(vt1.u, vt1.v, h1)
        'Gl.glMultiTexCoord3f(1, tangent.x, tangent.y, tangent.z)
        'Gl.glMultiTexCoord3f(2, biTangent.x, biTangent.y, biTangent.z)
        Gl.glVertex3f(vt1.x, vt1.z, vt1.y)

        Gl.glNormal3f(n.x, n.z, n.y)
        Gl.glTexCoord3f(vt2.u, vt2.v, h2)
        'Gl.glMultiTexCoord3f(1, tangent.x, tangent.y, tangent.z)
        'Gl.glMultiTexCoord3f(2, biTangent.x, biTangent.y, biTangent.z)
        Gl.glVertex3f(vt2.x, vt2.z, vt2.y)

        Gl.glNormal3f(n.x, n.z, n.y)
        Gl.glTexCoord3f(vt3.u, vt3.v, h3)
        'Gl.glMultiTexCoord3f(1, tangent.x, tangent.y, tangent.z)
        'Gl.glMultiTexCoord3f(2, biTangent.x, biTangent.y, biTangent.z)
        Gl.glVertex3f(vt3.x, vt3.z, vt3.y)





    End Sub
    Public Sub seam_map()
        Dim scale As Double = 100.0# / (64.0#)
        Dim uvinc As Double = 1.0# / 64.0#
        Dim u_start As Double = uvinc * 63.0#
        Dim almost1 As Double = 1.0
        Dim u_end As Double = almost1
        Dim uleft As Double = 0.0
        Dim vtop As Double = 0.0
        Dim v_start As Double = u_start
        Dim v_end As Double = 1.0
        Dim cnt As Integer = 0
        Dim y_pos As Integer = 0
        Dim x_pos As Integer = 0
        Dim yu, yl, xu, xl As Single
        Dim tl, bl, tr, br, cur_x, cur_y As Single
        Dim mmx, mmy As Single
        Dim mcolumn As Integer = 0
        Dim mod_ = (Sqrt(maplist.Length - 1)) And 1

        For mboardX = 0 To (Sqrt(maplist.Length - 1) - 1) '+ mod_
            For mboardy = 0 To (Sqrt(maplist.Length - 1)) '+ mod_
                mmy = mboardy
                If mboardy = 0 Then
                    GoTo endx
                End If
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                maplist(mapBoard(mboardX, mboardy)).seamCallId = Gl.glGenLists(1)
                Gl.glNewList(maplist(mapBoard(mboardX, mboardy)).seamCallId, Gl.GL_COMPILE)
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Gl.glBegin(Gl.GL_TRIANGLES)
                yu = maplist(mapBoard(mboardX, mboardy)).location.y + 50
                yl = yu - (1.0# * scale)
                x_pos = 0.0#
                mmx = mboardX
                If mmx = 7 Then
                    'Stop
                End If
                If mboardy > (Sqrt(maplist.Length - 1) - 1) Then
                    GoTo endx
                End If
                u_start = 0
                'Debug.WriteLine(mapBoard(mmx, mmy))
                'Debug.WriteLine(mapBoard(mmx + 1, mmy))
                For x1 = maplist(mapBoard(mboardX, mboardy)).location.x - 50 To _
                                            maplist(mapBoard(mboardX, mboardy)).location.x + 50 - (scale * 2) Step 1 * scale
                    tl = maplist(mapBoard(mboardX, mboardy + 1)).heights(x_pos, 0)
                    tr = maplist(mapBoard(mboardX, mboardy + 1)).heights(x_pos + 1, 0)
                    bl = maplist(mapBoard(mboardX, mboardy)).heights(x_pos, 63)
                    br = maplist(mapBoard(mboardX, mboardy)).heights(x_pos + 1, 63)
                    maplist(mapBoard(mboardX, mboardy)).heights(x_pos, 64) = tl

                    topleft.x = x1
                    topleft.y = yu
                    topleft.z = tl
                    topleft.u = u_start
                    topleft.v = almost1

                    bottomleft.x = x1
                    bottomleft.y = yl
                    bottomleft.z = bl
                    bottomleft.u = u_start
                    bottomleft.v = almost1 - uvinc

                    topRight.x = x1 + scale
                    topRight.y = yu
                    topRight.z = tr
                    topRight.u = u_start + uvinc
                    topRight.v = almost1

                    bottomRight.x = x1 + scale
                    bottomRight.y = yl
                    bottomRight.z = br
                    bottomRight.u = u_start + uvinc
                    bottomRight.v = almost1 - uvinc

                    make_strip_triangle(topRight, bottomRight, topleft, mapBoard(mboardX, mboardy))
                    make_strip_triangle(topleft, bottomRight, bottomleft, mapBoard(mboardX, mboardy))
                    u_start += uvinc
                    x_pos += 1
                    cur_x = x1
                Next
                If mmx > (Sqrt(maplist.Length - 1) - 2) Then
                    GoTo endx
                End If
                'this part does the one corner we can't loop in to
                tl = maplist(mapBoard(mmx, mmy)).heights(63, 63)
                tr = maplist(mapBoard(mmx + 1, mmy)).heights(0, 63)
                bl = maplist(mapBoard(mmx, mmy + 1)).heights(63, 0)
                br = maplist(mapBoard(mmx + 1, mmy + 1)).heights(0, 0)
                'these 3 positions was a pain to sort out :)
                maplist(mapBoard(mmx, mmy)).heights(64, 64) = br 'ok
                maplist(mapBoard(mmx, mmy)).heights(63, 64) = bl 'ok
                maplist(mapBoard(mmx, mmy)).heights(64, 63) = tr 'ok
                topleft.x = cur_x + scale
                topleft.y = yl
                topleft.z = tl
                topleft.u = almost1 - uvinc
                topleft.v = almost1 - uvinc
                topleft.hole = maplist(mapBoard(mmx, mmy)).holes(63, 63)

                topRight.x = cur_x + (scale * 2)
                topRight.y = yl
                topRight.z = tr
                topRight.u = almost1
                topRight.v = almost1 - uvinc
                topRight.hole = maplist(mapBoard(mmx, mmy)).holes(0, 63)

                bottomleft.x = cur_x + scale
                bottomleft.y = yu
                bottomleft.z = bl
                bottomleft.u = almost1 - uvinc
                bottomleft.v = almost1 '
                bottomleft.hole = maplist(mapBoard(mmx, mmy)).holes(63, 0)

                bottomRight.x = cur_x + (scale * 2)
                bottomRight.y = yu
                bottomRight.z = br
                bottomRight.u = almost1 '
                bottomRight.v = almost1
                bottomRight.hole = maplist(mapBoard(mmx, mmy)).holes(0, 0)

                make_strip_triangle(topRight, bottomRight, topleft, mapBoard(mboardX, mboardy))
                make_strip_triangle(topleft, bottomRight, bottomleft, mapBoard(mboardX, mboardy))

endx:
                If mboardy = 0 Then
                    GoTo endy
                End If
                If mmx > Sqrt(maplist.Length - 1) - 2 Then
                    mcolumn += 1
                    GoTo endy
                End If
                xu = maplist(mapBoard(mboardX, mboardy)).location.x + 50
                xl = xu - (1 * scale)
                cur_y = 0
                y_pos = 0
                'mmx = mcolumn
                v_start = 0
                'If mboardy = 8 And mmx = 0 Then
                '    Gl.glColor3f(1.0, 0.0, 0.0)
                'Else
                '    Gl.glColor3f(0.6, 0.6, 0.6)
                'End If
                For y1 = maplist(mapBoard(mmx, mboardy)).location.y - 50 To _
                          maplist(mapBoard(mmx, mboardy)).location.y + 50 - (scale * 2) Step 1 * scale
                    tl = maplist(mapBoard(mmx, mboardy)).heights(63, y_pos + 1)
                    tr = maplist(mapBoard(mmx + 1, mboardy)).heights(0, y_pos + 1)
                    bl = maplist(mapBoard(mmx, mboardy)).heights(63, y_pos)
                    br = maplist(mapBoard(mmx + 1, mboardy)).heights(0, y_pos)
                    maplist(mapBoard(mmx, mboardy)).heights(64, y_pos) = br
                    topleft.x = xl
                    topleft.y = y1 + scale
                    topleft.z = tl
                    topleft.u = almost1 - uvinc
                    topleft.v = v_start + uvinc

                    bottomleft.x = xl
                    bottomleft.y = y1
                    bottomleft.z = bl
                    bottomleft.u = almost1 - uvinc
                    bottomleft.v = v_start

                    topRight.x = xu
                    topRight.y = y1 + scale
                    topRight.z = tr
                    topRight.u = almost1
                    topRight.v = v_start + uvinc

                    bottomRight.x = xu
                    bottomRight.y = y1
                    bottomRight.z = br
                    bottomRight.u = almost1
                    bottomRight.v = v_start


                    make_strip_triangle(topRight, bottomRight, topleft, mapBoard(mboardX, mboardy))
                    make_strip_triangle(topleft, bottomRight, bottomleft, mapBoard(mboardX, mboardy))
                    v_start += uvinc
                    y_pos += 1
                    cur_y = y1
                Next
Endy:
                Gl.glEnd()
                Gl.glEndList()
                Gl.glFinish()
            Next
        Next




    End Sub

    Private loc_list(0) As l_
    Private Structure l_
        Dim cnt As Integer
        Dim list() As Integer
    End Structure

    Public Sub save_norms(ByRef a1 As vertex_data, ByRef a2 As vertex_data, ByRef a3 As vertex_data, ByRef a4 As vertex_data, ByRef a5 As vertex_data, ByRef a6 As vertex_data)
        a2.nx = a1.nx
        a2.ny = a1.ny
        a2.nz = a1.nz

        a3.nx = a1.nx
        a3.ny = a1.ny
        a3.nz = a1.nz

        a4.nx = a1.nx
        a4.ny = a1.ny
        a4.nz = a1.nz

        a5.nx = a1.nx
        a5.ny = a1.ny
        a5.nz = a1.nz

        a6.nx = a1.nx
        a6.ny = a1.ny
        a6.nz = a1.nz

    End Sub
    Public Sub average_mesh_btns()
        Dim count = triangle_count - 1
        'do the inside of the mesh
        Dim w = global_map_width
        Dim len As Single
        Dim n = ((w * 64) - 1) * 6
        Dim a1, a2, a3, a4, a5, a6 As vertex_data
        'If File.Exists("C:\!_test_data.data") Then
        '    File.Delete("C:\!_test_data.data")
        'End If
        'Dim f = File.OpenWrite("C:\!_test_data.data")
        'Dim br As New BinaryWriter(f)
        'For i = 0 To triangle_count - 1
        '    br.Write(CSng(triangle_holder(i).v.t.x))
        '    'br.Write(CSng(triangle_holder(i).v.y))
        '    br.Write(CSng(triangle_holder(i).v.t.y))

        'Next
        'br.Close()
        'f.Close()

        For i As Integer = 0 To (count - 1) - (n) Step n
            Application.DoEvents()
            For k As Integer = i To (i + n) - 12 Step 6
                'loop thru and grab the 6 vertices that share the same exact space.
                a1 = triangle_holder.v(k + 4)
                a2 = triangle_holder.v(k + 7)
                a3 = triangle_holder.v(k + 9)
                a4 = triangle_holder.v((k + n) + 2)
                a5 = triangle_holder.v((k + n) + 5)
                a6 = triangle_holder.v((k + n) + 6)
                'Average out the normals
                a1.nx = (a1.nx + a2.nx + a3.nx + a4.nx + a5.nx + a6.nx) / 6.0
                a1.ny = (a1.ny + a2.ny + a3.ny + a4.ny + a5.ny + a6.ny) / 6.0
                a1.nz = (a1.nz + a2.nz + a3.nz + a4.nz + a5.nz + a6.nz) / 6.0

                len = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
                If len = 0 Then len = 1.0 ' no divide by zero
                a1.nx /= len
                a1.ny /= len
                a1.nz /= len


                'Store the averaged normal back in to the 6 vertices
                save_norms(a1, a2, a3, a4, a5, a6)



                triangle_holder.v(k + 4) = a1
                triangle_holder.v(k + 7) = a2
                triangle_holder.v(k + 9) = a3
                triangle_holder.v((k + n) + 2) = a4
                triangle_holder.v((k + n) + 5) = a5
                triangle_holder.v((k + n) + 6) = a6


            Next
        Next
        Application.DoEvents()
        'do the right side
        For i As Integer = 0 To (count - 1) - (n) Step n
            a1 = triangle_holder.v(i + 1)
            a2 = triangle_holder.v((i + 0) + 3)
            a3 = triangle_holder.v((i + n) + 0)

            a1.nx = (a1.nx + a2.nx + a3.nx) / 3.0
            a1.ny = (a1.ny + a2.ny + a3.ny) / 3.0
            a1.nz = (a1.nz + a2.nz + a3.nz) / 3.0

            len = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
            If len = 0 Then len = 1.0 ' no divide by zero
            a1.nx /= len
            a1.ny /= len
            a1.nz /= len
            save_norms(a1, a2, a3, a4, a5, a6)

            triangle_holder.v(i + 1) = a1
            triangle_holder.v((i + 0) + 3) = a2
            triangle_holder.v((i + n) + 0) = a3


        Next
        Application.DoEvents()
        'bottom left corner
        Dim ii = n - 6

        a1 = triangle_holder.v(ii + 2)
        a2 = triangle_holder.v(ii + 5)

        a1.nx = (a1.nx + a2.nx) / 2.0
        a1.ny = (a1.ny + a2.ny) / 2.0
        a1.nz = (a1.nz + a2.nz) / 2.0

        len = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
        If len = 0 Then len = 1.0 ' no divide by zero
        a1.nx /= len
        a1.ny /= len
        a1.nz /= len

        save_norms(a1, a2, a3, a4, a5, a6)

        triangle_holder.v(ii + 2) = a1
        triangle_holder.v(ii + 5) = a2

        'top right corner
        ii = (count - n) + 1

        a1 = triangle_holder.v(ii + 1)
        a2 = triangle_holder.v(ii + 3)

        a1.nx = (a1.nx + a2.nx) / 2.0
        a1.ny = (a1.ny + a2.ny) / 2.0
        a1.nz = (a1.nz + a2.nz) / 2.0

        len = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
        If len = 0 Then len = 1.0 ' no divide by zero
        a1.nx /= len
        a1.ny /= len
        a1.nz /= len

        save_norms(a1, a2, a3, a4, a5, a6)

        triangle_holder.v(ii + 1) = a1
        triangle_holder.v(ii + 3) = a2
        Application.DoEvents()

        'do the left side
        For i As Integer = n - 6 To (count) - (n) Step n
            a1 = triangle_holder.v(i + 4)
            a2 = triangle_holder.v((i + n) + 2)
            a3 = triangle_holder.v((i + n) + 5)

            a1.nx = (a1.nx + a2.nx + a3.nx) / 3.0
            a1.ny = (a1.ny + a2.ny + a3.ny) / 3.0
            a1.nz = (a1.nz + a2.nz + a3.nz) / 3.0

            len = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
            If len = 0 Then len = 1.0 ' no divide by zero
            a1.nx /= len
            a1.ny /= len
            a1.nz /= len

            save_norms(a1, a2, a3, a4, a5, a6)

            triangle_holder.v(i + 4) = a1
            triangle_holder.v((i + n) + 2) = a2
            triangle_holder.v((i + n) + 5) = a3

        Next
        'do top row
        Application.DoEvents()
        For i As Integer = (count + 1) - n To (count + 1) - 12 Step 6
            a1 = triangle_holder.v(i + 4)
            a2 = triangle_holder.v((i + 6) + 1)
            a3 = triangle_holder.v((i + 6) + 3)

            a1.nx = (a1.nx + a2.nx + a3.nx) / 2.0
            a1.ny = (a1.ny + a2.ny + a3.ny) / 2.0
            a1.nz = (a1.nz + a2.nz + a3.nz) / 2.0

            len = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
            If len = 0 Then len = 1.0 ' no divide by zero
            a1.nx /= len
            a1.ny /= len
            a1.nz /= len

            save_norms(a1, a2, a3, a4, a5, a6)

            triangle_holder.v(i + 4) = a1
            triangle_holder.v((i + 6) + 1) = a2
            triangle_holder.v((i + 6) + 3) = a3

        Next
        Application.DoEvents()
        'do the bottom row (first row)
        For i As Integer = 0 To n - 12 Step 6
            a1 = triangle_holder.v(i + 2)
            a2 = triangle_holder.v((i + 0) + 5)
            a3 = triangle_holder.v((i + 6) + 0)

            a1.nx = (a1.nx + a2.nx + a3.nx) / 3.0
            a1.ny = (a1.ny + a2.ny + a3.ny) / 3.0
            a1.nz = (a1.nz + a2.nz + a3.nz) / 3.0

            len = Sqrt((a1.nx * a1.nx) + (a1.ny * a1.ny) + (a1.nz * a1.nz))
            If len = 0 Then len = 1.0 ' no divide by zero
            a1.nx /= len
            a1.ny /= len
            a1.nz /= len

            save_norms(a1, a2, a3, a4, a5, a6)

            triangle_holder.v(i + 2) = a1
            triangle_holder.v((i + 0) + 5) = a2
            triangle_holder.v((i + 6) + 0) = a3

        Next
        'now to put all these back in the mesh() so the chunk meshes can be created.. actually just display IDs'
        Dim local As New t_holder_
        For i = 0 To mesh.Length - 1
            mesh(i) = New vertex_data
        Next
        For i = 0 To triangle_count - 1
            check_bounds(triangle_holder.v(i)) ' get map bounding box size
            local = triangle_holder(i)
            If Not triangle_holder.mesh_location(i) >= mesh.Length Then
                mesh(triangle_holder.mesh_location(i)) = triangle_holder.v(i)
            Else
                'Stop
            End If
        Next
    End Sub
    Public Sub createTBNs()
        Dim bm_w As Integer = global_map_width
        Dim mesh_stride As Integer = (bm_w * 64)
        Dim cnt As Integer = 0

        For y1 = 0 To (mesh_stride) - 2
            For x1 = 0 To mesh_stride - 2
                get_TBN(x1, y1 * mesh_stride, mesh_stride)
            Next x1
        Next
        GC.Collect()
        GC.WaitForFullGCComplete()
    End Sub
    Private Sub get_TBN(ByVal x As Integer, ByVal y As Integer, ByVal mesh_stride As Integer)
        Dim p1, p2, p3, p4 As vertex_data
        Dim p1t, p2t, p3t, p4t As vertex_data
        Dim l1, l2, l3, l4 As Integer
        l1 = x + 0 + y
        l2 = x + 1 + y
        l3 = x + 0 + y + mesh_stride
        l4 = x + 1 + y + mesh_stride
        p1 = mesh(l1)
        p2 = mesh(l2)
        p3 = mesh(l3)
        p4 = mesh(l4)
        'so we can restore the UVs if they are changed for the math
        p1t = p1
        p2t = p2
        p3t = p3
        p4t = p4
        '10 fucking hours to figure out why the UVs where messed up. Grrrrrrrrr!!!
        'Im over-writing the vertices on every 64th column and row so the -10.0 became 0.0!!!
        'This fixes that bullshit!!! Has to be done before tangent creation!!!
        'They also can't has to be returned to their previous state.
        If p1.u = -9.84375 Then
            p2.u = -10.0
            p4.u = -10.0
        End If
        If p1.v = -9.84375 Then
            p3.v = -10.0
            p4.v = -10.0
        End If
        get_normal(p1, p3, p2)
        get_normal(p3, p4, p2)
        ComputeTangentBasis(p1, p3, p2, l1, l3, l2, p1t, p3t, p2t)
        ComputeTangentBasis(p3, p4, p2, l3, l4, l2, p3t, p4t, p2t)
        '==========================================================================

        mesh(l1) = p1
        mesh(l2) = p2
        mesh(l3) = p3
        mesh(l4) = p4
        'restore UVs
        mesh(l1).u = p1t.u
        mesh(l1).v = p1t.v
        mesh(l2).u = p2t.u
        mesh(l2).v = p2t.v
        mesh(l3).u = p3t.u
        mesh(l3).v = p3t.v
        mesh(l4).u = p4t.u
        mesh(l4).v = p4t.v
    End Sub
    Private Sub get_normal(ByRef p1 As vertex_data, ByRef p2 As vertex_data, ByRef p3 As vertex_data)

        Dim a, b, n As vect3

        a.x = p1.x - p2.x
        a.y = p1.y - p2.y
        a.z = p1.z - p2.z
        b.x = p2.x - p3.x
        b.y = p2.y - p3.y
        b.z = p2.z - p3.z
        n.x = (a.y * b.z) - (a.z * b.y)
        n.y = (a.z * b.x) - (a.x * b.z)
        n.z = (a.x * b.y) - (a.y * b.x)
        Dim len As Single = Sqrt((n.x * n.x) + (n.y * n.y) + (n.z * n.z))
        If len = 0 Then len = 1.0 ' no divide by zero
        n.x /= len
        n.y /= len
        n.z /= len

        p1.nx = n.x
        p1.ny = n.y
        p1.nz = n.z
        p2.nx = n.x
        p2.ny = n.y
        p2.nz = n.z
        p3.nx = n.x
        p3.ny = n.y
        p3.nz = n.z
    End Sub
    Private Sub ComputeTangentBasis(ByRef p0 As vertex_data, ByRef p1 As vertex_data, ByRef p2 As vertex_data, _
                           ByVal l1 As Integer, ByVal l2 As Integer, ByVal l3 As Integer, _
                           ByVal pt0 As vertex_data, ByVal pt1 As vertex_data, ByVal pt2 As vertex_data)

        Dim tangent, bitangent As Vector3D
        Dim n As Vector3D

        n.X = p0.nx
        n.Y = p0.ny
        n.Z = p0.nz

        'convert to vector3d type... they are WAY easier to do complex math with!!
        Dim v0 = convert_vector3d(p0)
        Dim v1 = convert_vector3d(p1)
        Dim v2 = convert_vector3d(p2)

        Dim edge1 = v1 - v0
        Dim edge2 = v2 - v0
        Dim deltaU1 = (p1.u - p0.u)
        Dim deltaU2 = (p2.u - p0.u)
        Dim deltaV1 = (p1.v - p0.v)
        Dim deltaV2 = (p2.v - p0.v)

        Dim f As Single = 1.0! / ((deltaU1 * deltaV2) - (deltaU2 * deltaV1))

        tangent.X = f * ((deltaV2 * edge1.X) - (deltaV1 * edge2.X))
        tangent.Y = f * ((deltaV2 * edge1.Y) - (deltaV1 * edge2.Y))
        tangent.Z = f * ((deltaV2 * edge1.Z) - (deltaV1 * edge2.Z))
        bitangent = Vector3D.CrossProduct(tangent, n)
        tangent = tangent - (Vector3D.DotProduct(tangent, n) * n)
        '
        tangent /= tangent.Length
        bitangent /= bitangent.Length

        p0.t.x = tangent.X
        p0.t.y = tangent.Y
        p0.t.z = tangent.Z
        p0.bt.x = bitangent.X
        p0.bt.y = bitangent.Y
        p0.bt.z = bitangent.Z
        p0.u = pt0.u
        p0.v = pt0.v
        '
        p1.t.x = tangent.X
        p1.t.y = tangent.Y
        p1.t.z = tangent.Z
        p1.bt.x = bitangent.X
        p1.bt.y = bitangent.Y
        p1.bt.z = bitangent.Z
        p1.u = pt1.u
        p1.v = pt1.v
        '
        p2.t.x = tangent.X
        p2.t.y = tangent.Y
        p2.t.z = tangent.Z
        p2.bt.x = bitangent.X
        p2.bt.y = bitangent.Y
        p2.bt.z = bitangent.Z
        p2.u = pt2.u
        p2.v = pt2.v

        '==========================================================================
        'add these to the buffer for averaging
        'triangle_holder(triangle_count + 0) = New t_holder_
        'triangle_holder(triangle_count + 1) = New t_holder_
        'triangle_holder(triangle_count + 2) = New t_holder_

        triangle_holder.v(triangle_count + 0) = p0
        triangle_holder.mesh_location(triangle_count + 0) = l1 ' need this so we can put them back in mesh()!

        triangle_holder.v(triangle_count + 1) = p1
        triangle_holder.mesh_location(triangle_count + 1) = l2

        triangle_holder.v(triangle_count + 2) = p2
        triangle_holder.mesh_location(triangle_count + 2) = l3


        triangle_count += 3

    End Sub

    Private Function normalize(ByVal normal As Vector3D) As vect3
        Dim len As Single = Sqrt((normal.X * normal.X) + (normal.Y * normal.Y) + (normal.Z * normal.Z))
        len = normal.Length
        ' avoid division by 0
        If len = 0.0F Then len = 1.0F
        Dim v As vect3
        ' reduce to unit size
        v.x = (normal.X / len)
        v.y = (normal.Y / len)
        v.z = (normal.Z / len)

        Return v
    End Function
    Public Function convert_vector3d(ByVal p As vertex_data) As Vector3D
        Dim v As Vector3D
        v.X = p.x
        v.Y = p.y
        v.Z = p.z
        Return v
    End Function
    Public Function convert_vect3(ByVal p As Vector3D) As vect3
        Dim v As vect3
        v.x = p.X
        v.y = p.Y
        v.z = p.Z
        Return v
    End Function
#End Region
    Public Function use_main_low_rez_texture(ByVal sections_per_side As Integer) As Boolean

        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Gl.glFinish()
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)

        '-------------------------------------------------------------------------------
        Dim psize = 256
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        '---------------------
        '============================
        frmMain.pb2.Location = New Point(0, 0)
        frmMain.pb2.Width = psize
        frmMain.pb2.Height = psize
        Application.DoEvents()


        ResizeGL_2(psize, psize)
        ViewOrtho_2()
        Gl.glClearColor(0.0, 0.0, 0.0, 1.0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        'first we draw the entire color_map
        Dim e = Gl.glGetError
        tb1.text = "Getting the Tarrain Textures..."
        For cnt = 0 To test_count

            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(cnt).colorMapId)
            Gl.glBegin(Gl.GL_QUADS)

            Gl.glTexCoord2f(0.0, 0.0)
            Gl.glVertex2f(0.0, -psize)

            Gl.glTexCoord2f(0.0, 1.0)
            Gl.glVertex2f(0.0, 0.0)

            Gl.glTexCoord2f(1.0, 1.0)
            Gl.glVertex2f(psize, 0.0)

            Gl.glTexCoord2f(1.0, 0.0)
            Gl.glVertex2f(psize, -psize)

            Gl.glEnd()
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) ' unbind main texture


            Application.DoEvents()
            Dim texID As Int32
            texID = Ilu.iluGenImage()
            Il.ilBindImage(texID)
            Dim success As Boolean = Il.ilTexImage(psize, psize, 0, 4, Il.IL_BGRA, Gl.GL_UNSIGNED_BYTE, Nothing) '  Texture specification 
            If Not success Then
                Dim er = Il.ilGetError
                MsgBox("Error splitting up Global_AM image:" + er.ToString, MsgBoxStyle.Exclamation, "Well Shit!")
            End If
            Dim ptr_2 As IntPtr = Il.ilGetData()
            Gl.glReadPixels(0, 0, psize, psize, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, ptr_2)
            Gl.glFinish()
            'Ilu.iluFlipImage()
            'Ilu.iluMirror()
            Dim image As Integer = -1
            Gl.glGenTextures(1, image)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, image)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
            Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
            Il.ilGetData()) '  Texture specification 
            blur_image(image, "vert", False)
            blur_image(image, "horz", True)
            For layer = 1 To map_layers(cnt).layer_count
                get_layer_image(cnt, layer)
            Next
            For layer = 1 To map_layers(cnt).layer_count
                If map_layers(cnt).layers(layer).l_name.ToLower.Contains("global_am") Then
                    map_layers(cnt).layers(layer).text_id = image
                    map_layers(cnt).main_texture = layer
                    GC.Collect()
                End If
            Next
            Il.ilBindImage(0)
            Il.ilDeleteImage(texID)
            frmMain.ProgressBar1.Value = cnt
        Next
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        frmMain.pb2.Visible = False
    End Function

    Public Function split_up_main_texture(ByVal sections_per_side As Integer, ByVal texture_id As Integer) As Boolean
        'If the hi rez map is not much bigger than the low rez map, 
        'we can just use the low rez for the color mix map;
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        If sections_per_side > 12 Then
            use_main_low_rez_texture(sections_per_side)
            Return True
        End If
        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Gl.glFinish()
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)

        '-------------------------------------------------------------------------------
        Dim w = CInt(Sqrt(maplist.Length - 1))
        Dim tex_width As Integer = 0

        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture_id)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_WIDTH, tex_width)

        Dim mod_ As Integer = (Sqrt(maplist.Length - 1)) And 1 ' Odd map side length?
        sections_per_side += mod_
        Dim sec_width = tex_width / sections_per_side
        Dim psize = tex_width
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        '---------------------
        'setup size of renderwindow
        '============================
        'debug
        'frmMain.pb2.Visible = True
        'frmMain.pb2.BringToFront()
        Application.DoEvents()
        '============================
        frmMain.pb2.Location = New Point(0, 0)
        frmMain.pb2.Width = psize
        frmMain.pb2.Height = psize
        Application.DoEvents()


        ResizeGL_2(psize, psize)
        ViewOrtho_2()
        Gl.glClearColor(0.0, 0.0, 0.0, 1.0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        'first we draw the entire color_map
        Dim e = Gl.glGetError
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture_id)
        Gl.glBegin(Gl.GL_QUADS)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex2f(0.0, -psize)

        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex2f(0.0, 0.0)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex2f(psize, 0.0)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex2f(psize, -psize)

        Gl.glEnd()
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) ' unbind main texture

        Dim cnt As Integer = 0
        tb1.text = "Getting the Tarrain Textures...."
        For row = 0 To w - 1
            For col = 0 To w - 1

                Application.DoEvents()
                Dim x1 As Integer = (tex_width / 2) - (((maplist(cnt).location.x + 50) / 100) * sec_width)
                Dim y1 As Integer = (tex_width / 2) - (((maplist(cnt).location.y - 50) / 100) * sec_width) - sec_width
                Dim texID As Int32
                texID = Ilu.iluGenImage()
                Il.ilBindImage(texID)
                Dim success As Boolean = Il.ilTexImage(sec_width, sec_width, 0, 4, Il.IL_BGRA, Gl.GL_UNSIGNED_BYTE, Nothing) '  Texture specification 
                If Not success Then
                    Dim er = Il.ilGetError
                    MsgBox("Error splitting up Global_AM image:" + er.ToString, MsgBoxStyle.Exclamation, "Well Shit!")
                End If
                Dim ptr_2 As IntPtr = Il.ilGetData()
                Gl.glReadPixels(x1, y1, CInt(sec_width), CInt(sec_width), Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, ptr_2)
                Gl.glFinish()
                Ilu.iluFlipImage()
                Ilu.iluMirror()
                Dim image As Integer = -1
                Gl.glGenTextures(1, image)
                Gl.glEnable(Gl.GL_TEXTURE_2D)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, image)
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)

                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
                'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
                Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
                Il.ilGetData()) '  Texture specification 
                For layer = 1 To map_layers(cnt).layer_count
                    get_layer_image(cnt, layer)
                Next
                For layer = 1 To map_layers(cnt).layer_count
                    If map_layers(cnt).layers(layer).l_name.ToLower.Contains("global_am") Then
                        map_layers(cnt).layers(layer).text_id = image
                        map_layers(cnt).main_texture = layer
                        GC.Collect()
                    End If
                Next
                Il.ilBindImage(0)
                Il.ilDeleteImage(texID)
                frmMain.ProgressBar1.Value = cnt
                cnt += 1
            Next
        Next
        Gl.glDeleteTextures(1, texture_id) ' Delete that huge map now that we are done using it.
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        frmMain.pb2.Visible = False
    End Function
    Private Sub debug_draw(ByVal image As Integer)
        'used just to show that an image was created correctly.
        'For debuging only.
        'PB2 MUST be visible and brought to front.
        'This swaps the buffers so a break point will need to
        'be added to see the texture.
        Dim psize As Integer
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, image)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_WIDTH, psize)
        Gdi.SwapBuffers(pb2_hDC)
        Gl.glClearColor(0.0, 0.0, 0.0, 1.0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_DEPTH_TEST)

        Dim e = Gl.glGetError
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, image)
        Gl.glBegin(Gl.GL_QUADS)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex2f(0.0, -psize)

        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex2f(0.0, 0.0)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex2f(psize, 0.0)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex2f(psize, -psize)

        Gl.glEnd()

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) ' unbind texture
        Gdi.SwapBuffers(pb2_hDC)

    End Sub
    Private Sub check_bounds(ByVal v As vertex_data)
        If v.x > x_max Then x_max = v.x
        If v.x < x_min Then x_min = v.x
        If v.y > z_max Then z_max = v.y
        If v.y < z_min Then z_min = v.y
        If v.z > y_max Then y_max = v.z
        If v.z < y_min Then y_min = v.z

    End Sub


    Public Sub get_surface_normals(ByRef s As MemoryStream, ByVal map As Int32)
        'This sub is no longer used.
        'I will leave it so others can see how the terrain normals are stored.
        Dim data((heightMapSize * heightMapSize * 2) + heightMapSize) As SByte
        Dim cnt As UInt32 = 0
        Dim i As UInt32 = 0
        s.Position = 0
        Dim cols As Integer = 0
        Dim x, y As Integer
        Using s
            'Try
            s.Position = 0
            Dim buf(s.Length) As Byte

            s.Position = 16 'skip bigworld header stuff
            Dim rdr As New PngReader(s) ' create png from stream 's'
            Dim imginfo As ImageInfo = rdr.ImgInfo
            cols = imginfo.Cols
            rdr.ChunkLoadBehaviour = ChunkLoadBehaviour.LOAD_CHUNK_ALWAYS
            x = rdr.ImgInfo.Cols
            y = rdr.ImgInfo.Rows
            If x * y <> 4096 Then
                MsgBox("Odd lodNormals file!!!", MsgBoxStyle.Exclamation, "Well Shit...")
            End If
            ReDim data(rdr.ImgInfo.Cols * rdr.ImgInfo.Rows * 2)
            For i = 0 To rdr.ImgInfo.Cols - 1
                Dim iline As ImageLine  ' create place to hold a scan line
                iline = rdr.GetRow(i)
                For j = 0 To iline.Scanline.Length - 1
                    'get the line and convert from word to byte and save in our buffer 'data'
                    Dim bytes() As Byte = BitConverter.GetBytes(iline.Scanline(j))
                    data(cnt) = bytes(0)
                    data(cnt + 1) = bytes(1)
                    cnt += 2
                Next
            Next
            rdr.End()
            s.Close()
            s.Dispose()
        End Using
        cnt = 0
        Dim b As New Bitmap(x, y, PixelFormat.Format32bppArgb)
        b.MakeTransparent(Color.Black)
        For j As Integer = 0 To y - 1
            For k As Integer = 0 To (x * 2) - 1 Step 2
                Dim R_ As SByte = data((k) + (j * (x * 2)))
                Dim G_ As SByte = data((k + 1) + (j * (x * 2)))
                Dim B_ As SByte = CByte(Sqrt(1.0 - (data((k) + (j * (x * 2))) ^ 2) + (data((k + 1) + (j * (x * 2))) ^ 2)))
                b.SetPixel((k / 2%), j, Color.FromArgb(255, R_ And &HFF, G_ And &HFF, B_ And &HFF))
                'Dim c As Color = b.GetPixel((k / 2%), j)
                'Debug.Write(R_.ToString + " " + B_.ToString + " : ")
            Next
            'Debug.Write(vbCrLf)
        Next
        b.RotateFlip(RotateFlipType.RotateNoneFlipX)
        Gl.glGenTextures(1, maplist(map).normMapID)
        Dim bitmapData = b.LockBits(New Rectangle(0, 0, b.Width, _
                             b.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(map).normMapID)

        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)
        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, b.Width, b.Height, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0)
        b.UnlockBits(bitmapData) ' Unlock The Pixel Data From Memory
        b.Dispose() ' Dispose The Bitmap

        Return
    End Sub

    Public Sub read_heights(ByVal r As MemoryStream, ByVal map As Int32)
        r.Position = 0
        ReDim maplist(map).BB(16)
        Dim f = New BinaryReader(r)
        Dim magic = f.ReadUInt32()
        h_width = f.ReadUInt32
        h_height = f.ReadUInt32
        Dim comp = f.ReadUInt32
        Dim version = f.ReadUInt32
        Dim h_min = f.ReadSingle
        Dim h_max = f.ReadSingle
        maplist(map).BB_Max.y = h_max
        maplist(map).BB_Min.y = h_min
        Dim crap = f.ReadUInt32
        Dim heaader = f.ReadUInt32
        Dim pos = r.Position

        Dim mapsize As UInt32
        Dim data(heightMapSize * heightMapSize * 4) As Byte
        Dim cnt As UInt32 = 0
        Using r
            r.Position = 36 'skip bigworld header stuff
            Dim rdr As New PngReader(r) ' create png from stream 's'
            Dim iInfo = rdr.ImgInfo
            mapsize = iInfo.Cols

            ReDim data(iInfo.Cols * iInfo.Cols * 4)
            Dim iline As ImageLine  ' create place to hold a scan line
            For i = 0 To iInfo.Cols - 1
                iline = rdr.GetRow(i)
                For j = 0 To iline.Scanline.Length - 1
                    'get the line and convert from word to byte and save in our buffer 'data'
                    Dim bytes() As Byte = BitConverter.GetBytes(iline.Scanline(j))
                    data(cnt) = iline.Scanline(j)
                    cnt += 1
                Next
            Next
            r.Close()
            r.Dispose()
        End Using
        Dim qtized As Single
        'Dim pnt As IntPtr = Marshal.AllocHGlobal(size)
        Dim ms As MemoryStream = New MemoryStream(data, False)
        Dim br As New BinaryReader(ms)
        Dim sv, ev As Integer
        Dim ty As Integer
        If mapsize < 64 Then
            ReDim bmp_data(64, 64)
            Dim div = 64 / (mapsize - 5)
            ReDim maplist(map).heights(64, 64)
            heightMapSize = 64
            For j As UInt32 = 2 To mapsize - 4
                For i As UInt32 = 2 To mapsize - 4
                    ms.Position = (i * 4) + (j * mapsize * 4)
                    sv = br.ReadInt32
                    ev = br.ReadInt32
                    For xp = (i - 2) * div To (((i + 1) - 2) * div)
                        Dim ii = (i - 2) * div
                        Dim xval As Single = (ev - sv) * ((xp - ii) / div)
                        bmp_data(64 - xp, (j - 2) * div) = (xval + sv) * 0.001
                        maplist(map).heights(64 - xp, (j - 2) * div) = (xval + sv) * 0.001
                        ty = xp

                        ms.Position = (i * 4) + ((j + 1) * mapsize * 4)
                        ev = br.ReadInt32
                        For yp = (j - 2) * div To (((j + 1) - 2) * div)
                            Dim jj = (j - 2) * div
                            Dim yval As Single = (ev - sv) * ((yp - jj) / div)
                            bmp_data(64 - xp, yp) = (yval + sv) * 0.001
                            maplist(map).heights(64 - xp, yp) = (yval + sv) * 0.001
                        Next
                    Next
                    ' Debug.Write(qtized & vbCrLf)
                Next
            Next

        Else

            ReDim bmp_data(heightMapSize, heightMapSize)
            ReDim maplist(map).heights(heightMapSize, heightMapSize)
            For j As UInt32 = 3 To mapsize - 3
                For i As UInt32 = 3 To mapsize - 3
                    ms.Position = (i * 4) + (j * mapsize * 4)
                    Dim tc = br.ReadInt32
                    qtized = tc * 0.001
                    ' Debug.Write(qtized & vbCrLf)
                    bmp_data(mapsize - i - 3, j - 3) = qtized
                    maplist(map).heights(mapsize - i - 3, j - 3) = qtized
                Next
            Next
        End If
        Dim avg As Single
        For j As UInt32 = 0 To heightMapSize - 1
            For i As UInt32 = 0 To heightMapSize - 1
                avg += bmp_data(i, j)
                If bmp_data(i, j) < y_min Then
                    y_min = bmp_data(i, j)
                End If
                If bmp_data(i, j) > y_max Then
                    y_max = bmp_data(i, j)
                End If
            Next
        Next
        maplist(map).heights_avg = avg / (heightMapSize ^ 2)
        br.Close()
        ms.Close()
        ms.Dispose()
        'End If
    End Sub

    Private Function get_string(ByVal f As BinaryReader) As String
        Dim c As Byte
        Dim os As String = ""
        While 1
            c = f.ReadByte
            If c = 0 Then
                Exit While
            End If
            os += Convert.ToChar(c)
        End While
        Return os
    End Function


    Public Function get_Z_at_XY(ByVal Lx As Double, ByVal Lz As Double) As Single
        'If Not maploaded Then Return 100.0
        If mapBoard Is Nothing Then Return Z_Cursor
        Dim tlx As Single = 100.0 / 64.0
        Dim tly As Single = 100.0 / 64.0
        Dim ts As Single = 64.0 / 100.0
        Dim tl, tr, br, bl, w As Vector3D
        Dim xvp, yvp As Integer
        Dim ryp, rxp As Single
        'Dim mod_ = (MAP_SIDE_LENGTH) And 1
        Dim s = Sqrt(maplist.Length - 1)
        For xo = s - 1 To 0 Step -1
            For yo = s To 0 Step -1
                Dim px = maplist(mapBoard(xo, yo)).location.x
                If px - 50 < Lx And px + 50 > Lx Then
                    xvp = xo
                    Dim pz = maplist(mapBoard(xo, yo)).location.y
                    If pz - 50 < Lz And pz + 50 > Lz Then
                        yvp = yo
                        GoTo exit2
                    End If
                    'GoTo exit1
                End If
            Next
        Next
exit1:
        For xo = s - 1 To 0 Step -1
            For yo = s - 1 To 0 Step -1
                Dim pz = maplist(mapBoard(xo, yo)).location.y
                If pz - 50 < Lz And pz + 50 > Lz Then
                    yvp = yo
                    GoTo exit2
                End If
            Next
        Next
exit2:

        'If maploaded Then
        '    Debug.Write("XP:" + xvp.ToString + "  ZP:" + yvp.ToString + vbCrLf)
        'End If
        'Dim msqrt = (MAP_SIDE_LENGTH / 2)

        Dim map = mapBoard(xvp, yvp)
        If maplist.Length - 1 < map Then
            Return eyeY
        End If
        If maplist(map).heights Is Nothing Then
            Return Z_Cursor
        End If

        Dim vxp As Double = ((((Lx) / 100)) - Truncate((Truncate(Lx) / 100))) * 64.0
        Dim tx As Int32 = Round(Truncate(Lx / 100))
        Dim tz As Int32 = Round(Truncate(Lz / 100))
        If Lx < 0 Then
            tx += -1
        End If
        If Lz < 0 Then
            tz += -1
        End If
        Dim tx1 = (tx * 100)
        Dim tz1 = (tz * 100)

        Dim vyp As Double = ((((Lz) / 100)) - Truncate((Truncate(Lz) / 100))) * 64.0

        If vyp < 0.0 Then
            vyp = 64.0 + vyp
        End If
        If vxp < 0 Then
            vxp = 64.0 + vxp

        End If
        vxp = Round(vxp, 12)
        vyp = Round(vyp, 12)
        rxp = (Floor(vxp))
        rxp *= tlx
        ryp = Floor(vyp)
        ryp *= tlx
        'rxp = 64 + rxp
        w.X = (vxp * tlx)
        w.Y = (vyp * tlx)
        'vaid.x = w.X + maplist(map).location.x - 50.0
        'vaid.y = w.Y + maplist(map).location.y - 50.0
        Dim HX, HY, OX, OY As Integer
        HX = Floor(vxp)
        OX = 1
        HY = Floor(vyp)
        OY = 1
        'd_hx = HX
        'd_hy = HY
        Dim altitude As Single = 0.0
        'Try
        'look_point_Y = cp
        'w.Z = 1.0 'dont need this but who cares?
        If HX + OX > 64 Then
            Return Z_Cursor
        End If
        tl.X = rxp
        tl.Y = ryp
        tl.Z = maplist(map).heights(HX, HY)

        tr.X = rxp + tlx
        tr.Y = ryp
        tr.Z = maplist(map).heights(HX + OX, HY)

        br.X = rxp + tlx
        br.Y = ryp + tlx
        br.Z = maplist(map).heights(HX + OX, HY + OY)

        bl.X = rxp
        bl.Y = ryp + tlx
        bl.Z = maplist(map).heights(HX, HY + OY)

        tr_ = tr
        br_ = br
        tl_ = tl
        bl_ = bl

        tr_.X += tx1
        br_.X += tx1
        tl_.X += tx1
        bl_.X += tx1

        tr_.Y += tz1
        br_.Y += tz1
        tl_.Y += tz1
        bl_.Y += tz1

        'for drawing the red square on the terrain
        T_1.x = tr.X + maplist(map).location.x - 50
        T_1.y = tr.Y + maplist(map).location.y - 50
        T_1.z = tr.Z

        T_2.x = tl.X + maplist(map).location.x - 50
        T_2.y = tl.Y + maplist(map).location.y - 50
        T_2.z = tl.Z

        T_3.x = br.X + maplist(map).location.x - 50
        T_3.y = br.Y + maplist(map).location.y - 50
        T_3.z = br.Z

        T_4.x = bl.X + maplist(map).location.x - 50
        T_4.y = bl.Y + maplist(map).location.y - 50
        T_4.z = bl.Z

        Dim agl = Atan2(w.Y - tr.Y, w.X - tr.X)
        If agl <= PI * 0.75 Then
            altitude = find_altitude(tr, bl, br, w)
            Return altitude
        End If
        If agl > PI * 0.75 Then
            altitude = find_altitude(tr, tl, bl, w)
            Return altitude
        End If
        'tb1.Update()
domath:
        Return altitude

        'Catch ex As Exception

        'End Try

    End Function
    Public Sub flipYZ(ByRef v As Vector3D)
        Dim t As Single
        t = v.Y
        v.Y = v.Z
        v.Z = t
    End Sub

    Private Function find_altitude(ByVal p As Vector3D, ByVal q As Vector3D, ByVal r As Vector3D, ByVal f As Vector3D) As Double
        'This finds the height on the face of a triangle at point f.x, f.z
        flipYZ(p)
        flipYZ(q)
        flipYZ(r)
        flipYZ(f)

        Cursor_point.X = f.X
        Cursor_point.Z = f.Z
        'It returns that value as a double

        Dim nc As Vector3D
        nc = CrossProduct(p - r, q - r)
        nc.Normalize()

        If p.Z = q.Z And q.Z = r.Z Then
            Return r.Y
        End If
        surface_normal.x = -nc.X
        surface_normal.y = -nc.Z
        surface_normal.z = -nc.Y
        'nc *= -1.0
        Dim k As Double
        k = (nc.X * (f.X - p.X)) + (nc.Z * (f.Z - q.Z))

        Dim y = ((k) / -nc.Y) + p.Y

        Cursor_point.Y = y
        Dim vx As Vector3D = r - f
        Dim vy = ((nc.Z * vx.Z) + (nc.X * vx.X)) / nc.Y
        y = r.Y + vy
        Return y
    End Function
End Module
