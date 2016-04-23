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
	public float maxWidth = 1080.00f;
	public float maxHeight = 1920.00f;
	public float alpha { get; set;}
	//Set where the text start
	public enum AlignmentSetting{
		LEFT,
		CENTER,
		RIGHT,
	}

	void Start(){
		alignment = AlignmentSetting.CENTER;
		m_DrawArea.x = enemy.getRectX ();
		m_DrawArea.width = enemy.m_DrawArea.width;
		getEnemy ();
	}
	
	void OnGUI(){

		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());
		GUI.depth = depth;
		getEneOpacity ();
		GUI.color = new Color(1,1,1,alpha);
		changeFontSize ();
		adjustAlignment ();
		GUI.Label(resizeGUI(resizeByParty()), label,labelStyle);
		GUI.color = Color.white;
		if(m_parent != null) GUILayout.EndArea ();
	}

	public Rect resizeByParty(){
		Rect partyRect = m_DrawArea;
		partyRect.height = m_DrawArea.height / enemy.partySize;
		partyRect.width = m_DrawArea.width / enemy.partySize;
	//	partyRect.x = m_DrawArea.x + (m_DrawArea.width - m_DrawArea.width/enemy.partySize)/2.0f ;
		partyRect.y = m_DrawArea.y + (m_DrawArea.height - m_DrawArea.height/enemy.partySize)/2.0f ;
		return partyRect;
	}
	

	public void getEneOpacity(){
		alpha = enemy.getOpacity();
	}

	public float getOpacity(){
		return alpha;
	}

	//Receive information from selected enemy.
	public void getEnemy(){
		label = "<color=white>LV. " + enemy.level + "  " + enemy.charName + "</color>" ;
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
		
		float widthScale = Screen.width / maxWidth;
		float heightScale = Screen.height / maxHeight;
		
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

		labelStyle.fontSize = (int)(fontSize* Screen.height / maxHeight);
		if(enemy.partySize >= 1 )
			labelStyle.fontSize = (int) ( ( fontSize* (Screen.height / maxHeight) ) / (enemy.partySize*0.73f));
	}

	public float widthPartySize(){

		return m_DrawArea.width / enemy.partySize;
	}

}
