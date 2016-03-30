using UnityEngine;
using System.Collections;

public class commandButton : simpleWindow {

	public string commandMessage;
	public GameObject receiverObject;
	public bool buttonPressed = false;
	//Special case purely for statbox display commands.
	public int charNumber{ get; set; }

	void OnGUI(){
		
		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());
		
		GUI.depth = depth;
		GUI.DrawTexture (resizeGUI (m_DrawArea), m_texture);
		if (GUI.Button (resizeGUI (m_DrawArea), "", GUIStyle.none) && buttonPressed == false){
			if (audioPath != "") {
				playSound();	
			}
		if (charNumber == 0) {
			sendMessage (receiverObject, commandMessage);
		}
		if (charNumber > 0) {
				sendMessageWithValue(receiverObject, commandMessage, charNumber);
		}
		}
		if(m_parent != null) GUILayout.EndArea ();
	}

	public void sendMessage(GameObject receiver, string message){
		receiver.SendMessage (message);
	}

	public void sendMessageWithValue(GameObject receiver, string message, int charNumber){
		receiver.SendMessage (message, charNumber);

	}
}
