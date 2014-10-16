using UnityEngine;
using System.Collections;

public class WarriorUnit : AntUnit {

	public GameObject combatCloud;
	
	private AntUnit attackTarget;
	private Tile attackTargetLastKnowTileLocation;

	// Use this for initialization
	protected override void Start() {
		base.Start();
	}
	
	//To be displayed on the GUI
	public override string description
	{
		get
		{
			return "Warriors can attack other units";
		}
		set
		{
		}
	}
	// Update is called once per frame
	protected override void Update() {
		base.Update();
		
		// Base our movement/pathfinding off of our attack target (if any)
		if (attackTarget != null) {
			Tile attackTargetCurrentLocation = mapManager.getTileAtPosition(attackTarget.transform.position);
			
			// If we have landed on the target, commence a 'battle', otherwise keep moving towards the target
			if (mapManager.getTileAtPosition(transform.position) == attackTargetCurrentLocation) {
				Debug.Log("Found target!");
				StartCoroutine(commenceBattle(attackTarget));
				clearAttackTarget();
			} else if (targetPath.getTilePath().Count == 0 || attackTargetLastKnowTileLocation != attackTargetCurrentLocation) {
				Debug.Log("Calculating route to target!");
				StartCoroutine(moveToTarget(attackTargetCurrentLocation));
			}
		}
	}
	
	public IEnumerator commenceBattle(AntUnit opponent) {
		// If we are already in a battle, we should abort the current commenceBattle request
		Debug.Log(isInBattle);
		if (isInBattle) yield break;
		Debug.Log("Commencing attack!");
		
		// Set both units to be in 'attack mode' and generate a 'combat cloud'
		isInBattle = true;
		opponent.startBattle();
		Vector3 pos = mapManager.getTileAtPosition(transform.position).transform.position;
		GameObject cloud = GameObject.Instantiate(combatCloud, 
												  new Vector3(pos.x, pos.y, transform.position.z - 1), 
												  Quaternion.identity) as GameObject;
		gameObject.renderer.enabled = false;
		opponent.gameObject.renderer.enabled = false;
		
		// Every second, battle calculations should take place
		while (true) {
			// Do one round of battle calculations
			exchangeBlows(this, opponent);
			
			// Wait for 1 seconds
			Debug.Log("AntOne: " + this.currentHp + ", AntTwo: " + opponent.currentHp);
			yield return new WaitForSeconds(1);
			
			// Check if one of the units has died
			if (this.currentHp <= 0 || opponent.currentHp <= 0) break;
		}
		
		// After the battle, 'cleanup' the unit(s) and the combat cloud
		GameObject.Destroy(cloud);
		gameObject.renderer.enabled = true;
		opponent.gameObject.renderer.enabled = true;
		this.removeFromBattle();
		opponent.removeFromBattle();
		if (opponent.currentHp <= 0) opponent.kill();
		// Note: if this unit dies, make sure to delete it at the end!
		if (this.currentHp <= 0) this.kill();
	}
	
	private void exchangeBlows(AntUnit antOne, AntUnit antTwo) {
		// Calculate each ant's attack based on their remaining health and base attack stat
		Debug.Log(antOne.currentHp / antOne.maxHp);
		float antOneAttack = antOne.attack * (antOne.currentHp / antOne.maxHp); //TODO: dynamic attack calculations
		float antTwoAttack = antTwo.attack * (antTwo.currentHp / antTwo.maxHp); //TODO: dynamic attack calculations
		
		// Calculate damage dealt by factoring in eachothers defenses and apply to current hp
		antOne.currentHp -= Mathf.Max(Random.Range(antTwoAttack*0.5f, antTwoAttack*1.5f) - antOne.defense, 0); //TODO: use random value in range
		antTwo.currentHp -= Mathf.Max(Random.Range(antOneAttack*0.5f, antOneAttack*1.5f) - antTwo.defense, 0); //TODO: use random value in range
	}
	
	// If a warrior comes in to contact with it's target, interrupt its movement so that 
	// they will eventually overlap to start a 'battle'
	void OnTriggerEnter2D(Collider2D other) {
		if (attackTarget != null && other.gameObject.GetComponent<AntUnit>() == attackTarget) {
			Debug.Log("Collided with target!");
			attackTarget.interrupt();
		}
	}
	
	public void setTarget(AntUnit target) {
		Debug.Log("New target: " + target.ToString());
		// Store data regarding our new target
		attackTarget = target;
		attackTargetLastKnowTileLocation = mapManager.getTileAtPosition(target.transform.position);
	}
	
	private void clearAttackTarget() {
		attackTarget = null;
		attackTargetLastKnowTileLocation = null;
	}
	
	// If a warrior is given a move command, we need to clear the attack target or else the unit will keep moving towards teh target
	public override IEnumerator moveTo(Tile tileToMoveTo) {
		clearAttackTarget();
		return base.moveTo(tileToMoveTo);
	}
	
	// Move to a target (without clearing attack target like moveTo)
	private IEnumerator moveToTarget(Tile tileToMoveTo) {
		return base.moveTo(tileToMoveTo);
	}
	
	// Warriors can walk on tiles and food items (but only if they aren't already carrying food themselves)
	protected override bool canWalkOn(GameObject gameObj) {
		// If it is a tile or an ant unit (friendly or not), we can walk on it
		// Since food can be carried by units, check for that too
		return gameObj.GetComponent<Tile>() != null || 
			   gameObj.GetComponent<AntUnit>() != null || 
			   (gameObj.transform.parent != null && gameObj.transform.parent.GetComponent<AntUnit>() != null);
	}
}
