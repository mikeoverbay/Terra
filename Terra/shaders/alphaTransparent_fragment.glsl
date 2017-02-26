// discards base on alpha level in texture

uniform sampler2D map;
varying vec2 texCoords;
void main()
  {
    vec4 c = texture2D(map,texCoords.xy);
	if (c.a <0.2) { discard;}
	gl_FragColor = c;
  }

