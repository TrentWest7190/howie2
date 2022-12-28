Shader "Hidden/Shader/BottomNoiseEffect_RLPRO"
{
    HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

	SAMPLER(_InputTexture);
	SAMPLER(_NoiseTexture);
	float _Intensity;
	float _OffsetNoiseX;
	half _OffsetNoiseY;
	half _NoiseBottomHeight;
	half _NoiseBottomIntensity;
	half tileX = 0;
	half tileY = 0;

    struct Attributes1
    {
        uint vertexID : SV_VertexID;
		float3 vertex : POSITION;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    struct Varyings1
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
		float2 texcoordStereo   : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
    };
    Varyings1 Vert1(Attributes1 input)
    {
        Varyings1 output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
		output.texcoordStereo = output.texcoord + float2(_OffsetNoiseX - 0.2f, _OffsetNoiseY);
		output.texcoordStereo *= float2(tileY, tileX);
        return output;
    }
	

    float4 CustomPostProcess(Varyings1 input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        half2 positionSS = input.texcoord ;
        float4 outColor = tex2D(_InputTexture, positionSS);
		half2 uv = input.texcoord;		
		float condition = saturate(floor(_NoiseBottomHeight / uv.y));
		float4 noise_bottom = tex2D(_NoiseTexture, input.texcoordStereo) * condition * _NoiseBottomIntensity;
		outColor = lerp(outColor, noise_bottom, -noise_bottom * ((uv.y / _NoiseBottomHeight) - 1.0));		
		return float4(pow(outColor.xyz, float3(1.0, 1.0, 1.0)), outColor.a);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
        Cull Off ZWrite Off ZTest Always

            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert1
            ENDHLSL
        }
    }
    Fallback Off
}