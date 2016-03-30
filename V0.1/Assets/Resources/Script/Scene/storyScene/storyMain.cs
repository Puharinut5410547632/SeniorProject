using UnityEngine;
using System.Collections;

public class storyMain : MonoBehaviour {

	public int directionStep; //Determine where the current placement is in the text.
	public int lastStep; //Obtained from reading the file. Last step to end the thing.
	public bool tapTrigger; // So that you can only tap the thing
	public bool preTap; // Check the tapping in order to finish text box
	public textBox m_textBox; //The text box
	public string curChapter; // Get from player prefs. curChapter depends on what the story button is pressed.
	public fadeableWindow m_background; // the picture. THis will change on changeBackground() command
	public simpleWindow m_oldBackground; // Old background picture for transitioning.
	//Story file reader
	public CStringList chapterFile = null;

	//Current step from the file
	public State m_state;
	public enum State
	{
		TEXT, //The story is processing a text related file.
		NOTTEXT, //The story is altering non-text stuffs.
	};

	void Start () {
		directionStep = 0;
		curChapter = PlayerPrefs.GetString ("StoryChapter");
		storyReader ();
		//Begin 1st direction
		nextDirection ();
	}

	void Update () {

		//Working with text box
		if (m_state == State.TEXT) {
			Debug.Log (preTap);
			//Text is not loaded yet but you're already touching the screen to force full text completion
			if (!m_textBox.isFullText () && detectTouch ()) {
				preTap = true;
				m_textBox.finishText ();
			}


			//tapping the screen within a certain area should proceed to the next step
			if (m_textBox.isFullText () && preTap == false) {
				//Detect Touch and trigger the next direction requirement
				if (detectTouch ()) {
					Debug.Log ("STORY TOUCH");
					tapTrigger = true;
				}
				//Release touch and prevent next direction requirement
				if (!detectTouch () && tapTrigger == true) {
					Debug.Log ("NEXT STEP");
					tapTrigger = false;
					nextDirection ();
				}
			}

			//Release key to remove pretap
			if (preTap == true) {
				if (!detectTouch ())
					preTap = false;
			}
		}
	}
		
	public bool detectTouch(){
		return (Input.touchCount > 0 || Input.GetMouseButton (0));
	}

	public void setText(string fullText){
		m_textBox.createText (fullText);
	}

	public CStringList loadStory(string chapter ){
		
		CStringList storyFile = new CStringList();
		
		storyFile.Load ("Story/" + chapter);


		return storyFile;
	}

	public void getFileLength(CStringList file){
		int index;
		int lastIndex = 0;;
		for(index = 0; index < 200; index++){
//			Debug.Log ("mStory" + (index+1) + "_action"); //Check key name
			//get Key until it runs into the one returning a 0 which isn't a key
			if(file.GetText ("mStory" + (index+1) + "_action") == "0" ) {
				lastIndex = index;
				break;
			}

//			Debug.Log (file.GetText("mStory" + (index+1) + "_action"));
		}
		//Save last step as the current index
		Debug.Log (lastIndex);
		lastStep = lastIndex;
	}

	public void storyReader(){
		Debug.Log ("<color=green>Loading story</color>");
		//Load file based on chapter file
		chapterFile = loadStory (curChapter);
		getFileLength (chapterFile);
		//Get the keys from the text file
		//p_name = charFile.GetText ("mChar_name");
	}

	public void doAction(){
		if (directionStep == lastStep+1) {
			Debug.Log ("Already finished the last step");

		}

		if (directionStep != lastStep+1) {
			string action = chapterFile.GetText ("mStory" + (directionStep) + "_action");
			//change Background
			if(action == "changeBackground"){
				StartCoroutine( changeBackground ()) ;
				m_state = State.NOTTEXT;
			}

			if(action == "changeText"){
				createText ();
				m_state = State.TEXT;
			}

			if(action == "beginBattle"){
				beginBattle ();
			}
			if(action == "endStory"){
				returnToPlay();
			}
		}

	}

	public void createText(){
		m_textBox.createCharText(chapterFile.GetText ("mStory" + (directionStep) + "_char"));
		m_textBox.createText (chapterFile.GetText ("mStory" + (directionStep) + "_text"));
	}

	public void beginBattle(){
		PlayerPrefs.SetString("BattleID", chapterFile.GetText ("mStory" + (directionStep) + "_battleID"));
		GameObject obj = Instantiate(Resources.Load ("Prefab/sceneChangeButton")) as GameObject;
		sceneChangeButton button = obj.GetComponent<sceneChangeButton> ();
		button.StartCoroutine (button.goToNextScene(0.2f,5));
	}

	public void returnToPlay(){
		GameObject obj = Instantiate(Resources.Load ("Prefab/sceneChangeButton")) as GameObject;
		sceneChangeButton button = obj.GetComponent<sceneChangeButton> ();
		button.StartCoroutine (button.goToNextScene(0.2f,4));
	}

	public IEnumerator changeBackground(){
		//Switch the old background in first
		string path = chapterFile.GetText ("mStory" + (directionStep) + "_BG");
		Debug.Log ("yeah");
		float delay = float.Parse (chapterFile.GetText("mStory" + (directionStep) + "_BGDelay") );
		Debug.Log ("Hayeah");
		m_oldBackground.changeTexture (m_background.m_texturePath);
		//Hide the current background
		m_background.alpha = 0.0f;
		//Switch current background picture with the new one and have it reappear
		m_background.changeTexture (path);
		m_background.alphaspeed = 0.03f;
		m_background.fadeIn();
		yield return new WaitForSeconds (delay);
		nextDirection ();

	}

	public void nextDirection(){
		directionStep++;
		doAction ();
	}
}
