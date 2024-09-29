#version 330 core

// -----Constants-----
#define FLAG0_SHADOWHATCHING_REF_ALPHA_BASECOLOR 0
#define FLAG1_MATERIAL_LIGHT 1
#define FLAG1_MATERIAL_TRANSPARENCY 0
#define FLAG2_PUNCHTHROUGH 0
#define FLAG2_RAMP_REFLIGHTDIRECTION 1
#define FLAG2_EDGE_BACKLIGHT 1
#define FLAG2_EDGE_REFERENCE_NORMALALPHA 1
#define FLAG2_EDGE_SEMITRANS 0

//const float matMetallic = 0.0;
//const float matSpecularThreshold = 1.0;
//const float matSpecularPower = 10.0;
//const float matRoughness = 1.0;
//const float matRampAlpha = 1.0;
//const float shadowThreshold = 0.5;
//const float shadowFactor = 20.0;
//const float edgeThreshold = 0.75;
//const float edgeFactor = 20;

const vec3 lightNeg = normalize(vec3(90.0, 45.0, 90.0));
//const vec4 matBaseColor = vec4(1.0);
//const vec3 matSpecularColor = vec3(1.0);
//const vec4 matShadowColor = vec4 (0.5);
//const vec4 matEmissiveColor = vec4(0.0);
//const vec4 matEdgeColor = vec4(0.98, 0.98, 0.98, 1.0);
vec4 sceneEnvColorToon = vec4(1.0);

const vec4 lightColor = vec4(1.0);
const vec4 lightAmbient = vec4(0.5);
const float lightShadowAlpha = 0.5;
const float lightSpecularIntensity = 1.0;
// -------------------


// in
in vec3 fPosition;
//in vec3 fNormal;
in vec3 fFacingNormal;
in vec2 fTex0;
in vec2 fTex1;
in vec2 fTex2;
in vec4 fColor0;
in vec4 fColor1;
//in vec4 fColor2;

// out
out vec4 oColor;

// samplers
uniform sampler2D uDiffuse;
uniform sampler2D uNormal;
uniform sampler2D uSpecular;
uniform sampler2D uReflection;
uniform sampler2D uHighlight;
uniform sampler2D uGlow;
//uniform sampler2D uNight;
uniform sampler2D uDetail;
uniform sampler2D uShadow;

// material properties
uniform int uMatFlags;
//uniform vec4 uMatAmbient;
//uniform vec4 uMatDiffuse;
//uniform vec4 uMatEmissive;
//uniform vec4 uMatSpecular;
//uniform float uMatReflectivity;
//uniform int DrawMethod;
uniform int HighlightMapBlendMode;
//uniform int alphaClip;
//uniform int alphaClipMode;
//uniform int uMatFlags2;
uniform int TexcoordFlags;
//uniform bool uMatHasType0;
//uniform bool uMatHasType1;
//uniform bool uMatHasType4;
//uniform int uMatType0Flags;

//uniform vec4 uMatToonLightColor;
//uniform float uMatToonLightThreshold;
//uniform float uMatToonLightFactor;
//uniform float uMatToonShadowBrightness;
//uniform float uMatToonShadowThreshold;
//uniform float uMatToonShadowFactor;

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

// METAPHOR
uniform float matMetallic;
uniform float matSpecularThreshold;
uniform float matSpecularPower;
uniform float matRoughness;
uniform float matRampAlpha;
uniform float shadowThreshold;
uniform float shadowFactor;
uniform float edgeThreshold;
uniform float edgeFactor;

uniform vec4 matBaseColor;
uniform vec3 matSpecularColor;
uniform vec4 matShadowColor;
uniform vec4 matEmissiveColor;
uniform vec4 matEdgeColor;

/*int swapEndianness(int value) {
	return ((value >> 24) & 0x000000FF) |
		   ((value >> 8)  & 0x0000FF00) |
		   ((value << 8)  & 0x00FF0000) |
		   ((value << 24) & 0xFF000000);
}*/

int bitfieldExtract(int value, int offset, int bits) {
	return (value >> offset) & ((1 << bits) - 1);
}

bool hasMatFlag(int bitFlag) {
	return ((uMatFlags & bitFlag) != 0);
}

vec3 sRGBToLinear( vec3 color ) 
{
	//return pow( color, vec3(2.2) );
	return color;
}

vec4 sRGBToLinear( vec4 color ) 
{
	return vec4( sRGBToLinear( color.rgb ), color.a );
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

const mat4 mtrx_dither = mat4(
	vec4(0.0625, 0.5625, 0.1875,  0.6875),
	vec4(0.8125, 0.3125, 0.9375,  0.4375),
	vec4(0.25, 0.75, 0.125, 0.625),
	vec4(1.0, 0.5, 0.875,  0.375));

float get_dither_value(int x, int y) {
	return mtrx_dither[x][y];
}

void main() {
	vec4 baseColor = vec4(1.0);
	if(hasMatFlag(MatFlag_HasDiffuseMap)) {
		int texcoord = bitfieldExtract(TexcoordFlags, 0, 3);
		switch(texcoord) {
			case 0:
				baseColor = sRGBToLinear(texture(uDiffuse, fTex0));
				break;
			case 1:
				baseColor = sRGBToLinear(texture(uDiffuse, fTex1));
				break;
			case 2:
				baseColor = sRGBToLinear(texture(uDiffuse, fTex2));
				break;
		}
	}
	baseColor *= matBaseColor;
	#if (FLAG2_PUNCHTHROUGH)
		float bayerThreshold = get_dither_value(int(gl_FragCoord.x) % 4, int(gl_FragCoord.y) % 4);
		if( baseColor.a < bayerThreshold )
			discard;
	#endif
		float metallic  = matMetallic;
		float specular  = 1.0;
		float emissive = 1.0;
		float roughness      = matRoughness;
		if(hasMatFlag(MatFlag_HasGlowMap)) {
			vec4 toonParams2 = vec4(0.0);
			int texcoord = bitfieldExtract(TexcoordFlags, 15, 3);
			switch(texcoord) {
				case 0:
					toonParams2 = texture(uGlow, fTex0);
					break;
				case 1:
					toonParams2 = texture(uGlow, fTex1);
					break;
				case 2:
					toonParams2 = texture(uGlow, fTex2);
					break;
			}
			metallic  = matMetallic * toonParams2.x;
			specular  = toonParams2.y;
			emissive = toonParams2.z;
		}
	#if (FLAG1_MATERIAL_LIGHT)
		vec3 normal = normalize(fFacingNormal);
		if(hasMatFlag(MatFlag_HasNormalMap)) {
			vec4 normalColor;
			int texcoord = bitfieldExtract(TexcoordFlags, 3, 3);
			vec3 tangent = vec3(0.0);
			switch(texcoord) {
				case 0:
					normalColor = texture(uNormal, fTex0);
					tangent = calcTangent(normal, fPosition, fTex0);
					break;
				case 1:
					normalColor = texture(uNormal, fTex1);
					tangent = calcTangent(normal, fPosition, fTex1);
					break;
				case 2:
					normalColor = texture(uNormal, fTex2);
					tangent = calcTangent(normal, fPosition, fTex2);
					break;
			}
			normalColor.y = 1.0 - normalColor.y; // OpenGL moment
			vec3 bitangent = cross(normal, tangent);
			mat3 TBN = mat3(tangent, bitangent, normal);
			normalColor.xyz = normalize(normalColor.xyz * 2.0 - 1.0);
			normalColor.xyz = normalize(TBN * normalColor.xyz);
		}

		float shadowMask = 0.0;
		float edgeMask   = 0.0;
		float shadowFix  = 0.0;
		if(hasMatFlag(MatFlag_HasDetailMap)) {
			vec4 toonParams;
			int texcoord = bitfieldExtract(TexcoordFlags, 21, 3);
			switch(texcoord) {
				case 0:
					toonParams = texture(uDetail, fTex0);
					break;
				case 1:
					toonParams = texture(uDetail, fTex1);
					break;
				case 2:
					toonParams = texture(uDetail, fTex2);
					break;
			}
			shadowMask = 1.0 - toonParams.x;
			edgeMask   = toonParams.y;
			shadowFix  = toonParams.z;
		}
		//baseColor.rgb = baseColor.rgb * ( 1.0 - metallic );
		vec3 viewNeg = normalize(-fPosition);
		float NL = clamp(dot( lightNeg, normal ), 0.0, 1.0);
		float NoL = dot( lightNeg, normal );
		float shadowNoL = clamp( max( NL - pow( shadowThreshold, 1.8 ), 0.0 ) * shadowFactor, 0.0, 1.0 );
		if(hasMatFlag(MatFlag_HasDetailMap))
			shadowNoL *= shadowFix;
		//oColor.rgb = vec3(shadowMask, edgeMask, shadowFix);
		//return;
		vec4 toonShadowColor = matShadowColor;
		if(hasMatFlag(MatFlag_HasSpecularMap)) {
			int texcoord = bitfieldExtract(TexcoordFlags, 6, 3);
			switch(texcoord) {
				case 0:
					toonShadowColor = sRGBToLinear(texture(uSpecular, fTex0));
					break;
				case 1:
					toonShadowColor = sRGBToLinear(texture(uSpecular, fTex1));
					break;
				case 2:
					toonShadowColor = sRGBToLinear(texture(uSpecular, fTex2));
					break;
			}
		}
		baseColor.rgb *= lightColor.rgb;
		toonShadowColor.rgb = clamp( toonShadowColor.rgb * lightAmbient.rgb, 0.0, 1.0);
		toonShadowColor.rgb = mix( baseColor.rgb, toonShadowColor.rgb, lightShadowAlpha);
		vec3 diffuseColor = toonShadowColor.rgb * ( 1.f - shadowNoL ) + baseColor.rgb * shadowNoL;
		vec4 edgeColor;
		#if (FLAG2_EDGE_BACKLIGHT)
			float NVW = clamp( dot( normal, mix( viewNeg, normal, -min( NoL, 0.f ) ) ), 0.0, 1.0 );
			float threshold = edgeThreshold;
			//#if (FLAG0_EDGE_REMOVAL_LIGHT_YAXIS)
			//	threshold = saturate( threshold + pow( abs( fragment.viewSpaceNormal.y ), edgeRemoveYAxisFactor ) );
			//#endif
			float E = pow( ( min( 1.f - NVW, threshold ) / threshold ), edgeFactor );

			vec3 toonEdgeColor = matEdgeColor.rgb;
			if(hasMatFlag(MatFlag_HasShadowMap)) {
				int texcoord = bitfieldExtract(TexcoordFlags, 24, 3);
				switch(texcoord) {
					case 0:
						toonEdgeColor = sRGBToLinear(texture(uShadow, fTex0).rgb);
						break;
					case 1:
						toonEdgeColor = sRGBToLinear(texture(uShadow, fTex1).rgb);
						break;
					case 2:
						toonEdgeColor = sRGBToLinear(texture(uShadow, fTex2).rgb);
						break;
				}
			}
			edgeColor.rgb = toonEdgeColor * lightColor.rgb;
			edgeColor.a   = matEdgeColor.a * E;
			if(hasMatFlag(MatFlag_HasDetailMap))
				edgeColor.a *= edgeMask;
		#else
			edgeColor = float4( 0.f, 0.f, 0.f, 0.f );
		#endif

		vec3 specularColor = vec3(0.0);;
		if(hasMatFlag(4)) {
			specularColor = matSpecularColor.rgb * clamp( pow( max( dot( normalize( lightNeg + viewNeg ), normal ), 0.0), matSpecularPower ) / matSpecularThreshold, 1.0, 0.0 ) * lightSpecularIntensity;
			if(hasMatFlag(MatFlag_HasGlowMap))
				specularColor *= specular;
			#if (FLAG2_EDGE_REFERENCE_NORMALALPHA)
				if(hasMatFlag(MatFlag_HasDetailMap))
					specularColor *= edgeMask;
			#endif
		}
		vec3 emissiveColor = matEmissiveColor.rgb * emissive;
		#if (FLAG2_EDGE_SEMITRANS)
			oColor.rgb = diffuseColor + specularColor;
			oColor.rgb = mix( result.color.rgb, edgeColor.rgb, edgeColor.a );
		#else
			oColor.rgb = diffuseColor + specularColor + ( edgeColor.rgb * edgeColor.a );
		#endif

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
					oColor.rgb = mix(oColor.rgb, highlightColor.rgb, highlightColor.a * matEmissiveColor.a);
					break;
				case 2:
					oColor.rgb += highlightColor.rgb * highlightColor.a * matEmissiveColor.a;
					break;
				case 3:
					oColor.rgb -= highlightColor.rgb * highlightColor.a * matEmissiveColor.a;
					break;
				case 4:
					oColor.rgb *= mix( vec3(1.0), highlightColor.rgb, highlightColor.a * matEmissiveColor.a );
					break;
			}
		}
		oColor.rgb += matEmissiveColor.rgb;
	#else
		oColor.rgb = baseColor.rgb;
	#endif

}