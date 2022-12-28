using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;



public class NTSCEncode : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
    [Tooltip("Blur size.")]
    public ClampedFloatParameter blur = new ClampedFloatParameter(0.83f, 0.01f, 2f);
    [Tooltip("Brigtness.")]
    public ClampedFloatParameter brigtness = new ClampedFloatParameter(3f, 1f, 40f);
    [Tooltip("Floating lines speed")]
    public ClampedFloatParameter lineSpeed = new ClampedFloatParameter(0.01f, 0f, 10f);
    public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}