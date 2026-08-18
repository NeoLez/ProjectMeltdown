using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class ExtractDepthFeature : ScriptableRendererFeature
{
    public RenderTexture destinationTexture;
    private ExtractDepthPass depthPass;

    class ExtractDepthPass : ScriptableRenderPass
    {
        private RenderTexture destTexture;
        private Material blitMaterial;
        private RTHandle destHandle;

        public ExtractDepthPass()
        {
            // Switch to the standard Blit material which correctly respects Render Graph scaling
            blitMaterial = CoreUtils.CreateEngineMaterial("Hidden/Universal Render Pipeline/Blit");
        }

        public void Setup(RenderTexture destination)
        {
            destTexture = destination;
            if (destHandle == null || destHandle.rt != destTexture)
            {
                RTHandles.Release(destHandle);
                destHandle = RTHandles.Alloc(destTexture);
            }
        }

        private class PassData
        {
            public TextureHandle sourceDepth;
            public Material material;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (destTexture == null || blitMaterial == null) return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            TextureHandle srcDepth = resourceData.cameraDepthTexture;

            if (!srcDepth.IsValid()) return;

            TextureHandle dest = renderGraph.ImportTexture(destHandle);

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Extract Camera Depth", out var passData))
            {
                passData.sourceDepth = srcDepth;
                passData.material = blitMaterial;

                // 1. Bind the source depth texture as a readable input for this pass
                builder.UseTexture(srcDepth, AccessFlags.Read);
        
                // 2. Set your custom Render Texture as the write target
                builder.SetRenderAttachment(dest, 0, AccessFlags.Write);

                // 3. Execute the blit safely using the bound input texture
                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    // Omit the source handle here; the raster pass binds UseTexture automatically.
                    Blitter.BlitTexture(context.cmd, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }
        }

        public void Dispose()
        {
            RTHandles.Release(destHandle);
        }
    }

    public override void Create()
    {
        depthPass = new ExtractDepthPass();
        depthPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (destinationTexture != null)
        {
            depthPass.Setup(destinationTexture);
            renderer.EnqueuePass(depthPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (depthPass != null)
        {
            depthPass.Dispose();
        }
    }
}