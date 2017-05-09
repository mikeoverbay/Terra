//Decals color pass.

#version 330 compatibility 

out mat4 matPrjInv;
out vec4 positionSS;
out vec4 positionWS;
out mat4 invd_mat;

uniform mat4 decal_matrix;

void main(void)
{
    gl_Position = decal_matrix * gl_Vertex;
    gl_Position = gl_ModelViewProjectionMatrix * gl_Position;

    positionWS =  (decal_matrix * gl_Vertex);;
    positionSS = gl_Position;
    matPrjInv = inverse(gl_ModelViewProjectionMatrix);

    invd_mat = inverse(decal_matrix);
}
