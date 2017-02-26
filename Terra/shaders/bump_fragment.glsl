// bump_fragment.glsl
// used to render all building and other models
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
uniform float ambient; in vec3 T; in vec3 BN; in vec3 g_vertexnormal;

in vec2 TC1; in vec2 TC2; in vec3 vVertex; in vec3 lightDirection; in vec4 rgb;
////////////////////////////////////////////////////////////////////////////////////////////
void main(void) {
  float invmax = inversesqrt(max(dot(T, T), dot(BN, BN)));
  mat3 TBN = mat3(T * invmax, BN * invmax, -g_vertexnormal);
  vec3 bump;
  float alpha;
  float harshness = 1.0;
  float boost = 1.0;
  float a;
  vec3 bumpMap;
  // get normal map based on type

  if (is_GAmap == int(1)) {
    bumpMap.xy = (2.0 * texture2D(normalMap, TC1.st).ag - 1.0);;
    bumpMap.z = sqrt(1.0 - dot(bumpMap.xy, bumpMap.xy));
    bumpMap.x *= -1.0;
    bumpMap = normalize(bumpMap);
    harshness = 1.4;
    a = textureLod(normalMap, TC1.st, 0).r;
    if (alphaTestEnable != 0) {
      alpha = 1.0 - float(alphaRef / 255);
      if (a < alpha) {
        discard;
      }

    }
  } else {
    bumpMap = (2.0 * texture2D(normalMap, TC1.st).rgb) - 1.0;
    bumpMap.xz *= -1.0;
    bumpMap = normalize(bumpMap);
    a = textureLod(colorMap, TC1.st, int(0)).a;
    if (a < 0.3) {
      discard;
    }

  }

  vec4 color = texture2D(colorMap, TC1.st);
  if (is_multi_textured == int(1)) {
    vec4 d2 = texture2D(colorMap_2, TC2.st);
    color *= d2;
    color *= 1.5;
  }

  color.xyz *= l_texture;

  vec3 N = normalize(g_vertexnormal);
  // Get the perturbed normal
  vec3 PN = normalize(TBN * bumpMap);
  // light position
  vec3 L = normalize(lightDirection);
  //caclulate lighting

  //color*=0.3;
  float NdotL = max(dot(PN, L), 0.0);
  color.rgb += (color.rgb * pow(NdotL, 3.0));
  vec4 final_color;
  final_color = color + (color * vec4(ambient, ambient, ambient, 1.0));

  gl_FragColor = final_color ;
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
  //const float LOG2 = 1.442695;
  //float z = gl_FragCoord.z / gl_FragCoord.w;
  //float fogFactor = exp2(-gl_Fog.density * gl_Fog.density * z * z * LOG2);
  //fogFactor = clamp(fogFactor, 0.0, 1.0);
  //if (enable_fog == int(1)) {
  //  gl_FragColor = mix(gl_Fog.color, gl_FragColor, fogFactor);
  //} else {
  //  gl_FragColor = gl_FragColor;
  //};
}