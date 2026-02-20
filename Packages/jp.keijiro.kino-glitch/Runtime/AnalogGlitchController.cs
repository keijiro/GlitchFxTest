using UnityEngine;
using UnityEngine.Rendering;
using ShaderIDs = KinoGlitch.ShaderPropertyIDs;

namespace KinoGlitch {

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("KinoGlitch/Analog Glitch Controller")]
public sealed class AnalogGlitchController : MonoBehaviour
{
    [field:SerializeField, Range(0, 1)]
    public float ScanLineJitter { get; set; }

    [field:SerializeField, Range(0, 1)]
    public float VerticalJump { get; set; }

    [field:SerializeField, Range(0, 1)]
    public float HorizontalShake { get; set; }

    [field:SerializeField, Range(0, 1)]
    public float ColorDrift { get; set; }

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

        var time = Time.time * 606.11f;
        var drift = new Vector4(time * 0.97f, time * 1.11f, time * 1.29f, ColorDrift * 0.04f);
        _material.SetVector(ShaderIDs.ColorDrift, drift);

        return _material;
    }

    void OnDestroy() => ReleaseResources();

    void OnDisable() => ReleaseResources();

    void Update()
      => _verticalJumpTime += Time.deltaTime * VerticalJump * 11.3f;
}

} // namespace KinoGlitch
