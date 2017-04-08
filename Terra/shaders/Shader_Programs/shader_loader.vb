Imports System
Imports System.Text
Imports System.String
Imports System.IO
Imports Tao.OpenGl
Imports Tao.Platform.Windows
Module shader_loader
    Public shader_list As New shader_list_
    Public Class shader_list_
        Public alphaTransparent_shader As Integer
        Public lowQualityTrees_shader As Integer
        Public branch_shader As Integer
        Public bump_shader As Integer
        Public colorMapper_shader As Integer
        Public comp_shader As Integer
        Public decals_shader As Integer
        Public depth_shader As Integer
        Public fog_shader As Integer
        Public frond_shader As Integer
        Public gaussian_shader As Integer
        Public hell_shader As Integer
        Public leaf_shader As Integer
        Public leafcolored_shader As Integer
        Public normal_shader As Integer
        Public render_shader As Integer
        Public shadowDepth_shader As Integer
        Public shadowTest_shader As Integer
        Public ss_shader As Integer
        Public tank_shader As Integer
        Public toLinear_shader As Integer
        Public trees_shader As Integer
        Public write3D_shader As Integer
    End Class
   
    Public view_normal_mode_ As Integer
    Public normal_mode As Integer = 0
    Public normal_length_ As Integer
    Public render_has_holes, render_hole_texture As Integer
    Public c_address, n_address, a_address, t_address, c_position As Integer
    Public c_address2, n_address2, a_address2, t_address2 As Integer
    Public c_address3, n_address3, a_address3, t_address3 As Integer
    Public branch_color_map_id, branch_normalmap_map_id, branch_tex_level_id As Integer
    Public a_address5 As Integer

    Public decal_color_map_id, decal_normal_map_id, decal_cam_position_id, _
        decal_tex_level_id, decal_gray_level_id, decal_gamma_level_id As Integer

    Public n_address6, f_address6, a_address6, t_address6 As Integer
    Public layer_1, layer_2, layer_3, layer_4, n_layer_1, n_layer_2, n_layer_3, n_layer_4 As Integer
    Public main_texture, is_bumped, is_multi_textured, colormap2, gamma, gamma_2 As Integer
    Public gamma_3, branch_gamma_level_id, gamma_6, c_position3, branch_eye_pos_id, is_bumped3, is_bumped4, is_bumped5 As Integer
    Public layer0U, layer1U, layer2U, layer3U As Integer
    Public layer0V, layer1V, layer2V, layer3V As Integer
    Public gray_level_1, gray_level_2, gray_level_3, gray_level_6, branch_gray_level_id As Integer
    Public is_GAmap, alphaRef, alphaTestEnable, tangent, binormal As Integer
    Public u_mat1, u_mat2 As Integer
    Public mixtexture As Integer
    Public sun_lock As Boolean = False
    Public leaf_color_map_id, leaf_normal_map_id, leaf_tex_level_id, leaf_gamma_level_id, leaf_eye_pos_id, _
                                  leaf_gray_level_id As Integer
    Public leaf_ambient_level_id, branch_ambient_level_id, decal_ambient, model_ambient As Integer
    Public decal_u_wrap, decal_v_wrap, decal_influence As Integer
    Public bump_out_ As Integer
    Public vismap_address As Integer
    Public noise_map_address As Integer
    Public basic_color, basic_normal, basic_color_level, basic_gamma As Integer
    Public frond_color_map_id, frond_normal_map_id, frond_tex_level_id, frond_gamma_level_id, frond_eye_pos_id, _
                                frond_gray_level_id As Integer
    Public frond_ambient_level_id As Integer
    Public colorMapper_mask_address, colorMapper_colorMap_address As Integer
    ' Public color_correct_addy As Integer

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
                    For Each g In g_list ' find matching geo if there is one.. useally there wont be
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
        info.Length = 1024
        Dim info_l As Integer


        If shader > 0 Then
            Gl.glUseProgram(0)
            Gl.glDeleteProgram(shader)
            Gl.glGetProgramiv(shader, Gl.GL_DELETE_STATUS, status_code)
            Gl.glFinish()
        End If

        Dim e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e).ToString
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
            Dim s = Glu.gluErrorString(e).ToString
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
                Dim s = Glu.gluErrorString(e).ToString
                Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
                MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
            End If
            Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_INPUT_TYPE_EXT, Gl.GL_TRIANGLES)
            Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_OUTPUT_TYPE_EXT, Gl.GL_LINE_STRIP)
            If name.Contains("normal") Then
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_VERTICES_OUT_EXT, 18)
            Else
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_VERTICES_OUT_EXT, 4) 'leaf needs 4
            End If
            e = Gl.glGetError
            If e <> 0 Then
                Dim s = Glu.gluErrorString(e).ToString
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
            Dim s = Glu.gluErrorString(e).ToString
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
            Dim s = Glu.gluErrorString(e).ToString
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
            Dim s = Glu.gluErrorString(e).ToString
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
    Public Sub set_shader_variables()

        colorMapper_mask_address = Gl.glGetUniformLocation(shader_list.colorMapper_shader, "mask")
        colorMapper_colorMap_address = Gl.glGetUniformLocation(shader_list.colorMapper_shader, "colorMap")
        '--------------------------------------------------------------------------

        c_address2 = Gl.glGetUniformLocation(shader_list.ss_shader, "colorMap")
        n_address2 = Gl.glGetUniformLocation(shader_list.ss_shader, "normalMap")
        a_address2 = Gl.glGetUniformLocation(shader_list.ss_shader, "l_ambient")
        t_address2 = Gl.glGetUniformLocation(shader_list.ss_shader, "l_texture")
        gray_level_2 = Gl.glGetUniformLocation(shader_list.ss_shader, "gray_level")
        gamma_2 = Gl.glGetUniformLocation(shader_list.ss_shader, "gamma")
        '--------------------------------------------------------------------------
        main_texture = Gl.glGetUniformLocation(shader_list.render_shader, "main_texture")
        'No idea how to use this or if its even needed.
        ' color_correct_addy = Gl.glGetUniformLocation(shader_list.render_shader, "dom_")
        c_position = Gl.glGetUniformLocation(shader_list.render_shader, "cam_position")
        a_address = Gl.glGetUniformLocation(shader_list.render_shader, "l_ambient")
        t_address = Gl.glGetUniformLocation(shader_list.render_shader, "l_texture")
        gray_level_1 = Gl.glGetUniformLocation(shader_list.render_shader, "gray_level")
        gamma = Gl.glGetUniformLocation(shader_list.render_shader, "gamma")
        render_has_holes = Gl.glGetUniformLocation(shader_list.render_shader, "has_holes")
        '--------------------------------------------------------------------------
        layer_1 = Gl.glGetUniformLocation(shader_list.render_shader, "layer_1")
        layer_2 = Gl.glGetUniformLocation(shader_list.render_shader, "layer_2")
        layer_3 = Gl.glGetUniformLocation(shader_list.render_shader, "layer_3")
        layer_4 = Gl.glGetUniformLocation(shader_list.render_shader, "layer_4")
        n_layer_1 = Gl.glGetUniformLocation(shader_list.render_shader, "n_layer_1")
        n_layer_2 = Gl.glGetUniformLocation(shader_list.render_shader, "n_layer_2")
        n_layer_3 = Gl.glGetUniformLocation(shader_list.render_shader, "n_layer_3")
        n_layer_4 = Gl.glGetUniformLocation(shader_list.render_shader, "n_layer_4")
        mixtexture = Gl.glGetUniformLocation(shader_list.render_shader, "mixtexture")
        c_address = Gl.glGetUniformLocation(shader_list.render_shader, "colorMap")
        'dominateTex = Gl.glGetUniformLocation(shader_list.render_shader, "DominateMap")
        layer0U = Gl.glGetUniformLocation(shader_list.render_shader, "layer0U")
        layer1U = Gl.glGetUniformLocation(shader_list.render_shader, "layer1U")
        layer2U = Gl.glGetUniformLocation(shader_list.render_shader, "layer2U")
        layer3U = Gl.glGetUniformLocation(shader_list.render_shader, "layer3U")
        layer0V = Gl.glGetUniformLocation(shader_list.render_shader, "layer0V")
        layer1V = Gl.glGetUniformLocation(shader_list.render_shader, "layer1V")
        layer2V = Gl.glGetUniformLocation(shader_list.render_shader, "layer2V")
        layer3V = Gl.glGetUniformLocation(shader_list.render_shader, "layer3V")
        render_hole_texture = Gl.glGetUniformLocation(shader_list.render_shader, "hole_texture")
        '--------------------------------------------------------------------------
        c_address3 = Gl.glGetUniformLocation(shader_list.bump_shader, "colorMap")
        colormap2 = Gl.glGetUniformLocation(shader_list.bump_shader, "colorMap_2")
        n_address3 = Gl.glGetUniformLocation(shader_list.bump_shader, "normalMap")
        'c_position3 = Gl.glGetUniformLocation(bump_shader, "camPos")
        t_address3 = Gl.glGetUniformLocation(shader_list.bump_shader, "l_texture")
        gray_level_3 = Gl.glGetUniformLocation(shader_list.bump_shader, "gray_level")
        is_bumped3 = Gl.glGetUniformLocation(shader_list.bump_shader, "is_bumped")
        is_GAmap = Gl.glGetUniformLocation(shader_list.bump_shader, "is_GAmap")
        is_multi_textured = Gl.glGetUniformLocation(shader_list.bump_shader, "is_multi_textured")
        gamma_3 = Gl.glGetUniformLocation(shader_list.bump_shader, "gamma")
        alphaRef = Gl.glGetUniformLocation(shader_list.bump_shader, "alphaRef")
        alphaTestEnable = Gl.glGetUniformLocation(shader_list.bump_shader, "alphaTestEnable")
        model_ambient = Gl.glGetUniformLocation(shader_list.bump_shader, "ambient")
        '--------------------------------------------------------------------------
        branch_color_map_id = Gl.glGetUniformLocation(shader_list.branch_shader, "colorMap")
        branch_normalmap_map_id = Gl.glGetUniformLocation(shader_list.branch_shader, "normalMap")
        branch_eye_pos_id = Gl.glGetUniformLocation(shader_list.branch_shader, "cam_position")
        branch_tex_level_id = Gl.glGetUniformLocation(shader_list.branch_shader, "l_texture")
        branch_gray_level_id = Gl.glGetUniformLocation(shader_list.branch_shader, "gray_level")
        branch_gamma_level_id = Gl.glGetUniformLocation(shader_list.branch_shader, "gamma")
        branch_ambient_level_id = Gl.glGetUniformLocation(shader_list.branch_shader, "ambient")
        '-----------------------------------------------------------------
        frond_color_map_id = Gl.glGetUniformLocation(shader_list.frond_shader, "colorMap")
        frond_normal_map_id = Gl.glGetUniformLocation(shader_list.frond_shader, "normalMap")
        frond_eye_pos_id = Gl.glGetUniformLocation(shader_list.frond_shader, "cam_position")
        frond_tex_level_id = Gl.glGetUniformLocation(shader_list.frond_shader, "l_texture")
        frond_gray_level_id = Gl.glGetUniformLocation(shader_list.frond_shader, "gray_level")
        frond_gamma_level_id = Gl.glGetUniformLocation(shader_list.frond_shader, "gamma")
        frond_ambient_level_id = Gl.glGetUniformLocation(shader_list.frond_shader, "ambient")
        '-----------------------------------------------------------------
        leaf_color_map_id = Gl.glGetUniformLocation(shader_list.leaf_shader, "colorMap")
        leaf_normal_map_id = Gl.glGetUniformLocation(shader_list.leaf_shader, "normalMap")
        leaf_eye_pos_id = Gl.glGetUniformLocation(shader_list.leaf_shader, "cam_position")
        leaf_tex_level_id = Gl.glGetUniformLocation(shader_list.leaf_shader, "l_texture")
        leaf_gray_level_id = Gl.glGetUniformLocation(shader_list.leaf_shader, "gray_level")
        leaf_gamma_level_id = Gl.glGetUniformLocation(shader_list.leaf_shader, "gamma")
        leaf_ambient_level_id = Gl.glGetUniformLocation(shader_list.leaf_shader, "ambient")
        '--------------------------------------------------------------------------
        decal_color_map_id = Gl.glGetUniformLocation(shader_list.decals_shader, "colorMap")
        decal_normal_map_id = Gl.glGetUniformLocation(shader_list.decals_shader, "normalMap")
        decal_cam_position_id = Gl.glGetUniformLocation(shader_list.decals_shader, "cam_position")
        decal_tex_level_id = Gl.glGetUniformLocation(shader_list.decals_shader, "l_texture")
        decal_gray_level_id = Gl.glGetUniformLocation(shader_list.decals_shader, "gray_level")
        decal_gamma_level_id = Gl.glGetUniformLocation(shader_list.decals_shader, "gamma")
        decal_ambient = Gl.glGetUniformLocation(shader_list.decals_shader, "ambient")
        decal_u_wrap = Gl.glGetUniformLocation(shader_list.decals_shader, "u_wrap")
        decal_v_wrap = Gl.glGetUniformLocation(shader_list.decals_shader, "v_wrap")
        decal_influence = Gl.glGetUniformLocation(shader_list.decals_shader, "influence")
        '-----------------------------------------------------------------
        '-----------------
        bump_out_ = Gl.glGetUniformLocation(shader_list.comp_shader, "amount")
        '-----------------
        vismap_address = Gl.glGetUniformLocation(shader_list.fog_shader, "map")
        noise_map_address = Gl.glGetUniformLocation(shader_list.fog_shader, "noise_map")
        '-----------------
        view_normal_mode_ = Gl.glGetUniformLocation(shader_list.normal_shader, "mode")
        normal_length_ = Gl.glGetUniformLocation(shader_list.normal_shader, "l_length")
        '-----------------
        basic_color = Gl.glGetUniformLocation(shader_list.lowQualityTrees_shader, "colorMap")
        basic_normal = Gl.glGetUniformLocation(shader_list.lowQualityTrees_shader, "normalMap")
        basic_color_level = Gl.glGetUniformLocation(shader_list.lowQualityTrees_shader, "c_level")
        basic_gamma = Gl.glGetUniformLocation(shader_list.lowQualityTrees_shader, "gamma")

    End Sub

End Module
