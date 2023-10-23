using Rewired;
using Rewired.Demos;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField]int gamePlayerId = 0;
    protected Rewired.Player player { get { return PlayerInputsAssigner.GetRewiredPlayer(gamePlayerId); } }
    private bool _isRegistered = false;

    #region Inputs
    public Vector2 MoveVector {  get; private set; } = Vector2.zero;
    public float DurationAction1Down { get; private set; } = 0f;
    #endregion

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
        Debug.Log(MoveVector);
        DurationAction1Down = (float)player.GetButtonTimePressed(RewiredConsts.Action.ACTION1);
    }
}
