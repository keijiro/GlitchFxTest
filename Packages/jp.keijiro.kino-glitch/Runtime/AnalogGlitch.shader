Shader "Hidden/KinoGlitch/Analog"
{
HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Random.hlsl"

half _ScanLineJitter;
half2 _VerticalJump;
half _HorizontalShake;
half2 _ColorDrift;

half MirrorRepeat(half x) { return 1 - abs(frac(x * 0.5h) * 2 - 1); }

half4 Frag(Varyings input) : SV_Target
{
    half u = input.texcoord.x;
    half v = input.texcoord.y;

    // Vertical pixel coodinate
    uint p_y = (uint)floor(v * _BlitTexture_TexelSize.w);

    // Pseudo frame count
    uint t = (uint)floor(_TimeParameters.x * 60.0);

    // Scan line jitter: Random horizontal offset per scanline (low + high)
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
    half drift = sin(jump + _ColorDrift.y) * _ColorDrift.x;

    // Displaced samples
    half2 uv1 = half2(MirrorRepeat(u + jitter + _HorizontalShake), jump);
    half2 uv2 = half2(MirrorRepeat(u + jitter + _HorizontalShake + drift), jump);
    half4 src1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv1);
    half4 src2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv2);

    return half4(src1.r, src2.g, src1.b, 1);
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
