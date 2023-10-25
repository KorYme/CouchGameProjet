using Rewired;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField]int gamePlayerId = 0;
    private Rewired.Player player { get { return PlayerInputsAssigner.GetRewiredPlayer(gamePlayerId); } }
    private bool _isRegistered = false;

    #region InputsMovement
    public Vector2 MoveVector {  get; private set; } = Vector2.zero;
    public bool OnMoveDown { get; private set; } = false;
    private bool _isMoveDownRefreshed = true;
    public Vector2 MoveDirection {  get; private set; } = Vector2.zero;
    public event Action OnAxisMoveStarted;
    #endregion

    #region InputsActions
    public bool IsAction1Pressed { get; private set; } = false;
    #endregion
    public float DurationAction1Down { get; private set; } = 0f;

    void Update()
    {
        if (!ReInput.isReady) return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
        if (player == null) return;
        if (!_isRegistered)
        {
            _isRegistered = true;
            Players.AddPlayerToList(this, (int) PlayerInputsAssigner.GetRolePlayer(gamePlayerId));
        }
        GetInputs();
    }

    private void GetInputs()
    {
        MoveVector = new Vector2(player.GetAxis(RewiredConsts.Action.MOVE_HORIZONTAL), player.GetAxis(RewiredConsts.Action.MOVE_VERTICAL));
        
        if (_isMoveDownRefreshed && !OnMoveDown && MoveVector != Vector2.zero)
        {
            OnMoveDown = true;
            OnAxisMoveStarted?.Invoke();
            _isMoveDownRefreshed = false;
        } else
        {
            if (!_isMoveDownRefreshed && OnMoveDown)
            {
                OnMoveDown = false;
            }
        }
        if (MoveVector == Vector2.zero)
        {
            _isMoveDownRefreshed = true;
        }
        DurationAction1Down = (float)player.GetButtonTimePressed(RewiredConsts.Action.ACTION1);
        IsAction1Pressed = player.GetButton(RewiredConsts.Action.ACTION1);
    }

    public bool GetInput(int inputId)
    {
        return player.GetButtonDown(inputId);
    }
}
