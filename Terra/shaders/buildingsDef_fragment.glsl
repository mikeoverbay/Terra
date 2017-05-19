//Used to render all buildings and such.

#version 330 compatibility

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gPosition;
layout (location = 3) out float gFlag;

uniform sampler2D normalMap;
uniform sampler2D colorMap;
uniform sampler2D colorMap_2;
uniform int is_GAmap;
uniform int is_bumped;
uniform int is_multi_textured;
uniform int alphaRef;
uniform int alphaTestEnable;
uniform int flag;
in vec2 TC1;
in vec2 TC2;
in vec4 Vertex;
in vec3 n;
in mat3 TBN;
////////////////////////////////////////////////////////////////////////////////////////////
void main(void) {
    float alpha;
    float a;
    vec4 bump;
    // get normal map based on type

    if (is_GAmap == int(1)) {
        bump.xy = (texture2D(normalMap, TC1.st).ag);
        ;
        bump.z = sqrt(1.0 - dot(bump.xy, bump.xy));
        bump.xyz   = normalize(bump.xyz);
        a         = textureLod(normalMap, TC1.st, 0).r;
        if (alphaTestEnable != 0) {
            alpha = 1.0 - float(alphaRef / 255);
            if (a < alpha) {
                discard;
            }
        }
    } else {
        bump = (texture2D(normalMap, TC1.st));
        bump.xyz = normalize(bump.xyz);
        a = textureLod(colorMap, TC1.st, int(0)).a;
        if (a < 0.3) {discard;}
    }

    vec4 base = texture2D(colorMap, TC1.st);
    if (is_multi_textured == int(1)) {
        vec4 d2 = texture2D(colorMap_2, TC2.st);
        base *= d2;
        }
    bump.xyz = TBN * (bump.xyz * 2.0 -1.0);
    gColor = base;
    if (is_bumped ==1 ){
        gNormal.xyz =  normalize(n.xyz + bump.xyz)*0.5+0.5;
    }else{
        gNormal.xyz =  normalize(n.xyz)*0.5+0.5;
    }
    gNormal.w = 0.35;
    gPosition = Vertex;
    gFlag = ((flag)/255.0);
}