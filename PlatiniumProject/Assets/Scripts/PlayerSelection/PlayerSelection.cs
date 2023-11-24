using System;
using System.Collections;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    Rewired.Player _player;
    public int PlayerId { get; set; }
    [SerializeField] float _repeatDelay = 0.2f;
    int _lastDirection = 0;
    bool _canMove = true;
    Coroutine _routineWaitMoveMenu;

    #region Events
    public event Action<int> OnLeftInput;
    public event Action<int> OnRightInput;
    public event Action<int> OnAccept;
    public event Action<int> OnReturn;
    #endregion
    IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerInputsAssigner.GetRewiredPlayerById(PlayerId) != null);
        _player = PlayerInputsAssigner.GetRewiredPlayerById(PlayerId);
    }

    private void Update()
    {
        if (_player != null)
        {
            CheckInputs();
        }
    }

    private void CheckInputs()
    {
        if (_player.GetButtonDown(RewiredConsts.Action.ACCEPT))
        {
            OnAccept?.Invoke(PlayerId);
        }
        if (_player.GetButtonDown(RewiredConsts.Action.RETURN))
        {
            OnAccept?.Invoke(PlayerId);
        }
        if (_player.GetAxis(RewiredConsts.Action.MOVEMENU) != 0f)
        {
            if (_repeatDelay > 0f) {
                if (_canMove)
                {
                    float direction = _player.GetAxis(RewiredConsts.Action.MOVEMENU);
                    CallOnMoveMenu(direction);
                    _routineWaitMoveMenu = StartCoroutine(RoutineMoveMenu());
                } else
                {
                }
            } else
            {
                CallOnMoveMenu(_player.GetAxis(RewiredConsts.Action.MOVEMENU));
            }
        }
    }

    private IEnumerator RoutineMoveMenu()
    {
        _canMove = false;
        yield return new WaitForSeconds(_repeatDelay);
        _canMove = true;
    } 

    private void CallOnMoveMenu(float direction)
    {
        if (direction > 0f)
        {
            OnRightInput?.Invoke(PlayerId);
        }
        else
        {
            OnLeftInput?.Invoke(PlayerId);
        }
    }
}
