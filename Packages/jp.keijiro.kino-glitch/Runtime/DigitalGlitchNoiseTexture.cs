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
    //
    // Public interface
    //

    public const int TextureWidth = 2048;

    public Texture2D Texture { get; private set; }

    public DigitalGlitchNoiseTexture()
    {
        Texture = new Texture2D(TextureWidth, 1, TextureFormat.ARGB32, false, true)
        {
            wrapMode = TextureWrapMode.Repeat,
            filterMode = FilterMode.Point
        };
        _pixels = new NativeArray<uint>(TextureWidth, Allocator.Persistent);
        Update();
    }

    public void Dispose()
    {
        CoreUtils.Destroy(Texture);
        Texture = null;
        if (_pixels.IsCreated) _pixels.Dispose();
    }

    public void Update()
    {
        UpdateNoise(ref _pixels, (uint)Random.Range(1, int.MaxValue));
        Texture.LoadRawTextureData(_pixels);
        Texture.Apply();
    }

    //
    // Private members
    //

    NativeArray<uint> _pixels;

    [BurstCompile]
    static void UpdateNoise(ref NativeArray<uint> pixels, uint seed)
    {
        var random = new Unity.Mathematics.Random(seed);
        for (var j = 0; j < 32; j++)
        {
            var start = random.NextInt(0, pixels.Length);
            var span1 = random.NextInt(1, 3);
            var span2 = (int)(math.pow(random.NextFloat(), 16) * 100);
            var end = math.min(start + span1 + span2, pixels.Length);
            var color = random.NextUInt();
            for (var i = start; i < end; i++) pixels[i] = color;
        }
    }
}

} // namespace KinoGlitch
