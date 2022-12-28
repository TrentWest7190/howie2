Shader "Hidden/Shader/ColormapPaletteEffect_RLPRO"
{
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"


    SAMPLER(_InputTexture);

    float _Intensity;
	float4 downsample;
	sampler3D _Colormap;
	float4 _Colormap_TexelSize;
	sampler2D _Palette;
	sampler2D _BlueNoise;
	float4 _BlueNoise_TexelSize;
	float _Opacity;
	float _Dither;
float width;
float height;
	float2 Resolution;
	half CalcLuminance(float3 color)
	{
		return dot(color, float3(0.299f, 0.587f, 0.114f));
	}

	float4 Frag0(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
float2 uv = i.uv;
                uv.x *= width;
                uv.y *= height;
                uv.x = round(uv.x);
                uv.y = round(uv.y);
                uv.x /= width;
                uv.y /= height;
		float4 inputColor = tex2D(_InputTexture, uv);
		inputColor = saturate(inputColor);
		float4 colorInColormap = tex3D(_Colormap, inputColor.rgb);
		float random = tex2D(_BlueNoise, i.positionCS.xy  / _BlueNoise_TexelSize.z).r;
		random = saturate(random);
		if (CalcLuminance(colorInColormap.r) > CalcLuminance(colorInColormap.g))
		{
			random = 1 - random;
		}
		float paletteIndex;
		float blend = colorInColormap.b;
		float threshold = saturate((1 / _Dither) * (blend - 0.5 + (_Dither / 2)));
		if (random < threshold)
		{
			paletteIndex = colorInColormap.g;
		}
		else
		{
			paletteIndex = colorInColormap.r;
		}
		float4 result = tex2D(_Palette, float2(paletteIndex, 0));
		result.a = inputColor.a;
		result = lerp(inputColor, result, _Opacity);
		return result;
	}
    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "#NAME#"

			Cull Off ZWrite Off ZTest Always

            HLSLPROGRAM
                #pragma fragment Frag0
                #pragma vertex Vert
            ENDHLSL
        }


    }
    Fallback Off
}