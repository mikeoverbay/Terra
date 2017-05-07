Imports System
Public Class FrmInfoWindow
    Dim drag As Boolean
    Dim mousex As Integer
    Dim mousey As Integer

    Private Sub FrmInfoWindow_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Me.Hide()
        Return
    End Sub

    Private Sub tb1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles tb1.MouseDoubleClick
        Me.visible = False
    End Sub

    Private Sub tb1_MouseDown(sender As Object, e As MouseEventArgs) Handles tb1.MouseDown
        drag = True
        mousex = Windows.Forms.Cursor.Position.X - Me.Left
        mousey = Windows.Forms.Cursor.Position.Y - Me.Top
    End Sub

    Private Sub tb1_MouseMove(sender As Object, e As MouseEventArgs) Handles tb1.MouseMove
        If drag Then
            Me.Top = Windows.Forms.Cursor.Position.Y - mousey
            Me.Left = Windows.Forms.Cursor.Position.X - mousex
        End If
    End Sub

    Private Sub tb1_MouseUp(sender As Object, e As MouseEventArgs) Handles tb1.MouseUp
        drag = False
    End Sub
End Class