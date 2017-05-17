#version 410 core

#define KERNEL_SIZE 64


uniform sampler2D u_normalTexture;
uniform sampler2D u_depthTexture;

uniform vec3 u_kernel[KERNEL_SIZE];

uniform sampler2D u_rotationNoiseTexture; 

uniform vec2 u_rotationNoiseScale;
uniform vec2 screen_size;

in mat4 u_inverseProjectionMatrix;
uniform mat4 u_projectionMatrix;

in vec2 v_texCoord;

out vec4 fragColor;
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

void main(void)
{
    // Calculate out of the current fragment in screen space the view space position.
    vec2 noise_uv = vec2 (textureSize(u_depthTexture,0)) / vec2(4.0,4.0);
    vec4 posView = getViewPos(v_texCoord);
    // Normal gathering.
    vec3 normalView = normalize(texture(u_normalTexture, v_texCoord).xyz );
    // Calculate the rotation matrix for the kernel.
    vec3 randomVector = normalize(texture(u_rotationNoiseTexture, v_texCoord * noise_uv).xyz );
    vec3 tangentView = normalize(randomVector - dot(randomVector, normalView) * normalView);
    vec3 bitangentView = cross(normalView, tangentView);
    // Final matrix to reorient the kernel depending on the normal and the random vector.
    mat3 kernelMatrix = mat3(tangentView, bitangentView, normalView); 
    // Go through the kernel samples and create occlusion factor.   
    float occlusion = -5.0;
    const int sampSize = 16;
    const float u_radius = 0.08;
    const float bias = 0.00005;
    for (int i = 0; i < sampSize; i++)
    {
        // Reorient sample vector in view space ...
        vec3 sampleVectorView = normalize(kernelMatrix * u_kernel[i]);
        // ... and calculate sample point.
        vec4 samplePointView = posView + u_radius * vec4(sampleVectorView, 0.0);
        
        // Project point and calculate NDC.
        
        vec4 samplePointNDC = u_projectionMatrix * samplePointView;
        
        samplePointNDC /= samplePointNDC.w;
        
        // Create texture coordinate out of it.
        
        vec2 samplePointTexCoord = samplePointNDC.xy * 0.5 + 0.5;   
        
        // Get sample out of depth texture
        float L = length(samplePointView.xyz - posView.xyz);
        float zSceneNDC = (texture(u_depthTexture, samplePointTexCoord).r * 2.0 - 1.0);

        float rangeCheck = smoothstep(0.0, 1.0, u_radius / L);

        occlusion += (samplePointNDC.z >= zSceneNDC + bias ? 1.0 : 0.0) * rangeCheck;

    }
    
    // No occlusion gets white, full occlusion gets black.
    occlusion = pow(1.0 - occlusion / (float(sampSize) - 1.0), 3.0);

    fragColor =  vec4(occlusion,occlusion,occlusion,1.0);

}