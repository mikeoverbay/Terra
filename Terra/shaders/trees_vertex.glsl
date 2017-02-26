// trees_vertex.txt
// used to render trees
#version 330 compatibility 

out vec3 lightVec;
out vec3 eyeVec;
out vec2 texCoord;
out vec3 norm,halfVector;
out float ln;
uniform vec3 cam_position;

void main(void)
{
    texCoord = gl_MultiTexCoord0.xy;
    gl_Position = ftransform();

    vec3 vVertex = vec3(gl_ModelViewMatrix * gl_Vertex);
    norm = normalize(gl_NormalMatrix * gl_Normal);
    vec3 lightDir =  gl_LightSource[0].position.xyz;
    halfVector = normalize(vec3(gl_LightSource[0].halfVector.xyz));
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
