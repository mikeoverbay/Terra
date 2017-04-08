// viewImage shader

#extension GL_EXT_gpu_shader4 : enable

uniform int mask;
uniform sampler2D colorMap;
varying vec2 texCoord;
varying vec4 color;
////////////////////////////////////////////////////////////////
  
void main(void)

{
float mk1, mk2, mk3, mk4;
    vec4 color = texture2D(colorMap, texCoord.xy);
    mk1 = float((mask & 1) / 1);
    mk2 = float((mask & 2) / 2);
    mk3 = float((mask & 4) / 4);
    mk4 = float((mask & 8) / 8);
    float alpha = color.a;
    color.r *= mk1;
    color.g *= mk2;
    color.b *= mk3;
    if (mk4 == 0.0) {color.a = 1.0;}
    vec3 alpha_only = vec3 (0.4-alpha);
    if (mask == 8)
    {
        gl_FragColor.rgb = alpha_only ;
        gl_FragColor.a = 1.0;
    }
    else
    {
        gl_FragColor = color;
    }
}