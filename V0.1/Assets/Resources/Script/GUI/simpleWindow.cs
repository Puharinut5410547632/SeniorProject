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
	public float maxWidth = 1080.00f;
	public float maxHeight = 1920.00f;
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
		setDefault ();
		if (audioPath != "") {
			source = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
		}
	}

	public void doStart(){
		if (m_texturePath != "")
			m_texture = Resources.Load (m_texturePath) as Texture;

		setDefault ();
		if (audioPath != "") {
			source = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
		}
	}

	void OnGUI(){

		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());

		if (m_texturePath != "" && m_texture == null)
			m_texture = Resources.Load (m_texturePath) as Texture;
		setLocation ();
		GUI.depth = depth;
		GUI.DrawTexture (resizeGUI (m_DrawArea), m_texture);

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
			if(speedX > 0 && m_DrawArea.x < targetX) m_DrawArea.x = targetX;
		}
		
		if (m_DrawArea.y <= targetY) {
			m_DrawArea.y += speedY;
		}
		
		if (m_DrawArea.y > targetY) {
			m_DrawArea.y -= speedY;
		}
	}

	public float recalculateX( float x){
		
		return x * Screen.width / maxWidth;
	}
	
	public float recalculateY( float y){
		
		return y * Screen.height / maxHeight;
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
		return resizeGUI(m_DrawArea);
	}

	public void playSound(){
		source.PlayOneShot (audioSE);
	}	

	public void playSound(string path){
		audioPath = path;
		audioSE = Resources.Load (audioPath) as AudioClip;
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

	public void playSoundLoop(string path){
		audioPath = path;
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
		m_DrawArea.x = x;
	}

	public void setLocationY(float y){
		m_DrawArea.x = y;
	}

	public void setTargetX(float x){
		targetX = x;
	}
	
	public void setTargetY(float y){
		targetY = y;
	}

	public void recalculateSpeedX(float newSpeed){
		float widthScale = Screen.width / maxWidth;
		speedX = widthScale * newSpeed;
	}

	public void changeTexture(string path){
		m_texturePath = path;
		m_texture = Resources.Load (path) as Texture;
	}

}
