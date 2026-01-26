Shader "Custom/GrabPass"
{
	Properties
	{
		_NormalTex("NormalMap", 2D) = "bump"{}
		_Distortion("Distortion", Range(0, 1.0)) = 1.0
		_Mask("Mask", 2D) = "white"{}
		_Speed("Speed", Range(0, 1.0)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "DisableBatching"="True"}

		ZWrite Off

		GrabPass{"_MainTex"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float2 screenPos : TEXCOORD1;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
				float4 T2W0 : TEXCOORD2; //w=worldPos.x       //为充分利用插值寄存器，把顶点世界坐标存储在矩阵行的w分量中。
				float4 T2W1 : TEXCOORD3; //w=worldPos.y
				float4 T2W2 : TEXCOORD4; //w=worldPos.z
				SHADOW_COORDS(5)
			};

			sampler2D _MainTex;
			sampler2D _NormalTex;
			float _Distortion;
			float _Speed;

			sampler2D _Mask;
			
			v2f vert (a2v v)
			{
				v2f o;
				//MVP变换--位置裁剪坐标
				o.pos = UnityObjectToClipPos(v.vertex);  //等同于：mul(MVP, v)
				//屏幕坐标
				o.screenPos = ComputeGrabScreenPos(o.pos);
				//纹理坐标
				//o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);  //=v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o.uv = v.texcoord;//无偏移
				


				//法线变换到世界坐标
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);//等同于： mul(v.normal, (float3x3)unity_WorldToObject);
				//切线变换到世界坐标
				fixed3 worldTangent= UnityObjectToWorldDir(v.tangent.xyz);
				//BiNormal世界坐标
				fixed3 worldBitangent = normalize(cross(worldNormal, worldTangent)) * v.tangent.w;
				//位置世界坐标
				fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				//Matrix
				o.T2W0 = float4(worldTangent.x, worldBitangent.x, worldNormal.x, worldPos.x);
				o.T2W1 = float4(worldTangent.y, worldBitangent.y, worldNormal.y, worldPos.y);
				o.T2W2 = float4(worldTangent.z, worldBitangent.z, worldNormal.z, worldPos.z);
				
				//ShadowMap坐标
				TRANSFER_SHADOW(o);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//Grab的像素着色
				float2 bumpCoords = i.uv + float2(_Time.y * _Speed, _Time.y * _Speed);
				float2 offset = UnpackNormal(tex2D(_NormalTex, bumpCoords)).xy * _Distortion * tex2D(_Mask, i.uv).r;
				float2 screenCoords = i.screenPos.xy / i.screenPos.w;
				screenCoords += offset;
				fixed4 color = tex2D(_MainTex, screenCoords);

				return color;
			}
			ENDCG
		}
	}
}
