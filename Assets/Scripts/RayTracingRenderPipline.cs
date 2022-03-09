using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using static UnityEngine.Experimental.Rendering.RayTracingAccelerationStructure;

public class RayTracingRenderPipline : RenderPipeline
{
    private RayTracingAccelerationStructure accelerationStructure;
    private RayTracingRenderer renderer;

    public RayTracingRenderPipline(RayTracingShader rayTracingShader, int imageWidth, int imgaeHeight)
    {
        accelerationStructure = new RayTracingAccelerationStructure(new RASSettings(ManagementMode.Automatic, RayTracingModeMask.Everything, ~0));
        // Renderer sphereRenderer = GameObject.Find("Sphere").GetComponent<Renderer>();
        // accelerationStructure.AddInstance(sphereRenderer);
        accelerationStructure.Build();

        renderer = new RayTracingRenderer();
        renderer.rayTracingShader = rayTracingShader;
        renderer.accelerationStructure = accelerationStructure;
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

    protected override void Dispose(bool disposing)
    {
        accelerationStructure.Release();
    }

    private class RayTracingRenderer
    {
        public RayTracingShader rayTracingShader;
        public RayTracingAccelerationStructure accelerationStructure;
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

            cmd.SetRayTracingShaderPass(rayTracingShader, "Sphere");
            accelerationStructure.Build();
            cmd.SetRayTracingAccelerationStructure(rayTracingShader, "_AccelerationStructure", accelerationStructure);
            cmd.SetRayTracingTextureParam(rayTracingShader, renderTarget, renderTarget);
            cmd.DispatchRays(rayTracingShader, "MainRaygenShader", (uint)imageWidth, (uint)imageHeight, 1, camera);

            cmd.Blit(renderTarget, BuiltinRenderTextureType.CameraTarget);

            cmd.ReleaseTemporaryRT(renderTarget);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            context.Submit();
        }
    }
}

