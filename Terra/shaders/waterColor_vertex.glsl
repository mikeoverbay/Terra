//water shader

#version 330 compatibility

uniform mat4 matrix;

out vec4 positionSS;
out mat4 matPrjInv;
out mat4 invd_mat;
out vec4 color;
void main(void)
{
    gl_Position = matrix * gl_Vertex;
    gl_Position = gl_ModelViewProjectionMatrix * gl_Position;
    positionSS = gl_Position;

    matPrjInv = inverse(gl_ModelViewProjectionMatrix);
    invd_mat = inverse(matrix);
    color = gl_Color;
}
