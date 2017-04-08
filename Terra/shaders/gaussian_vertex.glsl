//Used to blur mixmap and main color map

varying vec2 v_texCoord;

void main()
{
	gl_Position = ftransform();		
	v_texCoord = gl_MultiTexCoord0.xy;

}