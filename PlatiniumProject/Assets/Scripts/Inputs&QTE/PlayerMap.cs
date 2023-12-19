using Rewired;
public enum PlayerRole
{
    Barman = 0,
    DJ = 1,
    Bouncer = 2,
    None
}
public class PlayerMap
{
    public int RewiredPlayerId;
    public int GamePlayerId;
    public PlayerRole Role;
    public ControllerType Type;
    public InputDevice Device = InputDevice.XBox;
    public int IndexDevice;
    public PlayerMap(int rewiredPlayerId, int gamePlayerId, ControllerType type, PlayerRole role, int indexDevice)
    {
        RewiredPlayerId = rewiredPlayerId;
        GamePlayerId = gamePlayerId;
        Type = type;
        Role = role;
        IndexDevice = indexDevice;
    }

    public PlayerMap(int rewiredPlayerId, int gamePlayerId, ControllerType type, PlayerRole role, InputDevice device, int indexDevice) : this(rewiredPlayerId, gamePlayerId, type, role,indexDevice)
    {
        Device = device;
    }
}
