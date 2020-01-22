// used to render blended hirez terrain
//
//
#version 330 compatibility

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gPosition;
layout (location = 3) out float gFlag;

uniform sampler2D layer_1T1;
uniform sampler2D layer_2T1;
uniform sampler2D layer_3T1;
uniform sampler2D layer_4T1;

uniform sampler2D layer_1T2;
uniform sampler2D layer_2T2;
uniform sampler2D layer_3T2;
uniform sampler2D layer_4T2;

uniform sampler2D n_layer_1T1;
uniform sampler2D n_layer_2T1;
uniform sampler2D n_layer_3T1;
uniform sampler2D n_layer_4T1;

uniform sampler2D n_layer_1T2;
uniform sampler2D n_layer_2T2;
uniform sampler2D n_layer_3T2;
uniform sampler2D n_layer_4T2;

uniform sampler2D mixtexture1;
uniform sampler2D mixtexture2;
uniform sampler2D mixtexture3;
uniform sampler2D mixtexture4;

uniform sampler2D test_texture;

//low rex color map
uniform sampler2D colorMap;


uniform int has_holes;
uniform int test_mode;
// more and more crap added :)
//translation vectors
uniform vec4 layer0UT1;
uniform vec4 layer1UT1;
uniform vec4 layer2UT1;
uniform vec4 layer3UT1;

uniform vec4 layer0UT2;
uniform vec4 layer1UT2;
uniform vec4 layer2UT2;
uniform vec4 layer3UT2;


uniform vec4 layer0VT1;
uniform vec4 layer1VT1;
uniform vec4 layer2VT1;
uniform vec4 layer3VT1;

uniform vec4 layer0VT2;
uniform vec4 layer1VT2;
uniform vec4 layer2VT2;
uniform vec4 layer3VT2;

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
in vec4 mask_1;
in vec4 mask_2;
in mat3 TBN;
in float hole;
////////////////////////////////////////////////////////////////

vec4 add_norms (in vec4 n1 , in vec4 n2){
    n1.xyz += n2.xyz;
    return (n1);   
}
void main(void)
{
    // lets check for a hole before doing any math.. It saves time
    if (has_holes > 0.0)
      {
        if ( hole >0.0) {discard;}
      }
    vec4 t1;
    vec4 t2;
    vec4 t3;
    vec4 t4;

    vec4 t1_2;
    vec4 t2_2;
    vec4 t3_2;
    vec4 t4_2;

    vec4 n1;
    vec4 n2;
    vec4 n3;
    vec4 n4;

    vec4 n1_2;
    vec4 n2_2;
    vec4 n3_2;
    vec4 n4_2;

    vec2 MixLevel1;
    vec2 MixLevel2;
    vec2 MixLevel3;
    vec2 MixLevel4;
    vec4 Nmix1;
    vec4 Coutmix;
    vec4 Noutmix;
    vec2 mix_coords;

    vec3 PN1;
    vec3 PN2;
    vec3 PN3;
    vec3 PN4;

    vec2 color_uv = texCoord.xy * 0.1;
    vec3 vAmbient = vec3(l_ambient, l_ambient, l_ambient);

    //calcualte texcoords for mix texture.
    float scale = 128.0/144.0;
    mix_coords = color_uv;
    mix_coords *= scale;
    mix_coords += .05555555;// = 8/144

    // layer 4 ---------------------------------------------
    vec2 tv4;
    vec2 tv4_2;
    vec2 tv3;
    vec2 tv3_2;
    vec2 tv2;
    vec2 tv2_2;
    vec2 tv1;
    vec2 tv1_2;

    if (test_mode == 0 )
    {
    tv4 = vec2(dot(-layer3UT1, Vertex), dot(layer3VT1, Vertex));
    t4 = texture2D(layer_4T1, -tv4 + .5);
    n4 = texture2D(n_layer_4T1, -tv4 + .5);
    //
    tv4_2 = vec2(dot(-layer3UT2, Vertex), dot(layer3VT2, Vertex));
    t4_2 = texture2D(layer_4T2, -tv4_2 + .5);
    n4_2 = texture2D(n_layer_4T1, -tv4_2 + .5);

    // layer 3 ---------------------------------------------
    tv3 = vec2(dot(-layer2UT1, Vertex), dot(layer2VT1, Vertex));
    t3 = texture2D(layer_3T1, -tv3 + .5);
    n3 = texture2D(n_layer_3T1, -tv3 + .5);
    //
    tv3_2 = vec2(dot(-layer2UT2, Vertex), dot(layer2VT2, Vertex));
    t3_2 = texture2D(layer_3T2, -tv3_2 + .5);
    n3_2 = texture2D(n_layer_3T2, -tv3_2 + .5);

    // layer 2 ---------------------------------------------
    tv2 = vec2(dot(-layer1UT1, Vertex), dot(layer1VT1, Vertex));
    t2 = texture2D(layer_2T1, -tv2 + .5);
    n2 = texture2D(n_layer_2T1, -tv2 + .5);
    //
    tv2_2 = vec2(dot(-layer1UT2, Vertex), dot(layer1VT2, Vertex));
    t2_2 = texture2D(layer_2T2, -tv2_2 + .5);
    n2_2 = texture2D(n_layer_2T2, -tv2_2 + .5);

    // layer 1 ---------------------------------------------
    tv1 = vec2(dot(-layer0UT1, Vertex), dot(layer0VT1, Vertex));
    t1 = texture2D(layer_1T1, -tv1 + .5);
    n1 = texture2D(n_layer_1T1, -tv1 + .5);
    //
    tv1_2 = vec2(dot(-layer0UT2, Vertex), dot(layer0VT2, Vertex));
    t1_2 = texture2D(layer_1T2, -tv1_2 + .5);
    n1_2 = texture2D(n_layer_1T2, -tv1_2 + .5);
    //
    }
    else
    {
    tv4 = vec2(dot(-layer3UT1, Vertex), dot(layer3VT1, Vertex));
    t4 = texture2D(test_texture, -tv4 + .5);
    n4 = texture2D(n_layer_4T1, -tv4 + .5);
    //
    tv4_2 = vec2(dot(-layer3UT2, Vertex), dot(layer3VT2, Vertex));
    t4_2 = texture2D(test_texture, -tv4_2 + .5);
    n4_2 = texture2D(n_layer_4T1, -tv4_2 + .5);

    // layer 3 ---------------------------------------------
    tv3 = vec2(dot(-layer2UT1, Vertex), dot(layer2VT1, Vertex));
    t3 = texture2D(test_texture, -tv3 + .5);
    n3 = texture2D(n_layer_3T1, -tv3 + .5);
    //
    tv3_2 = vec2(dot(-layer2UT2, Vertex), dot(layer2VT2, Vertex));
    t3_2 = texture2D(test_texture, -tv3_2 + .5);
    n3_2 = texture2D(n_layer_3T2, -tv3_2 + .5);

    // layer 2 ---------------------------------------------
    tv2 = vec2(dot(-layer1UT1, Vertex), dot(layer1VT1, Vertex));
    t2 = texture2D(test_texture, -tv2 + .5);
    n2 = texture2D(n_layer_2T1, -tv2 + .5);
    //
    tv2_2 = vec2(dot(-layer1UT2, Vertex), dot(layer1VT2, Vertex));
    t2_2 = texture2D(test_texture, -tv2_2 + .5);
    n2_2 = texture2D(n_layer_2T2, -tv2_2 + .5);

    // layer 1 ---------------------------------------------
    tv1 = vec2(dot(-layer0UT1, Vertex), dot(layer0VT1, Vertex));
    t1 = texture2D(test_texture, -tv1 + .5);
    n1 = texture2D(n_layer_1T1, -tv1 + .5);
    //
    tv1_2 = vec2(dot(-layer0UT2, Vertex), dot(layer0VT2, Vertex));
    t1_2 = texture2D(test_texture, -tv1_2 + .5);
    n1_2 = texture2D(n_layer_1T2, -tv1_2 + .5);

    }
    //------------------------------------------------------------------
    
    MixLevel1.rg = texture2D(mixtexture1, mix_coords.xy).ag;
    MixLevel2.rg = texture2D(mixtexture2, mix_coords.xy).ag;
    MixLevel3.rg = texture2D(mixtexture3, mix_coords.xy).ag;
    MixLevel4.rg = texture2D(mixtexture4, mix_coords.xy).ag;
   
    vec4 empty = vec4(0.0);

      //mask out empty textures.
    t1 = mix(t1, empty, mask_1.a);
    t1_2 = mix(t1_2, empty, mask_1.b);
    t2 = mix(t2, empty, mask_1.g);
    t2_2 = mix(t2_2, empty, mask_1.r);

    t3 = mix(t3, empty, mask_2.a);
    t3_2 = mix(t3_2, empty, mask_2.b);
    t4 = mix(t4, empty, mask_2.g);
    t4_2 = mix(t4_2, empty, mask_2.r);

    //
     
    //Now we mix our color
    vec4 base = vec4(0.0);  

    if (test_mode == 0 )
    {

    base += t1 * MixLevel1.r ;
    base += t1_2 * MixLevel1.g ;
   
    base += t2 * MixLevel2.r ;
    base += t2_2 * MixLevel2.g ;

    base += t3 * MixLevel3.r ;
    base += t3_2 * MixLevel3.g ;

    base += t4 * MixLevel4.r ;
    base += t4_2 * MixLevel4.g ;
}
else
{
    base = vec4(0.4, 0.4, 0.4, 1.0);
    base += t1 * t1.a * MixLevel1.r ;
    base += t1_2 * t1_2.a * MixLevel1.g;
   
    base += t2 * t2.a * MixLevel2.r ;
    base += t2_2 * t2_2.a * MixLevel2.g;

    base += t3 * t3.a * MixLevel3.r ;
    base += t3_2 * t3_2.a * MixLevel3.g ;

    base += t4 * t4.a * MixLevel4.r ;
    base += t4_2 * t4_2.a * MixLevel4.g ;
}
    float spec;

    spec = n2.a * MixLevel1.r ;
    spec += n1_2.a * MixLevel1.g ;

    //This blends between low and highrez by distance
    //We remove the color_tex from the low rez image as its added back
    //after the lighting is calculated. This makes the colors of
    //the textures less washed out.
    //DISABLED UNTIL WE SORT THE REST OUT!
    //base.rgb = mix(texture2D(colorMap, color_uv).rgb, base.rgb, ln) ;

    //Get our normal maps.
    n1.rgb = normalize(2.0 * n1.rgb - 1.0) * MixLevel1.r;
    n1_2.rgb = normalize(2.0 * n1_2.rgb - 1.0) * MixLevel1.g;

    n2.rgb = normalize(2.0 * n2.rgb - 1.0) * MixLevel2.r;
    n2_2.rgb = normalize(2.0 * n2_2.rgb - 1.0) * MixLevel2.g;

    n3.rgb = normalize(2.0 * n3.rgb - 1.0) * MixLevel3.r;
    n3_2.rgb = normalize(2.0 * n3_2.rgb - 1.0) * MixLevel3.g;

    n4.rgb = normalize(2.0 * n4.rgb - 1.0) * MixLevel4.r;
    n4_2.rgb = normalize(2.0 * n4_2.rgb - 1.0) * MixLevel4.g;
    //flip Y axis
    n1.y *= -1.0;
    n2.y *= -1.0;
    n3.y *= -1.0;
    n4.y *= -1.0;

    n1_2.y *= -1.0;
    n2_2.y *= -1.0;
    n3_2.y *= -1.0;
    n4_2.y *= -1.0;

    n1.rgb = (TBN * n1.rgb) * MixLevel1.r;
    n1_2.rgb = (TBN * n1_2.rgb) * MixLevel1.g;

    n2.rgb = (TBN * n2.rgb) * MixLevel2.r;
    n2_2.rgb = (TBN * n2_2.rgb) * MixLevel2.g;

    n3.rgb = (TBN * n3.rgb) * MixLevel3.r;
    n3_2.rgb = (TBN * n3_2.rgb) * MixLevel3.g;

    n4.rgb = (TBN * n4.rgb) * MixLevel4.r;
    n4_2.rgb = (TBN * n4_2.rgb) * MixLevel4.g;

    //-------------------------------------------------------------
    vec3 N = normalize(n);
    
    vec4 out_n = vec4(N,0.0);

    out_n = add_norms(out_n, mix(n1, empty, mask_1.a));
    out_n = add_norms(out_n, mix(n1_2, empty, mask_1.b));
    out_n = add_norms(out_n, mix(n2, empty, mask_1.g));
    out_n = add_norms(out_n, mix(n2_2, empty, mask_1.r));
    out_n = add_norms(out_n, mix(n3, empty, mask_2.a));
    out_n = add_norms(out_n, mix(n3_2, empty, mask_2.b));
    out_n = add_norms(out_n, mix(n4, empty, mask_2.g));
    out_n = add_norms(out_n, mix(n4_2, empty, mask_2.r));

    gColor = base;
    gColor.a = 1.0;
    gNormal.xyz = normalize(out_n.xyz )*0.5+0.5;
    gNormal.a = spec;
    gPosition = world_vertex;
    gFlag =(64.0/255.0);
  
}

