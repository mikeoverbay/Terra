<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLighting
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.s_gamma = New System.Windows.Forms.TrackBar()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.s_terrain_ambient = New System.Windows.Forms.TrackBar()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.s_terrain_texture_level = New System.Windows.Forms.TrackBar()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.s_fog_level = New System.Windows.Forms.TrackBar()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.s_model_level = New System.Windows.Forms.TrackBar()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.s_gray_level = New System.Windows.Forms.TrackBar()
        Me.GroupBox1.SuspendLayout()
        CType(Me.s_gamma, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.s_terrain_ambient, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.s_terrain_texture_level, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        CType(Me.s_fog_level, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox3.SuspendLayout()
        CType(Me.s_model_level, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox4.SuspendLayout()
        CType(Me.s_gray_level, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.s_gamma)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.s_terrain_ambient)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.s_terrain_texture_level)
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(156, 223)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Terrain"
        '
        's_gamma
        '
        Me.s_gamma.Cursor = System.Windows.Forms.Cursors.Hand
        Me.s_gamma.DataBindings.Add(New System.Windows.Forms.Binding("Value", Global.Terra.My.MySettings.Default, "s_gamma", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.s_gamma.LargeChange = 10
        Me.s_gamma.Location = New System.Drawing.Point(104, 19)
        Me.s_gamma.Maximum = 100
        Me.s_gamma.Name = "s_gamma"
        Me.s_gamma.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.s_gamma.Size = New System.Drawing.Size(45, 155)
        Me.s_gamma.TabIndex = 5
        Me.s_gamma.TickFrequency = 10
        Me.s_gamma.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.s_gamma.Value = Global.Terra.My.MySettings.Default.s_gamma
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(101, 177)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(43, 26)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Gamma" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Level"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        's_terrain_ambient
        '
        Me.s_terrain_ambient.Cursor = System.Windows.Forms.Cursors.Hand
        Me.s_terrain_ambient.DataBindings.Add(New System.Windows.Forms.Binding("Value", Global.Terra.My.MySettings.Default, "s_terrain_ambient_level", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.s_terrain_ambient.LargeChange = 10
        Me.s_terrain_ambient.Location = New System.Drawing.Point(55, 19)
        Me.s_terrain_ambient.Maximum = 100
        Me.s_terrain_ambient.Name = "s_terrain_ambient"
        Me.s_terrain_ambient.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.s_terrain_ambient.Size = New System.Drawing.Size(45, 155)
        Me.s_terrain_ambient.TabIndex = 3
        Me.s_terrain_ambient.TickFrequency = 10
        Me.s_terrain_ambient.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.s_terrain_ambient.Value = Global.Terra.My.MySettings.Default.s_terrain_ambient_level
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(52, 177)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(45, 26)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Ambient" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Level"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 177)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(43, 26)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Texture" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Level"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        's_terrain_texture_level
        '
        Me.s_terrain_texture_level.Cursor = System.Windows.Forms.Cursors.Hand
        Me.s_terrain_texture_level.DataBindings.Add(New System.Windows.Forms.Binding("Value", Global.Terra.My.MySettings.Default, "s_terrian_texture_level", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.s_terrain_texture_level.LargeChange = 10
        Me.s_terrain_texture_level.Location = New System.Drawing.Point(6, 19)
        Me.s_terrain_texture_level.Maximum = 100
        Me.s_terrain_texture_level.Name = "s_terrain_texture_level"
        Me.s_terrain_texture_level.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.s_terrain_texture_level.Size = New System.Drawing.Size(45, 155)
        Me.s_terrain_texture_level.TabIndex = 0
        Me.s_terrain_texture_level.TickFrequency = 10
        Me.s_terrain_texture_level.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.s_terrain_texture_level.Value = Global.Terra.My.MySettings.Default.s_terrian_texture_level
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.s_fog_level)
        Me.GroupBox2.ForeColor = System.Drawing.Color.White
        Me.GroupBox2.Location = New System.Drawing.Point(174, 12)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(64, 223)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Fog"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 177)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(33, 26)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "Fog" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Level"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        's_fog_level
        '
        Me.s_fog_level.Cursor = System.Windows.Forms.Cursors.Hand
        Me.s_fog_level.DataBindings.Add(New System.Windows.Forms.Binding("Value", Global.Terra.My.MySettings.Default, "s_fog_level", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.s_fog_level.LargeChange = 10
        Me.s_fog_level.Location = New System.Drawing.Point(9, 19)
        Me.s_fog_level.Maximum = 100
        Me.s_fog_level.Name = "s_fog_level"
        Me.s_fog_level.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.s_fog_level.Size = New System.Drawing.Size(45, 155)
        Me.s_fog_level.TabIndex = 0
        Me.s_fog_level.TickFrequency = 10
        Me.s_fog_level.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.s_fog_level.Value = Global.Terra.My.MySettings.Default.s_fog_level
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Label3)
        Me.GroupBox3.Controls.Add(Me.s_model_level)
        Me.GroupBox3.ForeColor = System.Drawing.Color.White
        Me.GroupBox3.Location = New System.Drawing.Point(317, 12)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(64, 223)
        Me.GroupBox3.TabIndex = 3
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Models"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(15, 177)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(36, 26)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "Model" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Level" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        's_model_level
        '
        Me.s_model_level.Cursor = System.Windows.Forms.Cursors.Hand
        Me.s_model_level.DataBindings.Add(New System.Windows.Forms.Binding("Value", Global.Terra.My.MySettings.Default, "s_model_level", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.s_model_level.LargeChange = 10
        Me.s_model_level.Location = New System.Drawing.Point(11, 19)
        Me.s_model_level.Maximum = 100
        Me.s_model_level.Name = "s_model_level"
        Me.s_model_level.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.s_model_level.Size = New System.Drawing.Size(45, 155)
        Me.s_model_level.TabIndex = 0
        Me.s_model_level.TickFrequency = 10
        Me.s_model_level.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.s_model_level.Value = Global.Terra.My.MySettings.Default.s_model_level
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(306, 241)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "Close"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.Label6)
        Me.GroupBox4.Controls.Add(Me.s_gray_level)
        Me.GroupBox4.ForeColor = System.Drawing.Color.White
        Me.GroupBox4.Location = New System.Drawing.Point(245, 12)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(64, 223)
        Me.GroupBox4.TabIndex = 4
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Tone"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(15, 177)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(33, 26)
        Me.Label6.TabIndex = 1
        Me.Label6.Text = "Gray" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Level"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        's_gray_level
        '
        Me.s_gray_level.Cursor = System.Windows.Forms.Cursors.Hand
        Me.s_gray_level.DataBindings.Add(New System.Windows.Forms.Binding("Value", Global.Terra.My.MySettings.Default, "s_gray_level", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.s_gray_level.LargeChange = 10
        Me.s_gray_level.Location = New System.Drawing.Point(11, 19)
        Me.s_gray_level.Maximum = 100
        Me.s_gray_level.Name = "s_gray_level"
        Me.s_gray_level.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.s_gray_level.Size = New System.Drawing.Size(45, 155)
        Me.s_gray_level.TabIndex = 0
        Me.s_gray_level.TickFrequency = 10
        Me.s_gray_level.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.s_gray_level.Value = Global.Terra.My.MySettings.Default.s_gray_level
        '
        'frmLighting
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Gray
        Me.ClientSize = New System.Drawing.Size(394, 275)
        Me.ControlBox = False
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.Name = "frmLighting"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Lighting and fog settings"
        Me.TopMost = True
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.s_gamma, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.s_terrain_ambient, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.s_terrain_texture_level, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.s_fog_level, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        CType(Me.s_model_level, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        CType(Me.s_gray_level, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
	Friend WithEvents s_terrain_texture_level As System.Windows.Forms.TrackBar
	Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
	Friend WithEvents s_terrain_ambient As System.Windows.Forms.TrackBar
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
	Friend WithEvents Label4 As System.Windows.Forms.Label
	Friend WithEvents s_fog_level As System.Windows.Forms.TrackBar
	Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents s_model_level As System.Windows.Forms.TrackBar
	Friend WithEvents Button1 As System.Windows.Forms.Button
	Friend WithEvents s_gamma As System.Windows.Forms.TrackBar
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents s_gray_level As System.Windows.Forms.TrackBar
End Class
