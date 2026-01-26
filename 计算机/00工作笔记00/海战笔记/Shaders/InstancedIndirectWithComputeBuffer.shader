Shader "Instanced/InstanceIndirectPositionBuffer" 
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

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
 
		Pass
		{
			CGPROGRAM
			//#pragma nolightmap alpha:blend

			#pragma vertex vert
			#pragma fragment frag		
			#pragma target 4.5
			#pragma multi_compile_instancing
 
			#include "UnityCG.cginc"
			
 
			StructuredBuffer<float4> positionBuffer;
			//StructuredBuffer<float> waveBuffer[100];
 
			struct appdata
			{
			    float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};
 
 
			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal:TEXCOORD0;
                float3 worldPos:TEXCOORD1;
				float2 uv_WaveTex : TEXCOORD2;
			};
 
			sampler2D _WaveTex;
			float2 _WaveTex_ST;
			float4 _WaveTex_TexelSize;
			float _Displacement;
			float _BumpValue;
			fixed4 _Color;
			float _Roughness;
			float _OffsetX;
			float _OffsetY;
 
			v2f vert(appdata v, uint instanceID : SV_InstanceID)
			{	
				v2f o;
				//uv
				o.uv_WaveTex = v.texcoord.xy;
				//d
				float d = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY)), 1, 1)).r * _Displacement;
				//POS
				v.vertex.xyz += float3(0.0, 1.0, 0.0) * d;
				o.worldPos = positionBuffer[instanceID].xyz + v.vertex;
				o.pos = mul(UNITY_MATRIX_VP, float4(o.worldPos, 1.0f));

				
				//normal and disp compute
				float R = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY) + float2(_WaveTex_TexelSize.x, 0.0)), 0, 0)).r;
				float L = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY) - float2(_WaveTex_TexelSize.x, 0.0)), 0, 0)).r;
				float U = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY) + float2(0.0, _WaveTex_TexelSize.y)), 0, 0)).r;
				float D = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY) - float2(0.0, _WaveTex_TexelSize.y)), 0, 0)).r;

				float3 normTan1 = float3(1.0, 0.0, 0.0) * (R - L) * _BumpValue;
				float3 normTan2 = float3(0.0, 0.0, 1.0) * (U - D) * _BumpValue;

				//normal
				o.worldNormal = normalize(float3(0.0, 1.0, 0.0) + normTan1 + normTan2);

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