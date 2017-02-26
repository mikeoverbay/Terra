//#Version 130
varying vec3 rgb;
varying vec3 vVertex;
varying vec3 g_vertexnormal;
varying vec3 lightDirection;
varying vec2 TC1;
varying vec2 TC2;
varying vec3 T;
varying vec3 BN;
uniform mat4 ModelMatrix1;

uniform vec3 camPos;

void main(void) {

  TC1 = gl_MultiTexCoord0.xy;
  TC2 = gl_MultiTexCoord1.xy;

  mat3 ModelMatrix = mat3(ModelMatrix1);
  T = normalize(ModelMatrix * gl_MultiTexCoord2.xyz);
  BN = normalize(ModelMatrix * gl_MultiTexCoord3.xyz);

  g_vertexnormal = normalize(ModelMatrix * gl_Normal);

  vVertex = vec3(ModelMatrix1 * gl_Vertex);

  gl_Position = ftransform();
  lightDirection = gl_LightSource[0].position.xyz - vVertex;

  rgb = lightDirection;

}