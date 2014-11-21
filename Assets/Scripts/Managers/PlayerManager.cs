using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	public int myPlayerId = 1;
	private int totalUnitCount = 0;
	private int totalAnthillCount = 0;

	// Use this for initialization
	void Start () {
	
	}
	//assign whomever joins to be player 2
	void OnConnectedToServer()
	{
		myPlayerId = 2;
	}
	// Update is called once per frame
	void Update () {
		// Unless we are dead, a totalAnthillCount of 0 means we haven't properly loaded our counts
		if (totalAnthillCount == 0) {
			Object[] gameObjects = GameObject.FindObjectsOfType(typeof(Anthill)) as Object[];
			foreach (Anthill gameObj in gameObjects) {
				if (gameObj.GetComponent<Ownable>() && gameObj.GetComponent<Ownable>().isNeutralOrFriendly()) totalAnthillCount++;
			}
		}
	}
	
	public int getTotalUnitCount() {
		return totalUnitCount;
	}
	
	public int getTotalAnthillCount() {
		return totalAnthillCount;
	}
	
	public void modifyUnitCount(int add) {
		totalUnitCount += add;
	}
	
	public void modifyAnthillCount(int add) {
		totalAnthillCount += add;
	}
}
