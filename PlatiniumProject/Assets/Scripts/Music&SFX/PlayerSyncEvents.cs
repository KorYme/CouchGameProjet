using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSyncEvents", menuName = "ScriptableObject/PlayerSyncEvents")]
public class PlayerSyncEvents : ScriptableObject
{
    [SerializeField] SyncEvents _bouncerEvents, _barmanEvents, _djEvents;

    public SyncEvents this[PlayerRole index]
    {
        get
        {
            switch (index)
            {
                case PlayerRole.Bouncer: return _bouncerEvents;
                case PlayerRole.Barman: return _barmanEvents;
                case PlayerRole.DJ: return _djEvents;
                case PlayerRole.None:
                default:
                    throw new ArgumentException("No Character linked to this argument");
            }
        }
    }
}

[Serializable]
public struct SyncEvents
{
    public AK.Wwise.Event isNotSyncEvent;
    public AK.Wwise.Event isSyncEvent;
}