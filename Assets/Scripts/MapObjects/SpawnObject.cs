using UnityEngine;
using System.Collections;

public class SpawnObject : Selectable {

	// The prefab that will be used to create new objects on the map
	public GameObject objectToSpawn;
	
	// The rarity at which the object will spawn, the higher the value = the more often it will spawn
	// Example: at rarity 3, the object will spawn 3 times as often (rarity = 1 is the 'standard' point of rarity)
	public float objectRarity;
	
	public override string getDescription() {
		return "A good source of high quality food for your colony, but doesn't grow food very fast.";
	}
	
	private GameObject objectToSpawnParent;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		objectToSpawnParent = GameObject.Find("Objects");
		
		// Every second, run a calculation that will determine whether we spawn a new object
		StartCoroutine(spawn());
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
		if (!objectToSpawnParent) objectToSpawnParent = GameObject.Find("Objects");
	}
	
	private IEnumerator spawn() {
		// We will always be running this calculation so long as this spawn object 'lives'
		while (true) {
			// Pause for a second between calculations
			yield return new WaitForSeconds(1);
			
			/**
			 * Calculate the 'chance' that the fruit will spawn using the calculation:
			 * [Number of Open Adjacent Spaces (0-6)] / ([Number of Food Items on Adjacent Spaces (0-6)] + 1)
			 */
			float numOfOpenTiles = 0f;
			float numOfFoodTiles = 0f;
			Tile[] openTiles = new Tile[6];
			Tile[] adjacentTiles = mapManager.getAdjacentTiles(mapManager.getTileAtPosition(transform.position));
			for (int i = 0; i < adjacentTiles.Length; i++) {
				if (isOpen(adjacentTiles[i])) {
					openTiles[(int)numOfOpenTiles] = adjacentTiles[i];
					numOfOpenTiles++;
				}
				if (hasFood(adjacentTiles[i])) numOfFoodTiles++;
			}
			
			float chance = numOfOpenTiles / (numOfFoodTiles + 1);
			chance *= objectRarity;
			
			// Generate a random value to compare against our chance
			if (Random.Range(0f, 100f) < chance) {
				Debug.Log("Spawned an object at: " + Time.time);
				// Create the new object at a random open location
				GameObject newFood = (GameObject) Object.Instantiate(
					objectToSpawn,
					openTiles[Random.Range(0, ((int)numOfOpenTiles)-1)].transform.position,
					Quaternion.identity
				);
				
				// Configure its settings
				newFood.transform.parent = objectToSpawnParent.transform;
				newFood.transform.localPosition = new Vector3(
					newFood.transform.localPosition.x,
					newFood.transform.localPosition.y,
					0
				);
			}
		}
	}
	
	private bool isOpen(Tile tile) {
		Collider2D[] colliders = Physics2D.OverlapPointAll(tile.transform.position);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders[i].gameObject.GetComponent<Tile>() == null && colliders[i].gameObject.GetComponent<Scentpath>() == null) return false;
		}
		return true;
	}
	
	private bool hasFood(Tile tile) {
		Collider2D[] colliders = Physics2D.OverlapPointAll(tile.transform.position);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders[i].gameObject.GetComponent<Food>() != null) return true;
		}
		return false;
	}
}
