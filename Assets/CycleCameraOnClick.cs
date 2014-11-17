using UnityEngine;
using System.Collections;

public class CycleCameraOnClick : MonoBehaviour {

	public string nameToCycle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown() {
		transform.parent.gameObject.GetComponent<UnitCount>().centerOnNext(nameToCycle);
	}
}
