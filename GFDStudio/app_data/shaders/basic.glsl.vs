
#version 150 core
#extension GL_ARB_explicit_attrib_location : enable

// in
layout( location = 0 ) in vec3 vPosition;
layout( location = 1 ) in vec3 vNormal;
layout( location = 2 ) in vec2 vTex0;

// out
out vec2 fTex0;

// uniforms
uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
	fTex0 = vTex0;
    gl_Position = uProjection * uView * uModel * vec4( vPosition, 1f );
}