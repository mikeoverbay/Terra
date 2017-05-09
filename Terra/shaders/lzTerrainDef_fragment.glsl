﻿// For rendering low rez terrain

#version 330 compatibility 

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gPosition;
layout (location = 3) out vec4 gFlag;

uniform sampler2D colorMap;

in vec2 texCoord;
in vec3 n;
in vec4 Vertex;

void main (void)
{
  //discard;
  vec4 base = texture2D(colorMap,  texCoord);

  gColor = base;
  gNormal.xyz = normalize(n.xyz)*0.5+0.5;
  gNormal.w = 0.5;
  gPosition = Vertex;
  gFlag = vec4(64.0/255.0);
 
  }
