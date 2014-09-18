using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	private float minOrthoSize = 5f;
	private float maxOrthoSize = 15.0f;
	
	private Camera mainCam;
	
	// Use this for initialization
	void Start () { 
		mainCam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {  
		// Update zoom based on scrollwheel
		mainCam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
		mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, minOrthoSize, maxOrthoSize);
	}
}
