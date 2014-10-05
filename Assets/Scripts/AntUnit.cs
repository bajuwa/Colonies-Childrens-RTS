using UnityEngine;
using System.Collections;

public class AntUnit : MonoBehaviour {
	// Unit Stats (TODO: privatize once done dev testing)
	public float currentHp = 10f;
	public float maxHp = 10f;
	public float attack = 10f;
	public float defense = 10f;
	public float speed = 0.2f;

	// TODO: switch to private once done dev testing
	/* Movement variables: All coords must be in local position to handle map panning! */
	public MapManager mapManager;
	// Target Path carries the full list of tiles in order of planned traversal
	public Queue targetPath;
	// Target Coords represents the current short term target from targetPath when moving (should be an adjacent tile coord)
	public Vector2 targetCoords;
	
	
	private int pathIndex = 0;
	public Font tempFont;
	
	// Use this for initialization
	void Start() {
		GameObject[] managers = GameObject.FindGameObjectsWithTag("MapManager");
		if (managers.Length == 1) mapManager = (MapManager) managers[0].GetComponent(typeof(MapManager));
		else Debug.Log("WARN: Found incorrect number of GameObjects with MapManager Tag.  Expected 1, found " + managers.Length);
		recordPosition();
	}
	
	// Update is called once per frame
	void Update() {
		if ((Vector2) gameObject.transform.localPosition != targetCoords) {
			// If we haven't already reached our next target, continue moving towards that coord
			gameObject.transform.localPosition = Vector2.MoveTowards(gameObject.transform.localPosition, targetCoords, Time.deltaTime * speed);
		} else {
			// If we have reached our target, pick our next target from the given path
			if (targetPath.Count > 0) {
				targetCoords = (Vector2) targetPath.Dequeue();
				
				// TODO: Temp: provide a continuous random path
				Vector2[] availableCoords = mapManager.getAdjacentTilePositions(targetCoords);
				if (availableCoords.Length > 0) {
					targetPath.Enqueue(availableCoords[Random.Range(0, availableCoords.Length)]);
				}
				
				Debug.Log("To the next coords: " + targetCoords.x + ", " + targetCoords.y);
			}
		}
	}
	
	public void setPath(Vector2[] path) {
		targetPath = new Queue(path);
	}
	
	// Takes note of our current positioning for future pathfinding 
	// Note: should only be called during initial setup after the map 'snaps' the unit to the appropriate position
	public void recordPosition() {
		targetCoords = gameObject.transform.localPosition;
		targetPath = new Queue();
		targetPath.Enqueue(mapManager.getAdjacentTilePositions((Vector2)gameObject.transform.localPosition)[0]);
	}
}
