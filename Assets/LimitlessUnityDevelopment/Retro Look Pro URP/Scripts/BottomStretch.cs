using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;



public class BottomStretch : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
	[Tooltip("Height of Noise.")]
	public ClampedFloatParameter height = new ClampedFloatParameter(0.2f, 0.01f, 0.5f);
	[Space]
	[Tooltip("Stretch noise distortion.")]
	public BoolParameter distort = new BoolParameter(true);
	[Tooltip("Noise distortion frequency.")]
	public ClampedFloatParameter frequency = new ClampedFloatParameter(0.2f, 0.1f, 100f);
	[Tooltip("Noise distortion amplitude.")]
	public ClampedFloatParameter amplitude = new ClampedFloatParameter(0.2f, 0.01f, 200f);
	[Tooltip("Enable noise distortion random frequency.")]
	public BoolParameter distortRandomly = new BoolParameter(true);
	public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}