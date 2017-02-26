// used for fog 

uniform sampler2D map_color;
uniform sampler2D map_depth;
varying vec2 texCoords;
uniform float fog_level;
// depthSample from depthTexture.r, for instance
float linearDepth(float depthSample)
{
    float f = 4000.0;
    float n = 1.0;
    
    //depthSample = 2.0 * depthSample - 1.0;
    //float zLinear = 2.0 * zNear * zFar / (zFar + zNear - depthSample * (zFar - zNear));
    return  (2.0 * n) / (f + n - depthSample * (f - n));
}



void main()
{
    vec4 color    = texture2D(map_color, texCoords);
    vec4 depth    = vec4(texture2D(map_depth, texCoords).xyz,1.0);
    
    gl_FragColor = color;
    
  gl_FragColor.a   = 1.0;
    
}

