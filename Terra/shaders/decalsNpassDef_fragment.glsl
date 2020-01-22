﻿//Decals normal pass.

#version 330 compatibility

uniform sampler2D firstPassColor;
uniform sampler2D gFlag;
uniform sampler2D depthMap;
uniform sampler2D normal_in;
uniform sampler2D colorMap;
uniform sampler2D normalMap;
uniform vec2 uv_wrap;
uniform int influence;

uniform float fade_in;
uniform float fade_out;

uniform vec3 tr;
uniform vec3 bl;

in vec2 TexCoords;
in vec4 positionSS; // screen space
in vec4 positionWS; // world space

in mat4 invd_mat; // inverse decal matrix

in mat4 matPrjInv; // inverse projection matrix

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
   
   // Calculate UVs
    vec2 UV = postProjToScreen(positionSS);

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
  // sample the Depth from the Depthsampler
    float Depth = texture2D(depthMap, UV).x * 2.0 - 1.0;

    // Calculate Worldposition by recreating it out of the coordinates and depth-sample
    vec4 ScreenPosition;
    ScreenPosition.xy = UV * 2.0 - 1.0;
    ScreenPosition.z = Depth;
    ScreenPosition.w = 1.0f;

    // Transform position from screen space to world space
    vec4 WorldPosition = matPrjInv * ScreenPosition ;
    WorldPosition.xyz /= WorldPosition.w;
    WorldPosition.w = 1.0f;
    vec3 WP = WorldPosition.xyz;
    // trasform to decal original decal position and size.
    // 1 x 1 x 1
    WorldPosition = invd_mat * WorldPosition;

    clip(WorldPosition.xyz);


    vec4 normalDef = texture2D(normal_in, UV.xy );
	float firstPassAlpha = texture2D(firstPassColor, UV.xy ).a;
    //================================================
    //================================================
    //Get texture UVs
    WorldPosition.xy += 0.5;
    WorldPosition.y *= -1.0;
    float scaler = -WorldPosition.y;
    vec4 color = texture2D(colorMap, WorldPosition.xy * uv_wrap.xy);

    float alpha;
    float delta = fade_out - fade_in;
    if (fade_in != fade_out)
    {
        if (delta <0.0){ alpha = fade_out-(scaler * abs(delta)); }
        if (delta >0.0){ alpha = scaler * abs(delta) + fade_in; }
    }
    if (fade_in == fade_out) alpha = fade_in;

    color.a *= alpha*firstPassAlpha;
   
    if (color.a < 0.2) discard;
    
    vec4 bump;
    bump.xz = (texture2D(normalMap, WorldPosition.xy * uv_wrap.xy).ag);
    bump.y = clamp(sqrt(1.0 - bump.x * bump.x + bump.z * bump.z),-1.0,1.0);
    bump.xyz   = normalize(bump.xyz);
    //bump.y *= -1.0;
    bump.w = texture2D(normalMap, WorldPosition.xy * uv_wrap.xy).w; // specular is in the normal maps alpha channel
   
    vec4 bump_out;

    bump_out.xyz = normalize(bump.xyz)*0.5+0.5; // put in 0.0 to 1.0 range.
    bump_out.w = bump.w;
    gl_FragColor = mix(normalDef, bump_out,  color.a * firstPassAlpha);
    }
