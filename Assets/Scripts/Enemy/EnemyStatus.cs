using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
	[ReadOnly]
	[SerializeField]
	private float currentHealth;
	[SerializeField]
	public float MaxHealth;
	public float CurrentHealth { get { return currentHealth; } }

	private EnemyAI enemyAI;

	private void Awake()
	{
		currentHealth = MaxHealth;
	}

	private void Start()
	{
		enemyAI = GetComponent<EnemyAI>();
	}

	#region HealthState
	internal void TakeDamage(float damage)
	{
		currentHealth -= damage;
		enemyAI.wasAttacked = true;

		if (currentHealth <= 0)
			enemyAI.TransitionToState(EnemyAIState.Dead);
	}
	#endregion
}
