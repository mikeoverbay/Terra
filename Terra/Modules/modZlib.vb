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
'
Imports Ionic
Module modZlib
    Public has_high_rez_map As Boolean = False
    Public mini_map_loaded As Boolean = False
    Private totalEntriesToProcess As Integer = 0
    Private main_color_texture As String = ""
    Public colour_correct_tex_id As Integer
    Public LIGHT_COUNT_ As Integer
    Public WATER_LINE_ As Single
    Public SmallLights_List_Id As Integer
    Public Function get_team_locations(ByRef name As String) As Boolean
        Dim ar = name.Split(".")
        Dim script_pkg = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\scripts.pkg")
        Dim script As Ionic.Zip.ZipEntry = script_pkg("scripts\arena_defs\" & ar(0) & ".xml")
        Dim path As String = GAME_PATH & "\res\scripts\arena_defs\" & ar(0) & ".xml"
        Dim ms As New MemoryStream
        script.Extract(ms)

        ms.Position = 0
        openXml_stream(ms, "")
        script_pkg.Dispose()

        ms.Dispose()
        Dim t As DataSet = xmldataset.Copy
        Dim bb As DataTable = t.Tables("boundingbox")
        Dim t1 As DataTable = t.Tables("team1")
        Dim t2 As DataTable = t.Tables("team2")
        Dim s1 As String = t1.Rows(0).Item(0)
        Dim s2 As String = t2.Rows(0).Item(0)
        Dim bb_bl As String = bb.Rows(0).Item(0)
        Dim bb_ur As String = bb.Rows(0).Item(1)
        If s1.Length = 1 Then
            s1 = t1.Rows(1).Item(2)
            s2 = t1.Rows(1).Item(2)
        End If
        t.Dispose()
        t1.Dispose()
        t2.Dispose()
        bb.Dispose()
        ar = s1.Split(" ")
        team_1.x = ar(0)
        team_1.y = 0.0
        team_1.z = ar(1)
        ar = s2.Split(" ")
        team_2.x = ar(0)
        team_2.y = 0.0
        team_2.z = ar(1)
        ar = bb_bl.Split(" ")
        Dim scaler As Single = 1.0  'this is debug testing for minimap scale issues.
        MAP_BB_UR.x = -ar(0) * scaler
        MAP_BB_BL.y = ar(1) * scaler
        ar = bb_ur.Split(" ")
        MAP_BB_BL.x = -ar(0) * scaler
        MAP_BB_UR.y = ar(1) * scaler
        If MAP_BB_UR.y > 1000 Or MAP_BB_BL.x < -1000 Then
            Dim mmscale = 0.1
            MAP_BB_UR.x *= mmscale
            MAP_BB_BL.y *= mmscale
            MAP_BB_BL.x *= mmscale
            MAP_BB_UR.y *= mmscale

        End If
        MAP_BB_BL.x -= 0.78
        MAP_BB_BL.y -= 0.78
        MAP_BB_UR.x -= 0.78
        MAP_BB_UR.y -= 0.78
        'Stop
        Return True
    End Function
    Public Sub save_light_settings()
        If Not maploaded Then
            Return
        End If

        Try
            Dim f = File.Open(Application.StartupPath + "/light_settings/" + load_map_name + ".light", FileMode.Create)
            Dim b_writer As New BinaryWriter(f)
            'the order: all as unsigend bytes
            '	1. texture level
            '	2. ambient
            '	3. gamma
            '	4. fog
            '	5. model level
            '	6. gray level
            '	7. extra
            b_writer.Write(frmLighting.s_terrain_texture_level.Value)
            b_writer.Write(frmLighting.s_terrain_ambient.Value)
            b_writer.Write(frmLighting.s_gamma.Value)
            b_writer.Write(frmLighting.s_fog_level.Value)
            b_writer.Write(frmLighting.s_model_level.Value)
            b_writer.Write(frmLighting.s_gray_level.Value)
            Dim ext As Integer = 1
            b_writer.Write(ext) ' extras in case we want to add values later and dont wanna trash the settings.
            b_writer.Write(ext)
            ' no need to read the unused bytes but the must be saved before a reload or the form closes.
            b_writer.Dispose()
            f.Close()
        Catch ex As Exception
            MsgBox("I was unable to save the lighting settings!", MsgBoxStyle.Exclamation, "file Access Error...")
        End Try

    End Sub
    Public Function get_light_settings() As Boolean

        If File.Exists(Application.StartupPath + "/light_settings/" + load_map_name + ".light") Then

            Dim f = File.Open(Application.StartupPath + "/light_settings/" + load_map_name + ".light", FileMode.Open)
            Dim b_reader As New BinaryReader(f)
            'the order: all as integer
            '	1. texture level
            '	2. ambient
            '	3. gamma
            '	4. fog
            '	5. model level
            '	6. gray level
            '	7. extra 2
            frmLighting.s_terrain_texture_level.Value = b_reader.ReadInt32
            frmLighting.s_terrain_ambient.Value = b_reader.ReadInt32
            frmLighting.s_gamma.Value = b_reader.ReadInt32
            frmLighting.s_fog_level.Value = b_reader.ReadInt32
            frmLighting.s_model_level.Value = b_reader.ReadInt32
            frmLighting.s_gray_level.Value = b_reader.ReadInt32
            ' no need to read the unused integers but they must be saved before loading a new map or the form closes.
            lighting_terrain_texture = frmLighting.s_terrain_texture_level.Value / 50.0!
            lighting_ambient = frmLighting.s_terrain_ambient.Value / 300.0!
            lighting_fog_level = frmLighting.s_fog_level.Value / 10000.0! ' yes 10,000
            lighting_model_level = frmLighting.s_model_level.Value / 100.0!
            gamma_level = (frmLighting.s_gamma.Value / 100) * 1.0!
            gray_level = 1.0 - (frmLighting.s_gray_level.Value / 100)
            b_reader.Dispose()
            f.Close()
            Return True
        Else
            lighting_terrain_texture = frmLighting.s_terrain_texture_level.Value / 50.0!
            lighting_ambient = frmLighting.s_terrain_ambient.Value / 300.0!
            lighting_fog_level = frmLighting.s_fog_level.Value / 10000.0! ' yes 10,000
            lighting_model_level = frmLighting.s_model_level.Value / 100.0!
            gamma_level = (frmLighting.s_gamma.Value / 100) * 1.0!
            gray_level = 1.0 - (frmLighting.s_gray_level.Value / 100)
            Return False

        End If

    End Function
    Private Sub clear_info()
        frmMapInfo.I__Map_Textures_tb.Clear()
        frmMapInfo.I__Model_Textures_tb.Clear()
        frmMapInfo.I__Tree_Textures_tb.Clear()
        frmMapInfo.I__Decal_Textures_tb.Clear()
        frmMapInfo.I__General_Info_tb.Clear()
    End Sub

    Private Sub prepare_mesh_buffers()
        'prepares the buffers used to create the mesh
        map_odd = False ' used to signal maps have odd side lengths .. IE. 9 x 9... 15 x 15
        global_map_width = Sqrt(test_count + 1).ToString
        Dim o = global_map_width And 1 ' check for odd size widths
        If o = 1 Then
            map_odd = True
        End If
        'Caluculate data size for mesh. This will be used to average normals, tangents and bi-tangents
        ReDim mesh(0) 'first, clear out any old data
        mesh(0) = New vertex_data
        GC.Collect()
        GC.WaitForFullGCComplete()
        ReDim Preserve mesh((global_map_width * global_map_width) * 4096) '  4096 verts per chunk 
        triangle_count = 0 ' very important lol!!
        GC.Collect()
    End Sub

    Public Function open_pkg(ByVal name As String) As Boolean

        'This function reads the data and creates the map, models, trees and everything else.
        'SHOW_MAPS = False

        terrain_loaded = False
        trees_loaded = False
        decals_loaded = False
        models_loaded = False
        bases_loaded = False
        sky_loaded = False
        water_loaded = False

        frmMain.flush_data() 'clear everything

        clear_info() 'clear all the text in frmMapInfo
        frmMapInfo.I__General_Info_tb.Text += "Map Name: " + name + vbCrLf + vbCrLf

        normal_mode = 0
        frmMain.d_counter = 0
        has_high_rez_map = False
        get_light_settings()
        frmMain.make_map_buttons()

        compass_display_list = read_directX_model(Application.StartupPath + "\Resources\dial.x")
        compass_tex_id = load_png_file(Application.StartupPath + "\Resources\linear_face.png")

        '================================================================
        'need to reset these so changes in the menu items take effect
        'Also.. they trigger what mode to display the terrain and models
        screen_totaled_draw_time = 0
        hz_loaded = False
        model_bump_loaded = False
        uv2s_loaded = False
        frmMain.resetBoundingBox()
        '================================================================
        frmOutput.Close()
        frmFind.Close()
        FrmGrid_Listing.Close()
        frmShowImage.Close()
        it_was_added = False
        lod0_swap = 0 : lod1_swap = 0 : lod2_swap = 0
        Dim sw As New Stopwatch
        sw.Start()
        get_team_locations(name)
        layer_texture_cache(0) = New texture_
        tree_textures(0) = New tree_textures_
        treeCache(0) = New flora_
        Dim contents As New List(Of String)
        Dim sec_length(1) As UInt32
        loaded_models._count = 0
        ReDim loaded_models.stack(1)
        loaded_models.names = New List(Of String)
        loaded_models.stack(0) = New mdl_stack


        '********************** this has to be managed!!!

        'this zip stays until the very end when everything is loaded
        'This file contains most of the images and models on all the maps
        Dim hd_name = name.Replace(".pkg", "_hd.pkg")
        shared_content_sandbox_part1 = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\shared_content_sandbox-part1.pkg")
        shared_content_sandbox_part2 = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\shared_content_sandbox-part2.pkg")
        shared_content_part1 = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\shared_content-part1.pkg")
        shared_content_part2 = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\shared_content-part2.pkg")
        Try
            shared_content_sandbox_part1_hd = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\shared_content_sandbox_hd-part1.pkg")
            shared_content_sandbox_part2_hd = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\shared_content_sandbox_hd-part2.pkg")
            shared_content_part1_hd = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\shared_content_hd-part1.pkg")
            shared_content_part2_hd = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\shared_content_hd-part2.pkg")
            active_pkg_hd = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\" & hd_name)
            If active_pkg_hd Is Nothing Then
                has_high_rez_map = False
            Else
                has_high_rez_map = True
            End If

        Catch ex As Exception

        End Try
        'didn't work out all that great
        'Using misc As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\misc.pkg")
        '    Dim crc As ZipEntry = misc("\system\maps\post_processing\colour_correct_default.dds")
        '    'Dim crc As ZipEntry = misc("\system\maps\post_processing\s_curve\s_curve.dds")
        '    If crc IsNot Nothing Then
        '        Dim crc_ms As New MemoryStream
        '        crc.Extract(crc_ms)
        '        colour_correct_tex_id = get_basic_texture(crc_ms)
        '    Else
        '        MsgBox("Unable to find colour correction texture", MsgBoxStyle.Exclamation, "Well Hell!")
        '    End If
        'End Using

        'Dim filesize = FileLen("GAME_PATH & " & name)
        'Dim thepkg(filesize) As Byte

        'Dim pkms As New MemoryStream(thepkg)

        full_map_name = GAME_PATH & "\res\packages\" & name
        If full_map_name.ToLower.Contains("tutorial") Then
            SHOW_RINGS = False : Else : SHOW_RINGS = True
        End If

        Dim cnt As UInt32 = 0

        'this zip stays until the very end when everything is loaded
        active_pkg = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\" & name)
        For Each e As ZipEntry In active_pkg
            contents.Add(e.FileName)
            cnt += 1
            Application.DoEvents()
        Next
        Dim a1() = name.Split(".")
        Dim abs_name As String = a1(0)
        '-----------------------------------------
        'open the space.bin file
        Dim space_bin_file As Ionic.Zip.ZipEntry = active_pkg("spaces/" & abs_name & "/space.bin")
        If space_bin_file IsNot Nothing Then
            ' This is all new code -------------------
            space_bin_file.Extract(Temp_Storage, ExtractExistingFileAction.OverwriteSilently)
            If Not ReadSpaceBinData(space_bin_file.FileName) Then
                space_bin_file = Nothing
                MsgBox("Error decoding Space.bin", MsgBoxStyle.Exclamation, "File Error...")
                dispose_package_zips()
                Return False
            End If
            space_bin_file = Nothing
        Else
            space_bin_file = Nothing
            MsgBox("Unable to load Space.bin", MsgBoxStyle.Exclamation, "File Error...")
            dispose_package_zips()
            Return False
        End If


        'If False Then
        '    Dim space_ms As New MemoryStream(space_bin_file.UncompressedSize)
        '    space_bin_file.Extract()
        '    space_bin_file.Extract(space_ms)
        '    Dim space_br As New BinaryReader(space_ms)
        '    Dim space_bin_data(space_ms.Length) As Byte
        '    space_bin_data = space_ms.GetBuffer
        '    get_spaceBin_data(space_bin_data)
        '    space_br.Close()
        '    space_ms.Dispose()
        'Else
        '    'MsgBox("Unable to load Space.bin", MsgBoxStyle.Exclamation, "File Error...")
        '    'active_pkg.Dispose()
        '    'shared_content_sandbox_part1.Dispose()
        '    'shared_content_part1.Dispose()
        '    'Return False
        'End If



        ' get settings xml.. this sets map sizes and such
        Dim st As Ionic.Zip.ZipEntry = active_pkg("spaces/" & abs_name & "/space.settings")
        Dim settings As New MemoryStream
        st.Extract(settings)
        openXml_stream(settings, abs_name)
        getMapSizes(abs_name) ' this also gets the skydome full path
        Dim minimap_name As String = ""
        Dim vlo_name As String = ""
        Dim skyDomeName As String = ""
        Dim floraXML_Name As String = ""
        cnt = 0
        Dim count2 As UInt32 = 0
        speedtree_map = ""
        speedtree_normalmap = ""
        speedtree_name = ""
        '*******************************************
        'find all the chunk pieces.. and full paths of important items.
        For pos = 0 To contents.Count - 1
            If contents(pos).Contains("flora.xml") Then
                floraXML_Name = contents(pos).ToString
            End If
            If contents(pos).Contains("mmap.dds") Then
                minimap_name = contents(pos).ToString
            End If
            If contents(pos).Contains(".vlo") Then
                vlo_name = contents(pos).ToString
            End If

            If contents(pos).Contains(".cdata") Then

                sec_list(cnt) = contents(pos)
                ''maplist(cnt) = New grid_sec
                Dim a11() = contents(pos).Split("/")
                Dim a2() = a11(a11.Length - 1).Split(".")
                maplist(cnt).name = a2(0)
                cnt += 1
                ReDim Preserve maplist(cnt)
                ReDim Preserve sec_list(cnt)
            End If
            If contents(pos).ToLower.Contains("global_am.dds") Then
                main_color_texture = contents(pos).ToString
            End If
dont_grab_this:
        Next
        'Ok this is a hack to get the speedtree info from the shared_content.pkg


        If InStr(name, "11_murovanka") > 0 Or InStr(name, "22_slough") > 0 Then
            If speedtree_map = "" Then
                speedtree_map = "speedtree\11_Murovanka\compositemap_diffuse.dds"
            End If
            If speedtree_name = "" Then
                speedtree_name = "speedtree\11_Murovanka\CompositeMap.txt"
            End If
        End If
        If InStr(name, "14_siegfried_line") > 0 Then
            If speedtree_map = "" Then
                speedtree_map = "speedtree\02_Malinovka\compositemap_diffuse.dds"
            End If
            If speedtree_name = "" Then
                speedtree_name = "speedtree\02_Malinovka\CompositeMap.txt"
            End If
        End If
        If InStr(name, "04_himmelsdorf") > 0 Then
            If speedtree_map = "" Then
                speedtree_map = "speedtree\04_himmelsdorf\compositemap_diffuse.dds"
            End If
            If speedtree_name = "" Then
                speedtree_name = "speedtree\04_himmelsdorf\CompositeMap.txt"
            End If
        End If
        If InStr(name, "02_malinovka") > 0 Then
            If speedtree_map = "" Then
                speedtree_map = "speedtree\02_Malinovka\compositemap_diffuse.dds"
            End If
            If speedtree_name = "" Then
                speedtree_name = "speedtree\02_Malinovka\CompositeMap.txt"
            End If
        End If
        speedtree_normalmap = speedtree_map.Replace("Diffuse", "Normal")
        If speedtree_normalmap = speedtree_map Then
            speedtree_normalmap = speedtree_map.Replace("diffuse", "normal")
        End If
        ReDim mapBoard(Sqrt(maplist.Length - 2) + 1, Sqrt(maplist.Length - 1) + 2) 'size the map board
        'frmMapInfo.I__Tree_Textures_tb.Text += "Speed Tree Composite: " + speedtree_map + vbCrLf
        'frmMapInfo.I__Tree_Textures_tb.Text += "Speed Tree Composite: " + speedtree_normalmap + vbCrLf
        'frmMapInfo.I__Tree_Textures_tb.Text += "=====================================================================" + vbCrLf
        'Initilze the maplist data
        '************************************************
        Models = New model_
        ReDim Models.models(1)
        Models.models(0) = New primitive
        ReDim Models.models(0).componets(1)
        ReDim Models.Model_list(0)
        Models.models(0).componets(0) = New Model_Section
        Models.Model_list(0) = ""

        Application.DoEvents()

        '--------------------------------------
        'test to get flora.xml so I can look at its contents
        'currently, this isnt working.. Cant save the dataset due to root issue.
        'Dim fmp As New MemoryStream
        'Dim fxl As Ionic.Zip.ZipEntry = active_pkg(floraXML_Name)
        'fxl.Extract(fmp)
        'openXml_stream(fmp, "flora")
        'fmp.Dispose()
        '---------------------------------------
        'get the minimap image
        '====================================================
        Dim mmp As New MemoryStream
        Dim mmentry As Ionic.Zip.ZipEntry = active_pkg(minimap_name)
        mmentry.Extract(mmp)
        minimap_textureid = get_texture(mmp, False)
        mmp.Dispose()
        GC.Collect()

        '====================================================
        get_minimap_tank_icons()
        GC.Collect()
        '----------------- 
        '====================================================
        'let the user know whats going on
        cnt = 0
        tb1.text = "Extracting Data from .pkg files..."
        'get all the chunks and cdata parts and save them
        For p = 0 To sec_list.Length - 2
            ReDim maplist(cnt).cdata(1)
            Dim ms2 As New MemoryStream()
            Dim entry2 As Ionic.Zip.ZipEntry = active_pkg(sec_list(p).Replace("chunk", "cdata"))
            ReDim maplist(cnt).cdata(entry2.UncompressedSize)
            entry2.Extract(ms2)
            maplist(cnt).cdata = ms2.GetBuffer
            cnt += 1
            ms2.Dispose()
        Next

        contents.Clear() 'free up memory
        GC.Collect()

        test_count = maplist.Length - 2


        '******************************************************************************
        prepare_mesh_buffers() ' This is crashing on very large maps
        '******************************************************************************

        Dim mapsize As Integer = (Sqrt(maplist.Length - 1)) * 100

        frmMapInfo.I__General_Info_tb.Text += "Map Size: " + mapsize.ToString + " x " + mapsize.ToString + vbCrLf + vbCrLf
        'test_count = 1
        Dim map_start = 0
        Dim test2 As Int32 = test_count
        '******************************************************************************
        ' gotta setup for skydome.. Im using the modlist and will copy and reset the info when done with it
        load_skydome()

        '******************************************************************************
        'setup progress bar
        frmMain.ProgressBar1.Visible = True
        frmMain.ProgressBar1.Maximum = maplist.Length - 2
        frmMain.ProgressBar1.Minimum = 0
        frmMain.ProgressBar1.Value = 0
        Application.DoEvents()
        'this part gets the grid parts (terrain2) and builds the world.
        ReDim map_layers(test2)
        '**********************************************
        '**********************************************
        '**********************************************
        load_terrain(name)
        GC.Collect()
        GC.WaitForFullGCComplete() 'release memory.
        '**********************************************
        '**********************************************
        load_models()
        GC.Collect()
        GC.WaitForFullGCComplete()
        '**********************************************
        '**********************************************
        'load_trees()
        GC.Collect()
        GC.WaitForFullGCComplete()
        '**********************************************
        '**********************************************
        load_decals()
        'code to spilt up road_map.bin
        Dim r_m As Ionic.Zip.ZipEntry = active_pkg("spaces/" & abs_name & "/road_map.bin")
        If r_m IsNot Nothing Then
            split_road_map(r_m)
        End If
        GC.Collect()
        GC.WaitForFullGCComplete()
        '**********************************************
        '**********************************************

        '-----------------------------------
        If m_bases_ Then
            bases_loaded = True
        End If

        '**********************************************
        glob_str = ""
        'clean up data.. recover memory.
        For map = map_start To test_count
            ReDim maplist(map).cdata(1) ' clear
            GC.Collect()
        Next
        '*******************************************************************************************


        '*******************************************************************************************

        '*******************************************************************************************

        frmMapInfo.I__General_Info_tb.Text += "Decals: " + Format("0000", (decal_matrix_list.Length + road_decal_count) - 1) + vbCrLf
        frmMapInfo.I__General_Info_tb.Text += "Trees: " + Format("0000", speedtree_matrix_list.Length - 1) + vbCrLf
        frmMapInfo.I__General_Info_tb.Text += "Models: " + Format("0000", Model_Matrix_list.Length - 1) + vbCrLf

        'We are going to create the water now.. before closing the PKGs.. may want to get
        'textures from them??
        If m_water_ Then
            water_loaded = True
            If cBWWa.bwwa_t1(0).width > 0 Then
                water.IsWater = True
                build_water()
                'make_water_mesh() 'experimental.. Too slow to use as is
                water.textureID = Load_DDS_File(Application.StartupPath + "\Resources\water2.dds")
                water.normalID = Load_DDS_File(Application.StartupPath + "\Resources\water2_NM2.dds")
                GC.Collect()
            End If
        End If
        '##################################################################
        'These packages are HUGE!.. I need to find a way to read as needed.
        dispose_package_zips()
        '##################################################################

        frmMain.ProgressBar1.Visible = False
        'let the renderer know the models are loaded and ready
        sw.Stop() ' stopwatch for load time
        Cam_Y_angle = PI * -0.125
        Cam_X_angle = PI * 0.25
        look_point_X = 0.01
        look_point_Z = 0.01
        View_Radius = -150.0
        Dim tm = sw.Elapsed
        Dim t = Format(tm.Seconds, "00")
        Dim mt = Format(tm.Minutes, "00")
        tb1.text = "Load time " + mt + ":" + t + vbCrLf
        tb1.text += "Reused Textures: " + saved_texture_loads.ToString + vbCrLf
        tb1.text += "Reused Models: " + saved_model_loads.ToString + vbCrLf
        tb1.text += "lod2 used: " + lod2_swap.ToString + _
                                        "   lod1 used: " + lod1_swap.ToString + _
                                        "   lod0 used: " + lod0_swap.ToString

        make_lights()
        'For i = 0 To (light_count - 1) * 3 Step 3
        '    sl_light_pos(i + 1) = get_Z_at_XY(sl_light_pos(i + 0), sl_light_pos(i + 2)) + 10.0
        'Next
        find_street_lights()
        frmMain.pb1.Focus()
        maploaded = True ' entire map is ready for display.
        frmMain.position_camera() ' this should cause a redraw
        Dim radius As Single = MAP_BB_UR.x
        Dim lx_light = Cos(0.707) * radius
        Dim lz_light = Sin(0.707) * radius
        Dim ly_light = 400.0 'Z_Cursor = get_Z_at_XY(look_point_X, look_point_Z) ' + 1
        'ly_light = get_Z_at_XY(lx_light, lz_light) + 150.0
        position(0) = lx_light 'u_look_point_X - lx
        position(1) = ly_light 'u_look_point_Y + 10 'ly
        position(2) = lz_light 'u_look_point_Z - lz
        position(3) = 1.0
        frmMain.need_screen_update()
    End Function

    Public Sub dispose_package_zips()
        'disposes all the package data zip files
        active_pkg.Dispose()    ' VERY IMPORTANT !!!
        shared_content_sandbox_part1.Dispose() ' VERY IMPORTANT !!!
        shared_content_sandbox_part2.Dispose() ' VERY IMPORTANT !!!
        shared_content_part1.Dispose() ' VERY IMPORTANT !!!
        shared_content_part2.Dispose() ' VERY IMPORTANT !!!
        Try
            active_pkg_hd.Dispose() ' VERY IMPORTANT !!!
        Catch ex As Exception
        End Try
        Try
            shared_content_sandbox_part1_hd.Dispose() ' VERY IMPORTANT !!!
            shared_content_sandbox_part2_hd.Dispose() ' VERY IMPORTANT !!!
        Catch ex As Exception
        End Try
        Try
            shared_content_part1_hd.Dispose() ' VERY IMPORTANT !!!
            shared_content_part2_hd.Dispose() ' VERY IMPORTANT !!!
        Catch ex As Exception
        End Try
        GC.Collect()
    End Sub

    Public Sub find_street_lights()
        Dim l_cnt As Integer = 0
        LIGHT_COUNT_ = 0
        make_lights()
        For i = 0 To Model_Matrix_list.Length - 2
            With Model_Matrix_list(i)
                If l_cnt / 3 >= max_light_count - 3 Then
                    Return ' no slots left for lights
                End If
                If Model_Matrix_list(i).primitive_name IsNot Nothing Then

                    If Model_Matrix_list(i).primitive_name.Contains("hd_env_EU_040_StreetLamp_02") Then
                        Dim v As vect3
                        '1
                        v.x = 0.0
                        v.y = 3.7
                        v.z = 0.5
                        v = translate_to(v, .matrix)
                        sl_light_pos(l_cnt) = v.x
                        sl_light_pos(l_cnt + 1) = v.y
                        sl_light_pos(l_cnt + 2) = v.z
                        l_cnt += 3
                        LIGHT_COUNT_ = l_cnt / 3

                        '2
                        v.x = 0.0
                        v.y = 4.1
                        v.z = 0.0
                        v = translate_to(v, .matrix)
                        sl_light_pos(l_cnt) = v.x
                        sl_light_pos(l_cnt + 1) = v.y
                        sl_light_pos(l_cnt + 2) = v.z
                        l_cnt += 3
                        LIGHT_COUNT_ = l_cnt / 3

                        '3
                        v.x = 0.0
                        v.y = 3.7
                        v.z = -0.5
                        v = translate_to(v, .matrix)
                        sl_light_pos(l_cnt) = v.x
                        sl_light_pos(l_cnt + 1) = v.y
                        sl_light_pos(l_cnt + 2) = v.z
                        l_cnt += 3
                        LIGHT_COUNT_ = l_cnt / 3
                        Continue For

                    End If
                    sl_light_color(l_cnt + 0) = 1.0
                    sl_light_color(l_cnt + 1) = 1.0
                    sl_light_color(l_cnt + 2) = 0.7

                    If Model_Matrix_list(i).primitive_name.ToLower.Contains("env413_streetlamp1") Then
                        Dim v As vect3
                        v.x = 0.0
                        v.y = 2.99
                        v.z = 0.0
                        v = translate_to(v, .matrix)
                        sl_light_pos(l_cnt) = v.x
                        sl_light_pos(l_cnt + 1) = v.y
                        sl_light_pos(l_cnt + 2) = v.z
                        l_cnt += 3
                        LIGHT_COUNT_ = l_cnt / 3
                    End If
                    If Model_Matrix_list(i).primitive_name.ToLower.Contains("env413_streetlamp2") Then
                        Dim v As vect3
                        v.x = .matrix(12)
                        v.y = .matrix(13) + 2.9
                        v.z = .matrix(14)
                        sl_light_pos(l_cnt) = v.x
                        sl_light_pos(l_cnt + 1) = v.y
                        sl_light_pos(l_cnt + 2) = v.z
                        l_cnt += 3
                        LIGHT_COUNT_ = l_cnt / 3
                    End If
                    If Model_Matrix_list(i).primitive_name.ToLower.Contains("env413_streetlamp3") Then
                        Dim v As vect3
                        v.x = 0
                        v.y = 4.45
                        v.z = -0.5
                        v = translate_to(v, .matrix)
                        sl_light_pos(l_cnt) = v.x
                        sl_light_pos(l_cnt + 1) = v.y
                        sl_light_pos(l_cnt + 2) = v.z
                        l_cnt += 3
                        LIGHT_COUNT_ = l_cnt / 3
                    End If
                    If Model_Matrix_list(i).primitive_name.ToLower.Contains("env413_streetlamp4") Then
                        Dim v As vect3
                        v.x = 0.1
                        v.y = 2.9
                        v.z = 0
                        v = translate_to(v, .matrix)
                        sl_light_pos(l_cnt) = v.x
                        sl_light_pos(l_cnt + 1) = v.y
                        sl_light_pos(l_cnt + 2) = v.z
                        l_cnt += 3
                        LIGHT_COUNT_ = l_cnt / 3
                    End If
                    '-------------------
                End If 'name not nothing
            End With
        Next
        Gl.glDeleteLists(SmallLights_List_Id, 1)
        SmallLights_List_Id = Gl.glGenLists(1)
        Gl.glNewList(SmallLights_List_Id, Gl.GL_COMPILE)

        Gl.glBegin(Gl.GL_POINTS)
        For i = 0 To LIGHT_COUNT_ * 3 Step 3
            Gl.glTexCoord3f(sl_light_color(i + 0), sl_light_color(i + 1), sl_light_color(i + 2))
            Gl.glVertex3f(sl_light_pos(1 + 0), sl_light_pos(i + 1), sl_light_pos(i + 2))
        Next
        Gl.glEnd()
        Gl.glEndList()
    End Sub


    Private Sub load_terrain(ByVal name As String)
        Dim dms As New MemoryStream
        Dim abs_name = Path.GetFileNameWithoutExtension(name)
        'Dim decalBin As Ionic.Zip.ZipEntry = active_pkg("spaces/" & abs_name & "/decals.bin")
        'decalBin.Extract(dms)
        'get_decal_bin(dms) '




        get_map_extremes()
        Dim m_layers As Boolean = frmMain.m_high_rez_Terrain.Checked
        tb1.text = "Getting Terrain Data..."
        If m_terrain_ Then
            terrain_loaded = True
            For map = 0 To test_count
                'let the user know whats going on
                Application.DoEvents()
                Dim cms As New MemoryStream(maplist(map).cdata)
                Dim cdata As New MemoryStream
                Dim tms As New MemoryStream
                Dim norms As New MemoryStream
                Dim dom As New MemoryStream
                Dim holes_ms As New MemoryStream
                Using ck As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read(cms)
                    ReDim maplist(map).scr_coords(3) ' used with unproject
                    Dim heights As Ionic.Zip.ZipEntry = ck("terrain2/heights")
                    heights.Extract(cdata)
                    read_heights(cdata, map)

                    'Dim texture As Ionic.Zip.ZipEntry = ck("terrain2/lodTexture.dds")
                    'texture.Extract(tms)
                    'Using btm As New Bitmap(build_textures(map, tms).Clone, 64, 64)
                    '    maplist(map).bmap = btm.Clone 'used only for grid listing utility
                    'End Using
                    'maplist(map).bmap = build_textures(map, tms).Clone
                    GC.Collect()
                    Dim holes As Ionic.Zip.ZipEntry = ck("terrain2/holes")
                    ReDim maplist(map).holes(63, 63) ' set to 0 when on ReDim
                    If holes IsNot Nothing Then
                        maplist(map).has_holes = 1
                        holes.Extract(holes_ms)
                        open_hole_info(map, holes_ms)
                    Else
                        maplist(map).has_holes = 0
                    End If
                    ' dont know how to use it :(
                    Dim dominate As Ionic.Zip.ZipEntry = ck("terrain2/dominanttextures")
                    dominate.Extract(dom)
                    get_dominate_texture(map, dom) 'gets the dominate texture map

                    get_location(map)   'finds location in the world
                    build_terra(map) 'builds the geometry
                    frmMain.ProgressBar1.Value = map
                    Application.DoEvents()
                    ck.Dispose()
                End Using
                cms.Dispose()
                cdata.Dispose()
                tms.Dispose()
                norms.Dispose()
                dom.Dispose()
                holes_ms.Dispose()
                GC.Collect() ' try to force release of memory.
            Next
            'Debug.Write(testSR.ToString)
            GC.WaitForFullGCComplete() ' wait until memory is released. We need it.
            '**********************************************
            'File.WriteAllText("C:\MapLayerList.txt", testSR.ToString)
            'lets seam the World (creates map seam in between the chunks)
            seam_map()
            'Debug.WriteLine("old tri_count:" + tri_count.ToString)
            '=============================================================
            'Terrain rework stuff
            'At this point. the giant mesh "mesh" has been create.
            'We need to get the Tangents BiTangents and Normals
            '
            'order IS important :)
            tb1.text = "Creating the Terrain's Normals, Tangents and BiTangents."
            Application.DoEvents()
            createTBNs()
            'ReDim Preserve triangle_holder(triangle_count)
            tb1.text = "Averging the surface normals..."
            Application.DoEvents()
            average_mesh_btns()
            tb1.text = "Creating chunk display lists..."
            Application.DoEvents()
            make_chunk_meshes() ' this  breaks the big mesh in to small chunk meshes
            tb1.text = "Cleaning up memory..."
            Application.DoEvents()
            ReDim mesh(0) 'clear
            'ReDim triangle_holder(0) 'clear
            GC.Collect()
            GC.WaitForFullGCComplete(5000)
            '=============================================================
            ' this is where we will make the blend texture and slice the huge color_tex in to its chuck areas.
            Try
                If m_layers Then

                    hz_loaded = True ' this is to let the user know that switch to hirez does not work if the map was not loaded in hirez
                    'get the layers
                    frmMain.ProgressBar1.Maximum = test_count
                    frmMain.ProgressBar1.Minimum = 0
                    frmMain.ProgressBar1.Value = 0
                    Dim found_ As Boolean = False
                    layer_uv_list = "" 'clear this
                    get_main_texture = True
                    tb1.text = "Getting the main Terrain Texture.  This may take a while...."
                    Application.DoEvents()
                    Dim main_map As Ionic.Zip.ZipEntry = Nothing
                    main_map = active_pkg(main_color_texture)

                    'If name.Contains("19_") Then
                    '    main_map = active_pkg("\maps\landscape\19_Kurgan\color_tex.dds")
                    'Else
                    '    main_map = active_pkg("\maps\landscape\" + name.Replace(".pkg", "") + "\color_tex.dds")
                    'End If
                    Dim main_map_ms As New MemoryStream
                    If main_map Is Nothing Then
                        MsgBox("Can't find main texture")
                    End If
                    main_map.Extract(main_map_ms)
                    'MsgBox("Debug stop")
                    Dim w = Sqrt(maplist.Length - 1)
                    'Convert memorystream in to the color_tex image id.
                    Dim main_tex_id As Integer = get_main_tex_texture_id(main_map_ms, w)
                    main_map_ms.Dispose()
                    GC.Collect()

                    'This reads all the data from each chunks .cdata and extracts the layer and mixmap info.
                    tb1.text = "Getting the Terrain layer information and mixmaps..."
                    For map = 0 To test_count
                        Application.DoEvents()
                        Dim cms As New MemoryStream(maplist(map).cdata)
                        Using ck As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read(cms)
                            map_layers(map) = New layer_ ' create the structure to store data
                            Dim layer_count As Integer = get_layers(ck, map)
                        End Using
                        cms.Dispose()
                        GC.Collect()
                        frmMain.ProgressBar1.Value = map
                        Application.DoEvents()
                    Next
                    'reset progressbar
                    frmMain.ProgressBar1.Maximum = maplist.Length - 2
                    frmMain.ProgressBar1.Minimum = 0
                    frmMain.ProgressBar1.Value = 0

                    'create render mask for terrainDEF_Shader
                    For i = 0 To test_count

                        Dim mask As Integer = 0
                        If map_layers(i).layers(1).l_name <> "" Then
                            mask = mask Or 1
                        End If
                        If map_layers(i).layers(1).l_name2 <> "" Then
                            mask = mask Or 2
                        End If
                        If map_layers(i).layers(2).l_name <> "" Then
                            mask = mask Or 4
                        End If
                        If map_layers(i).layers(2).l_name2 <> "" Then
                            mask = mask Or 8
                        End If
                        If map_layers(i).layers(3).l_name <> "" Then
                            mask = mask Or 16
                        End If
                        If map_layers(i).layers(3).l_name2 <> "" Then
                            mask = mask Or 32
                        End If
                        If map_layers(i).layers(4).l_name <> "" Then
                            mask = mask Or 64
                        End If
                        If map_layers(i).layers(4).l_name2 <> "" Then
                            mask = mask Or 128
                        End If
                        map_layers(i).texture_mask = mask
                    Next
                    'create the blured mix textures.
                    'Rewrote to use single textures with 8 padding all around for bluring!!
                    For i = 1 To 4
                        create_mixMaps(i)
                    Next

                    GC.Collect()
                    GC.WaitForFullGCComplete()
                    '
                    frmMain.ProgressBar1.Maximum = maplist.Length - 2
                    frmMain.ProgressBar1.Minimum = 0
                    frmMain.ProgressBar1.Value = 0

                    split_up_main_texture(w, main_tex_id)

                    GC.Collect()
                    GC.WaitForFullGCComplete()

                    'Debug.WriteLine("========================")
                    frmMain.pb2.Visible = False
                    frmMain.pb2.SendToBack()

                End If ' if m_layers
            Catch ex As Exception
                MsgBox(ex.ToString + vbCrLf + "Crashed at loading the terrain!", MsgBoxStyle.Exclamation, "Well Shit!")
            End Try
        End If ' m_terrain_


    End Sub

    Private Sub load_models()
        If m_models_ Then
            models_loaded = True
            'setup progress bar
            frmMain.ProgressBar1.Value = 0
            frmMain.ProgressBar1.Maximum = Model_Matrix_list.Length
            ReDim Models.models(Model_Matrix_list.Length)
            Models.models(0) = New primitive
            Models.model_count = Model_Matrix_list.Length - 2
            'make all the models.
            tb1.text = "Loading the models..."
            For m = 0 To Model_Matrix_list.Length - 2
                If True Then
                    'stuff we dont want on the map.
                    If Model_Matrix_list(m).exclude Then
                        GoTo skip_this
                    End If
                    If Model_Matrix_list(m).primitive_name.ToLower.Contains("wgl_banner") _
                        Or Model_Matrix_list(m).primitive_name.ToLower.Contains("00000_base") _
                        Or Model_Matrix_list(m).primitive_name.ToLower.Contains("particles") Then
                        GoTo skip_this
                    End If
                    frmMain.ProgressBar1.Value = m
                    ReDim Preserve Models.Model_list(m + 1)
                    Models.Model_list(m) = Model_Matrix_list(m).primitive_name
                    Models.models(m) = New primitive
                    ReDim Models.models(m).componets(1)
                    Models.models(m).componets(0) = New Model_Section
                    Models.models(m).componets(0).color_id = -1
                    ReDim Preserve Models.matrix(m + 1)
                    ReDim Models.matrix(m).matrix(16)
                    Models.matrix(m).matrix = Model_Matrix_list(m).matrix
                    'some of the matrix has to be inverted because of Opengl/DirectX issues
                    'Models.matrix(m).matrix(1) *= -1.0
                    'Models.matrix(m).matrix(2) *= -1.0
                    'Models.matrix(m).matrix(4) *= -1.0
                    'Models.matrix(m).matrix(8) *= -1.0
                    'Models.matrix(m).matrix(12) *= -1.0
                    get_primitive(m, active_pkg)
                Else

                End If
skip_this:
                Application.DoEvents()
            Next
        End If

    End Sub

    Private Sub load_trees()

        If m_trees_ Then
            trees_loaded = True
            frmMain.ProgressBar1.Value = 0
            frmMain.ProgressBar1.Maximum = speedtree_matrix_list.Length

            Trees = New Tree_s
            ReDim Trees.flora(0)
            ReDim treeCache(0)
            treeCache(0) = New flora_
            ReDim Trees.Tree_list(speedtree_matrix_list.Length)
            'make all the trees and shrubs... 
            tb1.text = "Loading the trees..."
            For tree = 0 To speedtree_matrix_list.Length - 2
                frmMain.ProgressBar1.Value = tree
                'same as with models.. we must invert some of the matrix.
                speedtree_matrix_list(tree).matrix(1) *= -1.0
                speedtree_matrix_list(tree).matrix(2) *= -1.0
                speedtree_matrix_list(tree).matrix(4) *= -1.0
                speedtree_matrix_list(tree).matrix(8) *= -1.0
                speedtree_matrix_list(tree).matrix(12) *= -1.0

                Trees.Tree_list(tree) = speedtree_matrix_list(tree).tree_name
                ReDim Preserve Trees.matrix(tree + 1)
                ReDim Trees.matrix(tree).matrix(15)

                Trees.matrix(tree).matrix = speedtree_matrix_list(tree).matrix
                build_tree(tree, speedtree_matrix_list(tree).tree_name)
                Application.DoEvents()
            Next
        End If
        'Make the pr0jected decals' I need to speed this process up!

    End Sub

    Private Sub load_decals()
        If m_decals_ Then
            tb1.text = "Getting Decal textures and data...." 'talk to user
            decals_loaded = True
            make_new_decals()
            GC.Collect()
            GC.WaitForFullGCComplete()
            'build_decals() ' finish making the decals.. this involves projecting them on to the terrain and grabbing textures.
        End If

    End Sub

    Private Sub load_skydome()


        If m_sky_ Then
            sky_loaded = True
            tb1.Text = "Making Skydome..."
            setupthedome()
            UVs_ON = True ' so that uvs will be created
            Dim sk As New MemoryStream
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            'manually replace skydomes that are not in the file we are loading.
            If Models.Model_list(0).Contains("lost_city_sky") Then
                Models.Model_list(0) = "maps/skyboxes/28_Desert_sky/skydome/skybox.model"
            End If
            Try
                get_primitive(0, active_pkg)
            Catch ex As Exception
                MsgBox(ex.Message + vbCrLf + "Skydome model did not load!", MsgBoxStyle.Exclamation, "Load error...")
            End Try
            '==================================================================

            UVs_ON = False
            sk.Dispose()
            skydometextureID = Models.models(0).componets(0).color_id
            skydomelist = Models.models(0).componets(0).callList_ID
            Dim sk1 = New MemoryStream
            If Models.Model_list(0).Contains("lost_city_sky") Then
                Models.models(0).componets(0).color_name = "maps\skyboxes\28_Desert_sky\skydome\clouds.dds"
                Models.models(0).componets(0).color_name = _
                        Replace(Models.models(0).componets(0).color_name, "//", "/")
                Dim SKentry2 As Ionic.Zip.ZipEntry = active_pkg(Models.models(0).componets(0).color_name)
                skydomelist = Models.models(0).componets(0).callList_ID
                If SKentry2 Is Nothing Then
                    SKentry2 = get_shared(Models.models(0).componets(0).color_name)
                    If SKentry2 Is Nothing Then
                    Else
                        Try
                            'last ditch at getting this dome.. if it fails... this map wont have a skydome
                            SKentry2.Extract(sk1)
                            skydometextureID = get_texture(sk1, False)
                        Catch ex As Exception

                        End Try
                    End If
                Else
                    SKentry2.Extract(sk1)
                    skydometextureID = get_texture(sk1, False)
                End If
            End If
            sk.Dispose()
        End If
        'reset this cuz we are dont want leftover IDs
        Models.models(0).componets(0) = New Model_Section
        Models.models(0).componets(0).callList_ID = -1
        Models.Model_list(0) = ""
        Models.models(0)._count = 0
        'Need to clear this as well.. one gets shoved in by the skybox
        'and we dont want it!!
        ReDim loaded_models.stack(1)
        loaded_models.names = New List(Of String)
        loaded_models.stack(0) = New mdl_stack
        loaded_models._count = 0
        'mini_map and Sky dome are loaded... Tell app they are.
        mini_map_loaded = True

    End Sub

    Private Sub get_minimap_tank_icons()

        tb1.Text = "Getting minimap tank Icons..."
        ' get the little tank icons for the minimap
        Dim gui = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\gui.pkg")
        Dim sp = "\gui\maps\icons\filters\tanks\"
        'heavy
        Dim icon_ms As New MemoryStream
        Dim icon_entry As Ionic.Zip.ZipEntry = gui(sp + "heavyTank.png")
        icon_entry.Extract(icon_ms)
        tank_mini_icons(2) = load_png(icon_ms)
        'med
        icon_ms = New MemoryStream
        icon_entry = gui(sp + "mediumTank.png")
        icon_entry.Extract(icon_ms)
        tank_mini_icons(1) = load_png(icon_ms)
        'light
        icon_ms = New MemoryStream
        icon_entry = gui(sp + "lightTank.png")
        icon_entry.Extract(icon_ms)
        tank_mini_icons(0) = load_png(icon_ms)
        'td
        icon_ms = New MemoryStream
        icon_entry = gui(sp + "AT-SPG.png")
        icon_entry.Extract(icon_ms)
        tank_mini_icons(3) = load_png(icon_ms)
        'spg
        icon_ms = New MemoryStream
        icon_entry = gui(sp + "SPG.png")
        icon_entry.Extract(icon_ms)
        tank_mini_icons(4) = load_png(icon_ms)
        'this reads the custom minimap border I made.
        Dim mini_data() = File.ReadAllBytes(Application.StartupPath + "\resources\minimap_frame.png")
        icon_ms = New MemoryStream(mini_data)
        minimsp_frameT_TextureId = load_png(icon_ms)
        icon_ms.Dispose()
        gui.Dispose()
        GC.Collect()
        minimap_size = My.Settings.minimap_size


    End Sub

    Public Function get_shared(name As String) As Ionic.Zip.ZipEntry
        'search all packages for this item
        Dim et As Ionic.Zip.ZipEntry = Nothing
        Dim et2 As Ionic.Zip.ZipEntry = Nothing
        et = active_pkg(name)
        If et Is Nothing Then
            et = shared_content_sandbox_part1(name)
            If et Is Nothing Then
                et = shared_content_sandbox_part2(name)
                If et Is Nothing Then
                    et = shared_content_part1(name)
                    If et Is Nothing Then
                        et = shared_content_part2(name)
                    End If
                End If
            End If
        End If
        Dim hd_name = name.Replace(".dds", "_hd.dds")
        If has_high_rez_map Then
            et2 = active_pkg_hd(hd_name)
            If et2 Is Nothing Then
                et2 = shared_content_sandbox_part1_hd(hd_name)
                If et2 Is Nothing Then
                    et2 = shared_content_sandbox_part2_hd(name)
                    If et2 Is Nothing Then
                        et2 = shared_content_part1_hd(hd_name)
                        If et2 Is Nothing Then
                            et2 = shared_content_part2_hd(hd_name)
                        End If
                    End If
                End If
            End If
        End If
        If et2 Is Nothing And et Is Nothing Then
            try_map_pkgs(et, et2, name)
        End If
        If et2 IsNot Nothing And has_high_rez_map Then Return et2
        Return et
    End Function
    Public Function get_shared_model(name As String) As Ionic.Zip.ZipEntry
        Dim et As Ionic.Zip.ZipEntry = get_shared(name)
        Return et
    End Function
    Private Sub try_map_pkgs(ByRef et As Ionic.Zip.ZipEntry, ByRef et2 As Ionic.Zip.ZipEntry, name As String)
        Dim iPath = My.Settings.game_path + "\res\packages\"
        Dim f_info = Directory.GetFiles(iPath)

        Dim PKGS(150) As String
        Dim cnt = 0
        name = Path.GetFileName(name.Replace("/", "\"))
        Dim hd_name = name.Replace(".dds", "_hd.dds")

        'first, lets get a list of all the map files.
        For Each m In f_info
            If m.ToLower.Contains(".pkg") And Not m.ToLower.Contains("vehicles") _
                And Not m.ToLower.Contains("scripts") _
                And Not m.ToLower.Contains("shaders") _
                And Not m.ToLower.Contains("particles") _
                And Not m.ToLower.Contains("newyear") _
                And Not m.ToLower.Contains("audio") _
                And Not m.ToLower.Contains("hangar") _
                Then
                PKGS(cnt) = m
                cnt += 1
            End If

        Next
        For i = 0 To cnt - 1
            Using z As New Ionic.Zip.ZipFile(PKGS(i))
                For Each item In z
                    If item.FileName.ToLower.Contains(name) Then
                        ' item.Extract(oPath, ExtractExistingFileAction.OverwriteSilently)
                        If Not item.IsDirectory Then 'dont want empty directories
                            et = item
                            et2 = Nothing
                            z.Dispose()
                            GC.Collect()
                            Return
                        End If
                        Application.DoEvents()
                    End If
                    If has_high_rez_map Then
                        If item.FileName.ToLower.Contains(hd_name) Then
                            ' item.Extract(oPath, ExtractExistingFileAction.OverwriteSilently)
                            If Not item.IsDirectory Then 'dont want empty directories
                                et2 = item
                                et = Nothing
                                z.Dispose()
                                GC.Collect()
                                Return
                            End If
                        End If
                        Application.DoEvents()
                    End If
                Next
            End Using
        Next


    End Sub


End Module
