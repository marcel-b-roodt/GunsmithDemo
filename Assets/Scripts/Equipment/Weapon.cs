using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public float WeaponRange;

	public int MagazineMaxCapacity;
	private int magazineCurrentCapacity;

	public float FireRate;
	private float fireTimer;

	//TODO: Make all the getters to fetch and calculate the properties from the components

	public bool HasRequiredParts
	{
		get
		{
			return Barrel != null 
				&& LowerReceiver != null 
				&& Magazine != null
				&& UpperReceiver != null
				&& RearGrip != null
				&& TriggerGroup != null;
		}
	}

	public bool IsFunctional
	{
		get
		{
			return HasRequiredParts
				&& !Barrel.IsDestroyed
				&& !LowerReceiver.IsDestroyed
				&& !Magazine.IsDestroyed
				&& !UpperReceiver.IsDestroyed
				&& !RearGrip.IsDestroyed
				&& !TriggerGroup.IsDestroyed;
		}
	}

	//Required Components
	private Barrel Barrel;
	private LowerReceiver LowerReceiver; //A receiver should have the fire states
	private Magazine Magazine;
	private UpperReceiver UpperReceiver;
	private RearGrip RearGrip;
	private TriggerGroup TriggerGroup;

	//OptionalComponents
	private FireControlGroup FireControlGroup;
	private Muzzle Muzzle;
	private Optics Optics;
	private Stock Stock;
	private Underbarrel Underbarrel;

	//private WeaponFireState[] FireStates; //Each weapon will have a series of states that it is in. Thi
	//private WeaponReloadState[] ReloadStates;

	public void Fire()
	{
		Debug.Log("Fire!");
		//ReduceComponentCondition();
	}

	public void Reload()
	{
		Debug.Log("Requested Reload");
	}

	public void AimDownSights()
	{
		Debug.Log("Aiming Down Sights");
	}

	public void RequestWeaponSwitch()
	{
		//Finish the current fire/reload state.
		//Switch the weapon afterwards.
		//Return to the state we were in if we come back to this gun
		//e.g. requested after fired a bullet from a bolt-action rifle, need to cock the bolt on return
	}

	private void ReduceComponentCondition()
	{
		//Tally up all the Reliability and Condition Resistance components of the components.
		//Use this total damage to inflict Condition Damage
	}
}
