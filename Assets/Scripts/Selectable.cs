using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selectable : MonoBehaviour {

	public Dictionary<int, bool> selectedBy = new Dictionary<int, bool>();

	// Use this for initialization
	void Start () {
	
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
}
