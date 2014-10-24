using UnityEngine;
using System.Collections;

public class ScoutUnit : AntUnit {

	private GameObject scentpath;
	private GameObject scentpathParent;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		loadScentpath();
		loadScentpathParent();
	}
	
	private void loadScentpath() {
		scentpath = player.scentpathSprite;
	}
	
	private void loadScentpathParent() {
		scentpathParent = GameObject.Find("Objects");
	}
	
	protected override void loadSprite() {
		gameObject.GetComponent<SpriteRenderer>().sprite = player.scoutSprite;
	}
	
	protected override void loadDisplayImage() {
		displayImage = player.scoutDisplay;
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
		if (!scentpath) loadScentpath();
		if (!scentpathParent) loadScentpathParent();
		
		// If the current tile we are moving to does not have our scent path on it, apply it
		Scentpath path = mapManager.getScentpathAtPosition(transform.position);
		if (path) {
			// If a scentpath exists, and it isn't ours, replace it with our own
			if (path.ownedBy != player.id) {
				Destroy(path.gameObject);
			}
		} else {
			// Otherwise, create and place one
			GameObject newScentpath = (GameObject) Object.Instantiate(
				scentpath, 
				mapManager.getTileAtPosition(transform.position).transform.position, 
				Quaternion.identity
			);
			newScentpath.transform.parent = scentpathParent.transform;
			newScentpath.transform.localPosition = new Vector3(
				newScentpath.transform.localPosition.x,
				newScentpath.transform.localPosition.y,
				0
			);
			newScentpath.GetComponent<Selectable>().ownedBy = player.id;
		}
	}
}
