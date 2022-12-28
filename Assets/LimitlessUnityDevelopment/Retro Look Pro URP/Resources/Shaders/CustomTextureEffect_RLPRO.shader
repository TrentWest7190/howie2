Shader "Hidden/Shader/CustomTextureEffect_RLPRO"
{
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    SAMPLER(_InputTexture);
	SAMPLER(_CustomTexture);
	half fade;
    half alpha;

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        float2 positionSS = input.uv;
        float4 col = tex2D(_InputTexture, positionSS);
		float4 col2 = tex2D(_CustomTexture, positionSS);
        if(alpha<1)
		col2.a = fade;
		return lerp(col, col2, fade);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "#CustomTexture#"

			Cull Off ZWrite Off ZTest Always

            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}