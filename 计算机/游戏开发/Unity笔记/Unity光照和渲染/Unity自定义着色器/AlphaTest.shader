Shader "Custom/AlphaTest"
{
	Properties{
		_MainTex("MainTexture", 2D) = "white"{}
		_BumpMap("NormalMap", 2D) = "bump"{}
		_BumpScale("BumpScale", float) = 1.0
		_BumpMapMask("NormalMapMask", 2D) = "white"{}
		_SpecularMask("SpecularMask", 2D) = "white"{}
		_SpecularMinScale("SpecularMinScale", float) = 0.5
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Specular("Specular", Color) = (1.0, 1.0, 1.0, 1.0)
		_Gloss("Gloss", Range(8.0, 256)) = 20
		_Cutoff("CutOff", Range(0, 1.0)) = 0
	}
	
	SubShader {
		Pass{
			Tags{ "LightMode"="ForwardBase" "Queue"="AlphaTest" }
			
			Cull Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _MainTex;
			//float4 _MainTex_ST;

			sampler2D _BumpMap;
			//float4 _BumpMap_ST;
			fixed _BumpScale;

			sampler2D _BumpMapMask;

			sampler2D _SpecularMask;
			fixed _SpecularMinScale;
			

			fixed4 _Color;
			fixed4 _Specular;
			float _Gloss;
			float _Cutoff;

			struct a2v{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 T2W0 : TEXCOORD1; //w=worldPos.x       //为充分利用插值寄存器，把顶点世界坐标存储在矩阵行的w分量中。
				float4 T2W1 : TEXCOORD2; //w=worldPos.y
				float4 T2W2 : TEXCOORD3; //w=worldPos.z
				SHADOW_COORDS(4)
			};

			v2f vert (a2v v){
				v2f o;
				//MVP变换--位置裁剪坐标
				o.pos = UnityObjectToClipPos(v.vertex);  //等同于：mul(MVP, v)
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
				
				TRANSFER_SHADOW(o);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{

				

				//世界坐标
				float3 worldPos = float3(i.T2W0.w, i.T2W1.w, i.T2W2.w);
				//光源方向
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));//普通平行光也可用normalize(_WorldSpaceLightPos0.xyz);
				

				//世界法线
				fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv));
				_BumpScale *= (1-tex2D(_BumpMapMask, i.uv).r);
				bump *= _BumpScale;
				
				bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));
				fixed3 worldNormal = normalize(half3(dot(i.T2W0.xyz, bump), dot(i.T2W1.xyz, bump), dot(i.T2W2.xyz, bump)));


				//采样
				fixed4 texColor = tex2D(_MainTex, i.uv);
				//AlphaTest
				clip(texColor.a - _Cutoff);

				//环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				//漫反射
				fixed3 albedo = texColor.rgb * _Color.rgb;
				fixed3 diffuse = _LightColor0.rgb * albedo * clamp(dot(worldNormal, worldLightDir), 0, 1.0);//也可用半兰伯特：(0.5 + 0.5 * dot(worldNormal, worldLightDir));//半lambert
				//高光反射
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));//等同于：normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				fixed3 halfDir = normalize(viewDir + worldLightDir);
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);

				fixed specularStrenth = _SpecularMinScale + (1 - _SpecularMinScale) * tex2D(_SpecularMask, i.uv).r;
				specular *= specularStrenth;

				//阴影和衰减
				UNITY_LIGHT_ATTENUATION(atten, i, worldPos);

				return fixed4(ambient + (diffuse + specular) * atten, 1.0);
			}

			ENDCG
		}
	}
	//Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
	Fallback "Transparent/Cutout/VertexLit"
}
