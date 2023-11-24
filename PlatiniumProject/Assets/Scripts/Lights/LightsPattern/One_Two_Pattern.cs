using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class One_Two_Pattern : LightsPattern
{
    int _even_Odd;
    public One_Two_Pattern()
    {
        _even_Odd = Random.Range(0, 2);
    }

    public override void UpdatePattern() => _even_Odd = (_even_Odd + 1) % 2;

    public override bool IsThisLightEnlighted(int index) => index % 2 == _even_Odd;
}
