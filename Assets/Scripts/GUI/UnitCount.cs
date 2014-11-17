using UnityEngine;
using System.Collections;

public class UnitCount : Ownable {

	private bool setupSuccess = false;
	private int gathererCount = 0;
	private int warriorCount = 0;
	private int scoutCount = 0;
	private int queenCount = 0;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		setup();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
		if (!setupSuccess) setup();
		countUnits();
		updateCounts();
	}
	
	private void setup() {
		if (!getTextureFromPlayer("gathererHead")) return;
		transform.Find("gathererHead").gameObject.GetComponent<GUITexture>().texture = getTextureFromPlayer("gathererHead");
		transform.Find("warriorHead").gameObject.GetComponent<GUITexture>().texture = getTextureFromPlayer("warriorHead");
		transform.Find("scoutHead").gameObject.GetComponent<GUITexture>().texture = getTextureFromPlayer("scoutHead");
		transform.Find("queenHead").gameObject.GetComponent<GUITexture>().texture = getTextureFromPlayer("queenHead");
		setupSuccess = true;
	}
	
	private void countUnits() {
		gathererCount = 0;
		warriorCount = 0;
		scoutCount = 0;
		queenCount = 0;
		GameObject[] ants = GameObject.FindGameObjectsWithTag("AntUnit");
		foreach (GameObject ant in ants) {
			if (!ant.GetComponent<Ownable>().isNeutralOrFriendly()) continue;
			if (ant.GetComponent<GathererUnit>()) gathererCount++;
			if (ant.GetComponent<WarriorUnit>()) warriorCount++;
			if (ant.GetComponent<ScoutUnit>()) scoutCount++;
			if (ant.GetComponent<QueenUnit>()) queenCount++;
		}
	}
	
	private void updateCounts() {
		transform.Find("gathererHead").Find("count").gameObject.GetComponent<PlaceTextFromCorner>().text = "x" + gathererCount.ToString();
		transform.Find("warriorHead").Find("count").gameObject.GetComponent<PlaceTextFromCorner>().text = "x" + warriorCount.ToString();
		transform.Find("scoutHead").Find("count").gameObject.GetComponent<PlaceTextFromCorner>().text = "x" + scoutCount.ToString();
		transform.Find("queenHead").Find("count").gameObject.GetComponent<PlaceTextFromCorner>().text = "x" + queenCount.ToString();
	}
}
