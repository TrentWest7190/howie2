using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LimitlessGlitch3 : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);

    [Range(0f, 50f), Tooltip("Effect speed.")]
    public ClampedFloatParameter speed = new ClampedFloatParameter(1f,0f, 50f);
    [Range(0f, 505f), Tooltip("block size (higher value = smaller blocks).")]
    public ClampedFloatParameter blockSize = new ClampedFloatParameter(1f,0f, 505f);

    [Range(0f, 25f), Tooltip("maximum color shift on X axis.")]
    public ClampedFloatParameter maxOffsetX = new ClampedFloatParameter(1f,0f, 25f);
    [Range(0f, 25f), Tooltip("maximum color shift on Y axis.")]
    public ClampedFloatParameter maxOffsetY = new ClampedFloatParameter(1f, 0f, 25f);

    public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}