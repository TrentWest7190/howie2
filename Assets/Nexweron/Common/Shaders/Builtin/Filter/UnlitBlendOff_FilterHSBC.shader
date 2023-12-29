Shader "Nexweron/Builtin/Filter/UnlitBlendOff_FilterHSBC"
{
    Properties {
    	_MainTex("MainTex", 2D) = "white" { }
    	_BaseColor("BaseColor", Color) = (1,1,1,1)
        
    	[Tooltip(Add color by alpha value)]
	    _TintColor("TintColor", Color) = (1,1,1,0)
    	
        [KeywordEnum(HS, BC, HSBC)]
        _FilterMode("FilterMode", Float) = 0
        
    	[ShowIfKeyword(_FILTERMODE_HS, _FILTERMODE_HSBC)]
    	_Hue("Hue", range(0, 360)) = 0
    	[ShowIfKeyword(_FILTERMODE_HS, _FILTERMODE_HSBC)]
    	_Saturation("Saturation", range(-1, 1)) = 0.0
    	[ShowIfKeyword(_FILTERMODE_BC, _FILTERMODE_HSBC)]
    	_Brightness("Brightness", range(-1, 1)) = 0.0
    	[ShowIfKeyword(_FILTERMODE_BC, _FILTERMODE_HSBC)]
    	_Contrast("Contrast", range(-1, 1)) = 0.0
    }
	
	CGINCLUDE
    #include "UnityCG.cginc"
	#include "HSBC.hlsl"
	
	sampler2D _MainTex;
    half4 _MainTex_ST;
	
	half4 _BaseColor;
    half4 _TintColor;
	half _Hue;
	half _Saturation;
	half _Brightness;
	half _Contrast;
	
    struct v2f {
        half2 uv : TEXCOORD0;
        half4 vertex : SV_POSITION;
    };

	v2f vert (appdata_base v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
        return o;
    }
	
    half4 frag (v2f i) : SV_Target {
        half4 c = tex2D(_MainTex, i.uv)*_BaseColor;
		c.rgb = lerp(c.rgb, _TintColor.rgb, _TintColor.a);
    	
        #ifdef _FILTERMODE_HS
            c.rgb = ApplyHue(c.rgb, _Hue);
    		c.rgb = ApplySaturation(c.rgb, _Saturation);
    	#elif _FILTERMODE_BC
    		c.rgb = ApplyContrast(c.rgb, _Contrast);
    		c.rgb = ApplyBrightness(c.rgb, _Brightness);
        #else
            c.rgb = ApplyHue(c.rgb, _Hue);
    		c.rgb = ApplyContrast(c.rgb, _Contrast);
    		c.rgb = ApplyBrightness(c.rgb, _Brightness);
    		c.rgb = ApplySaturation(c.rgb, _Saturation);
        #endif
    	
        return c;
    }
    ENDCG
    
    SubShader {
		Lighting Off
		AlphaTest Off
    	ZWrite Off
		Blend Off
    	
		Pass {
			CGPROGRAM
			    #pragma multi_compile _FILTERMODE_HS _FILTERMODE_BC _FILTERMODE_HSBC
			    #pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
	}
	
	Fallback Off
}
