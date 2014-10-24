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
	
	public GameObject occupiedBy;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		occupiedBy = null;
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
		// If the user has 'selected' a tile, update the tiles appearance
		if (isSelected()) {
			spriteRenderer.sprite = selectedTile;
		} else {
			spriteRenderer.sprite = normalTile;
		}
	}
}
