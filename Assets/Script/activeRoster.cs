using UnityEngine;
using System.Collections;

public class activeRoster : MonoBehaviour
{
    Character[] activeCharacter;

    public Character getActiveCharacter(long uID)
    {
        for (int i = 0; i < activeCharacter.Length; i++)
        {
            if ((uID == activeCharacter[i].getuID()) && activeCharacter[i].isAlive)
            {
                return activeCharacter[i];
            }
        }
        return new Character();
    }

    public Character getActiveCharacter(string name)
    {
        for (int i = 0; i < activeCharacter.Length; i++)
        {
            if ((name == activeCharacter[i].getName()) && activeCharacter[i].isAlive)
            {
                return activeCharacter[i];
            }
        }
        return new Character();
    }

    public Character[] getAllActiveCharacter()
    {
        return activeCharacter;
    }
}
