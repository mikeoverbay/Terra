// used for SSAO
// I cant get SSAO working so this is never used!
#version 330 compatibility

uniform vec2 screen_size; // current screen size width
uniform float FOV = 60.0;
uniform mat4 prj_matrix; //g_buffer Fill projection matrix

out mat4 Prjmatrix;
out vec2 TexCoord;
out vec2 ViewRay;
void main()
{

    float aspect = screen_size.x / screen_size.y;
    float tanHalfFov = tan((FOV/2.0) * .017453293);

    TexCoord = (gl_Vertex.xy + vec2(1.0)) / 2.0;
    gl_Position = ftransform();
    Prjmatrix = gl_ModelViewProjectionMatrix;
    ViewRay.x = gl_Vertex.x * aspect * tanHalfFov;
    ViewRay.y = gl_Vertex.y * tanHalfFov;
}