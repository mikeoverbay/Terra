// decals_fragment.txt
// used to render decals
#version 330 compatibility
#extension GL_ARB_gpu_shader5 : enable

uniform sampler2D normalMap;
uniform sampler2D colorMap;
uniform float l_texture;
uniform float gray_level;
uniform float gamma;
uniform float ambient;
uniform float u_wrap;
uniform float v_wrap;
uniform float influence;

in vec2 texCoord;
in mat3 TBN;
in float ln;
in vec3 g_vertexnormal;
in vec3 g_viewvector;
in vec3 lightDirection;
//////////////////////////////////////////////////////////////
void main (void)
    {
    if (ln == 0.0) {discard;} // dont render out of range decals.. save some time.
    float a_table[40];// stupid hack.. i need a better way!
    
    for (int i =0; i<39 ; i++){
        a_table[i] = 1.0;
    }
    a_table[2]=0.7;
    a_table[16] = 1.5;
    a_table[18] = 1.5;
    a_table[30] = 1.5;

    vec2 wrap = vec2(u_wrap, v_wrap);
    vec3 N = normalize(g_vertexnormal);
    vec3 L = normalize(lightDirection);
    float a;
    float NdotL;
    vec4 color = texture2D(colorMap,  texCoord.st * wrap).rgba;
    a = color.a;
    color.rgb *= l_texture;

    vec4 bump = texture2D(normalMap, texCoord.st * wrap).rgba;
    bump.xyz = bump.xyz * 255.0/128.0 -1.0;
    bump =  normalize(bump);
//----------------------------------------
    const float s = 0.5;
    vec3 Ambient = vec3( ambient * s, ambient * s , ambient * s );

// calculate bump

    vec3 PN = TBN * bump.xyz;
    NdotL = max(dot(PN, L),0.0);
    //NdotL *= bump.a*8.0;//Use spec in bump alpha to kick up bumping.
    //NdotL -= sin((NdotL) * 1.570796);
    NdotL = max(NdotL, 0.0);
    color.rgb += ((color.rgb * NdotL));


    color.rgb += (color.rgb * Ambient);

//scale up level
    gl_FragColor.rgb = color.rgb *0.8 ;
    gl_FragColor.a = a;

// gamma correction
const float gam = 0.4;
    vec3 vG = vec3(gam , gam , gam);
    gl_FragColor.rgb = pow(gl_FragColor.rgb, vG / (gamma * 0.8));

//gray level
    vec3 luma = vec3(0.299, 0.587, 0.114);
    vec3 co = vec3(dot(luma,gl_FragColor.rgb));
    vec3 c = mix(co, gl_FragColor.rgb, gray_level);
    gl_FragColor.rgb = c;

// FOG calculation
    const float LOG2 = 1.442695;
    float z = gl_FragCoord.z / gl_FragCoord.w;
    float fogFactor = exp2( -gl_Fog.density * gl_Fog.density * z * z * LOG2 );
    fogFactor = clamp(fogFactor, 0.0, 1.0);        
    gl_FragColor.rgb = mix(gl_Fog.color.rgb, gl_FragColor.rgb, fogFactor );

    a *= ln;
    gl_FragColor.a = a * a_table[int(influence)];

}