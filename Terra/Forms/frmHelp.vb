Imports System
Imports System.IO
Imports System.Windows.Forms
Imports System.Net
Public Class frmHelp

  Private Sub frmHelp_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Dim p As String = Application.StartupPath + "\info.html"
		WebBrowser1.DocumentText = File.ReadAllText(p)
  End Sub

  Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles close_bt.Click
    Me.Close()
  End Sub
End Class