using UnityEngine;
using System.Collections;

public class Food : Selectable {

	private int foodValue;
	
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
}
