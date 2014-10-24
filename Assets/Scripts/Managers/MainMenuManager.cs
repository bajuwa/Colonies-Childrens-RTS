using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
	public Texture2D singlePlayerButton;
	public Texture2D multiPlayerButton;
	private float x;
	private float y;
	private Vector2 resolution;
	// Use this for initialization
	void Start () {
		resolution = new Vector2(Screen.width, Screen.height);
		x=Screen.width/1024.0f; //x value of the working resolution
		y=Screen.height/768.0f; //y value of the working resolution
	}
	
	// Update is called once per frame
	void Update() {
		//Updates the Object in the GUI to reflect the new resolutoin
		if (Screen.width!=resolution.x || Screen.height!=resolution.y)
		{
			resolution = new Vector2(Screen.width, Screen.height);
			x=resolution.x/1024.0f;
			y=resolution.y/768.0f;
		}
	}
	void OnGUI () {
		
		GUI.Label(new Rect(400*x, 150*y, singlePlayerButton.width/4*x, singlePlayerButton.height/4*y), "SinglePlayer");
		GUI.Label(new Rect(400*x, 350*y, multiPlayerButton.width/4*x, multiPlayerButton.height/4*y), "MultiPlayer");
		if (GUI.Button(new Rect(400*x,200*y, singlePlayerButton.width/4*x, singlePlayerButton.height/4*y), singlePlayerButton)) {
			Application.LoadLevel("Game");
			}
		if (GUI.Button(new Rect(400*x,400*y, multiPlayerButton.width/4*x, multiPlayerButton.height/4*y), multiPlayerButton)) {
			Debug.Log("Clicked MultiPlayer");
			}
		
	}
}
