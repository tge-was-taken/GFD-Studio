#version 330

// in
layout ( location = 0 ) in vec3 vPosition;

// out
out vec3 fPosition;

// uniforms
uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
	fPosition = ( uView * vec4( vPosition, 1.0 ) ).xyz;
	gl_Position = uProjection * uView * vec4( vPosition, 1.0 );
}