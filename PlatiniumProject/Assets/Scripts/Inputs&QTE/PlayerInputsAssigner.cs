using UnityEngine;
using System.Collections.Generic;
using Rewired;
using System;
using Unity.VisualScripting;

public class PlayerInputsAssigner : MonoBehaviour {

    const int MAXPLAYERS = 3;

    readonly int[] _rolesKB = new int[MAXPLAYERS] { RewiredConsts.Layout.Keyboard.PLAYER1, RewiredConsts.Layout.Keyboard.PLAYER2, RewiredConsts.Layout.Keyboard.PLAYER3 };
    [SerializeField] PlayerRole[] _playerRoles = new PlayerRole[MAXPLAYERS];
    [SerializeField] bool _characterSelectionInGame = false;
    public event Action OnPlayerJoined;
    private static PlayerInputsAssigner _instance;
    int _indexRoleKB = 0;
    CSVLoader _csvLoader;

    #region GetPlayer
    public static Player GetRewiredPlayerByRole(PlayerRole role) {
        if(!Rewired.ReInput.isReady) return null;
        if(_instance == null) {
            Debug.LogError("Not initialized.");
            return null;
        }
        for(int i = 0; i < _instance._playerMap.Count; i++) {
            
            if (_instance._playerMap[i].Role == role) {
                return ReInput.players.GetPlayer(_instance._playerMap[i].RewiredPlayerId); 
            }
        }
        return null;
    }
    public static Player GetRewiredPlayerById(int playerId)
    {
        if (!ReInput.isReady) return null;
        if (_instance == null)
        {
            Debug.LogError("Not initialized.");
            return null;
        }
        for (int i = 0; i < _instance._playerMap.Count; i++)
        {
            if (_instance._playerMap[i].GamePlayerId == playerId)
            {
                return ReInput.players.GetPlayer(_instance._playerMap[i].RewiredPlayerId);
            }
        }
        return null;
    }
    public static InputDevice GetDeviceByRole(PlayerRole role)
    {
        if (!Rewired.ReInput.isReady) return InputDevice.None;
        if (_instance == null)
        {
            Debug.LogError("Not initialized.");
            return InputDevice.None;
        }
        for (int i = 0; i < _instance._playerMap.Count; i++)
        {
            if (_instance._playerMap[i].Role == role)
            {
                return _instance._playerMap[i].Device;
            }
        }
        return InputDevice.None;
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
    private void Start()
    {
        _csvLoader = Globals.DataControllerType;
    }
    void Update() {

        if (_playerMap.Count < MAXPLAYERS)
        {
            // Watch for JoinGame action in each Player
            for (int i = 0; i < ReInput.players.playerCount; i++)
            {
                if (ReInput.players.GetPlayer(i).GetButtonDown("JoinGame"))
                {
                    AssignNextPlayer(i, ReInput.players.GetPlayer(i).controllers.GetLastActiveController());
                    
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
        _playerMap[indexPlayer].Role = role;
    }
    void AssignNextPlayer(int rewiredPlayerId,Controller controller) {
        if(_playerMap.Count >= MAXPLAYERS) {
            Debug.LogError("Max player limit already reached!");
            return;
        }
        int gamePlayerId = GetNextGamePlayerId();
        InputDevice device = _csvLoader.GetInputDeviceFromGUID(controller.hardwareTypeGuid.ToString());
        // Add the Rewired Player as the next open game player slot
        PlayerRole role = PlayerRole.None;
        if (!_characterSelectionInGame)
        {
            role = (PlayerRole)gamePlayerId;
        }
        _playerMap.Add(new PlayerMap(rewiredPlayerId, gamePlayerId, controller.type, role, device,controller.id));
        Debug.Log("Added Rewired Player id " + rewiredPlayerId + " to game player " + gamePlayerId);
    }

    public void RemovePlayer(int controllerId)
    {
        //_playerMap
    }
    #region ChangeMap
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
        Player rewiredPlayer = ReInput.players.GetPlayer(map.RewiredPlayerId);
        if (map.Type == ControllerType.Keyboard) {
            Debug.Log($"KB {indexPlayer} {_rolesKB[indexPlayer]}");
            rewiredPlayer.controllers.maps.SetMapsEnabled(false, ControllerType.Keyboard, RewiredConsts.Category.UI, _rolesKB[indexPlayer]);
            rewiredPlayer.controllers.maps.SetMapsEnabled(true, ControllerType.Keyboard, RewiredConsts.Category.DEFAULT, _rolesKB[indexPlayer]);
        } else if (map.Type == ControllerType.Joystick)
        {
            Debug.Log($"JS {indexPlayer}");
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
    #endregion
}