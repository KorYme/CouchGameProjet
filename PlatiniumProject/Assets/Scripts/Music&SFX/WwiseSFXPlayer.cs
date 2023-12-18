using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WwiseSFXPlayer : MonoBehaviour
{
    [SerializeField] List<AK.Wwise.Event> _sfxEvent;

    public void PlaySFX(string sfxName) => _sfxEvent.FirstOrDefault(x => x.Name == sfxName)?.Post(gameObject);
    public void PlaySFX(string sfxName, Action exitCallback) => _sfxEvent.FirstOrDefault(x => x.Name == sfxName)?.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncExit, (x, y, z) => exitCallback?.Invoke());
    public void PlaySFXWithIndex(int sfxIndex)
    {
        if (_sfxEvent.Count <= sfxIndex)
        {
            Debug.LogError("The index is not good");
            return;
        }
        _sfxEvent[sfxIndex]?.Post(gameObject);
    }
    public void PlayFirstSFX() => _sfxEvent.First()?.Post(gameObject);
    public void PlayFirstSFX(Action exitCallback) => _sfxEvent.First()?.Post(gameObject, (uint)AkCallbackType.AK_EndOfEvent, (x, y, z) => exitCallback?.Invoke());
}
