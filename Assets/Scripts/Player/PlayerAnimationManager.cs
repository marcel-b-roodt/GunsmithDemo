using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
	public enum AnimatorType
	{
		Movement,
		UI
	}

	public Animator PlayerMovementAnimator
	{
		get
		{
			if (playerMovementAnimator == null)
				playerMovementAnimator = GameObject.FindGameObjectWithTag(Helpers.Tags.Player).GetComponent<Animator>();
			return playerMovementAnimator;
		}
	}
	private Animator playerMovementAnimator;

	public Animator PlayerUIAnimator
	{
		get
		{
			if (playerUIAnimator == null)
				playerUIAnimator = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerHUD).GetComponentInChildren<Animator>();
			return playerUIAnimator;
		}
	}
	private Animator playerUIAnimator;

	internal void ResetAnimatorParameters()
	{
		SetAnimationBool(AnimatorType.UI, AnimationCodes.Blocking, false);
		SetAnimationBool(AnimatorType.UI, AnimationCodes.BasicAttacking, false);
		SetAnimationBool(AnimatorType.UI, AnimationCodes.ChargingAttack, false);
		SetAnimationBool(AnimatorType.UI, AnimationCodes.LungeAttacking, false);
		SetAnimationBool(AnimatorType.UI, AnimationCodes.UppercutAttacking, false);
		SetAnimationBool(AnimatorType.UI, AnimationCodes.JumpingPowerAttacking, false);
		SetAnimationBool(AnimatorType.UI, AnimationCodes.JumpKicking, false);
		SetAnimationBool(AnimatorType.UI, AnimationCodes.SlideKicking, false);
		ResetAnimationSpeed();
	}

	internal void ResetAnimationSpeed()
	{
		if (PlayerUIAnimator != null)
			PlayerUIAnimator.speed = 1;
	}

	internal void ChangeAnimationSpeed(float multiplier)
	{
		PlayerUIAnimator.speed = multiplier;
	}

	#region Setters
	internal void SetMovement(float value)
	{
		SetAnimationFloat(AnimatorType.Movement, AnimationCodes.MovementInput, value);
	}

	internal void SetWalking(bool value)
	{
		SetAnimationBool(AnimatorType.Movement, AnimationCodes.Walking, value);
	}

	internal void SetCrouch(bool value)
	{
		SetAnimationBool(AnimatorType.Movement, AnimationCodes.Crouching, value);
	}

	internal void SetSlide(bool value)
	{
		SetAnimationBool(AnimatorType.Movement, AnimationCodes.Sliding, value);
	}

	internal void ChargeUpAttack()
	{
		SetAnimationBool(AnimatorType.UI, AnimationCodes.ChargingAttack, true);
	}

	internal void ExecuteBasicAttack()
	{
		SetAnimationBool(AnimatorType.UI, AnimationCodes.ChargingAttack, false);
		SetAnimationBool(AnimatorType.UI, AnimationCodes.BasicAttacking, true);
		SetAnimationInteger(AnimatorType.UI, AnimationCodes.BasicAttackIndex, UnityEngine.Random.Range(0, 2));
	}

	internal void ExecuteLungeAttack()
	{
		SetAnimationBool(AnimatorType.UI, AnimationCodes.LungeAttacking, true);
	}

	internal void ExecuteUppercutAttack()
	{
		SetAnimationBool(AnimatorType.UI, AnimationCodes.UppercutAttacking, true);
	}

	internal void ExecuteJumpingPowerAttack()
	{
		SetAnimationBool(AnimatorType.UI, AnimationCodes.JumpingPowerAttacking, true);
	}


	internal void ExecuteBlock()
	{
		SetAnimationBool(AnimatorType.UI, AnimationCodes.Blocking, true);
	}

	internal void ExecuteJumpKick()
	{
		SetAnimationBool(AnimatorType.UI, AnimationCodes.JumpKicking, true);
	}

	internal void ExecuteSlideKick()
	{
		SetAnimationBool(AnimatorType.UI, AnimationCodes.SlideKicking, true);
	}
	#endregion

	#region HelperMethods
	private void SetAnimationBool(AnimatorType animatorType, string name, bool value)
	{
		switch (animatorType)
		{
			case AnimatorType.Movement:
				PlayerMovementAnimator?.SetBool(name, value); break;
			case AnimatorType.UI:
				PlayerUIAnimator?.SetBool(name, value); break;
		}
	}

	private void SetAnimationInteger(AnimatorType animatorType, string name, int value)
	{
		switch (animatorType)
		{
			case AnimatorType.Movement:
				PlayerMovementAnimator?.SetInteger(name, value); break;
			case AnimatorType.UI:
				PlayerUIAnimator?.SetInteger(name, value); break;
		}
	}

	private void SetAnimationFloat(AnimatorType animatorType, string name, float value)
	{
		switch (animatorType)
		{
			case AnimatorType.Movement:
				PlayerMovementAnimator?.SetFloat(name, value); break;
			case AnimatorType.UI:
				PlayerUIAnimator?.SetFloat(name, value); break;
		}
	}

	private void SetAnimationTrigger(AnimatorType animatorType, string name)
	{
		switch (animatorType)
		{
			case AnimatorType.Movement:
				PlayerMovementAnimator?.SetTrigger(name); break;
			case AnimatorType.UI:
				PlayerUIAnimator?.SetTrigger(name); break;
		}
	}
	#endregion

	private static class AnimationCodes
	{
		public const string MovementInput = "MovementInput";
		public const string Crouching = "Crouching";
		public const string Walking = "Walking";
		public const string Sliding = "Sliding";
		public const string Jumping = "Jumping";
		public const string Hanging = "Hanging";
		public const string ClimbingUp = "ClimbingUp";
		public const string Vaulting = "Vaulting";
		public const string SteppingUp = "SteppingUp";

		public const string Blocking = "Blocking";
		public const string BasicAttacking = "BasicAttacking";
		public const string ChargingAttack = "ChargingAttack";
		public const string LungeAttacking = "LungeAttacking";
		public const string UppercutAttacking = "UppercutAttacking";
		public const string JumpingPowerAttacking = "JumpingPowerAttacking";
		public const string BasicAttackIndex = "BasicAttackIndex";
		public const string JumpKicking = "JumpKicking";
		public const string SlideKicking = "SlideKicking";
	}
}
