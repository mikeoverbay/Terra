// basicONEtexture_fragment.glsl
//use for rendering a single texture
uniform sampler2D colorMap;
varying vec2 texCoord;


void main(){
    gl_FragColor = texture2D(colorMap, texCoord);
}