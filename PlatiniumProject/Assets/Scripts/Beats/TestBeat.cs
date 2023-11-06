using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBeat : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] float _offsetTest;
    [SerializeField] SpriteRenderer _spriteRenderer;

    DateTime _lastBeatTiming;
    Coroutine _offsetCoroutine;

    private void Reset()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _lastBeatTiming = DateTime.Now;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Timing between last input : {_lastBeatTiming.Millisecond}ms");
            _lastBeatTiming = DateTime.Now;
        }
    }

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

    public void ChangeColorWithTimer()
    {
        Debug.Log("Beat");
        if (_offsetCoroutine != null)
        {
            StopCoroutine(_offsetCoroutine);
            _spriteRenderer.color = Color.red;
        }
        _offsetCoroutine = StartCoroutine(WaitForOffset());
    }

    IEnumerator WaitForOffset()
    {
        yield return new WaitForSeconds(_offsetTest);
        ChangeColorToGreen();
        yield return new WaitForSeconds(.15f);
        ChangeColorToRed();
    }

    public void Print(string text)
    {
        if (gameObject.activeSelf)
            Debug.Log(text + $" : {DateTime.Now.Second} - {DateTime.Now.Millisecond}");
    }
}
