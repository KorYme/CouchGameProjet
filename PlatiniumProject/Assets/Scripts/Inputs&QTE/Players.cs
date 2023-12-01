using System;
using System.Collections.ObjectModel;
using UnityEngine;

public static class Players
{
    public const int MAXPLAYERS = 3;
    public static int PlayerConnected { get; private set; } = 0;

    private static PlayerInputController[] _playersController = new PlayerInputController[MAXPLAYERS];
    public static ReadOnlyCollection<PlayerInputController> PlayersController { get; private set; } = new ReadOnlyCollection<PlayerInputController>(_playersController);

    public static event Action<int> OnPlayerConnect;
    public static event Action<int> OnPlayerDisconnect;

    public static void AddPlayerToList(PlayerInputController playerController, int indexRole)
    {
        PlayerConnected++;
        _playersController[indexRole] = playerController;
        OnPlayerConnect?.Invoke(indexRole);
    }

    public static void RemovePlayerToList(int index)
    {
        PlayerConnected--;
        _playersController[index] = null;
        OnPlayerDisconnect?.Invoke(index);
    }
}
