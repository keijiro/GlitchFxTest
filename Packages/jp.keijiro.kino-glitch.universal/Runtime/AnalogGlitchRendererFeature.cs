using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KinoGlitch {

sealed class AnalogGlitchRendererFeature : ScriptableRendererFeature
{
    [SerializeField] RenderPassEvent _passEvent =
      RenderPassEvent.AfterRenderingPostProcessing;

    AnalogGlitchPass _pass;

    public override void Create()
      => _pass = new AnalogGlitchPass { renderPassEvent = _passEvent };

    public override void AddRenderPasses
      (ScriptableRenderer renderer, ref RenderingData renderingData)
      => renderer.EnqueuePass(_pass);
}

} // namespace KinoGlitch
