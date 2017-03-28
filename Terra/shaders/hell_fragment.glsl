// hell_fragment.txt

uniform sampler2D normalMap;
uniform sampler2D colorMap;

uniform vec3 level;
uniform int enable_fog;
uniform float l_ambient;
uniform float l_texture;
varying vec3 norm;
varying vec3 lightVec;
varying vec3 eyeVec;
varying vec2 texCoord;
varying vec3 lightPos;
varying vec3 v_pos;
void main (void)
{
vec2 texC = vec2(texCoord *.1);
float invRadius = -0.05;
   float distSqr = dot(lightVec, lightVec);
   float att = clamp(1.0 - (invRadius * sqrt(distSqr)), 0.0, 1.0);
   vec3 lVec = lightVec * inversesqrt(distSqr);
   float t_level = l_texture *.5;
   vec3 vVec = normalize(eyeVec);
   
   vec4 base = texture2D(colorMap, -texC+.001)  * t_level;
   
   vec3 bump = norm;

   vec4 vAmbient =  vec4(l_ambient,l_ambient,l_ambient,1.0);

   float diffuse = max( dot(lVec,norm*1.5), 0.1 ) * 0.5;
   
   vec4 vDiffuse = vec4(diffuse,diffuse,diffuse,1.0);//* l_ambient;   

   float specular = pow(clamp(dot(reflect(lVec, norm), -vVec), 0.2, 10.0), 100.0);   

   gl_FragColor = ( vAmbient + vDiffuse + specular) * base;


float d = distance(lightPos , v_pos);
   if (d < 500.0)
   {
   d = cos(1.5707*(d/500.0));
   }
   else
   {
   d=0.0;
   }
float d2 = distance(lightPos , v_pos);
   if (d2 < 200.0)
   {
   d2 = cos(1.5707*(d2/200.0));
   }
   else
   {
   d2=0.0;
   }
// FOG SHIT
 const float LOG2 = 1.442695;

   float z = gl_FragCoord.z / gl_FragCoord.w;
   z *= 0.4;
   float fogFactor = exp2( -gl_Fog.density * gl_Fog.density * z * z * LOG2 );
   fogFactor = clamp(fogFactor, 0.0, 1.0);
   if (enable_fog == 1)
      {
      gl_FragColor = mix(gl_Fog.color, gl_FragColor, fogFactor );
      gl_FragColor = mix(vec4(1.0, 0.0, 0.0, 0.0), gl_FragColor, 1.0-d);
      gl_FragColor = mix(gl_FragColor,(vec4(1.0, 1.0, 0.0, 0.0)  *0.6),  d2);
      }
      else
      {
      gl_FragColor = gl_FragColor;         
   };

}