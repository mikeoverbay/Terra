<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmInfoWindow
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
        Me.tb1 = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'tb1
        '
        Me.tb1.BackColor = System.Drawing.Color.Black
        Me.tb1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.tb1.Cursor = System.Windows.Forms.Cursors.SizeAll
        Me.tb1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tb1.ForeColor = System.Drawing.Color.Green
        Me.tb1.Location = New System.Drawing.Point(0, 0)
        Me.tb1.Multiline = True
        Me.tb1.Name = "tb1"
        Me.tb1.Size = New System.Drawing.Size(694, 80)
        Me.tb1.TabIndex = 0
        Me.tb1.WordWrap = False
        '
        'FrmInfoWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(694, 80)
        Me.Controls.Add(Me.tb1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FrmInfoWindow"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "FrmInfoWindow"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tb1 As System.Windows.Forms.TextBox
End Class
