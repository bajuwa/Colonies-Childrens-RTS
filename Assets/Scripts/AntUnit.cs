using UnityEngine;
using System.Collections;

public class AntUnit : Selectable {
	// Unit Stats (TODO: privatize once done dev testing)
	public float currentHp = 10f;
	public float maxHp = 10f;
	public float attack = 1f;
	public float defense = 2f;
	public float speed = 5f;
	public float calculatedVelocity;

	/* Movement variables: All coords must be in local position to handle map panning! */
	private MapManager mapManager;
	// Target Path carries the full list of tiles in order of planned traversal
	private Queue targetPath;
	// Target Coords represents the current short term target from targetPath when moving (should be an adjacent tile coord)
	private Tile targetTile;
	private Tile currentTile;
	
	// Use this for initialization
	void Start() {
		setMapManager();
	}
	
	// Update is called once per frame
	void Update() {
		findPath();
	}
	
	public override void select(int id) {
		Debug.Log("Selecting an Ant Unit");
		base.select(id);
		
		foreach (Tile tile in targetPath) {
			tile.select(GetInstanceID());
		}
		if (currentTile != null) currentTile.select(GetInstanceID());
		if (targetTile != null) targetTile.select(GetInstanceID());
	}
	
	public override void deselect(int id) {
		Debug.Log("Deselecting an Ant Unit");
		base.deselect(id);
		
		foreach (Tile tile in targetPath) {
			tile.deselect(GetInstanceID());
		}
		if (currentTile != null) currentTile.deselect(GetInstanceID());
		if (targetTile != null) targetTile.deselect(GetInstanceID());
	}
	
	public void moveTo(Tile tile) {
		//TODO: A*
		addToPath(tile);	
	}
	
	// Takes note of our current positioning for future pathfinding 
	// Note: should only be called during initial setup after the map 'snaps' the unit to the appropriate position
	public void recordPosition() {
		if (mapManager == null) setMapManager();
		currentTile = mapManager.getTileAtPosition((Vector2)gameObject.transform.localPosition);
		targetTile = currentTile;
		targetPath = new Queue();
		
		// TODO: temp in order to trigger initial movement
		//addToPath(mapManager.getAdjacentTiles(currentTile)[0]);
	}
	
	private Tile popFromPath() {
		Tile tile = (Tile) targetPath.Dequeue();
		tile.select(GetInstanceID());
		return tile;
	}
	
	private void addToPath(Tile tile) {
		tile.select(GetInstanceID());
		targetPath.Enqueue(tile);
	}
	
	private void findPath() {
		if (targetTile != null &&
			(Vector2) gameObject.transform.localPosition != (Vector2) targetTile.transform.localPosition) {
			// If we haven't already reached our next target, continue moving towards that coord
			gameObject.transform.localPosition = Vector2.MoveTowards(
				gameObject.transform.localPosition, 
				targetTile.transform.localPosition, 
				Time.deltaTime * calculatedVelocity
			);
		} else {
			// If we have reached our target, pick our next target from the given path
			if (targetPath.Count > 0) {
				// Deselect our previous tile and switch the target tile we reached
				currentTile.GetComponent<Selectable>().deselect(GetInstanceID());
				currentTile = targetTile;
				targetTile = popFromPath();
				// TODO: test out with 'max terrain value' instead of 'summed terrain value'
				float secondsToTraverse = ((float) (currentTile.terrainValue + targetTile.terrainValue + 1)) / speed;
				calculatedVelocity = Vector2.Distance(currentTile.gameObject.transform.position,
													  targetTile.gameObject.transform.position) / secondsToTraverse;
				
				// TODO: Temp: provide a continuous random path
				// Tile[] availableCoords = mapManager.getAdjacentTiles(targetTile);
				// if (availableCoords.Length > 0) {
					// addToPath(availableCoords[Random.Range(0, availableCoords.Length)]);
				// }
			}
		}
	}
	
	private void setMapManager() {
		GameObject[] managers = GameObject.FindGameObjectsWithTag("MapManager");
		if (managers.Length == 1) mapManager = (MapManager) managers[0].GetComponent(typeof(MapManager));
		else Debug.Log("WARN: Found incorrect number of GameObjects with MapManager Tag.  Expected 1, found " + managers.Length);
	}
}
