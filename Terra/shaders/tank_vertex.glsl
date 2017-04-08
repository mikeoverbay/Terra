// tankShader_fragment.txt
//used to render tanks

varying vec3 normal, eyeVec;
varying vec3 lightDir;
void main()
{
    gl_Position = ftransform();		
    normal = gl_NormalMatrix * gl_Normal;
    vec4 vVertex = gl_ModelViewMatrix * gl_Vertex;
    eyeVec = -vVertex.xyz;
    lightDir =  vec3(gl_LightSource[0].position.xyz - vVertex.xyz);
}
