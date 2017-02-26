// decals_vertex.txt
// used to render decals
//#version 130

out vec2 texCoord;

out float ln;
out vec3 g_vertexnormal;
out vec3 g_viewvector;
out vec3 lightDirection;
uniform mat4 ModelMatrix1;
uniform vec3 cam_position;
////////////////////////////

void main(void)
{
    vec3 n = gl_Normal;
    // some decals are upside down
     if (n.y < 0.0) {
         //n *= -1.0;
    }   
    g_vertexnormal = n;
    lightDirection = gl_LightSource[0].position.xyz - gl_Vertex;
    //lightDirection.z*= -1.0;
    g_viewvector   = cam_position - gl_Vertex.xyz;
    //g_viewvector.x *= -1.0;
    //////////////////////////////////////////////
    // bumb z out a little so the underlaying terrain dont peek thru.
    vec4 v = gl_ModelViewProjectionMatrix * gl_Vertex;
    
    texCoord    = gl_MultiTexCoord0.xy;
    
    vec3 camPos = normalize(cam_position);
    vec3 point = vec3(gl_Vertex.xyz);

    // This is the cut off distance for bumpping the surface.
    vec3 camera = vec3( vec4(cam_position, 1.0) );
    ln = distance(point.xyz ,cam_position);
    if (ln<800.0)
{
    ln = cos(3.14159*(ln/800.0) );
    }

    else
    {
        ln = 0.0;
    }
    //comp offset by distance
    v.z -= .01;
    gl_Position = v;

}
