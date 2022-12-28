using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;



public class Phosphor : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
	public ClampedFloatParameter width = new ClampedFloatParameter(0.4f, 0f, 20f);
	public ClampedFloatParameter amount = new ClampedFloatParameter(0.5f, 0f, 1f);
	public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0f, 1f);
	public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}