using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * Handles the generic behaviours that are common to all ant units, including:
 * - movement
 * - pathfinding
 * - interactions (blocked by objects, battle calculations, etc)
 */
public class AntUnit : Attackable {
	protected bool isCalculatingPath = false;
	
	public float speed = 5f;
	public float calculatedVelocity;
	
	public bool enableDiegeticUnitCount = true;
	private Transform backLeftAnt;
	private Transform backRightAnt;
	private Transform frontAnt;

	/* Movement variables: All coords must be in local position to handle map panning! */
	// Target Path carries the full list of tiles in order of planned traversal
	protected Path targetPath;
	// Target Coords represents the current short term target from targetPath when moving (should be an adjacent tile coord)
	private Tile targetTile;
	private Tile currentTile;

	protected float anthillRange = 3f;
	protected int anthillMask = 0;
	
	public GameObject healingAnimation;
	private const string HEALING_ANIMATION_NAME = "HealingAnimation";
	
	protected virtual string getAnimationName() {
		return "";
	}
	
	// Use this for initialization
	protected override void Start() {
		base.Start();
		targetPath = getNewPath();
		anthillMask = 1 << LayerMask.NameToLayer("Anthill");
	}
	
	// Takes note of our current positioning for future pathfinding 
	// Note: should only be called during initial setup after the map 'snaps' the unit to the appropriate position
	public void recordPosition() {
		Debug.Log("Recording position for: " + this.gameObject.ToString() + " " + this.gameObject.GetInstanceID());
		if (mapManager == null) setMapManager();
		currentTile = mapManager.getTileAtPosition((Vector2)gameObject.transform.localPosition);
		setTargetTile(currentTile);
		targetPath = getNewPath();
	}
	
	private void setTargetTile(Tile newTargetTile) {
		if (!newTargetTile) return;
		if (targetTile) targetTile.occupiedBy = null;
		targetTile = newTargetTile;
		newTargetTile.occupiedBy = this.gameObject;
	}
	
	protected void resetTargetTile() {
		targetTile.deselect(GetInstanceID());
		setTargetTile(currentTile);
	}
	
	public Tile getTargetTile() {
		return targetTile;
	}
	
	public Tile getCurrentTile() {
		return currentTile;
	}
	
	public override void startBattle() {
		base.startBattle();
		isInBattle = true;
		targetPath.setNewTileQueue(new Queue<Tile>());
	}
	
	public override void removeFromBattle() {
		base.removeFromBattle();
		isInBattle = false;
		targetPath.setNewTileQueue(new Queue<Tile>());
	}
	
	/**
	 * Overridden methods from being a Selectable
	 * When an ant is selected, the entire path they will be moving through will also be 'selected'
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

	/**
	 * A coroutine that uses A* pathfinding to find an optimal path between the unit's
	 * 'targetTile' to the given tileToMoveTo
	 * Note: it does not use 'currentTile' since that can often be considered 'previousTile' as well
	 */
	public virtual IEnumerator moveTo(Tile tileToMoveTo, bool activelySetNewTarget = false) {
		// Avoid multiple calls to move the unit while the path is still being calculated
		if ((isCalculatingPath && !activelySetNewTarget) || isInBattle || tileToMoveTo == null) yield break;
		isCalculatingPath = true;
		Debug.Log("Calculating path to new tile");
	
		// Clear the previous path in case the user gave overriding commands
		targetPath.setNewTileQueue(new Queue<Tile>());
		
		// Set up the priority queue for our pathfinding algo: A*
		PriorityQueue priorityQueue = new PriorityQueue();
		priorityQueue.add(
			getNewPath(
				targetTile, 
				Vector2.Distance(targetTile.gameObject.transform.position, tileToMoveTo.gameObject.transform.position), 
				getTileCost(targetTile)
			)
		);
		// Keep track of which tiles (by instance id) we have already 'visited' to cut down on running time
		Dictionary<int, bool> visitedTiles = new Dictionary<int, bool>();
		visitedTiles[targetTile.GetInstanceID()] = true;
		
		// Continue to check and expand the first Path in the queue until we reach our target
		Path path;
		float startingTime = Time.time;
		while (true) {
			// If we have no paths left in the queue, then a solution is impossible
			if (priorityQueue.getCount() == 0) {
				Debug.Log("Could not find a path to target tile");
				isCalculatingPath = false;
				return false;
			}
		
			// Pop our next path and check if we have reached our target yet
			path = priorityQueue.pop();
			if (tileToMoveTo == path.getLastTileInPath()) {
				//Debug.Log("Found optimal path: ");
				//path.printPath();
				break;
			}
			
			// If we havent reached the target, expand on the currently popped path with all adjacent tile options
			foreach (Tile adjacentTile in mapManager.getAdjacentTiles(path.getLastTileInPath())) {
				// Only add it to the path if it is unoccuppied and we haven't already visited it yet
				if (visitedTiles.ContainsKey(adjacentTile.GetInstanceID()) || !canWalkTo(adjacentTile.transform.position)) continue;
				else visitedTiles[adjacentTile.GetInstanceID()] = true;
				
				// Add the tile to a deep copy of our original path
				//Debug.Log("Adding adjacentTile to path: " + adjacentTile.GetInstanceID() + " - " + adjacentTile.gameObject.transform.position.ToString());
				Path copiedPath = getNewPath(path);
				// Our A* heuristic is the straight line distance between our next tile and our target
				float heuristic = Vector2.Distance(adjacentTile.gameObject.transform.position, tileToMoveTo.gameObject.transform.position);
				
				// Add the full path back to our priority queue
				copiedPath.add(adjacentTile, heuristic, getTileCost(adjacentTile));
				priorityQueue.add(copiedPath);
			}
			
			// If 100 milliseconds have passed Yield the coroutine to let the rest of unity work for a bit (until the next frame)
			if (Time.time - startingTime > 0.1f) {
				yield return null;
				startingTime = Time.time;
			}
		}
		//Debug.Log("Found optimal path");
		setPath(path);
		
		// Finish the coroutine
		isCalculatingPath = false;
		return true;
	}
	
	private float getTileCost(Tile tile) {
		float cost = tile.terrainValue;
		Scentpath scent = mapManager.getScentpathAtPosition(tile.transform.position);
		if (!scent || !scent.isNeutralOrFriendly()) cost *= 2;
		return cost;
	}
	
	// Every frame, units should continue working towards their current target tile (if any)
	protected override void Update() {
		base.Update();
		loadAnimator();
		if (targetTile == null) recordPosition();
		move();
		updateDiegeticUnits();
		flipIfGoingLeft();
	}
	
	private void flipIfGoingLeft() {
		// Until this is fixed using networking, disable it
		return;
		
		if ((targetTile.transform.position.x < this.transform.position.x && transform.localScale.x > 0) || 
			(targetTile.transform.position.x > this.transform.position.x && transform.localScale.x < 0)) {
			transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
		}
	}
	
	private void loadAnimator() {
		if (enableDiegeticUnitCount) {
			Transform[] diegeticChildren = {
				this.gameObject.transform.Find("BackLeftAnt"),
				this.gameObject.transform.Find("BackRightAnt"),
				this.gameObject.transform.Find("FrontAnt")
			};
			
			foreach (Transform diegeticChild in diegeticChildren) {
				if (!diegeticChild || diegeticChild.GetComponent("Animator")) continue;
				Animator childAnimator = diegeticChild.gameObject.AddComponent("Animator") as Animator;
				if (!childAnimator.runtimeAnimatorController) childAnimator.runtimeAnimatorController = getAnimatorFromPlayer(getAnimationName());
			}
		} else {
			Animator singleAnimator = this.gameObject.GetComponent("Animator") as Animator;
			if (!singleAnimator) singleAnimator = this.gameObject.AddComponent("Animator") as Animator;
			if (!singleAnimator.runtimeAnimatorController) singleAnimator.runtimeAnimatorController = getAnimatorFromPlayer(getAnimationName());
		}
	}
	
	private void updateDiegeticUnits() {
		if (!enableDiegeticUnitCount) return;
	
		if (!backLeftAnt || !backRightAnt || !frontAnt) {
			Debug.Log("Missing a diegetic ant sprite, attempting to initialize");
			initializeDiegeticAnts();
			return;
		}
		
		// Some aspects of the game may disable sprites, so only apply diegetics when we are still enabled
		if (!this.GetComponent<SpriteRenderer>().enabled) {
			backLeftAnt.gameObject.active = false;
			backRightAnt.gameObject.active = false;
			frontAnt.gameObject.active = false;
			return;
		}
		
		if (currentHp/maxHp <= .33f) {
			backLeftAnt.gameObject.active = false;
			backRightAnt.gameObject.active = false;
			frontAnt.gameObject.active = true;
		} else if (currentHp/maxHp <= .66f) {
			backLeftAnt.gameObject.active = true;
			backRightAnt.gameObject.active = true;
			frontAnt.gameObject.active = false;
		} else {
			backLeftAnt.gameObject.active = true;
			backRightAnt.gameObject.active = true;
			frontAnt.gameObject.active = true;
		}
	}
	
	private void initializeDiegeticAnts() {
		backLeftAnt = transform.Find("BackLeftAnt");
		backRightAnt = transform.Find("BackRightAnt");
		frontAnt = transform.Find("FrontAnt");
	}
	
	protected void setAnimation(int state) {
		if (enableDiegeticUnitCount) {
			if (backLeftAnt) backLeftAnt.gameObject.GetComponent<Animator>().SetInteger("STATE", state);
			if (backRightAnt) backRightAnt.gameObject.GetComponent<Animator>().SetInteger("STATE", state);
			if (frontAnt) frontAnt.gameObject.GetComponent<Animator>().SetInteger("STATE", state);
		} else {
			this.gameObject.GetComponent<Animator>().SetInteger("STATE", state);
		}
	}
	
	// Used to interrupt/cancel a units movement
	public override void interrupt() {
		base.interrupt();
		targetPath.setNewTileQueue(new Queue<Tile>());
	}
	
	protected void setMapManager() {
		GameObject[] managers = GameObject.FindGameObjectsWithTag("MapManager");
		if (managers.Length == 1) mapManager = (MapManager) managers[0].GetComponent(typeof(MapManager));
		else Debug.Log("WARN: Found incorrect number of GameObjects with MapManager Tag.  Expected 1, found " + managers.Length);
	}
	
	/**
	 * Moves from the 'currentTile' to the 'targetTile', taking in to account the units base speed and the terrain type
	 * If the unit has reached the targetTile, it reads the next tile from the path (if available)
	 */
	protected void move() {
		if (targetTile != null &&
			(Vector2) gameObject.transform.localPosition != (Vector2) targetTile.transform.localPosition) {
			// Get the tile we are currently over, and calculate speed based off its terrain value
			Tile tileCurrentlyOver = mapManager.getTileAtPosition(transform.position);
			float secondsToTraverse = (float) (tileCurrentlyOver.terrainValue + 2) / speed;
			
			// If we have a friendly scentpath on that tile, it will take half the time
			Scentpath path = mapManager.getScentpathAtPosition(tileCurrentlyOver.transform.position);
			if (path && path.isNeutralOrFriendly()) secondsToTraverse /= 2;
			
			// Calculate our velocity based on our speed
			calculatedVelocity = Vector2.Distance(currentTile.gameObject.transform.position, targetTile.gameObject.transform.position) / secondsToTraverse;
			
			// Move towards our target at our calculated velocity
			gameObject.transform.localPosition = Vector2.MoveTowards(
				gameObject.transform.localPosition, 
				targetTile.transform.localPosition, 
				Time.deltaTime * calculatedVelocity
			);
		} else if (targetPath) {
			// Since the positions are now the same, we should carry over our 'target' to our new 'current' tile references
			if (currentTile != targetTile) {
				currentTile.GetComponent<Selectable>().deselect(GetInstanceID());
				currentTile.occupiedBy = null;
				currentTile = targetTile;
				currentTile.occupiedBy = this.gameObject;
				if (isSelected()) currentTile.select(GetInstanceID());
			}
			
			// If we have reached our target, pick our next target from the given path
			if (targetPath.getTilePath().Count > 0) {
				// Before we actually move to the tile, we need to check that no obstacles are in the way
				Tile nextTile = targetPath.pop();
				if (!canWalkTo(nextTile.transform.position)) {
					// If we can't walk on our next tile, cancel movement
					Debug.Log("Encountered obstacle, recalculating path");
					Tile tile = targetPath.clearPath();
					Debug.Log(tile);
					StartCoroutine(moveTo(tile));
					return;
				}
				
				// Switch the target tile we reached
				setTargetTile(nextTile);
				if (isSelected()) targetTile.select(GetInstanceID());
			} else if (getNearbyAnthill(transform.position) && currentHp < maxHp) {
				// If we have stopped moving and have landed within range of a friendly anthill, start to heal
				healSelf();
			} else {
				stopHealingSelf();
			}
		}
	}
	
	protected void healSelf() {
		// If we are below perfect health, heal at a rate of 0.333 hp per second
		currentHp = Mathf.Min(maxHp, currentHp + Time.deltaTime/3);
		Debug.Log("Network view healing");
		networkView.RPC("NetworkHealSelf", RPCMode.Others,(int) currentHp);
		// Ensure we show a healing animation
		if ((!enableDiegeticUnitCount && !transform.Find(HEALING_ANIMATION_NAME)) || 
			(enableDiegeticUnitCount && !frontAnt.transform.Find(HEALING_ANIMATION_NAME))) {
				spawnHealingAnimation();
		}
		
	}
	
	[RPC] void NetworkHealSelf (int hp) {
		currentHp = hp;
	}
	
	protected void spawnHealingAnimation() {
		Debug.Log("Spawning Healing Animation");
		if (enableDiegeticUnitCount) {
			if (backLeftAnt) attachHealingAnimationTo(backLeftAnt.gameObject);
			if (backRightAnt) attachHealingAnimationTo(backRightAnt.gameObject);
			if (frontAnt) attachHealingAnimationTo(frontAnt.gameObject);
		} else {
			attachHealingAnimationTo(this.gameObject);
		}
	}
	
	protected void stopHealingSelf() {
		if (enableDiegeticUnitCount) {
			if (backLeftAnt && backLeftAnt.Find(HEALING_ANIMATION_NAME)) destroyThis(backLeftAnt.Find(HEALING_ANIMATION_NAME).gameObject);
			if (backRightAnt && backRightAnt.Find(HEALING_ANIMATION_NAME)) destroyThis(backRightAnt.Find(HEALING_ANIMATION_NAME).gameObject);
			if (frontAnt && frontAnt.Find(HEALING_ANIMATION_NAME)) destroyThis(frontAnt.Find(HEALING_ANIMATION_NAME).gameObject);
		} else {
			if (transform.Find(HEALING_ANIMATION_NAME)) destroyThis(transform.Find(HEALING_ANIMATION_NAME).gameObject);
		}
		// If we are below perfect health, heal at a rate of 0.333 hp per second
	}
	
	private void destroyThis(GameObject gameObj) {
		if (Network.isClient || Network.isServer) {
			Network.Destroy(gameObj);
		} else {
			Destroy(gameObj);
		}
	}
	
	private void attachHealingAnimationTo(GameObject gameObj) {
		GameObject newHealingAnimation;
		if (Network.isClient || Network.isServer) {
			newHealingAnimation = (GameObject) Network.Instantiate(healingAnimation, transform.position, Quaternion.identity, 0);
		} else {
			newHealingAnimation = (GameObject) GameObject.Instantiate(healingAnimation, transform.position, Quaternion.identity);
		}
		newHealingAnimation.name = HEALING_ANIMATION_NAME;
		newHealingAnimation.transform.parent = gameObj.transform;
		newHealingAnimation.transform.localScale = new Vector3(1,1,1);
		newHealingAnimation.transform.localPosition = new Vector3(
			0,
			-0.08f,
			-2
		);
	}
	
	protected Anthill getNearbyAnthill(Vector2 position) {
		// Get all anthills within a certain range
		Collider2D[] nearbyAnthills = Physics2D.OverlapCircleAll(position, anthillRange, anthillMask);
		for (int i = 0; i < nearbyAnthills.Length; i++) {
			// If we found an anthill, make sure its ours
			Anthill anthill = nearbyAnthills[i].gameObject.GetComponent<Anthill>();
			if (anthill.isNeutralOrFriendly()) return anthill;
		}
		
		// If we couldn't find a friendly anthill within range, return null
		return null;
	}
	
	// Given a coordinate position on the map, determines whether the current unit is allowed to move to the tile
	protected bool canWalkTo(Vector2 position) {
		Collider2D[] mapObjects = Physics2D.OverlapPointAll(position);
		foreach (Collider2D mapObj in mapObjects) {
			if (!canWalkOn(mapObj.gameObject) && mapObj.gameObject != this.gameObject && mapObj.gameObject != this.currentTile.gameObject) {
				return false;
			}
		}
		return true;
	}
	
	// Given a specific game object, determine whether the current unit is allowed to be on the same tile as this object
	// By default, Ant Units can only walk on tiles
	protected virtual bool canWalkOn(GameObject gameObj) {
		return (gameObj.GetComponent<Tile>() != null && !gameObj.GetComponent<Tile>().occupiedBy) || 
			    gameObj.GetComponent<Scentpath>() != null;
	}
	
	/**
	 * Must use this when setting a new path in order to avoid loosing reference to the targetPath object
	 * (it's destructor method won't be called if the reference is changed, so the previous path may never be
	 * "deselected" if the reference is disconnected)
	 */
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
		
		public int getCount() {
			return priorityQueue.Count;
		}
	}
	
	/**
	 * Some factory-esque methods to utilize the unique id of ScriptableObject's GetInstanceID
	 */
	protected Path getNewPath() {
		return (Path) ScriptableObject.CreateInstance("Path");
	}
	
	protected Path getNewPath(Tile startingTile, float startingHeuristic, float startingCost) {
		Path path = (Path) ScriptableObject.CreateInstance("Path");
		path.init(startingTile, startingHeuristic, startingCost);
		return path;
	}
	
	protected Path getNewPath(Path originalPath) {
		Path path = (Path) ScriptableObject.CreateInstance("Path");
		path.init(originalPath);
		return path;
	}
}
