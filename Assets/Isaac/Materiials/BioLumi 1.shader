Shader "Custom/URP_BioluminescentFlora"
{
    Properties
    {
        _BaseColor ("Base Plant Color", Color) = (0.1, 0.25, 0.2, 1)
        _MainTex ("Albedo Texture", 2D) = "white" {}
        [HDR] _EmissionColor ("Emission Color (HDR)", Color) = (0, 1, 0.8, 1)
        _PulseSpeed ("Glow Pulse Speed", Float) = 2.0
        _RimPower ("Rim Glow Power", Range(1.0, 8.0)) = 3.0
        _WindSpeed ("Wind Sway Speed", Float) = 1.5
        _WindStrength ("Wind Sway Strength", Float) = 0.04
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "Queue"="Geometry" 
            "RenderPipeline"="UniversalPipeline" 
        }

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float3 normalWS     : TEXCOORD0;
                float3 viewDirWS    : TEXCOORD1;
                float2 uv           : TEXCOORD2;
            };

            Texture2D _MainTex;
            SamplerState sampler_MainTex;

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _EmissionColor;
                float _PulseSpeed;
                float _RimPower;
                float _WindSpeed;
                float _WindStrength;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                // Procedural vertex wind sway (stronger at the tips/top where UV.y is higher)
                float3 posOS = input.positionOS.xyz;
                float heightFactor = saturate(input.uv.y); 
                float wind = sin(_Time.y * _WindSpeed + posOS.x * 2.0 + posOS.z * 2.0) * _WindStrength * heightFactor;
                posOS.x += wind;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(posOS);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);

                output.positionHCS = vertexInput.positionCS;
                output.normalWS = normalInput.normalWS;
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                output.uv = input.uv;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Sample texture
                half4 texColor = _MainTex.Sample(sampler_MainTex, input.uv) * _BaseColor;

                // Organic Pulsing Emission (Glows brighter at tips)
                float pulse = (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5);
                float tipMask = saturate(input.uv.y); 
                half3 emission = _EmissionColor.rgb * (0.4 + pulse * 0.6) * (tipMask * 1.5 + 0.5);

                // Rim / Fresnel Edge Glow
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(input.viewDirWS);
                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _RimPower);
                half3 rimGlow = _EmissionColor.rgb * fresnel * 2.0;

                // Combine Base, Emission, and Rim
                half3 finalRGB = texColor.rgb + emission + rimGlow;

                return half4(finalRGB, texColor.a);
            }
            ENDHLSL
        }
    }
}