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
		scentpath = getGameObjectFromPlayer("scentpathGameObject");
	}
	
	private void loadScentpathParent() {
		scentpathParent = GameObject.Find("Objects");
	}
	
	protected override void loadSprite() {
		gameObject.GetComponent<SpriteRenderer>().sprite = getSpriteFromPlayer("scoutSprite");
	}
	
	protected override void loadDisplayImage() {
		displayImage = getTextureFromPlayer("scoutDisplay");
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
			if (path.getPlayerId() != this.getPlayerId()) {
				Destroy(path.gameObject);
			}
		} else {
			// Otherwise, create and place one
			if (!scentpath) return;
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
			newScentpath.GetComponent<Ownable>().setAsMine(getPlayerId());
		}
	}
}
