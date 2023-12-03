using System;
using System.Collections;
using UnityEngine;

public class PlayerSelectionController : MonoBehaviour
{
    Rewired.Player _player;
    public int PlayerId { get; set; } = 0;
    [SerializeField] float _repeatDelay = 0.2f;
    bool _canMove = true;
    Coroutine _routineWaitMoveMenu;

    #region Events
    public event Action<int,int> OnMoveInput;
    public event Action<int> OnAccept;
    public event Action<int> OnReturn;
    #endregion

    public IEnumerator ChangePlayer(int playerId)
    {
        PlayerId = playerId;
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
        if (_player.GetButtonDown(RewiredConsts.Action.UISUBMIT))
        {
            OnAccept?.Invoke(PlayerId);
        }
        if (_player.GetButtonDown(RewiredConsts.Action.UICANCEL))
        {
            OnReturn?.Invoke(PlayerId);
        }
        if (_player.GetAxis(RewiredConsts.Action.UIHORIZONTAL) != 0f)
        {
            int direction = _player.GetAxis(RewiredConsts.Action.UIHORIZONTAL) > 0f ? 1 : -1;
            if (_repeatDelay > 0f) {
                if (_canMove)
                {
                    CallOnMoveMenu(direction);
                    _routineWaitMoveMenu = StartCoroutine(RoutineMoveMenu());
                } 
            } else
            {
                CallOnMoveMenu(direction);
            }
        } else if (_routineWaitMoveMenu != null)
        {
            StopCoroutine(_routineWaitMoveMenu);
            _routineWaitMoveMenu = null;
            ResetRoutine();
        }
    }

    private IEnumerator RoutineMoveMenu()
    {
        _canMove = false;
        yield return new WaitForSeconds(_repeatDelay);
        ResetRoutine();
    } 

    private void ResetRoutine()
    {
        _canMove = true;
    }
    private void CallOnMoveMenu(int direction)
    {
        OnMoveInput?.Invoke(PlayerId, direction);
    }
}
