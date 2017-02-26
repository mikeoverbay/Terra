Imports System.Math
Imports System.Windows.Forms
Imports System.String


Public Class FrmGrid_Listing

	Private Sub FrmGrid_Listing_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		frmOutput.Close()
	End Sub

	Private Sub FrmGrid_Listing_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		Dim size = Sqrt(maplist.Count - 1)
		Me.ClientSize = New Size(New System.Drawing.Point(size * 64, size * 64))
		For row = 1 To size
			For col = 0 To size - 1
				Dim bt As New Button
				bt.Width = 64 : bt.Height = 64
				bt.FlatStyle = FlatStyle.Flat
				Dim map = mapBoard(size - col - 1, size - row + 1)
				bt.BackgroundImageLayout = ImageLayout.Stretch
                bt.BackgroundImage = maplist(map).bmap
				bt.Tag = map
				bt.Font = frmMain.font_holder.Font.Clone
				bt.FlatAppearance.BorderColor = Color.Black
				bt.FlatAppearance.BorderSize = 1
				bt.ForeColor = Color.White
				AddHandler bt.MouseClick, AddressOf handle_map_grid_click
				bt.Text = map.ToString
				Me.Controls.Add(bt)
				bt.Location = New System.Drawing.Point((col) * 64, (row - 1) * 64)

			Next
		Next
	End Sub
	Public Sub handle_map_grid_click(sender As Object, e As EventArgs)
		frmOutput.Show()
        frmOutput.TextBox1.Text = ""
        Dim x1, x2, y1, y2 As Single
        Dim map = sender.tag

        x1 = maplist(map).location.x - 50
        x2 = maplist(map).location.x + 50
        y1 = maplist(map).location.y - 50
        y2 = maplist(map).location.y + 50

        For i = 0 To Model_Matrix_list.Count - 2
            Dim x, y As Single
            x = Model_Matrix_list(i).matrix(12)
            y = Model_Matrix_list(i).matrix(14)

            If x > x1 And x < x2 Then
                If y > y1 And y < y2 Then
                    frmOutput.TextBox1.Text += i.ToString + ": " + Model_Matrix_list(i).primitive_name + vbCrLf
                End If
            End If

        Next
        frmOutput.TextBox1.SelectionLength = 0
        'For Each thing In Models(sender.tag).model_list
        '    frmOutput.TextBox1.Text += thing + vbCrLf
        'Next
	End Sub
End Class