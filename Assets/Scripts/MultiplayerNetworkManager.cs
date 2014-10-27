using UnityEngine;
using System.Collections;

public class MultiplayerNetworkManager : MonoBehaviour {
	void OnLevelWasLoaded () {
		Network.isMessageQueueRunning = true;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
