
Shader "Custom/FXWaterCustom" {
Properties {
	_Color("Color", color) = (1.0, 1.0, 1.0, 1.0)
	_Transparency("Transparency", Range(0.0, 1.0)) = 0.5
	_WaveScale ("Wave scale", Range (0.1,0.01)) = 0.05
	_ReflDistort ("Reflection distort", Range (0,1.5)) = 0.44
	_RefrDistort ("Refraction distort", Range (0,1.5)) = 0.40
	_RefrColor ("Refraction color", COLOR)  = ( .34, .85, .92, 1)
	[NoScaleOffset] _Fresnel ("Fresnel (A) ", 2D) = "gray" {}
	[NoScaleOffset] _BumpMap ("Normalmap ", 2D) = "bump" {}
	WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
	[NoScaleOffset] _ReflectiveColor ("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
	_HorizonColor ("Horizon color(Useless Now)", COLOR)  = ( .172, .463, .435, 1)
	[HideInInspector] _ReflectionTex ("Internal Reflection", 2D) = "" {}
	[HideInInspector] _RefractionTex ("Internal Refraction", 2D) = "" {}

}


// -----------------------------------------------------------
// Fragment program cards


Subshader {
	Tags { "RenderType"="Transparent" "Queue"="Transparent" "WaterMode"="Refractive" }//。。。 
	Blend SrcAlpha OneMinusSrcAlpha
	ZWrite On
	ZTest On


	Pass {
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma multi_compile_fog
	#pragma multi_compile WATER_REFRACTIVE WATER_REFLECTIVE WATER_SIMPLE

	#if defined (WATER_REFLECTIVE) || defined (WATER_REFRACTIVE)
	#define HAS_REFLECTION 1
	#endif
	#if defined (WATER_REFRACTIVE)
	#define HAS_REFRACTION 1
	#endif


	#include "UnityCG.cginc"

	uniform fixed4 _Color;
	uniform float _Transparency;


	uniform float4 _WaveScale4;
	uniform float4 _WaveOffset;

	#if HAS_REFLECTION
	uniform float _ReflDistort;
	#endif
	#if HAS_REFRACTION
	uniform float _RefrDistort;
	#endif



struct appdata {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
};

struct v2f {
		float4 pos : SV_POSITION;
		float4 worldPos : TEXCOORD0;
		float3 worldNormal : NORMAL;
		#if defined(HAS_REFLECTION) || defined(HAS_REFRACTION)
		float4 ref : TEXCOORD1;
		float2 bumpuv0 : TEXCOORD2;
		float2 bumpuv1 : TEXCOORD3;
		#else
		float2 bumpuv0 : TEXCOORD1;
		float2 bumpuv1 : TEXCOORD2;
		#endif
		UNITY_FOG_COORDS(5)
};

v2f vert(appdata v)
{
	v2f o;
	
	o.worldPos = mul(unity_ObjectToWorld, v.vertex);
	o.worldNormal = float3(0.0, 1.0, 0.0);


	o.pos = UnityWorldToClipPos(o.worldPos);


	// scroll bump waves
	float4 temp;
	temp.xyzw = o.worldPos.xzxz * _WaveScale4 + _WaveOffset;
	o.bumpuv0 = temp.xy;
	o.bumpuv1 = temp.wz;


	
	
	#if defined(HAS_REFLECTION) || defined(HAS_REFRACTION)
	o.ref = ComputeNonStereoScreenPos(o.pos);
	#endif

	UNITY_TRANSFER_FOG(o,o.pos);
	return o;
}

#if defined (WATER_REFLECTIVE) || defined (WATER_REFRACTIVE)
sampler2D _ReflectionTex;
#endif
#if defined (WATER_REFLECTIVE) || defined (WATER_SIMPLE)
sampler2D _ReflectiveColor;
#endif
#if defined (WATER_REFRACTIVE)
sampler2D _Fresnel;
sampler2D _RefractionTex;
uniform float4 _RefrColor;
#endif
#if defined (WATER_SIMPLE)
uniform float4 _HorizonColor;
#endif
sampler2D _BumpMap;


half4 frag( v2f i ) : SV_Target
{
	// combine two scrolling bumpmaps into one
	half3 bump1 = UnpackNormal(tex2D( _BumpMap, i.bumpuv0 )).rgb;
	half3 bump2 = UnpackNormal(tex2D( _BumpMap, i.bumpuv1 )).rgb;
	half3 bump = (bump1 + bump2) * 0.5;

	bump = normalize(half3( bump.x, bump.z, bump.y));// * 0.5 + i.worldNormal * 0.5;// * 0.5;//i.normal * 0.5 + 

	
	// object space view direction (will normalize per pixel)
	half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

	float fragDistance = length(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

	half3 normal = lerp(bump, half3(0.0, 1.0, 0.0), saturate(fragDistance / 10000.0));
	half3 worldReflDir = reflect(-worldViewDir, normal);
	
	// fresnel factor
	half fresnelFac = clamp(dot(worldViewDir, normal ), 0, 1.0);
	
	// perturb reflection/refraction UVs by bumpmap, and lookup colors
	#if HAS_REFLECTION
	float4 uv1 = i.ref; uv1.xy += bump * _ReflDistort;
	half4 refl = tex2Dproj( _ReflectionTex, UNITY_PROJ_COORD(uv1) );
	UNITY_APPLY_FOG(i.fogCoord, refl);//反射颜色计算雾效
	#endif
	#if HAS_REFRACTION
	float4 uv2 = i.ref; uv2.xy -= bump * _RefrDistort;
	half4 refr = tex2Dproj( _RefractionTex, UNITY_PROJ_COORD(uv2) ) * _RefrColor;
	#endif
	
	// final color is between refracted and reflected based on fresnel
	half4 color;
	
	#if defined(WATER_REFRACTIVE)
	half fresnel = UNITY_SAMPLE_1CHANNEL( _Fresnel, float2(fresnelFac,fresnelFac) );
	color = lerp( refr, refl, fresnel );
	#endif
	
	#if defined(WATER_REFLECTIVE)
	half4 water = tex2D( _ReflectiveColor, float2(fresnelFac,fresnelFac) );
	color.a = (1.0 - (_Transparency * fresnelFac * fresnelFac));//refl.a * water.a * (1.0 - (_Transparency * fresnelFac));
	color.rgb = lerp( water.rgb, refl.rgb, pow((1 - fresnelFac), 5) );
	color = color * _Color;
	#endif
	
	#if defined(WATER_SIMPLE)
	half4 colorRefl = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldReflDir) * 2.0;

	//Legacy Simple
	//half4 water = lerp( tex2D(_ReflectiveColor, float2(fresnelFac,fresnelFac)), _Color, saturate(0.1 - fresnelFac) * 10.0);
	//color.rgb = lerp(water.rgb, colorRefl.rgb, water.a );
	//color.a = 1.0 - (_Transparency * fresnelFac * fresnelFac);

	half4 water = tex2D( _ReflectiveColor, float2(fresnelFac,fresnelFac));
	color.a = (1.0 - (_Transparency * fresnelFac * fresnelFac));//refl.a * water.a * (1.0 - (_Transparency * fresnelFac));
	color.rgb = lerp( water.rgb, colorRefl.rgb, pow((1 - fresnelFac), 5));
	color = color * _Color;


	#endif

	//Transparency(when alpha blend)
	

	UNITY_APPLY_FOG(i.fogCoord, color);

	if(fragDistance > 5000.0)
	{
		color.a *= saturate((20000.0 - fragDistance) / 15000.0);
	}

	return color;
}
ENDCG

	}
}

}
