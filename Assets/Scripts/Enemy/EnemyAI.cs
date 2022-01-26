using SensorToolkit;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyStatus))]
[RequireComponent(typeof(EnemyMovementController))]
public abstract class EnemyAI : MonoBehaviour
{
	[ReadOnly] public bool isAlerted;
	[ReadOnly] public bool wasAttacked;
	[ReadOnly]
	[SerializeField]
	private EnemyAIState currentEnemyState;

	[ReadOnly]
	[SerializeField]
	private float distanceFromTarget;
	public float DistanceFromTarget { get { return distanceFromTarget; } private set { distanceFromTarget = value; } }

	[ReadOnly]
	[SerializeField]
	private Vector3 destination;
	public Vector3 Destination { get { return destination; } set { destination = value; } } //Change this for where the character wants to move

	public float AttackRange = 1.2f;
	public float AttackDamage = 10f;
	public float AttackTime = 0.5f;

	private NavMeshPath _path;
	private Vector3[] _pathCorners = new Vector3[16];
	private Vector3 _lastValidDestination;

	public TriggerSensor EyeFieldOfViewSensor;
	public RangeSensor EarRangeSensor;
	public EnemyAIState PreviousEnemyState { get; private set; }
	public EnemyAIState CurrentEnemyState { get { return currentEnemyState; } private set { currentEnemyState = value; } }
	private float TimeEnteredState;

	public float TimeSinceEnteringState { get { return Time.time - TimeEnteredState; } }

	public GameObject Player { get; private set; }
	public Transform EyesAndEars { get; private set; }
	public EnemyMovementController EnemyController { get; private set; }
	public EnemyAnimationManager AnimationManager { get; private set; }
	public EnemyStatus Status { get; private set; }

	public Vector3 playerLastDetectedPosition { get; private set; }

	public virtual void Awake()
	{
		AnimationManager = GetComponent<EnemyAnimationManager>();
		Status = gameObject.GetComponent<EnemyStatus>();
		EnemyController = GetComponent<EnemyMovementController>();
		EyesAndEars = transform.Find("EyesAndEars");
		_path = new NavMeshPath();
	}

	public virtual void Start()
	{
		Player = GameObject.FindGameObjectWithTag(Helpers.Tags.Player);

		Destination = transform.position;
	}

	public void TransitionToState(EnemyAIState newState)
	{
		if (CurrentEnemyState == EnemyAIState.Dead)
			return;

		PreviousEnemyState = CurrentEnemyState;
		OnStateExit(PreviousEnemyState, newState);
		CurrentEnemyState = newState;
		TimeEnteredState = Time.time;
		OnStateEnter(newState, PreviousEnemyState);
	}

	public abstract void OnStateEnter(EnemyAIState state, EnemyAIState fromState);

	public abstract void OnStateExit(EnemyAIState state, EnemyAIState toState);

	public virtual void Update()
	{
		if (Status.CurrentHealth <= 0)
			TransitionToState(EnemyAIState.Dead);

		if (CurrentEnemyState != EnemyAIState.Dead)
		{
			var enemyInputs = HandleCharacterNavigation();
			SetControllerInputs(ref enemyInputs);

			DistanceFromTarget = (Destination - transform.position).magnitude;
			//Add Update State logic to children
		}
	}

	public void SetControllerInputs(ref EnemyMovementInputs enemyInputs)
	{
		EnemyController.SetInputs(ref enemyInputs);
	}

	public abstract void Attack(float damage);

	public virtual void ReceiveAttack(float damage)
	{
		Status.TakeDamage(damage);
	}

	//public IAttackable RaycastForMeleeAttackTarget(out bool hasEnemy)
	//{
	//	var raycastStartPosition = new Vector3(transform.position.x,
	//		transform.position.y + EnemyController.Motor.Capsule.height / 2,
	//		transform.position.z);

	//	int layerMask = LayerMask.GetMask(Helpers.Layers.Player, Helpers.Layers.Enemy);
	//	Physics.Raycast(raycastStartPosition, EnemyController.Motor.CharacterForward * AttackRange, out RaycastHit hit, AttackRange, layerMask);
	//	Debug.DrawRay(raycastStartPosition, EnemyController.Motor.CharacterForward * AttackRange, Color.blue, 0.5f);

	//	if (hit.transform.tag == Helpers.Tags.Player || hit.transform.tag == Helpers.Tags.Enemy)
	//	{
	//		hasEnemy = true;
	//		var attackableComponent = hit.transform.gameObject.GetAttackableComponent();
	//		return attackableComponent;
	//	}
	//	else
	//	{
	//		hasEnemy = false;
	//		return null;
	//	}
	//}

	public void TravelToPosition(Vector3 targetPosition)
	{
		Destination = targetPosition;
	}

	public struct EnemyMovementInputs
	{
		public Vector3 MoveVector;
		public Quaternion TargetLookRotation;
	}

	private EnemyMovementInputs HandleCharacterNavigation()
	{
		if (NavMesh.CalculatePath(EnemyController.transform.position, Destination, NavMesh.AllAreas, _path))
		{
			_lastValidDestination = Destination;
		}
		else
		{
			NavMesh.CalculatePath(EnemyController.transform.position, _lastValidDestination, NavMesh.AllAreas, _path);
		}

		EnemyMovementInputs enemyInputs = new EnemyMovementInputs();
		GetSensorInput();

		switch (CurrentEnemyState)
		{
			case EnemyAIState.Attacking:
			case EnemyAIState.Dead:
				{
					enemyInputs.MoveVector = Vector3.zero;
					break;
				}
			default:
				{
					int cornersCount = _path.GetCornersNonAlloc(_pathCorners);
					if (cornersCount > 1)
					{
						enemyInputs.MoveVector = (_pathCorners[1] - EnemyController.transform.position).normalized;
					}
					else
					{
						enemyInputs.MoveVector = Vector3.zero;
					}

					break;
				}
		}


		return enemyInputs;
	}

	private void GetSensorInput()
	{
		//var entitiesSpotted = EyeFieldOfViewSensor.GetDetectedByTag(Helpers.Tags.Enemy);
		//var playerSpotted = EyeFieldOfViewSensor.GetDetectedByTag(Helpers.Tags.Player);
		//entitiesSpotted.AddRange(playerSpotted);
		//HandlePlayerSpotted(playerSpotted);

		if (EyeFieldOfViewSensor.IsDetected(Player))
		{
			playerLastDetectedPosition = Player.transform.position;
			HandlePlayerSpotted();
		}
		else if (EarRangeSensor.IsDetected(Player))
		{
			playerLastDetectedPosition = Player.transform.position;
			HandlePlayerHeard();
		}

		//var entitiesHeard= EarRangeSensor.GetDetectedByTag(Helpers.Tags.Enemy);
		//var playerHeard = EarRangeSensor.GetDetectedByTag(Helpers.Tags.Player);
		//entitiesHeard.AddRange(playerHeard);
		//HandlePlayerHeard(playerHeard);
	}

	internal abstract void HandlePlayerSpotted();
	internal abstract void HandlePlayerHeard();
}

public enum EnemyAIState
{
	Idle,
	Chasing,
	Fleeing,
	Attacking,
	Dead,
}