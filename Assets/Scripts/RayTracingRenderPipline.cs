using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

public class RayTracingRenderPipline : RenderPipeline
{
    private RayTracingRenderer renderer = new RayTracingRenderer();

    public RayTracingRenderPipline(RayTracingShader rayTracingShader, int imageWidth, int imgaeHeight)
    {
        renderer.rayTracingShader = rayTracingShader;
        renderer.imageWidth = imageWidth;
        renderer.imageHeight = imgaeHeight;
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (Camera camera in cameras)
        {
            renderer.Render(context, camera);
        }
    }

    private class RayTracingRenderer
    {
        public RayTracingShader rayTracingShader;
        public int imageWidth;
        public int imageHeight;

        public void Render(ScriptableRenderContext context, Camera camera) 
        {
            context.SetupCameraProperties(camera);

            CommandBuffer cmd = CommandBufferPool.Get("Ray Tracing");

            int renderTarget = Shader.PropertyToID("_RenderTarget");
            cmd.GetTemporaryRT(renderTarget, imageWidth, imageHeight, 0, FilterMode.Point, GraphicsFormat.R8G8B8A8_UNorm, 1, true);

            cmd.SetRenderTarget(renderTarget);
            cmd.ClearRenderTarget(false, true, Color.clear);

            cmd.SetRayTracingTextureParam(rayTracingShader, "_RenderTarget", renderTarget);
            cmd.DispatchRays(rayTracingShader, "MainRaygenShader", (uint)imageWidth, (uint)imageHeight, 1, camera);

            cmd.Blit(renderTarget, BuiltinRenderTextureType.CameraTarget);

            cmd.ReleaseTemporaryRT(renderTarget);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            context.Submit();
        }
    }
}

