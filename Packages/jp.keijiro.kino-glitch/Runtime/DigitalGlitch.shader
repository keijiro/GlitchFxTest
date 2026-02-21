Shader "Hidden/KinoGlitch/Digital"
{
HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

TEXTURE2D_X(_NoiseTex);
TEXTURE2D_X(_TrashTex);
SAMPLER(sampler_NoiseTex);
SAMPLER(sampler_TrashTex);
float4 _NoiseTex_TexelSize;
float4 _TrashTex_TexelSize;

int _BlockCols;
int _BlockRows;
float _Threshold;

float4 Frag(Varyings input) : SV_Target
{
    float2 uv = input.texcoord;

    // Block selection
    int2 block = (int2)(uv * float2(_BlockCols, _BlockRows));
    float nx = (block.x + block.y * _BlockCols + 0.5) * _NoiseTex_TexelSize.x;

    // Noise sample
    float4 glitch = SAMPLE_TEXTURE2D_X(_NoiseTex, sampler_NoiseTex, float2(nx, 0.5));
    float4 glitch2 = glitch * glitch;

    // Displacement
    float2 uv2 = _Threshold < glitch2.z ? frac(uv + glitch.xy) : uv;

    // Color source samples
    float4 src_curr = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv2);
    float4 src_prev = SAMPLE_TEXTURE2D_X(_TrashTex, sampler_LinearClamp, uv2);

    // Sample selection
    float3 color = _Threshold < glitch2.w ? src_prev : src_curr;

    // Negate
    float3 neg = saturate(color.grb + (1 - dot(color, float3(1, 1, 1))) * 0.5);
    color = _Threshold < glitch2.z ? neg : color;

    return float4(color, src_curr.a);
}

ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Name "KinoGlitchDigital"
            ZTest Always ZWrite Off Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
