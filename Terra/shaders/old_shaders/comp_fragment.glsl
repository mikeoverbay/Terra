//comp_fragment.glsl
//used for shading untextured models, trees, terrain and decals
//phong shader.. no texture
#version 120
varying vec3 N;
varying vec3 normal, eyeVec;
varying vec3 lightDir;
varying vec4 color;
void main (void)  
{  
    vec4 final_color = color;
    vec3 N = normalize(normal);
    vec3 L = normalize(lightDir);
    float lambertTerm = dot(N,L);
    if (lambertTerm > 0.0)
        {
        final_color += gl_LightSource[0].diffuse * gl_FrontMaterial.diffuse * lambertTerm;    
        vec3 E = normalize(eyeVec);
        vec3 R = reflect(-L, N);
        float specular = pow(max(dot(R, E), 0.0), gl_FrontMaterial.shininess*.3);
        final_color += gl_LightSource[0].specular * gl_FrontMaterial.specular * specular; 
        }
    gl_FragColor = final_color;           
}
    