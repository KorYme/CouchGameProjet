using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _winMenu;
    [SerializeField] private TMP_Text _winText;
    
    public int SacrifiedClient { get; set; }

    private void Awake()
    {
        Globals.GameManager ??= this;
    }

    private void OnEnable()
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
        _winText.text = _winText.text.Replace("NANANA", SacrifiedClient.ToString());
    }
    
}
