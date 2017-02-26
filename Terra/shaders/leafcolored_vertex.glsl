//Leaf Vertex Shader
#version 330 compatibility 

out vec4 color;

void main(void)
{
    vec4 p =  gl_ModelViewMatrix * gl_Vertex;
    p.xyz += gl_MultiTexCoord1.xyz;
    p           = inverse(gl_ModelViewMatrix) * p;
    gl_Position = gl_ModelViewProjectionMatrix * p;	
	color = gl_Color;
}
