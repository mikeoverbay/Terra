// render_vertex.txt
// used to render blended hirez terrain
#version 330 compatibility 

uniform int texture_mask;
uniform vec3 cam_position;
uniform mat4 viewMatrix;
out vec2 texCoord;
out vec3 n;
out float ln;
out vec4 Vertex;
out vec4 world_vertex;
out vec4 mask_1;
out vec4 mask_2;
out mat3 TBN;
out float hole;

void main(void)
{ 
    gl_Position = ftransform();
    texCoord = -gl_MultiTexCoord0.xy;
    hole = gl_MultiTexCoord0.z;
    vec4 point = gl_Vertex;
    Vertex = gl_Vertex;
    Vertex.x *= -1.0;//For Texture Translation!!

    n = normalize(gl_Normal);
    vec3 t = normalize(gl_MultiTexCoord2.xyz);
    vec3 b = normalize(cross(t,n));
    TBN = mat3(t,b,n);
    world_vertex = gl_Vertex;

// Create the mask.  Used to cancel ant transform of tex_color;\
	float on = 1.0;
	float off = 0.0;
    mask_1 = vec4(1.0 ,1.0 ,1.0 ,1.0);
    mask_2 = vec4(1.0 ,1.0 ,1.0 ,1.0);
    if ( texture_mask > 128 ) mask_2.r = 0.0; 
    if ( texture_mask > 64 ) mask_2.g = 0.0;
    if ( texture_mask > 32 ) mask_2.b = 0.0;
    if ( texture_mask > 16 ) mask_2.a = 0.0;
    if ( texture_mask > 8  ) mask_1.r = 0.0;
    if ( texture_mask > 4  ) mask_1.g = 0.0;
    if ( texture_mask > 2  ) mask_1.b = 0.0;
    if ( texture_mask >= 1  ) mask_1.a = 0.0; // this should always be set
// This is the cut off distance for bumpping the surface.
    ln = distance(point.xyz,cam_position.xyz);
    if (ln<500.0) { ln = sin((1.0 - ln/500.0) * 1.5708);} //Creates sine curve.
    else {ln = 0.0;}
}
