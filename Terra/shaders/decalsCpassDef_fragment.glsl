//Decals color pass.

#version 330 compatibility
#extension GL_EXT_gpu_shader4 : enable

layout (location = 0) out vec4 gColor;
//layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gPosition;
uniform sampler2D gFlag;
uniform sampler2D depthMap;
//uniform sampler2D normal_in;
uniform sampler2D colorMap;
uniform vec2 uv_wrap;
uniform int influence;

uniform vec3 tr;
uniform vec3 bl;
in vec2 TexCoords;
in vec4 positionSS; // screen space
in vec4 positionWS; // world space
in mat4 invd_mat; // inverse decal matrix
in mat4 matPrjInv; // inverse projection matrix
in int flag;

void clip(vec3 v){
    if (v.x > tr.x || v.x < bl.x ) { discard; }
    if (v.y > tr.y || v.y < bl.y ) { discard; }
    if (v.z > tr.z || v.z < bl.z ) { discard; }
}

vec2 postProjToScreen(vec4 position)
{
    vec2 screenPos = position.xy / position.w;
    return 0.5 * (vec2(screenPos.x, screenPos.y) + 1);
}
void main(){
    float a_table[40];// stupid hack.. i need a better way!
    
    for (int i =0; i<39 ; i++){
        a_table[i] = 1.0;
    }
    a_table[2]=0.7;
    a_table[16] = 1.5;
    a_table[18] = 1.5;
    a_table[30] = 1.5;
    // Calculate UVs
    vec2 UV = postProjToScreen(positionSS);
    /*==================================================*/
      int flag = int(texture2D(gFlag, UV.xy).r * 255);
     if (flag != 64) {
        if (influence == 2) { discard;}
        }
     if (flag == 96) {
        if (influence != 2) { discard;}
        }
     if (flag != 128) {
        if (influence == 16) { discard;}
        }
    /*==================================================*/
    // sample the Depth from the Depthsampler
    float Depth = texture2D(depthMap, UV).x * 2.0 - 1.0;

    // Calculate Worldposition by recreating it out of the coordinates and depth-sample
    vec4 ScreenPosition;
    ScreenPosition.xy = UV * 2.0 - 1.0;
    ScreenPosition.z = (Depth);
    ScreenPosition.w = 1.0f;

    // Transform position from screen space to world space
    vec4 WorldPosition = matPrjInv * ScreenPosition ;
    WorldPosition.xyz /= WorldPosition.w;
    WorldPosition.w = 1.0f;
    // trasform to decal original and size.
    // 1 x 1 x 1
    gPosition = WorldPosition; //save this before next transform
    WorldPosition = invd_mat * WorldPosition;

    clip (WorldPosition.xyz);
    /*==================================================*/
    //Get texture UVs
    WorldPosition.xy += 0.5;
    WorldPosition.y *= -1.0;

    vec4 color = texture2D(colorMap, WorldPosition.xy*uv_wrap.xy);
    color.a *= a_table[influence];
    if (color.a < 0.05) { discard; }
    gColor = color;

    }
