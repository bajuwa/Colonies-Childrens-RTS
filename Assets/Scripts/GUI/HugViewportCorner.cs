using UnityEngine;
using System.Collections;

public class HugViewportCorner : MonoBehaviour {

	public bool hugLeft = false;
	public bool hugBottom = false;
	
	private float xModifier;
	private float yModifier;
	
	private float viewportX;
	private float viewportY;

	// Use this for initialization
	void Start () {
		float width = GetComponent<SpriteRenderer>().renderer.bounds.size.x;
		float height = GetComponent<SpriteRenderer>().renderer.bounds.size.y;
		xModifier = hugLeft ? width : -1 * width;
		yModifier = hugLeft ? height : -1 * height;
		viewportX = hugLeft ? 0 : 1;
		viewportY = hugBottom ? 0 : 1;
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 viewportBottomLeft = Camera.main.ViewportToWorldPoint(
			new Vector2(viewportX, viewportY)
		);
		transform.position = new Vector3(
			viewportBottomLeft.x + xModifier,
			viewportBottomLeft.y + yModifier,
			transform.position.z
		);
	}
}
