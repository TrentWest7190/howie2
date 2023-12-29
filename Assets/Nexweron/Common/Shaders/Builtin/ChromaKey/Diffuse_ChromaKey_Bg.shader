Shader "Nexweron/Builtin/ChromaKey/Diffuse_ChromaKey_Bg"
{
    Properties{
    	_BaseColor("BaseColor", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		_KeyColor("KeyColor", Color) = (0,1,0,1)
		
		[KeywordEnum(Color, Texture)][Tooltip(Bg source for Chroma delta)]
		_BgMode("BgMode", Float) = 0
		[ShowIfKeyword(_BGMODE_COLOR)]
		_BgColor("BgColor", Color) = (1,1,1,1)
		[ShowIfKeyword(_BGMODE_TEXTURE)]
		_BgTex("BgTex", 2D) = "white" {}
		
		_DChroma("DChroma", range(0.0, 1.0)) = 0.5
		_DChromaT("DChroma Tolerance", range(0.0, 1.0)) = 0.05
		_Chroma("Chroma (Main → Bg)", range(0.0, 1.0)) = 0.5
		_Luma("Luma (Main → Bg)", range(0.0, 1.0)) = 0.5
		_Saturation("Saturation (0 → Chroma)", range(0.0, 1.0)) = 1.0
		_Alpha("Alpha (Chroma → Bg)", range(0.0, 1.0)) = 1.0
    }
	
	CGINCLUDE
	#include "ChromaKey.hlsl"

	sampler2D _MainTex;
	half4 _BaseColor;
	
	half4 _BgColor;
    sampler2D _BgTex;
	
	half4 _KeyColor;
	half _DChroma;
	half _DChromaT;

	half _Chroma;
	half _Luma;
	half _Saturation;
	half _Alpha;
	
    struct Input {
        half2 uv_MainTex;
    	half2 uv_BgTex;
    };
    ENDCG
    
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
	    #pragma multi_compile _BGMODE_COLOR _BGMODE_TEXTURE
	
		#pragma surface surf Lambert alpha:fade
		#pragma target 3.0
	
		void surf (Input input, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, input.uv_MainTex);
		
			if (c.a > 0) {
			    #ifdef _BGMODE_TEXTURE
	            half4 c_bg = tex2D(_BgTex, input.uv_BgTex);
	            #else
	            half4 c_bg = _BgColor;
	            #endif

				c = ApplyChromaKeyBgYCbCr(c, _KeyColor.rgb, c_bg, _DChroma, _DChromaT, _Chroma, _Luma, _Saturation, _Alpha);
			}
    		c *= _BaseColor;
	        o.Albedo = c.rgb;
	        o.Alpha = c.a;
	    }
		ENDCG
	}
	
    Fallback "Diffuse"
}
