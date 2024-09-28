#version 150 core

// in
in vec2 fTex0;
in vec3 fNormal;
in vec3 fWorldPos;

// out
out vec4 oColor;

// uniforms
uniform sampler2D uDiffuse;
uniform bool uMatHasDiffuse;
uniform mat4 uView;
uniform vec3 uLightPos = vec3(0.0, 10.0, 10.0);  // Default light position
uniform vec3 uLightColor = vec3(1.0, 1.0, 1.0);  // Default white light
uniform vec3 uMaterialColor = vec3(0.7, 0.7, 0.7);  // Default grey material

void main()
{
    vec3 viewPos = -vec3(uView[3]) * mat3(uView);
    vec3 lightPos = viewPos;
    vec3 normal = normalize(fNormal);
    vec3 lightDir = normalize(lightPos - fWorldPos);
    vec3 viewDir = normalize(viewPos - fWorldPos);
    
    // Ambient
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * uLightColor;
    
    // Diffuse
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = diff * uLightColor;
    
    // Specular
    float specularStrength = 1.0;
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0);
    vec3 specular = specularStrength * spec * uLightColor;
    
    // Combine lighting
    vec3 result = (ambient + diffuse + specular) * uMaterialColor;
    
    // Apply texture if available
    if (uMatHasDiffuse)
    {
        vec4 texColor = texture(uDiffuse, fTex0);
        result *= texColor.rgb;
    }
    
    oColor = vec4(result, 1.0);
}