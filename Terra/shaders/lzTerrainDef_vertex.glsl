// For rendering low rez terrain

#version 330 compatibility 

out vec2 texCoord;
out vec3 n;
out vec4 Vertex;

void main(void)
{
    texCoord    = -gl_MultiTexCoord0.xy/10.0;
    gl_Position = ftransform();

    n = gl_NormalMatrix * gl_Normal;
    Vertex = gl_ModelViewMatrix * gl_Vertex;

}
