using UnityEngine;
using System.Collections;

public class fadeableWindow : simpleWindow {

	//For opacity fading in/out
	public float alpha;
	public float targetAlpha {get; set;}
	public float alphaspeed { get; set; }

	public State m_State;
	public enum State
	{
		FADEIN,		// Fade in
		FADEOUT,	// Fade out
	};

	// Update is called once per frame
	void Update () {
		fadeNormallySpeed ();
		if (m_State == State.FADEIN){
			fadeIn ();
			setOpacity ();
		}

		if (m_State == State.FADEOUT){
			fadeOut ();
			setOpacity ();
		}
	}

	void OnGUI(){

		GUI.color = new Color(1,1,1,alpha);
		GUI.depth = depth;
		GUI.DrawTexture (m_DrawArea, m_texture);
		GUI.Box (m_DrawArea, "" , GUIStyle.none);
		GUI.color = Color.white;

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

	public void blinkSpeed() {
		alphaspeed = 0.2f;
	}

	public void fadeNormallySpeed(){
		alphaspeed = 0.04f;
	}
}
