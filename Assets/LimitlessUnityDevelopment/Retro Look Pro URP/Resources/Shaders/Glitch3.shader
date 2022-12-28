Shader "LimitlessGlitch/Glitch3" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
        sampler2D _MainTex;

        float speed;
        float blockSize; 
        float maxOffsetX; 
        float maxOffsetY;

        inline float rand(float2 seed)
        {
            return frac(sin(dot(seed * floor(_Time.y * speed), float2(127.1, 311.7))) * 43758.5453123);
        }

         inline float rand(float seed)
         {
            return rand(float2(seed, 1.0));
         }

        float4 Frag(Varyings i) : SV_Target
        {
            float2 block = rand(floor(i.uv * blockSize));
            float OffsetX = pow(block.x, 8.0) * pow(block.x, 3.0) - pow(rand(7.2341), 17.0) * maxOffsetX;
            float OffsetY = pow(block.x, 8.0) * pow(block.x, 3.0) - pow(rand(7.2341), 17.0) * maxOffsetY;
            float4 r = tex2D(_MainTex, i.uv);
            float4 g = tex2D(_MainTex, i.uv + half2(OffsetX * 0.05 * rand(7.0), OffsetY*0.05*rand(12.0)));
            float4 b = tex2D(_MainTex, i.uv - half2(OffsetX * 0.05 * rand(13.0), OffsetY*0.05*rand(12.0)));

            return half4(r.x, g.g, b.z, (r.a+g.a+b.a));
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex Vert
                #pragma fragment Frag

            ENDHLSL
        }
    }
}