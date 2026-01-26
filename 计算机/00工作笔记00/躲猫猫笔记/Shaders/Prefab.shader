Shader "Custom/Prefab"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_U("U Top", int) = 0
		_V("V Top", int) = 0
    }
    SubShader
    {
		 Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows //SurfaceShader 加上alpha实现透明

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input
			{
				float3 worldNormal;
				float2 uv_MainTex;
			};

			half _Glossiness;
			half _Metallic;
			int _U;
			int _V;
			fixed4 _Color;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				float4 modelNormal = mul(unity_WorldToObject, float4(IN.worldNormal, 0.0));
				fixed4 c = fixed4(0.0, 0.0, 0.0, 0.0);
				// Albedo comes from a texture tinted by color
				c = tex2D(_MainTex, IN.uv_MainTex + float2(_U * 0.03125, _V * 0.03125)) * _Color;
				o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
    }
    FallBack "Diffuse"
}
