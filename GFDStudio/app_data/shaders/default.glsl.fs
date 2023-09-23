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
uniform sampler2D uSpecular;
uniform sampler2D uReflection;
uniform sampler2D uShadow;

// material properties
uniform int uMatFlags;
uniform vec4 uMatAmbient;
uniform vec4 uMatDiffuse;
uniform vec4 uMatEmissive;
uniform float uMatReflectivity;
uniform int DrawMethod;
uniform int alphaClip;
uniform bool uMatHasType0;
uniform bool uMatHasType1;
uniform bool uMatHasType4;
uniform int uMatType0Flags;

uniform vec4 uMatToonLightColor;
uniform float uMatToonLightThreshold;
uniform float uMatToonLightFactor;
uniform float uMatToonShadowBrightness;
uniform float uMatToonShadowThreshold;
uniform float uMatToonShadowFactor;

// material flags
const int cMF_OpaqueAlpha1   = 1 << 5;
const int cMF_OpaqueAlpha2   = 1 << 13;
const int cMF_EnableLight2   = 1 << 11;
const int cMF_HasDiffuseMap  = 1 << 20;
const int cMF_HasSpecularMap = 1 << 22;
const int cMF_HasReflectionMap = 1 << 23;
const int cMF_HasShadowMap   = 1 << 28;


float mapRange(float value, float min1, float max1, float min2, float max2) {
  return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
}

float RGBAtoGray(vec4 RGBA) {
    return (RGBA.r+RGBA.g+RGBA.b)/3.0;
}

bool hasBitFlag(int bitField, int bitFlag){
    return ((bitField & bitFlag) != 0);
}

bool hasMatFlag(int bitFlag){
    return ((uMatFlags & bitFlag) != 0);
}

vec4 DefaultShader(vec4 diffuseColor, vec4 specularColor, vec4 reflectionColor, vec4 shadowColor)
{
    vec3 lightDirection = vec3( 10.0, 45.0, 10.0 );
    //  basic shadow using normals
    float phongShadow = dot(normalize(lightDirection),normalize(fFacingNormal));
    float clampedPhongShadow = clamp(phongShadow, 0.0, 1.0);
    if (!hasMatFlag(cMF_EnableLight2))
        clampedPhongShadow = 1.0;
    vec4 shadow = clamp((vec4(clampedPhongShadow, clampedPhongShadow, clampedPhongShadow, 1.0) * vec4(shadowColor.rgb, 1.0) + uMatAmbient), 0, 1);
    // Phong specular
    vec3 ref = reflect(-normalize(lightDirection), normalize(fFacingNormal));
    float specAngle = max(dot(ref, normalize(-fPosition)), 0.0);
    vec4 specular = vec4(0.0);
    if ( uMatEmissive.a > 0.0 )
        specular = vec4(pow(specAngle, uMatEmissive.a));
    vec4 accumulatedColor = diffuseColor;
    if (hasMatFlag(cMF_HasSpecularMap) && hasMatFlag(cMF_HasReflectionMap)){
        accumulatedColor.rgb += uMatEmissive.rgb * specular.rgb;
        accumulatedColor.rgb += specularColor.r * reflectionColor.rgb * uMatReflectivity;
    }
    else{
        accumulatedColor.rgb += uMatEmissive.rgb * specular.rgb * specularColor.rgb;
        accumulatedColor.rgb += reflectionColor.rgb * uMatReflectivity;
    }
    accumulatedColor *= shadow;
    return accumulatedColor;
}

vec4 CharacterShader(vec4 diffuseColor, vec4 specularColor, vec4 reflectionColor, vec4 shadowColor)
{
    vec4 toonShadow = vec4(1.0);
    vec3 lightDirection = vec3( 90.0, 45.0, 90.0 );
    vec3 eyePos = normalize( -fPosition );
    //  basic shadow using normals
    float phongShadow = dot(normalize(lightDirection),normalize(fFacingNormal));
    float clampedPhongShadow = clamp(phongShadow, 0.0, 1.0);
    // Ramp the shadow, copypasted from p5r shader
    float D = clamp((max(clampedPhongShadow - pow(uMatToonShadowThreshold, 1.8), 0.0) * uMatToonShadowFactor), 0, 1);
    if (!hasMatFlag(cMF_EnableLight2)){
        D = 1.0;
    }
    if (uMatType0Flags != 77)
        toonShadow = vec4(D, D, D, 1.0);
    toonShadow.rgb *= shadowColor.rgb;
    // Calculate rim light, copypasted from p5r shader
    float NVW = clamp(( dot( fFacingNormal, mix( eyePos, fFacingNormal, -min( phongShadow, 0.0) ) )), 0.0, 1.0);
    float E = pow(( min( 1.0 - NVW, uMatToonLightThreshold) / uMatToonLightThreshold), uMatToonLightFactor);
    // Phong Specular
    vec3 ref = reflect(-normalize(lightDirection), normalize(fFacingNormal));
    float specAngle = max(dot(ref, normalize(-fPosition)), 0.0);
    vec4 specular = vec4(0.0);
    if ( uMatEmissive.a > 0.0 )
        specular = vec4(pow(specAngle, uMatEmissive.a));
    //colorize toon shadow, using ambient color values
    toonShadow.xyz = clamp((toonShadow.xyz + uMatAmbient.xyz), 0, 1);
    //Calculate accumated color so far
    vec4 accumulatedColor = diffuseColor;
    if (hasMatFlag(cMF_HasSpecularMap) && hasMatFlag(cMF_HasReflectionMap)){
        accumulatedColor.rgb += uMatEmissive.rgb * specular.rgb;
        accumulatedColor.rgb += specularColor.r * reflectionColor.rgb * uMatReflectivity;
    }
    else{
        accumulatedColor.rgb += uMatEmissive.rgb * specular.rgb * specularColor.rgb;
        accumulatedColor.rgb += reflectionColor.rgb * uMatReflectivity;
    }
    accumulatedColor = mix(accumulatedColor, uMatToonLightColor, E * uMatToonLightColor.a * shadowColor.a );
    vec4 addedShadow = accumulatedColor * toonShadow;
    accumulatedColor = mix(accumulatedColor, addedShadow, 1.0 - clamp(uMatToonShadowBrightness, 0, 1));
    return accumulatedColor;
}

vec4 PersonaShader(vec4 diffuseColor, vec4 specularColor, vec4 reflectionColor, vec4 shadowColor)
{
    vec4 toonShadow = vec4(1.0);
    vec3 lightDirection = vec3( 10.0, 45.0, 10.0 );
    vec3 eyePos = normalize( -fPosition );
    float phongShadow = dot(normalize(lightDirection),normalize(fFacingNormal));
    // Calculate rim light, copypasted from p5r shader
    float NVW = clamp(( dot( fFacingNormal, mix( eyePos, fFacingNormal, -min( phongShadow, 0.0) ) )), 0.0, 1.0);
    float E = pow(( min( 1.0 - NVW, uMatToonLightThreshold) / uMatToonLightThreshold), uMatToonLightFactor);
    // Phong Specular
    vec3 ref = reflect(-normalize(lightDirection), normalize(fFacingNormal));
    float specAngle = max(dot(ref, normalize(-fPosition)), 0.0);
    vec4 specular = vec4(0.0);
    if ( uMatEmissive.a > 0.0 )
        specular = vec4(pow(specAngle, uMatEmissive.a));
    //colorize toon shadow, using ambient color values
    toonShadow.rgb *= shadowColor.rgb;
    toonShadow.rgb = clamp((toonShadow.rgb + uMatAmbient.rgb), 0, 1);
    //Calculate accumated color so far
    vec4 accumulatedColor = diffuseColor;
    if (hasMatFlag(cMF_HasSpecularMap) && hasMatFlag(cMF_HasReflectionMap)){
        accumulatedColor.rgb += uMatEmissive.rgb * specular.rgb;
        accumulatedColor.rgb += specularColor.r * reflectionColor.rgb * uMatReflectivity;
    }
    else{
        accumulatedColor.rgb += uMatEmissive.rgb * specular.rgb * specularColor.rgb;
        accumulatedColor.rgb += reflectionColor.rgb * uMatReflectivity;
    }
    accumulatedColor.rgb *= toonShadow.rgb;
    accumulatedColor = mix(accumulatedColor, uMatToonLightColor, E * uMatToonLightColor.a * diffuseColor.a );
    return accumulatedColor;
}

void main()
{
    vec4 diffuseColor;
    vec4 specularColor = vec4(0.0, 0.0, 0.0, 1.0);
    vec4 reflectionColor = vec4(0.0, 0.0, 0.0, 1.0);
    vec4 shadowColor = vec4(1.0);
    vec4 resultColor = vec4(1.0);
    vec3 eyePos = normalize( -fPosition );
    vec3 reflectionCoord = reflect( eyePos, fFacingNormal);
    // setting up texture maps
    if ( hasMatFlag(cMF_HasDiffuseMap) ){
        diffuseColor = texture( uDiffuse, fTex0 );
        diffuseColor.a *= uMatDiffuse.a;
    }
    else{
        diffuseColor = uMatDiffuse;
    }
    if ( hasMatFlag(cMF_HasSpecularMap)){
        specularColor = texture( uSpecular, fTex0 );
    }
    if (hasMatFlag(cMF_HasReflectionMap) && DrawMethod != 2){ // not adding reflection to blackasalpha materials (glasses) cuz it looks ugly
        reflectionColor = texture( uReflection, reflectionCoord.xy );
    }
    if (hasMatFlag(cMF_HasShadowMap)){
        shadowColor = texture( uShadow, fTex0 );
        shadowColor.rgb = vec3(1.0 - shadowColor.r); // yes, the game only uses the red channel lmao
    }
    // selecting the shader
    if (uMatHasType0)
        resultColor = CharacterShader(diffuseColor, specularColor, reflectionColor, shadowColor);
    else if (uMatHasType1 || uMatHasType4)
        resultColor = PersonaShader(diffuseColor, specularColor, reflectionColor, shadowColor);
    else
        resultColor = DefaultShader(diffuseColor, specularColor, reflectionColor, shadowColor);

    if (DrawMethod == 2){
        resultColor.a *= resultColor.r; // black as alpha does just this ingame lol
    }
    if (DrawMethod == 1 || DrawMethod == 2 ){ // can add proper alpha blending in the future maybe
        if ( resultColor.a < 0.2 )
            discard;
    }
    if (!uMatHasType1 && !uMatHasType4){
        if (DrawMethod == 0)
            if ( resultColor.a < 0.01 )
                discard;
    }

    if (hasMatFlag(cMF_OpaqueAlpha1) && hasMatFlag(cMF_OpaqueAlpha2)){
        if ( resultColor.a < alphaClip / 256.0 )
            discard;
    }
    oColor = resultColor;
}