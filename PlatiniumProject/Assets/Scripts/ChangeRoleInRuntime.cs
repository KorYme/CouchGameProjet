using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeRoleInRuntime : MonoBehaviour
{
    [SerializeField] int _indexPlayer = 0;
    PlayerMap _playerMap;
    int _indexMap;
    Player _player;
    PlayerInputsAssigner _playersAssigner;
    IEnumerator Start()
    {
        _playersAssigner = Globals.PlayerInputsAssigner;
        yield return new WaitUntil(() => PlayerInputsAssigner.GetRewiredPlayerById(_indexPlayer) != null);
        _indexMap = _playersAssigner.PlayersMap.ToList().FindIndex(playerMap => playerMap.GamePlayerId == _indexPlayer);
        _playerMap = _playersAssigner.PlayersMap[_indexMap];
        _player = PlayerInputsAssigner.GetRewiredPlayerById(_indexPlayer);
    }

    void Update()
    {
        if (_player != null && _player.GetButtonDown(RewiredConsts.Action.PAUSE))
        {
            Debug.Log($"CHANGE ROLE");
            if (_playerMap.Role != PlayerRole.None)
            {
                int indexLastRole = (int)_playerMap.Role;
                int indexNewRole = (indexLastRole + 1) % 3;
                _playersAssigner.PlayersMap[_indexMap].Role = (PlayerRole) indexNewRole;
                Players.ExchangePlayers(indexLastRole, indexNewRole);
                Debug.Log($"CHANGE ROLE {(PlayerRole)indexLastRole} to {(PlayerRole)indexNewRole}");
            }
        }
    }
}
