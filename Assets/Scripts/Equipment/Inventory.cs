using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
	//TODO: Create a list of all of the items that we have of each category, that we can use to craft our weapons
	//TODO: Be able to dispose of the items
	//TODO: Remove items from the Inventory and add them to the Weapon when we craft. If we replace a part on the gun, put the item back here

	public List<Barrel> Barrels;
	public List<FireControlGroup> FireControlGroups;
	public List<LowerReceiver> LowerReceivers;
	public List<Muzzle> Muzzles;
	public List<Optics> Optics;
	public List<RearGrip> RearGrips;
	public List<Stock> Stocks;
	public List<TriggerGroup> TriggerGroups;
	public List<UpperReceiver> UpperReceivers;

	public Inventory()
	{
		Barrels = new List<Barrel>();
		FireControlGroups = new List<FireControlGroup>();
		LowerReceivers = new List<LowerReceiver>();
		Muzzles = new List<Muzzle>();
		Optics = new List<Optics>();
		RearGrips = new List<RearGrip>();
		Stocks = new List<Stock>();
		TriggerGroups = new List<TriggerGroup>();
		UpperReceivers = new List<UpperReceiver>();
	}
}
