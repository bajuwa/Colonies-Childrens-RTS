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
					0);
		Anthill antHill = anthillObject.GetComponent<Anthill>();
		antHill.addFoodPoints(20);
		/*antUnitObject.transform.parent = antUnitParent.transform;
				antUnitObject.transform.localPosition = new Vector3(
					antUnitObject.transform.localPosition.x,
					antUnitObject.transform.localPosition.y,
					0);*/
		//Network.Instantiate(gatherer, new Vector3(0,0,-2), Quaternion.identity, 0); //initial gatherer
	}
	//called when a player connects for the client player. Instantiates the blue anthill on the network
	//and then sends an RPC call the the server to get them to change the blue anthill to blue for the server player.
	void OnConnectedToServer()
	{
		GameObject antHillObject = (GameObject) Network.Instantiate(anthill, blueAnthillSpawn.transform.position, Quaternion.identity,0);
		//GameObject gathererObject = (GameObject) Network.Instantiate(gatherer, gathererBlueSpawn.transform.position, Quaternion.identity, 0); //initial anthill
		NetworkView anthillNetwork = antHillObject.networkView;
		//NetworkView gathererNetwork = gathererObject.networkView;
		antHillObject.transform.parent = antHillParent.transform;
				antHillObject.transform.localPosition = new Vector3(
					antHillObject.transform.localPosition.x,
					antHillObject.transform.localPosition.y,
					0);
		/*gathererObject.transform.parent = antUnitParent.transform;
				gathererObject.transform.localPosition = new Vector3(
					gathererObject.transform.localPosition.x,
					gathererObject.transform.localPosition.y,
					0);*/
		networkView.RPC("changePlayerId", RPCMode.All, anthillNetwork.viewID, 2);
		Anthill antHill = antHillObject.GetComponent<Anthill>();
		antHill.addFoodPoints(20);
	}
	public void changeID(GameObject instance)
	{
		NetworkView unitNetwork = instance.networkView;
		//networkView.RPC("fixInstantiation", RPCMode.AllBuffered, unitNetwork.viewID);
		networkView.RPC("changePlayerId", RPCMode.All, unitNetwork.viewID, 2);
		
	
	
	}
	//RPC call for changing the anthill and units to player 2
	[RPC] void changePlayerId(NetworkViewID anthillID, int player)
	{
	
		Debug.Log("Change player ID");
		NetworkView anthillNetwork = NetworkView.Find(anthillID);
		//NetworkView gathererNetwork = NetworkView.Find(gathererID);
		GameObject anthillObject = anthillNetwork.gameObject;
		//GameObject gathererObject = gathererNetwork.gameObject;
		anthillObject.GetComponent<Ownable>().setAsMine(player);
		//gathererObject.GetComponent<Ownable>().setAsMine(player);
		Debug.Log("Changed");
	}

	//Not working yet but when a player creates a unit from their anthill, it is no clickable by the enemy
	//(they only can get the information from the tiles). The purpose of this function is to get a player
	//to properly instantiate the other player's units. 
	[RPC] void fixInstantiation(NetworkViewID objectNetworkViewID)
	{
		Debug.Log("Fix Instantiation");
		NetworkView objectNetworkView = NetworkView.Find(objectNetworkViewID);
		GameObject gameObject = objectNetworkView.gameObject;
		gameObject.transform.parent = antUnitParent.transform;
				antUnitParent.transform.localPosition = new Vector3(
					antUnitParent.transform.localPosition.x,
					antUnitParent.transform.localPosition.y,
					0);
	
	}




	

	void OnGUI() {
		
	}
}
