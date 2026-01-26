// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Instanced/InstancedNoBuffer" 
{
	Properties
    {
        _Color ("Color", color) = (1,1,1,0)
        _WaveTex ("Disp Texture", 2D) = "gray" {}
        _Displacement ("Displacement", Range(0, 10.0)) = 0.3
		_BumpValue ("BumpValue", Range(0, 10.0)) = 0.3
		_Roughness ("Roughness", Range(0, 1.0)) = 0.0
		_OffsetX("OffsetX", Range(0, 1.0)) = 0.0
		_OffsetY("OffsetY", Range(0, 1.0)) = 0.0
    }

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue"="Transparent" }
		LOD 100

		Pass 
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			//Tags {"LightMode" = "ForwardBase"}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase nolightmap //alpha:blend
			#pragma target 4.5
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"


			sampler2D _WaveTex;
			float2 _WaveTex_ST;
			float4 _WaveTex_TexelSize;
			float _Displacement;
			float _BumpValue;
			fixed4 _Color;
			float _Roughness;
			float _OffsetX;
			float _OffsetY;

			struct appdata 
			{
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal:TEXCOORD0;
                float3 worldPos:TEXCOORD1;
				float2 uv_WaveTex : TEXCOORD2;
			};

			v2f vert(appdata v)//, uint instanceID : SV_InstanceID
			{
				UNITY_SETUP_INSTANCE_ID(v);

				v2f o;
				//uv
				o.uv_WaveTex = v.texcoord.xy;
				//d
				float d = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY)), 0, 0)).r * _Displacement;
				//POS
				v.vertex.xyz += float3(0.0, 1.0, 0.0) * d;
				o.worldPos = mul(UNITY_MATRIX_M, float4(v.vertex.xyz, 1.0));
				o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1.0f));

				
				//normal and disp compute
				float R = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY) + float2(_WaveTex_TexelSize.x, 0.0)), 0, 0)).r;
				float L = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY) - float2(_WaveTex_TexelSize.x, 0.0)), 0, 0)).r;
				float U = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY) + float2(0.0, _WaveTex_TexelSize.y)), 0, 0)).r;
				float D = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY) - float2(0.0, _WaveTex_TexelSize.y)), 0, 0)).r;

				float3 normTan1 = float3(1.0, 0.0, 0.0) * (R - L) * _BumpValue;
				float3 normTan2 = float3(0.0, 0.0, 1.0) * (U - D) * _BumpValue;

				//normal
				o.worldNormal = normalize(float3(0.0, 1.0, 0.0) + normTan1 + normTan2);//test

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                float3 worldNormal = normalize(i.worldNormal);
                float3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 halfDir = normalize(worldLightDir+worldViewDir);


                float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * _Color.rgb;
                float3 diffuse = max(0, dot(worldNormal, worldLightDir)) * _Color.rgb;
                float3 speclur = pow(max(0, dot(halfDir, worldNormal)), 100.0) * _Color.rgb;

				//...
                return fixed4(ambient + diffuse + speclur, _Color.a);
			}
			ENDCG
		}
	}
}