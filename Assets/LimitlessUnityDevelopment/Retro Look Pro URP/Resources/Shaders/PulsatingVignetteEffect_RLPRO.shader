Shader "Hidden/Shader/PulsatingVignetteEffect_RLPRO"
{
    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
		SAMPLER(_InputTexture);

	float vignetteAmount = 1.0;
	float vignetteSpeed = 1.0;
	float Time = 0.0;

	float vignette(float2 uv, float t)
	{
		float vigAmt = 2.5 + 0.1 * sin(t + 5.0 * cos(t * 5.0));
		float c = (1.0 - vigAmt * (uv.y - 0.5) * (uv.y - 0.5)) * (1.0 - vigAmt * (uv.x - 0.5) * (uv.x - 0.5));
		c = pow(abs(c), vignetteAmount);
		return c;
	}

		float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float2 p = i.uv;
		float4 col = tex2D(_InputTexture,p);
		col.rgb *= vignette(p, Time * vignetteSpeed);
		return half4(col);

	}

    ENDHLSL

    SubShader
    {
			Pass
		{
			Name "#PulsatingVignette#"

		Cull Off ZWrite Off ZTest Always

			HLSLPROGRAM
				#pragma fragment Frag
				#pragma vertex Vert
			ENDHLSL
		}
    }
    Fallback Off
}