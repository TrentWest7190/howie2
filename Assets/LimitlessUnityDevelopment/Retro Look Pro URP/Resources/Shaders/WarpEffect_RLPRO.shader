Shader "Hidden/Shader/WarpEffect_RLPRO"
{
    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
		SAMPLER(_InputTexture);

	float2 warp = float2(1.0 / 32.0, 1.0 / 24.0);
	float scale;
	float fade;
    half clamp;
	float2 Warp(float2 pos)
	{
		float2 h = pos - float2(0.5, 0.5);
		float r2 = dot(h, h);
		float f = 1.0 + r2 * (warp.x + warp.y * sqrt(r2));
		return f * scale * h + 0.5;
	}
	float2 Warp1(float2 pos)
	{
		pos = pos * 2.0 - 1.0;
		pos *= float2(1.0 + (pos.y * pos.y) * warp.x, 1.0 + (pos.x * pos.x) * warp.y);
		return pos * scale + 0.5;
	}

		float4 Frag0(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float4 col = tex2D(_InputTexture,i.uv  );
		float2 fragCoord = i.uv.xy * _ScreenParams.xy;
		float2 pos = Warp1(fragCoord.xy / _ScreenParams.xy);
        float4 col2;

            col2 = tex2D(_InputTexture, pos  );
            
		return lerp(col,col2,fade);
	}

		float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float4 col = tex2D(_InputTexture,i.uv  );
		float2 fragCoord = i.uv.xy * _ScreenParams.xy;
		float2 pos = Warp(fragCoord.xy / _ScreenParams.xy);
        float4 col2;

            col2 = tex2D(_InputTexture, pos  );
		return lerp(col,col2,fade);
	}

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "#NAME#"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment Frag0
                #pragma vertex Vert
            ENDHLSL
        }
			Pass
		{
			Name "#NAME#"

			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			HLSLPROGRAM
				#pragma fragment Frag
				#pragma vertex Vert
			ENDHLSL
		}
    }
    Fallback Off
}