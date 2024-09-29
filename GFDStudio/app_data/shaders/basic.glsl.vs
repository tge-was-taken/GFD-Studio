#version 150 core
#extension GL_ARB_explicit_attrib_location : enable

// in
layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec3 vNormal;
layout(location = 2) in vec2 vTex0;

// out
out vec2 fTex0;
out vec3 fNormal;
out vec3 fWorldPos;

// uniforms
uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
    vec4 worldPos = uModel * vec4(vPosition, 1.0);
    gl_Position = uProjection * uView * worldPos;
    
    fWorldPos = worldPos.xyz;
    fNormal = mat3(transpose(inverse(uModel))) * vNormal;
    fTex0 = vTex0;
}