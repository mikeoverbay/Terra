//wire shader.. very basic

#version 330 compatibility

layout (location = 0) out vec4 gColor;

in vec4 color;

void main (void)
{
            gColor = color;
}
