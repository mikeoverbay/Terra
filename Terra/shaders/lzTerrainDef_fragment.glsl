// For rendering low rez terrain

#version 330 compatibility 

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gPosition;
layout (location = 3) out float gFlag;
uniform sampler2D colorMap;
uniform sampler2D hole_texture;
uniform int has_holes;
in vec2 texCoord;
in vec3 n;
in vec4 Vertex;
flat in float hole;
void main (void)
{
// lets check for a hole before doing any math.. It saves time
  if (has_holes == 1.0)
    {
        if (hole > 0.0) discard;
    }
    vec4 base = texture2D(colorMap,  texCoord);
    gColor = base;
    gColor = vec4(0.50,0.50,0.25, 1.0);
    gNormal.xyz = normalize(n.xyz)*0.5+0.5;
    gNormal.w = 0.5;
    gPosition = Vertex;
    gFlag = (64.0/255.0);
}


