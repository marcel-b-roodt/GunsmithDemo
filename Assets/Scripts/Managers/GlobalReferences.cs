using UnityEngine;

public static class GlobalReferences
{
	public static GameManager GameManager
	{
		get
		{
			if (gameManager == null)
				gameManager = GameObject.FindGameObjectWithTag(Helpers.Tags.GameManager).GetComponent<GameManager>();

			return gameManager;
		}
	}
	private static GameManager gameManager;

	public static Camera MainCamera
	{
		get
		{
			if (mainCamera == null)
				mainCamera = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerCamera).GetComponent<Camera>();

			return mainCamera;
		}
	}
	private static Camera mainCamera;

	public static PlayerHUD PlayerHUD
	{
		get
		{
			if (playerHUD == null)
				playerHUD = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerHUD).GetComponent<PlayerHUD>();

			return playerHUD;
		}
	}
	private static PlayerHUD playerHUD;

	public static MenuManager MenuManager
	{
		get
		{
			if (menuManager == null)
				menuManager = GameObject.FindGameObjectWithTag(Helpers.Tags.MenuManager).GetComponent<MenuManager>();

			return menuManager;
		}
	}
	private static MenuManager menuManager;

	//public static Player Player
	//{
	//	get
	//	{
	//		if (player == null)
	//			player = GameObject.FindGameObjectWithTag(Helpers.Tags.Player).GetComponent<Player>();

	//		return player;
	//	}
	//}
	//private static Player player;

	#region Dialogue
	//public static DialogueRunner DialogueRunner
	//{
	//	get
	//	{
	//		if (dialogueRunner == null)
	//			dialogueRunner = GameObject.FindGameObjectWithTag(Helpers.Tags.DialogueRunner).GetComponent<DialogueRunner>();

	//		return dialogueRunner;
	//	}
	//}
	//private static DialogueRunner dialogueRunner;

	//public static DialogueUI DialogueUI
	//{
	//	get
	//	{
	//		if (dialogueUI == null)
	//			dialogueUI = GameObject.FindGameObjectWithTag(Helpers.Tags.DialogueRunner).GetComponent<DialogueUI>();

	//		return dialogueUI;
	//	}
	//}
	//private static DialogueUI dialogueUI;
	#endregion
}