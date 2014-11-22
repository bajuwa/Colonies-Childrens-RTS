using UnityEngine;
using System.Collections;

public class CreateUnitButton : Button {

	private Selectable parentSelectable;
	private GameObject antUnitParent;
	private MapManager mapManager;
	private Anthill anthillScript;
	private NetworkManager netMan;
	
	public GameObject unitToCreate;
	public int foodCost = 0;
	
	public Sprite enabledImage;
	public Sprite disabledImage;
	private bool buttonEnabled;

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
		if (!netMan) netMan = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		// If we our parent object is selected, show this button
		if (parentSelectable.isNeutralOrFriendly() && parentSelectable.isSelected()) {
			renderer.enabled = true;
			// If we can't afford to make this unit, disable the button
			if (anthillScript.getStoredFoodPoints() < foodCost) {
				buttonEnabled = false;
				GetComponent<SpriteRenderer>().sprite = disabledImage;
			} else {
				buttonEnabled = true;
				GetComponent<SpriteRenderer>().sprite = enabledImage;
			}
		} else {
			renderer.enabled = false;
			buttonEnabled = false;
		}
		
	}
	
	protected override void OnMouseOver() {
		base.OnMouseOver();
		
		// If user left-clicks the button, create the unit
		if (buttonEnabled && Input.GetMouseButtonDown(0)) {
			if (!unitToCreate) return;
			if (anthillScript.getStoredFoodPoints() < foodCost) return;
			anthillScript.spendStoredFoodPoints(foodCost);
			
			Debug.Log("Calculating where to put new unit");
			Tile nearestTile = anthillScript.getNearestUnoccupiedTile(transform.parent.position);
			
			Debug.Log("Creating unit: " + unitToCreate.ToString());
			//Note: Everything in the "if" statement is network based.
			//It checks if the game is networked (if the network is a server or client)
			//and then network instantiates that object.
			//Everything in the else clause is untouched (except for the else clause itself
			//and functions as it did before the change
			if (Network.isServer || Network.isClient) {
				GameObject instance = (GameObject) Network.Instantiate(
					unitToCreate,
					nearestTile.transform.position,
					Quaternion.identity,
					0
				);
				instance.transform.parent = antUnitParent.transform;
				instance.transform.localPosition = new Vector3(
					instance.transform.localPosition.x,
					instance.transform.localPosition.y,
					-2
				);
				if (Network.isClient) netMan.changeID(instance); //have to change ID of Player 2's stuff
			}
			else {
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
}
