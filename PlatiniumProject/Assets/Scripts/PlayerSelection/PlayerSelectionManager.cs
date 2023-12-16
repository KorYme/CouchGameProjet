using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerSelectionManager : MonoBehaviour
{
    [SerializeField] LerpTargetLight[] _objectsSelectionable;
    [SerializeField] CharacterSelectionHandler[] _selectionHandlers;
    int[] _idPlayerSelected; // -1 if player not selected else index of player 
    public IList<int> IdPlayerSelected {
        get {
            return _idPlayerSelected == null ? null : _idPlayerSelected.AsReadOnlyList();
        }
    }
    PlayerInputsAssigner _playersAssigner;
    [SerializeField] PlayerSelection _prefabPlayerSelection;
    List<PlayerSelection> _playersController;
    public ReadOnlyCollection<PlayerSelection> PlayersController => _playersController.AsReadOnly();
    public bool IsSetUp { get; private set; } = false;

    #region Events
    /// <summary>
    /// Parameters : indexPlayer in order of connexion, indexCharacter
    /// </summary>
    public event Action<int,int> OnPlayerJoined;
    /// <summary>
    /// Parameters : indexPlayer, indexCharacterChosen (barman, dj, bouncer)
    /// </summary>
    public event Action<int,int,PlayerRole> OnPlayerChooseCharacter;
    /// <summary>
    /// Parameters : indexPlayer, indexCharacterChosen (barman, dj, bouncer)
    /// </summary>
    public event Action<int,int, PlayerRole> OnPlayerUnchooseCharacter;
    /// <summary>
    /// Parameters : indexPlayer, indexCharacterChosen (barman, dj, bouncer), indexLastCharacter
    /// </summary>
    public event Action<int,int,int> OnPlayerMove;
    public event Action<bool> OnAllCharacterChosen;
    [SerializeField] UnityEvent OnChangeScene;
    #endregion
    private void Awake()
    {
        _playersController = new List<PlayerSelection>();
        if (_objectsSelectionable != null)
        {
            _idPlayerSelected = new int[_objectsSelectionable.Length];
        }
        for (int i = 0; i < _idPlayerSelected.Length; i++)
        {
            _idPlayerSelected[i] = -1;
        }
    }
    private void Start()
    {
        _playersAssigner = Globals.PlayerInputsAssigner;

        if (_playersAssigner == null)
        {
            Debug.LogWarning("No Player assigner instance found in the scene");
        } else
        {
            ReloadData();
            _playersAssigner.OnPlayerJoined += AddPlayerSelectionToList;
        }
        if (_selectionHandlers.Length != _objectsSelectionable.Length)
        {
            Debug.LogWarning("List of character roles (handlers) is not matching selectables");
        }
    }

    public void ReloadData()
    {
        foreach(PlayerMap playermap in _playersAssigner.PlayersMap)
        {
            _playersAssigner.SetRoleOfPlayer(playermap.GamePlayerId, PlayerRole.None);
            CreateInstancePlayerSelection(playermap.GamePlayerId);
        }
        IsSetUp = true;
    }

    public int NbPlayersHoverOnCharacter(int indexCharacter)
    {
        LerpTargetLight light;
        int total = 0;
        for (int i =0; i < _objectsSelectionable.Length; i++)
        {
            light = _objectsSelectionable[i];
            if (light.TargetIndex == indexCharacter && _playersController.Count > i)
                total++;
        }
        return total;
    }

    private void OnDestroy()
    {
        if (_playersAssigner != null)
        {
            _playersAssigner.OnPlayerJoined -= AddPlayerSelectionToList;
        }
    }

    private void AddPlayerSelectionToList()
    {
        int indexPlayer = _playersController.Count;
        CreateInstancePlayerSelection(indexPlayer);
        OnPlayerJoined?.Invoke(indexPlayer, indexPlayer);
    }

    private void CreateInstancePlayerSelection(int indexCharacterAtStart)
    {
        PlayerSelection instancePrefab = Instantiate(_prefabPlayerSelection, transform);
        int indexPlayer = _playersController.Count;
        instancePrefab.SetUp(indexCharacterAtStart, _objectsSelectionable.Length);
        instancePrefab.OnAccept += OnAcceptPlayer;
        instancePrefab.OnReturn += OnReturnPlayer;
        instancePrefab.OnMove += OnMovePlayer;
        _playersController.Add(instancePrefab);
    }
    private void OnMovePlayer(int indexPlayer, int indexCurrentCharacter,int indexLastCharacter)
    {
        _objectsSelectionable[indexPlayer].MoveToIndex(indexCurrentCharacter);
        OnPlayerMove?.Invoke(indexPlayer, indexCurrentCharacter, indexLastCharacter);
    }

    private void OnReturnPlayer(int indexPlayer, int indexCurrentCharacter)
    {
        if (_idPlayerSelected[indexCurrentCharacter] == indexPlayer) //Check if character is already chosen
        {
            bool allPlayersWereChosen = CheckAllCharactersChosen();
            _idPlayerSelected[indexCurrentCharacter] = -1;
            _playersController[indexPlayer].CanAccept = true;
            OnPlayerUnchooseCharacter?.Invoke(indexPlayer, indexCurrentCharacter, _selectionHandlers[indexCurrentCharacter].Role);
            if (allPlayersWereChosen)
            {
                OnAllCharacterChosen?.Invoke(false);
            }
        }
    }

    private void OnAcceptPlayer(int indexPlayer,int indexCurrentCharacter)
    {
        if (CheckAllCharactersChosen() && indexPlayer == 0)
        {
            ChangeScene();
        } else if (_idPlayerSelected[indexCurrentCharacter] == -1) //Check if character is not already chosen
        {
            _idPlayerSelected[indexCurrentCharacter] = indexPlayer;
            _playersController[indexPlayer].CanAccept = false;
            OnPlayerChooseCharacter?.Invoke(indexPlayer,indexCurrentCharacter, _selectionHandlers[indexCurrentCharacter].Role);
            if (CheckAllCharactersChosen())
            {
                OnAllCharacterChosen?.Invoke(true);
            }
        }
    }

    private bool CheckAllCharactersChosen() => _idPlayerSelected.ToList().TrueForAll(value => value != -1);
    
    private void ChangeScene()
    {
        for (int i = 0; i < _idPlayerSelected.Length; i++)
        {
            _playersAssigner.SetRoleOfPlayer(_idPlayerSelected[i],_selectionHandlers[i].Role);
            _playersAssigner.ChangeMapUIToNormal(_idPlayerSelected[i]);
        }
        OnChangeScene.Invoke();
    }
}
