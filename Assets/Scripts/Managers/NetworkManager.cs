using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	//Name MUST be unique on master server
	private const string typeName = "ColoniesAntBattle";
	private string gameName = CreateGameServer.gameName;
	private HostData hostGame = JoinGame.hostGame;
	private int lastLevelPrefix = 0;
	
	private void Start () {
		if (hostGame == null) {
			StartServer();
		} else {
			Network.Connect(hostGame);
		}
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
		Debug.Log("Player joined");
	}
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		
	}
}
