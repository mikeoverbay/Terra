﻿Imports System.Runtime.InteropServices
Imports System.Math
Public Class frmShowImage
    Dim old_width, old_heigth As Integer
    Dim old_w, old_h As Double
	Dim old_location As Point
    Public ready_to_render As Boolean = False
    Public current_image As Integer = 0
    Private me_w, me_h As Integer
    Public rect_location As New Point(0, 0)
    Public rect_size As New Point(512, 512)
    Public mouse_delta As New Point(0, 0)
    Public mouse_pos As New Point(0, 0)
    Public Zoom_Factor As Single = 1.0
    Public noise_id As Integer
    'Public noise_id As Integer
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
            Return
        End If
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        GC.Collect()
        frmMain.pb2.Visible = False
        frmMain.pb2.Parent = frmMain
        frmMain.pb2.Dock = DockStyle.None
        frmMain.pb2.Anchor = AnchorStyles.None
        frmMain.pb2.Location = old_location
        frmMain.pb2.Width = old_width
        frmMain.pb2.Height = old_heigth
        ready_to_render = False ' used by other forms as flag
    End Sub

    Private Sub frmShowImage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim headerHeight As Integer = TabControl1.Height - TabPage1.Height
        'TabControl1.Height = 512 - headerHeight
        Me.Show()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Me.ClientSize = New Size(512 + TabControl1.Width + 4, 512 + TextBox1.Height)
        SPC.SplitterDistance = 512
        'SPC.Panel2.Width = 300
        SPC.IsSplitterFixed = True

        old_heigth = frmMain.pb2.Height
        old_width = frmMain.pb2.Width
        old_location = frmMain.pb2.Location
        frmMain.pb2.Parent = SPC.Panel1
        frmMain.pb2.Visible = True
        frmMain.pb2.Width = SPC.Panel1.Width
        frmMain.pb2.Height = SPC.Panel1.Height - 25
        frmMain.pb2.Location = New Point(0, 0)
        frmMain.pb2.Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top Or AnchorStyles.Bottom
        'noise_id = Load_DDS_File(Application.StartupPath + "\Resources\Noise.dds")
        current_image = noise_id
        ready_to_render = True
        draw_texture(current_image)
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
    Public Sub draw_texture(ByVal id As Integer)
        current_image = id
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, id)
        Dim w, h As Single
        Gl.glGetTexLevelParameterfv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_WIDTH, w)
        Gl.glGetTexLevelParameterfv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_HEIGHT, h)
        If w = 0 Or h = 0 Then Return
        old_w = w : old_h = h ' save for image scaling
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        rect_size = New Point(CInt(w), CInt(h)) 'set quad size
        rect_location = New Point(0, 0) 'reset location
        Zoom_Factor = 1.0 'reset scale
        draw_(id)
    End Sub


    Public Sub draw_(ByVal id As Integer)
        If Not ready_to_render Then Return
        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            Return
        End If
        ResizeGL_2(SPC.Panel1.Width, SPC.Panel1.Height - 25)
        'ViewPerspective_2()
        ViewOrtho_2()
        Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, id)
        Dim e = Gl.glGetError
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)

        If alpha_cb.Checked Then
            Gl.glClearColor(0.9F, 0.9F, 0.9F, 1.0F)
            Gl.glEnable(Gl.GL_BLEND)
        Else
            Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
            Gl.glDisable(Gl.GL_BLEND)
        End If

        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        Dim u1() As Single = {1.0, 0.0}
        Dim u2() As Single = {0.0, 0.0}
        Dim u3() As Single = {0.0, 1.0}
        Dim u4() As Single = {1.0, 1.0}

        Dim p1, p2, p3, p4 As Point
        Dim L, S As New Point
        L = rect_location
        S = rect_size
        L.Y *= -1
        S.Y *= -1

        p1 = L
        p2 = L
        p2.X += rect_size.X
        p3 = L + S
        p4 = L
        p4.Y += S.Y
        Gl.glPushMatrix()
        'Dim xo = rect_location.X - mouse_pos.X
        'Dim yo = rect_location.Y - mouse_pos.Y
        'Gl.glTranslatef(mouse_pos.X, -mouse_pos.Y, 0.0)
        'Gl.glScalef(Zoom_Factor, Zoom_Factor, 1.0)
        'Gl.glTranslatef(-mouse_pos.X, mouse_pos.Y, 0.0)

        Gl.glBegin(Gl.GL_QUADS)
        '---
        Gl.glTexCoord2fv(u1)
        Gl.glVertex2f(p1.X, p1.Y)
        Gl.glTexCoord2fv(u2)
        Gl.glVertex2f(p2.X, p2.Y)
        Gl.glTexCoord2fv(u3)
        Gl.glVertex2f(p3.X, p3.Y)
        Gl.glTexCoord2fv(u4)
        Gl.glVertex2f(p4.X, p4.Y)
        '--
        Gl.glEnd()

        Gl.glFinish()
        Gdi.SwapBuffers(pb2_hDC)

        'We have to draw this again so its in the other buffer so we can read the pixel data.
        Gl.glBegin(Gl.GL_QUADS)
        '---
        Gl.glTexCoord2fv(u1)
        Gl.glVertex2f(p1.X, p1.Y)
        Gl.glTexCoord2fv(u2)
        Gl.glVertex2f(p2.X, p2.Y)
        Gl.glTexCoord2fv(u3)
        Gl.glVertex2f(p3.X, p3.Y)
        Gl.glTexCoord2fv(u4)
        Gl.glVertex2f(p4.X, p4.Y)
        Gl.glEnd()
        Gl.glPopMatrix()
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glDisable(Gl.GL_BLEND)
        e = Gl.glGetError

        Gl.glFinish()
        Gl.glFlush()
        'Gdi.SwapBuffers(pb2_hDC)
        Dim pb2s As New Point(frmMain.pb2.Width, frmMain.pb2.Height)
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            'End
        End If


    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If Not ready_to_render Then Return
        Dim select_text = ListBox1.SelectedItem.ToString()
        Dim ar = select_text.Split(vbTab)
        current_image = ar(0)
        draw_texture(current_image)
        Dim index = ListBox1.SelectedIndex
        TextBox1.Text = n_list1.Item(index)
    End Sub

    Private Sub ListBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox2.SelectedIndexChanged
        If Not ready_to_render Then Return
        Dim select_text = ListBox2.SelectedItem.ToString()
        Dim ar = select_text.Split(vbTab)
        current_image = ar(1)
        draw_texture(current_image)
        Dim index = ListBox2.SelectedIndex
        TextBox1.Text = n_list2.Item(index)

    End Sub

    Private Sub frmShowImage_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Not ready_to_render Then Return

        'draw_(current_image)
    End Sub

    Private Sub frmShowImage_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        If Not ready_to_render Then Return
        draw_(current_image)
    End Sub
    Public Sub img_scale_up()
        If Not ready_to_render Then Return
        If Zoom_Factor >= 4.0 Then
            Zoom_Factor = 4.0
            Return 'to big and the t_bmp creation will hammer memory.
        End If
        Dim amt As Single = 0.0625
        Zoom_Factor += amt

        'this bit of math zooms the texture around the mouses center during the resize.
        'old_w and old_h is the original size of the image in width and height
        'mouse_pos is current mouse position in the window.

        Dim offset As New Point
        Dim old_size_w, old_size_h As Double
        old_size_w = (old_w * (Zoom_Factor - amt))
        old_size_h = (old_h * (Zoom_Factor - amt))

        offset = rect_location - (mouse_pos)

        rect_size.X = Zoom_Factor * old_w
        rect_size.Y = Zoom_Factor * old_h

        Dim delta_x As Double = CDbl(offset.X / old_size_w)
        Dim delta_y As Double = CDbl(offset.Y / old_size_h)

        Dim x_offset = delta_x * (rect_size.X - old_size_w)
        Dim y_offset = delta_y * (rect_size.Y - old_size_h)

        rect_location.X += CInt(x_offset)
        rect_location.Y += CInt(y_offset)

        draw_(current_image)
    End Sub

    Public Sub img_scale_down()
        If Not ready_to_render Then Return
        If Zoom_Factor <= 0.25 Then
            Zoom_Factor = 0.25
            Return
        End If
        Dim amt As Single = 0.0625
        Zoom_Factor -= amt

        'this bit of math zooms the texture around the mouses center during the resize.
        'old_w and old_h is the original size of the image in width and height
        'mouse_pos is current mouse position in the window.

        Dim offset As New Point
        Dim old_size_w, old_size_h As Double

        old_size_w = (old_w * (Zoom_Factor - amt))
        old_size_h = (old_h * (Zoom_Factor - amt))

        offset = rect_location - (mouse_pos)

        rect_size.X = Zoom_Factor * old_w
        rect_size.Y = Zoom_Factor * old_h

        Dim delta_x As Double = CDbl(offset.X / (rect_size.X + (rect_size.X - old_size_w)))
        Dim delta_y As Double = CDbl(offset.Y / (rect_size.Y + (rect_size.Y - old_size_h)))

        Dim x_offset = delta_x * (rect_size.X - old_size_w)
        Dim y_offset = delta_y * (rect_size.Y - old_size_h)

        rect_location.X += -CInt(x_offset)
        rect_location.Y += -CInt(y_offset)

        draw_(current_image)
    End Sub

    Public Sub btn_scale_up_Click(sender As Object, e As EventArgs) Handles btn_scale_up.Click
        If Not ready_to_render Then Return
        If Zoom_Factor >= 4 Then Return 'to big and the t_bmp creation will hammer memory and render time. 2048 max at 4x setting
        Dim xc, yc As Single
        Dim factor As Single = 1.0625
        Zoom_Factor *= factor
        'this bit of math zooms the texture around the mouses center during the resize.
        If rect_location.X >= 0 Then
            xc = (frmMain.pb2.Width / 2) - (rect_location.X)
        Else
            xc = (frmMain.pb2.Width / 2) + (-rect_location.X)
        End If
        If rect_location.Y >= 0.0 Then
            yc = (frmMain.pb2.Height / 2) - (rect_location.Y)
        Else
            yc = (frmMain.pb2.Height / 2) + (-rect_location.Y)
        End If

        rect_size.X *= factor
        rect_size.Y *= factor
        TextBox1.Text = String.Format("Zoom:" + "{0}%", Round((Zoom_Factor / 1) * 100))
        xc = (rect_size.X / 2) - (xc * factor)    '* Zoom_Factor
        yc = (rect_size.Y / 2) - (yc * factor) '* -Zoom_Factor
        rect_location = New Point(((frmMain.pb2.Width - rect_size.X) / 2) + xc, ((frmMain.pb2.Height - rect_size.Y) / 2) + yc)
        draw_(current_image)
    End Sub

    Public Sub btn_scale_down_Click(sender As Object, e As EventArgs) Handles btn_scale_down.Click
        If Not ready_to_render Then Return
        If Zoom_Factor <= 0.25 Then Return ' no point in going to small
        Dim xc, yc As Single
        Dim factor As Single = 0.9375
        Zoom_Factor *= factor
        'this bit of math zooms the texture around the mouses center during the resize.
        If rect_location.X >= 0 Then
            xc = (frmMain.pb2.Width / 2) - (rect_location.X)
        Else
            xc = (frmMain.pb2.Width / 2) + (-rect_location.X)
        End If
        If rect_location.Y >= 0.0 Then
            yc = (frmMain.pb2.Height / 2) - (rect_location.Y)
        Else
            yc = (frmMain.pb2.Height / 2) + (-rect_location.Y)
        End If
        rect_size.X *= factor
        rect_size.Y *= factor
        TextBox1.Text = String.Format("Zoom:" + "{0}%", Round((Zoom_Factor / 1) * 100))
        xc = (rect_size.X / 2) - (xc * factor)  '* Zoom_Factor
        yc = (rect_size.Y / 2) - (yc * factor) '* -Zoom_Factor
        rect_location = New Point(((frmMain.pb2.Width - rect_size.X) / 2) + xc, ((frmMain.pb2.Height - rect_size.Y) / 2) + yc)
        draw_(current_image)
    End Sub

    Private Sub alpha_cb_CheckedChanged(sender As Object, e As EventArgs) Handles alpha_cb.CheckedChanged
        If Not ready_to_render Then Return
        draw_(current_image)
    End Sub
End Class