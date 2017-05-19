//water shader

#version 330 compatibility
uniform mat4 matrix;
out vec4 Vertex;
void main(void)
{
    gl_Position = matrix * gl_Vertex;
    gl_Position = gl_ModelViewProjectionMatrix * gl_Position;
    Vertex =  matrix * gl_Vertex;
}
