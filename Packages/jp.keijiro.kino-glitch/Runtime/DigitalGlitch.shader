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

    int2 block = (int2)(uv * float2(_BlockCols, _BlockRows));
    float nx = (block.x + block.y * _BlockCols + 0.5) * _NoiseTex_TexelSize.x;

    float4 glitch = SAMPLE_TEXTURE2D_X(_NoiseTex, sampler_NoiseTex, float2(nx, 0.5));

    float wD = step(_Threshold, pow(glitch.z, 2.5));
    float wF = step(_Threshold, pow(glitch.w, 2.5));
    float wC = step(_Threshold, pow(glitch.z, 3.5));

    float2 duv = frac(uv + glitch.xy * wD);
    float4 source = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, duv);

    float3 color = lerp(source, SAMPLE_TEXTURE2D_X(_TrashTex, sampler_TrashTex, duv), wF).rgb;

    float3 neg = saturate(color.grb + (1 - dot(color, float3(1, 1, 1))) * 0.5);
    color = lerp(color, neg, wC);

    return float4(color, source.a);
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
