using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu, _winMenu;
    [SerializeField] private TMP_Text _winText;
    
    public bool IsGamePaused {  get; private set; }
    public int SacrifiedClient { get; set; }

    public event Action<bool> OnGamePaused;

    private void Awake()
    {
        Globals.GameManager ??= this;
    }

    private void Start()
    {
        IsGamePaused = false;
        OnGamePaused += Globals.MenuMusicPlayer.PauseOrResumeMusicMenu;
        OnGamePaused += Globals.BeatManager.PauseOrResumeMainMusic;
        Globals.DropManager.OnGameWon += DisplayWinMenu;
        Players.OnPlayerConnect += playerRole => Players.PlayersController[playerRole].Pause.OnInputStart += () => AssignPlayerToPauseMenuAndPause(playerRole);
        for (int i = 0; i < Players.MAXPLAYERS; i++)
        {
            if (Players.PlayersController[i] != null)
            {
                Players.PlayersController[i].Pause.OnInputStart += () => AssignPlayerToPauseMenuAndPause(i);
            }
        }
    }

    private void OnDisable()
    {
        Globals.DropManager.OnGameWon -= DisplayWinMenu;
        Players.OnPlayerConnect -= playerRole => Players.PlayersController[playerRole].Pause.OnInputStart += () => AssignPlayerToPauseMenuAndPause(playerRole);
        for (int i = 0; i < Players.MAXPLAYERS; i++)
        {
            if (Players.PlayersController[i] != null)
            {
                Players.PlayersController[i].Pause.OnInputStart -= () => AssignPlayerToPauseMenuAndPause(i);
            }
        }
    }

    private void DisplayWinMenu()
    {
        _winMenu.SetActive(true);
        _winText.text = _winText.text.Replace("NANANA", SacrifiedClient.ToString());
    }
    
    public void PauseOrResumeGame()
    {
        IsGamePaused = !IsGamePaused;
        OnGamePaused?.Invoke(IsGamePaused);
        _pauseMenu.SetActive(IsGamePaused);
        if (!IsGamePaused)
        {
            UnAssignPlayerToPauseMenu();
        }
    }

    void AssignPlayerToPauseMenuAndPause(int playerRole)
    {
        if (!Globals.DropManager?.IsGamePlaying ?? false) return;
        AssignPlayerToPauseMenu(playerRole);
        PauseOrResumeGame();
    }

    public void AssignPlayerToPauseMenu(int playerRole)
    {
        Players.PlayersController[playerRole]?.newPlayer.controllers.maps.SetMapsEnabled(true, RewiredConsts.Category.UI);
        for (int i = 0; i < Players.MAXPLAYERS; i++)
        {
            Players.PlayersController[i]?.newPlayer.controllers.maps.SetMapsEnabled(false, "Default", "Default");
        }
    }

    public void SetAllPlayerToUIMode()
    {
        for (int i = 0; i < Players.MAXPLAYERS; i++)
        {
            Players.PlayersController[i]?.newPlayer.controllers.maps.SetMapsEnabled(true, RewiredConsts.Category.UI);
            Players.PlayersController[i]?.newPlayer.controllers.maps.SetMapsEnabled(false, "Default", "Default");
        }
    }

    public void UnAssignPlayerToPauseMenu()
    {
        for (int i = 0; i < Players.MAXPLAYERS; i++)
        {
            Players.PlayersController[i]?.newPlayer.controllers.maps.SetMapsEnabled(false, RewiredConsts.Category.UI);
            Players.PlayersController[i]?.newPlayer.controllers.maps.SetMapsEnabled(true, "Default", "Default");
        }
    }
}
