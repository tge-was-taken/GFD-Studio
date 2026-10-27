#version 330

// in
layout ( location = 0 ) in vec3 vPosition;
layout(location = 3) in vec4 vBoneWeights;
layout(location = 4) in vec4 vBoneIndices;

// out
out vec3 fPosition;

// uniforms
uniform mat4 uView;
uniform mat4 uProjection;


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

void main()
{
	vec3 position = vPosition;
	vec3 normal = vec3( 0.0, 1.0, 0.0 );
	skin( position, normal );

	fPosition = ( uView * vec4( position, 1.0 ) ).xyz;
	gl_Position = uProjection * uView * vec4( position, 1.0 );
}