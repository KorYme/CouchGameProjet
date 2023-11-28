using System;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    [SerializeField]PlayerSelectionController _controller;

    #region Events
    public event Action<int,int> OnAccept;
    public event Action<int,int> OnReturn;
    public event Action<int,int> OnMove; // id player, position
    #endregion
    int _indexCharacter = 0;
    int _maxCharacterPlayable = 1;
    public bool CanAccept { get; set; } = true;

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
                _indexCharacter = newIndex;
                OnMove?.Invoke(indexPlayer, _indexCharacter);
            }
        }
    }
}
