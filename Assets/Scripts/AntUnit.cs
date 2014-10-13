using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AntUnit : Selectable {
	protected const float PATHFINDING_TIMEOUT_SECONDS = 20f;
	
	// Sets ownership of the unit to determine allied vs enemy units
	public int ownedBy = 1;
	
	// Unit Stats (TODO: protect once done dev testing)
	public float currentHp = 10f;
	public float maxHp = 10f;
	public float attack = 1f;
	public float defense = 2f;
	public float speed = 5f;
	public float calculatedVelocity;
	
	protected MapManager mapManager;

	/* Movement variables: All coords must be in local position to handle map panning! */
	// Target Path carries the full list of tiles in order of planned traversal
	protected Path targetPath;
	// Target Coords represents the current short term target from targetPath when moving (should be an adjacent tile coord)
	protected Tile targetTile;
	protected Tile currentTile;
	
	// Use this for initialization
	protected virtual void Start() {
		setMapManager();
		targetPath = getNewPath();
	}
	
	/**
	 * Overridden methods from being a Selectable
	 */
	public override void select(int id) {
		base.select(id);
		targetPath.selectPath();
		if (currentTile != null) currentTile.select(GetInstanceID());
		if (targetTile != null) targetTile.select(GetInstanceID());
	}
	
	public override void deselect(int id) {
		base.deselect(id);
		targetPath.deselectPath();
		if (currentTile != null) currentTile.deselect(GetInstanceID());
		if (targetTile != null) targetTile.deselect(GetInstanceID());
	}
	
	// Takes note of our current positioning for future pathfinding 
	// Note: should only be called during initial setup after the map 'snaps' the unit to the appropriate position
	public void recordPosition() {
		if (mapManager == null) setMapManager();
		currentTile = mapManager.getTileAtPosition((Vector2)gameObject.transform.localPosition);
		targetTile = currentTile;
	}
	
	/**
	 * A coroutine that uses A* pathfinding to find an optimal path between the units
	 * 'targetTile' to the given tileToMoveTo
	 * Note: it does not use 'currentTile' since it is more often considered 'previousTile'
	 */
	public IEnumerator moveTo(Tile tileToMoveTo) {
		// Clear the previous path in case the user gave overriding commands
		targetPath.setNewTileQueue(new Queue<Tile>());
		
		// Set up the priority queue for our pathfinding algo: A*
		PriorityQueue priorityQueue = new PriorityQueue();
		priorityQueue.add(getNewPath(targetTile, Vector2.Distance(targetTile.gameObject.transform.position, tileToMoveTo.gameObject.transform.position)));
		// Keep track of which tiles (by instance id) we have already 'visited' to cut down on running time
		Dictionary<int, bool> visitedTiles = new Dictionary<int, bool>();
		visitedTiles[targetTile.GetInstanceID()] = true;
		
		// Continue to check and expand the first Path in the queue until we reach our target
		Path path;
		while (true) {
			// Pop our next path and check if we have reached our target yet
			path = priorityQueue.pop();
			if (tileToMoveTo == path.getLastTileInPath()) {
				Debug.Log("Found optimal path: ");
				path.printPath();
				break;
			}
			
			// If we havent reached the target, expand on the currently popped path with all adjacent tile options
			foreach (Tile adjacentTile in mapManager.getAdjacentTiles(path.getLastTileInPath())) {
				// Only add it to the path if we haven't already visited it yet
				if (visitedTiles.ContainsKey(adjacentTile.GetInstanceID())) continue;
				else visitedTiles[adjacentTile.GetInstanceID()] = true;
				
				// Add the tile to a deep copy of our original path
				Debug.Log("Adding adjacentTile to path: " + adjacentTile.GetInstanceID() + " - " + adjacentTile.gameObject.transform.position.ToString());
				Path copiedPath = getNewPath(path);
				// Our A* heuristic is the straight line distance between our next tile and our target
				float heuristic = Vector2.Distance(adjacentTile.gameObject.transform.position, tileToMoveTo.gameObject.transform.position);
				copiedPath.add(adjacentTile, heuristic);
				
				// Add the full path back to our priority queue
				priorityQueue.add(copiedPath);
			}
			
			// Yield the coroutine to let the rest of unity work for a bit
			yield return null;
		}
		Debug.Log("Found optimal path");
		setPath(path);
		
		// Finish the coroutine
		return true;
	}
	
	// Every frame, units should continue working towards their current target tile (if any)
	protected virtual void Update() {
		findPath();
	}
	
	protected void setMapManager() {
		GameObject[] managers = GameObject.FindGameObjectsWithTag("MapManager");
		if (managers.Length == 1) mapManager = (MapManager) managers[0].GetComponent(typeof(MapManager));
		else Debug.Log("WARN: Found incorrect number of GameObjects with MapManager Tag.  Expected 1, found " + managers.Length);
	}
	
	protected void findPath() {
		if (targetTile != null &&
			(Vector2) gameObject.transform.localPosition != (Vector2) targetTile.transform.localPosition) {
			// If we haven't already reached our next target, continue moving towards that coord
			gameObject.transform.localPosition = Vector2.MoveTowards(
				gameObject.transform.localPosition, 
				targetTile.transform.localPosition, 
				Time.deltaTime * calculatedVelocity
			);
		} else if (targetPath != null) {
			// If we have reached our target, pick our next target from the given path
			if (targetPath.getTilePath().Count > 0) {
				// Deselect our previous tile and switch the target tile we reached
				currentTile.GetComponent<Selectable>().deselect(GetInstanceID());
				currentTile = targetTile;
				targetTile = targetPath.pop();
				if (isSelected()) targetTile.select(GetInstanceID());
				// TODO: test out with 'max terrain value' instead of 'summed terrain value'
				float secondsToTraverse = ((float) (currentTile.terrainValue + targetTile.terrainValue)) / speed;
				calculatedVelocity = Vector2.Distance(currentTile.gameObject.transform.position, targetTile.gameObject.transform.position) / secondsToTraverse;
			}
		}
	}
	
	protected void setPath(Path path) {
		targetPath.setNewTileQueue(path.getTilePath());
		if (isSelected()) targetPath.selectPath();
	}
	
	/**
	 * A basic priority queue of Path objects
	 */
	protected class PriorityQueue {
		protected List<Path> priorityQueue = new List<Path>();
		
		public void add(Path newPath) {
			int insertIndex = 0;
			foreach (Path oldPath in priorityQueue) {
				if (newPath.CompareTo(oldPath) < 0) {
					break;
				}
				insertIndex++;
			}
			priorityQueue.Insert(insertIndex, newPath);
		}
		
		public Path pop() {
			Path popped = priorityQueue[0];
			priorityQueue.RemoveAt(0);
			return popped;
		}
	}
	
	/**
	 * Some factory-esque methods to utilize the unique id of ScriptableObject's GetInstanceID
	 */
	protected Path getNewPath() {
		return (Path) ScriptableObject.CreateInstance("Path");
	}
	
	protected Path getNewPath(Tile startingTile, float startingHeuristic) {
		Path path = (Path) ScriptableObject.CreateInstance("Path");
		path.init(startingTile, startingHeuristic);
		return path;
	}
	
	protected Path getNewPath(Path originalPath) {
		Path path = (Path) ScriptableObject.CreateInstance("Path");
		path.init(originalPath);
		return path;
	}
}
