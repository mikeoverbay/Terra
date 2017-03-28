// bump_fragment.glsl
//Used to light all models
#version 130
uniform sampler2D normalMap;
uniform sampler2D colorMap;
uniform sampler2D colorMap_2;
uniform int enable_fog;
uniform int is_GAmap;
uniform float l_texture;
uniform float gray_level;
uniform int is_bumped;
uniform int is_multi_textured;
uniform float gamma;
uniform int alphaRef;
uniform int alphaTestEnable;
uniform float ambient;
in vec2 TC1;
in vec2 TC2;
in vec3 vVertex;
in vec3 lightDirection;
in mat3 TBN;

in vec3 t;
in vec3 b;
in vec3 n;
////////////////////////////////////////////////////////////////////////////////////////////
void main(void) {
    vec3 bump;
    float alpha;
    float harshness = 1.0;
    float boost = 1.0;
    float a;
    vec3 bumpMap;
    // get normal map based on type

  if (is_GAmap == int(1)) {
        bumpMap.xy = (2.0 * texture2D(normalMap, TC1.st).ag - 1.0);
        ;
        bumpMap.z = sqrt(1.0 - dot(bumpMap.xy, bumpMap.xy));
        bumpMap   = normalize(bumpMap);
        harshness = 0.8;
        a         = textureLod(normalMap, TC1.st, 0).r;
        if (alphaTestEnable != 0) {
            alpha = 1.0 - float(alphaRef / 255);
            if (a < alpha) {
                discard;
            }


    }
  } else {
        bumpMap = (2.0 * texture2D(normalMap, TC1.st).rgb) - 1.0;
        bumpMap = normalize(bumpMap);
        a = textureLod(colorMap, TC1.st, int(0)).a;
        if (a < 0.3) {
            discard;
        }


  }

    vec4 base = texture2D(colorMap, TC1.st);
    if (is_multi_textured == int(1)) {
        vec4 d2 = texture2D(colorMap_2, TC2.st);
        base *= d2;
        base *= 1.5;
    }
   vec4 color = base;
   color.rgb *= l_texture;



    // Get the perturbed normal
  vec3 PN = normalize(TBN * bumpMap);
    // light position
  vec3 L = normalize(lightDirection);
    //caclulate lighting

  //color*=0.3;
  float NdotL = max(dot(PN, L), 0.0);
    color.rgb += (color.rgb * pow(NdotL, 1.0));
    vec4 final_color = color;
    final_color.rgb += (base.rgb * ambient*1.0);
    gl_FragColor = final_color;

    //gamma
    vec3 vG = vec3(harshness, harshness, harshness);
    gl_FragColor.rgb = pow(gl_FragColor.rgb, vG / gamma);
    //gray level
    vec3 luma = vec3(0.299, 0.587, 0.114);
    vec3 co = vec3(dot(luma, gl_FragColor.rgb));
    vec3 c = mix(co, gl_FragColor.rgb, gray_level);
    gl_FragColor.rgb = c;
    //======================================================
  //debug junk
  //gl_FragColor.rgb = (color.rgb * 0.00001 + (vec3(3.0) * NdotL));
  //======================================================

  // FOG calculation
  const float LOG2 = 1.442695;
    float z = gl_FragCoord.z / gl_FragCoord.w;
    float fogFactor = exp2(-gl_Fog.density * gl_Fog.density * z * z * LOG2);
    fogFactor = clamp(fogFactor, 0.0, 1.0);
    if (enable_fog == int(1)) {
        gl_FragColor = mix(gl_Fog.color, gl_FragColor, fogFactor);
    }
 else {
        gl_FragColor = gl_FragColor;
    }
    //gl_FragColor.rgb = gl_FragColor.rgb*.001 + n.xyz;
}