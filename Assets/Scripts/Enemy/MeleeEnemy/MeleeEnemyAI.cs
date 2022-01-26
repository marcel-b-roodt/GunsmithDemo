using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : EnemyAI
{
	private float healthPercentageFleeThreshold = 0.2f;

	public override void Awake()
	{
		base.Awake();
	}

	public override void OnStateEnter(EnemyAIState state, EnemyAIState fromState)
	{
		switch (state)
		{
			case EnemyAIState.Idle:
				{
					break;
				}
			case EnemyAIState.Chasing:
				{
					TravelToPosition(playerLastDetectedPosition);
					break;
				}
			case EnemyAIState.Attacking:
				{
					//Stop moving
					//Attack
					//Go back to chasing
					///Set animation bool
					AnimationManager.AnimatedBillboardSpriteManager.MirrorLeft = false;
					AnimationManager.SetAttacking(true);
					Attack(AttackDamage);
					break;
				}
			case EnemyAIState.Fleeing:
				{
					//Calculate safe position, or just run away from the player somewhere for a few seconds, before going back
					//TravelToPosition(playerLastDetectedPosition);
					break;
				}
			case EnemyAIState.Dead:
				{
					//TODO: Enemy Animator variable adds
					AnimationManager.SetDead(true);
					break;
				}
		}
	}

	public override void OnStateExit(EnemyAIState state, EnemyAIState toState)
	{
		switch (state)
		{
			case EnemyAIState.Attacking:
				{
					AnimationManager.AnimatedBillboardSpriteManager.MirrorLeft = true;
					AnimationManager.SetAttacking(false);
					break;
				}
		}
	}

	public override void Update()
	{
		base.Update();

		switch (CurrentEnemyState)
		{
			case EnemyAIState.Attacking:
				{
					if (TimeSinceEnteringState >= AttackTime)
						TransitionToState(EnemyAIState.Chasing);

					break;
				}
		}
	}

	//Use this attack in an animation frame with a trigger
	public override void Attack(float damage)
	{
		bool hasEnemy;
		//var attackableComponent = RaycastForMeleeAttackTarget(out hasEnemy);
		//
		//if (hasEnemy)
		//{
		//	attackableComponent.ReceiveAttack(damage);
		//}
	}

	internal override void HandlePlayerSpotted()
	{
		switch (CurrentEnemyState)
		{
			case EnemyAIState.Idle:
				{
					if (Status.CurrentHealth >= Status.MaxHealth * healthPercentageFleeThreshold)
						TransitionToState(EnemyAIState.Chasing);
					else if (Status.CurrentHealth < Status.MaxHealth * healthPercentageFleeThreshold)
						TransitionToState(EnemyAIState.Fleeing);
					
					break;
				}
			case EnemyAIState.Chasing:
				{
					TravelToPosition(playerLastDetectedPosition);

					if (DistanceFromTarget < AttackRange)
						TransitionToState(EnemyAIState.Attacking);
					 
					break;
				}
		}
	}

	internal override void HandlePlayerHeard()
	{
		//TODO: Fix this
		HandlePlayerSpotted();
	}
}
