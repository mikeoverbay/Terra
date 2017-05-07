//comp_vertex.glsl
//used for shading untextured models, trees, terrain and decals
//phong shader.. no texture
varying vec3 N;
varying vec4 color;
varying vec4 world_vertex;
uniform float amount;
void main(void)
{
    color = gl_Color;
    N = gl_NormalMatrix * gl_Normal;
 
    world_vertex = gl_ModelViewMatrix * gl_Vertex;
    
    // bumb z out a little.
    vec4 v =  gl_ModelViewProjectionMatrix * gl_Vertex;
    v.z -= amount ;
    v.y += amount ;
    gl_Position =  v;     

}