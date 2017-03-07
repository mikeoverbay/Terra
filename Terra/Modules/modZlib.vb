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
    Private totalEntriesToProcess As Integer

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
        Dim t1 As DataTable = t.Tables("team1")
        Dim t2 As DataTable = t.Tables("team2")
        Dim bb As DataTable = t.Tables("boundingbox")
        Dim s1 As String = t1.Rows(0).Item(0)
        Dim s2 As String = t2.Rows(0).Item(0)
        Dim bb_bl As String = bb.Rows(0).Item(0)
        Dim bb_ur As String = bb.Rows(0).Item(1)
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
        Try
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
        Catch ex As Exception
            lighting_terrain_texture = frmLighting.s_terrain_texture_level.Value / 50.0!
            lighting_ambient = frmLighting.s_terrain_ambient.Value / 300.0!
            lighting_fog_level = frmLighting.s_fog_level.Value / 10000.0! ' yes 10,000
            lighting_model_level = frmLighting.s_model_level.Value / 100.0!
            gamma_level = (frmLighting.s_gamma.Value / 100) * 1.0!
            gray_level = 1.0 - (frmLighting.s_gray_level.Value / 100)
            Return False
        End Try
        Return True
    End Function
    Public Function open_pkg(ByVal name As String) As Boolean

        'This function reads the data and creates the map, models, trees and everything else.

        'Dim decal_maker As New Thread(AddressOf make_decals) ' build the meshes in the back ground
        'Try
        '    decal_maker.Name = "decal_maker"
        '    decal_maker.Priority = ThreadPriority.Highest
        'Catch ex As Exception

        'End Try
        normal_mode = 0
        frmMain.d_counter = 0
        has_high_rez_map = False
        get_light_settings()
        frmMain.make_map_buttons()
        '================================================================
        'need to reset these so changes in the menu items take effect
        'Also.. they trigger what mode to display the terrain and models
        screen_totaled_draw_time = 1000
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

        map_layer_cache(0) = New tree_textures_ ' uses the same data members as the trees.
        normalMap_layer_cache(0) = New tree_textures_ ' uses the same data members as the trees.
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
        shared_content1 = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\shared_content_sandbox.pkg")
        shared_content2 = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\shared_content.pkg")
        Try
            shared_content1_hd = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\shared_content_sandbox_hd.pkg")
            shared_content2_hd = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\shared_content_hd.pkg")
            active_pkg_hd = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\" & hd_name)
            If active_pkg_hd Is Nothing Then
                has_high_rez_map = False
            Else
                has_high_rez_map = True
            End If

        Catch ex As Exception

        End Try
        SHOW_MAPS = False

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

            Dim space_ms As New MemoryStream
            space_bin_file.Extract(space_ms)
            Dim space_br As New BinaryReader(space_ms)
            Dim space_bin_data(space_ms.Length) As Byte
            space_bin_data = space_ms.GetBuffer
            get_spaceBin_data(space_bin_data)
            space_br.Close()
            space_ms.Dispose()
        Else
            MsgBox("Unable to load Space.bin", MsgBoxStyle.Exclamation, "File Error...")
            active_pkg.Dispose()
            shared_content1.Dispose()
            shared_content2.Dispose()
            Return False
        End If




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
                floraXML_Name = contents(pos)
            End If
            If contents(pos).Contains("mmap.dds") Then
                minimap_name = contents(pos)
            End If
            If contents(pos).Contains(".vlo") Then
                vlo_name = contents(pos)
            End If
            'Some texture names have capital letters.. deal with it!
            If contents(pos).ToLower.Contains("compositemap_diffuse.dds") Then
                speedtree_map = contents(pos)
            End If
            If contents(pos).ToLower.Contains("compositemap_normal.dds") Then
                speedtree_normalmap = contents(pos)
            End If
            If contents(pos).ToLower.Contains("compositemap.txt") Then
                speedtree_name = contents(pos)
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
        speedtree_normalmap = speedtree_map.Replace("diffuse", "normal")
        ReDim mapBoard(Sqrt(maplist.Length - 2) + 1, Sqrt(maplist.Length - 1) + 2) 'size the map board

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
        'get the minimap image and put on screen
        Dim mmp As New MemoryStream
        Dim mmentry As Ionic.Zip.ZipEntry = active_pkg(minimap_name)
        mmentry.Extract(mmp)
        minimap_textureid = get_texture(mmp, False)
        mmp.Dispose()
        GC.Collect()
        '----------------- 
        frmMain.tb1.Text = "Getting minimap tank Icons..."
        ' get the little tank icons for the minimap
        Dim gui = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\gui.pkg")
        Dim sp = "\gui\maps\icons\filters\tanks\"
        'heavy
        Dim icon_ms As New MemoryStream
        Dim icon_entry As Ionic.Zip.ZipEntry = gui(sp + "heavyTank.png")
        icon_entry.Extract(icon_ms)
        tank_mini_icons(0) = get_tank_icon_image(icon_ms)
        'med
        icon_ms = New MemoryStream
        icon_entry = gui(sp + "mediumTank.png")
        icon_entry.Extract(icon_ms)
        tank_mini_icons(1) = get_tank_icon_image(icon_ms)
        'light
        icon_ms = New MemoryStream
        icon_entry = gui(sp + "lightTank.png")
        icon_entry.Extract(icon_ms)
        tank_mini_icons(2) = get_tank_icon_image(icon_ms)
        'td
        icon_ms = New MemoryStream
        icon_entry = gui(sp + "AT-SPG.png")
        icon_entry.Extract(icon_ms)
        tank_mini_icons(3) = get_tank_icon_image(icon_ms)
        'spg
        icon_ms = New MemoryStream
        icon_entry = gui(sp + "SPG.png")
        icon_entry.Extract(icon_ms)
        tank_mini_icons(4) = get_tank_icon_image(icon_ms)
        'this reads the custom minimap border I made.
        Dim mini_data() = File.ReadAllBytes(Application.StartupPath + "\resources\minimap_frame.png")
        icon_ms = New MemoryStream(mini_data)
        minimsp_frameT_TextureId = get_tank_icon_image(icon_ms)
        icon_ms.Dispose()
        gui.Dispose()
        GC.Collect()
        minimap_size = My.Settings.minimap_size
        '	frmMain.draw_minimap()
        frmMain.draw_scene()

        cnt = 0
        'test section.. reading VLO xml
        'Dim vloentry As Ionic.Zip.ZipEntry = active_pkg(vlo_name)
        'Dim vlo_stream As New MemoryStream
        'If vloentry IsNot Nothing Then
        '    vloentry.Extract(vlo_stream)
        '    openXml_stream(vlo_stream, "VLO")
        '    Dim dSet As DataSet = xmldataset.Copy
        'End If
        'vlo_stream.Dispose()
        '---------------------------------------
        'get the speedtree image and and composite text
        Dim spna = speedtree_name.Split("\")
        Dim tmp As New MemoryStream

        Dim tmentry As Ionic.Zip.ZipEntry = active_pkg(speedtree_name)
        If tmentry IsNot Nothing Then
            tmentry.Extract(tmp)
        Else
            'plan X.. look in shared_content for it.
            tmentry = get_shared(speedtree_name)
            If tmentry Is Nothing Then
                'frmMain.tb1.AppendText("FNF: " & speedtree_name & vbCrLf)
            Else
                tmentry.Extract(tmp)
            End If
        End If
        'End If

        tmp.Position = 0
        Dim buf(tmp.Length) As Byte
        Dim coder As Encoding = Encoding.UTF8
        tmp.Read(buf, 0, tmp.Length)
        speedtree_text = coder.GetString(buf)
        tmp.Dispose()
        cnt = 0
        'Get the speedTree composite maps. The tree textures.
        '================================================
        'diffuse

        Dim timge As Ionic.Zip.ZipEntry = active_pkg(speedtree_map)
        Dim sptimg As New MemoryStream
        If timge IsNot Nothing Then
            timge.Extract(sptimg)
            speedtree_imageID = get_tree_texture(sptimg, False)
        Else
            timge = get_shared(speedtree_map)
            If timge Is Nothing Then
                'Stop
            Else
                timge.Extract(sptimg)
                speedtree_imageID = get_tree_texture(sptimg, False)
            End If
        End If
        sptimg.Dispose()
        GC.Collect()
        'normalMap
        Dim ntimge As Ionic.Zip.ZipEntry = active_pkg(speedtree_normalmap)
        Dim nsptimg As New MemoryStream
        If ntimge IsNot Nothing Then
            ntimge.Extract(nsptimg)
            speedtree_NormalMapID = get_normal_texture(nsptimg, False)
        Else
            ntimge = get_shared(speedtree_normalmap)
            If ntimge Is Nothing Then
                'Stop
            Else
                ntimge.Extract(nsptimg)
                speedtree_NormalMapID = get_normal_texture(nsptimg, False)
            End If
        End If
        sptimg.Dispose()
        GC.Collect()
        '====================================================
        'let the user know whats going on
        frmMain.tb1.Text = "Extracting Data from .pkg files..."
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

        contents.Clear()
        GC.Collect()

        '******************************************************************************
        test_count = maplist.Length - 2
        'test_count = 1
        Dim map_start = 0
        Dim test2 As Int32 = test_count
        ' gotta setup for skydome.. Im using the modlist and will copy and reset the info when done with it
        '******************************************************************************
        frmMain.tb1.Text = "Making Skydome..."
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
        cnt = 0
        'mini_map and Sky dome are loaded... Tell app they are.
        mini_map_loaded = True
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
        Dim m_layers As Boolean = frmMain.m_high_rez_Terrain.Checked
        '**********************************************
        '**********************************************
        '**********************************************
        get_map_extremes()
        For map = map_start To test2
            'let the user know whats going on
            frmMain.tb1.Text = "Building The Terrain sections ( " + map.ToString + " )"
            Application.DoEvents()
            Dim cms As New MemoryStream(maplist(map).cdata)
            Dim cdata As New MemoryStream
            Dim tms As New MemoryStream
            Dim norms As New MemoryStream
            Dim dom As New MemoryStream
            Using ck As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read(cms)
                ReDim maplist(map).scr_coords(3) ' used with unproject
                Dim heights As Ionic.Zip.ZipEntry = ck("terrain2/heights")
                heights.Extract(cdata)
                read_heights(cdata, map)

                Dim texture As Ionic.Zip.ZipEntry = ck("terrain2/lodTexture.dds")
                texture.Extract(tms)
                Using btm As New Bitmap(build_textures(map, tms).Clone, 64, 64)
                    maplist(map).bmap = btm.Clone 'used only for grid listing utility
                End Using
                'maplist(map).bmap = build_textures(map, tms).Clone
                GC.Collect()
                Dim normals As Ionic.Zip.ZipEntry = ck("terrain2/lodNormals")
                normals.Extract(norms)

                ' dont know how to use it :(
                'Dim dominate As Ionic.Zip.ZipEntry = ck("terrain2/dominantTextures")
                'dominate.Extract(dom)
                'open_dominate(map, dom)

                get_surface_normals(norms, map) 'gets the normal map
                get_location(map)   'finds location in the world
                build_terra(map) 'builds the geometry
                make_lists(map) 'makes the display lists
                frmMain.ProgressBar1.Value = map
                'frmMain.draw_scene()    'show each grid be drawn
                Application.DoEvents()
                ck.Dispose()
            End Using
            cms.Dispose()
            cdata.Dispose()
            tms.Dispose()
            norms.Dispose()
            dom.Dispose()
            GC.Collect() ' try to force release of memory.
        Next
        GC.WaitForFullGCComplete() ' wait until memory is released. We need it.
        '**********************************************
        'lets seam the World (creates map seam in between the chunks)
        seam_map()
        '**********************************************
        'decal_maker.Start() ' fire off back ground worker. This causes issues with memory for some unknown reason.
        '**********************************************

        ' this is where we will make the blend texture and slice the huge color_tex in to its chuck areas.
        If m_layers Then
            'main_layer_tex.Dispose()
            main_layer_tex = New Bitmap(12, 12, PixelFormat.Format24bppRgb)
            hz_loaded = True ' this is to let the user know that switch to hirez does not work if the map was not loaded in hirez
            'get the layers
            frmMain.ProgressBar1.Maximum = test2
            frmMain.ProgressBar1.Minimum = 0
            frmMain.ProgressBar1.Value = 0
            Dim found_ As Boolean = False
            layer_uv_list = "" 'clear this
            get_main_texture = True
            frmMain.tb1.Text = "Getting the main Terrain Texture.  This may take a while...."
            Application.DoEvents()
            Dim main_map As Ionic.Zip.ZipEntry = Nothing
            If name.Contains("19_") Then
                main_map = active_pkg("\maps\landscape\19_Kurgan\color_tex.dds")
            Else
                main_map = active_pkg("\maps\landscape\" + name.Replace(".pkg", "") + "\color_tex.dds")
            End If
            Dim main_map_ms As New MemoryStream
            main_map.Extract(main_map_ms)
            main_layer_tex = get_main_tex_bmp(main_map_ms)
            main_map_ms.Dispose()
            GC.Collect()
            For map = map_start To test2
                frmMain.tb1.Text = "Getting the Terrain Textures names (" + map.ToString + ")"
                Application.DoEvents()
                Dim cms As New MemoryStream(maplist(map).cdata)
                Using ck As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read(cms)
                    map_layers(map) = New layer_ ' create the structure to store data
                    Dim layer_count As Integer = get_layer_count(ck, map)
                End Using
                cms.Dispose()
                GC.Collect()
                frmMain.ProgressBar1.Value = map
                Application.DoEvents()
            Next
            'create the mix texture atlas.
            'we have to do this to stop the problems with blending the mix textures one at a time!!
            'It creates one big texture map with each mixmap and adds a 8 pixel boarder around each one.
            create_mix_atlas()
            GC.Collect()
            GC.WaitForFullGCComplete()
            '
            frmMain.ProgressBar1.Maximum = maplist.Length - 2
            frmMain.ProgressBar1.Minimum = 0
            frmMain.ProgressBar1.Value = 0

            Dim mod_ = (Sqrt(maplist.Length - 1)) And 1

            Dim w = Sqrt(maplist.Length - 1)
            tile_width = w + mod_
            Dim m_s = main_layer_tex.Width / tile_width
            Dim ms_i As Integer = m_s
            If ms_i = 341 And main_layer_tex.Width = 4096 Then
                main_layer_tex = ResizeImage(main_layer_tex, New Size(4104, 4104))
                m_s = main_layer_tex.Width / tile_width
                ms_i = m_s
            End If
            If ms_i = 410 And main_layer_tex.Width = 4096 Then
                main_layer_tex = ResizeImage(main_layer_tex, New Size(4100, 4100))
                m_s = main_layer_tex.Width / tile_width
                ms_i = m_s
            End If
            Dim text_w = main_layer_tex.Width
            cnt = 0
            Dim map_pnt As Integer = 0
            For row = 0 To w - 1
                For col = 0 To w - 1
                    maplist(cnt).col = col
                    maplist(cnt).row = row
                    frmMain.tb1.Text = "Getting the Tarrain Textures (" + cnt.ToString + ")"
                    Application.DoEvents()
                    Dim x1 As Integer = (main_layer_tex.Width / 2) - (((maplist(cnt).location.x + 50) / 100) * m_s)
                    Dim y1 As Integer = (main_layer_tex.Width / 2) - (((maplist(cnt).location.y - 50) / 100) * m_s) - m_s


                    Dim rec As New Rectangle(x1, y1, m_s, m_s)
                    Dim s As Integer = m_s
                    map_layers(cnt).color_tex = New Bitmap(s, s, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                    Dim g As Graphics = Graphics.FromImage(map_layers(cnt).color_tex)
                    g.DrawImage(main_layer_tex, 0, 0, rec, GraphicsUnit.Pixel)

                    Dim e = Gl.glGetError
                    For layer = 1 To map_layers(cnt).layer_count
                        get_layer_image(cnt, layer)
                    Next
                    Dim t_id = get_tex_id_from_bmp(map_layers(cnt).color_tex)
                    For layer = 1 To map_layers(cnt).layer_count
                        If map_layers(cnt).layers(layer).l_name.ToLower.Contains("color_tex") Then
                            map_layers(cnt).layers(layer).text_id = t_id
                            map_layers(cnt).main_texture = layer
                            'have to mod the mask to exclude the main texture 
                            ' Exit For
                            GC.Collect()
                        End If
                        Dim mask = map_layers(cnt).used_layers
                    Next
                    e = Gl.glGetError
                    frmMain.ProgressBar1.Value = cnt
                    Application.DoEvents()
                    cnt += 1
                    map_pnt += 1
                Next

            Next
            tile_width -= mod_
            frmMain.pb2.Visible = False
            frmMain.pb2.SendToBack()

        End If ' if m_layers
        GC.Collect()
        GC.WaitForFullGCComplete() 'release memory.
        'we got a world.. lets update the minimap,move the camera and draw the map
        Cam_Y_angle = PI * -0.125
        Cam_X_angle = PI * 0.25
        look_point_X = 0.01
        look_point_Z = 0.01
        View_Radius = -150.0
        frmMain.position_camera()
        frmMain.draw_scene()
        '-----------------------------------
        'lets make the team rings here? OK.
        ringDisplayID_1 = Gl.glGenLists(1)
        Gl.glNewList(ringDisplayID_1, Gl.GL_COMPILE)
        draw_ring(-team_1.x, team_1.z)
        Gl.glEndList()
        Gl.glFinish()

        ringDisplayID_2 = Gl.glGenLists(1)
        Gl.glNewList(ringDisplayID_2, Gl.GL_COMPILE)
        draw_ring(-team_2.x, team_2.z)
        Gl.glEndList()
        frmMain.draw_scene()
        Gl.glFinish()
        '---------------------------------------
        'lets make the sector outlines
        sector_outlineID = Gl.glGenLists(1)
        Gl.glNewList(sector_outlineID, Gl.GL_COMPILE)
        create_grid_marks()
        Gl.glEndList()
        'lets make the map outside border
        map_borderId = Gl.glGenLists(1)
        Gl.glNewList(map_borderId, Gl.GL_COMPILE)
        make_map_boarder()
        Gl.glEndList()


        '**********************************************
        glob_str = ""
        'clean up data.. recover memory.
        For map = map_start To test_count
            ReDim maplist(map).cdata(1) ' clear
            GC.Collect()
        Next
        '*******************************************************************************************


        'setup progress bar
        frmMain.ProgressBar1.Value = 0
        frmMain.ProgressBar1.Maximum = Model_Matrix_list.Length
        ReDim Models.models(Model_Matrix_list.Length)
        Models.models(0) = New primitive
        Models.model_count = Model_Matrix_list.Length - 2
        bw_strings.Clear()
        'make all the models.
        For m = 0 To Model_Matrix_list.Length - 2
            If True Then
                'stuff we dont want on the map.
                If Model_Matrix_list(m).primitive_name.ToLower.Contains("wgl_banner") _
                    Or Model_Matrix_list(m).primitive_name.ToLower.Contains("000_base") _
                    Or Model_Matrix_list(m).primitive_name.ToLower.Contains("particles") Then
                    GoTo skip_this
                End If
                frmMain.ProgressBar1.Value = m
                frmMain.tb1.Text = "Populating the sections with models ( " + m.ToString + " )"
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
                Models.matrix(m).matrix(1) *= -1.0
                Models.matrix(m).matrix(2) *= -1.0
                Models.matrix(m).matrix(4) *= -1.0
                Models.matrix(m).matrix(8) *= -1.0
                Models.matrix(m).matrix(12) *= -1.0
                get_primitive(m, active_pkg)
            Else

            End If
skip_this:
            Application.DoEvents()
        Next
        bw_strings.Clear()
        GC.Collect()
        GC.WaitForFullGCComplete()

        frmMain.ProgressBar1.Value = 0
        frmMain.ProgressBar1.Maximum = speedtree_matrix_list.Length

        Trees = New Tree_s
        ReDim Trees.flora(0)
        ReDim treeCache(0)
        treeCache(0) = New flora_
        ReDim Trees.Tree_list(speedtree_matrix_list.Length)
        'make all the trees and shrubs... 
        For tree = 0 To speedtree_matrix_list.Length - 2
            frmMain.tb1.Text = "Populating the sections with trees ( " + tree.ToString + " )"
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
        'GoTo fuck_this
        'While decal_maker.IsAlive
        '    frmMain.tb1.Text = "Waiting on Decal Thread to finish..."
        '    Application.DoEvents()
        '    Thread.Sleep(10)
        'End While
        make_decals() ' this wont run ok in background thread for some reason.
        GC.Collect()
        GC.WaitForFullGCComplete()
        build_decals() ' finish making the decals.. this involves projecting them on to the terrain and grabbing textures.
        GC.Collect()
        GC.WaitForFullGCComplete()

fuck_this:

        frmMain.tb1.Text = "Sorting the Texture Id list up..."
        'We are going to create the water now.. before closing the PKGs.. may want to get
        'textures from them?? This app does not refresh nonstop. no point in animated textures for the water
        If BWWa.bwwa_t1(0).width > 0 Then
            water.IsWater = True
            build_water()
            water.textureID = Load_DDS_File(Application.StartupPath + "\Resources\water2.dds")
            GC.Collect()
        End If
        'these packatges are HUGE!.. I need to find a way to read as needed.
        active_pkg.Dispose()    ' VERY IMPORTANT !!!
        shared_content1.Dispose() ' VERY IMPORTANT !!!
        shared_content2.Dispose() ' VERY IMPORTANT !!!
        Try
            active_pkg_hd.Dispose() ' VERY IMPORTANT !!!
        Catch ex As Exception
        End Try
        Try
            shared_content1_hd.Dispose() ' VERY IMPORTANT !!!
        Catch ex As Exception
        End Try
        Try
            shared_content2_hd.Dispose() ' VERY IMPORTANT !!!
        Catch ex As Exception
        End Try
        frmMain.ProgressBar1.Visible = False
        'let the renderer know the models are loaded and ready
        maploaded = True ' entire map is ready for display.
        sw.Stop() ' stopwatch for load time
        Cam_Y_angle = PI * -0.125
        Cam_X_angle = PI * 0.25
        look_point_X = 0.01
        look_point_Z = 0.01
        View_Radius = -150.0
        frmMain.position_camera()
        frmMain.draw_scene()
        Dim tm = sw.Elapsed
        Dim t = Format(tm.Seconds, "00")
        frmMain.tb1.Text = "Load time 00:" + t + vbCrLf
        frmMain.tb1.Text += "Reused Textures: " + saved_texture_loads.ToString + vbCrLf
        frmMain.tb1.Text += "Reused Models: " + saved_model_loads.ToString + vbCrLf
        frmMain.tb1.Text += "lod2 used: " + lod2_swap.ToString + _
                                        "   lod1 used: " + lod1_swap.ToString + _
                                        "   lod0 used: " + lod0_swap.ToString
        frmMain.pb1.Focus()
        frmMain.draw_scene()
        frmMain.draw_scene()
        frmMain.draw_scene()
    End Function
    Public Function get_shared(name As String) As Ionic.Zip.ZipEntry
        Dim et As Ionic.Zip.ZipEntry = Nothing
        If Not has_high_rez_map Then
            et = active_pkg(name)
            If et Is Nothing Then
                et = shared_content1(name)
                If et Is Nothing Then
                    et = shared_content2(name)
                End If
            End If
            If et IsNot Nothing Then
                Return et
            End If
        Else
            If has_high_rez_map Then
                Dim hd_name = name.Replace(".dds", "_hd.dds")
                et = active_pkg_hd(hd_name)
                If et Is Nothing Then
                    et = shared_content1_hd(hd_name)
                    If et Is Nothing Then
                        et = shared_content2_hd(hd_name)
                        If et Is Nothing Then
                            et = active_pkg(name)
                            If et Is Nothing Then
                                et = shared_content1(name)
                                If et Is Nothing Then
                                    et = shared_content2(name)
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If

        Return et
    End Function
    Public Function get_shared_model(name As String) As Ionic.Zip.ZipEntry
        Dim et As Ionic.Zip.ZipEntry = Nothing
        et = active_pkg(name)
        If et Is Nothing Then
            et = shared_content1(name)
            If et Is Nothing Then
                et = shared_content2(name)
            End If
        End If
        Return et
    End Function
End Module
