//Fast Approximate Anti-Aliasing 

#version 120
varying vec2 texCoords;

void main(void)
{
  gl_Position = ftransform();
  texCoords.xy = gl_MultiTexCoord0.xy;
}
