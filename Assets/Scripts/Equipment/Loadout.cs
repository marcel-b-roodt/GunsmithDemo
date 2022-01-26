using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Loadout
{
	public int LightAmmoCount { get; private set; }
	public int MediumAmmoCount { get; private set; }
	public int HeavyAmmoCount { get; private set; }

	public Weapon WeaponOne { get; private set; }
	public Weapon WeaponTwo { get; private set; }
}
