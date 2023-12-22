using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public static class Players
{
    public const int MAXPLAYERS = 3;

    private static PlayerInputController[] _playersController = new PlayerInputController[MAXPLAYERS];
    public static ReadOnlyCollection<PlayerInputController> PlayersController { get; private set; } = new ReadOnlyCollection<PlayerInputController>(_playersController);

    public static event Action<int> OnPlayerConnect;
    public static event Action<int> OnPlayerDisconnect;

    private static List<IIsControllable> _objectsUsingController = new List<IIsControllable>();

    public static void AddPlayerToList(PlayerInputController playerController, int indexRole)
    {
        _playersController[indexRole] = playerController;
        OnPlayerConnect?.Invoke(indexRole);
    }

    public static void RemovePlayerToList(int index)
    {
        _playersController[index] = null;
        OnPlayerDisconnect?.Invoke(index);
    }

    public static void ExchangePlayers(int indexFirstRole, int indexSecondRole)
    {
        PlayerInputController playerInputController = _playersController[indexFirstRole];
        _playersController[indexFirstRole] = _playersController[indexSecondRole];
        _playersController[indexSecondRole] = playerInputController;
        _playersController[indexFirstRole]?.SetPlayer();
        _playersController[indexSecondRole]?.SetPlayer();
        foreach(IIsControllable obj in _objectsUsingController)
        {
            obj.ChangeController();
        }
    }

    public static void AddListenerPlayerController(IIsControllable obj)
    {
        _objectsUsingController.Add(obj);
    }

    public static void RemoveListenerPlayerController(IIsControllable obj)
    {
        _objectsUsingController.Remove(obj);
    }
}
