using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSetupManager : MonoBehaviour {

	public List<Map> maps;
	private int mapIndex = 0;

	// Use this for initialization
	void Start () {
		updateMapSettings();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void changeMap(int modifyBy) {
		mapIndex += modifyBy;
		if (mapIndex > maps.Count - 1) mapIndex = 0;
		if (mapIndex < 0) mapIndex = maps.Count - 1;
		updateMapSettings();
	}
	
	private void updateMapSettings() {
		transform.Find("MapName").GetComponent<GUIText>().text = maps[mapIndex].name;
		transform.Find("MapSelection").GetComponent<SpriteRenderer>().sprite = maps[mapIndex].scenePreview;
		GameObject.Find("HostGame").GetComponent<ClickToLoadNextScene>().nextScene = (ClickToLoadNextScene.sceneName) maps[mapIndex].sceneId;
		GameObject.Find("MapSelectionNetworking").GetComponent<MapSelectionScene>().sceneIndex = maps[mapIndex].sceneId;
	}
	
	[System.Serializable]
	public class Map {
		public string name;
		public Sprite scenePreview;
		public int sceneId;
	}
}
