#version 330 core

// in
layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec3 vNormal;
layout(location = 2) in vec2 vTex0;
layout(location = 3) in vec4 vBoneWeights;
layout(location = 4) in uvec4 vBoneIndices;
layout(location = 5) in vec2 vTex1;
layout(location = 6) in vec2 vTex2;
layout(location = 7) in vec4 vColor0;
layout(location = 8) in vec4 vColor1;
layout(location = 9) in vec4 vColor2;

// out
out vec3 fPosition;
out vec3 fNormal;
out vec3 fFacingNormal;
out vec2 fTex0;
out vec2 fTex1;
out vec2 fTex2;
out vec4 fColor0;

// uniforms
uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

void main() {
    mat4 uModelView = uView * uModel;
    fPosition = (uModelView * vec4(vPosition, 1.0)).xyz;
    fNormal = vNormal.xyz;
    fFacingNormal = (uModelView * vec4(vNormal, 0.0)).xyz;
    fTex0 = vTex0;
    fTex1 = vTex1;
    fTex2 = vTex2;
    fColor0 = vColor0.bgra;
    gl_Position = uProjection * uModelView * vec4(vPosition, 1.0);
}