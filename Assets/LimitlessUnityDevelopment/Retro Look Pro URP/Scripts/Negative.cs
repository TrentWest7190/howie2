using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;



public class Negative : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
	[Range(0f, 2f), Tooltip("Brightness.")]
	public ClampedFloatParameter luminosity = new ClampedFloatParameter(0f, 0f, 1.1f);
	[Range(0f, 1f), Tooltip("Vignette amount.")]
	public ClampedFloatParameter vignette = new ClampedFloatParameter(1f, 0f, 1f);
	[Range(0f, 1f), Tooltip("Contrast amount.")]
	public ClampedFloatParameter contrast = new ClampedFloatParameter(0.7f, 0f, 1f);
	[Range(0f, 1f), Tooltip("Negative amount.")]
	public ClampedFloatParameter negative = new ClampedFloatParameter(1f, 0f, 1f);
	public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}