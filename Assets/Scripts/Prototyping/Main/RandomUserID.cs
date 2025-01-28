using System;
using UnityEngine;
using Random = System.Random;

public class RandomUserID : MonoBehaviour
{

    public static string RandomID()
    {
        char[] chars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        Random random = new Random((int)Time.time);

        int length = chars.Length;

        int randomInt1 = random.Next(0, length);
        int randomInt2 = random.Next(0, length);
        int randomInt3 = random.Next(0, 9);
        int randomInt4 = random.Next(0, 9);

        return $"{chars[randomInt1]}" + $"{chars[randomInt2]}" + randomInt3 + "" + randomInt4;
    }
}