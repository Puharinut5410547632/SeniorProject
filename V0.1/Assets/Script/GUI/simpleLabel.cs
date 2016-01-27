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
		LEFT,
		CENTER,
		RIGHT,
	}

	void OnGUI(){
		
		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());
		adjustFontSize ();
		adjustAlignment ();
		setLocation ();
		GUI.Label(m_DrawArea, label,labelStyle);
		if(m_parent != null) GUILayout.EndArea ();
	}

	public void moveUp(){
		speedY = recalculateY (5.0f);
		targetY = defaultY - recalculateY (100.0f);
	}

	public void adjustFontSize(){
		if(fontSizeAdjusted == false ){changeFontSize(); fontSizeAdjusted = true;}
		labelStyle.fontSize = (int) fontSize;
	}

	public void adjustAlignment(){
		if (alignment == AlignmentSetting.LEFT)
			labelStyle.alignment = TextAnchor.MiddleLeft;
		if (alignment == AlignmentSetting.RIGHT)
			labelStyle.alignment = TextAnchor.MiddleRight;
		if (alignment == AlignmentSetting.CENTER)
			labelStyle.alignment = TextAnchor.MiddleCenter;
	}

	public void changeFontSize(){
		
		fontSize = fontSize* Screen.height / 1280.000f;
	}


}
