#version 120
#extension GL_EXT_geometry_shader4 : enable         
 
//uniform float uNormalsLength;      
uniform int mode;
uniform float l_length; 
varying in vec3 normal[];      
void main()
{
    // assert(gl_VerticesIn == 3);
    // assert(GL_GEOMETRY_INPUT_TYPE_EXT == GL_TRIANGLES);
    // assert(GL_GEOMETRY_OUTPUT_TYPE_EXT == GL_LINE_STRIP);
    // assert(GL_GEOMETRY_VERTICES_OUT_EXT == 6);      
 if (mode == 1) {
 vec4 sumV;
 vec4 sumN;
 sumV = (gl_PositionIn[0] + gl_PositionIn[1] + gl_PositionIn[2]) / 3.0;
 sumN.xyz = (normal[0].xyz + normal[1].xyz + normal[2].xyz) / 3.0;
 sumN.w = 0.0;
        gl_Position = gl_ModelViewProjectionMatrix * sumV;
        EmitVertex();      
        gl_Position = gl_ModelViewProjectionMatrix * (sumV + (sumN * l_length));
        EmitVertex();      
 }
 else
 {
    for(int i = 0; i < gl_VerticesIn; ++i)
    {
        gl_Position = gl_ModelViewProjectionMatrix * gl_PositionIn[i];
        EmitVertex();      
 
        gl_Position = gl_ModelViewProjectionMatrix * (gl_PositionIn[i] + (vec4(normal[i], 0) * l_length));
        EmitVertex();      
 
        EndPrimitive();
    }
}
}