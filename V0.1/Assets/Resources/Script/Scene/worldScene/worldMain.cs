using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class worldMain : MonoBehaviour {

	public simpleWindow m_background;
	public fadeableWindow m_storyBackground;
	public sceneChangeButton m_homeButton;
	public commandButton m_pickStory;
	public commandButton m_pickFreeMode;
	public commandButton m_backButton;
	//Team buttons
	public commandButton m_Gine;

	//Unlock story
	public CStringList storyList = null;
	//Each Map location (Add this when you have more in the future.
	//Array List for saving location and button to use them
	public List<string> BoundBeyondTime;
	public commandButton BoundButton;
	public List<string> CelentineForest;
	public commandButton CelenButton;
	public List<string> RichtofenCastle;
	public commandButton RichtoButton;
	
	public List<chapterButton> m_chapters;

	//For Demo

	public commandButton demoButton;
	//state of current game picking
	public State m_state;
	public enum State
	{
		PICKTYPE, // Picking story or free mode
		STORY, //Story picking
		FREEMODE, // Free mode picking
		PICKINGLOCATION, // Picking a chapter
		PICKINGCHAPTER,
	};

	public TeamState m_team;
	public enum TeamState
	{
		GINE, //TEAM NAME
		VALOR, //
		ORIXIAN, //
		FARLAI,
	};

	// Use this for initialization
	void Start () {
		pickOption ();
	}
	
	// In this update, buttons will move into proper spot.
	void Update () {
	
	}

	public CStringList loadStory(string team ){
		
		CStringList storyFile = new CStringList();
		storyFile.Load ("Story/" +team+"Progress");
		return storyFile;
	}

	public void pickOption(){ 
		m_homeButton.gameObject.SetActive (true);
		m_backButton.buttonPressed = true;

		m_pickStory.targetX = 150.0f;
		m_pickStory.buttonPressed = false;


		m_Gine.targetX = 1200.0f;
		m_Gine.speedX = 30.0f;
		m_Gine.buttonPressed = true;

		m_state = State.PICKTYPE;

		//Demo Section
		demoButton.targetX = 150.0f;
		demoButton.buttonPressed = false;
	}

	public void pickStory(){
//		Debug.Log ("huh");
		m_homeButton.gameObject.SetActive (false);
		m_backButton.buttonPressed = false;

		m_pickStory.targetX = -900.0f;
		m_pickStory.speedX = 30.0f;
		m_pickStory.buttonPressed = true;

		m_Gine.targetX = 150.0f;
		m_Gine.speedX = 30.0f;
		m_Gine.buttonPressed = false;

		m_state = State.STORY;

		//Demo Section

		demoButton.targetX = -900.0f;
		demoButton.speedX = 30.0f;
		demoButton.buttonPressed = true;
	}

	public void pickFreeMode(){
		m_state = State.FREEMODE;
	}

	//Go back option
	public void backOption(){
		if (m_state == State.STORY) {
			pickOption();
		}
		if (m_state == State.PICKINGLOCATION) {
			disableWorldIconButton();
			pickStory ();
		}
		if (m_state == State.PICKINGCHAPTER) {
			clearChapterButton();
			pickingLocation();
		}
	}

	public void pickingLocation(){
		m_state = State.PICKINGLOCATION;
		enableAllWorldIcon();
		Debug.Log ("Fumikage Tokayami");
	}

	public void clearChapterButton(){
		for (int i = 0; i< m_chapters.Count; i++) {
			m_chapters[i].removeButton();
		}
		m_chapters.Clear ();
	}

	public void pickTeam(string name){

		m_Gine.buttonPressed = true;

		if (name == "Gine") {
			m_team = TeamState.GINE;
			readTeamStory ("Gine");
		}
		if (name == "Orixian") {
			m_team = TeamState.ORIXIAN;
			readTeamStory ("Orixian");
		}
		if (name == "Valor") {
			m_team = TeamState.VALOR;
			readTeamStory ("Valor");
		}
		if (name == "Farlai") {
			m_team = TeamState.FARLAI;
			readTeamStory ("Farlai");
		}
	}

	public void readTeamStory(string name){
		clearList ();
		prepareStory (name);
//		BoundBeyondTime.Add ("Prologue"); //For testing purpose
		m_Gine.targetX = -900.0f;
		StartCoroutine (loadMapButton());
	}

	public void prepareStory(string name){
		storyList = loadStory (name);
		prepareChapter (storyList);
	}

	public void prepareChapter(CStringList file){
		for (int index = 0; index < (PlayerPrefs.GetInt("StoryChapter")+1); index++) {
			//End of file
			if (file.GetText ("mStory" + (index + 1) + "_chapter") == "0") {
				break;
			}


			//Read location and add stuffs accordingly
			if(file.GetText ("mStory" + (index + 1) + "_location") == "BoundBeyondTime"){
				BoundBeyondTime.Add (file.GetText ("mStory" + (index + 1) + "_chapter"));
			}

			if(file.GetText ("mStory" + (index + 1) + "_location") == "CelentineForest"){
				CelentineForest.Add (file.GetText ("mStory" + (index + 1) + "_chapter"));
			}

			if(file.GetText ("mStory" + (index + 1) + "_location") == "RichtofenCastle"){
				RichtofenCastle.Add (file.GetText ("mStory" + (index + 1) + "_chapter"));
			}
		}
	}

	public IEnumerator loadMapButton(){
		yield return new WaitForSeconds (0.75f);
		//Check to make sure the array is not empty, then add a button
		pickingLocation ();
		enableWorldIconButton ();
	}

	public void enableWorldIconButton(){
		Debug.Log ("hey");
		if (BoundButton.isActiveAndEnabled == false && BoundBeyondTime.Count > 0)
			BoundButton.gameObject.SetActive (true);

		if (CelenButton.isActiveAndEnabled == false && CelentineForest.Count > 0)
			CelenButton.gameObject.SetActive (true);

		if (RichtoButton.isActiveAndEnabled == false && RichtofenCastle.Count > 0)
			RichtoButton.gameObject.SetActive (true);
	}

	public void disableWorldIconButton(){
		if (BoundButton.isActiveAndEnabled == true)
			BoundButton.gameObject.SetActive (false);
		if (CelenButton.isActiveAndEnabled == true)
			CelenButton.gameObject.SetActive (false);
		if (RichtoButton.isActiveAndEnabled == true)
			RichtoButton.gameObject.SetActive (false);
	}

	public void createChapterChoice(string location){
		m_state = State.PICKINGCHAPTER;
		disableAllWorldIcon ();

		//Create BBT chapters
		if (location == "BoundBeyondTime") {
			for( int i = 0; i < BoundBeyondTime.Count;  i++){
				GameObject obj = Instantiate(Resources.Load ("Prefab/chapterButton")) as GameObject;
				chapterButton button = obj.GetComponent<chapterButton> ();
				button.m_button.receiverObject = this.gameObject;
				button.changeParameter(BoundBeyondTime[i]);
				button.m_button.m_DrawArea.y += i*250.0f;
				button.moveButtonX(150.0f);
				button.changeLabel("<color=white>" + BoundBeyondTime[i] + "</color>");
				m_chapters.Add (button);
			}
		}

		//Create Celentine Chapters
		if (location == "CelentineForest") {
			for( int i = 0; i < CelentineForest.Count;  i++){
				GameObject obj = Instantiate(Resources.Load ("Prefab/chapterButton")) as GameObject;
				chapterButton button = obj.GetComponent<chapterButton> ();
				button.m_button.receiverObject = this.gameObject;
				button.changeParameter(CelentineForest[i]);
				button.m_button.m_DrawArea.y += i*250.0f;
				button.moveButtonX(150.0f);
				button.changeLabel("<color=white>" + CelentineForest[i] + "</color>");
				m_chapters.Add (button);
			}
		}

		//Create Richtofen Chapters
		if (location == "RichtofenCastle") {
			for( int i = 0; i < RichtofenCastle.Count;  i++){
				GameObject obj = Instantiate(Resources.Load ("Prefab/chapterButton")) as GameObject;
				chapterButton button = obj.GetComponent<chapterButton> ();
				button.m_button.receiverObject = this.gameObject;
				button.changeParameter(RichtofenCastle[i]);
				button.m_button.m_DrawArea.y += i*250.0f;
				button.moveButtonX(150.0f);
				button.changeLabel("<color=white>" + RichtofenCastle[i] + "</color>");
				m_chapters.Add (button);
			}
		}
	}

	public void disableAllWorldIcon(){
		BoundButton.gameObject.SetActive(false);
		CelenButton.gameObject.SetActive (false);
		RichtoButton.gameObject.SetActive (false);
	}

	public void enableAllWorldIcon(){
		enableWorldIconButton ();
	}

	public void playChapter(string chapter){
		pauseChapterButton ();
		PlayerPrefs.SetString ("PickedChapter", chapter);
		GameObject obj = Instantiate(Resources.Load ("Prefab/sceneChangeButton")) as GameObject;
		sceneChangeButton button = obj.GetComponent<sceneChangeButton> ();
		button.m_DrawArea.x = -500;
		button.StartCoroutine (button.goToNextScene(0.2f,7));
	}

	public void pauseChapterButton(){
		for(int i = 0; i < m_chapters.Count; i++){
			m_chapters[i].m_button.buttonPressed = true;
		}
	}

	public void clearList(){
		BoundBeyondTime.Clear ();
		CelentineForest.Clear ();
		RichtofenCastle.Clear ();
	}

	public void demoBattle(){
		m_pickStory.buttonPressed = true;
		demoButton.buttonPressed = true;
		m_homeButton.buttonPressed = true;
		PlayerPrefs.SetString("BattleID", "0");
		GameObject obj = Instantiate(Resources.Load ("Prefab/sceneChangeButton")) as GameObject;
		sceneChangeButton button = obj.GetComponent<sceneChangeButton> ();
		button.m_DrawArea.x = 100000;
		button.StartCoroutine (button.goToNextScene(0.2f,5));
	}
}
