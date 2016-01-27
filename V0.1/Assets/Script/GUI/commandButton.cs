using UnityEngine;
using System.Collections;

public class commandButton : simpleWindow {

	public string commandMessage;
	public GameObject receiverObject;

	void OnGUI(){
		
		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());
		
		GUI.depth = depth;
		GUI.DrawTexture (m_DrawArea, m_texture);
		if (GUI.Button (m_DrawArea, "", GUIStyle.none))
			sendMessage (receiverObject, commandMessage);
		
		if(m_parent != null) GUILayout.EndArea ();
	}

	public void sendMessage(GameObject receiver, string message){
		receiver.SendMessage (message);
	}
}
