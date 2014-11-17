using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {

	public GameObject eventIconPrefab;
	
	private List<GameObject> events;
	private List<GameObject> eventsToRemove;
	
	public void addEvent(Vector2 position) {
		// If the event is already happening on screen, ignore the request
		// (it would be deleted right afterwords anyways)
		if (isOnScreen(position)) return;
		
		// Create a new event icon on the screen
		GameObject eventIcon = GameObject.Instantiate(
			eventIconPrefab,
			new Vector2(0,0),
			Quaternion.identity
		) as GameObject;
		eventIcon.transform.parent = transform;
		eventIcon.transform.localPosition = new Vector3(
			eventIcon.transform.localPosition.x,
			eventIcon.transform.localPosition.y,
			0
		);
		eventIcon.GetComponent<EventIcon>().eventLocation = position;
		
		events.Add(eventIcon);
	}

	// Use this for initialization
	void Start () {
		events = new List<GameObject>();
		eventsToRemove = new List<GameObject>();
		
		// test
		addEvent(new Vector2(-5,5));
	}
	
	// Update is called once per frame
	void Update () {
		// Iterate over each of our events and display an icon for those happening off screen
		foreach (GameObject eventIcon in events) {
			if (isOnScreen(eventIcon.GetComponent<EventIcon>().eventLocation)) {
				// If the event is on screen, remove it from the event list
				//eventsToRemove.Add(eventIcon);
			} else {
				// If offscreen, display an icon that shows the user where to look
				
			}
		}
		
		// Since we can't remove the elements from the list as we iterate, remove them after iterating
		foreach (GameObject eventIcon in eventsToRemove) {
			Debug.Log("Removing event at location: " + eventIcon.GetComponent<EventIcon>().eventLocation);
			events.Remove(eventIcon);
			Destroy(eventIcon);
		}
		eventsToRemove.Clear();
	}
	
	private bool isOnScreen(Vector2 position) {
		// Translate our given position to a viewport position (position relative to camera)
		Vector2 viewport = Camera.main.WorldToViewportPoint(position);
		
		// If the viewport position is outside the bounds of the screen, it is considered 'offscreen'
		if (viewport.x < 0 || viewport.x > 1) return false;
		if (viewport.y < 0 || viewport.y > 1) return false;
		
		// Otherwise, it is considered onscreen
		return true;
	}
}
