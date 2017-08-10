
#version 330 core

// in
in vec3 fragVertNrm;
in vec2 fragVertTex0;
in vec4 fragVertClr0;

// out
out vec4 fragColor;

// samplers
uniform sampler2D diffuse;

// texture settings
uniform bool hasDiffuse;

// material properties
uniform vec4 matAmbient;
uniform vec4 matDiffuse;
uniform vec4 matSpecular;
uniform vec4 matEmissive;

void main()
{
	vec4 diffuseSample = vec4( 1f, 1f, 1f, 1f );

	if ( hasDiffuse )
	{
		diffuseSample = texture2D( diffuse, fragVertTex0 );
	}

	// simple lambert diffuse lighting
	vec3 lightDirection = vec3( 0f, 0f, -1f );
	float lightIntensity = 1.2f;

	float lambertDiffuse = max( 0.0f, dot( lightDirection, fragVertNrm ) );
	lambertDiffuse = clamp( lambertDiffuse * lightIntensity, 0.75f, 2.0f );

	fragColor = vec4( ( diffuseSample * fragVertClr0 * lambertDiffuse ).rgb, diffuseSample.a );
} 