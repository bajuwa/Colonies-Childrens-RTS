using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	//Name MUST be unique on master server
	public GameObject anthill;
	public GameObject gatherer;
	public GameObject gathererRedSpawn;
	public GameObject gathererBlueSpawn;
	public GameObject redAnthillSpawn;
	public GameObject blueAnthillSpawn;
	private const string typeName = "ColoniesAntBattle";
	private string gameName = CreateGameServer.gameName;
	private HostData hostGame = JoinGame.hostGame;
	private GameObject antHillParent;
	private GameObject antUnitParent;
	private MapManager mapManager;
	private int playerId = 1;
	
	private void Start () {
		if (hostGame == null) {
			StartServer();
		} else {
			Network.Connect(hostGame);
		}
	}
	// Update is called once per frame
	void Update () {
		if (!antUnitParent) antUnitParent = GameObject.Find("Units");
		if (!antHillParent) antHillParent = GameObject.Find("Objects");
		if (!mapManager) mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
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
	//instantiate Player 1 (the server) stuff
	void OnPlayerConnected() 
	{
		GameObject antUnitObject = (GameObject) Network.Instantiate(gatherer, gathererRedSpawn.transform.position, Quaternion.identity, 0); //initial gatherer
		GameObject anthillObject = (GameObject) Network.Instantiate(anthill, redAnthillSpawn.transform.position, Quaternion.identity, 0); //initial anthill
		anthillObject.transform.parent = antHillParent.transform;
				anthillObject.transform.localPosition = new Vector3(
					anthillObject.transform.localPosition.x,
					anthillObject.transform.localPosition.y,
					0);
		antUnitObject.transform.parent = antUnitParent.transform;
				antUnitObject.transform.localPosition = new Vector3(
					antUnitObject.transform.localPosition.x,
					antUnitObject.transform.localPosition.y,
					0);
	}

	void OnConnectedToServer()
	{
		GameObject antHillObject = (GameObject) Network.Instantiate(gatherer, gathererBlueSpawn.transform.position, Quaternion.identity,0);
		GameObject gathererObject = (GameObject) Network.Instantiate(anthill, blueAnthillSpawn.transform.position, Quaternion.identity, 0); //initial anthill
		NetworkView anthillNetwork = antHillObject.networkView;
		NetworkView gathererNetwork = gathererObject.networkView;
		networkView.RPC("changePlayerId", RPCMode.AllBuffered, anthillNetwork.viewID, gathererNetwork.viewID, 2);
	}
	[RPC] void changePlayerId(NetworkViewID anthillID, NetworkViewID gathererID, int player)
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



	

	void OnGUI() {
		
	}
}
