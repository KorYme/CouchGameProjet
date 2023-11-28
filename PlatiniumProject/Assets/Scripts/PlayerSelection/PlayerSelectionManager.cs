using System;
using System.Collections;
using System.Collections.Generic;
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
    public bool IsSetUp { get; private set; } = false;

    #region Events
    /// <summary>
    /// Parameters : indexPlayer in order of connexion
    /// </summary>
    public event Action<int> OnPlayerJoined;
    /// <summary>
    /// Parameters : indexPlayer, indexCharacterChosen (barman, dj, bouncer)
    /// </summary>
    public event Action<int,int,PlayerRole> OnPlayerChooseCharacter;
    /// <summary>
    /// Parameters : indexPlayer, indexCharacterChosen (barman, dj, bouncer)
    /// </summary>
    public event Action<int,int, PlayerRole> OnPlayerUnchooseCharacter;
    /// <summary>
    /// Parameters : indexPlayer, indexCharacterChosen (barman, dj, bouncer)
    /// </summary>
    public event Action<int,int> OnPlayerMove;
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
            int indexHandler = RoleToPlayerIndex(playermap.role);
            if (indexHandler != -1)
            {
                CreateInstancePlayerSelection(indexHandler);
            } else
            {
                CreateInstancePlayerSelection(playermap.gamePlayerId);
            }
            if (indexHandler != -1) //Check if player has chosen a role before
            {
                _objectsSelectionable[playermap.gamePlayerId].TargetIndex = indexHandler; //SetUp light 
                _idPlayerSelected[indexHandler] = playermap.gamePlayerId;
            }
        }
        IsSetUp = true;
    }
    public int RoleToPlayerIndex(PlayerRole role)
    {
        int indexHandler = _selectionHandlers.ToList().FindIndex(handler => handler.Role == role);
        return indexHandler;
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
        OnPlayerJoined?.Invoke(indexPlayer);
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
    private void OnMovePlayer(int indexPlayer, int indexCurrentCharacter)
    {
        _objectsSelectionable[indexPlayer].MoveToIndex(indexCurrentCharacter);
        OnPlayerMove?.Invoke(indexPlayer, indexCurrentCharacter);
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
            _playersAssigner.ChangeMap(_idPlayerSelected[i]);
        }
        OnChangeScene.Invoke();
        SceneManager.LoadScene(1);
    }
}
