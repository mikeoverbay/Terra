// Used for shadow matrix lookup and translation
// This is not used yet.
#version 130
#extension GL_ARB_gpu_shader5 : enable
varying vec3 norm;
uniform sampler2D depthMap;
uniform sampler2D colorMap;
varying vec3 normal, lightDir, eyeVec;
varying vec2 UV;
void main()
{   
    normal = gl_NormalMatrix * gl_Normal;
    UV.xy = gl_MultiTexCoord0.xy;
    vec4 v = texture2D(depthMap, UV.xy);
  
    v     = gl_ModelViewProjectionMatrix * v ;
    v.z -= .03;

    gl_Position = v;     
}
