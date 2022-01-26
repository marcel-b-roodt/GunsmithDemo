using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	[Header("Rotation")]
	public bool InvertX = false;
	public bool InvertY = false;
	[Range(-90f, 90f)]
	public float DefaultVerticalAngle = 20f;
	[Range(-90f, 90f)]
	public float MinVerticalAngle = -80f;
	[Range(-90f, 90f)]
	public float MaxVerticalAngle = 80f;
	public float RotationSpeed = 10f;
	public float RotationSharpness = 30f;
	public float MouseSensitivity = 0.01f;

	[Header("Obstruction")]
	public float ObstructionCheckRadius = 0.5f;
	public LayerMask ObstructionLayers = -1;
	public float ObstructionSharpness = 10000f;

	public Transform Transform { get; private set; }
	public Vector3 PlanarDirection { get; private set; }
	public PlayerMovementController FollowCharacter { get; set; }

	private List<Collider> _internalIgnoredColliders = new List<Collider>();
	private float _currentDistance;
	private float _targetVerticalAngle;
	private RaycastHit _obstructionHit;
	private int _obstructionCount;
	private RaycastHit[] _obstructions = new RaycastHit[MaxObstructions];
	private float _obstructionTime;

	private const int MaxObstructions = 32;

	PlayerInputActions playerInputActions;
	private Vector2 inputLook;

	void OnValidate()
	{
		DefaultVerticalAngle = Mathf.Clamp(DefaultVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
	}

	private void Awake()
	{
		Transform = this.transform;

		playerInputActions = new PlayerInputActions();
		playerInputActions.Player.Look.performed += ctx => inputLook = ctx.ReadValue<Vector2>(); 
		
		_targetVerticalAngle = 0f;

		PlanarDirection = Vector3.forward;
	}

	private void Start()
	{
		SetFollowCharacter(GameObject.FindGameObjectWithTag(Helpers.Tags.Player).GetComponent<PlayerMovementController>());
	}

	private void LateUpdate()
	{
		Vector2 mouseLook = inputLook;
		Vector3 lookInputVector = new Vector3(mouseLook.x * MouseSensitivity, mouseLook.y * MouseSensitivity, 0f);

		// Prevent moving the camera while the cursor isn't locked
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			lookInputVector = Vector3.zero;
		}

		UpdateWithInput(Time.deltaTime, lookInputVector);
	}

	// Set the transform that the camera will orbit around
	public void SetFollowCharacter(PlayerMovementController character)
	{
		FollowCharacter = character;
		PlanarDirection = FollowCharacter.Motor.CharacterForward;

		// Ignore the character's collider(s) for camera obstruction checks
		_internalIgnoredColliders.Clear();
		_internalIgnoredColliders.AddRange(FollowCharacter.GetComponentsInChildren<Collider>());
	}

	public void UpdateWithInput(float deltaTime, Vector3 lookInputVector)
	{
		if (FollowCharacter)
		{
			if (InvertX)
			{
				lookInputVector.x *= -1f;
			}
			if (InvertY)
			{
				lookInputVector.y *= -1f;
			}

			// Process rotation input
			Quaternion rotationFromInput = Quaternion.Euler(FollowCharacter.Motor.CharacterUp * (lookInputVector.x * RotationSpeed));
			PlanarDirection = rotationFromInput * PlanarDirection;
			PlanarDirection = Vector3.Cross(FollowCharacter.Motor.CharacterUp, Vector3.Cross(PlanarDirection, FollowCharacter.Motor.CharacterUp));
			_targetVerticalAngle -= (lookInputVector.y * RotationSpeed);
			_targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, MinVerticalAngle, MaxVerticalAngle);

			// Calculate smoothed rotation
			Quaternion planarRot = Quaternion.LookRotation(PlanarDirection, FollowCharacter.Motor.CharacterUp);
			Quaternion verticalRot = Quaternion.Euler(_targetVerticalAngle, 0, 0);
			Quaternion targetRotation = Quaternion.Slerp(Transform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-RotationSharpness * deltaTime));

			// Apply rotation
			Transform.rotation = targetRotation;
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