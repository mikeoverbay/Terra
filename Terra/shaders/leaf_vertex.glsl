//Leaf Vertex Shader
#version 330 compatibility 

out vec2 texCoord;
out vec3 norm;
out float ln;
out vec3 g_vertexnormal;
out vec3 tangent;
out vec3 bitangent;
out vec3 g_viewvector;
out vec3 lightDirection;
uniform vec3 cam_position;

void main(void)
{

 

    texCoord.xy = gl_MultiTexCoord0.st;
    vec4 p =  gl_ModelViewMatrix * gl_Vertex;
    p.xyz += gl_MultiTexCoord1.xyz;
    p           = inverse(gl_ModelViewMatrix) * p;
    gl_Position = gl_ModelViewProjectionMatrix * p;

    
    g_vertexnormal = normalize (gl_NormalMatrix * gl_Normal);
    tangent        = normalize (gl_NormalMatrix * gl_MultiTexCoord2.xyz);
    bitangent      = normalize( cross(tangent,gl_NormalMatrix * gl_Normal) );

    vec3 vVertex = vec3(gl_ModelViewMatrix * gl_Vertex);
    g_viewvector = cam_position - -vVertex;
    g_viewvector.z *= -1.0;    
    lightDirection = gl_LightSource[0].position.xyz - vVertex;


    vec3 camPos = vec3 (gl_ModelViewMatrix * vec4(cam_position,1.0));
    camPos = normalize(cam_position);
    vec3 point = vec3(gl_Vertex.xzy);
    // This is the cut off distance for bumpping the surface.
    ln = distance(vVertex ,camPos);
    if (ln<200.0)
{
        ln = 1.0-ln/200.0;
    }


    else
    {
        ln = 0.0;
    }


}
