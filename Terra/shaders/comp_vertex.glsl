//comp_vertex.glsl
//phong shader.. no texture
varying vec3 N;
varying vec3 v;
uniform float amount;
void main(void)
{

	N = normalize( gl_Normal);
	vec4 v = gl_ModelViewProjectionMatrix * gl_Vertex;
	// bumb z out a little.
	v.z += amount ;
	gl_Position = v;

}