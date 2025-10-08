Shader "Anm2/Invisible"
{
    Properties {} // 空属性
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            // 检查URP包是否存在
            #if defined(UNITY_RENDER_PIPELINE_URP)
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #else
                // 回退到内置着色器
                #include "UnityCG.cginc"
            #endif
            
            struct Attributes
            {
                float4 positionOS : POSITION;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                #if defined(UNITY_RENDER_PIPELINE_URP)
                    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                #else
                    output.positionCS = UnityObjectToClipPos(input.positionOS);
                #endif
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                return half4(0, 0, 0, 0);
            }
            
            ENDHLSL
        }
    }
    
    Fallback "Transparent/VertexLit"
}
