// trees_fragment.txt
// used to render trees
#version 330 compatibility

uniform sampler2D normalMap;
uniform sampler2D colorMap;
uniform int enable_fog;
uniform float l_texture;
uniform float gray_level;
uniform float gamma;
uniform float ambient;
in vec2 texCoord;
in vec3 norm;
in float ln;
in vec3 tangent;
in vec3 bitangent;
in vec3 g_vertexnormal;
in vec3 g_viewvector;
in vec3 lightDirection;
void main (void)
{
  //discard;
  // create TBN
    float invmax = inversesqrt( max( dot(tangent,tangent), dot(bitangent,bitangent) ));
    mat3 TBN = mat3( tangent * invmax, bitangent * invmax, -g_vertexnormal );
// Get color texture
    vec4 base = texture2D(colorMap,  texCoord);
    base.xyz *= l_texture;
    //if ( base.a <0.2 ) {discard;}
// get normalmap
    vec3 bump = texture2D(normalMap, texCoord).rgb * 2.0 - 1.0;
    float spec = texture2D(normalMap, texCoord).a;
    bump =  normalize(bump);
    bump.y*=-1.0;
    float specular = texture2D(normalMap, texCoord).a ; // specular hides in the alpha channel

    vec3 N = normalize(g_vertexnormal);
    // Get the perturbed normal
    vec3 PN = normalize( TBN * bump );
    // light position
    vec3 L = normalize(lightDirection);

//caclulate lighting

vec4 final_color = base * vec4(ambient,ambient,ambient,1.0);

float lambertTerm= max(pow(dot(PN,L),3.0) , 0.0);
float light_diffuse = 0.5;

   final_color += (light_diffuse * max(lambertTerm,0.0)*.62) * base;
   vec3 E = normalize(g_viewvector.xyz);
   vec3 R = normalize(reflect(-L, PN));
   float specular1 = pow( max(dot(R, E), 0.0), 30.0)*specular*spec*0.2;
   final_color +=  specular1;

   gl_FragColor = final_color *5.0;

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

        gl_FragColor.a = base.a;
        
    }
