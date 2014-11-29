using UnityEngine;
using System.Collections;

public class Attackable : Selectable {
	protected bool isInBattle = false;
	
	// Unit Stats (TODO: protect once done dev testing)
	// When changing stats between different unit types, change them in the Prefab, not in any classes
	public float currentHp = 10f;
	public float maxHp = 10f;
	public float attack = 1f;
	public float defense = 2f;
	
	// Destroy this unit, making sure to destroy paths and selections
	public void kill() {
		Debug.Log("I was killed!");
		this.deselect(GetInstanceID());
		if (Network.isClient || Network.isServer) {
			Network.Destroy(this.gameObject);
		}
		else {
		GameObject.Destroy(this.gameObject);
		}
	}
	
	public virtual void startBattle() {
		Debug.Log("Another unit attacked me!");
	}
	
	public virtual void removeFromBattle() {
		Debug.Log("Done battling");
	}
	
	public virtual void interrupt() {
		Debug.Log("Interrupted!");
	}
	
	public virtual Sprite getFightSprite() {
		return null;
	}

}
