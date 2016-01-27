using UnityEngine;
using System.Collections;

public class barDisplay : simpleWindow {

	public Character m_target;
	public string m_role;
	public float m_max;
	public float m_current;
	public float multiplier;
	public Texture m_color;
	public string m_colorPath;
	public Rect m_barFillRect;
	public float alpha;
	// Use this for initialization
	void Start () {
		m_DrawArea.x = m_target.xBeforeResize;
		m_barFillRect.x = m_DrawArea.x + 10.0f;
		doStart ();
		m_barFillRect = resizeGUI (m_barFillRect);
		m_parent = m_target.m_parent;
		if (m_role == "HP") {
			m_max = m_target.maxHP;
			m_current = m_target.currentHP;
			m_colorPath = "Texture/green";
		}

		if (m_role == "MP") {
	//		m_max = m_target.maxMP;
	//		m_current = m_target.currentMP;
			m_colorPath = "Texture/blue";
		}

		if (m_role == "AP") {
	//		m_max = m_target.maxMP;
	//		m_current = m_target.currentMP;
			m_colorPath = "Texture/blue";
		}

		if (m_colorPath != "")
			m_color = Resources.Load (m_colorPath) as Texture;
	}

	void OnGUI(){
		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());
		GUI.depth = depth;
		GUI.color = new Color(1,1,1,alpha);
		GUI.DrawTexture (m_DrawArea,  m_texture);
		GUI.DrawTexture (barFillPercent (), m_color);
		GUI.color = Color.white;
		if(m_parent != null) GUILayout.EndArea ();

	}

	// Update is called once per frame
	void Update () {
		if (m_role == "HP")  m_current = m_target.currentHP;
		if (m_role == "MP")  m_current = m_target.currentAP;
		if (m_role == "AP")  m_current = m_target.currentMP;
		if (m_current < 0)
			m_current = 0;
		multiplier = m_current / m_max;
		alpha = m_target.alpha;
	}

	//Return size of current % of value left in the bar.
	public Rect barFillPercent(){

		Rect newRect = m_barFillRect;
		newRect.width = m_barFillRect.width * multiplier;

		return newRect;
	}
}
