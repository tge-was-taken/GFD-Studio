
#version 150

// in
in vec2 fTex0;

// out
out vec4 oColor;

// uniform
uniform sampler2D uDiffuse;
uniform bool uMatHasDiffuse;

void main()
{
	vec4 diffuseColor = vec4( 1.0, 1.0, 1.0, 1.0 );

	if ( uMatHasDiffuse )
		diffuseColor = texture2D( uDiffuse, fTex0 );

	oColor = diffuseColor;
} 