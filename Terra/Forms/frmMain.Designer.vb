<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.pb1 = New System.Windows.Forms.Panel()
        Me.pb2 = New System.Windows.Forms.Panel()
        Me.pb4 = New System.Windows.Forms.Panel()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.font_holder = New System.Windows.Forms.Label()
        Me.OpenFileDialog2 = New System.Windows.Forms.OpenFileDialog()
        Me.mainMenu = New System.Windows.Forms.MenuStrip()
        Me.m_file = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_load_map = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem10 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_set_path = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_save = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_load = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_load_old = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem7 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_exit = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_map_info = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_render_stats = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_developer = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_post_effect_viewer = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_load_options = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_settings = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_lighting = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem13 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_small_lights = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_FXAA = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_SSAO = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_g_settings = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_tank_names = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_tank_comments = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_show_models = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_trees = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_water = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_decals = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_cursor = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem8 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_map_border = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_map_grid = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_chunks = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_chuckIds = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem9 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_high_rez_Terrain = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_show_uv2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_load_lod = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_load_details = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_low_quality_textures = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_low_quality_trees = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_wire_models = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_wire_trees = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_wire_decals = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_wire_terrain = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_constant_updates = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem14 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_show_minimap = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_status = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_info_window = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem11 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_fly_map = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_Orbit_Light = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_mouseSpeed = New System.Windows.Forms.ToolStripComboBox()
        Me.m_session = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_host_session = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_join_session = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem12 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_join_server_as_host = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_layout_mode = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_reset_tanks = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_chat = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_render_to_bitmap = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_comment = New System.Windows.Forms.ToolStripTextBox()
        Me.m_clear_tank_comments = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_minizoom = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_mini_down = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_find_Item_menu = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_edit_shaders = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_help = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_mini_up = New System.Windows.Forms.ToolStripMenuItem()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.pb1.SuspendLayout()
        Me.mainMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'pb1
        '
        Me.pb1.AllowDrop = True
        Me.pb1.BackColor = System.Drawing.Color.DimGray
        Me.pb1.Controls.Add(Me.pb2)
        Me.pb1.Controls.Add(Me.pb4)
        resources.ApplyResources(Me.pb1, "pb1")
        Me.pb1.ForeColor = System.Drawing.Color.Black
        Me.pb1.Name = "pb1"
        '
        'pb2
        '
        Me.pb2.Cursor = System.Windows.Forms.Cursors.Cross
        resources.ApplyResources(Me.pb2, "pb2")
        Me.pb2.Name = "pb2"
        '
        'pb4
        '
        Me.pb4.Cursor = System.Windows.Forms.Cursors.SizeAll
        resources.ApplyResources(Me.pb4, "pb4")
        Me.pb4.Name = "pb4"
        '
        'ProgressBar1
        '
        resources.ApplyResources(Me.ProgressBar1, "ProgressBar1")
        Me.ProgressBar1.Name = "ProgressBar1"
        '
        'font_holder
        '
        resources.ApplyResources(Me.font_holder, "font_holder")
        Me.font_holder.ForeColor = System.Drawing.Color.White
        Me.font_holder.Name = "font_holder"
        '
        'OpenFileDialog2
        '
        resources.ApplyResources(Me.OpenFileDialog2, "OpenFileDialog2")
        Me.OpenFileDialog2.InitialDirectory = "C:\"
        '
        'mainMenu
        '
        resources.ApplyResources(Me.mainMenu, "mainMenu")
        Me.mainMenu.BackColor = System.Drawing.SystemColors.MenuBar
        Me.mainMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_file, Me.m_settings, Me.m_session, Me.m_layout_mode, Me.m_reset_tanks, Me.m_show_chat, Me.m_render_to_bitmap, Me.m_comment, Me.m_clear_tank_comments, Me.m_minizoom, Me.m_mini_down, Me.m_find_Item_menu, Me.m_edit_shaders, Me.m_help, Me.m_mini_up})
        Me.mainMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.mainMenu.Name = "mainMenu"
        '
        'm_file
        '
        Me.m_file.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_load_map, Me.ToolStripMenuItem10, Me.m_set_path, Me.ToolStripMenuItem3, Me.m_save, Me.m_load, Me.m_load_old, Me.ToolStripMenuItem7, Me.m_exit, Me.ToolStripSeparator6, Me.m_map_info, Me.m_render_stats, Me.m_developer, Me.m_post_effect_viewer, Me.m_load_options})
        Me.m_file.ForeColor = System.Drawing.Color.Black
        Me.m_file.Name = "m_file"
        resources.ApplyResources(Me.m_file, "m_file")
        '
        'm_load_map
        '
        Me.m_load_map.Name = "m_load_map"
        resources.ApplyResources(Me.m_load_map, "m_load_map")
        '
        'ToolStripMenuItem10
        '
        Me.ToolStripMenuItem10.Name = "ToolStripMenuItem10"
        resources.ApplyResources(Me.ToolStripMenuItem10, "ToolStripMenuItem10")
        '
        'm_set_path
        '
        Me.m_set_path.Name = "m_set_path"
        resources.ApplyResources(Me.m_set_path, "m_set_path")
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        resources.ApplyResources(Me.ToolStripMenuItem3, "ToolStripMenuItem3")
        '
        'm_save
        '
        Me.m_save.Name = "m_save"
        resources.ApplyResources(Me.m_save, "m_save")
        '
        'm_load
        '
        Me.m_load.Name = "m_load"
        resources.ApplyResources(Me.m_load, "m_load")
        '
        'm_load_old
        '
        Me.m_load_old.Name = "m_load_old"
        resources.ApplyResources(Me.m_load_old, "m_load_old")
        '
        'ToolStripMenuItem7
        '
        Me.ToolStripMenuItem7.Name = "ToolStripMenuItem7"
        resources.ApplyResources(Me.ToolStripMenuItem7, "ToolStripMenuItem7")
        '
        'm_exit
        '
        Me.m_exit.Name = "m_exit"
        resources.ApplyResources(Me.m_exit, "m_exit")
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        resources.ApplyResources(Me.ToolStripSeparator6, "ToolStripSeparator6")
        '
        'm_map_info
        '
        Me.m_map_info.Name = "m_map_info"
        resources.ApplyResources(Me.m_map_info, "m_map_info")
        '
        'm_render_stats
        '
        Me.m_render_stats.Name = "m_render_stats"
        resources.ApplyResources(Me.m_render_stats, "m_render_stats")
        '
        'm_developer
        '
        Me.m_developer.CheckOnClick = True
        Me.m_developer.Name = "m_developer"
        resources.ApplyResources(Me.m_developer, "m_developer")
        '
        'm_post_effect_viewer
        '
        Me.m_post_effect_viewer.Name = "m_post_effect_viewer"
        resources.ApplyResources(Me.m_post_effect_viewer, "m_post_effect_viewer")
        '
        'm_load_options
        '
        Me.m_load_options.Name = "m_load_options"
        resources.ApplyResources(Me.m_load_options, "m_load_options")
        '
        'm_settings
        '
        Me.m_settings.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_lighting, Me.ToolStripMenuItem13, Me.m_small_lights, Me.m_FXAA, Me.m_SSAO, Me.m_g_settings, Me.ToolStripSeparator8, Me.m_constant_updates, Me.ToolStripMenuItem14, Me.m_show_minimap, Me.m_show_status, Me.m_info_window, Me.ToolStripMenuItem11, Me.m_fly_map, Me.m_Orbit_Light, Me.m_mouseSpeed})
        Me.m_settings.ForeColor = System.Drawing.Color.Black
        Me.m_settings.Name = "m_settings"
        resources.ApplyResources(Me.m_settings, "m_settings")
        '
        'm_lighting
        '
        Me.m_lighting.Name = "m_lighting"
        resources.ApplyResources(Me.m_lighting, "m_lighting")
        '
        'ToolStripMenuItem13
        '
        Me.ToolStripMenuItem13.Name = "ToolStripMenuItem13"
        resources.ApplyResources(Me.ToolStripMenuItem13, "ToolStripMenuItem13")
        '
        'm_small_lights
        '
        Me.m_small_lights.Checked = Global.Terra.My.MySettings.Default.m_small_lights
        Me.m_small_lights.CheckOnClick = True
        Me.m_small_lights.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_small_lights.Name = "m_small_lights"
        resources.ApplyResources(Me.m_small_lights, "m_small_lights")
        '
        'm_FXAA
        '
        Me.m_FXAA.Checked = Global.Terra.My.MySettings.Default.m_FXAA
        Me.m_FXAA.CheckOnClick = True
        Me.m_FXAA.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_FXAA.Name = "m_FXAA"
        resources.ApplyResources(Me.m_FXAA, "m_FXAA")
        '
        'm_SSAO
        '
        Me.m_SSAO.Checked = Global.Terra.My.MySettings.Default.SSAO
        Me.m_SSAO.CheckOnClick = True
        Me.m_SSAO.Name = "m_SSAO"
        resources.ApplyResources(Me.m_SSAO, "m_SSAO")
        '
        'm_g_settings
        '
        Me.m_g_settings.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_show_tank_names, Me.m_show_tank_comments, Me.ToolStripSeparator4, Me.m_show_models, Me.m_show_trees, Me.m_show_water, Me.m_show_decals, Me.m_show_cursor, Me.ToolStripMenuItem8, Me.m_map_border, Me.m_show_map_grid, Me.m_show_chunks, Me.m_show_chuckIds, Me.ToolStripMenuItem9, Me.m_high_rez_Terrain, Me.ToolStripSeparator5, Me.m_show_uv2, Me.m_load_lod, Me.m_load_details, Me.m_low_quality_textures, Me.m_low_quality_trees, Me.ToolStripSeparator7, Me.m_wire_models, Me.m_wire_trees, Me.m_wire_decals, Me.m_wire_terrain})
        Me.m_g_settings.Name = "m_g_settings"
        resources.ApplyResources(Me.m_g_settings, "m_g_settings")
        '
        'm_show_tank_names
        '
        Me.m_show_tank_names.Checked = True
        Me.m_show_tank_names.CheckOnClick = True
        Me.m_show_tank_names.CheckState = Global.Terra.My.MySettings.Default.m_show_tank_names
        Me.m_show_tank_names.Name = "m_show_tank_names"
        resources.ApplyResources(Me.m_show_tank_names, "m_show_tank_names")
        '
        'm_show_tank_comments
        '
        Me.m_show_tank_comments.Checked = True
        Me.m_show_tank_comments.CheckOnClick = True
        Me.m_show_tank_comments.CheckState = Global.Terra.My.MySettings.Default.m_show_tank_comments
        Me.m_show_tank_comments.Name = "m_show_tank_comments"
        resources.ApplyResources(Me.m_show_tank_comments, "m_show_tank_comments")
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        resources.ApplyResources(Me.ToolStripSeparator4, "ToolStripSeparator4")
        '
        'm_show_models
        '
        Me.m_show_models.Checked = True
        Me.m_show_models.CheckOnClick = True
        Me.m_show_models.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_show_models.Name = "m_show_models"
        resources.ApplyResources(Me.m_show_models, "m_show_models")
        '
        'm_show_trees
        '
        Me.m_show_trees.Checked = True
        Me.m_show_trees.CheckOnClick = True
        Me.m_show_trees.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_show_trees.Name = "m_show_trees"
        resources.ApplyResources(Me.m_show_trees, "m_show_trees")
        '
        'm_show_water
        '
        Me.m_show_water.Checked = True
        Me.m_show_water.CheckOnClick = True
        Me.m_show_water.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_show_water.Name = "m_show_water"
        resources.ApplyResources(Me.m_show_water, "m_show_water")
        '
        'm_show_decals
        '
        Me.m_show_decals.Checked = Global.Terra.My.MySettings.Default.m_show_decals
        Me.m_show_decals.CheckOnClick = True
        Me.m_show_decals.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_show_decals.Name = "m_show_decals"
        resources.ApplyResources(Me.m_show_decals, "m_show_decals")
        '
        'm_show_cursor
        '
        Me.m_show_cursor.CheckOnClick = True
        Me.m_show_cursor.Name = "m_show_cursor"
        resources.ApplyResources(Me.m_show_cursor, "m_show_cursor")
        '
        'ToolStripMenuItem8
        '
        Me.ToolStripMenuItem8.Name = "ToolStripMenuItem8"
        resources.ApplyResources(Me.ToolStripMenuItem8, "ToolStripMenuItem8")
        '
        'm_map_border
        '
        Me.m_map_border.CheckOnClick = True
        Me.m_map_border.Name = "m_map_border"
        resources.ApplyResources(Me.m_map_border, "m_map_border")
        '
        'm_show_map_grid
        '
        Me.m_show_map_grid.CheckOnClick = True
        Me.m_show_map_grid.Name = "m_show_map_grid"
        resources.ApplyResources(Me.m_show_map_grid, "m_show_map_grid")
        '
        'm_show_chunks
        '
        Me.m_show_chunks.CheckOnClick = True
        Me.m_show_chunks.Name = "m_show_chunks"
        resources.ApplyResources(Me.m_show_chunks, "m_show_chunks")
        '
        'm_show_chuckIds
        '
        Me.m_show_chuckIds.CheckOnClick = True
        Me.m_show_chuckIds.Name = "m_show_chuckIds"
        resources.ApplyResources(Me.m_show_chuckIds, "m_show_chuckIds")
        '
        'ToolStripMenuItem9
        '
        Me.ToolStripMenuItem9.Name = "ToolStripMenuItem9"
        resources.ApplyResources(Me.ToolStripMenuItem9, "ToolStripMenuItem9")
        '
        'm_high_rez_Terrain
        '
        Me.m_high_rez_Terrain.Checked = Global.Terra.My.MySettings.Default.hi_rez_terra
        Me.m_high_rez_Terrain.CheckOnClick = True
        Me.m_high_rez_Terrain.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_high_rez_Terrain.Name = "m_high_rez_Terrain"
        resources.ApplyResources(Me.m_high_rez_Terrain, "m_high_rez_Terrain")
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        resources.ApplyResources(Me.ToolStripSeparator5, "ToolStripSeparator5")
        '
        'm_show_uv2
        '
        Me.m_show_uv2.Checked = Global.Terra.My.MySettings.Default.m_show_uv2
        Me.m_show_uv2.CheckOnClick = True
        Me.m_show_uv2.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_show_uv2.Name = "m_show_uv2"
        resources.ApplyResources(Me.m_show_uv2, "m_show_uv2")
        '
        'm_load_lod
        '
        Me.m_load_lod.Checked = Global.Terra.My.MySettings.Default.lod0
        Me.m_load_lod.CheckOnClick = True
        Me.m_load_lod.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_load_lod.Name = "m_load_lod"
        resources.ApplyResources(Me.m_load_lod, "m_load_lod")
        '
        'm_load_details
        '
        Me.m_load_details.Checked = Global.Terra.My.MySettings.Default.load_extra
        Me.m_load_details.CheckOnClick = True
        Me.m_load_details.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_load_details.Name = "m_load_details"
        resources.ApplyResources(Me.m_load_details, "m_load_details")
        '
        'm_low_quality_textures
        '
        Me.m_low_quality_textures.Checked = Global.Terra.My.MySettings.Default.txt_256
        Me.m_low_quality_textures.CheckOnClick = True
        Me.m_low_quality_textures.Name = "m_low_quality_textures"
        resources.ApplyResources(Me.m_low_quality_textures, "m_low_quality_textures")
        '
        'm_low_quality_trees
        '
        Me.m_low_quality_trees.CheckOnClick = True
        Me.m_low_quality_trees.CheckState = Global.Terra.My.MySettings.Default.low_q_trees
        Me.m_low_quality_trees.Name = "m_low_quality_trees"
        resources.ApplyResources(Me.m_low_quality_trees, "m_low_quality_trees")
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        resources.ApplyResources(Me.ToolStripSeparator7, "ToolStripSeparator7")
        '
        'm_wire_models
        '
        Me.m_wire_models.CheckOnClick = True
        Me.m_wire_models.Name = "m_wire_models"
        resources.ApplyResources(Me.m_wire_models, "m_wire_models")
        '
        'm_wire_trees
        '
        Me.m_wire_trees.CheckOnClick = True
        Me.m_wire_trees.Name = "m_wire_trees"
        resources.ApplyResources(Me.m_wire_trees, "m_wire_trees")
        '
        'm_wire_decals
        '
        Me.m_wire_decals.CheckOnClick = True
        Me.m_wire_decals.Name = "m_wire_decals"
        resources.ApplyResources(Me.m_wire_decals, "m_wire_decals")
        '
        'm_wire_terrain
        '
        Me.m_wire_terrain.CheckOnClick = True
        Me.m_wire_terrain.Name = "m_wire_terrain"
        resources.ApplyResources(Me.m_wire_terrain, "m_wire_terrain")
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        resources.ApplyResources(Me.ToolStripSeparator8, "ToolStripSeparator8")
        '
        'm_constant_updates
        '
        Me.m_constant_updates.Checked = Global.Terra.My.MySettings.Default.m_constant_updates
        Me.m_constant_updates.CheckOnClick = True
        Me.m_constant_updates.Name = "m_constant_updates"
        resources.ApplyResources(Me.m_constant_updates, "m_constant_updates")
        '
        'ToolStripMenuItem14
        '
        Me.ToolStripMenuItem14.Name = "ToolStripMenuItem14"
        resources.ApplyResources(Me.ToolStripMenuItem14, "ToolStripMenuItem14")
        '
        'm_show_minimap
        '
        Me.m_show_minimap.Checked = True
        Me.m_show_minimap.CheckOnClick = True
        Me.m_show_minimap.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_show_minimap.Name = "m_show_minimap"
        resources.ApplyResources(Me.m_show_minimap, "m_show_minimap")
        '
        'm_show_status
        '
        Me.m_show_status.Checked = True
        Me.m_show_status.CheckOnClick = True
        Me.m_show_status.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_show_status.Name = "m_show_status"
        resources.ApplyResources(Me.m_show_status, "m_show_status")
        '
        'm_info_window
        '
        Me.m_info_window.Checked = True
        Me.m_info_window.CheckOnClick = True
        Me.m_info_window.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_info_window.Name = "m_info_window"
        resources.ApplyResources(Me.m_info_window, "m_info_window")
        '
        'ToolStripMenuItem11
        '
        Me.ToolStripMenuItem11.Name = "ToolStripMenuItem11"
        resources.ApplyResources(Me.ToolStripMenuItem11, "ToolStripMenuItem11")
        '
        'm_fly_map
        '
        Me.m_fly_map.CheckOnClick = True
        Me.m_fly_map.Name = "m_fly_map"
        resources.ApplyResources(Me.m_fly_map, "m_fly_map")
        '
        'm_Orbit_Light
        '
        Me.m_Orbit_Light.CheckOnClick = True
        Me.m_Orbit_Light.Name = "m_Orbit_Light"
        resources.ApplyResources(Me.m_Orbit_Light, "m_Orbit_Light")
        '
        'm_mouseSpeed
        '
        Me.m_mouseSpeed.Items.AddRange(New Object() {resources.GetString("m_mouseSpeed.Items"), resources.GetString("m_mouseSpeed.Items1"), resources.GetString("m_mouseSpeed.Items2"), resources.GetString("m_mouseSpeed.Items3"), resources.GetString("m_mouseSpeed.Items4"), resources.GetString("m_mouseSpeed.Items5"), resources.GetString("m_mouseSpeed.Items6"), resources.GetString("m_mouseSpeed.Items7"), resources.GetString("m_mouseSpeed.Items8"), resources.GetString("m_mouseSpeed.Items9")})
        Me.m_mouseSpeed.Name = "m_mouseSpeed"
        resources.ApplyResources(Me.m_mouseSpeed, "m_mouseSpeed")
        Me.m_mouseSpeed.Text = Global.Terra.My.MySettings.Default.mouse_speed_text
        '
        'm_session
        '
        Me.m_session.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_host_session, Me.ToolStripSeparator1, Me.m_join_session, Me.ToolStripMenuItem12, Me.ToolStripSeparator2, Me.ToolStripSeparator3, Me.m_join_server_as_host})
        Me.m_session.ForeColor = System.Drawing.Color.Black
        Me.m_session.Name = "m_session"
        resources.ApplyResources(Me.m_session, "m_session")
        '
        'm_host_session
        '
        Me.m_host_session.Name = "m_host_session"
        resources.ApplyResources(Me.m_host_session, "m_host_session")
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'm_join_session
        '
        Me.m_join_session.Name = "m_join_session"
        resources.ApplyResources(Me.m_join_session, "m_join_session")
        '
        'ToolStripMenuItem12
        '
        Me.ToolStripMenuItem12.Name = "ToolStripMenuItem12"
        resources.ApplyResources(Me.ToolStripMenuItem12, "ToolStripMenuItem12")
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        resources.ApplyResources(Me.ToolStripSeparator3, "ToolStripSeparator3")
        '
        'm_join_server_as_host
        '
        Me.m_join_server_as_host.Name = "m_join_server_as_host"
        resources.ApplyResources(Me.m_join_server_as_host, "m_join_server_as_host")
        '
        'm_layout_mode
        '
        Me.m_layout_mode.CheckOnClick = True
        Me.m_layout_mode.ForeColor = System.Drawing.Color.Black
        Me.m_layout_mode.Name = "m_layout_mode"
        resources.ApplyResources(Me.m_layout_mode, "m_layout_mode")
        '
        'm_reset_tanks
        '
        Me.m_reset_tanks.ForeColor = System.Drawing.Color.Black
        Me.m_reset_tanks.Name = "m_reset_tanks"
        resources.ApplyResources(Me.m_reset_tanks, "m_reset_tanks")
        '
        'm_show_chat
        '
        Me.m_show_chat.Checked = True
        Me.m_show_chat.CheckOnClick = True
        Me.m_show_chat.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_show_chat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_show_chat.ForeColor = System.Drawing.Color.Black
        Me.m_show_chat.Name = "m_show_chat"
        resources.ApplyResources(Me.m_show_chat, "m_show_chat")
        '
        'm_render_to_bitmap
        '
        Me.m_render_to_bitmap.ForeColor = System.Drawing.Color.Black
        Me.m_render_to_bitmap.Name = "m_render_to_bitmap"
        resources.ApplyResources(Me.m_render_to_bitmap, "m_render_to_bitmap")
        '
        'm_comment
        '
        resources.ApplyResources(Me.m_comment, "m_comment")
        Me.m_comment.BackColor = System.Drawing.Color.Black
        Me.m_comment.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_comment.ForeColor = System.Drawing.Color.White
        Me.m_comment.Name = "m_comment"
        '
        'm_clear_tank_comments
        '
        Me.m_clear_tank_comments.Name = "m_clear_tank_comments"
        resources.ApplyResources(Me.m_clear_tank_comments, "m_clear_tank_comments")
        '
        'm_minizoom
        '
        Me.m_minizoom.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_minizoom.Checked = True
        Me.m_minizoom.CheckOnClick = True
        Me.m_minizoom.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_minizoom.Name = "m_minizoom"
        resources.ApplyResources(Me.m_minizoom, "m_minizoom")
        '
        'm_mini_down
        '
        Me.m_mini_down.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_mini_down.Image = Global.Terra.My.Resources.Resources.control_270
        Me.m_mini_down.Name = "m_mini_down"
        resources.ApplyResources(Me.m_mini_down, "m_mini_down")
        '
        'm_find_Item_menu
        '
        Me.m_find_Item_menu.Name = "m_find_Item_menu"
        resources.ApplyResources(Me.m_find_Item_menu, "m_find_Item_menu")
        '
        'm_edit_shaders
        '
        Me.m_edit_shaders.Name = "m_edit_shaders"
        resources.ApplyResources(Me.m_edit_shaders, "m_edit_shaders")
        '
        'm_help
        '
        Me.m_help.ForeColor = System.Drawing.Color.Black
        Me.m_help.Name = "m_help"
        resources.ApplyResources(Me.m_help, "m_help")
        '
        'm_mini_up
        '
        Me.m_mini_up.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_mini_up.Image = Global.Terra.My.Resources.Resources.control_090
        Me.m_mini_up.Name = "m_mini_up"
        resources.ApplyResources(Me.m_mini_up, "m_mini_up")
        '
        'Timer1
        '
        Me.Timer1.Interval = 500
        '
        'OpenFileDialog1
        '
        resources.ApplyResources(Me.OpenFileDialog1, "OpenFileDialog1")
        Me.OpenFileDialog1.InitialDirectory = Global.Terra.My.MySettings.Default.game_path
        '
        'SaveFileDialog1
        '
        resources.ApplyResources(Me.SaveFileDialog1, "SaveFileDialog1")
        Me.SaveFileDialog1.InitialDirectory = Global.Terra.My.MySettings.Default.game_path
        '
        'frmMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.Controls.Add(Me.font_holder)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.pb1)
        Me.Controls.Add(Me.mainMenu)
        Me.ForeColor = System.Drawing.Color.Yellow
        Me.Name = "frmMain"
        Me.pb1.ResumeLayout(False)
        Me.mainMenu.ResumeLayout(False)
        Me.mainMenu.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pb1 As System.Windows.Forms.Panel
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents OpenFileDialog2 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents mainMenu As System.Windows.Forms.MenuStrip
    Friend WithEvents m_file As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_load_map As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_exit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_settings As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_lighting As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_g_settings As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_session As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_host_session As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_join_session As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem12 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_help As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_show_models As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_show_trees As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_show_water As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_show_cursor As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ToolStripMenuItem8 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents m_show_map_grid As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_show_chunks As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ToolStripMenuItem5 As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ToolStripMenuItem6 As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ToolStripMenuItem9 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_load_details As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_show_chuckIds As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_low_quality_textures As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_low_quality_trees As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_fly_map As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_layout_mode As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_reset_tanks As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents font_holder As System.Windows.Forms.Label
	Friend WithEvents ToolStripMenuItem10 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents m_set_path As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents m_save As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_load As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ToolStripMenuItem7 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
	Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
	Friend WithEvents m_show_chat As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_info_window As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ToolStripMenuItem13 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents ToolStripMenuItem14 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents ToolStripMenuItem11 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents m_map_border As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents m_join_server_as_host As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_show_tank_names As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_comment As System.Windows.Forms.ToolStripTextBox
	Friend WithEvents m_show_tank_comments As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents m_render_to_bitmap As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_find_Item_menu As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents pb2 As System.Windows.Forms.Panel
	Friend WithEvents m_edit_shaders As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_high_rez_Terrain As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents m_load_lod As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_Orbit_Light As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents m_developer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_clear_tank_comments As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_show_uv2 As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_show_minimap As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_minizoom As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_mini_up As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents m_mini_down As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_show_status As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_wire_trees As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_wire_decals As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_wire_terrain As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_wire_models As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_show_decals As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_post_effect_viewer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_render_stats As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_map_info As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_load_options As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents pb4 As System.Windows.Forms.Panel
    Friend WithEvents m_small_lights As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_FXAA As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_SSAO As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_constant_updates As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_load_old As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_mouseSpeed As System.Windows.Forms.ToolStripComboBox

End Class
