using UnityEngine;
using System.Collections;

//This script is for BASIC PICTURE IN THE BACKGROUND!!! IF you want to add something to the background, add this to the GUI.

public class simpleWindow : MonoBehaviour {
	
	//basic informations
	public string m_texturePath;
	public Texture m_texture;
	public Rect m_DrawArea;
	public int depth = 10;	
	public float speed = 10;
	
	// Load texture and use it as background
	void Start () {
		if (m_texturePath != "")
			m_texture = Resources.Load (m_texturePath) as Texture;
		m_DrawArea = resizeGUI (m_DrawArea);

	}

	void OnGUI(){
		GUI.depth = depth;
		GUI.DrawTexture (m_DrawArea, m_texture);
	}


	public Rect resizeGUI(Rect Drect){
	
		float widthScale = Screen.width / 1600.00f;
		float heightScale = Screen.height / 2560.000f;

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
}
