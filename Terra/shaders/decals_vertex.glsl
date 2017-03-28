// decals_vertex.txt
// used to render decals
//#version 130

varying vec2 texCoord;

varying float ln;
varying vec3 g_vertexnormal;
varying vec3 g_viewvector;
varying vec3 lightDirection;
varying mat3 TBN;
uniform mat4 ModelMatrix1;
uniform vec3 cam_position;
////////////////////////////

void main(void)
{
    vec3 n = gl_Normal;
    // some decals are upside down
     if (n.y < 0.0) {
        n *= -1.0;
    }
    vec3 t = cross(n,vec3(-1.0,0.0,0.0));
    vec3 b = cross(t,n);
    t   = normalize(t-dot(t,n)*n);
    b   = normalize(b-dot(b,n)*n);
    TBN = mat3(t,b,n);
    
    vec3 lpos = vec3(inverse(gl_ModelViewMatrix) * vec4(gl_LightSource[0].position.xyz,1.0));

    g_vertexnormal = n;
    lightDirection = lpos - gl_Vertex.xyz;

    g_viewvector = cam_position - gl_Vertex.xyz;

    //////////////////////////////////////////////
    vec4 v = gl_ModelViewProjectionMatrix * gl_Vertex;
    
    texCoord    = gl_MultiTexCoord0.xy;
    
    vec3 camPos = normalize(cam_position);
    vec3 point = vec3(gl_Vertex.xyz);

    // This is the cut off distance for bumpping the surface.
    vec3 camera = vec3( vec4(cam_position, 1.0) );
    ln = distance(point.xyz ,cam_position);
    if (ln<600.0)
{
    ln = cos(3.14159*(ln/600.0) );
    }

    else
    {
        ln = 0.0;
    }
    // bumb z,y out a little so the underlaying terrain dont peek thru.
    vec3 ns = gl_NormalMatrix * gl_Normal;
    v.z -= .1 * ns.z;
    v.y += .1 * ns.y;
    gl_Position = v;

}
