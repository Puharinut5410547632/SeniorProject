using UnityEngine;
using System.Collections;

public class titleScreen : MonoBehaviour {

	public fadeableWindow tapButton;
	public fadeableWindow fadeScene;
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


	// Use this for initialization
	void Start () {
		m_state = State.START;
		buttonStatus = 2;
		StartCoroutine (beginFirstFadeIn (1.5f)); 
	}
	
	// Update is called once per frame
	void Update () {
		if(m_state == State.WAIT) fadeButton ();
		if(m_state == State.PROCEED) blinkButton ();

		if (m_state == State.WAIT) {
			if (Input.touchCount > 0 || Input.GetMouseButton (0)) {
				m_state = State.PROCEED;
				StartCoroutine (goToNextScene (1.5f));
			}
		}
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
