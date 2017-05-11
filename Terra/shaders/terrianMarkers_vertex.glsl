//Terrianchunks Markers.. 

#version 330 compatibility

out vec2 uv;
out vec4 Vertex;

void main(void)
{
    Vertex = gl_Vertex;
    Vertex.y+=0.01;
    gl_Position = gl_ModelViewProjectionMatrix * Vertex;
    uv = -gl_MultiTexCoord0.xy/10.0;
}
