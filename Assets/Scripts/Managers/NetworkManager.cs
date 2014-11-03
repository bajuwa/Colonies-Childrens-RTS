using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	//Name MUST be unique on master server
	private const string typeName = "ColoniesAntBattle";
	private string gameName = CreateGameServer.gameName;
	private int lastLevelPrefix = 0;
	private void Start () {
		StartServer();
	}
	// Use this for initialization
	void StartServer () {
		Network.InitializeServer(2, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}
	
	//Messages

	
	void OnMasterServerEvent(MasterServerEvent mse) {
		if(mse == MasterServerEvent.RegistrationSucceeded) {
			Debug.Log("Registered");
		}
	}
	void OnPlayerConnected() {
		networkView.RPC("LoadLevel", RPCMode.All, "MultiPlayerGame", lastLevelPrefix + 1);
	}
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		
	}
	[RPC]
	public void LoadLevel(string level, int levelPrefix){
		StartCoroutine(loadLevel(level, levelPrefix));
	}
	private IEnumerator loadLevel (string level, int levelPrefix) {
		lastLevelPrefix = levelPrefix;
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
