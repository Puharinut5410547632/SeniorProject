using UnityEngine;
using System.Collections;

public class simpleLabel : simpleWindow {
	
	public string label = "";
	public float fontSize = 16;
	public AlignmentSetting alignment;
	public GUIStyle labelStyle;
	public bool fontSizeAdjusted = false;
	//Set where the text start
	public enum AlignmentSetting{
		TOPLEFT, //This is for test box
		LEFT, //This is for name plate and other stuffs
		CENTER,
		RIGHT,
	}

	void OnGUI(){
		
		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());
		GUI.depth = depth;
		changeFontSize ();
		adjustAlignment ();
		setLocation ();
		if (m_texture == null) {
			labelStyle.wordWrap=true;
			GUI.Label (resizeGUI (m_DrawArea), label, labelStyle);
		}

		//Not used anymore likely
		if (m_texture != null) {
			labelStyle.wordWrap=true;
			GUI.Box (resizeGUI (m_DrawArea), m_texture, labelStyle);
			GUI.Label (resizeGUI (m_DrawArea), label, labelStyle);
		}
		if(m_parent != null) GUILayout.EndArea ();
	}

	public void moveUp(){
		speedY = recalculateY (5.0f);
		targetY = defaultY - recalculateY (100.0f);
	}

	public void adjustAlignment(){
		if (alignment == AlignmentSetting.LEFT)
			labelStyle.alignment = TextAnchor.MiddleLeft;
		if (alignment == AlignmentSetting.RIGHT)
			labelStyle.alignment = TextAnchor.MiddleRight;
		if (alignment == AlignmentSetting.CENTER)
			labelStyle.alignment = TextAnchor.MiddleCenter;
		if (alignment == AlignmentSetting.TOPLEFT)
			labelStyle.alignment = TextAnchor.UpperLeft;
	}

	public void changeFontSize(){
		
		labelStyle.fontSize = (int) (fontSize* Screen.height / maxHeight);
	}


}
