// basicONEtexture_fragment.glsl
//use for rendering a single texture
#version 330 compatibility 

uniform sampler2D colorMap;
uniform sampler2D normalMap;
uniform float c_level;
uniform float gamma;

in vec2 texCoord;
in mat3 TBN;
in vec3 L;

void main(){

    vec4 color = texture2D(colorMap, texCoord);
  
    if (color.a <0.2) {discard;}
    vec3 bump = vec3(normalize(255.0/128.0 * texture2D(normalMap, texCoord).xyz -1.0));
    
    vec3 PN = normalize(TBN * bump);
    color *= color;
    float NdotL = max(dot(PN,normalize(L)),0.0);
    
    color.rgb += (NdotL * 0.15);
    
    gl_FragColor = color;
    
    // FOG calculation
    const float LOG2 = 1.442695;
    float z = gl_FragCoord.z / gl_FragCoord.w;
    float fogFactor = exp2(-gl_Fog.density * gl_Fog.density * z * z * LOG2);
    fogFactor = clamp(fogFactor, 0.0, 1.0);

    gl_FragColor = mix(gl_Fog.color, gl_FragColor, fogFactor);


}