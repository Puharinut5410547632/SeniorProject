using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{
    private long moveID;
    private int moveAttackDamage;
    private int moveMagicDamage;
    private int moveDurabilityCost;
    public string moveElement;

    public long getMoveID()
    {
        return moveID;
    }

    public int getMoveAttackDamage()
    {
        return moveAttackDamage;
    }

    public int getMoveMagicDamage()
    {
        return moveMagicDamage;
    }

    public int getMoveDurabilityCost()
    {
        return moveDurabilityCost;
    }

    public bool useMove()
    {
        return true; //not sure about this
    }

    public void calculateDamage()
    {
        // ??
    }

    public void calculateHeal()
    {
        // ??
    }
}
