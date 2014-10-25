using UnityEngine;
using System.Collections;

public class Scentpath : Selectable {

	public override string description
	{
		get
		{
			return "A Scentpath can help your Ants move faster, but only if it's your scentpath!";
		}
		set
		{
		}
	}
	
	protected override void loadDisplayImage() {
		displayImage = getTextureFromPlayer("scentpathDisplay");
	}
}
