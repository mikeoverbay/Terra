// Deferred final lighitng shader

#version 120
varying vec2 TexCoords;
void main()
{
    gl_Position = ftransform();
    TexCoords.xy = gl_MultiTexCoord0.xy;

}
