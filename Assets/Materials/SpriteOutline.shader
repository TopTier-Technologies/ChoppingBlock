Shader "Universal Render Pipeline/Sprites/Outline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize ("Outline Size", Range(0, 10)) = 1
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "PreviewType"="Plane"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                half4 _Color;
                half4 _OutlineColor;
                float _OutlineSize;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                color *= IN.color;

                // Calculate outline
                float outline = 0.0;
                float2 pixelSize = _MainTex_TexelSize.xy * _OutlineSize;

                // Sample surrounding pixels
                outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(pixelSize.x, 0)).a;
                outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-pixelSize.x, 0)).a;
                outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(0, pixelSize.y)).a;
                outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(0, -pixelSize.y)).a;
                outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(pixelSize.x, pixelSize.y)).a;
                outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-pixelSize.x, pixelSize.y)).a;
                outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(pixelSize.x, -pixelSize.y)).a;
                outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-pixelSize.x, -pixelSize.y)).a;

                outline = saturate(outline);

                // Draw outline where main sprite is transparent
                if (color.a < 0.01 && outline > 0.01)
                {
                    color = _OutlineColor;
                }

                return color;
            }
            ENDHLSL
        }
    }
}