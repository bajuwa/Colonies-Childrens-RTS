using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	public float minOrthoSize = 5f;
	public float maxOrthoSize = 15.0f;
	
	private Camera mainCam;
	private GameObject mapImage;
	
	private float cameraSpeed = 0.3f;
	private float camMinX;
	private float camMaxX;
	private float camMinY;
	private float camMaxY;
	
	// Use this for initialization
	void Start () { 
		mainCam = Camera.main;
		mapImage = GameObject.FindGameObjectWithTag("MapBackground");
	}
	
	// Update is called once per frame
	void Update () {  
		// Update zoom based on scrollwheel
		mainCam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
		mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, minOrthoSize, maxOrthoSize);
		
		// Update the Maps position while cursor is against the edge of the camera view
		Vector3 tempCamPosition = Camera.main.transform.position;
		if (mouseIsAgainstCameraEdge()) {
			// Then update the map position by how far the player moved their mouse while holding down
			Vector3 tempMousePosition = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
			
			// Adjust map position away from the mouse (since we move the map, not the camera)
			// Note: Moving 'towards' a point with negative speed actually moves it away, which is what we want
			tempCamPosition = Vector3.MoveTowards(tempCamPosition, tempMousePosition, cameraSpeed);
		}
		
		// Since we will need to enforce the bounds of the camera, get the camera bounds info
		camMinX = mainCam.transform.position.x - (mainCam.orthographicSize * Screen.width / Screen.height);
		camMaxX = mainCam.transform.position.x + (mainCam.orthographicSize * Screen.width / Screen.height);
		camMinY = mainCam.transform.position.y - mainCam.orthographicSize;
		camMaxY = mainCam.transform.position.y + mainCam.orthographicSize;
		
		// Ensure the player does not pan the camera past the limits of the maps edges
		if (camMinX < mapImage.renderer.bounds.min.x) {
			tempCamPosition.x += mapImage.renderer.bounds.min.x - camMinX;
		}
		if (camMinY < mapImage.renderer.bounds.min.y) {
			tempCamPosition.y += mapImage.renderer.bounds.min.y - camMinY;
		}
		if (camMaxX > mapImage.renderer.bounds.max.x) {
			tempCamPosition.x -= camMaxX - mapImage.renderer.bounds.max.x;
		}
		if (camMaxY > mapImage.renderer.bounds.max.y) {
			tempCamPosition.y -= camMaxY - mapImage.renderer.bounds.max.y;
		}
		
		tempCamPosition.z = Camera.main.transform.position.z;
		Camera.main.transform.position = tempCamPosition;
	}
	
	private bool mouseIsAgainstCameraEdge() {
		Vector2 mousePos = Input.mousePosition;
		
		// If the mouse is in the outer 10% of the screen, camera should pan
		// Note: Apparently unity still detects the mouse outside the webapp space, so keep it bounded by the screen dimensions as well
		if ((mousePos.x > (Screen.width * 0.9f) || 
			 mousePos.x < (Screen.width * 0.1f) ||
			 mousePos.y > (Screen.height * 0.9f) ||
			 mousePos.y < (Screen.height * 0.1f))
			&&
			mousePos.x < Screen.width && mousePos.x > 0 &&
			mousePos.y < Screen.height && mousePos.y > 0) {
				return true;
		}
		
		return false;
	}
}
