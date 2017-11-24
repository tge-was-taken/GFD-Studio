
#version 150 core
#extension GL_ARB_explicit_attrib_location : enable

// in
layout( location = 0 ) in vec3 vertPos;
layout( location = 1 ) in vec3 vertNrm;
layout( location = 2 ) in vec2 vertTex0;

// out
out vec2 fragVertTex0;

// uniforms
uniform mat4 modelView;
uniform mat4 projection;

void main()
{
	fragVertTex0 = vertTex0;
    gl_Position = projection * modelView * vec4( vertPos, 1f );
}