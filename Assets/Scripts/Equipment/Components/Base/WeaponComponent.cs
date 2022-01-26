using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class WeaponComponent
{
	public float CurrentCondition { get; private set; } //Total health of component
	public float MaxCondition { get; private set; } //Total health of component
	public float ConditionDamageResistance { get; } //Reduces flat condition damage directly
	public float Reliability { get; } //Factor which will multiply condition damage in a weapon when used in conjunction with other components.

	public bool IsDestroyed { get { return CurrentCondition <= 0; } }

	public void DamageComponent(float damage)
	{
		var effectiveDamage = Mathf.Max(damage - ConditionDamageResistance, 1);
		CurrentCondition = Mathf.Max(CurrentCondition - effectiveDamage, 0);
	}

	public void RepairComponent(float health)
	{
		CurrentCondition = Mathf.Min(CurrentCondition + health, MaxCondition);
	}
}
