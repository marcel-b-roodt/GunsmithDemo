using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuController : Menu
{
	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
		FadeIn(1.5f);
	}

	public override void Update()
	{
		base.Update();
	}
}
