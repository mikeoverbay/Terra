﻿#Region "Imports"

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
Imports System.Windows.Media.Media3D
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
    Dim bias() As Single = {0.5, 0.0, 0.0, 0.0, 0.0, 0.5, 0.0, 0.0, 0.0, 0.0, 0.5, 0.0, 0.5, 0.5, 0.5, 1.0}
    Dim autoEventScreen As New Threading.AutoResetEvent(False)
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
    Public update_thread As New Thread(AddressOf screen_updater)
    Public mouse_update_thread As New Thread(AddressOf check_mouse)
    'Public M_FLY As Boolean = False
    Public Shared d_counter As Integer = 0
    Dim clip_distance As Integer
    Dim M_current As New vect2
    Public pb2_mouse_down As Boolean = False
    Private welcome_screen As Integer
    Private old_window_state As Integer
    Private old_window_size As New Point(0, 0)
#End Region

    Private Sub frmMain_ClientSizeChanged(sender As Object, e As EventArgs) Handles Me.ClientSizeChanged
        If Me.WindowState = FormWindowState.Maximized Then
            If _STARTED Then
                G_Buffer.init()
            End If
            update_screen()
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim nonInvariantCulture As CultureInfo = New CultureInfo("en-US")
        nonInvariantCulture.NumberFormat.NumberDecimalSeparator = "."
        Thread.CurrentThread.CurrentCulture = nonInvariantCulture
        frmSplash.Show()
        set_frmLoadOptions()

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
        gl_busy = False
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
            Application.Restart()
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
        'pb4.Parent = FrmInfoWindow
        pb2.Parent = Me
        pb4.Visible = False
        pb2.Visible = False
        Me.Show()
        Me.Update()
        _STARTED = True

        icon_scale = My.Settings.icon_scale

        m_find_Item_menu.Visible = False
        m_edit_shaders.Visible = False

        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Me.Update()
        set_menu_strip_checkboxes()

        'map_holder.Visible = True
        'frmTanks.Show()
        draw_scene()
        draw_scene()
        Application.DoEvents()
        Application.DoEvents()
        FrmInfoWindow.Visible = True
        position_info_window()
        tb1.Visible = True
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        '-------------------------------------------------
        ' This has to be before any textures are created.
        'It sets the first texture to start deleting from!
        flush_data()
        '-------------------------------------------------
        get_tank_list() ' get the tanks and add them to the GUI

        tb1.Text = "Getting Map Images"
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Packet_in.tankId = -1
        Packet_out.tankId = -1
        Packet_in.comment = ""
        Packet_out.comment = ""

        m_comment.AllowDrop = True
        frmTanks.Hide()
        make_locations() ' setup tank location data and clear displaylist
        make_map_buttons()
        tb1.text = "Welcome to Terra!"
        Application.DoEvents()
        'open up our huge virual file for access.
        triangle_holder.open((15 * 15) * (4096 * 6))
        old_window_state = Me.WindowState
        old_window_size.X = Me.Width
        old_window_size.Y = Me.Height
        Timer1.Enabled = True
        frmSplash.Close()
        testing = True
        'frmDebug.Show()
        ' frmChat.Show()
        SHOW_MAPS = True
    End Sub

    Private Sub set_frmLoadOptions()
        m_terrain_ = My.Settings.m_load_terrain
        m_trees_ = My.Settings.m_load_trees
        m_models_ = My.Settings.m_load_models
        m_decals_ = My.Settings.m_load_decals
        m_water_ = My.Settings.m_water
        m_sky_ = My.Settings.m_sky
        m_bases_ = My.Settings.m_bases

    End Sub


    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
      
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
        If e.KeyCode = Keys.F Then
          
        End If
        If e.KeyCode = Keys.S Then
            d_counter += 1
            If d_counter > decal_matrix_list.Length - 1 Then
                d_counter = 0
            End If
            draw_scene()
        End If
        If e.KeyCode = Keys.Q Then
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
            Application.DoEvents()
            Application.DoEvents()
        End If
        If e.KeyCode = Keys.F2 Then
            m_show_status.PerformClick()
            Application.DoEvents()
            Application.DoEvents()
        End If
        If e.KeyCode = Keys.F3 Then
            m_Orbit_Light.PerformClick()
        End If
        If e.KeyCode = Keys.F4 Then
            m_fly_map.PerformClick()
        End If
        If e.KeyCode = Keys.F5 And m_small_lights.Checked Then
            find_street_lights()
            update_screen()
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
     

        npb.Update()
        Application.DoEvents()
        If _STARTED Then
            'make_post_FBO_and_Textures()
        End If
        If old_window_state <> Me.WindowState Then
            old_window_state = Me.WindowState
            If _STARTED Then
                If Me.WindowState <> FormWindowState.Minimized Then
                    G_Buffer.init()
                    position_info_window()
                End If
            End If
        End If
        'If Me.WindowState = FormWindowState.Maximized Or Me.WindowState = FormWindowState.Normal Then
        'End If
        If Not SHOW_MAPS Then
            draw_scene()
        Else
            draw_maps()
        End If

    End Sub
    Private Sub frmMain_ResizeEnd(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ResizeEnd

        npb.Update()
        Application.DoEvents()
        If Me.Width <> old_window_size.X Or Me.Height <> old_window_size.Y Then
            old_window_size.X = Me.Width : old_window_size.Y = Me.Height
            If _STARTED Then
                G_Buffer.init()
            End If
        End If

        If Not SHOW_MAPS Then
            update_screen()
        Else
            draw_maps()
        End If
        Try
            If pb1.Parent.Name = Me.Name Then
                pb1_screen_location = pb1.PointToScreen(New System.Drawing.Point)
            End If
        Catch ex As Exception

        End Try
        position_info_window()

    End Sub
    Private Sub frmMain_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        npb.Invalidate()
    End Sub

    Private Sub position_info_window()
        Dim w = (Me.Width - Me.ClientSize.Width) / 2
        Dim l = -FrmInfoWindow.Height + ((Me.Location.Y + Me.Height) - w)
        Dim b = (Me.ClientSize.Width - FrmInfoWindow.Width) / 2
        FrmInfoWindow.Location = New Point(Me.Location.X + b + w, l)
    End Sub
    Private Sub set_menu_strip_checkboxes()

        m_low_quality_trees.Checked = My.Settings.low_q_trees
        m_load_lod.Checked = My.Settings.lod0
        m_load_details.Checked = My.Settings.load_extra
        m_low_quality_textures.Checked = My.Settings.txt_256
        m_high_rez_Terrain.Checked = My.Settings.hi_rez_terra
        m_show_uv2.Checked = My.Settings.m_show_uv2
        m_show_cursor.Checked = False
        'may as well set the levels here
        lighting_fog_level = My.Settings.s_fog_level / 10000.0!
        lighting_ambient = My.Settings.s_terrain_ambient_level / 300.0!
        lighting_terrain_texture = My.Settings.s_terrian_texture_level / 50.0!
        lighting_model_level = My.Settings.s_model_level / 100.0!
        gamma_level = (My.Settings.s_gamma / 100.0!) * 2.0!
    End Sub

    Public Sub make_locations()
        ReDim locations.team_1(15)
        ReDim locations.team_2(15)
        For u = 0 To 14
            locations.team_1(u) = New t_l
            If locations.team_1(u).tank_displaylist > 0 Then
                Gl.glDeleteLists(locations.team_1(u).tank_displaylist, 1)
            End If
            locations.team_1(u).tank_displaylist = -1
            locations.team_1(u).id = ""
            locations.team_1(u).comment = ""
            locations.team_1(u).name = ""

            locations.team_2(u) = New t_l
            If locations.team_2(u).tank_displaylist > 0 Then
                Gl.glDeleteLists(locations.team_2(u).tank_displaylist, 1)
            End If
            locations.team_2(u).tank_displaylist = -1
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
            loadmaplist(cnt).realname = a(1).Replace("Winter ", "Wtr ")
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
        Using Zip As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read(GAME_PATH & "\res\packages\gui.pkg")
            Dim ms As New MemoryStream
            Dim entry As Ionic.Zip.ZipEntry = Zip("gui\maps\bg.png")
            entry.Extract(ms)
            welcome_screen = load_png(ms)

        End Using
        GC.Collect()
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
        If Not maploaded Then Return 0
        Dim path = Application.StartupPath & "\tanks\" & name
        Dim f_ = System.IO.File.Open(path, FileMode.Open, FileAccess.Read)
        Dim v(3) As Single
        Dim n(3) As Single
        Dim uv(2) As Single
        Dim b As New BinaryReader(f_)
        Dim poly_count As UInt32 = b.ReadUInt32

        If tank.tank_displaylist > 0 Then
            Gl.glDeleteLists(tank.tank_displaylist, 1)
        End If
        Dim ID = Gl.glGenLists(1)
        Gl.glNewList(ID, Gl.GL_COMPILE)

        tank.tank_displaylist = ID
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
        ''outline tank
        'b.BaseStream.Position = 0
        'poly_count = b.ReadUInt32

        'If tank.track_displaylistAdj > 0 Then
        '    Gl.glDeleteLists(tank.track_displaylistAdj, 1)
        'End If
        'ID = Gl.glGenLists(1)
        'Gl.glNewList(ID, Gl.GL_COMPILE)

        'tank.track_displaylistAdj = ID
        'Gl.glBegin(Gl.GL_TRIANGLES_ADJACENCY_EXT)
        'cnt = 0 ' for debug
        ''start pushing vertices
        'For c As UInt32 = 0 To poly_count - 1

        '    v(0) = -b.ReadSingle
        '    v(1) = b.ReadSingle
        '    v(2) = b.ReadSingle
        '    n(0) = -b.ReadSingle
        '    n(1) = b.ReadSingle
        '    n(2) = b.ReadSingle
        '    'uv(0) = -b.ReadSingle
        '    'uv(1) = b.ReadSingle
        '    Gl.glNormal3fv(n)
        '    'Gl.glTexCoord2fv(uv)
        '    Gl.glVertex3fv(v)
        '    cnt += 1
        'Next
        'Gl.glEnd()
        'Gl.glEndList()
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
            update_screen()
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
            update_screen()
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

#Region "Screen Pick methods"

    Public Function GetOGLPos_trees(ByVal x As Integer, ByVal y As Integer) As Integer
        If Not m_trees_ Then
            Return False
        End If
        If frmTanks.Visible Then
            Return False
        End If
        seek_scene_trees()
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        Dim type = pixel(3)
        Dim index As UInt32 = (CUInt(pixel(1) * 256) + pixel(2))
        Dim cacheId = pixel(0) - 1
        If index > 0 And pixel(0) > 0 Then
            index = index - 1
            tb1.text = treeCache(cacheId).name
            Return True
        End If
        Application.DoEvents()
        tb1.text = "Nothing...."
        Application.DoEvents()
        Return False
    End Function
    Public Sub seek_scene_trees()

        'set_eyes()
        Dim red, blue, green As Byte
        If maploaded Then   ' cant let this try and draw shit that isnt there yet!!!
            If m_show_trees.Checked And m_trees_ Then
                For mode = 0 To 1
                    If mode = 0 Then
                        Gl.glUseProgram(0)
                    Else
                        Gl.glUseProgram(shader_list.leafcoloredDef_shader)
                    End If
                    For k = 0 To treeCache.Length - 1
                        With treeCache(k)

                            For i = 0 To treeCache(k).tree_cnt - 1
                                If treeCache(k).BB(i).visible Then

                                    red = k + 1
                                    green = CByte((i + 1 And &HFF00) >> 8)
                                    blue = CByte(i + 1 And &HFF)
                                    Gl.glColor4ub(CByte(red), CByte(green), CByte(blue), CByte(0))
                                    'Gl.glColor3f(0.6, 0.6, 0.6)
                                    Gl.glPushMatrix()
                                    Gl.glMultMatrixf(.matrix_list(i).matrix)
                                    If mode = 0 Then
                                        If .branch_displayID > 0 Then
                                            Gl.glCallList(.branch_displayID)
                                        End If
                                        If .frond_displayID > 0 Then
                                            Gl.glCallList(.frond_displayID)
                                        End If
                                    Else
                                        If .leaf_displayID > 0 Then
                                            Gl.glCallList(.leaf_displayID)
                                        End If
                                    End If
                                    Gl.glPopMatrix()
                                End If
                            Next
                        End With

                    Next
                Next
                Gl.glUseProgram(0)

            End If


        End If
        'Gdi.SwapBuffers(pb1_hDC)
        '        '--------------------------------------------------------------------------
        Gl.glFinish()
    End Sub

    Public Function GetOGLPos_Decals(ByVal x As Integer, ByVal y As Integer) As Integer
        If Not m_decals_ Then
            Return False
        End If
        If frmTanks.Visible Then
            Return False
        End If
        seek_scene_decals()
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

                    tb1.text = "Index:" + index.ToString("0000") + " " + decal_matrix_list(index).decal_texture + vbCrLf + "flags:" _
                        + decal_matrix_list(index).flags.ToString("X") + vbCrLf + "Influence:" + decal_matrix_list(index).influence.ToString("000")
                End With
                Application.DoEvents()
                Return True
            End If
        Else
        End If
        tb1.text = "Nothing...."
        Application.DoEvents()
        Return False
    End Function
    Public Sub seek_scene_decals()

        'set_eyes()

        Dim red, blue, green, type As Byte
        type = 0
        '        '---------------------------------
        '        ' draw the decal boxes with its own color
        '        '---------------------------------
        '        'if we are in team edit mode.we dont display any models
        If maploaded Then   ' cant let this try and draw shit that isnt there yet!!!
            If m_show_decals.Checked And m_decals_ Then
                'For model As UInt32 = 0 To Models.matrix.Length - 1
                '    Gl.glColor4ub(0, 0, 0, 0)
                '    For k = 0 To Models.models(model)._count - 1
                '        If Models.models(model).componets(k).callList_ID > 0 Then

                '            Gl.glPushMatrix()
                '            Gl.glMultMatrixf(Models.matrix(model).matrix)

                '            Gl.glCallList(Models.models(model).componets(k).callList_ID)
                '            Gl.glPopMatrix()
                '        End If
                '    Next
                'Next
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
    End Sub


    Public Sub GetOGLPos(ByVal x As Integer, ByVal y As Integer)
        timeout = 11
        Thread.Sleep(5)
        stopGL = True
        Application.DoEvents()
        autoEventScreen.Reset()
        While gl_busy
        End While
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        Gl.glReadBuffer(Gl.GL_BACK)
        Gl.glDrawBuffer(Gl.GL_BACK)
        Gl.glFinish()
        ResizeGL()
        ViewPerspective()   ' set 3d view mode

        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)

        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_MULTISAMPLE)
        Gl.glDisable(Gl.GL_FOG)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_ALPHA_TEST)
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 0.0F)

        Gl.glFinish()
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        If GetOGLPos_Decals(x, y) Then
            stopGL = False
            draw_scene()
            Return
        End If
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        If GetOGLPos_trees(x, y) Then
            stopGL = False
            draw_scene()
            Return
        End If
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        seek_scene()
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
                If index > 0 And index < Model_Matrix_list.Length Then
                    index = index - 1
                    _SELECTED_model = index + 1
                    tb1.text = index.ToString("0000") + " : " + Model_Matrix_list(index).primitive_name
                    Application.DoEvents()
                End If
            Else
                _SELECTED_model = 0
                tb1.text = "Nothing...."
                Application.DoEvents()
            End If
        End If
        stopGL = False
        draw_scene()
    End Sub
    Public Sub seek_scene()
        'set_eyes()
        Dim red, blue, green, type As Byte
        type = 100
        '        '---------------------------------
        '        ' draw the models.each with its own color
        '        '---------------------------------
        '        'if we are in team edit mode.we dont display any models
        If maploaded Then   ' cant let this try and draw shit that isnt there yet!!!
            If Not frmTanks.Visible Then
                If m_show_models.Checked And m_models_ Then

                    For model As UInt32 = 0 To Models.matrix.Length - 1
                        green = 0 : blue = 0 : type = 0
                        For k = 0 To Models.models(model)._count - 1
                            If Models.models(model).visible Then

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
                            End If
                        Next
                    Next
                End If

            Else
                '                ' cant let this try and draw shit that isnt there yet!!!
                Dim cv = 57.2957795
                For i = 0 To 14
                    If locations.team_1(i).tank_displaylist > -1 Then
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
                        Gl.glCallList(locations.team_1(i).tank_displaylist)
                        Gl.glPopMatrix()
                    End If
                Next
                For i = 0 To 14
                    If locations.team_2(i).tank_displaylist > -1 Then
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
                        Gl.glCallList(locations.team_2(i).tank_displaylist)
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


    Public Sub show_data(ByVal v1 As Vector3D, ByVal v2 As Vector3D, ByVal v3 As Vector3D, ByVal v4 As Vector3D)
        Dim out_string As String = ""
        out_string = vbCrLf
        out_string += "p1  " + String.Format("X{0,12:F4} Y{0,12:F4} Z{0,12:F4} ", v1.X, v1.Y, v1.Z) + vbCrLf
        out_string += "p2  " + String.Format("X{0,12:F4} Y{0,12:F4} Z{0,12:F4} ", v2.X, v2.Y, v2.Z) + vbCrLf
        out_string += "p3  " + String.Format("X{0,12:F4} Y{0,12:F4} Z{0,12:F4} ", v3.X, v3.Y, v3.Z) + vbCrLf
        out_string += "L   " + String.Format("X{0,12:F4} Y{0,12:F4} Z{0,12:F4} ", v4.X, v4.Y, v4.Z)
        Debug.Write(out_string)
    End Sub

    Public Sub draw_maps()
        If Not _STARTED Then Return
        ' If Not SHOW_MAPS Then Return
        Gl.glDisable(Gl.GL_FRAMEBUFFER_SRGB_EXT)
        ResizeGL()
        ViewOrtho()
        'gl_busy = True
        If stopGL Then
            Return
        End If
        If stopGL Then Return
        If stopGL Then Return
        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)


        Gl.glClearColor(0.125F, 0.125F, 0.125F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        'ResizeGL()
        Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
        Dim w = pb1.Width
        Dim h = pb1.Height
        If w = 0 Then
            Return
            Gl.glDisable(Gl.GL_DEPTH_TEST)
        End If
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, welcome_screen)

        Gl.glBegin(Gl.GL_QUADS)

        Gl.glTexCoord2f(0, -1)
        Gl.glVertex2d(0, 0)

        Gl.glTexCoord2f(1, -1)
        Gl.glVertex2d(w, 0)

        Gl.glTexCoord2f(1, 0)
        Gl.glVertex2d(w, -h)

        Gl.glTexCoord2f(0, 0)
        Gl.glVertex2d(0, -h)
        Gl.glEnd()
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)


        Dim ms_x As Single = 120
        Dim ms_y As Single = -72
        Dim space_x As Single = 15

        Dim w_cnt As Single = 7 'Floor(w / (ms_x + space_x))
        Dim space_cnt As Single = (w_cnt - 1) * space_x
        Dim border As Single = (w - ((w_cnt * ms_x) + space_cnt)) / 2
        Dim map As Integer = 0
        Dim v_cnt = (map_texture_ids.Length - 1) / w_cnt
        If (v_cnt * (ms_x + space_x)) + (border * 2) < pb1.Width Then
            v_cnt -= 1
        End If
        Dim v_pos As Integer = 0
        Dim vi, hi, sz As Single
        For i = 0 To map_texture_ids.Length - 2
            If loadmaplist(i).grow_shrink Then
                loadmaplist(i).delay_time += 1
                If loadmaplist(i).delay_time = 1 Then
                    loadmaplist(i).delay_time = 0
                    If loadmaplist(i).size = 0 Or loadmaplist(i).size = 20 Then
                        loadmaplist(i).grow_shrink = False
                    Else
                        loadmaplist(i).size += loadmaplist(i).direction
                    End If
                End If
            End If
        Next
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        vi = -30
        While map < map_texture_ids.Length - 2
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
                If selected_map_hit > 0 And map = selected_map_hit - 1 Then
                    loadmaplist(map).grow_shrink = False
                    GoTo dont_draw
                Else
                    loadmaplist(map).direction = -0.25
                    If loadmaplist(map).size > 0.5 Then
                        loadmaplist(map).grow_shrink = True
                        loadmaplist(map).direction = -0.25
                        If loadmaplist(map).size = 20 Then
                            loadmaplist(map).size = 19.75
                        End If
                    End If

                End If
                sz = loadmaplist(map).size

                Gl.glBegin(Gl.GL_QUADS)
                Gl.glTexCoord2f(0, 1)
                Gl.glVertex2f(-sz + hi, -sz + vi + ms_y)

                Gl.glTexCoord2f(0, 0)
                Gl.glVertex2f(-sz + hi, sz + vi)

                Gl.glTexCoord2f(1, 0)
                Gl.glVertex2f(sz + hi + ms_x, sz + vi)

                Gl.glTexCoord2f(1, 1)
                Gl.glVertex2f(sz + hi + ms_x, -sz + vi + ms_y)

                Gl.glEnd()
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
                Dim cs As Single = loadmaplist(map).size / 40.0!
                glutPrintBox(-sz + hi, -sz + vi + ms_y, loadmaplist(map).realname, 0.5 + cs, 0.5 + cs, 0.5, 1.0)

dont_draw:

                map += 1
            Next
            vi += -space_x + ms_y
        End While
        vi = -30
        map = 0
        While map < map_texture_ids.Length - 2
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
                If selected_map_hit > 0 And map = selected_map_hit - 1 Then
                    Dim selm = selected_map_hit - 1
                    If loadmaplist(selm).size < 20 And Not loadmaplist(selm).grow_shrink Then
                        loadmaplist(selm).grow_shrink = True
                        loadmaplist(selm).direction = 1.0
                        If loadmaplist(selm).size < 1.0 Then
                            loadmaplist(selm).size = 1.0
                        End If
                    End If
                Else
                    GoTo skip
                End If
                sz = loadmaplist(map).size

                Gl.glBegin(Gl.GL_QUADS)
                Gl.glTexCoord2f(0, 1)
                Gl.glVertex2f(-sz + hi, -sz + vi + ms_y)

                Gl.glTexCoord2f(0, 0)
                Gl.glVertex2f(-sz + hi, sz + vi)

                Gl.glTexCoord2f(1, 0)
                Gl.glVertex2f(sz + hi + ms_x, sz + vi)

                Gl.glTexCoord2f(1, 1)
                Gl.glVertex2f(sz + hi + ms_x, -sz + vi + ms_y)

                Gl.glEnd()
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
                Dim cs As Single = loadmaplist(map).size / 40.0!
                glutPrintBox(-sz + hi, -sz + vi + ms_y, loadmaplist(map).realname, 0.5 + cs, 0.5 + cs, 0.5, 1.0)

skip:
                map += 1
            Next
            vi += -space_x + ms_y
        End While

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        'If selected_map_hit > 0 Then
        '    glutPrintBox(mouse.X, -mouse.Y, loadmaplist(selected_map_hit - 1).realname, 1.0, 1.0, 1.0, 1.0)

        'End If
        Gdi.SwapBuffers(pb1_hDC)
        Application.DoEvents()
        If finish_maps Then
            Dim no_stragglers As Boolean = True
            For i = 0 To loadmaplist.Length - 2
                If loadmaplist(i).size > 0.0 Then
                    no_stragglers = False
                End If
            Next
            If no_stragglers Then
                finish_maps = False
                SHOW_MAPS = False
                block_mouse = False
                open_pkg(load_map_name)
            End If
        End If
        autoEventScreen.Set()

    End Sub
    Public Sub draw_pick_map()
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            'MessageBox.Show("Unable to make rendering context current")
            'End
            Return
        End If
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

        Dim w_cnt As Single = 7 'Floor(w / (ms_x + space_x))
        Dim space_cnt As Single = (w_cnt - 1) * space_x
        Dim border As Single = (w - ((w_cnt * ms_x) + space_cnt)) / 2
        Dim map As Byte = 0
        Dim v_cnt = (map_texture_ids.Length - 1) / w_cnt
        If (v_cnt * (ms_x + space_x)) + (border * 2) < pb1.Width Then
            v_cnt -= 1
        End If
        Dim v_pos As Integer = 0
        Dim vi, hi As Single

        vi = -30

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
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            'MessageBox.Show("Unable to make rendering context current")
            Return
        End If
        m_Orbit_Light.Checked = False
        m_fly_map.Checked = False
        Gl.glDisable(Gl.GL_FRAMEBUFFER_SRGB_EXT)

        Gl.glDisable(Gl.GL_FOG)
        Gl.glReadBuffer(Gl.GL_BACK)
        draw_pick_map()


        Dim viewport(4) As Integer
        Gl.glFinish()
        Dim pixel() As Byte = {0, 0, 0, 0}


        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        Gl.glFinish()

        Dim hit = pixel(2)
        If hit > 0 Then
            selected_map_hit = hit
            'tb1.Text = loadmaplist(hit - 1).realname
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

#End Region





    Private finish_maps As Boolean = False




    Private Sub draw_light_sphear()
        If m_Orbit_Light.Checked Then
            '---------------------------------
            'draw the sphere as our sun
            '---------------------------------
            'Z_Cursor = get_Z_at_XY(look_point_X, look_point_Z)
            Gl.glDisable(Gl.GL_LIGHTING)
            Gl.glDisable(Gl.GL_BLEND)
            Gl.glPushMatrix()
            Gl.glTranslatef(position(0), position(1), position(2))
            Gl.glColor4f(1.0, 1.0, 0.0, 1.0)
            glutSolidSphere(4.0, 8, 8)
            Gl.glPopMatrix()
            Gl.glEnable(Gl.GL_LIGHTING)
        End If
    End Sub
    Private Sub setup_fog()
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
    End Sub
    Private Sub draw_base_rings()
        If Not m_bases_ Then
            Return
        End If
        If maploaded And SHOW_RINGS Then
            Gl.glColor3f(0.5, 0.0, 0.0)
            Gl.glCallList(ringDisplayID_1)

            Gl.glColor3f(0.0, 0.7, 0.0)
            Gl.glCallList(ringDisplayID_2)
        End If

    End Sub

    Private Sub draw_dome()
        If Not m_sky_ Then
            'Gl.glPointSize(1.0)
            'Gl.glDisable(Gl.GL_LIGHTING)
            'Gl.glDisable(Gl.GL_DEPTH_TEST)
            'Gl.glColor3f(0.7, 0.7, 1.0)
            'Gl.glPushMatrix()
            'Gl.glTranslatef(eyeX, eyeY - 1.0, eyeZ)
            'Gl.glCallList(stars_display_id)
            'Gl.glPopMatrix()
            'Gl.glEnable(Gl.GL_DEPTH_TEST)
            'Gl.glEnable(Gl.GL_LIGHTING)
            Return
        End If
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
        Dim c = lighting_ambient + 0.95
        Gl.glColor3f(c, c, c)
        'Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_DEPTH_TEST)

        Gl.glPushMatrix()
        'Position to get best view of sky while loading a map.
        'otherwise. translate to camera position position.
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
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, skydometextureID)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glUseProgram(shader_list.dome_shader)
        Gl.glUniform1i(dome_colormap, 0)
        Gl.glCallList(skydomelist)
        Gl.glPopMatrix()
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        '---------------------------------

        Gl.glUseProgram(0)

        Gl.glEnable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glEnable(Gl.GL_LIGHTING)

        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)

    End Sub

    Private Sub draw_g_terrain()
        If Not m_terrain_ Then
            Return
        End If
        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
        Gl.glEnable(Gl.GL_CULL_FACE)
        Gl.glFrontFace(Gl.GL_CCW)
        Gl.glDisable(Gl.GL_NORMALIZE)

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

            For i = 0 To test_count
                If maplist(i).visible Then

                    If Not m_high_rez_Terrain.Checked Or Not hz_loaded Then
                        'low rez terrain
                        Gl.glUseProgram(shader_list.lzTerrainDef_shader)
                        Gl.glUniform1i(c_address2, 0)

                        rendermode = True
                        Gl.glActiveTexture(Gl.GL_TEXTURE0)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(i).colorMapId)
                    Else
                        'hi rez terrain.
                        Gl.glUseProgram(shader_list.terrainDef_shader)


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

                        Gl.glUniform3f(c_position, eyeX, eyeY, eyeZ) 'must have this for distance calculations

                        Gl.glUniform1i(render_has_holes, maplist(i).has_holes)

                        Gl.glUniform1i(render_hole_texture, 0)
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

                        ' bind all the textures
                        Gl.glActiveTexture(Gl.GL_TEXTURE0)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(i).HolesId)

                        Gl.glActiveTexture(Gl.GL_TEXTURE1)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(1).text_id)

                        Gl.glActiveTexture(Gl.GL_TEXTURE2)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(2).text_id)

                        Gl.glActiveTexture(Gl.GL_TEXTURE3)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(3).text_id)

                        Gl.glActiveTexture(Gl.GL_TEXTURE4)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(4).text_id)

                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(1).norm_id)

                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(2).norm_id)

                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(3).norm_id)

                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 8)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).layers(4).norm_id)
                        'bind the mix layer. 
                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 9)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(i).mix_texture_Id)
                        'bind lowrez colormap
                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 10)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, maplist(i).colorMapId)
                    End If
                    Gl.glCallList(maplist(i).calllist_Id)
                End If
            Next
            Gl.glUseProgram(0)
            For patato = 0 To 4
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + patato)
                Gl.glEnable(Gl.GL_TEXTURE_2D)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
                Gl.glDisable(Gl.GL_TEXTURE_2D)
            Next
        End If
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        If m_show_chunks.Checked And maploaded Then
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glColor3f(0.4, 0.4, 0.4)
            For i = 0 To test_count
                If maplist(i).visible Then
                    Gl.glCallList(maplist(i).seamCallId)
                End If
            Next
        End If
        '---------------------------------------------------------------------------------
        'draw the map sections and seams WIRE
        If maploaded And m_wire_terrain.Checked Then
            bump_out_ = Gl.glGetUniformLocation(shader_list.comp_shader, "amount")
            Gl.glUseProgram(shader_list.comp_shader)
            'Gl.glUniform3f(phong_cam_pos, eyeX, eyeY, eyeZ)
            Gl.glUniform1f(bump_out_, 0)
            Gl.glColor3f(0.1, 0.1, 0.3)
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
            Gl.glPolygonOffset(1.0, 1.0)
            For i = 0 To test_count
                If maplist(i).visible Then
                    Gl.glCallList(maplist(i).calllist_Id)
                End If
            Next
            Gl.glDisable(Gl.GL_POLYGON_OFFSET_LINE)
            Gl.glPolygonOffset(0.0, 0.0)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
            Gl.glColor3f(0.3, 0.3, 0.3)
            For i = 0 To test_count
                If maplist(i).visible Then
                    Gl.glCallList(maplist(i).calllist_Id)
                End If
            Next
            Gl.glUseProgram(0)
            If normal_mode > 0 Then
                Gl.glUseProgram(shader_list.normal_shader)
                Gl.glUniform1i(view_normal_mode_, normal_mode)
                Gl.glUniform1f(normal_length_, 0.2)
                For i = 0 To test_count
                    If maplist(i).visible Then
                        Gl.glCallList(maplist(i).calllist_Id)
                    End If
                Next
                Gl.glUseProgram(0)
                Gl.glEnable(Gl.GL_DEPTH_TEST)
            End If
            Gl.glDisable(Gl.GL_DEPTH_TEST)
            Gl.glBegin(Gl.GL_TRIANGLES)
            Gl.glColor3f(1.0, 0.0, 0.0)
            Gl.glVertex3f(tr_.X, tr_.Z, tr_.Y)
            Gl.glVertex3f(tl_.X, tl_.Z, tl_.Y)
            Gl.glVertex3f(bl_.X, bl_.Z, bl_.Y)


            Gl.glVertex3f(bl_.X, bl_.Z, bl_.Y)
            Gl.glVertex3f(br_.X, br_.Z, br_.Y)
            Gl.glVertex3f(tr_.X, tr_.Z, tr_.Y)
            Gl.glEnd()
            Gl.glEnable(Gl.GL_DEPTH_TEST)
            Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
        End If

        '---------------------------------------------------------------------------------

    End Sub

    Private Sub draw_g_models(ByVal pass As Boolean)
        If Not m_models_ Then
            Return
        End If
        ' draw the models
        Gl.glEnable(Gl.GL_CULL_FACE)
        Gl.glFrontFace(Gl.GL_CW)
        If maploaded And m_wire_models.Checked And m_show_models.Checked Then
            'Dim e1 = Gl.glGetError
            Gl.glUseProgram(shader_list.comp_shader)
            Gl.glUniform1f(bump_out_, 0.0)
            Gl.glColor3f(0.2, 0.2, 0.0)
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glActiveTexture(Gl.GL_TEXTURE1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)

            Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
            Gl.glPolygonOffset(1.0, 1.0)
            'Dim e2 = Gl.glGetError

            For model As UInt32 = 0 To Models.matrix.Length - 1
                If Models.models(model).visible Then
                    For k = 0 To Models.models(model)._count - 1
                        Gl.glPushMatrix()
                        Gl.glMultMatrixf(Models.matrix(model).matrix)
                        Gl.glCallList(Models.models(model).componets(k).callList_ID)
                        Gl.glPopMatrix()
                    Next
                End If
            Next
            Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
            Gl.glPolygonOffset(0.0, 0.0)

            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
            Gl.glColor3f(0.3, 0.3, 0.3)
            For model As UInt32 = 0 To Models.matrix.Length - 1
                If Models.models(model).visible Then
                    For k = 0 To Models.models(model)._count - 1
                        Gl.glPushMatrix()
                        Gl.glMultMatrixf(Models.matrix(model).matrix)
                        Gl.glCallList(Models.models(model).componets(k).callList_ID)
                        Gl.glPopMatrix()
                    Next
                End If
            Next
            Gl.glUseProgram(0)
            'Gl.glColor3f(0.5, 0.5, 0.5)
            If normal_mode > 0 Then
                Gl.glUseProgram(shader_list.normal_shader)
                Gl.glUniform1i(view_normal_mode_, normal_mode)
                Gl.glUniform1f(normal_length_, 0.3)
                For model As UInt32 = 0 To Models.matrix.Length - 1
                    If Models.models(model).visible Then
                        For k = 0 To Models.models(model)._count - 1
                            Gl.glPushMatrix()
                            Gl.glMultMatrixf(Models.matrix(model).matrix)
                            Gl.glCallList(Models.models(model).componets(k).callList_ID)
                            Gl.glPopMatrix()
                        Next
                    End If
                Next
                Gl.glUseProgram(0)
            End If

        End If
        '---------------------------------------------------------------------------------
        If maploaded And m_show_models.Checked _
                    And Not m_wire_models.Checked _
                    Then ' cant let this try and draw shit that isnt there yet!!!
            Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
            ' Gl.glColor3f(lighting_model_level, lighting_model_level, lighting_model_level)
            'Gl.glDisable(Gl.GL_LIGHTING)
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glUseProgram(shader_list.buildingsDef_shader)

            Gl.glUniform1i(c_address3, 0)
            Gl.glUniform1i(n_address3, 1)
            Gl.glUniform1i(colormap2, 2)
            Dim uv2s As Boolean
            For model As UInt32 = 0 To loaded_models.stack.Length - 2
                'If Models.models(model).visible Then
                '    If Models.matrix(model).matrix IsNot Nothing Then

                Gl.glPushMatrix()
                'Gl.glMultMatrixf(Models.matrix(model).matrix)
                For k = 0 To loaded_models.stack(model)._count - 1
                    With loaded_models.stack(model)

                        Gl.glActiveTexture(Gl.GL_TEXTURE0)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, .textId(k))
                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, .normID(k))
                        uv2s = .multi_textured(k)
                        If (Not m_show_uv2.Checked) Or (Not uv2s_loaded) Then
                            uv2s = False    ' this stops showing the 2nd uv2 textures 
                        End If
                        If uv2s Then
                            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, .text2Id(k))
                            Gl.glUniform1i(is_multi_textured, 1)
                        Else
                            Gl.glUniform1i(is_multi_textured, 0)
                        End If

                        If .bumped(k) Then
                            Gl.glUniform1i(is_bumped3, 1)
                        Else
                            Gl.glUniform1i(is_bumped3, 0)
                        End If

                        If .GAmap(k) Then
                            Gl.glUniform1i(is_GAmap, 1)      ' tell shader if this is a bumped model
                        Else
                            Gl.glUniform1i(is_GAmap, 0)     ' tell shader if this is a bumped model

                        End If
                        Gl.glUniform1i(alphaRef, .alphaRef(k))
                        Gl.glUniform1i(alphaTestEnable, .alphaTestEnable(k))
                    End With

                    For i = 0 To loaded_models.stack(model).model_count - 1
                        If Models.models(loaded_models.stack(model).model_id(i)).visible Then
                            If Models.models(loaded_models.stack(model).model_id(i)).isBuilding Then
                                Gl.glUniform1i(bld_flag, 128)
                            Else
                                Gl.glUniform1i(bld_flag, 96)
                            End If
                            Gl.glUniformMatrix4fv(bld_matrix, 1, Gl.GL_FALSE, loaded_models.stack(model).matrix(i).matrix)
                            Gl.glCallList(loaded_models.stack(model).dispId(k))
                        End If

                    Next
                Next
                Gl.glPopMatrix()
                '       End If
                ' End If
            Next
            Gl.glUseProgram(0)
        End If
        '---------------------------------------------------------------------------------
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

    End Sub

    Private Sub draw_g_trees()
        If Not m_trees_ Then
            Return
        End If
        'Gl.glFrontFace(Gl.GL_CCW)
        If maploaded And m_wire_trees.Checked And m_show_trees.Checked Then
            'Gl.glEnable(Gl.GL_LIGHTING)
            'draw as soild
            Dim e1 = Gl.glGetError
            Gl.glColor3f(0.0, 0.2, 0.0)
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            Gl.glDisable(Gl.GL_CULL_FACE)

            Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
            Gl.glPolygonOffset(1.0, 1.0)
            For mode = 0 To 1
                If mode = 0 Then
                    Gl.glUseProgram(shader_list.comp_shader)
                    Gl.glUniform1f(bump_out_, 0.0)
                Else
                    Gl.glUseProgram(shader_list.leafcoloredDef_shader)
                End If

                For i As UInt32 = 0 To speedtree_matrix_list.Length - 2
                    Gl.glPushMatrix()
                    Gl.glMultMatrixf(Trees.matrix(i).matrix)
                    If mode = 0 Then
                        If Trees.flora(i).branch_displayID > 0 Then
                            Gl.glCallList(Trees.flora(i).branch_displayID)
                        End If
                        If Trees.flora(i).frond_displayID > 0 Then
                            Gl.glCallList(Trees.flora(i).frond_displayID)
                        End If
                    Else
                        If Trees.flora(i).leaf_displayID > 0 Then
                            Gl.glCallList(Trees.flora(i).leaf_displayID)
                        End If
                    End If
                    Gl.glPopMatrix()
                Next
                Gl.glUseProgram(0)
            Next
            Dim e = Gl.glGetError
            Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
            Gl.glPolygonOffset(0.0, 0.0)

            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
            Gl.glColor4f(0.3, 0.3, 0.3, 1.0)
            'draw as wire
            'Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glUseProgram(shader_list.comp_shader)
            Gl.glUniform1f(bump_out_, 0.0)
            For i As UInt32 = 0 To speedtree_matrix_list.Length - 2
                Gl.glPushMatrix()
                Gl.glMultMatrixf(Trees.matrix(i).matrix)
                If Trees.flora(i).branch_displayID > 0 Then
                    Gl.glCallList(Trees.flora(i).branch_displayID)

                End If
                If Trees.flora(i).frond_displayID > 0 Then
                    Gl.glCallList(Trees.flora(i).frond_displayID)
                End If
                Gl.glPopMatrix()

            Next
            Gl.glUseProgram(0)
            Gl.glUseProgram(shader_list.leafcoloredDef_shader)
            For i As UInt32 = 0 To speedtree_matrix_list.Length - 2
                Gl.glPushMatrix()
                If Trees.flora(i).leaf_displayID > 0 Then
                    Gl.glUniformMatrix4fv(leafColored_matrix, 1, 0, Trees.matrix(i).matrix)
                    Gl.glMultMatrixf(Trees.matrix(i).matrix)
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
            And Not m_low_quality_trees.Checked Then

            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            Gl.glDisable(Gl.GL_CULL_FACE)


            If Trees.flora IsNot Nothing Then
                For k = 0 To treeCache.Length - 2
                    For mode = 0 To 2
                        Select Case mode

                            Case 0 ' branchs
                                Gl.glUseProgram(shader_list.branchDef_shader)

                                Gl.glUniform1i(branch_color_map_id, 0)
                                Gl.glUniform1i(branch_normalmap_map_id, 1)

                            Case 1 ' fronds
                                Gl.glUseProgram(shader_list.frondDef_shader)

                                Gl.glUniform1i(frond_color_map_id, 0)
                                Gl.glUniform1i(frond_normal_map_id, 1)

                            Case 2 'leaves
                                Gl.glUseProgram(shader_list.leafDef_shader)

                                Gl.glUniform1i(leaf_color_map_id, 0)
                                Gl.glUniform1i(leaf_normal_map_id, 1)
                                Gl.glDisable(Gl.GL_CULL_FACE)

                        End Select
                        'Dim draw As Boolean = True
                        Dim t_cut_off As Single = 300000
                        With treeCache(k)
                            Select Case mode
                                'only bind textures ONCE for each type of tree.
                                Case 0 'branches
                                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, .branch_textureID)
                                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, .branch_normalID)
                                Case 1 'fronds
                                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, .billboard_textureID) 'speedtree composite texture
                                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, .billboard_normalID) 'speedtree composite texture
                                Case 2 'leaves
                                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, .billboard_textureID) 'speedtree composite texture
                                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, .billboard_normalID) 'speedtree composite texture

                            End Select
                            For i As UInt32 = 0 To .tree_cnt - 1
                                If treeCache(k).BB(i).visible Then

                                    Gl.glPushMatrix()
                                    'Gl.glMultMatrixf(.matrix_list(i).matrix)

                                    Gl.glDisable(Gl.GL_CULL_FACE)
                                    If mode = 0 Then
                                        If .branch_displayID > 0 Then
                                            Gl.glUniformMatrix4fv(branch_matrix_id, 1, Gl.GL_FALSE, .matrix_list(i).matrix)
                                            Gl.glCallList(.branch_displayID)
                                        End If
                                    End If

                                    If mode = 1 Then
                                        If .frond_displayID > 0 Then
                                            Gl.glUniformMatrix4fv(frond_matrix_id, 1, Gl.GL_FALSE, .matrix_list(i).matrix)
                                            Gl.glCallList(.frond_displayID)
                                        End If
                                    End If

                                    If mode = 2 Then
                                        If .leaf_displayID > 0 Then
                                            Gl.glUniformMatrix4fv(leaf_matrix_id, 1, Gl.GL_FALSE, .matrix_list(i).matrix)
                                            Gl.glMultMatrixf(.matrix_list(i).matrix)
                                            Gl.glCallList(.leaf_displayID)
                                        End If
                                    End If
                                    Gl.glPopMatrix()
                                End If
                            Next
                            Gl.glUseProgram(0)
                        End With
                    Next
                Next
            End If
            Gl.glUseProgram(0)
            Gl.glEnable(Gl.GL_LIGHTING)
        End If

        If m_low_quality_trees.Checked Then
            Gl.glEnable(Gl.GL_CULL_FACE)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glUseProgram(shader_list.lowQualityTrees_shader)
            Gl.glUniform1i(basic_color, 0)
            Gl.glUniform1i(basic_normal, 1)
            Gl.glUniform1i(basic_color_level, lighting_model_level)
            Gl.glUniform1i(basic_gamma, gamma_level)

            For k = 0 To treeCache.Length - 2
                With treeCache(k)
                    For i As UInt32 = 0 To .tree_cnt - 1
                        Gl.glPushMatrix()
                        Gl.glMultMatrixf(.matrix_list(i).matrix)
                        Gl.glActiveTexture(Gl.GL_TEXTURE0)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, .billboard_textureID) 'speedtree composite texture
                        Gl.glActiveTexture(Gl.GL_TEXTURE1)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, .billboard_normalID) 'speedtree normal texture
                        Gl.glCallList(.billboard_displayID)
                        Gl.glPopMatrix()
                    Next
                End With
            Next
            Gl.glUseProgram(0)
        End If


    End Sub

    Private Sub draw_g_decals()

        If Not m_decals_ Then
            Return
        End If
        G_Buffer.attach_color_and_postion_only()

        Dim width, height As Integer
        width = pb1.Width + pb1.Width Mod 1
        height = pb1.Height + pb1.Height Mod 1
        Gl.glFrontFace(Gl.GL_CW)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDepthMask(Gl.GL_FALSE)
        Gl.glUseProgram(shader_list.decalsCpassDef_shader)
        Gl.glUniform1i(prjd_depthmap, 0)
        Gl.glUniform1i(prjd_color, 1)
        'Gl.glUniform1i(prjd_normal_in, 3)
        Gl.glUniform1i(prjd_flagmap, 2)
        'Gl.glUniform2f(prjd_FOV, width, heigth)
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glBlendEquationSeparate(Gl.GL_FUNC_ADD, Gl.GL_FUNC_ADD)
        Gl.glBlendFuncSeparate(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA, Gl.GL_ONE, Gl.GL_ONE_MINUS_SRC_ALPHA)
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)

        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gDepthTexture)

        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gFlag)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gNormal)


        For k = 0 To decal_matrix_list.Length - 1
            With decal_matrix_list(k)
                'If decal_matrix_list(k).influence <> influence Then

                If decal_matrix_list(k).good And decal_matrix_list(k).visible Then
                    Gl.glUniformMatrix4fv(prjd_matrix, 1, Gl.GL_FALSE, .matrix)
                    Gl.glUniform3f(prjd_topright, .rtr.x, .rtr.y, .rtr.z)
                    Gl.glUniform3f(prjd_bottomleft, .lbl.x, .lbl.y, .lbl.z)
                    Gl.glUniform2f(prjd_uv_wrap, .u_wrap, .v_wrap)
                    Gl.glUniform1i(prjd_influence, CInt(.influence))
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, decal_matrix_list(k).texture_id)

                    Gl.glCallList(decal_matrix_list(k).display_id)
                End If
                'End If
            End With
        Next
        Gl.glUseProgram(0)
        '====================================================================================
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        Gl.glBindFramebufferEXT(Gl.GL_READ_FRAMEBUFFER_EXT, gBufferFBO)
        Gl.glReadBuffer(Gl.GL_COLOR_ATTACHMENT1_EXT)
        'Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        'ResizeGL()
        'ViewPerspective()
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glClearColor(0.0F, 0.0F, 0.4F, 0.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glBlitFramebufferEXT(0, 0, width, height, 0, 0, width, height, Gl.GL_COLOR_BUFFER_BIT, Gl.GL_NEAREST)
        Gl.glReadBuffer(Gl.GL_BACK)
        Gl.glBindFramebufferEXT(Gl.GL_READ_FRAMEBUFFER_EXT, 0)

        Gl.glUseProgram(shader_list.decalsNpassDef_shader)
        Gl.glUniform1i(decal_depthmap_id, 0)
        Gl.glUniform1i(decal_normal_in_id, 1)
        Gl.glUniform1i(decal_normal_map_id, 3)
        Gl.glUniform1i(decal_color_map_id, 2)

        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gDepthTexture)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gNormal)

        For k = 0 To decal_matrix_list.Length - 1
            With decal_matrix_list(k)
                'If decal_matrix_list(k).influence <> influence Then

                If decal_matrix_list(k).good And decal_matrix_list(k).visible Then
                    Gl.glUniformMatrix4fv(decal_matrix_id, 1, Gl.GL_FALSE, .matrix)
                    Gl.glUniform3f(decal_tr_id, .rtr.x, .rtr.y, .rtr.z)
                    Gl.glUniform3f(decal_bl_id, .lbl.x, .lbl.y, .lbl.z)
                    Gl.glUniform2f(decal_uv_wrap, .u_wrap, .v_wrap)
                    Gl.glUniform1i(decal_influence, .influence)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, decal_matrix_list(k).texture_id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, decal_matrix_list(k).normal_id)

                    Gl.glCallList(decal_matrix_list(k).display_id)
                End If
                'End If
            End With
        Next
        Gl.glUseProgram(0)
        Gl.glReadBuffer(Gl.GL_BACK)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gNormal)
        Gl.glCopyTexSubImage2D(Gl.GL_TEXTURE_2D, 0, 0, 0, 0, 0, width, height)
        Dim e2 = Gl.glGetError
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        'Gdi.SwapBuffers(pb1_hDC)
        'Thread.Sleep(20)
        '====================================================================================
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, gBufferFBO)
        'Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, gBufferFBO)
        Dim e1 = Gl.glGetError

        G_Buffer.attachFOBtextures()
        If m_wire_decals.Checked Then

            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
            Gl.glColor3f(1.0, 1.0, 1.0)
            For k = 0 To decal_matrix_list.Length - 1
                With decal_matrix_list(k)

                    If decal_matrix_list(k).good And decal_matrix_list(k).visible Then
                        Gl.glPushMatrix()
                        Gl.glMultMatrixf(.matrix)
                        Gl.glCallList(decal_matrix_list(k).display_id)
                        Gl.glPopMatrix()
                    End If
                End With
            Next
        End If
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDepthMask(Gl.GL_TRUE)

        Return


    End Sub

    Private Sub draw_g_tanks(ByRef cp() As Single, ByVal model_view() As Double, ByVal projection() As Double, ByVal viewport() As Integer, ByVal sx As Single, ByVal sy As Single, ByVal sz As Single)
        Dim mat(16) As Single
        'Gl.glFinish()
        Gl.glFrontFace(Gl.GL_CW)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        If maploaded Then
            Dim cv = 57.2957795
            Gl.glUseProgram(shader_list.tankDef_shader)
            For i = 0 To 14
                Gl.glColor3f(0.4!, 0.0!, 0.0!)
                If locations.team_1(i).tank_displaylist > -1 Then
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


                    Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, mat)
                    Gl.glUniformMatrix4fv(tankDef_matrix, 1, 0, mat)
                    If tankID = i Then
                        Gl.glColor3f(0.5, 0.5, 0.0)
                    End If
                    Gl.glCallList(locations.team_1(i).tank_displaylist)
                    Gl.glPopMatrix()
                End If
            Next
            For i = 0 To 14
                Gl.glColor3f(0.0, 0.3, 0.0)
                If locations.team_2(i).tank_displaylist > -1 Then
                    Gl.glPushMatrix()
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

                    Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, mat)
                    Gl.glUniformMatrix4fv(tankDef_matrix, 1, 0, mat)
                    If tankID = i Then
                        Gl.glColor3f(0.5, 0.5, 0.0)
                    End If
                    Gl.glCallList(locations.team_2(i).tank_displaylist)
                    Gl.glPopMatrix()
                End If
            Next
        End If
        Gl.glUseProgram(0)

    End Sub


    Dim terrain_time, model_time, decal_time, tree_time, cull_time, total_time As Long
    Dim sample_cnt As Integer = 0
    Public Shared worldMatrix(16), viewMatrix(16), projection_s(16), modelMatrix(16) As Single
    Public Sub draw_to_gBuffer()
        Dim model_view(16) As Double
        Dim projection(16) As Double
        Dim viewport(4) As Integer
        Dim cp(2) As Single
        Dim sx, sy, sz As Single
        Dim width, height As Integer
        width = pb1.Width + pb1.Width Mod 1
        height = pb1.Height + pb1.Height Mod 1
        swat1.Restart()
        setup_fog()
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, gBufferFBO)
        G_Buffer.attachFOBtextures()

        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)

        Gl.glDepthFunc(Gl.GL_LEQUAL)
        Gl.glFrontFace(Gl.GL_CW)
        Gl.glCullFace(Gl.GL_BACK)
        Gl.glLineWidth(1)
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 0.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT Or Gl.GL_STENCIL_BUFFER_BIT)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        Gl.glDisable(Gl.GL_NORMALIZE)
        Gl.glDisable(Gl.GL_MULTISAMPLE)

        ResizeGL()
        ViewPerspective()
        '---------------------------------------------------------------------------------
        '------------- cull objects
        swat2.Restart()
        ExtractFrustum()
        check_terrain_visible()
        check_models_visible() 'not sure if this is the best place for this
        check_trees_visible()
        check_decals_visible()
        If frmStats.Visible Then
            cull_time += swat2.ElapsedMilliseconds
            frmStats.rt_culled_count.Text = culled_count.ToString("0000")
        End If
        '---------------------------------------------------------------------------------
        Gl.glGetFloatv(Gl.GL_MODELVIEW, worldMatrix)
        Gl.glPushMatrix()
        Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, model_view)
        Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projection)
        'Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, projection_s)
        Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, modelMatrix)
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        '---------------------------------------------------------------------------------
        'If show chunk ids, get the locations
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
        '---------------------------------------------------------------------------------
        G_Buffer.attach_color_only()
        draw_dome()
        G_Buffer.attachFOBtextures()

        '---------------------------------------------------------------------------------
        'Terrain
        swat2.Restart()
        If terrain_loaded Then
            draw_g_terrain()
        End If
        If frmStats.Visible = True Then
            terrain_time += swat2.ElapsedMilliseconds
        End If
        '---------------------------------------------------------------------------------
        'Models bulidings only
        swat2.Restart()
        If models_loaded Then
            draw_g_models(False)
        End If
        If frmStats.Visible = True Then
            model_time += swat2.ElapsedMilliseconds
        End If
        '---------------------------------------------------------------------------------
        'I changed how decals are drawn on what objects.
        'I use a texture and set its colors based on what the object is being drawn.
        'flags for decals
        '64 = terrain
        '96 = buildings
        '128 = everything but buildings
        swat2.Restart()
        If m_decals_ And m_show_decals.Checked Then
            G_Buffer.get_depth_buffer(width, height)
            If decals_loaded And m_show_decals.Checked Then
                draw_g_decals()
            End If
        End If

        If frmStats.Visible = True Then
            decal_time += swat2.ElapsedMilliseconds
        End If

        '---------------------------------------------------------------------------------
        'Trees
        swat2.Restart()
        If trees_loaded Then
            draw_g_trees()
        End If

        If frmStats.Visible = True Then
            tree_time += swat2.ElapsedMilliseconds
        End If

        '---------------------------------------------------------------------------------
        'Tanks
        draw_g_tanks(cp, model_view, projection, viewport, sx, sy, sz)

        'only needed for SSAO and I cant get it to work!
        'G_Buffer.get_depth_buffer(width, height)


        Gl.glDisable(Gl.GL_LIGHTING)
        '---------------------------------------------------------------------------------
        If m_show_cursor.Checked Then
            Gl.glDisable(Gl.GL_LIGHTING)
            Gl.glColor3f(1.0, 1.0, 0.0)
            If Not NetData Then
                draw_cursor(u_look_point_X, u_look_point_Z)
            Else
                draw_cursor(Packet_in.Ex, Packet_in.Ez)
            End If
        End If
        'Gl.glEnable(Gl.GL_LIGHTING)
        '------------------------------------
        Gl.glActiveTexture(Gl.GL_TEXTURE0)

        'Attach only the color buffer'
        G_Buffer.attach_color_only()
        'debug only
        'Gl.glColor3f(1.0, 1.0, 0.0)
        'For model As UInt32 = 0 To Models.matrix.Length - 1
        '    If Models.matrix(model).matrix IsNot Nothing Then
        '        Gl.glBegin(Gl.GL_POINTS)
        '        For k = 0 To 7
        '            Gl.glVertex3f(Model_Matrix_list(model).BB(k).x, Model_Matrix_list(model).BB(k).y, Model_Matrix_list(model).BB(k).z)
        '        Next
        '        Gl.glEnd()
        '    End If
        'Next
        Gl.glPointSize(4)
        Gl.glColor3f(1.0, 1.0, 0.0)
        Gl.glBegin(Gl.GL_POINTS)
        Dim v As vect3
        For i = 0 To LIGHT_COUNT_ * 3 Step 3
            v.x = sl_light_pos(i + 0)
            v.y = sl_light_pos(i + 1)
            v.z = sl_light_pos(i + 2)
            'Gl.glVertex3f(v.x, v.y, v.z)
        Next
        Gl.glEnd()
        'Draw the lights location
        draw_light_sphear()

        'Base locations
        If bases_laoded Then
            draw_base_rings()
        End If
        'Draw the water. IF there is water....
        If water.IsWater And maploaded And m_show_water.Checked And m_water_ And water_loaded Then
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
            Gl.glFrontFace(Gl.GL_CW)
            Gl.glEnable(Gl.GL_LIGHTING)
            Gl.glEnable(Gl.GL_DEPTH_TEST)

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glEnable(Gl.GL_ALPHA_TEST)
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
            Gl.glPushMatrix()
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glColor4f(0.1, 0.1, 0.5, 0.4)
            Gl.glTranslatef(-water.position.x, -0.1, water.position.z)
            Gl.glRotatef(-water.orientation * 57.2957795, 0.0, 1.0, 0.0)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, water.textureID)
            Gl.glCallList(water.displayID)
            Gl.glPopMatrix()

            Gl.glDisable(Gl.GL_BLEND)
            Gl.glDisable(Gl.GL_FOG)
            Gl.glFrontFace(Gl.GL_CCW)
        End If
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_BLEND)


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

        '---------------------------------------------------------------------------------

        Gl.glFlush() ' needed?

        Gl.glPopMatrix()
        'Dim e6 = Gl.glGetError
        'Dim e5 = Gl.glGetError
        ViewOrtho()
        Gl.glFrontFace(Gl.GL_CW)

        Gl.glDisable(Gl.GL_DEPTH_TEST)
        'G_Buffer.getsize(width, heigth)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
        Gl.glEnable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glUseProgram(0)
        Gl.glDisable(Gl.GL_BLEND)
        '----------------------------------------------------------------------------------------------
        'SSAO pass
        'can't get this to work!! GRRRRR
        GoTo skip_SSAO
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT)
        Gl.glUseProgram(shader_list.SSAO_shader)
        Gl.glUniform1i(ss_gNormal, 0)
        Gl.glUniform1i(ss_gDepthMap, 1)
        Gl.glUniform1i(ss_noise, 2)
        Gl.glUniform2f(ss_screen_size, CSng(width), CSng(height))
        Gl.glUniform3fv(ss_kernel, 64, randomFloats)

        Gl.glUniformMatrix4fv(ss_prj_matrix, 1, 0, projection_s)
        Gl.glUniformMatrix4fv(ss_mdl_Matrix, 1, 0, modelMatrix)

        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gNormal)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gDepthTexture)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, NoiseTexture)

        Gl.glBegin(Gl.GL_QUADS)
        '---

        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex3f(0, 0, 0.0)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex3f(width, 0, 0.0)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex3f(width, -height, 0.0)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(0, -height, 0.0)
        Gl.glEnd()

        Gl.glUseProgram(0)
        Gl.glReadBuffer(Gl.GL_BACK)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gFlag)
        Gl.glCopyTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_LUMINANCE8, 0, 0, width, height, 0)
        Dim e = Gl.glGetError
        'Gl.glCopyTexSubImage2D(Gl.GL_TEXTURE_2D, 0, 0, 0, 0, 0, width, height)


skip_SSAO:
        '----------------------------------------------------------------------------------------------
        'Lighting pass.
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        Gl.glClearColor(0.0, 0.3, 0.3, 0.0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        Gl.glUseProgram(shader_list.deferred_shader)

        Gl.glUniform1i(deferred_gcolor, 0)
        Gl.glUniform1i(deferred_gnormal, 1)
        Gl.glUniform1i(deferred_gposition, 2)
        Gl.glUniform1i(deferred_depthmap, 3)
        Gl.glUniform3fv(deferred_light_position, 1, position)
        Gl.glUniform3f(deferred_cam_position, eyeX, eyeY, eyeZ)
        Gl.glUniform1f(deferred_bright, lighting_terrain_texture)
        Gl.glUniform1f(deferred_gray, gray_level)
        Gl.glUniform1f(deferred_spec, lighting_model_level)
        Gl.glUniform1f(deferred_ambient, lighting_ambient)
        Gl.glUniform1f(deferred_gamma, gamma_level)
        Gl.glUniform1f(deferred_mapHeight, z_max)
        If m_small_lights.Checked Then ' lights on?
            Gl.glUniform3fv(deferred_lights_pos, LIGHT_COUNT_, sl_light_pos)
            Gl.glUniform3fv(deferred_lights_color, LIGHT_COUNT_, sl_light_color)
            Gl.glUniform1i(deferred_light_count, LIGHT_COUNT_)
        Else
            Gl.glUniform1i(deferred_light_count, 0) ' send zero light count if off
        End If

        Gl.glUniform1f(deferred_water_line, WATER_LINE_)

        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gNormal)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gPosition)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gDepthTexture)


        Gl.glBegin(Gl.GL_QUADS)
        '---

        Gl.glTexCoord2f(0.0, 1.0)
        Gl.glVertex3f(0, 0, 0.0)

        Gl.glTexCoord2f(1.0, 1.0)
        Gl.glVertex3f(width, 0, 0.0)

        Gl.glTexCoord2f(1.0, 0.0)
        Gl.glVertex3f(width, -height, 0.0)

        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(0, -height, 0.0)
        Gl.glEnd()

        Gl.glUseProgram(0)

        '----------------------------------------------------------------------------------------------
        If m_FXAA.Checked Then
            'FXAA pass.
            Gl.glReadBuffer(Gl.GL_BACK)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
            Gl.glCopyTexSubImage2D(Gl.GL_TEXTURE_2D, 0, 0, 0, 0, 0, width, height)

            Gl.glUseProgram(shader_list.FXAA_shader)

            Gl.glUniform1i(fxaa_texture_in, 0)
            Gl.glUniform1f(fxaa_rt_h, height)
            Gl.glUniform1f(fxaa_rt_w, width)


            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)


            Gl.glBegin(Gl.GL_QUADS)
            '---

            Gl.glTexCoord2f(0.0, 1.0)
            Gl.glVertex3f(0, 0, 0.0)

            Gl.glTexCoord2f(1.0, 1.0)
            Gl.glVertex3f(width, 0, 0.0)

            Gl.glTexCoord2f(1.0, 0.0)
            Gl.glVertex3f(width, -height, 0.0)

            Gl.glTexCoord2f(0.0, 0.0)
            Gl.glVertex3f(0, -height, 0.0)
            Gl.glEnd()

            Gl.glUseProgram(0)
        End If

        'unbind all used textures
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        '----------------------------------------------------------------------------------------------

        draw_ortho_stuff()
        Gdi.SwapBuffers(pb1_hDC)

        '---------------------------------------------------------------------------------
        If frmStats.Visible = True Then
            total_time += swat1.ElapsedMilliseconds
            sample_cnt += 1
            If sample_cnt > 20 Then
                sample_cnt = 0
                cull_time /= 20
                terrain_time /= 20
                model_time /= 20
                tree_time /= 20
                decal_time /= 20
                total_time /= 20
                frmStats.rt_cull.Text = cull_time.ToString("00")
                frmStats.rt_terrian.Text = terrain_time.ToString("00")
                frmStats.rt_models.Text = model_time.ToString("00")
                frmStats.rt_trees.Text = tree_time.ToString("00")
                frmStats.rt_decals.Text = decal_time.ToString("00")
                frmStats.rt_total.Text = total_time.ToString("00")
                cull_time = 0
                terrain_time = 0
                model_time = 0
                tree_time = 0
                decal_time = 0
                total_time = 0
            End If
        End If
        '---------------------------------------------------------------------------------
        If frmTestView.Visible Then
            frmTestView.update_screen()
        End If
        '---------------------------------------------------------------------------------
        autoEventScreen.Set()
    End Sub

    Private Sub draw_ortho_stuff()
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)




        If m_show_status.Checked Then
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
                    If locations.team_1(i).tank_displaylist > -1 Then
                        If locations.team_1(i).scrn_coords(2) < 1.0 Then
                            glutPrintBox(locations.team_1(i).scrn_coords(0) - ((locations.team_1(i).name.Length / 2) * 8), locations.team_1(i).scrn_coords(1) - pb1.Height, locations.team_1(i).name, 1.0, 0.0, 0.0, 1.0)
                        End If
                    End If
                    If locations.team_2(i).tank_displaylist > -1 Then
                        If locations.team_2(i).scrn_coords(2) < 1.0 Then
                            glutPrintBox(locations.team_2(i).scrn_coords(0) - ((locations.team_2(i).name.Length / 2) * 8), locations.team_2(i).scrn_coords(1) - pb1.Height, locations.team_2(i).name, 0.0, 1.0, 0.0, 1.0)
                        End If

                    End If
                Next
            End If
            If maploaded And m_show_tank_comments.Checked Then
                For i = 0 To 14
                    If locations.team_1(i).tank_displaylist > -1 Then
                        If locations.team_1(i).scrn_coords(2) < 1.0 Then
                            glutPrintBox(locations.team_1(i).scrn_coords(0) - ((locations.team_1(i).comment.Length / 2) * 8), locations.team_1(i).scrn_coords(1) - pb1.Height - 15, locations.team_1(i).comment, 1.0, 1.0, 1.0, 1.0)
                        End If
                    End If
                    If locations.team_2(i).tank_displaylist > -1 Then
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
            If m_show_minimap.Checked And mini_map_loaded Then
                draw_minimap()
            End If
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
            Gl.glColor4f(0.3, 0.0, 0.0, 0.6)
            Gl.glBegin(Gl.GL_QUADS)
            Gl.glVertex3f(0.0, -pb1.Height, 0.0)
            Gl.glVertex3f(0.0, -pb1.Height + 20, 0.0)
            Gl.glColor4f(0.1, 0.0, 0.0, 0.6)
            Gl.glVertex3f(pb1.Width - w, -pb1.Height + 20, 0.0)
            Gl.glVertex3f(pb1.Width - w, -pb1.Height, 0.0)
            Gl.glEnd()

            If maploaded Then
                Dim fps As Integer = screen_totaled_draw_time
                Dim str = " FPS:" + fps.ToString
                str += "  Location: [ "
                str += coordStr + " ]"
                ' Dim yp = get_Z_at_XY(u_look_point_X, u_look_point_Z)
                'str += "   X:" + u_look_point_X.ToString("0.000000") + "  Y:" + u_look_point_Y.ToString("0.000000") + "  Z:" + u_look_point_Z.ToString("0.000000")
                'str += "   GX:" + d_hx.ToString + "  GY:" + d_hy.ToString
                'swat.Stop()
                glutPrint(10, 8 - pb1.Height, str.ToString, 0.0, 1.0, 0.0, 1.0)
            End If
        End If
        'Gl.glFinish()

    End Sub

    Public Sub draw_scene()
        'Application.DoEvents()
        If stopGL Then Return
        gl_busy = True
        If Not _STARTED Then Return
        If SHOW_MAPS Then
            gl_pick_map(mouse.X, mouse.Y)
            gl_busy = False
            Return
        End If
        If Not maploaded Then
            Return
        End If
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        draw_to_gBuffer()
        gl_busy = False
        Return
 
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
        'Gl.glBindTexture(Gl.GL_TEXTURE_2D, map_layers(0).main_texture)

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
        If xy = 0 Then Return ' no data loaded and the 'step xy' causes an infinite loop
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
            If locations.team_1(t1).tank_displaylist > -1 Then
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
            If locations.team_2(t1).tank_displaylist > -1 Then
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
            If locations.team_1(t1).tank_displaylist > -1 Then
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
            If locations.team_2(t1).tank_displaylist > -1 Then
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
            If locations.team_1(t1).tank_displaylist > -1 Then
                Dim lx = (((locations.team_1(t1).loc_x + xc) / vs) * (-xy)) + (minimap_size / 2.0) ' + (ic_x * scale)
                Dim ly = (((locations.team_1(t1).loc_z + yc) / vs) * (-xy)) + (minimap_size / 2.0) ' - (ic_y * scale)
                glutPrint(lx - ((locations.team_1(t1).name.Length / 2) * 8) + 2, -ly - 2, locations.team_1(t1).name, 0.0, 0.0, 0.0, 0.3)
                glutPrint(lx - ((locations.team_1(t1).name.Length / 2) * 8) + 1, -ly - 1, locations.team_1(t1).name, 0.0, 0.0, 0.0, 0.5)
                glutPrint(lx - ((locations.team_1(t1).name.Length / 2) * 8), -ly, locations.team_1(t1).name, 1.0, 1.0, 1.0, 1.0)

            End If
            If locations.team_2(t1).tank_displaylist > -1 Then
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
        Gl.glPushMatrix()
        Gl.glBlendEquation(Gl.GL_FUNC_ADD)
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        Gl.glEnable(Gl.GL_BLEND)
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

#Region "pb1 mouse and other functions"

    Private Sub pb1_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs) Handles pb1.PreviewKeyDown
        If e.KeyCode = Keys.F Then
            If pb1.Parent.Name = frmFullScreen.Name Then
                pb1.Parent = Nothing
                frmFullScreen.WindowState = FormWindowState.Normal
                Me.Controls.Add(pb1)
                pb1.Size = old_pb1_size
                pb1.Location = pb1_form_location
                pb1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top
                frmFullScreen.Visible = False
                frmFullScreen.Location = pb1_screen_location
                G_Buffer.init()
                pb1.Focus()
                update_screen()
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
                    G_Buffer.init()
                    update_screen()
                End If
            End If
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
        If block_mouse Then Return

        If SHOW_MAPS Then
            If e.Button = Forms.MouseButtons.Left Then
                If selected_map_hit = 0 And maploaded Then
                    SHOW_MAPS = False
                    draw_scene()
                    draw_scene()
                    Application.DoEvents()
                    Return
                Else
                    Dim dx = selected_map_hit - 1 'deal with posible false hit
                    If dx < 0 Then Return
                    block_mouse = True
                    finish_maps = True
                    mouse.X = 0
                    mouse.Y = 0
                    'SHOW_MAPS = False
                    If maploaded Then
                        save_light_settings() ' save our light settings
                    End If

                    Dim r_name = loadmaplist(dx).realname
                    Dim p_name = loadmaplist(dx).name.Replace(".png", ".pkg")
                    Dim a1 = p_name.Split(":")
                    p_name = a1(0)
                    Dim FileName = GAME_PATH & "\res\packages\" & p_name
                    tb1.text = "loading map: " + r_name + vbCrLf
                    Me.Text = r_name
                    View_Radius = -150
                    look_point_X = 0.0 : look_point_Y = 0.0 : look_point_Z = 0.0
                    m_fly_map.Checked = False
                    water.IsWater = False
                    Dim ar() = FileName.Split("\")
                    Dim sn1 = ar(ar.Length - 1)
                    Dim sn2 = sn1.Split(".")
                    Dim shortname = sn2(0)
                    saved_texture_loads = 0
                    saved_model_loads = 0
                    resetBoundingBox()
                    Gl.glFinish()
                    Application.DoEvents()
                    Application.DoEvents()

                    'if we are hosting a session we need to load 
                    'just like everyone else would.
                    load_map_name = sn1
                    JUST_MAP_NAME = Path.GetFileNameWithoutExtension(sn1)

                    mini_map_loaded = False
                    maploaded = False
                    'block_mouse = False
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
                mouse_moved = True
                activity = True
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
        If block_mouse Then Return
        M_current.x = e.X
        M_current.y = e.Y
        If SHOW_MAPS Then
            mouse_moved = True 'force screen updates
            activity = True 'force screen updates
            mouse.X = e.X
            mouse.Y = e.Y
            Application.DoEvents()
            Return
        End If
        Return
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
            'draw_scene()
        Else
            gl_pick_map(mouse.X, mouse.Y)

        End If
    End Sub
#End Region



    Public Sub position_camera()
        Dim dead As Integer = 0
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
                        look_point_X = tempX : look_point_Z = tempZ
                    Else
                        Cam_X_angle -= t
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
                        look_point_X = tempX : look_point_Z = tempZ
                    Else
                        Cam_X_angle += t
                    End If
                    If Cam_X_angle < 0 Then Cam_X_angle += (2 * PI)
                    mouse.X = e.X

                    Try
                        If maploaded Then
                            Z_Cursor = get_Z_at_XY(look_point_X, look_point_Z)  'maplist(map).heights(rxp, ryp) ' + 1
                            look_point_Y = Z_Cursor ' + 5
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
                        look_point_X = tempX : look_point_Z = tempZ
                    Else
                        Cam_Y_angle -= t
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
                        look_point_X = tempX : look_point_Z = tempZ

                    Else
                        Cam_Y_angle += t
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
                mouse.Y = e.Y
            End If
            If e.Y < (mouse.Y - dead) Then
                If mouse.Y - e.Y > 100 Then t = (10)
            Else : t = CSng(Sin((mouse.Y - e.Y) / 100)) * 12
                Dim tl = View_Radius
                View_Radius -= (t * (View_Radius * 0.2)) ' zoom is factored in to look radius
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
    Dim first_unused_texture As Integer = 0
    Public Sub flush_data()
        M_DOWN = False
        'let the user know whats going on
        'Try
        If first_unused_texture > 0 Then
            tb1.text = "Removing Previous map Data.."
        End If

        Dim er = Gl.glGetError
        Dim top As Integer = 0
        For i = 1 To 3000
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
        'Im so tired of having to change the starting texture to delete from, I'm automating it!
        If first_unused_texture = 0 Then
            Dim ft As Integer
            Gl.glGenTextures(1, ft)
            first_unused_texture = ft
        End If
        Try
            For i = first_unused_texture To 3000
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


#Region "pb2 functions"
    Private Sub pb2_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles pb2.MouseDoubleClick
        If pb2.Parent Is frmShowImage.SPC.Panel1 Then
            frmShowImage.rect_location = New Point(0, 0)
            frmShowImage.draw_(frmShowImage.current_image)
        End If

    End Sub

    Private Sub pb2_MouseDown(sender As Object, e As MouseEventArgs) Handles pb2.MouseDown
        pb2_mouse_down = True
        frmShowImage.mouse_delta = e.Location
    End Sub

    Private Sub pb2_MouseUp(sender As Object, e As MouseEventArgs) Handles pb2.MouseUp
        pb2_mouse_down = False
    End Sub

    Private Sub pb2_MouseWheel(sender As Object, e As MouseEventArgs) Handles pb2.MouseWheel
        If pb2.Parent Is frmShowImage.SPC.Panel1 Then
            frmShowImage.mouse_pos = e.Location
            frmShowImage.mouse_delta = e.Location
            If e.Delta > 0 Then
                frmShowImage.img_scale_up()
            Else
                frmShowImage.img_scale_down()
            End If

        End If
    End Sub

    Private Sub pb2_MouseEnter(sender As Object, e As EventArgs) Handles pb2.MouseEnter
        pb2.Focus()
    End Sub

    Private Sub pb2_MouseLeave(sender As Object, e As EventArgs) Handles pb2.MouseLeave

    End Sub

    Private Sub pb2_Paint(sender As Object, e As PaintEventArgs) Handles pb2.Paint
        If pb2.Parent Is frmShowImage.SPC.Panel1 Then
            If frmShowImage.ready_to_render Then
                frmShowImage.draw_(frmShowImage.current_image)
            End If
        End If
    End Sub

    Private Sub pb2_MouseMove(sender As Object, e As MouseEventArgs) Handles pb2.MouseMove
        If pb2_mouse_down Then
            Dim p As New Point
            p = e.Location - frmShowImage.mouse_delta
            frmShowImage.rect_location += p
            frmShowImage.mouse_delta = e.Location
            frmShowImage.draw_(frmShowImage.current_image)
            Return
        End If
        If pb2.Parent Is frmShowImage.SPC.Panel1 Then
            frmShowImage.mouse_pos = e.Location
            frmShowImage.draw_(frmShowImage.current_image)
            Application.DoEvents()
        End If
    End Sub
#End Region


    Private Sub ToolStripMenuItem15_Click(sender As Object, e As EventArgs) Handles m_edit_shaders.Click
        If Not _STARTED Then
            Return
        End If
        If Not maploaded Then
            Return
        End If

        frmEditFrag.Show()

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

#Region "updater thread functiions"

    Private Sub check_mouse()
        Dim dead As Integer = 0
        Dim t As Double
        Dim M_Speed As Double = 0.8
        Dim tempX, tempZ As Single
        While _STARTED

            tempZ = look_point_Z
            tempX = look_point_X
            Dim ms As Double = 1.0F * View_Radius ' distance away changes speed. THIS WORKS WELL!
            Dim ms2 As Double = 0.25 * View_Radius  ' distance away changes speed. THIS WORKS WELL!
            If M_DOWN Then
                If M_current.x > mouse.X Then
                    If M_current.x - mouse.X > 100 Then t = (1.0F * M_Speed)
                Else : t = CSng(Sin((M_current.x - mouse.X) / 100)) * M_Speed
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
                                mouse.X = M_current.x
                            End If
                        End If
                    End If
                End If
                If M_DOWN Then
                    If M_current.x < mouse.X Then
                        If mouse.X - M_current.x > 100 Then t = (M_Speed)
                    Else : t = CSng(Sin((mouse.X - M_current.x) / 100)) * M_Speed
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
                            mouse.X = M_current.x
                        End If
                    End If
                End If
            End If
            If M_DOWN Then
                If M_current.x > mouse.X Then
                    If M_current.x - mouse.X > 100 Then t = (1.0F * M_Speed)
                Else : t = CSng(Sin((M_current.x - mouse.X) / 100)) * M_Speed
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
                            look_point_X = tempX : look_point_Z = tempZ
                        Else
                            Cam_X_angle -= t
                            If Cam_X_angle > (2 * PI) Then Cam_X_angle -= (2 * PI)
                        End If
                        mouse.X = M_current.x
no_move_xz:
                    End If
                End If
                If M_current.x < mouse.X Then
                    If mouse.X - M_current.x > 100 Then t = (M_Speed)
                Else : t = CSng(Sin((mouse.X - M_current.x) / 100)) * M_Speed
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
                            look_point_X = tempX : look_point_Z = tempZ
                        Else

                            Cam_X_angle += t
                            If Cam_X_angle < 0 Then Cam_X_angle += (2 * PI)
                        End If
                        mouse.X = M_current.x

                        Try
                            If maploaded Then
                                Z_Cursor = get_Z_at_XY(look_point_X, look_point_Z)  'maplist(map).heights(rxp, ryp) ' + 1
                                look_point_Y = Z_Cursor ' + 5
                            End If
                        Catch ex As Exception

                        End Try
                    End If
                End If
                If M_current.y > mouse.Y Then
                    If M_current.y - mouse.Y > 100 Then t = (M_Speed)
                Else : t = CSng(Sin((M_current.y - mouse.Y) / 100)) * M_Speed
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
                            look_point_X = tempX : look_point_Z = tempZ

                        Else
                            If Not ROTATE_TANK And Not MOVE_TANK Then
                                Cam_Y_angle -= t
                            End If
                        End If
                    End If
                    mouse.Y = M_current.y
                End If
                If M_current.y < mouse.Y Then
                    If mouse.Y - M_current.y > 100 Then t = (M_Speed)
                Else : t = CSng(Sin((mouse.Y - M_current.y) / 100)) * M_Speed
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
                            look_point_X = tempX : look_point_Z = tempZ

                        Else
                            If Not ROTATE_TANK And Not MOVE_TANK Then
                                Cam_Y_angle += t
                            End If
                        End If
                    End If
                    mouse.Y = M_current.y
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
                If M_current.y > mouse.Y Then
                    If M_current.y - mouse.Y > 100 Then t = (10)
                Else : t = CSng(Sin((M_current.y - mouse.Y) / 100)) * 12
                    Dim tl = View_Radius
                    View_Radius += (t * (View_Radius * 0.2))    ' zoom is factored in to look radius
                    mouse.Y = M_current.y
                End If
                If M_current.y < mouse.Y Then
                    If mouse.Y - M_current.y > 100 Then t = (10)
                Else : t = CSng(Sin((mouse.Y - M_current.y) / 100)) * 12
                    Dim tl = View_Radius
                    View_Radius -= (t * (View_Radius * 0.2))    ' zoom is factored in to look radius
                    If View_Radius > -5.0 Then View_Radius = -5.0
                    mouse.Y = M_current.y
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
            mouse_moved = need_update()
            Thread.Sleep(1)
        End While

    End Sub
    Private mouse_moved As Boolean = False
    Public activity As Boolean = True
    Dim timeout As Integer
    Private Sub screen_updater()
        'This will run for the duration that Terra! is open.
        'Its in a closed loop
        Dim swat As New Stopwatch
        While _STARTED
            If mouse_moved Then
                swat.Start()
                activity = True
                timeout = 0
            End If
            If activity And Not stopGL Then
                timeout = fly(timeout)
                'If we need to update the screen, lets caclulate draw times and update the timer.
                If swat.ElapsedMilliseconds >= 1000 Then
                    timeout += 1
                    If timeout >= 10 Then
                        activity = False
                        screen_totaled_draw_time = 0
                        swat.Stop()
                    Else
                        swat.Restart()
                        screen_totaled_draw_time = screen_avg_counter
                        screen_avg_counter = 1.0!
                    End If
                Else
                    screen_avg_counter += 1.0!
                End If
                If SHOW_MAPS Then
                    draw_maps_buttons()
                Else
                    update_screen()
                End If
                autoEventScreen.WaitOne(300)
            End If
            Thread.Sleep(3)
        End While
    End Sub

    Private Delegate Sub update_screen_delegate()
    Private lockdrawing As New Object
    Public Sub update_screen()

        Try
            If Me.InvokeRequired Then
                SyncLock lockdrawing
                    Me.Invoke(New update_screen_delegate(AddressOf update_screen))
                End SyncLock
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
            timeout = 0
            activity = True
        Else
            'position(0) = old_light_position.x
            'position(1) = old_light_position.y
            'position(2) = old_light_position.z
        End If
    End Sub
    Dim lx_light, ly_light, lz_light As Single
    Public Function fly(ByVal time_out As Integer) As Integer
        'Dim swat As New Stopwatch
        'While m_fly_map.Checked Or m_Orbit_Light.Checked
        If stopGL Then
            Return False
        End If
        'scale = 700.0
        If m_fly_map.Checked And maploaded Then
            'FLY_ = True
            time_out = 0
            view_rot += 0.002
            look_point_X = Cos(view_rot) * MAP_BB_UR.x - 50.0
            look_point_Z = Sin(view_rot) * MAP_BB_UR.x - 50.0
            cam_x = Cos(view_rot - 0.01) * MAP_BB_UR.x - 50.0
            cam_z = Sin(view_rot - 0.01) * MAP_BB_UR.x - 50.0
            Z_Cursor = get_Z_at_XY(look_point_X, look_point_Z)  ' + 1
            cam_y = Z_Cursor + 15
            look_point_Y = cam_y
            angle_offset = -view_rot - (PI * 0.1)


            If view_rot > 2 * PI Then
                view_rot -= (2 * PI)
            End If
            Application.DoEvents()
        Else
            angle_offset = 0
        End If
        If m_Orbit_Light.Checked And maploaded Then
            time_out = 0
            'scale = 1000.0
            light_rot += 0.015
            Dim radius As Single = MAP_BB_UR.x
            lx_light = Cos(light_rot) * radius
            lz_light = Sin(light_rot) * radius
            ly_light = 400.0 'Z_Cursor = get_Z_at_XY(look_point_X, look_point_Z) ' + 1
            'ly_light = get_Z_at_XY(lx_light, lz_light) + 150.0
            position(0) = lx_light 'u_look_point_X - lx
            position(1) = ly_light 'u_look_point_Y + 10 'ly
            position(2) = lz_light 'u_look_point_Z - lz
            position(3) = 1.0

            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position)
            If light_rot > 2 * PI Then
                light_rot -= (2 * PI)
            End If
        Else

        End If
        Return time_out

    End Function

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Enabled = False
        update_thread.IsBackground = True
        update_thread.Name = "Screen updater"
        update_thread.Priority = ThreadPriority.Highest
        update_thread.Start()

        mouse_update_thread.Name = "Check mouse for movement"
        mouse_update_thread.Priority = ThreadPriority.Highest
        mouse_update_thread.IsBackground = True
        mouse_update_thread.Start()
    End Sub
#End Region
#Region "menu clicks"



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



    Private Sub m_layout_mode_CheckedChanged(sender As Object, e As EventArgs) Handles m_layout_mode.CheckedChanged
        If Not _STARTED Then Return
        stopGL = True
        While gl_busy
            Application.DoEvents()
        End While
        If m_layout_mode.Checked Then
            m_comment.Visible = True
            m_clear_tank_comments.Visible = True
            m_comment.AllowDrop = True
            m_reset_tanks.Visible = True
            frmTanks.Show()
        Else
            m_comment.Visible = False
            m_clear_tank_comments.Visible = False
            m_reset_tanks.Visible = False
            frmTanks.Close()
        End If
        stopGL = False
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

    Private Sub m_show_fog_Click(sender As Object, e As EventArgs)
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
            FrmInfoWindow.Visible = True
            pb1.Focus()
        Else
            FrmInfoWindow.Visible = False
            pb1.Focus()
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
        If m_fly_map.Checked Then
            timeout = 0
            activity = True
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


    Private Sub m_developer_CheckedChanged(sender As Object, e As EventArgs) Handles m_developer.CheckedChanged
        'Warn user than show developer tools.
        If Not _STARTED Then
            Return
        End If
        If m_developer.Checked Then
            m_edit_shaders.Visible = True
            m_find_Item_menu.Visible = True
            m_post_effect_viewer.Visible = True
            m_load_options.Visible = True
        Else
            m_edit_shaders.Visible = False
            m_find_Item_menu.Visible = False
            m_post_effect_viewer.Visible = False
            m_load_options.Visible = False
        End If
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
        update_screen()
    End Sub

    Private Sub m_mini_up_Click(sender As Object, e As EventArgs) Handles m_mini_up.Click
        If Not _STARTED Then Return
        minimap_size += 32.0!
        If minimap_size > 640.0! Then
            minimap_size = 640.0!
        End If
        'tb1.Text = "Minimap size: " + minimap_size.ToString
        My.Settings.minimap_size = minimap_size
        update_screen()

    End Sub


    Private Sub m_minizoom_Click(sender As Object, e As EventArgs) Handles m_minizoom.Click
        If Not _STARTED Then Return
        m_show_minimap.Checked = m_minizoom.Checked
        update_screen()
    End Sub
    Private Sub m_mini_down_Click(sender As Object, e As EventArgs) Handles m_mini_down.Click
        If Not _STARTED Then Return
        minimap_size -= 32.0!
        If minimap_size <= 160.0! Then
            minimap_size = 160.0!
        End If
        My.Settings.minimap_size = minimap_size
        update_screen()
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



    Private Sub m_post_effect_viewer_Click(sender As Object, e As EventArgs) Handles m_post_effect_viewer.Click
        frmTestView.Visible = True
    End Sub

    Private Sub m_render_stats_Click(sender As Object, e As EventArgs) Handles m_render_stats.Click
        frmStats.Show()
    End Sub


    Private Sub m_map_info_Click(sender As Object, e As EventArgs) Handles m_map_info.Click
        frmMapInfo.Show()
    End Sub

    Private Sub m_load_options_Click(sender As Object, e As EventArgs) Handles m_load_options.Click
        frmLoadOptions.Show()
    End Sub

    Private Sub m_FXAA_CheckedChanged(sender As Object, e As EventArgs) Handles m_FXAA.CheckedChanged
        If Not _STARTED Then Return
        update_screen()
    End Sub

    Private Sub m_small_lights_CheckedChanged(sender As Object, e As EventArgs) Handles m_small_lights.CheckedChanged
        If Not _STARTED Then Return
        If LIGHT_COUNT_ = 0 Then
            make_lights()
            If maploaded Then
                find_street_lights()
            End If
        End If
        update_screen()
    End Sub

#End Region


End Class

