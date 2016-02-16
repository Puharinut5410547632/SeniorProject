using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class battleMain : MonoBehaviour {

	public List<Enemy> enemies = new List<Enemy>();
	public commandButton m_commandButton;
	public simpleWindow m_stage;
	//Current wave on enemy the player is on
	public int m_currentWave;
	public Turn m_turn;
	//0 = no spawn yet, 1 = get ready to spawn, 2 = spawn next wave, 3 to check for enemy kill trigger
	public int m_checkSpawn;

	public enum Turn
	{
		WAIT, // Transitioning or no fight yet.
		PLAYER, // Player Turn.
		ENEMY, // Enemy Turn.
		OVER, // Combat ended.
		SWITCHTURN, // 
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
		if (m_checkSpawn == 0) { m_checkSpawn = 1; StartCoroutine(waveSpawn());
		}
		if(m_checkSpawn == 2) {nextWave();}
		if (m_checkSpawn == 3 && m_turn != Turn.WAIT)
		if(checkKilledAll ()){ m_checkSpawn = 0; m_currentWave++; m_turn = Turn.WAIT;}
	}

	public IEnumerator waveSpawn(){
		yield return new WaitForSeconds (3.0f);
		m_checkSpawn = 2;
	}

	public void nextWave(){
		m_checkSpawn = 3;
		m_turn = Turn.PLAYER;
		if (m_currentWave == 0)
			spawnThree ();
		if (m_currentWave == 1)
			spawnTwo ();
		if (m_currentWave == 2)
		StartCoroutine(	spawnBoss () );
		if (m_currentWave == 3)
			endBattleWin (); 
	}

	public bool checkKilledAll(){
		for (int i = 0; i < enemies.Count; i++) {
			if(enemies[i] == null) enemies.RemoveAt(i);
		}
		return enemies.Count == 0;
	}

	//Random hit
	public void hitOne(){
	if (m_turn == Turn.PLAYER) {
			if(enemies.Count != 0){
			int rand = Random.Range (0, enemies.Count); 
			playSound("Audio/se/atk1");
			enemies[rand].isHurt (70);
			endPlayerTurn();

			}
		}
	}

	//Hit everything
	public void hitAll(){
		if (m_turn == Turn.PLAYER) {

			if(enemies.Count != 0){
				for (int i = 0; i < enemies.Count; i++) {
				enemies[i].isHurt (50);
				}
			}
			endPlayerTurn();
			playSound("Audio/se/atk1");
		}
	}

	public void endPlayerTurn(){
		StartCoroutine (switchTurn(Turn.ENEMY));
	}

	public void endEnemyTurn(){
		StartCoroutine(endTest());
	}

	public void beginPlayerTurn(){
		m_turn = Turn.PLAYER;
	}

	public void beginEnemyTurn(){
		m_turn = Turn.ENEMY;
		//Test no AI yet
		endEnemyTurn ();
	}

	public IEnumerator endTest(){
		yield return new WaitForSeconds (0.2f);
		StartCoroutine (switchTurn(Turn.PLAYER));

	}

	//Create banner on turn change.
	public IEnumerator switchTurn(Turn turn){
		m_turn = Turn.SWITCHTURN;

		yield return new WaitForSeconds (2.0f);
		if (!checkKilledAll ()) {
	
			if (turn == Turn.PLAYER) {
				GameObject banner = Instantiate (Resources.Load ("Prefab/warningBanner")) as GameObject;
				banner.GetComponent<simpleWindow> ().changeTexture ("Texture/battle/turnPlayer");
				banner.GetComponent<simpleWindow> ().recalculateSpeedX (banner.GetComponent<simpleWindow> ().speed);
				banner.GetComponent<simpleWindow> ().setTargetX (0);
				yield return new WaitForSeconds (1.0f);
				banner.GetComponent<simpleWindow> ().setTargetX (-10000.0f);
				yield return new WaitForSeconds (0.7f);
				Destroy (banner);
				beginPlayerTurn ();
			}
			if (turn == Turn.ENEMY) {
				GameObject banner = Instantiate (Resources.Load ("Prefab/warningBanner")) as GameObject;
				banner.GetComponent<simpleWindow> ().changeTexture ("Texture/battle/turnEnemy");
				banner.GetComponent<simpleWindow> ().recalculateSpeedX (banner.GetComponent<simpleWindow> ().speed);
				banner.GetComponent<simpleWindow> ().setTargetX (0);
				yield return new WaitForSeconds (1.0f);
				banner.GetComponent<simpleWindow> ().setTargetX (-10000.0f);
				yield return new WaitForSeconds (0.7f);
				Destroy (banner);
				beginEnemyTurn ();
			}
		}
		
	}
	public void playSound(string path){
		audioPath = path;
		if (audioPath != "") {
			source = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
		}
		source.PlayOneShot (audioSE);
	}	

	public IEnumerator spawnBoss(){

		if (m_turn != Turn.OVER) {
			StartCoroutine (bossWarning ());
			yield  return new WaitForSeconds(5.0f);
			if (enemies.Count == 0) {
				m_stage.playSoundLoop ("Audio/bgm/boss1");
				Enemy enemy = createEnemy ("Dragon");
				enemy.partySize = 0.85f;
		//		Debug.Log ("e : " + enemy.m_DrawArea.width);
		//		Debug.Log ("w : " + m_stage.m_DrawArea.width);
				enemy.m_DrawArea.x = m_stage.m_DrawArea.width  * 0.33f;
				enemies.Add (enemy);
				playSound ("Audio/se/monSpawn");
				m_turn = Turn.PLAYER;
			}
		}
	}

	public void spawnTwo(){
		if (m_turn != Turn.OVER) {
			if (enemies.Count == 0) {
				Enemy enemy1 = createEnemy("Droplet");
				Enemy enemy2 = createEnemy("Droplet");
				//Resize and move
				enemy1.partySize = 1.5f;
				enemy1.m_DrawArea.x = 0.1f * enemy1.m_DrawArea.width/1.5f;
				enemy2.partySize = 1.5f;
				enemy2.m_DrawArea.x = enemy1.m_DrawArea.width/1.5f * 1.2f;
				enemies.Add(enemy1);
				//Adjusting the label

				enemy1.labelArea.m_DrawArea.width = (enemy1.m_DrawArea.width/enemy1.partySize) * 0.8f;
				enemy1.labelArea.m_DrawArea.x += enemy1.labelArea.m_DrawArea.width * 0.5f;
				enemy1.label.m_DrawArea.y += 50.0f;
				enemy1.healthBar.m_DrawArea.y += 80.0f;

				enemies.Add(enemy2);
				enemy2.label.m_DrawArea.y += 50.0f;
				enemy2.healthBar.m_DrawArea.y += 80.0f;
				enemy2.labelArea.m_DrawArea.width = (enemy2.m_DrawArea.width/enemy2.partySize) * 0.8f;
				playSound ("Audio/se/monSpawn");
			}
		}
	}

	public void spawnThree(){

		if (m_turn != Turn.OVER) {
			if (enemies.Count == 0) {
				Enemy enemy1 = createEnemy("Droplet");
				Enemy enemy2 = createEnemy("Droplet");
				Enemy enemy3 = createEnemy("Droplet");
				//Resize and move
				float parSize = 2.2f;
				float ymodification = 0f;

				enemy1.partySize = parSize;
				enemy1.m_DrawArea.x = 0.12f * enemy1.m_DrawArea.width/parSize;
				enemy1.m_DrawArea.y = ymodification * enemy1.m_DrawArea.y;

				enemy2.partySize = parSize;
				enemy2.m_DrawArea.x = enemy2.m_DrawArea.width/parSize * 1.24f;
				enemy2.m_DrawArea.y = ymodification * enemy2.m_DrawArea.y;

				enemy3.partySize = parSize;
				enemy3.m_DrawArea.x = enemy3.m_DrawArea.width/parSize * 2.36f;
				enemy3.m_DrawArea.y = ymodification * enemy3.m_DrawArea.y;

				//Adjusting the label
				enemies.Add(enemy1);
				enemy1.labelArea.m_DrawArea.width = (enemy1.m_DrawArea.width/enemy1.partySize) * 0.8f;
				enemy1.labelArea.m_DrawArea.x += enemy1.labelArea.m_DrawArea.width * 0.5f;
				enemy1.label.m_DrawArea.y += 100.0f;
				enemy1.healthBar.m_DrawArea.y += 150.0f;

				enemies.Add(enemy2);
				enemy2.label.m_DrawArea.y += 100.0f;
				enemy2.healthBar.m_DrawArea.y += 150.0f;
				enemy2.labelArea.m_DrawArea.width = (enemy2.m_DrawArea.width/enemy2.partySize) * 0.8f;

				enemies.Add(enemy3);
				enemy3.label.m_DrawArea.y += 100.0f;
				enemy3.healthBar.m_DrawArea.y += 150.0f;
				enemy3.labelArea.m_DrawArea.width = (enemy3.m_DrawArea.width/enemy3.partySize) * 0.8f;
				playSound ("Audio/se/monSpawn");
			}
		}

	}

	public Enemy createEnemy(string name){
		GameObject newEne = Instantiate (Resources.Load ("Prefab/enemy")) as GameObject;
		Enemy newEnemy = newEne.GetComponent<Enemy> ();
		changeEnemy (newEnemy, name);
		newEnemy.m_parent = m_stage;
		newEnemy.label.m_parent = m_stage;
		newEnemy.labelArea.m_parent = m_stage;
		newEnemy.healthBar = newEne.GetComponentInChildren<barDisplay> ();
		return newEnemy;
	}

	//Change texture and name of Enemy
	public void changeEnemy(Enemy enemy, string name){
		enemy.e_name = name;
		if (name == "Dragon")
			enemy.e_hp = 360;
		enemy.m_texturePath = "Texture/monster01/" + name;
		enemy.label.getEnemy ();
	}
	
	//Stop the BGM, display announcement and get ready to end the battle
	public void endBattleWin(){
		if(m_turn != Turn.OVER){
		m_turn = Turn.OVER;
		m_stage.endSound ();
		GameObject obj = Instantiate(Resources.Load ("Prefab/winBanner")) as GameObject;
			obj.GetComponent<simpleWindow> ().recalculateSpeedX (obj.GetComponent<simpleWindow> ().speed);
		obj.GetComponent<simpleWindow> ().setTargetX (0);
		playSound ("Audio/bgm/stageWin");
		StartCoroutine (endBattle());
		}
	}

	//Declare boss is coming with blinking red screen and banner
	public IEnumerator bossWarning(){
		m_turn = Turn.WAIT;
		GameObject banner = Instantiate(Resources.Load ("Prefab/warningBanner")) as GameObject;
		banner.GetComponent<simpleWindow> ().changeTexture ("Texture/battle/bossWarn");
		banner.GetComponent<simpleWindow> ().recalculateSpeedX (banner.GetComponent<simpleWindow> ().speed);
		banner.GetComponent<simpleWindow> ().setTargetX (0);
		if (banner.GetComponent<simpleWindow> ().m_DrawArea.x > banner.GetComponent<simpleWindow> ().targetX)
			banner.GetComponent<simpleWindow> ().m_DrawArea.x = banner.GetComponent<simpleWindow> ().targetX;
		GameObject fade = Instantiate(Resources.Load ("Prefab/warningFade")) as GameObject;
		m_stage.endSound ();
		m_stage.playSound ("Audio/se/bossAlert");
		for (int i = 0; i < 3; i++) {
			fade.GetComponent<fadeableWindow> ().fadeIn (0.6f);
			yield return new WaitForSeconds(0.8f);
			fade.GetComponent<fadeableWindow> ().fadeOut ();
			yield return new WaitForSeconds(0.8f);
		}
		Destroy (banner);
		Destroy (fade);
	}
	//End the battle by changing scene and fading everything to black.
	public IEnumerator endBattle(){
		yield return new WaitForSeconds (5.0f);
		GameObject obj = Instantiate(Resources.Load ("Prefab/sceneChangeButton")) as GameObject;
		sceneChangeButton button = obj.GetComponent<sceneChangeButton> ();
		button.StartCoroutine (button.goToNextScene(0.2f,5));
	}
}