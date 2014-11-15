using UnityEngine;

public class DeadAnthill : Selectable {
	
	//To be displayed on the GUI
	public override string getDescription() {
		return "Looks like the ruins of an old Ant Colony... Maybe you can bring your queen here to make it your own!";
	}
	
	public override string getName() {
		return "Ruins";
	}
	
}