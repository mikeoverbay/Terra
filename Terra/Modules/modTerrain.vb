﻿Imports System.IO
Imports System.Math
Imports System.Runtime.InteropServices
Imports System

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


    Public Sub open_dominate(ByVal map As Integer, ByVal dom As MemoryStream)
        Dim magic, version As UInt32

        Dim texture_count As Integer = 0

        Dim mapX, mapY As UInt32
        Dim str1_len As UInt32
        Dim strings(4) As String
        dom.Position = 0

        Dim br As New BinaryReader(dom)
        magic = br.ReadUInt32
        version = br.ReadUInt32

        texture_count = br.ReadUInt32
        str1_len = br.ReadUInt32
        mapX = br.ReadUInt32
        mapY = br.ReadUInt32
        br.ReadUInt64() ' wasted space

        Dim ds(str1_len) As Byte
        For k = 0 To texture_count - 1

            For i = 0 To str1_len - 1
                ds(i) = br.ReadByte
            Next

            strings(k) = System.Text.Encoding.UTF8.GetString(ds, 0, str1_len)

        Next

        Dim magic1 = br.ReadInt32
        Dim magic2 = br.ReadInt32
        Dim uncompressedsize = br.ReadInt32
        'Dim b(s.Length - 12) As Byte
        's.Read(b, 0, s.Length - 12)
        Dim buff(uncompressedsize) As Byte
        Dim ps As New MemoryStream(buff)


        Using Decompress As Zlib.ZlibStream = New Zlib.ZlibStream(dom, Zlib.CompressionMode.Decompress, False)
            ' Copy the compressed file into the decompression stream. 
            Dim buffer As Byte() = New Byte(4096) {}
            Dim numRead As Integer
            numRead = Decompress.Read(buffer, 0, buffer.Length)
            Do While numRead <> 0
                ps.Write(buffer, 0, numRead)
                numRead = Decompress.Read(buffer, 0, buffer.Length)
            Loop

        End Using

        '
        Dim dbuff(mapX * mapY * 4) As Byte
        'save the data to convert it
        For i = 0 To uncompressedsize - 1
            dbuff((i * 4) + 0) = buff(i)
            dbuff((i * 4) + 1) = buff(i)
            dbuff((i * 4) + 2) = buff(i)
            dbuff((i * 4) + 3) = buff(i)
        Next


        '------------------------------------------------------------------
        Dim bufPtr As IntPtr = Marshal.AllocHGlobal(dbuff.Length - 1)
        Marshal.Copy(dbuff, 0, bufPtr, dbuff.Length - 1)
        Dim texID = Ilu.iluGenImage() ' Generation of one image name
        Il.ilBindImage(texID) ' Binding of image name 
        Dim success = Il.ilGetError

        Il.ilTexImage(mapX, mapY, 1, 4, Il.IL_RGBA, Il.IL_UNSIGNED_BYTE, bufPtr) ' should have a new image here
        success = Il.ilGetError
        Marshal.FreeHGlobal(bufPtr) ' free this up
        If success = Il.IL_NO_ERROR Then
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)
            Dim f = Il.IL_FALSE
            Dim t = Il.IL_TRUE
            'success = Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE)    ' Convert every colour component into unsigned bytes
            'Ilu.iluFlipImage()
            Ilu.iluMirror()

            'If your image contains alpha channel you can replace IL_RGB with IL_RGBA 
            Gl.glGenTextures(1, maplist(map).DominateId)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(map).DominateId)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
            Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
            Il.ilGetData()) '  Texture specification 
            'Gl.glGenerateMipmapEXT(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Il.ilBindImage(0)
            ilu.iludeleteimage(texID)
        End If
        br.Close()
        br.Dispose()
        dom.Dispose()
    End Sub
    Public Sub get_horizonShadowMap(map As Integer, e As Ionic.Zip.ZipEntry)
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
            ilu.iludeleteimage(texID)
        End If
        br.Close()
        s.Dispose()

    End Sub

    Public Function get_layer_count(ByVal ck As Ionic.Zip.ZipFile, map As Integer) As Integer
        Dim layer_count As Integer = 0
        ReDim map_layers(map).layers(4)
        map_layers(map).used_layers = 0
        map_layers(map).layers(1).text_id = dummy_texture
        map_layers(map).layers(2).text_id = dummy_texture
        map_layers(map).layers(3).text_id = dummy_texture
        map_layers(map).layers(4).text_id = dummy_texture

        map_layers(map).layers(1).norm_id = dummy_texture
        map_layers(map).layers(2).norm_id = dummy_texture
        map_layers(map).layers(3).norm_id = dummy_texture
        map_layers(map).layers(4).norm_id = dummy_texture
        'this is useless so far.. may try horizonShadow file at some point.
        '	Dim HSM As Ionic.Zip.ZipEntry = ck("terrain2/horizonShadows")
        '	get_horizonShadowMap(map, HSM)


        layer_uv_list += "..........................................." + vbCrLf
        Dim sm = "map:" + map.ToString + vbCrLf
        layer_uv_list += sm
        Dim layer1 As Ionic.Zip.ZipEntry = ck("terrain2/layer 1")
        If layer1 IsNot Nothing Then
            map_layers(map).layers(1).ms = New MemoryStream
            layer1.Extract(map_layers(map).layers(1).ms)
            get_layer_data(map_layers(map).layers(1).ms, 1, map)
            map_layers(map).layers(1).ms.Dispose()
            layer_count += 1
        End If
        Dim layer2 As Ionic.Zip.ZipEntry = ck("terrain2/layer 2")
        If layer2 IsNot Nothing Then
            map_layers(map).layers(2).ms = New MemoryStream
            layer2.Extract(map_layers(map).layers(2).ms)
            get_layer_data(map_layers(map).layers(2).ms, 2, map)
            map_layers(map).layers(2).ms.Dispose()
            layer_count += 1
        End If
        Dim layer3 As Ionic.Zip.ZipEntry = ck("terrain2/layer 3")
        If layer3 IsNot Nothing Then
            map_layers(map).layers(3).ms = New MemoryStream
            layer3.Extract(map_layers(map).layers(3).ms)
            get_layer_data(map_layers(map).layers(3).ms, 3, map)
            map_layers(map).layers(3).ms.Dispose()
            layer_count += 1
        End If
        Dim layer4 As Ionic.Zip.ZipEntry = ck("terrain2/layer 4")
        If layer4 IsNot Nothing Then
            map_layers(map).layers(4).ms = New MemoryStream
            layer4.Extract(map_layers(map).layers(4).ms)
            get_layer_data(map_layers(map).layers(4).ms, 4, map)
            map_layers(map).layers(4).ms.Dispose()
            layer_count += 1
        End If
        map_layers(map).layer_count = layer_count
        'If layer_count = 2 Then
        '	set_layer_2(map)
        'End If
        If layer_count = 0 Then
            'Stop
            Return layer_count
        End If
        make_mix_texture_id(map, map_layers(map).mix_image)

        'frmMain.PictureBox1.Image = map_layers(map).mix_image.Clone
        'Application.DoEvents()
        'Application.DoEvents()
    End Function
    Public Sub get_layer_data(ms As MemoryStream, layer As Integer, map As Integer)
        ms.Position = 0
        Dim b As New BinaryReader(ms)

        'read magic
        b.ReadInt32()
        'read size
        map_layers(map).layers(layer).sizex = b.ReadInt32 ' get size in x
        map_layers(map).layers(layer).sizez = b.ReadInt32 ' get size in z
        'there should always be a layer 1 and at this point we need to create the mix value bitmap
        If layer = 1 Then
            map_layers(map).mix_image = New Bitmap(map_layers(map).layers(layer).sizex, map_layers(map).layers(layer).sizex, Imaging.PixelFormat.Format32bppArgb)
        End If
        b.ReadInt32() 'read off unused int32
        'read uv mapping. The next 8 bytes are actually 2 vect4 numbers. We want the X and the Z from these
        'read unused
        If map = 42 Then
            'Stop
        End If

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
        If map_name.Contains("maps/landscape/00_AllTiles/Farmland_A_4") And textcount = 0 Then
            map_layers(map).layers(layer).n_name = "maps/landscape/00_AllTiles/Farmland_A_4_NM."
            GoTo set_Mask
        End If
        If textcount > 0 Then
            Dim normal_name As String = b.ReadString.Replace(Chr(0), "") ' this is the normal map name
            map_layers(map).layers(layer).n_name = normal_name
            b.ReadByte()
            b.ReadByte()
            b.ReadByte()
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

        End If
        get_layer_mix(ms, map, layer)
        's.Dispose()
        b.Close()
        b.Dispose()

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
    Public Sub get_layer_mix(ByRef s As MemoryStream, ByVal map As Int32, ByRef layer As Integer)
        Dim data((map_layers(map).layers(layer).sizex * map_layers(map).layers(layer).sizex)) As Byte
        Dim cnt As UInt32 = 0
        Dim i As UInt32 = 0
        Dim cols As Integer = 0
        Dim x, y As Integer
        Using s
            'Try
            's.Position = 0
            Dim buf(s.Length) As Byte

            's.Position = 16 'skip bigworld header stuff
            Dim rdr As New PngReader(s) ' create png from stream 's'
            Dim imginfo As ImageInfo = rdr.ImgInfo
            cols = imginfo.Cols
            rdr.ChunkLoadBehaviour = ChunkLoadBehaviour.LOAD_CHUNK_ALWAYS
            x = rdr.ImgInfo.Cols
            y = rdr.ImgInfo.Rows
            ReDim data(rdr.ImgInfo.Cols * rdr.ImgInfo.Rows)
            For i = 0 To map_layers(map).layers(layer).sizex - 1
                Dim iline As ImageLine  ' create place to hold a scan line
                iline = rdr.GetRow(i)
                For j = 0 To iline.Scanline.Length - 1
                    'get the line and convert from word to byte and save in our buffer 'data'
                    Dim bytes() As Byte = BitConverter.GetBytes(iline.Scanline(j))
                    data(cnt) = bytes(0)
                    cnt += 1
                Next
            Next
            rdr.End()
            s.Close()
            s.Dispose()
        End Using

        Dim rect As New Rectangle(0, 0, map_layers(map).mix_image.Width, map_layers(map).mix_image.Height)
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
        For i = 0 To data.Length - 2
            Select Case layer
                Case 1
                    check += data(i)
                    rgbValues((i * 4) + 0) = data(i)
                Case 2
                    check += data(i)
                    rgbValues((i * 4) + 1) = data(i)
                Case 3
                    check += data(i)
                    rgbValues((i * 4) + 2) = data(i)
                Case 4
                    check += data(i)
                    rgbValues((i * 4) + 3) = data(i)
            End Select
        Next

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
 

    Public Function get_layer_image(map As Integer, layer As Integer) As Boolean
        Dim cnt = map_layer_cache.Length
        Dim color_update As Boolean = True
        Dim normal_update As Boolean = True

        Dim map_name As String = map_layers(map).layers(layer).l_name + "dds"
        Dim n_map_name As String = map_layers(map).layers(layer).n_name + "dds"
        'check if this texture already exists
        For i = 0 To cnt - 1
            If map_layer_cache(i).name = map_name Then
                map_layers(map).layers(layer).text_id = map_layer_cache(i).textureID
                saved_texture_loads += 1
                color_update = False
            End If
        Next
        If frmMain.m_high_rez_Terrain.Checked Then 'only if we are using bumps
            'check if this texture already exists
            cnt = normalMap_layer_cache.Length
            n_map_name = map_layers(map).layers(layer).n_name + "dds"
            For i = 0 To cnt - 1
                If normalMap_layer_cache(i).name = n_map_name Then
                    map_layers(map).layers(layer).norm_id = normalMap_layer_cache(i).textureNormID
                    saved_texture_loads += 1
                    normal_update = False
                End If
            Next
        End If
        '================= get color map
        If color_update Then

            Dim dds As New MemoryStream
            'map_layers(map).layers(layer).image = 
            'if get_main_texture is false, it wont read the huge main texture again!
            If map_name.ToLower.Contains("color_tex") Then
                '    get_main_texture = False
                '    main_layer_tex = build_layer_textures_bmp(map, dds, layer).Clone 'We want a Bitmap back
                '    'update the cache
                cnt = map_layer_cache.Length
                ReDim Preserve map_layer_cache(cnt)
                map_layer_cache(cnt - 1) = New tree_textures_
                map_layer_cache(cnt - 1).name = map_name
                'map_layer_cache(cnt - 1).textureID = map_layers(map).layers(layer).text_id
                'Gl.glDeleteTextures(1, map_layers(map).layers(layer).text_id)
                '    Return True
            Else
                Dim map_entry As Ionic.Zip.ZipEntry = active_pkg(map_name)
                If map_entry Is Nothing Then
                    map_entry = shared_content1(map_name)
                    If map_entry Is Nothing Then
                        map_entry = shared_content2(map_name)
                        If map_entry Is Nothing Then
                            Return False
                        End If
                    End If
                End If
                map_entry.Extract(dds)
                build_layer_textures_no_bmp(map, dds, layer)        'We dont need the Bitmap. This saves some time
            End If
        End If

        '================= get norml map
        If normal_update And frmMain.m_high_rez_Terrain.Checked Then
            Dim nmap_entry As Ionic.Zip.ZipEntry = active_pkg(n_map_name)
            If nmap_entry Is Nothing Then
                nmap_entry = shared_content1(n_map_name)
                If nmap_entry Is Nothing Then
                    nmap_entry = shared_content2(n_map_name)
                    If nmap_entry Is Nothing Then
                        Return False
                    End If
                End If
            End If
            Dim mdds As New MemoryStream
            nmap_entry.Extract(mdds)
            build_normal_layer_textures(map, mdds, layer)
        End If

        '---------------------------------------------------
        'update the cache

        If color_update Then
            cnt = map_layer_cache.Length
            ReDim Preserve map_layer_cache(cnt)
            map_layer_cache(cnt - 1) = New tree_textures_
            map_layer_cache(cnt - 1).name = map_name
            map_layer_cache(cnt - 1).textureID = map_layers(map).layers(layer).text_id
        End If
        If normal_update And frmMain.m_high_rez_Terrain.Checked Then
            'update the cache
            cnt = normalMap_layer_cache.Length
            ReDim Preserve normalMap_layer_cache(cnt)
            normalMap_layer_cache(cnt - 1) = New tree_textures_
            normalMap_layer_cache(cnt - 1).name = n_map_name
            normalMap_layer_cache(cnt - 1).textureNormID = map_layers(map).layers(layer).norm_id
        End If


        Return False
    End Function

    Public Sub create_mix_atlas()
        Dim uc As vect2
        Dim lc As vect2
        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Gl.glFinish()
        Gl.glDepthFunc(Gl.GL_LEQUAL)
        Gl.glFrontFace(Gl.GL_CW)
        Gl.glCullFace(Gl.GL_BACK)
        Gl.glLineWidth(1)
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        '-------------------------------------------------------------------------------
        Dim w = CInt(Sqrt(maplist.Length - 1))
        Dim psize = w * 272
        '---------------------
        'setup size of renderwindow
        frmMain.pb2.Location = New Point(0, frmMain.mainMenu.Height + frmMain.ProgressBar1.Height)
        frmMain.pb2.Width = psize
        frmMain.pb2.Height = psize

        'frmMain.pb2.BringToFront()
        'frmMain.pb2.Visible = True
        ResizeGL_2()
        ViewPerspective_2()
        ViewOrtho_2()
        Gl.glPushMatrix()
        Gl.glTranslatef(0.0, 0.0F, -0.1F)
        Gl.glDisable(Gl.GL_LIGHTING)
        'Gl.glColor3f(0.8!, 0.0!, 0.0!)
        'Gl.glColor3f(1.0!, 1.0!, 1.0!)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        '----------------------
        'We draw bigger and bigger squares to put a boarder around the actual image.
        'This is to reduce the impact of linear filtering.
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Dim x, y, cnt As Integer
        Dim wx, wy As Integer
        For x = 0 To w - 1
            For y = 0 To w - 1
                wx = (x * 272)
                wy = (-(w - y) * 272)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(cnt).mix_text_Id)

                Gl.glBegin(Gl.GL_TRIANGLES)
                '---
                uc.x = wx : uc.y = wy + 272
                lc.x = wx + 272 : lc.y = wy
                Gl.glTexCoord2f(0.0!, 0.0!)
                Gl.glVertex3f(uc.x, lc.y, -1.0!)

                Gl.glTexCoord2f(0.0!, -1.0!)
                Gl.glVertex3f(uc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, 0.0!)
                Gl.glVertex3f(lc.x, lc.y, -1.0!)
                '---
                Gl.glTexCoord2f(0.0!, -1.0!)
                Gl.glVertex3f(uc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, -1.0!)
                Gl.glVertex3f(lc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, 0.0!)
                Gl.glVertex3f(lc.x, lc.y, -1.0!)

                Gl.glEnd()
                cnt += 1
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Next
        Next
        cnt = 0
        Gl.glColor3f(0.0!, 0.0!, 1.0!)
        For x = 0 To w - 1
            For y = 0 To w - 1
                wx = (x * 272)
                wy = (-(w - y) * 272)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(cnt).mix_text_Id)

                Gl.glBegin(Gl.GL_TRIANGLES)
                '---
                uc.x = wx + 5 : lc.y = wy + 5
                lc.x = uc.x + 262 : uc.y = lc.y + 262
                Gl.glTexCoord2f(0.0!, 0.0!)
                Gl.glVertex3f(uc.x, lc.y, -1.0!)

                Gl.glTexCoord2f(0.0!, -1.0!)
                Gl.glVertex3f(uc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, 0.0!)
                Gl.glVertex3f(lc.x, lc.y, -1.0!)
                '---
                Gl.glTexCoord2f(0.0!, -1.0!)
                Gl.glVertex3f(uc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, -1.0!)
                Gl.glVertex3f(lc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, 0.0!)
                Gl.glVertex3f(lc.x, lc.y, -1.0!)

                Gl.glEnd()
                cnt += 1
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Next
        Next
        cnt = 0
        Gl.glColor3f(0.0!, 0.0!, 1.0!)
        For x = 0 To w - 1
            For y = 0 To w - 1
                wx = (x * 272)
                wy = (-(w - y) * 272)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(cnt).mix_text_Id)

                Gl.glBegin(Gl.GL_TRIANGLES)
                '---
                uc.x = wx + 6 : lc.y = wy + 6
                lc.x = uc.x + 260 : uc.y = lc.y + 260
                Gl.glTexCoord2f(0.0!, 0.0!)
                Gl.glVertex3f(uc.x, lc.y, -1.0!)

                Gl.glTexCoord2f(0.0!, -1.0!)
                Gl.glVertex3f(uc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, 0.0!)
                Gl.glVertex3f(lc.x, lc.y, -1.0!)
                '---
                Gl.glTexCoord2f(0.0!, -1.0!)
                Gl.glVertex3f(uc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, -1.0!)
                Gl.glVertex3f(lc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, 0.0!)
                Gl.glVertex3f(lc.x, lc.y, -1.0!)

                Gl.glEnd()
                cnt += 1
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Next
        Next
        cnt = 0
        Gl.glColor3f(0.0!, 0.0!, 1.0!)
        For x = 0 To w - 1
            For y = 0 To w - 1
                wx = (x * 272)
                wy = (-(w - y) * 272)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(cnt).mix_text_Id)

                Gl.glBegin(Gl.GL_TRIANGLES)
                '---
                uc.x = wx + 7 : lc.y = wy + 7
                lc.x = uc.x + 258 : uc.y = lc.y + 258
                Gl.glTexCoord2f(0.0!, 0.0!)
                Gl.glVertex3f(uc.x, lc.y, -1.0!)

                Gl.glTexCoord2f(0.0!, -1.0!)
                Gl.glVertex3f(uc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, 0.0!)
                Gl.glVertex3f(lc.x, lc.y, -1.0!)
                '---
                Gl.glTexCoord2f(0.0!, -1.0!)
                Gl.glVertex3f(uc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, -1.0!)
                Gl.glVertex3f(lc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, 0.0!)
                Gl.glVertex3f(lc.x, lc.y, -1.0!)

                Gl.glEnd()
                cnt += 1
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Next
        Next
        cnt = 0
        For x = 0 To w - 1
            For y = 0 To w - 1
                wx = (x * 272)
                wy = (-(w - y) * 272)
                Gl.glColor3f(0.0!, 0.6!, 0.0!)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(cnt).mix_text_Id)

                Gl.glBegin(Gl.GL_TRIANGLES)
                '---
                uc.x = wx + 8 : lc.y = wy + 8
                lc.x = uc.x + 256 : uc.y = lc.y + 256
                Gl.glTexCoord2f(0.0!, 0.0!)
                Gl.glVertex3f(uc.x, lc.y, -1.0!)

                Gl.glTexCoord2f(0.0!, -1.0!)
                Gl.glVertex3f(uc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, 0.0!)
                Gl.glVertex3f(lc.x, lc.y, -1.0!)
                '---
                Gl.glTexCoord2f(0.0!, -1.0!)
                Gl.glVertex3f(uc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, -1.0!)
                Gl.glVertex3f(lc.x, uc.y, -1.0!)

                Gl.glTexCoord2f(1.0!, 0.0!)
                Gl.glVertex3f(lc.x, lc.y, -1.0!)

                Gl.glEnd()
                cnt += 1
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Next
        Next
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glFinish()
        Gl.glFlush()
        Dim texID As Int32
        texID = Ilu.iluGenImage() ' Generation of one image name
        Il.ilBindImage(texID) ' Binding of image name 
        Dim success As Boolean = Il.ilTexImage(psize, psize, 0, 4, Il.IL_BGRA, Gl.GL_UNSIGNED_BYTE, Nothing) '  Texture specification 
        Dim ptr_2 As IntPtr = Il.ilGetData()
        Gl.glReadPixels(0, 0, psize, psize, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, ptr_2)



        success = Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes


        Ilu.iluFlipImage()
        Ilu.iluMirror()
        'Ilu.iluBlurGaussian(2)
        'Gdi.SwapBuffers(pb2_hDC)
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Gl.glGenTextures(1, mix_atlas_Id)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, mix_atlas_Id)

        If largestAnsio > 0 Then
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, largestAnsio)
        End If


        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
        'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
        'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
        'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)
        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
        Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
        Il.ilGetData()) '  Texture specification 

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Il.ilBindImage(0)
        ilu.iludeleteimage(texID)
        ' has to run on vertical and horz
        mix_atlas_Id = blur_image(mix_atlas_Id, "vert", True)
        mix_atlas_Id = blur_image(mix_atlas_Id, "horz", True)


    End Sub
    Public Function blur_image(ByRef image As Integer, orientatin As String, filter As Boolean) As Integer

        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Dim uc As vect2
        Dim lc As vect2
        'frmMain.pb2.Location = New Point(0, 0)
        'frmMain.pb2.BringToFront()
        'frmMain.pb2.Visible = True

        lc.x = frmMain.pb2.Width
        lc.y = -frmMain.pb2.Height  ' top to bottom is negitive
        uc.x = 0.0
        uc.y = 0.0
        ResizeGL_2()
        'ViewPerspective_2()
        ViewOrtho_2()
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glPushMatrix()
        Gl.glTranslatef(0.0, 0.0F, -0.1F)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
        Gl.glDisable(Gl.GL_LIGHTING)
        Dim comap, si As Integer
        si = Gl.glGetUniformLocation(shader_list.gaussian_shader, "blurScale")
        comap = Gl.glGetUniformLocation(shader_list.gaussian_shader, "s_texture")
        If orientatin = "vert" Then
            Gl.glUseProgram(shader_list.gaussian_shader)
            Gl.glUniform3f(si, 1.0 \ lc.x, 0.0, 0.0)
        Else
            Gl.glUseProgram(shader_list.gaussian_shader)
            Gl.glUniform3f(si, 0.0, 1.0 \ lc.y, 0.0)
        End If
        Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
        'Gl.glUseProgram(0)
        Gl.glUniform1i(comap, 0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, image)
        Dim e = Gl.glGetError

        Gl.glBegin(Gl.GL_TRIANGLES)
        '---
        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(uc.x, lc.y, -1.0!)

        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex3f(uc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(-1.0, 0.0)
        Gl.glVertex3f(lc.x, lc.y, -1.0!)
        '---
        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex3f(uc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(-1.0, 1.0)
        Gl.glVertex3f(lc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(-1.0, 0.0)
        Gl.glVertex3f(lc.x, lc.y, -1.0!)

        Gl.glEnd()
        Gl.glUseProgram(0)
        Gl.glPopMatrix()

        Gl.glDisable(Gl.GL_TEXTURE_2D)
        '--------------------
        ' rebuild texture
        Dim texID As Int32
        texID = Ilu.iluGenImage() ' Generation of one image name
        Il.ilBindImage(texID) ' Binding of image name 
        Dim success As Boolean = Il.ilTexImage(lc.x, -lc.y, 0, 4, Il.IL_BGRA, Gl.GL_UNSIGNED_BYTE, Nothing) '  Texture specification 
        If Not success Then
            Dim er = Il.ilGetError
            frmMain.tb1.Text = er.ToString

        End If
        Dim ptr_2 As IntPtr = Il.ilGetData()
        Gl.glReadPixels(0, 0, CInt(lc.x), CInt(-lc.y), Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, ptr_2)
        Ilu.iluMirror()

        'Ilu.iluFlipImage()
        'Ilu.iluBlurGaussian(2)
        Gdi.SwapBuffers(pb2_hDC)
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Gl.glDeleteTextures(1, image)
        Gl.glFinish()
        Gl.glGenTextures(1, image)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, image)
        If filter Then
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
        Else
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)

        End If
        'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
        'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
        'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)
        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
        Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
        Il.ilGetData()) '  Texture specification 
        ' Create the bitmap.
        'Dim bitmap = New System.Drawing.Bitmap(lc.x, -lc.y, PixelFormat.Format24bppRgb)
        If temp_bmp IsNot Nothing Then
            temp_bmp.Dispose()
            'GC.Collect()
            'GC.WaitForFullGCComplete()
        End If
        temp_bmp = New System.Drawing.Bitmap(lc.x, -lc.y, PixelFormat.Format24bppRgb)
        Dim rect As Rectangle = New Rectangle(0, 0, lc.x, -lc.y)

        ' Store the DevIL image data into the bitmap.
        Dim bitmapData As BitmapData = temp_bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb)

        Il.ilConvertImage(Il.IL_RGB, Il.IL_UNSIGNED_BYTE)
        Il.ilCopyPixels(0, 0, 0, lc.x, -lc.y, 1, Il.IL_RGB, Il.IL_UNSIGNED_BYTE, bitmapData.Scan0)
        temp_bmp.UnlockBits(bitmapData)
        'temp_bmp = bitmap.Clone
        temp_bmp.RotateFlip(RotateFlipType.RotateNoneFlipXY)
        'bitmap.Dispose()

        'success = Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes
        Il.ilBindImage(0)
        Ilu.iluDeleteImage(texID)
        'GC.Collect()
        'GC.WaitForFullGCComplete()
        'frmMain.pb2.Visible = False
        'frmMain.pb2.SendToBack()
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

    Public Sub build_terra(ByVal map As Int32)
        midz = 0 : midy = 0 : midx = 0
        ' Gl.glColor3f(0.6, 0.6, 0.6)
        Dim w As UInt32 = heightMapSize 'bmp_w
        Dim h As UInt32 = heightMapSize 'bmp_h
        Dim uvScale = (1.0# / 64.0#)
        Dim w_ = w / 2.0#
        Dim h_ = h / 2.0#
        Dim scale = 100.0 / (64.0#)
        Dim cnt As UInt32 = 0
        Gl.glBegin(Gl.GL_TRIANGLES)
        For j = 0 To w - 2
            For i = 0 To h - 2
                cnt += 1
                'Dim ans = (i Xor j) And 1
                'If i = 0 Or i = 63 Then
                '    Gl.glColor3f(1.0, 0.0, 0.0)
                'Else
                '    Gl.glColor3f(0.2, 0.2, 0.2)
                'End If
                'If j = 0 Or j = 63 Then
                '    Gl.glColor3f(0.0, 0.0, 1.0)
                'Else
                '    Gl.glColor3f(0.2, 0.2, 0.2)
                'End If
                midx += (i - w_)
                midy += (j - h_)
                midz += (bmp_data((i), (j)))

                topleft.x = (i) - w_
                topleft.y = (j) - h_
                topleft.z = bmp_data((i), (j))
                topleft.u = (i) * uvScale
                topleft.v = (j) * uvScale


                topRight.x = (i + 1) - w_
                topRight.y = (j) - h_
                topRight.z = bmp_data((i + 1), (j))
                topRight.u = (i + 1) * uvScale
                topRight.v = (j) * uvScale

                bottomRight.x = (i + 1) - w_
                bottomRight.y = (j + 1) - h_
                bottomRight.z = bmp_data((i + 1), (j + 1))
                bottomRight.u = (i + 1) * uvScale
                bottomRight.v = (j + 1) * uvScale

                bottomleft.x = (i) - w_
                bottomleft.y = (j + 1) - h_
                bottomleft.z = bmp_data((i), (j + 1))
                bottomleft.u = (i) * uvScale
                bottomleft.v = (j + 1) * uvScale

                make_world_triangle(topRight, bottomRight, topleft, scale, map)
                make_world_triangle(topleft, bottomRight, bottomleft, scale, map)
                '	If i = 62 Then
                '		Stop
                '	End If
            Next
        Next
        Gl.glEnd()
        midz = (midz / cnt)
        'look_point_Y = midz
        midx = (midx / cnt)
        look_point_X = midx
        midy = (midy / cnt)
        look_point_Z = midy
    End Sub
    Public Sub make_world_triangle(ByVal vt1 As vertex_data, ByVal vt2 As vertex_data, ByVal vt3 As vertex_data, ByRef scale As Single, ByVal map As Int32)
        tri_count += 1
        'add offsets
        vt1.x = (vt1.x * scale) + maplist(map).location.x
        vt1.y = (vt1.y * scale) + maplist(map).location.y
        vt2.x = (vt2.x * scale) + maplist(map).location.x
        vt2.y = (vt2.y * scale) + maplist(map).location.y
        vt3.x = (vt3.x * scale) + maplist(map).location.x
        vt3.y = (vt3.y * scale) + maplist(map).location.y
        Dim a, b, n As vect3

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
        Gl.glNormal3f(n.x, n.z, n.y)
        Gl.glTexCoord2f(vt1.u, vt1.v)
        Gl.glVertex3f(vt1.x, vt1.z, vt1.y)

        Gl.glNormal3f(n.x, n.z, n.y)
        Gl.glTexCoord2f(vt2.u, vt2.v)
        Gl.glVertex3f(vt2.x, vt2.z, vt2.y)
        'Gl.glNormal3b(vt2.nx, vt2.ny, vt2.nz)

        Gl.glNormal3f(n.x, n.z, n.y)
        Gl.glTexCoord2f(vt3.u, vt3.v)
        Gl.glVertex3f(vt3.x, vt3.z, vt3.y)
        'Gl.glNormal3b(vt3.nx, vt3.ny, vt3.nz)


    End Sub
    Private Sub make_strip_triangle(ByVal vt1 As vertex_data, ByVal vt2 As vertex_data, ByVal vt3 As vertex_data)
        tri_count += 1
        'add offsets
        Dim a, b, n As vect3

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
        'Dim delta = 0.0
        vt1.u *= -10.0
        vt1.v *= -10.0
        vt2.u *= -10.0
        vt2.v *= -10.0
        vt3.u *= -10.0
        vt3.v *= -10.0
        Gl.glNormal3f(n.x, n.z, n.y)
        Gl.glTexCoord2f(vt1.u, vt1.v)
        Gl.glVertex3f(vt1.x, vt1.z, vt1.y)
        'Gl.glNormal3b(vt1.nx, vt1.ny, vt1.nz)
        Gl.glNormal3f(n.x, n.z, n.y)
        Gl.glTexCoord2f(vt2.u, vt2.v)
        Gl.glVertex3f(vt2.x, vt2.z, vt2.y)
        'Gl.glNormal3b(vt2.nx, vt2.ny, vt2.nz)
        Gl.glNormal3f(n.x, n.z, n.y)
        Gl.glTexCoord2f(vt3.u, vt3.v)
        Gl.glVertex3f(vt3.x, vt3.z, vt3.y)
        'Gl.glNormal3b(vt3.nx, vt3.ny, vt3.nz)
        check_bounds(vt1)
        check_bounds(vt2)
        check_bounds(vt3)




    End Sub
    Private Sub check_bounds(ByVal v As vertex_data)
        If v.x > x_max Then x_max = v.x
        If v.x < x_min Then x_min = v.x
        If v.y > z_max Then z_max = v.y
        If v.y < z_min Then z_min = v.y
        If v.z > y_max Then y_max = v.z
        If v.z < y_min Then y_min = v.z

    End Sub
    Public Sub make_lists(ByVal map As UInt32)
        ' start list

        maplist(map).calllist_Id = Gl.glGenLists(1)
        Gl.glNewList(maplist(map).calllist_Id, Gl.GL_COMPILE)
        build_terra(map)
        Gl.glEndList()
        Gl.glFinish()

    End Sub

    Public Sub get_surface_normals(ByRef s As MemoryStream, ByVal map As Int32)
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

        Dim f = New BinaryReader(r)
        Dim magic = f.ReadUInt32()
        h_width = f.ReadUInt32
        h_height = f.ReadUInt32
        Dim comp = f.ReadUInt32
        Dim version = f.ReadUInt32
        Dim h_min = f.ReadSingle
        Dim h_max = f.ReadSingle
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
                For x = maplist(mapBoard(mboardX, mboardy)).location.x - 50 To _
                                            maplist(mapBoard(mboardX, mboardy)).location.x + 50 - (scale * 2) Step 1 * scale
                    tl = maplist(mapBoard(mboardX, mboardy + 1)).heights(x_pos, 0)
                    tr = maplist(mapBoard(mboardX, mboardy + 1)).heights(x_pos + 1, 0)
                    bl = maplist(mapBoard(mboardX, mboardy)).heights(x_pos, 63)
                    br = maplist(mapBoard(mboardX, mboardy)).heights(x_pos + 1, 63)
                    maplist(mapBoard(mboardX, mboardy)).heights(x_pos, 64) = tl

                    topleft.x = x
                    topleft.y = yu
                    topleft.z = tl
                    topleft.u = u_start
                    topleft.v = almost1

                    bottomleft.x = x
                    bottomleft.y = yl
                    bottomleft.z = bl
                    bottomleft.u = u_start
                    bottomleft.v = almost1 - uvinc

                    topRight.x = x + scale
                    topRight.y = yu
                    topRight.z = tr
                    topRight.u = u_start + uvinc
                    topRight.v = almost1

                    bottomRight.x = x + scale
                    bottomRight.y = yl
                    bottomRight.z = br
                    bottomRight.u = u_start + uvinc
                    bottomRight.v = almost1 - uvinc

                    make_strip_triangle(topRight, topleft, bottomleft)
                    make_strip_triangle(bottomleft, bottomRight, topRight)
                    u_start += uvinc
                    x_pos += 1
                    cur_x = x
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


                topRight.x = cur_x + (scale * 2)
                topRight.y = yl
                topRight.z = tr
                topRight.u = almost1
                topRight.v = almost1 - uvinc

                bottomleft.x = cur_x + scale
                bottomleft.y = yu
                bottomleft.z = bl
                bottomleft.u = almost1 - uvinc
                bottomleft.v = almost1 '

                bottomRight.x = cur_x + (scale * 2)
                bottomRight.y = yu
                bottomRight.z = br
                bottomRight.u = almost1 '
                bottomRight.v = almost1
                make_strip_triangle(bottomleft, topleft, topRight)
                make_strip_triangle(topRight, bottomRight, bottomleft)

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
                For y = maplist(mapBoard(mmx, mboardy)).location.y - 50 To _
                          maplist(mapBoard(mmx, mboardy)).location.y + 50 - (scale * 2) Step 1 * scale
                    tl = maplist(mapBoard(mmx, mboardy)).heights(63, y_pos + 1)
                    tr = maplist(mapBoard(mmx + 1, mboardy)).heights(0, y_pos + 1)
                    bl = maplist(mapBoard(mmx, mboardy)).heights(63, y_pos)
                    br = maplist(mapBoard(mmx + 1, mboardy)).heights(0, y_pos)
                    maplist(mapBoard(mmx, mboardy)).heights(64, y_pos) = br
                    topleft.x = xl
                    topleft.y = y + scale
                    topleft.z = tl
                    topleft.u = almost1 - uvinc
                    topleft.v = v_start + uvinc

                    bottomleft.x = xl
                    bottomleft.y = y
                    bottomleft.z = bl
                    bottomleft.u = almost1 - uvinc
                    bottomleft.v = v_start

                    topRight.x = xu
                    topRight.y = y + scale
                    topRight.z = tr
                    topRight.u = almost1
                    topRight.v = v_start + uvinc

                    bottomRight.x = xu
                    bottomRight.y = y
                    bottomRight.z = br
                    bottomRight.u = almost1
                    bottomRight.v = v_start


                    make_strip_triangle(topRight, topleft, bottomleft)
                    make_strip_triangle(bottomleft, bottomRight, topRight)
                    v_start += uvinc
                    y_pos += 1
                    cur_y = y
                Next
Endy:
                Gl.glEnd()
                Gl.glEndList()
                Gl.glFinish()
            Next
        Next




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

End Module
