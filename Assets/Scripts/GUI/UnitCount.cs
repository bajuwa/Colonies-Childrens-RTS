using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitCount : Ownable {

	private bool setupSuccess = false;
	
	private List<GameObject> gatherers;
	private int gathererIndex = 0;
	
	private List<GameObject> warriors;
	private int warriorIndex = 0;
	
	private List<GameObject> scouts;
	private int scoutIndex = 0;
	
	private List<GameObject> queens;
	private int queenIndex = 0;
	
	private int totalCount = 0;
	private int unitCap = 0;
	
	public void centerOnNext(string name) {
		switch (name) {
			case "gatherer":
				if (gatherers.Count == 0) return;
				if (gathererIndex >= gatherers.Count) gathererIndex = 0;
				centerOn(gatherers[gathererIndex++]);
				break;
			case "warrior":
				if (warriors.Count == 0) return;
				if (warriorIndex >= warriors.Count) warriorIndex = 0;
				centerOn(warriors[warriorIndex++]);
				break;
			case "scout":
				if (scouts.Count == 0) return;
				if (scoutIndex >= scouts.Count) scoutIndex = 0;
				centerOn(scouts[scoutIndex++]);
				break;
			case "queen":
				if (queens.Count == 0) return;
				if (queenIndex >= queens.Count) queenIndex = 0;
				centerOn(queens[queenIndex++]);
				break;
			default:
				Debug.Log("Invalid name given to center on!");
				break;
		}
	}

	// Use this for initialization
	protected override void Start () {
		base.Start();
		gatherers = new List<GameObject>();
		warriors = new List<GameObject>();
		scouts = new List<GameObject>();
		queens = new List<GameObject>();
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
		gatherers = new List<GameObject>();
		warriors = new List<GameObject>();
		scouts = new List<GameObject>();
		queens = new List<GameObject>();
		GameObject[] ants = GameObject.FindGameObjectsWithTag("AntUnit");
		foreach (GameObject ant in ants) {
			if (!ant.GetComponent<Ownable>().isNeutralOrFriendly()) continue;
			if (ant.GetComponent<GathererUnit>()) gatherers.Add(ant);
			if (ant.GetComponent<WarriorUnit>()) warriors.Add(ant);
			if (ant.GetComponent<ScoutUnit>()) scouts.Add(ant);
			if (ant.GetComponent<QueenUnit>()) queens.Add(ant);
		}
		totalCount = gatherers.Count + warriors.Count + scouts.Count + queens.Count;
		unitCap = 5 + (playerManager.getTotalAnthillCount() * 5);
	}
	
	private void updateCounts() {
		transform.Find("gathererHead").Find("count").gameObject.GetComponent<PlaceTextFromCorner>().text = "x" + gatherers.Count.ToString();
		transform.Find("warriorHead").Find("count").gameObject.GetComponent<PlaceTextFromCorner>().text = "x" + warriors.Count.ToString();
		transform.Find("scoutHead").Find("count").gameObject.GetComponent<PlaceTextFromCorner>().text = "x" + scouts.Count.ToString();
		transform.Find("queenHead").Find("count").gameObject.GetComponent<PlaceTextFromCorner>().text = "x" + queens.Count.ToString();
		transform.Find("TotalUnitCount").gameObject.GetComponent<PlaceTextFromCorner>().text = "Total: " + totalCount + "/" + unitCap;
	}
	
	private void centerOn(GameObject obj) {
		Camera.main.transform.position = new Vector3(
			obj.transform.position.x,
			obj.transform.position.y,
			Camera.main.transform.position.z
		);
	}
}
