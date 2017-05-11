//water shader.. very basic
#version 330 compatibility

uniform vec3 ring_center;
uniform float thickness;
out mat4 matPrjInv;
out vec4 positionSS;
out vec4 positionWS;
out vec4 color;

void main(void)
{
    color = gl_Color;
    vec4 local = gl_Vertex;
    local.xyz  += ring_center.xyz;
    
    gl_Position = gl_ModelViewProjectionMatrix * local;
    positionSS = gl_Position;

    positionWS = local;
    matPrjInv = inverse(gl_ModelViewProjectionMatrix);

}
