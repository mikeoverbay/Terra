//Leaf Vertex Shader
#version 330 compatibility 

out vec2 texCoord;
out vec3 norm;
out vec3 g_vertexnormal;
out vec3 tangent;
out vec3 bitangent;
out vec3 g_viewvector;
out vec3 lightDirection;
uniform vec3 cam_position;
out mat3 TBN;
out vec4 vVertex;
void main(void)
{

 

    texCoord.xy = gl_MultiTexCoord0.st;
    vec4 p =  gl_ModelViewMatrix * gl_Vertex;
    p.xyz += gl_MultiTexCoord1.xyz;
    p           = inverse(gl_ModelViewMatrix) * p;
    gl_Position = gl_ModelViewProjectionMatrix * p;

    
    g_vertexnormal = normalize (gl_NormalMatrix * gl_Normal);
    //g_vertexnormal.xz*= -1.0;
    tangent        = normalize (gl_MultiTexCoord2.xyz);
    bitangent      = normalize( cross(tangent,gl_NormalMatrix * gl_Normal) );
    float invmax = inversesqrt( max( dot(tangent,tangent), dot(bitangent,bitangent) ));
    mat3 TBN = mat3( tangent * invmax, bitangent * invmax, g_vertexnormal );

    lightDirection = gl_LightSource[0].position.xyz ;

    vVertex      = vec4( gl_Vertex);
    g_viewvector = lightDirection - gl_Vertex.xyz;
    //g_viewvector.z *= -1.0;    

}
