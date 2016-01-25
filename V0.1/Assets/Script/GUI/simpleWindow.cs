using UnityEngine;
using System.Collections;

//This script is for BASIC PICTURE IN THE BACKGROUND!!! IF you want to add something to the background, add this to the GUI.

public class simpleWindow : MonoBehaviour {
	
	//basic informations
	//If you place this on a BG, add the BG into the script so that it can use
	//the GUI Layout for auto scale and repositioning.
	public simpleWindow m_parent = null;
	public string m_texturePath;
	public Texture m_texture;
	public Rect m_DrawArea;
	public int depth = 10;	
	public float speed = 10;
	//For sound
	private AudioSource source;
	public string audioPath;
	public AudioClip audioSE;

	// Load texture and use it as background
	void Start () {
		if (m_texturePath != "")
			m_texture = Resources.Load (m_texturePath) as Texture;
		m_DrawArea = resizeGUI (m_DrawArea);

		if (audioPath != "") {
			source = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
		}
	}

	void OnGUI(){

		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());

		GUI.depth = depth;
		GUI.DrawTexture (m_DrawArea, m_texture);

		if(m_parent != null) GUILayout.EndArea ();
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

	public void playSound(){
		source.PlayOneShot (audioSE);
	}	
}
