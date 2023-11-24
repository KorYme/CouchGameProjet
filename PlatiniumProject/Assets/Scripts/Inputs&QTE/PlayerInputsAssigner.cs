
using UnityEngine;
using System.Collections.Generic;
using Rewired;
using System;

public enum ControlsType
{
    Keyboard,
    Joystick
}
public enum PlayerRole
{
    Barman = 0,
    DJ = 1,
    Bouncer = 2,
    None
}

public class PlayerInputsAssigner : MonoBehaviour {

    public class PlayerMap
    {
        public int rewiredPlayerId;
        public int gamePlayerId;
        public PlayerRole role;
        public ControlsType type;
        public PlayerMap(int rewiredPlayerId, int gamePlayerId, ControlsType type, PlayerRole role)
        {
            this.rewiredPlayerId = rewiredPlayerId;
            this.gamePlayerId = gamePlayerId;
            this.type = type;
            this.role = role;
        }
    }
    const int MAXPLAYERS = 3;

    [SerializeField] string[] _rolesKB = new string[MAXPLAYERS] { "Player1", "Player2", "Player3" };
    [SerializeField] PlayerRole[] _playerRoles = new PlayerRole[MAXPLAYERS];
    [SerializeField] bool _characterSelectionInGame = false;
    public event Action OnPlayerJoined;
    private static PlayerInputsAssigner _instance;
    int indexRoleKB = 0;

    public static Rewired.Player GetRewiredPlayerByRole(int role) {
        if(!Rewired.ReInput.isReady) return null;
        if(_instance == null) {
            Debug.LogError("Not initialized.");
            return null;
        }
        for(int i = 0; i < _instance.playerMap.Count; i++) {
            if(((int)_instance.playerMap[i].role) == role) return ReInput.players.GetPlayer(_instance.playerMap[i].rewiredPlayerId);
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
        for (int i = 0; i < _instance.playerMap.Count; i++)
        {
            if (_instance.playerMap[i].gamePlayerId == playerId) return ReInput.players.GetPlayer(_instance.playerMap[i].rewiredPlayerId);
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
        for (int i = 0; i < _instance.playerMap.Count; i++)
        {
            if (_instance.playerMap[i].gamePlayerId == gamePlayerId)
            {
                return _instance.playerMap[i].role;
            }
        }
        return PlayerRole.None;
    }

    private List<PlayerMap> playerMap; // Maps Rewired Player ids to game player ids
    private int gamePlayerIdCounter = 0;

    void Awake() {
        playerMap = new List<PlayerMap>();
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    void Update() {

        if (playerMap.Count < MAXPLAYERS)
        {
            // Watch for JoinGame action in each Player
            for (int i = 0; i < ReInput.players.playerCount; i++)
            {
                if (ReInput.players.GetPlayer(i).GetButtonDown("JoinGame"))
                {
                    AssignNextPlayer(i);
                    switch (ReInput.players.GetPlayer(i).controllers.GetLastActiveController().type)
                    {
                        case ControllerType.Joystick:
                            ChangeMapJoystick(i);
                            break;
                        case ControllerType.Keyboard:
                            ChangeMapKeyboard(i);
                            return;
                    }
                    OnPlayerJoined?.Invoke();
                }
            }
        }
    }

    void AssignNextPlayer(int rewiredPlayerId) {
        if(playerMap.Count >= MAXPLAYERS) {
            Debug.LogError("Max player limit already reached!");
            return;
        }
        int gamePlayerId = GetNextGamePlayerId();

        // Add the Rewired Player as the next open game player slot
        if (_characterSelectionInGame)
        {
            playerMap.Add(new PlayerMap(rewiredPlayerId, gamePlayerId, ControlsType.Joystick, PlayerRole.None));
        } else
        {
            playerMap.Add(new PlayerMap(rewiredPlayerId, gamePlayerId,ControlsType.Joystick,(PlayerRole) gamePlayerId));
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
    void ChangeMapKeyboard(int rewiredPlayerId)
    {
        Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

        // Disable the Assignment map category in Player so no more JoinGame Actions return
        rewiredPlayer.controllers.maps.SetMapsEnabled(false, RewiredConsts.Category.ASSIGNMENT);
        // Enable UI control for this Player now that he has joined
        if (_characterSelectionInGame)
        {
            rewiredPlayer.controllers.maps.SetMapsEnabled(true, RewiredConsts.Category.UI);
        }
        else
        {
            rewiredPlayer.controllers.maps.SetMapsEnabled(true, RewiredConsts.Category.DEFAULT, _rolesKB[indexRoleKB]);
        }
        ++indexRoleKB;
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