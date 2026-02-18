Shader "Hidden/KinoGlitch/Digital"
{
HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

TEXTURE2D_X(_NoiseTex);
TEXTURE2D_X(_TrashTex);
SAMPLER(sampler_NoiseTex);
SAMPLER(sampler_TrashTex);

float _Intensity;

float4 Frag(Varyings input) : SV_Target
{
    float2 uv = input.texcoord;

    float4 glitch = SAMPLE_TEXTURE2D_X(_NoiseTex, sampler_NoiseTex, uv);

    float thresh = 1.001 - _Intensity * 1.001;
    float wD = step(thresh, pow(glitch.z, 2.5));
    float wF = step(thresh, pow(glitch.w, 2.5));
    float wC = step(thresh, pow(glitch.z, 3.5));

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
