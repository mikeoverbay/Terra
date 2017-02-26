Module Globalvars
    'variables needed
    Public SERVER_DOWN As Boolean = False
    Public scrolling_flag As Boolean = False
    Public timer_flag As Boolean = False
    Public _refresh As Boolean = False
    Public SENDING As Boolean = False
    Public PM_VAL As Integer = 0
    Public dgv_position As Integer
    Public color_flip As Boolean = False
    Public tmr_cnt As Integer = 0
    Public msgCC As String
    Public ringCC As String
    Public client_name(30) As String
    Public client_room(30) As Integer
    Public This_Id As Integer = -1
    Public _SEND As Boolean = False
    Public _BELL As Boolean = False
    Public server As System.Net.IPAddress
    Public my_IP As String = "...."
    Public new_text As String = ""
    Public users_name As String = ""
    Public tcpClient As New System.Net.Sockets.TcpClient()
    Public thrd As New thd
    Public connected_user(30) As users
    ' Public mylistener As New TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), 1956)
    Public locktite As New Object
    Public ERRORS() = {"None", _
      "Can't Write to Stream", _
      "Can't Read from Stream", _
      "Unauthorized", _
      "No Response From Host", _
      "Port Error"}
    Public WAIT_SEND As Boolean
    Public isInitiated As Boolean = False
    Public form_height As Integer
    Public form_width As Integer
    Public form_client_size As Point
    Public current_line As Integer = 0
    Public RET_STRING As String = ""
    Public SEND_STRING As String = ""
    Public AVATAR As Integer = 0
    '------------------------------------
    Public _CLEAR_RTB1 As Boolean = False
    Public welcome_text As String = "WELCOME TO CC !"
    Public avatar_data() As String
    Public smilie_data() As String
    Public dummy_text As String
    Public Chat_text As String = ""
    Public color_data As String
    Public text_color() As String
    Public head_text As String
    Public pre_text As String
    Public _COLOR As Integer
    Public MY_COLOR As Integer
    Public _FONT_SIZE As Integer = 18
    Public Declare Function SendMessage Lib "user32.dll" Alias _
"SendMessageA" (ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal _
wParam As Integer, ByVal lParam As Integer) As Integer
    Public Const EM_GETLINECOUNT = &HBA
    Public Const EM_LINESCROLL = &HB6
    Public Const EM_GETFIRSTVISIBLELINE = &HCE
    Public Const SB_LINEDOWN = 1
    Public Const SB_LINEUP = 0

End Module
