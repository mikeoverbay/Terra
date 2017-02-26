Public Class frmLighting

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        save_light_settings()
        Me.Visible = False
    End Sub

    Private Sub frmLighting_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        save_light_settings()
        e.Cancel = True
        Me.Visible = False
    End Sub

    Private Sub frmLighting_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Show()
        While Me.Visible = False
            Application.DoEvents()
        End While
        If Not _STARTED Then Return
        If Not get_light_settings() Then
            Try

                s_fog_level.Value = My.Settings.s_fog_level
                s_terrain_ambient.Value = My.Settings.s_terrain_ambient_level
                s_terrain_texture_level.Value = My.Settings.s_terrian_texture_level
                s_model_level.Value = My.Settings.s_model_level
                s_gamma.Value = My.Settings.s_gamma
                s_gray_level.Value = My.Settings.s_gray_level
            Catch ex As Exception

            End Try
        End If

        frmMain.draw_scene()
    End Sub

	Private Sub s_terrain_texture_level_Scroll(sender As Object, e As EventArgs) Handles s_terrain_texture_level.Scroll
        If Not _STARTED Then Return
        lighting_terrain_texture = s_terrain_texture_level.Value / 50.0!
		My.Settings.s_terrian_texture_level = s_terrain_texture_level.Value
        frmMain.draw_scene()
	End Sub

	Private Sub s_terrain_ambient_Scroll(sender As Object, e As EventArgs) Handles s_terrain_ambient.Scroll
        If Not _STARTED Then Return
        lighting_ambient = s_terrain_ambient.Value / 300.0!
		My.Settings.s_terrain_ambient_level = s_terrain_ambient.Value
        frmMain.draw_scene()
	End Sub

	Private Sub s_fog_level_Scroll(sender As Object, e As EventArgs) Handles s_fog_level.Scroll
        If Not _STARTED Then Return
        lighting_fog_level = s_fog_level.Value / 10000.0! ' yes 10,000
		My.Settings.s_fog_level = s_fog_level.Value
        frmMain.draw_scene()
	End Sub

	Private Sub s_model_level_Scroll(sender As Object, e As EventArgs) Handles s_model_level.Scroll
        If Not _STARTED Then Return
        lighting_model_level = s_model_level.Value / 100.0!
		My.Settings.s_model_level = s_model_level.Value
        frmMain.draw_scene()
	End Sub

	Private Sub s_gamma_Scroll(sender As Object, e As EventArgs) Handles s_gamma.Scroll
        If Not _STARTED Then Return
        My.Settings.s_gamma = s_gamma.Value
        gamma_level = (s_gamma.Value / 100) * 1.0!
        frmMain.draw_scene()
	End Sub

    Private Sub s_gray_level_Scroll(sender As Object, e As EventArgs) Handles s_gray_level.Scroll
        If Not _STARTED Then Return
        My.Settings.s_gray_level = s_gray_level.Value
        gray_level = 1.0 - (s_gray_level.Value / 100)
        frmMain.draw_scene()
    End Sub
End Class