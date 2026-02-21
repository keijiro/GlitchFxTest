using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using ShaderIDs = KinoGlitch.ShaderPropertyIDs;

namespace KinoGlitch {

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("KinoGlitch/Digital Glitch Controller")]
public sealed class DigitalGlitchController : MonoBehaviour
{
    [field:SerializeField, Range(0, 1)]
    public float Intensity { get; set; }

    [SerializeField, HideInInspector] Shader _shader = null;

    Material _material;
    DigitalGlitchNoiseTexture _noise;
    RTHandle _trashFrame1;
    RTHandle _trashFrame2;

    void OnDestroy() => ReleaseResources();

    void OnDisable() => ReleaseResources();

    void ReleaseResources()
    {
        CoreUtils.Destroy(_material);
        _noise?.Dispose();
        _trashFrame1?.Release();
        _trashFrame2?.Release();
        _material = null;
        _noise = null;
        _trashFrame1 = null;
        _trashFrame2 = null;
    }

    public void PrepareBuffers(GraphicsFormat format)
    {
        if (_shader == null) _shader = Shader.Find("Hidden/KinoGlitch/Digital");
        if (_material == null) _material = CoreUtils.CreateEngineMaterial(_shader);

        _noise ??= new DigitalGlitchNoiseTexture(64, 32);

        if (_trashFrame1 != null) return;

        _trashFrame1 = RTHandles.Alloc(Vector3.one, format, name: "_KinoGlitch_TrashFrame1");
        _trashFrame2 = RTHandles.Alloc(Vector3.one, format, name: "_KinoGlitch_TrashFrame2");
    }

    public RTHandle ConsumeTrashFrame1()
      => Time.frameCount % 13 == 0 ? _trashFrame1 : null;

    public RTHandle ConsumeTrashFrame2()
      => Time.frameCount % 73 == 0 ? _trashFrame2 : null;

    public Material UpdateMaterial()
    {
        if (Random.value > Mathf.Lerp(0.9f, 0.5f, Intensity)) _noise.Update();

        _material.SetFloat(ShaderIDs.Intensity, Intensity);
        _material.SetTexture(ShaderIDs.NoiseTex, _noise.Texture);
        var trash = Random.value > 0.5f ? _trashFrame1.rt : _trashFrame2.rt;
        _material.SetTexture(ShaderIDs.TrashTex, trash);

        return _material;
    }
}

} // namespace KinoGlitch
