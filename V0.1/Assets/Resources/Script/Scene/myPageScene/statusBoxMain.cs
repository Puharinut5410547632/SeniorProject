using UnityEngine;
using System.Collections;

public class statusBoxMain : MonoBehaviour {

	public myPageMain page;
	public fadeableWindow coverScreen;
	public simpleWindow layout;
	public fadeableWindow inside;
	public commandButton closeButton;

	// Use this for initialization
	void Start () {
	
		statBoxPopIn ();
		coverScreen.fadeIn (0.7f);
	}

	private void statBoxPopIn(){
		layout.setTargetX(90.0f);
		layout.recalculateSpeedX(200);
	}


	public void closeWindow(){
		page.disableButtons (false);
		page.m_char1.playSound ();
		Destroy (this.gameObject);
	}
}
