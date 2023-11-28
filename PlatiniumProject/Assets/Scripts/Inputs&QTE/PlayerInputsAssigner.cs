
using UnityEngine;
using System.Collections.Generic;
using Rewired;
using System;
using Unity.VisualScripting;

public enum PlayerRole
{
    Barman = 0,
    DJ = 1,
    Bouncer = 2,
    None
}
public class PlayerMap
{
    public int rewiredPlayerId;
    public int gamePlayerId;
    public PlayerRole role;
    public ControllerType type;
    public PlayerMap(int rewiredPlayerId, int gamePlayerId, ControllerType type, PlayerRole role)
    {
        this.rewiredPlayerId = rewiredPlayerId;
        this.gamePlayerId = gamePlayerId;
        this.type = type;
        this.role = role;
    }
}
public class PlayerInputsAssigner : MonoBehaviour {

    const int MAXPLAYERS = 3;

    readonly int[] _rolesKB = new int[MAXPLAYERS] { RewiredConsts.Layout.Keyboard.PLAYER1, RewiredConsts.Layout.Keyboard.PLAYER2, RewiredConsts.Layout.Keyboard.PLAYER3 };
    [SerializeField] PlayerRole[] _playerRoles = new PlayerRole[MAXPLAYERS];
    [SerializeField] bool _characterSelectionInGame = false;
    public event Action OnPlayerJoined;
    private static PlayerInputsAssigner _instance;
    int _indexRoleKB = 0;
    #region GetPlayer
    public static Rewired.Player GetRewiredPlayerByRole(int role) {
        if(!Rewired.ReInput.isReady) return null;
        if(_instance == null) {
            Debug.LogError("Not initialized.");
            return null;
        }
        for(int i = 0; i < _instance._playerMap.Count; i++) {
            if(((int)_instance._playerMap[i].role) == role) return ReInput.players.GetPlayer(_instance._playerMap[i].rewiredPlayerId);
        }
        return null;
    }
    public static Rewired.Player GetRewiredPlayerById(int playerId)
    {
        if (!Rewired.ReInput.isReady) return null;
        if (_instance == null)
        {
            Debug.LogError("Not initialized.");
            return null;
        }
        for (int i = 0; i < _instance._playerMap.Count; i++)
        {
            if (_instance._playerMap[i].gamePlayerId == playerId)
            {
                return ReInput.players.GetPlayer(_instance._playerMap[i].rewiredPlayerId);
            }
        }
        return null;
    }

    public static PlayerRole GetRolePlayer(int gamePlayerId)
    {
        if (!Rewired.ReInput.isReady) return PlayerRole.None;
        if (_instance == null)
        {
            Debug.LogError("Not initialized.");
            return PlayerRole.None;
        }
        for (int i = 0; i < _instance._playerMap.Count; i++)
        {
            if (_instance._playerMap[i].gamePlayerId == gamePlayerId)
            {
                return _instance._playerMap[i].role;
            }
        }
        return PlayerRole.None;
    }
    #endregion
    private List<PlayerMap> _playerMap; // Maps Rewired Player ids to game player ids
    public IList<PlayerMap> PlayersMap => _playerMap.AsReadOnlyList();
    private int gamePlayerIdCounter = 0;

    void Awake() {
        _playerMap = new List<PlayerMap>();
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Globals.PlayerInputsAssigner ??= this;
        _instance = this;
    }

    void Update() {

        if (_playerMap.Count < MAXPLAYERS)
        {
            // Watch for JoinGame action in each Player
            for (int i = 0; i < ReInput.players.playerCount; i++)
            {
                if (ReInput.players.GetPlayer(i).GetButtonDown("JoinGame"))
                {
                    AssignNextPlayer(i, ReInput.players.GetPlayer(i).controllers.GetLastActiveController().type);
                    
                    switch (ReInput.players.GetPlayer(i).controllers.GetLastActiveController().type)
                    {
                        case ControllerType.Joystick:
                            ChangeMapJoystick(i);
                            OnPlayerJoined?.Invoke();
                            break;
                        case ControllerType.Keyboard:
                            ChangeMapKeyboard(i);
                            OnPlayerJoined?.Invoke();
                            return;
                    }
                }
            }
        }
    }

    public void SetRoleOfPlayer(int indexPlayer, PlayerRole role)
    {
        _playerMap[indexPlayer].role = role;
    }
    void AssignNextPlayer(int rewiredPlayerId,ControllerType type) {
        if(_playerMap.Count >= MAXPLAYERS) {
            Debug.LogError("Max player limit already reached!");
            return;
        }
        int gamePlayerId = GetNextGamePlayerId();

        // Add the Rewired Player as the next open game player slot
        if (_characterSelectionInGame)
        {
            _playerMap.Add(new PlayerMap(rewiredPlayerId, gamePlayerId, type, PlayerRole.None));
        } else
        {
            _playerMap.Add(new PlayerMap(rewiredPlayerId, gamePlayerId,type,(PlayerRole) gamePlayerId));
        }
        Debug.Log("Added Rewired Player id " + rewiredPlayerId + " to game player " + gamePlayerId);
    }

    void ChangeMapJoystick(int rewiredPlayerId)
    {
        Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

        // Disable the Assignment map category in Player so no more JoinGame Actions return
        rewiredPlayer.controllers.maps.SetMapsEnabled(false, RewiredConsts.Category.ASSIGNMENT);

        // Enable UI control for this Player now that he has joined
        if (_characterSelectionInGame)
        {
            rewiredPlayer.controllers.maps.SetMapsEnabled(true, RewiredConsts.Category.UI);
        } else
        {
            rewiredPlayer.controllers.maps.SetMapsEnabled(true, "Default", "Default");
        }
    }


    //TO DO enleve assignment au démarrage => que ans la scène de selection
    
    
    void ChangeMapKeyboard(int rewiredPlayerId)
    {
        Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

        // Disable the Assignment map category in Player so no more JoinGame Actions return
        rewiredPlayer.controllers.maps.SetMapsEnabled(false, RewiredConsts.Category.ASSIGNMENT);
        // Enable UI control for this Player now that he has joined
        if (_characterSelectionInGame)
        {
            rewiredPlayer.controllers.maps.SetMapsEnabled(true, ControllerType.Keyboard, RewiredConsts.Category.UI, _rolesKB[_indexRoleKB]);
        }
        else
        {
            rewiredPlayer.controllers.maps.SetMapsEnabled(true, ControllerType.Keyboard, RewiredConsts.Category.DEFAULT, _rolesKB[_indexRoleKB]);
        }
        ++_indexRoleKB;
    }
    public void ChangeMapUIToNormal(int indexPlayer)
    {
        PlayerMap map = _playerMap[indexPlayer];
        Player rewiredPlayer = ReInput.players.GetPlayer(map.rewiredPlayerId);
        if (map.type == ControllerType.Keyboard) {
            rewiredPlayer.controllers.maps.SetMapsEnabled(false, ControllerType.Keyboard, RewiredConsts.Category.UI, _rolesKB[indexPlayer]);
            rewiredPlayer.controllers.maps.SetMapsEnabled(true, ControllerType.Keyboard, RewiredConsts.Category.DEFAULT, _rolesKB[indexPlayer]);
        } else if (map.type == ControllerType.Joystick)
        {
            rewiredPlayer.controllers.maps.SetMapsEnabled(false, RewiredConsts.Category.UI);
            rewiredPlayer.controllers.maps.SetMapsEnabled(true, "Default", "Default");
        }
    }
    private int GetNextGamePlayerId() {
        int playerId;
        if (_characterSelectionInGame)
        {
            playerId = gamePlayerIdCounter;
        } else
        {
            playerId = (int)_playerRoles[gamePlayerIdCounter];
        }
        gamePlayerIdCounter++;
        return playerId;
    }

}