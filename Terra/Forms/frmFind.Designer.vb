<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFind
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFind))
        Me.find_btn = New System.Windows.Forms.Button()
        Me.cancel_btn = New System.Windows.Forms.Button()
        Me.search_tb = New System.Windows.Forms.TextBox()
        Me.results_tb = New System.Windows.Forms.TextBox()
        Me.grid_list_btn = New System.Windows.Forms.Button()
        Me.show_img_btn = New System.Windows.Forms.Button()
        Me.showlayeruvs_bt = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'find_btn
        '
        Me.find_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.find_btn.Location = New System.Drawing.Point(125, 9)
        Me.find_btn.Name = "find_btn"
        Me.find_btn.Size = New System.Drawing.Size(75, 23)
        Me.find_btn.TabIndex = 0
        Me.find_btn.Text = "Search"
        Me.find_btn.UseVisualStyleBackColor = True
        '
        'cancel_btn
        '
        Me.cancel_btn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cancel_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.cancel_btn.Location = New System.Drawing.Point(497, 253)
        Me.cancel_btn.Name = "cancel_btn"
        Me.cancel_btn.Size = New System.Drawing.Size(75, 23)
        Me.cancel_btn.TabIndex = 1
        Me.cancel_btn.Text = "Cancel"
        Me.cancel_btn.UseVisualStyleBackColor = True
        '
        'search_tb
        '
        Me.search_tb.AcceptsReturn = True
        Me.search_tb.BackColor = System.Drawing.Color.Black
        Me.search_tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.search_tb.ForeColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.search_tb.Location = New System.Drawing.Point(4, 10)
        Me.search_tb.Name = "search_tb"
        Me.search_tb.Size = New System.Drawing.Size(115, 20)
        Me.search_tb.TabIndex = 2
        '
        'results_tb
        '
        Me.results_tb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.results_tb.BackColor = System.Drawing.Color.Black
        Me.results_tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.results_tb.ForeColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.results_tb.Location = New System.Drawing.Point(4, 38)
        Me.results_tb.Multiline = True
        Me.results_tb.Name = "results_tb"
        Me.results_tb.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.results_tb.Size = New System.Drawing.Size(568, 209)
        Me.results_tb.TabIndex = 3
        '
        'grid_list_btn
        '
        Me.grid_list_btn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.grid_list_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.grid_list_btn.Location = New System.Drawing.Point(4, 253)
        Me.grid_list_btn.Name = "grid_list_btn"
        Me.grid_list_btn.Size = New System.Drawing.Size(75, 23)
        Me.grid_list_btn.TabIndex = 6
        Me.grid_list_btn.Text = "Grid Listing"
        Me.grid_list_btn.UseVisualStyleBackColor = True
        '
        'show_img_btn
        '
        Me.show_img_btn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.show_img_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.show_img_btn.Location = New System.Drawing.Point(85, 253)
        Me.show_img_btn.Name = "show_img_btn"
        Me.show_img_btn.Size = New System.Drawing.Size(84, 23)
        Me.show_img_btn.TabIndex = 7
        Me.show_img_btn.Text = "Show Images"
        Me.show_img_btn.UseVisualStyleBackColor = True
        '
        'showlayeruvs_bt
        '
        Me.showlayeruvs_bt.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.showlayeruvs_bt.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.showlayeruvs_bt.Location = New System.Drawing.Point(175, 253)
        Me.showlayeruvs_bt.Name = "showlayeruvs_bt"
        Me.showlayeruvs_bt.Size = New System.Drawing.Size(100, 23)
        Me.showlayeruvs_bt.TabIndex = 8
        Me.showlayeruvs_bt.Text = "Show Layer UVs"
        Me.showlayeruvs_bt.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Label3.Location = New System.Drawing.Point(217, 6)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(234, 26)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Search by partial name.." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "You can 'click' on an item to move to its location"
        '
        'frmFind
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(584, 284)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.showlayeruvs_bt)
        Me.Controls.Add(Me.show_img_btn)
        Me.Controls.Add(Me.grid_list_btn)
        Me.Controls.Add(Me.results_tb)
        Me.Controls.Add(Me.search_tb)
        Me.Controls.Add(Me.cancel_btn)
        Me.Controls.Add(Me.find_btn)
        Me.ForeColor = System.Drawing.Color.Silver
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimumSize = New System.Drawing.Size(600, 270)
        Me.Name = "frmFind"
        Me.Text = "Development Tools"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents find_btn As System.Windows.Forms.Button
    Friend WithEvents cancel_btn As System.Windows.Forms.Button
    Friend WithEvents search_tb As System.Windows.Forms.TextBox
    Friend WithEvents results_tb As System.Windows.Forms.TextBox
    Friend WithEvents grid_list_btn As System.Windows.Forms.Button
    Friend WithEvents show_img_btn As System.Windows.Forms.Button
    Friend WithEvents showlayeruvs_bt As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents m_exclude_cb As System.Windows.Forms.CheckBox
End Class
