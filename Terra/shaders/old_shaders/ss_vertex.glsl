// ss_vertext.txt
//used to render low rez terrain

varying vec2 texCoord;
uniform sampler2D normalMap;
uniform sampler2D colorMap;
varying vec3 norm;
varying vec3 Vertex;
void main(void)
{ 
vec3 tangent;
        gl_Position = ftransform();     
        texCoord = gl_MultiTexCoord0.xy/1.001;
norm.zx = ( texture2D(normalMap, -texCoord*.1).gb *2.0-1.0);
norm.y  = sqrt(1.0-((norm.x*norm.x)+(norm.z*norm.z))*0.5 );
norm    = normalize(norm);
norm.xz *= -1.0;
Vertex = vec3(  gl_Vertex);
}