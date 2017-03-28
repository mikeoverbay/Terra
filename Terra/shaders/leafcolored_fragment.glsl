//Leaf Vertex solid color Shader

#version 330 compatibility 
in vec3 N;
in vec3 normal, eyeVec;
in vec3 lightDir;
in vec4 color;

void main (void)  
{  

    vec4 final_color = color;
    final_color.a = 1.0;
    vec3 N = normalize(normal);
    vec3 L = normalize(lightDir);
    float lambertTerm = dot(N,L);
 if (gl_FrontFacing) {

    if (lambertTerm > 0.0)
    {
        final_color += 
        gl_LightSource[0].diffuse * 
        gl_FrontMaterial.diffuse * 
        lambertTerm;    
        vec3 E = normalize(eyeVec);
        vec3 R = reflect(-L, N);
        float specular = pow(max(dot(R, E), 0.0), gl_FrontMaterial.shininess*.3);
        final_color += gl_LightSource[0].specular * gl_FrontMaterial.specular * specular; 
    }
 }
    gl_FragColor = final_color;           
    //gl_FragColor.xyz = final_color.xyz *.001 + (normal.xyz);           
}
   