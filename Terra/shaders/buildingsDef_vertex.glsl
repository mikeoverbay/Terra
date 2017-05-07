//Used to render all buildings and such.

#version 330 compatibility

uniform mat4 matrix;
out vec4 Vertex;
out vec2 TC1;
out vec2 TC2;
out mat3 TBN;
out vec3 n;
void main(void) {

    TC1 = gl_MultiTexCoord0.xy;
    TC2 = gl_MultiTexCoord1.xy;

    n = normalize((mat3(matrix)) * gl_Normal);

    vec3 t = normalize((mat3(matrix)) * gl_MultiTexCoord2.xyz);
    vec3 b = normalize(cross(t,n));
    TBN = mat3(t,b,n);

    gl_Position = matrix * gl_Vertex;
    gl_Position = gl_ModelViewProjectionMatrix * gl_Position;
    Vertex = matrix * gl_Vertex;
}