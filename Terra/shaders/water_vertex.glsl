//water shader

#version 330 compatibility

uniform mat4 matrix;

out vec4 positionSS;
out mat4 matPrjInv;
out mat3 TBN;
out vec3 n;
out mat4 invd_mat;
out vec4 color;
void main(void)
{
    gl_Position = matrix * gl_Vertex;
    gl_Position = gl_ModelViewProjectionMatrix * gl_Position;
    positionSS = gl_Position;

    matPrjInv = inverse(gl_ModelViewProjectionMatrix);
    invd_mat = inverse(matrix);
    
    n = vec3(0.0, 1.0, 0.0);
    vec3 t = vec3 (1.0, 0.0, 0.0);
    t = normalize(inverse(mat3(matrix)) * t);
    vec3 b = cross(t,n);
    TBN = mat3(t,b,n);
    color = gl_Color;
}
