using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	public int myPlayerId = 1;

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
	
	}
}
