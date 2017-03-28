// 
// used to render water
// It was a good idea that didnt work!

uniform sampler2D colorMap;
uniform float uv_wrap_u;
uniform float uv_wrap_v;
varying vec2 texCoord;
void main (void)
{
vec2 UV_coords = vec2(texCoord.s * uv_wrap_u, texCoord.t * uv_wrap_v);
float a = texture2D(colorMap,UV_coords.xy).a;
if (a<.1) { discard; }

const float level = 0.1;
    gl_FragColor = vec4 (level ,level ,level ,a);        

}