using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu]
public class RayTracingRenderPipelineAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new RayTracingRenderPipline();
    }
}
