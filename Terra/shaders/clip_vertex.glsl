// clip_vertext.txt
//used to render water

varying vec2 texCoord;
uniform sampler2D colorMap;
varying vec4 v;
void main(void)
{ 
        gl_Position = ftransform();     
        v           = gl_Position.z;
        texCoord    = gl_MultiTexCoord0.xy;
}