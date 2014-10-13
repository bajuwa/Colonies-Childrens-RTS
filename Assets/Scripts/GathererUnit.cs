using UnityEngine;
using System.Collections;

public class GathererUnit : AntUnit {

	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
		// Leave the movement up to the AntUnit class
		base.Update();
		
		// If we have stopped moving and have landed on a food item, pick it up if we aren't already carrying some
		if (this.gameObject.GetComponentInChildren<Food>() == null && currentTile == targetTile && targetPath != null && targetPath.getTilePath().Count == 0) {
			Collider2D[] itemsOnSameTile = Physics2D.OverlapPointAll(transform.position);
			foreach (Collider2D col in itemsOnSameTile) {
				if (col.gameObject.GetComponent<Food>() != null) {
					Debug.Log("Detected food on current tile, picking up");
					// Set the parent to our unit so that it is 'carried' when the unit is moving
					col.gameObject.transform.parent = this.gameObject.transform;
					// Also set the local position's z value to -1 to ensure it is visible above the unit
					Vector3 tempPos = col.gameObject.transform.localPosition;
					tempPos.z = -1; 
					col.gameObject.transform.localPosition = tempPos;
				}
			}
		}
	}
	
	// Gatherers can walk on tiles and food items (but only if they aren't already carrying food themselves)
	protected override bool canWalkOn(GameObject gameObj) {
		// If this object is a child of us, we can safely ignore it
		if (gameObj.transform.parent == transform) return true;
		if (gameObj.GetComponent<Tile>() == null) {
			// If it isn't a tile, and also isn't food, then we can't walk on it
			if (gameObj.GetComponent<Food>() == null) return false;
			// If it is food, we can only walk on it if we aren't already carrying food
			if (this.gameObject.GetComponentInChildren<Food>() != null) return false;
		}
		// If it is a tile, we can walk on it
		return true;
	}
}
