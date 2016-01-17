using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    private long uID;
    private string name;
    private int attackPower;
    private int magicAttackPower;
    private int defense;
    private int hp;
    private int mp;
    public string job;
    public string side;
    public bool isAlive;
    public bool isDisabled;
    public string disabilityName;
    public int disabilityDuration;

    public string getName(){
        return name;
    }

    public long getuID()
    {
        return uID;
    }

    public int getattackPower()
    {
        return attackPower;
    }

    public int getmagicAttackPower()
    {
        return magicAttackPower;
    }

    public int getDefense()
    {
        return defense;
    }

    public int getHP()
    {
        return hp;
    }

    public int getMP()
    {
        return mp;
    }

    public string getSide()
    {
        return side;
    }

    public string getJob()
    {
        return job;
    }

    public bool getIsDisabled()
    {
        return isDisabled;
    }

    public string getDisability()
    {
        return disabilityName;
    }

    public bool feelDisability()
    {
        return true;
    }

    public void die()
    {
        isAlive = false;
    }

    public void revive()
    {
        isAlive = true;
        // maybe add some variable to store initial hp and mp 
    }

    public void isHurt(int hurt){
        hp -= hurt;
    }

    public void isHeal(int heal)
    {
        hp += heal;
    }

    public void isCure()
    {
        isDisabled = false;
    }

    // I have to commit it right now, because still don't have these support classes
    //public void useItem(Item uID)
    //{

    //}

    //public void useMove(Move uID)
    //{

    //}
}
