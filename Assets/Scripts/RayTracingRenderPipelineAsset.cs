using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

[CreateAssetMenu(fileName ="Ray Tracing RP", menuName = "Rendering/Ray Tracing Render Pipeline Asset")]
public class RayTracingRenderPipelineAsset : RenderPipelineAsset
{
    [SerializeField]
    private RayTracingShader rayTracingShader;
    [SerializeField, Range(1, 1024)]
    private int imageWidth = 400;
    [SerializeField, Range(1, 1024)]
    private int imageHeight = 225;

    protected override RenderPipeline CreatePipeline()
    {
        return new RayTracingRenderPipline(rayTracingShader, imageWidth, imageHeight);
    }
}
