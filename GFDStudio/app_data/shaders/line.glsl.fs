#version 330

// in
in vec3 fPosition;

// out
out vec4 oColor;

// uniforms
uniform vec4 uColor;
uniform float uMinZ;

void main()
{
    vec4 backColor = vec4( 0.82745098039215686274509803921569, 0.82745098039215686274509803921569, 0.82745098039215686274509803921569, 1.0 );
    float blend = ( fPosition.z / uMinZ ) * 8;
	oColor.r = min( uColor.r * blend, backColor.r );
    oColor.g = min( uColor.g * blend, backColor.g );
    oColor.b = min( uColor.b * blend, backColor.b );
}