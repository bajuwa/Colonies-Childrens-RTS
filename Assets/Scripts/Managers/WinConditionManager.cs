using UnityEngine;
using System.Collections;

public class WinConditionManager : MonoBehaviour {

	private bool gameHasStarted = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Get all the anthills on the map
		Object[] anthillObjects = GameObject.FindObjectsOfType(typeof(Anthill));
		// Iterate over them and look for one that belongs to each player
		bool imAlive = false;
		bool opponentAlive = false;
		foreach (Anthill obj in anthillObjects) {
			if (obj.isNeutralOrFriendly()) imAlive = true;
			else opponentAlive = true;
		}
		
		if (imAlive && opponentAlive) gameHasStarted = true;
		if (gameHasStarted && !imAlive) endGame((int) ClickToLoadNextScene.sceneName.LOSE_RESULTS);
		else if (gameHasStarted && !opponentAlive) endGame((int) ClickToLoadNextScene.sceneName.WIN_RESULTS);
	}
	
	private void endGame(int sceneToLoad) {
		Debug.Log("Loading next scene: " + sceneToLoad);
		Application.LoadLevel(sceneToLoad);
	}
}
