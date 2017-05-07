//used to display the normals, tangents and biTangents

varying vec3 n;      
varying vec3 t;      
varying vec3 b;      
varying vec4 Vertex;
void main(void)
{
    gl_Position = gl_Vertex;
    n           = normalize(gl_Normal);
    t           = normalize(gl_MultiTexCoord2.xyz);
    b           = normalize(cross(t,n));
    t           = normalize(t-dot(n,t)*n);
    b           = normalize(b-dot(n,b)*n);
}