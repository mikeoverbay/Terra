//Leaf Vertex Shader
#version 330 compatibility

uniform mat4 matrix;

out vec2 texCoord;
out vec3 n;
out vec4 Vertex;
out mat3 TBN;
void main(void)
{



    vec4 p =  gl_ModelViewMatrix * gl_Vertex;
    p.xyz += gl_MultiTexCoord1.xyz;

    p = inverse(gl_ModelViewMatrix) * p;
    gl_Position = gl_ModelViewProjectionMatrix * p;

    Vertex =  matrix * gl_Vertex;
    Vertex.xyz  += gl_MultiTexCoord1.xyz;
    Vertex * gl_ModelViewMatrix * Vertex;
    texCoord    = gl_MultiTexCoord0.xy;
    

    n = normalize((mat3(matrix)) * gl_Normal);
    vec3 t = normalize((mat3(matrix)) * gl_MultiTexCoord2.xyz);
    vec3 b = normalize(cross(t,n));
    TBN = mat3(t,b,n);

}
