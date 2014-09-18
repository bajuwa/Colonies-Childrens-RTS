using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	private float minOrthoSize = 5f;
	private float maxOrthoSize = 15.0f;
	
	private Vector3 rightClickCoord;
	private Vector3 oldCamCoord;
	
	private Camera mainCam;
	
	// Use this for initialization
	void Start () { 
		mainCam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {  
		// Update zoom based on scrollwheel
		mainCam.orthographicSize += Input.GetAxis("Mouse ScrollWheel");
		mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, minOrthoSize, maxOrthoSize);
		
		// Update camera location when holding and dragging right mouse button
		// Note: 1 stands for 'right mouse button'
		// First keep track of where the player first clicked down
		if (Input.GetMouseButtonDown(1)) {
			rightClickCoord = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
			oldCamCoord = mainCam.transform.position;
		}
		// Then update the camera position by how far the player moved their mouse while holding down
		if (Input.GetMouseButton(1)) {
			Vector3 tempCamPosition = mainCam.transform.position;
			Vector3 tempMousePosition = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
			tempCamPosition.x = oldCamCoord.x - (tempMousePosition.x - rightClickCoord.x);
			Debug.Log(Input.mousePosition.x);
			tempCamPosition.y = oldCamCoord.y - (tempMousePosition.y - rightClickCoord.y);
			mainCam.transform.position = tempCamPosition;
		}
	}
}
