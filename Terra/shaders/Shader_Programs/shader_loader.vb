﻿Imports System
Imports System.Text
Imports System.String
Imports System.IO
Imports Tao.OpenGl
Imports Tao.Platform.Windows
Module shader_loader
    Public shader_list As New shader_list_
    Public Class shader_list_
        Public ring_Shader As Integer
        Public alphaTransparent_shader As Integer
        Public lowQualityTrees_shader As Integer
        Public branchDef_shader As Integer
        Public buildingsDef_shader As Integer
        Public colorMapper_shader As Integer
        Public comp_shader As Integer
        Public decalsCpassDef_shader As Integer
        Public decalsNpassDef_shader As Integer
        Public deferred_shader As Integer
        Public dome_shader As Integer
        Public fog_shader As Integer
        Public frondDef_shader As Integer
        Public FXAA_shader As Integer
        Public gaussian_shader As Integer
        Public hell_shader As Integer
        Public leafDef_shader As Integer
        Public leafcoloredDef_shader As Integer
        Public normal_shader As Integer
        Public normalOffset_shader As Integer
        Public render_shader As Integer
        Public shadow_shader As Integer
        Public shadowDepth_shader As Integer
        Public SSAO_shader As Integer
        Public SSAOBlend_shader As Integer
        Public lzTerrainDef_shader As Integer
        Public tankDef_shader As Integer
        Public terrainDef_shader As Integer
        Public terrainMarkers_shader As Integer
        Public toLinear_shader As Integer
        Public trees_shader As Integer
        Public water_shader As Integer
        Public waterColor_shader As Integer
        Public waterMask_shader As Integer
        Public wire_shader As Integer
        Public write3D_shader As Integer
    End Class

#Region "variables"

    Public view_normal_mode_ As Integer
    Public normal_mode As Integer = 0
    Public normal_length_ As Integer
    Public render_has_holes, render_hole_texture As Integer
    Public c_address, n_address, a_address, t_address, c_position As Integer
    Public c_address2, n_address2, a_address2, t_address2 As Integer
    Public a_address3, t_address3 As Integer
    Public a_address5 As Integer


    Public n_address6, f_address6, a_address6, t_address6 As Integer
    Public layer_1, layer_2, layer_3, layer_4, n_layer_1, n_layer_2, n_layer_3, n_layer_4 As Integer
    Public main_texture, is_bumped, gamma, gamma_2 As Integer
    Public gamma_3, branch_gamma_level_id, gamma_6, c_position3, branch_eye_pos_id, is_bumped4, is_bumped5 As Integer
    Public layer0U, layer1U, layer2U, layer3U As Integer
    Public layer0V, layer1V, layer2V, layer3V As Integer
    Public gray_level_1, gray_level_2, gray_level_3, gray_level_6, branch_gray_level_id As Integer
    Public u_mat1, u_mat2 As Integer
    Public mixtexture As Integer
    Public sun_lock As Boolean = False
    Public leaf_ambient_level_id, branch_ambient_level_id, decal_ambient, model_ambient As Integer
    Public decal_uv_wrap, decal_influence As Integer
    Public bump_out_ As Integer
    Public vismap_address As Integer
    Public noise_map_address As Integer
    Public basic_color, basic_normal, basic_color_level, basic_gamma As Integer
    Public frond_ambient_level_id As Integer
    Public colorMapper_mask_address, colorMapper_colorMap_address As Integer
    ' Public color_correct_addy As Integer
#End Region


    Public Sub make_shaders()
        'I'm tired of all the work every time I add a shader.
        'So... Im going to automate the process.. Hey.. its a computer for fucks sake!
        Dim f_list() As String = IO.Directory.GetFiles(Application.StartupPath + "\shaders", "*fragment.glsl")
        Dim v_list() As String = IO.Directory.GetFiles(Application.StartupPath + "\shaders\", "*vertex.glsl")
        Dim g_list() As String = IO.Directory.GetFiles(Application.StartupPath + "\shaders", "*geo.glsl")
        Array.Sort(f_list)
        Array.Sort(v_list)
        Array.Sort(g_list)
        ReDim shaders.shader(f_list.Length - 1)
        With shaders

            For i = 0 To f_list.Length - 1
                .shader(i) = New shaders_
                With .shader(i)
                    Dim fn As String = Path.GetFileNameWithoutExtension(f_list(i))
                    Dim ar = fn.Split("_")
                    .shader_name = ar(0) + "_shader"
                    .fragment = f_list(i)
                    .vertex = v_list(i)
                    .geo = ""
                    For Each g In g_list ' find matching geo if there is one.. usually there wont be
                        If g.Contains(ar(0)) Then
                            .geo = g
                            .has_geo = True ' found a matching geo so we need to set this true
                        End If
                    Next
                    .shader_id = -1
                    .set_call_id(-1)
                End With
            Next

        End With
        Dim fs As String
        Dim vs As String
        Dim gs As String

        For i = 0 To shaders.shader.Length - 1
            With shaders.shader(i)
                vs = .vertex
                fs = .fragment
                gs = .geo
                Dim id = assemble_shader(vs, gs, fs, .shader_id, .shader_name, .has_geo)
                .set_call_id(id)
                .shader_id = id
             
                'Debug.WriteLine(.shader_name + "  Id:" + .shader_id.ToString)
            End With
        Next

    End Sub
    Public Function assemble_shader(v As String, g As String, f As String, ByRef shader As Integer, ByRef name As String, ByRef has_geo As Boolean) As Integer
        Dim vs(1) As String
        Dim gs(1) As String
        Dim fs(1) As String
        Dim vertexObject As Integer
        Dim geoObject As Integer
        Dim fragmentObject As Integer
        Dim status_code As Integer
        Dim info As New StringBuilder
        info.Length = 8192
        Dim info_l As Integer
     
        If shader > 0 Then
            Gl.glUseProgram(0)
            Gl.glDeleteProgram(shader)
            Gl.glGetProgramiv(shader, Gl.GL_DELETE_STATUS, status_code)
            Gl.glFinish()
        End If

        Dim e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            'MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If
        'have a hard time with files remaining open.. hope this fixes it! (yep.. it did)
        Using vs_s As New StreamReader(v)
            vs(0) = vs_s.ReadToEnd
            vs_s.Close()
            vs_s.Dispose()
        End Using
        Using fs_s As New StreamReader(f)
            fs(0) = fs_s.ReadToEnd
            fs_s.Close()
            fs_s.Dispose()
        End Using
        If has_geo Then
            Using gs_s As New StreamReader(g)
                gs(0) = gs_s.ReadToEnd
                gs_s.Close()
                gs_s.Dispose()
            End Using
        End If


        vertexObject = Gl.glCreateShader(Gl.GL_VERTEX_SHADER)
        fragmentObject = Gl.glCreateShader(Gl.GL_FRAGMENT_SHADER)
        '--------------------------------------------------------------------
        shader = Gl.glCreateProgram()

        ' Compile vertex shader
        Gl.glShaderSource(vertexObject, 1, vs, vs(0).Length)
        Gl.glCompileShader(vertexObject)
        Gl.glGetShaderInfoLog(vertexObject, 8192, info_l, info)
        Gl.glGetShaderiv(vertexObject, Gl.GL_COMPILE_STATUS, status_code)
        If Not status_code = Gl.GL_TRUE Then
            Gl.glDeleteShader(vertexObject)
            gl_error(name + "_vertex didn't compile!" + vbCrLf + info.ToString)
            'Return
        End If

        e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If

        If has_geo Then
            'geo
            geoObject = Gl.glCreateShader(Gl.GL_GEOMETRY_SHADER_EXT)
            Gl.glShaderSource(geoObject, 1, gs, gs(0).Length)
            Gl.glCompileShader(geoObject)
            Gl.glGetShaderInfoLog(geoObject, 8192, info_l, info)
            Gl.glGetShaderiv(geoObject, Gl.GL_COMPILE_STATUS, status_code)
            If Not status_code = Gl.GL_TRUE Then
                Gl.glDeleteShader(geoObject)
                gl_error(name + "_geo didn't compile!" + vbCrLf + info.ToString)
                'Return
            End If
            e = Gl.glGetError
            If e <> 0 Then
                Dim s = Glu.gluErrorString(e)
                Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
                MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
            End If
            If name.Contains("raytrace") Then

                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_INPUT_TYPE_EXT, Gl.GL_TRIANGLES)
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_OUTPUT_TYPE_EXT, Gl.GL_LINE_STRIP)
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_VERTICES_OUT_EXT, 6)
            End If
            If name.Contains("normal") Then
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_INPUT_TYPE_EXT, Gl.GL_TRIANGLES)
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_OUTPUT_TYPE_EXT, Gl.GL_LINE_STRIP)
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_VERTICES_OUT_EXT, 18)
            End If

            e = Gl.glGetError
            If e <> 0 Then
                Dim s = Glu.gluErrorString(e)
                Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
                MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
            End If

        End If

        ' Compile fragment shader

        Gl.glShaderSource(fragmentObject, 1, fs, fs(0).Length)
        Gl.glCompileShader(fragmentObject)
        Gl.glGetShaderInfoLog(fragmentObject, 8192, info_l, info)
        Gl.glGetShaderiv(fragmentObject, Gl.GL_COMPILE_STATUS, status_code)

        If Not status_code = Gl.GL_TRUE Then
            Gl.glDeleteShader(fragmentObject)
            gl_error(name + "_fragment didn't compile!" + vbCrLf + info.ToString)
            'Return
        End If
        e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If

        'attach shader objects
        Gl.glAttachShader(shader, fragmentObject)
        If has_geo Then
            Gl.glAttachShader(shader, geoObject)
        End If
        Gl.glAttachShader(shader, vertexObject)

        'link program
        Gl.glLinkProgram(shader)

        ' detach shader objects
        Gl.glDetachShader(shader, fragmentObject)
        If has_geo Then
            Gl.glDetachShader(shader, geoObject)
        End If
        Gl.glDetachShader(shader, vertexObject)

        e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If

        Gl.glGetShaderiv(shader, Gl.GL_LINK_STATUS, status_code)

        If Not status_code = Gl.GL_TRUE Then
            Gl.glDeleteProgram(shader)
            gl_error(name + " did not link!" + vbCrLf + info.ToString)
            'Return
        End If

        'delete shader objects
        Gl.glDeleteShader(fragmentObject)
        Gl.glGetShaderiv(fragmentObject, Gl.GL_DELETE_STATUS, status_code)
        If has_geo Then
            Gl.glDeleteShader(geoObject)
            Gl.glGetShaderiv(geoObject, Gl.GL_DELETE_STATUS, status_code)
        End If
        Gl.glDeleteShader(vertexObject)
        Gl.glGetShaderiv(vertexObject, Gl.GL_DELETE_STATUS, status_code)
        e = Gl.glGetError
        If e <> 0 Then
            'aways throws a error after deletion even though the status shows them as deleted.. ????
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            'MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If
        vs(0) = Nothing
        fs(0) = Nothing
        If has_geo Then
            gs(0) = Nothing
        End If
        GC.Collect()
        GC.WaitForFullGCComplete()

        Return shader
    End Function


    Public Sub gl_error(s As String)
        s = s.Replace(vbLf, vbCrLf)
        s.Replace("0(", vbCrLf + "(")
        frmShaderError.Show()
        frmShaderError.er_tb.Text += s
    End Sub
    Private Sub set_terrainDef_variables()
        main_texture = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "main_texture")
        'No idea how to use this or if its even needed.
        ' color_correct_addy = Gl.glGetUniformLocation(shader_list.render_shader, "dom_")
      
        render_has_holes = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "has_holes")
        '--------------------------------------------------------------------------
        layer_1 = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "layer_1")
        layer_2 = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "layer_2")
        layer_3 = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "layer_3")
        layer_4 = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "layer_4")
        n_layer_1 = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "n_layer_1")
        n_layer_2 = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "n_layer_2")
        n_layer_3 = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "n_layer_3")
        n_layer_4 = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "n_layer_4")
        mixtexture = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "mixtexture")
        c_address = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "colorMap")
        'dominateTex = Gl.glGetUniformLocation(shader_list.render_shader, "DominateMap")
        layer0U = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "layer0U")
        layer1U = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "layer1U")
        layer2U = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "layer2U")
        layer3U = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "layer3U")
        layer0V = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "layer0V")
        layer1V = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "layer1V")
        layer2V = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "layer2V")
        layer3V = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "layer3V")
        render_hole_texture = Gl.glGetUniformLocation(shader_list.terrainDef_shader, "hole_texture")
    End Sub

    Private Sub set_lzTerrainDef_variables()
        c_address2 = Gl.glGetUniformLocation(shader_list.lzTerrainDef_shader, "colorMap")
    End Sub

    Public deferred_gcolor, deferred_gnormal, deferred_gposition, _
        deferred_light_position, deferred_cam_position, deferred_gamma, _
        deferred_spec, deferred_gray, deferred_bright, deferred_ambient, _
        deferred_mapHeight, deferred_depthmap, deferred_lights_pos, _
        deferred_lights_color, deferred_light_count, deferred_gFlags As Integer

    Private Sub set_deferredLighting_variables()
        deferred_cam_position = Gl.glGetUniformLocation(shader_list.deferred_shader, "viewPos")
        deferred_light_position = Gl.glGetUniformLocation(shader_list.deferred_shader, "LightPos")
        deferred_gcolor = Gl.glGetUniformLocation(shader_list.deferred_shader, "gColor")
        deferred_gnormal = Gl.glGetUniformLocation(shader_list.deferred_shader, "gNormal")
        deferred_gposition = Gl.glGetUniformLocation(shader_list.deferred_shader, "gPosition")
        deferred_depthmap = Gl.glGetUniformLocation(shader_list.deferred_shader, "depthMap")
        deferred_ambient = Gl.glGetUniformLocation(shader_list.deferred_shader, "ambient_level")
        deferred_gray = Gl.glGetUniformLocation(shader_list.deferred_shader, "gray_level")
        deferred_spec = Gl.glGetUniformLocation(shader_list.deferred_shader, "spec_level")
        deferred_gamma = Gl.glGetUniformLocation(shader_list.deferred_shader, "gamma_level")
        deferred_bright = Gl.glGetUniformLocation(shader_list.deferred_shader, "bright_level")
        deferred_mapHeight = Gl.glGetUniformLocation(shader_list.deferred_shader, "mapHeight")
        deferred_lights_pos = Gl.glGetUniformLocation(shader_list.deferred_shader, "light_positions")
        deferred_lights_color = Gl.glGetUniformLocation(shader_list.deferred_shader, "light_colors")
        deferred_light_count = Gl.glGetUniformLocation(shader_list.deferred_shader, "light_count")
        deferred_gFlags = Gl.glGetUniformLocation(shader_list.deferred_shader, "gFlags")
        'deferred_Matrix = Gl.glGetUniformLocation(shader_list.deferred_shader, "projectionmatrix")
    End Sub

    Private Sub set_comp_variables()
        bump_out_ = Gl.glGetUniformLocation(shader_list.comp_shader, "amount")
    End Sub

    Private Sub set_normal_variables()
        view_normal_mode_ = Gl.glGetUniformLocation(shader_list.normal_shader, "mode")
        normal_length_ = Gl.glGetUniformLocation(shader_list.normal_shader, "l_length")
    End Sub

    Public is_GAmap, alphaRef, alphaTestEnable, is_multi_textured As Integer
    Public n_address3, bld_matrix, c_address3, colormap2, is_bumped3, bld_flag As Integer
    Private Sub set_buldingsDef_variables()
        c_address3 = Gl.glGetUniformLocation(shader_list.buildingsDef_shader, "colorMap")
        colormap2 = Gl.glGetUniformLocation(shader_list.buildingsDef_shader, "colorMap_2")
        n_address3 = Gl.glGetUniformLocation(shader_list.buildingsDef_shader, "normalMap")
        is_bumped3 = Gl.glGetUniformLocation(shader_list.buildingsDef_shader, "is_bumped")
        is_GAmap = Gl.glGetUniformLocation(shader_list.buildingsDef_shader, "is_GAmap")
        is_multi_textured = Gl.glGetUniformLocation(shader_list.buildingsDef_shader, "is_multi_textured")
        alphaRef = Gl.glGetUniformLocation(shader_list.buildingsDef_shader, "alphaRef")
        alphaTestEnable = Gl.glGetUniformLocation(shader_list.buildingsDef_shader, "alphaTestEnable")
        bld_matrix = Gl.glGetUniformLocation(shader_list.buildingsDef_shader, "matrix")
        bld_flag = Gl.glGetUniformLocation(shader_list.buildingsDef_shader, "flag")
    End Sub

    Public branch_color_map_id, branch_normalmap_map_id, branch_matrix_id As Integer
    Private Sub set_branchDef_variables()
        branch_color_map_id = Gl.glGetUniformLocation(shader_list.branchDef_shader, "colorMap")
        branch_normalmap_map_id = Gl.glGetUniformLocation(shader_list.branchDef_shader, "normalMap")
        branch_matrix_id = Gl.glGetUniformLocation(shader_list.branchDef_shader, "matrix")
    End Sub

    Public frond_color_map_id, frond_normal_map_id, frond_matrix_id As Integer
    Private Sub set_frondDef_variables()
        frond_color_map_id = Gl.glGetUniformLocation(shader_list.frondDef_shader, "colorMap")
        frond_normal_map_id = Gl.glGetUniformLocation(shader_list.frondDef_shader, "normalMap")
        frond_matrix_id = Gl.glGetUniformLocation(shader_list.frondDef_shader, "matrix")
    End Sub

    Public leaf_color_map_id, leaf_normal_map_id, leaf_matrix_id As Integer
    Private Sub set_leafDef_variables()
        leaf_color_map_id = Gl.glGetUniformLocation(shader_list.leafDef_shader, "colorMap")
        leaf_normal_map_id = Gl.glGetUniformLocation(shader_list.leafDef_shader, "normalMap")
        leaf_matrix_id = Gl.glGetUniformLocation(shader_list.leafDef_shader, "matrix")
    End Sub

    Public leafColored_matrix As Integer
    Private Sub set_leafColored_variables()
        leafColored_matrix = Gl.glGetUniformLocation(shader_list.leafcoloredDef_shader, "matrix")
    End Sub

    Private Sub set_colorMapper_variables()
        colorMapper_mask_address = Gl.glGetUniformLocation(shader_list.colorMapper_shader, "mask")
        colorMapper_colorMap_address = Gl.glGetUniformLocation(shader_list.colorMapper_shader, "colorMap")
    End Sub

    Public ring_radius, ring_thickness, ring_location, ring_depthmap As Integer
    Public Sub set_ring_variables()

        ring_location = Gl.glGetUniformLocation(shader_list.ring_Shader, "ring_center")
        ring_radius = Gl.glGetUniformLocation(shader_list.ring_Shader, "radius")
        ring_thickness = Gl.glGetUniformLocation(shader_list.ring_Shader, "thickness")
        ring_depthmap = Gl.glGetUniformLocation(shader_list.ring_Shader, "depthMap")
    End Sub

    Public decal_color_map_id, decal_normal_map_id, decal_normal_in_id, _
    decal_depthmap_id, decal_bl_id, decal_tr_id, decal_matrix_id, decal_flag, decal_flagmap As Integer
    Private Sub set_decalsNpassDef_variables()
        decal_color_map_id = Gl.glGetUniformLocation(shader_list.decalsNpassDef_shader, "colorMap")
        decal_normal_map_id = Gl.glGetUniformLocation(shader_list.decalsNpassDef_shader, "normalMap")
        decal_depthmap_id = Gl.glGetUniformLocation(shader_list.decalsNpassDef_shader, "depthMap")
        decal_normal_in_id = Gl.glGetUniformLocation(shader_list.decalsNpassDef_shader, "normal_in")
        decal_matrix_id = Gl.glGetUniformLocation(shader_list.decalsNpassDef_shader, "decal_matrix")
        decal_tr_id = Gl.glGetUniformLocation(shader_list.decalsNpassDef_shader, "tr")
        decal_bl_id = Gl.glGetUniformLocation(shader_list.decalsNpassDef_shader, "bl")

        decal_uv_wrap = Gl.glGetUniformLocation(shader_list.decalsNpassDef_shader, "uv_wrap")
        decal_influence = Gl.glGetUniformLocation(shader_list.decalsNpassDef_shader, "influence")
        'decal_flag = Gl.glGetUniformLocation(shader_list.decalsNpassDef_shader, "flag")
        decal_flagmap = Gl.glGetUniformLocation(shader_list.decalsNpassDef_shader, "gFlag")
    End Sub

    Public prjd_color, prjd_normal, prjd_depthmap, prjd_matrix, prjd_topright, _
        prjd_bottomleft, prjd_uv_wrap, prjd_influence, prjd_normal_in, prjd_flag, prjd_flagmap As Integer
    Private Sub set_decalsCpassDef_variables()
        prjd_color = Gl.glGetUniformLocation(shader_list.decalsCpassDef_shader, "colorMap")
        prjd_normal = Gl.glGetUniformLocation(shader_list.decalsCpassDef_shader, "normalMap")
        prjd_depthmap = Gl.glGetUniformLocation(shader_list.decalsCpassDef_shader, "depthMap")
        prjd_normal_in = Gl.glGetUniformLocation(shader_list.decalsCpassDef_shader, "normal_in")
        prjd_matrix = Gl.glGetUniformLocation(shader_list.decalsCpassDef_shader, "decal_matrix")
        prjd_topright = Gl.glGetUniformLocation(shader_list.decalsCpassDef_shader, "tr")
        prjd_bottomleft = Gl.glGetUniformLocation(shader_list.decalsCpassDef_shader, "bl")
        prjd_uv_wrap = Gl.glGetUniformLocation(shader_list.decalsCpassDef_shader, "uv_wrap")
        prjd_influence = Gl.glGetUniformLocation(shader_list.decalsCpassDef_shader, "influence")
        prjd_flag = Gl.glGetUniformLocation(shader_list.decalsCpassDef_shader, "flag")
        prjd_flagmap = Gl.glGetUniformLocation(shader_list.decalsCpassDef_shader, "gFlag")
    End Sub

    Public toLinear_tex As Integer
    Private Sub set_toLinear_variables()
        toLinear_tex = Gl.glGetUniformLocation(shader_list.toLinear_shader, "depthMap")
    End Sub

    Public tankDef_matrix As Integer
    Private Sub set_tankDef_variables()
        tankDef_matrix = Gl.glGetUniformLocation(shader_list.tankDef_shader, "matrix")
    End Sub
    Public fxaa_frameBufferSize, fxaa_texture_in As Integer
    Private Sub set_FXAA_variables()
        fxaa_frameBufferSize = Gl.glGetUniformLocation(shader_list.FXAA_shader, "frameBufSize")
        fxaa_texture_in = Gl.glGetUniformLocation(shader_list.FXAA_shader, "buf0")

    End Sub

    Public dome_colormap As Integer
    Private Sub set_dome_variables()
        dome_colormap = Gl.glGetUniformLocation(shader_list.dome_shader, "colorMap")
    End Sub

    Public shadow_cam_matrix, shadow_cam_position As Integer
    Private Sub set_shadow_variables()
        shadow_cam_matrix = Gl.glGetUniformLocation(shader_list.shadow_shader, "cam_matrix")
        shadow_cam_position = Gl.glGetUniformLocation(shader_list.shadow_shader, "cam_position")
    End Sub

    Public ss_gNormal, ss_gDepthMap, ss_noise, ss_kernel, ss_screen_size, ss_prj_matrix, ss_mdl_Matrix As Integer
    Private Sub set_SSAO_variables()
        ss_gNormal = Gl.glGetUniformLocation(shader_list.SSAO_shader, "u_normalTexture")
        ss_gDepthMap = Gl.glGetUniformLocation(shader_list.SSAO_shader, "u_depthTexture")
        ss_noise = Gl.glGetUniformLocation(shader_list.SSAO_shader, "u_rotationNoiseTexture")
        ss_kernel = Gl.glGetUniformLocation(shader_list.SSAO_shader, "u_kernel")
        ss_screen_size = Gl.glGetUniformLocation(shader_list.SSAO_shader, "screen_size")
        ss_prj_matrix = Gl.glGetUniformLocation(shader_list.SSAO_shader, "u_projectionMatrix")
        ss_mdl_Matrix = Gl.glGetUniformLocation(shader_list.SSAO_shader, "mdl_matrix")
    End Sub

    Public SSAOBlend_gcolor, SSAOBlend_gFlag As Integer
    Private Sub set_SSAOBlend_variables()
        SSAOBlend_gcolor = Gl.glGetUniformLocation(shader_list.SSAOBlend_shader, "gColor")
        SSAOBlend_gFlag = Gl.glGetUniformLocation(shader_list.SSAOBlend_shader, "gFlags")

    End Sub

    Public normalOffset_normal As Integer
    Private Sub set_normalOffset_variables()
        normalOffset_normal = Gl.glGetUniformLocation(shader_list.normalOffset_shader, "normalMap")
    End Sub

    Public tm_bb_tr, tm_bb_bl, tm_grid_size, tm_show_border, tm_show_grid, tm_show_chunks As Integer
    Private Sub set_terrainMarkers_variables()
        tm_bb_tr = Gl.glGetUniformLocation(shader_list.terrainMarkers_shader, "bb_tr")
        tm_bb_bl = Gl.glGetUniformLocation(shader_list.terrainMarkers_shader, "bb_bl")
        tm_grid_size = Gl.glGetUniformLocation(shader_list.terrainMarkers_shader, "g_size")
        tm_show_border = Gl.glGetUniformLocation(shader_list.terrainMarkers_shader, "show_border")
        tm_show_grid = Gl.glGetUniformLocation(shader_list.terrainMarkers_shader, "show_grid")
        tm_show_chunks = Gl.glGetUniformLocation(shader_list.terrainMarkers_shader, "show_chunks")

    End Sub

    Public water_colorMap, water_normalMap, water_normalMap2, water_gNormal, water_gDepthMap, _
        water_time, water_matrix, water_level, water_aspect, water_texture_shift, water_foam As Integer
    Private Sub set_water_variables()
        water_colorMap = Gl.glGetUniformLocation(shader_list.water_shader, "colorMap")
        water_normalMap = Gl.glGetUniformLocation(shader_list.water_shader, "normalMap")
        water_normalMap2 = Gl.glGetUniformLocation(shader_list.water_shader, "normalMap2")
        water_gNormal = Gl.glGetUniformLocation(shader_list.water_shader, "gNormalIn")
        water_gDepthMap = Gl.glGetUniformLocation(shader_list.water_shader, "gDepthMap")
        water_time = Gl.glGetUniformLocation(shader_list.water_shader, "time")
        water_matrix = Gl.glGetUniformLocation(shader_list.water_shader, "matrix")
        water_level = Gl.glGetUniformLocation(shader_list.water_shader, "water_level")
        water_aspect = Gl.glGetUniformLocation(shader_list.water_shader, "aspect")
        water_texture_shift = Gl.glGetUniformLocation(shader_list.water_shader, "shift")
        water_foam = Gl.glGetUniformLocation(shader_list.water_shader, "foamMap")
    End Sub

    Public waterC_color, waterC_matrix, waterC_Depthmap As Integer
    Private Sub set_waterColor_variables()
        waterC_color = Gl.glGetUniformLocation(shader_list.waterColor_shader, "colorMap")
        waterC_matrix = Gl.glGetUniformLocation(shader_list.waterColor_shader, "matrix")
        waterC_Depthmap = Gl.glGetUniformLocation(shader_list.waterColor_shader, "gDepthMap")
    End Sub
    '==============================================================================================================
    Public Sub set_shader_variables()
        set_ring_variables()
        set_terrainDef_variables()
        set_deferredLighting_variables()
        set_comp_variables()
        set_normal_variables()
        set_buldingsDef_variables()

        set_branchDef_variables()
        set_frondDef_variables()
        set_leafDef_variables()
        set_leafColored_variables()

        set_colorMapper_variables()
        set_lzTerrainDef_variables()

        set_decalsNpassDef_variables()
        set_decalsCpassDef_variables()

        set_toLinear_variables()
        set_tankDef_variables()
        set_FXAA_variables()
        set_dome_variables()
        set_shadow_variables()
        set_SSAO_variables()
        set_SSAOBlend_variables()
        set_terrainMarkers_variables()

        set_water_variables()
        set_waterColor_variables()
        Return

    End Sub
    '==============================================================================================================

End Module
