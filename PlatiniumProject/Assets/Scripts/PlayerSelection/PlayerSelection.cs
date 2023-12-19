using System;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    [SerializeField]PlayerSelectionController _controller;

    #region Events
    public event Action<int,int> OnAccept;
    public event Action<int,int> OnReturn;
    public event Action<int,int,int> OnMove; // id player, position, last position
    public event Action<int,int> OnReturnUp;
    #endregion
    int _indexCharacter = 0;
    int _maxCharacterPlayable = 1;
    public bool CanAccept { get; set; } = true;
    public bool HasStartedHolding { get; set; } = false;

    private void Awake()
    {
        _controller = GetComponent<PlayerSelectionController>();
        if (_controller == null)
        {
            Debug.LogWarning("Player Selection Controller not found");
        } else
        {
            _controller.OnAccept += OnAcceptController;
            _controller.OnReturn += OnReturnController;
            _controller.OnMoveInput += OnMoveController;
            _controller.OnReturnUp += OnReturnUpController;
        }
    }

    public void SetUp(int indexStart, int maxCharacters)
    {
        _indexCharacter = indexStart;
        StartCoroutine(_controller.ChangePlayer(_indexCharacter));
        _maxCharacterPlayable = maxCharacters;
    }

    private void OnReturnController(int indexPlayer)
    {
        OnReturn?.Invoke(indexPlayer, _indexCharacter);
    }

    private void OnAcceptController(int indexPlayer)
    {
        OnAccept?.Invoke(indexPlayer, _indexCharacter);
    }

    private void OnMoveController(int indexPlayer, int direction)
    {
        if (CanAccept)
        {
            int newIndex = Mathf.Clamp(_indexCharacter + direction, 0, _maxCharacterPlayable - 1);
            if (_indexCharacter !=  newIndex)
            {
                int lastIndex = _indexCharacter;
                _indexCharacter = newIndex;
                OnMove?.Invoke(indexPlayer, _indexCharacter, lastIndex);
            }
        }
    }

    private void OnReturnUpController(int indexPlayer)
    {
        if (HasStartedHolding)
        {
            OnReturnUp?.Invoke(indexPlayer, _indexCharacter);
            HasStartedHolding = false;
        }
    }
}
