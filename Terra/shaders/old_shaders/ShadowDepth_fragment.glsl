// writes to depth texture.
// This is not used yet.
// Im experimenting with pre-bluring the shadow map.
varying vec4 v_position;
varying vec4 color;

void main()
{
    if (color.r == 0.0) {
    gl_FragColor = vec4(0.0);
    }
    else
    {
    gl_FragColor = vec4( v_position.x, v_position.y, v_position.z, 1.0 );
    }
}