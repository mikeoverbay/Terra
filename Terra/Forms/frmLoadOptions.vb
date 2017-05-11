Public Class frmLoadOptions

    Private Sub m_terrain_CheckedChanged(sender As Object, e As EventArgs) Handles m_terrain.CheckedChanged
        m_terrain_ = m_terrain.Checked
        If Not terrain_loaded And maploaded And m_terrain_ Then
            MsgBox("You can change this setting but it was never loaded." + vbCrLf + "I can't draw it. It does not exist.", MsgBoxStyle.Exclamation, "Warning!")
        End If
        If maploaded Then
            frmMain.draw_scene()
        End If
    End Sub

    Private Sub m_trees_CheckedChanged(sender As Object, e As EventArgs) Handles m_trees.CheckedChanged
        m_trees_ = m_trees.Checked
        If Not trees_loaded And maploaded And m_trees_ Then
            MsgBox("You can change this setting but it was never loaded." + vbCrLf + "I can't draw it. It does not exist.", MsgBoxStyle.Exclamation, "Warning!")
        End If
        If maploaded Then
            frmMain.draw_scene()
        End If
    End Sub

    Private Sub m_models_CheckedChanged(sender As Object, e As EventArgs) Handles m_models.CheckedChanged
        m_models_ = m_models.Checked
        If Not models_loaded And maploaded And m_models_ Then
            MsgBox("You can change this setting but it was never loaded." + vbCrLf + "I can't draw it. It does not exist.", MsgBoxStyle.Exclamation, "Warning!")
        End If
        If maploaded Then
            frmMain.draw_scene()
        End If
    End Sub

    Private Sub m_decals_CheckedChanged(sender As Object, e As EventArgs) Handles m_decals.CheckedChanged
        m_decals_ = m_decals.Checked
        If Not decals_loaded And maploaded And m_decals_ Then
            MsgBox("You can change this setting but it was never loaded." + vbCrLf + "I can't draw it. It does not exist.", MsgBoxStyle.Exclamation, "Warning!")
        End If
        If maploaded Then
            frmMain.draw_scene()
        End If
    End Sub

    Private Sub m_water_CheckedChanged(sender As Object, e As EventArgs) Handles m_water.CheckedChanged
        m_water_ = m_water.Checked
        If Not water_loaded And maploaded And m_decals_ Then
            MsgBox("You can change this setting but it was never loaded." + vbCrLf + "I can't draw it. It does not exist.", MsgBoxStyle.Exclamation, "Warning!")
        End If
        If maploaded Then
            frmMain.draw_scene()
        End If
    End Sub

    Private Sub m_bases_CheckedChanged(sender As Object, e As EventArgs) Handles m_bases.CheckedChanged
        m_bases_ = m_bases.Checked
        If Not bases_loaded And maploaded And m_bases_ Then
            MsgBox("You can change this setting but it was never loaded." + vbCrLf + "I can't draw it. It does not exist.", MsgBoxStyle.Exclamation, "Warning!")
        End If
        If maploaded Then
            frmMain.draw_scene()
        End If
    End Sub

    Private Sub m_sky_CheckedChanged(sender As Object, e As EventArgs) Handles m_sky.CheckedChanged
        m_sky_ = m_sky.Checked
        If Not sky_loaded And maploaded And m_sky_ Then
            MsgBox("You can change this setting but it was never loaded." + vbCrLf + "I can't draw it. It does not exist.", MsgBoxStyle.Exclamation, "Warning!")
        End If
        If maploaded Then
            frmMain.draw_scene()
        End If
    End Sub

 
    Private Sub frmLoadOptions_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        My.Settings.Save()
        Me.Hide()
    End Sub
End Class