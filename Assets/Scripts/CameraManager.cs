using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	public float minOrthoSize = 5f;
	public float maxOrthoSize = 15.0f;
	
	private Camera mainCam;
	private GameObject mapManager;
	private GameObject mapImage;
	
	private Vector3 rightClickCoord;
	private Vector3 oldTileCoord;
	private float camMinX;
	private float camMaxX;
	private float camMinY;
	private float camMaxY;
	
	// Use this for initialization
	void Start () { 
		mainCam = Camera.main;
		mapManager = GameObject.FindGameObjectWithTag("MapManager");
		mapImage = GameObject.FindGameObjectWithTag("MapBackground");
	}
	
	// Update is called once per frame
	void Update () {  
		// Update zoom based on scrollwheel
		mainCam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
		mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, minOrthoSize, maxOrthoSize);
			
		// Update map location when holding and dragging right mouse button
		Vector3 tempMapPosition = mapManager.transform.position;
		
		// Note: 1 stands for 'right mouse button'
		// First keep track of where the player first clicked down
		if (Input.GetMouseButtonDown(1)) {
			rightClickCoord = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
			oldTileCoord = mapManager.transform.position;
		}
		
		// Update the Maps position while playing is holding down the right mouse button
		if (Input.GetMouseButton(1)) {
			// Then update the map position by how far the player moved their mouse while holding down
			Vector3 tempMousePosition = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
			
			// Adjust map position by the distance the mouse has moved
			tempMapPosition.x = oldTileCoord.x - (rightClickCoord.x - tempMousePosition.x);
			tempMapPosition.y = oldTileCoord.y - (rightClickCoord.y - tempMousePosition.y);
			mapManager.transform.position = tempMapPosition;
		}
		
		// Since we will need to enforce the bounds of the camera, get the camera bounds info
		camMinX = mainCam.transform.position.x - (mainCam.orthographicSize * Screen.width / Screen.height);
		camMaxX = mainCam.transform.position.x + (mainCam.orthographicSize * Screen.width / Screen.height);
		camMinY = mainCam.transform.position.y - mainCam.orthographicSize;
		camMaxY = mainCam.transform.position.y + mainCam.orthographicSize;
		
		// Ensure the player does not pan the camera past the limits of the maps edges
		if (camMinX < mapImage.renderer.bounds.min.x) {
			tempMapPosition.x -= mapImage.renderer.bounds.min.x - camMinX;
		}
		if (camMinY < mapImage.renderer.bounds.min.y) {
			tempMapPosition.y -= mapImage.renderer.bounds.min.y - camMinY;
		}
		if (camMaxX > mapImage.renderer.bounds.max.x) {
			tempMapPosition.x += camMaxX - mapImage.renderer.bounds.max.x;
		}
		if (camMaxY > mapImage.renderer.bounds.max.y) {
			tempMapPosition.y += camMaxY - mapImage.renderer.bounds.max.y;
		}
		mapManager.transform.position = tempMapPosition;
	}
}
