using UnityEngine;
using System.Collections;

public class User : MonoBehaviour {

	public string userID { get; set; }
	public string userName { get; set; }
	public int gold { get; set; }
	public int cash { get; set; }
	public int userLevel { get; set; }
	public int stamina { get; set; }
	public int timeToRegainStamina { get; set; }
	public int userEXP { get; set; }
	public int charSlots { get; set; }
	public int itemSlots { get; set; }
	//For the new user and booster, change variable to 1 for true, 0 for false.
	public int isNewUser { get; set; }
	public int isActiveBooster { get; set; }
	public string itemInventorySave{get; set;}
	public string characterInventorySave{ get; set; }
	// Use this for initialization
	void Start () {
	
	}
	
	public void setUserTest(){

	}
}
