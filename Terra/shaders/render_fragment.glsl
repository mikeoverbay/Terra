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
//low rex color map
uniform sampler2D colorMap;
uniform sampler2D mixtexture;
uniform sampler2D hole_texture;
uniform sampler2D dom_texture;

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

varying vec3 lightVec;
varying vec3 eyeVec;
varying vec2 texCoord;
varying vec3 g_viewvector;
varying vec3 lightDirection;
varying vec3 n;
varying vec4 Vertex;
varying float ln;
varying vec4 mask_2;
varying mat3 TBN;

////////////////////////////////////////////////////////////////
vec4 correct(in vec4 hdrColor, in float exposure){
  
    // Exposure tone mapping
    vec3 mapped = vec3(1.0) - exp(-hdrColor.rgb * exposure);
    // Gamma correction 
    mapped.rgb = pow(mapped.rgb, vec3(1.0 / gamma * 0.65));
  
     return vec4 (mapped, 1.0);
}
vec4 curves(vec4 inColor, sampler2D texCurve)
{
    return vec4(texture2D(texCurve, vec2(-inColor.r, 0.5)).r,
    texture2D(texCurve, vec2(-inColor.g, 0.5)).g,
    texture2D(texCurve, vec2(-inColor.b, 0.5)).b, inColor.a);
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
    vec2 loc;
    vec3 tc;
    vec4 kill = vec4(0.0,0.0,0.0,0.0);
    vec3 PN1;
    vec3 PN2;
    vec3 PN3;
    vec3 PN4;
    vec2 color_uv = texCoord.xy *0.1;
    vec3 vAmbient = vec3(l_ambient, l_ambient, l_ambient);

    //calcualte texcoords for mix texture.
    float scale = 256.0/272.0;
    vec2 mc;
    mc = color_uv;
    mc *= scale;
    mc += .030303030;// = 8/264
    mix_coords = mc.xy;
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
   
    //This blends between low and highrez by distance
    //We remove the color_tex from the low rez image as its added back
    //after the lighting is calculated. This makes the colors of
    //the textures less washed out.
    base.rgb = mix(texture2D(colorMap, color_uv).rgb, base.rgb, ln) ;

    //Get our normal maps.
    n1.rgb = normalize(1.9921875 * n1.rgb - 1.0) * MixLevel.r;
    n2.rgb = normalize(1.9921875 * n2.rgb - 1.0) * MixLevel.g;
    n3.rgb = normalize(1.9921875 * n3.rgb - 1.0) * MixLevel.b;
    n4.rgb = normalize(1.9921875 * n4.rgb - 1.0) * MixLevel.a;
      
    //-------------------------------------------------------------
    //Do lighting
    //There is no good way to add normals together and not destroy them.
    //We have to do the math on each one THAN mix them together.
    vec3 N = normalize(n);
    vec3 L = normalize(lightDirection);
    
    PN4 = normalize(TBN * n4.rgb);
    PN3 = normalize(TBN * n3.rgb);
    PN2 = normalize(TBN * n2.rgb);
    PN1 = normalize(TBN * n1.rgb);

    float NdotL = 0.0;
    NdotL = NdotL + max(dot(PN4, L), 0.0) * MixLevel.a;
    NdotL = NdotL + max(dot(PN3, L), 0.0) * MixLevel.b;
    NdotL = NdotL + max(dot(PN2, L), 0.0) * MixLevel.g;
    NdotL = NdotL + max(dot(PN1, L), 0.0) * MixLevel.r;
    NdotL = pow(NdotL,0.5);
 
    //diffuse += max(dot(N, L), 0.0) * (5.0 - (l_ambient*10.0))*0.04;
    NdotL = max(NdotL,0.0);
 
    //----------------------------------------------------------------------
    //Mix it all.
    NdotL = (NdotL * l_texture * 0.5) + (l_ambient * 0.5);
    gl_FragColor.rgb = base.rgb * NdotL ;
    //boost color just a tad
    gl_FragColor.rgb *= 5.0; // Makes color_tex 1/2 as strong.
    //Boost up the color to comp for loss of levels from diffuse mult.

    //================================================  
    //gray level
    vec3 luma = vec3(0.299, 0.587, 0.114);
    vec3 co = vec3(dot(luma, gl_FragColor.rgb));
    vec3 c = mix(co, gl_FragColor.rgb, gray_level);
    gl_FragColor.rgb = c;
    //================================================
    // FOG calculation
    const float LOG2 = 1.442695;
    float z = gl_FragCoord.z / gl_FragCoord.w;
    float fogFactor = exp2( -gl_Fog.density * gl_Fog.density * z * z * LOG2 );
    fogFactor = clamp(fogFactor, 0.0, 1.0);
    gl_FragColor = mix(gl_Fog.color, gl_FragColor, fogFactor );
    //================================================
    gl_FragColor *= 1.0;
    gl_FragColor = correct(gl_FragColor, 1.15);
   //gl_FragColor.rgb *= 2.0;
    //debug junk
    //gl_FragColor.rgb = gl_FragColor.rgb*.01 + diffuse*.4;
    //gl_FragColor.rgb = texture2D(layer_4, color_uv) ;
    //gl_FragColor.rgb = texture2D(layer_1, color_uv) ;

}



