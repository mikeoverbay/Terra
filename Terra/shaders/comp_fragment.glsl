//comp_fragment.glsl
//used for shading untextured models, trees, terrain and decals
//phong shader.. no texture
#version 330 compatibility

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gPosition;

in vec3 N;
in vec4 world_vertex;
in vec4 color;
void main (void)  
{  
    gColor = color*2.0;
    gNormal = vec4(normalize(N),0.25);
    gPosition = world_vertex;
}
    