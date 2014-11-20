using UnityEngine;
using System.Collections;

public class WinConditionManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Get all the anthills on the map
		Object[] anthillObjects = GameObject.FindObjectsOfType(typeof(Anthill));
		// Iterate over them and look for one that belongs to each player
		bool playerOneAlive = false;
		bool playerTwoAlive = false;
		foreach (Anthill obj in anthillObjects) {
			if (obj.getPlayerId() == 1) playerOneAlive = true;
			else playerTwoAlive = true;
		}
		
		//if (!playerOneAlive || !playerTwoAlive) endGame();
	}
	
	private void endGame() {
		Application.LoadLevel("Results");
	}
}
