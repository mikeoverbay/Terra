// For final render of SSAO to screen;

#version 330 compatibility 

out vec2 texCoords;

void main(void)
{
    texCoords    = gl_MultiTexCoord0.xy;
    gl_Position = ftransform();

}
