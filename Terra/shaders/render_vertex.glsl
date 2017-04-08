// render_vertex.txt
// used to render blended hirez terrain


uniform int main_texture;
uniform vec3 cam_position;
uniform float tile_width;

varying vec3 lightVec; 
varying vec3 eyeVec;
varying vec2 texCoord;
varying vec3 g_viewvector;
varying vec3 lightDirection;
varying vec3 n;
varying float ln;
varying vec4 Vertex;

varying vec4 mask_2;
varying mat3 TBN;
void main(void)
{ 
    gl_Position = ftransform();     
    texCoord = -gl_MultiTexCoord0.xy;  
    vec4 point = gl_Vertex;
    Vertex = gl_Vertex;
    Vertex.x *= -1.0;//For Texture Translation!!

    n   = gl_NormalMatrix * gl_Normal;
    vec3 t   = gl_NormalMatrix * gl_MultiTexCoord2.xyz;
    vec3 b   = gl_NormalMatrix *gl_MultiTexCoord3.xyz;
    t   = normalize(t - dot(n, t) * n);
    b   = normalize(b - dot(n, b) * n);
    TBN = mat3(t, b, n);

    vec4 vert = gl_ModelViewMatrix * gl_Vertex;
    lightDirection = gl_LightSource[0].position.xyz - vert.xyz;

    g_viewvector = cam_position - gl_Vertex.xyz;

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
