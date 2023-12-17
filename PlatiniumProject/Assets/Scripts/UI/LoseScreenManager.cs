using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoseScreenManager : MonoBehaviour
{
    [SerializeField] UnityEvent _onInputReceived;

    IEnumerator InputHandlerScene()
    {
        yield return new WaitUntil(() =>
        {
            for (int i = 0; i < 3; ++i)
            {
                if (PlayerInputsAssigner.GetRewiredPlayerById(i)?.GetAnyButtonDown() ?? false)
                    return true;
            }
            return false;
        });
        _onInputReceived?.Invoke();
    }
}
