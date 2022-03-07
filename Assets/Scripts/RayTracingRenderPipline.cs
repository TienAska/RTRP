using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

public class RayTracingRenderPipline : RenderPipeline
{
    private RayTracingRenderer renderer = new RayTracingRenderer();

    public RayTracingRenderPipline(RayTracingShader rayTracingShader)
    {
        renderer.rayTracingShader = rayTracingShader;
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

        public void Render(ScriptableRenderContext context, Camera camera) 
        {
            context.SetupCameraProperties(camera);

            CommandBuffer cmd = CommandBufferPool.Get("Ray Tracing");

            RenderTexture renderTarget = RenderTexture.GetTemporary(new RenderTextureDescriptor(200, 100) { depthBufferBits = 0, enableRandomWrite = true });
            
            cmd.SetRenderTarget(renderTarget);
            cmd.ClearRenderTarget(false, true, Color.clear);

            cmd.SetRayTracingTextureParam(rayTracingShader, "_RenderTarget", renderTarget);
            cmd.DispatchRays(rayTracingShader, "MainRaygenShader", 200, 100, 1, camera);

            cmd.Blit(renderTarget, BuiltinRenderTextureType.CameraTarget);
            
            RenderTexture.ReleaseTemporary(renderTarget);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            context.Submit();
        }
    }
}

