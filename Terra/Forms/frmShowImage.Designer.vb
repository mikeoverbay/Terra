<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmShowImage
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmShowImage))
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.ListBox2 = New System.Windows.Forms.ListBox()
        Me.SPC = New System.Windows.Forms.SplitContainer()
        Me.alpha_cb = New System.Windows.Forms.CheckBox()
        Me.btn_scale_down = New System.Windows.Forms.Button()
        Me.btn_scale_up = New System.Windows.Forms.Button()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        CType(Me.SPC, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SPC.Panel1.SuspendLayout()
        Me.SPC.Panel2.SuspendLayout()
        Me.SPC.SuspendLayout()
        Me.SuspendLayout()
        '
        'ListBox1
        '
        Me.ListBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ListBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListBox1.ForeColor = System.Drawing.Color.White
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(3, 3)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(277, 466)
        Me.ListBox1.TabIndex = 0
        '
        'TextBox1
        '
        Me.TextBox1.BackColor = System.Drawing.Color.Black
        Me.TextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.TextBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox1.ForeColor = System.Drawing.Color.White
        Me.TextBox1.Location = New System.Drawing.Point(0, 476)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(457, 25)
        Me.TextBox1.TabIndex = 1
        '
        'TabControl1
        '
        Me.TabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(291, 501)
        Me.TabControl1.TabIndex = 2
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.ListBox1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 25)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(283, 472)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Terrain"
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.ListBox2)
        Me.TabPage2.Location = New System.Drawing.Point(4, 25)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(283, 472)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Models"
        '
        'ListBox2
        '
        Me.ListBox2.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ListBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListBox2.ForeColor = System.Drawing.Color.White
        Me.ListBox2.FormattingEnabled = True
        Me.ListBox2.Location = New System.Drawing.Point(3, 3)
        Me.ListBox2.Name = "ListBox2"
        Me.ListBox2.Size = New System.Drawing.Size(277, 466)
        Me.ListBox2.TabIndex = 1
        '
        'SPC
        '
        Me.SPC.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SPC.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SPC.Location = New System.Drawing.Point(0, 0)
        Me.SPC.Name = "SPC"
        '
        'SPC.Panel1
        '
        Me.SPC.Panel1.BackColor = System.Drawing.Color.Black
        Me.SPC.Panel1.Controls.Add(Me.alpha_cb)
        Me.SPC.Panel1.Controls.Add(Me.btn_scale_down)
        Me.SPC.Panel1.Controls.Add(Me.btn_scale_up)
        Me.SPC.Panel1.Controls.Add(Me.TextBox1)
        '
        'SPC.Panel2
        '
        Me.SPC.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.SPC.Panel2.Controls.Add(Me.TabControl1)
        Me.SPC.Size = New System.Drawing.Size(752, 501)
        Me.SPC.SplitterDistance = 457
        Me.SPC.TabIndex = 3
        '
        'alpha_cb
        '
        Me.alpha_cb.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.alpha_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.alpha_cb.AutoSize = True
        Me.alpha_cb.BackColor = System.Drawing.Color.DimGray
        Me.alpha_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.alpha_cb.ForeColor = System.Drawing.Color.Black
        Me.alpha_cb.Location = New System.Drawing.Point(354, 477)
        Me.alpha_cb.Name = "alpha_cb"
        Me.alpha_cb.Size = New System.Drawing.Size(49, 23)
        Me.alpha_cb.TabIndex = 4
        Me.alpha_cb.Text = "Alpha"
        Me.alpha_cb.UseVisualStyleBackColor = False
        '
        'btn_scale_down
        '
        Me.btn_scale_down.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btn_scale_down.BackColor = System.Drawing.Color.DimGray
        Me.btn_scale_down.BackgroundImage = Global.Terra.My.Resources.Resources.control_270
        Me.btn_scale_down.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btn_scale_down.ForeColor = System.Drawing.Color.Black
        Me.btn_scale_down.Location = New System.Drawing.Point(430, 477)
        Me.btn_scale_down.Name = "btn_scale_down"
        Me.btn_scale_down.Size = New System.Drawing.Size(25, 23)
        Me.btn_scale_down.TabIndex = 3
        Me.btn_scale_down.UseVisualStyleBackColor = False
        '
        'btn_scale_up
        '
        Me.btn_scale_up.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btn_scale_up.BackColor = System.Drawing.Color.DimGray
        Me.btn_scale_up.BackgroundImage = Global.Terra.My.Resources.Resources.control_090
        Me.btn_scale_up.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btn_scale_up.ForeColor = System.Drawing.Color.Black
        Me.btn_scale_up.Location = New System.Drawing.Point(404, 477)
        Me.btn_scale_up.Name = "btn_scale_up"
        Me.btn_scale_up.Size = New System.Drawing.Size(25, 23)
        Me.btn_scale_up.TabIndex = 2
        Me.btn_scale_up.UseVisualStyleBackColor = False
        '
        'frmShowImage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(752, 501)
        Me.Controls.Add(Me.SPC)
        Me.ForeColor = System.Drawing.Color.Silver
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmShowImage"
        Me.Text = "Show Image"
        Me.TopMost = True
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.SPC.Panel1.ResumeLayout(False)
        Me.SPC.Panel1.PerformLayout()
        Me.SPC.Panel2.ResumeLayout(False)
        CType(Me.SPC, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SPC.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents ListBox2 As System.Windows.Forms.ListBox
    Friend WithEvents SPC As System.Windows.Forms.SplitContainer
    Friend WithEvents btn_scale_down As System.Windows.Forms.Button
    Friend WithEvents btn_scale_up As System.Windows.Forms.Button
    Friend WithEvents alpha_cb As System.Windows.Forms.CheckBox
End Class
