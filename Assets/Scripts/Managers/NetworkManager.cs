using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	//Name MUST be unique on master server
	private const string typeName = "ColoniesAntBattle";
	private string gameName = CreateGameServer.gameName;
	
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
	[RPC] void OnPlayerConnected() {
		NetworkLevelLoader.Instance.LoadLevel("MultiPlayerGame");
	}
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		
	}
}
