using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
	public AudioClip footstepAudioClip;
	public AudioClip jumpAudioClip;
	public AudioClip landLightAudioClip;
	public AudioClip landHeavyAudioClip;
	public AudioClip hurtLightAudioClip;
	public AudioClip hurtHeavyClip;
	public AudioClip dieClip;

	public AudioSource PlayerAudioSource
	{
		get
		{
			if (playerAudioSource == null)
				playerAudioSource = GetComponent<AudioSource>();
			return playerAudioSource;
		}
	}
	private AudioSource playerAudioSource;

	//public void PlayBasicAttackHitSound()
	//{
	//var randomIndex = Random.Range(0, basicAttackHitSounds.Length);
	//playerAudioSource.PlayOneShot(basicAttackHitSounds[randomIndex]);
	//}
}
