using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace KinoGlitch {

sealed class AnalogGlitchPass : ScriptableRenderPass
{
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        var camera = frameData.Get<UniversalCameraData>().camera;
        var controller = camera.GetComponent<AnalogGlitchController>();
        if (controller == null || !controller.enabled || !controller.IsActive) return;

        var resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer) return;

        var source = resourceData.activeColorTexture;
        if (!source.IsValid()) return;

        var desc = renderGraph.GetTextureDesc(source);
        desc.name = "_KinoAnalogGlitchColor";
        desc.clearBuffer = false;
        desc.depthBufferBits = 0;
        var dest = renderGraph.CreateTexture(desc);

        var mat = controller.UpdateMaterial();
        var param = new RenderGraphUtils.BlitMaterialParameters(source, dest, mat, 0);
        renderGraph.AddBlitPass(param, passName: "KinoGlitch (Analog)");

        resourceData.cameraColor = dest;
    }
}

} // namespace KinoGlitch
