#version 330 core

// thanks dniwetamp

// in
in vec3 fPosition;
in vec3 fNormal;
in vec3 fFacingNormal;
in vec2 fTex0;
in vec2 fTex1;
in vec2 fTex2;
in vec4 fColor0;

// out
out vec4 oColor;

// samplers
uniform sampler2D uDiffuse;
uniform sampler2D uNormal;
uniform sampler2D uSpecular;
uniform sampler2D uReflection;
uniform sampler2D uHighlight;
uniform sampler2D uGlow;
uniform sampler2D uNight;
uniform sampler2D uDetail;
uniform sampler2D uShadow;

// material properties
uniform int uMatFlags;
uniform vec4 uMatAmbient;
uniform vec4 uMatDiffuse;
uniform vec4 uMatEmissive;
uniform vec4 uMatSpecular;
uniform float uMatReflectivity;
//uniform int DrawMethod;
uniform int HighlightMapBlendMode;
uniform int alphaClip;
uniform int alphaClipMode;
uniform int uMatFlags2;
uniform int TexcoordFlags;
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
#define MatFlag_HasShadowMap     (1 << 28)
#define MatFlag_HasDetailMap     (1 << 27)
#define MatFlag_HasNightMap      (1 << 26)
#define MatFlag_HasGlowMap       (1 << 25)
#define MatFlag_HasHighlightMap  (1 << 24)
#define MatFlag_HasReflectionMap (1 << 23)
#define MatFlag_HasSpecularMap   (1 << 22)
#define MatFlag_HasNormalMap     (1 << 21)
#define MatFlag_HasDiffuseMap    (1 << 20)
#define MatFlag_NormalMapSpecular (1 << 18)
#define MatFlag_HasAttributes    (1 << 16)
#define MatFlag_AlphaTest     (1 << 13)
#define MatFlag_EnableLight2     (1 << 11)
#define MatFlag_Emissive      (1 << 8)
#define MatFlag_EnableLight      (1 << 7)
#define MatFlag_HasVertexColors  (1 << 4)

#define MatFlag2_ShadowMapAdd (1 << 1)
#define MatFlag2_ShadowMapMultiply (1 << 2)
#define MatFlag2_ColorOrLerp (1 << 7)
#define MatFlag2_ReflectMapAdd (1 << 8)
#define MatFlag2_Grayscale (1 << 9)

#define ToonFlag_LIGHTNORMALMAP (1 << 1)
#define ToonFlag_LIGHTSEMITRANS (1 << 2)
#define ToonFlag_SHADOWNORMALMAP (1 << 3)
#define ToonFlag_REMOVALLIGHTYAXIS (1 << 4)
#define ToonFlag_LIGHTNORMALALPHA (1 << 5)
#define ToonFlag_LIGHTDIFFUSEALPHA (1 << 6)
#define ToonFlag_LIGHTLIGHTALPHA (1 << 7)

const vec3 ENV_AmbientColor = vec3(0.8);
const vec3 ENV_DiffuseColor = vec3(1.0);
const vec3 ENV_SpecularColor = vec3(1.0);

int bitfieldExtract(int value, int offset, int bits) {
    return (value >> offset) & ((1 << bits) - 1);
}

bool hasMatFlag(int bitFlag) {
    return ((uMatFlags & bitFlag) != 0);
}

bool hasMatFlag2(int bitFlag) {
    return ((uMatFlags2 & bitFlag) != 0);
}

bool hasMatToonFlag(int bitFlag) {
    return ((uMatType0Flags & bitFlag) != 0);
}

vec3 calcTangent(vec3 normal, vec3 pos, vec2 texCoord) {
    vec3 tangent;
    vec3 edge1 = dFdx(pos);
    vec3 edge2 = dFdy(pos);
    vec2 deltaUV1 = dFdx(texCoord);
    vec2 deltaUV2 = dFdy(texCoord);
    float f = 1.0 / (deltaUV1.x * deltaUV2.y - deltaUV2.x * deltaUV1.y);
    tangent.x = f * (deltaUV2.y * edge1.x - deltaUV1.y * edge2.x);
    tangent.y = f * (deltaUV2.y * edge1.y - deltaUV1.y * edge2.y);
    tangent.z = f * (deltaUV2.y * edge1.z - deltaUV1.y * edge2.z);

    return normalize(tangent);
}

vec4 ShaderInfo(vec4 normalColor) {
    /*
    Calculates things:
    x - Base phong shadow (from -1 to 1)
    y - Clamped phong shadow (from 0 to 1)
    z - Phong specular higlight
    w - Rim light
    */
    float specular = 0.0;
    float phongShadow = 1.0;
    const vec3 lightDirection = normalize(vec3(90.0, 45.0, 90.0));
    vec3 eyePos = normalize(-fPosition);
    if(hasMatFlag(MatFlag_EnableLight2)) {
        phongShadow = dot(lightDirection, normalColor.xyz);
    }
    float clampedPhongShadow = clamp(phongShadow, 0.0, 1.0);
    vec3 H = normalize(eyePos + lightDirection);
    if(uMatSpecular.a > 0.0) {
        specular = pow( max( dot( H, normalColor.xyz), 0.0), uMatSpecular.a);
    }
    float baseRimLight = clamp((dot(normalColor.xyz, mix(eyePos, normalColor.xyz, -min(phongShadow, 0.0)))), 0.0, 1.0);
    float rampedRimLight = pow((min(1.0 - baseRimLight, uMatToonLightThreshold) / uMatToonLightThreshold), uMatToonLightFactor);
    return vec4(phongShadow, clampedPhongShadow, specular, rampedRimLight);
}

void DefaultShader(vec3 specularColor, vec3 reflectionColor, vec4 shadowColor, vec4 inShaderInfo) {
    //vec3 phongShadowCol = clamp((inShaderInfo.y * (1.0 - shadowColor.r) + uMatAmbient.rgb), 0, 1);
    //oColor.rgb *= phongShadowCol;                                                                                                                                         // Add shadow
    float Shadow;
    vec3 lightDiffuse = inShaderInfo.y * ENV_DiffuseColor;
    if(hasMatFlag2(MatFlag2_ShadowMapAdd)) {
        lightDiffuse = mix( ENV_DiffuseColor.rgb, lightDiffuse, shadowColor.r);
    }
    else if(hasMatFlag2(MatFlag2_ShadowMapMultiply)) {
        lightDiffuse *= 1.f - shadowColor.r;
    }
    else{
        lightDiffuse += shadowColor.rgb;
    }
    if(hasMatFlag2(MatFlag_Emissive)){
        if(hasMatFlag2(MatFlag2_ShadowMapAdd)) {
            vec3 lmAmbient = mix( ENV_AmbientColor, uMatAmbient.rgb * ENV_AmbientColor, shadowColor.r);
            oColor.rgb *= uMatEmissive.rgb + lmAmbient + uMatDiffuse.rgb * lightDiffuse;
        }
        else{
            oColor.rgb *= uMatEmissive.rgb + uMatAmbient.rgb * ENV_AmbientColor + uMatDiffuse.rgb * lightDiffuse;
        }
    }else{
        if(hasMatFlag2(MatFlag2_ShadowMapAdd)) {
            vec3 lmAmbient = mix( ENV_AmbientColor, uMatAmbient.rgb * ENV_AmbientColor, shadowColor.r);
            oColor.rgb *= lmAmbient + uMatDiffuse.rgb * lightDiffuse;
        }
        else{
            oColor.rgb *= uMatAmbient.rgb * ENV_AmbientColor + uMatDiffuse.rgb * lightDiffuse;
        }
    }
    oColor.rgb += uMatSpecular.rgb * ENV_SpecularColor * inShaderInfo.z * mix(vec3(1.0), specularColor.rgb, float(hasMatFlag(MatFlag_HasSpecularMap)));                                       // Add specular
}

void CharacterShader(vec3 specularColor, vec3 reflectionColor, vec4 shadowColor, vec4 inShaderInfo) {
    float shadowTex = 1.0 - shadowColor.r;
    float toonShadow = clamp((max(inShaderInfo.y - pow(uMatToonShadowThreshold, 1.8), 0.0) * uMatToonShadowFactor), 0, 1);
    if(hasMatFlag2(MatFlag2_ShadowMapAdd)) {
        toonShadow = clamp( toonShadow + uMatToonShadowBrightness + shadowTex, 0.0, 1.0);
    }
    else if(hasMatFlag2(MatFlag2_ShadowMapMultiply)) {
        toonShadow = clamp( toonShadow * shadowTex + uMatToonShadowBrightness, 0.0, 1.0);
    }
    else{
        toonShadow = clamp( toonShadow + uMatToonShadowBrightness, 0.0, 1.0);
    }
    vec3 toonShadowCol = mix(uMatAmbient.rgb, vec3(1.0), toonShadow);
	if(hasMatFlag2(MatFlag_Emissive)){
		oColor.rgb *= uMatEmissive.rgb + mix( uMatAmbient.rgb * ENV_AmbientColor, ENV_AmbientColor + uMatDiffuse.rgb * ENV_DiffuseColor, toonShadow);
    }else{
		oColor.rgb *= mix( uMatAmbient.rgb * ENV_AmbientColor, ENV_AmbientColor + uMatDiffuse.rgb * ENV_DiffuseColor, toonShadow);
    }
    oColor.rgb += uMatSpecular.rgb * ENV_SpecularColor * inShaderInfo.z * mix(vec3(1.0), specularColor.rgb, float(hasMatFlag(MatFlag_HasSpecularMap)));                                         // Add specular
}

void PersonaShader(vec3 specularColor, vec3 reflectionColor, vec4 shadowColor, vec4 inShaderInfo) {
    float shadowTex = 1.0 - shadowColor.r;
    float toonShadow = 1.0;
    if(hasMatFlag2(MatFlag2_ShadowMapAdd)) {
        toonShadow = clamp( toonShadow + uMatToonShadowBrightness + shadowTex, 0.0, 1.0);
    }
    else if(hasMatFlag2(MatFlag2_ShadowMapMultiply)) {
        toonShadow = clamp( toonShadow * shadowTex + uMatToonShadowBrightness, 0.0, 1.0);
    }
    vec3 toonShadowCol = mix(uMatAmbient.rgb, vec3(1.0), toonShadow);
    if(hasMatFlag2(MatFlag_Emissive)){
        if(hasMatFlag2(MatFlag2_ShadowMapAdd)) {
            vec3 lmAmbient = mix( ENV_AmbientColor, uMatAmbient.rgb * ENV_AmbientColor, shadowColor.r);
            oColor.rgb *= uMatEmissive.rgb + lmAmbient + uMatDiffuse.rgb * toonShadow;
        }
        else{
            oColor.rgb *= uMatEmissive.rgb + uMatAmbient.rgb * ENV_AmbientColor + uMatDiffuse.rgb * toonShadow;
        }
    }else{
        if(hasMatFlag2(MatFlag2_ShadowMapAdd)) {
            vec3 lmAmbient = mix( ENV_AmbientColor, uMatAmbient.rgb * ENV_AmbientColor, shadowColor.r);
            oColor.rgb *= lmAmbient + uMatDiffuse.rgb * toonShadow;
        }
        else{
            oColor.rgb *= uMatAmbient.rgb * ENV_AmbientColor + uMatDiffuse.rgb * toonShadow;
        }
    }
    oColor.rgb += uMatSpecular.rgb * ENV_SpecularColor * inShaderInfo.z * mix(vec3(1.0), specularColor.rgb, float(hasMatFlag(MatFlag_HasSpecularMap)));                                       // Add specular
}

void ApplyReflection(vec3 reflectionColor, float specularMask){
    if(hasMatFlag2(MatFlag2_ReflectMapAdd) || uMatReflectivity >= 1.0) {
        oColor.rgb += reflectionColor * uMatReflectivity * specularMask;
    }
    else{
        oColor.rgb = mix (oColor.rgb, reflectionColor, uMatReflectivity * specularMask);
    }
}

void ApplyRimLight(vec4 shaderInfo, float rimLightMask){
    if(hasMatToonFlag(ToonFlag_LIGHTSEMITRANS)){
        oColor.rgb += uMatToonLightColor.rgb * ENV_DiffuseColor * shaderInfo.w * uMatToonLightColor.a * rimLightMask;
    }
    else{
        oColor.rgb = mix(oColor.rgb, uMatToonLightColor.rgb * ENV_DiffuseColor, shaderInfo.w * uMatToonLightColor.a * rimLightMask);
    }
}

void main() {
    oColor = vec4(1.0);
    vec3 fFacingNormal_norm = normalize(fFacingNormal);
    vec4 normalColor = vec4(fFacingNormal_norm, 1.0);
    vec3 specularColor = vec3(0.0);
    vec3 reflectionColor = vec3(0.0);
    vec4 shadowColor = vec4(0.0, 0.0, 0.0, 1.0);

    // --- Setting up textures ---

    if(hasMatFlag(MatFlag_HasDiffuseMap)) {
        int texcoord = bitfieldExtract(TexcoordFlags, 0, 3);
        switch(texcoord) {
            case 0:
                oColor = texture(uDiffuse, fTex0);
                break;
            case 1:
                oColor = texture(uDiffuse, fTex1);
                break;
            case 2:
                oColor = texture(uDiffuse, fTex2);
                break;
        }
        oColor.a *= uMatDiffuse.a;
    }
    if(hasMatFlag(MatFlag_HasNormalMap)) {
        int texcoord = bitfieldExtract(TexcoordFlags, 3, 3);
        vec3 tangent = vec3(0.0);
        switch(texcoord) {
            case 0:
                normalColor = texture(uNormal, fTex0);
                tangent = calcTangent(fFacingNormal_norm, fPosition, fTex0);
                break;
            case 1:
                normalColor = texture(uNormal, fTex1);
                tangent = calcTangent(fFacingNormal_norm, fPosition, fTex1);
                break;
            case 2:
                normalColor = texture(uNormal, fTex2);
                tangent = calcTangent(fFacingNormal_norm, fPosition, fTex2);
                break;
        }
        normalColor.y = 1.0 - normalColor.y; // OpenGL moment
        vec3 bitangent = cross(fFacingNormal_norm, tangent);
        mat3 TBN = mat3(tangent, bitangent, fFacingNormal_norm);
        normalColor.xyz = normalize(normalColor.xyz * 2.0 - 1.0);
        normalColor.xyz = normalize(TBN * normalColor.xyz);
    }
    if(hasMatFlag(MatFlag_HasSpecularMap)) {
        int texcoord = bitfieldExtract(TexcoordFlags, 6, 3);
        switch(texcoord) {
            case 0:
                specularColor = texture(uSpecular, fTex0).rgb;
                break;
            case 1:
                specularColor = texture(uSpecular, fTex1).rgb;
                break;
            case 2:
                specularColor = texture(uSpecular, fTex2).rgb;
                break;
        }
    }
    if(hasMatFlag(MatFlag_HasReflectionMap)) {
        vec3 eyePos = normalize(-fPosition);
        vec3 reflectionCoord = reflect(eyePos, normalColor.xyz);
        reflectionColor = texture(uReflection, reflectionCoord.xy).rgb;
    }
    if(hasMatFlag(MatFlag_HasHighlightMap)) {
        vec4 highlightColor = vec4(0.0);
        int texcoord = bitfieldExtract(TexcoordFlags, 12, 3);
        switch(texcoord) {
            case 0:
                highlightColor = texture(uHighlight, fTex0);
                break;
            case 1:
                highlightColor = texture(uHighlight, fTex1);
                break;
            case 2:
                highlightColor = texture(uHighlight, fTex2);
                break;
        }
        switch(HighlightMapBlendMode) {
            case 1:
                oColor.rgb = mix(oColor.rgb, highlightColor.rgb, highlightColor.a * uMatAmbient.a);
                break;
            case 2:
                oColor.rgb += highlightColor.rgb * highlightColor.a * uMatAmbient.a;
                break;
            case 3:
                oColor.rgb -= highlightColor.rgb * highlightColor.a * uMatAmbient.a;
                break;
            case 4:
                oColor.rgb *= highlightColor.rgb * highlightColor.a * uMatAmbient.a;
                break;
        }
    }
    if(hasMatFlag(MatFlag_HasShadowMap)) {
        int texcoord = bitfieldExtract(TexcoordFlags, 24, 3);
        switch(texcoord) {
            case 0:
                shadowColor = texture(uShadow, fTex0);
                break;
            case 1:
                shadowColor = texture(uShadow, fTex1);
                break;
            case 2:
                shadowColor = texture(uShadow, fTex2);
                break;
        }
    }

    float specularMask = mix(specularColor.r, normalColor.a, float(hasMatFlag(MatFlag_HasNormalMap)) * float(hasMatFlag(MatFlag_NormalMapSpecular)));
    /*
    // accurate but toon flags no worky for some reason
    float rimLightMask = mix(1.0, normalColor.a, float(hasMatFlag(MatFlag_HasNormalMap)) * float(hasMatToonFlag(ToonFlag_LIGHTNORMALALPHA)));
    rimLightMask = mix(rimLightMask, oColor.a, float(hasMatToonFlag(ToonFlag_LIGHTDIFFUSEALPHA)));
    rimLightMask = mix(rimLightMask, shadowColor.a, float(hasMatFlag(MatFlag_HasShadowMap)) * float(hasMatToonFlag(ToonFlag_LIGHTLIGHTALPHA)));
    */
    // --- Selecting the shader ---

    vec4 inShaderInfo = ShaderInfo(normalColor);
    if(uMatHasType0) {
        CharacterShader(specularColor, reflectionColor, shadowColor, inShaderInfo);
        ApplyReflection(reflectionColor, specularMask);
        ApplyRimLight(inShaderInfo, shadowColor.a);
    } else if(uMatHasType1 || uMatHasType4) {
        PersonaShader(specularColor, reflectionColor, shadowColor, inShaderInfo);
        ApplyReflection(reflectionColor, specularMask);
        ApplyRimLight(inShaderInfo, oColor.a);
    } else {
        DefaultShader(specularColor, reflectionColor, shadowColor, inShaderInfo);
        ApplyReflection(reflectionColor, specularMask);
    }

    if(hasMatFlag(MatFlag_HasGlowMap)) {
        vec3 glowColor = vec3(0.0);
        int texcoord = bitfieldExtract(TexcoordFlags, 15, 3);
        switch(texcoord) {
            case 0:
                glowColor = texture(uGlow, fTex0).rgb;
                break;
            case 1:
                glowColor = texture(uGlow, fTex1).rgb;
                break;
            case 2:
                glowColor = texture(uGlow, fTex2).rgb;
                break;
        }
        oColor.rgb += glowColor;
    }
    if(hasMatFlag(MatFlag_HasNightMap)) {
        vec3 nightColor = vec3(1.0);
        int texcoord = bitfieldExtract(TexcoordFlags, 18, 3);
        switch(texcoord) {
            case 0:
                nightColor = texture(uNight, fTex0).rgb;
                break;
            case 1:
                nightColor = texture(uNight, fTex1).rgb;
                break;
            case 2:
                nightColor = texture(uNight, fTex2).rgb;
                break;
        }
        oColor.rgb *= nightColor.rgb;
    }
    if(hasMatFlag(MatFlag_HasDetailMap)) {
        vec3 detailColor = vec3(0.0);
        int texcoord = bitfieldExtract(TexcoordFlags, 21, 3);
        switch(texcoord) {
            case 0:
                detailColor = texture(uDetail, fTex0).rgb;
                break;
            case 1:
                detailColor = texture(uDetail, fTex1).rgb;
                break;
            case 2:
                detailColor = texture(uDetail, fTex2).rgb;
                break;
        }
        detailColor *= 2.0;
        oColor.rgb *= detailColor.rgb;
    }

    if(hasMatFlag2(MatFlag2_Grayscale)) {
        float gray = ( oColor.r + oColor.g + oColor.b ) / 3.f;
        oColor.rgb = mix( oColor.rgb, vec3(gray ), fColor0.a );
    }

    if(hasMatFlag(MatFlag_HasVertexColors)) {
        if(hasMatFlag2(MatFlag2_ColorOrLerp)) {
            oColor.rgb = mix(oColor.rgb, fColor0.rgb, fColor0.a);
        }
        else{
            oColor *= fColor0;
        }
    }

    // --- Handling transparency

    if(hasMatFlag(MatFlag_AlphaTest)) {
        float alphaClipFloat = alphaClip / 256.0;
        if (oColor.a < alphaClipFloat) discard;
        /*
	    if (alphaClipMode == 0) discard;
	    else if (alphaClipMode == 1 || alphaClipMode == 3)
	    	if(alphaClipFloat - oColor.a > 0) discard;
	    else if (alphaClipMode == 2)
	    	if(oColor.a != alphaClipFloat) discard;
	    else if (alphaClipMode == 4 || alphaClipMode == 6)
	    	if(oColor.a - alphaClipFloat < 0) discard;
	    else if (alphaClipMode == 5)
	    	if(oColor.a == alphaClipFloat) discard;
        */
    }
}
