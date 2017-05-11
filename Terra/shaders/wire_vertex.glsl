//wire shader.. very basic

#version 330 compatibility

out vec4 color;

void main(void)
{
    color = gl_Color;    
    gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
}
