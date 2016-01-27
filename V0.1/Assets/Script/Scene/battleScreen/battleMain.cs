using UnityEngine;
using System.Collections;

public class battleMain : MonoBehaviour {

	public Enemy m_enemy;
	public commandButton m_commandButton;
	public simpleWindow m_stage;

	public Turn m_turn;

	public enum Turn
	{
		PLAYER, // Player Turn.
		ENEMY, // Enemy Turn.
		OVER, // Combat ended.
	};

	//For sound
	private AudioSource source;
	public string audioPath;
	public AudioClip audioSE;

	// Use this for initialization
	void Start () {
		m_stage.playSoundLoop ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Hi(){
	if (m_turn == Turn.PLAYER) {
			if(m_enemy!= null){
			playSound("Audio/se/atk1");
			m_enemy.isHurt (70);
			createDamageLabel (70);
			endPlayerTurn();
			}
		}
	}

	public void createDamageLabel(int damage){
		GameObject obj = Instantiate(Resources.Load ("Prefab/damageLabel")) as GameObject;
		simpleLabel number = obj.GetComponent<simpleLabel>();
		number.m_DrawArea.x = m_enemy.xBeforeResize;
		number.label = "<color=red>" + damage + "</color>";
		number.moveUp ();
		Destroy (obj, 0.5f);
	}

	public void endPlayerTurn(){
		beginEnemyTurn ();
	}

	public void endEnemyTurn(){
		StartCoroutine(endTest());
	}

	public void beginPlayerTurn(){
		m_turn = Turn.PLAYER;
	}

	public void beginEnemyTurn(){
		m_turn = Turn.ENEMY;
		endEnemyTurn ();
	}

	public IEnumerator endTest(){
		yield return new WaitForSeconds (2.0f);
		beginPlayerTurn ();

	}

	public void playSound(string path){
		audioPath = path;
		if (audioPath != "") {
			source = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
		}
		source.PlayOneShot (audioSE);
	}	

	public void Spawn(){
	
			GameObject newEne = Instantiate(Resources.Load ("Prefab/enemy")) as GameObject;
		if (m_enemy == null) {
			m_enemy = newEne.GetComponent<Enemy> ();
			m_enemy.m_parent = m_stage;
			m_enemy.label.m_parent = m_stage;
			m_enemy.labelArea.m_parent = m_stage;
			playSound("Audio/se/monSpawn");
		}
		else
			Destroy (newEne);
	}

	//Stop the BGM, display announcement and get ready to end the battle
	public void endBattleWin(){
		m_stage.endSound ();
		GameObject obj = Instantiate(Resources.Load ("Prefab/winBanner")) as GameObject;
		obj.GetComponent<simpleWindow> ().setTargetX (0);
		playSound ("Audio/bgm/stageWin");
		StartCoroutine (endBattle());
	}

	//End the battle by changing scene and fading everything to black.
	public IEnumerator endBattle(){
		yield return new WaitForSeconds (5.0f);
		GameObject obj = Instantiate(Resources.Load ("Prefab/sceneChangeButton")) as GameObject;
		sceneChangeButton button = obj.GetComponent<sceneChangeButton> ();
		button.StartCoroutine (button.goToNextScene(0.2f,5));
	}
}