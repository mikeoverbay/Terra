// basicONEtexture_vertex.glsl
//use for rendering a single texture

varying vec2 texCoord;
void main(void)
{ 
        gl_Position = ftransform();     
        texCoord    = gl_MultiTexCoord0.xy;
}