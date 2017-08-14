#version 330 compatibility

layout (location = 0) out vec4 gColor;

uniform sampler2D colorMap;
uniform float location;
uniform float scale;
in vec2 texcoord;
in vec4 mcolor;
in vec4 v;

void main(void){

float z = v.z/v.w;
if (z > 0.0) discard;

float l = gl_FragCoord.x -location;
float d = (l / 65.0);
float a = abs(cos(d));
gColor = texture2D(colorMap, texcoord);// + mcolor;
gColor.a = a;
}