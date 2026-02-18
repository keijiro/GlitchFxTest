Shader "Hidden/KinoGlitch/Analog"
{
HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

float2 _ScanLineJitter;
float2 _VerticalJump;
float _HorizontalShake;
float2 _ColorDrift;

float Nrand(float x, float y)
{
    return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
}

float4 Frag(Varyings input) : SV_Target
{
    float u = input.texcoord.x;
    float v = input.texcoord.y;

    float jitter = Nrand(v, _TimeParameters.x) * 2 - 1;
    jitter *= step(_ScanLineJitter.y, abs(jitter)) * _ScanLineJitter.x;

    float jump = lerp(v, frac(v + _VerticalJump.y), _VerticalJump.x);
    float shake = (Nrand(_TimeParameters.x, 2) - 0.5) * _HorizontalShake;
    float drift = sin(jump + _ColorDrift.y) * _ColorDrift.x;

    float2 uv1 = frac(float2(u + jitter + shake, jump));
    float2 uv2 = frac(float2(u + jitter + shake + drift, jump));

    float4 src1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv1);
    float4 src2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv2);

    return float4(src1.r, src2.g, src1.b, 1);
}

ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Name "KinoGlitchAnalog"
            ZTest Always ZWrite Off Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
