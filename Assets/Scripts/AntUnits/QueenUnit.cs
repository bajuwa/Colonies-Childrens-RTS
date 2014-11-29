using UnityEngine;

public class QueenUnit : AntUnit {

	public GameObject anthillPrefab;
	//private NetworkManager netMan;
	//To be displayed on the GUI
	private NetworkManager netMan;
	public override string getDescription() {
		if (isNeutralOrFriendly()) 
			return "The Queen can be used to create one new Anthill on top of old Anthill ruins!";
		else
			return "Oh no!  The enemy's Queen is trying to build a new Anthill for them, stop her!";
	}
	
	public override string getName() {
		return "Queen";
	}
	
	// Use this for initialization
	protected override void Start() {
		base.Start();
		Debug.Log(this.ownedBy);
	}
	
	protected override void loadDisplayImage() {
		if (currentHp/maxHp <= .33f) {
			displayImage = getTextureFromPlayer("queenDisplayDying");
		} else if (currentHp/maxHp <= .66f) {
			displayImage = getTextureFromPlayer("queenDisplayDamaged");
		} else {
			displayImage = getTextureFromPlayer("queenDisplayHealthy");
		}
	}
	
	public override Sprite getFightSprite() {
		return getSpriteFromPlayer("queenSprite");
	}
	
	protected override string getAnimationName() {
		return "queenAnimator";
	}
	
	// Update is called once per frame
	protected override void Update() {
		base.Update();
		Debug.Log("Update1");
		if (!netMan && GameObject.Find("NetworkManager")) netMan = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		// If our queen lands on a DeadAnthill, create a new Anthill
		if (getCurrentTile() == getTargetTile() && targetPath != null && targetPath.getTilePath().Count == 0) {
			if (networkView.isMine) Debug.Log("Update2");
			Collider2D[] itemsOnSameTile = Physics2D.OverlapPointAll(transform.position);
			Debug.Log("Update3");
			foreach (Collider2D col in itemsOnSameTile) {
				Debug.Log("Update4");
				if (col.gameObject.GetComponent<DeadAnthill>() != null) {
					Debug.Log("Update5");
					Debug.Log("Detected dead anthill on current tile, creating new anthill");
					if (Network.isServer || Network.isClient) {
						if (networkView.isMine) createNewAnthill(col.gameObject);
						}
					else {
						createNewAnthill(col.gameObject);
					}
				}
			}
		}
		
		// Determine what animation we should be playing
		if (getCurrentTile() != getTargetTile()) setAnimation(1);
		else setAnimation(0);
	}
	
	private void createNewAnthill(GameObject deadAnthillObject) {
		if (Network.isServer || Network.isClient) {
			GameObject anthillObject = Network.Instantiate(anthillPrefab, 
				mapManager.getTileAtPosition(transform.position).gameObject.transform.position,
				Quaternion.identity,
				0) as GameObject;
			anthillObject.GetComponent<Ownable>().ownedBy = this.ownedBy;
			netMan.changeInstant(anthillObject, "Object");
			if (Network.isClient) netMan.changeID(anthillObject);
			//Network.Destroy(deadAnthillObject.networkView.viewID);
			Network.Destroy(this.networkView.viewID);
			Network.Destroy(deadAnthillObject);
			//deadAnthillObject.networkView.viewID = Network.AllocateViewID();
			//networkView.RPC("killDeadAnthill", RPCMode.All, deadAnthillObject.networkView.viewID);
		}
		else {
			GameObject anthillObject = GameObject.Instantiate(
				anthillPrefab,
				mapManager.getTileAtPosition(transform.position).gameObject.transform.position,
				Quaternion.identity
			) as GameObject;
			anthillObject.GetComponent<Ownable>().ownedBy = this.ownedBy;
			anthillObject.transform.parent = GameObject.Find("Objects").transform;
			anthillObject.transform.localPosition = new Vector3(
				anthillObject.transform.localPosition.x,
				anthillObject.transform.localPosition.y,
				0
			);
			// Delete both the queen unit and the dead anthill
			GameObject.Destroy(deadAnthillObject);
			this.kill();
		}
	}
	[RPC] void killDeadAnthill(NetworkViewID deadAntHillViewID){
		GameObject.Destroy(NetworkView.Find(deadAntHillViewID).gameObject);
	}
	// Warriors can walk on tiles and food items (but only if they aren't already carrying food themselves)
	protected override bool canWalkOn(GameObject gameObj) {
		if (gameObj.GetComponent<Scentpath>() != null) return true;
		
		if (gameObj.GetComponent<Tile>() != null) {
			if (gameObj.GetComponent<Tile>().occupiedBy != null) return false;
			return true;
		}
		
		if (gameObj.GetComponent<DeadAnthill>() != null) return true;
		
		return false;
	}
}
