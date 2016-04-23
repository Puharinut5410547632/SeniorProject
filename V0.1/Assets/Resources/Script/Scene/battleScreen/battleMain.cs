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
	public int enemySlot;
	public int enfysSlot = 10; // Keep track of where Enfys is if he's in the team to give the AP. If Enfys is not arund, this will remain at 10.
	public bool acted = false;
	public bool gained = false;
	public bool wait = false;

	//Button for command

	public commandButton attackButton;
	public List<skillButton> skillsbutton = new List<skillButton>();
	public commandButton cancel;
	public commandButton useskillButton;
	public commandButton ultimateButton;

	//From reading file
	//Story file reader
	public CStringList battleFile = null;
	public string battleID; // Get this from PLAYER PREF
	public int totalWave; // Get this from file
	public int clearPref; // This will unlock future stages
	public int totalMonster; // Decide what needs to be spawned
	public List<string> monsterName; // Add the monste name into this and spawn accordingly!

	public int clear; // This is the number for clearning the game that will be used to unlock new stages if it is above the currenet value.
	public enum Turn
	{
		WAIT, // Transitioning or no fight yet.
		PLAYER, // Player Turn.
		ENEMY, // Enemy Turn.
		OVER, // Combat ended.
		LOSE, // Everyone is dead.
		SWITCHTURN, // 
	};

	//For sound
	private AudioSource source;
	public string audioPath;
	public AudioClip audioSE;

	// Use this for initialization
	void Start () {
		cancel.setTargetX (1200);	
		m_currentWave = 1;
		battleID = PlayerPrefs.GetString ("BattleID");
		battleReader ();
		loadBattleFile ();
		playerCharSlot = 0;
		enemySlot = 0;
		m_stage.playSoundLoop ();
		createPlayerTeam ();
		//Always start at char one
		selectedChar = createdSelectedCharacter ();
		changeSelectedCharacter (playerCharSlot);
	}

	public void battleReader(){

		battleFile = loadBattleFile ();
		//get total Wave
		totalWave = int.Parse(battleFile.GetText ("mBattle" + (battleID) + "_wave"));
		clearPref = int.Parse(battleFile.GetText ("mBattle" + (battleID) + "_clear"));
		m_stage.playSoundLoop(battleFile.GetText ("mBattle" + (battleID) + "_bgm"));
		m_stage.changeTexture(battleFile.GetText ("mBattle" + (battleID) + "_bg"));
	}

	//To unlock the next stage if the battle is cleared and it's the latest one
	public void unlockStage(){
		clearPref++;
		if(PlayerPrefs.GetInt ("StoryChapter") < clear) PlayerPrefs.SetInt ("StoryChapter", clear);
	}

	public CStringList loadBattleFile(){
			
		CStringList batFile = new CStringList();
		batFile.Load ("Battle/Battle");

		return batFile;
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
		//Everyone is dead
		if (checkLose () && m_turn != Turn.LOSE) {
			StopAllCoroutines();
			StartCoroutine (loseBattle());
		}
		}

	public bool checkLose(){
		return (players [0].isAlive == false && players [1].isAlive == false && players [2].isAlive == false);
	}

	public IEnumerator loseBattle(){
		m_turn = Turn.LOSE;
		yield return new WaitForSeconds(2.0f);
		m_stage.endSound ();
		playSound ("Audio/bgm/stageLose");
		StartCoroutine (endBattle ());
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

	//Disable or Enable the ultimate button based on charge
	public void enableUltimate(int charSlot){
		if (selectedChar.currentUltimateCharge == 0) {
			ultimateButton.changeTexture ("Texture/battle/yesUltimate");
		} else {
			ultimateButton.changeTexture("Texture/battle/noUltimate");
		}
	}

	//Go from 0 to 2 in order. If the character is dead, switch. This is done after the command is given.
	//When reached 3, trigger end turn for player.
	public void changeSelectedCharacter(int charSlot){
		//Only display selected character of the person that is alive latest
		if (players [charSlot].isAlive) {
			selectedChar.changeStatus (players [charSlot]);
			selectedChar.isSelected ();
			selectedChar.displayCurrentCounter ();
			enableUltimate(charSlot);
		} else {
			//Turn out the person is dead
			if(wait == false){
			//Display next person instead
			playerCharSlot++;
			//Already at last person
			if(playerCharSlot>2) {wait = true; // Last person already and he/she is dead
				playerCharSlot = 0; // Revert back to display the first person
					if (!players [playerCharSlot].isAlive){
					//	Debug.Log("NANI! 1st PERSON ALSO DEAD");
						playerCharSlot++; // First person isn't alive either so check the 2nd one
					}
					if(m_turn == Turn.PLAYER) endPlayerTurn ();
				}
			}
			//Constantly update latest selected alive person
			else if(m_turn == Turn.ENEMY) {
				selectedChar.changeStatus (players [charSlot]);
				selectedChar.isSelected ();
				selectedChar.displayCurrentCounter ();}
		}
	}

	public void createPlayerTeam(){
		for (int i = 0; i<3; i++) {
			string charName = PlayerPrefs.GetString ("Char" + (i+1));
			if(charName == "Enfys") enfysSlot = i; //Keep track of Enfys position
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
		enemySlot = 0;
		m_turn = Turn.PLAYER;
		getCurrentWaveInfo ();
		spawnWave ();
		if (m_currentWave == (totalWave+1)) // Above total wave = ends
			endBattleWin (); 
	}

	public void getCurrentWaveInfo(){
			monsterName.Clear (); //Wipe first to make sure the list is always updated
			string monsters = battleFile.GetText ("mBattle" + (battleID) + "_enemy" + m_currentWave);
		if (monsters != "0") {
			string[] monstersSplit = monsters.Split (',');
			for (int i = 0; i <monstersSplit.Length; i++) {
				monsterName.Add (monstersSplit [i]);
			}
		}
//			for (int i = 0; i < monsterName.Count; i++)
//				Debug.Log (monsterName [i]);
	}

	public void spawnWave(){
		int totalMonster = monsterName.Count; //Get amount of monster
		Debug.Log (totalMonster);
		//Spawn accordingly
		if (totalMonster == 1) StartCoroutine(spawnBoss ());
		if(totalMonster == 2) spawnTwo ();
		if(totalMonster == 3) spawnThree ();

	}
	public bool checkKilledAll(){
		for (int i = 0; i < enemies.Count; i++) {
			if(enemies[i] == null) enemies.RemoveAt(i);
		}
		return enemies.Count == 0;
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
		enemySlot = 0;
		StartCoroutine (switchTurn(Turn.PLAYER));
	}


	public void beginPlayerTurn(){
		playerCharSlot = 0;
		wait = false;
		gained = false;
		acted = false;
		m_turn = Turn.PLAYER;
	}

	public void beginEnemyTurn(){
		m_turn = Turn.ENEMY;
		doEnemyTurn ();

	}

	public void doEnemyTurn(){
		if(enemySlot >= enemies.Count) endEnemyTurn ();
		else enemies[enemySlot].decideAction();
	}

	public IEnumerator endTest(){
		for(int i = 0; i < enemies.Count; i++){
			enemies[i].StartCoroutine (enemies[i].getHit ());
		//	damagePlayer (enemies[i].attackPower, 1.0f, false);
			yield return new WaitForSeconds (1.0f);
		}
		StartCoroutine (switchTurn(Turn.PLAYER));

	}

	//Deal damage to player only if the char is alive.
	public void damagePlayer(int damage, float multiplier, bool penetration){

		if (checkPlayerAlive ()) {
			int player = Random.Range (0, 3);
			if (players [player].isAlive == true) {
//				players [player].isHurt (damage, multiplier, penetration);
			} else {
			}
		//		damagePlayer (damage, multiplier, penetration);
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
				m_stage.playSoundLoop(battleFile.GetText ("mBattle" + (battleID) + "_bossbgm"));
				Enemy enemy = createEnemy (monsterName[0]);
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
				Enemy enemy1 = createEnemy(monsterName[0]);
				Enemy enemy2 = createEnemy(monsterName[1]);
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
				Enemy enemy1 = createEnemy(monsterName[0]);
				Enemy enemy2 = createEnemy(monsterName[1]);
				Enemy enemy3 = createEnemy(monsterName[2]);
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
		newEnemy.createEnemy (name);
	//	changeEnemy (newEnemy, name);
		newEnemy.m_parent = m_stage;
		newEnemy.m_battle = this;
		newEnemy.label.m_parent = m_stage;
		newEnemy.labelArea.m_parent = m_stage;
		newEnemy.healthBar = newEne.GetComponentInChildren<barDisplay> ();
		return newEnemy;
	}
	
	//Stop the BGM, display announcement and get ready to end the battle
	public void endBattleWin(){
		unlockStage (); // Only happens in a win
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
		Debug.Log ("YEAH");
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
		button.StartCoroutine (button.goToNextScene(0.2f,4));
	}

	public void retrieveSkills(){

		//THe player's turn, hasn't acted and there's no pending skills

	if (m_turn == Turn.PLAYER && acted == false && skillsbutton.Count == 0) {
			for (int i = 0; i <4; i++) {
				skillButton button = createSkillButton ();
				button.changeMove(players [playerCharSlot].moveSet [i]);
				button.m_battle = this;
				button.m_button.m_DrawArea.y = (265.0f + (150.0f * i));
				//No AP to use the skill!
				if(players [playerCharSlot].moveSet [i].moveCost > players [playerCharSlot].currentAP) {
					//Unless you're Enfys with enough Health Cost
				    if (players[playerCharSlot].charName!= "Enfys") {
					button.m_button.buttonPressed = true; // DISABLED
						button.m_button.changeTexture("Texture/battle/skillNo");
					}

					//Change label to Health cost for Enfys
					if (players[playerCharSlot].charName == "Enfys" ){
						int healthCost = players [playerCharSlot].moveSet [i].moveCost - players [playerCharSlot].currentAP; // Get new health cost
						button.m_costlabel.label = "<color=white>" + healthCost + "hp</color>";
					}
					//But if the cost is ABOVE the mana cost
					if (players[playerCharSlot].charName == "Enfys" && players[playerCharSlot].currentHP < players[playerCharSlot].moveSet [i].moveCost){
					button.m_button.buttonPressed = true;
					button.m_button.changeTexture("Texture/battle/skillNo");
					}
				}
				skillsbutton.Add (button);
			}

			cancel.speedX = 40.0f;
			cancel.setTargetX(100.0f);


		}
	}

	public skillButton createSkillButton(){
		GameObject obj = Instantiate(Resources.Load ("Prefab/skillButton")) as GameObject;
		skillButton sbutton = obj.GetComponent<skillButton> ();
		return sbutton;
	}

	//Preparing a move to be used from the skill button and get rid of all other buttons
	public void prepareMove(Move move){
	//	getRidOfSkillButtons ();
		useMove (move);
	}


	public void getRidOfSkillButtons(){
		//Get rid of the buttons if there are buttons
		cancel.m_DrawArea.x = 1200;
		cancel.setTargetX (1200);

		if (skillsbutton.Count > 0) {
			for (int i = 0; i<skillsbutton.Count; i++) {
				skillsbutton [i].destroyButton ();
			}
			skillsbutton.Clear ();  //Clear the list
		}
	}
	public void useMove(Move move){
		//Get targeting type
		getRidOfSkillButtons ();
		string type = move.targetType;
		if(m_turn==Turn.PLAYER) alterAP(move);
		
		if (type == "All") {
			targetAllMove(move);
		}

		if (type == "Single") {
			singleTargetMove(move);

		}
	}

	//For player char
	public void alterAP(Move move){
		Debug.Log ("ALTERING");
		players [playerCharSlot].useAP (move.moveCost); // Reduce AP based on Cost
		//Gain AP based on use cost if there's Enfys in the team.
		if(players[playerCharSlot].charName!= "Enfys" && enfysSlot != 10) giveEnfysAP(move.moveCost);
		if (players [playerCharSlot].charName == "Enfys" || players [playerCharSlot].charName == "Liel" || players [playerCharSlot].charName == "Sarnu") // Special case char name for the casters
			players [playerCharSlot].gainPercentAP ((float) move.moveApGain);
		else {
			Debug.Log ("gaining AP");
			players [playerCharSlot].gainAP (move.moveApGain);
		}
	}

	public void giveEnfysAP(int apUsed){
		//Check that enfys is alive
		if (players [enfysSlot].isAlive) {
			int giveAP = (int) (((float) apUsed) / 4.0f); //25% of AP used is given to Enfys
			Debug.Log ("GIVE ENFYS AP: " + giveAP);
			players[enfysSlot].gainAP (giveAP);
		}
	}
	//For the ultimate button
	public void useUltimate(){
		//Is a player, hasn't acted yet and ultimate is ready

		if (players [playerCharSlot].currentUltimateCharge == 0 && m_turn == Turn.PLAYER && acted == false) {
			acted = true;
			useMove (players [playerCharSlot].moveSet [4]); // USe last move aka ultimate move
			players[playerCharSlot].ultimateOnCD();
		}
	}

	public void playerDefaultAttack(){
		if (acted == false && m_turn == Turn.PLAYER) {
			acted = true;
			Move move = new Move ();
			move.createMove ("00000001");
			useMove (move);
		}
	}

	public void targetAllMove(Move move){
		string type = move.moveType;
		if (type == "Damage") {
			if(m_turn == Turn.ENEMY) StartCoroutine(enemyAllHit (move));
			if(m_turn == Turn.PLAYER) StartCoroutine(playerAllHit (move));
		}
		if (type == "Heal") {
			if(m_turn == Turn.ENEMY) StartCoroutine(enemyAllHeal (move));
		}
	}

	public void singleTargetMove(Move move){
		string type = move.moveType;
		if (type == "Damage") {
			if(m_turn == Turn.ENEMY) StartCoroutine(enemySingleHit (move));
			if(m_turn == Turn.PLAYER) StartCoroutine (playerSingleHit (move));
		}
		if (type == "Heal") {
			if(m_turn == Turn.ENEMY) StartCoroutine(enemySingleHeal (move));
		}
	}

	public IEnumerator enemySingleHeal(Move move){
		Debug.Log ("Single Healing");
		enemies[enemySlot].StartCoroutine (enemies[enemySlot].getHit ());
		yield return new WaitForSeconds (0.5f); 
	
		for(int i = 0; i < move.hits; i++){
			int rand = Random.Range (0, enemies.Count); 
			playSound(move.sound);
			enemies[rand].isHealed (move.flatValue, move.multiplier);
			playHitEnemyAnimation(rand, move);
			yield return new WaitForSeconds (0.33f); // Waiting between hit
		}

		enemySlot++;
		if(enemySlot == enemies.Count ) endEnemyTurn();
		if(enemySlot < enemies.Count)	doEnemyTurn();;

	}

	public IEnumerator enemyAllHeal(Move move){
		enemies[enemySlot].StartCoroutine (enemies[enemySlot].getHit ());
		yield return new WaitForSeconds (0.5f); 
		
		for(int i = 0; i < move.hits; i++){
			for (int x = 0; x < enemies.Count; x++) {
				enemies[x].isHealed (move.flatValue, move.multiplier);
				playHitEnemyAnimation(x, move);
			}
			playSound(move.sound);
			yield return new WaitForSeconds (0.33f); // Waiting between hit
			
		}
		enemySlot++;
		if(enemySlot == enemies.Count ) endEnemyTurn();
		if(enemySlot < enemies.Count)	doEnemyTurn();;
		
	}


	public IEnumerator enemySingleHit(Move move){
		//Attacking animation
		enemies[enemySlot].StartCoroutine (enemies[enemySlot].getHit ());
		yield return new WaitForSeconds (0.5f); // Wait for char to toggle before hitting.
		//Check that the players can be attacked in the first place
		if (checkPlayerAlive ()) {
			for(int i = 0 ; i < move.hits; i++){
			int player = Random.Range (0, 3);
			//target is alive
			if (players [player].isAlive == true) {
				players [player].isHurt (enemies[enemySlot].e_attackPower, move.flatValue, move.multiplier, move.pen);
			} 
			//target is dead
			else if(players [player].isAlive == false){
				player++; // target next player
			if(player>2) player-=3; // So it goes back to 0

				if (players [player].isAlive == true) {
					players [player].isHurt (enemies[enemySlot].e_attackPower, move.flatValue, move.multiplier, move.pen);
				} 

				//Player is STILL dead so target the last one and hit him anyway
				else if(players [player].isAlive == false){
					player++; // target next player
					if(player>2) player-=3; // So it goes back to 0
					players [player].isHurt (enemies[enemySlot].e_attackPower, move.flatValue, move.multiplier, move.pen);
					} 
				}
				playHitPlayerAnimation(player, move);
				playSound(move.sound);
				yield return new WaitForSeconds (0.2f); // Waiting between hit
			}
		}

		enemySlot++;
		if(enemySlot == enemies.Count) endEnemyTurn();
		if(enemySlot < enemies.Count)	doEnemyTurn();;
	}

	public IEnumerator playerSingleHit(Move move){
		if(enemies.Count != 0){
			if(players[playerCharSlot].charName != "Enfys")players[playerCharSlot].gainAP(move.moveApGain);
			if(players[playerCharSlot].charName == "Enfys"){
				players[playerCharSlot].gainPercentAP(move.moveApGain);
			}
				for(int i = 0; i < move.hits; i++){
				int rand = Random.Range (0, enemies.Count); 
				if(!enemies[rand].isAlive){
					rand++;
					if(rand >= enemies.Count) rand = 0;
					if(!enemies[rand].isAlive){
						rand++;
						if(rand >= enemies.Count) rand = 0;}
				}
				//Only hit alive target
				if(enemies[rand].isAlive){
				enemies[rand].isHurt (players[playerCharSlot].attackPower, move.flatValue, move.multiplier, move.pen);
					playSound(move.sound);
					playHitEnemyAnimation(rand, move);
				}
//				playSound(move.sound);
//				playSingleHitEnemyAnimation(rand, move);



				yield return new WaitForSeconds (0.2f); // Waiting between hit
			}

			if(playerCharSlot == 2 ) endPlayerTurn();
			if(playerCharSlot<2)	StartCoroutine(playNextChar());

		}
	}


	public void playHitEnemyAnimation(int target, Move move){

		GameObject obj = Instantiate(Resources.Load ("Prefab/singleEnemyAnim")) as GameObject;
		simpleWindow anim = obj.GetComponent<simpleWindow> ();
		//Set X
		anim.m_DrawArea.x = enemies[target].m_DrawArea.x;
		//Alter for 1 and 3
		if (monsterName.Count == 3) {
			anim.m_DrawArea.x -= 80;
			anim.m_DrawArea.y -= 80;
		}
		if (monsterName.Count == 1) anim.m_DrawArea.x = enemies[target].m_DrawArea.x + enemies[target].m_DrawArea.width - 450 ;


		string moveAnim = move.animation;
		string overallPath = "Move/Animation/single/" + moveAnim;
		Debug.Log ("OVerall PAth: " + overallPath);
		anim.playAnimation (overallPath);

	}

	public void playHitPlayerAnimation(int target, Move move){
		Debug.Log ("HITTING PLAYER");
		GameObject obj = Instantiate(Resources.Load ("Prefab/singleEnemyAnim")) as GameObject;
		simpleWindow anim = obj.GetComponent<simpleWindow> ();
		//Set X
		anim.m_DrawArea.x = players[target].m_DrawArea.x - 50;
		anim.m_DrawArea.y = 930;
		string moveAnim = move.animation;
		string overallPath = "Move/Animation/single/" + moveAnim;
		anim.playAnimation (overallPath);
		
	}

	public IEnumerator enemyAllHit(Move move){
		if (m_turn == Turn.ENEMY) {
			enemies[enemySlot].StartCoroutine (enemies[enemySlot].getHit ());
			yield return new WaitForSeconds (0.5f); // Wait for char to toggle before hitting.
			//		players[playerCharSlot].ultimateOnCD();
			for(int i = 0; i<move.hits; i++){
				if(enemies.Count != 0){
					for (int x = 0; x < players.Count; x++) {
					if(players[x].isAlive){players[x].isHurt (enemies[enemySlot].attackPower, move.flatValue, move.multiplier, move.pen);
						playHitPlayerAnimation(x, move);
						}
					}
				}
				playSound(move.sound);

				yield return new WaitForSeconds (0.33f); // Waiting between hit
			}
			enemySlot++;
			if(enemySlot == enemies.Count ) endEnemyTurn();
			if(enemySlot < enemies.Count)	doEnemyTurn();;
		}
	}

	public IEnumerator playerAllHit(Move move){
		if (m_turn == Turn.PLAYER) {
	//		players[playerCharSlot].ultimateOnCD();
			for(int i = 0; i<move.hits; i++){
			if(enemies.Count != 0){
				for (int x = 0; x < enemies.Count; x++) {
				if(enemies[x].isAlive){		enemies[x].isHurt (players[playerCharSlot].attackPower, move.flatValue, move.multiplier, move.pen);
							playHitEnemyAnimation(x, move);
						}
				}
			}
			playSound(move.sound);
				yield return new WaitForSeconds (0.3f); // Waiting between hit
			}
			if(playerCharSlot == 2 ) endPlayerTurn();
			if(playerCharSlot<2)	StartCoroutine (playNextChar ());
		}
	}
}