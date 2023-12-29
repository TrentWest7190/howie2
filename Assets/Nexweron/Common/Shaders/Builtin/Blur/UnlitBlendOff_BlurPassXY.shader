Shader "Nexweron/Builtin/Blur/UnlitBlendOff_BlurPassXY" {
	Properties{
		_MainTex("MainTex", 2D) = "white" {}
		
		[KeywordEnum(Gauss, Box)]
		_BlurMatrix("BlurMatrix", Float) = 0
		
		[Tooltip(Blur pixel offset XY)]
		_BlurOffset("BlurOffsetXY", Float) = 1.0
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Blur.hlsl"

	sampler2D _MainTex;
	half4 _MainTex_ST;
	half4 _MainTex_TexelSize;

	half _BlurOffset;
	
	struct v2f {
		half2 uv : TEXCOORD0;
        half4 vertex : SV_POSITION;
	};
	
	v2f vert(appdata_base input) {
		v2f o;
		o.vertex = UnityObjectToClipPos(input.vertex);
		o.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
		return o;
	}
	
	half4 fragX(v2f input) : SV_Target {
		half blurOffset = _MainTex_TexelSize*_BlurOffset;
		
		half4 c;
	    #ifdef _BLURMATRIX_BOX
			c = ApplyBlurBoxX3(_MainTex, input.uv, blurOffset);
        #elif _BLURMATRIX_GAUSS
			c = ApplyBlurGaussX3(_MainTex, input.uv, blurOffset);
        #endif
		
		return c;
	}
	
	half4 fragY(v2f input) : SV_Target {
		half blurOffset = _MainTex_TexelSize*_BlurOffset;
		
		half4 c;
	    #ifdef _BLURMATRIX_BOX
			c = ApplyBlurBoxY3(_MainTex, input.uv, blurOffset);
        #elif _BLURMATRIX_GAUSS
			c = ApplyBlurGaussY3(_MainTex, input.uv, blurOffset);
        #endif
		
		return c;
	}
	ENDCG
	
	SubShader {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent"}
		Lighting Off
		ZWrite Off
		AlphaTest Off
		
		Blend Off

		Pass {
			CGPROGRAM
				#pragma multi_compile _BLURMATRIX_BOX _BLURMATRIX_GAUSS
				#pragma vertex vert
				#pragma fragment fragX
				#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
		Pass {
			CGPROGRAM
				#pragma multi_compile _BLURMATRIX_BOX _BLURMATRIX_GAUSS
				#pragma vertex vert
				#pragma fragment fragY
				#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
	}
	
	Fallback Off
}