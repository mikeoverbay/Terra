//Leaf Vertex solid color Shader
#version 330 compatibility 


out vec3 N;
out vec4 Vertex;
out vec4 color;

void main(void)
{
    vec4 p =  gl_ModelViewMatrix * gl_Vertex;
    p.xyz += gl_MultiTexCoord1.xyz;
    p           = inverse(gl_ModelViewMatrix) * p;
    Vertex = p;
    gl_Position = gl_ModelViewProjectionMatrix * p; 

    
    //normal  = gl_NormalMatrix * gl_Normal;
    N.xyz = gl_Normal;
    N.x*= -1.0;
    color = gl_Color;
}
