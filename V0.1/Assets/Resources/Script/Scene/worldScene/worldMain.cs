using UnityEngine;
using System.Collections;

public class worldMain : MonoBehaviour {

	public simpleWindow m_background;
	public fadeableWindow m_storyBackground;
	public commandButton m_pickStory;
	public commandButton m_pickFreeMode;

	//state of current game picking
	public State m_state;
	public enum State
	{
		PICKTYPE, // Picking story or free mode
		STORY, //Story picking
		FREEMODE, // Free mode picking
	};

	// Use this for initialization
	void Start () {
		checkStoryPref ();
	}
	
	// In this update, buttons will move into proper spot.
	void Update () {
	
	}

	//Get the player of all chapter played to unlock the proper chapters
	public void checkStoryPref(){

	}

	public void pickStory(){

		m_state = State.STORY;
	}

	public void pickFreeMode(){

		m_state = State.FREEMODE;
	}

	public void backToPicktype(){
		m_state = State.PICKTYPE;
	}
}
