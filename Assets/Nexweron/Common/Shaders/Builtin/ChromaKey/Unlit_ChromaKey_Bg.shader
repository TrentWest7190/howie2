Shader "Nexweron/Builtin/ChromaKey/Unlit_ChromaKey_Bg" 
{
	Properties {
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
	#include "UnityCG.cginc"
	#include "ChromaKey.hlsl"

	sampler2D _MainTex;
	half4 _MainTex_ST;
	half4 _BaseColor;
	
	half4 _BgColor;
    sampler2D _BgTex;
	half4 _BgTex_ST;
	
	half4 _KeyColor;
	half _DChroma;
	half _DChromaT;

	half _Chroma;
	half _Luma;
	half _Saturation;
	half _Alpha;

	struct v2f {
		half2 uv : TEXCOORD0;
        half4 vertex : SV_POSITION;
		#ifdef _BGMODE_TEXTURE
			half2 uv1 : TEXCOORD1;
        #endif
	};
	
	v2f vert(appdata_base input) {
		v2f o;
		o.vertex = UnityObjectToClipPos(input.vertex);
		o.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
	    #ifdef _BGMODE_TEXTURE
		o.uv1 = TRANSFORM_TEX(input.texcoord, _BgTex);
        #endif
		return o;
	}
	
	half4 frag(v2f input) : SV_Target {
		half4 c = tex2D(_MainTex, input.uv);
		if (c.a > 0) {
		    #ifdef _BGMODE_TEXTURE
            half4 c_bg = tex2D(_BgTex, input.uv1);
            #else
            half4 c_bg = _BgColor;
            #endif

			c = ApplyChromaKeyBgYCbCr(c, _KeyColor.rgb, c_bg, _DChroma, _DChromaT, _Chroma, _Luma, _Saturation, _Alpha);
		}
		c *= _BaseColor;
		return c;
	}
	ENDCG
	
	SubShader {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		Lighting Off
		AlphaTest Off
		ZWrite Off
		
		Blend SrcAlpha OneMinusSrcAlpha, One Zero

		Pass {
			CGPROGRAM
		        #pragma multi_compile _BGMODE_COLOR _BGMODE_TEXTURE
		        #pragma vertex vert
		        #pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
	}
	
	Fallback Off
}