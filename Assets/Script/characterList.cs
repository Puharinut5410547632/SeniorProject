using UnityEngine;
using System.Collections;

public class characterList : MonoBehaviour
{
    Character[] characterList;

    public Character getCharacter(long uID)
    {
        for (int i = 0; i < characterList.Length; i++)
        {
            if (uID == characterList[i].getuID()) {
                return characterList[i];
            }
        }
        return new Character();
    }

    public Character getCharacter(string name)
    {
        for (int i = 0; i < characterList.Length; i++)
        {
            if (name == characterList[i].getName())
            {
                return characterList[i];
            }
        }
        return new Character();
    }

    public Character[] getAllCharacter()
    {
        return characterList;
    }
}
