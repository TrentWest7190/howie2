Shader "Hidden/Shader/VHSScanlinesEffect_RLPRO"
{
    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
		SAMPLER(_InputTexture);
	float4 _ScanLinesColor;
	float _ScanLines;
	float speed;
	float fade;
	float _OffsetDistortion;
	float sferical;
	float barrel;
	float scale;
	float _OffsetColor;
	float2 _OffsetColorAngle;
	float Time;

		float2 FisheyeDistortion(float2 coord, float spherical, float barrel, float scale)
	{
		float2 h = coord.xy - float2(0.5, 0.5);
		float r2 = dot(h, h);
		float f = 1.0 + r2 * (spherical + barrel * sqrt(r2));
		return f * scale * h + 0.5;
	}

		float4 FragH(Varyings i) : SV_Target
		{
			float2 coord = FisheyeDistortion(i.uv, sferical, barrel, scale);
			half4 color = tex2D(_InputTexture, i.uv);
			float lineSize = _ScreenParams.y * 0.005;
			float displacement = ((_Time.x / 4 * 1000) * speed) % _ScreenParams.y;
			float ps;
			ps = displacement + (coord.y * _ScreenParams.y / i.positionCS.w);
			float sc = i.uv.y;
			float4 result;
			result = ((uint)(ps / floor(_ScanLines * lineSize)) % 2 == 0) ? color : _ScanLinesColor;
			result += color * sc;
			return lerp(color,result,fade);
		}

			float4 FragHD(Varyings i) : SV_Target
		{
			float2 coord = FisheyeDistortion(i.uv, sferical, barrel, scale);
			half4 color = tex2D(_InputTexture, i.uv);
			float lineSize = _ScreenParams.y * 0.005;
			float displacement = ((_Time.x / 4 * 1000) * speed) % _ScreenParams.y;
			float ps;
			i.uv.y = frac(i.uv.y + cos((coord.x + _Time.x / 4) * 100) / _OffsetDistortion);
			ps = displacement + (i.uv.y * _ScreenParams.y / i.positionCS.w);
			float sc = i.uv.y;
			float4 result;
			result = ((uint)(ps / floor(_ScanLines * lineSize)) % 2 == 0) ? color : _ScanLinesColor;
			result += color * sc;
			return lerp(color,result,fade);
		}

			float4 FragV(Varyings i) : SV_Target
		{
			float2 coord = FisheyeDistortion(i.uv, sferical, barrel, scale);
			half4 color = tex2D(_InputTexture, i.uv);
			float lineSize = _ScreenParams.y * 0.005;
			float displacement = ((_Time.x / 4 * 1000) * speed) % _ScreenParams.y;
			float ps;
			ps = displacement + (coord.x * _ScreenParams.x / i.positionCS.w);
			float sc = i.uv.y;
			float4 result;
			result = ((uint)(ps / floor(_ScanLines * lineSize)) % 2 == 0) ? color : _ScanLinesColor;
			result += color * sc;
			return lerp(color,result,fade);
		}

			float4 FragVD(Varyings i) : SV_Target
		{
			float2 coord = FisheyeDistortion(i.uv, sferical, barrel, scale);
			half4 color = tex2D(_InputTexture, i.uv);
			float lineSize = _ScreenParams.y * 0.005;
			float displacement = ((_Time.x / 4 * 1000) * speed) % _ScreenParams.y;
			float ps;
			i.uv.x = frac(i.uv.x + cos((coord.y + (_Time.x / 4)) * 100) / _OffsetDistortion);
			ps = displacement + (i.uv.x * _ScreenParams.x / i.positionCS.w);
			float sc = i.uv.y;
			float4 result;
			result = ((uint)(ps / floor(_ScanLines * lineSize)) % 2 == 0) ? color : _ScanLinesColor;
			result += color * sc;
			return lerp(color,result,fade);
		}
    ENDHLSL

    SubShader
    {
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment FragH

			ENDHLSL
		}
			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment FragHD

			ENDHLSL
		}
			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment FragV

			ENDHLSL
		}
			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment FragVD

			ENDHLSL
		}
    }
    Fallback Off
}