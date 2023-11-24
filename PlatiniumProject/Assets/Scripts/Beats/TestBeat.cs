using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBeat : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] float _offsetTest;
    [SerializeField] SpriteRenderer _spriteRenderer;

    BeatManager _beatManager;
    DateTime _lastBeatTiming;
    Coroutine _offsetCoroutine;

    private void Reset()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _lastBeatTiming = DateTime.Now;
        _beatManager = Globals.BeatManager as BeatManager;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            double deltaTime = _beatManager.BeatDeltaTime;
            if (deltaTime < _beatManager.BeatDurationInMilliseconds / 2f)
            {
                Debug.Log($"Input at {deltaTime}");
            }
            else
            {
                Debug.Log($"Input at {deltaTime - _beatManager.BeatDurationInMilliseconds}");
            }
            if (_beatManager.IsInsideBeatWindow)
            {
                StartCoroutine(ChangeColorOnBeat());
            }
        }
    }

    IEnumerator ChangeColorOnBeat()
    {
        ChangeColorToGreen();
        yield return new WaitForSeconds(.15f);
        ChangeColorToRed();
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
