using UnityEngine;
using System.Collections;

public class Food : Selectable {

	public int foodValue = 0;
	
	public string specificDescription;
	public string specificName;
	
	public override string getDescription() {
		return specificDescription;
	}
	
	public override string getName() {
		return specificName;
	}
	
	public int getFoodValue() {
		return foodValue;
	}
}
