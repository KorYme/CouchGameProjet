using System;
using System.Collections;
using UnityEngine;

public class PlayerSelectionController : MonoBehaviour
{
    Rewired.Player _player;
    public int PlayerId { get; set; } = 0;
    [SerializeField] float _repeatDelay = 0.2f;
    int _lastDirection = 0;
    bool _canMove = true;
    Coroutine _routineWaitMoveMenu;

    #region Events
    public event Action<int,int> OnMoveInput;
    public event Action<int> OnAccept;
    public event Action<int> OnReturn;
    #endregion
    IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerInputsAssigner.GetRewiredPlayerById(PlayerId) != null);
        _player = PlayerInputsAssigner.GetRewiredPlayerById(PlayerId);
        Debug.Log("PLAYER "+PlayerId);
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
            Debug.Log("ACCEPT");
            OnAccept?.Invoke(PlayerId);
        }
        if (_player.GetButtonDown(RewiredConsts.Action.RETURN))
        {
            Debug.Log("RETURN");
            OnAccept?.Invoke(PlayerId);
        }
        if (_player.GetAxis(RewiredConsts.Action.MOVEMENU) != 0f)
        {
            int direction = _player.GetAxis(RewiredConsts.Action.MOVEMENU) > 0f ? 1 : -1;
            if (_repeatDelay > 0f) {
                
                if (_canMove)
                {
                    CallOnMoveMenu(direction);
                    _lastDirection = direction ;
                    _routineWaitMoveMenu = StartCoroutine(RoutineMoveMenu());
                } else if (_lastDirection != direction)
                {
                    StopCoroutine(_routineWaitMoveMenu);
                    CallOnMoveMenu(direction);
                    _lastDirection = direction;
                    _routineWaitMoveMenu = StartCoroutine(RoutineMoveMenu());
                }
            } else
            {
                CallOnMoveMenu(direction);
            }
        }
    }

    private IEnumerator RoutineMoveMenu()
    {
        _canMove = false;
        yield return new WaitForSeconds(_repeatDelay);
        _lastDirection = 0;
        _canMove = true;
    } 

    private void CallOnMoveMenu(int direction)
    {
        OnMoveInput?.Invoke(PlayerId, direction);
    }
}
