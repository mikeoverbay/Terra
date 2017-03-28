// basicONEtexture_vertex.glsl
//use for rendering a single texture
#version 330 compatibility 

out vec2 texCoord;
out mat3 TBN;
out vec3 L;


void main(void)
{ 
    gl_Position = ftransform();     
    texCoord    = gl_MultiTexCoord0.xy;
    vec4 vVertex = gl_ModelViewMatrix * gl_Vertex;
    L = gl_LightSource[0].position.xyz - vVertex.xyz;
    
    vec3 n = normalize(gl_NormalMatrix * gl_Normal);
    vec3 t = cross(n,vec3(-1.0,0.0,0.0));
    vec3 b = cross(t,n);
    
    TBN = mat3(t,b,n);

        
}