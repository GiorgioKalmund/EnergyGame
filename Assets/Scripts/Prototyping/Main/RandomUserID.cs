using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomUserID : MonoBehaviour
{

    public static string RandomID(int length = 5)
    {
        char[] chars = "abcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();
        int charslength = chars.Length - 1;

        string randomID = "";
        
        for (int index = 0; index < length; index++)
        {
            int randomInt = Random.Range(0, charslength);
            randomID += $"{chars[randomInt]}";
        }

        return randomID;
    }
}