using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageUiHolder : MonoBehaviour {
	
	public PlaceTextFromCorner title;
	public PlaceTextFromCorner message;
	public bool isReady;

	private string messageTitle;
	private string[] messages;
	private int messageIndex = 0;

	// Use this for initialization
	void Start () {
		messages = new string[0];
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) && guiTexture.HitTest(Input.mousePosition)) {
			// If the player clicked the box, advance to the next message
			messageIndex++;
		}
		
		if (messageIndex >= messages.Length) {
			title.enabled = false;
			message.enabled = false;
			guiTexture.enabled = false;
			isReady = true;
		} else {
			title.enabled = true;
			message.enabled = true;
			guiTexture.enabled = true;
			isReady = false;
			
			title.text = messageTitle;
			message.text = messages[messageIndex];
		}
	}
	
	public void setNewMessages(string title, string[] newMessages) {
		messageIndex = 0;
		messageTitle = title;
		messages = newMessages;
	}
}
