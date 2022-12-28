Shader "Hidden/Shader/AnalogTVNoiseEffect_RLPRO"
{
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
        struct Attributes1
    {
        uint vertexID : SV_VertexID;
		float3 vertex : POSITION;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    struct Varyings1
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
		float2 texcoordStereo   : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
    };
	sampler2D _InputTexture;
	SAMPLER(_Pattern);

	float _Intensity;
	float TimeX;
	half _Fade;
	half barHeight = 6.;
	half barOffset = 0.6;
	half barSpeed = 2.6;
	half barOverflow = 1.2;
	half edgeCutOff;
	half cut;
	half _OffsetNoiseX;
	half _OffsetNoiseY;
	half4 _MainTex_ST;
	half tileX = 0;
	half tileY = 0;
	half angle;
	uint horizontal;

    Varyings1 Vert1(Attributes1 input)
    {
        Varyings1 output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
		float2 pivot = float2(0.5, 0.5);
		// Rotation Matrix
		float cosAngle = cos(angle);
		float sinAngle = sin(angle);
		float2x2 rot = float2x2(cosAngle, -sinAngle, sinAngle, cosAngle);
		// Rotation consedering pivot
		float2 uv = output.positionCS.xy;
		float2 sfsf = mul(rot, uv);
		output.texcoordStereo = sfsf + output.texcoord + float2(_OffsetNoiseX - 0.2f, _OffsetNoiseY), _ScreenSize.zw * float2(tileY, tileX);
		output.texcoordStereo *= float2(tileY, tileX);
        return output;
    }

    float4 CustomPostProcess(Varyings1 input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        half2 positionSS = input.texcoord ;
        float4 outColor = tex2D(_InputTexture, positionSS);
		float3 pat = tex2D(_Pattern, input.texcoordStereo.xy).rgb;
		float3 col = outColor.rgb;
		float direction = horizontal > 0 ? input.texcoord.y : input.texcoord.x;
		float bar = floor(edgeCutOff + sin(direction * barHeight + TimeX * barSpeed) * 50);
		float f = clamp(bar * 0.03, 0, 1);
		col = lerp(pat, col, f);
		col = lerp(outColor.rgb, col, smoothstep(col.r - cut, 0, 1) * _Fade);
		return float4(col, outColor.a);        
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
        Cull Off ZWrite Off ZTest Always


            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert1
            ENDHLSL
        }
    }
    Fallback Off
}