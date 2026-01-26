Shader "Custom/Reflection"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ReflectColor("ReflectColor", Color) = (1, 1, 1, 1)
		_Smoothness("Smoothness", Range(0, 1.0)) = 0.5
		_FresnelScale("FresnelFactor", Range(0, 1)) = 0.5
	}
	SubShader
	{
		Pass
		{
			Tags { "LightMode"="ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 worldRefl : TEXCOORD2;
				float3 worldViewDir : TEXCOORD3;
				float2 uv : TEXCOORD4;
				SHADOW_COORDS(5)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _ReflectColor;
			float _Smoothness;
			float _FresnelScale;
			
			v2f vert (a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldViewDir = UnityWorldSpaceViewDir(o.worldPos);
				o.worldRefl = reflect(-o.worldViewDir, o.worldNormal);
				o.uv = v.uv;
				TRANSFER_SHADOW(o);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 worldViewDir = normalize(i.worldViewDir);

				fixed roughness = 1 - _Smoothness;

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				fixed3 diffuse = _LightColor0.rgb * tex2D(_MainTex, i.uv).rgb * max(0, dot(worldNormal, worldLightDir));
				
				fixed roughnessMipScale = roughness * (1.7 - 0.7 * roughness);

				fixed3 reflection = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, i.worldRefl, (roughnessMipScale) * UNITY_SPECCUBE_LOD_STEPS).rgb * _ReflectColor.rgb;
				
				fixed fresnel = _FresnelScale + (1 - _FresnelScale) * pow(1 - dot(worldViewDir, worldNormal), 5);

				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				return fixed4(ambient + lerp(diffuse, reflection, saturate(fresnel))* atten, 1.0);
			}
			ENDCG
		}
	}
}
