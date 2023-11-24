using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectionManager : MonoBehaviour
{
    [SerializeField] LerpTargetLight[] _objectsSelectionable;
    int[] _idPlayerSelected; // -1 if player not selected else index of player 
    PlayerInputsAssigner _playersAssigner;
    [SerializeField] PlayerSelection _prefabPlayerSelection;
    List<PlayerSelection> _playersController;
    public event Action<int> OnPlayerJoined;
    private void Start()
    {
        _playersController = new List<PlayerSelection>();
        _playersAssigner = FindObjectOfType<PlayerInputsAssigner>();
        if (_objectsSelectionable != null)
        {
            _idPlayerSelected = new int[_objectsSelectionable.Length];
        }
        for (int i = 0;i  < _idPlayerSelected.Length; i++)
        {
            _idPlayerSelected[i] = -1;
        }
        if (_playersAssigner == null)
        {
            Debug.LogWarning("No Player assigner instance found in the scene");
        } else
        {
            _playersAssigner.OnPlayerJoined += PlayerJoin;
        }
    }

    private void PlayerJoin()
    {
        PlayerSelection instancePrefab = Instantiate(_prefabPlayerSelection,transform);
        instancePrefab.SetUp(_playersController.Count, _objectsSelectionable.Length);
        instancePrefab.OnAccept += OnAcceptPlayer;
        instancePrefab.OnReturn += OnReturnPlayer;
        instancePrefab.OnMove += OnMovePlayer;
        _playersController.Add(instancePrefab);
    }

    private void OnMovePlayer(int indexPlayer, int indexCurrentCharacter)
    {
        _objectsSelectionable[indexPlayer].MoveToIndex(indexCurrentCharacter);
    }

    private void OnReturnPlayer(int indexPlayer, int indexCurrentCharacter)
    {
        if (_idPlayerSelected[indexCurrentCharacter] == indexPlayer) //Check if character is already chosen
        {
            _idPlayerSelected[indexCurrentCharacter] = -1;
            _playersController[indexPlayer].CanAccept = true;
        }
    }

    private void OnAcceptPlayer(int indexPlayer,int indexCurrentCharacter)
    {
        if (_idPlayerSelected[indexCurrentCharacter] == -1) //Check if character not already chosen
        {
            _idPlayerSelected[indexCurrentCharacter] = indexPlayer;
            _playersController[indexPlayer].CanAccept = false;
        }
    }
}
