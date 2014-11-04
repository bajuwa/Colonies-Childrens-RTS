using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	//MapUIManager object
	public MapUIManager mUM;
	
	public GUITexture uiHead;
	public GUITexture uiStatus;
	public PlaceTextFromCorner selectedName;
	public PlaceTextFromCorner description;
	public PlaceTextFromCorner stats;
	//Empty GUITexture that will display what is selected in the bottom left
	public GUITexture headDisplay;
	// Use this for initialization
	public GUIStyle descriptionStyle;
	public GUIStyle nameStyle;
	
	private Selectable currentlySelected;
	
	void Start () {
		//Set it to null on runtime because the default is
		//the unity logo.
		headDisplay.texture = null;
		mUM = GameObject.Find("UIManager").GetComponent<MapUIManager>();
	}
	
	void Update() {
		if (!mUM) mUM = GameObject.Find("UIManager").GetComponent<MapUIManager>();
		currentlySelected = mUM.getCurrentlySelectedObject();
	}
	
	// Update is called once per frame
	void OnGUI () {
		// Get the currently selected object that we will be displaying information about
		if (currentlySelected) {
			// Ensure the gui is visible if we have selected an object
			uiHead.enabled = true;
			uiStatus.enabled = true;
			headDisplay.enabled = true;
			selectedName.enabled = true;
			description.enabled = true;
			stats.enabled = true;
		
			// Assign the texture a new texture based on what is selected and display it in the bottom left
			headDisplay.texture = currentlySelected.getDisplayImage();
			// Assign the GUIText new text based on what is selected and display it next to the head image
			selectedName.text = currentlySelected.getName();
			description.text = currentlySelected.getDescription();
			stats.text = "";
			
			// If the currently selected object is an ant unit, display its stats
			AntUnit antUnitScript = currentlySelected.GetComponent<AntUnit>();
			if (antUnitScript != null) {
				string statsText = string.Format(
					"HP: {0:0.0}/{1}    Attack: {2}\nDefense: {3}    Speed: {4}", 
					antUnitScript.currentHp, 
					antUnitScript.maxHp,
					antUnitScript.attack,
					antUnitScript.defense,
					antUnitScript.speed
				);
				stats.text = statsText;
			}
		} else {
			// Ensure the gui is invisible if we have not selected an object
			uiHead.enabled = false;
			uiStatus.enabled = false;
			headDisplay.enabled = false;
			selectedName.enabled = false;
			description.enabled = false;
			stats.enabled = false;
		}
	}

}
