#version 330 core

// in
layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec3 vNormal;
layout(location = 2) in vec2 vTex0;
layout(location = 3) in vec4 vBoneWeights;
layout(location = 4) in vec4 vBoneIndices;
layout(location = 5) in vec2 vTex1;
layout(location = 6) in vec2 vTex2;
layout(location = 7) in vec4 vColor0;
layout(location = 8) in vec4 vColor1;
layout(location = 9) in vec4 vColor2;

// out
out vec3 fPosition;
out vec3 fNormal;
out vec3 fFacingNormal;
out vec2 fTex0;
out vec2 fTex1;
out vec2 fTex2;
out vec4 fColor0;

// uniforms
uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform vec4 uUVTransform;
uniform float uUVRotation;
uniform vec4 uUV1Transform;
uniform float uUV1Rotation;


// skinning
const int MAX_BONES = 96;
uniform vec4 uBones[MAX_BONES * 3];
uniform bool uIsSkinned;

// Each bone is three rows of an affine matrix; the fourth is always (0,0,0,1).
void skin( inout vec3 position, inout vec3 normal )
{
    if ( !uIsSkinned )
        return;

    vec4 point = vec4( position, 1.0 );
    vec3 skinnedPosition = vec3( 0.0 );
    vec3 skinnedNormal = vec3( 0.0 );

    for ( int i = 0; i < 4; ++i )
    {
        float weight = vBoneWeights[i];
        if ( weight == 0.0 )
            continue;

        int at = int( vBoneIndices[i] ) * 3;
        vec4 r0 = uBones[at + 0];
        vec4 r1 = uBones[at + 1];
        vec4 r2 = uBones[at + 2];

        skinnedPosition += weight * vec3( dot( r0, point ), dot( r1, point ), dot( r2, point ) );
        skinnedNormal += weight * vec3( dot( r0.xyz, normal ), dot( r1.xyz, normal ), dot( r2.xyz, normal ) );
    }

    position = skinnedPosition;
    normal = normalize( skinnedNormal );
}

void main() {
    vec3 position = vPosition;
    vec3 normal = vNormal;
    skin( position, normal );

    mat4 uModelView = uView * uModel;
    fPosition = (uModelView * vec4(position, 1.0)).xyz;
    fNormal = normal;
    fFacingNormal = (uModelView * vec4(normal, 0.0)).xyz;
    // UV animation: scale, then rotate, then offset, starting from a 0,0 origin.
    vec2 uv0 = vTex0 * uUVTransform.zw;
    float s0 = sin(uUVRotation);
    float c0 = cos(uUVRotation);
    uv0 = vec2(uv0.x * c0 - uv0.y * s0, uv0.x * s0 + uv0.y * c0);
    fTex0 = uv0 + vec2( -uUVTransform.x, uUVTransform.y );

    vec2 uv1 = vTex1 * uUV1Transform.zw;
    float s1 = sin(uUV1Rotation);
    float c1 = cos(uUV1Rotation);
    uv1 = vec2(uv1.x * c1 - uv1.y * s1, uv1.x * s1 + uv1.y * c1);
    fTex1 = uv1 + vec2( -uUV1Transform.x, uUV1Transform.y );
    fTex2 = vTex2;
    fColor0 = vColor0;
    gl_Position = uProjection * uModelView * vec4(position, 1.0);
}