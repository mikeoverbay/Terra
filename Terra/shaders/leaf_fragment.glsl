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
in vec3 tangent;
in vec3 bitangent;
in vec3 g_vertexnormal;
in vec3 g_viewvector;
in vec3 lightDirection;
in mat3 TBN;
in vec4 vVertex;
void main (void)
{
//discard;

// Get color texture
    vec4 base = texture2D(colorMap,  texCoord);
    vec4 color = base;
    color.xyz *= l_texture;
    if ( base.a <0.2 ) {discard;}
// get normalmap
    vec3 bump = normalize((255.0/128.0) * texture2D(normalMap, texCoord).rgb  - 1.0);

    float specular = texture2D(normalMap, texCoord).a ; // specular hides in the alpha channel

    vec3 N = normalize(g_vertexnormal);
    // Get the perturbed normal
    vec3 PN = normalize( TBN * bump );
    // light position
    vec3 L = normalize(lightDirection);

//caclulate lighting
   float lambertTerm;
   vec4 final_color = color;
      if (gl_FrontFacing) {
       lambertTerm = max(dot(N,L) , 0.0) * 0.5;

       final_color +=  lambertTerm * base;
       vec3 E = normalize(g_viewvector.xyz);
       vec3 R = normalize(reflect(-L, PN));
       float specular1 = pow( max(dot(R, E), 0.0), 30.0)* specular * 0.03;
       final_color +=  specular1;
   }
   
   final_color.rgb += base.rgb * vec3(ambient,ambient,ambient);
    final_color*=final_color;
   gl_FragColor = final_color*4.0;

// gamma correction
    const float vGv = 1.0;
    vec3 vG = vec3(vGv , vGv , vGv);

    gl_FragColor.rgb = pow(gl_FragColor.rgb, vG/gamma);
    
//gray level
    vec3 luma = vec3(0.299, 0.587, 0.114);
    vec3 co = vec3(dot(luma,gl_FragColor.rgb));
    vec3 c = mix(co, gl_FragColor.rgb,gray_level);
    gl_FragColor.rgb = c;
//======================================================
//Debug junk
//gl_FragColor.xyz = ( (base.rgb ) * (lambertTerm  * 1.0));

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
    //gl_FragColor.xyz = (gl_FragColor.rgb*.001 ) + (bump.xyz  * 0.5);
    
}
