#version 330 compatibility

out vec2 texcoord;
out vec4 mcolor;
out vec4 v;
void main(void)
{
  gl_Position = ftransform();
  v = gl_Position;
  texcoord = gl_MultiTexCoord0.xy;
  mcolor = gl_Color;
}
