using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using ShaderIDs = KinoGlitch.ShaderPropertyIDs;

namespace KinoGlitch {

public sealed partial class DigitalGlitchController
{
    [SerializeField, HideInInspector] Shader _shader = null;

    Material _material;
    Texture2D _noiseTexture;
    RTHandle _trashFrame1;
    RTHandle _trashFrame2;

    static Color RandomColor()
      => new(Random.value, Random.value, Random.value, Random.value);

    void ReleaseResources()
    {
        CoreUtils.Destroy(_material);
        CoreUtils.Destroy(_noiseTexture);
        _trashFrame1?.Release();
        _trashFrame2?.Release();
        _material = null;
        _noiseTexture = null;
        _trashFrame1 = null;
        _trashFrame2 = null;
    }

    public void PrepareBuffers(GraphicsFormat format)
    {
        if (_shader == null) _shader = Shader.Find("Hidden/KinoGlitch/Digital");
        if (_material == null) _material = CoreUtils.CreateEngineMaterial(_shader);

        if (_noiseTexture == null)
        {
            _noiseTexture = new Texture2D(64, 32, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point
            };
            UpdateNoiseTexture();
        }

        if (_trashFrame1 != null) return;

        _trashFrame1 = RTHandles.Alloc(Vector3.one, format, name: "_KinoGlitch_TrashFrame1");
        _trashFrame2 = RTHandles.Alloc(Vector3.one, format, name: "_KinoGlitch_TrashFrame2");
    }

    void UpdateNoiseTexture()
    {
        var color = RandomColor();

        for (var y = 0; y < _noiseTexture.height; y++)
        for (var x = 0; x < _noiseTexture.width; x++)
        {
            if (Random.value > 0.89f) color = RandomColor();
            _noiseTexture.SetPixel(x, y, color);
        }

        _noiseTexture.Apply();
    }

    public RTHandle ConsumeTrashFrame1()
      => Time.frameCount % 13 == 0 ? _trashFrame1 : null;

    public RTHandle ConsumeTrashFrame2()
      => Time.frameCount % 73 == 0 ? _trashFrame2 : null;

    public Material UpdateMaterial()
    {
        if (Random.value > Mathf.Lerp(0.9f, 0.5f, Intensity)) UpdateNoiseTexture();

        _material.SetFloat(ShaderIDs.Intensity, Intensity);
        _material.SetTexture(ShaderIDs.NoiseTex, _noiseTexture);
        var trash = Random.value > 0.5f ? _trashFrame1.rt : _trashFrame2.rt;
        _material.SetTexture(ShaderIDs.TrashTex, trash);

        return _material;
    }
}

} // namespace KinoGlitch
