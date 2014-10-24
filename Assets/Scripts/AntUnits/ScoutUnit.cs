using UnityEngine;
using System.Collections;

public class ScoutUnit : AntUnit {

	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
	}
	
	protected override void loadSprite() {
		gameObject.GetComponent<SpriteRenderer>().sprite = player.scoutSprite;
	}
}
