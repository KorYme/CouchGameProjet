using UnityEngine;
using System.Collections.Generic;
using Rewired;
using System;
using Unity.VisualScripting;
using System.Linq;

public class PlayerInputsAssigner : MonoBehaviour {

    const int MAXPLAYERS = 3;

    readonly int[] _rolesKB = new int[MAXPLAYERS] { RewiredConsts.Layout.Keyboard.PLAYER1, RewiredConsts.Layout.Keyboard.PLAYER2, RewiredConsts.Layout.Keyboard.PLAYER3 };
    [SerializeField] PlayerRole[] _playerRoles = new PlayerRole[MAXPLAYERS];
    [SerializeField] bool _characterSelectionInGame = false;
    public event Action OnPlayerJoined;
    public event Action<int> OnPlayerLeave;
    private static PlayerInputsAssigner _instance;
    int _indexRoleKB = 0;
    [SerializeField] CSVLoader _csvLoader;

    #region GetPlayer
    public static Player GetRewiredPlayerByRole(PlayerRole role) {
        if(!Rewired.ReInput.isReady) return null;
        if(_instance == null) {
            Debug.LogError("Not initialized.");
            return null;
        }
        for(int i = 0; i < _instance._playerMap.Count; i++) {
            
            if (_instance._playerMap[i].Role == role && _instance._playerMap[i].RewiredPlayerId != -1) {
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
            if (_instance._playerMap[i].GamePlayerId == playerId && _instance._playerMap[i].RewiredPlayerId != -1)
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
            if (_instance._playerMap[i].Role == role && _instance._playerMap[i].RewiredPlayerId != -1) //Device is not valid when there is no controller
            {
                return _instance._playerMap[i].Device;
            }
        }
        return InputDevice.None;
    }
    #endregion

    private List<PlayerMap> _playerMap; // Maps Rewired Player ids to game player ids
    public IList<PlayerMap> PlayersMap => _playerMap.AsReadOnlyList();
    public IList<PlayerMap> PlayersConnectedMap => _playerMap.ToList().Where(playerMap => playerMap.RewiredPlayerId != -1).AsReadOnlyList();
    public int CurrentNbOfPlayersConnected { get; private set; } = 0;
    int GetNextPlayerMapAvailable()
    {
        return _playerMap.FindIndex(map => map.RewiredPlayerId == -1);
    }

    void Awake() {
        _playerMap = new List<PlayerMap>();
        CreatePlayers();
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Globals.PlayerInputsAssigner ??= this;
        _instance = this;
    }
    void Update() {

        if (CurrentNbOfPlayersConnected < MAXPLAYERS)
        {
            // Watch for JoinGame action in each Player
            for (int i = 0; i < ReInput.players.playerCount; i++)
            {
                if (ReInput.players.GetPlayer(i).GetButtonDown("JoinGame"))
                {
                    switch (ReInput.players.GetPlayer(i).controllers.GetLastActiveController().type)
                    {
                        /*case ControllerType.Joystick:
                            //AssignNextPlayer(i, ReInput.players.GetPlayer(i).controllers.GetLastActiveController());
                            ChangeMapJoystick(i);
                            OnPlayerJoined?.Invoke();
                            break;*/
                        case ControllerType.Keyboard:
                            AssignNextPlayer(i, ReInput.players.GetPlayer(i).controllers.GetLastActiveController());
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
    public void AssignNextPlayer(int rewiredPlayerId, Controller controller, bool callEvent = true)
    {
        AddController(rewiredPlayerId, controller);
        if (callEvent)
            OnPlayerJoined?.Invoke();
    }
    // Create each playerMap (players' controllers are not set)
    public void CreatePlayers()
    {
        for (int i = 0;  i < MAXPLAYERS; i++)
        {
            PlayerRole role = PlayerRole.None;
            if (!_characterSelectionInGame)
            {
                role = _playerRoles[i];
            }
            _playerMap.Add(new PlayerMap(i,role));
        }
    }
    public void AddController(int rewiredIndex, Controller controller)
    {
        int indexPlayer = GetNextPlayerMapAvailable();
        if (indexPlayer != -1) // There is a controller available
        {
            InputDevice device = _csvLoader.GetInputDeviceFromGUID(controller.hardwareTypeGuid.ToString());
            ControllerType type = controller.type;
            _playerMap[indexPlayer].AddController(rewiredIndex, type, device,controller.id);
            Debug.Log("Added Rewired Player id " + rewiredIndex + " to game player " + indexPlayer);
            CurrentNbOfPlayersConnected++;
            Debug.Log($"Nb Players connected {CurrentNbOfPlayersConnected}");
        }
    }


    //Return Rewired.PlayerId
    public int RemovePlayer(int controllerId)
    {
        int indexMap = _playerMap.FindIndex(map => map.IndexDevice == controllerId);
        if (indexMap == -1)
        {
            Debug.LogError("Can't remove controller not in player maps");
            return -1;
        }
        int playerId = _playerMap[indexMap].RewiredPlayerId;
        _playerMap[indexMap].RemoveController();
        Debug.Log($"Controller {controllerId} removed {playerId}");
        CurrentNbOfPlayersConnected--;
        Debug.Log($"Nb Players connected {CurrentNbOfPlayersConnected}");
        OnPlayerLeave?.Invoke(playerId);
        return playerId;
    }
    #region ChangeMap  
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
        ++_indexRoleKB; //KEEP
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
    #endregion
}