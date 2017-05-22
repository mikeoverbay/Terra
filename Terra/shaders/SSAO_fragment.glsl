#version 410 core

#define KERNEL_SIZE 64


uniform sampler2D u_depthTexture;
uniform vec3 u_kernel[KERNEL_SIZE];
uniform sampler2D u_rotationNoiseTexture; 
uniform mat4 u_projectionMatrix;

in vec2 v_texCoord;
out float fragColor;

vec4 getViewPos(vec2 texCoord)
{
    // Calculate out of the fragment in screen space the view space position.
    float x = texCoord.s * 2.0 - 1.0;
    float y = texCoord.t * 2.0 - 1.0;    
    // Assume we have a normal depth range between 0.0 and 1.0
    float z = texture(u_depthTexture, texCoord).x * 2.0 - 1.0;    
    vec4 posProj = vec4(x, y, z, 1.0);    
    vec4 posView = inverse(u_projectionMatrix) * posProj;    
    posView /= posView.w;    
    return posView;
}

vec3 reconstructCSFaceNormal(vec3 C) {
    return normalize(cross(dFdx(C), dFdy(C)));
}
void main(void)
{
    // Calculate out of the current fragment in screen space the view space position.
    vec2 noise_uv = vec2 (textureSize(u_depthTexture,0)) / vec2(4.0,4.0);
    vec4 posView = getViewPos(v_texCoord);
    // Normal gathering.
    vec3 normalView = reconstructCSFaceNormal(posView.xyz);
    // Calculate the rotation matrix for the kernel.
    vec3 randomVector = normalize(texture(u_rotationNoiseTexture, v_texCoord * noise_uv).xyz * 2.0-1.0 );
    vec3 tangentView = normalize(randomVector - dot(randomVector, normalView) * normalView);
    vec3 bitangentView = cross(normalView, tangentView);
    // Final matrix to reorient the kernel depending on the normal and the random vector.
    mat3 kernelMatrix = mat3(tangentView, bitangentView, normalView); 
    // Go through the kernel samples and create occlusion factor.   
    float occlusion = 0.0;
    const int sampSize = 16;
    const float u_radius = 0.1;
    const float bias = 0.001;
    for (int i = 0; i < sampSize; i++)
    {
        // Reorient sample vector in view space ...
        vec3 K = (u_kernel[i]);
        vec3 sampleVectorView = (kernelMatrix * K);
        // ... and calculate sample point.
        vec4 samplePointView = posView + (u_radius * vec4(sampleVectorView, 0.0));        
        // Project point and calculate NDC.        
        vec4 samplePointNDC = u_projectionMatrix * samplePointView;        
        samplePointNDC /= samplePointNDC.w;        
        // Create texture coordinate out of it.
        vec2 samplePointTexCoord = samplePointNDC.xy * 0.5 + 0.5;        
        // Get sample out of depth texture
        vec4 n_pos = getViewPos(samplePointTexCoord);
        float L = length((samplePointView.xyz - n_pos.xyz));

        float zSceneNDC = (texture(u_depthTexture, samplePointTexCoord).x * 2.0 - 1.0);

        float rangeCheck = smoothstep(0.0, 1.0, u_radius/L);

        occlusion += (samplePointNDC.z > zSceneNDC + bias ? 1.0 : 0.0) * rangeCheck;

    }
    
    // No occlusion gets white, full occlusion gets black.
    occlusion = pow(1.0 - occlusion / (float(sampSize) - 1.0),1.0);

    fragColor =  occlusion;

}