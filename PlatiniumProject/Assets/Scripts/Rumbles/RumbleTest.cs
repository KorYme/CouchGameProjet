using Rewired;
using RewiredConsts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumbleTest : MonoBehaviour
{
    [SerializeField] RumbleValue _singlePressRumble;
    [SerializeField] RumbleValue _holdRumble;

    public RumbleValue SinglePressRumble => _singlePressRumble;
    public RumbleValue HoldRumble => _holdRumble;

    Coroutine _singlePressCoroutine;
    Coroutine _holdCoroutine;

    private void Start()
    {
        Players.OnPlayerConnect += playerId =>
        {
            Players.PlayersController[playerId].Action1.OnInputStart += () =>
            {
                if (_singlePressCoroutine != null) StopCoroutine(_singlePressCoroutine);
                _singlePressCoroutine = StartCoroutine(SinglePress(playerId));
            };
            Players.PlayersController[playerId].RT.OnInputStart += () =>
            {
                _holdCoroutine = StartCoroutine(Hold(playerId));
            };
            Players.PlayersController[playerId].RT.OnInputEnd += () =>
            {
                Players.PlayersController[playerId].newPlayer.SetVibration(0, 0f);
                StopCoroutine(_holdCoroutine);
            };
        };
    }

    IEnumerator SinglePress(int playerId)
    {
        float timer = 0;
        while (timer <= 1)
        {
            timer += Time.deltaTime / _singlePressRumble.time;
            Players.PlayersController[playerId].newPlayer.SetVibration(0, _singlePressRumble.rumbleCurve.Evaluate(timer));
            yield return null;
        }
        Players.PlayersController[playerId].newPlayer.SetVibration(0, 0f);
        _singlePressCoroutine = null;
    }

    IEnumerator Hold(int playerId)
    {
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime / _holdRumble.time;
            Players.PlayersController[playerId].newPlayer.SetVibration(0, _holdRumble.rumbleCurve.Evaluate(timer));
            yield return null;
        }
    }
}

[Serializable]
public struct RumbleValue
{
    public string rumbleName;
    [Range(.1f, 10f)] public float time;
    public AnimationCurve rumbleCurve;
}