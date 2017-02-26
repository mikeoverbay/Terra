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
        Me.RadioButton1 = New System.Windows.Forms.RadioButton()
        Me.img_2048_rb = New System.Windows.Forms.RadioButton()
        Me.img_1024_rb = New System.Windows.Forms.RadioButton()
        Me.img_512_rb = New System.Windows.Forms.RadioButton()
        Me.Render_btn = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.RadioButton1)
        Me.GroupBox1.Controls.Add(Me.img_2048_rb)
        Me.GroupBox1.Controls.Add(Me.img_1024_rb)
        Me.GroupBox1.Controls.Add(Me.img_512_rb)
        Me.GroupBox1.Location = New System.Drawing.Point(24, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(116, 126)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Set Image Size"
        '
        'RadioButton1
        '
        Me.RadioButton1.AutoSize = True
        Me.RadioButton1.Location = New System.Drawing.Point(6, 99)
        Me.RadioButton1.Name = "RadioButton1"
        Me.RadioButton1.Size = New System.Drawing.Size(84, 17)
        Me.RadioButton1.TabIndex = 3
        Me.RadioButton1.TabStop = True
        Me.RadioButton1.Text = "4096 x 4096"
        Me.RadioButton1.UseVisualStyleBackColor = True
        '
        'img_2048_rb
        '
        Me.img_2048_rb.AutoSize = True
        Me.img_2048_rb.Location = New System.Drawing.Point(6, 76)
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
        Me.img_1024_rb.Location = New System.Drawing.Point(6, 53)
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
        Me.img_512_rb.Location = New System.Drawing.Point(6, 30)
        Me.img_512_rb.Name = "img_512_rb"
        Me.img_512_rb.Size = New System.Drawing.Size(72, 17)
        Me.img_512_rb.TabIndex = 0
        Me.img_512_rb.TabStop = True
        Me.img_512_rb.Text = "512 x 512"
        Me.img_512_rb.UseVisualStyleBackColor = True
        '
        'Render_btn
        '
        Me.Render_btn.Location = New System.Drawing.Point(12, 164)
        Me.Render_btn.Name = "Render_btn"
        Me.Render_btn.Size = New System.Drawing.Size(63, 23)
        Me.Render_btn.TabIndex = 1
        Me.Render_btn.Text = "Render"
        Me.Render_btn.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(95, 164)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(61, 23)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "Cancel"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'FrmRenderMap
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(168, 203)
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
        Me.ResumeLayout(False)

    End Sub
	Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
	Friend WithEvents RadioButton1 As System.Windows.Forms.RadioButton
	Friend WithEvents img_2048_rb As System.Windows.Forms.RadioButton
	Friend WithEvents img_1024_rb As System.Windows.Forms.RadioButton
	Friend WithEvents img_512_rb As System.Windows.Forms.RadioButton
	Friend WithEvents Render_btn As System.Windows.Forms.Button
	Friend WithEvents Button1 As System.Windows.Forms.Button
End Class
