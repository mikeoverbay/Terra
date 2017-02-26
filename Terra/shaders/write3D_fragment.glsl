// writes to 3d fbo texture.

uniform sampler2D map;
uniform int flag;
varying vec4 v;
varying vec2 texCoords;
void main()
{  
    //if (a < 0.02) { discard; }
    if (flag ==1){
            
        gl_FragColor.x = v.x;
        gl_FragColor.y = v.y;
        gl_FragColor.z = v.z;
    }
    else
    {
        gl_FragColor.w = texture2D(map,  texCoords*2.0).a; 
    }
}