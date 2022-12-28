Shader "Hidden/Shader/LowResolution_RLPRO"
{
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

	SAMPLER(_InputTexture);

	half Width;
    half Height;

	float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = i.uv;
                uv.x *= Width;
                uv.y *= Height;
                uv.x = round(uv.x);
                uv.y = round(uv.y);
                uv.x /= Width;
                uv.y /= Height;

		float2 pos = uv;
		float2 centerTextureCoordinate = pos;

		float4 fragmentColor = tex2D(_InputTexture, centerTextureCoordinate);

		return fragmentColor;
	}

    ENDHLSL

    SubShader
    {
			Pass
		{
			Name "#Blit#"

			Cull Off ZWrite Off ZTest Always

			HLSLPROGRAM
				#pragma fragment Frag
				#pragma vertex Vert
			ENDHLSL
		}

    }
    Fallback Off
}