using UnityEngine;
using System.Collections;

public class PlaceFromCorner : MonoBehaviour {

	public bool snapToBottom = true;
	public bool snapToLeft = true;

	public int fromBottom = 0;
	public int fromTop = 0;
	public int fromLeft = 0;
	public int fromRight = 0;
	
	private float width = 0;
	private float height = 0;
	private float rectX = 0;
	private float rectY = 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (guiTexture.texture) {
			width = guiTexture.texture.width;
			height = guiTexture.texture.height;
			
			rectX = snapToLeft ? fromLeft : Screen.width - width - fromRight;
			rectY = snapToBottom ? fromBottom : Screen.height - height - fromTop;
			
			guiTexture.pixelInset = new Rect(rectX, rectY, width, height);
		}
	}
}
