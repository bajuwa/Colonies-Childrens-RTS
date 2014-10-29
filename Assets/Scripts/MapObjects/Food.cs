using UnityEngine;
using System.Collections;

public class Food : Selectable {

	public int foodValue = 0;
	
	public string specificDescription;
	
	public override string getDescription() {
		return specificDescription;
	}
	
	public int getFoodValue() {
		return foodValue;
	}
}
