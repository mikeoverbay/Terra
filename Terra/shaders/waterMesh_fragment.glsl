#version 330 compatibility

layout (location = 0) out vec4 FragColor;

uniform sampler2D colorMap;

uniform float time;
uniform float aspect;
uniform float shift;

in vec2 tc;
in vec4 color;

void main(void){

FragColor.rgb = color.rgb * texture2D(colorMap, tc).rgb;
FragColor.a = 1.0;
}