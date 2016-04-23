using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Character {
	
	public string e_name;
	public int e_attackPower;
	public int e_magicAttackPower;
	public int e_defense;
	public int e_hp;
	public int e_level;
	public enemyLabel label;
	public fadeableWindow labelArea;
	public barDisplay healthBar;
	
	public CStringList enemyDetail = null;

	//This will be used to send message on what to do to the battle and have it does the work of randomzing
	//and hitting people.
	//Has all move according to health
	public List<Move> health25Action;
	public List<Move> health50Action;
	public List<Move> health75Action;
	public List<Move> health100Action;
	public Move nextMove;

	public battleMain m_battle;

	public void overwriteStatus(){
		charName = e_name;
		attackPower = e_attackPower;
		defense = e_defense;
		maxHP = e_hp;
		maxAP = 0; // Monster don't care about AP
		currentHP = maxHP;
		currentAP = maxAP;
		side = "Enemy";
		level = e_level;
		drawCharacter ();
		labelArea.m_DrawArea.x = m_DrawArea.x;
		m_enemyLabel = label;
		labelArea.depth = 7;
	}

	public void createEnemy(string name){
		enemyDetail = loadEnemy (name);
		createStat (enemyDetail);
		createAI (enemyDetail);
		overwriteStatus ();
	}

	//Load the file that will be used to read the detail
	public CStringList loadEnemy(string name){
		CStringList enemyFile = new CStringList();
		enemyFile.Load ("Character/Enemy/" +name);
		return enemyFile;
	}

	//AI action will be decided from the text file command
	public void createAI(CStringList file){
		//100% action
		for (int index = 0; index < 20; index ++) {
			//Create new move and add them as long as there is possible move to use
			if(enemyDetail.GetText ("eChar_100AI" + (index+1) ) == "0") break; // Can't find the key and it will return 0
			string moveID = enemyDetail.GetText ("eChar_100AI" + (index+1) );
			Move move = new Move();
			move.createMove (moveID);
			health100Action.Add (move);
		}
		//75% action
		for (int index = 0; index < 20; index ++) {
			//Create new move and add them as long as there is possible move to use
			if(enemyDetail.GetText ("eChar_75AI" + (index+1) ) == "0") break; // Can't find the key and it will return 0
			string moveID = enemyDetail.GetText ("eChar_75AI" + (index+1) );
			Move move = new Move();
			move.createMove (moveID);
			health75Action.Add (move);
		}
		//50% action
		for (int index = 0; index < 20; index ++) {
			//Create new move and add them as long as there is possible move to use
			if(enemyDetail.GetText ("eChar_50AI" + (index+1) ) == "0") break; // Can't find the key and it will return 0
			string moveID = enemyDetail.GetText ("eChar_50AI" + (index+1) );
			Move move = new Move();
			move.createMove (moveID);
			health50Action.Add (move);
		}
		//25% action
		for (int index = 0; index < 20; index ++) {
			//Create new move and add them as long as there is possible move to use
			if(enemyDetail.GetText ("eChar_25AI" + (index+1) ) == "0") break; // Can't find the key and it will return 0
			string moveID = enemyDetail.GetText ("eChar_25AI" + (index+1) );
			Move move = new Move();
			move.createMove (moveID);
			health25Action.Add (move);
		}
	}

	public void createStat(CStringList file){
		e_name = file.GetText ("eChar_name");
		e_attackPower = int.Parse ( file.GetText ("eChar_attackPower") );
		e_defense = int.Parse ( file.GetText ("eChar_defense"));
		e_hp = int.Parse (file.GetText ("eChar_hp"));
		m_texturePath = "Texture/monster01/" + e_name;
	}

	//For AI based on text!
	public void decideAction(){
		//Has no queue move
		if (nextMove == null) {
		//	Debug.Log ("Do action");
			// Below X% health pool, do what move
			float percent = getCurrentHealthPercent ();
			if (percent <= 100.00f && percent > 75.00f) {
	
				do100Action ();
			}
			if (percent <= 75.00f && percent > 50.00f) {
				do75Action ();
			}

			if (percent <= 50.00f && percent > 25.00f) {
				do50Action ();
			}

			if (percent <= 25.00f) {
				do25Action ();
			}
		} else
			doNextMove ();

	}

	//Send action then get rid of the queue move
	public void doNextMove(){
		sendAction (m_battle, nextMove);
		nextMove = null;
	}

	public void do100Action(){
		int count = health100Action.Count;
		int action = Random.Range(0, count);
	//	Debug.Log ("A" + action);
	//	Debug.Log ("C" + count);
		Move move = health100Action [action];
		sendAction (m_battle, move);
	}

	public void do75Action(){
		int count = health75Action.Count;
		if (count == 0) // No action to do in 75%
			do100Action ();
		if (count > 0) {
			int action = Random.Range (0, count);
			Move move = health75Action [action];
			sendAction (m_battle, move);
		}
	}

	public void do50Action(){
		int count = health50Action.Count;
		if (count == 0)
			do75Action ();
		if (count > 0) {
			int action = Random.Range (0, count);
			Move move = health50Action [action];
			sendAction (m_battle, move);
		}
	}

	public void do25Action(){
		int count = health25Action.Count;
		if (count == 0) do50Action();
		if (count > 0) {
			int action = Random.Range (0, count);
			Move move = health25Action [action];
			sendAction (m_battle, move);
		}
	}
	

	public void sendAction(battleMain receiver, Move move){
		checkForNextMove (move);
		receiver.SendMessage ("useMove", move);
	}

	//Preperation in case there is a next move to use.
	public void checkForNextMove(Move move){
	//	Debug.Log (move.nextMoveID);
		if (move.nextMoveID != "None") {
			nextMove = new Move ();
			nextMove.createMove (move.nextMoveID);
		}
	}
}
