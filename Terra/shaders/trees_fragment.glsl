// trees_fragment.txt
// used to render trees
uniform sampler2D normalMap;
uniform sampler2D colorMap;
uniform int enable_fog;
uniform float l_texture;
uniform float gray_level;
uniform float gamma;
uniform float ambient;
varying vec2 texCoord;
varying vec3 norm;
varying vec3 halfVector;
varying float ln;
void main (void)
{
    vec3 halfV,lightDir;
    float NdotL;
    lightDir = normalize( gl_LightSource[0].position.xyz);
    halfV = normalize(halfVector);
    lightDir.x *= -1.0;
    vec3 n = normalize(norm);
    vec4 base = texture2D(colorMap,  texCoord);
    base.xyz *= l_texture;
   if ( base.a <0.2 ) {discard;}


    vec3 bump = texture2D(normalMap, texCoord).rgb * 2.0 - 1.0;
    bump =  normalize(bump);
    //----------------------------------------
   vec4 Ambient = vec4( ambient ,ambient , ambient , 1.0);
    vec4 color = Ambient;
    // calculate bump
    float diffuse =  max(dot(bump , lightDir.xyz), 0.0);
    base = base * Ambient;
    base *=12.0;

    float specular = texture2D(normalMap, texCoord).a ;
    NdotL = max(dot(n, lightDir.xzy),0.0);
    if (NdotL > 0.0) {
        color += Ambient * pow(NdotL,3.0);
    }

   
    gl_FragColor = base * ( (base *0.7) + clamp((diffuse *color ) + specular, 0.0,0.2) ) *ln;
    gl_FragColor += (base * color*1.0) *(1.0-ln);

    //gl_FragColor += vColor;
    // gamma correction
    const float vGv = 1.0;
    vec3 vG = vec3(vGv , vGv , vGv);
    gl_FragColor.rgb *= 1.3;
    gl_FragColor.rgb = pow(gl_FragColor.rgb, vG/gamma);
//gray level
vec3 luma = vec3(0.299, 0.587, 0.114);
vec3 co = vec3(dot(luma,gl_FragColor.rgb));
vec3 c = mix(co, gl_FragColor.rgb,gray_level);
gl_FragColor.rgb = c;
//======================================================
//Debug junk
//gl_FragColor.xyz =  ( (color *0.3) + (diffuse *color * 0.5));

//gl_FragColor.xyz = bump;
//======================================================

// FOG calculation
    //const float LOG2 = 1.442695;
    //float z = gl_FragCoord.z / gl_FragCoord.w;
    //float fogFactor = exp2( -gl_Fog.density * gl_Fog.density * z * z * LOG2 );
    //fogFactor = clamp(fogFactor, 0.0, 1.0);
    //if (enable_fog == 1)
    //    {
    //    gl_FragColor = mix(gl_Fog.color, gl_FragColor, fogFactor );
    //    }
    //    else
    //    {
    //    gl_FragColor = gl_FragColor;
    //    }
}
