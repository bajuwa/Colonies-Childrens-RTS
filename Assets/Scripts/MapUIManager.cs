using UnityEngine;
using System.Collections;

public class MapUIManager : MonoBehaviour {

	public Texture2D defaultCursor;
	public Texture2D moveToCursor;

	private Selectable selectedObject;
	private MapManager mapManager;
	private Transform tileSpriteParentTransform;

	// Use this for initialization
	void Start () {
		setMapManager();
		tileSpriteParentTransform = transform.Find("Tiles");
		Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
	}
	
	// Update is called once per frame
	void Update () {
		// If the user clicks on the map, try to 'select' what they have clicked on
		if (Input.GetMouseButtonDown(0)) {
			// First clear the last selected object
			if (selectedObject) {
				selectedObject.GetComponent<Selectable>().deselect(GetInstanceID());
				selectedObject = null;
			}
			
			// Get the newly selected object and set it to 'selected'
			selectedObject = getSelectableAtPosition(Camera.main.ScreenToWorldPoint((Vector2) Input.mousePosition), false);
			
			if (selectedObject) {
				selectedObject.GetComponent<Selectable>().select(GetInstanceID());
			}
		}
		
		// If the user right-clicks when a Friendly Ant Unit is selected, move that unit along a path to the right clicked tile
		if (Input.GetMouseButtonDown(1) && selectedObject != null && selectedObject.isSelectable()) {
			// Get the Ant Unit script (if any)
			AntUnit antUnitScript = selectedObject.GetComponent<AntUnit>();
			if (antUnitScript != null) {
				// Set the unit on a path to their target
				StartCoroutine(antUnitScript.moveTo(mapManager.getTileAtPosition(Camera.main.ScreenToWorldPoint((Vector2) Input.mousePosition), false)));
			}
		}
		
		// Depending on what the user has selected and what they are currenlty hovering over, change the mouse cursor
		if (selectedObject != null) {
			setCursor(selectedObject);
		}
	}
	
	private void setCursor(Selectable selectedObject) {
		// We only need to set custom cursors if a friendly ant unit is currently selected
		if (selectedObject != null && selectedObject.GetComponent<AntUnit>() != null && selectedObject.isSelectable()) {
			Selectable hoveredObject = getSelectableAtPosition((Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition), false);
			if (hoveredObject != null) {
				// If a Tile is the topMostSelectable then we should display a 'move' type cursor
				if (hoveredObject.GetComponent<Tile>() != null) {
					Cursor.SetCursor(moveToCursor, Vector2.zero, CursorMode.Auto);
					return;
				}
			}
		}
			
		// Lastly, if no cursor has been set, use the default
		Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
	}
		
	public Selectable getSelectableAtPosition(Vector2 position, bool isLocalPosition = true) {
		Collider2D[] overlappedSelectables = Physics2D.OverlapPointAll(
			isLocalPosition ? (Vector2) tileSpriteParentTransform.TransformPoint(position) : position
		);
		GameObject topMostSelectable = null;
		// Since multiple selectables might be stacked on top of each other, only return the 'top most' element
		for (int i=0; i<overlappedSelectables.Length; i++) {
			// Make sure our overlapped collider actually belongs to a selectable gameobject
			if (overlappedSelectables[i].gameObject.GetComponent<Selectable>() == null) continue;
			// and also that it's position is closer to the camera than the previous selectable (if any)
			if (topMostSelectable == null || 
				topMostSelectable.transform.position.z > overlappedSelectables[i].gameObject.transform.position.z) {
					topMostSelectable = overlappedSelectables[i].gameObject;
			}
		}
		return topMostSelectable != null ? topMostSelectable.GetComponent<Selectable>() : null;
	}
	
	private void setMapManager() {
		GameObject[] managers = GameObject.FindGameObjectsWithTag("MapManager");
		if (managers.Length == 1) mapManager = (MapManager) managers[0].GetComponent(typeof(MapManager));
		else Debug.Log("WARN: Found incorrect number of GameObjects with MapManager Tag.  Expected 1, found " + managers.Length);
	}
}
