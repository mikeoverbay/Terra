// writes to depth texture.
// This is not used yet.
// Im experimenting with pre-bluring the shadow map.
// if editing this in the shader editor,
// you will need to recompile V and H shaders!
 
uniform sampler2D s_texture;
uniform vec3 blurScale;
varying vec2 v_texCoord;
 
void main()
{

   vec4 color = vec4(0.0);

   color += texture2D(s_texture, v_texCoord + (vec2(-3.0) * blurScale.xy))* (1.0/64.0);
   color += texture2D(s_texture, v_texCoord + (vec2(-2.0) * blurScale.xy))* (6.0/64.0);
   color += texture2D(s_texture, v_texCoord + (vec2(-1.0) * blurScale.xy))* (15.0/64.0);
   color += texture2D(s_texture, v_texCoord + (vec2(-0.0) * blurScale.xy))* (20.0/64.0);
   color += texture2D(s_texture, v_texCoord + (vec2(1.0) * blurScale.xy))* (15.0/64.0);
   color += texture2D(s_texture, v_texCoord + (vec2(2.0) * blurScale.xy))* (6.0/64.0);
   color += texture2D(s_texture, v_texCoord + (vec2(3.0) * blurScale.xy))* (1.0/64.0);

   gl_FragColor = color;
}