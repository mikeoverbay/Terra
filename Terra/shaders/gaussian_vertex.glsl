// used for blurring textures
// This is pure beta testing.
// gaussian_H_vertex

varying vec2 v_texCoord;

void main()
{
	gl_Position = ftransform();		
	v_texCoord = gl_MultiTexCoord0.xy;

}