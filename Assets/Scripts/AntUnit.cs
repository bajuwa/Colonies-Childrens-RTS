using UnityEngine;
using System.Collections;

public class AntUnit : MonoBehaviour {
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
				currentTile = targetTile;
				targetTile = (Tile) targetPath.Dequeue();
				float secondsToTraverse = ((float) (currentTile.terrainValue + targetTile.terrainValue + 1)) / speed;
				calculatedVelocity = Vector2.Distance(currentTile.gameObject.transform.position,
													  targetTile.gameObject.transform.position) / secondsToTraverse;
				
				// TODO: Temp: provide a continuous random path
				Tile[] availableCoords = mapManager.getAdjacentTiles(targetTile);
				if (availableCoords.Length > 0) {
					targetPath.Enqueue(availableCoords[Random.Range(0, availableCoords.Length)]);
				}
			}
		}
	}
	
	public void setPath(Queue path) {
		targetPath = new Queue(path);
	}
	
	// Takes note of our current positioning for future pathfinding 
	// Note: should only be called during initial setup after the map 'snaps' the unit to the appropriate position
	public void recordPosition() {
		if (mapManager == null) setMapManager();
		currentTile = mapManager.getTileAtPosition((Vector2)gameObject.transform.localPosition);
		targetTile = currentTile;
		targetPath = new Queue();
		
		// TODO: temp in order to trigger initial movement
		targetPath.Enqueue(mapManager.getAdjacentTiles(currentTile)[0]);
	}
	
	private void setMapManager() {
		GameObject[] managers = GameObject.FindGameObjectsWithTag("MapManager");
		if (managers.Length == 1) mapManager = (MapManager) managers[0].GetComponent(typeof(MapManager));
		else Debug.Log("WARN: Found incorrect number of GameObjects with MapManager Tag.  Expected 1, found " + managers.Length);
	}
}
