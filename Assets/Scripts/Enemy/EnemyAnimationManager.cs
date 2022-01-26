using UnityEngine;

public class EnemyAnimationManager : MonoBehaviour
{
	private Animator enemySpriteAnimator;

	public Animator EnemySpriteAnimator
	{
		get
		{
			if (enemySpriteAnimator == null)
				enemySpriteAnimator = gameObject.GetComponentInChildren<Animator>();
			return enemySpriteAnimator;
		}
	}

	private AnimatedBillboardSpriteManager animatedBillboardSpriteManager;

	public AnimatedBillboardSpriteManager AnimatedBillboardSpriteManager
	{
		get
		{
			if (animatedBillboardSpriteManager == null)
				animatedBillboardSpriteManager = gameObject.GetComponentInChildren<AnimatedBillboardSpriteManager>();
			return animatedBillboardSpriteManager;
		}
	}

	internal void ResetAnimatorParameters()
	{
		SetAnimationBool(AnimationCodes.Attacking, false);
		SetAnimationBool(AnimationCodes.Attacking, false);
		ResetAnimationSpeed();
	}

	internal void ResetAnimationSpeed()
	{
		if (EnemySpriteAnimator != null)
			EnemySpriteAnimator.speed = 1;
	}

	internal void ChangeAnimationSpeed(float multiplier)
	{
		EnemySpriteAnimator.speed = multiplier;
	}

	#region Setters
	internal void SetMovement(float value)
	{
		SetAnimationFloat(AnimationCodes.MovementInput, value);
	}

	internal void SetAttacking(bool value)
	{
		SetAnimationBool(AnimationCodes.Attacking, value);
	}

	internal void SetStaggered(bool value)
	{
		SetAnimationBool(AnimationCodes.Staggered, value);
	}

	internal void SetDead(bool value)
	{
		SetAnimationBool(AnimationCodes.Dead, value);
	}
	#endregion

	#region HelperMethods
	private void SetAnimationBool(string name, bool value)
	{
		EnemySpriteAnimator?.SetBool(name, value);
	}

	private void SetAnimationInteger(string name, int value)
	{
		EnemySpriteAnimator?.SetInteger(name, value);
	}

	private void SetAnimationFloat(string name, float value)
	{
		EnemySpriteAnimator?.SetFloat(name, value);
	}

	private void SetAnimationTrigger(string name)
	{
		EnemySpriteAnimator?.SetTrigger(name);
	}
	#endregion

	private static class AnimationCodes
	{
		public const string MovementInput = "MovementInput";
		public const string Attacking = "Attacking";
		public const string Staggered = "Staggered";
		public const string Dead = "Dead";
	}
}
