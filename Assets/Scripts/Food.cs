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
	// Update is called once per frame
	void Update () {
	
	}
	
	public override void select(int id) {
		base.select(id);
		mapManager.getTileAtPosition(transform.position).select(GetInstanceID());
	}
	
	public override void deselect(int id) {
		base.deselect(id);
		mapManager.getTileAtPosition(transform.position).deselect(GetInstanceID());
	}
}
