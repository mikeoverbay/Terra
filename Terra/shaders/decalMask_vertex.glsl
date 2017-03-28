// 
// used to render water
// It was a good idea that didnt work!

varying vec2 texCoord;
void main(void)
{ 
        gl_Position = ftransform();     
        texCoord    = gl_MultiTexCoord0.xy;
}