using UnityEngine;
using System.Collections;

public class AntUnit : MonoBehaviour {
	// Unit Stats (TODO: privatize once done dev testing)
	public float currentHp = 10f;
	public float maxHp = 10f;
	public float attack = 10f;
	public float defense = 10f;
	public float speed = 0.2f;

	/* Movement variables: All coords must be in local position to handle map panning! */
	private MapManager mapManager;
	// Target Path carries the full list of tiles in order of planned traversal
	private Queue targetPath;
	// Target Coords represents the current short term target from targetPath when moving (should be an adjacent tile coord)
	private Tile targetTile;
	
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
				Time.deltaTime * speed
			);
		} else {
			// If we have reached our target, pick our next target from the given path
			if (targetPath.Count > 0) {
				targetTile = (Tile) targetPath.Dequeue();
				
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
		targetTile = null;
		targetPath = new Queue();
		if (mapManager == null) setMapManager();
		targetPath.Enqueue(mapManager.getAdjacentTiles(
			mapManager.getTileAtPosition((Vector2)gameObject.transform.localPosition)
		)[0]);
	}
	
	private void setMapManager() {
		GameObject[] managers = GameObject.FindGameObjectsWithTag("MapManager");
		if (managers.Length == 1) mapManager = (MapManager) managers[0].GetComponent(typeof(MapManager));
		else Debug.Log("WARN: Found incorrect number of GameObjects with MapManager Tag.  Expected 1, found " + managers.Length);
	}
}
