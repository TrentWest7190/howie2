Shader "Nexweron/Builtin/Lut/Unlit_Lut3D"
{
    Properties {
    	_BaseColor("BaseColor", Color) = (1,1,1,1)
        _MainTex("MainTex", 2D) = "white" {}
    	[NoScaleOffset]
        _LutTex("LutTex", 3D) = "white" {}
        
        [KeywordEnum(Direct, Inverse)][Tooltip(LutTex Green channel direction)]
        _LutGreenDir("LutGreenDir", Float) = 0
        
        _LutContribution("LutContribution", range(0, 1)) = 1.0
    }
	
	CGINCLUDE
    #include "UnityCG.cginc"
	#include "Lut.hlsl"

	float4 _BaseColor;
	sampler2D _MainTex;
    float4 _MainTex_ST;
    sampler3D _LutTex;
    float4 _LutTex_TexelSize;
    float _LutContribution;
	
    struct v2f {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
    };

	v2f vert (appdata_base v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
        return o;
    }
	
    float4 frag (v2f i) : SV_Target {
        float4 ci = tex2D(_MainTex, i.uv);
        float4 c = ci;
    	
        #ifdef _LUTGREENDIR_INVERSE
            c.rgb = ApplyLut3D(c.rgb, _LutTex, _LutTex_TexelSize.w, true);
        #else
            c.rgb = ApplyLut3D(c.rgb, _LutTex, _LutTex_TexelSize.w);
        #endif

    	c = lerp(ci, c, _LutContribution)*_BaseColor;
    	
        return c;
    }
    ENDCG
    
    SubShader {
		Lighting Off
		AlphaTest Off
		Blend SrcAlpha OneMinusSrcAlpha, One Zero
		
		Pass {
			CGPROGRAM
			    #pragma multi_compile _LUTGREENDIR_DIRECT _LUTGREENDIR_INVERSE
			    #pragma vertex vert
				#pragma fragment frag
			ENDCG
		}
	}
	Fallback Off
}
