Shader "Custom/Text Shader" {
    Properties {
        //这里的贴图为字体里面的FontTexture
        _MainTex ("Font Texture", 2D) = "white" {}
        _Color ("Text Color", Color) = (1,1,1,1)
    }

    SubShader {

        Tags {
            //渲染队列-通常这个索引用来渲染透明度混合的物体
            "Queue"="Transparent"
            //Projector为投影器，这样设置将会使该物体忽略任何投影类型的材质或贴图的影响
            "IgnoreProjector"="True"
            //渲染透明物体时使用
            "RenderType"="Transparent"
            //预览-平面
            "PreviewType"="Plane"
        }
        //关闭光照 剔除关闭(正背面全部显示) 深度测试开启 深度写入关闭
        //深度测试为当这个物体比深度缓冲中的像素靠近摄像机时显示，否则不显示
        Lighting Off Cull Off ZTest Always ZWrite Off
        //以这个物体的a值为标准，设置颜色缓冲区中的颜色为1-这个物体的a值
        Blend SrcAlpha OneMinusSrcAlpha
        //这里总体设置为不受光照影响，全部渲染。透明处理为打开深度测试，关闭深度写入自己定义颜色混合(以该物体a值为标准)

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //大概就是vr ar，xr之类的东西了
            //https://docs.unity3d.com/Manual/SinglePassStereoRendering.html
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                //UnityCG.cginc-UnityInstancing.cginc
                //- UNITY_VERTEX_INPUT_INSTANCE_ID Declare instance ID field in vertex shader input / output struct.在a2v或者v2f里面声明id领域
//                #ifdef SHADER_API_PSSL
//                  #define DEFAULT_UNITY_VERTEX_INPUT_INSTANCE_ID uint instanceID;
//              #else
//                  #define DEFAULT_UNITY_VERTEX_INPUT_INSTANCE_ID uint instanceID : SV_InstanceID;
//              #endif
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
                //UnityCG.cginc-UnityInstancing.cginc
                //- UNITY_VERTEX_OUTPUT_STEREO Declare stereo target eye field in vertex shader output struct.
//              #ifdef UNITY_STEREO_INSTANCING_ENABLED
//                  #define DEFAULT_UNITY_VERTEX_OUTPUT_STEREO                          uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
//              #elif defined(UNITY_STEREO_MULTIVIEW_ENABLED)
//                  #define DEFAULT_UNITY_VERTEX_OUTPUT_STEREO float stereoTargetEyeIndex : BLENDWEIGHT0;
//              #endif
            };

            sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform fixed4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                // - UNITY_SETUP_INSTANCE_ID        Should be used at the very beginning of the vertex shader / fragment shader,
//                #define DEFAULT_UNITY_SETUP_INSTANCE_ID(input)      { UnitySetupInstanceID(UNITY_GET_INSTANCE_ID(input)); 
//                  void UnitySetupInstanceID(uint inputInstanceID)
//                  {
//                      #ifdef UNITY_STEREO_INSTANCING_ENABLED
//                          // stereo eye index is automatically figured out from the instance ID
//                          unity_StereoEyeIndex = inputInstanceID & 0x01;
//                          unity_InstanceID = unity_BaseInstanceID + (inputInstanceID >> 1);
//                      #else
//                          unity_InstanceID = inputInstanceID + unity_BaseInstanceID;
//                      #endif
//                  }
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                //o.stereoTargetEyeIndex = unity_StereoEyeIndex

                //顶点坐标从模型空间(M)转换到观察空间(v,摄像机空间) 等同mul(Unity_MARTIX_MVP, v.vertex)
                o.vertex = UnityObjectToClipPos(v.vertex);
                //因为3dText的网格顶点是由TextMesh生成，TextMesh可以调整顶点颜色，这样可以使用TextMesh调整每个3d字体的特殊颜色，_Color确定总体颜色
                o.color = v.color * _Color;
                //UnityCG.cginc #define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)
                //主要计算_MainTex的tilling和offset但是感觉这个shader里面没有卵用。真的有人调整这个吗
                //感觉可以直接 o.texcoord = v.texcoord
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = i.color;
                col.a *= tex2D(_MainTex, i.texcoord).a;
//                col.a = (col.a + tex2D(_MainTex, i.texcoord).a)/2;
                return col;
            }
            ENDCG
        }
    }
}