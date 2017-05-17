//Leaf Vertex solid color Shader

#version 330 compatibility 
layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gPosition;
layout (location = 3) out vec4 gFlag;

in vec3 N;
in vec4 Vertex;
in vec4 color;
void main (void)  
{  

    gColor = color;
    gNormal.xyz = N *0.5+0.5;
    gPosition = Vertex;
    gFlag = vec4(192.0/255.0);
}
   