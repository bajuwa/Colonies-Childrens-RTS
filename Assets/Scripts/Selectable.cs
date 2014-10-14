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
	
	// Sets ownership to determine allied vs enemy vs neutral objects
	// 0 is neutral, 1 and 2 are their respective player ids
	// TODO: privatize once done dev
	public int ownedBy = 0;
	private Player player;

	// Use this for initialization
	void Start () {
		getPlayerScript();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public virtual void select(int id) {
		selectedBy[id] = true;
	}
	
	public virtual void deselect(int id) {
		selectedBy[id] = false;
	}
	
	public bool isNeutralOrFriendly() {
		if (player == null) getPlayerScript();
		return (ownedBy == 0 || ownedBy == player.id); 
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
	
	private void getPlayerScript() {
		player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Player>();
	}
}
