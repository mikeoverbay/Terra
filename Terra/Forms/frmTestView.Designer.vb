<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTestView
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTestView))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.pb3 = New System.Windows.Forms.PictureBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.sixtenth_scale = New System.Windows.Forms.RadioButton()
        Me.eigth_scale = New System.Windows.Forms.RadioButton()
        Me.quater_scale = New System.Windows.Forms.RadioButton()
        Me.half_scale = New System.Windows.Forms.RadioButton()
        Me.full_scale = New System.Windows.Forms.RadioButton()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.img_2 = New System.Windows.Forms.RadioButton()
        Me.img_1 = New System.Windows.Forms.RadioButton()
        Me.w_label = New System.Windows.Forms.Label()
        Me.h_label = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        CType(Me.pb3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.BackColor = System.Drawing.Color.Black
        Me.Panel1.Controls.Add(Me.pb3)
        Me.Panel1.Location = New System.Drawing.Point(1, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(385, 235)
        Me.Panel1.TabIndex = 0
        '
        'pb3
        '
        Me.pb3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pb3.Location = New System.Drawing.Point(0, 0)
        Me.pb3.Name = "pb3"
        Me.pb3.Size = New System.Drawing.Size(385, 235)
        Me.pb3.TabIndex = 0
        Me.pb3.TabStop = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.sixtenth_scale)
        Me.GroupBox1.Controls.Add(Me.eigth_scale)
        Me.GroupBox1.Controls.Add(Me.quater_scale)
        Me.GroupBox1.Controls.Add(Me.half_scale)
        Me.GroupBox1.Controls.Add(Me.full_scale)
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(392, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(60, 172)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Size"
        '
        'sixtenth_scale
        '
        Me.sixtenth_scale.Appearance = System.Windows.Forms.Appearance.Button
        Me.sixtenth_scale.ForeColor = System.Drawing.Color.White
        Me.sixtenth_scale.Location = New System.Drawing.Point(7, 135)
        Me.sixtenth_scale.Name = "sixtenth_scale"
        Me.sixtenth_scale.Size = New System.Drawing.Size(45, 23)
        Me.sixtenth_scale.TabIndex = 4
        Me.sixtenth_scale.Tag = "0.0625"
        Me.sixtenth_scale.Text = "1/16"
        Me.sixtenth_scale.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.sixtenth_scale.UseVisualStyleBackColor = True
        '
        'eigth_scale
        '
        Me.eigth_scale.Appearance = System.Windows.Forms.Appearance.Button
        Me.eigth_scale.ForeColor = System.Drawing.Color.White
        Me.eigth_scale.Location = New System.Drawing.Point(7, 106)
        Me.eigth_scale.Name = "eigth_scale"
        Me.eigth_scale.Size = New System.Drawing.Size(45, 23)
        Me.eigth_scale.TabIndex = 3
        Me.eigth_scale.Tag = "0.125"
        Me.eigth_scale.Text = "1/8"
        Me.eigth_scale.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.eigth_scale.UseVisualStyleBackColor = True
        '
        'quater_scale
        '
        Me.quater_scale.Appearance = System.Windows.Forms.Appearance.Button
        Me.quater_scale.Checked = True
        Me.quater_scale.ForeColor = System.Drawing.Color.White
        Me.quater_scale.Location = New System.Drawing.Point(7, 77)
        Me.quater_scale.Name = "quater_scale"
        Me.quater_scale.Size = New System.Drawing.Size(45, 23)
        Me.quater_scale.TabIndex = 2
        Me.quater_scale.TabStop = True
        Me.quater_scale.Tag = "0.25"
        Me.quater_scale.Text = "1/4"
        Me.quater_scale.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.quater_scale.UseVisualStyleBackColor = True
        '
        'half_scale
        '
        Me.half_scale.Appearance = System.Windows.Forms.Appearance.Button
        Me.half_scale.ForeColor = System.Drawing.Color.White
        Me.half_scale.Location = New System.Drawing.Point(7, 48)
        Me.half_scale.Name = "half_scale"
        Me.half_scale.Size = New System.Drawing.Size(45, 23)
        Me.half_scale.TabIndex = 1
        Me.half_scale.Tag = "0.5"
        Me.half_scale.Text = "1/2"
        Me.half_scale.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.half_scale.UseVisualStyleBackColor = True
        '
        'full_scale
        '
        Me.full_scale.Appearance = System.Windows.Forms.Appearance.Button
        Me.full_scale.ForeColor = System.Drawing.Color.White
        Me.full_scale.Location = New System.Drawing.Point(7, 19)
        Me.full_scale.Name = "full_scale"
        Me.full_scale.Size = New System.Drawing.Size(45, 23)
        Me.full_scale.TabIndex = 0
        Me.full_scale.Tag = "1.0"
        Me.full_scale.Text = "Full"
        Me.full_scale.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.full_scale.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.img_2)
        Me.GroupBox2.Controls.Add(Me.img_1)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.GroupBox2.ForeColor = System.Drawing.Color.White
        Me.GroupBox2.Location = New System.Drawing.Point(0, 241)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(464, 41)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Selected Image"
        '
        'img_2
        '
        Me.img_2.Appearance = System.Windows.Forms.Appearance.Button
        Me.img_2.AutoSize = True
        Me.img_2.Location = New System.Drawing.Point(97, 14)
        Me.img_2.Name = "img_2"
        Me.img_2.Size = New System.Drawing.Size(62, 23)
        Me.img_2.TabIndex = 1
        Me.img_2.Tag = "2"
        Me.img_2.Text = "ColorMap"
        Me.img_2.UseVisualStyleBackColor = True
        '
        'img_1
        '
        Me.img_1.Appearance = System.Windows.Forms.Appearance.Button
        Me.img_1.AutoSize = True
        Me.img_1.Checked = True
        Me.img_1.Location = New System.Drawing.Point(11, 14)
        Me.img_1.Name = "img_1"
        Me.img_1.Size = New System.Drawing.Size(67, 23)
        Me.img_1.TabIndex = 0
        Me.img_1.TabStop = True
        Me.img_1.Tag = "1"
        Me.img_1.Text = "DepthMap"
        Me.img_1.UseVisualStyleBackColor = True
        '
        'w_label
        '
        Me.w_label.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.w_label.AutoSize = True
        Me.w_label.ForeColor = System.Drawing.Color.White
        Me.w_label.Location = New System.Drawing.Point(389, 201)
        Me.w_label.Name = "w_label"
        Me.w_label.Size = New System.Drawing.Size(38, 13)
        Me.w_label.TabIndex = 3
        Me.w_label.Text = "Width:"
        '
        'h_label
        '
        Me.h_label.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.h_label.AutoSize = True
        Me.h_label.ForeColor = System.Drawing.Color.White
        Me.h_label.Location = New System.Drawing.Point(386, 219)
        Me.h_label.Name = "h_label"
        Me.h_label.Size = New System.Drawing.Size(41, 13)
        Me.h_label.TabIndex = 4
        Me.h_label.Text = "Height:"
        '
        'frmTestView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(464, 282)
        Me.Controls.Add(Me.h_label)
        Me.Controls.Add(Me.w_label)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(480, 320)
        Me.Name = "frmTestView"
        Me.Text = "Back Buffer Viewer"
        Me.TopMost = True
        Me.Panel1.ResumeLayout(False)
        CType(Me.pb3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents sixtenth_scale As System.Windows.Forms.RadioButton
    Friend WithEvents eigth_scale As System.Windows.Forms.RadioButton
    Friend WithEvents quater_scale As System.Windows.Forms.RadioButton
    Friend WithEvents half_scale As System.Windows.Forms.RadioButton
    Friend WithEvents full_scale As System.Windows.Forms.RadioButton
    Friend WithEvents pb3 As System.Windows.Forms.PictureBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents img_1 As System.Windows.Forms.RadioButton
    Friend WithEvents w_label As System.Windows.Forms.Label
    Friend WithEvents h_label As System.Windows.Forms.Label
    Friend WithEvents img_2 As System.Windows.Forms.RadioButton
End Class
