
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

float luma(vec3 color) {
  return dot(color, vec3(0.299, 0.587, 0.114));
}

float luma(vec4 color) {
  return dot(color.rgb, vec3(0.299, 0.587, 0.114));
}

void main()
{
	if ( hasDiffuse )
	{
		vec3 lightDirection = vec3( 0f, 0f, -1f );
		vec3 eyePos = normalize( -fragVertPos );
		vec3 reflection = normalize( -reflect( lightDirection, fragVertNrm ) );

		vec4 diffuseColor = texture2D( diffuse, fragVertTex0 );

		// Calculate ambient
		vec4 ambient = clamp( matAmbient, 0f, 0.025f );

		// Calculate diffuse
		vec4 diffuse = matDiffuse * max( dot( fragVertNrm, lightDirection), 0f );
		diffuse = clamp( diffuse, 0f, 0.025f );

		// Calculate specular
		vec4 specular = matSpecular * pow( max( dot( reflection, eyePos ), 0f ), 0.3 * 0.25f );
		specular = clamp( specular, 0f, 0.025f );

		// Calculate accumated color so far
		vec4 accumulatedColor = diffuseColor + ambient + diffuse + specular;

		// Calculate rim light
		float rimWidth = 0.65f;
	
		float intensity = rimWidth - max( dot( eyePos, fragVertNrm ), 0.0 );
		intensity = max( 0.0, intensity ); // ignore rim light if negative
		
		/*
		float brightness = clamp( luma( accumulatedColor ), 0, 1f );
		intensity = intensity / ( ( brightness / 0.5f ) * 2f ); 
		*/

		vec4 rimColor = vec4( intensity * ( ( matAmbient.xyz + accumulatedColor.xyz ) / 2 ), 1.0f );

		// Calculate final fragment color
		fragColor = accumulatedColor + rimColor;
	}
	else
	{
		fragColor = matDiffuse;
	}
}