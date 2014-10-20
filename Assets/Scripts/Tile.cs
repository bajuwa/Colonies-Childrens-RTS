using UnityEngine;
using System.Collections;

public class Tile : Selectable {

	public int terrainValue = 0;
	public Sprite normalTile;
	public Sprite selectedTile;
	private SpriteRenderer spriteRenderer;
	
	public override string description
	{
		get
		{
			return "A tile (specific tiles later";
		}
		set
		{
		}
	}
	
	public bool occupied;

	// Use this for initialization
	void Start () {
		occupied = false;
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		// If the user has 'selected' a tile, update the tiles appearance
		if (isSelected()) {
			spriteRenderer.sprite = selectedTile;
		} else {
			spriteRenderer.sprite = normalTile;
		}
	}
}
