﻿using UnityEngine;
using System.Collections;

public class titleScreen : MonoBehaviour {

	public fadeableWindow tapButton;
	public fadeableWindow fadeScene;
	public simpleLabel compatibleLabel;
	public int buttonStatus {get; set;} //0 = fade in, 1 = fade out
	public State m_state;
	public bool changeFade {get;set;}
	public enum State
	{
		START, // Not able to do anything yet. IT's loading
		WAIT, //Idle state. No screen tap yet
		PROCEED, //Screen tapped. Button blinking for a second
		FADE, //Screen fades to black.
		END, //Change scene. To 1 if it's a new user. To 2 if it's an old user.
	};
	
	//For sound
	private AudioSource source;
	public string audioPath;
	public AudioClip audioSE;


	// Use this for initialization
	void Start () {

		checkCompability ();

		m_state = State.START;

		if(!PlayerPrefs.HasKey("Char1")){
			PlayerPrefs.SetString("Char1", "Enfys");
		}

		if(!PlayerPrefs.HasKey("Char2")){
			PlayerPrefs.SetString("Char2", "Volfram");
		}

		if(!PlayerPrefs.HasKey("Char3")){
			PlayerPrefs.SetString("Char3", "Gwenette");
		}

		if(!PlayerPrefs.HasKey("Char4")){
			PlayerPrefs.SetString("Char4", "Liel");
		}

		if(!PlayerPrefs.HasKey("StoryChapter")){
			PlayerPrefs.SetInt("StoryChapter", 0);
		}

		if(!PlayerPrefs.HasKey("PickedChapter")){
			PlayerPrefs.SetString("PickedChapter", "0");
		}

		if(!PlayerPrefs.HasKey("BattleID")){
			PlayerPrefs.SetString("BattleID", "0");
		}
		buttonStatus = 2;
		StartCoroutine (beginFirstFadeIn (1.5f)); 

		if (audioPath != "") {
			source = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
			source.clip = audioSE;
			source.Play ();
			source.loop = true;
		}

	}
	
	// Update is called once per frame
	void Update () {
		if(m_state == State.WAIT) fadeButton ();
		if(m_state == State.PROCEED) blinkButton ();

		if (m_state == State.WAIT) {
			if (Input.touchCount > 0 || Input.GetMouseButton (0)) {
				tapButton.alpha = 1.0f;
				tapButton.playSound ();
				m_state = State.PROCEED;
				StartCoroutine (goToNextScene (1.5f));
			}
		}
	}

	public void checkCompability(){

		float width = Screen.width;
		float height = Screen.height;
		if (width > 1080.0f)
			compatibleLabel.label = "<color=red> This device is not fully supported.</color>";
		if (height > 1920.0f)
			compatibleLabel.label = "<color=red> This device is not fully supported.</color>";
		if (height/width < 1.2f)
			compatibleLabel.label = "<color=red> This device is not fully supported.</color>";
	}

	public IEnumerator goToNextScene(float seconds){
		yield return new WaitForSeconds(seconds);
		m_state = State.FADE;
		fadeScene.fadeIn ();
		yield return new WaitForSeconds(0.5f);
		m_state = State.END;
		Debug.Log ("MOVING ON TO NEXT SCENE");
		nextScene ();
	}

	public void nextScene(){
		//Switch to next scene. 1 is for new user and 2 is for HOME.
		Application.LoadLevel (2);
	}

	//Do this right after tapping the screen before changing to FADEOUT to change scene
	public void blinkButton(){
		tapButton.blinkSpeed ();
		if (buttonStatus == 0 && changeFade == true) {
			changeFade = false;
			StartCoroutine (beginFadeOut (0.1f));
		}
		//Call it to reappear.
		if (buttonStatus == 1 && changeFade == true){
			changeFade = false;
			StartCoroutine (beginFadeIn (0.1f));
		}
	}

	//Default speed while waiting for the screen to be tapped
	public void fadeButton(){
	
		//Is on the screen and needs to go back to hiding.
		if (buttonStatus == 0 && changeFade == true) {
			changeFade = false;
			StartCoroutine (beginFadeOut (0.6f));
		}
		//Call it to reappear.
		if (buttonStatus == 1 && changeFade == true){
			changeFade = false;
			StartCoroutine (beginFadeIn (0.6f));
		}
	}

	//Cause the tapbuton to fade in
	public IEnumerator beginFadeIn(float seconds){
		yield return new WaitForSeconds(seconds);
		tapButton.fadeIn ();
		buttonStatus = 0;
		changeFade = true;
	}

	//Cause the tapbuton to fade out
	public IEnumerator beginFadeOut(float seconds){
		yield return new WaitForSeconds(seconds);
		tapButton.fadeOut ();
		buttonStatus = 1;
		changeFade = true;
	}

	//For the first fading to change state to "Start" so you can tap the screen
	public IEnumerator beginFirstFadeIn(float seconds){
		StartCoroutine (beginFadeIn (2.0f)); 
		yield return new WaitForSeconds(seconds);
		m_state = State.WAIT;
	}
}
