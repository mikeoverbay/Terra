//comp_vertex.glsl
//used for shading untextured models, trees, terrain and decals
//phong shader.. no texture
varying vec3 N;
varying vec3 normal, eyeVec;
varying vec3 lightDir;
uniform float amount;
void main(void)
{
    normal = gl_NormalMatrix * gl_Normal;
 
     vec4 vVertex = gl_ModelViewMatrix * gl_Vertex;
     eyeVec   = -vVertex.xyz;
     lightDir = vec3(gl_LightSource[0].position.xyz - vVertex.xyz);
  // bumb z out a little.
     vec4 v =  gl_Vertex;
     v.z += amount ;
     v.y -= amount ;
     gl_Position = gl_ModelViewProjectionMatrix * v;     

}