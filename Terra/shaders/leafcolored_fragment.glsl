    // leaf fragment shader
    #version 330 compatibility 
    uniform sampler2D colorMap;
    in vec4 color;
    //out vec4 FragColor;
    void main (void)
    {
        gl_FragColor = color;   
    }
