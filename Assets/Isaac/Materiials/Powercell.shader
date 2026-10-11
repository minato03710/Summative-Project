Shader "Custom/URP_PowercellEnergyCore"
{
    Properties
    {
        [HDR] _BaseColor ("Core Base Color (HDR)", Color) = (0, 0.8, 1, 1)
        [HDR] _PulseColor ("Pulse Energy Color (HDR)", Color) = (0.2, 1, 0.8, 1)
        _PulseSpeed ("Pulse Speed", Float) = 3.0
        _FresnelPower ("Energy Rim Power", Range(0.5, 8.0)) = 2.5
        _PlasmaSpeed ("Plasma Motion Speed", Float) = 1.5
        _PlasmaTiling ("Plasma Density", Float) = 12.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline"="UniversalPipeline" 
        }

        Blend SrcAlpha One 
        ZWrite Off
        Cull Off          

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

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _PulseColor;
                float _PulseSpeed;
                float _FresnelPower;
                float _PlasmaSpeed;
                float _PlasmaTiling;
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

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
               
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(input.viewDirWS);
                float fresnel = 1.0 - saturate(dot(normalWS, viewDirWS));
                fresnel = pow(fresnel, _FresnelPower);

                float pulse = (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5);
                half4 coreColor = lerp(_BaseColor, _PulseColor, pulse);

              
                float2 scrollingUV = input.uv;
                scrollingUV.y -= _Time.y * _PlasmaSpeed * 0.1;
                float plasmaPattern = sin(scrollingUV.y * _PlasmaTiling) * cos(scrollingUV.x * _PlasmaTiling * 0.5);
                plasmaPattern = saturate(plasmaPattern * 1.5);

            
                half4 finalColor = coreColor * (1.0 + plasmaPattern * 0.8);
                
             
                finalColor.rgb += _PulseColor.rgb * fresnel * 2.0;

                // Alpha blending calculation
                finalColor.a = saturate(fresnel + plasmaPattern * 0.5 + 0.3) * coreColor.a;

                return finalColor;
            }
            ENDHLSL
        }
    }
}