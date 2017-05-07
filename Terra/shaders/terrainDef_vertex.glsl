// render_vertex.txt
// used to render blended hirez terrain
#version 330 compatibility 

uniform int main_texture;
uniform vec3 cam_position;
uniform mat4 viewMatrix;
out vec2 texCoord;
out vec3 n;
out float ln;
out vec4 Vertex;
out vec4 world_vertex;
out vec4 mask_2;
out mat3 TBN;

void main(void)
{ 
    gl_Position = ftransform();
    texCoord = -gl_MultiTexCoord0.xy;
    vec4 point = gl_Vertex;
    Vertex = gl_Vertex;
    Vertex.x *= -1.0;//For Texture Translation!!

    n = normalize(gl_Normal);
    vec3 t = normalize(gl_MultiTexCoord2.xyz);
    vec3 b = normalize(cross(t,n));
    TBN = mat3(t,b,n);
    world_vertex = gl_Vertex;

// Create the mask.  Used to cancel ant transform of tex_color;
    mask_2 = vec4(1.0 ,1.0 ,1.0 ,1.0);
    switch (main_texture){
    case 1: mask_2.r = 0.0; break;
    case 2: mask_2.g = 0.0; break;
    case 3: mask_2.b = 0.0; break;
    case 4: mask_2.a = 0.0; break;
    }
// This is the cut off distance for bumpping the surface.
    ln = distance(point.xyz,cam_position.xyz);
    if (ln<500.0) { ln = sin((1.0 - ln/500.0) * 1.5708);} //Creates sine curve.
    else {ln = 0.0;}
}
