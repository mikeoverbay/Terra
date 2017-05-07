//Leaf Vertex solid color Shader
#version 330 compatibility 


out vec3 N;
out vec3 normal, eyeVec;
out vec3 lightDir;
void main(void)
{
    vec4 p =  gl_ModelViewMatrix * gl_Vertex;
    p.xyz += gl_MultiTexCoord1.xyz;
    p           = inverse(gl_ModelViewMatrix) * p;
    gl_Position = gl_ModelViewProjectionMatrix * p; 

    
    //normal  = gl_NormalMatrix * gl_Normal;
    normal.xyz = gl_Normal;
    normal.x*= -1.0;
     eyeVec   = gl_Vertex.xyz;
     lightDir = gl_LightSource[0].position.xyz;// - vVertex.xyz);

  
}
