using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace KinoGlitch {

[BurstCompile]
sealed class DigitalGlitchNoiseTexture : IDisposable
{
    public Texture2D Texture { get; private set; }
    NativeArray<Color32> _pixels;

    static Color32 NextColor(ref Unity.Mathematics.Random random)
      => new
      (
          (byte)random.NextInt(256),
          (byte)random.NextInt(256),
          (byte)random.NextInt(256),
          (byte)random.NextInt(256)
      );

    [BurstCompile]
    static void UpdateNoise(ref NativeArray<Color32> pixels, uint seed)
    {
        var random = new Unity.Mathematics.Random(seed);
        for (var j = 0; j < 32; j++)
        {
            var start = random.NextInt(0, pixels.Length);
            var length = random.NextInt(1, 3) + (int)(math.pow(random.NextFloat(), 10) * 64);
            var color = NextColor(ref random);

            for (var i = start; i < math.min(start + length, pixels.Length); i++)
            {
                pixels[i] = color;
            }
        }
    }

    public DigitalGlitchNoiseTexture(int width, int height)
    {
        Texture = new Texture2D(width, height, TextureFormat.ARGB32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };
        _pixels = new NativeArray<Color32>(width * height, Allocator.Persistent);
        Update();
    }

    public void Update()
    {
        var seed = (uint)Random.Range(1, int.MaxValue);
        UpdateNoise(ref _pixels, seed);

        Texture.LoadRawTextureData(_pixels);
        Texture.Apply();
    }

    public void Dispose()
    {
        CoreUtils.Destroy(Texture);
        Texture = null;
        if (_pixels.IsCreated) _pixels.Dispose();
    }
}

} // namespace KinoGlitch
