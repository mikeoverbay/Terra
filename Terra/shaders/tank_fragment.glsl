// tankShader_fragment.txt
//used to render tanks

varying vec3 normal, eyeVec;
#define MAX_LIGHTS 8
#define NUM_LIGHTS 1
varying vec3 lightDir;
void main (void)
{

    vec4 final_color = gl_FrontColor;
    vec3 N = normalize(normal);
    vec3 L = normalize(lightDir);
    float lambertTerm = dot(N,L);
    if (lambertTerm > 0.0)
        {
            final_color += 
            gl_LightSource[0].diffuse * 
            gl_FrontMaterial.diffuse * 
            lambertTerm;	
            vec3 E = normalize(eyeVec);
            vec3 R = reflect(-L, N);
            float specular = pow(max(dot(R, E), 0.0), gl_FrontMaterial.shininess);
            final_color += gl_LightSource[0].specular * gl_FrontMaterial.specular * specular;	
        }
    // FOG calculation
    const float LOG2 = 1.442695;
    float z = gl_FragCoord.z / gl_FragCoord.w;
    float fogFactor = exp2( -gl_Fog.density * gl_Fog.density * z * z * LOG2 );
    fogFactor = clamp(fogFactor, 0.0, 1.0);
    gl_FragColor.rgb = mix(gl_Fog.color.rgb, gl_FragColor.rgb, fogFactor );
}