Shader "Custom/URP_WaterShader"
{
    Properties
    {
        [Header(Color Settings)]
        _ShallowColor ("Shallow Water Color", Color) = (0.0, 0.7, 0.8, 0.7)
        _DeepColor ("Deep Water Color", Color) = (0.0, 0.15, 0.35, 0.9)
        _Transparency ("Base Opacity", Range(0, 1)) = 0.85

        [Header(Distortion Settings)]
        _MainTex ("Noise / Wave Texture", 2D) = "white" {}
        _SpeedX ("Scroll Speed X", Float) = 0.03
        _SpeedY ("Scroll Speed Y", Float) = 0.05
        _WaveScale ("Wave Scale", Float) = 1.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline"="UniversalPipeline" 
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "UnlitWaterPass"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _ShallowColor;
                half4 _DeepColor;
                float _SpeedX;
                float _SpeedY;
                float _WaveScale;
                float _Transparency;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex) * _WaveScale;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Animated UV offsets for fluid motion
                float2 timeOffset = float2(_SpeedX, _SpeedY) * _Time.y;
                
                // Sample noise texture twice in opposite directions for realistic cross-ripples
                float2 uv1 = input.uv + timeOffset;
                float2 uv2 = input.uv - (timeOffset * 0.75f);

                half noise1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv1).r;
                half noise2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv2).r;

                half blendedNoise = (noise1 + noise2) * 0.5f;

                // Lerp between shallow and deep colors based on wave density
                half4 finalColor = lerp(_ShallowColor, _DeepColor, blendedNoise);

                // Apply global opacity multiplier
                finalColor.a *= _Transparency;

                return finalColor;
            }
            ENDHLSL
        }
    }
}