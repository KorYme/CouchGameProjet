using System;
using System.Collections.ObjectModel;

public static class Players
{
    const int MAXPLAYERS = 3;

    private static PlayerInputController[] _playersController = new PlayerInputController[MAXPLAYERS];
    public static ReadOnlyCollection<PlayerInputController> PlayersController { get; private set; } = new ReadOnlyCollection<PlayerInputController>(_playersController);

    public static void AddPlayerToList(PlayerInputController playerController,int index)
    {
        _playersController[index] = playerController;
    }

    public static void RemovePlayerToList(int index)
    {
        _playersController[index] = null;
    }
}
