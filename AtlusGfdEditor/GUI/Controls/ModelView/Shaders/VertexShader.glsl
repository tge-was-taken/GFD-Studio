
#version 330 core

// in
layout( location = 0 ) in vec3 vertPos;
layout( location = 1 ) in vec3 vertNrm;
layout( location = 2 ) in vec2 vertTex0;

// out
out vec3 fragVertPos;
out vec3 fragVertNrm;
out vec2 fragVertTex0;
out vec4 fragVertClr0;

// uniforms
uniform mat4 modelView;
uniform mat4 projection;

void main()
{
	fragVertPos = ( modelView * vec4( vertPos, 1f ) ).xyz;
	fragVertNrm = ( modelView * vec4( vertNrm, 0f ) ).xyz;
	fragVertTex0 = vertTex0;
	fragVertClr0 = vec4( 1f, 1f, 1f, 1f );
    gl_Position = projection * modelView * vec4( vertPos, 1f );
}