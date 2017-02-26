<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
    Me.stop_btn = New System.Windows.Forms.Button
    Me.start_btn = New System.Windows.Forms.Button
    Me.ComboBox1 = New System.Windows.Forms.ComboBox
    Me.user_list_tb = New System.Windows.Forms.TextBox
    Me.TextBox1 = New System.Windows.Forms.TextBox
    Me.TextBox2 = New System.Windows.Forms.TextBox
    Me.SuspendLayout()
    '
    'stop_btn
    '
    Me.stop_btn.Location = New System.Drawing.Point(278, 7)
    Me.stop_btn.Name = "stop_btn"
    Me.stop_btn.Size = New System.Drawing.Size(75, 23)
    Me.stop_btn.TabIndex = 1
    Me.stop_btn.Text = "stop"
    Me.stop_btn.UseVisualStyleBackColor = True
    '
    'start_btn
    '
    Me.start_btn.Location = New System.Drawing.Point(181, 7)
    Me.start_btn.Name = "start_btn"
    Me.start_btn.Size = New System.Drawing.Size(75, 23)
    Me.start_btn.TabIndex = 2
    Me.start_btn.Text = "Start"
    Me.start_btn.UseVisualStyleBackColor = True
    '
    'ComboBox1
    '
    Me.ComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple
    Me.ComboBox1.FormattingEnabled = True
    Me.ComboBox1.Location = New System.Drawing.Point(12, 7)
    Me.ComboBox1.Name = "ComboBox1"
    Me.ComboBox1.Size = New System.Drawing.Size(152, 112)
    Me.ComboBox1.TabIndex = 3
    '
    'user_list_tb
    '
    Me.user_list_tb.Location = New System.Drawing.Point(12, 117)
    Me.user_list_tb.Multiline = True
    Me.user_list_tb.Name = "user_list_tb"
    Me.user_list_tb.Size = New System.Drawing.Size(152, 202)
    Me.user_list_tb.TabIndex = 4
    '
    'TextBox1
    '
    Me.TextBox1.Location = New System.Drawing.Point(170, 117)
    Me.TextBox1.Multiline = True
    Me.TextBox1.Name = "TextBox1"
    Me.TextBox1.Size = New System.Drawing.Size(183, 202)
    Me.TextBox1.TabIndex = 5
    '
    'TextBox2
    '
    Me.TextBox2.Location = New System.Drawing.Point(181, 54)
    Me.TextBox2.Name = "TextBox2"
    Me.TextBox2.Size = New System.Drawing.Size(137, 20)
    Me.TextBox2.TabIndex = 6
    Me.TextBox2.Text = "24.93.203.105"
    '
    'frmMain
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(369, 331)
    Me.Controls.Add(Me.TextBox2)
    Me.Controls.Add(Me.TextBox1)
    Me.Controls.Add(Me.user_list_tb)
    Me.Controls.Add(Me.ComboBox1)
    Me.Controls.Add(Me.start_btn)
    Me.Controls.Add(Me.stop_btn)
    Me.DataBindings.Add(New System.Windows.Forms.Binding("Location", Global.Terra_server.My.MySettings.Default, "startup_location", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
    Me.Location = Global.Terra_server.My.MySettings.Default.startup_location
    Me.Name = "frmMain"
    Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
    Me.Text = "Terra Server"
    Me.TopMost = True
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
    Friend WithEvents stop_btn As System.Windows.Forms.Button
    Friend WithEvents start_btn As System.Windows.Forms.Button
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Friend WithEvents user_list_tb As System.Windows.Forms.TextBox
  Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
  Friend WithEvents TextBox2 As System.Windows.Forms.TextBox

End Class
