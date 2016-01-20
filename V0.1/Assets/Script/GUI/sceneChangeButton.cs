using UnityEngine;
using System.Collections;

public class sceneChangeButton : simpleWindow {

	//If you place the button on a BG, add the BG into the script so that it can use
	//the GUI Layout for auto scale and repositioning.
	public simpleWindow m_parent = null;
	public int nextSceneID = 0;
	public bool buttonPressed = false;

	void OnGUI(){
		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());
		GUI.depth = depth;
		GUI.DrawTexture (m_DrawArea, m_texture);

		//Trigger for pressing on the button
		if (Input.GetMouseButtonDown (0) && m_DrawArea.Contains (Event.current.mousePosition) && buttonPressed == false){
			buttonPressed = true;
			changeScene (nextSceneID);
			Debug.Log ("ButtonPressed");
		}

		if(m_parent != null) GUILayout.EndArea ();
	}

	public void changeScene(int id){
		Application.LoadLevel (id);
	}
}
