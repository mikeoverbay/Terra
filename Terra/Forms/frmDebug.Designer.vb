<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDebug
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
		Me.tb = New System.Windows.Forms.TextBox()
		Me.SuspendLayout()
		'
		'tb
		'
		Me.tb.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
				Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.tb.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
		Me.tb.ForeColor = System.Drawing.Color.Lime
		Me.tb.Location = New System.Drawing.Point(0, 245)
		Me.tb.Multiline = True
		Me.tb.Name = "tb"
		Me.tb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.tb.Size = New System.Drawing.Size(626, 17)
		Me.tb.TabIndex = 0
		'
		'frmDebug
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
		Me.ClientSize = New System.Drawing.Size(623, 262)
		Me.Controls.Add(Me.tb)
		Me.ForeColor = System.Drawing.Color.Lime
		Me.Name = "frmDebug"
		Me.Text = "Debug Info"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents tb As System.Windows.Forms.TextBox
End Class
