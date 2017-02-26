<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmGetIP
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGetIP))
        Me.Button1 = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.get_my_ip_btn = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        Me.Label5 = New System.Windows.Forms.Label
        Me.game_port_tb = New System.Windows.Forms.TextBox
        Me.my_ip_tb = New System.Windows.Forms.TextBox
        Me.user_name_tb = New System.Windows.Forms.TextBox
        Me.ip_tb = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(81, 172)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(100, 23)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Connect"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.Yellow
        Me.Label1.Location = New System.Drawing.Point(12, 106)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Name"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.Yellow
        Me.Label2.Location = New System.Drawing.Point(12, 8)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(62, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Dispatch IP"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.Color.Yellow
        Me.Label4.Location = New System.Drawing.Point(12, 54)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(63, 13)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "My WAN IP"
        '
        'get_my_ip_btn
        '
        Me.get_my_ip_btn.Location = New System.Drawing.Point(132, 123)
        Me.get_my_ip_btn.Name = "get_my_ip_btn"
        Me.get_my_ip_btn.Size = New System.Drawing.Size(128, 23)
        Me.get_my_ip_btn.TabIndex = 9
        Me.get_my_ip_btn.Text = "Get My WAN IP"
        Me.get_my_ip_btn.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(132, 70)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(128, 23)
        Me.Button2.TabIndex = 10
        Me.Button2.Text = "Port Forwarding Help"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.ForeColor = System.Drawing.Color.Yellow
        Me.Label5.Location = New System.Drawing.Point(129, 8)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(63, 13)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "Server. Port"
        '
        'game_port_tb
        '
        Me.game_port_tb.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.Terra.My.MySettings.Default, "game_port", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.game_port_tb.Location = New System.Drawing.Point(132, 24)
        Me.game_port_tb.Name = "game_port_tb"
        Me.game_port_tb.Size = New System.Drawing.Size(49, 20)
        Me.game_port_tb.TabIndex = 14
        Me.game_port_tb.Text = Global.Terra.My.MySettings.Default.game_port
        '
        'my_ip_tb
        '
        Me.my_ip_tb.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.Terra.My.MySettings.Default, "my_wan_ip", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.my_ip_tb.Location = New System.Drawing.Point(12, 70)
        Me.my_ip_tb.Name = "my_ip_tb"
        Me.my_ip_tb.Size = New System.Drawing.Size(100, 20)
        Me.my_ip_tb.TabIndex = 7
        Me.my_ip_tb.Text = Global.Terra.My.MySettings.Default.my_wan_ip
        '
        'user_name_tb
        '
        Me.user_name_tb.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.Terra.My.MySettings.Default, "username", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.user_name_tb.Location = New System.Drawing.Point(13, 125)
        Me.user_name_tb.Name = "user_name_tb"
        Me.user_name_tb.Size = New System.Drawing.Size(99, 20)
        Me.user_name_tb.TabIndex = 2
        Me.user_name_tb.Text = Global.Terra.My.MySettings.Default.username
        '
        'ip_tb
        '
        Me.ip_tb.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.Terra.My.MySettings.Default, "server_address", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ip_tb.Location = New System.Drawing.Point(12, 24)
        Me.ip_tb.Name = "ip_tb"
        Me.ip_tb.Size = New System.Drawing.Size(100, 20)
        Me.ip_tb.TabIndex = 1
        Me.ip_tb.Text = Global.Terra.My.MySettings.Default.server_address
        '
        'frmGetIP
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(275, 216)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.game_port_tb)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.get_my_ip_btn)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.my_ip_tb)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.user_name_tb)
        Me.Controls.Add(Me.ip_tb)
        Me.Controls.Add(Me.Button1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmGetIP"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Connect to...."
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents ip_tb As System.Windows.Forms.TextBox
    Friend WithEvents user_name_tb As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents my_ip_tb As System.Windows.Forms.TextBox
    Friend WithEvents get_my_ip_btn As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents game_port_tb As System.Windows.Forms.TextBox
End Class
