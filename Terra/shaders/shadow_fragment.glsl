
#version 330 compatibility

in vec4 v;
void main()

{
	float d = v.z / v.w ;

	d = d* 0.5 + 0.5;
	
	float d2 = d * d;

	// Adjusting moments (this is sort of bias per pixel) using derivative
	float dx = dFdx(d);
	float dy = dFdy(d);
	d2 += 0.25*(dx*dx+dy*dy) ;	
    gl_FragColor  = vec4( d,d2, 0.0, 1.0 );

}