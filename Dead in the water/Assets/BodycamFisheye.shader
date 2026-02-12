Shader "Hidden/Shader/BodycamFisheye"
{
    Properties
    {
        _MainTex("Main Texture", 2DArray) = "grey" {}
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch switch2

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    // ====== PARAMETERS ======
    float _Strength;
    float _ScaleBuffer;
    float _VignetteStrength;
    float _VignetteSmoothness;

    TEXTURE2D_X(_MainTex);

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        float2 uv = input.texcoord;

        // Convert to -1 to 1 space
        float2 centeredUV = uv * 2.0 - 1.0;

        float radius = length(centeredUV);

        // Barrel distortion
        float distortion = 1.0 + _Strength * radius * radius;
        float2 distortedUV = centeredUV * distortion;

        // Inward scale buffer (prevents snapping)
        distortedUV *= _ScaleBuffer;

        // Convert back to 0–1 space
        float2 finalUV = distortedUV * 0.5 + 0.5;

        // Sample scene color
        float3 sceneColor = SAMPLE_TEXTURE2D_X(
            _MainTex,
            s_linear_clamp_sampler,
            ClampAndScaleUVForBilinearPostProcessTexture(finalUV)
        ).xyz;

        // Curved vignette (based on distorted radius)
        float vignette = smoothstep(
            _VignetteSmoothness,
            1.0,
            radius
        );

        sceneColor *= (1.0 - vignette * _VignetteStrength);

        return float4(sceneColor, 1.0);
    }

    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }

        Pass
        {
            Name "Bodycam Fisheye"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }

    Fallback Off
}
