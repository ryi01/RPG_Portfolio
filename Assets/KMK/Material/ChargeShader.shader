Shader "Custom/ChargingOutline"
{
    Properties
    {
        _GlowColor("Glow Color", Color) = (1, 0, 0, 1)
        _FresnelPower("Fresnel Sharpness", Range(0.1, 8.0)) = 4.0
        _OutlineThickness("Thickness", Range(0, 0.1)) = 0.02
        _Intensity("Glow Intensity", Range(1.0, 10.0)) = 5.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
        
        // 가산 블렌딩으로 빛을 뚜렷하게 강조
        Blend SrcAlpha One 
        ZWrite Off
        ZTest LEqual

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 normalWS : TEXCOORD0; float3 viewDirWS : TEXCOORD1; };

            float4 _GlowColor;
            float _FresnelPower, _OutlineThickness, _Intensity;

            Varyings vert(Attributes input)
            {
                Varyings output;
                // 정점을 노멀 방향으로 밀어내어 두께 생성
                float3 pos = input.positionOS.xyz + (input.normalOS * _OutlineThickness);
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(pos);
                output.positionCS = vertexInput.positionCS;
                
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                output.normalWS = normalInput.normalWS;
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(input.normalWS);
                float3 viewDir = normalize(input.viewDirWS);
                
                // 프레넬 수치를 높여 외곽선이 쨍하게 꺾이도록 설정
                float fresnel = pow(1.0 - saturate(dot(normal, viewDir)), _FresnelPower);
                
                // 강도를 곱해 뚜렷하게 보이게 함
                float3 finalGlow = _GlowColor.rgb * fresnel * _Intensity;
                
                return half4(finalGlow, fresnel);
            }
            ENDHLSL
        }
    }
}