using UnityEngine;
using System.Collections;

public class ModifySceneSelection : MonoBehaviour {

	public int modifyBy;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseOver() {
		// If you want any hover effects, put them here!
	
		// If the user presses the button, load the next scene that is configured with this object
		if (Input.GetMouseButtonDown(0)) { 
			GameObject.Find("GameSetupManager").GetComponent<GameSetupManager>().changeMap(modifyBy);
		}
	}
}
