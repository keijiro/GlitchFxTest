using UnityEngine;
using UnityEngine.Rendering;
using ShaderIDs = KinoGlitch.ShaderPropertyIDs;

namespace KinoGlitch {

public sealed partial class AnalogGlitchController
{
    [SerializeField, HideInInspector] Shader _shader = null;

    Material _material;
    float _verticalJumpTime;

    void ReleaseResources()
    {
        CoreUtils.Destroy(_material);
        _material = null;
    }

    public Material UpdateMaterial()
    {
        if (_shader == null) _shader = Shader.Find("Hidden/KinoGlitch/Analog");
        if (_material == null) _material = CoreUtils.CreateEngineMaterial(_shader);

        _verticalJumpTime += Time.deltaTime * VerticalJump * 11.3f;

        var slThresh = Mathf.Clamp01(1 - ScanLineJitter * 1.2f);
        var slDisp = 0.002f + Mathf.Pow(ScanLineJitter, 3) * 0.05f;
        _material.SetVector(ShaderIDs.ScanLineJitter, new Vector2(slDisp, slThresh));

        var vj = new Vector2(VerticalJump, _verticalJumpTime);
        _material.SetVector(ShaderIDs.VerticalJump, vj);

        _material.SetFloat(ShaderIDs.HorizontalShake, HorizontalShake * 0.2f);

        var cd = new Vector2(ColorDrift * 0.04f, Time.time * 606.11f);
        _material.SetVector(ShaderIDs.ColorDrift, cd);

        return _material;
    }
}

} // namespace KinoGlitch
