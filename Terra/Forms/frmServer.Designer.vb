<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmServer
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.debuggerNonUserCode()> _
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
	<System.Diagnostics.debuggerStepThrough()> _
	Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmServer))
        Me.stop_btn = New System.Windows.Forms.Button()
        Me.start_btn = New System.Windows.Forms.Button()
        Me.chat_text_tb = New System.Windows.Forms.TextBox()
        Me.Diag_tb = New System.Windows.Forms.TextBox()
        Me.host_ip_tb = New System.Windows.Forms.TextBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.diagnostics_bt = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.password_tb = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cancel_bt = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'stop_btn
        '
        Me.stop_btn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.stop_btn.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.stop_btn.Location = New System.Drawing.Point(155, 273)
        Me.stop_btn.Name = "stop_btn"
        Me.stop_btn.Size = New System.Drawing.Size(63, 23)
        Me.stop_btn.TabIndex = 1
        Me.stop_btn.Text = "Stop"
        Me.stop_btn.UseVisualStyleBackColor = False
        '
        'start_btn
        '
        Me.start_btn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.start_btn.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.start_btn.Location = New System.Drawing.Point(12, 273)
        Me.start_btn.Name = "start_btn"
        Me.start_btn.Size = New System.Drawing.Size(66, 23)
        Me.start_btn.TabIndex = 2
        Me.start_btn.Text = "Start"
        Me.start_btn.UseVisualStyleBackColor = False
        '
        'chat_text_tb
        '
        Me.chat_text_tb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chat_text_tb.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.chat_text_tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.chat_text_tb.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chat_text_tb.ForeColor = System.Drawing.Color.White
        Me.chat_text_tb.Location = New System.Drawing.Point(12, 86)
        Me.chat_text_tb.Multiline = True
        Me.chat_text_tb.Name = "chat_text_tb"
        Me.chat_text_tb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.chat_text_tb.Size = New System.Drawing.Size(206, 184)
        Me.chat_text_tb.TabIndex = 4
        '
        'Diag_tb
        '
        Me.Diag_tb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Diag_tb.BackColor = System.Drawing.Color.Silver
        Me.Diag_tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Diag_tb.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Diag_tb.Location = New System.Drawing.Point(12, 86)
        Me.Diag_tb.MaxLength = 200
        Me.Diag_tb.Multiline = True
        Me.Diag_tb.Name = "Diag_tb"
        Me.Diag_tb.Size = New System.Drawing.Size(206, 184)
        Me.Diag_tb.TabIndex = 5
        '
        'host_ip_tb
        '
        Me.host_ip_tb.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.host_ip_tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.host_ip_tb.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.Terra.My.MySettings.Default, "server_address", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.host_ip_tb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.host_ip_tb.ForeColor = System.Drawing.Color.Lime
        Me.host_ip_tb.Location = New System.Drawing.Point(12, 6)
        Me.host_ip_tb.Name = "host_ip_tb"
        Me.host_ip_tb.Size = New System.Drawing.Size(111, 20)
        Me.host_ip_tb.TabIndex = 6
        Me.host_ip_tb.Text = Global.Terra.My.MySettings.Default.server_address
        Me.host_ip_tb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Button1.Location = New System.Drawing.Point(12, 29)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(102, 23)
        Me.Button1.TabIndex = 7
        Me.Button1.Text = "Whats my ip?"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'diagnostics_bt
        '
        Me.diagnostics_bt.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.diagnostics_bt.Location = New System.Drawing.Point(120, 29)
        Me.diagnostics_bt.Name = "diagnostics_bt"
        Me.diagnostics_bt.Size = New System.Drawing.Size(98, 23)
        Me.diagnostics_bt.TabIndex = 8
        Me.diagnostics_bt.Text = "Diagnostics"
        Me.diagnostics_bt.UseVisualStyleBackColor = False
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.Button2.Location = New System.Drawing.Point(12, 58)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(102, 23)
        Me.Button2.TabIndex = 9
        Me.Button2.Text = "Port Forwarding?"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(129, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(86, 13)
        Me.Label1.TabIndex = 10
        Me.Label1.Text = "<---Your WAN IP"
        '
        'password_tb
        '
        Me.password_tb.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.password_tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.password_tb.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.Terra.My.MySettings.Default, "lidgren_pw", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.password_tb.Location = New System.Drawing.Point(120, 60)
        Me.password_tb.Name = "password_tb"
        Me.password_tb.Size = New System.Drawing.Size(72, 20)
        Me.password_tb.TabIndex = 11
        Me.password_tb.Text = Global.Terra.My.MySettings.Default.lidgren_pw
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(197, 63)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(25, 13)
        Me.Label2.TabIndex = 12
        Me.Label2.Text = "PW"
        '
        'cancel_bt
        '
        Me.cancel_bt.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cancel_bt.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.cancel_bt.ForeColor = System.Drawing.Color.Black
        Me.cancel_bt.Location = New System.Drawing.Point(88, 273)
        Me.cancel_bt.Name = "cancel_bt"
        Me.cancel_bt.Size = New System.Drawing.Size(57, 23)
        Me.cancel_bt.TabIndex = 13
        Me.cancel_bt.Text = "Cancel"
        Me.cancel_bt.UseVisualStyleBackColor = False
        '
        'frmServer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(229, 300)
        Me.ControlBox = False
        Me.Controls.Add(Me.cancel_bt)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.password_tb)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.diagnostics_bt)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.host_ip_tb)
        Me.Controls.Add(Me.start_btn)
        Me.Controls.Add(Me.stop_btn)
        Me.Controls.Add(Me.Diag_tb)
        Me.Controls.Add(Me.chat_text_tb)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(500, 500)
        Me.MinimumSize = New System.Drawing.Size(245, 316)
        Me.Name = "frmServer"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Terra! Server"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
	Friend WithEvents stop_btn As System.Windows.Forms.Button
	Friend WithEvents start_btn As System.Windows.Forms.Button
	Friend WithEvents chat_text_tb As System.Windows.Forms.TextBox
	Friend WithEvents Diag_tb As System.Windows.Forms.TextBox
	Friend WithEvents host_ip_tb As System.Windows.Forms.TextBox
	Friend WithEvents Button1 As System.Windows.Forms.Button
	Friend WithEvents diagnostics_bt As System.Windows.Forms.Button
	Friend WithEvents Button2 As System.Windows.Forms.Button
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents password_tb As System.Windows.Forms.TextBox
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents cancel_bt As System.Windows.Forms.Button

End Class
