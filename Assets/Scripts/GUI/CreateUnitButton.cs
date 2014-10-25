using UnityEngine;
using System.Collections;

public class CreateUnitButton : Button {

	private Selectable parentSelectable;
	
	public GameObject unitToCreate;

	// Use this for initialization
	protected override void Start() {
		base.Start();
		loadParentSelectable();
	}
	
	private void loadParentSelectable() {
		if (!parentSelectable) parentSelectable = transform.parent.gameObject.GetComponentInChildren<Selectable>();
	}
	
	// Update is called once per frame
	protected override void Update() {
		base.Update();
		loadParentSelectable();
		
		// If we our parent object is selected, show this button
		if (parentSelectable.isNeutralOrFriendly() && parentSelectable.isSelected()) {
			renderer.enabled = true;
		} else {
			renderer.enabled = false;
		}
	}
	
	protected override void OnMouseOver() {
		base.OnMouseOver();
		
		// If user left-clicks the button, create the unit
		if (renderer.enabled && Input.GetMouseButtonDown(0)) {
			Debug.Log("Creating unit: " + unitToCreate.ToString());
		}
	}
}
