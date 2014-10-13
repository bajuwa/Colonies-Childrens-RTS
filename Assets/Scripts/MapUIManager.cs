﻿using UnityEngine;
using System.Collections;

/**
 * Handles the UI that is directly involved within the space of the 'map' such as:
 * - detects interactions with map objects (units, tiles, objects)
 * - changing the maps appearance based on interactions
 *
 * This class does NOT handle any GUI frame displays or any UI outside the scope of the map,
 * but it is intended to be used to access currently selected objects for addition GUI display purposes
 */
public class MapUIManager : MonoBehaviour {

	// Cursor textures: Ensure that their raw image is labelled as a 'cursor'
	public Texture2D defaultCursor;
	public Texture2D moveToCursor;
	public Texture2D gatherCursor;

	private Selectable selectedObject;
	private MapManager mapManager;

	// Use this for initialization
	void Start () {
		setMapManager();
		Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
	}
	
	/**
	 * Each frame, check for user input that would change our currently selected object
	 * Update the map and cursor appearance based on selections
	 */
	void Update () {
		// If the user clicks on the map, try to 'select' what they have clicked on
		if (Input.GetMouseButtonDown(0)) {
			// First clear the last selected object
			if (selectedObject) {
				selectedObject.GetComponent<Selectable>().deselect(GetInstanceID());
				selectedObject = null;
			}
			
			// Get the newly selected object and set it to 'selected'
			selectedObject = getSelectableAtPosition(Camera.main.ScreenToWorldPoint((Vector2) Input.mousePosition));
			
			// Make sure to 'select' the new object once we have obtained it
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
		
		// Depending on what the user has selected and what they are currently hovering over, change the mouse cursor
		if (selectedObject != null) {
			setCursor(selectedObject);
		}
	}

	public Selectable getCurrentlySelectedObject() {
		return selectedObject;
	}
	
	public Selectable getSelectableAtPosition(Vector2 position) {
		Collider2D[] overlappedSelectables = Physics2D.OverlapPointAll(position);
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
	
	/**
	 * Depending on the current situation with the selected object and where our mouse is hovering,
	 * different cursors will need to be displayed
	 */
	private void setCursor(Selectable selectedObject) {
		// We only need to set custom cursors if a friendly ant unit is currently selected
		if (selectedObject != null && selectedObject.GetComponent<AntUnit>() != null && selectedObject.isSelectable()) {
			Selectable hoveredObject = getSelectableAtPosition((Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition));
			if (hoveredObject != null) {
				// If a Tile is the topMostSelectable then we should display a 'move' type cursor
				if (hoveredObject.GetComponent<Tile>() != null) {
					Cursor.SetCursor(moveToCursor, Vector2.zero, CursorMode.Auto);
					return;
				}
				
				// If Food is the topMostSelectable and we currently have a Gatherer selected, display a 'gather' type cursor
				if (selectedObject.GetComponent<GathererUnit>() != null && hoveredObject.GetComponent<Food>() != null) {
					Cursor.SetCursor(gatherCursor, Vector2.zero, CursorMode.Auto);
					return;
				}
			}
		}
			
		// Lastly, if no cursor has been set, use the default
		Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
	}
}
