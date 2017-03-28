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
varying mat3 TBN;
varying float ln;
varying vec3 g_vertexnormal;
varying vec3 g_viewvector;
varying vec3 lightDirection;
//////////////////////////////////////////////////////////////
void main (void)
    {
    float a_table[40];
    a_table[2]=0.7;
    a_table[16] = 1.5;
    a_table[18] = 1.5;
    a_table[30] = 1.5;
    if (ln == 0.0) {discard;} // dont render out of range decals.. save some time.
    
    vec2 wrap = vec2(u_wrap, v_wrap);
    vec3 N = normalize(g_vertexnormal);
    vec3 L = normalize(lightDirection);
    float a;
    float NdotL;
    vec4 color = texture2D(colorMap,  texCoord / wrap);
    a = color.a;
    color.xyz *= l_texture;

    vec4 bump = texture2D(normalMap, texCoord*wrap);
    bump.xyz = bump.xyz * 2.0 -1.0;
    bump =  normalize(bump);
    //----------------------------------------
    const float s = 0.5;
    vec4 Ambient = vec4( ambient *s,ambient*s , ambient*s,1.0);

    // calculate bump

    vec3 PN = TBN * bump.xyz;
    NdotL = max(dot(PN, L),0.0)*1.0;
    color.rgb += ((color.rgb * NdotL));
    vec4 final_color = color * Ambient;

    //scale up level
    gl_FragColor = final_color * 7.0 ;


    // gamma correction
    vec3 vG = vec3(1.0 , 1.0 , 1.0);
    gl_FragColor.rgb = pow(gl_FragColor.rgb, vG/(gamma*.8));

    //gray level
    vec3 luma = vec3(0.299, 0.587, 0.114);
    vec3 co = vec3(dot(luma,gl_FragColor.rgb));
    vec3 c = mix(co, gl_FragColor.rgb,gray_level);
    gl_FragColor.rgb = c;

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
    gl_FragColor.rgb*=gl_FragColor.a;
    a *= ln;
    gl_FragColor.a = a * a_table[int(influence)];
    //gl_FragColor.rgb = gl_FragColor.rgb *.002 + ( NdotL);

}