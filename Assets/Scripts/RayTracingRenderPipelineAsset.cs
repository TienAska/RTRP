using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

[CreateAssetMenu(fileName ="Ray Tracing RP", menuName = "Rendering/Ray Tracing Render Pipeline Asset")]
public class RayTracingRenderPipelineAsset : RenderPipelineAsset
{
    [SerializeField]
    private RayTracingShader rayTracingShader;

    protected override RenderPipeline CreatePipeline()
    {
        return new RayTracingRenderPipline(rayTracingShader);
    }
}
