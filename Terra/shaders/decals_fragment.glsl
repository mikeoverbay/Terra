// decals_fragment.txt
// used to render decals
uniform sampler2D normalMap;
uniform sampler2D colorMap;
uniform int enable_fog;
uniform float l_texture;
uniform float gray_level;
uniform int is_bumped;
uniform float gamma;
uniform float ambient;
uniform float u_wrap;
uniform float v_wrap;
uniform float influence;
varying vec2 texCoord;

varying float ln;
varying vec3 g_vertexnormal;
varying vec3 g_viewvector;
varying vec3 lightDirection;
mat3 cotangent_frame( vec3 nn, vec3 p, vec2 uv )
{
    // get edge vectors of the pixel triangle

    vec3 dp1 = dFdx( p );
    vec3 dp2 = dFdy( p );
    vec2 duv1 = dFdx( uv );
    vec2 duv2 = dFdy( uv );
    // solve the linear system
    vec3 dp2perp = cross( dp2, nn);
    vec3 dp1perp = cross( nn, dp1 );

    vec3 T = normalize(dp2perp * duv1.x + dp1perp * duv2.x);
    vec3 B = normalize(dp2perp * duv1.y + dp1perp * duv2.y);
    // construct a scale-invariant frame 
    float invmax = inversesqrt( max( dot(T,T), dot(B,B) ) );

    return mat3( T * invmax, B * invmax, nn );
}
vec3 get_preturp(vec3 bump ,vec3 nn, vec3 p , vec2 uv) 
{
mat3 tbn = cotangent_frame( nn,  p,  uv );
return normalize( tbn * bump );
}
//////////////////////////////////////////////////////////////
void main (void)
{
    
    float a_table[40];
    a_table[2]=0.7;
    a_table[16] = 1.5;
    a_table[18] = 1.5;
    a_table[30] = 1.5;
if (ln == 0.0) { discard; } // no point in rendering if it's invisible.
    
    vec2 wrap = vec2(u_wrap, v_wrap);

vec3 N = normalize(g_vertexnormal);
vec3 L = normalize(lightDirection);
float a;

float NdotL;
   
vec4 color = texture2D(colorMap,  texCoord*wrap);
a = color.a;
color.xyz *= l_texture;
a *= ln;
//color.rgb = (color.rgb *0.001) + vec3(0.5,0.5,0.5);

vec4 bump = texture2D(normalMap, texCoord*wrap);
bump.xyz = bump.xyz * 2.0 -1.0;

bump.x *= -1.0;
bump =  normalize(bump);
   //----------------------------------------
   float s = 0.5;
   vec4 Ambient = vec4( ambient *s,ambient*s , ambient*s,1.0);

// calculate bump

vec3 PN = get_preturp(bump.rgb, N, -g_viewvector, texCoord);


NdotL = max(pow(dot(PN, L),2.0),0.0)*1.0;
color.rgb += ((color.rgb * NdotL));
vec4 final_color = color * Ambient;


   gl_FragColor = final_color *10.2 ;
   
   
// gamma correction
const float vGv = 1.0;
vec3 vG = vec3(vGv , vGv , vGv);
gl_FragColor.rgb *= 1.3;
gl_FragColor.rgb = pow(gl_FragColor.rgb, vG/(gamma*.8));

//gray level
vec3 luma = vec3(0.299, 0.587, 0.114);
vec3 co = vec3(dot(luma,gl_FragColor.rgb));
vec3 c = mix(co, gl_FragColor.rgb,gray_level);
gl_FragColor.rgb = c;
//======================================================
//Debug junk
//gl_FragColor.xyz = N.rgb + (gl_FragColor.xyz*0.1);
//gl_FragColor.w   = 1.0;
//gl_FragColor.xyz * lambertTerm;
//======================================================

// FOG calculation
 const float LOG2 = 1.442695;

float z = gl_FragCoord.z / gl_FragCoord.w;
float fogFactor = exp2( -gl_Fog.density * gl_Fog.density * z * z * LOG2 );
fogFactor = clamp(fogFactor, 0.0, 1.0);
if (enable_fog == 1)
{
gl_FragColor.rgb = mix(gl_Fog.color.rgb, gl_FragColor.rgb, fogFactor );
}
else
{
}
gl_FragColor.a = a* a_table[int(influence)];


}