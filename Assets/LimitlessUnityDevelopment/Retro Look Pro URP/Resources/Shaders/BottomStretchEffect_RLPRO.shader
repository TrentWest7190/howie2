Shader "Hidden/Shader/BottomStretchEffect_RLPRO"
{
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

	float amplitude;
	float frequency;
	half _NoiseBottomHeight;
	float Time;
	float onOff(float a, float b, float c, float t)
	{
		return step(c, sin(t + a * cos(t * b)));
	}
	float2 twitchHorizonalRand(float amp, float freq, float2 uv, float t)
	{
		float window = 1.0 / (1.0 + 150.0 * (uv.y - fmod(t * freq, 0.1)) * (uv.y - fmod(t * freq, 0.1)));
		uv.x += sin(uv.y * amp + t) / 40.0
			* onOff(2.1, 4.0, 0.3, t)
			* (150.0 + cos(t * 80.0))
			* window;
		uv.x += 20 * _NoiseBottomHeight;
		return uv;
	}
	float2 twitchHorizonal(float amp, float freq, float2 uv, float t)
	{
		float window = 1.0 / (1.0 + 150.0 * (uv.y - fmod(t * freq, 0.1)) * (uv.y - fmod(t * freq, 0.1)));
		uv.x += sin(uv.y * amp + t) / 40.0
			* (150.0 + cos(t * 80.0))
			* window;
		uv.x += 20 * _NoiseBottomHeight;
		return uv;
	}
    
    SAMPLER(_InputTexture);

	float4 FragDist(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);		
		i.uv.y = max(i.uv.y, twitchHorizonal(amplitude,frequency, i.uv,Time * 100.0).x * (_NoiseBottomHeight / 20));
		 half2 positionSS = i.uv ;
		half4 color = tex2D(_InputTexture, positionSS);
		float exp = 1.0;
		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
	}
	float4 FragDistRand(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);	
		i.uv.y = max(i.uv.y, twitchHorizonalRand(amplitude,frequency, i.uv,Time * 100.0).x * (_NoiseBottomHeight / 20));
		 half2 positionSS = i.uv  ;
		half4 color = tex2D(_InputTexture, positionSS);
		float exp = 1.0;
		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
	}
	float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		i.uv.y = max(i.uv.y, (_NoiseBottomHeight / 2) - 0.01);
		 half2 positionSS = i.uv  ;
		half4 color = tex2D(_InputTexture, positionSS);
		float exp = 1.0;
		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
	}
    ENDHLSL

    SubShader
    {
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment FragDist

			ENDHLSL
		}
			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment FragDistRand

			ENDHLSL
		}
			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment Frag

			ENDHLSL
		}
    }
    Fallback Off
}