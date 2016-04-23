using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	public string moveID; // ID to make searching easier
	public string moveName; // Move name
	public string moveDesc; // Move Description
	public int moveCost; // cost to use it
	public int moveApGain; //AP to gain
	public float flatValue; // The flat healing/damage it does
	public float multiplier; // The extra healing/damage multiplier
	public string moveType; // AoE, Single
	public string targetType; // Healing, debuff, buff, damage
	public string effect; // Special stuffs attached to it
	public bool pen; // Special stuffs attached to it
	public int hits; // Amount of hit
	public string animation; // What animation will be used
	public string sound; // What the sound path will be
	public string nextMoveID; // In case there's a next move to use on next turn
	//moveFileReader
	public CStringList moveFile = null;

	public void createMove(string ID){
//		Debug.Log (ID);
		moveFile = loadMoveFile ();
		obtainData (ID);
	}

	public CStringList loadMoveFile(){
		
		CStringList loadFile = new CStringList();
		
		loadFile.Load ("Move/MoveList");
		
		return loadFile;
	}

	public void obtainData(string ID){
		moveName = moveFile.GetText ("mMove" + ID + "_name");
		moveDesc = moveFile.GetText ("mMove" + ID + "_desc");
		moveCost = int.Parse (moveFile.GetText ("mMove" + ID + "_cost"));
		moveApGain = int.Parse (moveFile.GetText ("mMove" + ID + "_apGain"));
		flatValue = float.Parse (moveFile.GetText ("mMove" + ID + "_flatValue"));
		multiplier = float.Parse (moveFile.GetText ("mMove" + ID + "_multiplier"));
		moveType = moveFile.GetText ("mMove" + ID + "_moveType");
		targetType = moveFile.GetText ("mMove" + ID + "_targetType");
		hits = int.Parse (moveFile.GetText ("mMove" + ID + "_hits"));
		effect = moveFile.GetText ("mMove" + ID + "_effect");
		animation = moveFile.GetText ("mMove" + ID + "_animation");
		string soundLocation = moveFile.GetText ("mMove" + ID + "_sound");
		sound = "Audio/se/" + soundLocation;
		nextMoveID = moveFile.GetText ("mMove" + ID + "_nextMoveID");
		if (moveFile.GetText ("mMove" + ID + "_pen") == "False")
			pen = false;
		if (moveFile.GetText ("mMove" + ID + "_pen") == "True")
			pen = true;
	}
}
