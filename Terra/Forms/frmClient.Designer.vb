<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmClient
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmClient))
        Me.Button1 = New System.Windows.Forms.Button()
        Me.echo_window_tb = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.chat_entry_tb = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.client_panel = New System.Windows.Forms.Panel()
        Me.diagnostics_bt = New System.Windows.Forms.Button()
        Me.stop_button = New System.Windows.Forms.Button()
        Me.Ping_text = New System.Windows.Forms.Label()
        Me.diag_tb = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.password_tb = New System.Windows.Forms.TextBox()
        Me.imHost_cb = New System.Windows.Forms.CheckBox()
        Me.client_name_tb = New System.Windows.Forms.TextBox()
        Me.dest_ip_tb = New System.Windows.Forms.TextBox()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Button1.Location = New System.Drawing.Point(312, 5)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(62, 23)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "start"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'echo_window_tb
        '
        Me.echo_window_tb.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom), System.Windows.Forms.AnchorStyles)
        Me.echo_window_tb.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.echo_window_tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.echo_window_tb.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.echo_window_tb.ForeColor = System.Drawing.Color.White
        Me.echo_window_tb.Location = New System.Drawing.Point(0, 1)
        Me.echo_window_tb.Margin = New System.Windows.Forms.Padding(0)
        Me.echo_window_tb.Multiline = True
        Me.echo_window_tb.Name = "echo_window_tb"
        Me.echo_window_tb.Size = New System.Drawing.Size(278, 174)
        Me.echo_window_tb.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(126, 32)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Name"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(124, 10)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(95, 13)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "Host to connect to"
        '
        'chat_entry_tb
        '
        Me.chat_entry_tb.AcceptsReturn = True
        Me.chat_entry_tb.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.chat_entry_tb.BackColor = System.Drawing.Color.Black
        Me.chat_entry_tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.chat_entry_tb.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chat_entry_tb.ForeColor = System.Drawing.Color.White
        Me.chat_entry_tb.Location = New System.Drawing.Point(0, 175)
        Me.chat_entry_tb.Multiline = True
        Me.chat_entry_tb.Name = "chat_entry_tb"
        Me.chat_entry_tb.Size = New System.Drawing.Size(278, 36)
        Me.chat_entry_tb.TabIndex = 7
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.client_panel)
        Me.Panel1.Controls.Add(Me.diagnostics_bt)
        Me.Panel1.Controls.Add(Me.stop_button)
        Me.Panel1.Controls.Add(Me.Ping_text)
        Me.Panel1.Controls.Add(Me.chat_entry_tb)
        Me.Panel1.Controls.Add(Me.diag_tb)
        Me.Panel1.Controls.Add(Me.echo_window_tb)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 73)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(378, 211)
        Me.Panel1.TabIndex = 8
        '
        'client_panel
        '
        Me.client_panel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom), System.Windows.Forms.AnchorStyles)
        Me.client_panel.AutoScroll = True
        Me.client_panel.BackColor = System.Drawing.Color.DimGray
        Me.client_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.client_panel.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.client_panel.Location = New System.Drawing.Point(278, 27)
        Me.client_panel.Margin = New System.Windows.Forms.Padding(0)
        Me.client_panel.Name = "client_panel"
        Me.client_panel.Size = New System.Drawing.Size(100, 184)
        Me.client_panel.TabIndex = 9
        '
        'diagnostics_bt
        '
        Me.diagnostics_bt.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.diagnostics_bt.Location = New System.Drawing.Point(279, 1)
        Me.diagnostics_bt.Name = "diagnostics_bt"
        Me.diagnostics_bt.Size = New System.Drawing.Size(45, 23)
        Me.diagnostics_bt.TabIndex = 12
        Me.diagnostics_bt.Text = "Diag."
        Me.diagnostics_bt.UseVisualStyleBackColor = False
        '
        'stop_button
        '
        Me.stop_button.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.stop_button.Location = New System.Drawing.Point(329, 1)
        Me.stop_button.Name = "stop_button"
        Me.stop_button.Size = New System.Drawing.Size(45, 23)
        Me.stop_button.TabIndex = 10
        Me.stop_button.Text = "Close"
        Me.stop_button.UseVisualStyleBackColor = False
        '
        'Ping_text
        '
        Me.Ping_text.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Ping_text.AutoSize = True
        Me.Ping_text.ForeColor = System.Drawing.Color.White
        Me.Ping_text.Location = New System.Drawing.Point(3, 159)
        Me.Ping_text.Name = "Ping_text"
        Me.Ping_text.Size = New System.Drawing.Size(0, 13)
        Me.Ping_text.TabIndex = 13
        '
        'diag_tb
        '
        Me.diag_tb.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom), System.Windows.Forms.AnchorStyles)
        Me.diag_tb.BackColor = System.Drawing.Color.Silver
        Me.diag_tb.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.diag_tb.ForeColor = System.Drawing.Color.Black
        Me.diag_tb.Location = New System.Drawing.Point(0, 1)
        Me.diag_tb.Multiline = True
        Me.diag_tb.Name = "diag_tb"
        Me.diag_tb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.diag_tb.Size = New System.Drawing.Size(278, 174)
        Me.diag_tb.TabIndex = 10
        Me.diag_tb.Text = "this is a test"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(282, 32)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(25, 13)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "PW"
        '
        'password_tb
        '
        Me.password_tb.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.password_tb.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.Terra.My.MySettings.Default, "lidgren_pw", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.password_tb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.password_tb.Location = New System.Drawing.Point(194, 29)
        Me.password_tb.Name = "password_tb"
        Me.password_tb.Size = New System.Drawing.Size(84, 20)
        Me.password_tb.TabIndex = 12
        Me.password_tb.Text = Global.Terra.My.MySettings.Default.lidgren_pw
        '
        'imHost_cb
        '
        Me.imHost_cb.AutoSize = True
        Me.imHost_cb.ForeColor = System.Drawing.Color.White
        Me.imHost_cb.Location = New System.Drawing.Point(225, 8)
        Me.imHost_cb.Name = "imHost_cb"
        Me.imHost_cb.Size = New System.Drawing.Size(81, 17)
        Me.imHost_cb.TabIndex = 11
        Me.imHost_cb.Text = "CheckBox1"
        Me.imHost_cb.UseVisualStyleBackColor = True
        Me.imHost_cb.Visible = False
        '
        'client_name_tb
        '
        Me.client_name_tb.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.client_name_tb.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.Terra.My.MySettings.Default, "username", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.client_name_tb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.client_name_tb.Location = New System.Drawing.Point(12, 29)
        Me.client_name_tb.Name = "client_name_tb"
        Me.client_name_tb.Size = New System.Drawing.Size(108, 20)
        Me.client_name_tb.TabIndex = 4
        Me.client_name_tb.Text = Global.Terra.My.MySettings.Default.username
        '
        'dest_ip_tb
        '
        Me.dest_ip_tb.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.Terra.My.MySettings.Default, "my_wan_ip", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.dest_ip_tb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dest_ip_tb.Location = New System.Drawing.Point(12, 6)
        Me.dest_ip_tb.Name = "dest_ip_tb"
        Me.dest_ip_tb.Size = New System.Drawing.Size(108, 20)
        Me.dest_ip_tb.TabIndex = 3
        Me.dest_ip_tb.Text = Global.Terra.My.MySettings.Default.my_wan_ip
        '
        'frmClient
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(378, 284)
        Me.ControlBox = False
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.password_tb)
        Me.Controls.Add(Me.imHost_cb)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.client_name_tb)
        Me.Controls.Add(Me.dest_ip_tb)
        Me.Controls.Add(Me.Label3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(394, 600)
        Me.MinimumSize = New System.Drawing.Size(394, 300)
        Me.Name = "frmClient"
        Me.Text = "Terra! Chat:"
        Me.TopMost = True
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
	Friend WithEvents Button1 As System.Windows.Forms.Button
	Friend WithEvents echo_window_tb As System.Windows.Forms.TextBox
	Friend WithEvents dest_ip_tb As System.Windows.Forms.TextBox
	Friend WithEvents client_name_tb As System.Windows.Forms.TextBox
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents chat_entry_tb As System.Windows.Forms.TextBox
	Friend WithEvents Panel1 As System.Windows.Forms.Panel
	Friend WithEvents client_panel As System.Windows.Forms.Panel
	Friend WithEvents stop_button As System.Windows.Forms.Button
	Friend WithEvents imHost_cb As System.Windows.Forms.CheckBox
	Friend WithEvents diag_tb As System.Windows.Forms.TextBox
	Friend WithEvents diagnostics_bt As System.Windows.Forms.Button
	Friend WithEvents password_tb As System.Windows.Forms.TextBox
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents Ping_text As System.Windows.Forms.Label

End Class
