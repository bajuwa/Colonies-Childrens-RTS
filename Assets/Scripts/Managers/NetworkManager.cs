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
		Debug.Log(antUnitParent + " is Set");
		if (!antHillParent) antHillParent = GameObject.Find("Objects");
		if (!mapManager) mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
	}
	// initialize the server
	void StartServer () {
		Network.InitializeServer(2, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
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
		//GameObject antUnitObject = (GameObject) Network.Instantiate(gatherer, gathererRedSpawn.transform.position, Quaternion.identity, 0); //initial gatherer
		GameObject anthillObject = (GameObject) Network.Instantiate(anthill, redAnthillSpawn.transform.position, Quaternion.identity, 0); //initial anthill
		anthillObject.transform.parent = antHillParent.transform;
		anthillObject.transform.localPosition = new Vector3(
				anthillObject.transform.localPosition.x,
				anthillObject.transform.localPosition.y,
					-2);
		
		Anthill antHill = anthillObject.GetComponent<Anthill>();
		antHill.addFoodPoints(20);
		NetworkView anthillNetworkView = anthillObject.networkView;
		networkView.RPC("fixInstantiation", RPCMode.Others, anthillNetworkView.viewID, "Object");
	}
	//called when a player connects for the client player. Instantiates the blue anthill on the network
	//and then sends an RPC call the the server to get them to change the blue anthill to blue for the server player.
	void OnConnectedToServer()
	{
		Camera.mainCamera.transform.position = new Vector3(17, 12, -10); //set camera for player 2
		GameObject antHillObject = (GameObject) Network.Instantiate(anthill, blueAnthillSpawn.transform.position, Quaternion.identity,0);
		NetworkView anthillNetwork = antHillObject.networkView;
		antHillObject.transform.parent = antHillParent.transform;
				antHillObject.transform.localPosition = new Vector3(
					antHillObject.transform.localPosition.x,
					antHillObject.transform.localPosition.y,
					-2);
		networkView.RPC("changePlayerId", RPCMode.All, anthillNetwork.viewID, 2);
		networkView.RPC("fixInstantiation", RPCMode.Others, anthillNetwork.viewID, "Object");
		Anthill antHill = antHillObject.GetComponent<Anthill>();
		antHill.addFoodPoints(20);
	}
	public void changeID(GameObject instance)
	{
		NetworkView unitNetwork = instance.networkView;
		networkView.RPC("changePlayerId", RPCMode.All, unitNetwork.viewID, 2);
	}
	public void changeInstant(GameObject instance, string type)
	{
		NetworkView unitNetwork = instance.networkView;
		networkView.RPC("fixInstantiation", RPCMode.All, unitNetwork.viewID, type);
	}
	//RPC call for changing the anthill and units to player 2
	[RPC] void changePlayerId(NetworkViewID anthillID, int player)
	{
	
		Debug.Log("Change player ID");
		NetworkView anthillNetwork = NetworkView.Find(anthillID);
		GameObject anthillObject = anthillNetwork.gameObject;
		anthillObject.GetComponent<Ownable>().setAsMine(player);
		Debug.Log("Changed");
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
			antHillParent.transform.localPosition = new Vector3(
				antHillParent.transform.localPosition.x,
				antHillParent.transform.localPosition.y,
				-2);
		}
		if (type == "Unit") {
			gameObject.transform.parent = antUnitParent.transform;
			Debug.Log("GameObject is antunit: " + gameObject);
			Debug.Log(antUnitParent + " is here!!!!!!");
				antUnitParent.transform.localPosition = new Vector3(
					antUnitParent.transform.localPosition.x,
					antUnitParent.transform.localPosition.y,
					-2);
		}
	
	}




	

	void OnGUI() {
		
	}
}
