#version 130
//Used to light all models
out vec3 vVertex;
out vec3 lightDirection;
out vec2 TC1;
out vec2 TC2;
out mat3 TBN;

out vec3 t;
out vec3 b;
out vec3 n;
void main(void) {

    TC1 = gl_MultiTexCoord0.xy;
    TC2 = gl_MultiTexCoord1.xy;

    vec3 n = normalize(gl_NormalMatrix * gl_Normal);
    vec3 t = normalize(gl_NormalMatrix * gl_MultiTexCoord2.xyz);
    vec3 b = normalize(gl_NormalMatrix * gl_MultiTexCoord3.xyz);
    t = normalize(t - dot(n, t) * n);
    b = normalize(b - dot(n, b) * n);

    float invmax = inversesqrt(max(dot(t, t), dot(b, b)));
    TBN = mat3(t * invmax, b * invmax, n * invmax);

    vVertex = vec3(gl_ModelViewMatrix * gl_Vertex);

    lightDirection = gl_LightSource[0].position.xyz - vVertex;

    gl_Position    = ftransform();

}