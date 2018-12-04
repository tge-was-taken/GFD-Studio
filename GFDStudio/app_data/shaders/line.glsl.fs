#version 330

// in
in vec3 fPosition;

// out
out vec4 outColor;

// uniforms
uniform vec4 color;
uniform float minZ;

void main()
{
    vec4 backColor = vec4( 0.82745098039215686274509803921569, 0.82745098039215686274509803921569, 0.82745098039215686274509803921569, 1.0 );
    float blend = ( fPosition.z / minZ ) * 8;
	outColor.r = min( color.r * blend, backColor.r );
    outColor.g = min( color.g * blend, backColor.g );
    outColor.b = min( color.b * blend, backColor.b );
}