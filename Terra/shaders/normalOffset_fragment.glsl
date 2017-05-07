#version 330 compatibility
out vec4 fragColor;
uniform sampler2D normalMap;
in vec2 texcoord;

void main(void){
fragColor.xyz = normalize(texture2D(normalMap, texcoord).xyz*2.0-1.0);// shift to -1 to 1
fragColor.a = 1.0;
}