#version 330 compatibility

layout (location = 0) out vec4 gColor;

uniform sampler2D colorMap;
in vec2 texcoord;
in vec4 mcolor;
void main(void){

gColor = texture2D(colorMap, texcoord);

}