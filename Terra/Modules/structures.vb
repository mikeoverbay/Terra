Imports System.Math
Imports Tao.DevIl
Imports Tao.DevIl.Il
Imports Tao.DevIl.Ilu
Imports System.Data
Imports System.Net.Sockets
Imports System.Threading
Imports System.IO
Module structures
    Public tb1 As New tb1_
    Public Class tb1_
        Private str As String
        Private vis As Boolean = True

        Property Visible As Boolean
            Get
                Return vis
            End Get
            Set(value As Boolean)

                If Not value Then
                    FrmInfoWindow.Visible = False
                    vis = False
                Else
                    vis = True
                    FrmInfoWindow.Visible = True
                End If

            End Set

        End Property
        Property text As String
            Get
                Return str
            End Get
            Set(value As String)
                'Return
                str = value
                If Me.vis Then
                    FrmInfoWindow.tb1.Text = value
                    Application.DoEvents()
                End If
            End Set
        End Property

    End Class


    Public shaders As shaders__
    Public Structure shaders__
        Public shader() As shaders_
        Public Function f(ByVal name As String) As Integer
            For Each s In shader
                If s.shader_name = name Then
                    Return s.shader_id
                End If
            Next
        End Function

    End Structure

    Public Structure shaders_
        Public fragment As String
        Public vertex As String
        Public geo As String
        Public shader_name As String
        Public shader_id As Integer
        Public has_geo As Boolean
        Public Sub set_call_id(ByVal id As Integer)
            Try
                CallByName(shader_list, Me.shader_name, CallType.Set, Me.shader_id)

            Catch ex As Exception
                MsgBox("missing member from shader_list:" + Me.shader_name, MsgBoxStyle.Exclamation, "Oops!")
                End
            End Try
        End Sub
    End Structure


    Public map_layer_cache(1) As tree_textures_
    Public normalMap_layer_cache(1) As tree_textures_
    Public texture_cache(1) As tree_textures_
    Public map_layers() As layer_
    Public Structure layer_
        Public used_layers As Integer
        Public layers() As layer_data_
        Public layer_count As Integer
        Public main_texture As Integer
        Public main_texture_id As Integer
        Public mix_image As Bitmap
        Public color_tex As Bitmap
        Public mix_texture_Id As Integer
    End Structure
    Public Structure layer_data_
        Public l_name As String
        Public n_name As String
        Public text_id As Integer
        Public norm_id As Integer
        Public sizex, sizez As Integer
        Public u As Single
        Public v As Single
        Public r As Single
        Public uP As vect4
        Public vP As vect4
        Dim ms As IO.MemoryStream
        'Public raw_data() As Byte
        Public image As Bitmap
    End Structure
    Public locations As locations_
    Public Structure locations_
        Public team_1() As t_l
        Public team_2() As t_l
    End Structure
    Public tank_slot_update_counter As Byte
    Public Structure t_l
        Public scrn_coords() As Single
        Public rot_y As Single
        Public loc_x As Single
        Public loc_z As Single
        'This will be in the form of "n_a_n_n" 
        'team (1 or 2) _ (nation a,r,b,c,g) _ (index) _ button index
        Public id As String
        Public tank_displaylist As Integer
        Public type As Integer
        Public name As String
        Public comment As String
    End Structure
    Public Models As model_
    Public Structure model_
        Public Model_list() As String
        Public models() As primitive
        Public matrix() As matrix_
        Public model_count As UInt32
    End Structure

    Public Trees As Tree_s
    Public Structure Tree_s
        Public flora() As flora_
        Public Tree_list() As String
        Public matrix() As matrix_
    End Structure
    Public Structure matrix_
        Public matrix() As Single
    End Structure
    Public tree_textures(0) As tree_textures_
    Public treeCache(0) As flora_
    Public Structure tree_textures_
        Public name As String
        Public name2 As String
        Public normalname As String
        Public textureID As Integer
        Public texture2ID As Integer
        Public textureNormID As Integer
    End Structure


    Public tank_weights() = _
    {"heavy", "medium", "light", "SPG", "AT-SPG"}
    Public tank_types() = _
    {"Heavy", "Medium", "Light", "SPG", "TD"}
    Public tank_sortorder() = _
    {0, 1, 2, 4, 3}
    Public tank_mini_icons(5) As Integer

    Public Tankimagelist As New ImageList
    'this stores the maps that are loaded from maplist.txt
    Public loadmaplist() As map_item_
    Public Structure map_item_ : Implements IComparable(Of map_item_)
        Public name As String
        Public realname As String
        Public size As Single
        Public grow_shrink As Boolean
        Public direction As Single
        Public delay_time As Integer
        Public Function CompareTo(ByVal other As map_item_) As Integer Implements System.IComparable(Of map_item_).CompareTo
            Try
                Return Me.realname.CompareTo(other.realname)

            Catch ex As Exception
                Return 0
            End Try
        End Function
    End Structure
    Public a_tanks(1) As tank_
    Public r_tanks(1) As tank_
    Public g_tanks(1) As tank_
    Public f_tanks(1) As tank_
    Public b_tanks(1) As tank_
    Public c_tanks(1) As tank_
    Public j_tanks(1) As tank_
    Public Structure tank_
        Implements IComparable(Of tank_)
        Public gui_string As String
        Public file_name As String
        Public weight As String
        Public image As Bitmap
        Public sortorder As Integer

        Public Function CompareTo(ByVal other As tank_) As Integer Implements System.IComparable(Of tank_).CompareTo
            Try
                Return Me.sortorder.CompareTo(other.sortorder)

            Catch ex As Exception
                Return sortorder
            End Try
        End Function
    End Structure

    Public material_list(1) As materi_
    Public Structure materi_
        Public id As Integer
        Public damaged As Boolean
    End Structure

    Public Structure flora_
        Public name As String
        Public billboard_displayID As Integer
        Public branch_textureID As Integer
        Public branch_normalID As Integer
        Public frond_textureID As Integer
        Public frond_normalID As Integer
        Public leaf_textureID As Integer
        Public leaf_normalID As Integer
        Public billboard_textureID As Integer
        Public billboard_normalID As Integer

        Public branch_displayID As Integer
        Public frond_displayID As Integer
        Public leaf_displayID As Integer
        'adding these so I can reduce the state changes.
        'If I do this right, I can bind the textures once and render many trees.
        'It should save considerable time.
        Dim tree_id() As Integer
        Dim matrix_list() As matrix_
        Dim tree_cnt As Integer
        Public BB() As BB_
        Public D_BB As BB_
    End Structure
    Public Structure BB_
        Public BB_Min As vect3
        Public BB_Max As vect3
        Public BB() As vect3
        Public visible As Boolean
    End Structure
    Public Structure f_indices
        Public indices() As Integer
    End Structure
    Public Structure tree_
        Public b_vert() As Single
        Public f_vert() As Single
        Public l_vert() As Single
        Public b_indices() As Integer
        Public strip_inds() As f_indices
        Public l_indices() As Integer
        Public strip_count As Integer
        Public lod As Integer
    End Structure

    Public Structure fl_
        Public uvTL As uvcs
        Public uvTR As uvcs
        Public uvBL As uvcs
        Public uvBR As uvcs
        Public V_TL As vect3
        Public V_TR As vect3
        Public V_BL As vect3
        Public V_BR As vect3
        Public norm As vect2
    End Structure
    Public Structure uvcs
        Public topleft As vect2
        Public topright As vect2
        Public bottleft As vect2
        Public bottright As vect2
    End Structure
    Structure domtexturemapheader
        Public magic As UInt32
        Public version As UInt32
        Public numTextures As UInt32
        Public textNameSize As UInt32
        Public width As UInt32
        Public height As UInt32
        Public pad() As UInt32
        Const MAGIC_ As UInt32 = &H74616D
    End Structure
    Public Structure data_section
        Public raw_data() As Byte
        Public filename As String
        Public row1 As vect3
        Public row2 As vect3
        Public row3 As vect3
        Public row4 As vect3

    End Structure
    ' Public model_library(1) As library
    Public Structure library
        Public pgk_name As String
        Public entries As List(Of String)

    End Structure
    Public water As New water_model_
    Public Structure water_model_
        Public displayID_cube As Integer
        Public displayID_plane As Integer
        Public textureID As Integer
        Public normalID As Integer
        Public size_ As vect3
        Public position As vect3
        Public orientation As Single
        Public type As String
        Public IsWater As Boolean
        Public lbl As vect3
        Public lbr As vect3
        Public ltl As vect3
        Public ltr As vect3
        Public rbl As vect3
        Public rbr As vect3
        Public rtl As vect3
        Public rtr As vect3
        Public BB() As vect3
        Public matrix() As Single
    End Structure

    Public mapBoard(,) As Integer

    Public model_batchList() As m_batch_
    Public Structure m_batch_
        Public texture_ID As Integer
        Public count As Integer
        Public model() As m_model_
    End Structure
    Public Structure m_model_
        Public texture2_ID As Integer
        Public norm_ID As Integer
        Public display_list_id As Integer
        Public transform As transformStruct
        Public location As vect3
        Public bumped As Integer
        Public alphaRef As Integer
        Public GAmap As Integer
        Public alphaTestEnable As Integer
        Public multi_textured As Integer
    End Structure
    Public Structure grid_sec
        Public bmap As System.Drawing.Bitmap
        Public calllist_Id As Int32
        Public normMapID As Int32
        Public HZ_normMapID As Int32
        Public colorMapId As Int32
        Public HZ_colorMapID As Int32
        Public HolesId As Int32
        Public dominateId As Int32
        Public location As vect3
        Public name As String
        Public scr_coords() As Single
        Public cdata() As Byte
        Public model_count As UInt32
        Public model_list As List(Of String)
        Public tree_list As List(Of String)
        Public flora() As flora_
        Public flora_count As UInt32
        Public heights(,) As Single
        Public heights_avg As Single
        Public seamCallId As Integer
        Public has_holes As Integer
        Public BB_Max As vect3
        Public BB_Min As vect3
        Public BB() As vect3
        Public visible As Boolean
    End Structure

    Public Structure transformStruct
        Public row0 As vect3
        Public row1 As vect3
        Public row2 As vect3
        Public row3 As vect3
        Public roll As Single
        Public pitch As Single
        Public yaw As Single
        Public scale As vect3
        Dim matrix() As Single
    End Structure
    Public dest_buildings As destructibles
    Public Structure destructibles
        Public filename As List(Of String)
        Public matName As List(Of String)
    End Structure
    <Serializable()> Public Structure vertex_data
        Public x As Single
        Public y As Single
        Public z As Single
        Public u As Single
        Public v As Single
        Public nx As Single
        Public ny As Single
        Public nz As Single
        Public map As Integer
        Public t As vect3
        Public bt As vect3
    End Structure
    Public Function get_length_vertex(v As vertex_data)
        Dim s As Single = v.x + v.y + v.z
        Return s
    End Function
    Public Function get_length_vect3(v As vect3)
        Dim s As Single = Sqrt((v.x ^ 2) + (v.y ^ 2) + (v.z ^ 2))
        Return s
    End Function
    Public topRight As New vertex_data
    Public topleft As New vertex_data
    Public bottomRight As New vertex_data
    Public bottomleft As New vertex_data
    Public loaded_models As Loaded_Model_list
    Public Structure Loaded_Model_list
        Public names As List(Of String)
        Public stack() As mdl_stack
        Public _count As Integer
    End Structure
    Public Structure mdl_stack
        Public _count As Integer
        Public dispId() As Integer
        Public textId() As Integer
        Public text2Id() As Integer
        Public normID() As Integer
        Public bumped() As Boolean
        Public alphaRef() As Integer
        Public alphaTestEnable() As Integer
        Public GAmap() As Boolean
        Public multi_textured() As Boolean
        Public color_name() As String
        Public color2_name() As String
        Public isBuilding As Boolean
        Public matrix() As matrix_
        Public model_id() As Integer
        Public model_count As Integer
    End Structure
    Public Structure vect4
        Public x As Double
        Public y As Double
        Public z As Double
        Public w As Single
    End Structure
    Public Structure vect3
        Public x As Single
        Public y As Single
        Public z As Single

    End Structure
    Public Structure vect3Norm
        Public nx As Single
        Public ny As Single
        Public nz As Single
    End Structure
    Public Structure vect2
        Public x As Single
        Public y As Single
    End Structure
    Public Structure vect3B
        Public x As Byte
        Public y As Byte
        Public z As Byte
    End Structure
    Public Structure vect3sB
        Public x As SByte
        Public y As SByte
        Public z As SByte
    End Structure
    Public Structure vect2uv
        Public u As Single
        Public v As Single
    End Structure
    Public Structure pngStruct
        Public head1 As UInteger '4
        Public head2 As UInteger '4
        Public chunkLength As UInt32 '4
        Public IHDR As UInt32 '4
        Public chunkType As UInt32 '4
        Public width As UInt32 '4
        Public height As UInt32 '4
        Public bitDepth As Byte '1
        Public colorType As Byte '1
        Public CompMethod As Byte '1
        Public filterMethod As Byte '1
        Public interlaceMethod As Byte '1
        Public CRC As UInt32 '4
        Public phys1 As UInt32 '4
        Public physTag As UInt32 '4
        Public unitsX As UInt32 '4
        Public unitsY As UInt32 '4
        Public unintSpec As Byte '1
        Public physCRC As UInt32 '4
        Public IDAT_cSize As UInt32 '4
        Public IDAT_Tag As UInt32 '4


    End Structure
End Module
