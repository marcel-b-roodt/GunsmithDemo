using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Magazine : WeaponComponent
{
	public int MaxLightCapacity { get; }
	public int MaxMediumCapacity { get; }
	public int MaxHeavyCapacity { get; }
}
