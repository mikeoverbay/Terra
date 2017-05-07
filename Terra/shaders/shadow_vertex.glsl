//this is unusable as I cant figure out a way to do the transforms correctly.
//At some point I may look back in to this.
#version 330 compatibility

out vec4 color;
out vec4 v;
uniform vec3 light_position;
uniform mat4 cam_matrix;
void main()
{
 v = cam_matrix * gl_Vertex;
    gl_Position =  gl_ModelViewMatrix * gl_Vertex;
}