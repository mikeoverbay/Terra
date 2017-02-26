// hell_vertext.txt

varying vec3 lightVec; 
varying vec3 eyeVec;
varying vec2 texCoord;
uniform sampler2D normalMap;
uniform sampler2D colorMap;
varying vec3 norm;
varying vec3 lightPos;
varying vec3 v_pos;


void main(void)
{ 
vec3 tangent;
	float nx;
	float ny;
	float nz;
		gl_Position = gl_ModelViewProjectionMatrix* gl_Vertex;	

		texCoord = gl_MultiTexCoord0.xy/1.001;
norm.xz = ( texture2D(normalMap, -texCoord*.1).gb *2.0-1.0);
norm.y = texture2D(normalMap, -texCoord*.1).r;
norm.y = sqrt(1.0-((norm.x*norm.x)+(norm.y*norm.y))*0.1 );
norm = normalize(norm);
vec3 tn;
tn.z = norm.x;
tn.y = norm.y;
tn.x = -norm.z;
norm = tn;


vec3 c1 = cross( norm, vec3(0.0, 0.0, 1.0) ); 
vec3 c2 = cross( norm, vec3(0.0, 1.0, 0.0) ); 
if( length(c1)>length(c2) )
{
	tangent = c1;	
}
else
{
	tangent = c2;	
}
	
	vec3 n = normalize( norm);
	vec3 t = normalize( tangent);
	vec3 b = cross(n, t);
	
	vec3 vVertex = vec3( gl_Vertex);
	v_pos.xz = vVertex.xz;



	vec3 tmpVec = gl_LightSource[0].position.xyz - vVertex;
	lightPos.xz = gl_LightSource[0].position.xz ;
	lightVec.x = dot(tmpVec, t);
	lightVec.z = -dot(tmpVec, b);
	lightVec.y = dot(tmpVec, n);

	tmpVec = -vVertex;
	eyeVec.x = dot(tmpVec, t);
	eyeVec.y = dot(tmpVec, b);
	eyeVec.z = dot(tmpVec, n);
float d = distance(lightPos , v_pos);
float d2;
	if (d < 1500.0)
	{
	d2 = d/1500.0;
	d = cos(3.1415927*2.0*((d/1500.0)));
	}
	else
	{
	d=0.0;
	d2=0.0;
	}
//vVertex.xyz += (d*100)*(1.0-d2);
vVertex.y += (d*75.0)*(1.0-d2)-50.;
	
		gl_Position = gl_ModelViewProjectionMatrix* vec4(vVertex,gl_Vertex.w);	

}