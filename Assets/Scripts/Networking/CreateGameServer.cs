using UnityEngine;
using System.Collections;

public class CreateGameServer : MonoBehaviour {
	public static string gameName = "Enter GameName"; //text to initially appear in the box
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void OnGUI () {
		gameName = GUI.TextField(new Rect (Screen.width/2,Screen.height/2,200,50), gameName, 10);
	}
}
