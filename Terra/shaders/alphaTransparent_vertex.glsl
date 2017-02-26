// discards base on alpha level in texture

varying vec2 texCoords;

void main()
{
    gl_Position = ftransform();
   texCoords = gl_MultiTexCoord0.xy;
 }