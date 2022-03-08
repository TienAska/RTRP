Shader "Ray Tracing/ProceduralSphere"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
     Pass
        {
            // RayTracingShader.SetShaderPass must use this name in order to execute the ray tracing shaders from this Pass.
            Name "Sphere"

            Tags{ "LightMode" = "RayTracing" }

            HLSLPROGRAM

            #pragma multi_compile_local RAY_TRACING_PROCEDURAL_GEOMETRY

            #pragma raytracing Sphere

            #include "Common.hlsl"

            struct AttributeData
            {
                float2 barycentrics;
            };

            cbuffer UnityPerMaterial
            {
                float4 _Color;
            };

#if RAY_TRACING_PROCEDURAL_GEOMETRY
            [shader("intersection")]
            void IntersectionMain()
            {
                AttributeData attr;
                attr.barycentrics = float2(0, 0);
                ReportHit(0, 0, attr);
            }
#endif

            [shader("closesthit")]
            void ClosestHitMain(inout RayPayload payload : SV_RayPayload, AttributeData attribs : SV_IntersectionAttributes)
            {
                payload.color = _Color;
            }

            ENDHLSL
        }
    }
}
