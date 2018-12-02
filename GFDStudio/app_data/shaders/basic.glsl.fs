
#version 150

// in
in vec2 fTex0;

// out
out vec4 oColor;

// uniform
uniform sampler2D tDiffuse;
uniform bool matHasDiffuse;

void main()
{
	vec4 diffuseColor = vec4( 1.0, 1.0, 1.0, 1.0 );

	if ( matHasDiffuse )
		diffuseColor = texture2D( tDiffuse, fTex0 );

	oColor = diffuseColor;
} 