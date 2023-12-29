Shader "Nexweron/Builtin/ChromaKey/DiffuseCutout_ChromaKey_Alpha"
{
    Properties{
    	_BaseColor("BaseColor", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
    	
		[KeywordEnum(YCbCr, YIQ)][Tooltip(Color Space for Chroma delta)]
		_ColorSpace("ColorSpace", Float) = 0
		_KeyColor("KeyColor", Color) = (0,1,0,1)
		_DChroma("DChroma", range(0.0, 1.0)) = 0.5
		_DChromaT("DChroma Tolerance", range(0.0, 1.0)) = 0.05
        
        [KeywordEnum(Auto, Manual)][Tooltip(Mode for Luma delta)]
        _LumaMode("LumaMode", Float) = 0
        [ShowIfKeyword(_LUMAMODE_MANUAL)]
		_DLuma("DLuma", range(0.0, 1.0)) = 0.5
		[ShowIfKeyword(_LUMAMODE_MANUAL)]
		_DLumaT("DLuma Tolerance", range(0.0, 1.0)) = 0.05
    	
    	_Cutoff("Cutoff", Range (.01, 1)) = .5
	}
	
	CGINCLUDE
	#include "ChromaKey.hlsl"

	sampler2D _MainTex;
	half4 _BaseColor;
	
	half4 _KeyColor;
	half _DChroma;
	half _DChromaT;
	half _DLuma;
	half _DLumaT;
	
    struct Input {
        half2 uv_MainTex;
    };
    ENDCG
    
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha, One Zero
		
		CGPROGRAM
	    #pragma multi_compile _COLORSPACE_YCBCR _COLORSPACE_YIQ
		#pragma multi_compile _LUMAMODE_AUTO _LUMAMODE_MANUAL
	
		#pragma surface surf Lambert addshadow alphatest:_Cutoff
		#pragma target 3.0
	
		void surf (Input input, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, input.uv_MainTex);
		
			if (c.a > 0) {
				#ifdef _LUMAMODE_AUTO
					_DLuma = _DChroma;
					_DLumaT = _DChromaT;
	            #endif
				
			    #ifdef _COLORSPACE_YCBCR
					c = ApplyChromaKeyAlphaYCbCr(c, _KeyColor.rgb, _DChroma, _DChromaT, _DLuma, _DLumaT);
	            #elif _COLORSPACE_YIQ
					c = ApplyChromaKeyAlphaYIQ(c, _KeyColor.rgb, _DChroma, _DChromaT, _DLuma, _DLumaT);
	            #endif
			}
    		c *= _BaseColor;
	        o.Albedo = c.rgb;
	        o.Alpha = c.a;
	    }
		ENDCG
	}
	
    Fallback "Transparent/Cutout/VertexLit"
}
