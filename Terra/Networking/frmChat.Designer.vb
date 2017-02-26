<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmChat
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
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmChat))
    Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
    Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
    Me.chat_input_tb = New System.Windows.Forms.TextBox
    Me.chat_box_tb = New System.Windows.Forms.TextBox
    Me.clients_dgv = New System.Windows.Forms.DataGridView
    Me.hide_chat_bt = New System.Windows.Forms.Button
    Me.audio_cb = New System.Windows.Forms.CheckBox
    CType(Me.clients_dgv, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'chat_input_tb
    '
    Me.chat_input_tb.BackColor = System.Drawing.Color.DimGray
    Me.chat_input_tb.BorderStyle = System.Windows.Forms.BorderStyle.None
    Me.chat_input_tb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.chat_input_tb.ForeColor = System.Drawing.Color.White
    Me.chat_input_tb.Location = New System.Drawing.Point(6, 381)
    Me.chat_input_tb.Margin = New System.Windows.Forms.Padding(0)
    Me.chat_input_tb.Multiline = True
    Me.chat_input_tb.Name = "chat_input_tb"
    Me.chat_input_tb.Size = New System.Drawing.Size(212, 33)
    Me.chat_input_tb.TabIndex = 32
    Me.chat_input_tb.Text = "Testing 2 lines" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Testing 2 lines" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
    '
    'chat_box_tb
    '
    Me.chat_box_tb.BackColor = System.Drawing.Color.DimGray
    Me.chat_box_tb.BorderStyle = System.Windows.Forms.BorderStyle.None
    Me.chat_box_tb.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.chat_box_tb.ForeColor = System.Drawing.Color.White
    Me.chat_box_tb.Location = New System.Drawing.Point(6, 0)
    Me.chat_box_tb.Margin = New System.Windows.Forms.Padding(0)
    Me.chat_box_tb.Multiline = True
    Me.chat_box_tb.Name = "chat_box_tb"
    Me.chat_box_tb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
    Me.chat_box_tb.Size = New System.Drawing.Size(214, 354)
    Me.chat_box_tb.TabIndex = 33
    Me.chat_box_tb.Text = resources.GetString("chat_box_tb.Text")
    '
    'clients_dgv
    '
    Me.clients_dgv.AllowUserToAddRows = False
    Me.clients_dgv.AllowUserToDeleteRows = False
    Me.clients_dgv.AllowUserToResizeColumns = False
    Me.clients_dgv.AllowUserToResizeRows = False
    Me.clients_dgv.BackgroundColor = System.Drawing.Color.DimGray
    Me.clients_dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None
    DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
    DataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
    DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    DataGridViewCellStyle3.ForeColor = System.Drawing.Color.White
    DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
    DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Yellow
    DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
    Me.clients_dgv.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle3
    Me.clients_dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
    DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
    DataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
    DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText
    DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
    DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Yellow
    DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
    Me.clients_dgv.DefaultCellStyle = DataGridViewCellStyle4
    Me.clients_dgv.Dock = System.Windows.Forms.DockStyle.Right
    Me.clients_dgv.GridColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
    Me.clients_dgv.Location = New System.Drawing.Point(221, 0)
    Me.clients_dgv.Name = "clients_dgv"
    Me.clients_dgv.ReadOnly = True
    Me.clients_dgv.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
    Me.clients_dgv.RowHeadersVisible = False
    Me.clients_dgv.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
    Me.clients_dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
    Me.clients_dgv.Size = New System.Drawing.Size(130, 414)
    Me.clients_dgv.TabIndex = 18
    '
    'hide_chat_bt
    '
    Me.hide_chat_bt.BackColor = System.Drawing.SystemColors.ButtonFace
    Me.hide_chat_bt.FlatAppearance.BorderColor = System.Drawing.Color.DimGray
    Me.hide_chat_bt.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver
    Me.hide_chat_bt.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
    Me.hide_chat_bt.FlatStyle = System.Windows.Forms.FlatStyle.Popup
    Me.hide_chat_bt.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.hide_chat_bt.ForeColor = System.Drawing.Color.Black
    Me.hide_chat_bt.Location = New System.Drawing.Point(111, 355)
    Me.hide_chat_bt.Name = "hide_chat_bt"
    Me.hide_chat_bt.Size = New System.Drawing.Size(55, 23)
    Me.hide_chat_bt.TabIndex = 34
    Me.hide_chat_bt.Text = "Hide"
    Me.hide_chat_bt.UseVisualStyleBackColor = False
    '
    'audio_cb
    '
    Me.audio_cb.Appearance = System.Windows.Forms.Appearance.Button
    Me.audio_cb.AutoSize = True
    Me.audio_cb.BackColor = System.Drawing.SystemColors.ButtonFace
    Me.audio_cb.FlatStyle = System.Windows.Forms.FlatStyle.Popup
    Me.audio_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.audio_cb.Location = New System.Drawing.Point(52, 355)
    Me.audio_cb.Name = "audio_cb"
    Me.audio_cb.Size = New System.Drawing.Size(53, 23)
    Me.audio_cb.TabIndex = 35
    Me.audio_cb.Text = "Sound"
    Me.audio_cb.UseVisualStyleBackColor = False
    '
    'frmChat
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.BackColor = System.Drawing.Color.DimGray
    Me.ClientSize = New System.Drawing.Size(351, 414)
    Me.ControlBox = False
    Me.Controls.Add(Me.audio_cb)
    Me.Controls.Add(Me.hide_chat_bt)
    Me.Controls.Add(Me.clients_dgv)
    Me.Controls.Add(Me.chat_box_tb)
    Me.Controls.Add(Me.chat_input_tb)
    Me.DataBindings.Add(New System.Windows.Forms.Binding("Location", Global.Terra.My.MySettings.Default, "chat_local", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
    Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
    Me.Location = Global.Terra.My.MySettings.Default.chat_local
    Me.Name = "frmChat"
    Me.ShowIcon = False
    Me.ShowInTaskbar = False
    Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
    Me.Text = "Terra! Chat"
    Me.TopMost = True
    CType(Me.clients_dgv, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents chat_input_tb As System.Windows.Forms.TextBox
  Friend WithEvents chat_box_tb As System.Windows.Forms.TextBox
  Friend WithEvents clients_dgv As System.Windows.Forms.DataGridView
  Friend WithEvents hide_chat_bt As System.Windows.Forms.Button
  Friend WithEvents audio_cb As System.Windows.Forms.CheckBox
End Class
