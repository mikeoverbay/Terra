<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmBiasing
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBiasing))
        Me.decal_clip = New System.Windows.Forms.TrackBar()
        Me.terrain_clip = New System.Windows.Forms.TrackBar()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.exclude_chk_box = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.zoom_down = New System.Windows.Forms.Button()
        Me.zoom_up = New System.Windows.Forms.Button()
        Me.view_mode_btn = New System.Windows.Forms.Button()
        Me.show_select_mesh_btn = New System.Windows.Forms.Button()
        Me.results_tb = New System.Windows.Forms.TextBox()
        Me.decals = New System.Windows.Forms.Button()
        Me.models = New System.Windows.Forms.Button()
        Me.trees = New System.Windows.Forms.Button()
        Me.terrain = New System.Windows.Forms.Button()
        Me.info_tb = New System.Windows.Forms.TextBox()
        CType(Me.decal_clip, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.terrain_clip, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'decal_clip
        '
        Me.decal_clip.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.decal_clip.AutoSize = False
        Me.decal_clip.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.decal_clip.Cursor = System.Windows.Forms.Cursors.Hand
        Me.decal_clip.LargeChange = 1
        Me.decal_clip.Location = New System.Drawing.Point(271, 38)
        Me.decal_clip.Maximum = 1000
        Me.decal_clip.Name = "decal_clip"
        Me.decal_clip.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.decal_clip.Size = New System.Drawing.Size(31, 306)
        Me.decal_clip.TabIndex = 12
        Me.decal_clip.TickFrequency = 50
        '
        'terrain_clip
        '
        Me.terrain_clip.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.terrain_clip.AutoSize = False
        Me.terrain_clip.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.terrain_clip.Cursor = System.Windows.Forms.Cursors.Hand
        Me.terrain_clip.LargeChange = 1
        Me.terrain_clip.Location = New System.Drawing.Point(234, 38)
        Me.terrain_clip.Maximum = 1000
        Me.terrain_clip.Name = "terrain_clip"
        Me.terrain_clip.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.terrain_clip.Size = New System.Drawing.Size(31, 306)
        Me.terrain_clip.TabIndex = 11
        Me.terrain_clip.TickFrequency = 50
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(268, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(36, 26)
        Me.Label1.TabIndex = 14
        Me.Label1.Text = "Model" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Clip"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(226, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(40, 26)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "Terrain" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Clip"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'exclude_chk_box
        '
        Me.exclude_chk_box.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.exclude_chk_box.Appearance = System.Windows.Forms.Appearance.Button
        Me.exclude_chk_box.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.exclude_chk_box.ForeColor = System.Drawing.Color.White
        Me.exclude_chk_box.Location = New System.Drawing.Point(169, 237)
        Me.exclude_chk_box.Name = "exclude_chk_box"
        Me.exclude_chk_box.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.exclude_chk_box.Size = New System.Drawing.Size(54, 54)
        Me.exclude_chk_box.TabIndex = 15
        Me.exclude_chk_box.Text = "Exclude" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Models"
        Me.exclude_chk_box.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.exclude_chk_box.UseVisualStyleBackColor = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.zoom_down)
        Me.GroupBox1.Controls.Add(Me.zoom_up)
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(169, 9)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(54, 101)
        Me.GroupBox1.TabIndex = 16
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Zoom"
        '
        'zoom_down
        '
        Me.zoom_down.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.zoom_down.Image = Global.Terra.My.Resources.Resources.control_270
        Me.zoom_down.Location = New System.Drawing.Point(12, 58)
        Me.zoom_down.Name = "zoom_down"
        Me.zoom_down.Size = New System.Drawing.Size(30, 30)
        Me.zoom_down.TabIndex = 1
        Me.zoom_down.UseVisualStyleBackColor = False
        '
        'zoom_up
        '
        Me.zoom_up.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.zoom_up.Image = Global.Terra.My.Resources.Resources.control_090
        Me.zoom_up.Location = New System.Drawing.Point(12, 22)
        Me.zoom_up.Name = "zoom_up"
        Me.zoom_up.Size = New System.Drawing.Size(30, 30)
        Me.zoom_up.TabIndex = 0
        Me.zoom_up.UseVisualStyleBackColor = False
        '
        'view_mode_btn
        '
        Me.view_mode_btn.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.view_mode_btn.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.view_mode_btn.ForeColor = System.Drawing.Color.White
        Me.view_mode_btn.Location = New System.Drawing.Point(169, 116)
        Me.view_mode_btn.Name = "view_mode_btn"
        Me.view_mode_btn.Size = New System.Drawing.Size(54, 54)
        Me.view_mode_btn.TabIndex = 17
        Me.view_mode_btn.Text = "View" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Mode"
        Me.view_mode_btn.UseVisualStyleBackColor = False
        '
        'show_select_mesh_btn
        '
        Me.show_select_mesh_btn.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.show_select_mesh_btn.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.show_select_mesh_btn.ForeColor = System.Drawing.Color.White
        Me.show_select_mesh_btn.Location = New System.Drawing.Point(169, 177)
        Me.show_select_mesh_btn.Name = "show_select_mesh_btn"
        Me.show_select_mesh_btn.Size = New System.Drawing.Size(54, 54)
        Me.show_select_mesh_btn.TabIndex = 18
        Me.show_select_mesh_btn.Text = "Show" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Select" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Mesh"
        Me.show_select_mesh_btn.UseVisualStyleBackColor = False
        '
        'results_tb
        '
        Me.results_tb.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.results_tb.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.results_tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.results_tb.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.results_tb.ForeColor = System.Drawing.Color.White
        Me.results_tb.Location = New System.Drawing.Point(3, 9)
        Me.results_tb.Multiline = True
        Me.results_tb.Name = "results_tb"
        Me.results_tb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.results_tb.Size = New System.Drawing.Size(158, 308)
        Me.results_tb.TabIndex = 19
        Me.results_tb.WordWrap = False
        '
        'decals
        '
        Me.decals.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.decals.ForeColor = System.Drawing.Color.White
        Me.decals.Location = New System.Drawing.Point(169, 297)
        Me.decals.Name = "decals"
        Me.decals.Size = New System.Drawing.Size(20, 20)
        Me.decals.TabIndex = 20
        Me.decals.Text = "D"
        Me.decals.UseVisualStyleBackColor = False
        '
        'models
        '
        Me.models.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.models.ForeColor = System.Drawing.Color.White
        Me.models.Location = New System.Drawing.Point(203, 297)
        Me.models.Name = "models"
        Me.models.Size = New System.Drawing.Size(20, 20)
        Me.models.TabIndex = 21
        Me.models.Text = "M"
        Me.models.UseVisualStyleBackColor = False
        '
        'trees
        '
        Me.trees.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.trees.ForeColor = System.Drawing.Color.White
        Me.trees.Location = New System.Drawing.Point(169, 324)
        Me.trees.Name = "trees"
        Me.trees.Size = New System.Drawing.Size(20, 20)
        Me.trees.TabIndex = 22
        Me.trees.Text = "F"
        Me.trees.UseVisualStyleBackColor = False
        '
        'terrain
        '
        Me.terrain.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.terrain.ForeColor = System.Drawing.Color.White
        Me.terrain.Location = New System.Drawing.Point(203, 324)
        Me.terrain.Name = "terrain"
        Me.terrain.Size = New System.Drawing.Size(20, 20)
        Me.terrain.TabIndex = 23
        Me.terrain.Text = "T"
        Me.terrain.UseVisualStyleBackColor = False
        '
        'info_tb
        '
        Me.info_tb.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.info_tb.BackColor = System.Drawing.Color.Black
        Me.info_tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.info_tb.ForeColor = System.Drawing.Color.White
        Me.info_tb.Location = New System.Drawing.Point(3, 324)
        Me.info_tb.Name = "info_tb"
        Me.info_tb.Size = New System.Drawing.Size(158, 20)
        Me.info_tb.TabIndex = 24
        '
        'frmBiasing
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(308, 348)
        Me.Controls.Add(Me.info_tb)
        Me.Controls.Add(Me.terrain)
        Me.Controls.Add(Me.trees)
        Me.Controls.Add(Me.models)
        Me.Controls.Add(Me.decals)
        Me.Controls.Add(Me.results_tb)
        Me.Controls.Add(Me.show_select_mesh_btn)
        Me.Controls.Add(Me.view_mode_btn)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.exclude_chk_box)
        Me.Controls.Add(Me.decal_clip)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.terrain_clip)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(324, 900)
        Me.MinimumSize = New System.Drawing.Size(324, 382)
        Me.Name = "frmBiasing"
        Me.Text = "Biasing"
        Me.TopMost = True
        CType(Me.decal_clip, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.terrain_clip, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents decal_clip As System.Windows.Forms.TrackBar
    Friend WithEvents terrain_clip As System.Windows.Forms.TrackBar
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents exclude_chk_box As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents zoom_down As System.Windows.Forms.Button
    Friend WithEvents zoom_up As System.Windows.Forms.Button
    Friend WithEvents view_mode_btn As System.Windows.Forms.Button
    Friend WithEvents show_select_mesh_btn As System.Windows.Forms.Button
    Friend WithEvents results_tb As System.Windows.Forms.TextBox
    Friend WithEvents decals As System.Windows.Forms.Button
    Friend WithEvents models As System.Windows.Forms.Button
    Friend WithEvents trees As System.Windows.Forms.Button
    Friend WithEvents terrain As System.Windows.Forms.Button
    Friend WithEvents info_tb As System.Windows.Forms.TextBox
End Class
