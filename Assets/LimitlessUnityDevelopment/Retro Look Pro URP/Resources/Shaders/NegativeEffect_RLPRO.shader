Shader "Hidden/Shader/NegativeEffect_RLPRO"
{
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

	SAMPLER(_InputTexture);
	uniform float T;
	uniform float Luminosity;
	uniform float Vignette;
	uniform float Negative;
	uniform float Contrast;
	float3 linearLight(float3 s, float3 d)
	{
		return 2.0 * s + d - 1.0 * Luminosity;
	}


		float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

	float2 uv = i.uv ;
	float3 col = tex2D(_InputTexture, uv).rgb;
	col = lerp(col, 1 - col, Negative*1.5);
	float3 oldfilm = float3(1, 1, 1);
	col *= pow(abs(0.1 * uv.x * (1.0 - uv.x) * uv.y * (1.0 - uv.y)), Contrast) * 1 + Vignette;
	col = dot(float3(0.2126, 0.7152, 0.0722), col);
	col = linearLight(oldfilm, col);
	return float4(col, 1);
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