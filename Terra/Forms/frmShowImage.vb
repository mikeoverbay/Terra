Imports System.Runtime.InteropServices

Public Class frmShowImage
	Dim old_width, old_heigth As Integer
	Dim old_location As Point
    Public t_bmp As Bitmap
    Private value As Integer = -1
	<DllImport("user32.dll")> _
 Private Shared Function SendMessage( _
	ByVal hWnd As IntPtr, _
	ByVal wMsg As Int32, _
	ByVal wParam As IntPtr, _
	ByVal lParam As IntPtr) _
	As Int32
	End Function
	Private Const LB_SETTABSTOPS As Int32 = &H192
	Private Sub frmShowImage_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
			MessageBox.Show("Unable to make rendering context current")
			End
		End If
		Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
		frmMain.pb2.Parent = frmMain
		frmMain.pb2.Dock = DockStyle.None
		frmMain.pb2.Location = old_location
		frmMain.pb2.Width = old_width
		frmMain.pb2.Height = old_heigth
		frmMain.pb2.Visible = False

	End Sub

	Private Sub frmShowImage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		Dim headerHeight As Integer = TabControl1.Height - TabPage1.Height
		TabControl1.Width = 300
		'TabControl1.Height = 512 - headerHeight
		TabControl1.Dock = DockStyle.Right
		TextBox1.Height = 25
		TextBox1.Width = 512
		TextBox1.Location = New Point(0, 513)

		old_heigth = frmMain.pb2.Height
		old_width = frmMain.pb2.Width
		old_location = frmMain.pb2.Location
		frmMain.pb2.Parent = Me
		frmMain.pb2.Visible = True
		Me.ClientSize = New Size(512 + TabControl1.Width, 512 + TextBox1.Height)
		frmMain.pb2.Width = 512
		frmMain.pb2.Height = 512
        frmMain.pb2.Dock = DockStyle.Left
		draw_(0)
		populate_list()

	End Sub
	Private Sub populate_list()
		Dim m_layers As Boolean = frmMain.m_high_rez_Terrain.Checked

		ListBox1.Items.Clear()
		n_list1.Clear()
		n_list2.Clear()
		Dim ListBoxTabs() As Integer = {15, 80}
		Dim result As Integer
		Dim ptr As IntPtr
		Dim pinnedArray As GCHandle
		pinnedArray = GCHandle.Alloc(ListBoxTabs, GCHandleType.Pinned)
		ptr = pinnedArray.AddrOfPinnedObject()
		'Send LB_SETTABSTOPS message to ListBox.
		result = SendMessage(Me.ListBox1.Handle, LB_SETTABSTOPS, _
		  New IntPtr(ListBoxTabs.Length), ptr)
		pinnedArray.Free()
		Dim cnt As Integer = 0
        'get all the model texture ids
        For j = 0 To decal_matrix_list.Length - 1
            ListBox2.Items.Add(j.ToString("0000") + vbTab + decal_matrix_list(j).texture_id.ToString + vbTab + " Decal Diffuse")
            n_list2.Add(decal_matrix_list(j).decal_texture)

            ListBox2.Items.Add(j.ToString("0000") + vbTab + decal_matrix_list(j).normal_id.ToString + vbTab + " Decal Normal")
            n_list2.Add(decal_matrix_list(j).decal_normal)


        Next
        For j = 0 To Models.models.Length - 1
            For k = 0 To Models.models(j)._count - 1
                If Gl.glIsTexture(Models.models(j).componets(k).color_id) Then
                    ListBox2.Items.Add(j.ToString("0000") + vbTab + Models.models(j).componets(k).color_id.ToString + vbTab + " Model Texture")
                    n_list2.Add(Models.models(j).componets(k).color_name)
                    If uv2s_loaded Then
                        If Gl.glIsTexture(Models.models(j).componets(k).color2_Id) Then
                            ListBox2.Items.Add(j.ToString("0000") + vbTab + Models.models(j).componets(k).color2_Id.ToString + vbTab + " Model UV2 Texture")
                            n_list2.Add(Models.models(j).componets(k).color2_name)
                        End If
                    End If
                    cnt += 1
                End If
            Next
        Next
        For map = 0 To maplist.Length - 2
            ListBox1.Items.Add(maplist(map).colorMapId.ToString + vbTab + " : Map " + map.ToString + vbTab + " Terrain low Texture")
            n_list1.Add("lodTexture. This is in each chunk cdata file.")

            ListBox1.Items.Add(maplist(map).normMapID.ToString + vbTab + " : Map " + map.ToString + vbTab + " Terrain normal map")
            n_list1.Add("None")
            'If m_layers Then
            '	ListBox1.Items.Add(maplist(map).HZ_colorMapID.ToString + vbTab + " : Map " + map.ToString + vbTab + " Terrain high Texture")
            '	n_list.Add("None")
            '	If frmMain.m_BumpMap.Checked Then
            '		ListBox1.Items.Add(maplist(map).HZ_normMapID.ToString + vbTab + " : Map " + map.ToString + vbTab + " Terrain bump Texture")
            '		n_list.Add("None")
            '	End If

            'End If
            'ListBox1.Items.Add(map_layers(map).shadowMapID.ToString + vbTab + " : Map " + map.ToString + vbTab + " HorizonShadow Map")
            'n_list1.Add("HorizonShadow Map. Generated from data in the cdata/terrain2/.")
            For l = 1 To map_layers(map).layer_count
                If Gl.glIsTexture(map_layers(map).layers(l).text_id) Then
                    ListBox1.Items.Add(map_layers(map).layers(l).text_id.ToString + vbTab + " : Map/Layer " + map.ToString + "/" + l.ToString + vbTab + " layering Texture")
                    n_list1.Add(map_layers(map).layers(l).l_name)
                End If
                If Gl.glIsTexture(map_layers(map).layers(l).norm_id) Then
                    ListBox1.Items.Add(map_layers(map).layers(l).norm_id.ToString + vbTab + " : Map/Layer " + map.ToString + "/" + l.ToString + vbTab + " layering Texture")
                    n_list1.Add(map_layers(map).layers(l).n_name)
                End If
                cnt += 1
            Next

            cnt += 1
        Next
        If m_layers Then
            'For map = 0 To maplist.Length - 2
            '	ListBox1.Items.Add(map_layers(map).mix_text_Id.ToString + vbTab + " : Map " + map.ToString + " " + vbTab + " >> Mix Texture")
            '	n_list.Add("Created in App")
            '	cnt += 1

            '	For l = 1 To map_layers(map).layer_count
            '		ListBox1.Items.Add(map_layers(map).layers(l).text_id.ToString + vbTab + " : Map/Layer " + map.ToString + "/" + l.ToString + vbTab + " layering Texture")
            '		n_list.Add(map_layers(map).layers(l).l_name)
            '		cnt += 1
            '	Next
            'Next
        End If
        'For map = 0 To maplist.Length - 2
        '	For i As UInt32 = 0 To maplist(map).flora_count
        '		ListBox1.Items.Add(speedtree_imageID.ToString + vbTab + " : Map/Tree " + map.ToString + "/" + i.ToString + vbTab + " Tree Billboard")
        '		If maplist(map).flora IsNot Nothing Then

        '			If maplist(map).flora(i).branch_textureID > 0 Then
        '				ListBox1.Items.Add(maplist(map).flora(i).branch_textureID.ToString + vbTab + " : Map/Tree " + map.ToString + "/" + i.ToString + vbTab + " Tree Branch")
        '			End If
        '			If maplist(map).flora(i).frond_displayID > 0 Then
        '				ListBox1.Items.Add(speedtree_imageID.ToString + vbTab + " : Map/Tree " + map.ToString + "/" + i.ToString + vbTab + " Tree Branch")
        '			End If
        '			If maplist(map).flora(i).leaf_displayID > 0 Then
        '				ListBox1.Items.Add(speedtree_imageID.ToString + vbTab + " : Map/Tree " + map.ToString + "/" + i.ToString + vbTab + " Tree Branch")
        '			End If
        '		End If

        '	Next
        'Next
        TextBox1.Text = cnt.ToString + " Textures in list."
	End Sub

	Private Sub draw_(ByVal id As Integer)
		If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
			MessageBox.Show("Unable to make rendering context current")
			End
		End If
		Dim uc As vect2
		Dim lc As vect2
		Dim psize As Integer = 512
		lc.x = frmMain.pb2.Width
		lc.y = -frmMain.pb2.Height	' top to bottom is negitive
		uc.x = 0.0
		uc.y = 0.0
		Gl.glClearColor(0.8F, 0.8F, 0.8F, 1.0F)
		Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
		ResizeGL_2()
		ViewPerspective_2()
		ViewOrtho_2()
		Gl.glPushMatrix()
		Gl.glTranslatef(0.0, 0.0F, -0.1F)
        Gl.glDisable(Gl.GL_BLEND)
		Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
		Gl.glDisable(Gl.GL_LIGHTING)

		Gl.glActiveTexture(Gl.GL_TEXTURE0)
		Gl.glEnable(Gl.GL_TEXTURE_2D)
		Gl.glBindTexture(Gl.GL_TEXTURE_2D, id)
		Dim e = Gl.glGetError
		Gl.glColor4f(1.0, 1.0, 1.0, 1.0)


		'Gl.glActiveTexture(Gl.GL_TEXTURE0)
		'Gl.glUseProgram(0)


		Gl.glBegin(Gl.GL_TRIANGLES)
		'---
        Gl.glTexCoord2f(0.0, 1.0)
		Gl.glVertex3f(uc.x, lc.y, -1.0!)

        Gl.glTexCoord2f(0.0, 0.0)
		Gl.glVertex3f(uc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(1.0, 1.0)
		Gl.glVertex3f(lc.x, lc.y, -1.0!)
		'---
        Gl.glTexCoord2f(0.0, 0.0)
		Gl.glVertex3f(uc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(1.0, 0.0)
		Gl.glVertex3f(lc.x, uc.y, -1.0!)

        Gl.glTexCoord2f(1.0, 1.0)
		Gl.glVertex3f(lc.x, lc.y, -1.0!)

		Gl.glEnd()
		Gl.glFinish()
		Gdi.SwapBuffers(pb2_hDC)
		'We have to draw this again so its in the other buffer so we can read the pixel data.
		Gl.glBegin(Gl.GL_TRIANGLES)
		'---
		Gl.glTexCoord2f(0.0, 0.0)
		Gl.glVertex3f(uc.x, lc.y, -1.0!)

		Gl.glTexCoord2f(0.0, 1.0)
		Gl.glVertex3f(uc.x, uc.y, -1.0!)

		Gl.glTexCoord2f(1.0, 0.0)
		Gl.glVertex3f(lc.x, lc.y, -1.0!)
		'---
		Gl.glTexCoord2f(0.0, 1.0)
		Gl.glVertex3f(uc.x, uc.y, -1.0!)

		Gl.glTexCoord2f(1.0, 1.0)
		Gl.glVertex3f(lc.x, uc.y, -1.0!)

		Gl.glTexCoord2f(1.0, 0.0)
		Gl.glVertex3f(lc.x, lc.y, -1.0!)

		Gl.glEnd()
		Gl.glDisable(Gl.GL_TEXTURE_2D)
		Gl.glDisable(Gl.GL_BLEND)
		e = Gl.glGetError
		Gl.glPopMatrix()
		Gl.glFinish()
		Gl.glFlush()
		'Gdi.SwapBuffers(pb2_hDC)
		t_bmp = New Bitmap(psize, psize, PixelFormat.Format32bppArgb)
		Dim rect As New System.Drawing.Rectangle(0, 0, psize, psize)
		Dim data As BitmapData = t_bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
		Gl.glReadPixels(0, 0, psize, psize, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, data.Scan0)
		t_bmp.UnlockBits(data)
		If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
			MessageBox.Show("Unable to make rendering context current")
			End
		End If

		t_bmp.RotateFlip(RotateFlipType.RotateNoneFlipY)

	End Sub

	Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
		Dim select_text = ListBox1.SelectedItem.ToString()
		Dim ar = select_text.Split(vbTab)
        value = ar(0)
		draw_(value)
		Dim index = ListBox1.SelectedIndex
		TextBox1.Text = n_list1.Item(index)
	End Sub

	Private Sub ListBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox2.SelectedIndexChanged
		Dim select_text = ListBox2.SelectedItem.ToString()
		Dim ar = select_text.Split(vbTab)
        value = ar(1)
		draw_(value)
		Dim index = ListBox2.SelectedIndex
		TextBox1.Text = n_list2.Item(index)

	End Sub

    Private Sub frmShowImage_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        'If value < 0 Then Return
        draw_(value)
    End Sub

    Private Sub frmShowImage_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        If value < 0 Then Return
        ' draw_(value)
    End Sub
End Class