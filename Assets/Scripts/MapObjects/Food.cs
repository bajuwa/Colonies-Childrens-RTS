using UnityEngine;
using System.Collections;

public class Food : Selectable {

	public int foodValue = 0;
	
	public override string description
	{
		get
		{
			return "Send a gatherer to collect this";
		}
		set
		{
		}
	}
	
	public int getFoodValue() {
		return foodValue;
	}
}
