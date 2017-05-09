// render_fragment.txt
// used to render blended hirez terrain
//
//
#version 330 compatibility

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gPosition;
layout (location = 3) out vec4 gFlag;

uniform sampler2D layer_1;
uniform sampler2D layer_2;
uniform sampler2D layer_3;
uniform sampler2D layer_4;
uniform sampler2D n_layer_1;
uniform sampler2D n_layer_2;
uniform sampler2D n_layer_3;
uniform sampler2D n_layer_4;
//low rex color map
uniform sampler2D colorMap;
uniform sampler2D mixtexture;
uniform sampler2D hole_texture;


uniform int has_holes;

// more and more crap added :)
//translation vectors
uniform vec4 layer0U;
uniform vec4 layer1U;
uniform vec4 layer2U;
uniform vec4 layer3U;
uniform vec4 layer0V;
uniform vec4 layer1V;
uniform vec4 layer2V;
uniform vec4 layer3V;

uniform float l_ambient;
uniform float l_texture;
uniform float gray_level;
uniform float gamma;

in vec3 eyeVec;
in vec2 texCoord;
in vec3 n;
in vec4 Vertex;
in vec4 world_vertex;
in float ln;
in vec4 mask_2;
in mat3 TBN;
////////////////////////////////////////////////////////////////

vec4 add_norms (in vec4 n1 , in vec4 n2){
    n1.xyz += n2.xyz;
    return n1;   
}
void main(void)
{
    // lets check for a hole before doing any math.. It saves time
    float hole = texture2D(hole_texture,-texCoord/10.0).x;
    if (has_holes == 1)
      {
        if ( hole >0.0) {discard;}
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

    vec3 PN1;
    vec3 PN2;
    vec3 PN3;
    vec3 PN4;
    vec2 color_uv = texCoord.xy *0.1;
    vec3 vAmbient = vec3(l_ambient, l_ambient, l_ambient);

    //calcualte texcoords for mix texture.
    float scale = 256.0/272.0;
    mix_coords = color_uv;
    mix_coords *= scale;
    mix_coords += .030303030;// = 8/264
    // layer 4 ---------------------------------------------
    vec2 tv4;
    tv4 = vec2(dot(-layer3U, Vertex), dot(layer3V, Vertex));
    t4 = texture2D(layer_4, -tv4 + .5);
    n4 = texture2D(n_layer_4, -tv4 + .5);
    // layer 3 ---------------------------------------------
    vec2 tv3;
    tv3 = vec2(dot(-layer2U, Vertex), dot(layer2V, Vertex));
    t3 = texture2D(layer_3, -tv3 + .5);
    n3 = texture2D(n_layer_3, -tv3 + .5);
    // layer 2 ---------------------------------------------
    vec2 tv2;
    tv2 = vec2(dot(-layer1U, Vertex), dot(layer1V, Vertex));
    t2 = texture2D(layer_2, -tv2 + .5);
    n2 = texture2D(n_layer_2, -tv2 + .5);
    // layer 1 ---------------------------------------------
    vec2 tv1;
    tv1 = vec2(dot(-layer0U, Vertex), dot(layer0V, Vertex));
    t1 = texture2D(layer_1, -tv1 + .5);
    n1 = texture2D(n_layer_1, -tv1 + .5);
    //------------------------------------------------------------------
    
    MixLevel = texture2D(mixtexture, mix_coords.xy).rgba;
   
    //Which ever is the Color_Tex can't be translated.
    t1 = mix(texture2D(layer_1, color_uv), t1, mask_2.r);
    t2 = mix(texture2D(layer_2, color_uv), t2, mask_2.g);
    t3 = mix(texture2D(layer_3, color_uv), t3, mask_2.b);
    t4 = mix(texture2D(layer_4, color_uv), t4, mask_2.a);
    //
     
    //Now we mix our color
    vec4 base;
    base  = t4 * MixLevel.a ;
    base += t3 * MixLevel.b ;
    base += t2 * MixLevel.g ;
    base += t1 * MixLevel.r ;
   

    float spec;
    spec  = n4.a * MixLevel.a ;
    spec += n3.a * MixLevel.b ;
    spec += n2.a * MixLevel.g ;
    spec += n1.a * MixLevel.r ;

    //This blends between low and highrez by distance
    //We remove the color_tex from the low rez image as its added back
    //after the lighting is calculated. This makes the colors of
    //the textures less washed out.
    base.rgb = mix(texture2D(colorMap, color_uv).rgb, base.rgb, ln) ;

    //Get our normal maps.
    n1.rgb = normalize(2.0 * n1.rgb - 1.0) * MixLevel.r;
    n2.rgb = normalize(2.0 * n2.rgb - 1.0) * MixLevel.g;
    n3.rgb = normalize(2.0 * n3.rgb - 1.0) * MixLevel.b;
    n4.rgb = normalize(2.0 * n4.rgb - 1.0) * MixLevel.a;
    //flip Y axis
    n1.y *= -1.0;
    n2.y *= -1.0;
    n3.y *= -1.0;
    n4.y *= -1.0;
    n1.rgb = TBN * n1.rgb * MixLevel.r;
    n2.rgb = TBN * n2.rgb * MixLevel.g;
    n3.rgb = TBN * n3.rgb * MixLevel.b;
    n4.rgb = TBN * n4.rgb * MixLevel.a;
    //-------------------------------------------------------------
    vec3 N = normalize(n);
    
    vec4 out_n = vec4(N,0.0);
    out_n = add_norms(out_n, n1 );
    out_n = add_norms(out_n, n2 );
    out_n = add_norms(out_n, n3 );
    out_n = add_norms(out_n, n4 );

  
    gColor = base;
    gNormal.xyz = normalize(out_n.xyz)*0.5+0.5;
    gNormal.a = spec;
    gPosition = world_vertex;
    gFlag = vec4(64.0/255.0);
  
}



