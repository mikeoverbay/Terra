// tankShader_fragment.txt
//used to render tanks
#version 330 compatibility
uniform mat4 matrix;
out vec3 n;
out vec4 Vertex;
out vec4 color;
void main()
{
    color = gl_Color;
    gl_Position = matrix * gl_Vertex;
    gl_Position = gl_ModelViewProjectionMatrix * gl_Position;
    n = (mat3(matrix)) * gl_Normal;
    n.xz*=-1.0;
    Vertex =  (matrix) * gl_Vertex;
}
