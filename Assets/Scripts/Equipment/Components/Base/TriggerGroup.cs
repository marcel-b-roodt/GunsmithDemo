using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class TriggerGroup : WeaponComponent
{
	public float Accuracy { get; }
	public float FireRate { get; }
	public float TriggerSensitivity { get; } //TODO: For Consoles, this should make it more sensitive to fire

}
