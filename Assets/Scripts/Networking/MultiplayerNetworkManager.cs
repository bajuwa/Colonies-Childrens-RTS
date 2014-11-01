using UnityEngine;
using System.Collections;

public class MultiplayerNetworkManager : MonoBehaviour {
	public GameObject antPrefab;
	// Use this for initialization
	void Start () {
	
	}
	void OnConnectedToServer()
	{
		Network.Instantiate(antPrefab, transform.position, transform.rotation, 0);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
