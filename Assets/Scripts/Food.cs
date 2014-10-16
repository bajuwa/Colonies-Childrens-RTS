using UnityEngine;
using System.Collections;

public class Food : Selectable {

	private int foodValue;

	// Use this for initialization
	void Start () {
	
	}
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
	// Update is called once per frame
	void Update () {
	
	}
}
