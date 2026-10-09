Shader "Custom/URP_FuturisticForcefield"
{
    Properties
    {
        [HDR] _Color ("Shield Color", Color) = (0, 1, 1, 1)
        _FresnelPower ("Fresnel Rim Power", Range(0.1, 10)) = 3.0
        _IntersectionWidth ("Contact Intersection Width", Range(0.01, 2.0)) = 0.5
        _ScanlineSpeed ("Scanline Panning Speed", Range(-10, 10)) = 2.0
        _ScanlineTiling ("Scanline Density", Float) = 30.0
        _GridTiling ("Hex/Grid Tiling", Float) = 20.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline"="UniversalPipeline" 
        }

        // Additive glowing blend mode with no depth writing
        Blend SrcAlpha One
        ZWrite Off
        Cull Off // Visible from both inside and outside the arena

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

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
                float4 screenPos    : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _FresnelPower;
                float _IntersectionWidth;
                float _ScanlineSpeed;
                float _ScanlineTiling;
                float _GridTiling;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);

                output.positionHCS = vertexInput.positionCS;
                output.normalWS = normalInput.normalWS;
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                output.uv = input.uv;
                output.screenPos = ComputeScreenPos(vertexInput.positionCS);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // 1. Fresnel Edge Rim Calculation
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(input.viewDirWS);
                float NdotV = saturate(dot(normalWS, viewDirWS));
                float fresnel = pow(1.0 - NdotV, _FresnelPower);

                // 2. Scene Depth Intersection (Intersection Seam Glow)
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                float rawDepth = SampleSceneDepth(screenUV);
                float sceneLinearDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                float surfaceLinearDepth = LinearEyeDepth(input.positionHCS.z, _ZBufferParams);
                float depthDiff = saturate((sceneLinearDepth - surfaceLinearDepth) / _IntersectionWidth);
                float intersectionGlow = pow(1.0 - depthDiff, 2.0);

                // 3. Panning Scanline & Grid Effect
                float scanline = sin((input.uv.y * _ScanlineTiling) + (_Time.y * _ScanlineSpeed)) * 0.5 + 0.5;
                float grid = saturate(sin(input.uv.x * _GridTiling) * sin(input.uv.y * _GridTiling) * 10.0);

                // Combine Intensity
                float totalGlow = fresnel + (intersectionGlow * 2.0) + (scanline * grid * 0.4);
                
                half4 finalColor = _Color * totalGlow;
                finalColor.a = saturate(totalGlow) * _Color.a;

                return finalColor;
            }
            ENDHLSL
        }
    }
}