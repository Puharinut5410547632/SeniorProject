using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerChar : Character {

	public barDisplay healthBar;
	public barDisplay apBar;
	public simpleLabel ultimateCounter;
	public simpleWindow ultimateCounterBox;
	//Different picture for using skill or selecting attack
	//Type determines if the part being used is a display bar or current selected char
	//It's either "display" or "selected"
	public string selectType;
	public string faceTexturePath;
	public string headTexturePath;
	public string fullTexturePath;
	//Decide the type of bar he will be using. PointAccumulation, Stance, Dodge.
	public string specialMechanic;
	public string currentStance;
	public bool redrawSelected;

	//Default stats for everyone
	public string p_name;
	public int p_attackPower;
	public int p_defense;
	public int p_hp;
	public int p_ap;
	public int p_level;
	public int ultimateCharge;
	public int currentUltimateCharge;
	public string p_supportSkill;
	public string p_mechanic;
	//Character file reader
	public CStringList charFile = null;

	public List<Move> moveSet = new List<Move>();

	public enum activeStatus{
		ACTIVE, //Character is on the front and giving command
		WAITING, //Character is waiting for his/her turn
		DEAD, //Character health count is below 0
	}

	void Start () {
		submitBaseStatus ();
	}

	public void submitBaseStatus(){
		charName = p_name;
		attackPower = p_attackPower;
		defense = p_defense;
		maxHP = p_hp;
		maxAP = p_ap; // For simplicity, AP is now always 100 for everyone.
		currentHP = maxHP;
		currentAP = 0;
		if (p_name == "Sarnu" || p_name == "Enfys" || p_name == "Liel") {
			currentAP = maxAP;
		}
		side = "Player";	
		level = p_level;
		drawCharacter ();
		currentUltimateCharge = ultimateCharge;
		reduceCounter (0);
	}

	public void charReader(){
	//	Debug.Log ("<color=green>Loading Char</color>");
		//Load file based on char name
		charFile = loadCharacter (p_name);
		//Read file for the proper status
		readCharacterBaseStatus (charFile);
		redrawCharacter ();
		//Update the base status to go with the file
		submitBaseStatus ();
		createMoveSet ();

		//Read the extra stuffs based on character name

		if (p_name == "Katzix")
			readKatzixStatus ();
	}

	public void createMoveSet(){
		moveSet.Clear (); // Clear first for future stance mechanic
		//Character always has 4 skills to choose from
		for (int i =0; i < 5; i++) {
			Move move = new Move();
			if(i <= 3) move.createMove(charFile.GetText ("mChar_skill" + (i+1))); // 4 default moves
			if(i == 4) move.createMove(charFile.GetText ("mChar_UltimateSkill")); // Ultimate move
//			Debug.Log (move.moveName);
			moveSet.Add(move);
		}

	}

	public void redrawCharacter(){
//		Debug.Log ("<color=red>REDRAWING</color>");
		if (selectType == "display") {
	
			m_texturePath = faceTexturePath;
			drawCharacter();
		}

		if (selectType == "selected") {
			m_texturePath = headTexturePath;
			drawCharacter ();
		}
	}

	public void readCharacterBaseStatus(CStringList charFile){
		p_name = charFile.GetText ("mChar_name");
		p_attackPower = int.Parse ( charFile.GetText ("mChar_attackPower") );
		p_defense = int.Parse ( charFile.GetText ("mChar_defense"));
		p_hp = int.Parse (charFile.GetText ("mChar_hp"));
		p_ap = int.Parse ( charFile.GetText ("mChar_ap"));
		ultimateCharge = int.Parse (charFile.GetText ("mChar_UltimateCharge"));
		faceTexturePath = charFile.GetText ("mChar_faceTexturePath");
		headTexturePath = charFile.GetText ("mChar_headTexturePath");
		fullTexturePath = charFile.GetText ("mChar_fullTexturePath");

		p_supportSkill = charFile.GetText ("mChar_supportSkill");
		p_mechanic = charFile.GetText ("mChar_specialMechanic");

	}

	public CStringList loadCharacter(string charName ){
		
		CStringList charFile = new CStringList();
		
		charFile.Load ("Character/" + charName + "/status");
		
		return charFile;
	}

	//Special character reader functions below here
	public void readEnfysStatus(){
		
	}

	public void readKatzixStatus(){

	}
	
	public void readVolframStatus(){
		
	}

	public void readAgilStatus(){
		
	}

	public void readLielStatus(){
		
	}

	public void readGwenetteStatus(){
		
	}

	public void readSarnuStatus(){
		
	}
	
	public void isHurt(int attack, float flatDamage, float multiplier, bool penetration){

		//Damage hurt (default for people without special anti-damage mechanic)
		float flatReduction = (float) defense / 10.0f;
		float percentReduction = (float) defense / 10000.0f;
	//	Debug.Log (percentReduction);
		float calculateDamage =  (flatDamage*1.0f - flatReduction*1.0f + (((float) attack*1.0f) * multiplier)) * (1.0f - percentReduction);
		if (penetration == true)
			calculateDamage = (int) ((float) attack * multiplier + attack);
		if (calculateDamage < 1)
			calculateDamage = 1.0f;
		currentHP -= (int)calculateDamage;
		createDamageLabel ((int) calculateDamage);

		if(charName == "Volfram") {int rageAP = (int)( calculateDamage/20.0f);
			gainAP(rageAP);
		}
		if (currentHP <= 0) {
			currentHP = 0;
			die ();
		}
	}

	public void createDamageLabel(int damage){
		
		GameObject obj = Instantiate (Resources.Load ("Prefab/damageLabel")) as GameObject;
		simpleLabel number = obj.GetComponent<simpleLabel> ();
		number.m_DrawArea.width = m_DrawArea.width;
		number.m_DrawArea.x = m_DrawArea.x;
		number.m_DrawArea.y = m_DrawArea.y - 50;
		number.label = "<color=red>" + damage + "</color>";
		number.moveUp ();
		Destroy (obj, 0.5f);
		
	}

	//toggle alive = false and replace template with a gray version
	public void die()	{
		isAlive = false;
		string m_texturePathDead = m_texturePath + "d";
			m_texture = Resources.Load (m_texturePathDead) as Texture;
	}

	public void reduceCounter(int i){
		if(isAlive == true)	currentUltimateCharge -= i;
		if (currentUltimateCharge <0) currentUltimateCharge = 0;
		if (currentUltimateCharge > 0) {
			ultimateCounterBox.m_texturePath = "Texture/battle/ultimateCharge";
			ultimateCounterBox.m_texture = Resources.Load (ultimateCounter.m_texturePath) as Texture;
			ultimateCounter.label = "<color=white>" + "" + currentUltimateCharge + "</color>";
		}
		if (currentUltimateCharge == 0) {
			ultimateCounter.label = "";
			ultimateCounterBox.m_texturePath = "Texture/battle/ultimateReady";
			ultimateCounterBox.m_texture = Resources.Load (ultimateCounter.m_texturePath) as Texture;
		}
	}
	public void displayCurrentCounter(){
		reduceCounter (0);
	}

	//For changing status and everything, mainly for selectedChar prefab.
	public void changeStatus(playerChar chara){
		p_name = chara.charName;
		p_ap = chara.maxAP;
		p_hp = chara.maxHP;
		maxAP = p_ap;
		maxHP = p_hp;
		currentAP = chara.currentAP;
		currentHP = chara.currentHP;
		p_level = chara.level;
		p_attackPower = chara.attackPower;
		p_defense = chara.defense;
		side = chara.side;
		isAlive = chara.isAlive;
		isDisabled = chara.isDisabled;
		disabilityName = chara.disabilityName;
		disabilityDuration = chara.disabilityDuration;
		ultimateCharge = chara.ultimateCharge;
		currentUltimateCharge = chara.currentUltimateCharge;
		m_texturePath = chara.headTexturePath;
		drawCharacter ();
		displayCurrentCounter ();
	}

	public void isSelected(){
		selectType = "selected";
		//So the game won't reload texture if unnecessary
		if(m_texturePath == headTexturePath) redrawSelected = true;
		if(m_texturePath != headTexturePath) redrawSelected = false;
		if(redrawSelected == false)	redrawCharacter ();
	}

	public void ultimateOnCD(){
		currentUltimateCharge = ultimateCharge;
		displayCurrentCounter ();
	}

	//Special mechanic for char with stance.
	public void changeStance(){

	}

}
