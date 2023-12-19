using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Sequence = Unity.VisualScripting.Sequence;

public class WinMenu : MonoBehaviour
{
    [SerializeField] private RectTransform _winMenu;
    [SerializeField] private TMP_Text _winText;

    [SerializeField] private float _timeToDisplaySacrifises;
    [SerializeField] private AnimationCurve _displayCurve;

    [SerializeField] private AK.Wwise.Event _incremetation;
    
    [SerializeField] private Transform _flame;
    [SerializeField] private Vector2 _flameMinMaxPos;
    [SerializeField] private int _clientMaxAmountForFlames;

    public UnityEvent OnWinDisplay;
    public int DebugClient;

    private int _actualScore;

    private Coroutine _displayRoutine;

    public int SacrifiedClient { get; set; }

    private void Awake()
    {
        Globals.WinMenu ??= this;
    }

    private void Start()
    {
        Globals.DropManager.OnGameEnd += DisplayWinMenu;
    }

    private void OnDisable()
    {
        Globals.DropManager.OnGameEnd -= DisplayWinMenu;
    }

    private void DisplayWinMenu()
    {
        _winMenu.gameObject.SetActive(true);
        OnWinDisplay?.Invoke();
        _displayRoutine = StartCoroutine(DisplaySacrificeRoutine());
    }

    IEnumerator DisplaySacrificeRoutine()
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.Join(_winMenu.DOScale(Vector3.one * 1.05f, (float)Globals.BeatManager.BeatDurationInSeconds/2));
        sequence.SetLoops(-1, LoopType.Yoyo);
        _winText.text = _winText.text.Replace("NANANA", "0");
        _actualScore = 0;
        float timer = 0f;
        float percentage = 0f;
        while (timer < _timeToDisplaySacrifises)
        {
            timer += Time.deltaTime;
            percentage = _displayCurve.Evaluate(timer / _timeToDisplaySacrifises);
            _winText.text = _winText.text.Replace(_actualScore.ToString(), ((int)(SacrifiedClient * percentage)).ToString());
            if (_actualScore != (int)(SacrifiedClient * percentage))
            {
                _incremetation.Post(gameObject);
            }
            _actualScore = (int)(SacrifiedClient * percentage);
            _flame.localPosition = new Vector3(0, Mathf.Lerp(_flameMinMaxPos.x, _flameMinMaxPos.y, Mathf.Clamp01((_actualScore / (float)_clientMaxAmountForFlames))), 0);
            yield return null;
        }
        
        yield return new WaitUntil(() =>
        {
            for (int i = 0; i < 3; ++i)
            {
                if (PlayerInputsAssigner.GetRewiredPlayerById(i)?.GetAnyButtonDown() ?? false)
                    return true;
            }
            return false;
        });
        SceneManager.LoadScene("MainMenu");
        sequence.Kill();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _actualScore = 0;
            SacrifiedClient = DebugClient;
            DisplayWinMenu();
        }
    }
}
