﻿using UnityEngine;
using System.Collections;


/**
 * A script that manages a button to be displayed above a GathererUnit if they are holding a Food object
 * Note: the object that this is attached to must be a direct child of the GathererUnit, and also have its own rigidbody and collider
 */
public class DropFoodButton : MonoBehaviour {

	private MapUIManager uiManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// If we are carrying food, we have to display a button to 'drop' the food
		if (transform.parent.gameObject.GetComponentInChildren<Food>() != null) {
			renderer.enabled = true;
		} else {
			renderer.enabled = false;
		}
	}
	
	void OnMouseOver() {
		if (uiManager == null) uiManager = GameObject.Find("UIManager").GetComponent<MapUIManager>();
		uiManager.setDefaultCursor();
		// If user left-clicks the button, drop the fruit
		if (renderer.enabled && Input.GetMouseButtonDown(0)) {
			Debug.Log("Dropping Food");
			transform.parent.gameObject.GetComponent<GathererUnit>().dropFood();
		}
	}
}