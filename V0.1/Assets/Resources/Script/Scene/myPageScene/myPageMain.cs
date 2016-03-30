using UnityEngine;
using System.Collections;

public class myPageMain : MonoBehaviour {

	public commandButton m_char1;
	public commandButton m_char2;
	public commandButton m_char3;
	public commandButton m_support;
	public simpleWindow m_background;
	public sceneChangeButton m_playButton;
	public sceneChangeButton m_teamButton;
	public simpleLabel m_label;

	//For sound
	private AudioSource source;
	public string audioPath;
	public AudioClip audioSE;

	public State m_State;
	public enum State
	{
		NORMAL,		// Display Everything
		STATBOX,	// Has a statbox in front
	};

	// Use this for initialization
	void Start () {
	
		//Change the banner in the middle to represent current team characters
		m_char1.m_texturePath = ("Texture/myPage/Splash/" + PlayerPrefs.GetString ("Char1"));
		m_char1.doStart ();
		m_char1.charNumber = 1;

		m_char2.m_texturePath = ("Texture/myPage/Splash/" + PlayerPrefs.GetString ("Char2"));
		m_char2.doStart ();
		m_char2.charNumber = 2;

		m_char3.m_texturePath = ("Texture/myPage/Splash/" + PlayerPrefs.GetString ("Char3"));
		m_char3.doStart ();
		m_char3.charNumber = 3;

		m_support.m_texturePath = ("Texture/myPage/Splash/Support/" + PlayerPrefs.GetString ("Char4"));
		m_support.doStart ();
		m_support.charNumber = 4;

		if (audioPath != "") {
			source = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
			source.clip = audioSE;
			source.Play ();
			source.loop = true;
		}

	}

	//This is so that you can view the stat box and not be able to press the scene change button.
	//
	public void disableButtons(bool press){
		m_playButton.buttonPressed = press;
		m_teamButton.buttonPressed = press;
		m_char1.buttonPressed = press;
		m_char2.buttonPressed = press;
		m_char3.buttonPressed = press;
		m_support.buttonPressed = press;
	}
	public void closeGame(){
//		Debug.Log ("Exit Game");
		Application.Quit();
	}

	public void displayStatus(int i){
		disableButtons (true);
		GameObject statusBox = Instantiate(Resources.Load ("Prefab/StatusBox")) as GameObject;
		statusBoxMain status = statusBox.GetComponent<statusBoxMain> ();
		status.page = this;
		Debug.Log (PlayerPrefs.GetString ("Char" + i));
	}

}
