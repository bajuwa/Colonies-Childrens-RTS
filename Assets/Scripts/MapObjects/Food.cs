using UnityEngine;
using System.Collections;

public class Food : Selectable {

	public int foodValue = 0;
	
	public override string getDescription() {
		return " A tasty piece of food that can help build new units if you bring it back to your Anthill!";
	}
	
	public int getFoodValue() {
		return foodValue;
	}
}
