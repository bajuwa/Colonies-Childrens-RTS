using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	public Vector3 cameraPosForPlayerTwo;
	//Name MUST be unique on master server
	public GameObject anthill;
	public GameObject gatherer;
	public GameObject gathererRedSpawn;
	public GameObject gathererBlueSpawn;
	public GameObject redAnthillSpawn;
	public GameObject blueAnthillSpawn;
	//public GameObject deadAntHill;
	//public GameObject deadAntHillSpawn;
	private const string typeName = "ColoniesAntBattle";
	private string gameName = CreateGameServer.gameName;
	private HostData hostGame = JoinGame.hostGame;
	private GameObject antHillParent;
	private GameObject antUnitParent;
	private MapManager mapManager;
	
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
	// initialize the server
	void StartServer () {
		Network.InitializeServer(2, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(
			typeName, 
			gameName, 
			GameObject.Find("MapSelectionNetworking").GetComponent<MapSelectionScene>().sceneIndex.ToString()
		);
	}
	//Messages
	//When the server is created, log the registration
	void OnMasterServerEvent(MasterServerEvent mse) {
		if(mse == MasterServerEvent.RegistrationSucceeded) {
			Debug.Log("Registered");
		}
	}
	//called when a player connects for the server player. Instantiates the red anthill on the network
	void OnPlayerConnected() 
	{
		GameObject anthillObject = (GameObject) Network.Instantiate(anthill, redAnthillSpawn.transform.position, Quaternion.identity, 0); //initial anthill
		anthillObject.transform.parent = antHillParent.transform;
		anthillObject.transform.localPosition = new Vector3(
			anthillObject.transform.localPosition.x,
			anthillObject.transform.localPosition.y,
			0
		);
		
		Anthill antHill = anthillObject.GetComponent<Anthill>();
		NetworkView anthillNetworkView = anthillObject.networkView;
		networkView.RPC("fixInstantiation", RPCMode.Others, anthillNetworkView.viewID, "Object");
		Debug.Log("I am player 1");
		
	}
	//called when a player connects for the client player. Instantiates the blue anthill on the network
	//and then sends an RPC call the the server to get them to change the blue anthill to blue for the server player.
	void OnConnectedToServer()
	{
		Camera.mainCamera.transform.position = cameraPosForPlayerTwo; //set camera for player 2
		GameObject.Find("UnitCountDisplay").GetComponent<Ownable>().setAsMine(2);
		GameObject antHillObject = (GameObject) Network.Instantiate(anthill, blueAnthillSpawn.transform.position, Quaternion.identity,0);
		NetworkView anthillNetwork = antHillObject.networkView;
		antHillObject.transform.parent = antHillParent.transform;
		antHillObject.transform.localPosition = new Vector3(
			antHillObject.transform.localPosition.x,
			antHillObject.transform.localPosition.y,
			0
		);
		networkView.RPC("changePlayerId", RPCMode.All, anthillNetwork.viewID);
		networkView.RPC("fixInstantiation", RPCMode.Others, anthillNetwork.viewID, "Object");
		Anthill antHill = antHillObject.GetComponent<Anthill>();
		Debug.Log("I am player 2!");
	}
	public void changeID(GameObject instance)
	{
		NetworkView unitNetwork = instance.networkView;
		if (Network.isServer) {
			Debug.Log("Changing player ID");
		}
		networkView.RPC("changePlayerId", RPCMode.All, unitNetwork.viewID);
	}
	public void changeInstant(GameObject instance, string type)
	{
		NetworkView unitNetwork = instance.networkView;
		networkView.RPC("fixInstantiation", RPCMode.All, unitNetwork.viewID, type);
	}
	//RPC call for changing the anthill and units to player 2
	[RPC] void changePlayerId(NetworkViewID anthillID)
	{
	
		if (Network.isServer) {
			Debug.Log("Changing player ID");
		}
		NetworkView anthillNetwork = NetworkView.Find(anthillID);
		GameObject anthillObject = anthillNetwork.gameObject;
		anthillObject.GetComponent<Ownable>().setAsMine(2);
		Debug.Log("Changed: " + anthillObject);
	}

	//Not working yet but when a player creates a unit from their anthill, it is no clickable by the enemy
	//(they only can get the information from the tiles). The purpose of this function is to get a player
	//to properly instantiate the other player's units. 
	[RPC] void fixInstantiation(NetworkViewID objectNetworkViewID, string type)
	{
		
		NetworkView objectNetworkView = NetworkView.Find(objectNetworkViewID);
		GameObject gameObject = objectNetworkView.gameObject;
		if (type == "Object"){
			Debug.Log("GameObject is anthill: " + gameObject);
			gameObject.transform.parent = antHillParent.transform;
			gameObject.transform.localPosition = new Vector3(
				gameObject.transform.localPosition.x,
				gameObject.transform.localPosition.y,
				0
			);
		}
		if (type == "Unit") {
			gameObject.transform.parent = antUnitParent.transform;
			gameObject.transform.localPosition = new Vector3(
				gameObject.transform.localPosition.x,
				gameObject.transform.localPosition.y,
				0
			);
		}
		if (type == "Cloud") {
			gameObject.transform.parent = antHillParent.transform;
			gameObject.transform.localPosition = new Vector3(
				gameObject.transform.localPosition.x,
				gameObject.transform.localPosition.y,
				0);
			}
	
	}




	

	void OnGUI() {
		
	}
}
