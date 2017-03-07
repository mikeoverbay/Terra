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
        Public basicONEtexture_shader As Integer
        Public bump_shader As Integer
        Public clip_shader As Integer
        Public comp_shader As Integer
        Public decals_shader As Integer
        Public depth_shader As Integer
        Public fog_shader As Integer
        Public gaussian_shader As Integer
        Public hell_shader As Integer
        Public leaf_shader As Integer
        Public leafcolored_shader As Integer
        Public normal_shader As Integer
        Public render_shader As Integer
        Public renderTest_shader As Integer
        Public shadowDepth_shader As Integer
        Public shadowTest_shader As Integer
        Public ss_shader As Integer
        Public tank_shader As Integer
        Public toLinear_shader As Integer
        Public trees_shader As Integer
        Public write3D_shader As Integer
    End Class
    'Public bump_shader As Integer '0
    'Public decal_shader As Integer
    'Public tank_shader As Integer
    'Public surface_shader As Integer
    'Public comp_shader As Integer
    'Public hell_shader As Integer '5
    'Public render_shader As Integer
    'Public tree_shader As Integer
    'Public depth_shader As Integer
    'Public write3D_shader As Integer
    'Public gaussian_shader As Integer '10
    'Public leaf_shader As Integer
    'Public leaf_colored_shader As Integer
    'Public clip_shader As Integer
    'Public fog_shader As Integer
    'Public alpha_shader As Integer
    'Public normal_view_shader As Integer '16
    Public view_normal_mode_ As Integer
    Public normal_mode As Integer = 0
    Public normal_length_ As Integer
    Public c_address, n_address, f_address, a_address, t_address, c_position As Integer
    Public c_address2, n_address2, f_address2, a_address2, t_address2 As Integer
    Public c_address3, n_address3, f_address3, a_address3, t_address3 As Integer
    Public c_address4, n_address4, f_address4, a_address4, t_address4 As Integer
    Public c_address5, n_address5, f_address5, a_address5, t_address5 As Integer
    Public c_address6, n_address6, f_address6, a_address6, t_address6 As Integer
    Public layer_1, layer_2, layer_3, layer_4, n_layer_1, n_layer_2, n_layer_3, n_layer_4 As Integer
    Public used_layers, col_address, row_address, tile_w, main_texture, is_bumped, is_multi_textured, colormap2, gamma, gamma_2 As Integer
    Public gamma_3, gamma_4, gamma_5, gamma_6, c_position3, c_position4, c_position5, is_bumped3, is_bumped4, is_bumped5 As Integer
    Public layer0U, layer1U, layer2U, layer3U As Integer
    Public layer0V, layer1V, layer2V, layer3V As Integer
    Public gray_level_1, gray_level_2, gray_level_3, gray_level_6, gray_level_4, gray_level_5 As Integer
    Public is_GAmap, alphaRef, alphaTestEnable, tangent, binormal As Integer
    Public matrix_1 As Integer
    Public u_mat1, u_mat2, u_mat3 As Integer
    Public mixtexture, dominateTex As Integer
    Public sun_lock As Boolean = False
    Public leaf_c_map, leaf_n_map, leaf_level, leaf_contrast, leaf_camPos, leaf_matrix, leaf_gray_level, leaf_fog_enable As Integer
    Public leaf_ambient, branch_ambient, decal_ambient, model_ambient As Integer
    Public decal_u_wrap, decal_v_wrap, decal_influence As Integer
    Public phong_cam_pos As Integer
    Public bump_out_ As Integer
    Public vismap_address As Integer
    Public noise_map_address As Integer
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
                    'Dim radius As Integer = CallByName(car.chassis.wheel, "radius", Microsoft.VisualBasic.CallType.Get, Nothing)
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


        If shader > 0 Then
            Gl.glDeleteShader(shader)
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


        Dim vertexObject As Integer
        Dim geoObject As Integer
        Dim fragmentObject As Integer
        Dim status_code As Integer
        Dim info As New StringBuilder
        info.Length = 1024
        Dim info_l As Integer
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
            Gl.glGetShaderiv(geoObject, Gl.GL_COMPILE_STATUS, status_code)
            If Not status_code = Gl.GL_TRUE Then
                Gl.glDeleteShader(vertexObject)
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
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_VERTICES_OUT_EXT, 6)
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

        Gl.glAttachShader(shader, fragmentObject)
        If has_geo Then
            Gl.glAttachShader(shader, geoObject)
        End If
        Gl.glAttachShader(shader, vertexObject)

        Gl.glLinkProgram(shader)
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
        'I moved this out side of the main rendering loop.
        'Im hoping to speed up rendering as much as I can.
        c_address2 = Gl.glGetUniformLocation(shader_list.ss_shader, "colorMap")
        n_address2 = Gl.glGetUniformLocation(shader_list.ss_shader, "normalMap")
        f_address2 = Gl.glGetUniformLocation(shader_list.ss_shader, "enable_fog")
        a_address2 = Gl.glGetUniformLocation(shader_list.ss_shader, "l_ambient")
        t_address2 = Gl.glGetUniformLocation(shader_list.ss_shader, "l_texture")
        gray_level_2 = Gl.glGetUniformLocation(shader_list.ss_shader, "gray_level")
        gamma_2 = Gl.glGetUniformLocation(shader_list.ss_shader, "gamma")
        '--------------------------------------------------------------------------
        c_address6 = Gl.glGetUniformLocation(shader_list.clip_shader, "colorMap")
        'n_address6 = Gl.glGetUniformLocation(clip_shader, "normalMap")
        'f_address6 = Gl.glGetUniformLocation(clip_shader, "enable_fog")
        'a_address6 = Gl.glGetUniformLocation(clip_shader, "l_ambient")
        't_address6 = Gl.glGetUniformLocation(clip_shader, "l_texture")
        'gray_level_6 = Gl.glGetUniformLocation(clip_shader, "gray_level")
        'gamma_6 = Gl.glGetUniformLocation(clip_shader, "gamma")
        'clip_distance = Gl.glGetUniformLocation(clip_shader, "clip_distance")
        '--------------------------------------------------------------------------
        main_texture = Gl.glGetUniformLocation(shader_list.render_shader, "main_texture")
        n_address = Gl.glGetUniformLocation(shader_list.render_shader, "normalMap")
        c_position = Gl.glGetUniformLocation(shader_list.render_shader, "cam_position")
        f_address = Gl.glGetUniformLocation(shader_list.render_shader, "enable_fog")
        a_address = Gl.glGetUniformLocation(shader_list.render_shader, "l_ambient")
        t_address = Gl.glGetUniformLocation(shader_list.render_shader, "l_texture")
        gray_level_1 = Gl.glGetUniformLocation(shader_list.render_shader, "gray_level")
        gamma = Gl.glGetUniformLocation(shader_list.render_shader, "gamma")
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
        row_address = Gl.glGetUniformLocation(shader_list.render_shader, "row")
        col_address = Gl.glGetUniformLocation(shader_list.render_shader, "col")
        tile_w = Gl.glGetUniformLocation(shader_list.render_shader, "tile_width")
        used_layers = Gl.glGetUniformLocation(shader_list.render_shader, "used_layers")
        dominateTex = Gl.glGetUniformLocation(shader_list.render_shader, "DominateMap")
        layer0U = Gl.glGetUniformLocation(shader_list.render_shader, "layer0U")
        layer1U = Gl.glGetUniformLocation(shader_list.render_shader, "layer1U")
        layer2U = Gl.glGetUniformLocation(shader_list.render_shader, "layer2U")
        layer3U = Gl.glGetUniformLocation(shader_list.render_shader, "layer3U")
        layer0V = Gl.glGetUniformLocation(shader_list.render_shader, "layer0V")
        layer1V = Gl.glGetUniformLocation(shader_list.render_shader, "layer1V")
        layer2V = Gl.glGetUniformLocation(shader_list.render_shader, "layer2V")
        layer3V = Gl.glGetUniformLocation(shader_list.render_shader, "layer3V")
        '--------------------------------------------------------------------------
        c_address3 = Gl.glGetUniformLocation(shader_list.bump_shader, "colorMap")
        colormap2 = Gl.glGetUniformLocation(shader_list.bump_shader, "colorMap_2")
        n_address3 = Gl.glGetUniformLocation(shader_list.bump_shader, "normalMap")
        f_address3 = Gl.glGetUniformLocation(shader_list.bump_shader, "enable_fog")
        'c_position3 = Gl.glGetUniformLocation(bump_shader, "camPos")
        t_address3 = Gl.glGetUniformLocation(shader_list.bump_shader, "l_texture")
        gray_level_3 = Gl.glGetUniformLocation(shader_list.bump_shader, "gray_level")
        is_bumped3 = Gl.glGetUniformLocation(shader_list.bump_shader, "is_bumped")
        is_GAmap = Gl.glGetUniformLocation(shader_list.bump_shader, "is_GAmap")
        is_multi_textured = Gl.glGetUniformLocation(shader_list.bump_shader, "is_multi_textured")
        gamma_3 = Gl.glGetUniformLocation(shader_list.bump_shader, "gamma")
        alphaRef = Gl.glGetUniformLocation(shader_list.bump_shader, "alphaRef")
        alphaTestEnable = Gl.glGetUniformLocation(shader_list.bump_shader, "alphaTestEnable")
        u_mat3 = Gl.glGetUniformLocation(shader_list.bump_shader, "ModelMatrix1")
        model_ambient = Gl.glGetUniformLocation(shader_list.bump_shader, "ambient")
        '--------------------------------------------------------------------------
        c_address4 = Gl.glGetUniformLocation(shader_list.trees_shader, "colorMap")
        n_address4 = Gl.glGetUniformLocation(shader_list.trees_shader, "normalMap")
        f_address4 = Gl.glGetUniformLocation(shader_list.trees_shader, "enable_fog")
        c_position4 = Gl.glGetUniformLocation(shader_list.trees_shader, "cam_position")
        t_address4 = Gl.glGetUniformLocation(shader_list.trees_shader, "l_texture")
        gray_level_4 = Gl.glGetUniformLocation(shader_list.trees_shader, "gray_level")
        is_bumped4 = Gl.glGetUniformLocation(shader_list.trees_shader, "is_bumped")
        gamma_4 = Gl.glGetUniformLocation(shader_list.trees_shader, "gamma")
        branch_ambient = Gl.glGetUniformLocation(shader_list.trees_shader, "ambient")
        '--------------------------------------------------------------------------
        c_address5 = Gl.glGetUniformLocation(shader_list.decals_shader, "colorMap")
        n_address5 = Gl.glGetUniformLocation(shader_list.decals_shader, "normalMap")
        f_address5 = Gl.glGetUniformLocation(shader_list.decals_shader, "enable_fog")
        c_position5 = Gl.glGetUniformLocation(shader_list.decals_shader, "cam_position")
        t_address5 = Gl.glGetUniformLocation(shader_list.decals_shader, "l_texture")
        gray_level_5 = Gl.glGetUniformLocation(shader_list.decals_shader, "gray_level")
        gamma_5 = Gl.glGetUniformLocation(shader_list.decals_shader, "gamma")
        matrix_1 = Gl.glGetUniformLocation(shader_list.decals_shader, "ModelMatrix1")
        decal_ambient = Gl.glGetUniformLocation(shader_list.decals_shader, "ambient")
        decal_u_wrap = Gl.glGetUniformLocation(shader_list.decals_shader, "u_wrap")
        decal_v_wrap = Gl.glGetUniformLocation(shader_list.decals_shader, "v_wrap")
        decal_influence = Gl.glGetUniformLocation(shader_list.decals_shader, "influence")
        '-----------------------------------------------------------------
        leaf_c_map = Gl.glGetUniformLocation(shader_list.leaf_shader, "colorMap")
        leaf_camPos = Gl.glGetUniformLocation(shader_list.leaf_shader, "camPos")
        leaf_matrix = Gl.glGetUniformLocation(shader_list.leaf_shader, "matrix")
        leaf_fog_enable = Gl.glGetUniformLocation(shader_list.leaf_shader, "enable_fog")
        leaf_camPos = Gl.glGetUniformLocation(shader_list.leaf_shader, "cam_position")
        leaf_level = Gl.glGetUniformLocation(shader_list.leaf_shader, "l_texture")
        leaf_gray_level = Gl.glGetUniformLocation(shader_list.leaf_shader, "gray_level")
        leaf_contrast = Gl.glGetUniformLocation(shader_list.leaf_shader, "gamma")
        leaf_ambient = Gl.glGetUniformLocation(shader_list.leaf_shader, "ambient")
        '-----------------
        phong_cam_pos = Gl.glGetUniformLocation(shader_list.comp_shader, "cam_pos")
        bump_out_ = Gl.glGetUniformLocation(shader_list.comp_shader, "amount")
        '-----------------
        vismap_address = Gl.glGetUniformLocation(shader_list.fog_shader, "map")
        noise_map_address = Gl.glGetUniformLocation(shader_list.fog_shader, "noise_map")
        '-----------------
        view_normal_mode_ = Gl.glGetUniformLocation(shader_list.normal_shader, "mode")
        normal_length_ = Gl.glGetUniformLocation(shader_list.normal_shader, "l_length")
    End Sub

End Module
