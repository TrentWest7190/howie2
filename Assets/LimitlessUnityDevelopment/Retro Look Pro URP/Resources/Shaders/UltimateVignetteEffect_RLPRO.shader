Shader "Hidden/Shader/UltimateVignetteEffect_RLPRO"
{
    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
        SAMPLER(_InputTexture);
    
    half4 _Params;
	half3 _InnerColor;
	half4 _Center;
#pragma shader_feature VIGNETTE_CIRCLE
#pragma shader_feature VIGNETTE_SQUARE
#pragma shader_feature VIGNETTE_ROUNDEDCORNERS
	half2 _Params1;


		float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
	half4 color = tex2D(_InputTexture, i.uv);

#if VIGNETTE_CIRCLE
	half d = distance(i.uv, _Center.xy);
	half multiplier = smoothstep(0.8, _Params.x * 0.799, d * (_Params.y + _Params.x));
#elif VIGNETTE_ROUNDEDCORNERS
	half2 uv = -i.uv * i.uv + i.uv;
	half v = saturate(uv.x * uv.y * _Params1.x + _Params1.y);
	half multiplier = smoothstep(0.8, _Params.x * 0.799, v * (_Params.y + _Params.x));
#else
	half multiplier = 1.0;
#endif
	_InnerColor = -_InnerColor;
	color.rgb = (color.rgb - _InnerColor) * max((1.0 - _Params.z * (multiplier - 1.0) - _Params.w), 1.0) + _InnerColor;
	color.rgb *= multiplier;

	return color;
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