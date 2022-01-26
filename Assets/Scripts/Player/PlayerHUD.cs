using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
	public Text StateText;

	void Start()
	{
	}

	void Update()
	{
		StateText.text = GetStateText();
	}

	private string GetStateText()
	{
		return "";
	}
}
