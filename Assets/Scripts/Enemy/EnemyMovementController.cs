using KinematicCharacterController;
using System.Collections.Generic;
using UnityEngine;

//TODO: Refactor Player and General AI Controller into a single class
public class EnemyMovementController : BaseCharacterController
{
	public enum EnemyMovementState
	{
		Default,
	}

	[Header("Stable Movement")]
	public float MaxStableRunSpeed = 5f;
	public float StableMovementSharpness = 12;
	public float OrientationSharpness = 10;

	[Header("Air Movement")]
	public float MaxAirMoveSpeed = 5f;
	public float AirAccelerationSpeed = 0f;
	public float Drag = 0.1f;

	[Header("Jumping")]
	public bool AllowJumpingWhenSliding = false;
	public float JumpSpeed = 6f;
	public float JumpPreGroundingGraceTime = 0f;
	public float JumpPostGroundingGraceTime = 0f;
	public float JumpRecoveryTime = 0.2f;

	[Header("Misc")]
	public List<Collider> IgnoredColliders;
	public bool OrientTowardsGravity = false;
	public Vector3 Gravity = new Vector3(0, -25f, 0);
	public Transform MeshRoot;

	public EnemyMovementState CurrentEnemyState { get; private set; }
	public EnemyMovementState PreviousEnemyState { get; private set; }
	public float TimeEnteredState { get; private set; }
	public float TimeSinceEnteringState { get { return Time.time - TimeEnteredState; } }

	private Quaternion _targetHangingRotation = Quaternion.identity;

	private Collider[] _probedColliders = new Collider[8];
	private Vector3 _moveInputVector;
	private Vector3 _lookInputVector;
	private Vector3 _internalVelocityAdd = Vector3.zero;

	private Vector3 lastInnerNormal = Vector3.zero;
	private Vector3 lastOuterNormal = Vector3.zero;

	private EnemyAI.EnemyMovementInputs _bufferedInputs;

	private EnemyAI enemyAI;
	private EnemyAnimationManager enemyAnimationManager;

	private void Start()
	{
		enemyAI = GetComponent<EnemyAI>();
		enemyAnimationManager = GetComponent<EnemyAnimationManager>();
		TransitionToState(EnemyMovementState.Default);

		if (IgnoredColliders == null)
			IgnoredColliders = new List<Collider>();
	}

	/// <summary>
	/// Handles movement state transitions and enter/exit callbacks
	/// </summary>
	public void TransitionToState(EnemyMovementState newState)
	{
		PreviousEnemyState = CurrentEnemyState;
		OnStateExit(PreviousEnemyState, newState);
		CurrentEnemyState = newState;
		TimeEnteredState = Time.time;
		OnStateEnter(newState, PreviousEnemyState);
	}

	/// <summary>
	/// Event when entering a state
	/// </summary>
	public void OnStateEnter(EnemyMovementState state, EnemyMovementState fromState)
	{
		switch (state)
		{
			case EnemyMovementState.Default:
				{
					break;
				}
		}
	}

	/// <summary>
	/// Event when exiting a state
	/// </summary>
	public void OnStateExit(EnemyMovementState state, EnemyMovementState toState)
	{
		switch (state)
		{
			case EnemyMovementState.Default:
				{
					break;
				}
		}
	}

	/// <summary>
	/// This is called every frame by PlayerController in order to tell the character what its inputs are
	/// </summary>
	public void SetInputs(ref EnemyAI.EnemyMovementInputs inputs)
	{
		_bufferedInputs = inputs;

		// Clamp input
		Vector3 moveInputVector = inputs.MoveVector.normalized;
		enemyAnimationManager.SetMovement(moveInputVector.magnitude);

		Quaternion cameraPlanarRotation = CalculatePlanarRotation(inputs.TargetLookRotation);

		// Handle state transition from input
		switch (CurrentEnemyState)
		{
			case EnemyMovementState.Default:
				{
					// Move and look inputs
					_moveInputVector = cameraPlanarRotation * moveInputVector;
					enemyAnimationManager.SetMovement(_moveInputVector.magnitude);

					if (_moveInputVector.sqrMagnitude == 0f)
					{
						_lookInputVector = Motor.CharacterForward;
					}
					else
					{
						_lookInputVector = Vector3.ProjectOnPlane(_moveInputVector, Motor.CharacterUp);
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
		switch (CurrentEnemyState)
		{
			case EnemyMovementState.Default:
				{
					break;
				}
		}
	}

	/// <summary>
	/// (Called by KinematicCharacterMotor during its update cycle)
	/// This is where you tell your character what its rotation should be right now. 
	/// This is the ONLY place where you should set the character's rotation
	/// </summary>
	public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
		switch (CurrentEnemyState)
		{
			case EnemyMovementState.Default:
				{
					if (_lookInputVector != Vector3.zero && OrientationSharpness > 0f)
					{
						// Smoothly interpolate from current to target look direction
						Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

						// Set the current rotation (which will be used by the KinematicCharacterMotor)
						currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
					}
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
		switch (CurrentEnemyState)
		{
			case EnemyMovementState.Default:
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
						Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
						Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;

						targetMovementVelocity = reorientedInput * MaxStableRunSpeed;

						// Smooth movement Velocity
						currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
					}
					// Air movement
					else
					{
						// Add move input
						if (_moveInputVector.sqrMagnitude > 0f)
						{
							targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;

							// Prevent climbing on un-stable slopes with air movement
							if (Motor.GroundingStatus.FoundAnyGround)
							{
								Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
								targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
							}

							Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
							currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
						}

						// Gravity
						currentVelocity += Gravity * deltaTime;

						// Drag
						currentVelocity *= (1f / (1f + (Drag * deltaTime)));
					}

					// Take into account additive velocity
					if (_internalVelocityAdd.sqrMagnitude > 0f)
					{
						currentVelocity += _internalVelocityAdd;
						_internalVelocityAdd = Vector3.zero;
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
		switch (CurrentEnemyState)
		{
			case EnemyMovementState.Default:
				{
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
	}

	public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
	}

	public void AddVelocity(Vector3 velocity)
	{
		switch (CurrentEnemyState)
		{
			case EnemyMovementState.Default:
				{
					_internalVelocityAdd += velocity;
					break;
				}
		}
	}

	public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
	}

	protected void OnLanded()
	{
		switch (CurrentEnemyState)
		{
			case EnemyMovementState.Default:
				{
					break;
				}
		}
	}

	protected void OnLeaveStableGround()
	{
		switch (CurrentEnemyState)
		{
			case EnemyMovementState.Default:
				{
					break;
				}
		}
	}

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

	//private void OnDrawGizmos()
	//{
	//	Gizmos.color = Color.red;
	//	Gizmos.DrawWireSphere(_targetClimbUpPosition, 0.1f);
	//}
}
