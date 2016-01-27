using UnityEngine;
using System.Collections;

public class enemyLabel : MonoBehaviour {
	
	public Enemy enemy = null;
	public simpleWindow m_parent = null;
	public string label = "";
	public float fontSize = 16;
	public int depth = 5;	
	public AlignmentSetting alignment;
	public bool fontSizeAdjusted = false;
	public GUIStyle labelStyle;
	public Rect m_DrawArea;
	public float alpha { get; set;}
	//Set where the text start
	public enum AlignmentSetting{
		LEFT,
		CENTER,
		RIGHT,
	}

	void Start(){
		alignment = AlignmentSetting.CENTER;
		getEnemy ();
		m_DrawArea = resizeGUI (m_DrawArea);
		m_DrawArea.x = enemy.getRectX ();
	}

	void OnGUI(){

		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());
		GUI.depth = depth;
		getEneOpacity ();
		GUI.color = new Color(1,1,1,alpha);
		adjustFontSize ();
		adjustAlignment ();
		GUI.Label(m_DrawArea, label,labelStyle);
		GUI.color = Color.white;
		if(m_parent != null) GUILayout.EndArea ();
	}

	public void getEneOpacity(){
		alpha = enemy.getOpacity();
	}

	public float getOpacity(){
		return alpha;
	}
	//Receive information from selected enemy.
	public void getEnemy(){
		label = "LV. " + enemy.level + "  " + enemy.name ;
	//	label = "<color=white>LV. " + enemy.level + "  " + enemy.name + "</color>";
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

	public Rect resizeGUI(Rect Drect){
		
		float widthScale = Screen.width / 800.00f;
		float heightScale = Screen.height / 1280.000f;
		
		float rectWidth = widthScale * Drect.width;
		float rectHeight = heightScale * Drect.height;
		
		float rectX = Drect.x * widthScale;
		float rectY = Drect.y * heightScale;
		
		return new Rect (rectX, rectY, rectWidth, rectHeight);
	}

	public Rect getContentRect()
		
	{
		return m_DrawArea;
	}

	public void changeFontSize(){

		fontSize = fontSize* Screen.height / 1280.000f;
	}
}
