using UnityEngine;
using System.Collections;

public class simpleLabel : simpleWindow {


	public string label = "";
	public int fontSize = 16;
	public AlignmentSetting alignment;
	public GUIStyle labelStyle;

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
		GUI.Label(m_DrawArea, label,labelStyle);

		if(m_parent != null) GUILayout.EndArea ();
	}

	public void adjustFontSize(){
		labelStyle.fontSize = fontSize;
	}

	public void adjustAlignment(){
		if (alignment == AlignmentSetting.LEFT)
			labelStyle.alignment = TextAnchor.MiddleLeft;
		if (alignment == AlignmentSetting.RIGHT)
			labelStyle.alignment = TextAnchor.MiddleRight;
		if (alignment == AlignmentSetting.CENTER)
			labelStyle.alignment = TextAnchor.MiddleCenter;
	}
}
