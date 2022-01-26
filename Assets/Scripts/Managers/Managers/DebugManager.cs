using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
	public Transform[] debugPositions;

	private GameObject player;
	private GameObject[] enemies;

	private PlayerInputActions playerInputActions;

	private bool debug1Pressed;
	private bool debug2Pressed;
	private bool debug3Pressed;
	private bool debug4Pressed;
	private bool debug5Pressed;
	private bool debug6Pressed;
	private bool debug7Pressed;
	private bool debug8Pressed;
	private bool debug9Pressed;

	// Use this for initialization
	void Awake()
	{
		playerInputActions = new PlayerInputActions();

		playerInputActions.Player.Debug_1.performed += ctx => debug1Pressed = ctx.ReadValueAsButton();
		playerInputActions.Player.Debug_2.performed += ctx => debug2Pressed = ctx.ReadValueAsButton();
		playerInputActions.Player.Debug_3.performed += ctx => debug3Pressed = ctx.ReadValueAsButton();
		playerInputActions.Player.Debug_4.performed += ctx => debug4Pressed = ctx.ReadValueAsButton();
		playerInputActions.Player.Debug_5.performed += ctx => debug5Pressed = ctx.ReadValueAsButton();
		playerInputActions.Player.Debug_6.performed += ctx => debug6Pressed = ctx.ReadValueAsButton();
		playerInputActions.Player.Debug_7.performed += ctx => debug7Pressed = ctx.ReadValueAsButton();
		playerInputActions.Player.Debug_8.performed += ctx => debug8Pressed = ctx.ReadValueAsButton();
		playerInputActions.Player.Debug_9.performed += ctx => debug9Pressed = ctx.ReadValueAsButton();

		player = GameObject.FindGameObjectWithTag(Helpers.Tags.Player);
		enemies = GameObject.FindGameObjectsWithTag(Helpers.Tags.Enemy);
	}

	// Update is called once per frame
	void Update()
	{
		if (debug1Pressed && debugPositions[0] != null)
		{
			player.transform.position = debugPositions[0].position;
			debug1Pressed = false;
			return;
		}

		if (debug2Pressed && debugPositions[1] != null)
		{
			player.transform.position = debugPositions[1].position;
			debug2Pressed = false;
			return;
		}

		if (debug3Pressed && debugPositions[2] != null)
		{
			player.transform.position = debugPositions[2].position;
			debug3Pressed = false;
			return;
		}

		if (debug4Pressed && debugPositions[3] != null)
		{
			player.transform.position = debugPositions[3].position;
			debug4Pressed = false;
			return;
		}

		if (debug5Pressed && debugPositions[4] != null)
		{
			player.transform.position = debugPositions[4].position;
			debug5Pressed = false;
			return;
		}

		if (debug6Pressed && debugPositions[5] != null)
		{
			player.transform.position = debugPositions[5].position;
			debug6Pressed = false;
			return;
		}

		if (debug7Pressed && debugPositions[6] != null)
		{
			player.transform.position = debugPositions[6].position;
			debug7Pressed = false;
			return;
		}

		if (debug8Pressed && debugPositions[7] != null)
		{
			player.transform.position = debugPositions[7].position;
			debug8Pressed = false;
			return;
		}

		if (debug9Pressed && debugPositions[8] != null)
		{
			player.transform.position = debugPositions[8].position;
			debug9Pressed = false;
			return;
		}
	}

	private void OnEnable()
	{
#if DEBUG
		this.playerInputActions.Enable();
#endif
	}

	private void OnDisable()
	{
#if DEBUG
		this.playerInputActions.Disable();
#endif
	}
}
