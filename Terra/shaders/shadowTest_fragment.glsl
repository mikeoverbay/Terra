// Shadow map rendering
// This is not used yet.
#version 130
uniform sampler2D depthMap;
uniform sampler2D colorMap;
varying vec3 normal, lightDir, eyeVec;
varying vec2 UV;

void main (void)
{

        
    // Suppress the reverse projection.
    gl_FragColor = texture2D(colorMap, UV.xy);
         
}