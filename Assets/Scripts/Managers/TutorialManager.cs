using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {
	
	public MessageUiHolder messageUi;
	
	public TutorialStageInstruction[] instructions;
	private int instructionIndex = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (messageUi.isReady && instructionIndex < instructions.Length) {
			messageUi.setNewMessages(instructions[instructionIndex].title, instructions[instructionIndex].messages);
			instructionIndex++;
		}
	}
	
	[System.Serializable]
	public class TutorialStageInstruction {
		public string title;
		public string[] messages;
	}
}
