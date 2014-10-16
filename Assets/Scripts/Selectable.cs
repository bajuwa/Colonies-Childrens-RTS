using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Provides an interface for the user to 'select' objects on the map in order to give commands or view details
 */
public class Selectable : MonoBehaviour {

	// A list of 'entities' that have selected this selectable
	// This ensures that the selectable is not 'deselected' by one entity when another still wishes to 'select' it
	public Dictionary<int, bool> selectedBy = new Dictionary<int, bool>();
	
	// Every selected needs to be able to highlight the tile beneath it (if any) using mapManager
	protected MapManager mapManager;
	
	private PlayerManager playerManager;
	
	// Variable that gets the asset as a 2D texture
	public Texture2D displayImage;
	
	//The description of the asset's characteristic to be displayed on the GUI
	public virtual string description {get;set;}
	
	// Sets ownership to determine allied vs enemy vs neutral objects
	// 0 is neutral, 1 and 2 are their respective player ids
	// TODO: privatize once done dev
	public int ownedBy = 0;
	protected Player player;

	// Use this for initialization
	void Start () {
		getMapManager();
		playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if (player == null) loadPlayerScript(ownedBy);
	}
	
	protected void loadPlayerScript(int playerId) {
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player")) {
			Player pScript = obj.GetComponent<Player>();
			if (pScript.id == playerId) {
				Debug.Log("Loading script from player: " + pScript.id);
				player = pScript;
				break;
			}
		}
	}
	
	public virtual void select(int id) {
		selectedBy[id] = true;
	}
	
	public virtual void deselect(int id) {
		selectedBy[id] = false;
	}
	
	public bool isNeutralOrFriendly() {
		if (playerManager == null) playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
		return (ownedBy == 0 || ownedBy == playerManager.myPlayerId); 
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
