using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
    List<TriggerInputCheck> _allTriggerChecks = new List<TriggerInputCheck>();

    private void Awake()
    {
        Players.OnPlayerConnect += GenerateTriggerChecks;
    }

    private void OnDestroy()
    {
        Players.OnPlayerConnect -= GenerateTriggerChecks;
    }

    private void GenerateTriggerChecks(int playerRole)
    {
        new TriggerInputCheck(Players.PlayersController[playerRole].RT, 1);
    }
}
