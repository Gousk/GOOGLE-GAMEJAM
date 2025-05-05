Shader "Custom/URPOutlineGlow"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _OutlineWidth ("Outline Width", Float) = 0.1
        _EmissionStrength ("Emission Strength", Float) = 1.5
        _BlendSpeed ("Blend Speed", Float) = 1.0
        _Color1 ("Color 1", Color) = (1,0,0,1)
        _Color2 ("Color 2", Color) = (0,1,0,1)
        _Color3 ("Color 3", Color) = (0,0,1,1)
        _Color4 ("Color 4", Color) = (1,1,0,1)
        _Color5 ("Color 5", Color) = (1,0,1,1)
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 200

        // Outline Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "UniversalForward" }
            Cull Front
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex VertExpand
            #pragma fragment FragOutline
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float    _OutlineWidth;
            float    _EmissionStrength;
            float    _BlendSpeed;
            float4   _Color1;
            float4   _Color2;
            float4   _Color3;
            float4   _Color4;
            float4   _Color5;

            Varyings VertExpand(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos    = TransformObjectToWorld(IN.positionOS);
                float3 worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                worldPos += worldNormal * _OutlineWidth;
                OUT.positionCS = TransformWorldToHClip(worldPos);
                return OUT;
            }

            half4 FragOutline(Varyings IN) : SV_Target
            {
                // cycle through 5 colors
                float phase = frac(_Time.y * _BlendSpeed) * 5.0;
                int idx = (int)floor(phase);
                float t = frac(phase);
                float4 c1 = _Color1;
                float4 c2 = _Color1;
                if (idx == 0) { c1 = _Color1; c2 = _Color2; }
                else if (idx == 1) { c1 = _Color2; c2 = _Color3; }
                else if (idx == 2) { c1 = _Color3; c2 = _Color4; }
                else if (idx == 3) { c1 = _Color4; c2 = _Color5; }
                else if (idx == 4) { c1 = _Color5; c2 = _Color1; }
                float3 col = lerp(c1.rgb, c2.rgb, t);
                col *= _EmissionStrength;
                return half4(col, 1.0);
            }
            ENDHLSL
        }

        // Base Pass (Unlit)
        Pass
        {
            Name "Base"
            Tags { "LightMode" = "UniversalForward" }
            Cull Back
            ZWrite On
            Blend Off

            HLSLPROGRAM
            #pragma vertex VertBase
            #pragma fragment FragBase
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings VertBase(Attributes IN)
            {
                Varyings OUT;
                float3 posWS = TransformObjectToWorld(IN.positionOS);
                OUT.positionCS = TransformWorldToHClip(posWS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 FragBase(Varyings IN) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return col;
            }
            ENDHLSL
        }
    }
    FallBack Off
}