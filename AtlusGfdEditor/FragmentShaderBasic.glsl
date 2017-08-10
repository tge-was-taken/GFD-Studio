
#version 150

// in
in vec2 fragVertTex0;

// out
out vec4 fragColor;

// uniform
uniform sampler2D diffuse;
uniform bool isTextured;

void main()
{
	vec4 diffuseSample = vec4( 1f, 1f, 1f, 1f );

	if ( isTextured )
	{
		diffuseSample = texture2D( diffuse, fragVertTex0 );
	}

	fragColor = diffuseSample;
} 