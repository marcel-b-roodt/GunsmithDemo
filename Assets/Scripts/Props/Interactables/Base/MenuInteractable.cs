using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInteractable : Interactable
{
	public MenuTarget MenuTarget;

	public override void Activate()
	{
		Debug.Log($"Activated {gameObject.name}");
		GlobalReferences.MenuManager.OpenMenu(MenuTarget);
	}
}

