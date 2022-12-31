﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class BottomNoise : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
	[Range(0.01f, 0.5f), Tooltip("Noise Height.")]
	public ClampedFloatParameter height = new ClampedFloatParameter(0.2f, 0.01f, 0.5f);
	[Tooltip("Noise tiling.")]
	public Vector2Parameter tile = new Vector2Parameter(new Vector2(1, 1));
	[Range(0f, 3f), Tooltip("Noise intensity.")]
	public ClampedFloatParameter intencity = new ClampedFloatParameter(1.5f, 0f, 3f);

	public TextureParameter noiseTexture = new TextureParameter(null);
	public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}