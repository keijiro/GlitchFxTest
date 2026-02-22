using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KinoGlitch {

sealed class DigitalGlitchRendererFeature : ScriptableRendererFeature
{
    [SerializeField] RenderPassEvent _passEvent =
      RenderPassEvent.BeforeRenderingPostProcessing;

    DigitalGlitchPass _pass;

    public override void Create()
      => _pass = new DigitalGlitchPass { renderPassEvent = _passEvent };

    public override void AddRenderPasses
      (ScriptableRenderer renderer, ref RenderingData renderingData)
      => renderer.EnqueuePass(_pass);
}

} // namespace KinoGlitch
