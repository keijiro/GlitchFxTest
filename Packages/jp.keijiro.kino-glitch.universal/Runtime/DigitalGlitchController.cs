using Unity.Mathematics;
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
    (RTHandle frame1, RTHandle frame2) _history;

    void OnDestroy() => ReleaseResources();

    void OnDisable() => ReleaseResources();

    void Update() => _noise?.Update();

    void ReleaseResources()
    {
        CoreUtils.Destroy(_material);
        _noise?.Dispose();
        _history.frame1?.Release();
        _history.frame2?.Release();
        _material = null;
        _noise = null;
        _history = (null, null);
    }

    public bool IsActive => Intensity > 0;

    public void PrepareBuffers(GraphicsFormat format)
    {
        if (_history.frame1 != null) return;
        _history.frame1 = RTHandles.Alloc(Vector3.one, format, name: "_KinoGlitch_History1");
        _history.frame2 = RTHandles.Alloc(Vector3.one, format, name: "_KinoGlitch_History2");
    }

    public RTHandle ConsumeHistoryFrame()
    {
        if (Time.frameCount % 13 == 0) return _history.frame1;
        if (Time.frameCount % 73 == 0) return _history.frame2;
        return null;
    }

    public Material UpdateMaterial(int width, int height)
    {
        if (_material == null) _material = CoreUtils.CreateEngineMaterial(_shader);
        _noise ??= new DigitalGlitchNoiseTexture();

        var blocks = DigitalGlitchNoiseTexture.TextureWidth;
        var aspect = (float)width / height;
        var rows = Mathf.RoundToInt(Mathf.Sqrt(blocks / aspect));
        var cols = Mathf.RoundToInt(rows * aspect);

        var flag = (math.hash(math.int2(Time.frameCount, 1)) & 1) != 0;
        var history = flag ? _history.frame1.rt : _history.frame2.rt;

        _material.SetInt(ShaderIDs.BlockCols, cols);
        _material.SetInt(ShaderIDs.BlockRows, rows);
        _material.SetFloat(ShaderIDs.Threshold, 1 - Intensity);
        _material.SetTexture(ShaderIDs.NoiseTex, _noise.Texture);
        _material.SetTexture(ShaderIDs.HistoryTex, history);
        return _material;
    }
}

} // namespace KinoGlitch
