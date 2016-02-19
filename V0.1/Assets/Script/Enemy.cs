using UnityEngine;
using System.Collections;

public class Enemy : Character {
	
	public string e_name;
	public int e_attackPower;
	public int e_magicAttackPower;
	public int e_defense;
	public int e_hp;
	public int e_ap;
	public int e_level;
	public enemyLabel label;
	public fadeableWindow labelArea;
	public barDisplay healthBar;

	// Use this for initialization
	void Start () {
		charName = e_name;
		attackPower = e_attackPower;
		magicAttackPower = e_magicAttackPower;
		defense = e_defense;
		maxHP = e_hp;
		maxAP = e_ap;
		currentHP = maxHP;
		currentAP = maxAP;
		job = "Monster";
		side = "Enemy";
		level = e_level;
		drawCharacter ();
		labelArea.m_DrawArea.x = m_DrawArea.x;
		m_enemyLabel = label;
	}

	void Init (string Name, int level){

	}

	public void alterStatus(){

	}
}
