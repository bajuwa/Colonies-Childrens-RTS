using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	//MapUIManager object
	public MapUIManager mUM;
	//Empty GUITexture that will display what is selected in the bottom left
	public GUITexture headDisplay;
	// Use this for initialization
	void Start () {
		//Set it to null on runtime because the default is
		//the unity logo.
		headDisplay.texture = null;
	}
	
	// Update is called once per frame
	void Update () {
		//assign the texture a new texture based on what is selected and display it in the bottom left
		headDisplay.texture = mUM.getCurrentlySelectedObject().displayImage;
		
	}
}
