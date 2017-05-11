//Terrian Chunk Markers..

#version 330 compatibility
uniform vec2 bb_tr;
uniform vec2 bb_bl;
uniform float g_size;
uniform int show_border;
uniform int show_chunks;
uniform int show_grid;
//layout (location = 0) out vec4 gColor;

in vec4 color;
in vec2 uv;
in vec4 Vertex;
void main (void)
{
    vec2 V = Vertex.xz;
    vec4 gColor;
    int flag = 0;
if (show_chunks==1){
    if (uv.x > 0.98461538 || uv.y > 0.98461538)
    {
        gl_FragColor = vec4(0.2,0.2,0.2,0.95);
        flag = 1;
    }
}//show chunls
if (show_grid==1){
    if (V.x-0.53 >= bb_bl.x && V.x+0.53 <= bb_tr.x)
    {
    if (V.y-0.53 >= bb_bl.y && V.y+0.53 <= bb_tr.y)
    {
        if (mod(int(2001.250+Vertex.x), int(g_size)) == 0.0){
            gl_FragColor = vec4(0.75,0.75,0.0,0.95);
            flag = 1;
        }
        if (mod(int(2001.25+Vertex.z), int(g_size)) == 0.0){
            gl_FragColor = vec4(0.75,0.75,0.0,0.95);
            flag = 1;
        }
    }
    }
}//show grid
if(show_border==1){
    //X border
    if (V.y-0.53 < bb_tr.y && V.y+0.53 > bb_bl.y){
        if (V.x-.53 < bb_bl.x && V.x+.53 > bb_bl.x){
                gl_FragColor = vec4(1.0,0.0,0.0,0.95);
                flag = 1;
        }
        if (V.x-.53 < bb_tr.x && V.x+.53 > bb_tr.x){
                gl_FragColor = vec4(1.0,0.0,0.0,0.95);
                flag = 1;
        }
    }
    //Y border
    if (V.x-0.53 < bb_tr.x && V.x+0.53 > bb_bl.x){
        if (V.y-.53 < bb_bl.y && V.y+.53 > bb_bl.y){
                gl_FragColor = vec4(1.0,0.0,0.0,0.95);
                flag = 1;
        }
        if (V.y-.53 < bb_tr.y && V.y+.53 > bb_tr.y){
                gl_FragColor = vec4(1.0,0.0,0.0,0.95);
                flag = 1;
        }
    }

}//show border

 if (flag == 0) {discard;}// nothing to draw so discard.
}// main
