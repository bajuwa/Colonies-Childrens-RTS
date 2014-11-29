using UnityEngine;
using System.Collections;

public class DontDestroyOnLoad : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
    void Awake() {
		// Ensures this object is not destroyed when changing scenes
        DontDestroyOnLoad(transform.gameObject);
    }
}
