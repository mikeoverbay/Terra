//water shader

#version 330 compatibility

layout (location = 0) out vec4 gPosition;
layout (location = 1) out float gFlag;

in vec4 Vertex;

void main(void)
{
    gFlag = 160.0/255.0;
    gPosition = Vertex;
}
