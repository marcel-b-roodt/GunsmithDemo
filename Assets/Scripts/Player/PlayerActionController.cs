using Helpers;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerActionState
{
	Ready,
	Firing,
	Reloading,
	WeaponSwitching
}

public class PlayerActionController : MonoBehaviour
{
	private PlayerMovementController playerMovementController;
	private PlayerCamera playerCamera;
	private PlayerStatus playerStatus;

	public float InteractRange = 2.5f;

	private PlayerInputActions playerInputActions;
	private bool inputFire;
	private bool inputFireConsumed;
	private bool inputAim;
	private bool inputAimConsumed;
	private bool inputInteract;
	private bool inputInteractConsumed;
	private bool inputWeaponSwitch;
	private bool inputWeaponSwitchConsumed;
	private bool inputReload;
	private bool inputReloadConsumed;

	public PlayerActionState CurrentActionState { get; private set; }
	public PlayerActionState PreviousActionState { get; private set; }
	public float TimeEnteredState { get; private set; }
	public float TimeSinceEnteringState { get { return Time.time - TimeEnteredState; } }

	public void TransitionToState(PlayerActionState newState)
	{
		PreviousActionState = CurrentActionState;
		OnStateExit(PreviousActionState, newState);
		CurrentActionState = newState;
		TimeEnteredState = Time.time;
		OnStateEnter(newState, PreviousActionState);
	}

	public void OnStateEnter(PlayerActionState state, PlayerActionState fromState)
	{
		switch (state)
		{

		}
	}

	public void OnStateExit(PlayerActionState state, PlayerActionState toState)
	{
		switch (state)
		{

		}
	}

	private void Awake()
	{
		playerMovementController = gameObject.GetComponent<PlayerMovementController>();

		playerInputActions = new PlayerInputActions();
		playerInputActions.Player.Fire.performed += ctx => { inputFire = ctx.ReadValueAsButton(); };
		playerInputActions.Player.Fire.canceled += ctx => { inputFire = ctx.ReadValueAsButton(); inputFireConsumed = false; };
		playerInputActions.Player.Aim.performed += ctx => { inputAim = ctx.ReadValueAsButton(); };
		playerInputActions.Player.Aim.canceled += ctx => { inputAim = ctx.ReadValueAsButton(); inputAimConsumed = false; };
		playerInputActions.Player.Interact.performed += ctx => { inputInteract = ctx.ReadValueAsButton(); };
		playerInputActions.Player.Interact.canceled += ctx => { inputInteract = ctx.ReadValueAsButton(); inputInteractConsumed = false; };
		playerInputActions.Player.WeaponSwitch.performed += ctx => { inputWeaponSwitch = ctx.ReadValueAsButton(); };
		playerInputActions.Player.WeaponSwitch.canceled += ctx => { inputWeaponSwitch = ctx.ReadValueAsButton(); inputWeaponSwitchConsumed = false; };
		playerInputActions.Player.Reload.performed += ctx => { inputReload = ctx.ReadValueAsButton(); };
		playerInputActions.Player.Reload.canceled += ctx => { inputReload = ctx.ReadValueAsButton(); inputReloadConsumed = false; };
	}

	private void Start()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		if (Gamepad.current != null)
		{
			if (Gamepad.current.aButton.wasPressedThisFrame)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}
		else
		{
			if (Mouse.current.leftButton.wasPressedThisFrame)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}

		switch (CurrentActionState)
		{
			case PlayerActionState.Ready:
				{
					if (inputInteract && !inputInteractConsumed)
					{
						Interact();
						inputInteractConsumed = true;
					}

					if (inputAim && !inputAimConsumed)
					{
						SetAimDownSights(true);
						inputAimConsumed = true;
					}
					else if (!inputAim && !inputAimConsumed)
					{
						SetAimDownSights(false);
					}

					if (inputFire && !inputFireConsumed)
					{
						TransitionToState(PlayerActionState.Firing);
						inputFireConsumed = true;
					}

					if (!inputWeaponSwitchConsumed)
					{
						//if (inputWeaponSwitch)

					}

					break;
				}
		}
	}

	public void SetAimDownSights(bool aim)
	{

	}

	public void WeaponSwitch()
	{

	}

	public void Interact()
	{
		//TODO: Handle different cases, e.g. picking up, opening door, using item, etc
		GrabInteractable();
	}

	private void GrabInteractable()
	{
		int layerMask = LayerMask.GetMask(Helpers.Layers.Interactable);
		
		var ray = GlobalReferences.MainCamera.ScreenPointToRay(new Vector3(0.5f * Screen.width, 0.5f * Screen.height));
		var hit = Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange, layerMask);
		Debug.DrawRay(ray.origin, ray.direction * InteractRange, Color.green);

		if (hit)
		{
			var interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
			interactable.Activate();
		}
	}

	private void OnEnable()
	{
		this.playerInputActions.Enable();
	}

	private void OnDisable()
	{
		this.playerInputActions.Disable();
	}
}
