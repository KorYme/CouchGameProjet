
using UnityEngine;
using System.Collections.Generic;
using Rewired;

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
public class PlayerInputsAssigner : MonoBehaviour {


    const int MAXPLAYERS = 3;

    [SerializeField] string[] _rolesKB = new string[MAXPLAYERS] { "Player1", "Player2", "Player3" };
    [SerializeField] PlayerRole[] _playerRoles = new PlayerRole[MAXPLAYERS];

    private static PlayerInputsAssigner instance;
    int indexRoleKB = 0;

    public static Rewired.Player GetRewiredPlayer(int gamePlayerId) {
        if(!Rewired.ReInput.isReady) return null;
        if(instance == null) {
            Debug.LogError("Not initialized.");
            return null;
        }
        for(int i = 0; i < instance.playerMap.Count; i++) {
            if((int)instance.playerMap[i].role == gamePlayerId) return ReInput.players.GetPlayer(instance.playerMap[gamePlayerId].rewiredPlayerId);
        }
        return null;
    }

    public static PlayerRole GetRolePlayer(int gamePlayerId)
    {
        if (!Rewired.ReInput.isReady) return PlayerRole.None;
        if (instance == null)
        {
            Debug.LogError("Not initialized.");
            return PlayerRole.None;
        }
        for (int i = 0; i < instance.playerMap.Count; i++)
        {
            if (instance.playerMap[i].gamePlayerId == gamePlayerId)
            {
                return instance.playerMap[i].role;
            }
        }
        return PlayerRole.None;
    }
    // Instance

    private List<PlayerMap> playerMap; // Maps Rewired Player ids to game player ids
    private int gamePlayerIdCounter = 0;

    void Awake() {
        playerMap = new List<PlayerMap>();
        instance = this;
    }
    void Update() {

        // Watch for JoinGame action in each Player
        for(int i = 0; i < ReInput.players.playerCount; i++) {
            if (ReInput.players.GetPlayer(i).GetButtonDown("JoinGame")) {
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
        playerMap.Add(new PlayerMap(rewiredPlayerId, gamePlayerId,ControlsType.Joystick,_playerRoles[gamePlayerId]));
        Debug.Log("Added Rewired Player id " + rewiredPlayerId + " to game player " + gamePlayerId + " "+ _playerRoles[gamePlayerId]);
    }


    void ChangeMapJoystick(int rewiredPlayerId)
    {
        Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

        // Disable the Assignment map category in Player so no more JoinGame Actions return
        rewiredPlayer.controllers.maps.SetMapsEnabled(false, RewiredConsts.Category.ASSIGNMENT);

        // Enable UI control for this Player now that he has joined
        rewiredPlayer.controllers.maps.SetMapsEnabled(true, "Default", "Default");
    }
    void ChangeMapKeyboard(int rewiredPlayerId)
    {
        Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

        // Disable the Assignment map category in Player so no more JoinGame Actions return
        rewiredPlayer.controllers.maps.SetMapsEnabled(false, RewiredConsts.Category.ASSIGNMENT);
        // Enable UI control for this Player now that he has joined
        rewiredPlayer.controllers.maps.SetMapsEnabled(true, RewiredConsts.Category.DEFAULT, _rolesKB[indexRoleKB]);
        ++indexRoleKB;
    }
    private int GetNextGamePlayerId() {
        return gamePlayerIdCounter++;
    }
}