using UnityEngine;
using System.Collections;

public class PlaceTextFromCorner : MonoBehaviour {

	public string text = "Dsecription that is super long and should eventually be word wrapped.... but we'll see I guess.";

	public bool snapToBottom = true;
	public bool snapToLeft = true;

	public int fromBottom = 0;
	public int fromTop = 0;
	public int fromLeft = 0;
	public int fromRight = 0;
	
	public float width = 0;
	public float height = 0;
	
	public GUIStyle textStyle;
	
	private GUIText myText;
	private float rectX = 0;
	private float rectY = 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI () {
		rectX = snapToLeft ? fromLeft : Screen.width - width - fromRight;
		rectY = snapToBottom ? Screen.height - height - fromBottom : fromTop;
		
        GUI.Label(new Rect(rectX, rectY, width, height), text, textStyle);
	}
}
