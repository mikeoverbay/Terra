//Leaf Vertex solid color Shader
#version 330 compatibility 


uniform mat4 matrix;

out vec3 N;
out vec4 Vertex;
out vec4 color;

void main(void)
{
    vec4 p =  gl_ModelViewMatrix * gl_Vertex;
    p.xyz += gl_MultiTexCoord1.xyz;

    p = inverse(gl_ModelViewMatrix) * p;
    gl_Position = gl_ModelViewProjectionMatrix * p;

    Vertex =  matrix * gl_Vertex;
    Vertex.xyz  += gl_MultiTexCoord1.xyz;
    Vertex * gl_ModelViewMatrix * Vertex;

    
    //normal  = gl_NormalMatrix * gl_Normal;
    N = normalize((mat3(matrix)) * gl_Normal);
    color = gl_Color;
}
