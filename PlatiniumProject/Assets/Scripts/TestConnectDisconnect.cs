using Rewired;
using System.Collections.Generic;
using UnityEngine;

public class TestConnectDisconnect : MonoBehaviour
{
    [SerializeField] PlayerInputsAssigner _playerAssigner;
    Dictionary<int,bool> _indexPlayers = new Dictionary<int, bool> 
    { [1] = false,
      [3] = false,
      [5] = false
    };
    void Awake()
    {
        // Subscribe to events
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
       
    }

    private void Start()
    {
        foreach (Joystick j in ReInput.controllers.Joysticks)
        {
            if (ReInput.controllers.IsJoystickAssigned(j)) continue; // Joystick is already assigned
            // Assign Joystick to first Player that doesn't have any assigned
            AssignJoystickToNextOpenPlayer(j,false);
        }
    }

    // This function will be called when a controller is connected
    // You can get information about the controller that was connected via the args parameter
    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        if (args.controllerType != ControllerType.Joystick) return; // skip if this isn't a Joystick

        // Assign Joystick to first Player that doesn't have any assigned
        AssignJoystickToNextOpenPlayer(ReInput.controllers.GetJoystick(args.controllerId));
        Debug.Log("A controller was connected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
    }

    // This function will be called when a controller is fully disconnected
    // You can get information about the controller that was disconnected via the args parameter
    void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
    {
        Debug.Log("A controller was disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        ReInput.controllers.RemoveControllerFromAllPlayers(args.controller);
        int playerId = _playerAssigner.RemovePlayer(args.controllerId);
        if (playerId != -1)
        {
            _indexPlayers[playerId] = false;
        }
    }

    // This function will be called when a controller is about to be disconnected
    // You can get information about the controller that is being disconnected via the args parameter
    // You can use this event to save the controller's maps before it's disconnected
    /*void OnControllerPreDisconnect(ControllerStatusChangedEventArgs args)
    {
        Debug.Log("A controller is being disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
    }*/
    void AssignJoystickToNextOpenPlayer(Joystick j, bool callEvent = true)
    {
        foreach (Player p in ReInput.players.Players)
        {
            if (p.controllers.joystickCount > 0) continue; // player already has a joystick
            if (!_indexPlayers.ContainsKey(p.id) || _indexPlayers[p.id]) continue;
            Debug.Log($"ASSIGNED to {p.id} {p.name}");
            p.controllers.AddController(j, true); // assign joystick to player
            _indexPlayers[p.id] = true;
            _playerAssigner.AssignNextPlayer(p.id, j, callEvent);
            return;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        ReInput.ControllerConnectedEvent -= OnControllerConnected;
        ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
        //ReInput.ControllerPreDisconnectEvent -= OnControllerPreDisconnect;
    }
}
