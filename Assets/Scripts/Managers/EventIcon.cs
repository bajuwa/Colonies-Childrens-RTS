using UnityEngine;
using System.Collections;

public class EventIcon : MonoBehaviour {

    public Vector2 eventLocation;
	
	private Transform arrow;
	private float spaceBuffer = 0.05f;

	// Use this for initialization
	void Start () {
		arrow = transform.Find("arrow");
	}
	
	// Make sure we are constantly pointing at our event location
	void Update () {
		// Position the icon on the edge of the screen at the closest possible location to the actual event
		Vector3 v3Screen = Camera.main.WorldToViewportPoint(eventLocation);
		if (!(v3Screen.x > -0.01f && v3Screen.x < 1.01f && v3Screen.y > -0.01f && v3Screen.y < 1.01f)) {
			v3Screen.x = Mathf.Clamp (v3Screen.x, spaceBuffer, 1-spaceBuffer);
			v3Screen.y = Mathf.Clamp (v3Screen.y, spaceBuffer, 1-spaceBuffer);
			transform.position = Camera.main.ViewportToWorldPoint (v3Screen);
			transform.localPosition = new Vector3(
				transform.localPosition.x,
				transform.localPosition.y,
				0
			);
		}
		
		// Rotate the arrow indicator to point at the location
		float newAngle = SignedAngleBetween(transform.position, eventLocation)-90;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, newAngle));
	}
	
	private float SignedAngleBetween(Vector2 p1, Vector2 p2) {
		return Mathf.Atan2(p2.y-p1.y, p2.x-p1.x)*180 / Mathf.PI;
	}
}
