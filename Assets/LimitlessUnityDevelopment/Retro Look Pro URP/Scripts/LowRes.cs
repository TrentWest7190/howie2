using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;



public class LowRes : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
    public IntParameter height = new IntParameter(180);
    public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}