﻿using UnityEngine;
using System.Collections;

public class ScoutUnit : AntUnit {

	private GameObject scentpath;
	private GameObject scentpathParent;
	
	//To be displayed on the GUI
	public override string getDescription() {
		if (isNeutralOrFriendly())
			return "Automatically builds scent paths as they walk that help your other units move faster.";
		else
			return "Builds scent paths that help your enemy move faster!";
	}
	
	public override string getName() {
		return "Scout";
	}

	// Use this for initialization
	protected override void Start () {
		base.Start();
		loadScentpath();
		loadScentpathParent();
	}
	
	private void loadScentpath() {
		scentpath = getGameObjectFromPlayer("scentpathGameObject");
	}
	
	private void loadScentpathParent() {
		scentpathParent = GameObject.Find("Objects");
	}
	
	protected override void loadAnimator() {
		if (animator) return; 
		Debug.Log("Loading animator");
		animator = gameObject.AddComponent("Animator") as Animator;
		animator.runtimeAnimatorController = getAnimatorFromPlayer("scoutAnimator");
	}
	
	protected override void loadDisplayImage() {
		displayImage = getTextureFromPlayer("scoutDisplay");
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
		if (!scentpath) loadScentpath();
		if (!scentpathParent) loadScentpathParent();
		
		// If the current tile we are moving to does not have our scent path on it, apply it
		Scentpath path = mapManager.getScentpathAtPosition(transform.position);
		if (path) {
			// If a scentpath exists, and it isn't ours, replace it with our own
			if (path.getPlayerId() != this.getPlayerId()) {
				Destroy(path.gameObject);
			}
		} else {
			// Otherwise, create and place one
			if (!scentpath) return;
			GameObject newScentpath = (GameObject) Object.Instantiate(
				scentpath, 
				mapManager.getTileAtPosition(transform.position).transform.position, 
				Quaternion.identity
			);
			newScentpath.transform.parent = scentpathParent.transform;
			newScentpath.transform.localPosition = new Vector3(
				newScentpath.transform.localPosition.x,
				newScentpath.transform.localPosition.y,
				0
			);
			newScentpath.GetComponent<Ownable>().setAsMine(getPlayerId());
		}
		
		// Determine what animation we should be playing
		if (getCurrentTile() != getTargetTile()) animator.SetInteger("STATE", 1);
		else animator.SetInteger("STATE", 0);
	}
}
