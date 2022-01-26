using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRoomTransitionManager : MonoBehaviour
{
	private PlayerSpawnManager playerSpawnManager;

	private void Start()
	{
		playerSpawnManager = GlobalReferences.GameManager.PlayerSpawnManager;

		SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
		SceneManager.sceneLoaded += UpdateActiveSceneOnLoad;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= UpdateActiveSceneOnLoad;
	}

	public void TransitionToLevel(RoomTransitionID destination)
	{
		var sceneName = destination.ToString().Split('_')[0];
		playerSpawnManager.SetRoomSpawnID(destination);
		//TODO: On game start, set the active scene to the level we are playing.
		SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
		SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
	}

	private void UpdateActiveSceneOnLoad(Scene targetScene, LoadSceneMode mode)
	{
		SceneManager.SetActiveScene(targetScene);
	}
}

public enum RoomTransitionID
{
	//SceneName_UsefulName
	ErrorRoom_NotSet = Int32.MaxValue,
	SafeHouse_NewGame = 0,
	SafeHouse_Entrance = 1,
	FiringRange_Entrance = 2,
}
