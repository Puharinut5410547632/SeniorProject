using UnityEngine;
using System.Collections;

public class sceneChangeButton : simpleWindow {
	
	public int nextSceneID = 0;
	public bool buttonPressed = false;
	public fadeableWindow fadeScene;

	public State m_state;
	public enum State
	{
		WAIT, //Idle state. No screen tap yet
		FADE, //Screen fades to black.
		END, //Change scene. To 1 if it's a new user. To 2 if it's an old user.
	};
	
	void OnGUI(){

		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());

		GUI.depth = depth;
		GUI.DrawTexture (m_DrawArea, m_texture);

		//Trigger for pressing on the button
		if (Input.GetMouseButtonDown (0) && m_DrawArea.Contains (Event.current.mousePosition) && buttonPressed == false){
			playOnceSound ();
			buttonPressed = true;
			StartCoroutine (goToNextScene(0.2f,nextSceneID));
		}

		if(m_parent != null) GUILayout.EndArea ();

	}

	public void changeScene(int id){
		Application.LoadLevel (id);
	}

	public IEnumerator goToNextScene(float seconds, int id){
		yield return new WaitForSeconds(seconds);
		m_state = State.FADE;
		fadeScene.fadeIn ();
		yield return new WaitForSeconds(0.5f);
		m_state = State.END;
		Debug.Log ("MOVING ON TO NEXT SCENE");
		changeScene (id );
	}

	public void playOnceSound(){
		if (audioPath != "") {
			source = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
		}
		source.PlayOneShot (audioSE);
	}	
}
