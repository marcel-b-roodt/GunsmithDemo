using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Muzzle : WeaponComponent
{
	public SuppressionLevel SuppressionLevel { get; }
	public Vector2 RecoilKickMultiplier { get; }
	public float RecoilRecoveryMultiplier { get; }
	public float AccuracyMultiplier { get; }
}
