// used for shadow mapping
// This is not used yet.

varying vec4 v;
varying vec2 texCoords;
void main()
{
    gl_Position = ftransform();
    v           = gl_ModelViewMatrix * gl_Vertex;
    texCoords   = gl_MultiTexCoord0.xy;
}