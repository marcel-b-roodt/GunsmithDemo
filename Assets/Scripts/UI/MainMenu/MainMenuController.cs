using Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : Menu
{
	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
	}

	public override void Update()
	{
		base.Update();
	}

	public void NewGame_OnClick()
	{
		FadeOutBeforeAction(3f, () =>
		{
			SceneManager.LoadScene(Levels.ManagerScene);
			SceneManager.LoadScene(Levels.SafeHouse, LoadSceneMode.Additive);
		});
	}

	public void Options_OnClick()
	{

	}

	public void Credits_OnClick()
	{ }

	public void Quit_OnClick()
	{
		Application.Quit();
	}
}
