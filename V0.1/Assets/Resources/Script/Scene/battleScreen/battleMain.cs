using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class battleMain : MonoBehaviour {

	//List for Enemies and Players to be used.
	public List<Enemy> enemies = new List<Enemy>();
	public List<playerChar> players = new List<playerChar> ();
	public playerChar selectedChar;
	public commandButton m_commandButton;
	public simpleWindow m_stage;
	//Current wave on enemy the player is on
	public int m_currentWave;
	public Turn m_turn;
	//0 = no spawn yet, 1 = get ready to spawn, 2 = spawn next wave, 3 to check for enemy kill trigger
	public int m_checkSpawn;
	public int playerCharSlot;

	public bool acted = false;
	public bool gained = false;
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
		playerCharSlot = 0;
		m_stage.playSoundLoop ();
		createPlayerTeam ();
		//Always start at char one
		selectedChar = createdSelectedCharacter ();
	}
	
	// Update is called once per frame
	void Update () {
		changeSelectedCharacter (playerCharSlot);
		if (m_checkSpawn == 0) { m_checkSpawn = 1; StartCoroutine(waveSpawn());
		}
		if(m_checkSpawn == 2) {nextWave(); }
		if (m_checkSpawn == 3 && m_turn != Turn.WAIT)
		if(checkKilledAll ()){ 
			m_checkSpawn = 0; 
			m_currentWave++; 
			m_turn = Turn.WAIT; 
			if(gained == false) {gainCharge();}
			}
		}

	public IEnumerator waveSpawn(){
		yield return new WaitForSeconds (3.0f);
		m_checkSpawn = 2;
	}

	public playerChar createdSelectedCharacter(){
		GameObject newPlayer = Instantiate (Resources.Load ("Prefab/selectedCharacter")) as GameObject;
		playerChar player = newPlayer.GetComponent<playerChar> ();
		//Selected char only has to display current health,mana and head portrait
		player.m_texturePath = players [0].headTexturePath;
		player.maxHP = players [0].maxHP;
		player.currentHP = players [0].currentHP;
		player.maxAP = players [0].maxAP;
		player.currentAP = players [0].currentAP;
		player.currentUltimateCharge = players [0].currentUltimateCharge;

//		Debug.Log (player.maxHP);

		return player;

	}

	public IEnumerator playNextChar(){
		yield return new WaitForSeconds (0.7f);
		acted = false;
		playerCharSlot++;
	}

	//Go from 0 to 2 in order. If the character is dead, switch. This is done after the command is given.
	//When reached 3, trigger end turn for player.
	public void changeSelectedCharacter(int charSlot){
		selectedChar.changeStatus (players[charSlot]);
		selectedChar.isSelected ();
		selectedChar.displayCurrentCounter ();
	}

	public void createPlayerTeam(){
		for (int i = 0; i<3; i++) {
			string charName = PlayerPrefs.GetString ("Char" + (i+1));
			playerChar player = createPlayer (charName);
			player.m_DrawArea.x = 360*i;
			player.healthBar.m_DrawArea.x = player.m_DrawArea.x;
			player.apBar.m_DrawArea.x = player.m_DrawArea.x+180;
			player.ultimateCounter.m_DrawArea.x = player.m_DrawArea.x + player.m_DrawArea.width - player.ultimateCounter.m_DrawArea.width;
			player.ultimateCounter.m_DrawArea.y = player.m_DrawArea.y - player.ultimateCounter.m_DrawArea.height/2.0f;
			player.ultimateCounterBox.m_DrawArea.x = player.m_DrawArea.x + player.m_DrawArea.width - player.ultimateCounter.m_DrawArea.width;
			player.ultimateCounterBox.m_DrawArea.y = player.m_DrawArea.y - player.ultimateCounter.m_DrawArea.height/2.0f;
			players.Add (player);
		}
	
	}

	public playerChar createPlayer(string charName){

		GameObject newPlayer = Instantiate (Resources.Load ("Prefab/characterTab")) as GameObject;
		playerChar player = newPlayer.GetComponent<playerChar> ();
		player.p_name = charName;
		player.selectType = "display";
//		Debug.Log ("Testing Char Name: " + charName);
		player.charReader ();
		return player;

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
			if(enemies.Count != 0 && acted == false){
				acted = true;
				players[playerCharSlot].gainAP(10);
			int rand = Random.Range (0, enemies.Count); 
			playSound("Audio/se/atk1");
				enemies[rand].isHurt (players[playerCharSlot].attackPower, 1.0f, false);
			if(playerCharSlot == 2 ) endPlayerTurn();
				if(playerCharSlot<2)	StartCoroutine(playNextChar());
			

			}
		}
	}

	//Hit everything
	public void hitAll(){
		if (m_turn == Turn.PLAYER && players[playerCharSlot].currentUltimateCharge == 0 && acted == false) {
			acted = true;
			players[playerCharSlot].ultimateOnCD();
			if(enemies.Count != 0){
				for (int i = 0; i < enemies.Count; i++) {
					enemies[i].isHurt (players[playerCharSlot].attackPower, 1.5f, false);
				}
			}
			if(playerCharSlot == 2 ) endPlayerTurn();
			if(playerCharSlot<2)	StartCoroutine (playNextChar ());
			playSound("Audio/se/atk1");
		}
	}
	//Reduce ultimate counter too
	public void endPlayerTurn(){
		gainCharge ();
		StartCoroutine (switchTurn(Turn.ENEMY));
	}

	public void gainCharge(){
		for(int i = 0; i<=2; i++){
			players[i].reduceCounter(1);
		}
		gained = true;
	}

	public void endEnemyTurn(){
		StartCoroutine(endTest());
	}


	public void beginPlayerTurn(){
		playerCharSlot = 0;
		gained = false;
		acted = false;
		m_turn = Turn.PLAYER;
	}

	public void beginEnemyTurn(){
		m_turn = Turn.ENEMY;
		//Test no AI yet
		endEnemyTurn ();
	}

	public IEnumerator endTest(){
		for(int i = 0; i < enemies.Count; i++){
			enemies[i].StartCoroutine (enemies[i].getHit ());
			damagePlayer (enemies[i].attackPower, 1.0f, false);
			yield return new WaitForSeconds (1.0f);
		}
		StartCoroutine (switchTurn(Turn.PLAYER));

	}

	//Deal damage to player only if the char is alive.
	public void damagePlayer(int damage, float multiplier, bool penetration){

		if (checkPlayerAlive ()) {
			int player = Random.Range (0, 3);
			if (players [player].isAlive == true) {
				players [player].isHurt (damage, multiplier, penetration);
			} else
				damagePlayer (damage, multiplier, penetration);
		}
	}

	public bool checkPlayerAlive(){
		if (players [0].isAlive == false && players [1].isAlive == false && players [2].isAlive == false)
			return false;
		else 
			return true;

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
				enemy.m_DrawArea.x = enemy.m_DrawArea.width * 0.20f;
				enemy.healthBar.m_DrawArea.y = m_stage.m_DrawArea.height * 0.20f;
				enemies.Add (enemy);
				playSound ("Audio/se/monSpawn");
				beginPlayerTurn();
			}
		}
	}

	public void spawnTwo(){
		if (m_turn != Turn.OVER) {
			if (enemies.Count == 0) {
				beginPlayerTurn();
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
				enemy1.label.m_DrawArea.y = m_stage.m_DrawArea.height * 0.15f;
				enemy1.healthBar.m_DrawArea.y = m_stage.m_DrawArea.height * 0.25f;

				enemies.Add(enemy2);
				enemy2.labelArea.m_DrawArea.width = (enemy2.m_DrawArea.width/enemy2.partySize) * 0.8f;
				enemy2.label.m_DrawArea.y = m_stage.m_DrawArea.height * 0.15f;
				enemy2.healthBar.m_DrawArea.y = m_stage.m_DrawArea.height * 0.25f;
				playSound ("Audio/se/monSpawn");
			}
		}
	}

	public void spawnThree(){

		if (m_turn != Turn.OVER) {
			if (enemies.Count == 0) {
				beginPlayerTurn();
				Enemy enemy1 = createEnemy("Droplet");
				Enemy enemy2 = createEnemy("Droplet");
				Enemy enemy3 = createEnemy("Droplet");
				//Resize and move
				float parSize = 2.2f;

				enemy1.partySize = parSize;
				enemy1.m_DrawArea.x = enemy1.m_DrawArea.width/parSize * 1.24f;
				enemy1.m_DrawArea.y = m_stage.m_DrawArea.height * 0.1f;

				enemy2.partySize = parSize;
				enemy2.m_DrawArea.x = enemy2.m_DrawArea.width/parSize * 0.12f;
				enemy2.m_DrawArea.y = m_stage.m_DrawArea.height * 0.1f;

				enemy3.partySize = parSize;
				enemy3.m_DrawArea.x = enemy3.m_DrawArea.width/parSize * 2.36f;
				enemy3.m_DrawArea.y = m_stage.m_DrawArea.height * 0.1f;

				//Adjusting the label
				enemies.Add(enemy1);
				enemy1.labelArea.m_DrawArea.width = (enemy1.m_DrawArea.width/enemy1.partySize) * 0.8f;
				enemy1.label.m_DrawArea.y = m_stage.m_DrawArea.height * 0.2f;
				enemy1.healthBar.m_DrawArea.y = m_stage.m_DrawArea.height * 0.3f;

				enemies.Add(enemy2);
				enemy2.labelArea.m_DrawArea.width = (enemy2.m_DrawArea.width/enemy2.partySize) * 0.8f;
				enemy2.label.m_DrawArea.y = m_stage.m_DrawArea.height * 0.2f;
				enemy2.healthBar.m_DrawArea.y = m_stage.m_DrawArea.height * 0.3f;


				enemies.Add(enemy3);
				enemy3.labelArea.m_DrawArea.width = (enemy3.m_DrawArea.width/enemy3.partySize) * 0.8f;
				enemy3.label.m_DrawArea.y = m_stage.m_DrawArea.height * 0.2f;
				enemy3.healthBar.m_DrawArea.y = m_stage.m_DrawArea.height * 0.3f;

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
		button.StartCoroutine (button.goToNextScene(0.2f,2));
	}
}