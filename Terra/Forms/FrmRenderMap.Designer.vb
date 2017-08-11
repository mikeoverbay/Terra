<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmRenderMap
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
        Me.img_2048_rb = New System.Windows.Forms.RadioButton()
        Me.img_1024_rb = New System.Windows.Forms.RadioButton()
        Me.img_512_rb = New System.Windows.Forms.RadioButton()
        Me.Render_btn = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.open_after_save_cb = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.m_as_jpg = New System.Windows.Forms.RadioButton()
        Me.m_as_png = New System.Windows.Forms.RadioButton()
        Me.m_as_dds_5 = New System.Windows.Forms.RadioButton()
        Me.m_as_bmp = New System.Windows.Forms.RadioButton()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.m_r_screen = New System.Windows.Forms.RadioButton()
        Me.m_r_layout = New System.Windows.Forms.RadioButton()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.img_2048_rb)
        Me.GroupBox1.Controls.Add(Me.img_1024_rb)
        Me.GroupBox1.Controls.Add(Me.img_512_rb)
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(24, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(116, 97)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Set Image Size"
        '
        'img_2048_rb
        '
        Me.img_2048_rb.AutoSize = True
        Me.img_2048_rb.Location = New System.Drawing.Point(13, 68)
        Me.img_2048_rb.Name = "img_2048_rb"
        Me.img_2048_rb.Size = New System.Drawing.Size(84, 17)
        Me.img_2048_rb.TabIndex = 2
        Me.img_2048_rb.TabStop = True
        Me.img_2048_rb.Text = "2048 x 2048"
        Me.img_2048_rb.UseVisualStyleBackColor = True
        '
        'img_1024_rb
        '
        Me.img_1024_rb.AutoSize = True
        Me.img_1024_rb.Location = New System.Drawing.Point(13, 45)
        Me.img_1024_rb.Name = "img_1024_rb"
        Me.img_1024_rb.Size = New System.Drawing.Size(84, 17)
        Me.img_1024_rb.TabIndex = 1
        Me.img_1024_rb.TabStop = True
        Me.img_1024_rb.Text = "1024 x 1024"
        Me.img_1024_rb.UseVisualStyleBackColor = True
        '
        'img_512_rb
        '
        Me.img_512_rb.AutoSize = True
        Me.img_512_rb.Location = New System.Drawing.Point(13, 22)
        Me.img_512_rb.Name = "img_512_rb"
        Me.img_512_rb.Size = New System.Drawing.Size(72, 17)
        Me.img_512_rb.TabIndex = 0
        Me.img_512_rb.TabStop = True
        Me.img_512_rb.Text = "512 x 512"
        Me.img_512_rb.UseVisualStyleBackColor = True
        '
        'Render_btn
        '
        Me.Render_btn.ForeColor = System.Drawing.Color.White
        Me.Render_btn.Location = New System.Drawing.Point(12, 344)
        Me.Render_btn.Name = "Render_btn"
        Me.Render_btn.Size = New System.Drawing.Size(63, 23)
        Me.Render_btn.TabIndex = 1
        Me.Render_btn.Text = "Render"
        Me.Render_btn.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.ForeColor = System.Drawing.Color.White
        Me.Button1.Location = New System.Drawing.Point(95, 344)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(61, 23)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "Cancel"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'open_after_save_cb
        '
        Me.open_after_save_cb.AutoSize = True
        Me.open_after_save_cb.Checked = True
        Me.open_after_save_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.open_after_save_cb.ForeColor = System.Drawing.Color.White
        Me.open_after_save_cb.Location = New System.Drawing.Point(30, 317)
        Me.open_after_save_cb.Name = "open_after_save_cb"
        Me.open_after_save_cb.Size = New System.Drawing.Size(105, 17)
        Me.open_after_save_cb.TabIndex = 3
        Me.open_after_save_cb.Text = "Open After Save"
        Me.open_after_save_cb.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.m_as_jpg)
        Me.GroupBox2.Controls.Add(Me.m_as_png)
        Me.GroupBox2.Controls.Add(Me.m_as_dds_5)
        Me.GroupBox2.Controls.Add(Me.m_as_bmp)
        Me.GroupBox2.ForeColor = System.Drawing.Color.White
        Me.GroupBox2.Location = New System.Drawing.Point(24, 116)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(116, 113)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Save As"
        '
        'm_as_jpg
        '
        Me.m_as_jpg.AutoSize = True
        Me.m_as_jpg.Location = New System.Drawing.Point(13, 89)
        Me.m_as_jpg.Name = "m_as_jpg"
        Me.m_as_jpg.Size = New System.Drawing.Size(52, 17)
        Me.m_as_jpg.TabIndex = 3
        Me.m_as_jpg.Text = "JPEG"
        Me.m_as_jpg.UseVisualStyleBackColor = True
        '
        'm_as_png
        '
        Me.m_as_png.AutoSize = True
        Me.m_as_png.Checked = True
        Me.m_as_png.Location = New System.Drawing.Point(13, 66)
        Me.m_as_png.Name = "m_as_png"
        Me.m_as_png.Size = New System.Drawing.Size(48, 17)
        Me.m_as_png.TabIndex = 2
        Me.m_as_png.TabStop = True
        Me.m_as_png.Text = "PNG"
        Me.m_as_png.UseVisualStyleBackColor = True
        '
        'm_as_dds_5
        '
        Me.m_as_dds_5.AutoSize = True
        Me.m_as_dds_5.Location = New System.Drawing.Point(13, 43)
        Me.m_as_dds_5.Name = "m_as_dds_5"
        Me.m_as_dds_5.Size = New System.Drawing.Size(57, 17)
        Me.m_as_dds_5.TabIndex = 1
        Me.m_as_dds_5.Text = "DDS 5"
        Me.m_as_dds_5.UseVisualStyleBackColor = True
        '
        'm_as_bmp
        '
        Me.m_as_bmp.AutoSize = True
        Me.m_as_bmp.Location = New System.Drawing.Point(13, 20)
        Me.m_as_bmp.Name = "m_as_bmp"
        Me.m_as_bmp.Size = New System.Drawing.Size(48, 17)
        Me.m_as_bmp.TabIndex = 0
        Me.m_as_bmp.Text = "BMP"
        Me.m_as_bmp.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.m_r_screen)
        Me.GroupBox3.Controls.Add(Me.m_r_layout)
        Me.GroupBox3.ForeColor = System.Drawing.Color.White
        Me.GroupBox3.Location = New System.Drawing.Point(24, 235)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(116, 70)
        Me.GroupBox3.TabIndex = 5
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Source"
        '
        'm_r_screen
        '
        Me.m_r_screen.AutoSize = True
        Me.m_r_screen.Location = New System.Drawing.Point(13, 41)
        Me.m_r_screen.Name = "m_r_screen"
        Me.m_r_screen.Size = New System.Drawing.Size(59, 17)
        Me.m_r_screen.TabIndex = 3
        Me.m_r_screen.Text = "Screen"
        Me.m_r_screen.UseVisualStyleBackColor = True
        '
        'm_r_layout
        '
        Me.m_r_layout.AutoSize = True
        Me.m_r_layout.Checked = True
        Me.m_r_layout.Location = New System.Drawing.Point(13, 18)
        Me.m_r_layout.Name = "m_r_layout"
        Me.m_r_layout.Size = New System.Drawing.Size(57, 17)
        Me.m_r_layout.TabIndex = 2
        Me.m_r_layout.TabStop = True
        Me.m_r_layout.Text = "Layout"
        Me.m_r_layout.UseVisualStyleBackColor = True
        '
        'FrmRenderMap
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(168, 380)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.open_after_save_cb)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Render_btn)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.Name = "FrmRenderMap"
        Me.ShowInTaskbar = False
        Me.Text = "Render to BitMap"
        Me.TopMost = True
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents img_2048_rb As System.Windows.Forms.RadioButton
    Friend WithEvents img_1024_rb As System.Windows.Forms.RadioButton
    Friend WithEvents img_512_rb As System.Windows.Forms.RadioButton
    Friend WithEvents Render_btn As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents open_after_save_cb As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents m_as_jpg As System.Windows.Forms.RadioButton
    Friend WithEvents m_as_png As System.Windows.Forms.RadioButton
    Friend WithEvents m_as_dds_5 As System.Windows.Forms.RadioButton
    Friend WithEvents m_as_bmp As System.Windows.Forms.RadioButton
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents m_r_screen As System.Windows.Forms.RadioButton
    Friend WithEvents m_r_layout As System.Windows.Forms.RadioButton
End Class
