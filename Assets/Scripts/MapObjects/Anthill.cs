using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Anthill : Attackable {

	private GameObject gathererToCreate;
	private GameObject warriorToCreate;
	private GameObject scoutToCreate;

	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
		maxHp = currentHp;
	}
	
	public void addFoodPoints(int points) {
		currentHp += points;
		maxHp = currentHp;
	}
	
	public void spendStoredFoodPoints(int points) {
		currentHp -= points;
		maxHp = currentHp;
	}
	
	protected override void loadSprite() {
		gameObject.GetComponent<SpriteRenderer>().sprite = getSpriteFromPlayer("anthillSprite");
	}
	
	protected override void loadDisplayImage() {
		displayImage = getTextureFromPlayer("anthillDisplay");
	}
	
	public int getStoredFoodPoints() {
		return (int) Mathf.Ceil(currentHp);
	}
		
	public Tile getNearestUnoccupiedTile(Vector2 position) {
		Tile nextTile = mapManager.getTileAtPosition(position);
		
		// Iterate over all the adjacent tiles until you find an unoccupied tile
		Tile unoccupiedTile = null;
		Dictionary<int, bool> visitedTiles = new Dictionary<int, bool>();
		visitedTiles[nextTile.GetInstanceID()] = true;
		Queue<Tile> tileQueue = new Queue<Tile>();
		tileQueue.Enqueue(nextTile);
		
		while (!unoccupiedTile) {
			// Get the next tile we should look at
			nextTile = tileQueue.Dequeue();
			if (!nextTile) {
				Debug.Log("Unable to find unoccupied tile");
				return null;
			}
			
			// Add its neighbours to the queue if we haven't visited them yet
			Tile[] adjacentTiles = mapManager.getAdjacentTiles(nextTile);
			for (int i = 0; i < adjacentTiles.Length; i++) {
				if (visitedTiles.ContainsKey(adjacentTiles[i].GetInstanceID())) continue;
				visitedTiles[adjacentTiles[i].GetInstanceID()] = true;
				tileQueue.Enqueue(adjacentTiles[i]);
			}
			
			// Check if our given tile is occupied by a moving unit
			if (nextTile.occupiedBy) continue;
			
			// Check if our given tile is occupied by a static object
			Collider2D[] objectsAtTile = Physics2D.OverlapPointAll(nextTile.transform.position);
			bool foundInvalid = false;
			for (int j = 0; j < objectsAtTile.Length; j++) {
				if (objectsAtTile[j].GetComponent<Tile>() == null && 
					objectsAtTile[j].GetComponent<Scentpath>() == null) {
						foundInvalid = true;
						break;
				}
			}
			
			// If all of our 'occupied' checks passed, we found our unoccupied tile
			if (!foundInvalid) return nextTile;
		}
	
		return null;
	}
}
