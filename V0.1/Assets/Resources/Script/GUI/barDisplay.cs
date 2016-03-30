	using UnityEngine;
using System.Collections;

public class barDisplay : simpleWindow {

	public Character m_target;
	public string m_role;
	public float m_max;
	public float m_current;
	public float multiplier;
	public float alpha;
	public fadeableWindow bar;
	// Use this for initialization
	void Start () {
		if ( m_target.m_enemyLabel != null)	m_DrawArea.x  = m_target.m_DrawArea.x;
		doStart ();
		m_parent = m_target.m_parent;
		bar.m_parent = m_parent;

		if (m_role == "HP") {
			m_max = m_target.maxHP;
			m_current = m_target.currentHP;
			m_texturePath = "Texture/green";
		}

		if (m_role == "AP") {
			m_max = m_target.maxAP;
			m_current = m_target.currentAP;
			m_texturePath = "Texture/blue";
		}

		if (m_texturePath != "")
			m_texture = Resources.Load (m_texturePath) as Texture;
	}

	void OnGUI(){
		//In case of using this on enemy
		if (m_target.m_enemyLabel != null) {
			m_DrawArea.x = m_target.m_enemyLabel.m_DrawArea.x;
			m_DrawArea.width = m_target.m_enemyLabel.widthPartySize ();
		}

		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());
		GUI.depth = depth;
		GUI.color = new Color(1,1,1,alpha);
		GUI.DrawTexture (resizeGUI(barFillPercent ()), m_texture);
		GUI.color = Color.white;
		if(m_parent != null) GUILayout.EndArea ();

	}

	// Update is called once per frame
	void Update () {
		bar.m_DrawArea = m_DrawArea;
		bar.alpha = alpha;
		if (m_role == "HP") {
			m_max = m_target.maxHP;
			m_current = m_target.currentHP;
		}
		if (m_role == "AP") {
			m_max = m_target.maxAP;
			m_current = m_target.currentAP;
		}
		if (m_current < 0)
			m_current = 0;
		multiplier = m_current / m_max;
		alpha = m_target.alpha;
	}

	//Return size of current % of value left in the bar.
	public Rect barFillPercent(){

		Rect newRect = m_DrawArea;
		newRect.width = m_DrawArea.width * multiplier;

		return newRect;
	}
}
