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

	public commandButton skipButton;
	public sceneChangeButton sceneButton;

	//Story file reader
	public CStringList chapterFile = null;

	//For song
	public AudioSource songSource; // for song
	public AudioSource sfxSource; // for sfx
	public string audioPath;
	public AudioClip audioSE;

	//Current step from the file
	public State m_state;
	public enum State
	{
		TEXT, //The story is processing a text related file.
		NOTTEXT, //The story is altering non-text stuffs.
		SKIPPING, //Skip button pressed
	};

	void Start () {
		directionStep = 0;
		curChapter = PlayerPrefs.GetString ("PickedChapter"); //Use picked chapter to determine what will be played.
		Debug.Log (curChapter);
		//		curChapter = "0"; // For tesing purpose. Toggle this off later when playing actual story.
//		curChapter = "Prologue"; // Testing purpose # 2
		StartCoroutine (startStory ());

	}

	public IEnumerator startStory(){
		yield return new WaitForSeconds (1.5f);
		storyReader ();
		//Begin 1st direction
		nextDirection ();
		skipButton.gameObject.SetActive (true);
	}

	void Update () {

		//Working with text box
		if (m_state == State.TEXT) {
			//Text is not loaded yet but you're already touching the screen to force full text completion
			if (!m_textBox.isFullText () && detectTouch ()) {
				if(preTap == false && m_textBox.runningText) m_textBox.finishText ();
				preTap = true;

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

	public void skip(){
//		Debug.Log ("skipping");
		skipButton.buttonPressed = true;
		sceneButton.buttonPressed = true;
		StopAllCoroutines ();
		m_state = State.SKIPPING;
		//Only do either end story, begin battle or clear chapter
		for(int i = directionStep; i<lastStep+1; i++){
			string action = chapterFile.GetText ("mStory" + (i) + "_action");
	//		Debug.Log (i);
	//		Debug.Log (action);

			if(action == "beginBattle"){
				Debug.Log ("This");
		//		Debug.Log (directionStep);
				skipButton.buttonPressed = true;
				sceneButton.buttonPressed = true;
				directionStep = i;
				beginBattle ();
			}

			if(action == "clearChapter"){
				//clear is current chapter
				directionStep = i;
				int clear = int.Parse(chapterFile.GetText ("mStory" + (directionStep) + "_clear"));
				//Increase chapter value
				clear++;
				//Save to unlock next chapter
				if(PlayerPrefs.GetInt ("StoryChapter") < clear) PlayerPrefs.SetInt ("StoryChapter", clear);
			}

			if(action == "endStory"){
				Debug.Log ("This 2");
				skipButton.buttonPressed = true;
				sceneButton.buttonPressed = true;
				returnToPlay();
			}

		

		}

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
				skipButton.buttonPressed = true;
				sceneButton.buttonPressed = true;
				beginBattle ();
			}
			if(action == "endStory"){
				skipButton.buttonPressed = true;
				sceneButton.buttonPressed = true;
				returnToPlay();
			}

			if(action == "changeSong"){
				playSoundLoop(chapterFile.GetText ("mStory" + (directionStep) + "_song"));
				m_state = State.NOTTEXT;
				nextDirection ();
			}

			if(action == "changeSFX"){
				playSound(chapterFile.GetText ("mStory" + (directionStep) + "_sfx"));
				m_state = State.NOTTEXT;
				nextDirection ();
			}

			if(action == "clearChapter"){
				//clear is current chapter
				int clear = int.Parse(chapterFile.GetText ("mStory" + (directionStep) + "_clear"));
				//Increase chapter value
				clear++;
				//Save to unlock next chapter
				if(PlayerPrefs.GetInt ("StoryChapter") < clear) PlayerPrefs.SetInt ("StoryChapter", clear);
			}
		}

	}

	public void playSound(){
		sfxSource.PlayOneShot (audioSE);
	}	
	
	public void playSound(string path){
		audioPath = path;
		audioSE = Resources.Load (audioPath) as AudioClip;
		sfxSource.PlayOneShot (audioSE);
	}	
	
	public void playSoundLoop(){
		
		if (audioPath != "") {
			songSource = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
			songSource.clip = audioSE;
			songSource.Play ();
			songSource.loop = true;
		}
		
	}
	
	public void playSoundLoop(string path){
		audioPath = path;
		if (audioPath != "") {
			songSource = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
			songSource.clip = audioSE;
			songSource.Play ();
			songSource.loop = true;
		}
		
	}

	public void createText(){
		m_textBox.createCharText(chapterFile.GetText ("mStory" + (directionStep) + "_char"));
		m_textBox.createText (chapterFile.GetText ("mStory" + (directionStep) + "_text"));
	}

	public void beginBattle(){
		Debug.Log (directionStep);
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
		float delay = float.Parse (chapterFile.GetText("mStory" + (directionStep) + "_BGDelay") );
//		Debug.Log ("Get DELAY " + delay);
		m_oldBackground.changeTexture (m_background.m_texturePath);
		//Hide the current background
		m_background.alpha = 0.0f;
		//Switch current background picture with the new one and have it reappear
		m_background.changeTexture (path);
		m_background.alphaspeed = 0.03f;
		m_background.fadeIn();
		yield return new WaitForSeconds (delay);
		Debug.Log (path);
		Debug.Log (delay);
		nextDirection ();

	}

	public void nextDirection(){
		directionStep++;
		doAction ();
	}

	public void skipStory(){
		directionStep = lastStep;
		doAction ();
	}
}
