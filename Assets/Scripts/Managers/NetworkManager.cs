using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	//Name MUST be unique on master server
	private const string typeName = "ColoniesAntBattle";
	private const string gameName = "RoomName";
	
	// Use this for initialization
	void StartServer () {
		Network.InitializeServer(2, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}
	
	void OnServerInitialized() {
		Debug.Log("Server Started Up!");
	}
	// Update is called once per frame
	void Update () {
	
	}
}
