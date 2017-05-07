#version 330 compatibility

out vec2 texcoord;
out vec4 mcolor;
void main(void)
{
  gl_Position = ftransform();
  texcoord = gl_MultiTexCoord0.xy;
  mcolor = gl_Color;
}
