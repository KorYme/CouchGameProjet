using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WwiseSFXPlayer : MonoBehaviour
{
    [SerializeField] List<AK.Wwise.Event> _sfxEvent;

    public void PlaySFX(string sfxName) => /*_sfxEvent.FirstOrDefault(x => x.Name == sfxName)?.Post(gameObject)*/Debug.Log(sfxName);
    public void PlayFirstSFX() => /*_sfxEvent.First()?.Post(gameObject)*/Debug.Log("FirstEvent");
}
