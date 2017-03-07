// render_vertex.txt
// used to render blended hirez terrain

uniform sampler2D normalMap;
uniform int main_texture;
uniform vec3 cam_position;
uniform float tile_width;

varying vec3 lightVec; 
varying vec3 eyeVec;
varying vec2 texCoord;
varying vec3 g_viewvector;
varying vec3 lightDirection;

varying vec3 norm;
varying float ln;
varying vec4 Vposition;
varying vec3 Vertex;
varying vec4 mask;
varying vec4 mask_2;

void main(void)
{ 
    lightDirection = gl_LightSource[0].position.xyz - gl_Vertex.xyz;
    lightDirection.z*= -1.0;

    g_viewvector = cam_position - gl_Vertex.xyz;

vec3 tangent;
// Create the mask. 
float c_scale = 1.0; // trial and error... These are best fit values.
float m_scale = 1.0;
    mask = vec4( c_scale ,c_scale , c_scale, c_scale);
    mask_2 = vec4(1.0 ,1.0 ,1.0 ,1.0);

if (main_texture == 1 )
{
mask.r = m_scale;
mask_2.r = 0.0;
}
if (main_texture == 2 )
{
mask.g = m_scale;
mask_2.g = 0.0;
}
if (main_texture == 3 )
{
mask.b = m_scale;
mask_2.b = 0.0;
}
if (main_texture == 4 )
{
mask.a = m_scale;
mask_2.a = 0.0;
}


    vec4 point = gl_Vertex;
    Vposition.x = point.x ;
    Vposition.y = point.y;
    Vposition.z = -point.z;
    Vposition.w = point.w;


    gl_Position = ftransform();     
    gl_TexCoord[0] = -gl_MultiTexCoord0;
    texCoord = gl_MultiTexCoord0.xy/1.001;

norm.zx = ( texture2D(normalMap, -texCoord*.1).gb *2.0-1.0);
norm.y  = sqrt(1.0-((norm.x*norm.x)+(norm.z*norm.z)));
norm    = normalize(norm);
norm.xz *= -1.0;

// This is the cut off distance for bumpping the surface.
ln = distance(point.xyz,cam_position.xyz);
if (ln<400.0)
{
    ln = 1.0 - ln/400.0;
    }
    else
    {
    ln = 0.0;
}
}
