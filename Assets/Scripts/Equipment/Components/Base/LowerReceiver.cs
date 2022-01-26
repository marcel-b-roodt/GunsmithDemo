using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class LowerReceiver : WeaponComponent
{
	public AmmoType AmmoType { get; }
	public Vector2 BaseRecoilKick { get; }
	public float BaseRecoilRecoveryRate { get; }
	public float BaseVelocity { get; }
	public float BaseReloadTime { get; }
	public Size Size { get; } 
}
