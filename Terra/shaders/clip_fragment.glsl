// clip_fragment.txt
//used to render water
uniform sampler2D colorMap;
varying vec2 texCoord;
varying vec4 v;
void main (void)
{
float a = texture2D(colorMap,texCoord).a;
if (a<.1) { discard; }
    float c = v.z/v.w;
    gl_FragColor = vec4(c,c,c,1.0);          

}