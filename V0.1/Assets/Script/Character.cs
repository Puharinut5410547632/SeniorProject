using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	private long uID { get; set; }
	public string name { get; set; }
	public int level{get; set;}
	public int attackPower { get; set; }
	public int magicAttackPower { get; set; }
	public int defense { get; set; }
	public int maxHP { get; set; }
	public int currentHP { get; set; }
	public int maxMP { get; set; }
	public int currentMP { get; set; }
	public string job { get; set; }
	public string side { get; set; }
	public bool isAlive { get; set; }
	public bool isDisabled { get; set; }
	public string disabilityName { get; set; }
	public int disabilityDuration { get; set; }

	public void die()
	{
		isAlive = false;
	}
	
	public void revive()
	{
		isAlive = true;
	}
	
	public void isHurt(int hurt){
		currentHP -= hurt;
	}
	
	public void isHeal(int heal)
	{
		//Prevent overhealing
		if (( currentHP + heal) > maxHP)
			heal = maxHP - currentHP;

		currentHP += heal;

	}
	
	public void isCure()
	{
		isDisabled = false;
	}

}
