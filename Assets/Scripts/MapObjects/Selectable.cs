using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Provides an interface for the user to 'select' objects on the map in order to give commands or view details
 */
public class Selectable : Ownable {

	// A list of 'entities' that have selected this selectable
	// This ensures that the selectable is not 'deselected' by one entity when another still wishes to 'select' it
	public Dictionary<int, bool> selectedBy = new Dictionary<int, bool>();
	
	// Every selected needs to be able to highlight the tile beneath it (if any) using mapManager
	protected MapManager mapManager;
	
	//The description of the asset's characteristic to be displayed on the GUI
	public virtual string getDescription() {
		return "";
	}
	public virtual string getName() {
		return "";
	}

	// Use this for initialization
	protected override void Start () {
		base.Start();
		getMapManager();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
		if (!displayImage) loadDisplayImage();
		if (gameObject.GetComponent<SpriteRenderer>().sprite == null) loadSprite();
	}
	
	public Texture2D getDisplayImage() {
		return displayImage;
	}
	
	public virtual void select(int id) {
		selectedBy[id] = true;
		if (!this.gameObject.GetComponent<Tile>()) mapManager.getTileAtPosition(transform.position).select(GetInstanceID());
	}
	
	public virtual void deselect(int id) {
		selectedBy[id] = false;
		if (!this.gameObject.GetComponent<Tile>()) mapManager.getTileAtPosition(transform.position).deselect(GetInstanceID());
	}
	
	public bool isSelected() {
		foreach (KeyValuePair<int, bool> entry in selectedBy) {
			if (entry.Value) return true;
		}
		return false;
	}
	
	public bool isSelectedBy(int id) {
		if (selectedBy.ContainsKey(id)) return selectedBy[id];
		return false;
	}
	
	private void getMapManager() {
		mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
	}
}
