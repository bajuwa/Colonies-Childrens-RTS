using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {

		// Use this for initialization
		void Start (){
			// Get all the tiles on the screen and "snap" them to the closest valid position
			GameObject[] objects = GameObject.FindGameObjectsWithTag("MapObject") as GameObject[];
			if (objects.Length == 0) return;
			Debug.Log("Found " + objects.Length.ToString() + " MapObjects");
			
			// In order to calculate the valid positions, we need to take in to account each tiles height/width
			// (use the first tile as a test, assume all are the same)
			float height = objects[0].renderer.bounds.size.y * 0.75f;
			float width = objects[0].renderer.bounds.size.x;
			
			foreach (GameObject obj in objects) {
				snapToNearestLocation(obj, width, height);
			}
		}
	
		// Update is called once per frame
		void Update () {
		
		}
		
		private void snapToNearestLocation(GameObject obj, float xInterval, float yInterval) {
			// Grab a temporary copy of the current position
			Vector2 tempPos = obj.transform.position;
			// Set the y coord to the closest valid 'row'
			tempPos.y = closestDistance (tempPos.y, minIncrement(tempPos.y, yInterval), maxIncrement(tempPos.y, yInterval));
			// If we are on an 'odd' row, then stagger it the x coord by half a width so that the hex tiles mesh together
			float xIncrement = xInterval;
			int rowNumber = (int)Mathf.Abs(tempPos.y/yInterval) % 2;
			if (rowNumber % 2 == 1) {
				tempPos.x = closestDistance(tempPos.x, minIncrement(tempPos.x, xIncrement)+xIncrement/2f, maxIncrement(tempPos.x, xIncrement)+xIncrement/2f);
			} else {
				tempPos.x = closestDistance(tempPos.x, minIncrement(tempPos.x, xIncrement), maxIncrement(tempPos.x, xIncrement));
			}
			// Set the tile's position to our modified vector
			obj.transform.position = tempPos;
		}

		private float diffBetween (float a, float b) {
			return Mathf.Abs (a - b);
		}

		private float closestDistance (float target, float optionOne, float optionTwo) {
			return diffBetween (target, optionOne) < diffBetween (target, optionTwo) ? optionOne : optionTwo;
		}
		
		private float minIncrement(float x, float increment) {
			return Mathf.Floor (x / increment) * increment;
		}
		
		private float maxIncrement(float x, float increment) {
			return Mathf.Ceil (x / increment) * increment;
		}
}
