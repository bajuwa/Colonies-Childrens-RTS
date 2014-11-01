using UnityEngine;
using System.Collections;

public class JoinGame : MonoBehaviour {
	private bool refreshing;
	private HostData[] hostData;
	// Use this for initialization
	void Start () {
		refreshHostList();
	}
	void refreshHostList(){
		MasterServer.RequestHostList("ColoniesAntBattle");
		refreshing = true;
	}
	void OnFailedToConnect(NetworkConnectionError error) {
		Debug.Log("Could not connect to server: " + error);
	}
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
				if(GUI.Button(new Rect(Screen.width/2 - 150, 100*i, Screen.width/4, 50), hostData[i].gameName)){
					Network.Connect(hostData[i]);
					Application.LoadLevel("MultiPlayerGame");
					Debug.Log(hostData[i]);
					
				}
			} 
		} 
	}
}
