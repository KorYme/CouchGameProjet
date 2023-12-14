using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinMenu : MonoBehaviour
{
    [SerializeField] private GameObject _winMenu;
    [SerializeField] private TMP_Text _winText;

    [SerializeField] private float _timeToDisplaySacrifises;
    [SerializeField] private AnimationCurve _displayCurve;

    private int _actualScore;

    private Coroutine _displayRoutine;

    public int SacrifiedClient { get; set; } = 50;

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
        _winMenu.SetActive(true);
        _displayRoutine = StartCoroutine(DisplaySacrificeRoutine());
    }

    IEnumerator DisplaySacrificeRoutine()
    {
        _winText.text = _winText.text.Replace("NANANA", "0");
        _actualScore = 0;
        float timer = 0f;
        float percentage = 0f;
        //yield return new WaitForSeconds(0.5f);
        while (timer < _timeToDisplaySacrifises)
        {
            timer += Time.deltaTime;
            percentage = _displayCurve.Evaluate(timer / _timeToDisplaySacrifises);
            _winText.text = _winText.text.Replace(_actualScore.ToString(), ((int)(SacrifiedClient * percentage)).ToString());
            _actualScore = (int)(SacrifiedClient * percentage);
            yield return null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayWinMenu();
        }
    }
}
