using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * The AntUnit with the unique ability to walk on top of food in order to pick them up.
 * Once food is picked up, they will move at half their speed, and they can press a small
 * button displayed above them in order to drop their food
 */
public class GathererUnit : AntUnit {

	private bool droppedFood = true;
	private GameObject dropOffAtAnthill;
	
	//To be displayed on the GUI
	public override string getDescription() {
		if (isNeutralOrFriendly())
			return "Able to pick up fruit and carry it back to your Anthill.";
		else
			return "Helps your enemy's colony grow larger if it can find food!";
	}
	
	public override string getName() {
		return "Gatherer";
	}
	
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
		// If the next tile in our path has an anthill we are scheduled to drop food at, stop and drop food
		if (getTargetTile()) {
			Collider2D[] colliders = Physics2D.OverlapPointAll(getTargetTile().transform.position);
			bool foundAnthill = false;
			foreach (Collider2D col in colliders) {
				if (col.gameObject == dropOffAtAnthill) foundAnthill = true;
			}
			if (foundAnthill) {
				Debug.Log("Auto-dropping food off at the anthill");
				resetTargetTile();
				Queue<Tile> goBackToCurrent = new Queue<Tile>();
				goBackToCurrent.Enqueue(getCurrentTile());
				targetPath.setNewTileQueue(goBackToCurrent);
				dropFood();
			}
		}
		
		// Leave the movement up to the AntUnit class
		base.Update();
		
		// If we have stopped moving and have landed on a food item, pick it up if we aren't already carrying some
		if (!droppedFood && this.gameObject.GetComponentInChildren<Food>() == null && getCurrentTile() == getTargetTile() && targetPath != null && targetPath.getTilePath().Count == 0) {
			Collider2D[] itemsOnSameTile = Physics2D.OverlapPointAll(transform.position);
			foreach (Collider2D col in itemsOnSameTile) {
				if (col.gameObject.GetComponent<Food>() != null) {
						Debug.Log("Detected food on current tile, picking up V.Offline");
						pickUpFood(col.gameObject);
				}
			}
		}
		
		// Determine what animation we should be playing
		if (GetComponentsInChildren<Food>().Length > 0) setAnimation(2);
		else if (getCurrentTile() != getTargetTile()) setAnimation(1);
		else setAnimation(0);
		
		// If the player is moving, then clear our 'dropped food' flag so that the unit can pick up food again
		if (getCurrentTile() != getTargetTile()) droppedFood = false;
	}
	
	protected override void loadDisplayImage() {
		if (currentHp/maxHp <= .33f) {
			displayImage = getTextureFromPlayer("gathererDisplayDying");
		} else if (currentHp/maxHp <= .66f) {
			displayImage = getTextureFromPlayer("gathererDisplayDamaged");
		} else {
			displayImage = getTextureFromPlayer("gathererDisplayHealthy");
		}
	}
	
	public override Sprite getFightSprite() {
		return getSpriteFromPlayer("gathererSprite");
	}
	
	protected override string getAnimationName() {
		return "gathererAnimator";
	}
	
	// If a gatherer is given a move command to an anthill while holding fruit, we need to set the drop off variable
	public override IEnumerator moveTo(Tile tileToMoveTo, bool activelySetNewTarget = false) {
		if (this.gameObject.GetComponentInChildren<Food>() != null) {
			Debug.Log("Have food, going to drop off at home");
			Anthill anthill = getNearbyAnthill(tileToMoveTo.transform.position);
			if (anthill && anthill.isNeutralOrFriendly()) dropOffAtAnthill = anthill.gameObject;
		}
		return base.moveTo(tileToMoveTo, activelySetNewTarget);
	}
	
	public void setDropOffAtAnthill(GameObject obj) {
		dropOffAtAnthill = obj;
	}
	
	public void pickUpFood(GameObject gameObj) {
		if (Network.isServer || Network.isClient) networkView.RPC("networkPickUpFood", RPCMode.All, gameObj.networkView.viewID);
		
		else {
			// Set the parent to our unit so that it is 'carried' when the unit is moving
			gameObj.transform.parent = this.gameObject.transform;
		
			// Also set the local position's z value to -1 to ensure it is visible above the unit
			Vector3 tempPos = gameObj.transform.localPosition;
			tempPos.y += 0.8f; 
			tempPos.z = -1f; 
			gameObj.transform.localPosition = tempPos;
			gameObj.transform.localScale = new Vector3(1,1,1);
		
			// Re-enable the selectable script so that we can select it again
			gameObj.GetComponent<Selectable>().enabled = false;
			gameObj.GetComponent<CircleCollider2D>().enabled = false;
		
			// When gatherers are carrying food, they move at half their original speed
			speed /= 2f;
		}
	}
	[RPC] void networkPickUpFood(NetworkViewID netViewIDGameObj) {
		GameObject gameObj = NetworkView.Find(netViewIDGameObj).gameObject;
		gameObj.transform.parent = this.gameObject.transform;
		Vector3 tempPos = gameObj.transform.localPosition;
		tempPos.y += 0.8f;
		tempPos.z = -1f;
		gameObj.transform.localPosition = tempPos;
		gameObj.transform.localScale = new Vector3(1,1,1);
		
		gameObj.GetComponent<Selectable>().enabled = false;
		gameObj.GetComponent<CircleCollider2D>().enabled = false;
		speed /= 2f;
	}
	public void dropFood() {
		if (Network.isServer || Network.isClient) networkView.RPC("networkDropFood", RPCMode.All);
		else {
			// To avoid automatically picking the food back up again, set a flag
			droppedFood = true;
			dropOffAtAnthill = null;
		
			// When gatherers are carrying food, they move at half their original speed, so when they drop food, fix the speed stat
			speed *= 2f;
		
			// Reassign the food back to the 'Objects' sprite in the map
			Transform foodTransform = this.gameObject.GetComponentInChildren<Food>().gameObject.transform;
			foodTransform.parent = GameObject.Find("Objects").transform;
		
			// Make sure when dropped it snaps to the appropriate tile
			foodTransform.position = mapManager.getTileAtPosition(foodTransform.position).transform.position;
		
			// Check to see if the new position would be in range of a friendly anthill
			Anthill anthill = getNearbyAnthill(foodTransform.position);
			if (anthill) {
				anthill.addFoodPoints(foodTransform.gameObject.GetComponent<Food>().getFoodValue());
				Destroy(foodTransform.gameObject);
				return;
			}
		
			// Reset the z back to 0 to force the food back underneath the unit
			Vector3 tempPos = foodTransform.localPosition;
			tempPos.z = 0;
			foodTransform.localPosition = tempPos;
		
			// Disable the selectable script so that it doesn't interfere with selecting the underlying unit
			foodTransform.gameObject.GetComponent<Selectable>().enabled = true;
			foodTransform.gameObject.GetComponent<Collider2D>().enabled = true;
		}
	}
	[RPC] void networkDropFood() {
		// To avoid automatically picking the food back up again, set a flag
			droppedFood = true;
			dropOffAtAnthill = null;
		
			// When gatherers are carrying food, they move at half their original speed, so when they drop food, fix the speed stat
			speed *= 2f;
		
			// Reassign the food back to the 'Objects' sprite in the map
			Transform foodTransform = this.gameObject.GetComponentInChildren<Food>().gameObject.transform;
			foodTransform.parent = GameObject.Find("Objects").transform;
		
			// Make sure when dropped it snaps to the appropriate tile
			foodTransform.position = mapManager.getTileAtPosition(foodTransform.position).transform.position;
		
			// Check to see if the new position would be in range of a friendly anthill
			Anthill anthill = getNearbyAnthill(foodTransform.position);
			if (anthill) {
				anthill.addFoodPoints(foodTransform.gameObject.GetComponent<Food>().getFoodValue());
				Network.Destroy(foodTransform.gameObject);
				return;
			}
		
			// Reset the z back to 0 to force the food back underneath the unit
			Vector3 tempPos = foodTransform.localPosition;
			tempPos.z = 0;
			foodTransform.localPosition = tempPos;
		
			// Disable the selectable script so that it doesn't interfere with selecting the underlying unit
			foodTransform.gameObject.GetComponent<Selectable>().enabled = true;
			foodTransform.gameObject.GetComponent<Collider2D>().enabled = true;
	}
	
	// Gatherers can walk on tiles and food items (but only if they aren't already carrying food themselves)
	protected override bool canWalkOn(GameObject gameObj) {
		// If this object is a child of us, we can safely ignore it
		if (gameObj.transform.parent == transform) return true;
		
		// If it is the anthill we are commanded to drop food off at, we can (sort of) walk on it
		if (gameObj == dropOffAtAnthill) return true;
		
		// If it is an unoccupied tile, we can walk on it
		if ((gameObj.GetComponent<Tile>() != null && !gameObj.GetComponent<Tile>().occupiedBy) ||
			gameObj.GetComponent<Scentpath>() != null) {
			return true;
		}
		
		// If it isn't a tile, and also isn't food, then we can't walk on it
		if (gameObj.GetComponent<Food>() == null) return false;
		
		// If it is food, we can only walk on it if we aren't already carrying food
		if (this.gameObject.GetComponentInChildren<Food>() != null) return false;
		
		// If none of the above conditions are met, we can safely walk on it
		return true;
	}
}
