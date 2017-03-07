// render_fragment.txt
// used to render blended hirez terrain
//
//
uniform sampler2D layer_1;
uniform sampler2D layer_2;
uniform sampler2D layer_3;
uniform sampler2D layer_4;
uniform sampler2D n_layer_1;
uniform sampler2D n_layer_2;
uniform sampler2D n_layer_3;
uniform sampler2D n_layer_4;
uniform sampler2D colorMap;
uniform sampler2D mixtexture;
//uniform sampler2D DominateMap;

uniform float col;
uniform float row;
uniform float tile_width;
uniform int used_layers;
// more and more crap added :)
uniform vec4 layer0U;
uniform vec4 layer1U;
uniform vec4 layer2U;
uniform vec4 layer3U;
uniform vec4 layer0V;
uniform vec4 layer1V;
uniform vec4 layer2V;
uniform vec4 layer3V;
uniform int enable_fog;
uniform float l_ambient;
uniform float l_texture;
uniform float gray_level;
uniform float gamma;
uniform int main_texture;
varying vec3 lightVec;
varying vec3 eyeVec;
varying vec2 texCoord;
varying vec3 g_vertexnormal;
varying vec3 g_viewvector;
varying vec3 lightDirection;
varying vec4 RGB;
varying vec3 norm;
varying vec4 Vposition;
varying vec3 Vertex;
varying float ln;
varying vec4 mask;
varying vec4 mask_2;
////////////////////////////////////////////////////////////////
mat3 cotangent_frame(vec3 nn, vec3 p, vec2 uv) {
  // get edge vectors of the pixel triangle

  vec3 dp1 = dFdx(p);
  vec3 dp2 = dFdy(p);
  vec2 duv1 = dFdx(uv);
  vec2 duv2 = dFdy(uv);
  // solve the linear system
  vec3 dp2perp = cross(dp2, nn);
  vec3 dp1perp = cross(nn, dp1);

  vec3 T = normalize(dp2perp * duv1.x + dp1perp * duv2.x);
  vec3 B = normalize(dp2perp * duv1.y + dp1perp * duv2.y);
  // construct a scale-invariant frame 
  float invmax = inversesqrt(max(dot(T, T), dot(B, B)));

  return mat3(T * invmax, B * invmax, nn);
}
vec3 get_preturp(vec3 bump, vec3 nn, vec3 p, vec2 uv) {
    mat3 tbn = cotangent_frame(nn, p, uv);
    return normalize(tbn * bump);
  }
  //////////////////////////////////////////////////////////////
void main(void)

{
  vec4 t1;
  vec4 t2;
  vec4 t3;
  vec4 t4;
  vec4 n1;
  vec4 n2;
  vec4 n3;
  vec4 n4;
  vec4 MixLevel;
  vec4 Nmix1;
  vec4 Coutmix;
  vec4 Noutmix;
  vec2 mix_coords;
  vec2 loc;
  vec3 tc;
  float mk1, mk2, mk3, mk4; // our use layer masks
  float uv_scale = 10.0;
  vec2 texUV = -gl_TexCoord[0].xy;

  float adj_scale = uv_scale / tile_width;
  float cell_scale = (256.0 / 272.0);
  float pad = (8.0 / (272.0 * tile_width));
  adj_scale = (adj_scale * cell_scale) / uv_scale;

  loc.xy = texUV.xy / uv_scale;

  loc = loc * adj_scale; // ok

  mix_coords.x = +loc.x - ((row) / tile_width);
  mix_coords.y = -loc.y - ((col + 1.0) / tile_width);
  mix_coords.x = mix_coords.x - pad;
  mix_coords.y = mix_coords.y + pad;

  // useless so far. I dont know how to use it.
  //vec4 shadow = texture2D(shadowMap, gl_TexCoord[0].st/uv_scale).xyzw;
  //float angle_1 = shadow.r + (shadow.g * 256.0);
  //float angle_2 = shadow.b + (shadow.a * 256.0);
  //////////
  // five weeks to figure this out! I have tried everything from pre-creating
  // the textures to a combo of premixed and shader to finaly this.
  // Its not perfect but its as close as I can get and I'm tired
  // of working on this!!!!!!!!!!
  //----------------------------------------------------------------
  ////// layer 4
  //------------------------------------------------------------------

  vec2 tv4;
  tv4 = vec2(dot(-layer3U, Vposition), dot(layer3V, Vposition));
  t4 = texture2D(layer_4, tv4 + .5); // new
  n4 = texture2D(n_layer_4, tv4 + .5); // new

  ////// layer 3
  //------------------------------------------------------------------

  vec2 tv3;
  tv3 = vec2(dot(-layer2U, Vposition), dot(layer2V, Vposition));
  t3 = texture2D(layer_3, tv3 + .5); // new
  n3 = texture2D(n_layer_3, tv3 + .5); // new

  ////// layer 2
  //------------------------------------------------------------------
  vec2 tv2;

  tv2 = vec2(dot(-layer1U, Vposition), dot(layer1V, Vposition));
  t2 = texture2D(layer_2, tv2 + .5); // new
  n2 = texture2D(n_layer_2, tv2 + .5); // new

  ////// layer 1
  //------------------------------------------------------------------
  vec2 tv1;
  tv1 = vec2(dot(-layer0U, Vposition), dot(layer0V, Vposition));
  t1 = texture2D(layer_1, tv1 + .5); // new
  n1 = texture2D(n_layer_1, tv1 + .5); // new

  //------------------------------------------------------------------
  mk1 = float((used_layers & 1) / 1);
  mk2 = float((used_layers & 2) / 2);
  mk3 = float((used_layers & 4) / 4);
  mk4 = float((used_layers & 8) / 8);

  MixLevel = texture2D(mixtexture, mix_coords.xy).rgba;
  Nmix1 = MixLevel;

  MixLevel.r *= mask.r;
  MixLevel.g *= mask.g;
  MixLevel.b *= mask.b;
  MixLevel.a *= mask.a;

  // Don't know how to use it :(
  //float DomMix = texture2D(DominateMap, gl_TexCoord[0].st/uv_scale).r;

  t1 = mix(texture2D(layer_1, gl_TexCoord[0].st / uv_scale), t1, mask_2.r);
  t2 = mix(texture2D(layer_2, gl_TexCoord[0].st / uv_scale), t2, mask_2.g);
  t3 = mix(texture2D(layer_3, gl_TexCoord[0].st / uv_scale), t3, mask_2.b);
  t4 = mix(texture2D(layer_4, gl_TexCoord[0].st / uv_scale), t4, mask_2.a);

  // mix our color
  Coutmix = t4 * MixLevel.a;
  Coutmix += t3 * MixLevel.b;
  Coutmix += t2 * MixLevel.g;
  Coutmix += t1 * MixLevel.r;
  Coutmix *= 1.2;

  //vec4 bump;
  //bump.xyzw = mix(Noutmix.xyzw, vec4(0.0, 0.0, 0.0, 0.0), 1.0 - ln); // bump is mixed as a function of distance
  // this blends between low and highrez with distance
  float texture_level = (l_texture * .5);
  vec4 lowrez = texture2D(colorMap, gl_TexCoord[0].st / uv_scale);
  vec4 base = mix(Coutmix, lowrez, 1.0 - ln) * texture_level; /// blend lowrez and highrez textures
  base.a = 0.0;
  //-------------------------------------------------------------
  // Do lighting

  vec4 vAmbient = vec4(l_ambient, l_ambient, l_ambient, 1.0);

  n1.rgb = normalize(2.0 * n1.rgb - 1.0) * MixLevel.r;
  n1.y *= -1.0;
  n2.rgb = normalize(2.0 * n2.rgb - 1.0) * MixLevel.g;
  n2.y *= -1.0;
  n3.rgb = normalize(2.0 * n3.rgb - 1.0) * MixLevel.b;
  n3.y *= -1.0;
  n4.rgb = normalize(2.0 * n4.rgb - 1.0) * MixLevel.a;
  n4.y *= -1.0;
  
  //-------------------------------------------------------------
  //There is no good way to add normals together and not destroy them.
  // We have to do the math on each one THAN mix them together.
  vec3 N = normalize(norm);
  vec3 L = normalize(lightDirection);

  float diffuse = 0.0;
  vec3 PN1;
  vec3 PN2;
  vec3 PN3;
  vec3 PN4;
  
  float bump_level = 0.3;
  PN4 = get_preturp(n4.rgb, N, -g_viewvector, tv4 + .5) * MixLevel.a * bump_level;
  PN3 = get_preturp(n3.rgb, N, -g_viewvector, tv3 + .5) * MixLevel.b * bump_level;
  PN2 = get_preturp(n2.rgb, N, -g_viewvector, tv2 + .5) * MixLevel.g * bump_level;
  PN1 = get_preturp(n1.rgb, N, -g_viewvector, tv1 + .5) * MixLevel.r * bump_level;
  
  float spec = 1.0;
  diffuse = diffuse + max(dot(PN4, L) * mk4, 0.0) * MixLevel.a ;
  diffuse = diffuse + max(dot(PN3, L) * mk3, 0.0) * MixLevel.b ;
  diffuse = diffuse + max(dot(PN2, L) * mk2, 0.0) * MixLevel.g ;
  diffuse = diffuse + max(dot(PN1, L) * mk1, 0.0) * MixLevel.r ;

  diffuse += ((Coutmix-diffuse) *.1);


  vec3 lightDir = vec3(normalize(gl_LightSource[0].position.xyz - Vertex.xyz));
  vec3 n = normalize(norm) * .28;

  float diffuse_2 = max(dot(n, L), l_ambient) * 1.9;

  // final color mix

  gl_FragColor.rgb = clamp(clamp((diffuse_2 * diffuse * 1.0), 0.0, 0.1) * base * 8.0 * ln, 0.0, 0.4);

  // This adds brightnees shift from zooming out to cancel the bump brightness effect;
  gl_FragColor.rgb += clamp((1.0 - ln) * base.xyz * (diffuse_2), 0.0, 0.3);

  gl_FragColor += vAmbient * base;

  vec3 vG = vec3(1.0, 1.0, 1.0);
  gl_FragColor.rgb = pow(gl_FragColor.rgb, vG / gamma);
  gl_FragColor *= 12.0;

  //gray level
  vec3 luma = vec3(0.299, 0.587, 0.114);
  vec3 co = vec3(dot(luma, gl_FragColor.rgb));
  vec3 c = mix(co, gl_FragColor.rgb, gray_level);
  gl_FragColor.rgb = c;

  //----------------------------------------------------------------------
  // debug crap
  //gl_FragColor.r *= spec_scale;
  //gl_FragColor.xyz =vSpecular + (gl_FragColor.xyz*.1);
  //gl_FragColor.xyz = diffuse+ vSpecular + (gl_FragColor.xyz*.01);

  //----------------------------------------------------------------------
  // FOG addition
  const float LOG2 = 1.442695;
  float z = gl_FragCoord.z / gl_FragCoord.w;
  float fogFactor = exp2(-gl_Fog.density * gl_Fog.density * z * z * LOG2);
  fogFactor = clamp(fogFactor, 0.0, 1.0);
  if (enable_fog == 1) {
    gl_FragColor = mix(gl_Fog.color, gl_FragColor, fogFactor);
  } else {
    gl_FragColor = gl_FragColor;
  }

}