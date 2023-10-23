using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBeat : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;

    public void ChangeColorToRed()
    {
        _spriteRenderer.color = Color.red;
    }

    public void ChangeColorToBlue()
    {
        _spriteRenderer.color = Color.blue;
    }

    public void ChangeColorToGreen()
    {
        _spriteRenderer.color = Color.green;
    }

    public void Print(string text)
    {
        if (gameObject.activeSelf)
            Debug.Log(text + $" : {DateTime.Now.Second} - {DateTime.Now.Millisecond}");
    }
}
