<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmStats
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmStats))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.rt_terrian = New System.Windows.Forms.Label()
        Me.rt_models = New System.Windows.Forms.Label()
        Me.rt_trees = New System.Windows.Forms.Label()
        Me.rt_total = New System.Windows.Forms.Label()
        Me.rt_decals = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(74, 26)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Render times" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "in milliseconds"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 46)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(43, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Terrain:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(11, 68)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(44, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Models:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(18, 89)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(37, 13)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Trees:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(21, 138)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(34, 13)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Total:"
        '
        'rt_terrian
        '
        Me.rt_terrian.AutoSize = True
        Me.rt_terrian.Location = New System.Drawing.Point(57, 46)
        Me.rt_terrian.Name = "rt_terrian"
        Me.rt_terrian.Size = New System.Drawing.Size(19, 13)
        Me.rt_terrian.TabIndex = 5
        Me.rt_terrian.Text = "00"
        '
        'rt_models
        '
        Me.rt_models.AutoSize = True
        Me.rt_models.Location = New System.Drawing.Point(57, 68)
        Me.rt_models.Name = "rt_models"
        Me.rt_models.Size = New System.Drawing.Size(19, 13)
        Me.rt_models.TabIndex = 6
        Me.rt_models.Text = "00"
        '
        'rt_trees
        '
        Me.rt_trees.AutoSize = True
        Me.rt_trees.Location = New System.Drawing.Point(57, 90)
        Me.rt_trees.Name = "rt_trees"
        Me.rt_trees.Size = New System.Drawing.Size(19, 13)
        Me.rt_trees.TabIndex = 7
        Me.rt_trees.Text = "00"
        '
        'rt_total
        '
        Me.rt_total.AutoSize = True
        Me.rt_total.Location = New System.Drawing.Point(57, 138)
        Me.rt_total.Name = "rt_total"
        Me.rt_total.Size = New System.Drawing.Size(19, 13)
        Me.rt_total.TabIndex = 8
        Me.rt_total.Text = "00"
        '
        'rt_decals
        '
        Me.rt_decals.AutoSize = True
        Me.rt_decals.Location = New System.Drawing.Point(57, 113)
        Me.rt_decals.Name = "rt_decals"
        Me.rt_decals.Size = New System.Drawing.Size(19, 13)
        Me.rt_decals.TabIndex = 10
        Me.rt_decals.Text = "00"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(15, 113)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(40, 13)
        Me.Label7.TabIndex = 9
        Me.Label7.Text = "Decals"
        '
        'frmStats
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(98, 160)
        Me.Controls.Add(Me.rt_decals)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.rt_total)
        Me.Controls.Add(Me.rt_trees)
        Me.Controls.Add(Me.rt_models)
        Me.Controls.Add(Me.rt_terrian)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmStats"
        Me.Text = "Render Stats"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents rt_terrian As System.Windows.Forms.Label
    Friend WithEvents rt_models As System.Windows.Forms.Label
    Friend WithEvents rt_trees As System.Windows.Forms.Label
    Friend WithEvents rt_total As System.Windows.Forms.Label
    Friend WithEvents rt_decals As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
End Class
