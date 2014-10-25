using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public int id = 1;

	// List of sprites/textures/objects that can change depending on who they are owned by
	private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
	private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
	private Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();
	
	public Sprite gathererSprite;
	public Sprite warriorSprite;
	public Sprite scoutSprite;
	public Sprite anthillSprite;

	public Texture2D gathererDisplay;
	public Texture2D warriorDisplay;
	public Texture2D scoutDisplay;
	public Texture2D scentpathDisplay;
	public Texture2D anthillDisplay;
	
	public GameObject scentpathGameObject;
	
	// Use this for initialization
	void Start () {
		sprites.Add("gathererSprite", gathererSprite);
		sprites.Add("warriorSprite", warriorSprite);
		sprites.Add("scoutSprite", scoutSprite);
		sprites.Add("anthillSprite", anthillSprite);
		
		textures.Add("gathererDisplay", gathererDisplay);
		textures.Add("warriorDisplay", warriorDisplay);
		textures.Add("scoutDisplay", scoutDisplay);
		textures.Add("scentpathDisplay", scentpathDisplay);
		textures.Add("anthillDisplay", anthillDisplay);
		
		gameObjects.Add("scentpathGameObject", scentpathGameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public int getId() {
		return id;
	}
	
	public Sprite getSprite(string name) {
		if (sprites.ContainsKey(name)) return sprites[name];
		return null;
	}
	
	public Texture2D getTexture(string name) {
		if (textures.ContainsKey(name)) return textures[name];
		return null;
	}
	
	public GameObject getGameObject(string name) {
		if (gameObjects.ContainsKey(name)) return gameObjects[name];
		return null;
	}
}
