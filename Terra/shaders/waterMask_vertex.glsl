//water shader

#version 330 compatibility

void main(void)
{
    gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
    
}
