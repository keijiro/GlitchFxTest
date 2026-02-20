Shader "Hidden/KinoGlitch/Analog"
{
HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Random.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/Noise1D.hlsl"

half _ScanLineJitter;
half2 _VerticalJump;
half _HorizontalShake;
half4 _ColorDrift;
half _HorizontalRipple;

half MirrorRepeat(half x) { return 1 - abs(frac(x * 0.5h) * 2 - 1); }

half4 Frag(Varyings input) : SV_Target
{
    half u = input.texcoord.x;
    half v = input.texcoord.y;

    // Vertical pixel coodinate
    uint p_y = (uint)floor(v * _BlitTexture_TexelSize.w);

    // Pseudo frame count
    uint t = (uint)floor(_TimeParameters.x * 60.0);

    // Scan line jitter
    uint2 jitterSeed1 = uint2(p_y, t);
    uint2 jitterSeed2 = uint2(p_y, t + 1000);
    half jitter1 = GenerateHashedRandomFloat(jitterSeed1) * 2 - 1;
    half jitter2 = GenerateHashedRandomFloat(jitterSeed2) * 2 - 1;
    jitter2 = jitter2 * jitter2 * jitter2 * jitter2 * jitter2;
    half jitter = (jitter1 + jitter2 * 2.5) * _ScanLineJitter;

    // Vertical jump
    half v_disp = frac(v + _VerticalJump.y);
    v_disp = max(1 - smoothstep(0, 0.05, v_disp), v_disp);
    half jump = lerp(v, v_disp, _VerticalJump.x);

    // Color drift
    half drift1 = GradientNoise(jump * 1.5 - _ColorDrift.x, 1);
    half drift2 = GradientNoise(jump * 1.5 - _ColorDrift.y, 2);

    // Horizontal ripple
    half burst = abs(drift1);
    burst = burst / (burst + (1 - burst) * lerp(20, 1, _HorizontalRipple));
    half wiggle = abs(GradientNoise(jump * 20 + _Time.y * 16, 12));
    half ripple = 0.3 * _HorizontalRipple * burst * (wiggle + abs(jitter2));

    // Displaced samples
    half x = u + jitter + _HorizontalShake - ripple;
    half2 uv1 = half2(MirrorRepeat(x + drift1 * _ColorDrift.w), jump);
    half2 uv2 = half2(MirrorRepeat(x + drift2 * _ColorDrift.w), jump);
    half2 uv3 = half2(MirrorRepeat(x - drift2 * _ColorDrift.w), jump);
    half r = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv1).r;
    half g = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv2).g;
    half b = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv3).b;

    return half4(r, g, b, 1);
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
