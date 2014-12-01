using UnityEngine;
using System.Collections;

public class FrameRateManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Awake() {
		// Turn off vsync to allow our forced framerate to take effect
		QualitySettings.vSyncCount = 0;
		// Setting this to 60 does a best effort force for the framerate at 60 fps
		Application.targetFrameRate = 60;
	}
}
