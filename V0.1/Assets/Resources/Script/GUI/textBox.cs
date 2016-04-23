using UnityEngine;
using System.Collections;

public class textBox : MonoBehaviour {

	//Condition for textbox that is running
	public bool runningText;

	public string fullText;
	public string currentText;
	public string currentCharName;
	public string oldCharName;
	public int textPos;
	public char[] textChar;

	public simpleLabel text;
	public simpleLabel charName;

	public bool waitSwap;
	public simpleWindow textPlate;
	public fadeableWindow textArea;
	public fadeableWindow nextButton;
	public fadeableWindow characterArt;

	public AudioSource source; // for typing sfx
	public string audioPath;
	public AudioClip audioSE;

	public State m_state;
	public enum State
	{
		HASTEXT, //In case the fulltext isn't empty, start to print text
		FULLTEXT, //In case the current text is already the printed full text
		NOTEXT, //In case the full text is empty, hide textbox
	};

	// Use this for initialization
	void Start () {
		characterArt.alphaspeed = 0.1f;
//		fullText = "";
//		Debug.Log (fullText.ToCharArray ().Length);
//		currentText = "";
//		currentCharName = "";
		runningText = false;
//		StartCoroutine (test ());
	}

	public IEnumerator test(){
		yield return new WaitForSeconds (1.0f);
		createText("If I have to blame someone, it will have to be you, Enfys.");
		createCharText("Volfram");

		yield return new WaitForSeconds (5.0f);
		createText("Sure, sure. It always has to be me.");
		createCharText("Enfys");

	}
	
	// Update is called once per frame
	void Update () {
	
//		//Hide text box
//		if (m_state == State.NOTEXT) {
//			nextButton.alpha = 0.0f;
//			hideTextBox();
//		}

		if (m_state == State.HASTEXT) {
			nextButton.alpha = 0.0f;
		//	if(runningText == false){  //Have the storyMain call setText instead.
		//	StartCoroutine(readText ());
		//	}

	//		if (textTouch == true && (Input.touchCount > 0 || Input.GetMouseButton (0) )) {
	//			Debug.Log ("TOUCHING");
	//			finishText ();
	//		}

		}

		if (m_state == State.FULLTEXT) {
			if( fullText != "") nextButton.alpha = 1.0f;
		}
	}

	public void hideTextBox(){
		setPicture ();
	}

	//Tap the screen while the text is loading to skip it right away
	public void skipToFullText(){
		m_state = State.FULLTEXT;
		currentText = fullText;
	}

	public bool isFullText(){
		return m_state == State.FULLTEXT;
	}

	public void setCharText(){
		charName.label = "<color=white>" + currentCharName + "</color>";
	}

	public void setText(){
		text.label =  "<color=white>" +currentText + "</color>";
	}

	//For when you tap the thing to finish the text.
	public void finishText(){
		textPos = textChar.Length - 1;
		currentText = fullText;

		setText ();
		m_state = State.FULLTEXT;
		runningText = false;
	}

	public void createCharText(string name){
		currentCharName = name;
		setCharText ();
		setPicture ();
	}
	public void createText(string text){
		StartCoroutine (readText (text));
	}
	public IEnumerator readText(string text){
	
		//Prevent this from being called again
		currentText = "";
		fullText = text;
		m_state = State.HASTEXT;


		yield return new WaitForSeconds(0.3f);
		runningText = true;
		//Reprint the whole thing from start.

		textChar = fullText.ToCharArray ();

		for (textPos = 0; textPos < textChar.Length; textPos++) {
			currentText = currentText + textChar[textPos];
			setText();
			yield return new WaitForSeconds(0.03f);
		}
		if (textPos == textChar.Length)
			finishText();
	}

	public void newText(){
		currentText = "";
		textPos = 0;
		textChar = fullText.ToCharArray ();
		setPicture ();
	}

	public void setPicture(){
		//For when no one is speaking, picture will fade out.
		if (currentCharName == "")
			characterArt.fadeOut ();
		
		//The name is the same so just use the same picture
		if (currentCharName == oldCharName && currentCharName != "") 
		{
			characterArt.m_texturePath = "Character/" + currentCharName + "/dialogue";
			characterArt.changeTexture(characterArt.m_texturePath);}


		//The name is different so switch picture
		if (currentCharName != oldCharName) {
			oldCharName = currentCharName;
			StartCoroutine(swapPicture ());
		}

	}
	
	public IEnumerator swapPicture(){
		//Will only fade the picture at first if there's a picture to begin with
		//If not, just skip right into fading in
		if (characterArt.m_texture == null) {
			characterArt.m_texturePath = "Character/" + currentCharName + "/dialogue";
			characterArt.changeTexture(characterArt.m_texturePath);
			characterArt.fadeIn ();
		}
		//There are two different names so fade out first then load after a few second
		else {
			characterArt.fadeOut ();
			yield return new WaitForSeconds(0.7f);
			characterArt.m_texturePath = "Character/" + currentCharName + "/dialogue";
			characterArt.changeTexture(characterArt.m_texturePath);
			Debug.Log ("Hey");
			characterArt.fadeIn ();
		}
	}

	public void playSound(){
		source.PlayOneShot (audioSE);
	}	
	
	public void playSound(string path){
		audioPath = path;
		audioSE = Resources.Load (audioPath) as AudioClip;
		source.PlayOneShot (audioSE);
	}	
}
