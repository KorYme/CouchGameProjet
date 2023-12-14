using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RumbleController : MonoBehaviour
{
    [SerializeField] PlayerRole _role;
    [SerializeField] List<RumbleValues> _allRumbles;
    Coroutine _singlePressCoroutine, _holdCoroutine;

    public void PlayRumbles(string rumbleName)
    {
        RumbleValues rumble = _allRumbles.FirstOrDefault(x => x.rumbleName == rumbleName);
        if (rumble == default(RumbleValues)) return;
        if (rumble.isHolding)
        {
            if (_singlePressCoroutine != null)
            {
                StopCoroutine(_singlePressCoroutine); 
                _singlePressCoroutine = null;
            }
            if (_holdCoroutine != null) StopCoroutine(_holdCoroutine);
            _holdCoroutine = StartCoroutine(Hold((int)_role, rumble));
        }
        else
        {
            if (_holdCoroutine != null) return;
            if (_singlePressCoroutine != null) StopCoroutine(_singlePressCoroutine);
            _singlePressCoroutine = StartCoroutine(_role == PlayerRole.None ? SinglePress(rumble) : SinglePress((int)_role, rumble));
        }
    }

    public void StopAllRumbles()
    {
        StopAllCoroutines();
        _singlePressCoroutine = null;
        _holdCoroutine = null;
        if (_role != PlayerRole.None)
        {
            Players.PlayersController[(int)_role].newPlayer.SetVibration(0, 0f);
        }
        else
        {
            for (int i = 0; i < 3; ++i)
            {
                Players.PlayersController[i].newPlayer.SetVibration(0, 0f);
            }
        }
    }

    IEnumerator SinglePress(int role, RumbleValues rumbleValues)
    {
        float timer = 0;
        while (timer <= 1)
        {
            timer += Time.deltaTime / rumbleValues.time;
            Players.PlayersController[role].newPlayer.SetVibration(0, rumbleValues.rumbleCurve.Evaluate(timer));
            yield return null;
        }
        Players.PlayersController[role].newPlayer.SetVibration(0, 0f);
        _singlePressCoroutine = null;
    }
    
    IEnumerator SinglePress(RumbleValues rumbleValues)
    {
        float timer = 0;
        while (timer <= 1)
        {
            timer += Time.deltaTime / rumbleValues.time;
            for (int i = 0; i < 3; i++)
            {
                Players.PlayersController[i]?.newPlayer.SetVibration(0, rumbleValues.rumbleCurve.Evaluate(timer));
            }
            yield return null;
        }
        for (int i = 0; i < 3; i++)
        {
            Players.PlayersController[i]?.newPlayer.SetVibration(0, 0f);
        }
        _singlePressCoroutine = null;
    }

    IEnumerator Hold(int role, RumbleValues rumbleValues)
    {
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime / rumbleValues.time;
            Players.PlayersController[role].newPlayer.SetVibration(0, rumbleValues.rumbleCurve.Evaluate(timer));
            yield return null;
        }
    }
}