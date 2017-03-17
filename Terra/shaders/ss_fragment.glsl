// ss_fragment.txt
//used to render low rez terrain
uniform sampler2D normalMap;
uniform sampler2D colorMap;

uniform vec3 level;
uniform int enable_fog;
uniform float l_ambient;
uniform float l_texture;
uniform float gray_level;
uniform float gamma;
varying vec3 norm;
varying vec2 texCoord;
varying vec3 Vertex;
void main (void)
{

vec3 L = vec3(normalize(gl_LightSource[0].position.xyz - Vertex.xyz));
L.z *= -1.0;
float texture_level = (l_texture *.5) ;

 
vec2 texC = -vec2(texCoord*0.1);
vec4 base = vec4(texture2D(colorMap, texC +.001)  * texture_level);
   
 
vec4 vAmbient =  vec4(l_ambient, l_ambient, l_ambient, 1.0);
vec3 n = normalize(norm) *.3;

float diffuse = max( dot(n,L), l_ambient);

vec4 vDiffuse =  vec4(diffuse,diffuse,diffuse,1.0);


gl_FragColor = (( vAmbient + vDiffuse ) * base) *1.5;

vec3 vG = vec3(1.0 , 1.0 , 1.0);
gl_FragColor.rgb = pow(gl_FragColor.rgb, vG/gamma);
gl_FragColor *= 12.0;

//gray level
vec3 luma = vec3(0.299, 0.587, 0.114);
vec3 co = vec3(dot(luma,gl_FragColor.rgb));
vec3 c = mix(co, gl_FragColor.rgb,gray_level);
gl_FragColor.rgb = c;

//============================
// debug
//gl_FragColor.xyz =diffuse;
//============================
// FOG calculation
 const float LOG2 = 1.442695;

   float z = gl_FragCoord.z / gl_FragCoord.w;
   float fogFactor = exp2( -gl_Fog.density * gl_Fog.density * z * z * LOG2 );
   fogFactor = clamp(fogFactor, 0.0, 1.0);
   if (enable_fog == 1)
      {
      gl_FragColor = mix(gl_Fog.color, gl_FragColor, fogFactor );
      }
      else
      {
      gl_FragColor = gl_FragColor;         
   }

}