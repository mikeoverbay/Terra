Imports System.Xml
Imports Tao.FreeGlut.Glut
Imports System.Math
Imports Lidgren.Network
Imports System.Windows.Media.Media3D
Module Globals
    '==============
    Public gui_pkg = New Ionic.Zip.ZipFile
    Public global_map_width As Integer
    Public triangle_count As Integer
    Public m_decals_, m_terrain_, m_trees_, m_sky_, m_water_, m_bases_, m_models_ As Boolean
    Public map_odd As Boolean = False
    '===============
    Public terrain_loaded As Boolean
    Public trees_loaded As Boolean
    Public decals_loaded As Boolean
    Public models_loaded As Boolean
    Public bases_loaded As Boolean
    Public sky_loaded As Boolean
    Public water_loaded As Boolean
    Public animated_water_ids(64) As Integer
    Public map_x_min, map_x_max, map_y_max, map_y_min As Single
    Public map_center_offset_x, map_center_offset_y As Single
    Public noise_map_id As Integer
    Public smrs As Int32 = 256
    Public numer() = {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0"}
    Public alpha() = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J"}
    Public testing As Boolean = True
    Public old_pb1_size As System.Drawing.Size
    Public pb1_screen_location As System.Drawing.Point
    Public pb1_form_location As System.Drawing.Point
    Public bias() As Single = {0.5, 0.0, 0.0, 0.0, 0.0, 0.5, 0.0, 0.0, 0.0, 0.0, 0.5, 0.0, 0.5, 0.5, 0.5, 1.0}
    Public position() As Single = {600.0, 350.0, 600.0, 0.0}
    Public angle_offset As Single
    'Public FLY_ As Boolean = False
    Public swat2 As New Stopwatch
    Public drawbuffer0() = {Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_NONE}
    Public drawbuffer1() = {Gl.GL_NONE, Gl.GL_COLOR_ATTACHMENT1_EXT}
    Public attachstatus(5) As Integer

    Public MV(16) As Single
    Public lightProject(16) As Single
    Public selected_map_hit As Integer = 0
    Public tl_, tr_, br_, bl_ As Vector3D


    Public decal_grid_size As Integer = 32
    Public decal_count As Integer = 0
    Public decal_data() As vertex_data
    Public largestAnsio As Single
    Public rendermode As Boolean = False
    Public dummyT As Integer
    '======================
    Public y_offset As Single
    Public u_y_offset As Single
    Public layer_uv_list As String = ""
    Public tile_width As Integer
    Public it_was_added As Boolean = False
    Public client As NetClient
    Public clients As New List(Of client_)
    Public Serverclients As New List(Of client_)
    Public chat_message_buffer As New List(Of String)
    Public CanStart As Boolean = False
    Public chat_string As String = ""
    Public ImHost As Boolean = False
    Public ImDriver As Boolean = False
    Public NetData As Boolean = False
    Public uTank As New t_l
    '========================
    Public mDrag As Boolean = False
    Public packet_lock As New Object
    Public Packet_in As New packet_
    Public Packet_out As packet_
    Public render_size As UInteger = 1024
    Public decal_chunks(1) As decal_chunks_
    Public JUST_MAP_NAME As String = ""
    Public Structure decal_chunks_
        Public chunk_Ids() As Integer
        Public seam_Ids() As Integer
        Public chunk_count As Integer
    End Structure


    Public Structure packet_
        Public Ex As Single
        Public Ez As Single
        Public Ey As Single
        Public Rx As Single
        Public Ry As Single
        Public Lr As Single
        '
        Public tankId As Int16
        '
        Public Tx As Single
        Public Tz As Single
        Public Tr As Single
        Public ID As String
        '
        Public index_tank As Int16
        '
        Public Ix As Single
        Public Iz As Single
        Public Ir As Single
        Public I_Id As String
        Public comment As String
        Public Icomment As String
    End Structure
    Public current_tank_index As Int16 = 0
    Public Structure mouse_
        Public x As Single
        Public z As Single
        Public y As Single
        Public rot_x As Single
        Public rot_y As Single
        Public look_r As Single
        Public tank_index As Int16
        Public tankID As Int16
        Public tank_id_string As String
    End Structure

    '========================
    Enum packetType
        login
        login_ack
        logout
        chatMsg
        terra_state
        map_change
        client_reset
        driverChange
        timedOut
        req_client_list
        ack_req_client_list
    End Enum
    Enum clientType
        guest
        host
        driver
        driverAndhost
    End Enum
    Public Class client_
        Public Name As String
        Public client_type As Byte
        Public connection As NetConnection
        Public driver As Boolean
        Public host As Boolean = False
    End Class
    '======================
    Public SHOW_RINGS As Boolean = True
    Public map_texture_ids(0) As Integer
    Public SHOW_MAPS As Boolean = False
    Public block_mouse As Boolean = False
    Public hz_loaded As Boolean = False
    Public model_bump_loaded As Boolean = False
    Public uv2s_loaded As Boolean = False
    Public lighting_terrain_texture As Single = 0
    Public lighting_ambient As Single = 0
    Public lighting_fog_level As Single
    Public lighting_model_level As Single
    Public gamma_level As Single
    Public gray_level As Single
    Public minimap_size As Single = 0.0!
    Public imtemphost As Boolean = False
    Public GAME_PATH As String = ""
    Public tankID As Int16 = -1
    Public NetOldtankID As Int16 = -1
    Public old_tankID As Integer = -1
    Public transmite_sync As New Object
    '==============
    Public speedtree_name As String = ""
    Public speedtree_normalmap As String = ""
    Public speedtree_map As String = ""
    Public buff(1) As Byte
    Public icon_scale As Single = 20
    Public n_list1 As New List(Of String)
    Public n_list2 As New List(Of String)
    Public dummy_texture As Integer
    Public mix_atlas_Id As Integer
    Public temp_bmp As Bitmap
    Public load_map_name As String = " "
    Public glob_str As String = ""
    Public highZmap As Bitmap
    Public npb As New Panel
    Public nations(10) As String
    Public current_nation As Integer = 0
    Public Far_Clip As Single = 700
    Public lod1_swap As Integer
    Public lod2_swap As Integer
    Public lod0_swap As Integer
    Public MOVE_TANK As Boolean = False
    Public ROTATE_TANK As Boolean = False
    Public surface_normal As vect3
    Public team_setup_selected_tank As String = ""
    Public bmap As Bitmap
    Public vaid As vect3
    Public T_1, T_2, T_3, T_4 As vect3
    Public MAP_BB_BL, MAP_BB_UR As vect2
    Public minimap_textureid As Integer
    Public minimsp_frameT_TextureId As Integer
   Public sector_outlineID As Integer
    Public map_borderId As Integer
    Public team_1 As vect3
    Public team_2 As vect3

    Public saved_texture_loads As Integer = 0
    Public saved_model_loads As Integer = 0
    Public gl_busy As Boolean = False
    Public seammapID As Integer
    Public test_count As Int32
    Public shared_content1 As Ionic.Zip.ZipFile
    Public shared_content2 As Ionic.Zip.ZipFile
    Public shared_content1_hd As Ionic.Zip.ZipFile
    Public shared_content2_hd As Ionic.Zip.ZipFile
    Public active_pkg As Ionic.Zip.ZipFile
    Public active_pkg_hd As Ionic.Zip.ZipFile
    Public maploaded As Boolean = False
    Public xDoc As New XmlDocument
    Public sec_list(5) As String
    Public Z_Cursor As Single = 0
    Public skydomelist As Integer
    Public skydometextureID As Integer
    Public normalmap As System.Drawing.Bitmap = My.Resources.control
    Public skyDomeName As String = ""
    Public sun_multiplier As Single = 1.0
    Public coordStr As String = ""
    Public UVs_ON As Boolean = False
    Public eyeX, eyeY, eyeZ As Single
    Public full_map_name As String = ""
    'Public minimap As Bitmap
    Public bmp_data(1, 1) As Single
    Public colorID As Integer
    Public colors(,) As Single
    Public maplist(1) As grid_sec
    Public skydome(1) As grid_sec
    Public tri_count As Int32 = 0
    Public midx As Single = 0
    Public midy As Single = 0
    Public midz As Single = 0
    Public heights() As Single
    Public h_width As UInt32
    Public h_height As UInt32
    'Public Texture(15) As UInt32
    Public lodMapSize, normalMapSize, heightMapSize, aoMapSize, _
              holeMapSize, shadowMapSize, blendMapsize As UInt32
    Public z_move As Boolean = False
    Public move_mod As Boolean = False
    Public rename_x As Integer
    Public rename_y As Integer
    Public pfd As Gdi.PIXELFORMATDESCRIPTOR
    Public pfd2 As Gdi.PIXELFORMATDESCRIPTOR
    Public x_max As Single = -10000
    Public x_min As Single = 10000
    Public y_min As Single = -10000
    Public y_max As Single = 10000
    Public z_max As Single = -10000
    Public z_min As Single = 10000
    Public look_point_X As Single = 0.0
    Public look_point_Y As Single = 0.0
    Public look_point_Z As Single = 0.0
    Public View_Radius As Single = 10.0
    Public x_center As Single
    Public y_center As Single
    Public z_center As Single
    Public Cam_Y_angle As Single
    Public Cam_X_angle As Single
    Public u_look_point_X As Single = 0.0
    Public u_look_point_Y As Single = 0.0
    Public u_look_point_Z As Single = 0.0
    Public u_View_Radius As Single = 10.0
    Public u_x_center As Single
    Public u_y_center As Single
    Public u_z_center As Single
    Public u_Cam_Y_angle As Single
    Public u_Cam_X_angle As Single
    Public u_tank_r As Single
    Public u_tank_x As Single
    Public u_tank_z As Single
    Public Screen_draw_time As Single
    Public screen_avg_draw_time As Single
    Public screen_avg_counter As Single
    Public screen_totaled_draw_time As Single
    Public old_light_position As vect3
    Public old_look_point As vect3
    Public face_count As Integer = 0
    Public master_cnt As Integer = 0
    Public object_count As Integer = 0
    'Public path As String = "C:\Boinc.NET\3D-Models\"
    Public _STARTED As Boolean = False
    Public _SELECTED_map As UInt32 = 0
    Public _SELECTED_model As UInt32 = 0
    Public _SELECTED_tree As UInt32 = 0
    Public _OBJ_ID As Integer = 0
    Public seek_flag As Boolean = False
    Public pb1_hDC As System.IntPtr
    Public pb1_hRC As System.IntPtr
    Public pb2_hDC As System.IntPtr
    Public pb2_hRC As System.IntPtr
    Public pb3_hDC As System.IntPtr
    Public pb3_hRC As System.IntPtr
    Public pb4_hDC As System.IntPtr
    Public pb4_hRC As System.IntPtr
    Public _ZOOM As Single = 5.0F
    Public _VERTICAL As Single = 0.0F
    Public _ROTATION As Single = 3.1415926
    Public move_cam As Boolean = False
    Public M_MOVE As Boolean = False
    Public M_DOWN As Boolean = False
    'Public Look_Z_angle As Single
    'Public Look_at_X As Single = 0
    'Public Look_at_Y As Single = 0
    ''Public Look_at_Z As Single = 0
    Public cam_x As Single = 0
    Public cam_y As Single = 0
    Public cam_z As Single = 5
    Public move_cam_z As Boolean = False
    Public glutFonts() = { _
    Glut.GLUT_BITMAP_9_BY_15, _
    GLUT_BITMAP_8_BY_13, _
    GLUT_BITMAP_TIMES_ROMAN_10, _
    GLUT_BITMAP_TIMES_ROMAN_24, _
    GLUT_BITMAP_HELVETICA_10, _
    GLUT_BITMAP_HELVETICA_12, _
    GLUT_BITMAP_HELVETICA_18 _
 }
    Public stopGL As Boolean = False
    Public Declare Sub ZeroMemory Lib "kernel32.dll" Alias "RtlZeroMemory" (ByVal Destination As Gdi.PIXELFORMATDESCRIPTOR, ByVal Length As Integer)
    Public mouse As New Point
    Public normal_map(256) As Byte
    Public cur_chunk As Integer = 0
    Public vertex() As p_set
    Public vertex_working() As p_set
    Public vertex_count As Integer = 0

    Public Class p_set
        Public p1x, p1y, p1z As Single
        Public p1u, p1v As Single

        Public p2x, p2y, p2z As Single
        Public p2u, p2v As Single

        Public p3x, p3y, p3z As Single
        Public p3u, p3v As Single

        Public p4x, p4y, p4z As Single
        Public p4u, p4v As Single

        Public p5x, p5y, p5z As Single
        Public p5u, p5v As Single

        Public p6x, p6y, p6z As Single
        Public p6u, p6v As Single
    End Class
    Public Class point_
        Public x, y As Single

    End Class
    Public Class MyRenderer
        Inherits ToolStripProfessionalRenderer
        Protected Overloads Overrides Sub OnRenderMenuItemBackground(ByVal e As ToolStripItemRenderEventArgs)
            Dim rc As New Rectangle(Point.Empty, e.Item.Size)
            Dim c As Color = IIf(e.Item.Selected, SystemColors.MenuHighlight, SystemColors.MenuBar)
            Using brush As New SolidBrush(c)
                e.Graphics.FillRectangle(brush, rc)
            End Using
            e.Item.ForeColor = IIf(e.Item.Selected, Color.White, Color.Black)
        End Sub
    End Class
End Module
