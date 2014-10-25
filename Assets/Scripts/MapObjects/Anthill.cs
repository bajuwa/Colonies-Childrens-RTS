using UnityEngine;
using System.Collections;

public class Anthill : Selectable {

	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
	}
	
	protected override void loadSprite() {
		gameObject.GetComponent<SpriteRenderer>().sprite = getSpriteFromPlayer("anthillSprite");
	}
	
	protected override void loadDisplayImage() {
		displayImage = getTextureFromPlayer("anthillDisplay");
	}
}
