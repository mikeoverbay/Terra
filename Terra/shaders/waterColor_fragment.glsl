//water shader

#version 330 compatibility
layout (location = 0) out vec4 FragColor;

uniform sampler2D colorMap;
uniform sampler2D gDepthMap;
uniform float time;

in vec4 positionSS; // screen space
in mat4 matPrjInv; // inverse projection matrix
in mat4 invd_mat;
in vec4 color;

vec2 postProjToScreen(vec4 position)
{
    vec2 screenPos = position.xy / position.w;
    return 0.5 * (vec2(screenPos.x, screenPos.y) + 1);
}

// depthSample from depthTexture.r, for instance
void clip(vec3 v){
    if (v.x > 0.5 || v.x < -0.5 ) { discard; }
    if ( v.y > 0.0 ) { discard; }
    if (v.z > 0.5 || v.z < -0.5 ) { discard; }
}

void main(void) {

    vec2 uv_wrap = vec2 (6.0,6.0);

    vec2 UV = postProjToScreen(positionSS);

    float Depth = (texture2D(gDepthMap, UV).x * 2.0 - 1.0);
    // Calculate Worldposition by recreating it out of the coordinates and depth-sample
    vec4 ScreenPosition;
    ScreenPosition.xy = UV * 2.0 - 1.0;
    ScreenPosition.z = (Depth);
    ScreenPosition.w = 1.0f;


    // Transform position from screen space to world space
    vec4 WorldPosition = matPrjInv * ScreenPosition ;
    WorldPosition.xyz /= WorldPosition.w;
    WorldPosition.w = 1.0f;
    // 1 x 1 x 1

    WorldPosition = invd_mat * WorldPosition;
    WorldPosition.xyz/=WorldPosition.w;

    clip(WorldPosition.xyz);

    //==================================================================
    //Get texture UVs
    WorldPosition.xy += 0.5;
    WorldPosition.y *= -1.0;
    vec2 tc = WorldPosition.xz * 16.0;
     //=================================================================
  
    FragColor = texture2D(colorMap, tc.xy )* color;
    FragColor.a = 0.35;
 
}

