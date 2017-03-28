// render_fragment.txt
// used to render blended hirez terrain
//
//
#extension GL_EXT_gpu_shader4 : enable
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
uniform sampler2D hole_texture;
uniform int has_holes;
uniform float mapHeight;
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
varying vec3 g_viewvector;
varying vec3 lightDirection;
varying vec3 n;
varying vec4 Vposition;
varying float ln;
varying vec4 mask;
varying vec4 mask_2;
varying vec2 hUV;
varying mat3 TBN;

////////////////////////////////////////////////////////////////
void main(void)
{
    // lets check for a hole before doing any math.. It saves time
  float hole = texture2D(hole_texture,hUV).x;
    if (has_holes == 1)
      {
        if ( hole >0.0) {
            discard;
        }

      }
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
    float mk1, mk2, mk3, mk4;
    // our use layer masks
  float uv_scale = 10.0;
    vec2 texUV = -gl_TexCoord[0].xy;
    float adj_scale = uv_scale / tile_width;
    float cell_scale = (256.0 / 272.0);
    float pad = (8.0 / (272.0 * tile_width));
    adj_scale = (adj_scale * cell_scale) / uv_scale;
    loc.xy = texUV.xy / uv_scale;
    loc = loc * adj_scale;
    // ok

  mix_coords.x = +loc.x - ((row) / tile_width);
    mix_coords.y = -loc.y - ((col + 1.0) / tile_width);
    mix_coords.x = mix_coords.x - pad;
    mix_coords.y = mix_coords.y + pad;
    ////// layer 4
  //------------------------------------------------------------------
  vec2 tv4;
    tv4 = vec2(dot(-layer3U, Vposition), dot(layer3V, Vposition));
    t4 = texture2D(layer_4, tv4 + .5);
    n4 = texture2D(n_layer_4, tv4 + .5);
    ////// layer 3
  //------------------------------------------------------------------
  vec2 tv3;
    tv3 = vec2(dot(-layer2U, Vposition), dot(layer2V, Vposition));
    t3 = texture2D(layer_3, tv3 + .5);
    n3 = texture2D(n_layer_3, tv3 + .5);
    ////// layer 2
  //------------------------------------------------------------------
  vec2 tv2;
    tv2 = vec2(dot(-layer1U, Vposition), dot(layer1V, Vposition));
    t2 = texture2D(layer_2, tv2 + .5);
    n2 = texture2D(n_layer_2, tv2 + .5);
    ////// layer 1
  //------------------------------------------------------------------
  vec2 tv1;
    tv1 = vec2(dot(-layer0U, Vposition), dot(layer0V, Vposition));
    t1 = texture2D(layer_1, tv1 + .5);
    n1 = texture2D(n_layer_1, tv1 + .5);
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
    t1 = mix(texture2D(layer_1, gl_TexCoord[0].st / uv_scale), t1, mask_2.r);
    t2 = mix(texture2D(layer_2, gl_TexCoord[0].st / uv_scale), t2, mask_2.g);
    t3 = mix(texture2D(layer_3, gl_TexCoord[0].st / uv_scale), t3, mask_2.b);
    t4 = mix(texture2D(layer_4, gl_TexCoord[0].st / uv_scale), t4, mask_2.a);
    // mix our color
    const float boost = 4.0;
    Coutmix = t4 * MixLevel.a;
    Coutmix += t3 * MixLevel.b;
    Coutmix += t2 * MixLevel.g;
    Coutmix += t1 * MixLevel.r;
    Coutmix *= boost;
    // this blends between low and highrez with distance
    float texture_level = (l_texture );
    vec4 lowrez = texture2D(colorMap, gl_TexCoord[0].st / uv_scale);
    vec4 base = mix(lowrez*boost, Coutmix, ln) * texture_level;
    /// blend lowrez and highrez textures
    base.a = 0.0;
    //-------------------------------------------------------------

    vec4 vAmbient = vec4(l_ambient, l_ambient, l_ambient, 1.0);

    //-------------------------------------------------------------
    //Get our normal maps.
    n1.rgb = normalize(1.9921875 * n1.rgb - 1.0) * MixLevel.r;
    n2.rgb = normalize(1.9921875 * n2.rgb - 1.0) * MixLevel.g;
    n3.rgb = normalize(1.9921875 * n3.rgb - 1.0) * MixLevel.b;
    n4.rgb = normalize(1.9921875 * n4.rgb - 1.0) * MixLevel.a;
  
    //-------------------------------------------------------------
    // Do lighting
    //There is no good way to add normals together and not destroy them.
    //We have to do the math on each one THAN mix them together.
    vec3 N = normalize(n);
    vec3 L = normalize(lightDirection);
    float diffuse = 0.0;
    vec3 PN1;
    vec3 PN2;
    vec3 PN3;
    vec3 PN4;
    float bump_level = 1.0;
    PN4 = TBN * n4.rgb * MixLevel.a * bump_level;
    PN3 = TBN * n3.rgb * MixLevel.b * bump_level;
    PN2 = TBN * n2.rgb * MixLevel.g * bump_level;
    PN1 = TBN * n1.rgb * MixLevel.r * bump_level;
    diffuse = diffuse + max(dot(PN4, L) * mk4, 0.0) * MixLevel.a ;
    diffuse = diffuse + max(dot(PN3, L) * mk3, 0.0) * MixLevel.b ;
    diffuse = diffuse + max(dot(PN2, L) * mk2, 0.0) * MixLevel.g ;
    diffuse = diffuse + max(dot(PN1, L) * mk1, 0.0) * MixLevel.r ;
    //tone down some of the over brightness.
    diffuse = sin((diffuse) * 1.570796)*.25;
    float diffuse_2 = max(dot(N, L), 0.0) * 0.5* (5.0 - (l_ambient*10.0))*0.2;
    //tone down some of the over brightness.
    diffuse_2 = sin((diffuse_2) * 1.570796)*.25;

    //----------------------------------------------------------------------
    //Mix it all.
    gl_FragColor += vAmbient * base;
    //gl_FragColor.rgb += (vec3(diffuse , diffuse , diffuse) * base.rgb);
    gl_FragColor.rgb += (vec3(diffuse , diffuse , diffuse)* base.rgb);
    gl_FragColor.rgb += (vec3(diffuse_2, diffuse_2, diffuse_2) * base.rgb);
    //----------------------------------------------------------------------
    //gamma
    vec3 vG = vec3(1.1, 1.1, 1.1);
    gl_FragColor.rgb = pow(gl_FragColor.rgb, vG / gamma);
    gl_FragColor *= 1.0;
    //gray level
    vec3 luma = vec3(0.299, 0.587, 0.114);
    vec3 co = vec3(dot(luma, gl_FragColor.rgb));
    vec3 c = mix(co, gl_FragColor.rgb, gray_level);
    gl_FragColor.rgb = c;

    // FOG calculation
    const float LOG2 = 1.442695;
    float z = gl_FragCoord.z / gl_FragCoord.w;
    float fogFactor = exp2( -gl_Fog.density * gl_Fog.density * z * z * LOG2 );
    fogFactor = clamp(fogFactor, 0.0, 1.0);
    if (enable_fog == 1)
    {
        gl_FragColor = mix(gl_Fog.color, gl_FragColor, fogFactor );
    }
    else
    {
        gl_FragColor = gl_FragColor;
    } 
  
}



