Shader "Instanced/InstancedSurface" {
    Properties {
        _Color ("Color", color) = (1,1,1,0)
        _WaveTex ("Disp Texture", 2D) = "gray" {}
        _Displacement ("Displacement", Range(0, 10.0)) = 0.3
		_BumpValue ("BumpValue", Range(0, 10.0)) = 0.3
		_Roughness ("Roughness", Range(0, 1.0)) = 0.0
		_OffsetX("OffsetX", Range(0, 1.0)) = 0.0
		_OffsetY("OffsetY", Range(0, 1.0)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "LightMode"="ForwardBase"}
        LOD 300

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		
        CGPROGRAM
		#include "UnityCG.cginc"
        #include "UnityPBSLighting.cginc"

        #pragma surface surf Standard addshadow fullforwardshadows vertex:disp nolightmap alpha:blend
		//#pragma surface surf Lambert vertex:vert
        //#pragma target 4.6
        #pragma multi_compile_instancing

		//#pragma vertex disp
		//#pragma fragment frag
		//#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
		//#pragma target 4.5
		//#pragma multi_compile_instancing
		
        sampler2D _WaveTex;
		float2 _WaveTex_ST;
		float4 _WaveTex_TexelSize;
        float _Displacement;
		float _BumpValue;
        fixed4 _Color;
        float _Metallic;
		float _Roughness;
		float _OffsetX;
		float _OffsetY;


        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
			UNITY_VERTEX_INPUT_INSTANCE_ID
        };
		
        struct Input 
		{
				float4 pos : SV_POSITION;
				float3 normal:TEXCOORD0;
                float3 worldPos:TEXCOORD1;
				float2 uv_WaveTex : TEXCOORD2;
        };



        void disp (inout appdata v, out Input o) //, uint instanceID : SV_InstanceID    //, out Input o
		{
				UNITY_SETUP_INSTANCE_ID(v);

				//v2f o;
				//uv
				o.uv_WaveTex = v.texcoord.xy;
				//d
				float d = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY)), 0, 0)).r * _Displacement;
				//POS
				v.vertex.xyz += float3(0.0, 1.0, 0.0) * d;
				o.worldPos = mul(UNITY_MATRIX_M, float4(v.vertex.xyz, 1.0));
				o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1.0));

				
				//normal and disp compute
				float R = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY) + float2(_WaveTex_TexelSize.x, 0.0)), 0, 0)).r;
				float L = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY) - float2(_WaveTex_TexelSize.x, 0.0)), 0, 0)).r;
				float U = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY) + float2(0.0, _WaveTex_TexelSize.y)), 0, 0)).r;
				float D = tex2Dlod(_WaveTex, float4((v.texcoord.xy + float2(_OffsetX, _OffsetY) - float2(0.0, _WaveTex_TexelSize.y)), 0, 0)).r;

				float3 normTan1 = float3(1.0, 0.0, 0.0) * (R - L) * _BumpValue;
				float3 normTan2 = float3(0.0, 0.0, 1.0) * (U - D) * _BumpValue;

				//normal
				o.normal = normalize(float3(0.0, 1.0, 0.0) + normTan1 + normTan2);//test

				//return o;
        }

		

        void surf (Input IN, inout SurfaceOutputStandard o) {
            half4 c = _Color;
            o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = IN.normal;
            o.Metallic = 0.2f;
            o.Smoothness = 1 - _Roughness;

        }
        ENDCG
		
    }
    FallBack "Diffuse"
}