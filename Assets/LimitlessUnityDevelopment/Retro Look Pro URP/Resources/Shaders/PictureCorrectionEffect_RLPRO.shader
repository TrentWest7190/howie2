Shader "Hidden/Shader/PictureCorrectionEffect_RLPRO"
{
    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
		SAMPLER(_InputTexture);
	float signalAdjustY = 0.0;
	float signalAdjustI = 0.0;
	float signalAdjustQ = 0.0;

	float signalShiftY = 0.0;
	float signalShiftI = 0.0;
	float signalShiftQ = 0.0;
	float gammaCorection = 1.0;
	half3 rgb2yiq(half3 c) {
		return half3(
			(0.2989 * c.x + 0.5959 * c.y + 0.2115 * c.z),
			(0.5870 * c.x - 0.2744 * c.y - 0.5229 * c.z),
			(0.1140 * c.x - 0.3216 * c.y + 0.3114 * c.z)
			);
	};

	half3 yiq2rgb(half3 c) {
		return half3(
			(1.0 * c.x + 1.0 * c.y + 1.0 * c.z),
			(0.956 * c.x - 0.2720 * c.y - 1.1060 * c.z),
			(0.6210 * c.x - 0.6474 * c.y + 1.7046 * c.z)
			);
	};
		float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float3 signal = float3(0.0, 0.0, 0.0);
		float2 p = i.uv;
		signal = rgb2yiq(tex2D(_InputTexture, p).xyz);
		signal.x += signalAdjustY;
		signal.y += signalAdjustI;
		signal.z += signalAdjustQ;
		signal.x *= signalShiftY;
		signal.y *= signalShiftI;
		signal.z *= signalShiftQ;

		float3 rgb = yiq2rgb(signal);
		if (gammaCorection != 1.0) rgb = pow(abs(rgb), gammaCorection);

		return float4(rgb, tex2D(_InputTexture, p).a);



	}

    ENDHLSL

    SubShader
    {
			Pass
		{
			Name "#NAME#"

		Cull Off ZWrite Off ZTest Always

			HLSLPROGRAM
				#pragma fragment Frag
				#pragma vertex Vert
			ENDHLSL
		}
    }
    Fallback Off
}