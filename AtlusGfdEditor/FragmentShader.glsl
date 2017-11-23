
#version 330 core

// in
in vec3 fragVertPos;
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
	vec3 lightDirection = vec3( 0f, 0f, -1f );
	vec3 eyePos = normalize( -fragVertPos );
	vec3 reflection = normalize( -reflect( lightDirection, fragVertNrm ) );

	vec4 diffuseColor = vec4( 0f, 0f, 0f, 0f );
	if ( hasDiffuse )
		diffuseColor = texture2D( diffuse, fragVertTex0 );

	// Calculate ambient
	vec4 ambient = clamp( matAmbient, 0f, 0.05f );

	// Calculate diffuse
	vec4 diffuse = matDiffuse * max( dot( fragVertNrm, lightDirection), 0f );
	diffuse = clamp( diffuse, 0f, 0.05f );

	// Calculate specular
	vec4 specular = matSpecular * pow( max( dot( reflection, eyePos ), 0f ), 0.3 * 1.0f );
	specular = clamp( specular, 0f, 0.05f );

	// Calculate final fragment color
	fragColor = diffuseColor + ambient + diffuse + specular;
}