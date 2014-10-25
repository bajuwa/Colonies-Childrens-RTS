using UnityEngine;
using System.Collections;


/**
 * A script that manages a button to be displayed above a GathererUnit if they are holding a Food object
 * Note: the object that this is attached to must be a direct child of the GathererUnit, and also have its own rigidbody and collider
 */
public class Button : Ownable {

	private MapUIManager uiManager;

	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
	}
	
	protected virtual void OnMouseOver() {
		if (uiManager == null) uiManager = GameObject.Find("UIManager").GetComponent<MapUIManager>();
		uiManager.setDefaultCursor();
	}
}
