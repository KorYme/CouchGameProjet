using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseSFXPlayer : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event _sfxEvent;

    public void PlaySFX() => _sfxEvent?.Post(gameObject);
}
