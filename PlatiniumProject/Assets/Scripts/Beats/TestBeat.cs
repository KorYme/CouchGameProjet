using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TestBeat : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] float _offsetTest;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] AK.Wwise.Event _musicEvent, _phaseEvent;

    DateTime _lastBeatTiming;
    Coroutine _offsetCoroutine;
    float _timer;

    private void Reset()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    IEnumerator Start()
    {
        _lastBeatTiming = DateTime.Now;
        yield return null;
        Debug.Log("Salut");
        _musicEvent?.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncGrid | (uint)AkCallbackType.AK_MusicSyncEntry, BeatCallBack);
        _phaseEvent?.Post(gameObject);
    }

    private void BeatCallBack(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        _timer = 0f;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(_timer);
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
