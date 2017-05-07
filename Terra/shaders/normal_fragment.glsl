#version 330 compatibility

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gPosition;

in vec4 color;
in vec4 n;
in vec4 Vertex;
void main()
{
  gColor = color;
  gNormal = vec4(0.0,0.0,0.0,0.0);
  gPosition = Vertex;
}