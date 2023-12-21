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
    public int RewiredPlayerId = -1;
    public int GamePlayerId;
    public PlayerRole Role = PlayerRole.None;
    public ControllerType Type;
    public InputDevice Device = InputDevice.None; //A VERIFIER AVEC L'AFFICHAGE DES QTE
    public int IndexDevice = -1; // Pour retrouver la bonne manette lors d'une déconnexion

    public PlayerMap(int rewiredPlayerId, int gamePlayerId, ControllerType type, PlayerRole role, InputDevice device, int indexDevice)
    {
        RewiredPlayerId = rewiredPlayerId;
        GamePlayerId = gamePlayerId;
        Type = type;
        Role = role;
        Device = device;
        IndexDevice = indexDevice;
    }

    public PlayerMap(int gamePlayerId, PlayerRole role)
    {
        GamePlayerId = gamePlayerId;
        Role = role;
    }

    public void AddController(int rewiredPlayerId, ControllerType type, InputDevice device, int indexDevice)
    {
        RewiredPlayerId = rewiredPlayerId;
        Type = type;
        Device = device;
        IndexDevice = indexDevice;
    }

    public void RemoveController()
    {
        RewiredPlayerId = -1;
        IndexDevice = -1;
        Device = InputDevice.None;
    }
}
