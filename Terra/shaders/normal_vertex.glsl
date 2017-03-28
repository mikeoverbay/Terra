//used to display the normals, tangents and biTangents

varying vec3 n;      
varying vec3 t;      
varying vec3 b;      
 
void main(void)
{
    gl_Position = gl_Vertex;
    n           = normalize(gl_Normal);
    t           = normalize(gl_MultiTexCoord2.xyz);
    b           = normalize(gl_MultiTexCoord3.xyz);
    t           = normalize(t-dot(n,t)*n);
    b           = normalize(b-dot(n,b)*n);
}