using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	//Name MUST be unique on master server
	public GameObject anthill;
	public GameObject gatherer;
	public GameObject gathererRedSpawn;
	public GameObject gathererBlueSpawn;
	private const string typeName = "ColoniesAntBattle";
	private string gameName = CreateGameServer.gameName;
	private HostData hostGame = JoinGame.hostGame;
	private GameObject antHillParent;
	private GameObject antUnitParent;
	private int playerId = 1;
	
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
	void OnPlayerConnected() 
	{
		Network.Instantiate(gatherer, transform.position = new Vector3(-14, -12, -2), transform.rotation, 0);
		//Network.Instantiate(gatherer, gathererBlueSpawn.transform.position, transform.rotation, 0);
		Network.Instantiate(anthill, transform.position = new Vector3(0,0,-2), transform.rotation, 0);
	}

	void OnConnectedToServer()
	{
		playerId = 2;
		GameObject testobject = (GameObject) Network.Instantiate(gatherer, transform.position = new Vector3(1,2,-4), transform.rotation,0);
		testobject.GetComponent<Ownable>().setAsMine(playerId);
		//testobject.networkView.RPC("changePlayerId", RPCMode.AllBuffered, playerId);
	}
	[RPC] void changePlayerId(int player)
	{

		NetworkView anthillNetwork = NetworkView.Find(anthillID);
		NetworkView gathererNetwork = NetworkView.Find(gathererID);
		GameObject anthillObject = anthillNetwork.gameObject;
		GameObject gathererObject = gathererNetwork.gameObject;
		anthillObject.GetComponent<Ownable>().setAsMine(player);
		gathererObject.GetComponent<Ownable>().setAsMine(player);
		anthillObject.transform.parent = antHillParent.transform;
				anthillObject.transform.localPosition = new Vector3(
					anthillObject.transform.localPosition.x,
					anthillObject.transform.localPosition.y,
					0);
		gathererObject.transform.parent = antUnitParent.transform;
				gathererObject.transform.localPosition = new Vector3(
					gathererObject.transform.localPosition.x,
					gathererObject.transform.localPosition.y,
					0);
	}


	// Update is called once per frame
	void Update () {
		if (!antUnitParent) antUnitParent = GameObject.Find("Units");
		if (!antHillParent) antHillParent = GameObject.Find("Objects");
	}
	

	void OnGUI() {
		
	}
}
