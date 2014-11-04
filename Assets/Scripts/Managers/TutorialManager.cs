using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {
	
	public MessageUiHolder messageUi;
	public GUITexture nextTutorialButton;
	
	public TutorialStageInstruction[] instructions;
	private int instructionIndex = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (messageUi.isReady) {
			if (instructionIndex < instructions.Length) {
				messageUi.setNewMessages(instructions[instructionIndex].title, instructions[instructionIndex].messages);
				instructionIndex++;
			} 
			if (instructionIndex >= instructions.Length && messageUi.isReady) {
				if (nextTutorialButton) nextTutorialButton.enabled = true;
			}
		}
	}
	
	[System.Serializable]
	public class TutorialStageInstruction {
		public string title;
		public string[] messages;
	}
}
