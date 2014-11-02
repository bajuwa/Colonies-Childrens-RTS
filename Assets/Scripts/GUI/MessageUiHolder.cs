using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageUiHolder : MonoBehaviour {

	public string messageTitle;
	public string messageContent;
	public Queue<string> messageQueue;
	
	public PlaceTextFromCorner title;
	public PlaceTextFromCorner message;

	// Use this for initialization
	void Start () {
		messageQueue = new Queue<string>();
	}
	
	// Update is called once per frame
	void Update () {
		if (messageContent == "" && messageQueue.Count == 0) {
			title.enabled = false;
			message.enabled = false;
			guiTexture.enabled = false;
		} else {
			title.enabled = true;
			message.enabled = true;
			guiTexture.enabled = true;
			
			if (Input.GetMouseButtonDown(0) && guiTexture.HitTest(Input.mousePosition)) {
				// If the player clicked the box, advance to the next message
				if (messageQueue.Count == 0) messageContent = "";
				else messageContent = messageQueue.Dequeue();
			}
			
			title.text = messageTitle;
			message.text = messageContent;
		}
	}
	
	public void setNewMessages(string title, Queue<string> messages) {
		messageTitle = title;
		messageContent = messages.Dequeue();
		messageQueue = messages;
	}
}
