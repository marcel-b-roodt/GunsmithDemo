using Helpers;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static bool Paused { get; set; }

	#region ManagerReferences
	public DebugManager DebugManager
	{
		get
		{
			if (debugManager == null)
				debugManager = gameObject.FindGameObjectInChildren("DebugManager").GetComponent<DebugManager>();

			return debugManager;
		}
	}
	private DebugManager debugManager;

	public PlayerSpawnManager PlayerSpawnManager
	{
		get
		{
			if (playerSpawnManager == null)
				playerSpawnManager = gameObject
					.FindGameObjectInChildren("PlayerManager")
					.GetComponent<PlayerSpawnManager>();

			return playerSpawnManager;
		}
	}
	private PlayerSpawnManager playerSpawnManager;

	public PlayerRoomTransitionManager PlayerRoomTransitionManager
	{
		get
		{
			if (playerRoomTransitionManager == null)
				playerRoomTransitionManager = gameObject
					.FindGameObjectInChildren("PlayerManager")
					.GetComponent<PlayerRoomTransitionManager>();

			return playerRoomTransitionManager;
		}
	}
	private PlayerRoomTransitionManager playerRoomTransitionManager;
	#endregion

	public void ResetState()
	{
		Paused = false;
	}

	//private void Update()
	//{
	//	UpdatePlayerInput();
	//}

	#region Events
	public void SetGamePaused(bool paused) { Paused = paused; }
	#endregion

	//private void UpdatePlayerInput()
	//{
	//	PlayerInputs.ClearInputs();

	//	PlayerInputs.LStick_XAxis = Input.GetAxisRaw(PlayerInputCodes.LStick_XAxis);
	//	PlayerInputs.LStick_YAxis = Input.GetAxisRaw(PlayerInputCodes.LStick_YAxis); 
	//	PlayerInputs.RStick_XAxis = Input.GetAxisRaw(PlayerInputCodes.RStick_XAxis);
	//	PlayerInputs.RStick_YAxis = Input.GetAxisRaw(PlayerInputCodes.RStick_YAxis);
	//	PlayerInputs.L2_Axis = Input.GetAxisRaw(PlayerInputCodes.L2);
	//	PlayerInputs.R2_Axis = Input.GetAxisRaw(PlayerInputCodes.R2);

	//	PlayerInputs.A_Pressed = Input.GetButtonDown(PlayerInputCodes.A);
	//	PlayerInputs.A_Held = Input.GetButton(PlayerInputCodes.A);
	//	PlayerInputs.B_Pressed = Input.GetButtonDown(PlayerInputCodes.B);
	//	PlayerInputs.B_Held = Input.GetButton(PlayerInputCodes.B);
	//	PlayerInputs.X_Pressed = Input.GetButtonDown(PlayerInputCodes.X);
	//	PlayerInputs.X_Held = Input.GetButton(PlayerInputCodes.X);
	//	PlayerInputs.Y_Pressed = Input.GetButtonDown(PlayerInputCodes.Y);
	//	PlayerInputs.Y_Held = Input.GetButton(PlayerInputCodes.Y);

	//	PlayerInputs.L1_Pressed = Input.GetButtonDown(PlayerInputCodes.L1);
	//	PlayerInputs.L1_Held = Input.GetButton(PlayerInputCodes.L1);
	//	PlayerInputs.L3_Pressed = Input.GetButtonDown(PlayerInputCodes.L3);
	//	PlayerInputs.L3_Held = Input.GetButton(PlayerInputCodes.L3);
	//	PlayerInputs.R1_Pressed = Input.GetButtonDown(PlayerInputCodes.R1);
	//	PlayerInputs.R1_Held = Input.GetButton(PlayerInputCodes.R1);
	//	PlayerInputs.R3_Pressed = Input.GetButtonDown(PlayerInputCodes.R3);
	//	PlayerInputs.R3_Held = Input.GetButton(PlayerInputCodes.R3);

	//	PlayerInputs.Start_Pressed = Input.GetButtonDown(PlayerInputCodes.Start);
	//	PlayerInputs.Start_Held = Input.GetButton(PlayerInputCodes.Start);
	//	PlayerInputs.Select_Pressed = Input.GetButtonDown(PlayerInputCodes.Select);
	//	PlayerInputs.Select_Held = Input.GetButton(PlayerInputCodes.Select);

	//	PlayerInputs.Keyboard_Walk_Pressed = Input.GetButtonDown(PlayerInputCodes.Keyboard_Walk);
	//	PlayerInputs.Keyboard_Walk_Held = Input.GetButton(PlayerInputCodes.Keyboard_Walk);
	//}
}
