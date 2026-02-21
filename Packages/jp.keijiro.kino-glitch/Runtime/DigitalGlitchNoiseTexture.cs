using System;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace KinoGlitch {

sealed class DigitalGlitchNoiseTexture : IDisposable
{
    public Texture2D Texture { get; private set; }

    static Color RandomColor()
      => new(Random.value, Random.value, Random.value, Random.value);

    public DigitalGlitchNoiseTexture(int width, int height)
    {
        Texture = new Texture2D(width, height, TextureFormat.ARGB32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };
        Update();
    }

    public void Update()
    {
        var color = RandomColor();

        for (var y = 0; y < Texture.height; y++)
        for (var x = 0; x < Texture.width; x++)
        {
            if (Random.value > 0.89f) color = RandomColor();
            Texture.SetPixel(x, y, color);
        }

        Texture.Apply();
    }

    public void Dispose()
    {
        CoreUtils.Destroy(Texture);
        Texture = null;
    }
}

} // namespace KinoGlitch
