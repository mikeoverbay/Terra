// used for SSAO
// I cant get SSAO working so this is never used!
#version 330 compatibility

uniform vec2 screen_size; // current screen size width
uniform float FOV = 60.0;
uniform mat4 prj_matrix; //g_buffer Fill projection matrix

out mat4 Prjmatrix;
out mat4 u_inverseProjectionMatrix;
out mat4 u_projectionMatrix;
out vec2 v_texCoord;
void main()
{

    v_texCoord = gl_MultiTexCoord0.xy;
    gl_Position = ftransform();
    u_projectionMatrix = gl_ModelViewProjectionMatrix;
    u_inverseProjectionMatrix = inverse(gl_ModelViewProjectionMatrix);
}