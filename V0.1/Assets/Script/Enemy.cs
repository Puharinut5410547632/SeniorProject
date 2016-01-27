using UnityEngine;
using System.Collections;

public class Enemy : Character {
	
	public string e_name;
	public int e_attackPower;
	public int e_magicAttackPower;
	public int e_defense;
	public int e_hp;
	public int e_mp;
	public int e_level;
	public enemyLabel label;
	public fadeableWindow labelArea;

	// Use this for initialization
	void Start () {
		name = e_name;
		attackPower = e_attackPower;
		magicAttackPower = e_magicAttackPower;
		defense = e_defense;
		maxHP = e_hp;
		maxMP = e_mp;
		currentHP = maxHP;
		currentMP = maxMP;
		job = "Monster";
		side = "Enemy";
		level = e_level;
		drawCharacter ();
	
	}

	void Init (string Name, int level){

	}

	public void alterStatus(){

	}
}
