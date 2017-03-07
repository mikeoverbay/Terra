#Region "Imports"

Imports System
Imports System.Windows
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Text
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
Imports System.Net.Sockets
Imports System.Data
Imports Tao.DevIl
Imports Tao.DevIl.Ilu
Imports Tao.DevIl.Ilut
Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Globalization
'Imports ICSharpCode.SharpZipLib.Zip.Compression.Streams
'Imports ICSharpCode.SharpZipLib.Zip.Compression
Imports Hjg.Pngcs
Imports System.Drawing.FontFamily
Imports Ionic.Zip
Imports Ionic.BZip2
'
Imports Ionic
'Imports MySql.Data
Imports System.Windows.Forms.Design
#End Region

Public Class frmMain
    Inherits Form
    Public Sub New()
        InitializeComponent()
        mainMenu.Renderer = New MyRenderer()
    End Sub
    'Dim TextArea As RectangleF
    'Dim TabArea As Rectangle
#Region "variables"

    Dim swat2 As New Stopwatch
    Dim swat1 As New Stopwatch
    Dim angle_offset As Single
    Dim real_names_list As New List(Of String)
    Dim Lx, Ly As Single
    Dim angle As Single
    'Dim mass_value As Single = 2
    Dim m_rot As Single = 0
    Dim view_rot As Single = 0
    Dim light_rot As Single = 0
    Dim paused As Boolean = False
    Dim point_set(311, 311) As p_set
    Public opac As Single = 0.9
    Dim drawing As Boolean = False
    'Dim step_ As Single = 1.0
    Dim divisions As Single = 100
    'Dim tera As Integer
    'Dim highrez_tera As Integer
    Dim bmp_w, bmp_h As Integer
    Dim tx As New Stopwatch
    Dim fps As Single = 0.0
    Dim fps_out As Single = 0.0
    'Dim tr_matrix(100, 100) As Single
    'Dim tri() As p_set = {New p_set}
    Dim sin_x, cos_x, cos_y, sin_y As Single
    Private _backgroundWorker1 As System.ComponentModel.BackgroundWorker
    Private _operationCanceled As Boolean
    Private nFilesCompleted As Integer
    Private _appCuKey As Microsoft.Win32.RegistryKey
    Public pfc As New PrivateFontCollection
    Public update_thread As New Thread(AddressOf update_mouse)

    Public Shared d_counter As Integer = 0
    Dim clip_distance As Integer
    Public view_mode As Boolean = False
#End Region

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim nonInvariantCulture As CultureInfo = New CultureInfo("en-US")
        nonInvariantCulture.NumberFormat.NumberDecimalSeparator = "."
        Thread.CurrentThread.CurrentCulture = nonInvariantCulture
        frmSplash.Show()
        cam_x = 0
        cam_y = 0
        cam_z = 10
        View_Radius = -50.0
        Cam_X_angle = 0
        Cam_Y_angle = -PI / 4
        View_Radius = -600.0
        'setup point_set
        'For q = 0 To 311
        '    For r = 0 To 311
        '        point_set(q, r) = New p_set
        '    Next
        'Next
        Me.KeyPreview = True    'so i catch keyboard before despatching it
        Il.ilInit()
        Ilu.iluInit()
        Ilut.ilutInit()
        EnableOpenGL()
        make_shaders()
        set_shader_variables() ' now that we have shaders, we need the uniforms.
        maploaded = False
        GAME_PATH = My.Settings.game_path
        Dim script_pkg As Ionic.Zip.ZipFile = Nothing
        Dim ms As New MemoryStream
        Try
            script_pkg = Ionic.Zip.ZipFile.Read(GAME_PATH & "res\packages\scripts.pkg")
            Dim script As Ionic.Zip.ZipEntry = script_pkg("\scripts\destructibles.xml")
            script.Extract(ms)
            ms.Position = 0
            Dim bdata As Byte()
            Dim br As BinaryReader = New BinaryReader(ms)
            bdata = br.ReadBytes(br.BaseStream.Length)
            Dim des As MemoryStream = New MemoryStream(bdata, 0, bdata.Length)
            des.Write(bdata, 0, bdata.Length)
            openXml_stream(des, "destructibles.xml")
            des.Close()
            des.Dispose()
        Catch ex As Exception
            'script_pkg.Dispose()
            'ms.Dispose()
            If frmSplash.Visible Then
                frmSplash.TopMost = False
                Application.DoEvents()
            End If
            MsgBox("Im sorry. I can't find the World of Tanks Folder." + vbCrLf + _
          "Try setting the Path under File on the menu." + vbCrLf + _
          "Example:   C:\Games\World of Tanks\" + vbCrLf + _
          "and than restart Terra!", MsgBoxStyle.Exclamation, "Path not set.")
            m_set_path.PerformClick()
            init_terra()
            Return
        End Try
        script_pkg.Dispose()
        ms.Dispose()
        Dim matName, entry As DataTable
        matName = xmldataset.Tables("matName")
        entry = xmldataset.Tables("entry")
        Dim q = From fname_ In entry.AsEnumerable _
                  Join mName In matName On fname_("entry_Id") Equals _
                  mName("entry_Id") _
             Select _
                  filename = fname_.Field(Of String)("filename"), _
                  mat = mName.Field(Of String)("matName_Text")

        dest_buildings.filename = New List(Of String)
        dest_buildings.matName = New List(Of String)
        For Each it In q
            If InStr(it.filename, "bld_Construc") = 0 Then
                dest_buildings.filename.Add(it.filename.Replace("model", "visual").ToLower)
                dest_buildings.matName.Add(it.mat.ToLower)
            End If
        Next
        '---------------------------------------
fail_path:
        'make_compus() 'this sets up the compus model and texture
        pfc.AddFontFile("tiny.ttf")
        Me.Show()
        Me.Update()
        _STARTED = True
        AddHandler m_layout_mode.CheckedChanged, AddressOf edit_mode_cb_CheckedChanged
        icon_scale = My.Settings.icon_scale

        m_find_Item_menu.Visible = False
        m_edit_shaders.Visible = False

        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Me.Update()
        set_menu_strip_checkboxes()
        add_view_distance_menu_events()

        'map_holder.Visible = True
        'frmTanks.Show()
        draw_scene()
        draw_scene()
        tb1.Text = "Getting Tank Data"
        Application.DoEvents()
        Application.DoEvents()

        get_tank_list() ' get the tanks and add them to the GUI

        tb1.Text = "Getting Map Images"
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        tb1.SelectionLength = 0
        Packet_in.tankId = -1
        Packet_out.tankId = -1
        Packet_in.comment = ""
        Packet_out.comment = ""

        m_comment.AllowDrop = True
        frmTanks.Hide()
        make_locations() ' setup tank location data and clear displaylist
        make_map_buttons()
        tb1.Text = "Welcome to Terra!"
        tb1.SelectionLength = 0

        Timer1.Enabled = True
        frmSplash.Close()
        testing = True
        'frmDebug.Show()
        ' frmChat.Show()
        SHOW_MAPS = True
    End Sub
    Private Sub init_terra()
        'let everything dependent on OpenGL know its started.
        GAME_PATH = My.Settings.game_path
        _STARTED = True
        If My.Settings.vr_1200 = True Then
            Far_Clip = 1200
        End If
        If My.Settings.vr_900 = True Then
            Far_Clip = 600
        End If
        If My.Settings.vr_700 = True Then
            Far_Clip = 400
        End If
        If My.Settings.vr_400 = True Then
            Far_Clip = 200
        End If
        'setup custom font
        pfc.AddFontFile("tiny.ttf")
        '---------------------------------
        'this will create a list of everything in the game that can be destroyed.
        'I will use this to find the undamaged state of any object.
        'the material name is for the object that is intact
        Dim script_pkg As Ionic.Zip.ZipFile = Nothing
        Dim ms As New MemoryStream
        Try
            script_pkg = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\scripts.pkg")
            Dim script As Ionic.Zip.ZipEntry = script_pkg("\scripts\destructibles.xml")
            script.Extract(ms)
            ms.Position = 0
            Dim bdata As Byte()
            Dim br As BinaryReader = New BinaryReader(ms)
            bdata = br.ReadBytes(br.BaseStream.Length)
            Dim des As MemoryStream = New MemoryStream(bdata, 0, bdata.Length)
            des.Write(bdata, 0, bdata.Length)
            openXml_stream(des, "destructibles.xml")
            des.Close()
            des.Dispose()
        Catch ex As Exception
            script_pkg.Dispose()
            ms.Dispose()
            If frmSplash.Visible Then
                frmSplash.TopMost = False
            End If
            MsgBox("Im sorry. I STILL can't find the World of Tanks Folder." + vbCrLf + _
          "Try setting the Path under File on the menu." + vbCrLf + _
          "Example:   C:\Games\World of Tanks\" + vbCrLf + _
          "and than restart Terra!", MsgBoxStyle.Exclamation, "Path not set.")
            GoTo fail_path
        End Try
        script_pkg.Dispose()
        ms.Dispose()
        Dim matName, entry As DataTable
        matName = xmldataset.Tables("matName")
        entry = xmldataset.Tables("entry")
        Dim q = From fname_ In entry.AsEnumerable _
                  Join mName In matName On fname_("entry_Id") Equals _
                  mName("entry_Id") _
             Select _
                  filename = fname_.Field(Of String)("filename"), _
                  mat = mName.Field(Of String)("matName_Text")

        dest_buildings.filename = New List(Of String)
        dest_buildings.matName = New List(Of String)
        For Each it In q
            If InStr(it.filename, "bld_Construc") = 0 Then
                dest_buildings.filename.Add(it.filename.Replace("model", "visual").ToLower)
                dest_buildings.matName.Add(it.mat.ToLower)
            End If
        Next
        matName.Dispose()
        entry.Dispose()
        '---------------------------------------
fail_path:
        'make_compus() 'this sets up the compus model and texture
        Me.Show()
        Me.Update()
        _STARTED = True
        AddHandler m_layout_mode.CheckedChanged, AddressOf edit_mode_cb_CheckedChanged
        icon_scale = My.Settings.icon_scale

        m_find_Item_menu.Visible = False
        m_edit_shaders.Visible = False

        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Me.Update()
        set_menu_strip_checkboxes()
        add_view_distance_menu_events()

        'map_holder.Visible = True
        'frmTanks.Show()
        tb1.Text = "Getting Tank Data"
        Application.DoEvents()
        Application.DoEvents()

        get_tank_list() ' get the tanks and add them to the GUI

        tb1.Text = "Getting Map Images"
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        tb1.Text = "Welcome to Terra!"
        tb1.SelectionLength = 0
        Packet_in.tankId = -1
        Packet_out.tankId = -1
        Packet_in.comment = ""
        Packet_out.comment = ""

        m_comment.AllowDrop = True
        frmTanks.Hide()
        make_locations() ' setup tank location data and clear displaylist
        make_map_buttons()
        Timer1.Enabled = True
        frmSplash.Close()
        testing = True
        'frmDebug.Show()
        ' frmChat.Show()
        SHOW_MAPS = True
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If EDIT_INCULDERS Then
            Dim answer = MsgBox("You are editing the decal Incuders..." + vbCrLf + _
                                 "Do you wish to save the file?", MsgBoxStyle.YesNoCancel, "Warning!!!")
            If answer = MsgBoxResult.Cancel Then
                e.Cancel = True
                Return
            End If
            If answer = MsgBoxResult.Yes Then
                File.WriteAllText(Application.StartupPath + "\decal_includer_files\" + JUST_MAP_NAME + "_decal_includers.txt", decal_includers_string)
            End If
        End If
        m_fly_map.Checked = False
        m_Orbit_Light.Checked = False
        If maploaded Then
            save_light_settings() ' save the settings before closing if a map is loaded!
        End If
        _STARTED = False ' stops all controls and OpenGL draw
        'if we are connected and the user 'X'es out. we have do deal with this.
        If frmShowImage.Visible Then
            frmShowImage.Close()
        End If
        If frmClient.Visible Then
            frmClient.shut_down_gracefully()
            While frmClient.Visible
                Application.DoEvents()
            End While
        End If
        If frmServer.Visible Then
            frmServer.serverAlive = False
            While frmServer.Visible
                frmServer.serverAlive = False
                Application.DoEvents()
                frmServer.Close()
            End While
        End If
        Try
            maploaded = False
            'flush_data()
            DisableOpenGL()

        Catch ex As Exception

        End Try
        Application.Exit()
    End Sub
    Private Sub Form1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If m_comment.Focused Then '' if typing a tank comment, we dont want to change any thing.
            Return
        End If
        If e.Control Then
            If e.KeyCode = Keys.OemMinus Then
                icon_scale -= 0.5!
                If icon_scale < 3 Then
                    icon_scale = 3
                End If
                tb1.Text = "Icon size: " + icon_scale.ToString
                My.Settings.icon_scale = icon_scale
                draw_scene()
                Return
            End If
            If e.KeyCode = Keys.Oemplus Then
                icon_scale += 0.5!
                If icon_scale > 100.0! Then
                    icon_scale = 100.0!
                End If
                tb1.Text = "Icon size: " + icon_scale.ToString
                My.Settings.icon_scale = icon_scale
                draw_scene()
                Return
            End If
        End If
        'If e.KeyCode = Keys.M Then
        '    make_decals()
        '    draw_scene()
        '    draw_scene()
        'End If
        '====================================================================
        If e.KeyCode = Keys.A Then
            d_counter -= 1
            If d_counter < 0 Then
                d_counter = decal_matrix_list.Length - 1
            End If
            draw_scene()
        End If
        If e.KeyCode = Keys.S Then
            d_counter += 1
            If d_counter > decal_matrix_list.Length - 1 Then
                d_counter = 0
            End If
            draw_scene()
        End If
        If e.KeyCode = Keys.Q Then
            If view_mode Then
                view_mode = False
            Else
                view_mode = True
            End If
            draw_scene()
        End If
        '====================================================================
        If e.KeyCode = Keys.D1 Then
            m_wire_decals.PerformClick()
        End If
        If e.KeyCode = Keys.D2 Then
            m_wire_models.PerformClick()
        End If
        If e.KeyCode = Keys.D3 Then
            m_wire_trees.PerformClick()
        End If
        If e.KeyCode = Keys.D4 Then
            m_wire_terrain.PerformClick()
        End If
        If e.KeyCode = Keys.E Then
            frmEditFrag.Show()
        End If
        If e.KeyCode = Keys.F1 Then
            m_info_window.PerformClick()
        End If
        If e.KeyCode = Keys.F2 Then
            m_Orbit_Light.PerformClick()
        End If
        If e.KeyCode = Keys.F3 Then
            m_fly_map.PerformClick()
        End If
        If e.KeyCode = Keys.H Then
            m_hell_mode.PerformClick()
        End If
        If e.KeyCode = Keys.L Then
            m_lighting.PerformClick()
        End If
        If e.KeyCode = Keys.O Then
            m_comment.Text = ""
            m_comment.Visible = False
            SHOW_MAPS = True
            gl_pick_map(0, 0)
            gl_pick_map(0, 0)
        End If
        If e.KeyCode = Keys.N Then
            normal_mode += 1
            If normal_mode = 3 Then
                normal_mode = 0
            End If
            draw_scene()
        End If
        If e.KeyCode = Keys.G Then
            m_show_map_grid.PerformClick()
        End If
        If e.KeyCode = Keys.B Then
            m_map_border.PerformClick()
        End If
        If e.KeyCode = Keys.I Then
            m_show_chuckIds.PerformClick()
        End If

        If e.KeyCode = Keys.W Then
            m_show_water.PerformClick()
        End If
        If e.KeyCode = Keys.M Then
            m_show_models.PerformClick()
        End If
        If e.KeyCode = Keys.T And tankID = -1 Then
            m_show_trees.PerformClick()
        End If
        If e.KeyCode = Keys.D Then
            m_show_decals.PerformClick()
        End If
        If e.KeyCode = Keys.C Then
            If m_show_chunks.Checked Then
                m_show_chunks.Checked = False
            Else
                m_show_chunks.Checked = True
            End If
        End If
        If e.KeyCode = Keys.V Then
            frmShowImage.Visible = True
        End If
        If e.KeyCode = Keys.T Then
            If tankID > -1 Then
                MOVE_TANK = True
                move_mod = True ' SHIFT KET
            End If
        End If
        If e.KeyCode = Keys.R Then
            If tankID > -1 Then
                ROTATE_TANK = True
            End If
        End If
        If e.KeyCode = 16 Then
            If Not move_mod Then
                move_mod = True ' SHIFT KET
                If Not NetData Then
                    draw_scene()
                End If
            End If

        End If
        If e.KeyCode = 17 Then
            If Not z_move Then
                z_move = True ' CTRL KEY
                'draw_scene()
            End If
        End If
        If e.KeyCode = Keys.OemMinus Then
            minimap_size -= 32.0!
            If minimap_size < 128.0! Then
                minimap_size = 128.0!
            End If
            'tb1.Text = "Minimap size: " + minimap_size.ToString
            My.Settings.minimap_size = minimap_size
            draw_scene()
        End If
        If e.KeyCode = Keys.Oemplus Then
            minimap_size += 32.0!
            If minimap_size > 640.0! Then
                minimap_size = 640.0!
            End If
            'tb1.Text = "Minimap size: " + minimap_size.ToString
            My.Settings.minimap_size = minimap_size
            draw_scene()
        End If
        If e.KeyCode = Keys.X Then
            If sun_lock Then
                sun_lock = False
            Else
                sun_lock = True
            End If
            sun_lock_update()
        End If
    End Sub
    Private Sub Form1_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        MOVE_TANK = False
        ROTATE_TANK = False
        If move_mod Then
            move_mod = False
            draw_scene()
        End If
        If z_move Then
            z_move = False
            draw_scene()
        End If
    End Sub
    Private Sub frmMain_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        pb1.Width = Me.ClientSize.Width
        If m_info_window.Checked Then
            pb1.Height = Me.ClientSize.Height - mainMenu.Height - tb1.Height - 2
        Else
            pb1.Height = Me.ClientSize.Height - mainMenu.Height
        End If
        pb1.Location = New System.Drawing.Point(0, mainMenu.Height + 1)



        npb.Update()
        Application.DoEvents()
        If _STARTED Then
            'make_post_FBO_and_Textures()
        End If
        If Not SHOW_MAPS Then
            draw_scene()
        Else
            draw_maps()
        End If

    End Sub
    Private Sub frmMain_ResizeEnd(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ResizeEnd
        pb1.Width = Me.ClientSize.Width
        If m_info_window.Checked Then
            pb1.Height = Me.ClientSize.Height - mainMenu.Height - tb1.Height - 2
        Else
            pb1.Height = Me.ClientSize.Height - mainMenu.Height
        End If
        pb1.Location = New System.Drawing.Point(0, mainMenu.Height + 1)



        npb.Update()
        Application.DoEvents()
        If _STARTED Then
            'make_post_FBO_and_Textures()
        End If
        If Not SHOW_MAPS Then
            draw_scene()
        Else
            draw_maps()
        End If
        If pb1.Parent.Name = Me.Name Then
            pb1_screen_location = pb1.PointToScreen(New System.Drawing.Point)
        End If


    End Sub
    Private Sub frmMain_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        npb.Invalidate()
    End Sub
    Private Sub set_menu_strip_checkboxes()
        m_1000.Checked = My.Settings.vr_1200
        m_700.Checked = My.Settings.vr_900
        m_400.Checked = My.Settings.vr_700
        m_200.Checked = My.Settings.vr_400

        m_low_quality_trees.Checked = My.Settings.low_q_trees
        m_load_lod.Checked = My.Settings.lod0
        m_load_details.Checked = My.Settings.load_extra
        m_low_quality_textures.Checked = My.Settings.txt_256
        m_show_fog.Checked = My.Settings.enable_fog
        m_high_rez_Terrain.Checked = My.Settings.hi_rez_terra
        m_bump_map_models.Checked = My.Settings.m_bump_map_models
        m_show_uv2.Checked = My.Settings.m_show_uv2
        m_show_cursor.Checked = False
        'may as well set the levels here
        lighting_fog_level = My.Settings.s_fog_level / 10000.0!
        lighting_ambient = My.Settings.s_terrain_ambient_level / 300.0!
        lighting_terrain_texture = My.Settings.s_terrian_texture_level / 50.0!
        lighting_model_level = My.Settings.s_model_level / 100.0!
        gamma_level = (My.Settings.s_gamma / 100.0!) * 2.0!
    End Sub
    Private Sub add_view_distance_menu_events()
        AddHandler m_200.Click, AddressOf set_radio
        AddHandler m_400.Click, AddressOf set_radio
        AddHandler m_700.Click, AddressOf set_radio
        AddHandler m_1000.Click, AddressOf set_radio


    End Sub
    Private Sub set_radio(ByVal sender As ToolStripMenuItem, ByVal e As System.EventArgs)
        'Sets up the menut items like radio buttons'
        'Only one in the collection can be checked.
        If sender.Text = "200 Meters" Then
            If Not sender.Checked Then
                sender.Checked = True
                Far_Clip = 200
                Return
            Else
                sender.Checked = True
                m_1000.Checked = False
                m_700.Checked = False
                m_400.Checked = False
                Far_Clip = 200
                draw_scene()
                Return
            End If
        End If
        '-----------------------------------
        If sender.Text = "400 Meters" Then
            If Not sender.Checked Then
                sender.Checked = True
                Far_Clip = 400
                Return
            Else
                sender.Checked = True
                m_1000.Checked = False
                m_700.Checked = False
                m_200.Checked = False
                Far_Clip = 400
                draw_scene()
                Return
            End If
        End If
        '-----------------------------------
        If sender.Text = "700 Meters" Then
            If Not sender.Checked Then
                sender.Checked = True
                Far_Clip = 700
                Return
            Else
                sender.Checked = True
                m_1000.Checked = False
                m_400.Checked = False
                m_200.Checked = False
                Far_Clip = 700
                draw_scene()
                Return
            End If
        End If
        '-----------------------------------
        If sender.Text = "1200 Meters" Then
            sender.Checked = True
            If Not sender.Checked Then
                Far_Clip = 1000
                Return
            Else
                sender.Checked = True
                m_700.Checked = False
                m_400.Checked = False
                m_200.Checked = False
                Far_Clip = 1200
                draw_scene()
                Return
            End If
        End If
    End Sub

    Public Sub make_locations()
        ReDim locations.team_1(15)
        ReDim locations.team_2(15)
        For u = 0 To 14
            locations.team_1(u) = New t_l
            If locations.team_1(u).track_displaylist > 0 Then
                Gl.glDeleteLists(locations.team_1(u).track_displaylist, 1)
            End If
            locations.team_1(u).track_displaylist = -1
            locations.team_1(u).id = ""
            locations.team_1(u).comment = ""
            locations.team_1(u).name = ""

            locations.team_2(u) = New t_l
            If locations.team_2(u).track_displaylist > 0 Then
                Gl.glDeleteLists(locations.team_2(u).track_displaylist, 1)
            End If
            locations.team_2(u).track_displaylist = -1
            locations.team_2(u).id = ""
            locations.team_2(u).comment = ""
            locations.team_2(u).name = ""
        Next
    End Sub
    Public Sub get_materials()

        Dim z As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\misc.pkg")
        Dim e As Ionic.Zip.ZipEntry = z("\system\data\material_kinds.xml")
        Dim ms As New MemoryStream
        e.Extract(ms)
        z.Dispose()
        openXml_stream(ms, "materil_kinds")
        Dim t As DataTable = xmldataset.Tables("kind")
        Dim cnt As Integer = 0
        For i = 0 To t.Rows.Count - 1
            material_list(i) = New materi_
            material_list(i).id = t.Rows(i).Item(0)
            If InStr(t.Rows(i).Item(1), "broken") > 0 Then
                material_list(i).damaged = True
            Else
                material_list(i).damaged = False
            End If
            cnt += 1
            ReDim Preserve material_list(cnt)
        Next


    End Sub
    Private Function set_panelsize(ByRef p As Panel) As Panel
        frmTanks.Panel1.Controls.Add(p)
        p.Width = frmTanks.Panel1.Width
        p.Height = frmTanks.Panel1.Height - 23
        p.Location = New System.Drawing.Point(0, 24)
        p.Anchor = AnchorStyles.Bottom Or AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        p.BackColor = Color.DimGray
        p.ForeColor = Color.White
        p.AutoScroll = True
        Return p
    End Function
    Public Sub get_tank_list()
        Dim american_list As String = "usa\list.xml"
        Dim russian_list As String = "ussr\list.xml"
        Dim chinese_list As String = "china\list.xml"
        Dim british_list As String = "uk\list.xml"
        Dim german_list As String = "germany\list.xml"
        Dim french_list As String = "france\list.xml"
        Dim japan_list As String = "japan\list.xml"

        Dim ussr_short As String = "ussr-"
        Dim usa_short As String = "usa-"
        Dim china_short As String = "china-"
        Dim germany_short As String = "germany-"
        Dim french_short As String = "france-"
        Dim uk_short As String = "uk-"
        Dim japan_short As String = "japan-"

        'load the real names file.
        Dim fname = File.ReadAllText(Application.StartupPath & "\tanks\selected_tanks.txt")

        Dim n_array = fname.Split(vbCrLf)
        For Each fn In n_array
            real_names_list.Add(fn)
        Next

        'Try

        build_tank_data(a_tanks, american_list, ".american", usa_short)
        build_tank_data(b_tanks, british_list, ".british", uk_short)
        build_tank_data(r_tanks, russian_list, ".russian", ussr_short)
        build_tank_data(g_tanks, german_list, ".german", germany_short)
        build_tank_data(c_tanks, chinese_list, ".chinese", china_short)
        build_tank_data(f_tanks, french_list, ".french", french_short)
        build_tank_data(j_tanks, japan_list, ".japan", japan_short)
        'Catch ex As Exception

        'End Try
        'now that we have all the tank data. we need to add it to the form. but. how?
        're-writing all of this so it will work in Win7/Vista and hopefully,, Win8
        'Need 6 panels
        Dim p1, p2, p3, p4, p5, p6, p7 As New Panel
        p1 = set_panelsize(p1)
        p2 = set_panelsize(p2)
        p3 = set_panelsize(p3)
        p4 = set_panelsize(p4)
        p5 = set_panelsize(p5)
        p6 = set_panelsize(p6)
        p7 = set_panelsize(p7)
        nations(0) = ("American")
        nations(1) = ("Russian")
        nations(2) = ("German")
        nations(3) = ("British")
        nations(4) = ("French")
        nations(5) = ("Chinese")
        nations(6) = ("Japanese")

        Dim cnt As Integer = 0
        'these 2 are used to scale the buttons so they fit the space they have
        Dim sbw = SystemInformation.VerticalScrollBarWidth + 8
        Dim ww = frmTanks.Panel1.Width - sbw
        'The nice thing is, the pngs that are loaded in have a alpha channel. this
        'enables blending with the background color of the buttons.
        'american
        For Each t In a_tanks
            If t.image IsNot Nothing Then
                Dim butt As New Button
                Dim m = ww / t.image.Width
                butt.Width = t.image.Width * m
                butt.Height = t.image.Height * m
                butt.Tag = cnt.ToString & ":A"
                butt.Text = t.gui_string.ToUpper.Replace("_", " ")
                butt.BackgroundImage = t.image
                butt.BackgroundImageLayout = ImageLayout.Stretch
                butt.TextAlign = ContentAlignment.TopRight
                butt.ForeColor = Color.White
                butt.Font = New Font(pfc.Families(0), 6, System.Drawing.FontStyle.Regular)
                Dim lbl As New Label
                lbl.Width = butt.Width
                lbl.Height = butt.Height - 3
                lbl.BackColor = Color.Transparent
                lbl.Font = butt.Font
                lbl.ForeColor = Color.Red 'dont matter. it will show as gray cuz its disabled.
                lbl.TextAlign = ContentAlignment.BottomLeft
                lbl.Text = t.weight
                lbl.Enabled = False
                butt.Controls.Add(lbl)
                lbl.Location = New System.Drawing.Point(3, 0)
                ' butt.BackColor = Color.Wheat
                AddHandler butt.Click, AddressOf Me.picked_tank
                p1.Controls.Add(butt)
                butt.Location = (New System.Drawing.Point(0, cnt * butt.Height))
                cnt += 1
            End If
        Next
        'russian
        cnt = 0
        For Each t In r_tanks
            If t.image IsNot Nothing Then
                Dim butt As New Button
                Dim m = ww / t.image.Width
                butt.Width = t.image.Width * m
                butt.Height = t.image.Height * m
                butt.Tag = cnt.ToString & ":R"
                butt.Text = t.gui_string.ToUpper.Replace("_", " ")
                butt.BackgroundImage = t.image
                butt.BackgroundImageLayout = ImageLayout.Stretch
                butt.TextAlign = ContentAlignment.TopRight
                butt.ForeColor = Color.White
                butt.Font = New Font(pfc.Families(0), 6, System.Drawing.FontStyle.Regular)
                Dim lbl As New Label
                lbl.Width = butt.Width
                lbl.Height = butt.Height - 3
                lbl.BackColor = Color.Transparent
                lbl.Font = butt.Font
                lbl.ForeColor = Color.Red 'dont matter. it will show as gray cuz its disabled.
                lbl.TextAlign = ContentAlignment.BottomLeft
                lbl.Text = t.weight
                lbl.Enabled = False
                butt.Controls.Add(lbl)
                lbl.Location = New System.Drawing.Point(3, 0)
                ' butt.BackColor = Color.Wheat
                AddHandler butt.Click, AddressOf Me.picked_tank
                p2.Controls.Add(butt)
                butt.Location = (New System.Drawing.Point(0, cnt * butt.Height))
                cnt += 1
            End If
        Next
        'german
        cnt = 0
        For Each t In g_tanks
            If t.image IsNot Nothing Then
                Dim butt As New Button
                Dim m = ww / t.image.Width
                butt.Width = t.image.Width * m
                butt.Height = t.image.Height * m
                butt.Tag = cnt.ToString & ":G"
                butt.Text = t.gui_string.ToUpper.Replace("_", " ")
                butt.BackgroundImage = t.image
                butt.BackgroundImageLayout = ImageLayout.Stretch
                butt.TextAlign = ContentAlignment.TopRight
                butt.ForeColor = Color.White
                butt.Font = New Font(pfc.Families(0), 6, System.Drawing.FontStyle.Regular)
                Dim lbl As New Label
                lbl.Width = butt.Width
                lbl.Height = butt.Height - 3
                lbl.BackColor = Color.Transparent
                lbl.Font = butt.Font
                lbl.ForeColor = Color.Red 'dont matter. it will show as gray cuz its disabled.
                lbl.TextAlign = ContentAlignment.BottomLeft
                lbl.Text = t.weight
                lbl.Enabled = False
                butt.Controls.Add(lbl)
                lbl.Location = New System.Drawing.Point(3, 0)
                ' butt.BackColor = Color.Wheat
                AddHandler butt.Click, AddressOf Me.picked_tank
                p3.Controls.Add(butt)
                butt.Location = (New System.Drawing.Point(0, cnt * butt.Height))
                cnt += 1
            End If
        Next
        'british
        cnt = 0
        For Each t In b_tanks
            If t.image IsNot Nothing Then
                Dim butt As New Button
                Dim m = ww / t.image.Width
                butt.Width = t.image.Width * m
                butt.Height = t.image.Height * m
                butt.Tag = cnt.ToString & ":B"
                butt.Text = t.gui_string.ToUpper.Replace("_", " ")
                butt.BackgroundImage = t.image
                butt.BackgroundImageLayout = ImageLayout.Stretch
                butt.TextAlign = ContentAlignment.TopRight
                butt.ForeColor = Color.White
                butt.Font = New Font(pfc.Families(0), 6, System.Drawing.FontStyle.Regular)
                Dim lbl As New Label
                lbl.Width = butt.Width
                lbl.Height = butt.Height - 3
                lbl.BackColor = Color.Transparent
                lbl.Font = butt.Font
                lbl.ForeColor = Color.Red 'dont matter. it will show as gray cuz its disabled.
                lbl.TextAlign = ContentAlignment.BottomLeft
                lbl.Text = t.weight
                lbl.Enabled = False
                butt.Controls.Add(lbl)
                lbl.Location = New System.Drawing.Point(3, 0)
                ' butt.BackColor = Color.Wheat
                AddHandler butt.Click, AddressOf Me.picked_tank
                p4.Controls.Add(butt)
                butt.Location = (New System.Drawing.Point(0, cnt * butt.Height))
                cnt += 1
            End If
        Next
        'french
        cnt = 0
        For Each t In f_tanks
            If t.image IsNot Nothing Then
                Dim butt As New Button
                Dim m = ww / t.image.Width
                butt.Width = t.image.Width * m
                butt.Height = t.image.Height * m
                butt.Tag = cnt.ToString & ":F"
                butt.Text = t.gui_string.ToUpper.Replace("_", " ")
                butt.Text = butt.Text.Replace("ILLON", " ")
                butt.BackgroundImage = t.image
                butt.BackgroundImageLayout = ImageLayout.Stretch
                butt.TextAlign = ContentAlignment.TopRight
                butt.ForeColor = Color.White
                butt.Font = New Font(pfc.Families(0), 6, System.Drawing.FontStyle.Regular)
                Dim lbl As New Label
                lbl.Width = butt.Width
                lbl.Height = butt.Height - 3
                lbl.BackColor = Color.Transparent
                lbl.Font = butt.Font
                lbl.ForeColor = Color.Red 'dont matter. it will show as gray cuz its disabled.
                lbl.TextAlign = ContentAlignment.BottomLeft
                lbl.Text = t.weight
                lbl.Enabled = False
                butt.Controls.Add(lbl)
                lbl.Location = New System.Drawing.Point(3, 0)
                ' butt.BackColor = Color.Wheat
                AddHandler butt.Click, AddressOf Me.picked_tank
                p5.Controls.Add(butt)
                butt.Location = (New System.Drawing.Point(0, cnt * butt.Height))
                cnt += 1
            End If
        Next
        'chinese
        cnt = 0
        For Each t In c_tanks
            If t.image IsNot Nothing Then
                Dim butt As New Button
                Dim m = ww / t.image.Width
                butt.Width = t.image.Width * m
                butt.Height = t.image.Height * m
                butt.Tag = cnt.ToString & ":C"
                butt.Text = t.gui_string.ToUpper.Replace("_", " ")
                butt.BackgroundImage = t.image
                butt.BackgroundImageLayout = ImageLayout.Stretch
                butt.TextAlign = ContentAlignment.TopRight
                butt.ForeColor = Color.White
                butt.Font = New Font(pfc.Families(0), 6, System.Drawing.FontStyle.Regular)
                Dim lbl As New Label
                lbl.Width = butt.Width
                lbl.Height = butt.Height - 3
                lbl.BackColor = Color.Transparent
                lbl.Font = butt.Font
                lbl.ForeColor = Color.Red 'dont matter. it will show as gray cuz its disabled.
                lbl.TextAlign = ContentAlignment.BottomLeft
                lbl.Text = t.weight
                lbl.Enabled = False
                butt.Controls.Add(lbl)
                lbl.Location = New System.Drawing.Point(3, 0)
                ' butt.BackColor = Color.Wheat
                AddHandler butt.Click, AddressOf Me.picked_tank
                p6.Controls.Add(butt)
                butt.Location = (New System.Drawing.Point(0, cnt * butt.Height))
                cnt += 1
            End If
        Next
        'Japan
        cnt = 0
        For Each t In j_tanks
            If t.image IsNot Nothing Then
                Dim butt As New Button
                Dim m = ww / t.image.Width
                butt.Width = t.image.Width * m
                butt.Height = t.image.Height * m
                butt.Tag = cnt.ToString & ":J"
                butt.Text = t.gui_string.ToUpper.Replace("_", " ")
                butt.BackgroundImage = t.image
                butt.BackgroundImageLayout = ImageLayout.Stretch
                butt.TextAlign = ContentAlignment.TopRight
                butt.ForeColor = Color.White
                butt.Font = New Font(pfc.Families(0), 6, System.Drawing.FontStyle.Regular)
                Dim lbl As New Label
                lbl.Width = butt.Width
                lbl.Height = butt.Height - 3
                lbl.BackColor = Color.Transparent
                lbl.Font = butt.Font
                lbl.ForeColor = Color.Red 'dont matter. it will show as gray cuz its disabled.
                lbl.TextAlign = ContentAlignment.BottomLeft
                lbl.Text = t.weight
                lbl.Enabled = False
                butt.Controls.Add(lbl)
                lbl.Location = New System.Drawing.Point(3, 0)
                ' butt.BackColor = Color.Wheat
                AddHandler butt.Click, AddressOf Me.picked_tank
                p7.Controls.Add(butt)
                butt.Location = (New System.Drawing.Point(0, cnt * butt.Height))
                cnt += 1
            End If
        Next
        'Lets grab the nation flags and put them in the imagelist
        'set the image size

        Tankimagelist.ImageSize = New System.Drawing.Point(36, 23)
        Using z As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\gui.pkg")
            Dim ens = z.Entries
            For Each ze In ens
                If InStr(ze.FileName, "/nations/") > 0 Then
                    If InStr(ze.FileName, "usa.png") > 0 Then
                        Dim bms As New MemoryStream
                        ze.Extract(bms)
                        Tankimagelist.Images.Add(get_tank_image(bms, 0, False).Clone)
                        bms.Dispose()
                    End If
                End If
            Next

            For Each ze In ens
                If InStr(ze.FileName, "/nations/") > 0 Then

                    If InStr(ze.FileName, "ussr.png") > 0 Then
                        Dim bms As New MemoryStream
                        ze.Extract(bms)
                        Tankimagelist.Images.Add(get_tank_image(bms, 0, False).Clone)
                        bms.Dispose()
                    End If
                End If
            Next
            For Each ze In ens
                If InStr(ze.FileName, "/nations/") > 0 Then
                    If InStr(ze.FileName, "germany.png") > 0 Then
                        Dim bms As New MemoryStream
                        ze.Extract(bms)
                        Tankimagelist.Images.Add(get_tank_image(bms, 0, False).Clone)
                        bms.Dispose()
                    End If
                End If
            Next
            For Each ze In ens
                If InStr(ze.FileName, "/nations/") > 0 Then
                    If InStr(ze.FileName, "uk.png") > 0 Then
                        Dim bms As New MemoryStream
                        ze.Extract(bms)
                        Tankimagelist.Images.Add(get_tank_image(bms, 0, False).Clone)
                        bms.Dispose()
                    End If
                End If
            Next
            For Each ze In ens
                If InStr(ze.FileName, "/nations/") > 0 Then
                    If InStr(ze.FileName, "france.png") > 0 Then
                        Dim bms As New MemoryStream
                        ze.Extract(bms)
                        Tankimagelist.Images.Add(get_tank_image(bms, 0, False).Clone)
                        bms.Dispose()
                    End If
                End If
            Next
            For Each ze In ens
                If InStr(ze.FileName, "/nations/") > 0 Then
                    If InStr(ze.FileName, "china.png") > 0 Then
                        Dim bms As New MemoryStream
                        ze.Extract(bms)
                        Tankimagelist.Images.Add(get_tank_image(bms, 0, False).Clone)
                        bms.Dispose()
                    End If
                End If
            Next
            For Each ze In ens
                If InStr(ze.FileName, "/nations/") > 0 Then
                    If InStr(ze.FileName, "japan.png") > 0 Then
                        Dim bms As New MemoryStream
                        ze.Extract(bms)
                        Tankimagelist.Images.Add(get_tank_image(bms, 0, False).Clone)
                        bms.Dispose()
                    End If
                End If
            Next


        End Using

        AddHandler npb.Paint, AddressOf npb_Paint
        npb.BorderStyle = BorderStyle.None
        npb.Width = 98
        npb.Height = 23
        npb.BackColor = Color.Black
        frmTanks.Panel1.Controls.Add(npb)
        npb.BringToFront()
        npb.Location = New System.Drawing.Point(16, 0)

        Dim br As New Button
        AddHandler br.MouseDown, AddressOf frmTanks.next_nation
        br.FlatStyle = FlatStyle.Flat
        br.FlatAppearance.BorderSize = 0
        br.Width = 15
        br.Height = 23
        br.BackColor = Color.DimGray
        br.Image = My.Resources.control
        frmTanks.Panel1.Controls.Add(br)
        br.Location = New System.Drawing.Point(16 + 99, 0)
        br.BringToFront()
        br.Anchor = AnchorStyles.Right Or AnchorStyles.Top

        Dim bl As New Button
        AddHandler bl.MouseDown, AddressOf frmTanks.prev_nation
        bl.FlatStyle = FlatStyle.Flat
        bl.FlatAppearance.BorderSize = 0
        bl.Width = 15
        bl.Height = 23
        bl.BackColor = Color.DimGray
        bl.Image = My.Resources.control_180
        frmTanks.Panel1.Controls.Add(bl)
        bl.Location = New System.Drawing.Point(0, 0)
        bl.BringToFront()
        bl.Anchor = AnchorStyles.Right Or AnchorStyles.Top

        'you would think the controls would have the index that they are added. this is NOT the case!
        'This code reorders them.
        frmTanks.Panel1.Controls.SetChildIndex(p1, 0)
        frmTanks.Panel1.Controls.SetChildIndex(p2, 1)
        frmTanks.Panel1.Controls.SetChildIndex(p3, 2)
        frmTanks.Panel1.Controls.SetChildIndex(p4, 3)
        frmTanks.Panel1.Controls.SetChildIndex(p5, 4)
        frmTanks.Panel1.Controls.SetChildIndex(p6, 5)
        frmTanks.Panel1.Controls.SetChildIndex(p7, 6)

        'frmTanks.Panel1.Controls.SetChildIndex(npb, 6)
        frmTanks.Panel1.Controls.SetChildIndex(npb, 7) 'picturebox
        frmTanks.Panel1.Controls.SetChildIndex(br, 8)
        frmTanks.Panel1.Controls.SetChildIndex(bl, 9)
        p1.Visible = True
        p2.Visible = False
        p3.Visible = False
        p4.Visible = False
        p5.Visible = False
        p6.Visible = False
        p7.Visible = False
        current_nation = 0
        Application.DoEvents()
        npb.Update()

    End Sub
    Private Sub npb_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        'this draws the current nation at the top of the panel.
        If frmTanks.Panel1.Controls.Count < 8 Then Return 'Cant draw if it dont exist so return
        Using g As Graphics = npb.CreateGraphics
            Dim font As New Font("Arial", 10.0!, System.Drawing.FontStyle.Bold)
            Dim brush As New SolidBrush(Color.White)
            Dim brush2 As New SolidBrush(Color.DimGray)
            Dim rect = npb.ClientRectangle
            g.FillRectangle(brush2, rect)
            g.DrawString(nations(current_nation), font, brush, 0, 3)
            g.DrawImage(Tankimagelist.Images(current_nation), 100 - 36, 0)
        End Using

    End Sub

    Public Sub update_npb()
        If frmTanks.Panel1.Controls.Count < 9 Then Return ' cant draw if it dont exist so return
        Using g As Graphics = npb.CreateGraphics
            Dim font As New Font("Arial", 10.0!, System.Drawing.FontStyle.Bold)
            Dim brush As New SolidBrush(Color.White)
            Dim brush2 As New SolidBrush(Color.DimGray)
            Dim rect = npb.ClientRectangle
            g.FillRectangle(brush2, rect)
            g.DrawString(nations(current_nation), font, brush, 0, 3)
            g.DrawImage(Tankimagelist.Images(current_nation), 100 - 36, 0)
        End Using

    End Sub

    Public Sub make_map_buttons()
        'this is called here to keep the order correct
        dummy_texture = make_dummy_texture()

        Dim f = System.IO.File.ReadAllLines(Application.StartupPath.ToString + "\map_list.txt")
        Dim cnt As Integer = 0
        For Each fi In f
            If fi.Contains("#") Then
                GoTo dontaddthis
            End If
            ReDim Preserve loadmaplist(cnt + 1)
            loadmaplist(cnt) = New map_item_
            loadmaplist(cnt).name = fi
            Dim a = fi.Split(":")
            loadmaplist(cnt).realname = a(1)
            cnt += 1
dontaddthis:
        Next
        ReDim Preserve loadmaplist(cnt - 1)

        Array.Sort(loadmaplist)
        Application.DoEvents()

        Using Zip As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\gui.pkg")
            cnt = 0
            For Each thing In loadmaplist
                Dim itm = thing.name
                If Not itm.Contains("#") Then
                    Dim ar = itm.Split(":")
                    Dim entry As Ionic.Zip.ZipEntry = Zip("gui\maps\icons\map\small\" + ar(0))
                    Dim ms As New MemoryStream
                    entry.Extract(ms)
                    Dim img = get_tank_image(ms, cnt, True)
                    cnt += 1
                End If
            Next

        End Using
    End Sub


    Private Sub build_tank_data(ByRef tank() As tank_, ByVal xml_name As String, ByVal nation As String, nat_short As String)
        Dim tank_number As Integer = 0
        Try
            Dim script_pkg = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\scripts.pkg")
            Dim script As Ionic.Zip.ZipEntry = script_pkg("scripts\item_defs\vehicles\" & xml_name)
            Dim ms As New MemoryStream
            script.Extract(ms)

            openXml_stream(ms, "list")
            ms.Dispose()
            Dim tl(50) As String
            Dim fi As New System.IO.DirectoryInfo(Application.StartupPath & "\tanks\")
            Dim di = fi.GetFiles
            Using z As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\gui.pkg")
                Dim ens = z.Entries
                For Each fn In di
                    If fn.Extension = nation Then
                        tank(tank_number) = New tank_

                        Dim ar = fn.Name.Split(".")
                        tb1.Text = "Getting Tank Data: " + ar(0) ' let the user know whats going on
                        Application.DoEvents()
                        Dim ar2 = ar(0).Split("_")
                        Dim tankNnum As String = ar2(0).ToLower
                        Dim s As String = ""
                        For i = 1 To ar2.Length - 1
                            If i > 1 Then
                                s += "_" + ar2(i)
                            Else
                                s += ar2(i)
                            End If
                        Next
                        Dim da_real_name = s

                        If nation = ".british" Then s = ar(0)
                        If nation = ".chinese" Then s = ar(0)
                        For Each n In real_names_list
                            If n.Contains(s) Then
                                Dim n_s = n.Split(",")
                                s = n_s(0)
                                da_real_name = n_s(1)
                                s = s.Replace(vbLf, "")
                            End If
                        Next
                        Dim t As DataTable = xmldataset.Tables(ar(0))
                        If t Is Nothing Then
                            t = xmldataset.Tables(ar(0) + "_IGR")
                            If t Is Nothing Then
                                MsgBox("Cant find tank in gui.pkg", MsgBoxStyle.Exclamation, "Opps")
                                Return
                            End If
                        End If
                        Dim qq = From row In t Select _
                             un = row.Field(Of String)("userString"), _
                             tags = row.Field(Of String)("tags") _
                             Order By tags Descending
                        Dim weight = ""
                        Dim u_name = ""

                        For Each rr In qq
                            u_name = rr.un
                            weight = rr.tags
                        Next
                        If u_name = "" Then
                            Stop
                        End If
                        ar2 = t.TableName.Split("_")

                        ar = weight.Split(" ")
                        For ix = 0 To tank_weights.Length - 1
                            If InStr(ar(0), tank_weights(ix)) > 0 Then
                                tank(tank_number).weight = tank_types(ix)
                                tank(tank_number).sortorder = tank_sortorder(ix)
                                tank(tank_number).gui_string = da_real_name
                                tank(tank_number).file_name = fn.Name
                                For Each ze In ens
                                    If ze.FileName.Contains(ar2(0)) And Not ze.FileName.Contains("small") _
                                        And Not ze.FileName.Contains("fallout") _
                                        And ze.FileName.Contains(nat_short) _
                                    And InStr(ze.FileName, "contour") = 0 And InStr(ze.FileName, "unique") = 0 Then
                                        Dim bms As New MemoryStream
                                        ze.Extract(bms)
                                        tank(tank_number).image = get_tank_image(bms, 0, False).Clone
                                        bms.Dispose()
                                        GC.Collect()
                                    End If
nope:
                                Next

                            End If
                        Next

                        tank_number += 1
                        ReDim Preserve tank(tank_number)
                        t.Dispose()
                    End If
                Next
            End Using
        Catch ex As Exception
            frmSplash.TopMost = False
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Opps")
        End Try
        ReDim Preserve tank(tank_number - 1)
        Array.Sort(tank)
    End Sub
    Friend Function get_tank(ByVal name As String, ByRef tank As t_l) As Integer
        'If Not maploaded Then Return 0
        Dim path = Application.StartupPath & "\tanks\" & name
        Dim f_ = System.IO.File.Open(path, FileMode.Open, FileAccess.Read)
        Dim v(3) As Single
        Dim n(3) As Single
        Dim uv(2) As Single
        Dim b As New BinaryReader(f_)
        'Dim dump() = b.ReadBytes(1000)
        Dim poly_count As UInt32 = b.ReadUInt32

        'get tracks polys
        If Gl.glIsList(tank.track_displaylist) Then
            Gl.glDeleteLists(tank.track_displaylist, 1)
        End If
        Dim ID = Gl.glGenLists(1)
        Gl.glNewList(ID, Gl.GL_COMPILE)

        tank.track_displaylist = ID
        Gl.glBegin(Gl.GL_TRIANGLES)
        Dim cnt As Integer = 0 ' for debug
        'start pushing vertices
        For c As UInt32 = 0 To poly_count - 1

            v(0) = -b.ReadSingle
            v(1) = b.ReadSingle
            v(2) = b.ReadSingle
            n(0) = -b.ReadSingle
            n(1) = b.ReadSingle
            n(2) = b.ReadSingle
            'uv(0) = -b.ReadSingle
            'uv(1) = b.ReadSingle
            Gl.glNormal3fv(n)
            'Gl.glTexCoord2fv(uv)
            Gl.glVertex3fv(v)
            cnt += 1
        Next
        Gl.glEnd()
        Gl.glEndList()
        Gl.glFinish()

        '----------------------------
        ' None of this turned out usable
        ' It slows the drawing down, uses to much mem and
        ' for the inprovment in looks, it aint worth it.
        '----------------------------
        ' get chassis texture
        '    Dim t_size = b.ReadUInt64
        '    If t_size = 0 Then GoTo read_hull
        '    Dim imgl() = b.ReadBytes(t_size)
        '    File.WriteAllBytes("c:\test.dds", imgl)
        '    Dim t_ms As New MemoryStream(imgl)
        '    tank.tracks_texId = get_texture(t_ms, False)
        '    '---------------------------------------------------
        'read_hull:
        '    'get rest of the tanks polys
        '    If Gl.glIsList(tank.hull_displaylist) Then
        '      Gl.glDeleteLists(tank.hull_displaylist, 1)
        '    End If
        '    ID = Gl.glGenLists(1)
        '    Gl.glNewList(ID, Gl.GL_COMPILE)
        '    tank.hull_displaylist = ID
        '    Gl.glBegin(Gl.GL_TRIANGLES)
        '    poly_count = b.ReadUInt32

        '    'start pushing vertices
        '    For c As UInt32 = 0 To poly_count - 1
        '      v(0) = -b.ReadSingle
        '      v(1) = b.ReadSingle
        '      v(2) = b.ReadSingle
        '      n(0) = -b.ReadSingle
        '      n(1) = b.ReadSingle
        '      n(2) = b.ReadSingle
        '      uv(0) = -b.ReadSingle
        '      uv(1) = b.ReadSingle
        '      Gl.glNormal3fv(n)
        '      Gl.glTexCoord2fv(uv)
        '      Gl.glVertex3fv(v)
        '    Next
        '    Gl.glEnd()
        '    Gl.glEndList()
        '    Gl.glFinish()
        '    ' get chassis texture
        '    t_size = b.ReadUInt64
        '    Dim img2() = b.ReadBytes(t_size)
        '    Dim t_ms2 As New MemoryStream(img2)
        '    tank.hull_textId = get_texture(t_ms2, False)
        '    '---------------------------------------------------

        b.Close()
        f_.Close()
        f_.Dispose()
        Return ID

    End Function
    Public Sub picked_tank(ByVal sender As Object, ByVal e As System.EventArgs)
        If team_setup_selected_tank.Length = 0 Then
            Return
        End If
        Dim ar1 = team_setup_selected_tank.Split("_")
        Dim team As Integer = ar1(0)
        Dim onground As Boolean = False
        Dim b_index As Integer
        If ar1.Length > 2 Then
            onground = True
            b_index = ar1(3)
        Else
            b_index = ar1(1)
        End If
        If tankID > -1 Then
            If tankID >= 100 Then
                b_index = tankID - 100
                team = 2
            Else
                b_index = tankID
                team = 1
            End If
        End If
        Dim s As String = sender.tag
        Dim ar = s.Split(":")
        Dim ID As Integer = ar(0)
        'If Gl.glIsList(tankID) Then
        '    Gl.glDeleteLists(tankID, 1)
        'End If
        If team = 1 Then
            frmTanks.SplitContainer1.Panel1.Controls(b_index).Font = sender.font
            frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = sender.text
            locations.team_1(b_index).name = sender.text
            Select Case ar(1)
                Case "A"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                    = a_tanks(ID).image

                    get_tank(a_tanks(ID).file_name, locations.team_1(b_index))

                    locations.team_1(b_index).id = team.ToString + "_A_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_A_" + ID.ToString & "_" & b_index.ToString
                    ' frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = a_tanks(ID).gui_string
                    locations.team_1(b_index).type = a_tanks(ID).sortorder
                Case "R"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                    = r_tanks(ID).image

                    get_tank(r_tanks(ID).file_name, locations.team_1(b_index))

                    locations.team_1(b_index).id = team.ToString + "_R_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_R_" + ID.ToString & "_" & b_index.ToString
                    'frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = r_tanks(ID).gui_string
                    locations.team_1(b_index).type = r_tanks(ID).sortorder
                Case "G"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                    = g_tanks(ID).image

                    get_tank(g_tanks(ID).file_name, locations.team_1(b_index))

                    locations.team_1(b_index).id = team.ToString + "_G_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_G_" + ID.ToString & "_" & b_index.ToString
                    'frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = g_tanks(ID).gui_string
                    locations.team_1(b_index).type = g_tanks(ID).sortorder
                Case "B"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                    = b_tanks(ID).image

                    get_tank(b_tanks(ID).file_name, locations.team_1(b_index))

                    locations.team_1(b_index).id = team.ToString + "_B_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_B_" + ID.ToString & "_" & b_index.ToString
                    'frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = b_tanks(ID).gui_string
                    locations.team_1(b_index).type = b_tanks(ID).sortorder
                Case "F"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                    = f_tanks(ID).image
                    Application.DoEvents()
                    get_tank(f_tanks(ID).file_name, locations.team_1(b_index))

                    locations.team_1(b_index).id = team.ToString + "_F_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_F_" + ID.ToString & "_" & b_index.ToString
                    ' frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = f_tanks(ID).gui_string
                    locations.team_1(b_index).type = f_tanks(ID).sortorder
                Case "C"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                    = c_tanks(ID).image

                    get_tank(c_tanks(ID).file_name, locations.team_1(b_index))

                    locations.team_1(b_index).id = team.ToString + "_C_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_C_" + ID.ToString & "_" & b_index.ToString
                    ' frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = c_tanks(ID).gui_string
                    locations.team_1(b_index).type = c_tanks(ID).sortorder
                Case "J"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                    = j_tanks(ID).image

                    get_tank(j_tanks(ID).file_name, locations.team_1(b_index))

                    locations.team_1(b_index).id = team.ToString + "_J_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_J_" + ID.ToString & "_" & b_index.ToString
                    ' frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = c_tanks(ID).gui_string
                    locations.team_1(b_index).type = j_tanks(ID).sortorder
            End Select
            SyncLock packet_lock
                Packet_out.ID = locations.team_1(b_index).id
            End SyncLock
            If Not onground Then
                locations.team_1(b_index).loc_x = look_point_X
                locations.team_1(b_index).loc_z = look_point_Z
                locations.team_1(b_index).rot_y = Cam_X_angle + PI
                Packet_out.Tx = look_point_X
                Packet_out.Tz = look_point_Z
                Packet_out.Tr = Cam_X_angle + PI
            Else
                look_point_X = locations.team_1(b_index).loc_x
                look_point_Z = locations.team_1(b_index).loc_z
                locations.team_1(b_index).rot_y = Cam_X_angle + PI
                Packet_out.Tr = Cam_X_angle + PI
            End If
            tankID = b_index
            draw_scene()
        Else
            frmTanks.SplitContainer1.Panel2.Controls(b_index).Font = sender.font
            frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = sender.text
            locations.team_2(b_index).name = sender.text
            Select Case ar(1)
                Case "A"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = a_tanks(ID).image
                    get_tank(a_tanks(ID).file_name, locations.team_2(b_index))
                    locations.team_2(b_index).id = team.ToString + "_A_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_A_" + ID.ToString & "_" & b_index.ToString
                    'frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = a_tanks(ID).gui_string
                    locations.team_2(b_index).type = a_tanks(ID).sortorder
                Case "R"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = r_tanks(ID).image
                    get_tank(r_tanks(ID).file_name, locations.team_2(b_index))
                    locations.team_2(b_index).id = team.ToString + "_R_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_R_" + ID.ToString & "_" & b_index.ToString
                    ' frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = r_tanks(ID).gui_string
                    locations.team_2(b_index).type = r_tanks(ID).sortorder
                Case "G"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = g_tanks(ID).image
                    get_tank(g_tanks(ID).file_name, locations.team_2(b_index))
                    locations.team_2(b_index).id = team.ToString + "_G_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_G_" + ID.ToString & "_" & b_index.ToString
                    ' frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = g_tanks(ID).gui_string
                    locations.team_2(b_index).type = g_tanks(ID).sortorder
                Case "B"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = b_tanks(ID).image
                    get_tank(b_tanks(ID).file_name, locations.team_2(b_index))
                    locations.team_2(b_index).id = team.ToString + "_B_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_B_" + ID.ToString & "_" & b_index.ToString
                    ' frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = b_tanks(ID).gui_string
                    locations.team_2(b_index).type = b_tanks(ID).sortorder
                Case "F"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = f_tanks(ID).image
                    get_tank(f_tanks(ID).file_name, locations.team_2(b_index))
                    locations.team_2(b_index).id = team.ToString + "_F_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_F_" + ID.ToString & "_" & b_index.ToString
                    ' frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = f_tanks(ID).gui_string
                    locations.team_2(b_index).type = f_tanks(ID).sortorder
                Case "C"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = c_tanks(ID).image

                    get_tank(c_tanks(ID).file_name, locations.team_2(b_index))

                    locations.team_2(b_index).id = team.ToString + "_C_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_C_" + ID.ToString & "_" & b_index.ToString
                    ' frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = c_tanks(ID).gui_string
                    locations.team_2(b_index).type = c_tanks(ID).sortorder
                Case "J"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = j_tanks(ID).image

                    get_tank(j_tanks(ID).file_name, locations.team_2(b_index))

                    locations.team_2(b_index).id = team.ToString + "_J_" + ID.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_J_" + ID.ToString & "_" & b_index.ToString
                    ' frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = c_tanks(ID).gui_string
                    locations.team_2(b_index).type = j_tanks(ID).sortorder
            End Select
            SyncLock packet_lock
                Packet_out.ID = locations.team_2(b_index).id
            End SyncLock
            If Not onground Then
                locations.team_2(b_index).loc_x = look_point_X
                locations.team_2(b_index).loc_z = look_point_Z
                locations.team_2(b_index).rot_y = Cam_X_angle + PI
                Packet_out.Tx = look_point_X
                Packet_out.Tz = look_point_Z
                Packet_out.Tr = Cam_X_angle + PI
            Else
                look_point_X = locations.team_2(b_index).loc_x
                look_point_Z = locations.team_2(b_index).loc_z
                locations.team_2(b_index).rot_y = Cam_X_angle + PI
                Packet_out.Tr = Cam_X_angle + PI
            End If
            tankID = b_index + 100
            draw_scene()

        End If
        old_tankID = tankID
        Packet_out.tankId = tankID
        If team = 1 Then
            frmTanks.SplitContainer1.Panel1.Controls(b_index).BackColor = Color.Blue
            look_point_X = locations.team_1(b_index).loc_x
            look_point_Y = get_Z_at_XY(locations.team_1(b_index).loc_x, locations.team_1(b_index).loc_z)
            look_point_Z = locations.team_1(b_index).loc_z
        Else
            frmTanks.SplitContainer1.Panel2.Controls(b_index).BackColor = Color.Blue
            look_point_X = locations.team_2(b_index).loc_x
            look_point_Y = get_Z_at_XY(locations.team_2(b_index).loc_x, locations.team_2(b_index).loc_z)
            look_point_Z = locations.team_2(b_index).loc_z

        End If

    End Sub


    Public Function GetOGLPos_Decals(ByVal x As Integer, ByVal y As Integer) As Integer
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT Or Gl.GL_STENCIL_BUFFER_BIT)
        Gl.glDisable(Gl.GL_FOG)
        seek_scene_decals()
        If m_show_fog.Checked Then
            Gl.glEnable(Gl.GL_FOG)
        End If
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        Dim type = pixel(3)
        Dim index As UInt32 = (CUInt(pixel(1) * 256) + pixel(2))
        If type = 0 Then
            If index > 0 Then
                index = index - 1
                With decal_matrix_list(index)

                    d_counter = index
                    Dim str As String = "Decal Index: " + index.ToString("0000") + _
                    vbCrLf + "Terrain Bias:" + .t_bias.ToString("0.00") + _
                    " Decal Bias: " + .t_bias.ToString("0.00") + " Excluded:" + .exclude.ToString
                    tb1.Text = str
                    If EDIT_INCULDERS Then
                        Dim s = "-" + index.ToString("0000")
                        TERRAIN_BIAS = .t_bias
                        DECAL_BIAS = .d_bias
                        EXCLUDED = .exclude
                        If decal_includers_string.Contains(s) Then
                            Dim newlist As List(Of String) = frmBiasing.results_tb.Lines.ToList
                            Dim cnt = 0
                            For Each line In newlist
                                If line.Contains(s) Then
                                    newlist.RemoveAt(cnt)
                                    frmBiasing.results_tb.Lines = newlist.ToArray
                                    tb1.Text += " (Removed)"
                                    frmBiasing.info_tb.Text = "Decal -" + index.ToString("0000") + " (Removed)"
                                    decal_includers_string = frmBiasing.results_tb.Text
                                    Exit For
                                End If
                                cnt += 1
                            Next
                        Else
                            TERRAIN_BIAS = .t_bias
                            DECAL_BIAS = .d_bias
                            EXCLUDED = .exclude
                            decal_matrix_list(index).old_bias = DECAL_BIAS
                            If frmBiasing.Visible Then
                                Application.DoEvents()

                                frmBiasing.terrain_clip.Value = CInt((TERRAIN_BIAS / 15.0) * 1000.0)
                                frmBiasing.terrain_clip.PerformLayout()
                                frmBiasing.terrain_clip.Invalidate()
                                Application.DoEvents()
                                frmBiasing.decal_clip.Value = CInt((DECAL_BIAS / 15.0) * 1000.0)
                                frmBiasing.decal_clip.PerformLayout()
                                frmBiasing.decal_clip.Invalidate()
                                Application.DoEvents()
                                frmBiasing.exclude_chk_box.Checked = .exclude
                                frmBiasing.exclude_chk_box.Invalidate()
                                Application.DoEvents()
                            End If
                            s = "-" + index.ToString("0000") + ":" + .t_bias.ToString("0.00") + ":" + .d_bias.ToString("0.00") + ":" + .exclude.ToString + vbCrLf
                            decal_includers_string += s
                            tb1.Text += " (Added)"
                            frmBiasing.info_tb.Text = "Decal -" + index.ToString("0000") + " (Added)"
                        End If
                    End If

                    decal_includers_string = decal_includers_string.Replace(vbCrLf + vbCrLf, vbCrLf)
                    frmBiasing.results_tb.Text = decal_includers_string
                    'get_decal_bias_settings()
                End With
                Application.DoEvents()
                Return True
            End If
        Else
        End If
        tb1.Text = "Nothing...."
        Application.DoEvents()
        Return False
    End Function
    Public Sub seek_scene_decals()

        Gl.glEnable(Gl.GL_DEPTH_TEST)

        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_BLEND)

        Gl.glClearColor(0.0F, 0.0F, 0.0F, 0.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        ResizeGL()
        ViewPerspective()   ' set 3d view mode
        'set_eyes()

        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Dim red, blue, green, type As Byte
        type = 0
        '        '---------------------------------
        '        ' draw the decal boxes with its own color
        '        '---------------------------------
        '        'if we are in team edit mode.we dont display any models
        If maploaded Then   ' cant let this try and draw shit that isnt there yet!!!
            If m_show_decals.Checked Then
                For model As UInt32 = 0 To Models.matrix.Length - 1
                    Gl.glColor4ub(0, 0, 0, 0)
                    For k = 0 To Models.models(model)._count - 1
                        If Models.models(model).componets(k).callList_ID > 0 Then

                            Gl.glPushMatrix()
                            Gl.glMultMatrixf(Models.matrix(model).matrix)

                            Gl.glCallList(Models.models(model).componets(k).callList_ID)
                            Gl.glPopMatrix()
                        End If
                    Next
                Next
                For k As UInt32 = 0 To decal_matrix_list.Length - 1
                    green = 0 : blue = 0 : type = 0
                    If decal_matrix_list(k).display_id > 0 Then
                        Gl.glPushMatrix()
                        red = 0
                        green = CByte((k + 1 And &HFF00) >> 8)
                        blue = CByte(k + 1 And &HFF)
                        Gl.glColor4ub(CByte(red), CByte(green), CByte(blue), CByte(type))
                        Gl.glMultMatrixf(decal_matrix_list(k).matrix)
                        glutSolidCube(1.0)
                        Gl.glPopMatrix()
                    End If
                Next
            End If


        End If
        '        '--------------------------------------------------------------------------
        Gl.glFinish()
        Application.DoEvents()
    End Sub


    Public Sub GetOGLPos(ByVal x As Integer, ByVal y As Integer)
        'ResizeGL()
        Gl.glPushMatrix()
        If GetOGLPos_Decals(x, y) Then
            Gl.glPopMatrix()
            draw_scene()
            'draw_scene()
            Return
        End If

        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        'Gl.glPushMatrix()
        Gl.glDisable(Gl.GL_FOG)
        seek_scene()
        'Gl.glPopMatrix()
        If m_show_fog.Checked Then
            Gl.glEnable(Gl.GL_FOG)
        End If
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        _SELECTED_map = 0
        _SELECTED_tree = 0
        _SELECTED_model = 0
        Dim type = pixel(3)
        Dim component = pixel(0)
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        Dim index As UInt32 = (CUInt(pixel(1) * 256) + pixel(2))
        If m_layout_mode.Checked Then
            tankID = -1
            Dim r = pixel(0)
            Dim b = pixel(2)
            SyncLock packet_lock
                If r > 0 Then
                    tankID = r - 1
                End If
                If b > 0 Then
                    tankID = (b - 1) + 100
                End If
                If old_tankID > -1 Then
                    If old_tankID >= 100 Then
                        frmTanks.SplitContainer1.Panel2.Controls(old_tankID - 100).BackColor = Color.Green
                    Else
                        frmTanks.SplitContainer1.Panel1.Controls(old_tankID).BackColor = Color.DarkRed
                    End If
                End If
                old_tankID = tankID
                Packet_out.tankId = tankID

                If tankID > -1 Then

                    If tankID < 100 Then
                        frmTanks.SplitContainer1.Panel1.Controls(tankID).BackColor = Color.Blue
                        look_point_X = locations.team_1(tankID).loc_x
                        look_point_Y = get_Z_at_XY(locations.team_1(tankID).loc_x, locations.team_1(tankID).loc_z)
                        look_point_Z = locations.team_1(tankID).loc_z
                        m_comment.Text = locations.team_1(tankID).comment
                        Cam_X_angle = locations.team_1(tankID).rot_y - PI
                        Packet_out.Rx = Cam_X_angle
                        Packet_out.Ex = locations.team_1(tankID).loc_x
                        Packet_out.Ez = locations.team_1(tankID).loc_z
                        Packet_out.ID = locations.team_1(tankID).id

                        Packet_out.Tx = locations.team_1(tankID).loc_x
                        Packet_out.Tz = locations.team_1(tankID).loc_z
                        Packet_out.Tr = locations.team_1(tankID).rot_y
                        Packet_out.ID = locations.team_1(tankID).id
                    Else
                        frmTanks.SplitContainer1.Panel2.Controls(tankID - 100).BackColor = Color.Blue
                        look_point_X = locations.team_2(tankID - 100).loc_x
                        look_point_Y = get_Z_at_XY(locations.team_2(tankID - 100).loc_x, locations.team_2(tankID - 100).loc_z)
                        look_point_Z = locations.team_2(tankID - 100).loc_z
                        m_comment.Text = locations.team_2(tankID - 100).comment
                        Cam_X_angle = locations.team_2(tankID - 100).rot_y - PI
                        Packet_out.Rx = Cam_X_angle
                        Packet_out.Ex = locations.team_2(tankID - 100).loc_x
                        Packet_out.Ez = locations.team_2(tankID - 100).loc_z
                        Packet_out.ID = locations.team_2(tankID - 100).id

                        Packet_out.Tx = locations.team_2(tankID - 100).loc_x
                        Packet_out.Tz = locations.team_2(tankID - 100).loc_z
                        Packet_out.Tr = locations.team_2(tankID - 100).rot_y
                        Packet_out.ID = locations.team_2(tankID - 100).id
                    End If
                End If
            End SyncLock

        Else
            If type = 0 Then
                If index > 0 Then
                    index = index - 1
                    _SELECTED_model = index + 1
                    tb1.Text = "Index: " + index.ToString("0000") + _
                    vbCrLf + "Model: " + vbCrLf
                    frmBiasing.info_tb.Text = "Model Index: " + index.ToString("0000")
                    If EDIT_INCULDERS Then

                        Dim s = "+" + index.ToString + vbCrLf
                        If decal_includers_string.Contains(s) Then
                            decal_includers_string = decal_includers_string.Replace(s, "")
                            tb1.Text += " (Removed)"
                            frmBiasing.info_tb.Text += " (Removed)"
                            Model_Matrix_list(index).mask = False
                        Else
                            decal_includers_string += s
                            tb1.Text += " (Added)"
                            frmBiasing.info_tb.Text += " (Added)"
                            Model_Matrix_list(index).mask = True
                        End If
                        decal_includers_string = decal_includers_string.Replace(vbCrLf + vbCrLf, vbCrLf)
                        frmBiasing.results_tb.Text = decal_includers_string
                        check_decal_include_strings()
                    Else
                        tb1.Text = index.ToString("0000") + " : " + Model_Matrix_list(index).primitive_name

                    End If
                    Application.DoEvents()
                End If
            Else
                _SELECTED_model = 0
                tb1.Text = "Nothing...."
                Application.DoEvents()
            End If



        End If
        Gl.glPopMatrix()
        draw_scene()
    End Sub
    Public Sub seek_scene()

        Gl.glEnable(Gl.GL_DEPTH_TEST)

        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_BLEND)

        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        ResizeGL()
        ViewPerspective()   ' set 3d view mode
        'set_eyes()

        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)

        Dim red, blue, green, type As Byte
        type = 100
        '        '---------------------------------
        '        ' draw the models.each with its own color
        '        '---------------------------------
        '        'if we are in team edit mode.we dont display any models
        If maploaded Then   ' cant let this try and draw shit that isnt there yet!!!
            If Not frmTanks.Visible Then
                If m_show_models.Checked Then

                    For model As UInt32 = 0 To Models.matrix.Length - 1
                        green = 0 : blue = 0 : type = 0
                        For k = 0 To Models.models(model)._count - 1
                            If Models.models(model).componets(k).callList_ID > 0 Then

                                Gl.glPushMatrix()
                                Gl.glMultMatrixf(Models.matrix(model).matrix)
                                red = 0
                                Dim model_ = model + 1
                                green = CByte((model_ And &HFF00) >> 8)
                                blue = CByte(model_ And &HFF)
                                Gl.glColor4ub(CByte(red), CByte(green), CByte(blue), CByte(type))
                                Gl.glCallList(Models.models(model).componets(k).callList_ID)
                                Gl.glPopMatrix()
                            End If
                        Next
                    Next
                End If

            Else
                '                ' cant let this try and draw shit that isnt there yet!!!
                Dim cv = 57.2957795
                For i = 0 To 14
                    If locations.team_1(i).track_displaylist > -1 Then
                        Gl.glPushMatrix()
                        Gl.glTranslatef(locations.team_1(i).loc_x, get_Z_at_XY(locations.team_1(i).loc_x, locations.team_1(i).loc_z), locations.team_1(i).loc_z)
                        Dim rot = locations.team_1(i).rot_y
                        Dim rx = (cv * surface_normal.y * Cos(rot)) + (cv * surface_normal.x * Sin(rot))
                        Dim rz = (cv * surface_normal.x * Cos(rot)) + (cv * -surface_normal.y * Sin(rot))
                        Gl.glRotatef(cv * rot, 0.0, 1.0, 0.0)
                        Gl.glRotatef(rz, 0.0, 0.0, 1.0)
                        Gl.glRotatef(rx, -1.0, 0.0, 0.0)
                        red = i + 1
                        green = 0
                        blue = 0
                        Gl.glColor4ub(CInt(red), CInt(green), CInt(blue), 0)
                        Gl.glCallList(locations.team_1(i).track_displaylist)
                        Gl.glPopMatrix()
                    End If
                Next
                For i = 0 To 14
                    If locations.team_2(i).track_displaylist > -1 Then
                        Gl.glPushMatrix()
                        Gl.glTranslatef(locations.team_2(i).loc_x, get_Z_at_XY(locations.team_2(i).loc_x, locations.team_2(i).loc_z), locations.team_2(i).loc_z)
                        Dim rot = locations.team_2(i).rot_y
                        Dim rx = (cv * surface_normal.y * Cos(rot)) + (cv * surface_normal.x * Sin(rot))
                        Dim rz = (cv * surface_normal.x * Cos(rot)) + (cv * -surface_normal.y * Sin(rot))
                        Gl.glRotatef(cv * rot, 0.0, 1.0, 0.0)
                        Gl.glRotatef(rz, 0.0, 0.0, 1.0)
                        Gl.glRotatef(rx, -1.0, 0.0, 0.0)
                        Gl.glRotatef(cv * rot, 0.0, 1.0, 0.0)
                        Gl.glRotatef(rz, 0.0, 0.0, 1.0)
                        Gl.glRotatef(rx, -1.0, 0.0, 0.0)
                        red = 0
                        green = 0
                        blue = i + 1
                        Gl.glColor4ub(CInt(red), CInt(green), CInt(blue), 0)
                        Gl.glCallList(locations.team_2(i).track_displaylist)
                        Gl.glPopMatrix()
                    End If
                Next
            End If

        End If
        '        '--------------------------------------------------------------------------
        Gl.glFinish()
        'Gdi.SwapBuffers(pb1_hDC)
        'Stop
        'Gdi.SwapBuffers(pb1_hDC)
        'Gl.glDisable(Gl.GL_DEPTH_TEST)
        'Gl.glEnable(Gl.GL_LIGHTING)
    End Sub


    Private Sub show_data(ByVal v As vect3, ByVal n As Integer)
        frmDebug.tb.Text += "N:" + n.ToString + String.Format("X{0,12:F4} Y{0,12:F4} Z{0,12:F4} ", v.x, v.y, v.z) + vbCrLf
    End Sub
    Dim bias() As Single = {0.5, 0.0, 0.0, 0.0, 0.0, 0.5, 0.0, 0.0, 0.0, 0.0, 0.5, 0.0, 0.5, 0.5, 0.5, 1.0}


    Public Sub draw_little_window(decal As Integer)
        If Not maploaded Then
            Return
        End If
        'Return
        Gl.glPushMatrix()
        Gl.glTranslatef(0.0, 0.0, 200.0)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
        Gl.glDisable(Gl.GL_LIGHTING)
        'Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
        Dim e = Gl.glGetError

        Gl.glColor3f(0.5, 0.5, 0.5)

        '---
        '============================================
        'draw textures
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, coMapID)
        'Gl.glBindTexture(Gl.GL_TEXTURE_2D, minimap_textureid)
        Gl.glBegin(Gl.GL_QUADS)
        '---
        Dim gridS = frmBiasing.SB_SIZE
        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex3f(0, 0, 0.0)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex3f(gridS, 0, 0.0)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex3f(gridS, -gridS, 0.0)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(0, -gridS, 0.0)
        Gl.glEnd()


        '---
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, coMapID2)
        Gl.glBegin(Gl.GL_QUADS)

        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex3f(gridS, 0, 0.0)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex3f(gridS + gridS, 0, 0.0)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex3f(gridS + gridS, -gridS, 0.0)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(gridS, -gridS, 0.0)
        Gl.glEnd()

        Gl.glDisable(Gl.GL_TEXTURE_2D)
        '============================================
        Gl.glColor3f(1.0, 1.0, 0.0)
        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_LINE)
        'draw outline
        Gl.glBegin(Gl.GL_QUADS)
        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex3f(0, 0, 0.0)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex3f(gridS, 0, 0.0)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex3f(gridS, -gridS, 0.0)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(0, -gridS, 0.0)

        '---
        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex3f(gridS, 0, 0.0)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex3f(gridS + gridS, 0, 0.0)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex3f(gridS + gridS, -gridS, 0.0)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(gridS, -gridS, 0.0)

        Gl.glEnd()
        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)

        Gl.glPopMatrix()

    End Sub

    Private Function check_bounds(ByVal v As vect2) As vect2
        If v.x > x_max Then
            v.x = x_max
        End If
        If v.x < x_min Then
            v.x = x_min
        End If
        If v.y > z_max Then
            v.y = z_max
        End If
        If v.y < z_min Then
            v.y = z_min
        End If
        Return v
    End Function



    Public Sub draw_maps()
        If Not _STARTED Then Return
        ' If Not SHOW_MAPS Then Return
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            'MessageBox.Show("Unable to make rendering context current")
            Return
        End If
        Gl.glDisable(Gl.GL_FRAMEBUFFER_SRGB_EXT)
        ResizeGL()
        ViewOrtho()

        'gl_busy = True
        If stopGL Then Return
        If stopGL Then Return
        If stopGL Then Return
        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)


        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        Gl.glDisable(Gl.GL_BLEND)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
        'ResizeGL()

        Dim w = pb1.Width
        Dim h = pb1.Height
        If w = 0 Then
            Return

        End If
        Dim ms_x As Single = 120
        Dim ms_y As Single = -72
        Dim space_x As Single = 15

        Dim w_cnt As Single = Floor(w / (ms_x + space_x))
        Dim border As Single = (w - (w_cnt * (ms_x + space_x))) / 2
        Dim map As Integer = 0
        Dim v_cnt = (map_texture_ids.Length - 1) / w_cnt
        If (v_cnt * (ms_x + space_x)) + (border * 2) < pb1.Width Then
            v_cnt -= 1
        End If
        Dim v_pos As Integer = 0
        Dim vi, hi, sz As Single

        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        vi = -15
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        While map < map_texture_ids.Length
            If pb1.Width = 0 Then
                Exit While
            End If
            For i = 0 To w_cnt - 1
                If map + 1 = map_texture_ids.Length Then
                    Exit While
                End If
                hi = border + (i * (ms_x + space_x))
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_texture_ids(map))
                Gl.glColor3f(1.0, 1.0, 1.0)
                Gl.glBegin(Gl.GL_QUADS)
                If selected_map_hit > 0 Then
                    If selected_map_hit - 1 = map Then
                        sz = 20
                    Else
                        sz = 0
                    End If
                End If
                Gl.glTexCoord2f(0, 1)
                Gl.glVertex2f(-sz + hi, -sz + vi + ms_y)

                Gl.glTexCoord2f(0, 0)
                Gl.glVertex2f(-sz + hi, sz + vi)

                Gl.glTexCoord2f(1, 0)
                Gl.glVertex2f(sz + hi + ms_x, sz + vi)

                Gl.glTexCoord2f(1, 1)
                Gl.glVertex2f(sz + hi + ms_x, -sz + vi + ms_y)

                Gl.glEnd()
                map += 1
            Next
            vi += -space_x + ms_y
        End While
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        If selected_map_hit > 0 Then
            glutPrintBox(mouse.X, -mouse.Y, loadmaplist(selected_map_hit - 1).realname, 1.0, 1.0, 1.0, 1.0)

        End If
        Gdi.SwapBuffers(pb1_hDC)
        Application.DoEvents()
    End Sub
    Public Sub draw_pick_map()
        ResizeGL()
        ViewOrtho()
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_DEPTH_TEST)

        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_TEXTURE_2D)

        Dim w = pb1.Width
        Dim h = pb1.Height
        If w = 0 Then
            Return
        End If
        Dim ms_x As Single = 120
        Dim ms_y As Single = -72
        Dim space_x As Single = 15

        Dim w_cnt As Single = Floor(w / (ms_x + space_x))
        Dim border As Single = (w - (w_cnt * (ms_x + space_x))) / 2
        Dim map As Byte = 0
        Dim v_cnt = (map_texture_ids.Length - 1) / w_cnt
        If (v_cnt * (ms_x + space_x)) + (border * 2) < pb1.Width Then
            v_cnt -= 1
        End If
        Dim v_pos As Integer = 0
        Dim vi, hi As Single

        vi = -15

        Gl.glBegin(Gl.GL_QUADS)
        While True
            If pb1.Width = 0 Then
                Exit While
            End If
            For i = 0 To w_cnt - 1
                map += 1
                Gl.glColor4ub(CByte(map), CByte(map), CByte(map), CByte(255))
                Gl.glFinish()
                Gl.glFinish()
                Gl.glFinish()
                Gl.glFinish()
                Gl.glFinish()
                If map = map_texture_ids.Length Then
                    Exit While
                End If
                hi = border + (i * (ms_x + space_x))

                Gl.glVertex3f(hi, vi + ms_y, 0.0)

                Gl.glVertex3f(hi, vi, 0.0)

                Gl.glVertex3f(hi + ms_x, vi, 0.0)

                Gl.glVertex3f(hi + ms_x, vi + ms_y, 0.0)

            Next
            vi += -space_x + ms_y
        End While
        Gl.glEnd()
        Gl.glFinish()


        '--------------------------------------------------------------------------
        Gl.glFinish()
        'Gdi.SwapBuffers(pb1_hDC)
        'Gdi.SwapBuffers(pb1_hDC)
        'Gl.glDisable(Gl.GL_DEPTH_TEST)
        'Gl.glEnable(Gl.GL_LIGHTING)

    End Sub
    Public Sub gl_pick_map(ByVal x As Integer, ByVal y As Integer)
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            'MessageBox.Show("Unable to make rendering context current")
            'End
        End If
        m_Orbit_Light.Checked = False
        m_fly_map.Checked = False
        Gl.glDisable(Gl.GL_FRAMEBUFFER_SRGB_EXT)

        Gl.glDisable(Gl.GL_FOG)
        Gl.glReadBuffer(Gl.GL_BACK)
        draw_pick_map()


        If m_show_fog.Checked Then
            Gl.glEnable(Gl.GL_FOG)
        End If
        Dim viewport(4) As Integer
        Gl.glFinish()
        Dim pixel() As Byte = {0, 0, 0, 0}


        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        Gl.glFinish()

        Dim hit = pixel(2)
        If hit > 0 Then
            selected_map_hit = hit
            tb1.Text = loadmaplist(hit - 1).realname
            Application.DoEvents()
            Application.DoEvents()
            Application.DoEvents()
            Application.DoEvents()
        Else
            selected_map_hit = 0
            'tb1.Text = x.ToString + "   " + y.ToString + vbCrLf + hit.ToString
            Application.DoEvents()
        End If
        Gl.glFinish()
        Application.DoEvents()
        draw_maps()
    End Sub



    Private Sub start_drawing()

    End Sub
    Private Sub draw_terrain()
        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
        'dim shadowMap As Integer
        'c_address = -1
        'Gl.glEnable(Gl.GL_STENCIL_TEST)
        'Gl.glStencilMask(&HFF)
        'Gl.glStencilFunc(Gl.GL_ALWAYS, 0, &HFFFF)
        'Gl.glStencilOp(Gl.GL_KEEP, Gl.GL_ZERO, Gl.GL_REPLACE)

        Dim d1, d2, d3 As Single
        If maploaded And Not m_wire_terrain.Checked Then
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glColor4f(0.6, 0.0, 0.0, 0.5)
            If m_map_border.Checked Then
                Gl.glColor3f(0.7, 0.0, 0.0)
                Gl.glCallList(map_borderId)
            End If
            If m_show_map_grid.Checked Then
                Gl.glColor3f(0.7, 0.7, 0.0)
                Gl.glCallList(sector_outlineID)
            End If
            Gl.glColor3f(0.8, 0.8, 0.8)
            Dim u, v As vect4
            Dim rad As Single
            For i = 0 To test_count
                d1 = eyeX - maplist(i).location.x
                d2 = eyeZ - maplist(i).location.y
                d3 = eyeY - maplist(i).location.z
                rad = (d1 ^ 2) + (d2 ^ 2) + (d3 ^ 2)
                If Not hz_loaded Then ' must have the hirez data or we cant render it.
                    rad = 3000000 ' force low rez rendering of terrain.
                End If
                If Not m_high_rez_Terrain.Checked Then
                    rad = 3000000 ' force low rez rendering of terrain.
                End If
                If m_hell_mode.Checked Then
                    rad = 3000000 ' force low rez rendering of hell terrain.
                End If
                If rad > 200000 Then
                    'low rez terrain.
                    If m_hell_mode.Checked Then
                        Gl.glUseProgram(shader_list.hell_shader)

                    Else
                        Gl.glUseProgram(shader_list.ss_shader)

                    End If
                    Gl.glUniform1f(a_address2, lighting_ambient)
                    Gl.glUniform1f(t_address2, lighting_terrain_texture * sun_multiplier)
                    Gl.glUniform1i(c_address2, 0)
                    Gl.glUniform1i(n_address2, 1)
                    Gl.glUniform1f(gamma_2, gamma_level * 0.75)
                    Gl.glUniform1f(gray_level_2, gray_level)

                    If m_show_fog.Checked Then
                        Gl.glUniform1i(f_address2, 1)
                    Else
                        Gl.glUniform1i(f_address2, 0)
                    End If
                    rendermode = True
                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(i).colorMapId)
                    Gl.glActiveTexture(Gl.GL_TEXTURE1)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(i).normMapID)
                Else

                    'hi rez terrain.
                    Gl.glUseProgram(shader_list.render_shader)
                    'Dim er = Gl.glGetError
                    'sr = Glu.gluErrorString(er)
                    Gl.glUniform1f(row_address, maplist(i).row)
                    Gl.glUniform1f(col_address, maplist(i).col)
                    Gl.glUniform1f(tile_w, tile_width)
                    Gl.glUniform1i(used_layers, map_layers(i).used_layers)
                    u = map_layers(i).layers(1).uP
                    Gl.glUniform4f(layer0U, u.x, u.y, u.z, u.w)
                    u = map_layers(i).layers(2).uP
                    Gl.glUniform4f(layer1U, u.x, u.y, u.z, u.w)
                    u = map_layers(i).layers(3).uP
                    Gl.glUniform4f(layer2U, u.x, u.y, u.z, u.w)
                    u = map_layers(i).layers(4).uP
                    Gl.glUniform4f(layer3U, u.x, u.y, u.z, u.w)

                    v = map_layers(i).layers(1).vP
                    Gl.glUniform4f(layer0V, v.x, v.y, v.z, v.w)
                    v = map_layers(i).layers(2).vP
                    Gl.glUniform4f(layer1V, v.x, v.y, v.z, v.w)
                    v = map_layers(i).layers(3).vP
                    Gl.glUniform4f(layer2V, v.x, v.y, v.z, v.w)
                    v = map_layers(i).layers(4).vP
                    Gl.glUniform4f(layer3V, v.x, v.y, v.z, v.w)

                    Gl.glUniform1i(main_texture, map_layers(i).main_texture)
                    'lighting variables
                    Gl.glUniform1f(a_address, lighting_ambient)
                    Gl.glUniform1f(t_address, lighting_terrain_texture * sun_multiplier)
                    Gl.glUniform1f(gray_level_1, gray_level)

                    Gl.glUniform3f(c_position, eyeX, eyeY, eyeZ)
                    Gl.glUniform1i(n_address, 1)
                    Gl.glUniform1f(gamma, gamma_level * 0.75)
                    If m_show_fog.Checked Then
                        Gl.glUniform1i(f_address, 1)
                    Else
                        Gl.glUniform1i(f_address, 0)
                    End If
                    'er = Gl.glGetError
                    'bind the lowrez normal map
                    Gl.glUniform1i(n_address, 0)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(i).normMapID)
                    ' bind all the textures
                    Gl.glUniform1i(layer_1, 1)
                    Gl.glUniform1i(layer_2, 2)
                    Gl.glUniform1i(layer_3, 3)
                    Gl.glUniform1i(layer_4, 4)
                    Gl.glUniform1i(n_layer_1, 5)
                    Gl.glUniform1i(n_layer_2, 6)
                    Gl.glUniform1i(n_layer_3, 7)
                    Gl.glUniform1i(n_layer_4, 8)
                    Gl.glUniform1i(mixtexture, 9)
                    Gl.glUniform1i(c_address, 10)
                    Gl.glUniform1i(dominateTex, 11)
                    If Gl.glIsTexture(map_layers(i).layers(1).text_id) Then
                        Gl.glActiveTexture(Gl.GL_TEXTURE1)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(1).text_id)
                    End If
                    If Gl.glIsTexture(map_layers(i).layers(2).text_id) Then
                        Gl.glActiveTexture(Gl.GL_TEXTURE2)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(2).text_id)
                    End If
                    If Gl.glIsTexture(map_layers(i).layers(3).text_id) Then
                        Gl.glActiveTexture(Gl.GL_TEXTURE3)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(3).text_id)
                    End If
                    If Gl.glIsTexture(map_layers(i).layers(4).text_id) Then
                        Gl.glActiveTexture(Gl.GL_TEXTURE4)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(4).text_id)
                    End If
                    If Gl.glIsTexture(map_layers(i).layers(1).norm_id) Then
                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(1).norm_id)
                    End If
                    If Gl.glIsTexture(map_layers(i).layers(2).norm_id) Then
                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(2).norm_id)
                    End If
                    If Gl.glIsTexture(map_layers(i).layers(3).norm_id) Then
                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(3).norm_id)
                    End If
                    If Gl.glIsTexture(map_layers(i).layers(4).norm_id) Then
                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 8)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(4).norm_id)
                    End If
                    'er = Gl.glGetError
                    'sr = Glu.gluErrorString(er)
                    'bind the mix layer. 
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 9)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, mix_atlas_Id)
                    'bind lowrez colormap
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 10)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(i).colorMapId)
                    'bind ShadowMap ** not used. yet
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 11)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(i).DominateId)

                    'er = Gl.glGetError
                    'sr = Glu.gluErrorString(er)
                End If
                Gl.glCallList(maplist(i).calllist_Id)
                Gl.glCallList(maplist(i).seamCallId)
                Gl.glUseProgram(0)
            Next
            For patato = 0 To 10
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + patato)
                Gl.glEnable(Gl.GL_TEXTURE_2D)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
                Gl.glDisable(Gl.GL_TEXTURE_2D)
            Next
        End If
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        'Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        'Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        If m_show_chunks.Checked And maploaded Then
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glColor3f(0.4, 0.4, 0.4)
            For i = 0 To test_count
                Gl.glCallList(maplist(i).seamCallId)
            Next
        End If
        '---------------------------------------------------------------------------------
        'draw the map sections and seams WIRE
        If maploaded And m_wire_terrain.Checked And Not m_hell_mode.Checked Then
            phong_cam_pos = Gl.glGetUniformLocation(shader_list.comp_shader, "cam_pos")
            bump_out_ = Gl.glGetUniformLocation(shader_list.comp_shader, "amount")
            Gl.glUseProgram(shader_list.comp_shader)
            Gl.glUniform3f(phong_cam_pos, eyeX, eyeY, eyeZ)
            Gl.glUniform1f(bump_out_, 0)
            Gl.glColor3f(0.1, 0.1, 0.3)
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
            Gl.glPolygonOffset(1.0, 1.0)
            For i = 0 To test_count
                Gl.glCallList(maplist(i).calllist_Id)
                Gl.glCallList(maplist(i).seamCallId)
            Next
            Gl.glDisable(Gl.GL_POLYGON_OFFSET_LINE)
            Gl.glPolygonOffset(0.0, 0.0)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
            Gl.glColor3f(0.0, 0.0, 0.0)
            For i = 0 To test_count
                Gl.glCallList(maplist(i).calllist_Id)
                Gl.glCallList(maplist(i).seamCallId)
            Next
            Gl.glUseProgram(0)
            If normal_mode > 0 Then
                Gl.glColor3f(0.5, 0.5, 0.5)
                Gl.glUseProgram(shader_list.normal_shader)
                Gl.glUniform1i(view_normal_mode_, normal_mode)
                Gl.glUniform1f(normal_length_, 0.2)
                For i = 0 To test_count
                    Gl.glCallList(maplist(i).calllist_Id)
                    Gl.glCallList(maplist(i).seamCallId)
                Next
                Gl.glUseProgram(0)
            End If
            Gl.glDisable(Gl.GL_DEPTH_TEST)
            Gl.glBegin(Gl.GL_LINE_STRIP)
            Gl.glColor3f(1.0, 0.0, 0.0)
            Gl.glVertex3f(tl_.X, tl_.Z, tl_.Y)
            Gl.glVertex3f(tr_.X, tr_.Z, tr_.Y)
            Gl.glVertex3f(br_.X, br_.Z, br_.Y)
            Gl.glVertex3f(bl_.X, bl_.Z, bl_.Y)
            Gl.glVertex3f(tl_.X, tl_.Z, tl_.Y)
            Gl.glEnd()
            Gl.glEnable(Gl.GL_DEPTH_TEST)
            Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
        End If

        '---------------------------------------------------------------------------------

    End Sub
    Private Sub draw_models()
        ' draw the models
        Gl.glEnable(Gl.GL_CULL_FACE)
        If maploaded And m_wire_models.Checked And m_show_models.Checked Then
            Gl.glUseProgram(shader_list.comp_shader)
            Gl.glUniform3f(phong_cam_pos, eyeX, eyeY, eyeZ)
            Gl.glUniform1f(bump_out_, 0.005)
            Gl.glColor3f(0.2, 0.2, 0.0)
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glActiveTexture(Gl.GL_TEXTURE1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)

            For model As UInt32 = 0 To Models.matrix.Length - 1
                For k = 0 To Models.models(model)._count - 1
                    Gl.glPushMatrix()
                    Gl.glMultMatrixf(Models.matrix(model).matrix)
                    Gl.glCallList(Models.models(model).componets(k).callList_ID)
                    Gl.glPopMatrix()
                Next
            Next
            Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
            Gl.glPolygonOffset(0.0, 0.0)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
            Gl.glColor3f(0.0, 0.0, 0.0)
            Gl.glUniform1f(bump_out_, -0.005)
            For model As UInt32 = 0 To Models.matrix.Length - 1
                For k = 0 To Models.models(model)._count - 1
                    Gl.glPushMatrix()
                    Gl.glMultMatrixf(Models.matrix(model).matrix)
                    Gl.glCallList(Models.models(model).componets(k).callList_ID)
                    Gl.glPopMatrix()
                Next
            Next
            Gl.glUseProgram(0)
            Gl.glColor3f(0.5, 0.5, 0.5)
            If normal_mode > 0 Then
                Gl.glUseProgram(shader_list.normal_shader)
                Gl.glUniform1i(view_normal_mode_, normal_mode)
                Gl.glUniform1f(normal_length_, 0.1)
                For model As UInt32 = 0 To Models.matrix.Length - 1
                    For k = 0 To Models.models(model)._count - 1
                        Gl.glPushMatrix()
                        Gl.glMultMatrixf(Models.matrix(model).matrix)
                        Gl.glCallList(Models.models(model).componets(k).callList_ID)
                        Gl.glPopMatrix()
                    Next
                Next
                Gl.glUseProgram(0)
            End If

        End If
        '---------------------------------------------------------------------------------
        If maploaded And m_show_models.Checked _
                    And Not m_hell_mode.Checked _
                    And Not m_wire_models.Checked _
                    Then ' cant let this try and draw shit that isnt there yet!!!
            Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
            ' Gl.glColor3f(lighting_model_level, lighting_model_level, lighting_model_level)
            If m_bump_map_models.Checked And model_bump_loaded Then
                'Gl.glDisable(Gl.GL_LIGHTING)
                Gl.glDisable(Gl.GL_TEXTURE_2D)
                Gl.glUseProgram(shader_list.bump_shader)
                If m_show_fog.Checked Then
                    Gl.glUniform1i(f_address3, 1)
                Else
                    Gl.glUniform1i(f_address3, 0)
                End If
                Gl.glUniform1i(c_address3, 0)
                Gl.glUniform1i(n_address3, 1)
                Gl.glUniform1i(colormap2, 2)
                ' Gl.glUniform3f(c_position3, eyeX, eyeY, eyeZ)
                Gl.glUniform1f(t_address3, lighting_model_level)
                Gl.glUniform1f(gray_level_3, gray_level)
                Gl.glUniform1f(gamma_3, gamma_level)
                Gl.glUniform1f(model_ambient, lighting_ambient)

                Dim uv2s As Boolean
                For model As UInt32 = 0 To Models.matrix.Length - 1
                    If Models.matrix(model).matrix IsNot Nothing Then
                        Gl.glUniformMatrix4fv(u_mat3, 1, Gl.GL_FALSE, Models.matrix(model).matrix)    ' pass matrix

                        Gl.glPushMatrix()
                        Gl.glMultMatrixf(Models.matrix(model).matrix)
                        For k = 0 To Models.models(model)._count - 1

                            Gl.glActiveTexture(Gl.GL_TEXTURE0)
                            Gl.glEnable(Gl.GL_TEXTURE_2D)
                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Models.models(model).componets(k).color_id)
                            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                            Gl.glEnable(Gl.GL_TEXTURE_2D)
                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Models.models(model).componets(k).normal_Id)
                            uv2s = Models.models(model).componets(k).multi_textured
                            If (Not m_show_uv2.Checked) Or (Not uv2s_loaded) Then
                                uv2s = False    ' this stops showing the 2nd uv2 textures 
                            End If
                            If uv2s Then
                                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                                Gl.glEnable(Gl.GL_TEXTURE_2D)
                                Gl.glBindTexture(Gl.GL_TEXTURE_2D, Models.models(model).componets(k).color2_Id)
                                Gl.glUniform1i(is_multi_textured, 1)
                            Else
                                Gl.glUniform1i(is_multi_textured, 0)
                            End If

                            If Models.models(model).componets(k).bumped Then
                                Gl.glUniform1i(is_bumped3, 1)
                            Else
                                Gl.glUniform1i(is_bumped3, 0)
                            End If

                            If Models.models(model).componets(k).GAmap Then
                                Gl.glUniform1i(is_GAmap, 1)      ' tell shader if this is a bumped model
                            Else
                                Gl.glUniform1i(is_GAmap, 0)     ' tell shader if this is a bumped model

                            End If
                            Gl.glUniform1i(alphaRef, Models.models(model).componets(k).alphaRef)
                            Gl.glUniform1i(alphaTestEnable, Models.models(model).componets(k).alphaTestEnable)


                            Gl.glCallList(Models.models(model).componets(k).callList_ID)
                        Next
                        Gl.glPopMatrix()
                    End If
                Next
                Gl.glUseProgram(0)
            Else
                Gl.glUseProgram(0)
                Gl.glEnable(Gl.GL_LIGHTING)
                Gl.glColor3f(lighting_model_level, lighting_model_level, lighting_model_level)
                'Gl.glEnable(Gl.GL_ALPHA_TEST)
                Gl.glAlphaFunc(Gl.GL_GREATER, 0.3)

                Gl.glActiveTexture(Gl.GL_TEXTURE3)
                Gl.glDisable(Gl.GL_TEXTURE_2D)
                ' Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

                Gl.glActiveTexture(Gl.GL_TEXTURE2)
                Gl.glDisable(Gl.GL_TEXTURE_2D)
                'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

                Gl.glActiveTexture(Gl.GL_TEXTURE1)
                Gl.glDisable(Gl.GL_TEXTURE_2D)
                'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

                Gl.glActiveTexture(Gl.GL_TEXTURE0)
                Gl.glEnable(Gl.GL_TEXTURE_2D)

                For model As UInt32 = 0 To Models.matrix.Length - 1
                    If Models.matrix(model).matrix IsNot Nothing Then
                        Gl.glPushMatrix()
                        Gl.glMultMatrixf(Models.matrix(model).matrix)
                        'Gl.glActiveTexture(Gl.GL_TEXTURE0)
                        For k = 0 To Models.models(model)._count - 1
                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Models.models(model).componets(k).color_id)
                            'If Models.models(model).componets(k).multi_textured Then
                            '    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                            '    Gl.glBindTexture(Gl.GL_TEXTURE_2D, Models.models(model).componets(k).color2_Id)
                            '    Gl.glEnable(Gl.GL_TEXTURE_2D)
                            'Else
                            '    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                            '    Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
                            '    Gl.glEnable(Gl.GL_TEXTURE_2D)
                            'End If
                            Gl.glCallList(Models.models(model).componets(k).callList_ID)
                        Next
                        Gl.glPopMatrix()
                    End If
                Next
            End If
            Gl.glDisable(Gl.GL_ALPHA_TEST)
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glActiveTexture(Gl.GL_TEXTURE3)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glActiveTexture(Gl.GL_TEXTURE2)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glActiveTexture(Gl.GL_TEXTURE1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        End If
        '---------------------------------------------------------------------------------
        Gl.glUseProgram(0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)

    End Sub
    Private Sub draw_trees()
        'draw trees wire
        If maploaded And m_wire_trees.Checked And m_show_trees.Checked And Not m_hell_mode.Checked Then
            Gl.glEnable(Gl.GL_LIGHTING)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            Gl.glPolygonOffset(1.0, 1.0)
            Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glColor3f(0.0, 0.2, 0.0)
            For mode = 0 To 1
                If mode = 0 Then
                Else
                    Gl.glUseProgram(shader_list.leafcolored_shader)
                End If

                For i As UInt32 = 0 To speedtree_matrix_list.Length - 2
                    Gl.glPushMatrix()
                    Gl.glMultMatrixf(Trees.matrix(i).matrix)
                    If Trees.flora(i).branch_displayID > 0 And mode = 0 Then
                        Gl.glCallList(Trees.flora(i).branch_displayID)
                    End If
                    If Trees.flora(i).frond_displayID > 0 And mode = 0 Then
                        Gl.glCallList(Trees.flora(i).frond_displayID)
                    End If
                    If Trees.flora(i).leaf_displayID > 0 And mode = 1 Then
                        Gl.glCallList(Trees.flora(i).leaf_displayID)
                    End If
                    Gl.glPopMatrix()
                Next
                Gl.glUseProgram(0)
            Next
            Gl.glDisable(Gl.GL_POLYGON_OFFSET_LINE)
            Gl.glPolygonOffset(0.0, 0.0)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
            Gl.glColor3f(0.0, 0.0, 0.0)
            'For mode = 0 To 1
            '    If mode = 0 Then
            '    Else
            '    End If

            '    For i As UInt32 = 0 To speedtree_matrix_list.Length - 2
            '        Gl.glPushMatrix()
            '        Gl.glMultMatrixf(Trees.matrix(i).matrix)
            '        If Trees.flora(i).branch_displayID > 0 And mode = 0 Then
            '            Gl.glCallList(Trees.flora(i).branch_displayID)
            '        Else
            '        End If
            '        If Trees.flora(i).frond_displayID > 0 And mode = 0 Then
            '            Gl.glCallList(Trees.flora(i).frond_displayID)
            '        End If
            '        If Trees.flora(i).leaf_displayID > 0 And mode = 1 Then
            '            Gl.glCallList(Trees.flora(i).leaf_displayID)
            '        End If
            '        Gl.glPopMatrix()
            '    Next
            'Next
            For i As UInt32 = 0 To speedtree_matrix_list.Length - 2
                Gl.glPushMatrix()
                Gl.glMultMatrixf(Trees.matrix(i).matrix)
                If Trees.flora(i).branch_displayID > 0 Then
                    Gl.glCallList(Trees.flora(i).branch_displayID)
                Else
                    'Stop
                End If
                If Trees.flora(i).frond_displayID > 0 Then
                    Gl.glCallList(Trees.flora(i).frond_displayID)
                End If
                Gl.glPopMatrix()

            Next
            Gl.glUseProgram(shader_list.leafcolored_shader)
            For i As UInt32 = 0 To speedtree_matrix_list.Length - 2
                Gl.glPushMatrix()
                Gl.glMultMatrixf(Trees.matrix(i).matrix)

                If Trees.flora(i).leaf_displayID > 0 Then
                    Gl.glCallList(Trees.flora(i).leaf_displayID)
                End If
                Gl.glPopMatrix()
            Next
            Gl.glUseProgram(0)



        End If
        '---------------------------------------------------------------------------------
        'draw trees 
        If maploaded And m_show_trees.Checked _
            And Not m_wire_trees.Checked _
            And Not m_hell_mode.Checked Then

            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            Gl.glDisable(Gl.GL_CULL_FACE)


            If Trees.flora IsNot Nothing Then
                Dim rad As Single
                For mode = 0 To 1

                    If mode = 0 Then
                        Gl.glUseProgram(shader_list.trees_shader)
                        If m_show_fog.Checked Then
                            Gl.glUniform1i(f_address4, 1)
                        Else
                            Gl.glUniform1i(f_address4, 0)
                        End If
                        Gl.glUniform1i(c_address4, 0)
                        Gl.glUniform1i(n_address4, 1)
                        Gl.glUniform3f(c_position4, eyeX, eyeZ, eyeY)
                        Gl.glUniform1f(t_address4, lighting_model_level)
                        Gl.glUniform1f(gray_level_4, gray_level)
                        Gl.glUniform1f(gamma_4, gamma_level)
                        Gl.glUniform1f(branch_ambient, lighting_ambient)
                    Else
                        Gl.glUseProgram(shader_list.leaf_shader)
                        If m_show_fog.Checked Then
                            Gl.glUniform1i(leaf_fog_enable, 1)
                        Else
                            Gl.glUniform1i(leaf_fog_enable, 0)
                        End If
                        Gl.glUniform1i(leaf_c_map, 0)
                        Gl.glUniform1i(leaf_n_map, 1)
                        Gl.glUniform3f(leaf_camPos, eyeX, eyeZ, eyeY)
                        Gl.glUniform1f(leaf_level, lighting_model_level)
                        Gl.glUniform1f(leaf_gray_level, gray_level)
                        Gl.glUniform1f(leaf_contrast, gamma_level)
                        Gl.glUniform1f(leaf_ambient, lighting_ambient)
                        Gl.glDisable(Gl.GL_CULL_FACE)

                    End If
                    'Dim draw As Boolean = True
                    Dim t_cut_off As Single = 300000
                    For i As UInt32 = 0 To speedtree_matrix_list.Length - 2
                        Gl.glPushMatrix()
                        Gl.glMultMatrixf(Trees.matrix(i).matrix)
                        Dim l As vect3
                        l.x = Trees.matrix(i).matrix(12)
                        l.y = Trees.matrix(i).matrix(13)
                        l.z = Trees.matrix(i).matrix(14)
                        l.x -= eyeX
                        l.y -= eyeY
                        l.z -= eyeZ
                        rad = (l.x ^ 2) + (l.y ^ 2) + (l.z ^ 2)


                        'rad = 10
                        If m_low_quality_trees.Checked Then
                            rad = 200001
                        End If
                        'If rad > 300000 Then
                        'draw = True
                        If rad > t_cut_off And mode = 0 Then
                            'draw = False
                            Gl.glEnable(Gl.GL_CULL_FACE)
                            Gl.glActiveTexture(Gl.GL_TEXTURE0)
                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, speedtree_imageID)
                            Gl.glCallList(Trees.flora(i).billboard_displayID)
                        Else
                            Gl.glDisable(Gl.GL_CULL_FACE)
                            If mode = 0 Then
                                If Trees.flora(i).branch_displayID > 0 Then
                                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, Trees.flora(i).branch_textureID)
                                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, Trees.flora(i).branch_normalID)
                                    Gl.glCallList(Trees.flora(i).branch_displayID)
                                Else
                                End If
                                If Trees.flora(i).frond_displayID > 0 Then
                                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, speedtree_imageID)
                                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, speedtree_NormalMapID)
                                    Gl.glCallList(Trees.flora(i).frond_displayID)
                                End If
                            Else
                                If rad <= t_cut_off Then
                                    If Trees.flora(i).leaf_displayID > 0 Then
                                        Gl.glActiveTexture(Gl.GL_TEXTURE0)
                                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, speedtree_imageID)
                                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, speedtree_NormalMapID)
                                        Gl.glCallList(Trees.flora(i).leaf_displayID)
                                    End If
                                End If
                            End If
                        End If
                        Gl.glPopMatrix()
                    Next
                    Gl.glUseProgram(0)
                Next

            End If
            Gl.glUseProgram(0)
            Gl.glEnable(Gl.GL_LIGHTING)


        End If

    End Sub
    Private Sub draw_decals()

    End Sub
    Private Sub draw_dome()
        If mini_map_loaded Then

            Gl.glColor3f(1.0, 1.0, 1.0)
            Gl.glDisable(Gl.GL_LIGHTING)
            Gl.glDisable(Gl.GL_DEPTH_TEST)

            Gl.glPushMatrix()
            'Position to get best view of sky while loading a map.
            'otherwise. translate to usual position.
            If Not maploaded Then
                u_Cam_Y_angle = -0.15
                u_Cam_X_angle = 0.0
                Gl.glTranslatef(eyeX, eyeY - 8.0, eyeZ - 6.0)
                Gl.glRotatef(90.0, 0.0, 1.0, 0.0)
            Else
                Gl.glTranslatef(eyeX, eyeY - 1.0, eyeZ)
                Gl.glRotatef(90.0, 0.0, 1.0, 0.0)

            End If
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            Gl.glColor4f(0.6, 0.6, 0.6, 0.5)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, skydometextureID)
            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glCallList(skydomelist)


            Gl.glEnable(Gl.GL_CULL_FACE)
            Gl.glPopMatrix()
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glEnable(Gl.GL_DEPTH_TEST)
            Gl.glEnable(Gl.GL_LIGHTING)
        End If

    End Sub
    Private Sub draw_light_sphear()
        If m_Orbit_Light.Checked And Not m_hell_mode.Checked Then
            '---------------------------------
            'draw the sphere as our sun
            '---------------------------------
            'Z_Cursor = get_Z_at_XY(look_point_X, look_point_Z)
            Gl.glDisable(Gl.GL_LIGHTING)
            Gl.glPushMatrix()
            Gl.glTranslatef(position(0), position(1), position(2))
            Gl.glColor3f(1.0, 1.0, 0.0)
            glutSolidSphere(4.0, 8, 8)
            Gl.glPopMatrix()
            Gl.glEnable(Gl.GL_LIGHTING)
        End If
    End Sub
    Private Sub setup_fog()
        If m_show_fog.Checked Then
            'Gl.glEnable(Gl.GL_FOG)
            'fog is currently disabled in all shaders.
            'Im working towards adding this in deferred rendering
            Dim fmode As Integer = Gl.GL_EXP2
            Gl.glFogi(Gl.GL_FOG_MODE, fmode)
            Dim fogcolor() As Single = {0.6, 0.6, 0.65}

            Gl.glFogfv(Gl.GL_FOG_COLOR, fogcolor)
            Gl.glFogf(Gl.GL_FOG_DENSITY, lighting_fog_level * 0.8)
            Gl.glHint(Gl.GL_FOG_HINT, Gl.GL_NICEST)
            Gl.glFogf(Gl.GL_FOG_START, 1.0)
            Gl.glFogf(Gl.GL_FOG_END, 2.0)
        Else
        End If
    End Sub
    Private Sub draw_base_rings()
        If maploaded And Not m_hell_mode.Checked And SHOW_RINGS Then
            Gl.glColor3f(0.5, 0.0, 0.0)
            Gl.glCallList(ringDisplayID_1)

            Gl.glColor3f(0.0, 0.7, 0.0)
            Gl.glCallList(ringDisplayID_2)
        End If

    End Sub
    Private Sub setup_view()
        If Not view_mode Then

            ViewPerspective()   ' set 3d view mode
            'Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position)
            'set_eyes()
        Else
            '========================================================================
            If maploaded Then
                Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position)

                Dim c_ As vect3 = decal_matrix_list(d_counter).cam_pos
                Dim l_ As vect3 = decal_matrix_list(d_counter).look_at
                Dim tl = decal_matrix_list(d_counter).top_left
                Dim tr = decal_matrix_list(d_counter).top_right
                Dim bl = decal_matrix_list(d_counter).bot_left
                Dim br = decal_matrix_list(d_counter).bot_right
                Dim c_rot = decal_matrix_list(d_counter).cam_rotation
                Dim c_pos = decal_matrix_list(d_counter).cam_location
                Dim near = decal_matrix_list(d_counter).near_clip
                Dim far = decal_matrix_list(d_counter).far_clip
                Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
                Gl.glLoadIdentity() 'Reset The Matrix
                Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
                Gl.glLoadIdentity() 'Reset The Matrix

                Glu.gluLookAt(c_.x, c_.y, c_.z, l_.x, l_.y, l_.z, c_rot.x, c_rot.y, c_rot.z)
                Dim m(16) As Single
                Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, m)
                tl = translate_to(tl, m)
                tr = translate_to(tr, m)
                bl = translate_to(bl, m)
                br = translate_to(br, m)
                'l_ = translate_to(l_, m)
                Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
                Gl.glLoadIdentity() 'Reset The Mat4rix
                With decal_matrix_list(d_counter)
                    Dim scale = pb1.Height / pb1.Width
                    'Gl.glOrtho(-lr, lr, (-lr) * Scale(), (lr) * Scale(), -5, 40) 'Select Ortho Mode
                    Gl.glOrtho(br.x / scale, tl.x / scale, bl.y, tr.y, -Abs(get_length_vect3(near) * .t_bias), Abs(get_length_vect3(far) * .t_bias)) 'Select Ortho Mode
                End With
                Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
                Gl.glLoadIdentity() 'Reset The Matrix
                Glu.gluLookAt(c_.x, c_.y, c_.z, l_.x, l_.y, l_.z, c_rot.x, c_rot.y, c_rot.z)

                look_point_X = c_pos.x
                look_point_Z = c_pos.z
            End If

            'Gl.glOrtho(-128, 128, (-128) * scale, (128) * scale, -300.0, 500.0) 'Select Ortho Mode

        End If

    End Sub

    Public Sub draw_scene()
        If Not _STARTED Then Return
        If Not maploaded Then
            Return
        End If
        swat1.Restart()
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If

        Gl.glEnable(Gl.GL_FRAMEBUFFER_SRGB_EXT)
        'gl_busy = True
        If stopGL Then Return

        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)

        Gl.glDepthFunc(Gl.GL_LEQUAL)
        Gl.glFrontFace(Gl.GL_CW)
        Gl.glCullFace(Gl.GL_BACK)
        Gl.glLineWidth(1)
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
        ResizeGL()
        Gl.glEnable(Gl.GL_NORMALIZE)

        '========================================================================
        Gl.glEnable(Gl.GL_CULL_FACE)
        Gl.glEnable(Gl.GL_COLOR_MATERIAL)
        '----------------------------------------------------
        Gl.glDisable(Gl.GL_SMOOTH)
        '----------------------------------------------------
        Dim ambientn As Single = 0.8 'm_Ambient_level.TrackBar.Value / 100
        Dim ambient() As Single = {ambientn, ambientn, ambientn}
        Dim diffusen As Single = 0.9!
        Dim diffuse() As Single = {diffusen, diffusen, diffusen}

        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, diffuse)
        Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, 95)
        Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, ambient)
        '----------------------------------------------------
        setup_view()
        '---------------------------------
        ' this gets the point on the screen for chunkIDs
        '---------------------------------
        Dim model_view(16) As Double
        Dim projection(16) As Double
        Dim viewport(4) As Integer
        Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, model_view)
        Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projection)
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Dim cp(2) As Single
        Dim sx, sy, sz As Single
        If m_show_chuckIds.Checked Then
            For i = 0 To maplist.Length - 1
                ReDim maplist(i).scr_coords(3)
                cp(0) = maplist(i).location.x
                cp(1) = maplist(i).location.y
                cp(2) = maplist(i).heights_avg + 10
                Glu.gluProject(cp(0), cp(2), cp(1), model_view, projection, viewport, sx, sy, sz)
                maplist(i).scr_coords(0) = sx
                maplist(i).scr_coords(1) = sy
                maplist(i).scr_coords(2) = sz
            Next
        End If
        '---------------------------------
        'Draw SkyDome
        draw_dome()
        '---------------------------------
        'draw the lights location
        draw_light_sphear()
        '---------------------------------
        'fog!
        setup_fog()
        '---------------------------------
        Dim global_ambient() As Single = {0.99F, 0.99F, 0.99F, 1.0F}
        Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, global_ambient)
        '---------------------------------------------------------------------------------
        'draw the map sections and seams SOLID
        swat2.Restart()
        draw_terrain()
        If frmStats.Visible = True Then
            frmStats.rt_terrian.Text = swat2.ElapsedMilliseconds.ToString
        End If

        '---------------------------------------------------------------------------------
        swat2.Restart()
        draw_models()
        If frmStats.Visible = True Then
            frmStats.rt_models.Text = swat2.ElapsedMilliseconds.ToString
        End If
        '---------------------------------------------------------------------------------
        swat2.Restart()
        draw_trees()
        If frmStats.Visible = True Then
            frmStats.rt_trees.Text = swat2.ElapsedMilliseconds.ToString
        End If
        '---------------------------------------------------------------------------------
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_ALPHA_TEST)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glDepthMask(Gl.GL_FALSE)
        'draw decals
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        '-------- decals
        swat2.Restart()
        If maploaded And m_wire_decals.Checked And m_show_decals.Checked And Not m_hell_mode.Checked Then
            If Not view_mode Then
                ViewPerspective_d()
            End If

            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glUseProgram(shader_list.comp_shader)
            Gl.glUniform3f(phong_cam_pos, eyeX, eyeY, eyeZ)
            Gl.glUniform1f(bump_out_, -0.005)
            Gl.glDisable(Gl.GL_BLEND)
            Gl.glColor3f(0.2, 0.05, 0.0)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            For k = 0 To decal_matrix_list.Length - 1
                If decal_matrix_list(k).good Then
                    Gl.glCallList(decal_matrix_list(k).display_id)
                End If
            Next

            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
            Gl.glColor3f(0.0, 0.0, 0.0)
            Gl.glUniform1f(bump_out_, -0.015)

            For k = 0 To decal_matrix_list.Length - 1
                If decal_matrix_list(k).good Then
                    Gl.glCallList(decal_matrix_list(k).display_id)
                End If
            Next
            Gl.glUseProgram(0)
            Gl.glDisable(Gl.GL_STENCIL_TEST)
            If normal_mode > 0 Then
                Gl.glColor3f(0.5, 0.5, 0.5)
                Gl.glUseProgram(shader_list.normal_shader)
                Gl.glUniform1i(view_normal_mode_, normal_mode)
                Gl.glUniform1f(normal_length_, 0.1)
                For k = 0 To decal_matrix_list.Length - 1
                    If decal_matrix_list(k).good Then
                        Gl.glCallList(decal_matrix_list(k).display_id)
                    End If
                Next
                Gl.glUseProgram(0)
            End If
        End If
        If maploaded And Not m_wire_decals.Checked And m_show_decals.Checked And Not m_hell_mode.Checked Then
            If Not view_mode Then
                ViewPerspective_d()
            End If
            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glColor3f(0.5, 0.5, 0.5)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            Gl.glUseProgram(shader_list.decals_shader)
            If m_show_fog.Checked Then
                Gl.glUniform1i(f_address5, 1)
            Else
                Gl.glUniform1i(f_address5, 0)
            End If
            Gl.glUniform1i(c_address5, 0)
            Gl.glUniform1i(n_address5, 1)
            Gl.glUniform3f(c_position5, eyeX, eyeY, eyeZ)
            Gl.glUniform1f(t_address5, lighting_model_level)
            Gl.glUniform1f(gray_level_5, gray_level)
            Gl.glUniform1f(gamma_5, gamma_level)
            Gl.glUniform1f(decal_ambient, lighting_ambient)

            Dim E = Gl.glGetError
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glBlendEquationSeparate(Gl.GL_FUNC_ADD, Gl.GL_FUNC_ADD)
            Gl.glBlendFuncSeparate(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA, Gl.GL_ONE, Gl.GL_ONE)
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
            E = Gl.glGetError
            ''''''''''''''''''''''''''''''''''''''''''''''''''
            For k = 0 To decal_matrix_list.Length - 1
                If decal_matrix_list(k).good Then
                    Gl.glUniform1f(decal_u_wrap, decal_matrix_list(k).u_wrap)
                    Gl.glUniform1f(decal_v_wrap, decal_matrix_list(k).v_wrap)
                    Gl.glUniform1f(decal_influence, decal_matrix_list(k).influence)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, decal_matrix_list(k).texture_id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, decal_matrix_list(k).normal_id)

                    Gl.glCallList(decal_matrix_list(k).display_id)
                End If
            Next
            Gl.glUseProgram(0)
            Gl.glEnable(Gl.GL_DEPTH_TEST)
            Gl.glDisable(Gl.GL_BLEND)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        End If
        If Not view_mode Then
            ViewPerspective()

        End If
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDepthMask(Gl.GL_TRUE)
        '---------------------------------------------------------------------------------
        'decal editing junk
        If maploaded And EDIT_INCULDERS Then
            If Not view_mode Then
                ViewPerspective_d()
            End If
            Gl.glDisable(Gl.GL_LIGHTING)

            For k = 0 To decal_matrix_list.Length - 1
                Dim m = decal_matrix_list(k).matrix
                If decal_matrix_list(k).good Then
                    With decal_matrix_list(k)
                        If k = d_counter Then
                            If frmBiasing.SHOW_SELECT_MESH Then

                                Gl.glLineWidth(1.0)
                                Gl.glUseProgram(shader_list.comp_shader)
                                Gl.glColor4f(1.0, 0.0, 0.0, 1.0)
                                Gl.glLineWidth(1.0)
                                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
                                Gl.glUniform1f(bump_out_, -0.01)
                                Gl.glCallList(decal_matrix_list(d_counter).display_id)
                                Gl.glUseProgram(0)
                            End If
                            Gl.glColor4f(1.0, 0.0, 0.0, 1.0)

                            Gl.glPushMatrix()
                            Gl.glMultMatrixf(decal_matrix_list(k).matrix)
                            glutWireCube(1.0)
                            Gl.glPopMatrix()

                            Gl.glBegin(Gl.GL_LINES)
                            Gl.glVertex3f(.near_clip.x + m(12), .near_clip.y + m(13), .near_clip.z + m(14))
                            Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
                            Gl.glVertex3f(.far_clip.x + m(12), .far_clip.y + m(13), .far_clip.z + m(14))
                            Gl.glEnd()
                            Gl.glLineWidth(2.0)
                        Else
                            Gl.glLineWidth(2.0)
                            Gl.glColor4f(0.0, 1.0, 0.0, 1.0)

                            Gl.glPushMatrix()
                            Gl.glMultMatrixf(decal_matrix_list(k).matrix)
                            glutWireCube(1.0)
                            Gl.glPopMatrix()
                            Gl.glBegin(Gl.GL_LINE_STRIP)
                            Gl.glVertex3f(.top_left.x, .top_left.y, .top_left.z)
                            Gl.glVertex3f(.top_right.x, .top_right.y, .top_right.z)
                            Gl.glVertex3f(.bot_right.x, .bot_right.y, .bot_right.z)
                            Gl.glVertex3f(.bot_left.x, .bot_left.y, .bot_left.z)
                            Gl.glVertex3f(.top_left.x, .top_left.y, .top_left.z)
                            Gl.glEnd()

                            Gl.glBegin(Gl.GL_LINES)
                            Gl.glVertex3f(.near_clip.x + m(12), .near_clip.y + m(13), .near_clip.z + m(14))
                            Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
                            Gl.glVertex3f(.far_clip.x + m(12), .far_clip.y + m(13), .far_clip.z + m(14))
                            Gl.glEnd()
                        End If
                    End With
                End If
            Next
            Gl.glEnable(Gl.GL_LIGHTING)
            Gl.glLineWidth(1.0)
        End If
        If frmStats.Visible = True Then
            frmStats.rt_decals.Text = swat2.ElapsedMilliseconds.ToString
        End If

        Gl.glEnable(Gl.GL_DEPTH_TEST)
        ViewPerspective()
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)

        '---------------------------------
        If m_show_cursor.Checked And Not m_hell_mode.Checked Then
            Gl.glDisable(Gl.GL_LIGHTING)
            Gl.glColor3f(1.0, 1.0, 0.0)
            If Not NetData Then
                draw_cursor(u_look_point_X, u_look_point_Z)
            Else
                draw_cursor(Packet_in.Ex, Packet_in.Ez)
            End If
        End If
        Gl.glEnable(Gl.GL_LIGHTING)
        '------------------------------
        ' Display tanks.. if there are some
        Dim tdl As Integer = 0
        If maploaded And Not m_hell_mode.Checked Then
            If tankID > -1 And m_show_tank_comments.Checked Then
                m_comment.Visible = True
                m_clear_tank_comments.Visible = True
                m_comment.AllowDrop = True
            Else
                m_comment.Visible = False
                m_clear_tank_comments.Visible = False
            End If
            Dim cv = 57.2957795
            For i = 0 To 14
                Gl.glUseProgram(shader_list.tank_shader)
                If locations.team_1(i).track_displaylist > -1 Then
                    Gl.glPushMatrix()
                    ReDim locations.team_1(i).scrn_coords(3)
                    Dim y_ = get_Z_at_XY(locations.team_1(i).loc_x, locations.team_1(i).loc_z)
                    Gl.glTranslatef(locations.team_1(i).loc_x, y_, locations.team_1(i).loc_z)
                    cp(0) = locations.team_1(i).loc_x
                    cp(1) = y_ + 3
                    cp(2) = locations.team_1(i).loc_z

                    Glu.gluProject(cp(0), cp(1), cp(2), model_view, projection, viewport, sx, sy, sz)

                    locations.team_1(i).scrn_coords(0) = sx
                    locations.team_1(i).scrn_coords(1) = sy
                    locations.team_1(i).scrn_coords(2) = sz
                    Dim rot = locations.team_1(i).rot_y
                    Dim rx = (cv * surface_normal.y * Cos(rot)) + (cv * surface_normal.x * Sin(rot))
                    Dim rz = (cv * surface_normal.x * Cos(rot)) + (cv * -surface_normal.y * Sin(rot))
                    Gl.glRotatef(cv * rot, 0.0!, 1.0!, 0.0!)
                    Gl.glRotatef(rz, 0.0!, 0.0!, 1.0!)
                    Gl.glRotatef(rx, -1.0!, 0.0!, 0.0!)
                    tdl = locations.team_1(i).track_displaylist
                    Gl.glColor3f(0.4!, 0.0!, 0.0!)
                    If tankID = i Then
                        Gl.glColor3f(0.4!, 0.0!, 0.0!)
                        Gl.glClearStencil(0)
                        Gl.glClear(Gl.GL_STENCIL_BUFFER_BIT)

                        ' Render the mesh into the stencil buffer.

                        Gl.glEnable(Gl.GL_STENCIL_TEST)

                        Gl.glStencilFunc(Gl.GL_ALWAYS, 1, -1)
                        Gl.glStencilOp(Gl.GL_KEEP, Gl.GL_KEEP, Gl.GL_REPLACE)

                        Gl.glCallList(tdl)

                        ' Render the thick wireframe version.

                        Gl.glStencilFunc(Gl.GL_NOTEQUAL, 1, -1)
                        Gl.glStencilOp(Gl.GL_KEEP, Gl.GL_KEEP, Gl.GL_REPLACE)

                        Gl.glLineWidth(10)
                        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_LINE)
                        Gl.glColor3f(1.0!, 1.0!, 0.0!)

                        Gl.glCallList(tdl)
                        Gl.glDisable(Gl.GL_STENCIL_TEST)

                    Else
                        Gl.glCallList(tdl)
                    End If
                    Gl.glPopMatrix()
                End If
                Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
            Next
            For i = 0 To 14
                If locations.team_2(i).track_displaylist > -1 Then
                    Gl.glPushMatrix()
                    Gl.glColor3f(0.0, 0.3, 0.0)
                    ReDim locations.team_2(i).scrn_coords(3)
                    Dim y_ = get_Z_at_XY(locations.team_2(i).loc_x, locations.team_2(i).loc_z)
                    Gl.glTranslatef(locations.team_2(i).loc_x, y_, locations.team_2(i).loc_z)
                    cp(0) = locations.team_2(i).loc_x
                    cp(1) = y_ + 3
                    cp(2) = locations.team_2(i).loc_z
                    Glu.gluProject(cp(0), cp(1), cp(2), model_view, projection, viewport, sx, sy, sz)

                    locations.team_2(i).scrn_coords(0) = sx
                    locations.team_2(i).scrn_coords(1) = sy
                    locations.team_2(i).scrn_coords(2) = sz

                    Dim rot = locations.team_2(i).rot_y
                    Dim rx = (cv * surface_normal.y * Cos(rot)) + (cv * surface_normal.x * Sin(rot))
                    Dim rz = (cv * surface_normal.x * Cos(rot)) + (cv * -surface_normal.y * Sin(rot))
                    Gl.glRotatef(cv * rot, 0.0, 1.0, 0.0)
                    Gl.glRotatef(rz, 0.0, 0.0, 1.0)
                    Gl.glRotatef(rx, -1.0, 0.0, 0.0)
                    tdl = locations.team_2(i).track_displaylist
                    If tankID - 100 = i Then
                        Gl.glClearStencil(0)
                        Gl.glClear(Gl.GL_STENCIL_BUFFER_BIT)

                        ' Render the mesh into the stencil buffer.

                        Gl.glEnable(Gl.GL_STENCIL_TEST)

                        Gl.glStencilFunc(Gl.GL_ALWAYS, 1, -1)
                        Gl.glStencilOp(Gl.GL_KEEP, Gl.GL_KEEP, Gl.GL_REPLACE)

                        Gl.glCallList(tdl)

                        ' Render the thick wireframe version.

                        Gl.glStencilFunc(Gl.GL_NOTEQUAL, 1, -1)
                        Gl.glStencilOp(Gl.GL_KEEP, Gl.GL_KEEP, Gl.GL_REPLACE)

                        Gl.glLineWidth(6)
                        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_LINE)
                        Gl.glColor3f(1.0!, 1.0!, 0.0!)

                        Gl.glCallList(tdl)
                        Gl.glDisable(Gl.GL_STENCIL_TEST)

                    Else
                        Gl.glCallList(tdl)
                    End If
                    Gl.glPopMatrix()
                End If
                Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
            Next
        End If
        Gl.glUseProgram(0)
        '---------------------------------
        ' base locations
        draw_base_rings()

        '---------------------------------
        '------------------------------------
        Gl.glActiveTexture(Gl.GL_TEXTURE0)

        'draw the water. IF there is water?
        If water.IsWater And maploaded And m_show_water.Checked And Not m_hell_mode.Checked Then
            Gl.glEnable(Gl.GL_LIGHTING)
            Gl.glEnable(Gl.GL_DEPTH_TEST)

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glEnable(Gl.GL_ALPHA_TEST)
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
            Gl.glPushMatrix()
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glColor4f(0.1, 0.1, 0.2, 0.5)
            Gl.glTranslatef(-water.position.x, -0.1, water.position.z)
            Gl.glRotatef(-water.orientation * 57.2957795, 0.0, 1.0, 0.0)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, water.textureID)
            Gl.glCallList(water.displayID)
            Gl.glPopMatrix()

            Gl.glDisable(Gl.GL_BLEND)
            Gl.glDisable(Gl.GL_FOG)
        End If

        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_STENCIL_TEST)
        '---------------------------------------------------------------------------------
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        '---------------------------------------------------------------------------------
        '---------------------------------------------------------------------------------
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        '---------------------------------
        'draw_XZ_grid()
        '---------------------------------
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glColor3f(1.0, 1.0, 1.0)

        If move_mod Or z_move Then  'draw reference lines to eye center
            Gl.glLineWidth(1.0)
            Gl.glBegin(Gl.GL_LINES)
            If Not NetData Then
                Gl.glVertex3f(u_look_point_X, u_look_point_Y + 1000, u_look_point_Z)
                Gl.glVertex3f(u_look_point_X, u_look_point_Y - 1000, u_look_point_Z)

                Gl.glVertex3f(u_look_point_X + 1000, u_look_point_Y, u_look_point_Z)
                Gl.glVertex3f(u_look_point_X - 1000, u_look_point_Y, u_look_point_Z)

                Gl.glVertex3f(u_look_point_X, u_look_point_Y, u_look_point_Z + 1000)
                Gl.glVertex3f(u_look_point_X, u_look_point_Y, u_look_point_Z - 1000)
            Else

                Gl.glVertex3f(Packet_in.Ex, Packet_in.Ey + 1000, Packet_in.Ez)
                Gl.glVertex3f(Packet_in.Ex, Packet_in.Ey - 1000, Packet_in.Ez)

                Gl.glVertex3f(Packet_in.Ex + 1000, Packet_in.Ey, Packet_in.Ez)
                Gl.glVertex3f(Packet_in.Ex - 1000, Packet_in.Ey, Packet_in.Ez)

                Gl.glVertex3f(Packet_in.Ex, Packet_in.Ey, Packet_in.Ez + 1000)
                Gl.glVertex3f(Packet_in.Ex, Packet_in.Ey, Packet_in.Ez - 1000)
            End If
            Gl.glEnd()
        End If
        Gl.glDisable(Gl.GL_FRAMEBUFFER_SRGB_EXT)
        '---------------------------------
        'copy_to_post_map()
        '---------------------------------

        'all below is 2D ortho stuff
        '---------------------------------
        ViewOrtho()
        '==========================
        'draw_post_test()
        '==========================
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glTranslatef(0.0, 0.0F, -0.01F)
        If maploaded And EDIT_INCULDERS Then
            For k = 0 To decal_matrix_list.Length - 1
                With decal_matrix_list(k)
                    If k = d_counter Then
                        If decal_matrix_list(k).good Then
                            draw_little_window(k)
                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
                        End If
                    End If
                End With
            Next
        End If

        If Not m_hell_mode.Checked And m_show_status.Checked Then
            If maploaded And m_show_chuckIds.Checked Then
                For i = 0 To maplist.Length - 2
                    If maplist(i).scr_coords(2) < 1.0 Then
                        glutPrint(maplist(i).scr_coords(0), maplist(i).scr_coords(1) - pb1.Height + 15, maplist(i).name, 1.0, 1.0, 1.0, 1.0)
                        glutPrint(maplist(i).scr_coords(0) + 18, maplist(i).scr_coords(1) - pb1.Height - 0, i, 1.0, 1.0, 1.0, 1.0)
                    End If
                Next
            End If
            If maploaded And m_show_tank_names.Checked Then
                For i = 0 To 14
                    If locations.team_1(i).track_displaylist > -1 Then
                        If locations.team_1(i).scrn_coords(2) < 1.0 Then
                            glutPrintBox(locations.team_1(i).scrn_coords(0) - ((locations.team_1(i).name.Length / 2) * 8), locations.team_1(i).scrn_coords(1) - pb1.Height, locations.team_1(i).name, 1.0, 0.0, 0.0, 1.0)
                        End If
                    End If
                    If locations.team_2(i).track_displaylist > -1 Then
                        If locations.team_2(i).scrn_coords(2) < 1.0 Then
                            glutPrintBox(locations.team_2(i).scrn_coords(0) - ((locations.team_2(i).name.Length / 2) * 8), locations.team_2(i).scrn_coords(1) - pb1.Height, locations.team_2(i).name, 0.0, 1.0, 0.0, 1.0)
                        End If

                    End If
                Next
            End If
            If maploaded And m_show_tank_comments.Checked Then
                For i = 0 To 14
                    If locations.team_1(i).track_displaylist > -1 Then
                        If locations.team_1(i).scrn_coords(2) < 1.0 Then
                            glutPrintBox(locations.team_1(i).scrn_coords(0) - ((locations.team_1(i).comment.Length / 2) * 8), locations.team_1(i).scrn_coords(1) - pb1.Height - 15, locations.team_1(i).comment, 1.0, 1.0, 1.0, 1.0)
                        End If
                    End If
                    If locations.team_2(i).track_displaylist > -1 Then
                        If locations.team_2(i).scrn_coords(2) < 1.0 Then
                            glutPrintBox(locations.team_2(i).scrn_coords(0) - ((locations.team_2(i).comment.Length / 2) * 8), locations.team_2(i).scrn_coords(1) - pb1.Height - 15, locations.team_2(i).comment, 1.0, 1.0, 1.0, 1.0)
                        End If

                    End If
                Next
            End If
            '====================================
            draw_heading()
            Dim w As Single = 0
            If m_show_minimap.Checked And mini_map_loaded Then
                w = (minimap_size * 1.066666)    ' used to make room for the minimap
            End If
            Gl.glDisable(Gl.GL_DEPTH_TEST)
            If m_show_minimap.Checked And mini_map_loaded Then
                draw_minimap()
            End If
            Gl.glDisable(Gl.GL_LIGHTING)
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
            Gl.glColor4f(0.3, 0.0, 0.0, 0.6)
            Gl.glBegin(Gl.GL_QUADS)
            Gl.glVertex3f(0.0, -pb1.Height, 0.0)
            Gl.glVertex3f(0.0, -pb1.Height + 20, 0.0)
            Gl.glVertex3f(pb1.Width - w, -pb1.Height + 20, 0.0)
            Gl.glVertex3f(pb1.Width - w, -pb1.Height, 0.0)
            Gl.glEnd()
            Gl.glDisable(Gl.GL_BLEND)
            Gl.glEnable(Gl.GL_LIGHTING)
            If frmStats.Visible = True Then
                frmStats.rt_total.Text = swat1.ElapsedMilliseconds.ToString
            End If

            If maploaded Then
                Gl.glDisable(Gl.GL_DEPTH_TEST)
                Dim fps As Integer = 1.0 / (screen_totaled_draw_time * 0.001)
                Dim str = " FPS:" + fps.ToString
                str += "  Location: [ "
                str += coordStr + " ]"
                ' Dim yp = get_Z_at_XY(u_look_point_X, u_look_point_Z)
                str += "   X:" + u_look_point_X.ToString("0.000000") + "  Y:" + u_look_point_Y.ToString("0.000000") + "  Z:" + u_look_point_Z.ToString("0.000000")
                str += "   GX:" + d_hx.ToString + "  GY:" + d_hy.ToString
                'swat.Stop()
                glutPrint(10, 8 - pb1.Height, str.ToString, 0.0, 1.0, 0.0, 1.0)
            End If
        End If
        Gdi.SwapBuffers(pb1_hDC)
        If frmTestView.Visible Then
            frmTestView.update_screen()
        End If
    End Sub

    Public Sub draw_minimap()
        Dim uc As vect2
        Dim lc As vect2
        'draw the minimap_frame. (this is a custom resource I created.
        lc.x = pb1.Width
        lc.y = -pb1.Height
        uc.x = pb1.Width - (minimap_size * 1.066666)
        uc.y = -pb1.Height + (minimap_size * 1.066666) ' top to bottom is negitive
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glColor3f(1.0!, 1.0!, 1.0!)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, minimsp_frameT_TextureId)
        Gl.glBegin(Gl.GL_TRIANGLES)
        '---
        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(uc.x, lc.y, -1.0!)

        Gl.glTexCoord2f(0.0, -1.0)
        Gl.glVertex3f(uc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(-1.0, 0.0)
        Gl.glVertex3f(lc.x, lc.y, -1.0!)
        '---
        Gl.glTexCoord2f(0.0, -1.0)
        Gl.glVertex3f(uc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(-1.0, -1.0)
        Gl.glVertex3f(lc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(-1.0, 0.0)
        Gl.glVertex3f(lc.x, lc.y, -1.0!)

        Gl.glEnd()
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        '-------------------------------------
        'draw loaded minimap
        lc.x = pb1.Width
        lc.y = -pb1.Height
        uc.x = pb1.Width - minimap_size
        uc.y = -pb1.Height + minimap_size ' top to bottom is negitive
        Gl.glColor3f(1.0!, 1.0!, 1.0!)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, minimap_textureid)
        'Gl.glBindTexture(Gl.GL_TEXTURE_2D, noise_map_id)
        Gl.glBegin(Gl.GL_TRIANGLES)
        '---
        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(uc.x, lc.y, -1.0!)

        Gl.glTexCoord2f(0.0, -1.0)
        Gl.glVertex3f(uc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(-1.0, 0.0)
        Gl.glVertex3f(lc.x, lc.y, -1.0!)
        '---
        Gl.glTexCoord2f(0.0, -1.0)
        Gl.glVertex3f(uc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(-1.0, -1.0)
        Gl.glVertex3f(lc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(-1.0, 0.0)
        Gl.glVertex3f(lc.x, lc.y, -1.0!)

        Gl.glEnd()
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        '-------------------------------------
        'Dim mod_ = MAP_SIDE_LENGTH Mod 2
        Dim iw = 18.0! : Dim ih = 11.33!
        Dim ic_x As Single = 13.5!
        Dim ic_y As Single = 8.5!
        Dim w As Double = MAP_BB_UR.x + MAP_BB_BL.x
        Dim v As Double = MAP_BB_UR.y + MAP_BB_BL.y
        If w = 0 Then Return
        Dim scale As Single = (minimap_size / 300)
        Dim xc = -(w / 2)
        Dim yc = -(v / 2)
        Dim xy As Single = (minimap_size) / 10
        Dim x1 = 0 : Dim x2 = minimap_size
        ' Dim mxc, mzc As Single
        '------------
        'draw the base rings
        Dim vs As Single = (MAP_BB_UR.x - MAP_BB_BL.x) / 10
        Dim rx, rz, r As Single
        Dim sz = (100) / vs * xy
        'Try
        If SHOW_RINGS Then

            rx = (((-team_1.x + 50 + xc) / vs) * (-xy)) + (minimap_size / 2) '- (25.0! * scale)
            rz = (((team_1.z + 50 + yc) / vs) * (-xy)) + (minimap_size / 2)  '- (25.0! * scale)
            Gl.glColor3f(0.8!, 0.0!, 0.0!)

            Gl.glLineWidth(2.0! * scale)

            Gl.glBegin(Gl.GL_LINE_LOOP)
            r = sz / 2
            For i = 0 To 2 * PI Step 0.2
                Gl.glVertex3f((r * Cos(i)) + uc.x + rx + r, (r * Sin(i)) + uc.y - rz - r, -0.1!)
            Next
            Gl.glEnd()


            rx = (((-team_2.x + 50 + xc) / vs) * (-xy)) + (minimap_size / 2) '- (25.0! * scale)
            rz = (((team_2.z + 50 + yc) / vs) * (-xy)) + (minimap_size / 2) '- (25.0! * scale)


            Gl.glColor3f(0.0!, 0.8!, 0.0!)
            Gl.glBegin(Gl.GL_LINE_LOOP)

            r = sz / 2
            For i = 0 To 2 * PI Step 0.2
                Gl.glVertex3f((r * Cos(i)) + uc.x + rx + r, (r * Sin(i)) + uc.y - rz - r, -0.1!)
            Next
            Gl.glEnd()
        End If
        '-------------------------------------------------
        'draw the grid
        Gl.glColor3f(0.5!, 0.5!, 0.5!)
        Gl.glLineWidth(1.0!)
        If xy = 0 Then Return ' no data lodaed and the 'step xy' causes an infinite loop
        Gl.glBegin(Gl.GL_LINES)
        For y As Single = xy - xy To (xy * 11.0!) Step xy
            Gl.glVertex3f(x1 + uc.x, -y + uc.y, -0.2!)
            Gl.glVertex3f(x2 + uc.x, -y + uc.y, -0.2!)
        Next
        'Gl.glEnd()
        'Gl.glBegin(Gl.GL_LINES)
        For y As Single = xy - xy To (xy * 11.0!) Step xy
            Gl.glVertex3f(y + uc.x, -x1 + uc.y, -0.2!)
            Gl.glVertex3f(y + uc.x, -x2 + uc.y, -0.2!)
        Next
        Gl.glEnd()


        Dim mex As Single = (((look_point_X + xc) / vs) * (-xy)) + (minimap_size / 2)
        Dim mey As Single = (((look_point_Z + yc) / vs) * (-xy)) + (minimap_size / 2)

        'draw the tanks locations and types
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glEnable(Gl.GL_TEXTURE_2D)

        For t1 = 0 To 14
            Gl.glColor3f(0.9!, 0.0!, 0.0!)
            If locations.team_1(t1).track_displaylist > -1 Then
                Dim lx = (((locations.team_1(t1).loc_x + xc) / vs) * (-xy)) + (minimap_size / 2.0) ' + (ic_x * scale)
                Dim ly = (((locations.team_1(t1).loc_z + yc) / vs) * (-xy)) + (minimap_size / 2.0) ' - (ic_y * scale)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, tank_mini_icons(locations.team_1(t1).type))
                Gl.glBegin(Gl.GL_QUADS)

                Gl.glTexCoord2f(0, 1)
                Gl.glVertex3f(-icon_scale + lx + uc.x, (-icon_scale * 0.63) + uc.y - ly, -0.11)

                Gl.glTexCoord2f(0, 0)
                Gl.glVertex3f(-icon_scale + lx + uc.x, (icon_scale * 0.63) + uc.y - ly, -0.11)

                Gl.glTexCoord2f(-1, 0)
                Gl.glVertex3f(icon_scale + lx + uc.x, (icon_scale * 0.63) + uc.y - ly, -0.11)

                Gl.glTexCoord2f(-1, 1)
                Gl.glVertex3f(icon_scale + lx + uc.x, (-icon_scale * 0.63) + uc.y - ly, -0.11)
                Gl.glEnd()
            End If
            Gl.glColor3f(0.0!, 0.9!, 0.0!)
            If locations.team_2(t1).track_displaylist > -1 Then
                Dim lx = (((locations.team_2(t1).loc_x + xc) / vs) * (-xy)) + (minimap_size / 2.0) ' - (ic_x * scale)
                Dim ly = (((locations.team_2(t1).loc_z + yc) / vs) * (-xy)) + (minimap_size / 2.0) ' - (ic_y * scale)


                Gl.glBindTexture(Gl.GL_TEXTURE_2D, tank_mini_icons(locations.team_2(t1).type))
                Gl.glBegin(Gl.GL_QUADS)

                Gl.glTexCoord2f(0, 1)
                Gl.glVertex3f(-icon_scale + lx + uc.x, (-icon_scale * 0.63) + uc.y - ly, -0.11)

                Gl.glTexCoord2f(0, 0)
                Gl.glVertex3f(-icon_scale + lx + uc.x, (icon_scale * 0.63) + uc.y - ly, -0.11)

                Gl.glTexCoord2f(-1, 0)
                Gl.glVertex3f(icon_scale + lx + uc.x, (icon_scale * 0.63) + uc.y - ly, -0.11)

                Gl.glTexCoord2f(-1, 1)
                Gl.glVertex3f(icon_scale + lx + uc.x, (-icon_scale * 0.63) + uc.y - ly, -0.11)
                Gl.glEnd()
            End If
        Next
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Dim os = 2 * scale


        Gl.glColor3f(0.9!, 0.9!, 0.0!)
        Gl.glPointSize(6.0)
        Gl.glColor3f(0.9!, 0.9!, 0.0!)
        Gl.glBegin(Gl.GL_POINTS)
        Gl.glVertex3f(mex + uc.x, -mey + uc.y, -0.1)
        Gl.glEnd()
        Gl.glLineWidth(2.0)
        Gl.glBegin(Gl.GL_LINES)
        Gl.glVertex3f(mex + uc.x, -mey + uc.y, -0.1)

        Dim hpie As Single = PI / 2
        Dim d_x As Single = Cos((PI * 1.5) - (Cam_X_angle + angle_offset)) * (15 * scale)
        Dim d_y As Single = Sin((PI * 1.5) - (Cam_X_angle + angle_offset)) * (15 * scale)
        Gl.glVertex3f(mex + d_x + uc.x, -mey - d_y + uc.y, -0.1)
        Gl.glEnd()
        If m_Orbit_Light.Checked Then
            mex = (((position(0) + xc) / vs) * (-xy)) + (minimap_size / 2)
            mey = (((position(2) + yc) / vs) * (-xy)) + (minimap_size / 2)
            Gl.glBegin(Gl.GL_POINTS)
            Gl.glColor3f(1.0, 0.0, 0.0)
            Gl.glVertex3f(mex + uc.x, -mey + uc.y, -0.1)
            Gl.glEnd()
        End If
        'Gl.glFinish()
        Gl.glEnable(Gl.GL_LIGHTING)
        Try
            Dim xr = Floor((mex / minimap_size) * 10)
            Dim yr = Floor((mey / minimap_size) * 10)
            coordStr = alpha(CInt(yr)) + numer(CInt(xr))
        Catch ex As Exception
        End Try
        'Catch ex As Exception

        'End Try

    End Sub
    Public Function Render_minimap(ByVal Psize As Integer) As Bitmap
        Dim uc As vect2
        Dim lc As vect2
        'draw the minimap_frame. (this is a custom resource I created.
        Dim old_wx, old_wy, old_msize As Integer
        old_wx = pb1.Width
        old_wy = pb1.Height
        old_msize = minimap_size
        minimap_size = Psize
        Application.DoEvents()
        Dim dck = pb1.Dock
        pb1.Width = Psize
        pb1.Height = Psize
        lc.x = pb1.Width
        lc.y = -pb1.Height
        uc.x = pb1.Width - (minimap_size * 1.066666)
        uc.y = -pb1.Height + (minimap_size * 1.066666) ' top to bottom is negitive

        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        ResizeGL()
        ViewPerspective()
        ViewOrtho()
        Gl.glPushMatrix()
        Gl.glTranslatef(0.0, 0.0F, -0.1F)


        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glColor3f(1.0!, 1.0!, 1.0!)
        '-------------------------------------
        'draw loaded minimap
        lc.x = pb1.Width
        lc.y = -pb1.Height
        uc.x = pb1.Width - minimap_size
        uc.y = -pb1.Height + minimap_size ' top to bottom is negitive
        Gl.glColor3f(1.0!, 1.0!, 1.0!)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, minimap_textureid)
        Gl.glBegin(Gl.GL_TRIANGLES)
        '---
        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(uc.x, lc.y, -1.0!)

        Gl.glTexCoord2f(0.0, -1.0)
        Gl.glVertex3f(uc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(-1.0, 0.0)
        Gl.glVertex3f(lc.x, lc.y, -1.0!)
        '---
        Gl.glTexCoord2f(0.0, -1.0)
        Gl.glVertex3f(uc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(-1.0, -1.0)
        Gl.glVertex3f(lc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(-1.0, 0.0)
        Gl.glVertex3f(lc.x, lc.y, -1.0!)

        Gl.glEnd()
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        '-------------------------------------
        Dim NUMS = "1234567890".ToArray
        Dim letters = "ABCDEFGHIJK".ToArray
        Dim iw = 18.0! : Dim ih = 11.33!
        Dim ic_x As Single = 13.5!
        Dim ic_y As Single = 8.5!
        Dim w As Double = MAP_BB_UR.x + MAP_BB_BL.x
        Dim v As Double = MAP_BB_UR.y + MAP_BB_BL.y
        If w = 0 Then Return Nothing
        Dim scale As Single = 1.0 '(minimap_size / 300)
        Dim xc = -(w / 2)
        Dim yc = -(v / 2)
        Dim xy As Single = (minimap_size) / 10
        Dim x1 = 0 : Dim x2 = minimap_size
        ' Dim mxc, mzc As Single
        '------------
        'draw the base rings
        Dim vs As Single = (MAP_BB_UR.x - MAP_BB_BL.x) / 10
        Dim rx, rz, r As Single
        Dim sz = (100) / vs * xy
        rx = (((-team_1.x + 50 + xc) / vs) * (-xy)) + (minimap_size / 2) '- (25.0! * scale)
        rz = (((team_1.z + 50 + yc) / vs) * (-xy)) + (minimap_size / 2) '- (25.0! * scale)
        Gl.glColor3f(0.8!, 0.0!, 0.0!)

        Gl.glLineWidth(2.0! * scale)

        Gl.glBegin(Gl.GL_LINE_LOOP)
        r = sz / 2
        For i = 0 To 2 * PI Step 0.2
            Gl.glVertex3f((r * Cos(i)) + uc.x + rx + r, (r * Sin(i)) + uc.y - rz - r, -0.1!)
        Next
        Gl.glEnd()


        rx = (((-team_2.x + 50 + xc) / vs) * (-xy)) + (minimap_size / 2) '- (25.0! * scale)
        rz = (((team_2.z + 50 + yc) / vs) * (-xy)) + (minimap_size / 2) '- (25.0! * scale)


        Gl.glColor3f(0.0!, 0.8!, 0.0!)
        Gl.glBegin(Gl.GL_LINE_LOOP)

        r = sz / 2
        For i = 0 To 2 * PI Step 0.2
            Gl.glVertex3f((r * Cos(i)) + uc.x + rx + r, (r * Sin(i)) + uc.y - rz - r, -0.1!)
        Next
        Gl.glEnd()
        '-------------------------------------------------
        'draw the grid
        Gl.glColor3f(0.9!, 0.9!, 0.9!)
        Gl.glLineWidth(1.0!)
        If xy = 0 Then Return Nothing ' no data lodaed and the 'step xy' causes an infinite loop
        For y As Single = xy - xy To (xy * 11.0!) Step xy
            Gl.glBegin(Gl.GL_LINES)
            Gl.glVertex3f(x1 + uc.x, -y + uc.y, -0.2!)
            Gl.glVertex3f(x2 + uc.x, -y + uc.y, -0.2!)
            Gl.glEnd()
        Next
        For y As Single = xy - xy To (xy * 11.0!) Step xy
            Gl.glBegin(Gl.GL_LINES)
            Gl.glVertex3f(y + uc.x, -x1 + uc.y, -0.2!)
            Gl.glVertex3f(y + uc.x, -x2 + uc.y, -0.2!)
            Gl.glEnd()
        Next

        Dim mex As Single = (((look_point_X + xc) / vs) * (-xy)) + (minimap_size / 2)
        Dim mey As Single = (((look_point_Z + yc) / vs) * (-xy)) + (minimap_size / 2)

        'draw the tanks locations and types
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glEnable(Gl.GL_TEXTURE_2D)

        For t1 = 0 To 14
            Gl.glColor3f(0.9!, 0.0!, 0.0!)
            If locations.team_1(t1).track_displaylist > -1 Then
                Dim lx = (((locations.team_1(t1).loc_x + xc) / vs) * (-xy)) + (minimap_size / 2.0) ' + (ic_x * scale)
                Dim ly = (((locations.team_1(t1).loc_z + yc) / vs) * (-xy)) + (minimap_size / 2.0) ' - (ic_y * scale)
                'glutPrint(lx - ((locations.team_1(t1).name.Length / 2) * 8), ly - Psize, locations.team_1(t1).name, 1.0, 0.0, 0.0, 1.0)

                Gl.glBindTexture(Gl.GL_TEXTURE_2D, tank_mini_icons(locations.team_1(t1).type))
                Gl.glBegin(Gl.GL_QUADS)

                Gl.glTexCoord2f(0, 1)
                Gl.glVertex3f(-icon_scale + lx + uc.x, (-icon_scale * 0.63) + uc.y - ly, -0.11)

                Gl.glTexCoord2f(0, 0)
                Gl.glVertex3f(-icon_scale + lx + uc.x, (icon_scale * 0.63) + uc.y - ly, -0.11)

                Gl.glTexCoord2f(-1, 0)
                Gl.glVertex3f(icon_scale + lx + uc.x, (icon_scale * 0.63) + uc.y - ly, -0.11)

                Gl.glTexCoord2f(-1, 1)
                Gl.glVertex3f(icon_scale + lx + uc.x, (-icon_scale * 0.63) + uc.y - ly, -0.11)
                Gl.glEnd()
            End If
            Gl.glColor3f(0.0!, 0.9!, 0.0!)
            If locations.team_2(t1).track_displaylist > -1 Then
                Dim lx = (((locations.team_2(t1).loc_x + xc) / vs) * (-xy)) + (minimap_size / 2.0) ' - (ic_x * scale)
                Dim ly = (((locations.team_2(t1).loc_z + yc) / vs) * (-xy)) + (minimap_size / 2.0) ' - (ic_y * scale)
                'glutPrint(lx - ((locations.team_2(t1).name.Length / 2) * 8), ly - Psize, locations.team_2(t1).name, 0.0, 1.0, 0.0, 1.0)


                Gl.glBindTexture(Gl.GL_TEXTURE_2D, tank_mini_icons(locations.team_2(t1).type))
                Gl.glBegin(Gl.GL_QUADS)

                Gl.glTexCoord2f(0, 1)
                Gl.glVertex3f(-icon_scale + lx + uc.x, (-icon_scale * 0.63) + uc.y - ly, -0.11)

                Gl.glTexCoord2f(0, 0)
                Gl.glVertex3f(-icon_scale + lx + uc.x, (icon_scale * 0.63) + uc.y - ly, -0.11)

                Gl.glTexCoord2f(-1, 0)
                Gl.glVertex3f(icon_scale + lx + uc.x, (icon_scale * 0.63) + uc.y - ly, -0.11)

                Gl.glTexCoord2f(-1, 1)
                Gl.glVertex3f(icon_scale + lx + uc.x, (-icon_scale * 0.63) + uc.y - ly, -0.11)
                Gl.glEnd()
            End If
        Next
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        'Gl.glPopMatrix()
        'Gl.glPushMatrix()
        'Gl.glTranslatef(0.0, 0.0F, -55.1F)

        'draw the text/numbered columns and rows
        For i = 0 To 9
            Dim K = ((i + 1) * xy) - 10
            glutPrintBox((K * scale) + uc.x - (xy / 2) + 4, uc.y - 14, NUMS(i), 1.0!, 1.0!, 0.0!, 1.0!)
        Next
        For i = 0 To 9
            Dim K = ((i + 1) * xy) - 3
            glutPrintBox((2) + uc.x, (-K * scale) + uc.y + (xy / 2), letters(i), 1.0!, 1.0!, 0.0!, 1.0!)
        Next
        For t1 = 0 To 14
            If locations.team_1(t1).track_displaylist > -1 Then
                Dim lx = (((locations.team_1(t1).loc_x + xc) / vs) * (-xy)) + (minimap_size / 2.0) ' + (ic_x * scale)
                Dim ly = (((locations.team_1(t1).loc_z + yc) / vs) * (-xy)) + (minimap_size / 2.0) ' - (ic_y * scale)
                glutPrint(lx - ((locations.team_1(t1).name.Length / 2) * 8) + 2, -ly - 2, locations.team_1(t1).name, 0.0, 0.0, 0.0, 0.3)
                glutPrint(lx - ((locations.team_1(t1).name.Length / 2) * 8) + 1, -ly - 1, locations.team_1(t1).name, 0.0, 0.0, 0.0, 0.5)
                glutPrint(lx - ((locations.team_1(t1).name.Length / 2) * 8), -ly, locations.team_1(t1).name, 1.0, 1.0, 1.0, 1.0)

            End If
            If locations.team_2(t1).track_displaylist > -1 Then
                Dim lx = (((locations.team_2(t1).loc_x + xc) / vs) * (-xy)) + (minimap_size / 2.0) ' - (ic_x * scale)
                Dim ly = (((locations.team_2(t1).loc_z + yc) / vs) * (-xy)) + (minimap_size / 2.0) ' - (ic_y * scale)
                glutPrint(lx - ((locations.team_2(t1).name.Length / 2) * 8) + 2, -ly - 2, locations.team_2(t1).name, 0.0, 0.0, 0.0, 0.3)
                glutPrint(lx - ((locations.team_2(t1).name.Length / 2) * 8) + 1, -ly - 1, locations.team_2(t1).name, 0.0, 0.0, 0.0, 0.5)
                glutPrint(lx - ((locations.team_2(t1).name.Length / 2) * 8), -ly, locations.team_2(t1).name, 1.0, 1.0, 1.0, 1.0)


            End If
        Next
        Gl.glDisable(Gl.GL_BLEND)
        Dim os = 2 * scale


        'Gl.glColor3f(0.9!, 0.9!, 0.0!)
        'Gl.glPointSize(6.0)
        'Gl.glColor3f(0.9!, 0.9!, 0.0!)
        'Gl.glBegin(Gl.GL_POINTS)
        'Gl.glVertex3f(mex + uc.x, -mey + uc.y, -0.1)
        'Gl.glEnd()
        'Gl.glLineWidth(2.0)
        'Gl.glBegin(Gl.GL_LINES)
        'Gl.glVertex3f(mex + uc.x, -mey + uc.y, -0.1)

        'Dim hpie As Single = PI / 2
        'Dim d_x As Single = Cos((PI * 1.5) - (Cam_X_angle + angle_offset)) * (15 * scale)
        'Dim d_y As Single = Sin((PI * 1.5) - (Cam_X_angle + angle_offset)) * (15 * scale)
        'Gl.glVertex3f(mex + d_x + uc.x, -mey - d_y + uc.y, -0.1)
        'Gl.glEnd()
        Gl.glEnable(Gl.GL_LIGHTING)
        Try
            Dim xr = Floor((mex / minimap_size) * 10)
            Dim yr = Floor((mey / minimap_size) * 10)
            coordStr = letters(CInt(yr)) + NUMS(CInt(xr))
        Catch ex As Exception
        End Try
        Gl.glFlush()
        'Gdi.SwapBuffers(hDC)
        Dim bmp = New Bitmap(Psize, Psize)
        Dim rect As New System.Drawing.Rectangle(0, 0, Psize, Psize)
        Dim data As BitmapData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
        Gl.glReadPixels(0, 0, Psize, Psize, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, data.Scan0)
        bmp.UnlockBits(data)

        bmp.RotateFlip(RotateFlipType.RotateNoneFlipY)
        pb1.Width = old_wx
        pb1.Height = old_wy
        minimap_size = old_msize
        Gl.glPopMatrix()
        Return bmp

    End Function
    Public Sub draw_heading()

        Dim sc As Single = 75
        Dim r As Single = 35.0
        Dim r2 As Single = 25.0
        Dim xo As Single = -12
        Dim yo As Single = -5
        'draw the compus face
        Gl.glDisable(Gl.GL_LIGHTING) 'shut this off so it dont get messed up by the lights
        Gl.glPushMatrix()
        Gl.glBlendEquation(Gl.GL_FUNC_ADD)
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        'Dim loc = Gl.glGetUniformLocation(comp_shader, "colorMap")
        'Gl.glUseProgram(comp_shader)
        'Gl.glUniform1i(loc, 0)
        'Gl.glActiveTexture(Gl.GL_TEXTURE0)
        ''Gl.glRotatef(Cam_X_angle * 57.2957795, 0.0, 0.0, 1.0)
        'Gl.glRotatef(90, 1.0, 0.0, 0.0)
        ''Gl.glTranslatef(-sc, -0.1, -sc)
        Gl.glColor4f(0.3, 0.0, 0.0, 0.6)
        Gl.glTranslatef(pb1.Width / 2, -sc, 0.0)
        'Gl.glScalef(1.0, 1.0, 1.0)
        'Gl.glBindTexture(Gl.GL_TEXTURE_2D, compus_textureID)
        'Gl.glCallList(compus_displayID)
        Dim disk As Tao.OpenGl.Glu.GLUquadric
        disk = Glu.gluNewQuadric
        Glu.gluDisk(disk, 38.0, 55.0, 60, 10)
        'Gl.glUseProgram(0)
        Gl.glLineWidth(3)
        Dim sx, sy As Single
        Dim halfPI As Single = PI / 2
        Dim dv As Single = PI / 30
        Dim radi As Single = 0.0F
        'Gl.glScalef(1.0, 1.0, 1.0)
        Dim agl As Single
        If NetData Then
            agl = (PI / 2) - Packet_in.Rx
        Else
            agl = (PI / 2) - Cam_X_angle
        End If
        sx = (-Cos(agl) * r)
        sy = (-Sin(agl) * r)
        Dim lx As Single = (-Cos(agl - dv) * r2)
        Dim rx As Single = (-Cos(agl + dv) * r2)
        Dim ly As Single = (-Sin(agl - dv) * r2)
        Dim ry As Single = (-Sin(agl + dv) * r2)

        Gl.glBegin(Gl.GL_LINES)
        Gl.glColor4f(1.0, 1.0, 0.0, 0.0)
        'Gl.glColor3f(0.0, 0.0, 0.0)
        Gl.glVertex3f(0, 0, -0.01)
        Gl.glColor4f(1.0, 1.0, 0.0, 1.0)
        Gl.glVertex3f(sx, -sy, -0.01)

        Gl.glVertex3f(sx, -sy, -0.01)
        Gl.glVertex3f(lx, -ly, -0.01)

        Gl.glVertex3f(sx, -sy, -0.01)
        Gl.glVertex3f(rx, -ry, -0.01)
        'Gl.glDisable(Gl.GL_BLEND)

        Gl.glEnd()
        r = 50.0
        glutPrint((Cos(radi) * r) + xo + 3, (Sin(radi) * r) + yo, "E", 0.0, 1.0, 0.0, 1.0)
        radi += halfPI
        glutPrint((Cos(radi) * r) + xo + 8, (Sin(radi) * r) + yo - 3, "N", 0.0, 1.0, 0.0, 1.0)
        radi += halfPI
        glutPrint((Cos(radi) * r) + xo + 12, (Sin(radi) * r) + yo, "W", 0.0, 1.0, 0.0, 1.0)
        radi += halfPI
        glutPrint((Cos(radi) * r) + xo + 8, (Sin(radi) * r) + yo + 4, "S", 0.0, 1.0, 0.0, 1.0)
        radi += halfPI
        Gl.glPopMatrix()
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glEnable(Gl.GL_LIGHTING) ' turn lighting back on
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        'glutPrint(10, -300, m_rot.ToString, 1, 1, 1, 1)
    End Sub

    'Public Sub make_compus()
    '    'setting up compus model for heading display
    '    'Im using a .primitives and .visual I made for this
    '    'By setting this up in a maplist type, I can reuse that code to load it.
    '    If Not compus_displayID > -1 Then
    '        maplist(0) = New grid_sec
    '        ReDim maplist(0).models(1)
    '        maplist(0).model_list = New List(Of String)
    '        maplist(0).model_list.Add("compus")

    '        maplist(0).models(0) = New primitive
    '        ReDim maplist(0).models(0).componets(1)
    '        maplist(0).models(0).componets(0) = New Model_Section
    '        maplist(0).models(0)._count = 0
    '        TheXML_String = IO.File.ReadAllText(Application.StartupPath.ToString + "\compus\compus.visual")
    '        Dim tary2 = IO.File.ReadAllBytes(Application.StartupPath.ToString + "\compus\compus.primitives")
    '        Dim ms As New MemoryStream(tary2)
    '        get_primitive_data(ms, 0, 0, False)
    '        maplist(0).models(0).componets(0).callList_ID = Gl.glGenLists(1)
    '        Gl.glNewList(maplist(0).models(0).componets(0).callList_ID, Gl.GL_COMPILE)
    '        UVs_ON = True
    '        simple_vertex(0, 0, 0)
    '        UVs_ON = False
    '        Gl.glEndList()
    '        Gl.glFinish()
    '        ms.Dispose()
    '        Dim tary1 = IO.File.ReadAllBytes(Application.StartupPath.ToString + "\compus\compus.dds")
    '        ms = New MemoryStream(tary1)
    '        compus_displayID = maplist(0).models(0).componets(0).callList_ID
    '        compus_textureID = get_texture(ms, False)
    '        maplist(0).model_list.Clear()
    '    End If


    'End Sub

    Public Sub draw_XZ_grid()
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glLineWidth(1)
        Gl.glBegin(Gl.GL_LINES)
        Gl.glColor3f(0.0F, 0.0F, 0.0F)
        For z As Single = -100.0F To -1.0F Step 1.0
            Gl.glVertex3f(-100.0F, 0.0F, z)
            Gl.glVertex3f(100.0F, 0.0F, z)
        Next
        For z As Single = 1.0F To 100.0F Step 1.0
            Gl.glVertex3f(-100.0F, 0.0F, z)
            Gl.glVertex3f(100.0F, 0.0F, z)
        Next
        For x As Single = -100.0F To -1.0F Step 1.0
            Gl.glVertex3f(x, 0.0F, 100.0F)
            Gl.glVertex3f(x, 0.0F, -100.0F)
        Next
        For x As Single = 1.0F To 100.0F Step 1.0
            Gl.glVertex3f(x, 0.0F, 100.0F)
            Gl.glVertex3f(x, 0.0F, -100.0F)
        Next
        Gl.glEnd()
        Gl.glLineWidth(1)
        Gl.glBegin(Gl.GL_LINES)
        Gl.glColor3f(0.6F, 0.6F, 0.6F)
        Gl.glVertex3f(1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(-1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(0.0F, 0.0F, 1.0F)
        Gl.glVertex3f(0.0F, 0.0F, -1.0F)
        Gl.glEnd()
        'begin axis markers
        ' red is z+
        ' green is x-
        'blue is z-
        ' yellow x+
        Gl.glLineWidth(1)

        Gl.glBegin(Gl.GL_LINES)
        'z+ red
        Gl.glColor3f(1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(0.0F, 0.0F, 1.0F)
        Gl.glVertex3f(0.0F, 0.0F, 100.0F)
        'z- blue
        Gl.glColor3f(0.0F, 0.0F, 1.0F)
        Gl.glVertex3f(0.0F, 0.0F, -1.0F)
        Gl.glVertex3f(0.0F, 0.0F, -100.0F)
        'x+ yellow
        Gl.glColor3f(1.0F, 1.0F, 0.0F)
        Gl.glVertex3f(1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(100.0F, 0.0F, 0.0F)
        'x- green
        Gl.glColor3f(0.0F, 1.0F, 0.0F)
        Gl.glVertex3f(-1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(-100.0F, 0.0F, 0.0F)
        '---------
        Gl.glEnd()

        Gl.glEnable(Gl.GL_LIGHTING)

    End Sub

    Public Sub set_eyes()
        If NetData Then
            sin_x = Sin(Packet_in.Rx + angle_offset)
            cos_x = Cos(Packet_in.Rx + angle_offset)
            cos_y = Cos(Packet_in.Ry)
            sin_y = Sin(Packet_in.Ry)
            cam_y = Sin(Packet_in.Ry) * Packet_in.Lr
            cam_x = (sin_x - (1 - cos_y) * sin_x) * Packet_in.Lr
            cam_z = (cos_x - (1 - cos_y) * cos_x) * Packet_in.Lr
            Packet_in.Ey = get_Z_at_XY(Packet_in.Ex, Packet_in.Ez)
            'Look_at_X = cam_x + Sin(Cam_X_angle) - ((1 - Cos(Cam_Y_angle)) * Sin(Cam_X_angle))
            'Look_at_Y = cam_y + Sin(Cam_Y_angle)
            'Look_at_Z = cam_z + Cos(Cam_X_angle) - ((1 - Cos(Cam_Y_angle)) * Cos(Cam_X_angle))
            Glu.gluLookAt(cam_x + Packet_in.Ex, cam_y + Packet_in.Ey, cam_z + Packet_in.Ez, _
                              Packet_in.Ex, Packet_in.Ey, Packet_in.Ez, 0.0F, 1.0F, 0.0F)
            'Glu.gluLookAt(0, 0, 30, look_point_X, look_point_Y, look_point_Z, 0.0F, 1.0F, 0.0F)
            look_point_X = Packet_in.Ex
            look_point_Z = Packet_in.Ez
            View_Radius = Packet_in.Lr
            Cam_X_angle = Packet_in.Rx
            Cam_Y_angle = Packet_in.Ry
            eyeX = cam_x + Packet_in.Ex
            eyeY = cam_y + Packet_in.Ey
            eyeZ = cam_z + Packet_in.Ez
            'tb1.Text = "X: " + Packet_in.Ex.ToString + "   y: " + Packet_in.Ez.ToString + _
            '	"   Xrot: " + Packet_in.Rx.ToString + _
            '	"   Yrot: " + Packet_in.Ry.ToString + vbCrLf + "   TankId: " + Packet_in.tankId.ToString + _
            '	"  Tank Id string: " + Packet_in.ID
        Else

            sin_x = Sin(u_Cam_X_angle + angle_offset)
            cos_x = Cos(u_Cam_X_angle + angle_offset)
            cos_y = Cos(u_Cam_Y_angle)
            sin_y = Sin(u_Cam_Y_angle)
            cam_y = Sin(u_Cam_Y_angle) * View_Radius
            cam_x = (sin_x - (1 - cos_y) * sin_x) * View_Radius
            cam_z = (cos_x - (1 - cos_y) * cos_x) * View_Radius

            Glu.gluLookAt(cam_x + u_look_point_X, cam_y + u_look_point_Y + u_y_offset, cam_z + u_look_point_Z, _
                              u_look_point_X, u_look_point_Y + u_y_offset, u_look_point_Z, 0.0F, 1.0F, 0.0F)

            eyeX = cam_x + u_look_point_X
            eyeY = cam_y + u_look_point_Y + u_y_offset
            eyeZ = cam_z + u_look_point_Z
        End If

    End Sub
    Private Sub pb1_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pb1.MouseDoubleClick
        If Not _STARTED Then Return
        Return
        If frmFullScreen.Visible Then
            If pb1.Parent.Name = frmFullScreen.Name Then
                pb1.Parent = Nothing
                frmFullScreen.WindowState = FormWindowState.Normal
                Me.Controls.Add(pb1)
                pb1.Size = old_pb1_size
                pb1.Location = pb1_form_location
                pb1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top
                frmFullScreen.Visible = False
                frmFullScreen.Location = pb1_screen_location
                pb1.Focus()
            End If
        Else
            If pb1.Parent.Name = Me.Name Then
                pb1_screen_location = pb1.PointToScreen(New System.Drawing.Point)
                pb1_form_location = pb1.Location
                old_pb1_size = pb1.Size
                pb1.Parent = Nothing
                frmFullScreen.Visible = True
                frmFullScreen.WindowState = FormWindowState.Maximized
                'frmFullScreen.WindowState = FormWindowState.Normal
                frmFullScreen.TopMost = True
                frmFullScreen.Controls.Add(pb1)
                pb1.Dock = DockStyle.Fill
                frmFullScreen.Location = pb1_screen_location
            End If
        End If
        frmFullScreen.Size = old_pb1_size
    End Sub
    Private Sub pb1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pb1.MouseDown
        'let the user cancel loading a map"
        If view_mode Then
            Return
        End If
        If SHOW_MAPS Then
            If e.Button = Forms.MouseButtons.Left Then
                If selected_map_hit = 0 Then
                    SHOW_MAPS = False
                    draw_scene()
                    draw_scene()
                    Application.DoEvents()
                    Application.DoEvents()


                Else
                    SHOW_MAPS = False
                    mini_map_loaded = False
                    If maploaded Then
                        save_light_settings() ' save our light settings
                    End If
                    maploaded = False
                    draw_scene()
                    Dim dx = selected_map_hit - 1
                    Dim r_name = loadmaplist(dx).realname
                    Dim p_name = loadmaplist(dx).name.Replace(".png", ".pkg")
                    Dim a1 = p_name.Split(":")
                    p_name = a1(0)
                    Dim FileName = GAME_PATH & "\res\packages\" & p_name
                    tb1.Text = "loading map: " + r_name + vbCrLf
                    Me.Text = r_name
                    View_Radius = -150
                    look_point_X = 0.0 : look_point_Y = 0.0 : look_point_Z = 0.0
                    m_fly_map.Checked = False
                    water.IsWater = False
                    Dim ar() = FileName.Split("\")
                    Dim sn1 = ar(ar.Length - 1)
                    Dim sn2 = sn1.Split(".")
                    Dim shortname = sn2(0)
                    flush_data() 'clear everything
                    saved_texture_loads = 0
                    saved_model_loads = 0
                    resetBoundingBox()
                    Gl.glFinish()
                    Application.DoEvents()
                    Application.DoEvents()

                    'if we are hosting a session we need to load 
                    'just like everyone else would.
                    draw_scene()
                    load_map_name = sn1
                    JUST_MAP_NAME = Path.GetFileNameWithoutExtension(sn1)

                    open_pkg(sn1)
                    SHOW_MAPS = False
                    draw_scene()
                    SHOW_MAPS = False
                    Return
                End If
            End If
        End If


        'do some math and see if we clicked on the minimap
        'if so. move the eye center to that place and also any tank that happens to be selected.
        If m_show_minimap.Checked And m_show_status.Checked Then
            If e.X > (pb1.Width - minimap_size) And e.Y > (pb1.Height - minimap_size) Then
                Dim xc = MAP_BB_UR.x + MAP_BB_BL.x
                Dim yc = MAP_BB_UR.y + MAP_BB_BL.y
                Dim w As Double = MAP_BB_UR.x - MAP_BB_BL.x
                Dim v As Double = MAP_BB_UR.y - MAP_BB_BL.y
                If w = 0 Then Return
                Dim scale As Single = (w / minimap_size)
                Dim xp = e.X - (pb1.Width - minimap_size) - (minimap_size / 2)
                Dim yp = e.Y - (pb1.Height - minimap_size) - (minimap_size / 2)
                look_point_X = (-xp * scale) + (xc / 2)
                look_point_Z = (-yp * scale) + (yc / 2)
                If tankID > -1 Then
                    If tankID < 100 Then
                        locations.team_1(tankID).loc_x = look_point_X
                        locations.team_1(tankID).loc_z = look_point_Z
                        Packet_out.Tx = look_point_X
                        Packet_out.Tz = look_point_Z
                    Else
                        Dim k = tankID - 100
                        locations.team_2(k).loc_x = look_point_X
                        locations.team_2(k).loc_z = look_point_Z
                        Packet_out.Tx = look_point_X
                        Packet_out.Tz = look_point_Z
                    End If
                End If
                Packet_out.Ex = look_point_X
                Packet_out.Ez = look_point_Z
                position_camera()
                set_eyes()
                draw_scene()
            End If
        End If

        pb1.Focus()
        If e.Button = Forms.MouseButtons.Right Then
            'Timer1.Enabled = False
            move_cam_z = True
            mouse.X = e.X
            mouse.Y = e.Y
        End If
        If e.Button = Forms.MouseButtons.Left Then
            M_DOWN = True
            mouse.X = e.X
            mouse.Y = e.Y
        End If
        If e.Button = Forms.MouseButtons.Middle Then
            GetOGLPos(e.X, e.Y)
        End If
    End Sub

    Private Sub pb1_MouseEnter(sender As Object, e As EventArgs) Handles pb1.MouseEnter
        If m_comment.Focused Then
            Return
        End If
        pb1.Focus()
    End Sub


    Private Sub pb1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pb1.MouseMove

        If SHOW_MAPS Then
            mouse.X = e.X
            mouse.Y = e.Y
            gl_pick_map(e.X, e.Y)
            Application.DoEvents()
            Return
        End If

        Dim dead As Integer = 5
        Dim t As Double
        Dim M_Speed As Double = 0.8
        Dim tempX, tempZ As Single
        tempZ = look_point_Z
        tempX = look_point_X
        Dim ms As Double = 1.0F * View_Radius ' distance away changes speed. THIS WORKS WELL!
        Dim ms2 As Double = 0.25 * View_Radius  ' distance away changes speed. THIS WORKS WELL!
        If M_DOWN Then
            If e.X > (mouse.X + dead) Then
                If e.X - mouse.X > 100 Then t = (1.0F * M_Speed)
            Else : t = CSng(Sin((e.X - mouse.X) / 100)) * M_Speed
                If Not ROTATE_TANK Then
                Else
                    If ROTATE_TANK Then
                        Dim tr As Single
                        If tankID > -1 Then
                            If tankID >= 100 Then
                                tr = locations.team_2(tankID - 100).rot_y
                                tr -= t
                                If tr < 0 Then tr += (2 * PI)
                                locations.team_2(tankID - 100).rot_y = tr
                                Packet_out.Tr = tr
                            Else
                                tr = locations.team_1(tankID).rot_y
                                tr -= t
                                If tr < 0 Then tr += (2 * PI)
                                locations.team_1(tankID).rot_y = tr
                                Packet_out.Tr = tr
                            End If
                            mouse.X = e.X
                            'draw_scene()
                        End If
                    End If
                End If
            End If
            If M_DOWN Then
                If e.X < (mouse.X - dead) Then
                    If mouse.X - e.X > 100 Then t = (M_Speed)
                Else : t = CSng(Sin((mouse.X - e.X) / 100)) * M_Speed
                    If Not ROTATE_TANK Then
                    Else
                        If ROTATE_TANK Then
                            Dim tr As Single
                            If tankID > -1 Then
                                If tankID >= 100 Then
                                    tr = locations.team_2(tankID - 100).rot_y
                                    tr += t
                                    If tr > (2 * PI) Then tr -= (2 * PI)
                                    locations.team_2(tankID - 100).rot_y = tr
                                    Packet_out.Tr = tr
                                Else
                                    tr = locations.team_1(tankID).rot_y
                                    tr += t
                                    If tr > (2 * PI) Then tr -= (2 * PI)
                                    locations.team_1(tankID).rot_y = tr
                                    Packet_out.Tr = tr
                                End If
                            End If
                        End If
                        mouse.X = e.X
                    End If
                End If
            End If
        End If
        If M_DOWN Then
            If e.X > (mouse.X + dead) Then
                If e.X - mouse.X > 100 Then t = (1.0F * M_Speed)
            Else : t = CSng(Sin((e.X - mouse.X) / 100)) * M_Speed
                If Not z_move Then
                    If move_mod Then ' check for modifying flag
                        tempX -= ((t * ms) * (Cos(Cam_X_angle)))
                        tempZ -= ((t * ms) * (-Sin(Cam_X_angle)))
                        If tempX < MAP_BB_BL.x Then
                            tempX = MAP_BB_BL.x
                        End If
                        If tempX > MAP_BB_UR.x Then
                            tempX = MAP_BB_UR.x
                        End If
                        If tempZ < MAP_BB_BL.y Then
                            tempZ = MAP_BB_BL.y
                        End If
                        If tempZ > MAP_BB_UR.y Then
                            tempZ = MAP_BB_UR.y
                        End If
                        If Not check_zoom_collision() Then
                            look_point_X = tempX : look_point_Z = tempZ
                        End If

                    Else
                        ' If Not ROTATE_TANK Then
                        Cam_X_angle = check_border_collision_y_rot(-t)
                        'Cam_X_angle -= t
                        If Cam_X_angle > (2 * PI) Then Cam_X_angle -= (2 * PI)
                    End If
                    'End If
                    mouse.X = e.X
no_move_xz:
                End If
            End If
            If e.X < (mouse.X - dead) Then
                If mouse.X - e.X > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.X - e.X) / 100)) * M_Speed
                If Not z_move Then
                    If move_mod Then ' check for modifying flag
                        tempX += ((t * ms) * (Cos(Cam_X_angle)))
                        tempZ += ((t * ms) * (-Sin(Cam_X_angle)))
                        If tempX < MAP_BB_BL.x Then
                            tempX = MAP_BB_BL.x
                        End If
                        If tempX > MAP_BB_UR.x Then
                            tempX = MAP_BB_UR.x
                        End If
                        If tempZ < MAP_BB_BL.y Then
                            tempZ = MAP_BB_BL.y
                        End If
                        If tempZ > MAP_BB_UR.y Then
                            tempZ = MAP_BB_UR.y
                        End If
                        If Not check_zoom_collision() Then
                            look_point_X = tempX : look_point_Z = tempZ
                        End If
                    Else
                        '     If Not ROTATE_TANK And Not MOVE_TANK Then

                        Cam_X_angle = check_border_collision_y_rot(t)
                        'Cam_X_angle += t
                        If Cam_X_angle < 0 Then Cam_X_angle += (2 * PI)
                    End If
                    '  End If
                    mouse.X = e.X

                    Try
                        'This has to remain "INLINE" as it kills the mouse's reaction time.
                        If maploaded Then

                            'Dim xvp As Integer = ((look_point_X - 50) / 100) + (MAP_SIDE_LENGTH / 2)
                            'Dim yvp As Integer = ((look_point_Z + 50) / 100) + (MAP_SIDE_LENGTH / 2)
                            'Dim rxp As Integer = (((Floor(look_point_X) / 100)) - Floor((Floor(look_point_X) / 100))) * 65
                            'Dim ryp As Integer = (((Floor(look_point_Z) / 100)) - Floor((Floor(look_point_Z) / 100))) * 65
                            'Dim map = mapBoard(xvp, yvp)
                            Z_Cursor = get_Z_at_XY(look_point_X, look_point_Z)  'maplist(map).heights(rxp, ryp) ' + 1
                            look_point_Y = Z_Cursor ' + 5
                        End If
                    Catch ex As Exception

                    End Try
                End If
            End If
            If e.Y > (mouse.Y + dead) Then
                If e.Y - mouse.Y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((e.Y - mouse.Y) / 100)) * M_Speed
                If z_move Then
                    y_offset -= (t * ms)

                Else
                    If move_mod Then ' check for modifying flag
                        tempZ -= ((t * ms) * (Cos(Cam_X_angle)))
                        tempX -= ((t * ms) * (Sin(Cam_X_angle)))
                        If tempX < MAP_BB_BL.x Then
                            tempX = MAP_BB_BL.x
                        End If
                        If tempX > MAP_BB_UR.x Then
                            tempX = MAP_BB_UR.x
                        End If
                        If tempZ < MAP_BB_BL.y Then
                            tempZ = MAP_BB_BL.y
                        End If
                        If tempZ > MAP_BB_UR.y Then
                            tempZ = MAP_BB_UR.y
                        End If
                        If Not check_zoom_collision() Then
                            look_point_X = tempX : look_point_Z = tempZ
                        End If

                    Else
                        If Not ROTATE_TANK And Not MOVE_TANK Then
                            Cam_Y_angle = check_border_collision_x_rot(-t)
                        End If
                    End If
                End If
                'If Cam_Y_angle < -1.5707 Then Cam_Y_angle = -1.5707
                mouse.Y = e.Y
            End If
            If e.Y < (mouse.Y - dead) Then
                If mouse.Y - e.Y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.Y - e.Y) / 100)) * M_Speed
                If z_move Then
                    y_offset += (t * ms)
                Else
                    If move_mod Then ' check for modifying flag
                        tempZ += ((t * ms) * (Cos(Cam_X_angle)))
                        tempX += ((t * ms) * (Sin(Cam_X_angle)))
                        If tempX < MAP_BB_BL.x Then
                            tempX = MAP_BB_BL.x
                        End If
                        If tempX > MAP_BB_UR.x Then
                            tempX = MAP_BB_UR.x
                        End If
                        If tempZ < MAP_BB_BL.y Then
                            tempZ = MAP_BB_BL.y
                        End If
                        If tempZ > MAP_BB_UR.y Then
                            tempZ = MAP_BB_UR.y
                        End If
                        If Not check_zoom_collision() Then
                            look_point_X = tempX : look_point_Z = tempZ
                        End If

                    Else
                        If Not ROTATE_TANK And Not MOVE_TANK Then
                            Cam_Y_angle = check_border_collision_x_rot(t)
                        End If
                    End If
                End If
                mouse.Y = e.Y
            End If
            If Cam_Y_angle > -0.0 Then Cam_Y_angle = -0.0
            If Cam_Y_angle < -1.5707 Then Cam_Y_angle = -1.5707
            If Not ROTATE_TANK Then
                If MOVE_TANK Then
                    If tankID > -1 Then
                        If tankID > -1 Then
                            If tankID >= 100 Then
                                locations.team_2(tankID - 100).loc_x = look_point_X
                                locations.team_2(tankID - 100).loc_z = look_point_Z
                                Packet_out.Tx = look_point_X
                                Packet_out.Tz = look_point_Z
                            Else
                                locations.team_1(tankID).loc_x = look_point_X
                                locations.team_1(tankID).loc_z = look_point_Z
                                Packet_out.Tx = look_point_X
                                Packet_out.Tz = look_point_Z
                            End If
                        End If
                    End If
                End If
            End If
            Packet_out.Ex = look_point_X
            Packet_out.Ez = look_point_Z
            Packet_out.Ey = look_point_Y
            Packet_out.Rx = Cam_X_angle
            Packet_out.Ry = Cam_Y_angle
            Packet_out.Lr = View_Radius

        End If
        If move_cam_z Then
            If e.Y > (mouse.Y + dead) Then
                If e.Y - mouse.Y > 100 Then t = (10)
            Else : t = CSng(Sin((e.Y - mouse.Y) / 100)) * 12
                Dim tl = View_Radius
                View_Radius += (t * (View_Radius * 0.2))    ' zoom is factored in to look radius
                If check_zoom_collision() Then
                    View_Radius = tl
                End If
                mouse.Y = e.Y
            End If
            If e.Y < (mouse.Y - dead) Then
                If mouse.Y - e.Y > 100 Then t = (10)
            Else : t = CSng(Sin((mouse.Y - e.Y) / 100)) * 12
                Dim tl = View_Radius
                View_Radius -= (t * (View_Radius * 0.2))    ' zoom is factored in to look radius
                If check_zoom_collision() Then
                    View_Radius = tl
                End If
                If View_Radius > -5.0 Then View_Radius = -5.0
                mouse.Y = e.Y
            End If
            If View_Radius > -1.0 Then View_Radius = -1.0
            'If View_Radius < -550 Then View_Radius = -550
            If View_Radius < -1550 Then View_Radius = -1550
            Packet_out.Ex = look_point_X
            Packet_out.Ez = look_point_Z
            Packet_out.Ey = look_point_Y
            Packet_out.Rx = Cam_X_angle
            Packet_out.Ry = Cam_Y_angle
            Packet_out.Lr = View_Radius

        End If

    End Sub
    Private Sub pb1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pb1.MouseUp
        If e.Button = Forms.MouseButtons.Right Then
            move_cam_z = False
        End If
        If e.Button = Forms.MouseButtons.Left Then
            M_DOWN = False
            M_MOVE = True
        End If

    End Sub
    Private Sub pb1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pb1.Paint
        If Not _STARTED Then Return
        If frmShowImage.Visible Then
            Return
        End If
        If Not SHOW_MAPS Then
            draw_scene()
        Else
            gl_pick_map(mouse.X, mouse.Y)

        End If
    End Sub

    Public Function check_border_collision_y_rot(ByVal t As Single) As Single
        If m_fly_map.Checked Then
            Cam_X_angle += t
            Return Cam_X_angle
        End If
        Dim tx = Cam_X_angle
        Dim ty = Cam_Y_angle
        Cam_X_angle += t
        sin_x = Sin(Cam_X_angle + angle_offset)
        cos_x = Cos(Cam_X_angle + angle_offset)
        cos_y = Cos(Cam_Y_angle)
        sin_y = Sin(Cam_Y_angle)
        cam_y = Sin(Cam_Y_angle) * View_Radius
        cam_x = (sin_x - (1 - cos_y) * sin_x) * View_Radius
        cam_z = (cos_x - (1 - cos_y) * cos_x) * View_Radius
        'Look_at_X = cam_x + Sin(Cam_X_angle) - ((1 - Cos(Cam_Y_angle)) * Sin(Cam_X_angle))
        'Look_at_Y = cam_y + Sin(Cam_Y_angle)
        'Look_at_Z = cam_z + Cos(Cam_X_angle) - ((1 - Cos(Cam_Y_angle)) * Cos(Cam_X_angle))
        'Glu.gluLookAt(cam_x + look_point_X, cam_y + look_point_Y, cam_z + look_point_Z, look_point_X, look_point_Y, look_point_Z, 0.0F, 1.0F, 0.0F)
        'Glu.gluLookAt(0, 0, 30, look_point_X, look_point_Y, look_point_Z, 0.0F, 1.0F, 0.0F)
        'Dim X = cam_x + look_point_X
        'Dim Z = cam_z + look_point_Z
        'If X < MAP_BB_BL.x Then
        '	Return tx
        'End If
        'If X > MAP_BB_UR.x Then
        '	Return tx
        'End If
        'If Z < MAP_BB_BL.y Then
        '	Return tx
        'End If
        'If Z > MAP_BB_UR.y Then
        '	Return tx
        'End If
        Return Cam_X_angle
    End Function
    Public Function check_border_collision_x_rot(ByVal t As Single) As Single
        If m_fly_map.Checked Then
            Cam_Y_angle += t
            Return Cam_Y_angle
        End If
        Dim tx = Cam_X_angle
        Dim ty = Cam_Y_angle
        Cam_Y_angle += t
        sin_x = Sin(Cam_X_angle + angle_offset)
        cos_x = Cos(Cam_X_angle + angle_offset)
        cos_y = Cos(Cam_Y_angle)
        sin_y = Sin(Cam_Y_angle)
        cam_y = Sin(Cam_Y_angle) * View_Radius
        cam_x = (sin_x - (1 - cos_y) * sin_x) * View_Radius
        cam_z = (cos_x - (1 - cos_y) * cos_x) * View_Radius
        'Dim X = cam_x + look_point_X
        'Dim Y = cam_z + look_point_Z
        'If X < MAP_BB_BL.x Then
        '	Return ty
        'End If
        'If X > MAP_BB_UR.x Then
        '	Return ty
        'End If
        'If Y < MAP_BB_BL.y Then
        '	Return ty
        'End If
        'If Y > MAP_BB_UR.y Then
        '	Return ty
        'End If
        Return Cam_Y_angle
    End Function
    Public Function check_zoom_collision() As Boolean
        If m_fly_map.Checked Then
            Return False
        End If
        'sin_x = Sin(Cam_X_angle + angle_offset)
        'cos_x = Cos(Cam_X_angle + angle_offset)
        'cos_y = Cos(Cam_Y_angle)
        'sin_y = Sin(Cam_Y_angle)
        'cam_x = (sin_x - (1 - cos_y) * sin_x) * View_Radius
        'cam_z = (cos_x - (1 - cos_y) * cos_x) * View_Radius
        'Dim X = cam_x + look_point_X
        'Dim Y = cam_z + look_point_Z
        'If X < MAP_BB_BL.x Then
        '	Return True
        'End If
        'If X > MAP_BB_UR.x Then
        '	Return True
        'End If
        'If Y < MAP_BB_BL.y Then
        '	Return True
        'End If
        'If Y > MAP_BB_UR.y Then
        '	Return True
        'End If
        Return False
    End Function


    Public Sub position_camera()
        Dim dead As Integer = 5
        Dim t As Double
        Dim M_Speed As Double = 0.8
        Dim tempX, tempZ As Single
        tempZ = look_point_Z
        tempX = look_point_X
        Dim e As New Point
        e.X = mouse.X : e.Y = mouse.Y
        Dim ms As Double = 1.0F * View_Radius ' distance away changes speed. THIS WORKS WELL!
        If M_DOWN Then
            If e.X > (mouse.X + dead) Then
                If e.X - mouse.X > 100 Then t = (1.0F * M_Speed)
            Else : t = CSng(Sin((e.X - mouse.X) / 100)) * M_Speed
                If Not z_move Then
                    If move_mod Then ' check for modifying flag
                        tempX -= ((t * ms) * (Cos(Cam_X_angle)))
                        tempZ -= ((t * ms) * (-Sin(Cam_X_angle)))
                        If tempX < MAP_BB_BL.x Then
                            tempX = MAP_BB_BL.x
                        End If
                        If tempX > MAP_BB_UR.x Then
                            tempX = MAP_BB_UR.x
                        End If
                        If tempZ < MAP_BB_BL.y Then
                            tempZ = MAP_BB_BL.y
                        End If
                        If tempZ > MAP_BB_UR.y Then
                            tempZ = MAP_BB_UR.y
                        End If
                        If Not check_zoom_collision() Then
                            look_point_X = tempX : look_point_Z = tempZ
                        End If

                    Else
                        Cam_X_angle = check_border_collision_y_rot(-t)
                        'Cam_X_angle -= t
                        If Cam_X_angle > (2 * PI) Then Cam_X_angle -= (2 * PI)
                    End If
                    mouse.X = e.X
no_move_xz:
                End If
            End If
            If e.X < (mouse.X - dead) Then
                If mouse.X - e.X > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.X - e.X) / 100)) * M_Speed
                If Not z_move Then
                    If move_mod Then ' check for modifying flag
                        tempX += ((t * ms) * (Cos(Cam_X_angle)))
                        tempZ += ((t * ms) * (-Sin(Cam_X_angle)))
                        If tempX < MAP_BB_BL.x Then
                            tempX = MAP_BB_BL.x
                        End If
                        If tempX > MAP_BB_UR.x Then
                            tempX = MAP_BB_UR.x
                        End If
                        If tempZ < MAP_BB_BL.y Then
                            tempZ = MAP_BB_BL.y
                        End If
                        If tempZ > MAP_BB_UR.y Then
                            tempZ = MAP_BB_UR.y
                        End If
                        If Not check_zoom_collision() Then
                            look_point_X = tempX : look_point_Z = tempZ
                        End If
                    Else
                        Cam_X_angle = check_border_collision_y_rot(t)
                        'Cam_X_angle += t
                    End If
                    If Cam_X_angle < 0 Then Cam_X_angle += (2 * PI)
                    mouse.X = e.X

                    Try
                        'This has to remain "INLINE" as it kills the mouse's reaction time.
                        If maploaded Then

                            'Dim xvp As Integer = ((look_point_X - 50) / 100) + (MAP_SIDE_LENGTH / 2)
                            'Dim yvp As Integer = ((look_point_Z + 50) / 100) + (MAP_SIDE_LENGTH / 2)
                            'Dim rxp As Integer = (((Floor(look_point_X) / 100)) - Floor((Floor(look_point_X) / 100))) * 65
                            'Dim ryp As Integer = (((Floor(look_point_Z) / 100)) - Floor((Floor(look_point_Z) / 100))) * 65
                            'Dim map = mapBoard(xvp, yvp)
                        End If
                    Catch ex As Exception

                    End Try
                End If
            End If
            ' ------- Y moves ----------------------------------
            If e.Y > (mouse.Y + dead) Then
                If e.Y - mouse.Y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((e.Y - mouse.Y) / 100)) * M_Speed
                If z_move Then
                    look_point_Y -= (t * ms)
                Else
                    If move_mod Then ' check for modifying flag
                        tempZ -= ((t * ms) * (Cos(Cam_X_angle)))
                        tempX -= ((t * ms) * (Sin(Cam_X_angle)))
                        If tempX > MAP_BB_UR.x Then
                            tempX = MAP_BB_UR.x
                        End If
                        If tempZ < MAP_BB_BL.y Then
                            tempZ = MAP_BB_BL.y
                        End If
                        If tempZ > MAP_BB_UR.y Then
                            tempZ = MAP_BB_UR.y
                        End If
                        If Not check_zoom_collision() Then
                            look_point_X = tempX : look_point_Z = tempZ
                        End If

                    Else
                        Cam_Y_angle = check_border_collision_x_rot(-t)
                    End If
                End If
                If Cam_Y_angle < -1.5707 Then Cam_Y_angle = -1.5707
                mouse.Y = e.Y
            End If
            If e.Y < (mouse.Y - dead) Then
                If mouse.Y - e.Y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.Y - e.Y) / 100)) * M_Speed
                If z_move Then
                    look_point_Y += (t * ms)
                Else
                    If move_mod Then ' check for modifying flag
                        tempZ += ((t * ms) * (Cos(Cam_X_angle)))
                        tempX += ((t * ms) * (Sin(Cam_X_angle)))
                        If tempX < MAP_BB_BL.x Then
                            tempX = MAP_BB_BL.x
                        End If
                        If tempX > MAP_BB_UR.x Then
                            tempX = MAP_BB_UR.x
                        End If
                        If tempZ < MAP_BB_BL.y Then
                            tempZ = MAP_BB_BL.y
                        End If
                        If tempZ > MAP_BB_UR.y Then
                            tempZ = MAP_BB_UR.y
                        End If
                        If Not check_zoom_collision() Then
                            look_point_X = tempX : look_point_Z = tempZ
                        End If

                    Else
                        Cam_Y_angle = check_border_collision_x_rot(t)
                    End If
                End If
                mouse.Y = e.Y
            End If
            If Cam_Y_angle > -0.3 Then Cam_Y_angle = -0.3
            If Cam_Y_angle < -1.5707 Then Cam_Y_angle = -1.5707
            If Not NetData Then
                draw_scene()
            End If
        End If
        If move_cam_z Then
            If e.Y > (mouse.Y + dead) Then
                If e.Y - mouse.Y > 100 Then t = (10)
            Else : t = CSng(Sin((e.Y - mouse.Y) / 100)) * 12
                Dim tl = View_Radius
                View_Radius += (t * (View_Radius * 0.2)) ' zoom is factored in to look radius
                If check_zoom_collision() Then
                    View_Radius = tl
                End If
                mouse.Y = e.Y
            End If
            If e.Y < (mouse.Y - dead) Then
                If mouse.Y - e.Y > 100 Then t = (10)
            Else : t = CSng(Sin((mouse.Y - e.Y) / 100)) * 12
                Dim tl = View_Radius
                View_Radius -= (t * (View_Radius * 0.2)) ' zoom is factored in to look radius
                If check_zoom_collision() Then
                    View_Radius = tl
                End If
                If View_Radius > -0.5 Then View_Radius = -0.5
                mouse.Y = e.Y
            End If
            If View_Radius > -0.5 Then View_Radius = -0.5
            If View_Radius < -150 Then View_Radius = -150
            If Not NetData Then
                draw_scene()
            End If

        End If
        Z_Cursor = get_Z_at_XY(look_point_X, look_point_Z)  'maplist(map).heights(rxp, ryp) ' + 1
        look_point_Y = Z_Cursor ' + 5
    End Sub

    Public Sub flush_data()
        M_DOWN = False
        'let the user know whats going on
        'Try
        tb1.Text = "Removing Previous map Data.."

        Dim er = Gl.glGetError
        Dim top As Integer = 0
        For i = 0 To 3000
            Gl.glDeleteLists(1, i)
            Gl.glFinish()
        Next
        er = Gl.glGetError
        'For i = 0 To 12
        '    Dim t_id As Integer
        '    Gl.glActiveTexture(Gl.GL_TEXTURE0 + i)
        '    Gl.glGetIntegerv(Gl.GL_TEXTURE_BINDING_2D, t_id)
        '    'Console.WriteLine("bound id" + t_id.ToString)
        'Next

        Try
            For i = 3 To 3000
                Gl.glDeleteTextures(1, i)
                'Il.ilBindImage(i)
                'Ilu.iluDeleteImage(i)
                Gl.glFinish()
            Next
        Catch ex As Exception
            MsgBox("Crash Deleteing Textures!", MsgBoxStyle.Exclamation, "Opps..")
            End
        End Try
        Gl.glFinish()
        Gl.glFinish()

        make_locations()
        team_setup_selected_tank = ""
        tankID = -1

        For i = 0 To maplist.Length - 2
            maplist(i) = New grid_sec
            'maplist(i).bmap.Dispose()
            ReDim maplist(i).cdata(0)
        Next
        ReDim map_layers(0)
        map_layers(0) = New layer_
        ReDim tree_textures(0)
        tree_textures(0) = New tree_textures_
        ReDim decal_cache(0)
        decal_cache(0) = New decal_
        ReDim texture_cache(0)
        texture_cache(0) = New tree_textures_
        ReDim normalMap_layer_cache(0)
        normalMap_layer_cache(0) = New tree_textures_
        ReDim map_layer_cache(0)
        map_layer_cache(0) = New tree_textures_
        ReDim treeCache(0)
        treeCache(0) = New flora_
        ReDim maplist(0)
        maplist(0) = New grid_sec
        xDoc = New XmlDocument
        water.displayID = -1
        water.textureID = -1
        ringDisplayID_1 = -1
        ringDisplayID_2 = -1
        loaded_models = New Loaded_Model_list
        ReDim loaded_models.stack(0)
        loaded_models.stack(0) = New mdl_stack
        Models = New model_
        ReDim Model_Matrix_list(0)
        ReDim decal_matrix_list(0)
        ReDim speedtree_matrix_list(0)
        Model_Matrix_list(0) = New model_matrix_list_
        decal_matrix_list(0) = New decal_matrix_list_
        speedtree_matrix_list(0) = New speedtree_matrix_list_
        Try
            frmTanks.make_btns()

        Catch ex As Exception
            'MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Oops")
        End Try
        'Catch ex As Exception

        'End Try

    End Sub

    Public Sub resetBoundingBox()
        x_max = -10000
        x_min = 10000
        y_min = 10000
        y_max = -10000
        z_max = -10000
        z_min = 10000

    End Sub


    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Not _STARTED Then Return
        draw_scene()
    End Sub


    Private Sub ambient_tb_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Not _STARTED Then Return
        draw_scene()

    End Sub

    Private Sub show_Ids_cb_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub show_grids_cb_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Not _STARTED Then Return
        draw_scene()
    End Sub



    Private Sub show_water_cb_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub show_sectors_cb_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub show_cursor_cb_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub show_models_cb_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub show_trees_cb_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Not _STARTED Then Return
        draw_scene()
    End Sub


    Private Sub edit_mode_cb_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'If Not maploaded Then
        '    sender.checked = False
        '    Return
        'End If
        If m_layout_mode.Checked Then
            m_reset_tanks.Visible = True
            frmTanks.Show()

        Else
            m_reset_tanks.Visible = False
            frmTanks.Close()
        End If
    End Sub

    Private Sub m_load_map_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_load_map.Click
        m_comment.Text = ""
        m_comment.Visible = False
        SHOW_MAPS = True
        gl_pick_map(0, 0)
        gl_pick_map(0, 0)
        ' map_holder.Visible = True
        ' map_holder.Focus()
    End Sub

    Private Sub m_show_models_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_show_models.CheckedChanged
        If Not _STARTED Then
            Return
        End If
        draw_scene()
    End Sub

    Private Sub m_Ambient_level_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub m_show_trees_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_show_trees.CheckedChanged
        If Not _STARTED Then
            Return
        End If
        draw_scene()
    End Sub

    Private Sub m_show_water_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_show_water.CheckedChanged
        If Not _STARTED Then
            Return
        End If
        draw_scene()
    End Sub

    Private Sub m_show_cursor_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_show_cursor.CheckedChanged
        If Not _STARTED Then
            Return
        End If
        draw_scene()
    End Sub

    Private Sub m_show_map_grid_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_show_map_grid.CheckedChanged
        If Not _STARTED Then
            Return
        End If
        draw_scene()

    End Sub

    Private Sub m_show_chunks_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_show_chunks.CheckedChanged
        If Not _STARTED Then
            Return
        End If
        draw_scene()

    End Sub

    Private Sub m_lighting_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_lighting.Click
        If Not maploaded Then Return
        frmLighting.Visible = True
        get_light_settings()
    End Sub

    Private Sub HelpToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HelpToolStripMenuItem.Click
        frmHelp.Show()
    End Sub

    Private Sub m_low_quality_trees_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_low_quality_trees.Click
        If Not _STARTED Then
            Return
        End If
        draw_scene()
    End Sub

    Private Sub m_reset_tanks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_reset_tanks.Click
        frmTanks.TopMost = False
        If MsgBox("Are you sure?", MsgBoxStyle.YesNo, "Reset Assignments.") = MsgBoxResult.Yes Then
            frmTanks.TopMost = True
            make_locations()
            team_setup_selected_tank = ""
            If ImHost And Not frmClient.server_dead Then
                frmClient.resetClients()
            End If
            tankID = -1
            frmTanks.make_btns()
            'draw_minimap()
            draw_scene()
        End If
    End Sub

    Private Sub m_show_fog_Click(sender As Object, e As EventArgs) Handles m_show_fog.Click
        If Not _STARTED Then
            Return
        End If
        draw_scene()
    End Sub

    Private Sub m_host_session_Click(sender As Object, e As EventArgs) Handles m_host_session.Click
        ImHost = True
        frmServer.Visible = True
        m_host_session.Enabled = False
        m_join_session.Enabled = False
        m_join_server_as_host.Enabled = False
    End Sub

    Private Sub m_join_session_Click(sender As Object, e As EventArgs) Handles m_join_session.Click
        ImHost = False
        m_join_session.Enabled = False
        m_host_session.Enabled = False
        m_join_server_as_host.Enabled = False
        frmClient.Visible = True
        frmClient.echo_window_tb.Text = ""
        frmClient.diag_tb.Text = ""
        frmClient.TopMost = True
        m_show_chat.Visible = True
        m_show_chat.Checked = True
        m_show_chat.Text = "Hide Chat"

    End Sub

    Private Sub m_set_path_Click(sender As Object, e As EventArgs) Handles m_set_path.Click
        If FolderBrowserDialog1.ShowDialog(Me) = Forms.DialogResult.OK Then
            GAME_PATH = FolderBrowserDialog1.SelectedPath
            My.Settings.game_path = GAME_PATH
            My.Settings.Save()
        End If
    End Sub

    Private Sub m_save_Click(sender As Object, e As EventArgs) Handles m_save.Click
        If Not maploaded Then
            MsgBox("Nothing to save.", MsgBoxStyle.Exclamation, "load a map. place some tanks.")
            Return
        End If
        If SaveFileDialog1.ShowDialog(Me) = Forms.DialogResult.OK Then
            Dim s As String = SaveFileDialog1.FileName
            s = s.Replace(".tra", "")
            Dim fs As New FileStream(s + ".tra", FileMode.OpenOrCreate, FileAccess.Write)
            Dim bw As New BinaryWriter(fs)
            Dim btn As New Button
            bw.Write(load_map_name)
            For i = 0 To 14
                btn = frmTanks.SplitContainer1.Panel1.Controls(i)
                bw.Write(locations.team_1(i).comment)
                bw.Write(locations.team_1(i).id)    ' this is a string
                bw.Write(locations.team_1(i).loc_x)
                bw.Write(locations.team_1(i).loc_z)
                bw.Write(locations.team_1(i).rot_y)
            Next
            For i = 0 To 14
                btn = frmTanks.SplitContainer1.Panel2.Controls(i)
                bw.Write(locations.team_2(i).comment)
                bw.Write(locations.team_2(i).id)    ' this is a string
                bw.Write(locations.team_2(i).loc_x)
                bw.Write(locations.team_2(i).loc_z)
                bw.Write(locations.team_2(i).rot_y)
            Next
            fs.Close()
            bw.Close()

        End If
    End Sub

    Private Sub m_load_Click(sender As Object, e As EventArgs) Handles m_load.Click
        If OpenFileDialog1.ShowDialog(Me) = Forms.DialogResult.OK Then
            maploaded = False
            m_comment.Text = ""
            m_comment.Visible = False
            flush_data()
            Dim fs As New FileStream(OpenFileDialog1.FileName, FileMode.Open, FileAccess.Read)
            Dim br As New BinaryReader(fs)
            Dim btn As New Button
            frmTanks.Visible = True
            While Not frmTanks.Visible

            End While
            load_map_name = br.ReadString
            tankID = -1
            Dim ma = load_map_name.Split(".")
            For Each n In loadmaplist
                If n.name.Contains(ma(0)) Then
                    Me.Text = n.realname
                    Exit For
                End If
            Next
            JUST_MAP_NAME = Path.GetFileNameWithoutExtension(OpenFileDialog1.FileName)
            open_pkg(load_map_name)
            Dim txt As String = ""
            For i = 0 To 29
                uTank.comment = br.ReadString
                uTank.id = br.ReadString
                uTank.loc_x = br.ReadSingle
                uTank.loc_z = br.ReadSingle
                uTank.rot_y = br.ReadSingle
                net_button_update()
                Application.DoEvents()
            Next
            fs.Close()
            br.Close()
            br.Dispose()
            tankID = -1
            Packet_out.tankId = -1

            m_reset_tanks.Visible = False
            frmTanks.Visible = False
            position_camera()
            draw_scene()
        End If


    End Sub
    Public Sub net_button_update()
        If uTank.id.Length = 0 Then
            Return
        End If
        Dim Id, team, b_index As Integer
        Dim ar = uTank.id.Split("_")
        If ar.Length > 2 Then
            team = CInt(ar(0))
            Id = CInt(ar(2))
            b_index = CInt(ar(3))
        Else
            Return ' there is no data for this button
        End If
        If team = 1 Then
            If locations.team_1(b_index).id = uTank.id Then
                Return ' we dont want to update tanks that are already updated
            End If
            frmTanks.SplitContainer1.Panel1.Controls(b_index).Font = _
                            New Font(pfc.Families(0), 6, System.Drawing.FontStyle.Regular)
            frmTanks.SplitContainer1.Panel1.Controls(b_index).BackColor = Color.DarkRed
            locations.team_1(b_index).loc_x = uTank.loc_x
            locations.team_1(b_index).loc_z = uTank.loc_z
            locations.team_1(b_index).rot_y = uTank.rot_y
            locations.team_1(b_index).comment = uTank.comment
            Select Case ar(1)
                Case "A"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                            = a_tanks(Id).image
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = a_tanks(Id).gui_string
                    locations.team_1(b_index).name = a_tanks(Id).gui_string
                    get_tank(a_tanks(Id).file_name, locations.team_1(b_index))
                    locations.team_1(b_index).id = team.ToString + "_A_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = _
                        team.ToString + "_A_" + Id.ToString & "_" & b_index.ToString
                    locations.team_1(b_index).type = a_tanks(Id).sortorder
                Case "R"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                    = r_tanks(Id).image
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = r_tanks(Id).gui_string
                    locations.team_1(b_index).name = r_tanks(Id).gui_string
                    get_tank(r_tanks(Id).file_name, locations.team_1(b_index))
                    locations.team_1(b_index).id = team.ToString + "_R_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_R_" + Id.ToString & "_" & b_index.ToString
                    locations.team_1(b_index).type = r_tanks(Id).sortorder
                Case "G"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                    = g_tanks(Id).image
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = g_tanks(Id).gui_string
                    locations.team_1(b_index).name = g_tanks(Id).gui_string
                    get_tank(g_tanks(Id).file_name, locations.team_1(b_index))
                    locations.team_1(b_index).id = team.ToString + "_G_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_G_" + Id.ToString & "_" & b_index.ToString
                    locations.team_1(b_index).type = g_tanks(Id).sortorder
                Case "B"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                    = b_tanks(Id).image
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = b_tanks(Id).gui_string
                    locations.team_1(b_index).name = b_tanks(Id).gui_string
                    get_tank(b_tanks(Id).file_name, locations.team_1(b_index))
                    locations.team_1(b_index).id = team.ToString + "_B_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_B_" + Id.ToString & "_" & b_index.ToString
                    locations.team_1(b_index).type = b_tanks(Id).sortorder
                Case "F"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                    = f_tanks(Id).image
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = f_tanks(Id).gui_string
                    locations.team_1(b_index).name = f_tanks(Id).gui_string
                    get_tank(f_tanks(Id).file_name, locations.team_1(b_index))
                    locations.team_1(b_index).id = team.ToString + "_F_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_F_" + Id.ToString & "_" & b_index.ToString
                    locations.team_1(b_index).type = f_tanks(Id).sortorder
                Case "C"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                    = c_tanks(Id).image
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = c_tanks(Id).gui_string
                    locations.team_1(b_index).name = c_tanks(Id).gui_string
                    get_tank(c_tanks(Id).file_name, locations.team_1(b_index))
                    locations.team_1(b_index).id = team.ToString + "_C_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_C_" + Id.ToString & "_" & b_index.ToString
                    locations.team_1(b_index).type = c_tanks(Id).sortorder
                Case "J"
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).BackgroundImage _
                    = j_tanks(Id).image
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Text = j_tanks(Id).gui_string
                    locations.team_1(b_index).name = j_tanks(Id).gui_string
                    get_tank(j_tanks(Id).file_name, locations.team_1(b_index))
                    locations.team_1(b_index).id = team.ToString + "_J_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel1.Controls(b_index).Tag = team.ToString + "_J_" + Id.ToString & "_" & b_index.ToString
                    locations.team_1(b_index).type = j_tanks(Id).sortorder
            End Select
        Else
            If locations.team_2(b_index).id = uTank.id Then
                Return ' we dont want to update tanks that are already updated
            End If
            frmTanks.SplitContainer1.Panel2.Controls(b_index).Font = _
                    New Font(pfc.Families(0), 6, System.Drawing.FontStyle.Regular)
            frmTanks.SplitContainer1.Panel2.Controls(b_index).BackColor = Color.Green
            locations.team_2(b_index).loc_x = uTank.loc_x
            locations.team_2(b_index).loc_z = uTank.loc_z
            locations.team_2(b_index).rot_y = uTank.rot_y
            locations.team_2(b_index).comment = uTank.comment
            Select Case ar(1)
                Case "A"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = a_tanks(Id).image
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = a_tanks(Id).gui_string
                    locations.team_2(b_index).name = a_tanks(Id).gui_string
                    get_tank(a_tanks(Id).file_name, locations.team_2(b_index))
                    locations.team_2(b_index).id = team.ToString + "_A_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_A_" + Id.ToString & "_" & b_index.ToString
                    locations.team_2(b_index).type = a_tanks(Id).sortorder
                Case "R"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = r_tanks(Id).image
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = r_tanks(Id).gui_string
                    locations.team_2(b_index).name = r_tanks(Id).gui_string
                    get_tank(r_tanks(Id).file_name, locations.team_2(b_index))
                    locations.team_2(b_index).id = team.ToString + "_R_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_R_" + Id.ToString & "_" & b_index.ToString
                    locations.team_2(b_index).type = r_tanks(Id).sortorder
                Case "G"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = g_tanks(Id).image
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = g_tanks(Id).gui_string
                    locations.team_2(b_index).name = g_tanks(Id).gui_string
                    get_tank(g_tanks(Id).file_name, locations.team_2(b_index))
                    locations.team_2(b_index).id = team.ToString + "_G_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_G_" + Id.ToString & "_" & b_index.ToString
                    locations.team_2(b_index).type = g_tanks(Id).sortorder
                Case "B"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = b_tanks(Id).image
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = b_tanks(Id).gui_string
                    locations.team_2(b_index).name = b_tanks(Id).gui_string
                    get_tank(b_tanks(Id).file_name, locations.team_2(b_index))
                    locations.team_2(b_index).id = team.ToString + "_B_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_B_" + Id.ToString & "_" & b_index.ToString
                    locations.team_2(b_index).type = b_tanks(Id).sortorder
                Case "F"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = f_tanks(Id).image
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = f_tanks(Id).gui_string
                    locations.team_2(b_index).name = f_tanks(Id).gui_string
                    get_tank(f_tanks(Id).file_name, locations.team_2(b_index))
                    locations.team_2(b_index).id = team.ToString + "_F_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_F_" + Id.ToString & "_" & b_index.ToString
                    locations.team_2(b_index).type = f_tanks(Id).sortorder
                Case "C"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = c_tanks(Id).image
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = c_tanks(Id).gui_string
                    locations.team_2(b_index).name = c_tanks(Id).gui_string
                    get_tank(c_tanks(Id).file_name, locations.team_2(b_index))
                    locations.team_2(b_index).id = team.ToString + "_C_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_C_" + Id.ToString & "_" & b_index.ToString
                    locations.team_2(b_index).type = c_tanks(Id).sortorder
                Case "J"
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).BackgroundImage _
                    = j_tanks(Id).image
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Text = j_tanks(Id).gui_string
                    locations.team_2(b_index).name = j_tanks(Id).gui_string
                    get_tank(j_tanks(Id).file_name, locations.team_2(b_index))
                    locations.team_2(b_index).id = team.ToString + "_J_" + Id.ToString & "_" & b_index.ToString
                    frmTanks.SplitContainer1.Panel2.Controls(b_index).Tag = team.ToString + "_J_" + Id.ToString & "_" & b_index.ToString
                    locations.team_2(b_index).type = j_tanks(Id).sortorder
            End Select

        End If
    End Sub

    Private Sub m_show_chat_Click(sender As Object, e As EventArgs) Handles m_show_chat.Click
        If m_show_chat.Checked Then
            m_show_chat.Text = "Hide Chat"
            frmClient.Visible = True
        Else
            m_show_chat.Text = "Show Chat"
            frmClient.Visible = False
        End If
    End Sub

    Private Sub m_info_window_Click(sender As Object, e As EventArgs) Handles m_info_window.Click
        If m_info_window.Checked Then
            tb1.Visible = True
            pb1.Height = Me.ClientSize.Height - tb1.Height - mainMenu.Height - 2
            pb1.Width = Me.ClientSize.Width
            pb1.Location = New Point(0, mainMenu.Height + 1)
            draw_scene()
        Else
            tb1.Visible = False
            pb1.Height = Me.ClientSize.Height - mainMenu.Height
            pb1.Width = Me.ClientSize.Width
            pb1.Location = New Point(0, mainMenu.Height + 1)
            draw_scene()
        End If
    End Sub

    Private Sub m_show_chuckIds_Click(sender As Object, e As EventArgs) Handles m_show_chuckIds.Click
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub m_map_border_Click(sender As Object, e As EventArgs) Handles m_map_border.Click
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub m_fly_map_CheckedChanged(sender As Object, e As EventArgs) Handles m_fly_map.CheckedChanged
        Cam_X_angle += angle_offset
        'If m_fly_map.Checked Then
        '	Timer2.Enabled = True
        'Else
        '	angle_offset = 0
        '	If Not m_Orbit_Light.Checked Then
        '		Timer2.Enabled = False
        '	End If
        'End If
        If m_fly_map.Checked Then
            fly()
        End If
    End Sub


    Private Sub m_join_server_as_host_Click(sender As Object, e As EventArgs) Handles m_join_server_as_host.Click
        ImHost = True
        m_host_session.Enabled = False
        m_join_session.Enabled = False
        m_join_server_as_host.Enabled = False
        If ImHost Then
            frmClient.Visible = True
            frmClient.imHost_cb.Checked = True
            m_show_chat.Visible = True
            m_show_chat.Checked = True
            m_show_chat.Text = "Hide Chat"
        Else
            'better never be here connecting as a client ;)
        End If

    End Sub

    Private Sub m_show_tank_names_Click(sender As Object, e As EventArgs) Handles m_show_tank_names.Click
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub m_comment_DragDrop(sender As Object, e As Forms.DragEventArgs) Handles m_comment.DragDrop
        m_comment.Text = e.Data.GetData(System.Windows.Forms.DataFormats.Text)
    End Sub

    Private Sub m_comment_DragEnter(sender As Object, e As Forms.DragEventArgs) Handles m_comment.DragEnter
        e.Effect = Forms.DragDropEffects.Copy

    End Sub

    Private Sub m_comment_KeyDown(sender As Object, e As KeyEventArgs) Handles m_comment.KeyDown
        If e.KeyCode = Keys.Enter Then
            Return
        End If
    End Sub

    Private Sub m_comment_TextChanged(sender As Object, e As EventArgs) Handles m_comment.TextChanged
        If maploaded And tankID > -1 Then
            If tankID < 100 Then
                locations.team_1(tankID).comment = m_comment.Text
                Packet_out.comment = m_comment.Text
            Else
                locations.team_2(tankID - 100).comment = m_comment.Text
                Packet_out.comment = m_comment.Text
            End If
            draw_scene()
        End If
    End Sub

    Private Sub m_show_tank_comments_Click(sender As Object, e As EventArgs) Handles m_show_tank_comments.Click
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub m_comment_Click(sender As Object, e As EventArgs) Handles m_comment.Click

    End Sub

    Private Sub m_comment_MouseEnter(sender As Object, e As EventArgs) Handles m_comment.MouseEnter
        m_comment.Focus()
    End Sub

    Private Sub m_render_to_bitmap_Click(sender As Object, e As EventArgs) Handles m_render_to_bitmap.Click
        If Not maploaded Then
            Return
        End If
        FrmRenderMap.Visible = True
    End Sub

    Private Sub Find_Item_menu_Click(sender As Object, e As EventArgs) Handles m_find_Item_menu.Click
        frmFind.Show()
        frmFind.TopMost = True
    End Sub

    <DllImport("user32.dll")> _
    Private Shared Function GetDC(ByVal hWnd As IntPtr) As IntPtr

    End Function

    <DllImport("user32.dll")> _
    Private Shared Function ReleaseDC(ByVal hWnd As IntPtr, ByVal hDC As IntPtr) As Integer
    End Function
    <DllImport("gdi32.dll")> _
    Private Shared Function GetPixel(ByVal hDC As IntPtr, ByVal x As Integer, ByVal y As Integer) As UInteger

    End Function
    <DllImport("gdi32.dll")> _
    Private Shared Function SetPixel(ByVal hDC As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal color As Integer) As Integer
    End Function

    Private Sub pb2_MouseEnter(sender As Object, e As EventArgs) Handles pb2.MouseEnter
        pb2.Focus()
    End Sub
    Private Sub pb2_MouseLeave(sender As Object, e As EventArgs) Handles pb2.MouseLeave

    End Sub

    Private Sub pb2_MouseMove(sender As Object, e As MouseEventArgs) Handles pb2.MouseMove
        If pb2.Parent Is frmShowImage Then
            Dim loc As New Point
            Dim Csize = Cursor.Size
            Dim HS As New Point(0, -16)
            loc = pb2.PointToClient(Cursor.Position)
            loc.Y = loc.Y - (Csize.Height / 2) + 8
            loc.Y = loc.Y - (Csize.Height / 2)
            Dim g = GetDC(pb2.Handle)
            Dim c As Color
            Try
                c = frmShowImage.t_bmp.GetPixel(loc.X, loc.Y)
                'frmShowImage.TextBox1.BackColor = c
            Catch ex As Exception

            End Try
            Dim cr, cg, cb, ca As Integer
            ca = c.A
            cr = c.R
            cg = c.G
            cb = c.B
            frmShowImage.TextBox1.Text = _
                "R:" + cr.ToString + "  G:" + cg.ToString + "   B:" + cb.ToString + "   A:" + ca.ToString
            'frmShowImage.TextBox1.Text = "X:" + loc.X.ToString + "   Y:" + loc.Y.ToString
            ReleaseDC(pb2.Handle, g)
            Application.DoEvents()
        End If
    End Sub


    Private Sub ToolStripMenuItem15_Click(sender As Object, e As EventArgs) Handles m_edit_shaders.Click
        If Not _STARTED Then
            Return
        End If
        If Not maploaded Then
            Return
        End If

        frmEditFrag.Show()

    End Sub

    Private Sub m_high_rez_Terrain_CheckedChanged(sender As Object, e As EventArgs) Handles m_high_rez_Terrain.CheckedChanged
        If Not _STARTED Then
            Return
        End If
        If m_high_rez_Terrain.Checked Then
            If Not hz_loaded Then
                MsgBox("You will need to load a map!" + vbCrLf + "Maps are not loaded as high rez unless this is checked first.", MsgBoxStyle.Critical, "Reload Map.")

            End If
        End If
        My.Settings.hi_rez_terra = m_high_rez_Terrain.Checked
        draw_scene()
    End Sub

    Private Sub m_BumpMap_CheckedChanged(sender As Object, e As EventArgs)
        If Not _STARTED Then
            Return
        End If
        My.Settings.m_bumpMap = m_high_rez_Terrain.Checked

    End Sub
    Private Sub sun_lock_update()
        If sun_lock Then
            old_look_point.x = u_look_point_X
            old_look_point.y = u_look_point_Y
            old_look_point.z = u_look_point_Z
        Else
            look_point_X = old_look_point.x
            look_point_Y = old_look_point.y
            look_point_Z = old_look_point.z
        End If
    End Sub
    Public Function need_update() As Boolean
        'This updates the display if the mouse has changed the view angles, locations or distance.
        Dim update As Boolean = False
        If u_y_offset <> y_offset Then
            u_y_offset = y_offset
            update = True
        End If
        If y_offset < 0 Then
            y_offset = 0
            u_y_offset = 0
            update = True
        End If
        If look_point_X <> u_look_point_X Then
            u_look_point_X = look_point_X
            update = True
        End If
        If look_point_Y <> u_look_point_Y Then
            u_look_point_Y = look_point_Y
            update = True
        End If
        If look_point_Z <> u_look_point_Z Then
            u_look_point_Z = look_point_Z
            update = True
        End If
        If Cam_X_angle <> u_Cam_X_angle Then
            u_Cam_X_angle = Cam_X_angle
            update = True
        End If
        If Cam_Y_angle <> u_Cam_Y_angle Then
            u_Cam_Y_angle = Cam_Y_angle
            update = True
        End If
        If View_Radius <> u_View_Radius Then
            u_View_Radius = View_Radius
            update = True
        End If
        If ROTATE_TANK Then
            If tankID > -1 Then
                If tankID >= 100 Then
                    If u_tank_r <> locations.team_2(tankID - 100).rot_y Then
                        u_tank_r = locations.team_2(tankID - 100).rot_y
                        update = True
                    End If
                Else
                    If u_tank_r <> locations.team_1(tankID).rot_y Then
                        u_tank_r = locations.team_1(tankID).rot_y
                        update = True
                    End If
                End If
            End If
        End If
        If Not ROTATE_TANK Then
            If MOVE_TANK Then
                If tankID > -1 Then
                    If tankID > -1 Then
                        If tankID >= 100 Then
                            If u_tank_x <> locations.team_2(tankID - 100).loc_x Then
                                u_tank_x = locations.team_2(tankID - 100).loc_x
                                update = True
                            End If
                            If u_tank_z <> locations.team_2(tankID - 100).loc_z Then
                                u_tank_z = locations.team_2(tankID - 100).loc_z
                                update = True
                            End If
                        Else
                            If u_tank_x <> locations.team_1(tankID).loc_x Then
                                u_tank_x = locations.team_1(tankID).loc_x
                                update = True
                            End If
                            If u_tank_z <> locations.team_1(tankID).loc_z Then
                                u_tank_z = locations.team_1(tankID).loc_z
                                update = True
                            End If
                        End If
                    End If
                End If
            End If
        End If
        Return update
    End Function
    Private Sub update_mouse()
        'This will run for the duration that Terra! is open.
        'Its in a closed loop
        Dim swat As New Stopwatch
        While _STARTED
            If Not m_fly_map.Checked And maploaded Then
                angle_offset = 0
                If need_update() Then
                    'If we need to update the screen, lets caclulate draw times and update the timer.
                    If screen_avg_counter > 5 Then
                        screen_totaled_draw_time = screen_avg_draw_time / screen_avg_counter
                        screen_avg_counter = 0
                        screen_avg_draw_time = 0
                    Else
                        If Screen_draw_time < 15 Then
                            Screen_draw_time = 30
                        End If
                        screen_avg_counter += 1
                        screen_avg_draw_time += Screen_draw_time
                    End If
                    swat.Reset()
                    swat.Start()
                    update_screen()
                    Screen_draw_time = CInt(swat.ElapsedMilliseconds)
                    ' Thread.Sleep(5)
                End If
            End If
            If SHOW_MAPS Then
                'draw_maps_buttons()
                'Thread.Sleep(30)
            End If
            'Application.DoEvents()
            Thread.Sleep(5)
        End While
        'Thread.CurrentThread.Abort()
    End Sub

    Private Delegate Sub update_screen_delegate()
    Public Sub update_screen()
        Try
            If Me.InvokeRequired Then
                Me.Invoke(New update_screen_delegate(AddressOf update_screen))
            Else
                draw_scene()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Delegate Sub draw_maps_buttons_delegate()
    Public Sub draw_maps_buttons()
        Try
            If Me.InvokeRequired Then
                Me.Invoke(New draw_maps_buttons_delegate(AddressOf draw_maps_buttons))
            Else
                gl_pick_map(mouse.X, mouse.Y)
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub m_Orbit_Light_CheckedChanged(sender As Object, e As EventArgs) Handles m_Orbit_Light.CheckedChanged
        If m_Orbit_Light.Checked Then
            old_light_position.x = position(0)
            old_light_position.y = position(1)
            old_light_position.z = position(2)
            fly()
        Else
            'position(0) = old_light_position.x
            'position(1) = old_light_position.y
            'position(2) = old_light_position.z
        End If
    End Sub
    Public Sub fly()
        Dim scale As Single = MAP_BB_UR.x - 50.0
        Dim lx, ly, lz As Single
        While m_fly_map.Checked Or m_Orbit_Light.Checked

            'scale = 700.0
            If m_fly_map.Checked And maploaded Then
                FLY_ = True
                view_rot += 0.002
                look_point_X = Cos(view_rot) * scale
                look_point_Z = Sin(view_rot) * scale
                cam_x = Cos(view_rot - 0.01) * scale
                cam_z = Sin(view_rot - 0.01) * scale
                Z_Cursor = get_Z_at_XY(look_point_X, look_point_Z)  ' + 1
                cam_y = Z_Cursor + 15
                look_point_Y = cam_y
                angle_offset = -view_rot - (PI * 0.1)

                If screen_avg_counter > 10 Then
                    screen_totaled_draw_time = screen_avg_draw_time / screen_avg_counter
                    screen_avg_counter = 1
                    screen_avg_draw_time = Screen_draw_time
                Else
                    If Screen_draw_time < 5 Then
                        Screen_draw_time = 30
                    End If
                    screen_avg_counter += 1
                    screen_avg_draw_time += (Screen_draw_time)
                End If
                If need_update() Then
                    'swat1.Reset()
                    'swat1.Start()
                    draw_scene()
                    Screen_draw_time = CInt(swat1.ElapsedMilliseconds)
                End If
                If view_rot > 2 * PI Then
                    view_rot -= (2 * PI)
                End If
                Application.DoEvents()
            Else
                angle_offset = 0
            End If
            If m_Orbit_Light.Checked And maploaded Then

                'scale = 1000.0
                light_rot += 0.015
                lx = Cos(light_rot) * scale
                lz = Sin(light_rot) * scale
                ly = 120.0 'Z_Cursor = get_Z_at_XY(look_point_X, look_point_Z) ' + 1
                'ly = 320.0 'Z_Cursor = get_Z_at_XY(look_point_X, look_point_Z) ' + 1
                position(0) = lx 'u_look_point_X - lx
                position(1) = ly 'u_look_point_Y + 10 'ly
                position(2) = lz 'u_look_point_Z - lz
                position(3) = 1.0

                'Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position)
                If Not m_fly_map.Checked Then
                    If screen_avg_counter > 10 Then
                        screen_totaled_draw_time = screen_avg_draw_time / screen_avg_counter
                        screen_avg_counter = 1
                        screen_avg_draw_time = Screen_draw_time
                    Else
                        If Screen_draw_time < 5 Then
                            Screen_draw_time = 30
                        End If
                        screen_avg_counter += 1
                        screen_avg_draw_time += (Screen_draw_time)
                    End If
                    'swat2.Reset()
                    'swat2.Start()
                    need_update()

                    update_screen()

                    Screen_draw_time = CInt(swat1.ElapsedMilliseconds)
                End If
                If light_rot > 2 * PI Then
                    light_rot -= (2 * PI)
                End If
                Application.DoEvents()
            Else
                'position(0) = old_light_position.x
                'position(1) = old_light_position.y
                'position(2) = old_light_position.z

            End If
            Application.DoEvents()
        End While


    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Enabled = False
        update_thread.IsBackground = False
        update_thread.Name = "mouse updater"
        update_thread.Priority = ThreadPriority.Highest
        update_thread.Start()
    End Sub

    Private Sub m_hell_mode_CheckedChanged(sender As Object, e As EventArgs) Handles m_hell_mode.CheckedChanged
        If Not _STARTED Then
            Return
        End If
        If m_hell_mode.Checked Then
            m_Orbit_Light.Checked = True
        Else
            m_Orbit_Light.CheckState = False
            draw_scene()
        End If
    End Sub


    Private Sub m_developer_CheckedChanged(sender As Object, e As EventArgs) Handles m_developer.CheckedChanged
        'Warn user than show developer tools.
        If Not _STARTED Then
            Return
        End If
        If m_developer.Checked Then
            m_edit_shaders.Visible = True
            m_find_Item_menu.Visible = True
            m_edit_biasing.Visible = True
            m_post_effect_viewer.Visible = True
        Else
            m_edit_shaders.Visible = False
            m_find_Item_menu.Visible = False
            m_edit_biasing.Visible = False
            m_post_effect_viewer.Visible = False

        End If
    End Sub

    Private Sub m_bump_map_models_CheckedChanged(sender As Object, e As EventArgs) Handles m_bump_map_models.CheckedChanged
        If Not _STARTED Then
            Return
        End If
        If m_bump_map_models.Checked And Not model_bump_loaded Then
            MsgBox("There are no NormalMaps loaded!" + _
                      vbCrLf + "You will need to reload a map.", _
                        MsgBoxStyle.Exclamation, "No NormalMaps Loaded..")
        End If
        draw_scene()
    End Sub


    Private Sub m_clear_tank_comments_Click(sender As Object, e As EventArgs) Handles m_clear_tank_comments.Click
        If MsgBox("Are you sure you want to" + vbCrLf + _
                     "clear all tank comments?", MsgBoxStyle.YesNo, "Clear Comments.") = MsgBoxResult.No Then
            Return
        Else
            For i = 0 To 15
                locations.team_1(i).comment = ""
                locations.team_2(i).comment = ""
                m_comment.Text = ""
            Next
            draw_scene()
        End If
    End Sub

    Private Sub m_show_uv2_CheckedChanged(sender As Object, e As EventArgs) Handles m_show_uv2.CheckedChanged
        If Not _STARTED Then Return
        If m_show_uv2.Checked And Not uv2s_loaded Then
            MsgBox("There are no UV2 Textures loaded." + vbCrLf + "You will need to reload a map.", MsgBoxStyle.Exclamation, "No UV2 Textures..")
        End If
        My.Settings.m_show_uv2 = m_show_uv2.Checked
        draw_scene()
    End Sub

    Private Sub m_exit_Click(sender As Object, e As EventArgs) Handles m_exit.Click
        Me.Close()
    End Sub

    Private Sub m_show_minimap_Click(sender As Object, e As EventArgs) Handles m_show_minimap.Click
        If Not _STARTED Then Return
        m_minizoom.Checked = m_show_minimap.Checked
        draw_scene()
    End Sub

    Private Sub m_mini_up_Click(sender As Object, e As EventArgs) Handles m_mini_up.Click
        If Not _STARTED Then Return
        minimap_size += 32.0!
        If minimap_size > 640.0! Then
            minimap_size = 640.0!
        End If
        'tb1.Text = "Minimap size: " + minimap_size.ToString
        My.Settings.minimap_size = minimap_size
        draw_scene()

    End Sub

    Private Sub m_mini_down_Click(sender As Object, e As EventArgs) Handles m_mini_down.Click
        If Not _STARTED Then Return
        minimap_size -= 32.0!
        If minimap_size < 128.0! Then
            minimap_size = 128.0!
        End If
        'tb1.Text = "Minimap size: " + minimap_size.ToString
        My.Settings.minimap_size = minimap_size
        draw_scene()
    End Sub

    Private Sub m_minizoom_Click(sender As Object, e As EventArgs) Handles m_minizoom.Click
        If Not _STARTED Then Return
        m_show_minimap.Checked = m_minizoom.Checked
        draw_scene()
    End Sub

    Private Sub m_show_status_Click(sender As Object, e As EventArgs) Handles m_show_status.Click
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub m_wire_decals_CheckedChanged(sender As Object, e As EventArgs) Handles m_wire_decals.CheckedChanged
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub m_wire_terrain_CheckedChanged(sender As Object, e As EventArgs) Handles m_wire_terrain.CheckedChanged
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub m_wire_trees_CheckedChanged(sender As Object, e As EventArgs) Handles m_wire_trees.CheckedChanged
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub m_wire_models_CheckedChanged(sender As Object, e As EventArgs) Handles m_wire_models.CheckedChanged
        If Not _STARTED Then Return
        draw_scene()
    End Sub

    Private Sub m_show_decals_CheckedChanged(sender As Object, e As EventArgs) Handles m_show_decals.CheckedChanged
        If Not _STARTED Then Return
        draw_scene()
    End Sub


    Private Sub m_edit_biasing_Click(sender As Object, e As EventArgs) Handles m_edit_biasing.Click
        If Not frmBiasing.Visible Then
            frmBiasing.Show()
        End If
    End Sub

    Private Sub m_post_effect_viewer_Click(sender As Object, e As EventArgs) Handles m_post_effect_viewer.Click
        frmTestView.Visible = True
    End Sub

    Private Sub m_render_stats_Click(sender As Object, e As EventArgs) Handles m_render_stats.Click
        frmStats.Show()
    End Sub


End Class

