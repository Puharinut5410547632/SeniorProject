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
	public AudioSource source;
	public string audioPath;
	public AudioClip audioSE;

	// Movement related

	public float targetX {get; set;}
	public float targetY {get; set;}
	public float defaultX {get; set;}
	public float defaultY {get; set;}
	public float speedX;
	public float speedY;

	// Load texture and use it as background
	void Start () {
		if (m_texturePath != "")
			m_texture = Resources.Load (m_texturePath) as Texture;
		m_DrawArea = resizeGUI (m_DrawArea);
		setDefault ();
		recalculateSpeedX (speed);
		if (audioPath != "") {
			source = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
		}
	}

	public void doStart(){
		if (m_texturePath != "")
			m_texture = Resources.Load (m_texturePath) as Texture;
		m_DrawArea = resizeGUI (m_DrawArea);
		setDefault ();
		recalculateSpeedX (speed);
		if (audioPath != "") {
			source = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
		}
	}

	void OnGUI(){

		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());
		setLocation ();
		GUI.depth = depth;
		GUI.DrawTexture (m_DrawArea, m_texture);

		if(m_parent != null) GUILayout.EndArea ();
	}

	public void setDefault(){
		defaultX = m_DrawArea.x;
		defaultY = m_DrawArea.y;

	}

	//Move the texture around on death or on attack.
	public void setLocation(){
		if (m_DrawArea.x <= targetX) {
			m_DrawArea.x += speedX;
		}
		
		if (m_DrawArea.x > targetX) {
			m_DrawArea.x -= speedX;
		}
		
		if (m_DrawArea.y <= targetY) {
			m_DrawArea.y += speedY;
		}
		
		if (m_DrawArea.y > targetY) {
			m_DrawArea.y -= speedY;
		}
	}

	public float recalculateX( float x){
		
		return x*Screen.width / 1600.00f;
	}
	
	public float recalculateY( float y){
		
		return y*Screen.height / 2560.000f;
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

	public void playSound(){
		source.PlayOneShot (audioSE);
	}	

	public void playSoundLoop(){

		if (audioPath != "") {
			source = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
			source.clip = audioSE;
			source.Play ();
			source.loop = true;
		}

	}

	public void endSound(){
		source.Stop ();
	}

	public void setLocationX(float x){
		m_DrawArea.x = recalculateX (x);
	}

	public void setLocationY(float y){
		m_DrawArea.x = recalculateX (y);
	}

	public void setTargetX(float x){
		targetX = recalculateX (x);
	}
	
	public void setTargetY(float y){
		targetY = recalculateX (y);
	}

	public void recalculateSpeedX(float newSpeed){
		float widthScale = Screen.width / 800.00f;
		speedX = widthScale * newSpeed;
	}

}
