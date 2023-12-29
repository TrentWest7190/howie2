// COMMON
half4 GetColorInfluence(half4 c) {
    c.rgb *= c.a;
    return c;
}

// BLUR BOX 3x3
half4 ApplyBlurBoxXY9(sampler2D baseTex, half2 uv, half2 blurOffset) {
    half2 ox = half2(blurOffset.x, 0);
    half2 oy = half2(0, blurOffset.y);
    
    half4 c = GetColorInfluence(tex2D(baseTex, uv));
    
    c += GetColorInfluence(tex2D(baseTex, uv -ox))		
        +GetColorInfluence(tex2D(baseTex, uv -oy))		
        +GetColorInfluence(tex2D(baseTex, uv +ox))		
        +GetColorInfluence(tex2D(baseTex, uv +oy))
	
        +GetColorInfluence(tex2D(baseTex, uv -ox -oy))
        +GetColorInfluence(tex2D(baseTex, uv -ox +oy))
        +GetColorInfluence(tex2D(baseTex, uv +ox -oy))
        +GetColorInfluence(tex2D(baseTex, uv +ox +oy));
    
    c.rgb /= c.a;
    c.a /= 9;
    return c;
}

half4 ApplyBlurBoxXY5(sampler2D baseTex, half2 uv, half2 blurOffset) {
    half2 ox = half2(blurOffset.x, 0);
    half2 oy = half2(0, blurOffset.x);
    
    half4 c = GetColorInfluence(tex2D(baseTex, uv));
    c += GetColorInfluence(tex2D(baseTex, uv -ox))		
        +GetColorInfluence(tex2D(baseTex, uv -oy))		
        +GetColorInfluence(tex2D(baseTex, uv +ox))		
        +GetColorInfluence(tex2D(baseTex, uv +oy));
    
    c.rgb /= c.a;
    c.a /= 5;
    return c;
}

half4 ApplyBlurBoxX3(sampler2D baseTex, half2 uv, half blurOffsetX) {
    half2 ox = half2(blurOffsetX, 0);
    
    half4 c = GetColorInfluence(tex2D(baseTex, uv));
    c += GetColorInfluence(tex2D(baseTex, uv -ox))		
        +GetColorInfluence(tex2D(baseTex, uv +ox));
    
    c.rgb /= c.a;
    c.a /= 3;
    return c;
}

half4 ApplyBlurBoxY3(sampler2D baseTex, half2 uv, half blurOffsetY) {
    half2 oy = half2(0, blurOffsetY);
    
    half4 c = GetColorInfluence(tex2D(baseTex, uv));
    c += GetColorInfluence(tex2D(baseTex, uv -oy))		
        +GetColorInfluence(tex2D(baseTex, uv +oy));
    
    c.rgb /= c.a;
    c.a /= 3;
    return c;
}

// BLUR GAUSS 3x3
half4 ApplyBlurGaussXY9(sampler2D baseTex, half2 uv, half2 blurOffset) {
    half2 ox = half2(blurOffset.x, 0);
    half2 oy = half2(0, blurOffset.y);
    
    half4 c = 4*GetColorInfluence(tex2D(baseTex, uv));

    c += 2*( GetColorInfluence(tex2D(baseTex, uv -ox))
            +GetColorInfluence(tex2D(baseTex, uv -oy))
            +GetColorInfluence(tex2D(baseTex, uv +ox))
            +GetColorInfluence(tex2D(baseTex, uv +oy)))
            
            +GetColorInfluence(tex2D(baseTex, uv -ox -oy))
            +GetColorInfluence(tex2D(baseTex, uv -ox +oy))
            +GetColorInfluence(tex2D(baseTex, uv +ox -oy))
            +GetColorInfluence(tex2D(baseTex, uv +ox +oy));
    
    c.rgb /= c.a;
    c.a /= 16;
    return c;
}

half4 ApplyBlurGaussX3(sampler2D baseTex, half2 uv, half blurOffsetX) {
    half2 ox = half2(blurOffsetX, 0);
    
    half4 c = 2*GetColorInfluence(tex2D(baseTex, uv));
    c += GetColorInfluence(tex2D(baseTex, uv -ox))
        +GetColorInfluence(tex2D(baseTex, uv +ox));
		
    c.rgb /= c.a;
    c.a /= 4;
    return c;
}

half4 ApplyBlurGaussY3(sampler2D baseTex, half2 uv, half blurOffsetY) {
    half2 oy = half2(0, blurOffsetY);
    
    half4 c = 2*GetColorInfluence(tex2D(baseTex, uv));
    c += GetColorInfluence(tex2D(baseTex, uv -oy))
        +GetColorInfluence(tex2D(baseTex, uv +oy));
		
    c.rgb /= c.a;
    c.a /= 4;
    return c;
}