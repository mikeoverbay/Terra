// toLinear_fragment.gsls
// used to linearize depth textures to linear colors.

uniform sampler2D colorMap;
varying vec2 texCoord;


float linearDepth(float depthSample)
{
    float f = 4000.0;
    float n = 1.0;
    
    //depthSample = 2.0 * depthSample - 1.0;
    //float zLinear = 2.0 * zNear * zFar / (zFar + zNear - depthSample * (zFar - zNear));
    return  (2.0 * n) / (f + n - depthSample * (f - n));
}

void main(){

    float r = linearDepth(texture2D(colorMap, texCoord).r);
    float g = linearDepth(texture2D(colorMap, texCoord).g);
    float b = linearDepth(texture2D(colorMap, texCoord).b);
    gl_FragColor = vec4(r, g, b, 1.0);

}