Shader "Nexweron/Builtin/Lut/Diffuse_Lut2D_Filter"
{
    Properties {
        _BaseColor("BaseColor", Color) = (1,1,1,1)
        _MainTex("MainTex", 2D) = "white" {}
	    [NoScaleOffset]
        _LutTex("LutTex", 2D) = "white" {}
        
        [KeywordEnum(Direct, Inverse)][Tooltip(LutTex Green channel direction)]
        _LutGreenDir("LutGreenDir", Float) = 0
        
        _LutContribution("LutContribution", range(0, 1)) = 1.0
    	
    	_Hue("Hue", range(0, 360)) = 0
    	_Saturation("Saturation", range(-1, 1)) = 0.0
    	_Brightness("Brightness", range(-1, 1)) = 0.0
    	_Contrast("Contrast", range(-1, 1)) = 0.0
    }
    
    CGINCLUDE
    #include "UnityCG.cginc"
	#include "Lut.hlsl"
    #include "Assets/Nexweron/Common/Shaders/Builtin/Filter/HSBC.hlsl"

    sampler2D _MainTex;
	float4 _BaseColor;
    sampler2D _LutTex;
    float4 _LutTex_TexelSize;
    float _LutContribution;
    
	float _Hue;
	float _Saturation;
	float _Brightness;
	float _Contrast;
	
    struct Input {
        float2 uv_MainTex;
    };
    ENDCG
    
    SubShader {
    	Tags { "RenderType"="Opaque" }
		LOD 200
    	
	    CGPROGRAM
		    #pragma multi_compile _LUTGREENDIR_DIRECT _LUTGREENDIR_INVERSE
			#pragma surface surf Lambert
			#pragma target 3.0

		void surf (Input IN, inout SurfaceOutput o) {
			float4 ci = tex2D(_MainTex, IN.uv_MainTex);
	        float4 c = ci;
    		
	        #ifdef _LUTGREENDIR_INVERSE
	            c.rgb = ApplyLut2D(c.rgb, _LutTex, _LutTex_TexelSize.w, true);
	        #else
	            c.rgb = ApplyLut2D(c.rgb, _LutTex, _LutTex_TexelSize.w);
	        #endif
			
    		c = lerp(ci, c, _LutContribution)*_BaseColor;
    		
    		c.rgb = ApplyHue(c.rgb, _Hue);
    		c.rgb = ApplyContrast(c.rgb, _Contrast);
    		c.rgb = ApplyBrightness(c.rgb, _Brightness);
    		c.rgb = ApplySaturation(c.rgb, _Saturation);
    		    		
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
		ENDCG
	}
	Fallback "Legacy Shaders/Diffuse"
}
