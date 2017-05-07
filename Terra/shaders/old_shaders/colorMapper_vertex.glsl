// viewImage shader

varying vec2 texCoord;

void main(void)
{ 
    texCoord = gl_MultiTexCoord0;
    gl_Position = ftransform();     
}
