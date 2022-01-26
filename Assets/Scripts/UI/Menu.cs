using UnityEngine;
using UnityEngine.UI;
using Helpers;
using System;
using System.Collections;

public class Menu : MonoBehaviour
{
	public Image FadeScreen { get; set; }

	private bool busy;

	public virtual void Awake()
	{
		FadeScreen = GameObject.FindGameObjectWithTag(UIComponents.CameraUI)
			.FindGameObjectInChildren(UIComponents.FadeScreen).GetComponent<Image>();
		FadeScreen.gameObject.SetActive(false);
	}

	public virtual void Start()
	{

	}

	public virtual void Update()
	{

	}

	public void FadeOutBeforeAction(float fadeTime, Action action)
	{
		busy = true;
		FadeScreen.gameObject.SetActive(true);
		StartCoroutine(FadeOut(fadeTime, action));
	}

	public void FadeIn(float fadeTime)
	{
		busy = true;
		FadeScreen.gameObject.SetActive(true);
		StartCoroutine(FadeIn(fadeTime, () => { }));
	}

	#region Fades
	private IEnumerator FadeOut(float seconds, Action action)
	{
		for (float alphaIncrement = 0f; alphaIncrement <= 1; alphaIncrement += ((1 / seconds) * Time.deltaTime))
		{
			Color colour = FadeScreen.color;
			colour.a = alphaIncrement;
			FadeScreen.color = colour;
			yield return true;
		}

		action.Invoke();
		busy = false;
		yield return 0;
	}

	private IEnumerator FadeIn(float seconds, Action action)
	{
		for (float alphaIncrement = 1f; alphaIncrement >= 0; alphaIncrement -= ((1 / seconds) * Time.deltaTime))
		{
			Color colour = FadeScreen.color;
			colour.a = alphaIncrement;
			FadeScreen.color = colour;
			yield return true;
		}

		action.Invoke();
		busy = false;
		yield return 0;
	}
	#endregion
}
