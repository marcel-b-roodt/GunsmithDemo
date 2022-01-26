using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class FireControlGroup : WeaponComponent
{
	public List<FireSelectionType> FireSelectionTypes { get; } = new List<FireSelectionType>();
	public float FireRate { get; }
	public float TriggerSensitivity { get; }
}
