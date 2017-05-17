//water shader

#version 330 compatibility

uniform sampler2D colorMap;
uniform sampler2D normalMap;
uniform sampler2D normalMap2;
uniform sampler2D gNormalIn;
uniform sampler2D gDepthMap;
uniform float time;
uniform float aspect;
in vec4 positionSS; // screen space
in mat4 matPrjInv; // inverse projection matrix
in vec3 n;
in mat3 TBN;
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
    //water images
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
    vec2 tc = WorldPosition.xz * 8.0;
    tc.x*= aspect;
     //=================================================================
    vec4 b1,b2;
    b1.xy = (texture2D(normalMap, tc.xy ).ga);        
    b1.z = sqrt(1.0 - ((b1.x*b1.x) + (b1.y*b1.y)));
    b1.a  = texture2D(normalMap, tc.xy ).r;

    b2.xy = (texture2D(normalMap2, tc.xy ).ga);        
    b2.z = sqrt(1.0 - ((b2.x*b2.x) + (b2.y*b2.y)));
    b2.a  = texture2D(normalMap2, tc.xy).r;
    
    float p = 0.75;
    vec4 bump =  pow(mix(b1, b2 , time),vec4(p,p,p,p)); // blend from b1 to b2 based on time sample.

    vec4 normalDef = texture2D(gNormalIn, tc.xy+ (bump.xy*.10) );
    float alpha = texture2D(colorMap, tc.xy + (bump.xy*.1)).a;
    alpha*=1.0;

    bump.xyz = normalize(TBN * bump.xyz);
    normalDef.xyz = normalize(normalDef.xyz * TBN);
   
    gl_FragColor.xyz =mix(normalDef.xyz, bump.xyz+n, alpha) ;
    gl_FragColor.w = 2.0;
}

