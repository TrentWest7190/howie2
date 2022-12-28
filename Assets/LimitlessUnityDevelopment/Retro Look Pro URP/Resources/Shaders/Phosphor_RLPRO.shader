Shader "Hidden/Shader/Phosphor_RLPRO"
{
	HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    SAMPLER(_InputTexture);
	SAMPLER(_Tex);

	float speed = 10.00;
	half amount = 5;
	half fade;
	float T;

	float fract(float x) {
		return  x - floor(x);
	}
	float2 fract(float2 x) {
		return  x - floor(x);
	}

	float random(float2 noise)
	{
		return fract(sin(dot(noise.xy, float2(0.0001, 98.233))) * 925895933.14159265359);
	}

	float random_color(float noise)
	{
		return frac(sin(noise));
	}
	float4 Frag0(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
half2 uv = i.uv;
		float4 col =  tex2D(_InputTexture,uv);
		float4 result = col+  tex2D(_Tex, uv);
		return lerp(col,result,fade);
	}

	half4 Frag(Varyings i) : SV_Target
	{
		half2 uv = fract(i.uv.xy / 12 * ((T * speed)));
		half4 color = float4(random(uv.xy), random(uv.xy), random(uv.xy), random(uv.xy));

		color.r *= random_color(sin(T * speed));
		color.g *= random_color(cos(T * speed));
		color.b *= random_color(tan(T * speed));

		return color;

	}

		ENDHLSL

		SubShader
	{
		Pass
		{
			Name "#MixPass#"

		Cull Off ZWrite Off ZTest Always

			HLSLPROGRAM
				#pragma fragment Frag0
				#pragma vertex Vert
			ENDHLSL
		}
			Pass
		{
			Name "#NoisePass#"

		Cull Off ZWrite Off ZTest Always

			HLSLPROGRAM
				#pragma fragment Frag
				#pragma vertex Vert
			ENDHLSL
		}
	}
	Fallback Off
}