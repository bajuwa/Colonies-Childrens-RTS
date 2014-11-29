using UnityEngine;
using System.Collections;

public class DieAfterSeconds : MonoBehaviour {

	public float seconds;
	private float startingTime;

	// Use this for initialization
	void Start () {
		startingTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - startingTime > seconds) Destroy(this.gameObject);
	}
}
