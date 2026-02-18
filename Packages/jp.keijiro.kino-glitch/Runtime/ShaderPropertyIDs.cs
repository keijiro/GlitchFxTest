using UnityEngine;

namespace KinoGlitch {

static class ShaderPropertyIDs
{
    public static readonly int ScanLineJitter = Shader.PropertyToID("_ScanLineJitter");
    public static readonly int VerticalJump = Shader.PropertyToID("_VerticalJump");
    public static readonly int HorizontalShake = Shader.PropertyToID("_HorizontalShake");
    public static readonly int ColorDrift = Shader.PropertyToID("_ColorDrift");
    public static readonly int Intensity = Shader.PropertyToID("_Intensity");
    public static readonly int NoiseTex = Shader.PropertyToID("_NoiseTex");
    public static readonly int TrashTex = Shader.PropertyToID("_TrashTex");
}

} // namespace KinoGlitch
