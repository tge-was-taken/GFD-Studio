#version 330

// in
layout ( location = 0 ) in vec3 vPosition;

// out
out vec3 fPosition;

// uniforms
uniform mat4 view;
uniform mat4 projection;

void main()
{
	fPosition = ( view * vec4( vPosition, 1.0 ) ).xyz;
	gl_Position = projection * view * vec4( vPosition, 1.0 );
}