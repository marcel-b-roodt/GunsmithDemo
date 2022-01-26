using KinematicCharacterController;
using System.Collections.Generic;
using UnityEngine;
using Helpers;
using SensorToolkit;
using System.Linq;

public enum PlayerMovementState
{
	Default,
	Sliding,
	Sprinting,
}

public class PlayerMovementController : BaseCharacterController
{
	//TODO: Make getters for all of these derived properties. Use multipliers and add sliders to configure the properties
	//e.g. SprintVelocityThreshold should be 0.6f or something, with a getter grabbing MaxStableRunSpeed * SprintVelocityThreshold
	[Header("Stable Movement")]
	public float RunSpeed = 5f;
	[Range(0, 1)] public float CrouchSpeedFactor = 0.7f;
	[Range(1, 3)] public float SprintSpeedMultiplier = 1.6f;
	[Range(0, 1)] public float WalkSpeedFactor = 0.5f;
	[Range(0, 1)] public float SprintVelocityThresholdFactor = 0.3f;
	public float StableMovementSharpness = 12;
	public float OrientationSharpness = 12;

	public bool IsCrouching { get { return isCrouching; } }
	private float CrouchSpeed { get { return RunSpeed * CrouchSpeedFactor; } }
	private float SprintVelocityThreshold { get { return RunSpeed * SprintVelocityThresholdFactor; } }

	private bool shouldBeCrouching = false;
	private bool isCrouching = false;
	private float crouchTime = 0;

	[Header("Air Movement")]
	[Range(0, 20)] public float MaxAirSpeed = 20f;
	[Range(0, 3)] public float AirControlFactor = 1.1f;
	[Range(0, 4)] public float Drag = 0.1f;

	private float AirSpeed { get { return RunSpeed; } }
	private float AirAccelerationSpeed { get { return RunSpeed * AirControlFactor; } }

	[Header("Jumping")]
	public float JumpHeight = 1.3f;
	public float JumpCooldownTime = 0.2f;

	private bool jumpRequested = false;
	private bool jumpConsumed = false;
	private bool jumpedThisFrame = false;

	[Header("Sliding")]
	public float SlideVelocityThresholdPreGrace = 2.25f;
	public float SlideVelocityThresholdPostGrace = 5f;
	[Range(1, 3)] public float SlideSpeedMultiplier = 2.5f;
	[Range(0, 3)] public float SlideRecoverySpeedMultiplier = 1f;
	[Range(1, 3)] public float StandingSlideOnLandVelocityMultiplier = 2f;
	[Range(1, 3)] public float CrouchingSlideOnLandVelocityMultiplier = 1.5f;
	[Range(0, 1)] public float SlideMinimumDuration = 0.3f;
	[Range(1, 4)] public float SlideDrag = 2.5f;
	[Range(0, 3)] public float SlideGravityAccelerationFactor = 1f;
	[Range(0, 3)] public float SlideGravityUpwardMotionMultiplier = 1.5f;
	public float SlideSlopeGraceTime = 0.1f;
	public float SlideSlopeGraceAngle = 45f;
	public float SlideSlopeUnstableAngle = 15f;
	[Range(0, 360)] public int SlideRotationDegreesPerSecondStable = 135;
	[Range(0, 360)] public int SlideRotationDegreesPerSecondUnstable = 90;

	private bool SlideIsPostGraceThreshold { get { return TimeSinceEnteringState >= SlideSlopeGraceTime; } }
	private float StandingSlideOnLandVelocityThreshold { get { return RunSpeed * StandingSlideOnLandVelocityMultiplier; } }
	private float CrouchingSlideOnLandVelocityThreshold { get { return CrouchSpeed * CrouchingSlideOnLandVelocityMultiplier; } }

	private Vector3 slideDirection;
	private bool startedSlide;
	private bool slideRecovering;
	private bool slideUnstable;

	[Header("Wall Jump")]
	public float WallJumpRayLength = 0.6f;
	public float WallJumpHeight = 1.8f;
	public float WallJumpCooldownTime = 0.2f;
	public float WallJumpMinWallAngle = 80f;
	public float WallJumpMaxWallAngle = 100f;
	public float WallJumpMaxJumpAngleFromNormal = 90f;

	private RaySensor[] wallJumpRaySensors;
	private bool wallJumpRequested;
	private bool wallJumpConsumed;
	private float timeWallJumped;

	private float TimeSinceWallJump { get { return Time.time - timeWallJumped; } }

	[Header("Camera")]
	public float CameraMovementSharpness = 12f;
	public float CrouchingCameraHeight = 0.85f;
	public float StandingCameraHeight = 1.5f;

	private Transform movementCameraHolder;
	private float movementAffectedCameraTargetHeight = 1.5f;

	[Header("Misc")]
	public List<Collider> IgnoredColliders;
	public bool OrientTowardsGravity = false;
	public Vector3 Gravity = new Vector3(0, -20f, 0);

	public bool IsGrounded { get { return Motor.GroundingStatus.IsStableOnGround; } }
	public float TimeSinceGrounded { get { return IsGrounded ? 0 : Time.time - timeLeftStableGround; } }
	public float TimeInAir { get { return IsGrounded ? 0 : Time.time - timeLeftStableGround; } }

	private Collider[] _probedColliders = new Collider[8];
	private Vector3 moveInputVector;
	private Vector3 internalVelocityAdd = Vector3.zero;
	private float timeLeftStableGround;
	private float maxStableSlopeAngle = 45f;

	private PlayerInputActions playerInputActions;
	private Vector2 inputMove;
	private Vector2 inputLook;
	private bool inputWalk;
	private bool inputSprint;
	private bool inputSprintConsumed;
	private bool inputJump;
	private bool inputJumpConsumed;
	private bool inputCrouch;

	private PlayerAnimationManager playerAnimationManager;

	public PlayerMovementState CurrentCharacterState { get; private set; }
	public PlayerMovementState PreviousCharacterState { get; private set; }
	public float TimeEnteredState { get; private set; }
	public float TimeSinceEnteringState { get { return Time.time - TimeEnteredState; } }
	public bool LocalMovementIsForwardFacing { get { return inputMove.y > 0; } }

	private void Awake()
	{
		wallJumpRaySensors = gameObject.FindGameObjectInChildren("WallJumpSensors").GetComponents<RaySensor>();
		foreach (var sensor in wallJumpRaySensors)
		{
			sensor.Length = WallJumpRayLength;
		}

		playerInputActions = new PlayerInputActions();
		playerInputActions.Player.Move.performed += ctx => inputMove = ctx.ReadValue<Vector2>();
		playerInputActions.Player.Look.performed += ctx => inputLook = ctx.ReadValue<Vector2>();
		playerInputActions.Player.Walk.performed += ctx => { inputWalk = ctx.ReadValueAsButton(); };
		playerInputActions.Player.Sprint.performed += ctx => { inputSprint = ctx.ReadValueAsButton(); };
		playerInputActions.Player.Sprint.canceled += ctx => { inputSprint = ctx.ReadValueAsButton(); inputSprintConsumed = false; };
		playerInputActions.Player.Jump.performed += ctx => { inputJump = ctx.ReadValueAsButton(); };
		playerInputActions.Player.Jump.canceled += ctx => { inputJump = ctx.ReadValueAsButton(); inputJumpConsumed = false; };
		playerInputActions.Player.Crouch.performed += ctx => { inputCrouch = ctx.ReadValueAsButton(); };

		playerAnimationManager = GetComponent<PlayerAnimationManager>();

		movementCameraHolder = gameObject.FindGameObjectInChildren("MovementCameraHolder").transform;

		TransitionToState(PlayerMovementState.Default);
	}

	private void Start()
	{
		if (IgnoredColliders == null)
			IgnoredColliders = new List<Collider>();
	}

	public void TransitionToState(PlayerMovementState newState)
	{
		PreviousCharacterState = CurrentCharacterState;
		OnStateExit(PreviousCharacterState, newState);
		CurrentCharacterState = newState;
		TimeEnteredState = Time.time;
		OnStateEnter(newState, PreviousCharacterState);
		//Debug.Log($"State Change: Previous state was {PreviousCharacterState}; New state is {CurrentPlayerState}");
	}

	public void OnStateEnter(PlayerMovementState state, PlayerMovementState fromState)
	{
		switch (state)
		{
			case PlayerMovementState.Default:
				{
					break;
				}
			case PlayerMovementState.Sprinting:
				{
					//Debug.Log("Started Sprint");
					break;
				}
			case PlayerMovementState.Sliding:
				{
					Motor.MaxStableSlopeAngle = SlideSlopeGraceAngle;
					slideUnstable = false;

					var moveInputVectorAtSlide = new Vector3(inputMove.x, 0f, inputMove.y).normalized;
					slideDirection = CalculatePlanarRotation(GlobalReferences.MainCamera.transform.rotation) * moveInputVectorAtSlide;
					startedSlide = true; //If we are slide recovering, then we didn't start the slide

					HandleCrouching(true);
					playerAnimationManager.SetSlide(true);
					break;
				}
		}
	}

	public void OnStateExit(PlayerMovementState state, PlayerMovementState toState)
	{
		switch (state)
		{
			case PlayerMovementState.Default:
				{
					break;
				}
			case PlayerMovementState.Sprinting:
				{
					//Debug.Log("Stopped Sprint");
					break;
				}
			case PlayerMovementState.Sliding:
				{
					Motor.MaxStableSlopeAngle = maxStableSlopeAngle;

					if (!Motor.GroundingStatus.IsStableOnGround || !inputCrouch)
						HandleCrouching(false);

					playerAnimationManager.SetSlide(false);
					break;
				}
		}
	}

	/// <summary>
	/// This is called every frame by PlayerController in order to tell the character what its inputs are
	/// </summary>
	public void SetInputs()
	{
		// Clamp input
		Vector3 moveInputVector = new Vector3(inputMove.x, 0f, inputMove.y).normalized * (inputWalk ? WalkSpeedFactor : 1);
		playerAnimationManager.SetMovement(moveInputVector.magnitude);

		Quaternion cameraPlanarRotation = CalculatePlanarRotation(GlobalReferences.MainCamera.transform.rotation);

		// Handle state transition from input
		switch (CurrentCharacterState)
		{
			case PlayerMovementState.Default:
				{
					// Move and look inputs
					this.moveInputVector = cameraPlanarRotation * moveInputVector;

					// Jumping input
					if (inputJump && !inputJumpConsumed)
					{
						inputJumpConsumed = true;

						if (Motor.GroundingStatus.FoundAnyGround)
						{
							jumpRequested = true;
						}
						else
						{
							wallJumpRequested = true;
						}
					}

					if (inputCrouch && Motor.GroundingStatus.IsStableOnGround)
					{
						HandleCrouching(true);
					}

					if (inputSprint && !inputSprintConsumed && Motor.GroundingStatus.IsStableOnGround && LocalMovementIsForwardFacing)
					{
						inputSprintConsumed = true;
						TransitionToState(PlayerMovementState.Sprinting);
					}

					break;
				}
			case PlayerMovementState.Sprinting:
				{
					// Move and look inputs
					this.moveInputVector = cameraPlanarRotation * moveInputVector;

					// Jumping input
					if (inputJump && !inputJumpConsumed)
					{
						inputJumpConsumed = true;
						jumpRequested = true;
						TransitionToState(PlayerMovementState.Default);
					}

					if (inputCrouch)
					{
						TransitionToState(PlayerMovementState.Sliding);
					}

					break;
				}
			case PlayerMovementState.Sliding:
				{
					// Move and look inputs
					this.moveInputVector = cameraPlanarRotation * moveInputVector;

					// Jumping input
					if (inputJump && !inputJumpConsumed && Motor.GroundingStatus.FoundAnyGround)
					{
						inputJumpConsumed = true;
						jumpRequested = true;
					}

					break;
				}
		}
	}

	/// <summary>
	/// (Called by KinematicCharacterMotor during its update cycle)
	/// This is called before the character begins its movement update
	/// </summary>
	public override void BeforeCharacterUpdate(float deltaTime)
	{
		SetInputs();

		switch (CurrentCharacterState)
		{
			case PlayerMovementState.Sprinting:
				{
					if (!LocalMovementIsForwardFacing)
					{
						TransitionToState(PlayerMovementState.Default);
					}

					break;
				}
		}

		if (crouchTime < CameraMovementSharpness)
			crouchTime += deltaTime;

		movementCameraHolder.localPosition = Vector3.Lerp(movementCameraHolder.localPosition, new Vector3(0, movementAffectedCameraTargetHeight, 0), 1 - Mathf.Exp(-CameraMovementSharpness * deltaTime));
	}

	/// <summary>
	/// (Called by KinematicCharacterMotor during its update cycle)
	/// This is where you tell your character what its rotation should be right now. 
	/// This is the ONLY place where you should set the character's rotation
	/// </summary>
	public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
		switch (CurrentCharacterState)
		{
			case PlayerMovementState.Default:
				{
					if (OrientTowardsGravity)
					{
						// Rotate from current up to invert gravity
						currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), -Gravity) * currentRotation;
					}
					break;
				}
		}
	}

	/// <summary>
	/// (Called by KinematicCharacterMotor during its update cycle)
	/// This is where you tell your character what its velocity should be right now. 
	/// This is the ONLY place where you can set the character's velocity
	/// </summary>
	public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{
		switch (CurrentCharacterState)
		{
			case PlayerMovementState.Default:
				{
					Vector3 targetMovementVelocity = Vector3.zero;

					// Ground movement
					if (Motor.GroundingStatus.IsStableOnGround)
					{
						Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
						if (currentVelocity.sqrMagnitude > 0f && Motor.GroundingStatus.SnappingPrevented)
						{
							// Take the normal from where we're coming from
							Vector3 groundPointToCharacter = Motor.TransientPosition - Motor.GroundingStatus.GroundPoint;
							if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
							{
								effectiveGroundNormal = Motor.GroundingStatus.OuterGroundNormal;
							}
							else
							{
								effectiveGroundNormal = Motor.GroundingStatus.InnerGroundNormal;
							}
						}

						// Reorient velocity on slope
						currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocity.magnitude;

						// Calculate target velocity
						Vector3 inputRight = Vector3.Cross(moveInputVector, Motor.CharacterUp);
						Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * moveInputVector.magnitude;

						if (isCrouching)
							targetMovementVelocity = reorientedInput * CrouchSpeed;
						else
							targetMovementVelocity = reorientedInput * RunSpeed;

						// Smooth movement Velocity
						currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
					}
					// Air movement
					else
					{
						currentVelocity += moveInputVector * AirAccelerationSpeed * deltaTime;

						if (Motor.GroundingStatus.FoundAnyGround)
						{
							Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

							//Reorient direction to slope
							currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocity.magnitude;
						}

						// Gravity
						currentVelocity += Gravity * deltaTime;

						// Drag
						currentVelocity *= (1f / (1f + (Drag * deltaTime)));
					}

					// Handle jumping
					jumpedThisFrame = false;

					if (jumpRequested)
					{
						//Debug.Log($"Trying to Jump");

						// See if we actually are allowed to jump
						if (!jumpConsumed && Motor.GroundingStatus.FoundAnyGround)
						{
							// Calculate jump direction before ungrounding
							Vector3 jumpDirection = Motor.CharacterUp;
							if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
							{
								jumpDirection = Motor.GroundingStatus.GroundNormal;
							}

							// Makes the character skip ground probing/snapping on its next update. 
							Motor.ForceUnground();

							// Add to the return velocity and reset jump state
							currentVelocity += (jumpDirection * CalculateJumpSpeed(JumpHeight)) - Vector3.Project(currentVelocity, Motor.CharacterUp);
							jumpRequested = false;
							jumpConsumed = true;
							jumpedThisFrame = true;
						}
					}
					else if (wallJumpRequested)
					{
						//Debug.Log($"Requested Wall Jump");

						if (!wallJumpConsumed && !Motor.GroundingStatus.FoundAnyGround && moveInputVector.magnitude > 0)
						{
							//Debug.Log($"Trying to Wall Jump. Sensor count: {wallJumpRaySensors.Length}");

							var foundWall = false;
							var wallDistance = float.MaxValue;
							var wallHitNormal = Vector3.zero;
							var wallHitPoint = Vector3.zero;
							var rayDirection = Vector3.zero;

							foreach (var sensor in wallJumpRaySensors)
							{
								sensor.Pulse();
								var wall = sensor.GetNearest();

								if (wall != null)
								{
									//Debug.Log($"Found a wall!");
									
									foundWall = true;
									var ray = sensor.GetRayHit(wall);
									if (ray.distance < wallDistance)
									{
										wallDistance = ray.distance;
										wallHitNormal = ray.normal;
										rayDirection = sensor.Direction;
									}
								}
							}

							if (foundWall)
							{
								var wallAngle = Vector3.Angle(wallHitNormal, Motor.CharacterUp);

								// See if we actually are allowed to jump
								if (MathfExtensions.Between(WallJumpMinWallAngle, wallAngle, WallJumpMaxWallAngle) 
									&& Vector3.Angle(-wallHitNormal, moveInputVector) >= WallJumpMaxJumpAngleFromNormal)
								{
									Vector3 jumpDirection = new Vector3(moveInputVector.x, Mathf.Abs(rayDirection.y), moveInputVector.z).normalized;

									// Add to the return velocity and reset jump state
									currentVelocity = (jumpDirection * CalculateJumpSpeed(WallJumpHeight));
									wallJumpRequested = false;
									wallJumpConsumed = true;
									timeWallJumped = Time.time;
								}
							}
						}
					}

					// Take into account additive velocity
					if (internalVelocityAdd.sqrMagnitude > 0f)
					{
						currentVelocity += internalVelocityAdd;
						internalVelocityAdd = Vector3.zero;
					}

					break;
				}
			case PlayerMovementState.Sprinting:
				{
					Vector3 targetMovementVelocity = Vector3.zero;

					Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
					if (currentVelocity.sqrMagnitude > 0f && Motor.GroundingStatus.SnappingPrevented)
					{
						// Take the normal from where we're coming from
						Vector3 groundPointToCharacter = Motor.TransientPosition - Motor.GroundingStatus.GroundPoint;
						if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
						{
							effectiveGroundNormal = Motor.GroundingStatus.OuterGroundNormal;
						}
						else
						{
							effectiveGroundNormal = Motor.GroundingStatus.InnerGroundNormal;
						}
					}

					// Reorient velocity on slope
					currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocity.magnitude;

					// Calculate target velocity
					Vector3 inputRight = Vector3.Cross(moveInputVector, Motor.CharacterUp);
					Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * moveInputVector.magnitude;

					targetMovementVelocity = reorientedInput * RunSpeed * SprintSpeedMultiplier;

					// Smooth movement Velocity
					currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));

					break;
				}
			case PlayerMovementState.Sliding:
				{
					if (!Motor.GroundingStatus.IsStableOnGround)
					{
						if (!Motor.GroundingStatus.FoundAnyGround)
						{
							currentVelocity += Gravity * deltaTime;
						}
						else
						{
							Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

							//Reorient direction to slope
							currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocity.magnitude;

							if (inputMove.magnitude > 0 && inputMove.y >= 0)
							{
								// Calculate target velocity
								Vector3 inputRight = Vector3.Cross(moveInputVector, Motor.CharacterUp);
								Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized;

								currentVelocity = Vector3.RotateTowards(currentVelocity, reorientedInput * currentVelocity.magnitude, Mathf.Deg2Rad * SlideRotationDegreesPerSecondUnstable * deltaTime, 0);
							}

							// Handle jumping
							jumpedThisFrame = false;

							if (jumpRequested)
							{
								// See if we actually are allowed to jump
								if (!jumpConsumed && Motor.GroundingStatus.FoundAnyGround)
								{
									// Calculate jump direction before ungrounding
									Vector3 jumpDirection = Motor.CharacterUp;
									if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
									{
										jumpDirection = Motor.GroundingStatus.GroundNormal;
									}

									// Makes the character skip ground probing/snapping on its next update. 
									Motor.ForceUnground();

									// Add to the return velocity and reset jump state
									currentVelocity += (jumpDirection * CalculateJumpSpeed(JumpHeight)) - Vector3.Project(currentVelocity, Motor.CharacterUp);
									jumpRequested = false;
									jumpConsumed = true;
									jumpedThisFrame = true;
								}
							}

							var gravityAngle = Vector3.Angle(Gravity, effectiveGroundNormal);
							var gravityComponent = Gravity * Mathf.Sin(Mathf.Deg2Rad * gravityAngle) * SlideGravityAccelerationFactor;
							var newGravityComponentY = (currentVelocity.y < 0) ? gravityComponent.y : gravityComponent.y * SlideGravityUpwardMotionMultiplier;
							gravityComponent = new Vector3(gravityComponent.x, newGravityComponentY, gravityComponent.z);
							currentVelocity += (gravityComponent * deltaTime);
						}

						// Drag
						currentVelocity *= (1f / (1f + (Drag * deltaTime)));
					}
					else
					{
						Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

						//Reorient direction to slope
						currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocity.magnitude;

						// When starting to slide, set velocity to max
						if (startedSlide)
						{
							// Calculate target velocity
							Vector3 inputRight = Vector3.Cross(slideDirection, Motor.CharacterUp);
							Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * slideDirection.magnitude;

							if (slideRecovering)
							{
								currentVelocity = (currentVelocity * SlideRecoverySpeedMultiplier);
							}
							else
							{
								currentVelocity = (reorientedInput * currentVelocity.magnitude * SlideSpeedMultiplier);
							}
							startedSlide = false;
							slideRecovering = false;
						}

						if (inputMove.magnitude > 0 && inputMove.y >= 0)
						{
							// Calculate target velocity
							Vector3 inputRight = Vector3.Cross(moveInputVector, Motor.CharacterUp);
							Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized;

							currentVelocity = Vector3.RotateTowards(currentVelocity, reorientedInput * currentVelocity.magnitude, Mathf.Deg2Rad * SlideRotationDegreesPerSecondStable * deltaTime, 0);
						}

						// Handle jumping
						jumpedThisFrame = false;

						if (jumpRequested)
						{
							// See if we actually are allowed to jump
							if (!jumpConsumed && Motor.GroundingStatus.FoundAnyGround)
							{
								// Calculate jump direction before ungrounding
								Vector3 jumpDirection = Motor.CharacterUp;
								if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
								{
									jumpDirection = Motor.GroundingStatus.GroundNormal;
								}

								// Makes the character skip ground probing/snapping on its next update. 
								Motor.ForceUnground();

								// Add to the return velocity and reset jump state
								currentVelocity += (jumpDirection * CalculateJumpSpeed(JumpHeight)) - Vector3.Project(currentVelocity, Motor.CharacterUp);
								jumpRequested = false;
								jumpConsumed = true;
								jumpedThisFrame = true;
							}
						}

						var gravityAngle = Vector3.Angle(Gravity, effectiveGroundNormal);
						var gravityComponent = Gravity * Mathf.Sin(Mathf.Deg2Rad * gravityAngle) * SlideGravityAccelerationFactor;
						var newGravityComponentY = (currentVelocity.y < 0) ? gravityComponent.y : gravityComponent.y * SlideGravityUpwardMotionMultiplier;
						gravityComponent = new Vector3(gravityComponent.x, newGravityComponentY, gravityComponent.z);
						currentVelocity += (gravityComponent * deltaTime);

						// Drag
						currentVelocity *= (1f / (1f + (SlideDrag * deltaTime)));
					}

					break;
				}
		}
	}

	/// <summary>
	/// (Called by KinematicCharacterMotor during its update cycle)
	/// This is called after the character has finished its movement update
	/// </summary>
	public override void AfterCharacterUpdate(float deltaTime)
	{
		switch (CurrentCharacterState)
		{
			case PlayerMovementState.Default:
				{
					// Handle jumping pre-ground grace period
					if (jumpRequested && TimeSinceGrounded >= JumpCooldownTime)
					{
						jumpRequested = false;
					}

					wallJumpRequested = false;
					// Handle wall jumping pre-ground grace period
					if (wallJumpConsumed && TimeSinceWallJump >= WallJumpCooldownTime)
					{
						wallJumpConsumed = false;
					}

					if (Motor.GroundingStatus.FoundAnyGround)
					{
						// If we're on a ground surface, reset jumping values
						if (!jumpedThisFrame)
						{
							jumpConsumed = false;
						}
						else if (jumpedThisFrame)
						{
							if (shouldBeCrouching)
								HandleCrouching(false);
						}
					}

					if (!inputCrouch && shouldBeCrouching)
						HandleCrouching(false);

					// Handle uncrouching
					HandleUncrouching();

					break;
				}
			case PlayerMovementState.Sprinting:
				{
					if (Motor.Velocity.magnitude <= SprintVelocityThreshold)
					{
						TransitionToState(PlayerMovementState.Default);
					}
					break;
				}
			case PlayerMovementState.Sliding:
				{
					// Handle jumping pre-ground grace period
					if (jumpRequested && TimeSinceGrounded >= JumpCooldownTime)
					{
						jumpRequested = false;
					}

					if (Motor.GroundingStatus.FoundAnyGround)
					{
						// If we're on a ground surface, reset jumping values
						if (!jumpedThisFrame)
						{
							jumpConsumed = false;
						}
						else if (jumpedThisFrame)
						{
							if (shouldBeCrouching)
								HandleCrouching(false);
						}
					}

					if (!slideUnstable)
					{
						if (TimeSinceEnteringState <= SlideSlopeGraceTime)
						{
							Motor.MaxStableSlopeAngle = SlideSlopeGraceAngle;
						}
						else
						{
							slideUnstable = true;
							Motor.MaxStableSlopeAngle = SlideSlopeUnstableAngle;
						}
					}

					if (TimeSinceEnteringState >= SlideMinimumDuration &&
						SlideIsPostGraceThreshold ?
						Motor.Velocity.magnitude <= SlideVelocityThresholdPostGrace :
						Motor.Velocity.magnitude <= SlideVelocityThresholdPreGrace)
					{
						TransitionToState(PlayerMovementState.Default);
					}
					break;
				}
		}
	}

	public override void PostGroundingUpdate(float deltaTime)
	{
		// Handle landing and leaving ground
		if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
		{
			OnLanded();
		}
		else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
		{
			OnLeaveStableGround();
		}
	}

	public override bool IsColliderValidForCollisions(Collider coll)
	{
		if (IgnoredColliders.Count >= 0)
		{
			return true;
		}

		if (IgnoredColliders.Contains(coll))
		{
			return false;
		}
		return true;
	}

	public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		switch (CurrentCharacterState)
		{
			case PlayerMovementState.Default:
				{
					if ((IsCrouching || inputCrouch) && Motor.Velocity.magnitude >= CrouchingSlideOnLandVelocityThreshold)
					{
						slideRecovering = true;
						TransitionToState(PlayerMovementState.Sliding);
						//Debug.Log($"Landed into Slide Recovery from crouch");
					}
					else if (!IsCrouching && Motor.Velocity.magnitude >= StandingSlideOnLandVelocityThreshold)
					{
						slideRecovering = true;
						TransitionToState(PlayerMovementState.Sliding);
						//Debug.Log($"Landed into Slide Recovery from standing");
					}

					break;
				}
		}
	}

	public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		switch (CurrentCharacterState)
		{
			case PlayerMovementState.Sliding:
				{
					if (Motor.GroundingStatus.IsStableOnGround && Vector3.Angle(hitNormal, Motor.CharacterUp) > Motor.MaxStableSlopeAngle)
					{
						Motor.ForceUnground();
					}

					break;
				}
		}
	}

	public void AddVelocity(Vector3 velocity)
	{
		switch (CurrentCharacterState)
		{
			case PlayerMovementState.Default:
				{
					internalVelocityAdd += velocity;
					break;
				}
		}
	}

	public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
	}

	protected void OnLanded()
	{
		switch (CurrentCharacterState)
		{
			case PlayerMovementState.Default:
				{
					if ((IsCrouching || inputCrouch) && Motor.Velocity.magnitude >= CrouchingSlideOnLandVelocityThreshold)
					{
						slideRecovering = true;
						TransitionToState(PlayerMovementState.Sliding);
						//Debug.Log($"Landed into Slide Recovery from crouch");
					}
					else if (!IsCrouching && Motor.Velocity.magnitude >= StandingSlideOnLandVelocityThreshold)
					{
						slideRecovering = true;
						TransitionToState(PlayerMovementState.Sliding);
						//Debug.Log($"Landed into Slide Recovery from standing");
					}

					break;
				}
		}
	}

	protected void OnLeaveStableGround()
	{
		timeLeftStableGround = Time.time;

		switch (CurrentCharacterState)
		{
			case PlayerMovementState.Sprinting:
				{
					TransitionToState(PlayerMovementState.Default);
					break;
				}
		}
	}

	private bool HandleCrouching(bool setCrouching)
	{
		//Debug.Log($"SetCrouching is {setCrouching}");
		if (setCrouching)
		{
			shouldBeCrouching = true;

			if (!isCrouching)
			{
				isCrouching = true;
				SetCrouchingDimensions();
				crouchTime = 0;
				movementAffectedCameraTargetHeight = CrouchingCameraHeight;
				playerAnimationManager.SetCrouch(true);
			}

			return true;
		}
		else
		{
			shouldBeCrouching = false;
			return false;
		}

	}

	private void HandleUncrouching()
	{
		if (isCrouching && !shouldBeCrouching)
		{
			// Do an overlap test with the character's standing height to see if there are any obstructions
			SetStandingDimensions();
			if (Motor.CharacterOverlap(
				Motor.TransientPosition,
				Motor.TransientRotation,
				_probedColliders,
				Motor.CollidableLayers,
				QueryTriggerInteraction.Ignore) > 0)
			{
				// If obstructions, just stick to crouching dimensions
				SetCrouchingDimensions();
			}
			else
			{
				// If no obstructions, uncrouch
				isCrouching = false;
				crouchTime = 0;
				movementAffectedCameraTargetHeight = StandingCameraHeight;
				playerAnimationManager.SetCrouch(false);
			}
		}
	}

	#region MovementCalculationFunctions
	// Calculate camera direction and rotation on the character plane
	private Quaternion CalculatePlanarRotation(Quaternion inputCameraRotation)
	{
		Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputCameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
		if (cameraPlanarDirection.sqrMagnitude == 0f)
		{
			cameraPlanarDirection = Vector3.ProjectOnPlane(inputCameraRotation * Vector3.up, Motor.CharacterUp).normalized;
		}
		Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);
		return cameraPlanarRotation;
	}

	private float CalculateJumpSpeed(float jumpHeight)
	{
		return Mathf.Sqrt(2 * jumpHeight * Gravity.magnitude);
	}
	#endregion

	private void SetCrouchingDimensions()
	{
		Motor.SetCapsuleDimensions(0.3f, 0.9f, 0.45f);
	}

	private void SetStandingDimensions()
	{
		Motor.SetCapsuleDimensions(0.3f, 1.8f, 0.9f);
	}

	//private void OnDrawGizmos()
	//{
	//	Gizmos.color = Color.red;
	//	Gizmos.DrawWireSphere(_targetClimbUpPosition, 0.1f);
	//}

	private void OnEnable()
	{
		playerInputActions.Enable();
	}

	private void OnDisable()
	{
		playerInputActions.Disable();
	}
}
