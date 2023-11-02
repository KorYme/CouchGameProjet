using System;
using System.Collections.ObjectModel;

public static class Players
{
    const int MAXPLAYERS = 3;

    private static PlayerInputController[] _playersController = new PlayerInputController[MAXPLAYERS];
    public static ReadOnlyCollection<PlayerInputController> PlayersController { get; private set; } = new ReadOnlyCollection<PlayerInputController>(_playersController);

    public static event Action<int> OnPlayerConnect;
    public static event Action<int> OnPlayerDisconnect;

    public static void AddPlayerToList(PlayerInputController playerController,int index)
    {
        _playersController[index] = playerController;
        OnPlayerConnect?.Invoke(index);
    }

    public static void RemovePlayerToList(int index)
    {
        _playersController[index] = null;
        OnPlayerDisconnect?.Invoke(index);
    }
}
