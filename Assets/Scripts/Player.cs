using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public int id = 1;

	// List of sprites/textures/objects that can change depending on who they are owned by
	private Dictionary<string, RuntimeAnimatorController> animators = new Dictionary<string, RuntimeAnimatorController>();
	private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
	private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
	private Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();
	
	public RuntimeAnimatorController gathererAnimator;
	public RuntimeAnimatorController warriorAnimator;
	public RuntimeAnimatorController scoutAnimator;
	public RuntimeAnimatorController queenAnimator;
	
	public Sprite gathererSprite;
	public Sprite warriorSprite;
	public Sprite scoutSprite;
	public Sprite queenSprite;
	public Sprite anthillSprite;

	public Texture2D gathererDisplay;
	public Texture2D warriorDisplay;
	public Texture2D scoutDisplay;
	public Texture2D queenDisplay;
	public Texture2D scentpathDisplay;
	public Texture2D anthillDisplay;
	public Texture2D gathererHead;
	public Texture2D warriorHead;
	public Texture2D scoutHead;
	public Texture2D queenHead;
	
	public GameObject scentpathGameObject;
	
	// Use this for initialization
	void Start () {
		animators.Add("gathererAnimator", gathererAnimator);
		animators.Add("warriorAnimator", warriorAnimator);
		animators.Add("scoutAnimator", scoutAnimator);
		animators.Add("queenAnimator", queenAnimator);
		
		sprites.Add("gathererSprite", gathererSprite);
		sprites.Add("warriorSprite", warriorSprite);
		sprites.Add("scoutSprite", scoutSprite);
		sprites.Add("queenSprite", queenSprite);
		sprites.Add("anthillSprite", anthillSprite);
		
		textures.Add("gathererDisplay", gathererDisplay);
		textures.Add("warriorDisplay", warriorDisplay);
		textures.Add("scoutDisplay", scoutDisplay);
		textures.Add("queenDisplay", queenDisplay);
		textures.Add("scentpathDisplay", scentpathDisplay);
		textures.Add("anthillDisplay", anthillDisplay);
		textures.Add("gathererHead", gathererHead);
		textures.Add("warriorHead", warriorHead);
		textures.Add("scoutHead", scoutHead);
		textures.Add("queenHead", queenHead);
		
		gameObjects.Add("scentpathGameObject", scentpathGameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public int getId() {
		return id;
	}
	
	public RuntimeAnimatorController getAnimator(string name) {
		if (animators.ContainsKey(name)) return animators[name];
		return null;
	}
	
	public Sprite getSprite(string name) {
		if (sprites.ContainsKey(name)) return sprites[name];
		return null;
	}
	
	public Texture2D getTexture(string name) {
		if (textures.ContainsKey(name)) return textures[name];
		Debug.Log("Did not find texture by the name: " + name);
		return null;
	}
	
	public GameObject getGameObject(string name) {
		if (gameObjects.ContainsKey(name)) return gameObjects[name];
		return null;
	}
}
