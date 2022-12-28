using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;



public class OldFilm : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
	[Range(0f, 60f), Tooltip("Frames per second.")]
	public ClampedFloatParameter fps = new ClampedFloatParameter(1f, 0f, 60f);
	[Range(0f, 5f), Tooltip(".")]
	public ClampedFloatParameter contrast = new ClampedFloatParameter(1f, 0f, 5f);

	[Range(-2f, 4f), Tooltip("Image burn.")]
	public ClampedFloatParameter burn = new ClampedFloatParameter(0.88f, -2f, 4f);
	[Range(0f, 16f), Tooltip("Scene cut off.")]
	public ClampedFloatParameter sceneCut = new ClampedFloatParameter(0.88f, 0f, 16f);
	[Range(0f, 1f), Tooltip("Effect fade.")]
	public ClampedFloatParameter fade = new ClampedFloatParameter(0.88f, 0f, 1f);
	public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}