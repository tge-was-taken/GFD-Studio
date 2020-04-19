
#version 330 core

// in
layout( location = 0 ) in vec3 vPosition;
layout( location = 1 ) in vec3 vNormal;
layout( location = 2 ) in vec2 vTex0;
layout( location = 3 ) in vec4 vBoneWeights;
layout( location = 4 ) in uvec4 vBoneIndices;

// out
out vec3 fPosition;
out vec3 fNormal;
out vec2 fTex0;
out vec4 fColor0;

// uniforms
uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
	mat4 uModelView = uView * uModel;
	fPosition = ( uModelView * vec4( vPosition, 1.0 ) ).xyz;
	fNormal = ( uModelView * vec4( vNormal, 0.0 ) ).xyz;
	fTex0 = vTex0;
	fColor0 = vec4( 1.0, 1.0, 1.0, 1.0 );
	gl_Position = uProjection * uModelView * vec4( vPosition, 1.0 );
}