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
        if (_material == null) _material = CoreUtils.CreateEngineMaterial(_shader);

        _material.SetFloat(ShaderIDs.ScanLineJitter, ScanLineJitter * 0.05f);

        var jump = new Vector2(VerticalJump, _verticalJumpTime);
        _material.SetVector(ShaderIDs.VerticalJump, jump);

        var shake = (Random.value * 2 - 1) * HorizontalShake * 0.1f;
        _material.SetFloat(ShaderIDs.HorizontalShake, shake);

        var drift = new Vector2(ColorDrift * 0.04f, Time.time * 606.11f);
        _material.SetVector(ShaderIDs.ColorDrift, drift);

        return _material;
    }
}

} // namespace KinoGlitch
