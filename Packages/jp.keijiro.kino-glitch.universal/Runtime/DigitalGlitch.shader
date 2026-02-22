Shader "Hidden/KinoGlitch/Digital"
{
HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

TEXTURE2D_X(_NoiseTex);
TEXTURE2D_X(_HistoryTex);
SAMPLER(sampler_NoiseTex);
SAMPLER(sampler_HistoryTex);
float4 _NoiseTex_TexelSize;
float4 _HistoryTex_TexelSize;

int _BlockCols;
int _BlockRows;
half _Threshold;

half3 DamageColor(half3 color)
{
    color = LinearToSRGB(color);
    return SRGBToLinear(half3(color.zx, 1 - color.y));
}

float4 Frag(Varyings input) : SV_Target
{
    float2 uv = input.texcoord;

    // Block selection
    int2 block = (int2)(uv * float2(_BlockCols, _BlockRows));
    float nx = (block.x + block.y * _BlockCols + 0.5) * _NoiseTex_TexelSize.x;

    // Noise sample
    half4 glitch = SAMPLE_TEXTURE2D_X(_NoiseTex, sampler_NoiseTex, float2(nx, 0.5));
    half4 glitch2 = glitch * glitch;
    half glitch_ex = frac(glitch.x * 83.32);

    // Displacement
    if (_Threshold < glitch2.z) uv = frac(uv + glitch.xy);

    // Color source samples
    half4 src_cur = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
    half4 src_prev = SAMPLE_TEXTURE2D_X(_HistoryTex, sampler_LinearClamp, uv);

    // Sample selection
    half3 color = _Threshold < glitch2.w ? src_prev : src_cur;

    // Damaged blocks
    if (_Threshold * 0.2 + 0.8 < glitch_ex) color = DamageColor(color);

    return half4(color, src_cur.a);
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
