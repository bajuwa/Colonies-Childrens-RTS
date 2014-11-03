using UnityEngine;
using System.Collections;

public class Scentpath : Selectable {

	public override string getDescription() {
		if (isNeutralOrFriendly())
			return "A scent path can help your Ants move faster!";
		else
			return "This scent path makes your enemy's Ants move faster, have your own scout build over top of it instead!";
	}
	
	public override string getName() {
		return "Scentpath";
	}
	
	protected override void loadDisplayImage() {
		displayImage = getTextureFromPlayer("scentpathDisplay");
	}
}
