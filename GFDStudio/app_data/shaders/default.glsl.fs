 #version 330 core

// thanks dniwetamp

// in
in vec3 fPosition;
in vec3 fNormal;
in vec3 fFacingNormal;
in vec2 fTex0;
in vec4 fColor0;

// out
out vec4 oColor;

// samplers
uniform sampler2D uDiffuse;

// material properties
uniform bool uMatHasDiffuse;
uniform vec4 uMatAmbient;
uniform vec4 uMatDiffuse;
uniform bool uMatHasAlphaTransparency;

float map(float value, float min1, float max1, float min2, float max2) {
  return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
}

void DefaultShader()
{
    vec4 diffuseColor;
    vec3 lightDirection = vec3( 10.0, 45, 10.0 );

    if ( uMatHasDiffuse )
    {
        diffuseColor = texture( uDiffuse, fTex0 );
        diffuseColor.a *= uMatDiffuse.a;
        if ( diffuseColor.a < 0.2 ) //alpha clip
            discard;
    }
    else
    {
        diffuseColor = uMatDiffuse;
        if ( diffuseColor.a < 0.2 ) //alpha clip
            discard;
    }

    //  basic shadow using normals
    float phongShadow = dot(normalize(lightDirection),normalize(fFacingNormal));
    float clampedPhongShadow = clamp(phongShadow, 0.0, 1.0);
    vec4 shadow = clamp((vec4(clampedPhongShadow) + uMatAmbient), 0, 1);

    oColor = diffuseColor * shadow;
}

void CharacterShader()
{
    vec4 diffuseColor;
    vec4 toonShadow;
    vec3 lightDirection = vec3( 90.0, 45.0, 90.0 );
    vec3 eyePos = normalize( -fPosition );
    vec4 toonLightColor = vec4(0.98, 0.98, 0.98, 0.36);
    float toonLightThreshold = 0.7;
    float toonLightFactor = 14.0;
    float toonShadowBrightness = 0.5; // not used by the shader, but ingame the higher the value = the brighter the shadow (duh), 1.0 is no shadow basically
    float toonShadowThreshold = 0.5;
    float toonShadowFactor = 20.0;

    if ( uMatHasDiffuse )
    {
        diffuseColor = texture( uDiffuse, fTex0 );
        diffuseColor.a *= uMatDiffuse.a;
        if ( diffuseColor.a < 0.2 ) //alpha clip
            discard;
    }
    else
    {
        diffuseColor = uMatDiffuse;
        if ( diffuseColor.a < 0.2 ) //alpha clip
            discard;
    }

    //  basic shadow using normals
    float phongShadow = dot(normalize(lightDirection),normalize(fFacingNormal));
    float clampedPhongShadow = clamp(phongShadow, 0.0, 1.0);
    // Ramp the shadow, copypasted from p5r shader
    float D = clamp((max(clampedPhongShadow - pow(toonShadowThreshold, 1.8), 0.0) * toonShadowFactor), 0, 1);
    toonShadow = vec4(D);
    // Calculate rim light, copypasted from p5r shader
    float NVW = clamp(( dot( fFacingNormal, mix( eyePos, fFacingNormal, -min( phongShadow, 0.0) ) )), 0.0, 1.0);
    float E = pow(( min( 1.0 - NVW, toonLightThreshold) / toonLightThreshold), toonLightFactor);

    // Phong Specular
    //vec3 ref = reflect(-normalize(lightDirection), normalize(fFacingNormal));
    //float specAngle = max(dot(ref, normalize(-fPosition)), 0.0);
    //vec4 specular = vec4(pow(specAngle, 1.0)); // 1.0 is glossiness, in the p5 material it's emissive color's alpha channel

    //colorize toon shadow, using ambient color values
    toonShadow.xyz = clamp((toonShadow.xyz + uMatAmbient.xyz), 0, 1);

    //Calculate accumated color so far
    vec4 accumulatedColor = mix(diffuseColor, toonLightColor, E * toonLightColor.a ) * toonShadow;

    oColor = vec4(accumulatedColor);
}

void main()
{
    CharacterShader();
}