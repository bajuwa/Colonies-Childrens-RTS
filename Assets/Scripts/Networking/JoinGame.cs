using UnityEngine;
using System.Collections;

public class JoinGame : MonoBehaviour {
	private bool refreshing;
	private HostData[] hostData;
	public static HostData hostGame;
	public int buttonWidth = 400;
	public int buttonHeight = 50;
	// Use this for initialization
	void Start () {
		refreshHostList();
	}
	void refreshHostList(){
		MasterServer.RequestHostList("ColoniesAntBattle");
		refreshing = true;
	}
	/*
	void OnFailedToConnect(NetworkConnectionError error) {
		Debug.Log("Could not connect to server: " + error);
	}
	*/
	// Update is called once per frame
	void Update () {
		//generates the list of ongoing games
		if(refreshing){
			if(MasterServer.PollHostList().Length > 0) {
				refreshing = false;
				hostData = MasterServer.PollHostList();
			}
		}
	}
	void OnGUI(){
		if(hostData != null){
			for (int i = 0; i<hostData.Length; i++) {
				if(GUI.Button(new Rect(Screen.width/2 - buttonWidth/2, 100+(buttonHeight*i), buttonWidth, buttonHeight), hostData[i].gameName)){
					hostGame = hostData[i];
					Application.LoadLevel(int.Parse(hostGame.comment));
					//Network.Connect(hostData[i]);
				}
			} 
		} 
	}
	[RPC]
	public void LoadLevel(string level, int levelPrefix){
		StartCoroutine(loadLevel(level, levelPrefix));
	}
	private IEnumerator loadLevel (string level, int levelPrefix) {
	
		Debug.Log("Loading level " + level + " with prefix " + levelPrefix);
		// There is no reason to send any more data over the network on the default channel,
		// because we are about to load the level, thus all those objects will get deleted anyway
		Network.SetSendingEnabled(0, false);
		// We need to stop receiving because first the level must be loaded.
		// Once the level is loaded, RPC's and other state update attached to objects in the level are allowed to fire
		Network.isMessageQueueRunning = false;
		// All network views loaded from a level will get a prefix into their NetworkViewID.
		// This will prevent old updates from clients leaking into a newly created scene.
		Network.SetLevelPrefix(levelPrefix);
		Application.LoadLevel(level);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Debug.Log("Loading complete");
		Debug.Log("load level DONE");
		// Allow receiving data again
		Network.isMessageQueueRunning = true;
		// Now the level has been loaded and we can start sending out data
		Network.SetSendingEnabled(0, true);
		Debug.Log("sending load msg");
		// Notify our objects that the level and the network is ready
		foreach (GameObject go in FindObjectsOfType(typeof(GameObject)) ){
			Debug.Log("sending load msg");
			go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
		}
	}
}
