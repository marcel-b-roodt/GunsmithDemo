using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class UpperReceiver : WeaponComponent
{
	public int BaseAccuracy { get; }
	public float BaseFireRate { get; }
	public Category Category { get; }
	public Size Size { get; } //TODO: Mismatched sizes will take the heaviest size as the weapon's size
	public int Projectiles { get; } //How many bullets should the weapon fire. Multiply recoil by projectiles. Take into account accuracy variance
}
