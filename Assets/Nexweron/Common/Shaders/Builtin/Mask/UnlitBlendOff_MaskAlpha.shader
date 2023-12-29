Shader "Nexweron/Builtin/Mask/UnlitBlendOff_MaskAlpha" {
	Properties {
		_MainTex("MainTex", 2D) = "white" {}
		_MaskTex("MaskTex", 2D) = "white" {}
		
		[KeywordEnum(Default, Manual)]
		_EdgeMode("Edge Mode", Float) = 0
		
		[ShowIfKeyword(_EDGEMODE_MANUAL)]
		_AlphaPow("AlphaPow", range(1.0, 10.0)) = 1.0
		[ShowIfKeyword(_EDGEMODE_MANUAL)]
		_AlphaEdge("AlphaEdge", range(-0.5, 0.5)) = 0.0
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	half4 _MainTex_ST;
	sampler2D _MaskTex;
	half4 _MaskTex_ST;
	half _AlphaPow;
	half _AlphaEdge;
	
	struct v2f {
		half2 uv0 : TEXCOORD0;
		half2 uv1 : TEXCOORD1;
        half4 vertex : SV_POSITION;
	};
	
	v2f vert(appdata_base input) {
		v2f o;
		o.vertex = UnityObjectToClipPos(input.vertex);
		o.uv0 = TRANSFORM_TEX(input.texcoord, _MainTex);
		o.uv1 = TRANSFORM_TEX(input.texcoord, _MaskTex);
		return o;
	}
	
	half4 frag(v2f input) : SV_Target {
		half4 c = tex2D(_MainTex, input.uv0);
		half4 cm = tex2D(_MaskTex, input.uv1);

		#ifdef _EDGEMODE_MANUAL
		if (cm.a < 1 && cm.a > 0) {
			half a = pow(cm.a, _AlphaPow);
			half e = _AlphaEdge + 0.500001;
			
			if (a > e) {
				a = 0.5 + 0.5*(a - e) / (1 - e);
			} else
			if (a < e) {
				a = 0.5 * a / e;
			}
			cm.a = a;
		}
		#endif
		
		c.a *= cm.a;
		return c;
	}
	ENDCG

	SubShader {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		ZWrite Off
		Lighting Off
		AlphaTest Off
		Blend Off
		
		Pass {
			CGPROGRAM
			#pragma multi_compile _EDGEMODE_DEFAULT _EDGEMODE_MANUAL
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
	}
	
	Fallback Off
}