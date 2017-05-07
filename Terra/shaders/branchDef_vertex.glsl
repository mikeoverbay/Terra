// trees_vertex.txt
// used to render trees
#version 330 compatibility

uniform mat4 matrix;

out vec2 texCoord;
out vec3 n;
out vec4 Vertex;
out mat3 TBN;
void main(void)
{
    texCoord    = gl_MultiTexCoord0.xy;

    n = normalize((mat3(matrix)) * gl_Normal);
    vec3 t = normalize((mat3(matrix)) * gl_MultiTexCoord2.xyz);
    vec3 b = normalize(cross(t,n));
    TBN = mat3(t,b,n);

    gl_Position = matrix * gl_Vertex;
    gl_Position = gl_ModelViewProjectionMatrix * gl_Position;
    Vertex = matrix * gl_Vertex;
}
