// For final render of SSAO to screen;

#version 330 compatibility 

uniform sampler2D gColor;
uniform sampler2D gFlags;

in vec2 texCoords;

void main(void){

    //SSAO testing junk.
        gl_FragColor = texture(gColor, texCoords.xy).rgba;
        vec2 texelSize = 1.0 / vec2(textureSize(gFlags, 0));
        float result = 0.0;
        for (int x = -2; x < 2; ++x) 
            {
            for (int y = -2; y < 2; ++y) 
                {
                    vec2 offset = vec2(float(x), float(y)) * texelSize;
                    result += texture(gFlags, texCoords + offset).r;
                }//y
            }//x
        float r = (result / (4.0 * 4.0));

        gl_FragColor.xyz *= r;
        gl_FragColor.a = 1.0;

}