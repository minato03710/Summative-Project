Shader "Custom/URP_VolumetricLightBeam"
{
    Properties
    {
        [HDR] _Color ("Beam Color (HDR)", Color) = (0, 1, 1, 0.5)
        _DepthFadeDist ("Depth Fade Distance", Range(0.01, 3.0)) = 1.0
        _FresnelPower ("Edge Softening (Fresnel)", Range(0.1, 5.0)) = 1.5
        _NoiseSpeed ("Dust Motion Speed (X, Y)", Vector) = (0.05, 0.2, 0, 0)
        _NoiseTiling ("Dust Density (X, Y)", Vector) = (3.0, 10.0, 0, 0)
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline"="UniversalPipeline" 
        }

        Blend SrcAlpha One // Additive blend for soft glow
        ZWrite Off
        Cull Off          // Visible from all camera angles

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
                float _DepthFadeDist;
                float _FresnelPower;
                float4 _NoiseSpeed;
                float4 _NoiseTiling;
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
                // 1. Soft Edge Rim (Fresnel)
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(input.viewDirWS);
                float fresnel = pow(saturate(dot(normalWS, viewDirWS)), _FresnelPower);

                // 2. Vertical Gradient Fade (Strong at top, soft at bottom)
                float verticalFade = saturate(input.uv.y);

                // 3. Depth Fading (Prevents hard geometry intersections)
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                float rawDepth = SampleSceneDepth(screenUV);
                float sceneLinearDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                float surfaceLinearDepth = LinearEyeDepth(input.positionHCS.z, _ZBufferParams);
                float depthFade = saturate((sceneLinearDepth - surfaceLinearDepth) / _DepthFadeDist);

                // 4. Procedural Scrolling Dust Noise
                float2 noiseUV = (input.uv * _NoiseTiling.xy) + (_Time.y * _NoiseSpeed.xy);
                float dustNoise = sin(noiseUV.x * 12.56) * cos(noiseUV.y * 12.56) * 0.5 + 0.5;
                dustNoise = lerp(0.7, 1.2, dustNoise);

                // Combine All Features
                float alphaMask = fresnel * verticalFade * depthFade;
                half4 finalColor = _Color * dustNoise * alphaMask;
                finalColor.a = alphaMask * _Color.a;

                return finalColor;
            }
            ENDHLSL
        }
    }
}