#version 330 compatibility 
#extension GL_EXT_geometry_shader4 : enable         
 
 out vec2 texCoord;
out vec4 color;
uniform mat4 matrix;
layout(points) in;
layout(triangle_strip, max_vertices = 4) out;
//in vec3 camPos;
void main(void)
{
    vec4 vVertex;
    // = gl_in[0].gl_Position;
    vec4 oVertex;
    vec2 uvs[4];
    uvs[0] = gl_TexCoord[0].st;
    uvs[1] = gl_TexCoord[1].st;
    uvs[2] = gl_TexCoord[2].st;
    uvs[3] = gl_TexCoord[3].st;
    vec2 scale = gl_TexCoord[4].st;
    //float vx[4];
    //vx[0] = -1.0;
    //vx[1] = -1.0;
    //vx[2] = 1.0;
    //vx[3] = 1.0;
    //float vy[4];
    //vy[0]= -1.0;
    //vy[1]= 1.0;
    //vy[2]= 1.0;
    //vy[3]= -1.0;
    mat4 MV = gl_ModelViewMatrix;
 
  vec3 right = vec3(MV[0][0], 
                    MV[1][0], 
                    MV[2][0]);
 
  vec3 up = vec3(MV[0][1], 
                 MV[1][1], 
                 MV[2][1]);
  
  vec3 P = gl_PositionIn[0].xyz;
 
  mat4 VP = gl_ModelViewProjectionMatrix;

    color = vec4(scale.x, scale.y, 0.5, 1.0);
    vVertex =  gl_PositionIn[0];
	vec4 sVertex = matrix * vVertex;
	P.xyz += sVertex.xyz;
scale.x = 1.0;
	scale.y = 1.0;
//1
	oVertex.xyz = P - (right + up);
	oVertex.x *= scale.x;
	oVertex.y *= scale.y;
	texCoord.xy = uvs[ 0 ].xy;
	gl_Position = gl_ModelViewProjectionMatrix * vec4(oVertex.xyz,1.0);
	EmitVertex();
//2		

	oVertex.xyz = P - (right - up);
	oVertex.x *= scale.x;
	oVertex.y *= scale.y;
	texCoord.xy = uvs[ 3 ].xy;
	gl_Position = gl_ModelViewProjectionMatrix * vec4(oVertex.xyz,1.0);
	EmitVertex();
//3		
	oVertex.xyz = P + (right - up);
	oVertex.x *= scale.x;
	oVertex.y *= scale.y;
	texCoord.xy = uvs[ 1 ].xy;
	gl_Position = gl_ModelViewProjectionMatrix * vec4(oVertex.xyz,1.0);
	EmitVertex();
//4
	oVertex.xyz = P + (right + up);
	oVertex.x *= scale.x;
	oVertex.y *= scale.y;
	texCoord.xy = uvs[ 2 ].xy;
	gl_Position = gl_ModelViewProjectionMatrix * vec4(oVertex.xyz,1.0);
	EmitVertex();

	EndPrimitive();

}

