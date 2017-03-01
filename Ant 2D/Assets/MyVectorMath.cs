using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MyVectorMath
{
    public static bool Equals(Vector2 a, Vector2 b, int decimal_places = 6)
    {
        if(decimal_places < 6)
        {
            a = RoundOff(a, decimal_places);
            b = RoundOff(b, decimal_places);
        }
        if (a.x == b.x && a.y == b.y)
            return true;
        else
            return false;
    }

    public static Vector2 RoundOff(Vector2 v, int decimal_places)
    {
        int power = (int)Mathf.Pow(10, decimal_places);
        v.x += 0.5f * Mathf.Sign(v.x) / power;
        v.x = ((int)(v.x * power)) / (float)power;
        v.y += (0.5f * Mathf.Sign(v.y) / power);
        v.y = ((int)(v.y * power)) / (float)power;
        return v;
    }
}
