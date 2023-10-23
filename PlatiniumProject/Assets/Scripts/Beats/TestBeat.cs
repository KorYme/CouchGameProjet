using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBeat : MonoBehaviour
{
    public void ChangeColorToRed()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void ChangeColorToBlue()
    {
        GetComponent<SpriteRenderer>().color = Color.blue;
    }

    public void Print(string text)
    {
        if (enabled)
            Debug.Log(text);
    }
}
