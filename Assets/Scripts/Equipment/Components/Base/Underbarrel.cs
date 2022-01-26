using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Underbarrel : WeaponComponent
{
	public UnderbarrelType Type { get; }
	public float AccuracyMultiplier { get; }
	public float RecoilMultiplier { get; }
}
