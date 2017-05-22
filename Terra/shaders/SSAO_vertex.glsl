// used for SSAO
// I cant get SSAO working all that great!
#version 330 compatibility

out vec2 v_texCoord;

void main()
{

    v_texCoord = gl_MultiTexCoord0.xy;
    gl_Position = ftransform();

}