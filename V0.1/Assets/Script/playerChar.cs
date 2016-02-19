using UnityEngine;
using System.Collections;

public class playerChar : Character {

	public barDisplay healthBar;
	public barDisplay apBar;
	public simpleLabel ultimateCounter;
	public string faceTexturePath;
	public string headTexturePath;
	public string fullTexturePath;

	public string p_name;
	public int p_attackPower;
	public int p_magicAttackPower;
	public int p_defense;
	public int p_hp;
	public int p_ap;
	public int p_level;
	public string p_job;
	public int ultimateCharge;
	public int currentUltimateCharge;

	public enum activeStatus{
		ACTIVE, //Character is on the front and giving command
		WAITING, //Character is waiting for his/her turn
		DEAD, //Character health count is below 0
	}

	void Start () {
		charName = p_name;
		attackPower = p_attackPower;
		magicAttackPower = p_magicAttackPower;
		defense = p_defense;
		maxHP = p_hp;
		maxAP = p_ap;
		currentHP = maxHP;
		currentAP = 0;
		if (p_job == "Mage") {
			currentAP = maxAP;
		}
		job = p_job;
		side = "Player";	
		level = p_level;
		drawCharacter ();
		currentUltimateCharge = ultimateCharge;
		reduceCounter (0);
	}

	public void isHurt(int hurt){
		currentHP -= hurt;
		if (currentHP <= 0) {
			currentHP = 0;
			die ();
		}
		createDamageLabel (hurt);
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
			ultimateCounter.m_texturePath = "Texture/battle/ultimateCharge";
			ultimateCounter.m_texture = Resources.Load (ultimateCounter.m_texturePath) as Texture;
			ultimateCounter.label = "<color=white>" + "" + currentUltimateCharge + "</color>";
		}
		if (currentUltimateCharge == 0) {
			ultimateCounter.label = "";
			ultimateCounter.m_texturePath = "Texture/battle/ultimateReady";
			ultimateCounter.m_texture = Resources.Load (ultimateCounter.m_texturePath) as Texture;
		}
	}
	public void displayCurrentCounter(){
		reduceCounter (0);
	}

	//For changing status and everything, mainly for selectedChar prefab.
	public void changeStatus(playerChar chara){
		charName = chara.charName;
		maxAP = chara.maxAP;
		maxHP = chara.maxHP;
		currentAP = chara.currentAP;
		currentHP = chara.currentHP;
		level = chara.level;
		attackPower = chara.attackPower;
		magicAttackPower = chara.magicAttackPower;
		defense = chara.defense;
		job = chara.job;
		side = chara.side;
		isAlive = chara.isAlive;
		isDisabled = chara.isDisabled;
		disabilityName = chara.disabilityName;
		disabilityDuration = chara.disabilityDuration;
		ultimateCharge = chara.ultimateCharge;
		currentUltimateCharge = chara.currentUltimateCharge;
	}

	public void ultimateOnCD(){
		currentUltimateCharge = ultimateCharge;
		displayCurrentCounter ();
	}

}
