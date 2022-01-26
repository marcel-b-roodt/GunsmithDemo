using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Barrel : WeaponComponent
{
	public float BarrelLength { get; }
	public List<float> BulletDamageRange { get; } = new List<float>();
	public Vector2 RecoilKickMultiplier { get; }
	public float AccuracyMultiplier { get; }
	public float BulletVelocityMultiplier { get; }


}
