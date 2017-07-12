
#version 330 compatibility

uniform mat4 matrix;
uniform sampler2D normalMap;
uniform sampler2D normalMap2;

uniform float time;
uniform float aspect;
uniform float shift;

out vec2 tc;
out vec4 color;

void main(void)
{
vec4 b1,b2;
    tc.xy = gl_Vertex.xz*5.0;
    gl_Position = matrix * gl_Vertex;

    b1.xy = (texture2D(normalMap, tc.xy ).ag);        
    b1.z = sqrt(1.0 - ((b1.x*b1.x) + (b1.y*b1.y)));
    b1.a  = texture2D(normalMap, tc.xy ).r;

    b2.xy = (texture2D(normalMap2, tc.xy ).ag);        
    b2.z = sqrt(1.0 - ((b2.x*b2.x) + (b2.y*b2.y)));
    b2.a  = texture2D(normalMap2, tc.xy).r;

    float p = 1.0;
    vec4 v_shift =  pow(mix(b1, b2, time),vec4(p,p,p,p))*2.0-1.0; // blend from b1 to b2 based on time sample.
    gl_Position.xz += v_shift.xz*20.0;
    gl_Position.y += v_shift.y*3.0;

    gl_Position = gl_ModelViewProjectionMatrix * gl_Position;
    color = gl_Color;
}
