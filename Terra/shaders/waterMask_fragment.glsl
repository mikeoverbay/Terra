//water shader

#version 330 compatibility
layout (location = 0) out vec4 gFlag;

uniform sampler2D colorMap;
void main(void)
{
    gFlag = vec4((160)/255.0);
    
}
