Shader "Custom/HullCullOff"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("NormalMap", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

			
			

			//-------------------------------------------------------------------
			Cull Front

			CGPROGRAM

			sampler2D _MainTex;
			sampler2D _BumpMap;

			struct Input
			{
			    float2 uv_MainTex;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			bool _Front;
			half3 _Emission;


			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows
	
			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf (Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
	
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
				o.Emission = _Emission;

				//float value = dot(mul(UNITY_MATRIX_IT_MV, float4(o.Normal, 0.0)).xyz, float3(0.0, -1.0, 0.0));
				//if(value < 0.0){o.Normal = -o.Normal;}
				o.Normal = -o.Normal;
			}
			ENDCG





			//-------------------------------------------------------------------
			Cull Back

			CGPROGRAM

			sampler2D _MainTex;
			sampler2D _BumpMap;

			struct Input
			{
			    float2 uv_MainTex;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			bool _Front;
			half3 _Emission;


			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows
	
			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0


			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf (Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
	
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
				o.Emission = _Emission;

				//float value = dot(mul(UNITY_MATRIX_IT_MV, float4(o.Normal, 0.0)).xyz, float3(0.0, -1.0, 0.0));
				//if(value < 0.0){o.Normal = -o.Normal;}
				//o.Normal = o.Normal;
			}
			ENDCG
		
    }
    FallBack "Diffuse"
}
