// used for shadow mapping
// This is not used yet.

varying vec4 v_position;
varying vec4 color;

void main()
{
    color       = gl_Color;
    gl_Position = ftransform();
    vec4 v = gl_Vertex;
    if (color.g == 1.0){
        v = gl_ModelViewMatrix * v;
        }
    v_position = v;
}