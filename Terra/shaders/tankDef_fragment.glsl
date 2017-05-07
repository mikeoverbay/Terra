// tankShader_fragment.txt
//used to render tanks

#version 330 compatibility

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gPosition;

in vec3 n;
in vec4 Vertex;
in vec4 color;
void main (void)
{
  //discard;

  gColor = color;
  gNormal.xyz = normalize(n.xyz)*0.5+0.5;
  gNormal.w = 0.25;
  gPosition = Vertex;
  
  }
