// Deferred final lighitng shader
#version 330 compatibility

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gColor;
uniform sampler2D depthMap;
uniform vec3 LightPos;
uniform vec3 viewPos;
uniform float spec_level;
uniform float ambient_level;
uniform float gamma_level;
uniform float bright_level;
uniform float gray_level;
uniform float mapHeight;
uniform float water_line;

uniform int light_count;
uniform vec3 light_positions[256];
uniform vec3 light_colors[256];

in vec2 TexCoords;

vec4 correct(in vec4 hdrColor, in float exposure){  
    // Exposure tone mapping
    vec3 mapped = vec3(1.0) - exp(-hdrColor.rgb * exposure);
    // Gamma correction 
    mapped.rgb = pow(mapped.rgb, vec3(1.0 / gamma_level));  
    return vec4 (mapped, 1.0);
}

void main(void)
{        

     
    vec3 light_Color = vec3 (1.0, 1.0, 1.0);
    // Retrieve data from G-buffer
    vec3 FragPos = texture2D(gPosition, TexCoords).rgb;
    vec3 Normal = normalize(texture2D(gNormal, TexCoords).rgb*2.0-1.0);
    vec3 Albedo = texture2D(gColor, TexCoords).rgb;
    float Specular = texture2D(gNormal, TexCoords).a;
    float depth = texture2D(depthMap, TexCoords).x;
    // Then calculate lighting as usual
    // Diffuse
    vec3 lightDir = normalize(LightPos - FragPos);
    vec3 diffuse = max(dot(Normal, lightDir), 0.0) * Albedo * light_Color*1.5;

    vec3 lighting = Albedo * ambient_level * 3.0;
    gl_FragColor.rgb = lighting;
    vec3 viewDir = normalize(viewPos - FragPos);
    float viewDistance = length(viewPos - FragPos);
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    float spec = pow(max(dot(Normal, halfwayDir), 0.0), 90.0) * 0.3 * spec_level;
    vec3 specular = light_Color * spec * Specular;
    if (length(FragPos) > 0.0){ // so we dont effect the skydome!
        gl_FragColor = correct(vec4(lighting + diffuse + specular, 1.0), 3.0 * bright_level);
    }
 
    //gray level
    vec3 luma = vec3(0.299, 0.587, 0.114);
    vec3 co = vec3(dot(luma, gl_FragColor.rgb));
    vec3 c = mix(co, gl_FragColor.rgb, gray_level);
    gl_FragColor.rgb = c;
    // FOG calculation... using distance from camera and height on map.

    float height = 2.0-(sin((FragPos.y / (50.0 + mapHeight))*3.14158));
    const float LOG2 = 1.442695;
    float z = viewDistance ;
    float density = (gl_Fog.density * height) * 0.5;
    float fogFactor = exp2(-density * density * z * z * LOG2);
    fogFactor = clamp(fogFactor, 0.0, 1.0);
    vec4 fog_color = gl_Fog.color* ambient_level*3.0;
  
    gl_FragColor = mix(fog_color, gl_FragColor, fogFactor );
    const float cutoff = 15.0;
    // do all the small lights
    for(int i = 0; i < light_count; ++i)
    {
        // Diffuse
        //lp.xyz *=0.25;
        float dist =  length(light_positions[i] - FragPos);
        if (length(FragPos) > 0.0){// sow we dont affect the sky

        if (dist < cutoff) {
        if (dist <0.4) { Normal.xz *= -1.0; }
        vec3 lightDir = normalize(light_positions[i] - FragPos);

        vec3 diffuse = max(dot(Normal, lightDir), 0.0) * Albedo * light_colors[i];
        if (dist <0.5) { diffuse *= 2.0; }
        gl_FragColor.xyz += mix(diffuse.xyz, vec3(0.0), dist/cutoff );
        }
        }
    }
 
}
