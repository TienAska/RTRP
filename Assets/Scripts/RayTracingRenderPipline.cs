using UnityEngine;
using UnityEngine.Rendering;

public class RayTracingRenderPipline : RenderPipeline
{
    RayTracingRenderer renderer = new RayTracingRenderer();

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (Camera camera in cameras)
        {
            renderer.Render(context, camera);
        }
    }

    private class RayTracingRenderer
    {
        public void Render(ScriptableRenderContext context, Camera camera) 
        {
            context.SetupCameraProperties(camera);

            CommandBuffer cmd = CommandBufferPool.Get("Ray Tracing");

            RenderTexture rt = RenderTexture.GetTemporary(new RenderTextureDescriptor(200, 100) { depthBufferBits = 0, enableRandomWrite = true });
            
            cmd.SetRenderTarget(rt);
            cmd.ClearRenderTarget(false, true, Color.clear);
            cmd.Blit(rt, BuiltinRenderTextureType.CameraTarget);
            
            RenderTexture.ReleaseTemporary(rt);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            context.Submit();
        }
    }
}

