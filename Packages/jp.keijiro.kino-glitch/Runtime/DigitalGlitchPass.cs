using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace KinoGlitch {

sealed class DigitalGlitchPass : ScriptableRenderPass
{
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        var camera = frameData.Get<UniversalCameraData>().camera;
        var controller = camera.GetComponent<DigitalGlitchController>();
        if (controller == null || !controller.enabled) return;

        var resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer) return;

        var source = resourceData.activeColorTexture;
        if (!source.IsValid()) return;

        var desc = renderGraph.GetTextureDesc(source);
        controller.PrepareBuffers(desc.format);

        var trash1 = controller.ConsumeTrashFrame1();
        if (trash1 != null)
        {
            var imported = renderGraph.ImportTexture(trash1);
            var mat = Blitter.GetBlitMaterial(TextureDimension.Tex2D);
            var param = new RenderGraphUtils.BlitMaterialParameters(source, imported, mat, 0);
            renderGraph.AddBlitPass(param, passName: "KinoGlitch (Trash 1)");
        }

        var trash2 = controller.ConsumeTrashFrame2();
        if (trash2 != null)
        {
            var imported = renderGraph.ImportTexture(trash2);
            var mat = Blitter.GetBlitMaterial(TextureDimension.Tex2D);
            var param = new RenderGraphUtils.BlitMaterialParameters(source, imported, mat, 0);
            renderGraph.AddBlitPass(param, passName: "KinoGlitch (Trash 2)");
        }

        desc.name = "_KinoDigitalGlitchColor";
        desc.clearBuffer = false;
        desc.depthBufferBits = 0;
        var dest = renderGraph.CreateTexture(desc);

        var glitch = controller.UpdateMaterial();
        var blit = new RenderGraphUtils.BlitMaterialParameters(source, dest, glitch, 0);
        renderGraph.AddBlitPass(blit, passName: "KinoGlitch (Digital)");

        resourceData.cameraColor = dest;
    }
}

} // namespace KinoGlitch
