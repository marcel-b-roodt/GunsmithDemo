using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	[ReadOnly] public MenuTarget ActiveMenu;
	[ReadOnly] public GameObject ActiveMenuTransform;
	public Dictionary<string, GameObject> MenuList = new Dictionary<string, GameObject>();

	private void Awake()
	{
		var menus = GetComponentsInChildren<RectTransform>();
		foreach (var menu in menus)
		{
			menu.gameObject.SetActive(false);
			MenuList.Add(menu.name, menu.gameObject);
		}
	}

	public void OpenMenu(MenuTarget menuTarget)
	{
		var menuName = menuTarget.ToString();

		if (menuTarget == ActiveMenu || menuTarget == MenuTarget.Inactive)
		{
			ActiveMenuTransform.gameObject.SetActive(false);
			ActiveMenuTransform = null;
			ActiveMenu = MenuTarget.Inactive;
			//TODO: Flip the Player Control Scheme to Game
		}
		else if (ActiveMenu == MenuTarget.Inactive)
		{
			ActiveMenuTransform = MenuList[menuName];
			ActiveMenuTransform.gameObject.SetActive(true);
			//TODO: Flip the Player Control Scheme to UI
		}
		else
		{
			ActiveMenuTransform.gameObject.SetActive(false);
			ActiveMenu = menuTarget;
			ActiveMenuTransform = MenuList[menuName];
			ActiveMenuTransform.gameObject.SetActive(true);
		}
	}
}
