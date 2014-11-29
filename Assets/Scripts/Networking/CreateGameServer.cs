using UnityEngine;
using System.Collections;

public class CreateGameServer : MonoBehaviour {
	public static string gameName = "Enter a Game Name"; //text to initially appear in the box
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void OnGUI () {
		gameName = GUI.TextField(new Rect (Screen.width/2-100,Screen.height-200,200,25), gameName, 50);
	}
}
