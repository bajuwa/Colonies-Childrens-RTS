using UnityEngine;
using System.Collections;

public class Ownable : MonoBehaviour {
	
	private PlayerManager playerManager;
	
	// Sets ownership to determine allied vs enemy vs neutral objects
	// 0 is neutral, 1 and 2 are their respective player ids
	// TODO: privatize once done dev
	public int ownedBy = 0;
	private Player player;
	private Player opponentPlayer;
	
	// Variable that gets the asset as a 2D texture
	public Texture2D displayImage;

	// Use this for initialization
	protected virtual void Start () {
		playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (!player) loadPlayerScript(ownedBy);
	}
	
	public bool isNeutralOrFriendly() {
		if (playerManager == null) playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
		return (ownedBy == 0 || ownedBy == playerManager.myPlayerId); 
	}
	
	public int getPlayerId() {
		if (player.id == null)
		{
			if (Network.isServer) return 1;
			else if (Network.isClient) return 2;
		}
		else return player.id;
	}
	
	public void setAsMine(int playerId) {
		ownedBy = playerId;
	}
	
	protected RuntimeAnimatorController getAnimatorFromPlayer(string name) {
		if (!player) return null;
		return player.getAnimator(name);
	}
	
	protected Sprite getSpriteFromPlayer(string name) {
		if (!player) return null;
		return player.getSprite(name);
	}
	
	protected Sprite getSpriteFromOpponent(string name) {
		if (!opponentPlayer) return null;
		return opponentPlayer.getSprite(name);
	}
	
	protected Texture2D getTextureFromPlayer(string name) {
		if (!player) return null;
		return player.getTexture(name);
	}
	
	protected GameObject getGameObjectFromPlayer(string name) {
		if (!player) return null;
		return player.getGameObject(name);
	}
	
	private void loadPlayerScript(int playerId) {
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player")) {
			Player pScript = obj.GetComponent<Player>();
			if (pScript.getId() == playerId) {
				Debug.Log("Loading script from player: " + pScript.getId());
				player = pScript;
				break;
			} else {
				opponentPlayer = pScript;
			}
		}
	}
	
	protected virtual void loadSprite() {}
	
	protected virtual void loadDisplayImage() {}
}
