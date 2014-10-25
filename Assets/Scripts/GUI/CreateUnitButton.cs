using UnityEngine;
using System.Collections;

public class CreateUnitButton : Button {

	private Selectable parentSelectable;
	private GameObject antUnitParent;
	private MapManager mapManager;
	private Anthill anthillScript;
	
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
		if (!antUnitParent) antUnitParent = GameObject.Find("Units");
		if (!mapManager) mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
		if (!anthillScript) anthillScript = transform.parent.GetComponent<Anthill>();
		
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
			if (!unitToCreate) return;
			Debug.Log("Calculating where to put new unit");
			Tile nearestTile = anthillScript.getNearestUnoccupiedTile(transform.parent.position);
			
			Debug.Log("Creating unit: " + unitToCreate.ToString());
			GameObject instance = (GameObject) Object.Instantiate(
				unitToCreate,
				nearestTile.transform.position,
				Quaternion.identity
			);
			instance.transform.parent = antUnitParent.transform;
			instance.transform.localPosition = new Vector3(
				instance.transform.localPosition.x,
				instance.transform.localPosition.y,
				0
			);
			instance.GetComponent<Ownable>().setAsMine(anthillScript.getPlayerId());
		}
	}
}
