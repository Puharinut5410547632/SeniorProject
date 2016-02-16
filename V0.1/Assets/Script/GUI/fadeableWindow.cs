using UnityEngine;
using System.Collections;

public class fadeableWindow : simpleWindow {

	//For opacity fading in/out
	public float alpha;
	public float targetAlpha {get; set;}
	public float alphaspeed { get; set; }
	public enemyLabel m_enelabel;
	public State m_State;
	public enum State
	{
		FADEIN,		// Fade in
		FADEOUT,	// Fade out
		DISPLAY, 	// Just for displaying
	};

	// Update is called once per frame
	void Update () {
		fadeNormallySpeed ();
		if (m_State == State.FADEIN){
			fadeIn (targetAlpha);
			setOpacity ();
		}

		if (m_State == State.FADEOUT){
			fadeOut ();
			setOpacity ();
		}

	}

	void OnGUI(){
		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());

		if (m_enelabel != null) {
			getEneLabelLocation ();
			GUI.color = new Color(1,1,1,alpha* getLabelOpacity());
		}

		if (m_enelabel == null)GUI.color = new Color(1,1,1,alpha);
		GUI.depth = depth;
		GUI.DrawTexture (m_DrawArea, m_texture);
		GUI.Box (m_DrawArea, "" , GUIStyle.none);
		GUI.color = Color.white;
		if (m_parent != null) GUILayout.EndArea ();
	}

	public float getLabelOpacity(){
		return m_enelabel.getOpacity ();
	}
	public void getEneLabelLocation(){
		m_DrawArea.x = m_enelabel.getContentRect().x + (m_enelabel.enemy.m_DrawArea.width - m_DrawArea.width)/2.0f;
		m_DrawArea.y = m_enelabel.getContentRect().y;
	}
	public void setOpacity(){
		
		if (alpha < targetAlpha) {
			alpha += alphaspeed;
			if (alpha > targetAlpha)
				alpha = targetAlpha;
		}
		
		if (alpha > targetAlpha) {
			alpha -= alphaspeed;
			if (alpha < targetAlpha)
				alpha = targetAlpha;
		}
		
	}

	public void fadeIn(){
		targetAlpha = 1.00f;
		m_State = State.FADEIN;
	}

	public void fadeOut(){
		targetAlpha = 0.00f;
		m_State = State.FADEOUT;
	}

	public void fadeIn(float newAlpha){
		targetAlpha = newAlpha;
		m_State = State.FADEIN;
	}

	public void blinkSpeed() {
		alphaspeed = 0.2f;
	}

	public void fadeNormallySpeed(){
		alphaspeed = 0.04f;
	}
}
